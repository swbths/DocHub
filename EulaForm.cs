using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DocHub
{
    public sealed class EulaForm : Form
    {
        private const int HeaderHeight = 62;
        private const int WmNcLButtonDown = 0xA1;
        private const int HtCaption = 2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private readonly GlyphCloseButton _closeButton;
        private readonly RoundedButton _agreeButton;

        public EulaForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(660, 600);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            DoubleBuffered = true;

            _closeButton = new GlyphCloseButton
            {
                Bounds = new Rectangle(ClientSize.Width - 54, 16, 34, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _closeButton.Click += (_, _) => Close();

            var title = MakeLabel(Loc.T(Txt.EulaTitle), Theme.SectionFont, "title", 28, 78);
            var intro = MakeLabel(Loc.T(Txt.EulaIntro), Theme.SmallFont, "muted", 28, 106);

            var card = new CardPanel
            {
                Bounds = new Rectangle(28, 134, 604, 374),
                Radius = 14
            };
            var body = new RichTextBox
            {
                Bounds = new Rectangle(18, 14, 568, 346),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                DetectUrls = false,
                WordWrap = true,
                BackColor = Theme.Card,
                ForeColor = Theme.TextSecondary,
                Font = Theme.BaseFont,
                Text = Loc.T(Txt.EulaBody),
                TabStop = false
            };
            card.Controls.Add(body);

            _agreeButton = new RoundedButton
            {
                Text = Loc.T(Txt.EulaAgree),
                Role = ButtonRole.Primary,
                Bounds = new Rectangle(ClientSize.Width - 28 - 190, 530, 190, 38),
                Radius = 9,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _agreeButton.ApplyTheme();
            _agreeButton.Click += (_, _) => Close();

            Controls.AddRange(new Control[] { _closeButton, title, intro, card, _agreeButton });
            SkinManager.Apply(this);
        }

        private Label MakeLabel(string text, Font font, string role, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = Theme.TextPrimary,
                BackColor = Color.Transparent,
                Tag = role,
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var background = new SolidBrush(Theme.Background))
                g.FillRectangle(background, ClientRectangle);

            var headerRect = new RectangleF(0, 0, Width, HeaderHeight);
            using (var headerBrush = new LinearGradientBrush(headerRect, Theme.HeaderStart, Theme.HeaderEnd, 0f))
                g.FillRectangle(headerBrush, headerRect);

            using (var titleBrush = new SolidBrush(Theme.HeaderText))
                g.DrawString(Loc.T(Txt.EulaTitle), Theme.DialogTitleFont, titleBrush, 24f, 17f);

            using (var pen = new Pen(Theme.Border, 1f))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
