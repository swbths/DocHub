using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>与系统 MessageBox 用法一致，但使用当前皮肤的窗口、文字与按钮样式。</summary>
    public static class ThemedMessageBox
    {
        public static DialogResult Show(IWin32Window? owner, string text, string caption,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            using var dialog = new MessageDialogForm(text, caption, buttons, icon, defaultButton);
            Form? target = owner as Form ?? (owner as Control)?.FindForm();
            return target != null && target.IsHandleCreated
                ? dialog.ShowDialog(target)
                : dialog.ShowDialog();
        }
    }

    internal sealed class MessageDialogForm : ThemedDialogForm
    {
        private const int Pad = 24;
        private const int BadgeSize = 34;
        private const int BadgeGap = 14;
        private const int MaxTextWidth = 430;
        private const int MinButtonWidth = 88;
        private const int ButtonHeight = 34;
        private const int ButtonGap = 10;
        private const int HeaderInset = 20;

        private readonly string _text;
        private readonly MessageBoxIcon _icon;
        private readonly MessageBoxButtons _buttons;
        private readonly MessageBoxDefaultButton _defaultButton;
        private readonly List<RoundedButton> _buttonsCreated = new();
        private readonly Dictionary<RoundedButton, DialogResult> _buttonResults = new();
        private RoundedButton? _defaultControl;
        private int _bodyTop;
        private int _textLeft;
        private int _textWidth;
        private int _textHeight;

        public MessageDialogForm(string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            _text = text ?? string.Empty;
            _icon = icon;
            _buttons = buttons;
            _defaultButton = defaultButton;
            DialogTitle = caption ?? string.Empty;
            BuildUi();
            AddCloseButton();
        }

        private void BuildUi()
        {
            SuspendLayout();

            Size measured = TextRenderer.MeasureText(
                _text, Theme.BaseFont, new Size(MaxTextWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
            _textWidth = Math.Min(MaxTextWidth, Math.Max(1, measured.Width));
            _textHeight = Math.Max(1, measured.Height);

            int iconWidth = _icon == MessageBoxIcon.None ? 0 : BadgeSize + BadgeGap;
            int titleWidth = TextRenderer.MeasureText(
                DialogTitle, Theme.DialogTitleFont, Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix).Width;

            int width = Math.Clamp(
                Math.Max(iconWidth + _textWidth + Pad * 2, titleWidth + 88 + Pad), 400, 560);

            _bodyTop = HeaderHeight + HeaderInset;
            _textLeft = Pad + iconWidth;
            _textWidth = width - _textLeft - Pad;

            int bodyHeight = Math.Max(BadgeSize, _textHeight);
            int buttonTop = _bodyTop + bodyHeight + 22;
            ClientSize = new Size(width, buttonTop + ButtonHeight + HeaderInset);

            BuildButtons(buttonTop);
            Shown += (_, _) => _defaultControl?.Focus();

            ResumeLayout(false);
        }

        private void BuildButtons(int top)
        {
            var order = new List<(string Text, DialogResult Result, bool Primary)>();
            switch (_buttons)
            {
                case MessageBoxButtons.OKCancel:
                    order.Add((Loc.T(Txt.Cancel), DialogResult.Cancel, false));
                    order.Add((Loc.T(Txt.Ok), DialogResult.OK, true));
                    break;
                case MessageBoxButtons.YesNo:
                    order.Add((Loc.T(Txt.No), DialogResult.No, false));
                    order.Add((Loc.T(Txt.Yes), DialogResult.Yes, true));
                    break;
                case MessageBoxButtons.YesNoCancel:
                    order.Add((Loc.T(Txt.Cancel), DialogResult.Cancel, false));
                    order.Add((Loc.T(Txt.No), DialogResult.No, false));
                    order.Add((Loc.T(Txt.Yes), DialogResult.Yes, true));
                    break;
                default:
                    order.Add((Loc.T(Txt.Ok), DialogResult.OK, true));
                    break;
            }

            DialogResult defaultResult = _buttons switch
            {
                MessageBoxButtons.OKCancel => _defaultButton == MessageBoxDefaultButton.Button2
                    ? DialogResult.Cancel : DialogResult.OK,
                MessageBoxButtons.YesNo => _defaultButton == MessageBoxDefaultButton.Button2
                    ? DialogResult.No : DialogResult.Yes,
                MessageBoxButtons.YesNoCancel => _defaultButton switch
                {
                    MessageBoxDefaultButton.Button2 => DialogResult.No,
                    MessageBoxDefaultButton.Button3 => DialogResult.Cancel,
                    _ => DialogResult.Yes
                },
                _ => DialogResult.OK
            };

            int[] widths = order
                .Select(item => Math.Max(MinButtonWidth,
                    TextRenderer.MeasureText(item.Text, Theme.ButtonFont).Width + 30))
                .ToArray();
            int total = widths.Sum() + (order.Count - 1) * ButtonGap;
            int x = ClientSize.Width - Pad - total;

            for (int i = 0; i < order.Count; i++)
            {
                (string text, DialogResult result, bool primary) = order[i];
                var button = new RoundedButton
                {
                    Text = text,
                    Role = primary ? ButtonRole.Primary : ButtonRole.Secondary,
                    Bounds = new Rectangle(x, top, widths[i], ButtonHeight),
                    Radius = 8
                };
                button.ApplyTheme();
                button.Click += (_, _) => Finish(result);
                Controls.Add(button);
                _buttonsCreated.Add(button);
                _buttonResults[button] = result;

                if (result == defaultResult && _defaultControl == null)
                    _defaultControl = button;

                x += widths[i] + ButtonGap;
            }

            _defaultControl ??= _buttonsCreated.FirstOrDefault();
        }

        private DialogResult EscapeResult => _buttons switch
        {
            MessageBoxButtons.OKCancel => DialogResult.Cancel,
            MessageBoxButtons.YesNo => DialogResult.No,
            MessageBoxButtons.YesNoCancel => DialogResult.Cancel,
            _ => DialogResult.OK
        };

        private void Finish(DialogResult result)
        {
            DialogResult = result;
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Finish(_defaultControl != null && _buttonResults.TryGetValue(_defaultControl, out DialogResult result)
                    ? result
                    : DialogResult.OK);
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Finish(EscapeResult);
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_icon != MessageBoxIcon.None)
            {
                float top = _bodyTop + (_textHeight - BadgeSize) / 2f;
                DrawBadge(g, new RectangleF(Pad, top, BadgeSize, BadgeSize));
            }

            TextRenderer.DrawText(g, _text, Theme.BaseFont,
                new Rectangle(_textLeft, _bodyTop, _textWidth, _textHeight + 4),
                Theme.TextPrimary,
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.Top);
        }

        private void DrawBadge(Graphics g, RectangleF badge)
        {
            Color color = _icon switch
            {
                MessageBoxIcon.Error => Theme.IsDark
                    ? Color.FromArgb(255, 122, 112)
                    : Color.FromArgb(198, 58, 58),
                MessageBoxIcon.Warning => Theme.IsDark
                    ? Color.FromArgb(240, 184, 84)
                    : Color.FromArgb(184, 122, 16),
                _ => Theme.Accent
            };

            using (var fill = new SolidBrush(Color.FromArgb(Theme.IsDark ? 58 : 30, color)))
                g.FillEllipse(fill, badge);

            float cx = badge.X + badge.Width / 2f;
            float cy = badge.Y + badge.Height / 2f;

            using var pen = new Pen(color, 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
            using var solid = new SolidBrush(color);

            switch (_icon)
            {
                case MessageBoxIcon.Error:
                    g.DrawLine(pen, cx - 5.5f, cy - 5.5f, cx + 5.5f, cy + 5.5f);
                    g.DrawLine(pen, cx + 5.5f, cy - 5.5f, cx - 5.5f, cy + 5.5f);
                    break;
                case MessageBoxIcon.Warning:
                    using (var triangle = new GraphicsPath())
                    {
                        triangle.AddLine(cx, badge.Y + 8.5f, badge.X + 27.5f, badge.Y + 26f);
                        triangle.AddLine(badge.X + 27.5f, badge.Y + 26f, badge.X + 6.5f, badge.Y + 26f);
                        triangle.CloseFigure();
                        g.DrawPath(pen, triangle);
                    }
                    g.DrawLine(pen, cx, badge.Y + 14f, cx, badge.Y + 20.5f);
                    g.FillEllipse(solid, cx - 1.1f, badge.Y + 22.6f, 2.2f, 2.2f);
                    break;
                case MessageBoxIcon.Question:
                    using (var font = new Font(Theme.DialogTitleFont.FontFamily, 15f, FontStyle.Bold))
                    using (var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    })
                    {
                        g.DrawString("?", font, solid,
                            new RectangleF(badge.X, badge.Y - 1f, badge.Width, badge.Height), format);
                    }
                    break;
                default:
                    g.DrawLine(pen, cx, cy - 3f, cx, cy + 7.5f);
                    g.FillEllipse(solid, cx - 1.2f, cy - 8.7f, 2.4f, 2.4f);
                    break;
            }
        }
    }
}
