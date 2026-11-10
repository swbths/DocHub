using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DocHub
{
    public sealed class AboutForm : Form
    {
        private const int HeaderHeight = 62;
        private const int WmNcLButtonDown = 0xA1;
        private const int HtCaption = 2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private readonly GlyphCloseButton _closeButton;
        private readonly RoundedButton _okButton;
        private readonly RoundedButton _eulaButton;
        private readonly CardPanel _changelogCard;
        private readonly RichTextBox _changelog;
        private readonly Font _changelogHeaderFont;
        private readonly Font _changelogSectionFont;
        private readonly string[] _features;

        public AboutForm()
        {
            _features = new[]
            {
                Loc.T(Txt.Feature1),
                Loc.T(Txt.Feature2),
                Loc.T(Txt.Feature3),
                Loc.T(Txt.Feature4),
                Loc.T(Txt.Feature5),
                Loc.T(Txt.Feature6),
                Loc.T(Txt.Feature7)
            };

            _changelogHeaderFont = new Font(Theme.BaseFont.FontFamily, 9.5f, FontStyle.Bold);
            _changelogSectionFont = new Font(Theme.BaseFont.FontFamily, 9f, FontStyle.Bold);

            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(640, 720);
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

            var titleFont = new Font(Theme.TitleFont.FontFamily, 18f, FontStyle.Bold);
            var brandFont = new Font(Theme.BaseFont.FontFamily, 10.5f, FontStyle.Bold);
            var appName = MakeLabel(Loc.T(Txt.AppName), titleFont, "title", 108, 76);
            var brandDefinition = MakeLabel(Loc.T(Txt.BrandDefinition), brandFont, "accent", 110, 108);

            // 版本号从程序集读取，避免每个语言文件里各写一份造成遗漏
            string versionText = "v0.0.0";
            try
            {
                Version? asmVersion = typeof(AboutForm).Assembly.GetName().Version;
                if (asmVersion != null)
                {
                    versionText = $"v{asmVersion.Major}.{asmVersion.Minor}.{asmVersion.Build}";
                }
            }
            catch
            {
                // 读取失败时保留占位文本
            }

            var version = MakeLabel(string.Format(Loc.T(Txt.VersionLine), versionText),
                Theme.SmallFont, "muted", 110, 134);
            var intro = MakeLabel(Loc.T(Txt.AboutIntro), Theme.SmallFont, "muted", 110, 156);
            Controls.AddRange(new Control[] { appName, brandDefinition, version, intro });

            var changelogTitle = MakeLabel(Loc.T(Txt.ChangelogTitle), Theme.SectionFont, "section", 28, 338);

            _changelogCard = new CardPanel
            {
                Bounds = new Rectangle(28, 364, 584, 230),
                Radius = 14
            };
            _changelog = new RichTextBox
            {
                Bounds = new Rectangle(16, 12, 552, 206),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                DetectUrls = false,
                WordWrap = true,
                BackColor = Theme.Card,
                ForeColor = Theme.TextSecondary,
                Font = Theme.SmallFont,
                TabStop = false
            };
            _changelog.Text = string.Empty;
            RenderChangelog();
            _changelogCard.Controls.Add(_changelog);

            var copyright = MakeLabel(Loc.T(Txt.Copyright), Theme.SmallFont, "muted", 28, 604);

            var author = MakeLabel(Loc.T(Txt.ContactAuthor), Theme.SmallFont, "muted", 28, 626);
            int authorWidth = TextRenderer.MeasureText(author.Text, Theme.SmallFont).Width;
            var email = new LinkLabel
            {
                Text = "cado@163.com",
                Font = Theme.SmallFont,
                AutoSize = true,
                Location = new Point(28 + authorWidth + 10, 626),
                BackColor = Color.Transparent,
                LinkColor = Theme.Accent,
                ActiveLinkColor = Theme.AccentHover,
                VisitedLinkColor = Theme.Accent,
                LinkBehavior = LinkBehavior.HoverUnderline
            };
            email.LinkClicked += (_, _) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "mailto:cado@163.com",
                        UseShellExecute = true
                    });
                }
                catch { /* 本机没有邮件客户端时忽略 */ }
            };

            int eulaWidth = Math.Max(132,
                TextRenderer.MeasureText(Loc.T(Txt.EulaButton), Theme.BaseFont).Width + 30);
            _eulaButton = new RoundedButton
            {
                Text = Loc.T(Txt.EulaButton),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(28, 654, eulaWidth, 36),
                Radius = 9
            };
            _eulaButton.ApplyTheme();
            _eulaButton.Click += (_, _) =>
            {
                using var dialog = new EulaForm();
                dialog.ShowDialog(this);
            };

            _okButton = new RoundedButton
            {
                Text = Loc.T(Txt.Close),
                Role = ButtonRole.Primary,
                Bounds = new Rectangle(526, 654, 86, 36),
                Radius = 9,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _okButton.ApplyTheme();
            _okButton.Click += (_, _) => Close();

            Controls.AddRange(new Control[]
            {
                _closeButton, changelogTitle, _changelogCard, copyright, author, email, _eulaButton, _okButton
            });

            SkinManager.Apply(this);
        }

        /// <summary>
        /// 一次性拼出整段 RTF 再赋值。原来每行都做一次 Select + SelectionColor + SelectionFont，
        /// RichTextBox 每次都要重排全文，构造要 260ms 以上，点「关于」会有明显停顿。
        /// </summary>
        private void RenderChangelog()
        {
            var rtf = new System.Text.StringBuilder(8192);
            rtf.Append(@"{\rtf1\ansi\ansicpg936\deff0");
            rtf.Append(@"{\fonttbl{\f0\fnil\fcharset134 ")
               .Append(RtfEscape(Theme.SmallFont.FontFamily.Name)).Append(";}}");
            rtf.Append(@"{\colortbl ;");
            rtf.Append(RtfColor(Theme.TextPrimary));
            rtf.Append(RtfColor(Theme.TextSecondary));
            rtf.Append(RtfColor(Theme.Accent));
            rtf.Append('}');
            rtf.Append(@"\viewkind4\uc1\pard\f0\fs").Append(HalfPoints(Theme.SmallFont)).Append(' ');

            AppendEntry(rtf, Loc.T(Txt.Changelog4522), Loc.T(Txt.ChangelogItems4522));
            AppendEntry(rtf, Loc.T(Txt.Changelog4521), Loc.T(Txt.ChangelogItems4521));
            AppendEntry(rtf, Loc.T(Txt.Changelog4520), Loc.T(Txt.ChangelogItems4520));
            AppendEntry(rtf, Loc.T(Txt.Changelog4519), Loc.T(Txt.ChangelogItems4519));
            AppendEntry(rtf, Loc.T(Txt.Changelog4518), Loc.T(Txt.ChangelogItems4518));
            AppendEntry(rtf, Loc.T(Txt.Changelog4517), Loc.T(Txt.ChangelogItems4517));
            AppendEntry(rtf, Loc.T(Txt.Changelog4516), Loc.T(Txt.ChangelogItems4516));
            AppendEntry(rtf, Loc.T(Txt.Changelog4515), Loc.T(Txt.ChangelogItems4515));
            AppendEntry(rtf, Loc.T(Txt.Changelog4514), Loc.T(Txt.ChangelogItems4514));
            AppendEntry(rtf, Loc.T(Txt.Changelog4513), Loc.T(Txt.ChangelogItems4513));
            AppendEntry(rtf, Loc.T(Txt.Changelog4512), Loc.T(Txt.ChangelogItems4512));
            AppendEntry(rtf, Loc.T(Txt.Changelog4511), Loc.T(Txt.ChangelogItems4511));
            AppendEntry(rtf, Loc.T(Txt.Changelog4510), Loc.T(Txt.ChangelogItems4510));
            AppendEntry(rtf, Loc.T(Txt.Changelog459), Loc.T(Txt.ChangelogItems459));
            AppendEntry(rtf, Loc.T(Txt.Changelog458), Loc.T(Txt.ChangelogItems458));
            AppendEntry(rtf, Loc.T(Txt.Changelog457), Loc.T(Txt.ChangelogItems457));
            AppendEntry(rtf, Loc.T(Txt.Changelog456), Loc.T(Txt.ChangelogItems456));
            AppendEntry(rtf, Loc.T(Txt.Changelog455), Loc.T(Txt.ChangelogItems455));
            AppendEntry(rtf, Loc.T(Txt.Changelog454), Loc.T(Txt.ChangelogItems454));
            AppendEntry(rtf, Loc.T(Txt.Changelog453), Loc.T(Txt.ChangelogItems453));
            AppendEntry(rtf, Loc.T(Txt.Changelog452), Loc.T(Txt.ChangelogItems452));
            AppendEntry(rtf, Loc.T(Txt.Changelog451), Loc.T(Txt.ChangelogItems451));
            AppendEntry(rtf, Loc.T(Txt.Changelog450), Loc.T(Txt.ChangelogItems450));
            AppendEntry(rtf, Loc.T(Txt.Changelog440), Loc.T(Txt.ChangelogItems440));
            AppendEntry(rtf, Loc.T(Txt.Changelog430), Loc.T(Txt.ChangelogItems430));
            AppendEntry(rtf, Loc.T(Txt.Changelog420), Loc.T(Txt.ChangelogItems420));
            AppendEntry(rtf, Loc.T(Txt.Changelog412), Loc.T(Txt.ChangelogItems412));
            AppendEntry(rtf, Loc.T(Txt.Changelog411), Loc.T(Txt.ChangelogItems411));
            AppendEntry(rtf, Loc.T(Txt.Changelog410), Loc.T(Txt.ChangelogItems410));
            AppendEntry(rtf, Loc.T(Txt.Changelog400), Loc.T(Txt.ChangelogItems400));
            AppendEntry(rtf, Loc.T(Txt.Changelog310), Loc.T(Txt.ChangelogItems310));
            AppendEntry(rtf, Loc.T(Txt.Changelog300), Loc.T(Txt.ChangelogItems300));
            AppendEntry(rtf, Loc.T(Txt.Changelog200), Loc.T(Txt.ChangelogItems200));
            AppendEntry(rtf, Loc.T(Txt.Changelog130), Loc.T(Txt.ChangelogItems130));
            AppendEntry(rtf, Loc.T(Txt.Changelog120), Loc.T(Txt.ChangelogItems120));
            AppendEntry(rtf, Loc.T(Txt.Changelog110), Loc.T(Txt.ChangelogItems110));
            AppendEntry(rtf, Loc.T(Txt.Changelog100), Loc.T(Txt.ChangelogItems100));
            rtf.Append('}');

            _changelog.Rtf = rtf.ToString();
            _changelog.Select(0, 0);
            _changelog.ScrollToCaret();
        }

        private void AppendEntry(System.Text.StringBuilder rtf, string header, string items)
        {
            rtf.Append(@"\pard\sb120\sa60\cf1\fs").Append(HalfPoints(_changelogHeaderFont))
               .Append(@"\b ").Append(RtfText(header)).Append(@"\b0\par");

            foreach (string raw in items.Split('\n'))
            {
                string line = raw.TrimEnd('\r');
                if (line.Length == 0) continue;

                if (line.StartsWith("#", StringComparison.Ordinal))
                {
                    rtf.Append(@"\pard\sa60\sl280\cf3\fs").Append(HalfPoints(_changelogSectionFont))
                       .Append(@"\b ").Append(RtfText(line.Substring(1))).Append(@"\b0\par");
                }
                else
                {
                    rtf.Append(@"\pard\sa60\sl280\cf2\fs").Append(HalfPoints(Theme.SmallFont))
                       .Append(RtfText(line)).Append(@"\par");
                }
            }
        }

        private static int HalfPoints(Font font)
        {
            return Math.Max(8, (int)Math.Round(font.SizeInPoints * 2));
        }

        private static string RtfColor(Color color)
        {
            return @"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + ";";
        }

        /// <summary>RTF 正文转义：特殊字符加反斜杠，非 ASCII 用 \uN? 表示。</summary>
        private static string RtfText(string text)
        {
            var builder = new System.Text.StringBuilder(text.Length + 16);
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\':
                    case '{':
                    case '}':
                        builder.Append('\\').Append(c);
                        break;
                    case '\t':
                        builder.Append(@"\tab ");
                        break;
                    case '\n':
                        builder.Append(@"\par ");
                        break;
                    default:
                        if (c < 128)
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            builder.Append(@"\u").Append((int)c).Append('?');
                        }

                        break;
                }
            }

            return builder.ToString();
        }

        /// <summary>字体名等只需转义，不做 Unicode 转换（字体表里的中文名由 \fcharset134 处理）。</summary>
        private static string RtfEscape(string text)
        {
            var builder = new System.Text.StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c == '\\' || c == '{' || c == '}') builder.Append('\\');
                builder.Append(c);
            }

            return builder.ToString();
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
                g.DrawString(Loc.T(Txt.AboutTitle), Theme.DialogTitleFont, titleBrush, 24f, 17f);

            Art.DrawMark(g, new RectangleF(28, 78, 68, 68));

            using (var divider = new Pen(Theme.Separator, 1f))
                g.DrawLine(divider, 28, 180, Width - 28, 180);

            float y = 192;
            foreach (string feature in _features)
            {
                using (var dot = new SolidBrush(Theme.Accent))
                    g.FillEllipse(dot, 29, y + 6, 6, 6);
                TextRenderer.DrawText(g, feature, Theme.BaseFont,
                    new Rectangle(44, (int)y, Width - 76, 20), Theme.TextSecondary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                y += 22;
            }

            using (var divider = new Pen(Theme.Separator, 1f))
                g.DrawLine(divider, 28, 330, Width - 28, 330);

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
