using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace DocHub
{
    /// <summary>
    /// 水印参数。除文字、字体、颜色与位置外，还支持任意不透明度、大小、位置微调与倾斜方向；
    /// 所有数值都可以直接键入，也可以在预览框里拖动/滚轮调整。
    /// </summary>
    public sealed record WatermarkSpec(
        string Text,
        WatermarkTint Tint,
        int OpacityPercent = 35,
        int ScalePercent = 100,
        int RotationDegrees = 30,
        int OffsetXPercent = 0,
        int OffsetYPercent = 0,
        WatermarkAnchor Anchor = WatermarkAnchor.Tile,
        string FontFamily = WatermarkPainter.DefaultFontFamily);

    public static class WatermarkPainter
    {
        /// <summary>默认水印字体；旧版本设置文件没有字体字段时沿用它。</summary>
        public const string DefaultFontFamily = "Microsoft YaHei UI";

        /// <summary>设置页提供的字体候选；只列出系统已安装的项，缺失时自动跳过。</summary>
        private static readonly string[] FontChoices =
        {
            "Microsoft YaHei UI", "Microsoft YaHei", "DengXian", "SimSun", "SimHei", "KaiTi", "FangSong",
            "Arial", "Times New Roman", "Georgia", "Segoe UI", "Calibri"
        };

        private const float FontPixelSize = 72f;

        /// <summary>水印图片宽度占页面宽度的基准比例，再叠乘用户设置的大小倍率。</summary>
        private const double BaseWidthRatio = 0.30;

        /// <summary>固定在角/正中时，水印到页面边缘的留白比例。</summary>
        private const double EdgeMarginRatio = 0.06;

        /// <summary>预览绘制时复用的度量字体（按字体族缓存），避免每帧新建 72px 字体造成卡顿。</summary>
        private static readonly Dictionary<string, Font> ReferenceFonts = new(StringComparer.Ordinal);
        private static readonly Dictionary<string, float> ReferenceWidthCache = new(StringComparer.Ordinal);
        private static readonly object CacheLock = new();

        private static Font? _previewFont;
        private static string _previewFontKey = string.Empty;

        /// <summary>已安装字体族：语言名与英文名都映射到实际可用的族名。</summary>
        private static Dictionary<string, string>? _installedFonts;

        private static Dictionary<string, string> InstalledFontMap()
        {
            lock (CacheLock)
            {
                if (_installedFonts != null) return _installedFonts;
            }

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var collection = new InstalledFontCollection())
            {
                foreach (FontFamily family in collection.Families)
                {
                    string name = family.Name;
                    map[name] = name;
                    try { map[family.GetName(0x0409)] = name; }
                    catch (ArgumentException) { }
                }
            }

            lock (CacheLock) { _installedFonts = map; }
            return map;
        }

        /// <summary>把设置里的字体名解析成系统实际可用的族名；缺失或无效时回退默认字体。</summary>
        public static string NormalizeFontFamily(string? name)
        {
            Dictionary<string, string> installed = InstalledFontMap();
            string trimmed = (name ?? string.Empty).Trim();
            if (trimmed.Length > 0 && installed.TryGetValue(trimmed, out string? actual))
                return actual;
            if (installed.TryGetValue(DefaultFontFamily, out string? fallback))
                return fallback;
            return DefaultFontFamily;
        }

        /// <summary>设置页字体下拉的候选：默认字体在前，其后是已安装的常用字体与当前值。</summary>
        public static IReadOnlyList<string> AvailableFontChoices(string? current)
        {
            Dictionary<string, string> installed = InstalledFontMap();
            var result = new List<string>();

            void Add(string name)
            {
                if (installed.TryGetValue(name, out string? actual) && !result.Contains(actual))
                    result.Add(actual);
            }

            Add(DefaultFontFamily);
            foreach (string name in FontChoices)
                Add(name);
            Add(current ?? string.Empty);

            if (result.Count == 0)
                result.Add(DefaultFontFamily);
            return result;
        }

        private static Font CreateFont(string? family, float size)
        {
            return new Font(NormalizeFontFamily(family), size, FontStyle.Bold, GraphicsUnit.Pixel);
        }


        private static Color TintColor(WatermarkTint tint, int opacityPercent)
        {
            int alpha = (int)Math.Round(Math.Clamp(opacityPercent, 0, 100) * 255.0 / 100.0);
            return tint switch
            {
                WatermarkTint.Red => Color.FromArgb(alpha, 196, 46, 46),
                WatermarkTint.Blue => Color.FromArgb(alpha, 40, 88, 190),
                _ => Color.FromArgb(alpha, 110, 116, 126)
            };
        }

        private static double TargetWidth(double pageWidth, WatermarkSpec spec)
        {
            double scale = Math.Clamp(spec.ScalePercent, 10, 400) / 100.0;
            return Math.Max(4.0, pageWidth * BaseWidthRatio * scale);
        }

        private static double RotationOf(WatermarkSpec spec)
        {
            return Math.Clamp(spec.RotationDegrees, -180, 180);
        }

        private static double HorizontalShift(double pageWidth, WatermarkSpec spec)
        {
            return pageWidth * Math.Clamp(spec.OffsetXPercent, -100, 100) / 100.0;
        }

        private static double VerticalShift(double pageHeight, WatermarkSpec spec)
        {
            return pageHeight * Math.Clamp(spec.OffsetYPercent, -100, 100) / 100.0;
        }

        /// <summary>按位置算出水印左上角坐标（未叠加用户微调偏移）。</summary>
        private static (double X, double Y) AnchorOrigin(
            double pageWidth, double pageHeight, double width, double height, WatermarkAnchor anchor)
        {
            double marginX = pageWidth * EdgeMarginRatio;
            double marginY = pageHeight * EdgeMarginRatio;

            return anchor switch
            {
                WatermarkAnchor.TopLeft => (marginX, marginY),
                WatermarkAnchor.TopRight => (pageWidth - width - marginX, marginY),
                WatermarkAnchor.Center => ((pageWidth - width) / 2.0, (pageHeight - height) / 2.0),
                WatermarkAnchor.BottomLeft => (marginX, pageHeight - height - marginY),
                WatermarkAnchor.BottomRight => (pageWidth - width - marginX, pageHeight - height - marginY),
                _ => (0, 0)
            };
        }

        /// <summary>水印旋转后实际占用的外框尺寸。定位与越界判断都按外框算，而不是按未旋转的矩形。</summary>
        private static (double Width, double Height) RotatedBox(double width, double height, double degrees)
        {
            double radians = Math.Abs(degrees) * Math.PI / 180.0;
            double cos = Math.Abs(Math.Cos(radians));
            double sin = Math.Abs(Math.Sin(radians));
            return (width * cos + height * sin, width * sin + height * cos);
        }

        /// <summary>
        /// 按页面可用区域把水印等比缩小。用户把「大小」拉到 400% 时，原来的实现会画出比页面还宽的
        /// 水印，合并后在阅读器里被页边裁掉、只看得到中间一段；这里保证旋转后的外框始终落在页边距内。
        /// </summary>
        private static double FitFactor(
            double pageWidth, double pageHeight, double width, double height, double degrees)
        {
            (double boxWidth, double boxHeight) = RotatedBox(width, height, degrees);
            double availableWidth = Math.Max(1.0, pageWidth * (1.0 - EdgeMarginRatio * 2.0));
            double availableHeight = Math.Max(1.0, pageHeight * (1.0 - EdgeMarginRatio * 2.0));

            double factor = Math.Min(
                availableWidth / Math.Max(0.001, boxWidth),
                availableHeight / Math.Max(0.001, boxHeight));
            return Math.Min(1.0, factor);
        }

        /// <summary>把外框左上角限制在页面内，避免用户拖到边缘后水印被页面切掉。</summary>
        private static double ClampOrigin(double origin, double boxSize, double pageSize)
        {
            if (boxSize >= pageSize) return (pageSize - boxSize) / 2.0;
            return Math.Clamp(origin, 0.0, pageSize - boxSize);
        }

        /// <summary>在合并结果的所有页上叠加水印。图片文件临时写入 tempDir，需在保存 PDF 之后再删除目录。</summary>
        public static void Apply(PdfDocument document, WatermarkSpec spec, string tempDir)
        {
            if (string.IsNullOrWhiteSpace(spec.Text) || document.PageCount == 0)
                return;
            if (spec.OpacityPercent <= 0)
                return;

            string imagePath = CreateTextImage(
                spec.Text, TintColor(spec.Tint, spec.OpacityPercent), tempDir, spec.FontFamily);
            using XImage image = XImage.FromFile(imagePath);

            foreach (PdfPage page in document.Pages)
                DrawOnPage(page, image, spec);
        }

        /// <summary>在栅格图片（PDF 转图片的输出）上叠加水印；只修改内存中的输出图，不触碰源 PDF。</summary>
        public static void ApplyToBitmap(Bitmap bitmap, WatermarkSpec spec)
        {
            if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0) return;
            if (string.IsNullOrWhiteSpace(spec.Text) || spec.OpacityPercent <= 0) return;

            double targetWidth = TargetWidth(bitmap.Width, spec);
            using Bitmap textImage = CreateTextBitmap(
                spec.Text, TintColor(spec.Tint, spec.OpacityPercent), targetWidth, spec.FontFamily);

            double targetHeight = targetWidth * textImage.Height / textImage.Width;
            double rotation = RotationOf(spec);
            double shiftX = HorizontalShift(bitmap.Width, spec);
            double shiftY = VerticalShift(bitmap.Height, spec);

            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingMode = CompositingMode.SourceOver;

            if (spec.Anchor != WatermarkAnchor.Tile)
            {
                double fit = FitFactor(bitmap.Width, bitmap.Height, targetWidth, targetHeight, rotation);
                double width = targetWidth * fit;
                double height = targetHeight * fit;
                (double boxWidth, double boxHeight) = RotatedBox(width, height, rotation);

                (double anchorX, double anchorY) = AnchorOrigin(
                    bitmap.Width, bitmap.Height, boxWidth, boxHeight, spec.Anchor);
                double x = ClampOrigin(anchorX + shiftX, boxWidth, bitmap.Width);
                double y = ClampOrigin(anchorY + shiftY, boxHeight, bitmap.Height);

                GraphicsState state = graphics.Save();
                graphics.TranslateTransform(
                    (float)(x + boxWidth / 2.0), (float)(y + boxHeight / 2.0));
                graphics.RotateTransform((float)rotation);
                graphics.DrawImage(textImage,
                    (float)(-width / 2.0), (float)(-height / 2.0),
                    (float)width, (float)height);
                graphics.Restore(state);
                return;
            }

            double span = Math.Sqrt(bitmap.Width * bitmap.Width + bitmap.Height * bitmap.Height);
            GraphicsState tileState = graphics.Save();
            graphics.TranslateTransform(
                (float)(bitmap.Width / 2.0 + shiftX), (float)(bitmap.Height / 2.0 + shiftY));
            graphics.RotateTransform((float)rotation);
            double stepX = targetWidth * 1.35;
            double stepY = targetHeight * 1.7;
            int rowIndex = 0;
            for (double y = -span; y < span; y += stepY)
            {
                double rowShift = (rowIndex % 2 == 0) ? 0 : stepX / 2;
                for (double x = -span - rowShift; x < span; x += stepX)
                    graphics.DrawImage(textImage,
                        (float)x, (float)y, (float)targetWidth, (float)targetHeight);
                rowIndex++;
            }
            graphics.Restore(tileState);
        }

        /// <summary>按目标宽度生成高分辨率文字贴图，避免放大 72px 基准图造成模糊。</summary>
        private static Bitmap CreateTextBitmap(string text, Color color, double targetWidth, string? fontFamily)
        {
            string family = NormalizeFontFamily(fontFamily);
            using var probe = new Bitmap(4, 4);
            using var measureGraphics = Graphics.FromImage(probe);
            SizeF reference = measureGraphics.MeasureString(text, ReferenceFontFor(family));
            double referenceWidth = Math.Max(1.0, reference.Width + 32.0);

            float fontSize = (float)Math.Max(2.0, FontPixelSize * targetWidth / referenceWidth);
            using var font = CreateFont(fontFamily, fontSize);
            SizeF textSize = measureGraphics.MeasureString(text, font);

            float ratio = fontSize / FontPixelSize;
            int padX = Math.Max(1, (int)Math.Round(16 * ratio));
            int padY = Math.Max(1, (int)Math.Round(12 * ratio));
            int width = Math.Max(8, (int)Math.Ceiling(textSize.Width) + padX * 2);
            int height = Math.Max(8, (int)Math.Ceiling(textSize.Height) + padY * 2);

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(color);
                graphics.DrawString(text, font, brush, padX, padY);
            }

            return bitmap;
        }

        private static string CreateTextImage(string text, Color color, string tempDir, string? fontFamily)
        {
            Directory.CreateDirectory(tempDir);

            using var font = CreateFont(fontFamily, FontPixelSize);
            using var probe = new Bitmap(4, 4);
            using var measureGraphics = Graphics.FromImage(probe);
            SizeF textSize = measureGraphics.MeasureString(text, font);

            int width = Math.Max(8, (int)Math.Ceiling(textSize.Width) + 32);
            int height = Math.Max(8, (int)Math.Ceiling(textSize.Height) + 32);

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(color);
                graphics.DrawString(text, font, brush, 16, 12);
            }

            string path = Path.Combine(tempDir, "watermark_" + Guid.NewGuid().ToString("N") + ".png");
            bitmap.Save(path, ImageFormat.Png);
            return path;
        }

        private static void DrawOnPage(PdfPage page, XImage image, WatermarkSpec spec)
        {
            double pageWidth = page.Width.Point;
            double pageHeight = page.Height.Point;

            double targetWidth = TargetWidth(pageWidth, spec);
            double targetHeight = targetWidth * image.PixelHeight / image.PixelWidth;
            double rotation = RotationOf(spec);
            double shiftX = HorizontalShift(pageWidth, spec);
            double shiftY = VerticalShift(pageHeight, spec);

            using var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

            if (spec.Anchor != WatermarkAnchor.Tile)
            {
                // 用旋转后的外框参与定位与限位：水印整体永远落在页面内，不会在阅读器里被页边裁掉
                double fit = FitFactor(pageWidth, pageHeight, targetWidth, targetHeight, rotation);
                double width = targetWidth * fit;
                double height = targetHeight * fit;
                (double boxWidth, double boxHeight) = RotatedBox(width, height, rotation);

                (double anchorX, double anchorY) = AnchorOrigin(
                    pageWidth, pageHeight, boxWidth, boxHeight, spec.Anchor);
                double x = ClampOrigin(anchorX + shiftX, boxWidth, pageWidth);
                double y = ClampOrigin(anchorY + shiftY, boxHeight, pageHeight);

                gfx.Save();
                gfx.TranslateTransform(x + boxWidth / 2, y + boxHeight / 2);
                gfx.RotateAtTransform(rotation, new XPoint(0, 0));
                gfx.DrawImage(image, -width / 2, -height / 2, width, height);
                gfx.Restore();
                return;
            }

            // 斜纹平铺：以页面中心为原点旋转后向四周铺满，位置偏移同样按页面比例换算
            double span = Math.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
            gfx.Save();
            gfx.TranslateTransform(pageWidth / 2 + shiftX, pageHeight / 2 + shiftY);
            gfx.RotateAtTransform(rotation, new XPoint(0, 0));
            double stepX = targetWidth * 1.35;
            double stepY = targetHeight * 1.7;
            int rowIndex = 0;
            for (double y = -span; y < span; y += stepY)
            {
                double rowShift = (rowIndex % 2 == 0) ? 0 : stepX / 2;
                for (double x = -span - rowShift; x < span; x += stepX)
                    gfx.DrawImage(image, x, y, targetWidth, targetHeight);
                rowIndex++;
            }
            gfx.Restore();
        }

        /// <summary>在设置页的预览框中绘制效果示意（GDI+，与 PDF 输出观感一致）。</summary>
        public static void RenderPreview(Graphics graphics, Rectangle bounds, WatermarkSpec spec)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color color = TintColor(spec.Tint, spec.OpacityPercent);
            string text = string.IsNullOrWhiteSpace(spec.Text) ? " " : spec.Text;

            // 按与 PDF 输出相同的「图片宽度占比 × 大小倍率」换算字号
            string family = NormalizeFontFamily(spec.FontFamily);
            float referenceWidth = ReferenceWidth(text, family);
            float targetWidth = (float)TargetWidth(bounds.Width, spec);
            float fontSize = Math.Max(6f, FontPixelSize * targetWidth / Math.Max(1f, referenceWidth));
            Font font = PreviewFont(family, text, fontSize);

            float rotation = (float)RotationOf(spec);
            float shiftX = (float)HorizontalShift(bounds.Width, spec);
            float shiftY = (float)VerticalShift(bounds.Height, spec);

            if (spec.Anchor != WatermarkAnchor.Tile)
            {
                SizeF anchorText = graphics.MeasureString(text, font);
                double naturalWidth = Math.Max(4.0, anchorText.Width);
                double naturalHeight = Math.Max(4.0, anchorText.Height);

                // 与 PDF 输出同一套算法：先按页面把水印缩到可见范围内，再把位置夹在纸张内
                double fit = FitFactor(bounds.Width, bounds.Height, naturalWidth, naturalHeight, rotation);
                double width = naturalWidth * fit;
                double height = naturalHeight * fit;
                (double boxWidth, double boxHeight) = RotatedBox(width, height, rotation);

                (double anchorX, double anchorY) = AnchorOrigin(
                    bounds.Width, bounds.Height, boxWidth, boxHeight, spec.Anchor);
                double x = ClampOrigin(anchorX + shiftX, boxWidth, bounds.Width);
                double y = ClampOrigin(anchorY + shiftY, boxHeight, bounds.Height);

                using var brush = new SolidBrush(color);
                GraphicsState state = graphics.Save();

                // 裁到纸张范围内：拖动/放大时水印不会画到预览框外的留白上
                graphics.SetClip(bounds, CombineMode.Intersect);
                graphics.TranslateTransform(
                    bounds.X + (float)(x + boxWidth / 2.0),
                    bounds.Y + (float)(y + boxHeight / 2.0));
                graphics.RotateTransform(rotation);
                graphics.ScaleTransform((float)fit, (float)fit);
                graphics.DrawString(text, font, brush, -anchorText.Width / 2f, -anchorText.Height / 2f);
                graphics.Restore(state);
                return;
            }

            // 平铺：预先画好「白底 + 水印」的整块图层，拖动时只按周期平移拷贝。
            // 原来按网格逐格 DrawString，缩到 10% 时每帧上千次排版、近 190ms；
            // 换成不透明图层 + SourceCopy 拷贝后，拖动每帧 0.2ms 左右。
            SizeF textSize = graphics.MeasureString(text, font);
            int imageWidth = Math.Max(8, (int)Math.Ceiling(textSize.Width) + 32);
            int imageHeight = Math.Max(8, (int)Math.Ceiling(textSize.Height) + 32);

            TiledLayer layer = PreviewLayer(text, family, font, color,
                imageWidth, imageHeight, rotation, bounds.Width, bounds.Height);

            int dx = Mod((int)Math.Round(bounds.Width / 2f + shiftX), layer.TileWidth);
            int dy = Mod((int)Math.Round(bounds.Height / 2f + shiftY), layer.TileHeight);
            var source = new Rectangle(layer.TileWidth - dx, layer.TileHeight - dy,
                bounds.Width, bounds.Height);

            CompositingMode previousMode = graphics.CompositingMode;
            GraphicsState tileState = graphics.Save();
            graphics.SetClip(bounds, CombineMode.Intersect);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.DrawImage(layer.Bitmap, bounds, source, GraphicsUnit.Pixel);
            graphics.CompositingMode = previousMode;
            graphics.Restore(tileState);
        }

        private static int Mod(int value, int period)
        {
            if (period <= 0) return 0;
            int result = value % period;
            return result < 0 ? result + period : result;
        }

        /// <summary>与 PDF 输出一致的贴图几何：文字四周留白，横向步长 1.35 倍、纵向 1.7 倍。</summary>
        private sealed class TiledLayer
        {
            public Bitmap Bitmap = null!;
            public int TileWidth;
            public int TileHeight;
        }

        /// <summary>平铺图层缓存：文字、颜色、字号、角度与预览尺寸不变时直接复用。</summary>
        private static readonly Dictionary<string, TiledLayer> LayerCache = new(StringComparer.Ordinal);

        private static TiledLayer PreviewLayer(string text, string family, Font font, Color color,
            int imageWidth, int imageHeight, float rotation, int width, int height)
        {
            if (width <= 0 || height <= 0) width = height = 1;

            int tileWidth = Math.Max(4, (int)Math.Ceiling(imageWidth * 1.35));
            int tileHeight = Math.Max(4, (int)Math.Ceiling(imageHeight * 1.7) * 2);

            string key = string.Join("|", family, text, color.ToArgb().ToString(),
                tileWidth.ToString(), tileHeight.ToString(),
                width.ToString(), height.ToString(),
                rotation.ToString("F1"));

            lock (CacheLock)
            {
                if (LayerCache.TryGetValue(key, out TiledLayer? cached))
                    return cached;
            }

            Bitmap tile = PreviewTile(text, family, font, color, imageWidth, imageHeight, tileWidth, tileHeight);

            // 尺寸多留一个周期，保证任意相位平移后都能铺满预览区
            var layer = new TiledLayer
            {
                Bitmap = new Bitmap(width + tileWidth, height + tileHeight, PixelFormat.Format32bppPArgb),
                TileWidth = tileWidth,
                TileHeight = tileHeight
            };

            using (var g = Graphics.FromImage(layer.Bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using var brush = new TextureBrush(tile, WrapMode.Tile);
                brush.TranslateTransform(tileWidth, tileHeight);
                brush.RotateTransform(rotation);
                g.FillRectangle(brush, 0, 0, layer.Bitmap.Width, layer.Bitmap.Height);
            }

            lock (CacheLock)
            {
                if (LayerCache.Count >= 6)
                {
                    var stale = new List<string>(LayerCache.Keys);
                    for (int i = 0; i < stale.Count - 2 && i < stale.Count; i++)
                    {
                        LayerCache[stale[i]].Bitmap.Dispose();
                        LayerCache.Remove(stale[i]);
                    }
                }

                LayerCache[key] = layer;
            }

            return layer;
        }

        /// <summary>预览平铺贴图缓存：文字、颜色、字号不变时直接复用上一次的贴图。</summary>
        private static readonly Dictionary<string, Bitmap> TileCache = new(StringComparer.Ordinal);

        private static Bitmap PreviewTile(string text, string family, Font font, Color color,
            int imageWidth, int imageHeight, int tileWidth, int tileHeight)
        {
            string key = string.Join("|", family, text, color.ToArgb().ToString(),
                imageWidth.ToString(), imageHeight.ToString(),
                tileWidth.ToString(), tileHeight.ToString());

            lock (CacheLock)
            {
                if (TileCache.TryGetValue(key, out Bitmap? cached))
                    return cached;
            }

            var tile = new Bitmap(tileWidth, tileHeight, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(tile))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(color);

                // 第 1 行
                g.DrawString(text, font, brush, 16f, 12f);
                // 第 2 行右移半格：右侧超出的部分在左侧补画一次，保证平铺无缝
                float row2Y = 12f + tileHeight / 2f;
                g.DrawString(text, font, brush, 16f + tileWidth / 2f, row2Y);
                g.DrawString(text, font, brush, 16f + tileWidth / 2f - tileWidth, row2Y);
            }

            lock (CacheLock)
            {
                if (TileCache.Count >= 48)
                {
                    // 一次清掉一批旧条目，避免每帧都做淘汰判断
                    var stale = new List<string>(TileCache.Keys);
                    for (int i = 0; i < stale.Count - 32 && i < stale.Count; i++)
                    {
                        TileCache[stale[i]].Dispose();
                        TileCache.Remove(stale[i]);
                    }
                }

                TileCache[key] = tile;
            }

            return tile;
        }

        /// <summary>同一段文字的基准宽度只量一次；拖动滑条时省掉重复 MeasureString。</summary>
        private static float ReferenceWidth(string text, string family)
        {
            string key = family + "|" + text;
            lock (CacheLock)
            {
                if (ReferenceWidthCache.TryGetValue(key, out float cached))
                    return cached;
            }

            float width;
            using (var probe = new Bitmap(4, 4))
            using (Graphics g = Graphics.FromImage(probe))
                width = g.MeasureString(text, ReferenceFontFor(family)).Width + 32;

            lock (CacheLock)
            {
                if (ReferenceWidthCache.Count > 24) ReferenceWidthCache.Clear();
                ReferenceWidthCache[key] = width;
            }

            return width;
        }

        /// <summary>按字体族缓存 72px 基准字体，用于测量文字基准宽度。</summary>
        private static Font ReferenceFontFor(string family)
        {
            lock (CacheLock)
            {
                if (ReferenceFonts.TryGetValue(family, out Font? cached))
                    return cached;
            }

            Font font = CreateFont(family, FontPixelSize);
            lock (CacheLock)
            {
                if (ReferenceFonts.Count >= 8)
                {
                    var stale = new List<string>(ReferenceFonts.Keys);
                    for (int i = 0; i < stale.Count - 4 && i < stale.Count; i++)
                    {
                        ReferenceFonts[stale[i]].Dispose();
                        ReferenceFonts.Remove(stale[i]);
                    }
                }
                ReferenceFonts[family] = font;
            }
            return font;
        }

        /// <summary>按字号复用字体对象：字号与字体族不变时不再新建 Font（预览重绘的主要开销之一）。</summary>
        private static Font PreviewFont(string family, string text, float fontSize)
        {
            int keySize = (int)Math.Round(fontSize * 4);
            string key = family + "|" + keySize + "|" + text.Length;
            if (_previewFont != null && _previewFontKey == key)
                return _previewFont;

            _previewFont?.Dispose();
            _previewFont = CreateFont(family, keySize / 4f);
            _previewFontKey = key;
            return _previewFont;
        }
    }
}
