using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>
    /// 由窗体实现，向子控件提供本窗口的背景位图：
    /// SharpBackdrop 为清晰的渐变光斑，FrostedBackdrop 为同尺寸的高斯模糊版本。
    /// </summary>
    public interface IGlassScene
    {
        Bitmap? SharpBackdrop { get; }
        Bitmap? FrostedBackdrop { get; }
    }

    /// <summary>
    /// 应用内磨砂玻璃渲染。窗口底层是渐变光斑壁纸，面板层取同一坐标的模糊版本，
    /// 再叠加半透明色与 1px 高光描边，从而得到毛玻璃观感。
    /// 不依赖系统 Acrylic / Mica，Windows 10、Windows 11 与所有皮肤都能一致工作。
    /// </summary>
    public static class GlassPaint
    {
        /// <summary>模糊降采样倍率：越大越糊，9 时接近亚克力的观感。</summary>
        private const int FrostFactor = 9;

        /// <summary>生成壁纸：底色渐变 + 三团柔光，颜色全部来自当前皮肤。</summary>
        public static Bitmap BuildWallpaper(Size size, SkinPalette palette)
        {
            int w = Math.Max(1, size.Width);
            int h = Math.Max(1, size.Height);

            var bitmap = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var bounds = new Rectangle(0, 0, w, h);
                using (var background = new LinearGradientBrush(
                    bounds, palette.WallpaperStart, palette.WallpaperEnd, 58f))
                    g.FillRectangle(background, bounds);

                // 右上主光团
                Blob(g, new RectangleF(w * 0.52f, -h * 0.30f, w * 0.95f, h * 0.95f), palette.WallpaperGlowA);
                // 左下副光团
                Blob(g, new RectangleF(-w * 0.25f, h * 0.42f, w * 0.85f, h * 0.90f), palette.WallpaperGlowB);
                // 顶部偏左的冷色补光
                Blob(g, new RectangleF(w * 0.10f, -h * 0.22f, w * 0.52f, h * 0.60f), palette.WallpaperGlowC);

                // 顶部一层极淡的白色高光，让玻璃边缘有反光感
                if (palette.Surface is SkinSurface.Glass or SkinSurface.Skeuomorphic)
                {
                    using var sheen = new LinearGradientBrush(
                        new Rectangle(0, 0, w, Math.Max(1, h / 2)),
                        Color.FromArgb(26, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        90f);
                    g.FillRectangle(sheen, 0, 0, w, Math.Max(1, h / 2));
                }

                SkinStyles.PaintWallpaperOverlay(g, new Size(w, h), palette);
            }

            return bitmap;
        }

        /// <summary>降采样后用双三次插值放大，得到平滑的高斯模糊近似（毫秒级）。</summary>
        public static Bitmap Frost(Bitmap source)
        {
            int smallW = Math.Max(1, source.Width / FrostFactor);
            int smallH = Math.Max(1, source.Height / FrostFactor);

            using var small = new Bitmap(smallW, smallH, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(small))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(source, new Rectangle(0, 0, smallW, smallH));
            }

            var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(small, new Rectangle(0, 0, result.Width, result.Height));
            }

            return result;
        }

        /// <summary>一份壁纸：清晰渐变 + 磨砂版本，内容由皮肤与尺寸唯一确定。</summary>
        public sealed class Backdrop : IDisposable
        {
            internal Backdrop(Bitmap sharp, Bitmap frosted)
            {
                Sharp = sharp;
                Frosted = frosted;
            }

            public Bitmap Sharp { get; }
            public Bitmap Frosted { get; }

            public void Dispose()
            {
                Sharp.Dispose();
                Frosted.Dispose();
            }
        }

        // 弹窗每次打开都要重算壁纸（约 20ms）与模糊（约 45ms），这是点开设置时的主要卡顿来源。
        // 壁纸只由「皮肤 + 尺寸」决定，缓存之后第二次打开几乎零成本；组合有限，最多保留 4 份。
        private const int BackdropCacheLimit = 4;
        private static readonly object BackdropGate = new();
        private static readonly List<KeyValuePair<string, Backdrop>> BackdropCache = new();

        private static string BackdropKey(Size size, SkinPalette palette)
        {
            return palette.Id + "@" + size.Width + "x" + size.Height;
        }

        /// <summary>位图可能已被误释放：访问尺寸会抛 ArgumentException，此时应重建而不是继续使用。</summary>
        private static bool IsUsable(Bitmap? bitmap)
        {
            if (bitmap == null) return false;

            try
            {
                return bitmap.Width > 0 && bitmap.Height > 0;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static bool IsUsable(Backdrop backdrop)
            => IsUsable(backdrop.Sharp) && IsUsable(backdrop.Frosted);

        /// <summary>按皮肤与尺寸取（或构建）壁纸缓存。返回的位图由缓存持有，调用方不要释放。</summary>
        public static Backdrop GetBackdrop(Size size, SkinPalette palette)
        {
            int width = Math.Max(1, size.Width);
            int height = Math.Max(1, size.Height);
            string key = BackdropKey(new Size(width, height), palette);

            lock (BackdropGate)
            {
                for (int i = 0; i < BackdropCache.Count; i++)
                {
                    if (BackdropCache[i].Key != key) continue;

                    Backdrop hit = BackdropCache[i].Value;
                    BackdropCache.RemoveAt(i);

                    // 缓存里的位图若被外部误释放（例如窗口重建时直接 Dispose），
                    // 继续返回会让所有玻璃控件的绘制抛 “Parameter is not valid”，
                    // 表现为整窗红叉。这里丢弃坏条目并重新生成。
                    if (IsUsable(hit))
                    {
                        // 命中后移到末尾，保持「最近使用」顺序
                        BackdropCache.Add(new KeyValuePair<string, Backdrop>(key, hit));
                        return hit;
                    }

                    hit.Dispose();
                    break;
                }

                Bitmap sharp = BuildWallpaper(new Size(width, height), palette);
                Backdrop created;
                try
                {
                    created = new Backdrop(sharp, Frost(sharp));
                }
                catch
                {
                    sharp.Dispose();
                    throw;
                }

                BackdropCache.Add(new KeyValuePair<string, Backdrop>(key, created));
                while (BackdropCache.Count > BackdropCacheLimit)
                {
                    Backdrop oldest = BackdropCache[0].Value;
                    BackdropCache.RemoveAt(0);
                    oldest.Dispose();
                }

                return created;
            }
        }

        /// <summary>在后台线程预先生成弹窗壁纸，让第一次点开设置也是秒开。</summary>
        public static void PrewarmBackdrop(Size size, SkinPalette palette)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    GetBackdrop(size, palette);
                }
                catch
                {
                    // 预生成失败时不影响正常流程，真正打开窗口时会再构建一次
                }
            });
        }

        /// <summary>把窗体背景（sharp = true）或磨砂层画到指定控件的画布上，并叠加 tint。</summary>
        public static void Paint(Control control, Graphics g, Color tint, bool sharp = false)
        {
            Rectangle client = control.ClientRectangle;
            if (client.Width <= 0 || client.Height <= 0) return;

            Color fallback = sharp ? Theme.Background : Theme.Card;
            Form? form = control.FindForm();
            Bitmap? bitmap = form is IGlassScene scene
                ? (sharp ? scene.SharpBackdrop : scene.FrostedBackdrop)
                : null;

            if (form == null || bitmap == null || !IsUsable(bitmap))
            {
                using var solid = new SolidBrush(fallback);
                g.FillRectangle(solid, client);
                return;
            }

            using (var solid = new SolidBrush(fallback))
                g.FillRectangle(solid, client);

            Rectangle controlScreen = control.RectangleToScreen(client);
            Rectangle formScreen = form.RectangleToScreen(form.ClientRectangle);
            int offsetX = controlScreen.X - formScreen.X;
            int offsetY = controlScreen.Y - formScreen.Y;

            var source = new Rectangle(offsetX, offsetY, client.Width, client.Height);
            Rectangle valid = Rectangle.Intersect(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), source);

            if (valid.Width > 0 && valid.Height > 0)
            {
                var destination = new Rectangle(
                    valid.X - source.X, valid.Y - source.Y, valid.Width, valid.Height);

                InterpolationMode previous = g.InterpolationMode;
                PixelOffsetMode previousOffset = g.PixelOffsetMode;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(bitmap, destination, valid, GraphicsUnit.Pixel);
                g.InterpolationMode = previous;
                g.PixelOffsetMode = previousOffset;
            }

            if (tint.A > 0)
            {
                using var overlay = new SolidBrush(tint);
                g.FillRectangle(overlay, client);
            }
        }

        /// <summary>取控件所在区域的磨砂平均色（叠加 tint 后不透明），供原生 ListBox、TextBox 使用。</summary>
        public static Color SampleOpaque(Control control, Color tint)
        {
            Color fallback = Theme.Card;
            Form? form = control.FindForm();
            if (form is not IGlassScene scene || scene.FrostedBackdrop is not Bitmap bitmap)
                return fallback;

            Rectangle controlScreen = control.RectangleToScreen(control.ClientRectangle);
            Rectangle formScreen = form.RectangleToScreen(form.ClientRectangle);
            int left = Math.Max(0, controlScreen.X - formScreen.X);
            int top = Math.Max(0, controlScreen.Y - formScreen.Y);
            int right = Math.Min(bitmap.Width, left + Math.Max(1, control.Width));
            int bottom = Math.Min(bitmap.Height, top + Math.Max(1, control.Height));
            if (right <= left || bottom <= top) return fallback;

            int stepX = Math.Max(1, (right - left) / 20);
            int stepY = Math.Max(1, (bottom - top) / 20);

            long red = 0, green = 0, blue = 0;
            int count = 0;
            for (int y = top; y < bottom; y += stepY)
            {
                for (int x = left; x < right; x += stepX)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    red += pixel.R;
                    green += pixel.G;
                    blue += pixel.B;
                    count++;
                }
            }

            if (count == 0) return fallback;
            var average = Color.FromArgb(255, (int)(red / count), (int)(green / count), (int)(blue / count));
            return Blend(average, tint);
        }

        /// <summary>alpha 混合：over 覆盖在 under 上，返回不透明色。</summary>
        public static Color Blend(Color under, Color over)
        {
            if (over.A >= 255) return Color.FromArgb(255, over);
            float alpha = over.A / 255f;
            return Color.FromArgb(
                255,
                (int)Math.Round(over.R * alpha + under.R * (1 - alpha)),
                (int)Math.Round(over.G * alpha + under.G * (1 - alpha)),
                (int)Math.Round(over.B * alpha + under.B * (1 - alpha)));
        }

        /// <summary>绘制一团柔光（中心有色、边缘透明的椭圆）。</summary>
        private static void Blob(Graphics g, RectangleF rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || color.A == 0) return;

            using var path = new GraphicsPath();
            path.AddEllipse(rect);

            var surround = new Color[path.PointCount];
            for (int i = 0; i < surround.Length; i++)
                surround[i] = Color.FromArgb(0, color);

            using var brush = new PathGradientBrush(path)
            {
                CenterColor = color,
                SurroundColors = surround
            };
            g.FillPath(brush, path);
        }

        /// <summary>标题栏跟随皮肤深浅、窗口描边圆角（Windows 11 生效，其余系统静默忽略）。</summary>
        public static void ApplyWindowChrome(Form form, bool dark)
        {
            if (form == null || !form.IsHandleCreated) return;

            try
            {
                int useDark = dark ? 1 : 0;
                DwmSetWindowAttribute(form.Handle, DwmwaUseImmersiveDarkMode, ref useDark, sizeof(int));

                int corner = DwmwcpRound;
                DwmSetWindowAttribute(form.Handle, DwmwaWindowCornerPreference, ref corner, sizeof(int));
            }
            catch
            {
                // 旧系统没有这些 DWM 属性时保持系统默认外观
            }
        }

        private const int DwmwaUseImmersiveDarkMode = 20;
        private const int DwmwaWindowCornerPreference = 33;
        private const int DwmwcpRound = 2;

        [DllImport("dwmapi.dll", ExactSpelling = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
    }
}
