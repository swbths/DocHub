using System;
using System.Drawing;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>
    /// 加密文件的密码输入框：合并前逐个询问，取消则放弃本次合并。
    /// </summary>
    public sealed class PasswordPromptForm : ThemedDialogForm
    {
        private readonly RoundedInput _input;
        private readonly Label _error;

        public string Password => _input.Text.Trim();

        public PasswordPromptForm(string fileName, string? errorText)
        {
            ClientSize = new Size(400, 232);
            DialogTitle = Loc.T(Txt.MergePasswordTitle);
            Font = Theme.BaseFont;
            BackColor = Theme.Background;
            DoubleBuffered = true;

            var hint = new Label
            {
                Text = Loc.T(Txt.MergePasswordHint),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                AutoSize = false,
                AutoEllipsis = true,
                Bounds = new Rectangle(24, 68, 352, 18)
            };

            var file = new Label
            {
                Text = fileName,
                Font = Theme.ItemFont,
                ForeColor = Theme.TextPrimary,
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                Bounds = new Rectangle(24, 92, 352, 22)
            };

            _input = new RoundedInput
            {
                Bounds = new Rectangle(24, 118, 352, 34)
            };
            _input.Box.UseSystemPasswordChar = true;
            _input.Box.PlaceholderText = Loc.T(Txt.MergePasswordPlaceholder);

            _error = new Label
            {
                Text = errorText ?? string.Empty,
                Font = Theme.SmallFont,
                ForeColor = Theme.IsDark ? Color.FromArgb(255, 122, 112) : Color.FromArgb(206, 76, 76),
                BackColor = Color.Transparent,
                AutoSize = false,
                Bounds = new Rectangle(24, 156, 352, 28)
            };

            var cancel = new RoundedButton
            {
                Text = Loc.T(Txt.Cancel),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(172, 188, 100, 32),
                Radius = 6
            };
            var ok = new RoundedButton
            {
                Text = Loc.T(Txt.Ok),
                Role = ButtonRole.Primary,
                Bounds = new Rectangle(282, 188, 100, 32),
                Radius = 6
            };
            cancel.ApplyTheme();
            ok.ApplyTheme();

            cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
            ok.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };

            KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.AddRange(new Control[] { hint, file, _input, _error, cancel, ok });
            AddCloseButton();
        }
    }
}
