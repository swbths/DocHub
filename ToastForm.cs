using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>
    /// 与皮肤一致的右下角完成提醒卡片：从屏幕下方滑入淡入，数秒后自动收起，
    /// 点击「查看」可直接打开合并结果。替换系统托盘气泡，避免与整体风格割裂。
    /// </summary>
    public sealed class ToastForm : Form
    {
        private const int CardWidth = 380;
        private const int CardHeight = 138;
        private const int AutoCloseMs = 6000;

        private readonly string _title;
        private readonly string _subtitle;
        private readonly string _fileName;
        private readonly Action _openAction;
        private readonly RoundedButton _openButton;
        private readonly GlyphCloseButton _closeButton;
        private readonly System.Windows.Forms.Timer _slideTimer = new() { Interval = 15 };
        private readonly System.Windows.Forms.Timer _autoTimer = new() { Interval = AutoCloseMs };
        private int _targetTop;
        private bool _closing;

        public ToastForm(string title, string subtitle, string fileName, Rectangle workingArea, Action openAction)
        {
            _title = title;
            _subtitle = subtitle;
            _fileName = fileName;
            _openAction = openAction;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(CardWidth, CardHeight);
            BackColor = Theme.Card;
            Font = Theme.BaseFont;
            DoubleBuffered = true;
            StartPosition = FormStartPosition.Manual;
            Opacity = 0;

            _openButton = new RoundedButton
            {
                Text = Loc.T(Txt.ToastOpen),
                Role = ButtonRole.Primary,
                Radius = 8,
                Font = Theme.SmallFont,
                Size = new Size(74, 28)
            };
            _openButton.ApplyTheme();
            _openButton.Location = new Point(CardWidth - 16 - _openButton.Width, CardHeight - 16 - _openButton.Height);
            _openButton.Click += (_, _) => InvokeOpen();

            _closeButton = new GlyphCloseButton
            {
                Bounds = new Rectangle(CardWidth - 38, 10, 26, 24)
            };
            _closeButton.Click += (_, _) => HideToast();

            Controls.Add(_openButton);
            Controls.Add(_closeButton);

            _targetTop = workingArea.Bottom - CardHeight - 24;
            Top = workingArea.Bottom + 8;
            Left = workingArea.Right - CardWidth - 24;

            _slideTimer.Tick += (_, _) =>
            {
                if (Top < _targetTop)
                {
                    Top = Math.Min(_targetTop, Top + Math.Max(4, (_targetTop - Top) / 3));
                    Opacity = Math.Min(1, Opacity + 0.16);
                }
                else
                {
                    Top = _targetTop;
                    Opacity = 1;
                    _slideTimer.Stop();
                }
            };

            _autoTimer.Tick += (_, _) =>
            {
                _autoTimer.Stop();
                HideToast();
            };

            MouseEnter += (_, _) => _autoTimer.Stop();
            MouseLeave += (_, _) => { if (!_closing) _autoTimer.Start(); };
        }

        public void ShowToast()
        {
            Show();
            _slideTimer.Start();
            _autoTimer.Start();
        }

        private void InvokeOpen()
        {
            HideToast();
            _openAction();
        }

        /// <summary>淡出后关闭；重复调用无副作用。</summary>
        public void HideToast()
        {
            if (_closing) return;
            _closing = true;
            _slideTimer.Stop();
            _autoTimer.Stop();

            var fade = new System.Windows.Forms.Timer { Interval = 15 };
            fade.Tick += (_, _) =>
            {
                if (Opacity <= 0.08)
                {
                    fade.Stop();
                    fade.Dispose();
                    Close();
                    return;
                }
                Opacity -= 0.12;
                Top += 6;
            };
            fade.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width <= 0 || Height <= 0) return;
            using GraphicsPath path = Theme.RoundedRect(new RectangleF(0, 0, Width, Height), 12);
            Region? old = Region;
            Region = new Region(path);
            old?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (GraphicsPath path = Theme.RoundedRect(bounds, 12))
            {
                using (var fill = new SolidBrush(Theme.Card))
                    g.FillPath(fill, path);

                // 左侧强调色条，与主界面的强调色保持一致
                g.SetClip(path);
                using (var accent = new SolidBrush(Theme.Accent))
                    g.FillRectangle(accent, 0, 0, 4, Height);
                g.ResetClip();

                using var pen = new Pen(Theme.Border, 1f);
                g.DrawPath(pen, path);
            }

            Art.DrawMark(g, new RectangleF(18, 16, 34, 34));

            TextRenderer.DrawText(g, _title, Theme.SectionFont,
                new Rectangle(62, 15, Width - 100, 24), Theme.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            TextRenderer.DrawText(g, _subtitle, Theme.SmallFont,
                new Rectangle(62, 39, Width - 100, 20), Theme.TextMuted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            using (var divider = new Pen(Theme.Separator, 1f))
                g.DrawLine(divider, 16, 72, Width - 16, 72);

            TextRenderer.DrawText(g, _fileName, Theme.SmallFont,
                new Rectangle(18, 82, Width - 110, 22), Theme.TextSecondary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.PathEllipsis | TextFormatFlags.NoPrefix);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _slideTimer.Dispose();
                _autoTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
