using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace DocHub
{
    /// <summary>一次图片转 PDF 任务的参数。</summary>
    public sealed class ImagePdfOptions
    {
        public ImagePdfPageSize PageSize { get; init; } = ImagePdfPageSize.A4;
        public ImagePdfOrientation Orientation { get; init; } = ImagePdfOrientation.Auto;

        /// <summary>页面四边留白（毫米）；页面大小为自适应时忽略。</summary>
        public int MarginMm { get; init; } = 10;

        /// <summary>叠加到输出 PDF 页面上的水印；null 表示不添加。</summary>
        public WatermarkSpec? Watermark { get; init; }
    }

    public sealed record ImagePdfProgress(int FileIndex, int FileCount, int Percent);

    public sealed record ImagePdfResult(
        string OutputPath, int PageCount, TimeSpan Elapsed, bool Cancelled);

    /// <summary>逐张输出结果：OutputPaths 只包含已成功写完的 PDF；取消时保留这些文件。</summary>
    public sealed record ImagePdfBatchResult(
        IReadOnlyList<string> OutputPaths,
        int FileCount,
        int FailedCount,
        TimeSpan Elapsed,
        bool Cancelled,
        string? FirstError = null);

    public sealed class ImagePdfException : Exception
    {
        public ImagePdfException(string message, Exception? inner = null) : base(message, inner) { }
    }

    /// <summary>
    /// 图片批量合成 PDF：按列表顺序每张图片一页，可选择 A4 / A3 等纸张、方向与留白。
    /// 图片等比缩放并居中，不裁剪、不拉伸；自适应纸张则页面尺寸跟随图片。
    /// </summary>
    public static class ImagePdfEngine
    {
        public const int MinMarginMm = 0;
        public const int MaxMarginMm = 30;

        private const double PointsPerMm = 72.0 / 25.4;
        private const double MaxPagePoints = 14400.0;
        private const int PreviewMaxEdge = 900;

        /// <summary>标准纸张的纵向尺寸（point）。</summary>
        private static (double Width, double Height) PaperSize(ImagePdfPageSize size)
        {
            return size switch
            {
                ImagePdfPageSize.A3 => (841.89, 1190.55),
                ImagePdfPageSize.A5 => (419.53, 595.28),
                ImagePdfPageSize.B5 => (498.90, 708.66),
                ImagePdfPageSize.Letter => (612.0, 792.0),
                _ => (595.28, 841.89)
            };
        }

        public static string PageSizeLabel(ImagePdfPageSize size)
        {
            return size switch
            {
                ImagePdfPageSize.A4 => "A4",
                ImagePdfPageSize.A3 => "A3",
                ImagePdfPageSize.A5 => "A5",
                ImagePdfPageSize.B5 => "B5",
                ImagePdfPageSize.Letter => "Letter",
                _ => Loc.T(Txt.ImagePdfPageSizeAuto)
            };
        }

        public static ImagePdfResult Run(
            IReadOnlyList<string> files,
            string outputPath,
            ImagePdfOptions options,
            IProgress<ImagePdfProgress>? progress,
            CancellationToken token)
        {
            if (files.Count == 0)
                throw new ImagePdfException(Loc.T(Txt.ImagePdfNoFiles));

            foreach (string file in files)
            {
                if (!File.Exists(file))
                    throw new ImagePdfException(Loc.T(Txt.ImagePdfUnreadable, Path.GetFileName(file), ""));
            }

            var stopwatch = Stopwatch.StartNew();
            var images = new List<LoadedImage>(files.Count);
            int completed = 0;
            string? watermarkTemp = null;

            try
            {
                using var document = new PdfDocument();
                document.Info.Title = Path.GetFileNameWithoutExtension(outputPath);

                for (int i = 0; i < files.Count; i++)
                {
                    token.ThrowIfCancellationRequested();

                    string source = files[i];
                    LoadedImage image = Load(files[i]);
                    images.Add(image);

                    var page = document.AddPage();
                    (double pageWidth, double pageHeight, double drawX, double drawY,
                        double drawWidth, double drawHeight) = Layout(image, options);

                    page.Width = XUnit.FromPoint(pageWidth);
                    page.Height = XUnit.FromPoint(pageHeight);

                    using (var graphics = XGraphics.FromPdfPage(page))
                        graphics.DrawImage(image.Image, drawX, drawY, drawWidth, drawHeight);

                    completed++;
                    progress?.Report(new ImagePdfProgress(i, files.Count,
                        (int)(completed * 100.0 / files.Count)));
                }

                token.ThrowIfCancellationRequested();

                if (options.Watermark != null)
                {
                    watermarkTemp = Path.Combine(
                        Path.GetTempPath(), "DocHub", "wm_" + Guid.NewGuid().ToString("N"));
                    WatermarkPainter.Apply(document, options.Watermark, watermarkTemp);
                }

                string? folder = Path.GetDirectoryName(Path.GetFullPath(outputPath));
                if (!string.IsNullOrEmpty(folder))
                    Directory.CreateDirectory(folder);

                // 图片对象需保留到 Save 完成，临时 PNG 也在 finally 中统一清理
                document.Save(outputPath);

                if (!File.Exists(outputPath) || new FileInfo(outputPath).Length == 0)
                    throw new ImagePdfException(Loc.T(Txt.ImagePdfWriteError, outputPath, "empty"));

                stopwatch.Stop();
                return new ImagePdfResult(outputPath, completed, stopwatch.Elapsed, false);
            }
            catch (OperationCanceledException)
            {
                TryDelete(outputPath);
                stopwatch.Stop();
                return new ImagePdfResult(outputPath, completed, stopwatch.Elapsed, true);
            }
            catch (ImagePdfException)
            {
                TryDelete(outputPath);
                throw;
            }
            catch (IOException ex)
            {
                TryDelete(outputPath);
                throw new ImagePdfException(Loc.T(Txt.ImagePdfWriteError, outputPath, ex.Message), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                TryDelete(outputPath);
                throw new ImagePdfException(Loc.T(Txt.ImagePdfWriteError, outputPath, ex.Message), ex);
            }
            catch (Exception ex)
            {
                TryDelete(outputPath);
                throw new ImagePdfException(Loc.T(Txt.ImagePdfFailGeneric, ex.Message), ex);
            }
            finally
            {
                DisposeImages(images);
                TryDeleteDirectory(watermarkTemp);
            }
        }

        /// <summary>
        /// 逐张输出：每张图片生成一个独立 PDF。单张失败会继续处理其余文件；
        /// 取消时停止并保留已写完的 PDF，返回成功数量与失败数量。
        /// </summary>
        public static ImagePdfBatchResult RunSeparate(
            IReadOnlyList<string> files,
            IReadOnlyList<string> outputPaths,
            ImagePdfOptions options,
            IProgress<ImagePdfProgress>? progress,
            CancellationToken token)
        {
            if (files.Count == 0)
                throw new ImagePdfException(Loc.T(Txt.ImagePdfNoFiles));
            if (outputPaths.Count != files.Count)
                throw new ImagePdfException(Loc.T(Txt.ImagePdfFailGeneric, "output path count mismatch"));

            var stopwatch = Stopwatch.StartNew();
            var outputs = new List<string>(files.Count);
            int failed = 0;
            string? firstError = null;
            bool cancelled = false;

            for (int i = 0; i < files.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    cancelled = true;
                    break;
                }

                int index = i;
                var inner = new InlineProgress<ImagePdfProgress>(p =>
                {
                    int percent = (int)((index + p.Percent / 100.0) * 100.0 / files.Count);
                    progress?.Report(new ImagePdfProgress(index, files.Count,
                        Math.Clamp(percent, 0, 100)));
                });

                try
                {
                    ImagePdfResult result = Run(new[] { files[i] }, outputPaths[i],
                        options, inner, token);
                    if (result.Cancelled)
                    {
                        cancelled = true;
                        break;
                    }

                    outputs.Add(outputPaths[i]);
                }
                catch (OperationCanceledException)
                {
                    cancelled = true;
                    break;
                }
                catch (Exception ex)
                {
                    failed++;
                    firstError ??= ex.Message;
                }
            }

            stopwatch.Stop();

            // 全部失败时按失败处理，让界面显示具体原因；部分成功则正常结束并汇总
            if (!cancelled && outputs.Count == 0 && failed > 0)
                throw new ImagePdfException(firstError ?? Loc.T(Txt.ImagePdfFailStatus));

            return new ImagePdfBatchResult(outputs, files.Count, failed,
                stopwatch.Elapsed, cancelled, firstError);
        }

        private sealed class InlineProgress<T> : IProgress<T>
        {
            private readonly Action<T> _report;

            public InlineProgress(Action<T> report) => _report = report;

            public void Report(T value) => _report(value);
        }

        /// <summary>计算页面尺寸与图片绘制矩形：等比缩放、居中，保留四边留白。</summary>
        private static (double PageWidth, double PageHeight, double X, double Y,
            double Width, double Height) Layout(LoadedImage image, ImagePdfOptions options)
        {
            double pixelWidth = Math.Max(1, image.PixelWidth);
            double pixelHeight = Math.Max(1, image.PixelHeight);
            bool imageLandscape = pixelWidth > pixelHeight;

            if (options.PageSize == ImagePdfPageSize.Auto)
            {
                double width = Math.Max(1.0, pixelWidth / image.DpiX * 72.0);
                double height = Math.Max(1.0, pixelHeight / image.DpiY * 72.0);

                double overflow = Math.Max(width, height) / MaxPagePoints;
                if (overflow > 1.0)
                {
                    width /= overflow;
                    height /= overflow;
                }

                return (width, height, 0, 0, width, height);
            }

            (double paperWidth, double paperHeight) = PaperSize(options.PageSize);
            bool landscape = options.Orientation switch
            {
                ImagePdfOrientation.Portrait => false,
                ImagePdfOrientation.Landscape => true,
                _ => imageLandscape
            };

            double pageWidth = landscape ? paperHeight : paperWidth;
            double pageHeight = landscape ? paperWidth : paperHeight;

            double margin = Math.Clamp(options.MarginMm, MinMarginMm, MaxMarginMm) * PointsPerMm;
            if (margin * 2 >= pageWidth || margin * 2 >= pageHeight) margin = 0;

            double contentWidth = Math.Max(1.0, pageWidth - margin * 2);
            double contentHeight = Math.Max(1.0, pageHeight - margin * 2);

            double scale = Math.Min(contentWidth / pixelWidth, contentHeight / pixelHeight);
            double drawWidth = Math.Max(1.0, pixelWidth * scale);
            double drawHeight = Math.Max(1.0, pixelHeight * scale);
            double x = (pageWidth - drawWidth) / 2.0;
            double y = (pageHeight - drawHeight) / 2.0;

            return (pageWidth, pageHeight, x, y, drawWidth, drawHeight);
        }

        // ---------------- 图片解码 ----------------

        private sealed class LoadedImage : IDisposable
        {
            public XImage Image { get; init; } = null!;
            public int PixelWidth { get; init; }
            public int PixelHeight { get; init; }
            public double DpiX { get; init; } = 96;
            public double DpiY { get; init; } = 96;
            public string? TempPath { get; set; }

            public void Dispose()
            {
                try { Image.Dispose(); } catch { /* 文档保存后再释放临时文件 */ }
            }
        }

        private static LoadedImage Load(string source)
        {
            string name = Path.GetFileName(source);
            string? temp = null;

            try
            {
                bool readable = TryReadImageInfo(source, out ImageInfo info);
                XImage? xImage = null;

                if (readable && CanEmbedDirectly(source, info.Orientation))
                {
                    try { xImage = XImage.FromFile(source); }
                    catch { xImage = null; }
                }

                if (xImage == null)
                {
                    temp = Path.Combine(Path.GetTempPath(),
                        "DocHub_img_" + Guid.NewGuid().ToString("N") + ".png");

                    if (readable)
                    {
                        NormalizeImage(source, temp);
                    }
                    else if (SystemImageDecoder.TryConvertToPng(source, temp,
                                 out int wicWidth, out int wicHeight,
                                 out double wicDpiX, out double wicDpiY, out string error))
                    {
                        info = new ImageInfo(wicWidth, wicHeight, wicDpiX, wicDpiY, 1);
                    }
                    else
                    {
                        throw new ImagePdfException(Loc.T(Txt.ImagePdfUnreadable, name, error));
                    }

                    xImage = XImage.FromFile(temp);
                }

                return new LoadedImage
                {
                    Image = xImage,
                    PixelWidth = info.Width,
                    PixelHeight = info.Height,
                    DpiX = info.DpiX > 0 ? info.DpiX : 96,
                    DpiY = info.DpiY > 0 ? info.DpiY : 96,
                    TempPath = temp
                };
            }
            catch (ImagePdfException)
            {
                TryDelete(temp);
                throw;
            }
            catch (Exception ex)
            {
                TryDelete(temp);
                throw new ImagePdfException(Loc.T(Txt.ImagePdfUnreadable, name, ex.Message), ex);
            }
        }

        private static void DisposeImages(List<LoadedImage> images)
        {
            foreach (LoadedImage image in images)
            {
                image.Dispose();
                TryDelete(image.TempPath);
                image.TempPath = null;
            }
            images.Clear();
        }

        private sealed record ImageInfo(int Width, int Height, double DpiX, double DpiY, int Orientation);

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

            if (orientation is >= 5 and <= 8)
            {
                (width, height) = (height, width);
                (dpiX, dpiY) = (dpiY, dpiX);
            }

            return new ImageInfo(width, height, dpiX, dpiY, orientation);
        }

        private static bool CanEmbedDirectly(string path, int orientation)
        {
            string extension = Path.GetExtension(path);
            if (!extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                return false;

            return orientation == 1;
        }

        private static void NormalizeImage(string source, string target)
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

        /// <summary>读取图片预览；返回的位图由调用方释放。</summary>
        public static Bitmap? LoadPreview(string path, out int pixelWidth, out int pixelHeight,
            out string error)
        {
            pixelWidth = 0;
            pixelHeight = 0;
            error = string.Empty;

            try
            {
                using var original = Image.FromFile(path);
                int orientation = ReadOrientation(original);
                using var rotated = new Bitmap(original);
                rotated.RotateFlip(OrientationToRotateFlip(orientation));

                pixelWidth = Math.Max(1, rotated.Width);
                pixelHeight = Math.Max(1, rotated.Height);

                double scale = Math.Min(1.0,
                    PreviewMaxEdge / (double)Math.Max(pixelWidth, pixelHeight));
                int width = Math.Max(1, (int)Math.Round(pixelWidth * scale));
                int height = Math.Max(1, (int)Math.Round(pixelHeight * scale));

                var preview = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                try { preview.SetResolution(96f, 96f); }
                catch { /* 忽略 */ }

                using (var graphics = Graphics.FromImage(preview))
                {
                    graphics.Clear(Color.White);
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(rotated, 0, 0, width, height);
                }

                return preview;
            }
            catch
            {
                // GDI+ 不支持 WebP / HEIC 等格式时改用系统解码器
                string temp = Path.Combine(Path.GetTempPath(),
                    "DocHub_preview_" + Guid.NewGuid().ToString("N") + ".png");
                try
                {
                    if (!SystemImageDecoder.TryConvertToPng(path, temp,
                            out int w, out int h, out _, out _, out error))
                        return null;

                    pixelWidth = w;
                    pixelHeight = h;

                    using var decoded = Image.FromFile(temp);
                    double scale = Math.Min(1.0,
                        PreviewMaxEdge / (double)Math.Max(decoded.Width, decoded.Height));
                    int width = Math.Max(1, (int)Math.Round(decoded.Width * scale));
                    int height = Math.Max(1, (int)Math.Round(decoded.Height * scale));

                    var preview = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                    using (var graphics = Graphics.FromImage(preview))
                    {
                        graphics.Clear(Color.White);
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(decoded, 0, 0, width, height);
                    }
                    return preview;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    TryDelete(temp);
                }
            }
        }

        private static void TryDelete(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try { if (File.Exists(path)) File.Delete(path); }
            catch { /* 临时文件交给系统稍后清理 */ }
        }

        private static void TryDeleteDirectory(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try { if (Directory.Exists(path)) Directory.Delete(path, true); }
            catch { /* 临时目录交给系统稍后清理 */ }
        }
    }
}
