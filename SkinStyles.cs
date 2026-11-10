using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DocHub
{
    /// <summary>
    /// 三种新增皮肤的表面细节：拟物斜面、新拟态双向阴影、终端扫描线。
    /// 玻璃系列皮肤不经过这里，保持原有磨砂渲染。
    /// </summary>
    internal static class SkinStyles
    {
        public static void PaintWallpaperOverlay(Graphics g, Size size, SkinPalette palette)
        {
            int w = Math.Max(1, size.Width);
            int h = Math.Max(1, size.Height);

            switch (palette.Surface)
            {
                case SkinSurface.Neumorphic:
                {
                    using var glow = new LinearGradientBrush(
                        new Rectangle(0, 0, w, h),
                        Color.FromArgb(42, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255), 115f);
                    g.FillRectangle(glow, 0, 0, w, h);
                    break;
                }
                case SkinSurface.Skeuomorphic:
                {
                    using var fiber = new Pen(Color.FromArgb(7, 110, 78, 44), 1f);
                    for (int y = 0; y < h; y += 4) g.DrawLine(fiber, 0, y + 0.5f, w, y + 0.5f);
                    break;
                }
            }
        }

        public static void PaintCard(Graphics g, GraphicsPath path, RectangleF rect, float radius)
        {
            switch (Theme.Surface)
            {
                case SkinSurface.Skeuomorphic:
                    PaintSkeuomorphicCard(g, path, rect, radius);
                    break;
                case SkinSurface.Neumorphic:
                    PaintNeumorphicCard(g, path);
                    break;
            }
        }

        public static void PaintButton(Graphics g, GraphicsPath path, RectangleF rect,
            ButtonRole role, bool pressed, bool enabled)
        {
            if (!enabled) return;

            switch (Theme.Surface)
            {
                case SkinSurface.Skeuomorphic:
                    using (var gloss = new LinearGradientBrush(rect,
                        Color.FromArgb(pressed ? 20 : 70, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255), 90f))
                        g.FillPath(gloss, path);
                    using (var light = new Pen(Color.FromArgb(150, 255, 255, 255), 1.2f))
                        g.DrawLine(light, rect.Left + 6, rect.Top + 1.2f, rect.Right - 6, rect.Top + 1.2f);
                    using (var shade = new Pen(Color.FromArgb(55, 80, 55, 35), 1.4f))
                        g.DrawLine(shade, rect.Left + 6, rect.Bottom - 1.2f, rect.Right - 6, rect.Bottom - 1.2f);
                    break;

                case SkinSurface.Neumorphic:
                {
                    float offset = pressed ? 1.5f : -1.5f;
                    Color light = pressed
                        ? Color.FromArgb(35, 255, 255, 255)
                        : Color.FromArgb(120, 255, 255, 255);
                    Color dark = Color.FromArgb(pressed ? 70 : 55, 132, 148, 170);
                    using (var lightPath = OffsetPath(path, offset, offset))
                    using (var pen = new Pen(light, 2f))
                        g.DrawPath(pen, lightPath);
                    using (var darkPath = OffsetPath(path, -offset, -offset))
                    using (var pen = new Pen(dark, 2f))
                        g.DrawPath(pen, darkPath);
                    break;
                }
            }
        }

        public static void PaintInput(Graphics g, GraphicsPath path, RectangleF rect, bool focused)
        {
            switch (Theme.Surface)
            {
                case SkinSurface.Skeuomorphic:
                    using (var dark = new Pen(Color.FromArgb(60, 110, 82, 52), 1.4f))
                    {
                        g.DrawLine(dark, rect.Left + 4, rect.Top + 1.2f, rect.Right - 4, rect.Top + 1.2f);
                        g.DrawLine(dark, rect.Left + 1.2f, rect.Top + 4, rect.Left + 1.2f, rect.Bottom - 4);
                    }
                    using (var light = new Pen(Color.FromArgb(180, 255, 255, 255), 1.2f))
                    {
                        g.DrawLine(light, rect.Left + 4, rect.Bottom - 1.2f, rect.Right - 4, rect.Bottom - 1.2f);
                        g.DrawLine(light, rect.Right - 1.2f, rect.Top + 4, rect.Right - 1.2f, rect.Bottom - 4);
                    }
                    break;

                case SkinSurface.Neumorphic:
                    using (var dark = new Pen(Color.FromArgb(55, 130, 146, 168), 1.6f))
                    {
                        g.DrawLine(dark, rect.Left + 5, rect.Top + 1.3f, rect.Right - 5, rect.Top + 1.3f);
                        g.DrawLine(dark, rect.Left + 1.3f, rect.Top + 5, rect.Left + 1.3f, rect.Bottom - 5);
                    }
                    using (var light = new Pen(Color.FromArgb(150, 255, 255, 255), 1.4f))
                    {
                        g.DrawLine(light, rect.Left + 5, rect.Bottom - 1.3f, rect.Right - 5, rect.Bottom - 1.3f);
                        g.DrawLine(light, rect.Right - 1.3f, rect.Top + 5, rect.Right - 1.3f, rect.Bottom - 5);
                    }
                    break;
            }
        }

        public static void PaintSkinPreview(Graphics g, GraphicsPath headerPath,
            RectangleF header, SkinPalette palette)
        {
            g.SetClip(headerPath);
            using (var gradient = new LinearGradientBrush(
                header, palette.HeaderStart, palette.HeaderEnd, 0f))
                g.FillRectangle(gradient, header);

            switch (palette.Surface)
            {
                case SkinSurface.Neumorphic:
                    using (var light = new Pen(Color.FromArgb(150, 255, 255, 255), 2f))
                        g.DrawLine(light, header.Left + 8, header.Top + 1.5f, header.Right - 8, header.Top + 1.5f);
                    using (var dark = new Pen(Color.FromArgb(45, 120, 136, 158), 2f))
                        g.DrawLine(dark, header.Left + 8, header.Bottom - 1.5f, header.Right - 8, header.Bottom - 1.5f);
                    break;

                case SkinSurface.Skeuomorphic:
                    using (var gloss = new LinearGradientBrush(header,
                        Color.FromArgb(60, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), 90f))
                        g.FillRectangle(gloss, header);
                    using (var shade = new Pen(Color.FromArgb(60, 60, 40, 25), 1.5f))
                        g.DrawLine(shade, header.Left + 8, header.Bottom - 1.5f, header.Right - 8, header.Bottom - 1.5f);
                    break;
            }

            g.ResetClip();
        }

        private static GraphicsPath OffsetPath(GraphicsPath path, float dx, float dy)
        {
            var clone = (GraphicsPath)path.Clone();
            using var matrix = new Matrix();
            matrix.Translate(dx, dy);
            clone.Transform(matrix);
            return clone;
        }

        private static void PaintSkeuomorphicCard(Graphics g, GraphicsPath path,
            RectangleF rect, float radius)
        {
            using (var shade = new LinearGradientBrush(rect,
                Color.FromArgb(34, 255, 255, 255), Color.FromArgb(22, 70, 45, 25), 90f))
                g.FillPath(shade, path);

            float inset = Math.Min(radius, rect.Width / 4f);
            using (var light = new Pen(Color.FromArgb(170, 255, 255, 255), 1.2f))
                g.DrawLine(light, rect.Left + inset, rect.Top + 1.2f,
                    rect.Right - inset, rect.Top + 1.2f);
            using (var dark = new Pen(Color.FromArgb(70, 80, 55, 35), 1.4f))
                g.DrawLine(dark, rect.Left + inset, rect.Bottom - 1.2f,
                    rect.Right - inset, rect.Bottom - 1.2f);
            using (var border = new Pen(Theme.Current.Border, 1f))
                g.DrawPath(border, path);
        }

        private static void PaintNeumorphicCard(Graphics g, GraphicsPath path)
        {
            using (var lightPath = OffsetPath(path, -2.5f, -2.5f))
            using (var light = new Pen(Color.FromArgb(115, 255, 255, 255), 3f))
                g.DrawPath(light, lightPath);

            using (var darkPath = OffsetPath(path, 2.5f, 2.5f))
            using (var dark = new Pen(Color.FromArgb(58, 128, 144, 166), 3f))
                g.DrawPath(dark, darkPath);
        }
    }
}
