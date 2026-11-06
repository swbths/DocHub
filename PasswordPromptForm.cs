using System;
using System.Drawing;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>
    /// 加密文件的密码输入框：合并前逐个询问，取消则放弃本次合并。
    /// </summary>
    public sealed class PasswordPromptForm : Form
    {
        private readonly RoundedInput _input;
        private readonly Label _error;

        public string Password => _input.Text.Trim();

        public PasswordPromptForm(string fileName, string? errorText)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(400, 212);
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            Text = Loc.T(Txt.AppName);
            DoubleBuffered = true;
            KeyPreview = true;

            var title = new Label
            {
                Text = Loc.T(Txt.MergePasswordTitle),
                Font = Theme.DialogTitleFont,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var hint = new Label
            {
                Text = Loc.T(Txt.MergePasswordHint),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                AutoSize = true,
                Location = new Point(24, 52)
            };

            var file = new Label
            {
                Text = fileName,
                Font = Theme.ItemFont,
                ForeColor = Theme.TextPrimary,
                AutoEllipsis = true,
                Bounds = new Rectangle(24, 76, 352, 22)
            };

            _input = new RoundedInput
            {
                Bounds = new Rectangle(24, 104, 352, 34)
            };
            _input.Box.UseSystemPasswordChar = true;
            _input.Box.PlaceholderText = Loc.T(Txt.MergePasswordPlaceholder);

            _error = new Label
            {
                Text = errorText ?? string.Empty,
                Font = Theme.SmallFont,
                ForeColor = Color.FromArgb(206, 76, 76),
                AutoSize = true,
                Location = new Point(24, 146)
            };

            var cancel = new RoundedButton
            {
                Text = Loc.T(Txt.Cancel),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(184, 168, 90, 32),
                Radius = 6
            };
            var ok = new RoundedButton
            {
                Text = Loc.T(Txt.Ok),
                Role = ButtonRole.Primary,
                Bounds = new Rectangle(286, 168, 90, 32),
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

            Controls.AddRange(new Control[] { title, hint, file, _input, _error, cancel, ok });
        }
    }
}
