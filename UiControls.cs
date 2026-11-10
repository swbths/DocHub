using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace DocHub
{
    public static class Theme
    {
        public static SkinPalette Current { get; private set; } = SkinPalette.ClassicBlue;

        public static void ApplySkin(SkinPalette palette)
        {
            Current = palette ?? SkinPalette.ClassicBlue;
        }

        public static bool IsDark => Current.IsDark;
        public static SkinSurface Surface => Current.Surface;

        /// <summary>按皮肤风格调整圆角：新拟态更柔和，拟物略收敛。</summary>
        public static float ControlRadius(float baseRadius)
        {
            if (baseRadius <= 0) return 0;
            return Current.Surface switch
            {
                SkinSurface.Neumorphic => baseRadius + 4f,
                SkinSurface.Skeuomorphic => Math.Max(2f, baseRadius - 2f),
                _ => baseRadius
            };
        }

        public static Color Background => Current.Background;
        public static Color Card => Current.Card;
        public static Color Border => Current.Border;
        public static Color TextPrimary => Current.TextPrimary;
        public static Color TextSecondary => Current.TextSecondary;
        public static Color TextMuted => Current.TextMuted;

        public static Color Accent => Current.Accent;
        public static Color AccentLight => Current.AccentLight;
        public static Color AccentHover => Current.AccentHover;
        public static Color AccentPressed => Current.AccentPressed;
        public static Color AccentText => Current.AccentText;

        public static Color InputFill => Current.InputFill;
        public static Color InputReadOnlyFill => Current.InputReadOnlyFill;
        public static Color ListHover => Current.ListHover;
        public static Color ListSelected => Current.ListSelected;
        public static Color Separator => Current.Separator;

        public static Color HeaderStart => Current.HeaderStart;
        public static Color HeaderEnd => Current.HeaderEnd;
        public static Color HeaderText => Current.HeaderText;
        public static Color HeaderSubtitle => Current.HeaderSubtitle;

        public static Color LogoStart => Current.LogoStart;
        public static Color LogoEnd => Current.LogoEnd;
        public static Color LogoText => Current.LogoText;

        public static Color ProgressTrack => Current.ProgressTrack;
        public static Color ProgressStart => Current.ProgressStart;

        // ---------- 磨砂玻璃 ----------
        public static Color GlassFill => Current.GlassFill;
        public static Color GlassStrong => Current.GlassStrong;
        public static Color GlassBorder => Current.GlassBorder;
        public static Color GlassEdge => Current.GlassEdge;
        public static Color GlassTint => Current.GlassTint;
        public static Color GlassHover => Current.GlassHover;
        public static Color GlassInput => Current.GlassInput;

        /// <summary>输入框等原生控件使用的不透明底色，用来近似玻璃。</summary>
        public static Color InputSurface => Current.GlassInput;

        private static string _fontFamily = "Microsoft YaHei UI";

        public static Font BaseFont { get; private set; } = new("Microsoft YaHei UI", 9f);
        public static Font TitleFont { get; private set; } = new("Microsoft YaHei UI", 21f, FontStyle.Bold);
        public static Font LogoFont { get; private set; } = new("Microsoft YaHei UI", 22f, FontStyle.Bold);
        public static Font SubtitleFont { get; private set; } = new("Microsoft YaHei UI", 9.5f);
        public static Font SectionFont { get; private set; } = new("Microsoft YaHei UI", 12f, FontStyle.Bold);
        public static Font ItemFont { get; private set; } = new("Microsoft YaHei UI", 10f);
        public static Font MetaFont { get; private set; } = new("Microsoft YaHei UI", 8.5f);
        public static Font BadgeFont { get; private set; } = new("Microsoft YaHei UI", 7.5f, FontStyle.Bold);
        public static Font InputFont { get; private set; } = new("Microsoft YaHei UI", 9.5f);
        public static Font ButtonFont { get; private set; } = new("Microsoft YaHei UI", 9.5f);
        public static Font PrimaryButtonFont { get; private set; } = new("Microsoft YaHei UI", 11f, FontStyle.Bold);
        public static Font SmallFont { get; private set; } = new("Microsoft YaHei UI", 8.5f);
        public static Font DialogTitleFont { get; private set; } = new("Microsoft YaHei UI", 14f, FontStyle.Bold);
        public static Font PercentFont { get; private set; } = new("Microsoft YaHei UI", 9f, FontStyle.Bold);

        /// <summary>按当前界面语言重建字体（拉丁/西里尔/韩文用 Segoe UI，中日文用微软雅黑）。</summary>
        public static void RefreshFonts()
        {
            _fontFamily = Loc.Current is Language.En or Language.Ru or Language.Ko
                ? "Segoe UI"
                : "Microsoft YaHei UI";

            BaseFont = new Font(_fontFamily, 9f);
            TitleFont = new Font(_fontFamily, 21f, FontStyle.Bold);
            LogoFont = new Font(_fontFamily, 22f, FontStyle.Bold);
            SubtitleFont = new Font(_fontFamily, 9.5f);
            SectionFont = new Font(_fontFamily, 12f, FontStyle.Bold);
            ItemFont = new Font(_fontFamily, 10f);
            MetaFont = new Font(_fontFamily, 8.5f);
            BadgeFont = new Font(_fontFamily, 7.5f, FontStyle.Bold);
            InputFont = new Font(_fontFamily, 9.5f);
            ButtonFont = new Font(_fontFamily, 9.5f);
            PrimaryButtonFont = new Font(_fontFamily, 11f, FontStyle.Bold);
            SmallFont = new Font(_fontFamily, 8.5f);
            DialogTitleFont = new Font(_fontFamily, 14f, FontStyle.Bold);
            PercentFont = new Font(_fontFamily, 9f, FontStyle.Bold);
        }

        public static GraphicsPath RoundedRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = Math.Max(1f, radius * 2f);
            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public enum ButtonRole
    {
        Primary,
        Secondary,
        Header,
        Ghost
    }

    public sealed class DocumentItem
    {
        public string Path { get; }
        public string FileName { get; }
        public string ExtensionLabel { get; }
        public bool IsPdf { get; }
        public bool IsWord { get; }
        public bool IsImage { get; }
        public string KindLabel { get; }
        public string SizeText { get; }
        public string FolderText { get; }

        /// <summary>PDF 页数；-1 表示尚未读取，-2 表示读取失败。</summary>
        public int PageCount { get; set; } = -1;

        public DocumentItem(string path)
        {
            Path = path;
            FileName = System.IO.Path.GetFileName(path);
            FolderText = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
            string extension = System.IO.Path.GetExtension(path).TrimStart('.');
            ExtensionLabel = extension.ToUpperInvariant();
            IsPdf = extension.Equals("pdf", StringComparison.OrdinalIgnoreCase);
            IsWord = MergeEngine.IsWordFile(path);
            IsImage = MergeEngine.IsImageFile(path);
            KindLabel = IsPdf ? "PDF" : IsImage ? "IMG" : "WORD";
            long length = 0;
            try { length = new FileInfo(path).Length; } catch { /* 文件可能在添加后不可读 */ }
            SizeText = FormatSize(length);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:0.#} KB";
            double mb = kb / 1024.0;
            if (mb < 1024) return $"{mb:0.#} MB";
            return $"{mb / 1024.0:0.##} GB";
        }
    }

    public class RoundedButton : Control
    {
        private bool _hover;
        private bool _pressed;

        public ButtonRole Role { get; set; } = ButtonRole.Secondary;
        public Color FillColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.White;
        public Color PressedColor { get; set; } = Color.White;
        public Color TextColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.Transparent;
        public Color DisabledFill { get; set; } = Color.White;
        public Color DisabledText { get; set; } = Color.White;
        public Color DisabledBorder { get; set; } = Color.White;
        public int Radius { get; set; } = 10;
        public bool ShowBorder { get; set; }

        /// <summary>按钮内可选的前置图标，与文字一起居中排布。</summary>
        public IconKind Icon { get; set; } = IconKind.None;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 7;

        public RoundedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = Theme.ButtonFont;
            Cursor = Cursors.Hand;
            TabStop = true;
            ApplyTheme();
        }

        public void ApplyTheme()
        {
            switch (Role)
            {
                case ButtonRole.Primary:
                    FillColor = Theme.Accent;
                    HoverColor = Theme.AccentHover;
                    PressedColor = Theme.AccentPressed;
                    TextColor = Theme.AccentText;
                    BorderColor = Color.Transparent;
                    DisabledFill = Theme.Current.PrimaryDisabledFill;
                    DisabledText = Theme.Current.PrimaryDisabledText;
                    DisabledBorder = Theme.Current.PrimaryDisabledFill;
                    ShowBorder = false;
                    break;
                case ButtonRole.Header:
                    FillColor = Theme.Current.HeaderButtonFill;
                    HoverColor = Theme.Current.HeaderButtonHover;
                    PressedColor = Theme.Current.HeaderButtonHover;
                    TextColor = Theme.Current.HeaderText;
                    BorderColor = Theme.Current.HeaderButtonBorder;
                    DisabledFill = Theme.Current.HeaderButtonFill;
                    DisabledText = Theme.Current.HeaderSubtitle;
                    DisabledBorder = Theme.Current.HeaderButtonBorder;
                    ShowBorder = true;
                    break;
                case ButtonRole.Ghost:
                    FillColor = Color.Transparent;
                    HoverColor = Theme.ListHover;
                    PressedColor = Theme.ListSelected;
                    TextColor = Theme.TextSecondary;
                    BorderColor = Color.Transparent;
                    DisabledFill = Color.Transparent;
                    DisabledText = Theme.Current.SecondaryDisabledText;
                    DisabledBorder = Color.Transparent;
                    ShowBorder = false;
                    break;
                default:
                    FillColor = Theme.Current.SecondaryFill;
                    HoverColor = Theme.Current.SecondaryHover;
                    PressedColor = Theme.Current.SecondaryPressed;
                    TextColor = Theme.Current.SecondaryText;
                    BorderColor = Theme.Border;
                    DisabledFill = Theme.Current.SecondaryFill;
                    DisabledText = Theme.Current.SecondaryDisabledText;
                    DisabledBorder = Theme.Current.SecondaryDisabledBorder;
                    ShowBorder = true;
                    break;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fill = !Enabled ? DisabledFill : _pressed ? PressedColor : _hover ? HoverColor : FillColor;
            Color text = !Enabled ? DisabledText : TextColor;
            Color border = !Enabled ? DisabledBorder : BorderColor;

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath path = Theme.RoundedRect(rect, Theme.ControlRadius(Radius));
            if (fill.A > 0)
            {
                using var brush = new SolidBrush(fill);
                g.FillPath(brush, path);
            }

            if ((ShowBorder || border.A > 0) && Theme.Surface != SkinSurface.Neumorphic)
            {
                using var pen = new Pen(border, 1f);
                g.DrawPath(pen, path);
            }

            SkinStyles.PaintButton(g, path, rect, Role, _pressed, Enabled);

            if (Icon == IconKind.None)
            {
                TextRenderer.DrawText(
                    g, Text, Font,
                    new Rectangle(0, 0, Width, Height), text,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
            else
            {
                int textWidth = TextRenderer.MeasureText(Text, Font, Size.Empty,
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix).Width;
                int total = IconSize + IconGap + textWidth;
                int left = Math.Max(6, (Width - total) / 2);

                IconPainter.Draw(g, Icon,
                    new RectangleF(left, (Height - IconSize) / 2f, IconSize, IconSize), text, 2.1f);

                TextRenderer.DrawText(
                    g, Text, Font,
                    new Rectangle(left + IconSize + IconGap, 0,
                        Math.Max(1, Width - left - IconSize - IconGap), Height), text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            _pressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    public class CardPanel : Panel
    {
        public int Radius { get; set; } = 16;

        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Card;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (Width <= 0 || Height <= 0) return;
            using GraphicsPath path = Theme.RoundedRect(new RectangleF(0, 0, Width, Height), Theme.ControlRadius(Radius));
            Region? old = Region;
            Region = new Region(path);
            old?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = Theme.RoundedRect(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), Theme.ControlRadius(Radius));
            if (Theme.Surface != SkinSurface.Glass)
            {
                SkinStyles.PaintCard(e.Graphics, path, new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), Theme.ControlRadius(Radius));
                return;
            }
            using var pen = new Pen(Theme.Border, 1f);
            e.Graphics.DrawPath(pen, path);
        }
    }

    /// <summary>扁平化品牌标记：圆角底色 + 两张重叠文档，无任何文字。</summary>
    public static class Art
    {
        private static readonly Color MarkBadge = Color.FromArgb(255, 255, 189, 61);
        private static readonly Color MarkBadgeGlyph = Color.FromArgb(255, 35, 49, 74);

        /// <summary>扁平品牌标记：品牌色圆角底 + 两张倾斜的白色文档 + 琥珀色合并徽标。</summary>
        public static void DrawMark(Graphics g, RectangleF rect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float unit = rect.Width;
            using (GraphicsPath path = Theme.RoundedRect(rect, unit * 0.235f))
            using (var brush = new SolidBrush(Theme.Accent))
                g.FillPath(brush, path);

            DrawSheet(g, rect, 0.415f, 0.50f, 0.40f, 0.50f, -9f,
                Color.FromArgb(120, 255, 255, 255));
            DrawSheet(g, rect, 0.545f, 0.47f, 0.45f, 0.54f, 7f,
                Color.FromArgb(248, 255, 255, 255));

            GraphicsState state = g.Save();
            g.TranslateTransform(rect.X + unit * 0.545f, rect.Y + unit * 0.47f);
            g.RotateTransform(7f);
            using (var pen = new Pen(Theme.Accent, Math.Max(1f, unit * 0.024f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            })
            {
                float half = unit * 0.45f / 2f;
                float pad = unit * 0.075f;
                float lineLength = half * 2f - pad * 2f;
                for (int i = 0; i < 3; i++)
                {
                    float y = unit * 0.54f * (i - 1) * 0.30f;
                    g.DrawLine(pen,
                        -half + pad, y,
                        -half + pad + lineLength * (i == 2 ? 0.68f : 1f), y);
                }
            }
            g.Restore(state);

            float badgeX = rect.X + unit * 0.795f;
            float badgeY = rect.Y + unit * 0.795f;
            using (var ring = new SolidBrush(Theme.Accent))
                g.FillEllipse(ring, badgeX - unit * 0.20f, badgeY - unit * 0.20f,
                    unit * 0.40f, unit * 0.40f);
            using (var fill = new SolidBrush(MarkBadge))
                g.FillEllipse(fill, badgeX - unit * 0.155f, badgeY - unit * 0.155f,
                    unit * 0.31f, unit * 0.31f);
            using (var plus = new Pen(MarkBadgeGlyph, Math.Max(1.2f, unit * 0.045f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            })
            {
                float arm = unit * 0.070f;
                g.DrawLine(plus, badgeX - arm, badgeY, badgeX + arm, badgeY);
                g.DrawLine(plus, badgeX, badgeY - arm, badgeX, badgeY + arm);
            }
        }

        private static void DrawSheet(Graphics g, RectangleF rect,
            float centerX, float centerY, float width, float height, float angle, Color fill)
        {
            float unit = rect.Width;
            GraphicsState state = g.Save();
            g.TranslateTransform(rect.X + unit * centerX, rect.Y + unit * centerY);
            g.RotateTransform(angle);
            var sheet = new RectangleF(-unit * width / 2f, -unit * height / 2f,
                unit * width, unit * height);
            using (GraphicsPath path = Theme.RoundedRect(sheet, unit * 0.058f))
            using (var brush = new SolidBrush(fill))
                g.FillPath(brush, path);
            g.Restore(state);
        }
    }

    public class HeaderPanel : Panel
    {
        /// <summary>右上角徽标距右边缘的距离，由主窗体按按钮实际宽度设置。</summary>
        public int BadgeRightInset { get; set; } = 176;

        public HeaderPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.HeaderStart;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0, 0, Width, Height);
            using (var background = new LinearGradientBrush(rect, Theme.HeaderStart, Theme.HeaderEnd, 0f))
                g.FillRectangle(background, rect);

            int glowAlpha = Theme.Current.HeaderOnLight ? 20 : 18;
            Color glowColor = Theme.Current.HeaderOnLight
                ? Color.FromArgb(glowAlpha, 120, 90, 70)
                : Color.FromArgb(glowAlpha, 255, 255, 255);
            using (var glow = new SolidBrush(glowColor))
            {
                g.FillEllipse(glow, Width - 250, -130, 360, 360);
                g.FillEllipse(glow, Width - 90, 42, 170, 170);
            }
            using (var accentGlow = new SolidBrush(Color.FromArgb(26, Theme.Accent)))
                g.FillEllipse(accentGlow, Width - 170, -70, 220, 220);

            Art.DrawMark(g, new RectangleF(30, (Height - 48) / 2f - 4, 48, 48));

            TextRenderer.DrawText(
                g, Loc.T(Txt.AppName), Theme.TitleFont,
                new Rectangle(94, 12, Math.Max(60, Width - 280), 36), Theme.HeaderText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            // 右上角徽标
            string badgeText = Loc.T(Txt.LocalBadge);
            Size badgeSize = TextRenderer.MeasureText(badgeText, Theme.SmallFont, Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            float badgeWidth = badgeSize.Width + 30f;
            float badgeRight = Width - BadgeRightInset;
            var badgeRect = new RectangleF(badgeRight - badgeWidth, 36f, badgeWidth, 28f);

            float subtitleRight = badgeRect.Left - 14f;
            bool showBadge = subtitleRight > 260f;

            if (showBadge)
            {
                using GraphicsPath badgePath = Theme.RoundedRect(badgeRect, 14);
                using (var fill = new SolidBrush(Theme.Current.HeaderBadgeFill))
                    g.FillPath(fill, badgePath);
                using (var border = new Pen(Theme.Current.HeaderBadgeBorder, 1f))
                    g.DrawPath(border, badgePath);

                TextRenderer.DrawText(
                    g, badgeText, Theme.SmallFont,
                    new Rectangle((int)badgeRect.X, (int)badgeRect.Y, (int)badgeRect.Width, (int)badgeRect.Height),
                    Theme.Current.HeaderBadgeText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }
            else
            {
                subtitleRight = Width - BadgeRightInset;
            }

            int subtitleWidth = Math.Max(80, (int)(subtitleRight - 96f));
            TextRenderer.DrawText(
                g, Loc.T(Txt.Tagline), Theme.SubtitleFont,
                new Rectangle(96, 54, subtitleWidth, 24), Theme.HeaderSubtitle,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }
    }

    public class RoundedInput : Panel
    {
        private bool _focused;
        public TextBox Box { get; }

        public RoundedInput()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.GlassInput;

            Box = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = Theme.InputFont,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.GlassInput
            };
            Box.GotFocus += (_, _) => { _focused = true; Invalidate(); };
            Box.LostFocus += (_, _) => { _focused = false; Invalidate(); };
            Box.TextChanged += (_, _) => OnTextChanged(EventArgs.Empty);
            Controls.Add(Box);

            Height = 38;
        }

        public override string Text
        {
            get => Box.Text;
            set => Box.Text = value;
        }

        public bool ReadOnly
        {
            get => Box.ReadOnly;
            set
            {
                Box.ReadOnly = value;
                ApplyTheme();
            }
        }

        public int MaxLength
        {
            get => Box.MaxLength;
            set => Box.MaxLength = value;
        }

        public void ApplyTheme()
        {
            BackColor = Theme.GlassInput;
            Box.BackColor = Box.ReadOnly || !Enabled ? Theme.InputReadOnlyFill : Theme.GlassInput;
            Box.ForeColor = Box.ReadOnly || !Enabled ? Theme.TextSecondary : Theme.TextPrimary;
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            ApplyTheme();
            base.OnEnabledChanged(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            int height = Box.PreferredHeight;
            Box.SetBounds(14, Math.Max(4, (Height - height) / 2), Math.Max(10, Width - 28), height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath path = Theme.RoundedRect(rect, Theme.ControlRadius(10));
            Color fill = Box.ReadOnly || !Enabled ? Theme.InputReadOnlyFill : Theme.GlassInput;
            using (var brush = new SolidBrush(fill))
                e.Graphics.FillPath(brush, path);

            Color border = _focused && Enabled ? Theme.Accent : Theme.Border;
            bool drawBorder = Theme.Surface != SkinSurface.Neumorphic || (_focused && Enabled);
            if (drawBorder)
            {
                using var pen = new Pen(border, _focused ? 1.4f : 1f);
                e.Graphics.DrawPath(pen, path);
            }

            SkinStyles.PaintInput(e.Graphics, path, rect, _focused && Enabled);
        }
    }

    /// <summary>跟随主题配色的细进度条，右侧实时显示合并百分比。</summary>
    public class SlimProgressBar : Control
    {
        private int _value;

        /// <summary>放在更实的玻璃卡（右侧输出面板）里时置为 true。</summary>
        public bool StrongGlass { get; set; }

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Clamp(value, 0, 100);
                Invalidate();
            }
        }

        public SlimProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Height = 20;
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            GlassPaint.Paint(this, e.Graphics, StrongGlass ? Theme.GlassStrong : Theme.GlassFill);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            string percentText = _value + "%";
            int textWidth = TextRenderer.MeasureText(percentText, Theme.PercentFont, Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix).Width;
            int labelWidth = Math.Max(38, textWidth + 8);
            int barWidth = Math.Max(24, Width - labelWidth - 8);

            const float thickness = 10f;
            float top = (Height - thickness) / 2f;

            var track = new RectangleF(0, top, barWidth, thickness);
            using (GraphicsPath path = Theme.RoundedRect(track, thickness / 2f))
            using (var brush = new SolidBrush(Theme.ProgressTrack))
                g.FillPath(brush, path);

            if (_value > 0)
            {
                float fillWidth = Math.Max(thickness, barWidth * _value / 100f);
                var fill = new RectangleF(0, top, fillWidth, thickness);
                using (GraphicsPath path = Theme.RoundedRect(fill, thickness / 2f))
                using (var brush = new LinearGradientBrush(fill, Theme.ProgressStart, Theme.Accent, 0f))
                    g.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                g, percentText, Theme.PercentFont,
                new Rectangle(Width - labelWidth, 0, labelWidth, Height),
                _value > 0 ? Theme.Accent : Theme.TextMuted,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }

    public class DocumentListBox : ListBox
    {
        private const int ActionSize = 24;
        private const int ActionGap = 4;

        private int _hoverIndex = -1;
        private int _hoverZone;

        private bool _marqueeActive;
        private bool _marqueeMoved;
        private bool _applyingMarquee;
        private Point _marqueeStart;
        private Point _marqueeCurrent;
        private Rectangle _marqueeFrame;
        private readonly HashSet<int> _marqueeBase = new();
        private HashSet<int> _marqueeBefore = new();
        private HashSet<int>? _marqueeUndo;
        private int _marqueeUndoCount;

        /// <summary>是否正在框选；主窗口据此避免把框选当成拖拽排序。</summary>
        public bool MarqueeActive => _marqueeActive;

        /// <summary>最近一次框选是否可以用“取消框选”还原。</summary>
        public bool HasMarqueeSelection =>
            _marqueeActive || (_marqueeUndo != null && _marqueeUndoCount == Items.Count);

        /// <summary>框选开始或结束时触发，供工具栏刷新“取消框选”按钮状态。</summary>
        public event EventHandler? MarqueeStateChanged;

        /// <summary>还原到框选前的选择；正在拖动时则直接中止本次框选。</summary>
        public void CancelMarqueeSelection()
        {
            if (_marqueeActive)
            {
                EraseMarqueeFrame();
                _marqueeActive = false;
                _marqueeMoved = false;
                if (Capture) Capture = false;
                ApplySelection(_marqueeBefore);
                MarqueeStateChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_marqueeUndo == null) return;
            HashSet<int> restore = _marqueeUndo;
            _marqueeUndo = null;
            _marqueeUndoCount = 0;
            ApplySelection(restore);
            MarqueeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>把两个点规范成左上到右下的矩形。</summary>
        public static Rectangle NormalizeMarqueeRect(Point a, Point b) =>
            Rectangle.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y),
                Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        /// <summary>返回与选框相交的行号；坐标均使用列表内坐标。</summary>
        public static List<int> MarqueeHits(IReadOnlyList<Rectangle> rowBounds, Rectangle marquee)
        {
            var hits = new List<int>();
            for (int i = 0; i < rowBounds.Count; i++)
            {
                Rectangle row = rowBounds[i];
                if (row.Width > 0 && row.Height > 0 && row.IntersectsWith(marquee))
                    hits.Add(i);
            }
            return hits;
        }

        /// <summary>行内操作：上移、下移、移除。由主窗口接管真正的移动逻辑。</summary>
        public event EventHandler? ActionUp;
        public event EventHandler? ActionDown;
        public event EventHandler? ActionRemove;

        /// <summary>转换页不需要调整顺序，置为 false 时只显示移除按钮。</summary>
        public bool ShowMoveActions { get; set; } = true;
        /// <summary>是否绘制右侧拖拽手柄；转换页关闭。</summary>
        public bool ShowDragHandle { get; set; } = true;
        /// <summary>是否在副标题里显示 PDF 页数；只有转换页需要。</summary>
        public bool ShowPageCount { get; set; }

        public DocumentListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 62;
            BorderStyle = BorderStyle.None;
            BackColor = Theme.Card;
            ForeColor = Theme.TextPrimary;
            SelectionMode = SelectionMode.MultiExtended;
            IntegralHeight = false;
            Font = Theme.ItemFont;
            // 双缓冲 + 仅重绘变化的行，消除悬停/滚动时的闪烁
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public void ApplyTheme()
        {
            BackColor = Theme.Card;
            ForeColor = Theme.TextPrimary;
            Font = Theme.ItemFont;
            Invalidate();
        }

        private void InvalidateRow(int index)
        {
            if (index >= 0 && index < Items.Count)
                Invalidate(GetItemRectangle(index));
        }

        /// <summary>行内三个操作按钮的位置（相对控件坐标）。</summary>
        private Rectangle[] GetActionBounds(int index)
        {
            if (index < 0 || index >= Items.Count) return Array.Empty<Rectangle>();

            Rectangle item = GetItemRectangle(index);
            if (item.Height <= 0) return Array.Empty<Rectangle>();

            var row = new Rectangle(item.Left + 10, item.Top + 3,
                Math.Max(24, item.Width - 20), Math.Max(20, item.Height - 8));
            int top = row.Top + (row.Height - ActionSize) / 2;
            int right = row.Right - (ShowDragHandle ? 34 : 22);

            if (!ShowMoveActions)
            {
                return new[] { new Rectangle(right - ActionSize, top, ActionSize, ActionSize) };
            }

            int left = right - (ActionSize * 3 + ActionGap * 2);
            return new[]
            {
                new Rectangle(left, top, ActionSize, ActionSize),
                new Rectangle(left + ActionSize + ActionGap, top, ActionSize, ActionSize),
                new Rectangle(left + 2 * (ActionSize + ActionGap), top, ActionSize, ActionSize)
            };
        }

        private int ZoneAt(int index, Point point)
        {
            Rectangle[] bounds = GetActionBounds(index);
            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].Contains(point)) return i + 1;
            }
            return 0;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_marqueeActive)
            {
                UpdateMarquee(e.Location);
                return;
            }

            int index = IndexFromPoint(e.Location);
            int zone = ZoneAt(index, e.Location);
            if (index != _hoverIndex || zone != _hoverZone)
            {
                int previous = _hoverIndex;
                _hoverZone = zone;
                _hoverIndex = index;
                InvalidateRow(previous);
                InvalidateRow(index);
            }
            Cursor = zone > 0 ? Cursors.Hand : Cursors.Default;
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int index = IndexFromPoint(e.Location);
                int zone = ZoneAt(index, e.Location);
                // 只有 Shift 连选和框选保留多选；普通点击与 Ctrl+点击都只保留当前行
                bool plainClick = (ModifierKeys & Keys.Shift) == Keys.None;
                if (zone > 0)
                {
                    // 多选模式下 SelectedIndex 是追加选中，这里必须显式收起其他行
                    if (plainClick) CollapseSelectionTo(index);
                    else SelectedIndex = index;
                    if (!ShowMoveActions) ActionRemove?.Invoke(this, EventArgs.Empty);
                    else if (zone == 1) ActionUp?.Invoke(this, EventArgs.Empty);
                    else if (zone == 2) ActionDown?.Invoke(this, EventArgs.Empty);
                    else ActionRemove?.Invoke(this, EventArgs.Empty);
                    return;
                }

                // 行卡片之外的空白（含左右留白）按下即开始框选，避免与拖拽排序冲突
                if (SelectionMode != SelectionMode.One && !IsRowBody(index, e.Location))
                {
                    BeginMarquee(e.Location);
                    return;
                }

                // 普通点击（含 Ctrl）只保留当前行；即使原生控件延迟收起（例如随后被判为拖拽），也先在这里定案
                if (plainClick && index >= 0) CollapseSelectionTo(index);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_marqueeActive && e.Button == MouseButtons.Left)
            {
                EndMarquee();
                return;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            if (_marqueeActive && !Capture) EndMarquee();
            base.OnMouseCaptureChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_marqueeActive || HasMarqueeSelection)
                {
                    CancelMarqueeSelection();
                    e.Handled = true;
                    return;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            // 普通点选会清掉“取消框选”的快照，避免还原出过期的选择
            if (!_applyingMarquee && _marqueeUndo != null)
            {
                _marqueeUndo = null;
                _marqueeUndoCount = 0;
                MarqueeStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool IsRowBody(int index, Point point)
        {
            if (index < 0 || index >= Items.Count) return false;
            return RowBounds(index).Contains(point);
        }

        private Rectangle RowBounds(int index)
        {
            Rectangle item = GetItemRectangle(index);
            return new Rectangle(item.Left + 10, item.Top + 3,
                Math.Max(10, item.Width - 20), Math.Max(10, item.Height - 8));
        }

        private void BeginMarquee(Point start)
        {
            _marqueeActive = true;
            _marqueeMoved = false;
            _marqueeStart = start;
            _marqueeCurrent = start;
            _marqueeBefore.Clear();
            foreach (int i in SelectedIndices) _marqueeBefore.Add(i);
            _marqueeUndo = null;
            _marqueeUndoCount = 0;
            _marqueeBase.Clear();
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                foreach (int i in SelectedIndices) _marqueeBase.Add(i);
            }
            else if (SelectedIndices.Count > 0)
            {
                ApplySelection(_marqueeBase);
            }

            int previous = _hoverIndex;
            _hoverIndex = -1;
            _hoverZone = 0;
            InvalidateRow(previous);
            Cursor = Cursors.Default;
            // ListBox 获得焦点时会把焦点行滚入视图，这里记住滚动位置再还原，
            // 否则在空白处按下开始框选时列表会突然跳一下。
            int top = TopIndex;
            Focus();
            if (TopIndex != top) TopIndex = top;
            Capture = true;
            MarqueeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateMarquee(Point point)
        {
            if (!_marqueeMoved)
            {
                Rectangle moved = NormalizeMarqueeRect(_marqueeStart, point);
                if (moved.Width >= 3 || moved.Height >= 3) _marqueeMoved = true;
            }

            _marqueeCurrent = point;
            EraseMarqueeFrame();
            ApplyMarqueeSelection();
            DrawMarqueeFrame();
        }

        /// <summary>普通点选时只保留目标行：先选中目标，再清掉其它行，避免上一行残留选中。</summary>
        private void CollapseSelectionTo(int index)
        {
            if (index < 0 || index >= Items.Count) return;
            if (SelectedIndices.Count == 1 && GetSelected(index)) return;

            int top = TopIndex;
            BeginUpdate();
            try
            {
                if (!GetSelected(index)) SetSelected(index, true);
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i != index && GetSelected(i)) SetSelected(i, false);
                }
            }
            finally
            {
                EndUpdate();
            }
            if (TopIndex != top) TopIndex = top;
        }

        private void ApplySelection(ICollection<int> desired)
        {
            int top = TopIndex;
            _applyingMarquee = true;
            BeginUpdate();
            try
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    bool want = desired.Contains(i);
                    if (GetSelected(i) != want) SetSelected(i, want);
                }
            }
            finally
            {
                EndUpdate();
                _applyingMarquee = false;
            }
            if (TopIndex != top) TopIndex = top;
        }

        private void ApplyMarqueeSelection()
        {
            Rectangle box = NormalizeMarqueeRect(_marqueeStart, _marqueeCurrent);
            var desired = new HashSet<int>(_marqueeBase);
            int count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                Rectangle item = GetItemRectangle(i);
                if (item.Width <= 0 || item.Height <= 0) continue;
                if (RowBounds(i).IntersectsWith(box)) desired.Add(i);
            }
            ApplySelection(desired);
        }

        private void DrawMarqueeFrame()
        {
            Rectangle client = Rectangle.Intersect(
                NormalizeMarqueeRect(_marqueeStart, _marqueeCurrent), ClientRectangle);
            if (client.Width <= 0 || client.Height <= 0) return;
            _marqueeFrame = RectangleToScreen(client);
            ControlPaint.DrawReversibleFrame(_marqueeFrame, Theme.Accent, FrameStyle.Dashed);
        }

        private void EraseMarqueeFrame()
        {
            if (_marqueeFrame.Width <= 0 || _marqueeFrame.Height <= 0) return;
            ControlPaint.DrawReversibleFrame(_marqueeFrame, Theme.Accent, FrameStyle.Dashed);
            _marqueeFrame = Rectangle.Empty;
        }

        private void EndMarquee()
        {
            if (!_marqueeActive) return;
            EraseMarqueeFrame();
            _marqueeActive = false;
            if (Capture) Capture = false;
            ApplyMarqueeSelection();

            if (_marqueeMoved)
            {
                _marqueeUndo = _marqueeBefore;
                _marqueeUndoCount = Items.Count;
            }
            MarqueeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_hoverIndex != -1 || _hoverZone != 0)
            {
                int previous = _hoverIndex;
                _hoverIndex = -1;
                _hoverZone = 0;
                InvalidateRow(previous);
            }
            Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var item = (DocumentItem)Items[e.Index];
            bool selected = (e.State & DrawItemState.Selected) != 0;
            bool hovered = e.Index == _hoverIndex && !selected;

            // 卡片式行：留白、圆角；未选中时不描边，只用细分隔线保持清爽
            var row = new Rectangle(
                e.Bounds.Left + 10, e.Bounds.Top + 3,
                Math.Max(10, e.Bounds.Width - 20), Math.Max(10, e.Bounds.Height - 8));

            using (GraphicsPath path = Theme.RoundedRect(row, 10))
            {
                // 未悬停 / 未选中时不填充，让 ListBox 的磨砂底色直接透出来
                if (selected || hovered)
                {
                    Color fill = selected ? Theme.ListSelected : Theme.ListHover;
                    using (var brush = new SolidBrush(fill))
                        g.FillPath(brush, path);

                    Color borderColor = selected
                        ? Color.FromArgb(120, Theme.Accent)
                        : Color.FromArgb(90, Theme.Current.DragDots);
                    using var border = new Pen(borderColor, 1f);
                    g.DrawPath(border, path);
                }
            }

            if (selected)
            {
                var bar = new RectangleF(row.Left + 1.5f, row.Top + 14, 3.5f, row.Height - 28);
                using GraphicsPath path = Theme.RoundedRect(bar, 2f);
                using var accent = new SolidBrush(Theme.Accent);
                g.FillPath(accent, path);
            }

            // 序号：选中时用强调色方块，更容易一眼看出多选范围
            var numberRect = new Rectangle(row.Left + 12, row.Top + (row.Height - 24) / 2, 24, 24);
            using (GraphicsPath path = Theme.RoundedRect(numberRect, Theme.ControlRadius(7)))
            using (var brush = new SolidBrush(selected ? Theme.Accent : Theme.Current.NumberFill))
                g.FillPath(brush, path);
            TextRenderer.DrawText(g, (e.Index + 1).ToString(), Theme.BadgeFont, numberRect,
                selected ? Theme.AccentText : Theme.Current.NumberText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // 类型徽标
            var badgeRect = new Rectangle(numberRect.Right + 10, row.Top + (row.Height - 20) / 2, 44, 20);
            Color badgeFill;
            Color badgeText;
            if (item.IsPdf)
            {
                badgeFill = Theme.Current.PdfBadgeFill;
                badgeText = Theme.Current.PdfBadgeText;
            }
            else if (item.IsImage)
            {
                badgeFill = Theme.Current.ImageBadgeFill;
                badgeText = Theme.Current.ImageBadgeText;
            }
            else
            {
                badgeFill = Theme.Current.WordBadgeFill;
                badgeText = Theme.Current.WordBadgeText;
            }
            using (GraphicsPath path = Theme.RoundedRect(badgeRect, 5))
            using (var brush = new SolidBrush(badgeFill))
                g.FillPath(brush, path);
            TextRenderer.DrawText(g, item.KindLabel, Theme.BadgeFont, badgeRect, badgeText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // 文件名 + 文件信息
            int textLeft = badgeRect.Right + 14;
            int textWidth = Math.Max(60, row.Right - textLeft - 118);
            var nameRect = new Rectangle(textLeft, row.Top + 7, textWidth, 22);
            TextRenderer.DrawText(g, item.FileName, Theme.ItemFont, nameRect, Theme.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            var metaRect = new Rectangle(textLeft, row.Top + 29, textWidth, 17);
            string pageText = string.Empty;
            if (ShowPageCount)
            {
                pageText = item.PageCount switch
                {
                    > 0 => Loc.T(Txt.ConvertPageCountFormat, item.PageCount) + "  ·  ",
                    -2 => Loc.T(Txt.ConvertPageCountFailed) + "  ·  ",
                    _ => Loc.T(Txt.ConvertPageCountUnknown) + "  ·  "
                };
            }
            string meta = item.FolderText.Length > 0
                ? $"{pageText}{item.SizeText}  ·  {item.ExtensionLabel}  ·  {item.FolderText}"
                : $"{pageText}{item.SizeText}  ·  {item.ExtensionLabel}";
            TextRenderer.DrawText(g, meta, Theme.MetaFont, metaRect, Theme.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // 悬停时浮出的行内操作：上移 / 下移 / 移除
            if (e.Index == _hoverIndex)
            {
                Rectangle[] actions = GetActionBounds(e.Index);
                IconKind[] kinds = ShowMoveActions
                    ? new[] { IconKind.Up, IconKind.Down, IconKind.Trash }
                    : new[] { IconKind.Trash };
                for (int i = 0; i < actions.Length && i < kinds.Length; i++)
                {
                    bool hot = _hoverZone == i + 1;
                    if (hot)
                    {
                        using GraphicsPath hotPath = Theme.RoundedRect(actions[i], Theme.ControlRadius(6));
                        using var hotBrush = new SolidBrush(Color.FromArgb(38, Theme.Accent));
                        g.FillPath(hotBrush, hotPath);
                    }
                    IconPainter.Draw(g, kinds[i], actions[i],
                        hot ? Theme.Accent : Theme.TextMuted, 1.9f);
                }
            }

            // 拖拽手柄
            if (ShowDragHandle)
            {
                Color dotColor = selected ? Theme.Current.NumberText : Theme.Current.DragDots;
                int dotX = row.Right - 22;
                int dotY = row.Top + (row.Height - 16) / 2;
                for (int col = 0; col < 2; col++)
                {
                    for (int line = 0; line < 3; line++)
                    {
                        using var dot = new SolidBrush(dotColor);
                        g.FillEllipse(dot, dotX + col * 6, dotY + line * 6, 2.4f, 2.4f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 空列表时的提示层（ListBox 由系统原生绘制，无法在 OnPaint 中叠加，故用独立覆盖层）。
    /// </summary>
    public class EmptyStatePanel : Control
    {
        /// <summary>为空时使用 Txt.EmptyTitle；转换页提供自己的文案。</summary>
        public string? TitleOverride { get; set; }
        public string? SubtitleOverride { get; set; }
        public string? FormatsOverride { get; set; }

        public EmptyStatePanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            GlassPaint.Paint(this, e.Graphics, Theme.GlassFill);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var outer = new RectangleF(8, 4, Width - 16, Height - 12);
            using (GraphicsPath path = Theme.RoundedRect(outer, 12))
            using (var pen = new Pen(Theme.Current.EmptyBorder, 1f) { DashStyle = DashStyle.Dash })
                g.DrawPath(pen, path);

            float centerX = Width / 2f;
            float centerY = Height / 2f;

            // 图标
            var backDoc = new RectangleF(centerX - 23, centerY - 70, 32, 42);
            var frontDoc = new RectangleF(centerX - 7, centerY - 62, 32, 42);
            using (GraphicsPath path = Theme.RoundedRect(backDoc, 5))
            {
                using var fill = new SolidBrush(Theme.Current.EmptyIconBack);
                g.FillPath(fill, path);
                using var pen = new Pen(Theme.Current.EmptyIconBorder, 1f);
                g.DrawPath(pen, path);
            }
            using (GraphicsPath path = Theme.RoundedRect(frontDoc, 5))
            {
                using var fill = new SolidBrush(Theme.Card);
                g.FillPath(fill, path);
                using var pen = new Pen(Theme.Current.EmptyIconBorder, 1.2f);
                g.DrawPath(pen, path);
                using var linePen = new Pen(Theme.Current.EmptyIconBorder, 1f);
                g.DrawLine(linePen, frontDoc.Left + 8, frontDoc.Top + 14, frontDoc.Right - 8, frontDoc.Top + 14);
                g.DrawLine(linePen, frontDoc.Left + 8, frontDoc.Top + 22, frontDoc.Right - 13, frontDoc.Top + 22);
                g.DrawLine(linePen, frontDoc.Left + 8, frontDoc.Top + 30, frontDoc.Right - 18, frontDoc.Top + 30);
            }

            string title = TitleOverride ?? Loc.T(Txt.EmptyTitle);
            string subtitle = SubtitleOverride ?? Loc.T(Txt.EmptySubtitle);
            string formats = FormatsOverride ?? Loc.T(Txt.EmptyFormats);

            using (var font = new Font(Theme.SectionFont.FontFamily, 11f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, title, font,
                    new Rectangle(0, (int)(centerY - 12), Width, 24), Theme.TextSecondary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            TextRenderer.DrawText(g, subtitle, Theme.SmallFont,
                new Rectangle(0, (int)(centerY + 16), Width, 20), Theme.TextMuted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, formats, Theme.SmallFont,
                new Rectangle(0, (int)(centerY + 42), Width, 18), Theme.Current.EmptyText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public enum IconKind
    {
        None,
        Merge,
        Settings,
        Info,
        Plus,
        Up,
        Down,
        Trash,
        Clear,
        Folder,
        Lock,
        External,
        Contrast,
        Watermark,
        Bell,
        Convert,
        Image,
        WordPdf,
        Close,
        Refresh,
        Palette,
        MarqueeOff
    }

    /// <summary>统一的线性图标绘制（24×24 网格按目标矩形缩放），不依赖任何图标资源文件。</summary>
    public static class IconPainter
    {
        public static void Draw(Graphics g, IconKind kind, RectangleF bounds, Color color, float stroke = 1.9f)
        {
            if (kind == IconKind.None || bounds.Width <= 0 || bounds.Height <= 0) return;

            float scale = Math.Min(bounds.Width, bounds.Height) / 24f;
            if (scale <= 0f) return;

            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(
                bounds.X + (bounds.Width - 24f * scale) / 2f,
                bounds.Y + (bounds.Height - 24f * scale) / 2f);
            g.ScaleTransform(scale, scale);

            // 小尺寸下限制线宽在 24 网格里的占比，避免 14–16px 图标糊成一团
            using var pen = new Pen(color, Math.Max(0.5f, Math.Min(2.4f, stroke / scale)))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
            using var softBrush = new SolidBrush(Color.FromArgb(30, color));
            using var solidBrush = new SolidBrush(color);
            bool compact = bounds.Width < 20f;

            switch (kind)
            {
                case IconKind.Merge:
                {
                    // 三份文档向中间汇合：上方两份、下方一份，两条连线表示合并
                    using (GraphicsPath topLeft = Theme.RoundedRect(new RectangleF(3.6f, 3.4f, 7.4f, 9.2f), 1.7f))
                    {
                        g.FillPath(softBrush, topLeft);
                        g.DrawPath(pen, topLeft);
                    }
                    using (GraphicsPath topRight = Theme.RoundedRect(new RectangleF(13f, 3.4f, 7.4f, 9.2f), 1.7f))
                    {
                        g.FillPath(softBrush, topRight);
                        g.DrawPath(pen, topRight);
                    }
                    using (GraphicsPath bottom = Theme.RoundedRect(new RectangleF(6.8f, 13.4f, 10.4f, 7f), 1.8f))
                    {
                        g.FillPath(softBrush, bottom);
                        g.DrawPath(pen, bottom);
                    }
                    g.DrawLine(pen, 7.5f, 12.8f, 10.2f, 14.2f);
                    g.DrawLine(pen, 16.5f, 12.8f, 13.8f, 14.2f);
                    break;
                }
                case IconKind.Settings:
                {
                    g.DrawLine(pen, 4f, 7f, 20f, 7f);
                    g.DrawLine(pen, 4f, 12f, 20f, 12f);
                    g.DrawLine(pen, 4f, 17f, 20f, 17f);
                    g.FillEllipse(solidBrush, 6.1f, 4.9f, 4.2f, 4.2f);
                    g.FillEllipse(solidBrush, 13.7f, 9.9f, 4.2f, 4.2f);
                    g.FillEllipse(solidBrush, 4.4f, 14.9f, 4.2f, 4.2f);
                    break;
                }
                case IconKind.Info:
                {
                    g.FillEllipse(softBrush, 3.5f, 3.5f, 17f, 17f);
                    g.DrawEllipse(pen, 3.5f, 3.5f, 17f, 17f);
                    g.DrawLine(pen, 12f, 11f, 12f, 16.6f);
                    g.FillEllipse(solidBrush, 11f, 6.9f, 2f, 2f);
                    break;
                }
                case IconKind.Plus:
                {
                    g.DrawLine(pen, 12f, 5f, 12f, 19f);
                    g.DrawLine(pen, 5f, 12f, 19f, 12f);
                    break;
                }
                case IconKind.Up:
                {
                    using (var head = new GraphicsPath())
                    {
                        head.AddPolygon(new[]
                        {
                            new PointF(12f, 4.4f), new PointF(18.2f, 11.6f),
                            new PointF(5.8f, 11.6f)
                        });
                        g.FillPath(softBrush, head);
                        g.DrawPath(pen, head);
                    }
                    g.DrawLine(pen, 12f, 10.6f, 12f, 19.4f);
                    break;
                }
                case IconKind.Down:
                {
                    using (var head = new GraphicsPath())
                    {
                        head.AddPolygon(new[]
                        {
                            new PointF(12f, 19.6f), new PointF(5.8f, 12.4f),
                            new PointF(18.2f, 12.4f)
                        });
                        g.FillPath(softBrush, head);
                        g.DrawPath(pen, head);
                    }
                    g.DrawLine(pen, 12f, 4.6f, 12f, 13.4f);
                    break;
                }
                case IconKind.Trash:
                {
                    using (GraphicsPath body = Theme.RoundedRect(new RectangleF(6.3f, 7.8f, 11.4f, 12.2f), 2.1f))
                    {
                        g.FillPath(softBrush, body);
                        g.DrawPath(pen, body);
                    }
                    g.DrawLine(pen, 4.4f, 7.2f, 19.6f, 7.2f);
                    g.DrawLine(pen, 9.2f, 7.2f, 9.2f, 4.6f);
                    g.DrawLine(pen, 9.2f, 4.6f, 14.8f, 4.6f);
                    g.DrawLine(pen, 14.8f, 4.6f, 14.8f, 7.2f);
                    g.DrawLine(pen, 10.2f, 10.6f, 10.4f, 16.8f);
                    g.DrawLine(pen, 13.8f, 10.6f, 13.6f, 16.8f);
                    break;
                }
                case IconKind.Clear:
                {
                    g.DrawLine(pen, 4f, 6.8f, 20f, 6.8f);
                    g.DrawLine(pen, 4f, 12f, 20f, 12f);
                    g.DrawLine(pen, 4f, 17.2f, 11.4f, 17.2f);
                    g.DrawLine(pen, 14.6f, 15f, 20f, 20.4f);
                    g.DrawLine(pen, 20f, 15f, 14.6f, 20.4f);
                    break;
                }
                case IconKind.Folder:
                {
                    using var path = new GraphicsPath();
                    path.AddLines(new[]
                    {
                        new PointF(3.6f, 19f), new PointF(3.6f, 7f), new PointF(9f, 7f),
                        new PointF(11f, 9.6f), new PointF(20.4f, 9.6f), new PointF(20.4f, 19f)
                    });
                    path.CloseFigure();
                    g.FillPath(softBrush, path);
                    g.DrawPath(pen, path);
                    break;
                }
                case IconKind.Lock:
                {
                    using (GraphicsPath body = Theme.RoundedRect(new RectangleF(5f, 10.6f, 14f, 9.4f), 2.4f))
                    {
                        g.FillPath(softBrush, body);
                        g.DrawPath(pen, body);
                    }
                    g.DrawArc(pen, 8f, 4.6f, 8f, 9f, 180f, 180f);
                    g.FillEllipse(solidBrush, 11f, 13.4f, 2f, 2f);
                    break;
                }
                case IconKind.External:
                {
                    g.DrawLine(pen, 14.5f, 4.5f, 19.5f, 4.5f);
                    g.DrawLine(pen, 19.5f, 4.5f, 19.5f, 9.5f);
                    g.DrawLine(pen, 19.5f, 4.5f, 12.2f, 11.8f);
                    using var path = new GraphicsPath();
                    path.AddLines(new[]
                    {
                        new PointF(17f, 14f), new PointF(17f, 19.6f),
                        new PointF(4.6f, 19.6f), new PointF(4.6f, 7.2f), new PointF(10f, 7.2f)
                    });
                    g.FillPath(softBrush, path);
                    g.DrawPath(pen, path);
                    break;
                }
                case IconKind.Contrast:
                {
                    // 外观：左实右空的圆，用来表示「皮肤 / 明暗」
                    g.DrawEllipse(pen, 3.6f, 3.6f, 16.8f, 16.8f);
                    using (var half = new SolidBrush(color))
                        g.FillPie(half, 3.6f, 3.6f, 16.8f, 16.8f, 90f, 180f);
                    break;
                }
                case IconKind.Watermark:
                {
                    // 水印：一滴水；小尺寸下不再叠加内部斜线，保证 19px 时轮廓完整清晰
                    using var drop = new GraphicsPath();
                    drop.AddBezier(12f, 3.6f, 16.2f, 8.4f, 18.2f, 12f, 18.2f, 15.2f);
                    drop.AddArc(5.8f, 9f, 12.4f, 12.4f, 0f, 180f);
                    drop.AddBezier(5.8f, 15.2f, 5.8f, 12f, 7.8f, 8.4f, 12f, 3.6f);
                    drop.CloseFigure();
                    g.FillPath(softBrush, drop);
                    g.DrawPath(pen, drop);
                    if (!compact) g.DrawLine(pen, 9.8f, 14.8f, 14.2f, 10.4f);
                    break;
                }
                case IconKind.Bell:
                {
                    // 完成提醒：铃铛
                    using var bell = new GraphicsPath();
                    bell.AddArc(6.2f, 6.2f, 11.6f, 11.6f, 180f, 180f);
                    bell.AddLine(17.8f, 12f, 19.2f, 17.4f);
                    bell.AddLine(19.2f, 17.4f, 4.8f, 17.4f);
                    bell.AddLine(4.8f, 17.4f, 6.2f, 12f);
                    bell.CloseFigure();
                    g.FillPath(softBrush, bell);
                    g.DrawPath(pen, bell);
                    g.DrawLine(pen, 12f, 6.4f, 12f, 4.2f);
                    g.FillEllipse(solidBrush, 10.9f, 18f, 2.2f, 2.2f);
                    break;
                }
                case IconKind.Convert:
                {
                    // PDF 转图片：左侧文档 + 右侧图片 + 指向图片的箭头
                    using (GraphicsPath page = Theme.RoundedRect(new RectangleF(3.4f, 3.6f, 8f, 16.8f), 2f))
                    {
                        g.FillPath(softBrush, page);
                        g.DrawPath(pen, page);
                    }
                    if (compact)
                    {
                        // 16px 下右侧图片框会与箭头糊在一起，只保留文档 + 箭头
                        g.DrawLine(pen, 5.8f, 8f, 9f, 8f);
                        g.DrawLine(pen, 5.8f, 12f, 9f, 12f);
                        g.DrawLine(pen, 10.4f, 12f, 19f, 12f);
                        g.DrawLine(pen, 19f, 12f, 17.2f, 10.2f);
                        g.DrawLine(pen, 19f, 12f, 17.2f, 13.8f);
                        break;
                    }
                    g.DrawLine(pen, 5.8f, 7.6f, 9f, 7.6f);
                    g.DrawLine(pen, 5.8f, 10.6f, 9f, 10.6f);
                    g.DrawLine(pen, 5.8f, 13.6f, 8.2f, 13.6f);
                    g.DrawLine(pen, 12.2f, 12f, 15.4f, 12f);
                    g.DrawLine(pen, 15.4f, 12f, 13.9f, 10.5f);
                    g.DrawLine(pen, 15.4f, 12f, 13.9f, 13.5f);
                    using (GraphicsPath frame = Theme.RoundedRect(new RectangleF(15.6f, 7.4f, 5.4f, 6.2f), 1.4f))
                    {
                        if (!compact) g.FillPath(softBrush, frame);
                        g.DrawPath(pen, frame);
                    }
                    if (!compact)
                    {
                        using var mountain = new GraphicsPath();
                        mountain.AddLines(new[]
                        {
                            new PointF(16.4f, 12.6f), new PointF(18f, 10.6f),
                            new PointF(19.2f, 11.9f), new PointF(20.2f, 10.9f)
                        });
                        g.DrawPath(pen, mountain);
                    }
                    break;
                }
                case IconKind.Image:
                {
                    using (GraphicsPath frame = Theme.RoundedRect(new RectangleF(3.4f, 4.4f, 17.2f, 15.2f), 2.4f))
                    {
                        g.FillPath(softBrush, frame);
                        g.DrawPath(pen, frame);
                    }
                    g.FillEllipse(solidBrush, 7f, 7.6f, 2.8f, 2.8f);
                    using var mountain = new GraphicsPath();
                    mountain.AddLines(new[]
                    {
                        new PointF(4.6f, 17.6f), new PointF(9.6f, 12.2f),
                        new PointF(13.2f, 15.8f), new PointF(15.8f, 13.2f), new PointF(19.4f, 17.2f)
                    });
                    g.DrawPath(pen, mountain);
                    break;
                }
                case IconKind.Close:
                {
                    g.DrawLine(pen, 6f, 6f, 18f, 18f);
                    g.DrawLine(pen, 18f, 6f, 6f, 18f);
                    break;
                }
                case IconKind.Refresh:
                {
                    g.DrawArc(pen, 4.4f, 4.4f, 15.2f, 15.2f, -40f, 280f);
                    g.DrawLine(pen, 18.6f, 4.2f, 19.4f, 8.4f);
                    g.DrawLine(pen, 19.4f, 8.4f, 15.2f, 7.6f);
                    break;
                }
                case IconKind.Palette:
                {
                    // 皮肤：调色板 + 四个色点
                    using var palette = new GraphicsPath();
                    palette.AddBezier(new PointF(12f, 3.6f), new PointF(6.6f, 3.6f),
                        new PointF(3.4f, 7.3f), new PointF(3.4f, 11.6f));
                    palette.AddBezier(new PointF(3.4f, 11.6f), new PointF(3.4f, 16.4f),
                        new PointF(6.8f, 20.2f), new PointF(11.6f, 20.2f));
                    palette.AddBezier(new PointF(11.6f, 20.2f), new PointF(13.6f, 20.2f),
                        new PointF(13.6f, 18.4f), new PointF(13.6f, 18.4f));
                    palette.AddBezier(new PointF(13.6f, 18.4f), new PointF(13.6f, 17.1f),
                        new PointF(15.1f, 16.6f), new PointF(15.1f, 16.6f));
                    palette.AddBezier(new PointF(15.1f, 16.6f), new PointF(20.6f, 16.6f),
                        new PointF(20.6f, 11.4f), new PointF(20.6f, 11.4f));
                    palette.AddBezier(new PointF(20.6f, 11.4f), new PointF(20.6f, 6.9f),
                        new PointF(16.7f, 3.6f), new PointF(12f, 3.6f));
                    palette.CloseFigure();
                    g.FillPath(softBrush, palette);
                    g.DrawPath(pen, palette);
                    g.FillEllipse(solidBrush, 7.4f, 7.2f, 2.1f, 2.1f);
                    g.FillEllipse(solidBrush, 11.2f, 6.2f, 2.1f, 2.1f);
                    g.FillEllipse(solidBrush, 15f, 7.4f, 2.1f, 2.1f);
                    g.FillEllipse(solidBrush, 6.8f, 13.2f, 2.1f, 2.1f);
                    break;
                }
                case IconKind.WordPdf:
                {
                    // Word 文档：右上折角纸张 + 内部 W 折线
                    using (GraphicsPath page = new GraphicsPath())
                    {
                        page.AddLine(6.2f, 2.8f, 14.4f, 2.8f);
                        page.AddLine(14.4f, 2.8f, 18.2f, 6.6f);
                        page.AddLine(18.2f, 6.6f, 18.2f, 21.2f);
                        page.AddLine(18.2f, 21.2f, 6.2f, 21.2f);
                        page.CloseFigure();
                        g.FillPath(softBrush, page);
                        g.DrawPath(pen, page);
                    }
                    g.DrawLine(pen, 14.4f, 2.8f, 14.4f, 6.6f);
                    g.DrawLine(pen, 14.4f, 6.6f, 18.2f, 6.6f);
                    g.DrawLines(pen, new[]
                    {
                        new PointF(8.9f, 10.4f), new PointF(10.4f, 17.2f),
                        new PointF(12.2f, 12.4f), new PointF(14f, 17.2f),
                        new PointF(15.5f, 10.4f)
                    });
                    break;
                }
                case IconKind.MarqueeOff:
                {
                    // 取消框选：虚线选框 + 斜杠
                    using var dashed = (Pen)pen.Clone();
                    dashed.DashStyle = DashStyle.Dash;
                    dashed.DashPattern = new[] { 2f, 1.8f };
                    g.DrawRectangle(dashed, 3.8f, 4.6f, 16.4f, 14.8f);
                    g.DrawLine(pen, 4.8f, 19.2f, 19.2f, 4.8f);
                    break;
                }
            }

            g.Restore(state);
        }
    }

    /// <summary>磨砂玻璃卡片：背景取本窗口的模糊壁纸 + 半透明填充，边缘 1px 描边与顶部高光。</summary>
    public class GlassCard : Panel
    {
        public int Radius { get; set; } = 16;
        /// <summary>为 Empty 时按 Strong 自动选择皮肤里的玻璃色。</summary>
        public Color Tint { get; set; } = Color.Empty;
        public bool DrawBorder { get; set; } = true;
        public bool RightEdge { get; set; }
        public bool Strong { get; set; }
        /// <summary>为 true 时使用清晰壁纸（与窗口背景完全一致），适合整页容器。</summary>
        public bool Sharp { get; set; }

        public GlassCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        private Color ResolveTint()
        {
            if (!Tint.IsEmpty) return Tint;
            return Strong ? Theme.GlassStrong : Theme.GlassFill;
        }

        /// <summary>新拟态卡片需要给外侧阴影留出空间，其他风格仍贴合控件边界。</summary>
        private bool NeumorphInset => Theme.Surface == SkinSurface.Neumorphic && Radius > 0 && DrawBorder;

        private RectangleF SurfaceBounds => NeumorphInset
            ? new RectangleF(5.5f, 5.5f, Math.Max(1f, Width - 11f), Math.Max(1f, Height - 11f))
            : new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);

        private float SurfaceRadius => Math.Max(0f, Theme.ControlRadius(Radius));

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            GlassPaint.Paint(this, e.Graphics, ResolveTint(), Sharp);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF bounds = SurfaceBounds;
            if (bounds.Width <= 1f || bounds.Height <= 1f) return;

            using GraphicsPath path = Theme.RoundedRect(bounds, SurfaceRadius);

            if (DrawBorder)
            {
                if (Theme.Surface == SkinSurface.Glass)
                {
                    using (var border = new Pen(Theme.GlassBorder, 1f))
                        g.DrawPath(border, path);

                    if (Width > 40)
                    {
                        using var edge = new LinearGradientBrush(
                            new RectangleF(0, 0, Width, 1f),
                            Theme.GlassEdge, Color.FromArgb(0, Theme.GlassEdge), 0f);
                        using var edgePen = new Pen(edge, 1.2f);
                        g.DrawLine(edgePen,
                            bounds.Left + SurfaceRadius * 0.6f, 1.2f,
                            bounds.Right - SurfaceRadius * 0.6f, 1.2f);
                    }
                }
                else
                {
                    SkinStyles.PaintCard(g, path, bounds, SurfaceRadius);
                }
            }

            if (RightEdge)
            {
                using var line = new Pen(Theme.GlassBorder, 1f);
                g.DrawLine(line, Width - 0.5f, 0f, Width - 0.5f, Height);
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (Radius <= 0 || Width <= 0 || Height <= 0 || NeumorphInset)
            {
                Region = null;
                return;
            }

            using GraphicsPath path = Theme.RoundedRect(
                new RectangleF(0, 0, Width, Height), SurfaceRadius);
            Region? old = Region;
            Region = new Region(path);
            old?.Dispose();
        }
    }

    /// <summary>队列工具栏使用的紧凑图标按钮。</summary>
    public sealed class IconButton : Control
    {
        private bool _hover;
        private bool _pressed;

        public IconKind Icon { get; set; } = IconKind.Plus;
        /// <summary>玻璃底色，缺省与该控件所在的玻璃卡片保持一致。</summary>
        public Color Tint { get; set; } = Color.Empty;

        public IconButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Size = new Size(30, 30);
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            GlassPaint.Paint(this, e.Graphics, Tint.IsEmpty ? Theme.GlassFill : Tint);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath path = Theme.RoundedRect(bounds, 8);

            if (Enabled && _pressed)
            {
                using var pressed = new SolidBrush(Color.FromArgb(40, Theme.Accent));
                g.FillPath(pressed, path);
            }
            else if (Enabled && _hover)
            {
                using var hover = new SolidBrush(Theme.GlassHover);
                g.FillPath(hover, path);
                using var border = new Pen(Theme.GlassBorder, 1f);
                g.DrawPath(border, path);
            }

            Color color = !Enabled
                ? Theme.Current.SecondaryDisabledText
                : _hover ? Theme.TextPrimary : Theme.TextSecondary;

            const int iconSize = 16;
            IconPainter.Draw(g, Icon,
                new RectangleF((Width - iconSize) / 2f, (Height - iconSize) / 2f, iconSize, iconSize),
                color, 1.9f);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            _pressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>左侧栏顶部的品牌标记（玻璃底 + 扁平图标）。</summary>
    public sealed class LogoBadge : Control
    {
        public LogoBadge()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(38, 38);
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            GlassPaint.Paint(this, e.Graphics, Color.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Art.DrawMark(e.Graphics, new RectangleF(0, 0, Width, Height));
        }
    }

    /// <summary>左侧栏底板：品牌渐变 + 顶部柔光 + 右侧分隔线。</summary>
    public sealed class SideNavPanel : Control
    {
        public SideNavPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.HeaderStart;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0, 0, Width, Height);
            if (Width > 0 && Height > 0)
            {
                using var background = new LinearGradientBrush(rect, Theme.HeaderStart, Theme.HeaderEnd, 0f);
                g.FillRectangle(background, rect);
            }

            int glowAlpha = Theme.Current.HeaderOnLight ? 18 : 14;
            using (var glow = new SolidBrush(Color.FromArgb(glowAlpha,
                       Theme.Current.HeaderOnLight ? Color.FromArgb(120, 90, 70) : Color.White)))
            {
                g.FillEllipse(glow, -120, Height - 240, 320, 320);
                g.FillEllipse(glow, Width - 150, -90, 240, 240);
            }

            using var line = new Pen(Theme.Separator, 1f);
            g.DrawLine(line, Width - 0.5f, 0, Width - 0.5f, Height);
        }
    }

    /// <summary>卡片式导航入口：图标块 + 标题 + 副标题，选中时强调色描边与左侧竖条。</summary>
    public sealed class NavCard : Control
    {
        private bool _hover;
        private bool _pressed;
        private bool _selected;

        public IconKind Icon { get; set; } = IconKind.Merge;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;
                Invalidate();
            }
        }

        public NavCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Size = new Size(204, 66);
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        // 不采样窗口壁纸：侧栏是自带渐变的父控件，透明背景才能透出正确的底色

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            if (rect.Width <= 1f || rect.Height <= 1f) return;

            using GraphicsPath path = Theme.RoundedRect(rect, 12);

            Color fill;
            Color border;
            if (!Enabled)
            {
                fill = Color.FromArgb(20, Theme.TextPrimary);
                border = Color.FromArgb(40, Theme.Border);
            }
            else if (_selected)
            {
                fill = Color.FromArgb(46, Theme.Accent);
                border = Color.FromArgb(130, Theme.Accent);
            }
            else if (_hover || _pressed)
            {
                fill = Theme.Current.HeaderButtonHover;
                border = Theme.Current.HeaderButtonBorder;
            }
            else
            {
                fill = Theme.Current.HeaderButtonFill;
                border = Theme.Current.HeaderButtonBorder;
            }

            using (var brush = new SolidBrush(fill))
                g.FillPath(brush, path);
            using (var pen = new Pen(border, 1f))
                g.DrawPath(pen, path);

            if (_selected && Enabled)
            {
                var bar = new RectangleF(1.5f, 16f, 3.5f, Math.Max(8f, Height - 32f));
                using GraphicsPath barPath = Theme.RoundedRect(bar, 2f);
                using var accent = new SolidBrush(Theme.Accent);
                g.FillPath(accent, barPath);
            }

            var iconRect = new RectangleF(16, (Height - 34) / 2f, 34, 34);
            using (GraphicsPath iconPath = Theme.RoundedRect(iconRect, 9))
            {
                // 侧栏始终是 HeaderStart→HeaderEnd 渐变，图标块与文字必须用页头色系，
                // 否则经典蓝 / 网易红这类深色页头配正文深灰字会看不清。
                Color iconFill = !Enabled
                    ? Color.FromArgb(24, Theme.Current.HeaderText)
                    : _selected ? Theme.Accent : Color.FromArgb(40, Theme.Current.HeaderText);
                using var iconBrush = new SolidBrush(iconFill);
                g.FillPath(iconBrush, iconPath);
            }

            Color iconColor = !Enabled
                ? Mix(Theme.Current.HeaderText, Theme.Current.HeaderStart, 0.45f)
                : _selected ? Theme.AccentText : Theme.Current.HeaderText;
            IconPainter.Draw(g, Icon,
                new RectangleF(iconRect.X + 8, iconRect.Y + 8, 18, 18), iconColor, 1.8f);

            int textLeft = 60;
            int textWidth = Math.Max(40, Width - textLeft - 12);
            Color titleColor = !Enabled
                ? Mix(Theme.Current.HeaderText, Theme.Current.HeaderStart, 0.45f)
                : Theme.Current.HeaderText;
            Color subtitleColor = !Enabled
                ? Mix(Theme.Current.HeaderSubtitle, Theme.Current.HeaderStart, 0.45f)
                : _selected ? Theme.Current.HeaderText : Theme.Current.HeaderSubtitle;

            TextRenderer.DrawText(g, Title, Theme.SectionFont,
                new Rectangle(textLeft, 11, textWidth, 22), titleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            TextRenderer.DrawText(g, Subtitle, Theme.SmallFont,
                new Rectangle(textLeft, 33, textWidth, 18), subtitleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        /// <summary>按比例把前景色混向背景色；TextRenderer 不支持 alpha，只能预先算好实色。</summary>
        private static Color Mix(Color foreground, Color background, float foregroundRatio)
        {
            float ratio = Math.Clamp(foregroundRatio, 0f, 1f);
            return Color.FromArgb(
                (int)Math.Round(foreground.R * ratio + background.R * (1 - ratio)),
                (int)Math.Round(foreground.G * ratio + background.G * (1 - ratio)),
                (int)Math.Round(foreground.B * ratio + background.B * (1 - ratio)));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            _pressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>侧栏底部的轻量文字入口（设置、关于）。</summary>
    public sealed class SideLink : Control
    {
        private bool _hover;

        public IconKind Icon { get; set; } = IconKind.Settings;

        public SideLink()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Height = 34;
            Font = Theme.BaseFont;
        }

        public void ApplyTheme()
        {
            Font = Theme.BaseFont;
            Invalidate();
        }

        // 不采样窗口壁纸：透明背景直接透出侧栏渐变

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_hover && Enabled)
            {
                var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
                using GraphicsPath path = Theme.RoundedRect(rect, 8);
                using var hover = new SolidBrush(Theme.Current.HeaderButtonHover);
                g.FillPath(hover, path);
            }

            // 与导航卡片一致：使用页头色系，保证深色侧栏上依然清晰
            Color color = !Enabled
                ? BlendToHeader(Theme.Current.HeaderSubtitle, 0.4f)
                : _hover ? Theme.Current.HeaderText : Theme.Current.HeaderSubtitle;
            IconPainter.Draw(g, Icon, new RectangleF(10, (Height - 16) / 2f, 16, 16), color, 1.8f);

            TextRenderer.DrawText(g, Text, Font,
                new Rectangle(34, 0, Math.Max(10, Width - 40), Height), color,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        /// <summary>把页头文字色按比例混向侧栏底色，得到禁用态的实色。</summary>
        private static Color BlendToHeader(Color color, float ratio)
        {
            Color background = Theme.Current.HeaderStart;
            float r = Math.Clamp(ratio, 0f, 1f);
            return Color.FromArgb(
                (int)Math.Round(color.R * r + background.R * (1 - r)),
                (int)Math.Round(color.G * r + background.G * (1 - r)),
                (int)Math.Round(color.B * r + background.B * (1 - r)));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>合并页右侧的名言卡片：磨砂玻璃底、大引号、斜体名言与右下角出处，点击可换一句。</summary>
    public sealed class QuoteCard : Control
    {
        private Language _language = Language.ZhCn;
        private int _index = -1;
        private Quote _quote = new(string.Empty, string.Empty);

        public QuoteCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = false;
            SetLanguage(Loc.Current);
        }

        /// <summary>切换到指定语言的语录池，并随机抽一句作为起始句。</summary>
        public void SetLanguage(Language language)
        {
            _language = language;
            _index = -1;
            NextQuote();
        }

        /// <summary>随机抽一句；只要池里有多句，就不会与当前句重复。</summary>
        public void NextQuote()
        {
            int index = Quotes.NextRandomIndex(_language, _index);
            _index = index;
            _quote = index < 0 ? new Quote(string.Empty, string.Empty) : Quotes.At(_language, index);
            Invalidate();
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        /// <summary>按词换行，避免俄语等长单词被 GDI 从中间截断；中日文无空格时按字符换行。</summary>
        private static List<string> WrapText(Graphics g, string text, Font font, int width)
        {
            const TextFormatFlags measureFlags = TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;

            foreach (string paragraph in text.Split('\n'))
            {
                string remaining = paragraph.Trim();
                while (remaining.Length > 0)
                {
                    int fit = 0;
                    for (int i = 0; i < remaining.Length; i++)
                    {
                        string candidate = remaining.Substring(0, i + 1).TrimEnd();
                        if (TextRenderer.MeasureText(g, candidate, font, Size.Empty, measureFlags).Width > width)
                            break;
                        fit = i + 1;
                    }

                    if (fit >= remaining.Length)
                    {
                        lines.Add(remaining);
                        break;
                    }

                    if (fit <= 0) fit = 1;
                    int cut = remaining.LastIndexOf(' ', Math.Max(0, fit - 1), fit);
                    if (cut <= 0) cut = fit;

                    string line = remaining.Substring(0, cut).TrimEnd();
                    if (line.Length > 0) lines.Add(line);
                    remaining = remaining.Substring(cut).TrimStart();

                    // 极端窄宽下的保护，避免死循环
                    if (lines.Count > 200) return lines;
                }
            }

            return lines;
        }

        protected override void OnClick(EventArgs e)
        {
            NextQuote();
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 60 || Height < 60) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            float radius = Theme.ControlRadius(14);
            using GraphicsPath path = Theme.RoundedRect(bounds, radius);

            // 磨砂玻璃：圆角内取窗口同区域的模糊壁纸，再叠半透明底色与顶部反光
            GraphicsState glass = g.Save();
            g.SetClip(path, CombineMode.Replace);
            GlassPaint.Paint(this, g, Color.Transparent);

            using (var veil = new SolidBrush(Color.FromArgb(Theme.IsDark ? 104 : 128, Theme.Card)))
                g.FillRectangle(veil, ClientRectangle);

            int sheenHeight = Math.Max(2, Height / 2);
            using (var sheen = new LinearGradientBrush(
                new Rectangle(0, 0, Math.Max(1, Width), sheenHeight),
                Color.FromArgb(Theme.IsDark ? 24 : 88, Color.White),
                Color.FromArgb(0, Color.White), 90f))
                g.FillRectangle(sheen, 0, 0, Math.Max(1, Width), sheenHeight);
            g.Restore(glass);

            // 中性描边与顶部反光边缘：不随强调色变化，夜间模式下也不会出现红线
            if (Theme.Surface == SkinSurface.Glass)
            {
                using (var border = new Pen(Theme.GlassBorder, 1f))
                    g.DrawPath(border, path);

                using var edge = new LinearGradientBrush(
                    new RectangleF(0, 0, Math.Max(1, Width), 1f),
                    Theme.GlassEdge, Color.FromArgb(0, Theme.GlassEdge), 0f);
                using var edgePen = new Pen(edge, 1.2f);
                g.DrawLine(edgePen, bounds.Left + radius * 0.6f, 1.2f,
                    bounds.Right - radius * 0.6f, 1.2f);
            }
            else
            {
                SkinStyles.PaintCard(g, path, bounds, radius);
            }

            // 大号开引号，用中性色，避免抢正文
            using (var markFont = new Font("Georgia", Math.Min(36f, Height * 0.26f), FontStyle.Bold))
            using (var markBrush = new SolidBrush(Color.FromArgb(Theme.IsDark ? 66 : 44, Theme.TextPrimary)))
                g.DrawString("\u201C", markFont, markBrush, new PointF(22f, 2f));

            int left = 24;
            int textWidth = Math.Max(20, Width - left - 18);
            int top = 42;
            int sourceTop = Height - 30;
            int quoteHeight = Math.Max(20, sourceTop - top - 4);

            bool slanted = _language is Language.En or Language.Ru;
            float quoteSize = _language == Language.Ru ? 9.7f : 10.2f;
            const float minQuoteSize = 8.8f;
            const TextFormatFlags measureFlags = TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

            // 长句放不下时先略微缩小字号，尽量完整显示，避免直接截断成省略号
            Font textFont = new Font(Theme.BaseFont.FontFamily, quoteSize,
                slanted ? FontStyle.Italic : FontStyle.Regular);
            List<string> lines;
            int lineHeight;
            int maxLines;
            while (true)
            {
                lineHeight = TextRenderer.MeasureText(g, "Ag", textFont, Size.Empty, measureFlags).Height;
                lines = WrapText(g, _quote.Text, textFont, textWidth);
                maxLines = Math.Max(1, (sourceTop - top - 4) / Math.Max(1, lineHeight));
                if (lines.Count <= maxLines || quoteSize <= minQuoteSize) break;

                textFont.Dispose();
                quoteSize = Math.Max(minQuoteSize, quoteSize - 0.3f);
                textFont = new Font(Theme.BaseFont.FontFamily, quoteSize,
                    slanted ? FontStyle.Italic : FontStyle.Regular);
            }

            using (textFont)
            {
                if (lines.Count > maxLines)
                {
                    lines.RemoveRange(maxLines, lines.Count - maxLines);
                    lines[maxLines - 1] = lines[maxLines - 1].TrimEnd() + "…";
                }

                int textTop = top + Math.Max(0, (quoteHeight - lines.Count * lineHeight) / 2);
                for (int i = 0; i < lines.Count; i++)
                {
                    TextRenderer.DrawText(g, lines[i], textFont,
                        new Rectangle(left, textTop + i * lineHeight, textWidth, lineHeight),
                        Theme.TextPrimary,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.SingleLine |
                        TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
                }
            }

            // 出处与正文之间的细分隔线（中性灰，不使用强调色）
            int lineY = sourceTop - 7;
            using (var line = new Pen(Color.FromArgb(Theme.IsDark ? 46 : 30, Theme.TextPrimary), 1f))
                g.DrawLine(line, left, lineY, Width - 18, lineY);

            using (var sourceFont = new Font(Theme.SmallFont.FontFamily, 8.2f))
            {
                TextRenderer.DrawText(g, _quote.Source, sourceFont,
                    new Rectangle(left, sourceTop - 3, textWidth, 21),
                    Theme.TextSecondary,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
        }
    }

    /// <summary>分段选择器：格式 / PPI 等少量互斥选项。</summary>
    public sealed class SegmentedControl : Control
    {
        private string[] _segments = Array.Empty<string>();
        private int _selectedIndex;
        private int _hoverIndex = -1;

        public string[] Segments
        {
            get => _segments;
            set
            {
                _segments = value ?? Array.Empty<string>();
                if (_selectedIndex >= _segments.Length) _selectedIndex = Math.Max(0, _segments.Length - 1);
                Invalidate();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                int next = Math.Clamp(value, 0, Math.Max(0, _segments.Length - 1));
                if (_selectedIndex == next) return;
                _selectedIndex = next;
                Invalidate();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? SelectionChanged;

        public SegmentedControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Height = 32;
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Width <= 2 || Height <= 2) return;
            var outer = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath outerPath = Theme.RoundedRect(outer, Theme.ControlRadius(9));
            using (var fill = new SolidBrush(Theme.GlassInput))
                g.FillPath(fill, outerPath);
            using (var border = new Pen(Theme.Border, 1f))
                g.DrawPath(border, outerPath);

            if (_segments.Length == 0) return;

            float segmentWidth = (Width - 4f) / _segments.Length;
            for (int i = 0; i < _segments.Length; i++)
            {
                var cell = new RectangleF(2 + i * segmentWidth, 2, segmentWidth, Height - 4f);
                bool selected = i == _selectedIndex;
                bool hovered = i == _hoverIndex && Enabled;

                if (selected)
                {
                    var inner = RectangleF.Inflate(cell, -1.5f, -1.5f);
                    using GraphicsPath selectedPath = Theme.RoundedRect(inner, Theme.ControlRadius(7));
                    using var brush = new SolidBrush(Theme.Accent);
                    g.FillPath(brush, selectedPath);
                }
                else if (hovered)
                {
                    var inner = RectangleF.Inflate(cell, -1.5f, -1.5f);
                    using GraphicsPath hoverPath = Theme.RoundedRect(inner, Theme.ControlRadius(7));
                    using var brush = new SolidBrush(Theme.GlassHover);
                    g.FillPath(brush, hoverPath);
                }

                Color color = !Enabled
                    ? Theme.Current.SecondaryDisabledText
                    : selected ? Theme.AccentText : Theme.TextSecondary;
                TextRenderer.DrawText(g, _segments[i], Theme.SmallFont,
                    new Rectangle((int)cell.X, (int)cell.Y, (int)Math.Ceiling(cell.Width), (int)cell.Height),
                    color,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
        }

        private int IndexAt(Point point)
        {
            if (_segments.Length == 0 || Width <= 4 || point.X < 0 || point.X >= Width) return -1;
            int index = (int)((point.X - 2) / ((Width - 4f) / _segments.Length));
            return Math.Clamp(index, 0, _segments.Length - 1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int index = IndexAt(e.Location);
            if (index != _hoverIndex)
            {
                _hoverIndex = index;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hoverIndex = -1;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Enabled)
            {
                int index = IndexAt(e.Location);
                if (index >= 0) SelectedIndex = index;
            }
            base.OnMouseDown(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && _selectedIndex > 0) SelectedIndex = _selectedIndex - 1;
            else if (e.KeyCode == Keys.Right && _selectedIndex < _segments.Length - 1) SelectedIndex = _selectedIndex + 1;
            else base.OnKeyDown(e);
        }
    }

    /// <summary>细滑块：用于 JPEG 质量等 1–100 的取值。</summary>
    public sealed class SlimSlider : Control
    {
        private int _minimum = 1;
        private int _maximum = 100;
        private int _value = 90;
        private bool _dragging;

        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_maximum < _minimum) _maximum = _minimum;
                Value = Math.Clamp(_value, _minimum, _maximum);
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(value, _minimum);
                Value = Math.Clamp(_value, _minimum, _maximum);
                Invalidate();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                int next = Math.Clamp(value, _minimum, _maximum);
                if (_value == next) return;
                _value = next;
                Invalidate();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? ValueChanged;

        public SlimSlider()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Height = 22;
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        private float KnobX => 7f + (Width - 14f) * (_value - _minimum) / Math.Max(1f, _maximum - _minimum);

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float centerY = Height / 2f;
            const float trackHeight = 6f;
            var track = new RectangleF(7f, centerY - trackHeight / 2f, Math.Max(1f, Width - 14f), trackHeight);
            using (GraphicsPath trackPath = Theme.RoundedRect(track, trackHeight / 2f))
            using (var brush = new SolidBrush(Theme.ProgressTrack))
                g.FillPath(brush, trackPath);

            float knobX = KnobX;
            float filled = Math.Max(trackHeight, knobX - track.X);
            if (filled > 0)
            {
                var fillRect = new RectangleF(track.X, track.Y, filled, trackHeight);
                using GraphicsPath fillPath = Theme.RoundedRect(fillRect, trackHeight / 2f);
                using var brush = new LinearGradientBrush(fillRect, Theme.ProgressStart, Theme.Accent, 0f);
                g.FillPath(brush, fillPath);
            }

            var knob = new RectangleF(knobX - 7f, centerY - 7f, 14f, 14f);
            using (var fill = new SolidBrush(Enabled ? Theme.Card : Theme.Current.SecondaryFill))
                g.FillEllipse(fill, knob);
            using var pen = new Pen(Enabled ? Theme.Accent : Theme.Current.SecondaryDisabledText, 2f);
            g.DrawEllipse(pen, knob);
        }

        private void SetFromPoint(int x)
        {
            if (Width <= 14) return;
            float ratio = Math.Clamp((x - 7f) / (Width - 14f), 0f, 1f);
            Value = _minimum + (int)Math.Round(ratio * (_maximum - _minimum));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Enabled)
            {
                _dragging = true;
                Capture = true;
                SetFromPoint(e.X);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging && (e.Button & MouseButtons.Left) != 0) SetFromPoint(e.X);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            Capture = false;
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) Value = _value - 1;
            else if (e.KeyCode == Keys.Right) Value = _value + 1;
            else base.OnKeyDown(e);
        }
    }

    /// <summary>页面预览：按比例居中显示，透明背景时使用棋盘格衬底。</summary>
    public sealed class ImagePreviewBox : Control
    {
        public Image? Image { get; set; }
        public bool Checkerboard { get; set; }
        public string EmptyText { get; set; } = string.Empty;

        public ImagePreviewBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        public void ApplyTheme()
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (Width <= 4 || Height <= 4) return;

            var outer = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath clip = Theme.RoundedRect(outer, 10);
            GraphicsState state = g.Save();
            g.SetClip(clip);

            // 深色皮肤下白纸会和白底连成一片，用画布底色当衬底，再给页面描一圈细边
            if (Checkerboard && Image != null)
            {
                using var light = new SolidBrush(Color.FromArgb(255, 255, 255));
                using var dark = new SolidBrush(Color.FromArgb(226, 228, 232));
                g.FillRectangle(light, outer);
                const int cell = 10;
                for (int y = 0; y < Height; y += cell)
                {
                    for (int x = (y / cell % 2) * cell; x < Width; x += cell * 2)
                        g.FillRectangle(dark, x, y, cell, cell);
                }
            }
            else
            {
                using var backdrop = new SolidBrush(Theme.InputReadOnlyFill);
                g.FillRectangle(backdrop, outer);
            }

            if (Image != null)
            {
                float scale = Math.Min(Width / (float)Image.Width, Height / (float)Image.Height);
                float width = Image.Width * scale;
                float height = Image.Height * scale;
                var target = new RectangleF((Width - width) / 2f, (Height - height) / 2f, width, height);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(Image, target);

                using var pageBorder = new Pen(Color.FromArgb(Theme.IsDark ? 110 : 44, 0, 0, 0), 1f);
                g.DrawRectangle(pageBorder, target.X, target.Y, target.Width - 1f, target.Height - 1f);
            }

            g.Restore(state);

            if (Image == null && EmptyText.Length > 0)
            {
                TextRenderer.DrawText(g, EmptyText, Theme.SmallFont,
                    new Rectangle(8, 0, Width - 16, Height), Theme.TextMuted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
            }

            using var border = new Pen(Theme.Border, 1f);
            using GraphicsPath borderPath = Theme.RoundedRect(outer, 10);
            g.DrawPath(border, borderPath);
        }
    }

    /// <summary>按角色把当前皮肤应用到整棵控件树。</summary>
    public static class SkinManager
    {
        public static void Apply(Control root)
        {
            foreach (Control child in root.Controls)
            {
                switch (child)
                {
                    case RoundedButton button:
                        button.ApplyTheme();
                        break;
                    case RoundedInput input:
                        input.ApplyTheme();
                        break;
                    case GlassCard glass:
                        glass.ApplyTheme();
                        break;
                    case IconButton icon:
                        icon.ApplyTheme();
                        break;
                    case CardPanel card:
                        card.BackColor = Theme.Card;
                        break;
                    case HeaderPanel header:
                        header.BackColor = Theme.HeaderStart;
                        break;
                    case DocumentListBox list:
                        list.ApplyTheme();
                        break;
                    case EmptyStatePanel empty:
                        empty.BackColor = Color.Transparent;
                        break;
                    case SlimProgressBar progress:
                        progress.ApplyTheme();
                        break;
                    case NavCard navCard:
                        navCard.ApplyTheme();
                        break;
                    case SideLink sideLink:
                        sideLink.ApplyTheme();
                        break;
                    case SegmentedControl segments:
                        segments.ApplyTheme();
                        break;
                    case SlimSlider slider:
                        slider.ApplyTheme();
                        break;
                    case ImagePreviewBox preview:
                        preview.ApplyTheme();
                        break;
                    case SkinCheckBox check:
                        check.ForeColor = Theme.TextPrimary;
                        break;
                    case SkinRadioButton radio:
                        radio.ForeColor = Theme.TextPrimary;
                        break;
                    case SideNavPanel side:
                        side.BackColor = Theme.HeaderStart;
                        break;
                    case Label label:
                        label.BackColor = Color.Transparent;
                        label.ForeColor = LabelColor(label.Tag as string, label.ForeColor);
                        break;
                    case Panel panel when panel.Tag is string role && role == "accentBar":
                        panel.BackColor = Theme.Accent;
                        break;
                    case TextBox:
                        // RoundedInput 内部文本框，由父控件统一设置
                        break;
                }

                child.Invalidate();
                Apply(child);
            }
        }

        private static Color LabelColor(string? role, Color fallback)
        {
            switch (role)
            {
                case "title":
                case "section":
                    return Theme.TextPrimary;
                case "secondary":
                    return Theme.TextSecondary;
                case "muted":
                    return Theme.TextMuted;
                case "accent":
                    return Theme.Accent;
                case "header":
                    return Theme.HeaderText;
                case "inverse":
                    return Theme.AccentText;
                default:
                    return fallback;
            }
        }
    }

    public class SkinRadioButton : Control
    {
        private bool _hover;
        private bool _checked;

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                if (value) CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CheckedChanged;

        public SkinRadioButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = Theme.BaseFont;
            Cursor = Cursors.Hand;
            TabStop = true;
            Height = 24;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = 18;
            int top = (Height - size) / 2;
            var ring = new Rectangle(1, top, size, size);
            bool enabled = Enabled;
            Color accent = enabled ? Theme.Accent : Theme.TextMuted;
            Color ringColor = enabled && (_checked || _hover) ? Theme.Accent : Theme.Border;
            using (var brush = new SolidBrush(Theme.GlassInput))
                g.FillEllipse(brush, ring);
            using (var pen = new Pen(ringColor, 1.4f))
                g.DrawEllipse(pen, ring);

            if (_checked)
            {
                using var dot = new SolidBrush(accent);
                g.FillEllipse(dot, ring.X + 5, ring.Y + 5, size - 10, size - 10);
            }

            TextRenderer.DrawText(g, Text, Font,
                new Rectangle(size + 8, 0, Math.Max(10, Width - size - 8), Height),
                enabled ? Theme.TextPrimary : Theme.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            if (!Enabled) _hover = false;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!Enabled) return;
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (!Checked) Checked = true;
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Checked = true;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    public class SkinCheckBox : Control
    {
        private bool _hover;
        private bool _checked;

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CheckedChanged;

        public SkinCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = Theme.BaseFont;
            Cursor = Cursors.Hand;
            TabStop = true;
            Height = 24;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = 18;
            int top = (Height - size) / 2;
            var box = new Rectangle(1, top, size, size);
            using (GraphicsPath path = Theme.RoundedRect(box, Theme.ControlRadius(5)))
            {
                if (_checked)
                {
                    using var fill = new SolidBrush(Enabled ? Theme.Accent : Theme.Border);
                    g.FillPath(fill, path);
                }
                else
                {
                    using var fill = new SolidBrush(Theme.GlassInput);
                    g.FillPath(fill, path);
                    using var border = new Pen(Enabled && _hover ? Theme.Accent : Theme.Border, 1.2f);
                    g.DrawPath(border, path);
                }
            }

            if (_checked)
            {
                using var pen = new Pen(Enabled ? Theme.AccentText : Theme.TextMuted, 1.8f);
                g.DrawLines(pen, new[]
                {
                    new PointF(box.X + 4, box.Y + 9),
                    new PointF(box.X + 7.5f, box.Y + 12.5f),
                    new PointF(box.X + 14, box.Y + 5.5f)
                });
            }

            TextRenderer.DrawText(g, Text, Font,
                new Rectangle(size + 10, 0, Math.Max(10, Width - size - 10), Height),
                Enabled ? Theme.TextPrimary : Theme.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = Enabled;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (!Enabled) return;
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Checked = !Checked;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>皮肤选择卡片。</summary>
    public class SkinCard : Control
    {
        private bool _hover;
        private bool _selected;

        public SkinPalette Palette { get; set; } = SkinPalette.ClassicBlue;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;
                Invalidate();
            }
        }

        public SkinCard()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TabStop = true;
            Size = new Size(136, 76);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using GraphicsPath path = Theme.RoundedRect(rect, Theme.ControlRadius(10));

            using (var fill = new SolidBrush(Palette.Card))
                g.FillPath(fill, path);

            // 顶部皮肤预览条：按皮肤风格叠加渐变、扫描线、网格或斜面
            var headerRect = new RectangleF(0, 0, Width, 30f);
            SkinStyles.PaintSkinPreview(g, path, headerRect, Palette);

            if (!_selected)
            {
                using var accentDot = new SolidBrush(Palette.Accent);
                g.FillEllipse(accentDot, Width - 22, 10, 9, 9);
            }

            TextRenderer.DrawText(g, Loc.SkinName(Palette.Id), Theme.BadgeFont,
                new Rectangle(11, 2, Width - 40, 26), Palette.HeaderText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix |
                TextFormatFlags.EndEllipsis);

            // 名称与说明
            TextRenderer.DrawText(g, Loc.SkinName(Palette.Id), Theme.BaseFont,
                new Rectangle(11, 34, Width - 22, 20), Palette.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix |
                TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(g, Loc.SkinDescription(Palette.Id), Theme.SmallFont,
                new Rectangle(11, 52, Width - 22, 18), Palette.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix |
                TextFormatFlags.EndEllipsis);

            Color border = _selected ? Theme.Accent : _hover ? Theme.Accent : Theme.Border;
            using (var pen = new Pen(border, _selected ? 2f : 1f))
                g.DrawPath(pen, path);

            if (_selected)
            {
                var check = new Rectangle(Width - 24, 4, 16, 16);
                using var circle = new SolidBrush(Theme.Accent);
                g.FillEllipse(circle, check);
                using var tick = new Pen(Theme.AccentText, 1.6f);
                g.DrawLines(tick, new[]
                {
                    new PointF(check.X + 3.5f, check.Y + 8),
                    new PointF(check.X + 6.5f, check.Y + 11),
                    new PointF(check.X + 12f, check.Y + 5)
                });
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            Selected = true;
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                Selected = true;
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    public class GlyphCloseButton : Control
    {
        private bool _hover;

        public GlyphCloseButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Size = new Size(34, 30);
            TabStop = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_hover)
            {
                using var hover = new SolidBrush(Theme.GlassHover);
                g.FillEllipse(hover, 1, 1, Width - 2, Height - 2);
            }

            using var pen = new Pen(Theme.TextSecondary, 1.6f);
            float cx = Width / 2f;
            float cy = Height / 2f;
            float d = 5.5f;
            g.DrawLine(pen, cx - d, cy - d, cx + d, cy + d);
            g.DrawLine(pen, cx + d, cy - d, cx - d, cy + d);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            FindForm()?.Close();
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) FindForm()?.Close();
            base.OnKeyDown(e);
        }
    }

    /// <summary>让右键菜单跟随当前皮肤绘制。</summary>
    public class SkinMenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.Clear(Theme.Card);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using var pen = new Pen(Theme.Border, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected && !e.Item.Pressed) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(2, 1, e.Item.Width - 4, e.Item.Height - 2);
            using GraphicsPath path = Theme.RoundedRect(rect, 5);
            using var brush = new SolidBrush(Theme.ListHover);
            e.Graphics.FillPath(brush, path);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? Theme.TextPrimary : Theme.TextMuted;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Rectangle bounds = e.Item.ContentRectangle;
            int y = bounds.Top + bounds.Height / 2;
            using var pen = new Pen(Theme.Separator, 1f);
            e.Graphics.DrawLine(pen, bounds.Left + 6, y, bounds.Right - 6, y);
        }
    }

    public static class AppIconProvider
    {
        private static Icon? _icon;

        public static Icon Get()
        {
            if (_icon != null) return _icon;

            try
            {
                string? path = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(path))
                {
                    var icon = Icon.ExtractAssociatedIcon(path);
                    if (icon != null)
                    {
                        _icon = icon;
                        return _icon;
                    }
                }
            }
            catch
            {
                // 忽略图标读取失败
            }

            _icon = SystemIcons.Application;
            return _icon;
        }
    }
}
