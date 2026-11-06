using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DocHub
{
    /// <summary>一次 PDF 转图片任务的参数。</summary>
    public sealed class PdfConvertOptions
    {
        public ConvertImageFormat Format { get; init; } = ConvertImageFormat.Jpeg;
        public int Ppi { get; init; } = 300;
        public int JpegQuality { get; init; } = 100;
        public bool TransparentBackground { get; init; }
        public ConvertPageSelection PageSelection { get; init; } = ConvertPageSelection.All;
        public string PageRange { get; init; } = string.Empty;
        public bool CreateSubfolder { get; init; } = true;

        /// <summary>叠加到输出图片上的水印；null 表示不添加。源 PDF 始终只读。</summary>
        public WatermarkSpec? Watermark { get; init; }

        /// <summary>用于生成输出文件名的设置副本（命名方式与自定义语句）。</summary>
        public AppSettings? Naming { get; init; }
    }

    public sealed record PdfConvertProgress(
        int FileIndex, int FileCount, int Page, int PageCount, int Percent, string Status);

    public sealed record PdfConvertFileResult(
        string Source, string OutputFolder, int PageCount,
        IReadOnlyList<string> Images, string? Error);

    public sealed record PdfConvertResult(
        IReadOnlyList<PdfConvertFileResult> Files,
        int ImageCount,
        TimeSpan Elapsed,
        bool Cancelled)
    {
        public int SucceededCount => Files.Count(f => f.Error == null);
        public int FailedCount => Files.Count(f => f.Error != null);
    }

    public sealed class PdfConvertException : Exception
    {
        public PdfConvertException(string message) : base(message) { }
    }

    /// <summary>
    /// PDF 逐页栅格化：复用系统的 Windows.Data.Pdf（与内置阅读器同源），
    /// 再用 System.Drawing 编码为 PNG / JPEG / BMP / TIFF。逐页串行，避免大图同时占用内存。
    /// </summary>
    public static class PdfImageEngine
    {
        public const int MinPpi = 36;
        public const int MaxPpi = 1200;

        private const int MaxDimension = 16384;
        private const long MaxPixels = 100_000_000;

        public static string Extension(ConvertImageFormat format)
        {
            return format switch
            {
                ConvertImageFormat.Jpeg => ".jpg",
                ConvertImageFormat.Bmp => ".bmp",
                ConvertImageFormat.Tiff => ".tif",
                _ => ".png"
            };
        }

        private static ImageFormat SystemFormat(ConvertImageFormat format)
        {
            return format switch
            {
                ConvertImageFormat.Jpeg => ImageFormat.Jpeg,
                ConvertImageFormat.Bmp => ImageFormat.Bmp,
                ConvertImageFormat.Tiff => ImageFormat.Tiff,
                _ => ImageFormat.Png
            };
        }

        /// <summary>计算指定页在给定 PPI 下的目标像素尺寸。</summary>
        public static void GetTargetSize(double dipWidth, double dipHeight, int ppi,
            out uint width, out uint height)
        {
            double scale = Math.Clamp(ppi, MinPpi, MaxPpi) / 96.0;
            width = (uint)Math.Max(1, Math.Round(Math.Max(1.0, dipWidth) * scale));
            height = (uint)Math.Max(1, Math.Round(Math.Max(1.0, dipHeight) * scale));
        }

        /// <summary>按最长边限制渲染预览图，避免切换文件时卡顿。</summary>
        public static void GetPreviewSize(double dipWidth, double dipHeight, int maxEdge,
            out uint width, out uint height)
        {
            double w = Math.Max(1.0, dipWidth);
            double h = Math.Max(1.0, dipHeight);
            double scale = Math.Min(1.0, Math.Max(1, maxEdge) / Math.Max(w, h));
            width = (uint)Math.Max(1, Math.Round(w * scale));
            height = (uint)Math.Max(1, Math.Round(h * scale));
        }

        /// <summary>按页码模式解析要导出的页；页码保持原文档中的真实编号。</summary>
        public static bool TryResolvePages(ConvertPageSelection selection, string? customRange,
            int pageCount, out List<int> pages, out string? error)
        {
            pages = new List<int>();
            error = null;

            if (pageCount <= 0)
            {
                error = Loc.T(Txt.ConvertNoPages);
                return false;
            }

            switch (selection)
            {
                case ConvertPageSelection.Odd:
                    for (int page = 1; page <= pageCount; page += 2) pages.Add(page);
                    break;
                case ConvertPageSelection.Even:
                    for (int page = 2; page <= pageCount; page += 2) pages.Add(page);
                    break;
                case ConvertPageSelection.Custom:
                    if (string.IsNullOrWhiteSpace(customRange))
                    {
                        error = Loc.T(Txt.ConvertRangeNeeded);
                        return false;
                    }
                    return TryParsePageRange(customRange, pageCount, out pages, out error);
                default:
                    for (int page = 1; page <= pageCount; page++) pages.Add(page);
                    break;
            }

            if (pages.Count == 0)
            {
                error = Loc.T(Txt.ConvertNoMatchingPages);
                return false;
            }

            return true;
        }

        /// <summary>解析页码范围；空文本表示全部页。支持 1-3,5 与中文逗号。</summary>
        public static bool TryParsePageRange(string? text, int pageCount,
            out List<int> pages, out string? error)
        {
            pages = new List<int>();
            error = null;

            string raw = (text ?? string.Empty).Trim();
            if (raw.Length == 0)
            {
                for (int i = 1; i <= pageCount; i++) pages.Add(i);
                return true;
            }

            string normalized = raw
                .Replace('，', ',')
                .Replace('、', ',')
                .Replace('；', ',')
                .Replace(';', ',')
                .Replace(' ', ',')
                .Replace('\t', ',');

            var selected = new SortedSet<int>();
            foreach (string rawToken in normalized.Split(','))
            {
                string token = rawToken.Trim();
                if (token.Length == 0) continue;

                int dash = token.IndexOfAny(new[] { '-', '–', '—', '－' });
                if (dash >= 0)
                {
                    string startText = token.Substring(0, dash).Trim();
                    string endText = token.Substring(dash + 1).Trim();
                    if (!int.TryParse(startText, out int start) ||
                        !int.TryParse(endText, out int end) ||
                        start < 1 || end < start)
                    {
                        error = Loc.T(Txt.ConvertBadRange);
                        return false;
                    }

                    if (pageCount > 0 && end > pageCount)
                    {
                        error = Loc.T(Txt.ConvertRangeOut, pageCount);
                        return false;
                    }

                    for (int page = start; page <= end; page++) selected.Add(page);
                }
                else
                {
                    if (!int.TryParse(token, out int page) || page < 1)
                    {
                        error = Loc.T(Txt.ConvertBadRange);
                        return false;
                    }

                    if (pageCount > 0 && page > pageCount)
                    {
                        error = Loc.T(Txt.ConvertRangeOut, pageCount);
                        return false;
                    }

                    selected.Add(page);
                }
            }

            if (selected.Count == 0)
            {
                error = Loc.T(Txt.ConvertBadRange);
                return false;
            }

            pages.AddRange(selected);
            return true;
        }

        /// <summary>页码模式对应的本地化短名，用于默认输出子文件夹。</summary>
        public static string PageSelectionLabel(ConvertPageSelection selection) => selection switch
        {
            ConvertPageSelection.Odd => Loc.T(Txt.ConvertPageModeOdd),
            ConvertPageSelection.Even => Loc.T(Txt.ConvertPageModeEven),
            ConvertPageSelection.Custom => Loc.T(Txt.ConvertPageModeCustom),
            _ => Loc.T(Txt.ConvertPageModeAll)
        };

        /// <summary>读取页数；加密或损坏时返回 null。</summary>
        public static int? TryGetPageCount(string path, string? password = null)
        {
            try
            {
                PdfDocument document = OpenDocument(path, password);
                uint count = document.PageCount;
                return count > int.MaxValue ? int.MaxValue : (int)count;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>读取指定页的 DIP 尺寸（1/96 英寸），用于换算目标像素。</summary>
        public static bool TryGetPageSize(string path, int pageIndex, string? password,
            out double width, out double height)
        {
            width = 0;
            height = 0;
            try
            {
                PdfDocument document = OpenDocument(path, password);
                if (pageIndex < 0 || pageIndex >= (int)document.PageCount) return false;
                using PdfPage page = document.GetPage((uint)pageIndex);
                width = page.Size.Width;
                height = page.Size.Height;
                return width > 0 && height > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>渲染指定页的缩略图；失败返回 null，由调用方显示空状态。</summary>
        public static Bitmap? RenderPreview(string path, int pageIndex, int maxEdge,
            string? password, bool transparent)
        {
            string? tempPath = null;
            try
            {
                PdfDocument document = OpenDocument(path, password);
                if (pageIndex < 0 || pageIndex >= (int)document.PageCount) return null;

                using PdfPage page = document.GetPage((uint)pageIndex);
                GetPreviewSize(page.Size.Width, page.Size.Height, maxEdge,
                    out uint width, out uint height);

                tempPath = Path.Combine(Path.GetTempPath(), "DocHub",
                    "preview_" + Guid.NewGuid().ToString("N") + ".png");
                using Bitmap rendered = RenderPage(page, width, height, transparent, tempPath);
                return new Bitmap(rendered);
            }
            catch
            {
                return null;
            }
            finally
            {
                TryDelete(tempPath);
            }
        }

        /// <summary>
        /// 批量转换入口。单个 PDF 失败不会中止整批；取消时保留已导出的图片并返回部分结果。
        /// </summary>
        public static PdfConvertResult Run(
            IReadOnlyList<string> pdfPaths,
            string outputRoot,
            PdfConvertOptions options,
            IProgress<PdfConvertProgress>? progress,
            CancellationToken token,
            IReadOnlyDictionary<string, string>? passwords)
        {
            if (pdfPaths.Count == 0)
                throw new PdfConvertException(Loc.T(Txt.ConvertNoFiles));
            if (string.IsNullOrWhiteSpace(outputRoot))
                throw new PdfConvertException(Loc.T(Txt.ConvertNeedFolder));

            outputRoot = Path.GetFullPath(outputRoot);
            Directory.CreateDirectory(outputRoot);

            var stopwatch = Stopwatch.StartNew();
            var results = new List<PdfConvertFileResult>(pdfPaths.Count);
            int imageCount = 0;
            bool cancelled = false;

            // 批量转换强制分文件夹，避免不同 PDF 的图片混在一起
            bool createSubfolder = options.CreateSubfolder || pdfPaths.Count > 1;
            string formatExtension = Extension(options.Format);
            bool transparent = options.TransparentBackground && options.Format == ConvertImageFormat.Png;
            AppSettings naming = options.Naming ?? new AppSettings();

            string workDir = Path.Combine(Path.GetTempPath(), "DocHub");
            Directory.CreateDirectory(workDir);
            string renderTemp = Path.Combine(workDir, "render_" + Guid.NewGuid().ToString("N") + ".png");

            try
            {
                for (int fileIndex = 0; fileIndex < pdfPaths.Count; fileIndex++)
                {
                    if (token.IsCancellationRequested)
                    {
                        cancelled = true;
                        break;
                    }

                    string source = pdfPaths[fileIndex];
                    string fileName = Path.GetFileName(source);
                    var images = new List<string>();
                    string outputFolder = outputRoot;
                    int totalPages = 0;
                    string? error = null;

                    try
                    {
                        string? openPassword = null;
                        if (passwords != null &&
                            !passwords.TryGetValue(source, out openPassword))
                            passwords.TryGetValue(fileName, out openPassword);

                        PdfDocument document = OpenDocument(source, openPassword);
                        totalPages = (int)document.PageCount;
                        if (totalPages == 0)
                            throw new PdfConvertException(Loc.T(Txt.ConvertNoPages));

                        if (!TryResolvePages(options.PageSelection, options.PageRange, totalPages,
                                out List<int> pages, out string? rangeError))
                            throw new PdfConvertException(rangeError ?? Loc.T(Txt.ConvertBadRange));

                        string stem = SanitizeFileName(
                            naming.BuildNameStem(source, DateTime.Now, forConversion: true));
                        if (stem.Length == 0) stem = SanitizeFileName(Loc.T(Txt.DefaultConvertName));

                        outputFolder = createSubfolder
                            ? CreateUniqueFolder(outputRoot, SanitizeFileName(
                                Path.GetFileNameWithoutExtension(source) + "_" + PageSelectionLabel(options.PageSelection)))
                            : outputRoot;

                        int pageDigits = Math.Max(3, totalPages.ToString().Length);
                        int done = 0;

                        foreach (int pageNumber in pages)
                        {
                            if (token.IsCancellationRequested)
                            {
                                cancelled = true;
                                break;
                            }

                            using PdfPage page = document.GetPage((uint)(pageNumber - 1));
                            GetTargetSize(page.Size.Width, page.Size.Height, options.Ppi,
                                out uint width, out uint height);

                            if (width > MaxDimension || height > MaxDimension ||
                                (long)width * height > MaxPixels)
                            {
                                throw new PdfConvertException(Loc.T(Txt.ConvertPpiTooLarge, width, height));
                            }

                            using Bitmap rendered = RenderPage(page, width, height, transparent, renderTemp);
                            using Bitmap final = PrepareForEncoding(rendered, options, transparent);
                            if (options.Watermark != null)
                                WatermarkPainter.ApplyToBitmap(final, options.Watermark);
                            final.SetResolution(options.Ppi, options.Ppi);

                            string target = Path.Combine(outputFolder,
                                stem + "_p" + pageNumber.ToString("D" + pageDigits) + formatExtension);
                            if (!createSubfolder) target = UniqueFilePath(target);
                            SaveAtomic(final, target, options.Format, options.JpegQuality);
                            images.Add(target);

                            done++;
                            int filePercent = pages.Count == 0 ? 100 : (int)(done * 100.0 / pages.Count);
                            int percent = (int)((fileIndex + done / (double)Math.Max(1, pages.Count))
                                * 100.0 / Math.Max(1, pdfPaths.Count));
                            progress?.Report(new PdfConvertProgress(
                                fileIndex + 1, pdfPaths.Count, done, pages.Count,
                                Math.Clamp(percent, 0, 100),
                                Loc.T(Txt.ConvertProcessing, fileIndex + 1, pdfPaths.Count, fileName)
                                + " · " + Loc.T(Txt.ConvertPageStatus, done, pages.Count)));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        cancelled = true;
                    }
                    catch (PdfConvertException ex)
                    {
                        error = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    imageCount += images.Count;
                    results.Add(new PdfConvertFileResult(
                        source, outputFolder, totalPages, images, error));

                    if (cancelled) break;
                }
            }
            finally
            {
                TryDelete(renderTemp);
            }

            stopwatch.Stop();
            return new PdfConvertResult(results, imageCount, stopwatch.Elapsed, cancelled);
        }

        // ---------------- 内部实现 ----------------

        private static PdfDocument OpenDocument(string path, string? password)
        {
            string fullPath = Path.GetFullPath(path);
            // Windows.Data.Pdf 以只读方式加载：转换只读取页面内容，不会回写源 PDF
            StorageFile file = StorageFile.GetFileFromPathAsync(fullPath)
                .AsTask().GetAwaiter().GetResult();

            try
            {
                return string.IsNullOrEmpty(password)
                    ? PdfDocument.LoadFromFileAsync(file).AsTask().GetAwaiter().GetResult()
                    : PdfDocument.LoadFromFileAsync(file, password).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (IsPasswordError(ex))
            {
                throw new PdfConvertException(Loc.T(Txt.MergePasswordTitle));
            }
        }

        private static bool IsPasswordError(Exception ex)
        {
            if (ex is UnauthorizedAccessException) return true;
            int hr = ex.HResult;
            if (hr == unchecked((int)0x80070005)) return true; // E_ACCESSDENIED
            if (hr == unchecked((int)0x80070056)) return true; // ERROR_INVALID_PASSWORD
            string message = ex.Message ?? string.Empty;
            return message.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                   message.Contains("encrypted", StringComparison.OrdinalIgnoreCase);
        }

        private static Bitmap RenderPage(PdfPage page, uint width, uint height,
            bool transparent, string tempPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
            using (File.Create(tempPath)) { }

            StorageFile tempFile = StorageFile.GetFileFromPathAsync(tempPath)
                .AsTask().GetAwaiter().GetResult();

            var renderOptions = new PdfPageRenderOptions
            {
                DestinationWidth = width,
                DestinationHeight = height,
                BackgroundColor = transparent
                    ? Windows.UI.Color.FromArgb(0, 0, 0, 0)
                    : Windows.UI.Color.FromArgb(255, 255, 255, 255)
            };

            using (IRandomAccessStream stream = tempFile.OpenAsync(FileAccessMode.ReadWrite)
                .AsTask().GetAwaiter().GetResult())
            {
                page.RenderToStreamAsync(stream, renderOptions).AsTask().GetAwaiter().GetResult();
            }

            using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var decoded = Image.FromStream(fileStream);
            return new Bitmap(decoded);
        }

        private static Bitmap PrepareForEncoding(Bitmap source, PdfConvertOptions options, bool transparent)
        {
            // JPEG / BMP / TIFF 统一铺白底，避免透明区域在部分查看器里变黑
            if (options.Format == ConvertImageFormat.Png && transparent) return new Bitmap(source);

            var flattened = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(flattened))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(source, 0, 0, source.Width, source.Height);
            }
            return flattened;
        }

        private static void SaveAtomic(Bitmap bitmap, string target, ConvertImageFormat format, int quality)
        {
            string extension = Extension(format);
            string temp = target + ".tmp" + extension;
            try
            {
                switch (format)
                {
                    case ConvertImageFormat.Jpeg:
                        SaveWithQuality(bitmap, temp, ImageFormat.Jpeg, Encoder.Quality, quality);
                        break;
                    case ConvertImageFormat.Tiff:
                        SaveWithQuality(bitmap, temp, ImageFormat.Tiff, Encoder.Compression,
                            (long)EncoderValue.CompressionLZW);
                        break;
                    case ConvertImageFormat.Bmp:
                        bitmap.Save(temp, SystemFormat(ConvertImageFormat.Bmp));
                        break;
                    default:
                        bitmap.Save(temp, SystemFormat(ConvertImageFormat.Png));
                        break;
                }
                File.Move(temp, target, true);
            }
            finally
            {
                TryDelete(temp);
            }
        }

        private static void SaveWithQuality(Bitmap bitmap, string path, ImageFormat imageFormat,
            Encoder encoder, long value)
        {
            ImageCodecInfo? codec = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(c => c.FormatID == imageFormat.Guid);
            if (codec == null)
            {
                bitmap.Save(path, imageFormat);
                return;
            }

            using var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(encoder, value);
            bitmap.Save(path, codec, parameters);
        }

        private static string SanitizeFileName(string name)
        {
            string result = (name ?? string.Empty).Trim();
            foreach (char invalid in Path.GetInvalidFileNameChars())
                result = result.Replace(invalid, '_');
            return result.Trim().TrimEnd('.');
        }

        private static string CreateUniqueFolder(string root, string desired)
        {
            string name = desired.Length == 0 ? Loc.T(Txt.DefaultConvertName) : desired;
            string candidate = Path.Combine(root, name);
            int index = 1;
            while (Directory.Exists(candidate))
            {
                candidate = Path.Combine(root, name + "-" + index);
                index++;
            }
            Directory.CreateDirectory(candidate);
            return candidate;
        }

        private static string UniqueFilePath(string target)
        {
            if (!File.Exists(target)) return target;

            string directory = Path.GetDirectoryName(target)!;
            string stem = Path.GetFileNameWithoutExtension(target);
            string extension = Path.GetExtension(target);
            for (int index = 1; ; index++)
            {
                string candidate = Path.Combine(directory, stem + "-" + index + extension);
                if (!File.Exists(candidate)) return candidate;
            }
        }

        private static void TryDelete(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try { if (File.Exists(path)) File.Delete(path); } catch { /* 临时文件交给系统清理 */ }
        }
    }
}
