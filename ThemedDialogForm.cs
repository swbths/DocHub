using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>应用内弹窗基类：无边框、磨砂背景、皮肤标题栏，并支持拖动。</summary>
    public class ThemedDialogForm : Form, IGlassScene
    {
        private const int WmNcLButtonDown = 0xA1;
        private const int HtCaption = 0x2;

        protected const int HeaderHeight = 56;

        private Bitmap? _backdropSharp;
        private Bitmap? _backdropFrosted;
        private string _backdropKey = string.Empty;
        private string _dialogTitle = string.Empty;

        public string DialogTitle
        {
            get => _dialogTitle;
            set
            {
                _dialogTitle = value ?? string.Empty;
                Invalidate();
            }
        }

        protected ThemedDialogForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            DoubleBuffered = true;
            KeyPreview = true;
        }

        /// <summary>在标题栏右上角放置关闭按钮，点击等同取消。</summary>
        protected void AddCloseButton()
        {
            var close = new GlyphCloseButton
            {
                Bounds = new Rectangle(ClientSize.Width - 54, 13, 34, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(close);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildBackdrop();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RebuildBackdrop();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RebuildBackdrop();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!IsHandleCreated || _backdropSharp == null)
            {
                base.OnPaintBackground(e);
                return;
            }
            GlassPaint.Paint(this, e.Graphics, Theme.GlassTint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var header = new SolidBrush(Theme.GlassStrong))
                g.FillRectangle(header, 0, 0, Width, HeaderHeight);

            if (_dialogTitle.Length > 0)
            {
                using var titleBrush = new SolidBrush(Theme.TextPrimary);
                using var format = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_dialogTitle, Theme.DialogTitleFont, titleBrush,
                    new RectangleF(24f, 12f, Math.Max(0, Width - 88), HeaderHeight - 12f), format);
            }

            using (var border = new Pen(Theme.GlassBorder, 1f))
            {
                g.DrawLine(border, 0, HeaderHeight, Width, HeaderHeight);
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Y < HeaderHeight)
            {
                ReleaseCapture();
                SendMessage(Handle, WmNcLButtonDown, (IntPtr)HtCaption, IntPtr.Zero);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.None)
                DialogResult = DialogResult.Cancel;
            base.OnFormClosing(e);
        }

        Bitmap? IGlassScene.SharpBackdrop => _backdropSharp;
        Bitmap? IGlassScene.FrostedBackdrop => _backdropFrosted;

        private void RebuildBackdrop()
        {
            if (!IsHandleCreated) return;
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            string key = Theme.Current.Id + "@" + ClientSize.Width + "x" + ClientSize.Height;
            if (_backdropSharp != null && _backdropKey == key) return;

            GlassPaint.Backdrop backdrop = GlassPaint.GetBackdrop(ClientSize, Theme.Current);
            _backdropSharp = backdrop.Sharp;
            _backdropFrosted = backdrop.Frosted;
            _backdropKey = key;

            GlassPaint.ApplyWindowChrome(this, Theme.IsDark);
            Invalidate(true);
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
