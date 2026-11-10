using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace DocHub
{
    public sealed record MergeProgress(int Percent, string Status);

    public sealed record MergeResult(
        string OutputPath, int PageCount, TimeSpan Elapsed,
        int WordCount, int PdfCount, int ImageCount, int SkippedWordCount);

    /// <summary>Word 转 PDF 可用的转换引擎。</summary>
    public enum WordConverter
    {
        None,
        MicrosoftWord,
        LibreOffice
    }

    public sealed class MergeException : Exception
    {
        public MergeException(string message, Exception? inner = null) : base(message, inner) { }
    }

    /// <summary>
    /// 合并引擎：Word 先由本机 Word 无损导出为 PDF，图片按原始尺寸排成单页 PDF，
    /// 再按列表顺序逐页合并。不重新排版、不转图片、不压缩页面内容。
    /// </summary>
    public static class MergeEngine
    {
        public static readonly string[] WordExtensions =
            { ".doc", ".docx", ".docm", ".dot", ".dotx", ".dotm", ".rtf", ".odt" };

        public static readonly string[] ImageExtensions =
        {
            ".jpg", ".jpeg", ".jpe", ".jfif", ".png", ".bmp", ".dib",
            ".tif", ".tiff", ".gif", ".webp", ".heic", ".heif", ".avif"
        };

        private static readonly HashSet<string> WordSet =
            new(WordExtensions, StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> ImageSet =
            new(ImageExtensions, StringComparer.OrdinalIgnoreCase);

        public static bool IsWordFile(string path)
        {
            return WordSet.Contains(Path.GetExtension(path));
        }

        public static bool IsPdfFile(string path)
        {
            return string.Equals(Path.GetExtension(path), ".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsImageFile(string path)
        {
            return ImageSet.Contains(Path.GetExtension(path));
        }

        public static bool IsSupportedFile(string path)
        {
            return IsPdfFile(path) || IsWordFile(path) || IsImageFile(path);
        }

        /// <summary>PDF 是否设置了打开密码。</summary>
        public static bool PdfNeedsPassword(string path)
        {
            try
            {
                using var probe = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                return false;
            }
            catch (Exception ex)
            {
                // PDFsharp 对加密文档抛出 PdfReaderException，消息里含 password
                return ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>用给定密码尝试打开 PDF，用于校验用户输入是否正确。</summary>
        public static bool VerifyPdfPassword(string path, string password)
        {
            try
            {
                using var probe = PdfReader.Open(path, password, PdfDocumentOpenMode.Import);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 加密的 OOXML（docx/docm/dotx/dotm）保存为 OLE 复合文档而不是 ZIP，
        /// 文件头不是 "PK" 即说明该文档设置了打开密码。
        /// </summary>
        public static bool WordNeedsPassword(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext != ".docx" && ext != ".docm" && ext != ".dotx" && ext != ".dotm") return false;

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] head = new byte[4];
                if (stream.Read(head, 0, head.Length) < head.Length) return false;
                return !(head[0] == 0x50 && head[1] == 0x4B);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsWordAvailable => DetectedConverter != WordConverter.None;

        public static WordConverter DetectedConverter
        {
            get
            {
                if (HasWordCom()) return WordConverter.MicrosoftWord;
                if (FindLibreOffice() != null) return WordConverter.LibreOffice;
                return WordConverter.None;
            }
        }

        private static bool HasWordCom()
        {
            try { return Type.GetTypeFromProgID("Word.Application") != null; }
            catch { return false; }
        }

        private static string? _sofficePath;
        private static bool _sofficeProbed;

        /// <summary>查找本机 LibreOffice 的 soffice.exe；没有安装时返回 null。</summary>
        public static string? FindLibreOffice()
        {
            if (_sofficeProbed) return _sofficePath;
            _sofficeProbed = true;

            try
            {
                foreach (string? root in new[]
                {
                    Environment.GetEnvironmentVariable("ProgramFiles"),
                    Environment.GetEnvironmentVariable("ProgramW6432"),
                    Environment.GetEnvironmentVariable("ProgramFiles(x86)")
                })
                {
                    if (string.IsNullOrEmpty(root)) continue;
                    string candidate = Path.Combine(root, "LibreOffice", "program", "soffice.exe");
                    if (File.Exists(candidate))
                    {
                        _sofficePath = candidate;
                        return _sofficePath;
                    }
                }

                foreach (string keyName in new[]
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\soffice.exe",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\soffice.exe",
                    @"SOFTWARE\LibreOffice\UNO\InstallPath",
                    @"SOFTWARE\WOW6432Node\LibreOffice\UNO\InstallPath"
                })
                {
                    using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyName);
                    string? value = key?.GetValue(null) as string;
                    if (string.IsNullOrEmpty(value)) continue;

                    string candidate = value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                        ? value
                        : Path.Combine(value, "program", "soffice.exe");
                    if (File.Exists(candidate))
                    {
                        _sofficePath = candidate;
                        return _sofficePath;
                    }
                }
            }
            catch
            {
                // 注册表或目录不可访问时按未安装处理
            }

            return null;
        }

        public static MergeResult Run(
            IReadOnlyList<string> inputFiles,
            string outputPath,
            IProgress<MergeProgress>? progress,
            bool allowSkipWord = false,
            WatermarkSpec? watermark = null,
            IReadOnlyDictionary<string, string>? passwords = null)
        {
            if (inputFiles.Count == 0)
                throw new MergeException(Loc.T(Txt.MergeNoDocs));

            foreach (var file in inputFiles)
            {
                if (!File.Exists(file))
                    throw new MergeException(Loc.T(Txt.MergeFileMissing, file));
            }

            WordConverter converter = DetectedConverter;
            int wordCount = inputFiles.Count(IsWordFile);
            int pdfCount = inputFiles.Count(IsPdfFile);
            int imageCount = inputFiles.Count(IsImageFile);
            int skippedWordCount = 0;

            if (wordCount > 0 && converter == WordConverter.None)
            {
                if (!allowSkipWord)
                    throw new MergeException(Loc.T(Txt.MergeWordMissing));

                // 用户选择跳过无法转换的 Word 文件，其余 PDF / 图片继续合并
                skippedWordCount = wordCount;
                wordCount = 0;
            }

            var stopwatch = Stopwatch.StartNew();

            // 临时目录按需创建：纯 PDF 合并（且不加水印）不需要临时文件，
            // 这样即使临时目录被安全软件拦写，纯 PDF 合并仍然可用。
            string? tempDir = null;
            if (wordCount > 0 || imageCount > 0 || watermark != null)
                tempDir = CreateWorkDirectory();

            object? word = null;
            object? documents = null;

            try
            {
                int total = inputFiles.Count;
                string?[] pdfPaths = new string?[total];

                // PDF 文件原样使用，不做任何复制或重新编码
                for (int i = 0; i < total; i++)
                {
                    if (IsPdfFile(inputFiles[i]))
                        pdfPaths[i] = inputFiles[i];
                }

                int convertible = imageCount + wordCount;
                int processed = 0;

                // 图片转 PDF 相互独立，按 CPU 核心并行转换
                var imageIndexes = new List<int>(imageCount);
                for (int i = 0; i < total; i++)
                    if (IsImageFile(inputFiles[i])) imageIndexes.Add(i);

                if (imageIndexes.Count > 0)
                {
                    int maxDegree = Math.Max(1, Math.Min(Environment.ProcessorCount, 4));
                    Parallel.ForEach(imageIndexes,
                        new ParallelOptions { MaxDegreeOfParallelism = maxDegree },
                        index =>
                        {
                            string source = inputFiles[index];
                            string imagePdf = Path.Combine(tempDir!, $"image_{index:D4}.pdf");
                            ExportImageToPdf(source, imagePdf);
                            pdfPaths[index] = imagePdf;

                            int doneCount = Interlocked.Increment(ref processed);
                            int percent = (int)(doneCount * 76.0 / Math.Max(1, convertible));
                            Report(progress, percent,
                                Loc.T(Txt.MergeProcessing, doneCount, total, Path.GetFileName(source)));
                        });
                }

                // Word 转换必须按顺序执行：COM 单线程，LibreOffice 进程也逐个转换更稳定
                for (int i = 0; i < total; i++)
                {
                    if (!IsWordFile(inputFiles[i]) || converter == WordConverter.None) continue;

                    string source = inputFiles[i];
                    if (word == null && converter == WordConverter.MicrosoftWord)
                        Report(progress, (int)(processed * 76.0 / Math.Max(1, convertible)),
                            Loc.T(Txt.MergeStartingWord));

                    string tempPdf = Path.Combine(tempDir!, $"word_{i:D4}.pdf");
                    if (converter == WordConverter.MicrosoftWord)
                    {
                        EnsureWord(ref word, ref documents);
                        ExportWordToPdf(word!, documents!, source, tempPdf,
                            passwords != null && passwords.TryGetValue(source, out string? wordPassword)
                                ? wordPassword
                                : null);
                    }
                    else
                    {
                        ExportWordWithLibreOffice(source, tempPdf);
                    }

                    pdfPaths[i] = tempPdf;
                    int doneCount = Interlocked.Increment(ref processed);
                    int percent = (int)(doneCount * 76.0 / Math.Max(1, convertible));
                    Report(progress, percent,
                        Loc.T(Txt.MergeProcessing, doneCount, total, Path.GetFileName(source)));
                }

                QuitWord(ref word, ref documents);
                Report(progress, 80, Loc.T(Txt.MergeMergingPages));

                // 阶段二：按列表顺序逐页合并，直接写入目标文件（不再多一次整文件复制）
                var entries = new List<(string Path, string Name)>(total);
                for (int i = 0; i < total; i++)
                {
                    if (pdfPaths[i] != null)
                        entries.Add((pdfPaths[i]!, Path.GetFileName(inputFiles[i])));
                }

                int pageCount = 0;
                var sources = new List<PdfDocument>(entries.Count);
                try
                {
                    using var output = new PdfDocument();

                    for (int i = 0; i < entries.Count; i++)
                    {
                        string pdfPath = entries[i].Path;
                        string displayName = entries[i].Name;
                        int percent = 80 + (int)(i * 18.0 / Math.Max(1, entries.Count));
                        Report(progress, percent, Loc.T(Txt.MergeMerging, i + 1, entries.Count, displayName));

                        PdfDocument input;
                        try
                        {
                            string? password = passwords != null && passwords.TryGetValue(pdfPath, out string? saved)
                                ? saved
                                : null;
                            input = string.IsNullOrEmpty(password)
                                ? PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import)
                                : PdfReader.Open(pdfPath, password, PdfDocumentOpenMode.Import);
                        }
                        catch (Exception ex)
                        {
                            throw new MergeException(Loc.T(Txt.MergePdfUnreadable, displayName), ex);
                        }

                        sources.Add(input);
                        foreach (var page in input.Pages)
                        {
                            output.AddPage(page);
                            pageCount++;
                        }
                    }

                    if (pageCount == 0)
                        throw new MergeException(Loc.T(Txt.MergeNoPages));

                    if (watermark != null)
                        WatermarkPainter.Apply(output, watermark, tempDir!);

                    string? outputFolder = Path.GetDirectoryName(Path.GetFullPath(outputPath));
                    if (!string.IsNullOrEmpty(outputFolder))
                        Directory.CreateDirectory(outputFolder);

                    output.Save(outputPath);
                }
                catch (MergeException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MergeException(Loc.T(Txt.MergePdfError), ex);
                }
                finally
                {
                    foreach (var source in sources) source.Dispose();
                }

                Report(progress, 100, Loc.T(Txt.MergeDone));
                stopwatch.Stop();
                return new MergeResult(outputPath, pageCount, stopwatch.Elapsed,
                    wordCount, pdfCount, imageCount, skippedWordCount);
            }
            catch (MergeException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new MergeException(Loc.T(Txt.MergeWriteError, outputPath, ex.Message), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new MergeException(Loc.T(Txt.MergeNoPermission, outputPath), ex);
            }
            catch (Exception ex)
            {
                throw new MergeException(Loc.T(Txt.MergeFailedGeneric, ex.Message), ex);
            }
            finally
            {
                QuitWord(ref word, ref documents);
                if (tempDir != null)
                    TryDeleteDirectory(tempDir);
            }
        }

        /// <summary>
        /// 创建本次合并的工作目录。直接建在系统临时目录下、不再套一层 DocHub 子目录：
        /// 旧版（以管理员身份运行时）创建的目录带高完整性级别，普通权限启动的程序
        /// 无法在其中新建子目录，会报「Access to the path ... is denied」。
        /// </summary>
        internal static string CreateWorkDirectory()
        {
            string name = "DocHub_" + Guid.NewGuid().ToString("N");
            string[] candidates =
            {
                Path.Combine(Path.GetTempPath(), name),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DocHub", "temp", Guid.NewGuid().ToString("N"))
            };

            foreach (string candidate in candidates)
            {
                try
                {
                    Directory.CreateDirectory(candidate);
                    return candidate;
                }
                catch
                {
                    // 换下一个位置再试
                }
            }

            throw new MergeException(Loc.T(Txt.MergeTempFail));
        }

        /// <summary>PDF 规范允许的单页最长边：200 英寸（14400 point）。超过会导致保存阶段失败。</summary>
        private const double MaxPagePoints = 14400.0;

        /// <summary>把图片按原始像素与 DPI 排成单页 PDF：横向图保持横向、纵向图保持纵向。</summary>
        private static void ExportImageToPdf(string source, string targetPdf)
        {
            string name = Path.GetFileName(source);
            XImage? image = null;
            string? normalized = null;

            try
            {
                // GDI+ 能读就用它（快、像素原样）；读不了（WebP / HEIC / AVIF 等）
                // 再交给 Windows 自带的 WIC 解码器转成 PNG，避免整批合并因一个格式失败。
                bool readable = TryReadImageInfo(source, out ImageInfo info);

                if (readable && CanEmbedDirectly(source, info.Orientation))
                {
                    try
                    {
                        image = XImage.FromFile(source);
                    }
                    catch
                    {
                        image = null;
                    }
                }

                if (image == null)
                {
                    normalized = Path.Combine(Path.GetDirectoryName(targetPdf) ?? Path.GetTempPath(),
                        Path.GetFileNameWithoutExtension(targetPdf) + ".png");

                    if (readable)
                    {
                        normalized = NormalizeImage(source, normalized);
                    }
                    else if (SystemImageDecoder.TryConvertToPng(source, normalized,
                                 out int wicWidth, out int wicHeight, out double wicDpiX, out double wicDpiY, out string error))
                    {
                        // WIC 解码时已按 EXIF 方向摆正，这里按普通方向处理
                        info = new ImageInfo(wicWidth, wicHeight, wicDpiX, wicDpiY, 1);
                    }
                    else
                    {
                        throw new MergeException(Loc.T(Txt.MergeImageConvertFail, name, error));
                    }

                    image = XImage.FromFile(normalized);
                }

                // 页面尺寸按图片自身的像素与 DPI 换算，避免解码库默认 96 DPI 改变原始大小
                double width = Math.Max(1.0, info.Width / info.DpiX * 72.0);
                double height = Math.Max(1.0, info.Height / info.DpiY * 72.0);

                // 超出 PDF 单页上限时等比缩小（超高 DPI 的扫描件会算出极小的页面，反之亦然）
                double overflow = Math.Max(width, height) / MaxPagePoints;
                if (overflow > 1.0)
                {
                    width /= overflow;
                    height /= overflow;
                }

                using var document = new PdfDocument();
                var page = document.AddPage();
                page.Width = XUnit.FromPoint(width);
                page.Height = XUnit.FromPoint(height);
                using (var graphics = XGraphics.FromPdfPage(page))
                    graphics.DrawImage(image, 0, 0, width, height);

                document.Save(targetPdf);

                if (!File.Exists(targetPdf) || new FileInfo(targetPdf).Length == 0)
                    throw new MergeException(Loc.T(Txt.MergeImageConvertFail, name, "empty"));
            }
            catch (MergeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MergeException(Loc.T(Txt.MergeImageConvertFail, name, ex.Message), ex);
            }
            finally
            {
                image?.Dispose();
                if (normalized != null)
                {
                    try { if (File.Exists(normalized)) File.Delete(normalized); }
                    catch { /* 临时文件由系统稍后清理 */ }
                }
            }
        }

        private sealed record ImageInfo(int Width, int Height, double DpiX, double DpiY, int Orientation);

        /// <summary>GDI+ 读取图片信息；格式不受支持时返回 false，由调用方改用系统解码器。</summary>
        private static bool TryReadImageInfo(string path, out ImageInfo info)
        {
            try
            {
                info = ReadImageInfo(path);
                return true;
            }
            catch
            {
                info = new ImageInfo(1, 1, 96, 96, 1);
                return false;
            }
        }

        private static ImageInfo ReadImageInfo(string path)
        {
            using var image = Image.FromFile(path);
            int orientation = ReadOrientation(image);

            int width = Math.Max(1, image.Width);
            int height = Math.Max(1, image.Height);
            double dpiX = image.HorizontalResolution > 0 ? image.HorizontalResolution : 96.0;
            double dpiY = image.VerticalResolution > 0 ? image.VerticalResolution : 96.0;

            // EXIF 5~8 表示旋转 90/270 度，页面的宽高与 DPI 需要交换
            if (orientation is >= 5 and <= 8)
            {
                (width, height) = (height, width);
                (dpiX, dpiY) = (dpiY, dpiX);
            }

            return new ImageInfo(width, height, dpiX, dpiY, orientation);
        }

        /// <summary>JPEG/PNG 且方向正常时直接嵌入，避免二次编码；其余格式先规整为白底 PNG。</summary>
        private static bool CanEmbedDirectly(string path, int orientation)
        {
            string extension = Path.GetExtension(path);
            if (!extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                return false;

            return orientation == 1;
        }

        private static string NormalizeImage(string source, string target)
        {
            using var original = Image.FromFile(source);
            int orientation = ReadOrientation(original);

            using var rotated = new Bitmap(original);
            rotated.RotateFlip(OrientationToRotateFlip(orientation));

            int width = Math.Max(1, rotated.Width);
            int height = Math.Max(1, rotated.Height);
            using var flattened = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            float dpiX = rotated.HorizontalResolution > 0 ? rotated.HorizontalResolution : 96f;
            float dpiY = rotated.VerticalResolution > 0 ? rotated.VerticalResolution : 96f;
            try { flattened.SetResolution(dpiX, dpiY); }
            catch { /* 某些编码器不支持自定义 DPI */ }

            using (var graphics = Graphics.FromImage(flattened))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(rotated, 0, 0, width, height);
            }

            flattened.Save(target, ImageFormat.Png);
            return target;
        }

        private static int ReadOrientation(Image image)
        {
            try
            {
                PropertyItem? item = image.GetPropertyItem(0x0112);
                if (item?.Value is { Length: >= 2 } value)
                    return BitConverter.ToUInt16(value, 0);
            }
            catch
            {
                // 没有 EXIF 方向信息
            }
            return 1;
        }

        private static RotateFlipType OrientationToRotateFlip(int orientation)
        {
            switch (orientation)
            {
                case 2: return RotateFlipType.RotateNoneFlipX;
                case 3: return RotateFlipType.Rotate180FlipNone;
                case 4: return RotateFlipType.RotateNoneFlipY;
                case 5: return RotateFlipType.Rotate90FlipX;
                case 6: return RotateFlipType.Rotate90FlipNone;
                case 7: return RotateFlipType.Rotate270FlipX;
                case 8: return RotateFlipType.Rotate270FlipNone;
                default: return RotateFlipType.RotateNoneFlipNone;
            }
        }

        private static void Report(IProgress<MergeProgress>? progress, int percent, string status)
        {
            progress?.Report(new MergeProgress(Math.Clamp(percent, 0, 100), status));
        }

        internal static void EnsureWord(ref object? word, ref object? documents)
        {
            if (word != null && documents != null) return;

            Type? type = Type.GetTypeFromProgID("Word.Application");
            if (type == null)
                throw new MergeException(Loc.T(Txt.MergeWordNotInstalled));

            try
            {
                word = Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new MergeException(Loc.T(Txt.MergeWordStartFail), ex);
            }

            if (word == null)
                throw new MergeException(Loc.T(Txt.MergeWordStartFail));

            Set(word, "Visible", false);
            Set(word, "DisplayAlerts", 0);
            // 禁用宏，确保本地文件安全
            try { Set(word, "AutomationSecurity", 3); } catch { /* 某些版本不支持 */ }
            try { Set(word, "ScreenUpdating", false); } catch { /* 忽略 */ }

            documents = Get(word, "Documents");
        }

        internal static void ExportWordToPdf(object word, object documents, string source, string targetPdf, string? password)
        {
            object? document = null;
            string name = Path.GetFileName(source);
            try
            {
                string docPassword = password ?? string.Empty;

                // Open(FileName, ConfirmConversions=false, ReadOnly=true, AddToRecentFiles=false,
                //      PasswordDocument, PasswordTemplate, Revert, WritePasswordDocument, ...)
                document = documents.GetType().InvokeMember(
                    "Open", BindingFlags.InvokeMethod, null, documents,
                    new object[]
                    {
                        source, false, true, false, docPassword, "", false, docPassword, "", 0, 0,
                        false, false, 0, true, Type.Missing
                    });

                if (document == null)
                    throw new MergeException(Loc.T(Txt.MergeWordOpenFail, name));

                // ExportAsFixedFormat：保留分页、纸张、方向、页眉页脚及页码字段结果
                document.GetType().InvokeMember(
                    "ExportAsFixedFormat", BindingFlags.InvokeMethod, null, document,
                    new object[]
                    {
                        targetPdf, 17, false, 0, 0, 0, 0, 0, true, true, 0, true, true, false, Type.Missing
                    });

                if (!File.Exists(targetPdf) || new FileInfo(targetPdf).Length == 0)
                    throw new MergeException(Loc.T(Txt.MergeWordConvertEmpty, name));
            }
            catch (MergeException)
            {
                throw;
            }
            catch (COMException ex)
            {
                throw new MergeException(Loc.T(Txt.MergeWordConvertFail, name, ex.Message), ex);
            }
            catch (Exception ex)
            {
                throw new MergeException(Loc.T(Txt.MergeWordConvertFail, name, ex.Message), ex);
            }
            finally
            {
                if (document != null)
                {
                    try { document.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, document, new object[] { 0 }); }
                    catch { /* 忽略关闭异常 */ }
                    ReleaseCom(document);
                }
            }
        }

        /// <summary>用 LibreOffice 无界面模式把 Word 文档导出为 PDF（无 Microsoft Word 时的备用方案）。</summary>
        internal static void ExportWordWithLibreOffice(string source, string targetPdf)
        {
            string name = Path.GetFileName(source);
            string? soffice = FindLibreOffice();
            if (string.IsNullOrEmpty(soffice))
                throw new MergeException(Loc.T(Txt.MergeWordNotInstalled));

            string workDir = Path.Combine(Path.GetDirectoryName(targetPdf) ?? Path.GetTempPath(),
                "lo_" + Path.GetFileNameWithoutExtension(targetPdf));
            string profileDir = Path.Combine(workDir, "profile") + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(workDir);

            try
            {
                string profileUri;
                try { profileUri = new Uri(profileDir).AbsoluteUri; }
                catch { profileUri = "file:///" + profileDir.Replace('\\', '/'); }

                var startInfo = new ProcessStartInfo
                {
                    FileName = soffice,
                    Arguments =
                        $"-env:UserInstallation={profileUri} --headless --norestore " +
                        $"--convert-to pdf --outdir \"{workDir}\" \"{source}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                    throw new MergeException(Loc.T(Txt.MergeWordStartFail));

                var log = new System.Text.StringBuilder();
                process.OutputDataReceived += (_, e) => { if (e.Data != null) lock (log) log.AppendLine(e.Data); };
                process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (log) log.AppendLine(e.Data); };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit(180000))
                {
                    try { process.Kill(true); } catch { /* 忽略 */ }
                    throw new MergeException(Loc.T(Txt.MergeWordConvertFail, name, "LibreOffice timeout"));
                }
                process.WaitForExit();

                string expected = Path.Combine(workDir, Path.GetFileNameWithoutExtension(source) + ".pdf");
                if (!File.Exists(expected))
                {
                    string[] found = Directory.GetFiles(workDir, "*.pdf");
                    if (found.Length > 0) expected = found[0];
                }

                if (!File.Exists(expected) || new FileInfo(expected).Length == 0)
                {
                    string details;
                    lock (log) details = log.ToString().Trim();
                    if (details.Length > 512) details = details.Substring(0, 512);
                    throw new MergeException(Loc.T(Txt.MergeWordConvertFail, name,
                        details.Length > 0 ? details : "LibreOffice produced no output"));
                }

                File.Copy(expected, targetPdf, true);
            }
            catch (MergeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MergeException(Loc.T(Txt.MergeWordConvertFail, name, ex.Message), ex);
            }
            finally
            {
                TryDeleteDirectory(workDir);
            }
        }

        internal static void QuitWord(ref object? word, ref object? documents)
        {
            if (word != null)
            {
                try { word.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, word, new object[] { 0 }); }
                catch { /* 忽略退出异常 */ }
            }

            ReleaseCom(documents);
            ReleaseCom(word);
            documents = null;
            word = null;
        }

        private static object Get(object target, string property, params object?[] args)
        {
            object[]? invokeArgs = args.Length == 0 ? null : args;
            return target.GetType().InvokeMember(
                property, BindingFlags.GetProperty, null, target, invokeArgs);
        }

        private static void Set(object target, string property, object value)
        {
            target.GetType().InvokeMember(
                property, BindingFlags.SetProperty, null, target, new[] { value });
        }

        private static void ReleaseCom(object? target)
        {
            if (target == null || !Marshal.IsComObject(target)) return;
            try { Marshal.FinalReleaseComObject(target); } catch { /* 忽略 */ }
        }

        internal static void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch { /* 临时文件由系统稍后清理 */ }
        }
    }
}
