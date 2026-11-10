using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>
    /// 设置窗口：左侧一级目录 + 右侧内容面板。任何改动都即时生效，无需点保存。
    /// </summary>
    public sealed class SettingsForm : Form, IGlassScene
    {
        private const int HeaderHeight = 62;
        private const int NavWidth = 176;
        private const int ContentLeft = 216;
        private const int ContentTop = 78;
        private const int ContentWidth = 624;
        private const int ContentHeight = 490;
        private const int WmNcLButtonDown = 0xA1;
        private const int HtCaption = 2;

        private static readonly int[] ConvertPpiPresets = { 72, 96, 150, 300, 600 };

        /// <summary>设置窗口的固定尺寸；主窗口据此预生成磨砂壁纸，让第一次打开也是秒开。</summary>
        public static readonly Size DialogSize = new(860, 640);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private readonly AppSettings _working;
        private SettingsNavItem[] _navItems = new SettingsNavItem[5];
        private Panel[] _pages = new Panel[5];
        private Label _liveHint = null!;
        private Bitmap? _backdropSharp;
        private Bitmap? _backdropFrosted;
        private string _backdropKey = string.Empty;

        private SkinRadioButton[] _modeRadios = new SkinRadioButton[4];
        private SkinRadioButton[] _outputRadios = new SkinRadioButton[3];
        private SkinRadioButton[] _soundRadios = new SkinRadioButton[6];
        private SkinRadioButton[] _languageRadios = new SkinRadioButton[6];
        private SkinRadioButton[] _presetRadios = new SkinRadioButton[5];
        private SkinRadioButton[] _tintRadios = new SkinRadioButton[3];
        private SkinRadioButton[] _anchorRadios = new SkinRadioButton[6];
        private SkinRadioButton[] _convertFormatRadios = new SkinRadioButton[4];
        private SkinRadioButton[] _convertPpiRadios = new SkinRadioButton[5];
        private RoundedInput _phraseInput = null!;
        private RoundedInput _customInput = null!;
        private RoundedInput _outputInput = null!;
        private RoundedInput _watermarkInput = null!;
        private RoundedInput _convertPpiInput = null!;
        private SkinSlider _convertQualitySlider = null!;
        private SkinCheckBox _checkConvertTransparent = null!;
        private SkinCheckBox _checkConvertSubfolder = null!;
        private SkinSlider _opacitySlider = null!;
        private SkinSlider _scaleSlider = null!;
        private SkinSlider _rotationSlider = null!;
        private SkinSlider _offsetXSlider = null!;
        private SkinSlider _offsetYSlider = null!;
        private Label _preview = null!;
        private SkinCard[] _skinCards = Array.Empty<SkinCard>();
        private SkinCheckBox _checkNotify = null!;
        private SkinCheckBox _checkSound = null!;
        private SkinCheckBox _checkOpen = null!;
        private SkinCheckBox _checkQuotes = null!;
        private SkinCheckBox _checkQuoteAutoRefresh = null!;
        private RoundedInput _quoteIntervalInput = null!;
        private SkinCheckBox _checkWatermark = null!;
        private SkinCheckBox _checkWatermarkPdfImage = null!;
        private SkinCheckBox _checkWatermarkImagePdf = null!;
        private SkinCheckBox _checkWatermarkWordPdf = null!;
        private RoundedButton _outputBrowse = null!;
        private RoundedButton _resetButton = null!;
        private RoundedButton _closeSaveButton = null!;
        private GlyphCloseButton _closeButton = null!;
        private WatermarkPreviewPanel _watermarkPreview = null!;
        private readonly System.Windows.Forms.Timer _applyTimer = new();
        private bool _applyPending;
        private bool _loading = true;

        /// <summary>任一设置项改动后立即回调，用于主窗口即时应用并保存。</summary>
        public event Action<AppSettings>? Applied;

        /// <summary>当前显示的目录页。</summary>
        public int CurrentPage { get; private set; }

        public SettingsForm(AppSettings current, int initialPage = 0)
        {
            _working = current.Clone();
            CurrentPage = Math.Clamp(initialPage, 0, 4);

            _applyTimer.Interval = 240;
            _applyTimer.Tick += (_, _) => FlushApply();

            FormBorderStyle = FormBorderStyle.None;
            ClientSize = DialogSize;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            DoubleBuffered = true;
            KeyPreview = true;

            BuildUi();
        }

        /// <summary>构建全部界面控件；切换语言时在同一个窗口内重建，避免窗口关闭重开造成跳动。</summary>
        private void BuildUi()
        {
            SuspendLayout();

            _closeButton = new GlyphCloseButton
            {
                Bounds = new Rectangle(ClientSize.Width - 54, 16, 34, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _closeButton.Click += (_, _) => CloseSettings();

            var title = MakeLabel(Loc.T(Txt.SettingsTitle), Theme.DialogTitleFont, "header", 24, 16);
            _liveHint = new Label
            {
                Text = Loc.T(Txt.SettingsLiveHint),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = true,
                Location = new Point(24 + TextRenderer.MeasureText(title.Text, title.Font).Width + 14, 24)
            };

            var navCard = new GlassCard
            {
                Bounds = new Rectangle(20, ContentTop, NavWidth, ContentHeight),
                Radius = 14,
                Strong = true
            };

            string[] navTexts =
            {
                Loc.T(Txt.SettingsNavLook),
                Loc.T(Txt.SettingsNavNaming),
                Loc.T(Txt.SettingsNavConvert),
                Loc.T(Txt.SettingsNavDone),
                Loc.T(Txt.SettingsNavWatermark)
            };
            for (int i = 0; i < _navItems.Length; i++)
            {
                int index = i;
                _navItems[i] = new SettingsNavItem
                {
                    Text = navTexts[i],
                    Bounds = new Rectangle(10, 12 + i * 46, NavWidth - 20, 40)
                };
                _navItems[i].Click += (_, _) => ShowPage(index);
                navCard.Controls.Add(_navItems[i]);
            }

            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new GlassCard
                {
                    Bounds = new Rectangle(ContentLeft, ContentTop, ContentWidth, ContentHeight),
                    Radius = 0,
                    DrawBorder = false,
                    Tint = Theme.GlassTint,
                    Sharp = true,
                    Visible = false
                };
                Controls.Add(_pages[i]);
            }

            // ---------- 外观 ----------
            var pageLook = _pages[0];
            var sectionSkin = PageLabel(pageLook, Loc.T(Txt.SkinSection), Theme.SectionFont, "section", 0, 0);
            PageHint(pageLook, Loc.T(Txt.SkinHint), sectionSkin, 0);

            _skinCards = new SkinCard[SkinPalette.All.Count];
            for (int i = 0; i < SkinPalette.All.Count; i++)
            {
                _skinCards[i] = new SkinCard
                {
                    Palette = SkinPalette.All[i],
                    Bounds = new Rectangle((i % 4) * 154, 28 + (i / 4) * 84, 146, 76)
                };
                pageLook.Controls.Add(_skinCards[i]);
            }
            for (int i = 0; i < _skinCards.Length; i++)
            {
                int index = i;
                _skinCards[i].Click += (_, _) =>
                {
                    if (_loading) return;
                    SelectSkin(_skinCards[index]);
                };
            }

            var sectionLanguage = PageLabel(pageLook, Loc.T(Txt.LanguageSection), Theme.SectionFont, "section", 0, 204);
            PageHint(pageLook, Loc.T(Txt.LanguageHint), sectionLanguage, 0);
            for (int i = 0; i < Loc.All.Length; i++)
            {
                _languageRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle((i % 3) * 208, 232 + (i / 3) * 30, 200, 24),
                    Text = Loc.NativeName(Loc.All[i])
                };
                pageLook.Controls.Add(_languageRadios[i]);
            }

            var sectionQuote = PageLabel(pageLook, Loc.T(Txt.QuoteSection), Theme.SectionFont, "section", 0, 304);
            PageHint(pageLook, Loc.T(Txt.QuoteHint), sectionQuote, 0);
            _checkQuotes = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 336, ContentWidth, 26),
                Text = Loc.T(Txt.QuoteToggle)
            };
            pageLook.Controls.Add(_checkQuotes);

            _checkQuoteAutoRefresh = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 370, ContentWidth, 26),
                Text = Loc.T(Txt.QuoteAutoRefresh)
            };
            pageLook.Controls.Add(_checkQuoteAutoRefresh);

            PageLabel(pageLook, Loc.T(Txt.QuoteRefreshInterval), Theme.BaseFont, "secondary", 0, 408);
            _quoteIntervalInput = new RoundedInput
            {
                Bounds = new Rectangle(150, 402, 84, 30)
            };
            _quoteIntervalInput.MaxLength = 5;
            pageLook.Controls.Add(_quoteIntervalInput);
            PageLabel(pageLook, Loc.T(Txt.QuoteRefreshSeconds), Theme.BaseFont, "secondary", 242, 408);
            PageLabel(pageLook, Loc.T(Txt.QuoteRefreshHint), Theme.SmallFont, "muted", 0, 444);

            // ---------- 命名与保存 ----------
            var pageNaming = _pages[1];
            var sectionNaming = PageLabel(pageNaming, Loc.T(Txt.NamingSection), Theme.SectionFont, "section", 0, 0);
            PageHint(pageNaming, Loc.T(Txt.NamingHintShared), sectionNaming, 0);

            string[] optionTexts =
            {
                Loc.T(Txt.NamingFirst),
                Loc.T(Txt.NamingPhrase),
                Loc.T(Txt.NamingTime),
                Loc.T(Txt.NamingCustom)
            };
            for (int i = 0; i < 4; i++)
            {
                _modeRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle(0, 30 + i * 34, 300, 26),
                    Text = optionTexts[i]
                };
                pageNaming.Controls.Add(_modeRadios[i]);
            }

            _phraseInput = new RoundedInput
            {
                Bounds = new Rectangle(320, 60, 300, 30)
            };
            _phraseInput.Box.PlaceholderText = Loc.T(Txt.PhrasePlaceholder);
            _phraseInput.MaxLength = 40;
            pageNaming.Controls.Add(_phraseInput);

            _customInput = new RoundedInput
            {
                Bounds = new Rectangle(320, 126, 300, 30)
            };
            _customInput.Box.PlaceholderText = Loc.T(Txt.CustomPlaceholder);
            _customInput.MaxLength = 120;
            pageNaming.Controls.Add(_customInput);

            _preview = PageLabel(pageNaming, string.Empty, Theme.SmallFont, "muted", 0, 174);

            var sectionOutput = PageLabel(pageNaming, Loc.T(Txt.OutputSection), Theme.SectionFont, "section", 0, 224);
            PageHint(pageNaming, Loc.T(Txt.OutputHint), sectionOutput, 0);

            string[] outputTexts =
            {
                Loc.T(Txt.OutputFirst),
                Loc.T(Txt.OutputLast),
                Loc.T(Txt.OutputCustom)
            };
            for (int i = 0; i < _outputRadios.Length; i++)
            {
                _outputRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle(0, 252 + i * 26, 300, 24),
                    Text = outputTexts[i]
                };
                pageNaming.Controls.Add(_outputRadios[i]);
            }

            _outputInput = new RoundedInput
            {
                Bounds = new Rectangle(320, 330, 220, 30)
            };
            _outputInput.Box.PlaceholderText = Loc.T(Txt.OutputCustomPlaceholder);
            _outputInput.MaxLength = 260;
            pageNaming.Controls.Add(_outputInput);

            _outputBrowse = new RoundedButton
            {
                Text = Loc.T(Txt.Browse),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(552, 330, 68, 30),
                Radius = 8,
                Font = Theme.SmallFont
            };
            _outputBrowse.ApplyTheme();
            pageNaming.Controls.Add(_outputBrowse);

            // ---------- PDF 转图片 ----------
            var pageConvert = _pages[2];
            var sectionConvert = PageLabel(pageConvert, Loc.T(Txt.ConvertSection), Theme.SectionFont, "section", 0, 0);
            PageHint(pageConvert, Loc.T(Txt.ConvertDefaultsHint), sectionConvert, 0);

            PageLabel(pageConvert, Loc.T(Txt.ConvertDefaultFormat), Theme.BaseFont, "secondary", 0, 30);
            string[] formatTexts = { "PNG", "JPEG", "BMP", "TIFF" };
            for (int i = 0; i < _convertFormatRadios.Length; i++)
            {
                _convertFormatRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle(i * 104, 54, 100, 24),
                    Text = formatTexts[i]
                };
                pageConvert.Controls.Add(_convertFormatRadios[i]);
            }

            PageLabel(pageConvert, Loc.T(Txt.ConvertDefaultPpi), Theme.BaseFont, "secondary", 0, 96);
            for (int i = 0; i < _convertPpiRadios.Length; i++)
            {
                _convertPpiRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle(i * 76, 120, 72, 24),
                    Text = ConvertPpiPresets[i].ToString()
                };
                pageConvert.Controls.Add(_convertPpiRadios[i]);
            }

            _convertPpiInput = new RoundedInput
            {
                Bounds = new Rectangle(392, 118, 140, 30)
            };
            _convertPpiInput.Box.PlaceholderText = Loc.T(Txt.ConvertPpiCustom);
            _convertPpiInput.MaxLength = 4;
            pageConvert.Controls.Add(_convertPpiInput);

            PageLabel(pageConvert, Loc.T(Txt.ConvertDefaultQuality), Theme.BaseFont, "secondary", 0, 168);
            _convertQualitySlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 192, 420, 30),
                Minimum = 1,
                Maximum = 100,
                SmallChange = 1
            };
            pageConvert.Controls.Add(_convertQualitySlider);

            _checkConvertTransparent = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 232, 420, 26),
                Text = Loc.T(Txt.ConvertDefaultBackground)
            };
            pageConvert.Controls.Add(_checkConvertTransparent);
            PageLabel(pageConvert, Loc.T(Txt.ConvertBackgroundHint), Theme.SmallFont, "muted", 28, 258);

            _checkConvertSubfolder = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 288, 460, 26),
                Text = Loc.T(Txt.ConvertDefaultSubfolder)
            };
            pageConvert.Controls.Add(_checkConvertSubfolder);
            PageLabel(pageConvert, Loc.T(Txt.ConvertSubfolderHint), Theme.SmallFont, "muted", 28, 314);

            PageLabel(pageConvert, Loc.T(Txt.ConvertWatermarkNote), Theme.SmallFont, "muted", 0, 352);

            // ---------- 完成提醒 ----------
            var pageDone = _pages[3];
            var sectionDone = PageLabel(pageDone, Loc.T(Txt.DoneSection), Theme.SectionFont, "section", 0, 0);
            PageHint(pageDone, Loc.T(Txt.SoundHint), sectionDone, 0);

            _checkNotify = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 30, ContentWidth, 26),
                Text = Loc.T(Txt.DoneNotify)
            };
            _checkSound = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 58, ContentWidth, 26),
                Text = Loc.T(Txt.DoneSound)
            };
            _checkOpen = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 86, ContentWidth, 26),
                Text = Loc.T(Txt.DoneOpenShared)
            };
            pageDone.Controls.AddRange(new Control[] { _checkNotify, _checkSound, _checkOpen });

            var sectionSound = PageLabel(pageDone, Loc.T(Txt.SoundSection), Theme.SectionFont, "section", 0, 132);

            for (int i = 0; i < SoundLibrary.All.Length; i++)
            {
                _soundRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle((i % 3) * 208, 164 + (i / 3) * 30, 196, 24),
                    Text = Loc.SoundName(SoundLibrary.All[i])
                };
                pageDone.Controls.Add(_soundRadios[i]);
            }

            // ---------- 水印 ----------
            var pageMark = _pages[4];
            var sectionMark = PageLabel(pageMark, Loc.T(Txt.WatermarkSection), Theme.SectionFont, "section", 0, 0);
            PageHint(pageMark, Loc.T(Txt.WatermarkHint), sectionMark, 0);

            _checkWatermark = new SkinCheckBox
            {
                Bounds = new Rectangle(0, 30, 150, 26),
                Text = Loc.T(Txt.WatermarkForMerge)
            };
            _checkWatermarkPdfImage = new SkinCheckBox
            {
                Bounds = new Rectangle(156, 30, 150, 26),
                Text = Loc.T(Txt.WatermarkForPdfImage)
            };
            _checkWatermarkImagePdf = new SkinCheckBox
            {
                Bounds = new Rectangle(312, 30, 150, 26),
                Text = Loc.T(Txt.WatermarkForImagePdf)
            };
            _checkWatermarkWordPdf = new SkinCheckBox
            {
                Bounds = new Rectangle(468, 30, 150, 26),
                Text = Loc.T(Txt.WatermarkForWordPdf)
            };
            pageMark.Controls.AddRange(new Control[]
            {
                _checkWatermark, _checkWatermarkPdfImage, _checkWatermarkImagePdf,
                _checkWatermarkWordPdf
            });

            PageLabel(pageMark, Loc.T(Txt.WatermarkPresets), Theme.BaseFont, "secondary", 0, 64);

            string[] presetTexts =
            {
                Loc.T(Txt.WatermarkPreset1),
                Loc.T(Txt.WatermarkPreset2),
                Loc.T(Txt.WatermarkPreset3),
                Loc.T(Txt.WatermarkPreset4),
                Loc.T(Txt.WatermarkCustom)
            };
            for (int i = 0; i < _presetRadios.Length; i++)
            {
                _presetRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle((i % 3) * 104, 84 + (i / 3) * 24, 100, 24),
                    Text = presetTexts[i]
                };
                pageMark.Controls.Add(_presetRadios[i]);
            }

            _watermarkInput = new RoundedInput
            {
                Bounds = new Rectangle(0, 136, 300, 30)
            };
            _watermarkInput.Box.PlaceholderText = Loc.T(Txt.WatermarkCustomPlaceholder);
            _watermarkInput.MaxLength = 40;
            pageMark.Controls.Add(_watermarkInput);

            // 位置：平铺整页，或固定到页面某一处
            PageLabel(pageMark, Loc.T(Txt.WatermarkPosition), Theme.BaseFont, "secondary", 0, 170);
            string[] anchorTexts =
            {
                Loc.T(Txt.WatermarkPosTile),
                Loc.T(Txt.WatermarkPosTopLeft),
                Loc.T(Txt.WatermarkPosTopRight),
                Loc.T(Txt.WatermarkPosCenter),
                Loc.T(Txt.WatermarkPosBottomLeft),
                Loc.T(Txt.WatermarkPosBottomRight)
            };
            for (int i = 0; i < _anchorRadios.Length; i++)
            {
                _anchorRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle((i % 3) * 100, 190 + (i / 3) * 24, 96, 24),
                    Text = anchorTexts[i]
                };
                pageMark.Controls.Add(_anchorRadios[i]);
            }

            PageLabel(pageMark, Loc.T(Txt.WatermarkTint), Theme.BaseFont, "secondary", 0, 244);
            string[] tintTexts =
            {
                Loc.T(Txt.WatermarkTintGray),
                Loc.T(Txt.WatermarkTintRed),
                Loc.T(Txt.WatermarkTintBlue)
            };
            for (int i = 0; i < _tintRadios.Length; i++)
            {
                _tintRadios[i] = new SkinRadioButton
                {
                    Bounds = new Rectangle(i * 100, 264, 96, 24),
                    Text = tintTexts[i]
                };
                pageMark.Controls.Add(_tintRadios[i]);
            }

            _opacitySlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 292, 300, 30),
                Label = Loc.T(Txt.WatermarkOpacity),
                Minimum = 0,
                Maximum = 100,
                SmallChange = 1
            };
            _scaleSlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 322, 300, 30),
                Label = Loc.T(Txt.WatermarkScale),
                Minimum = 10,
                Maximum = 400,
                SmallChange = 2
            };
            _rotationSlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 352, 300, 30),
                Label = Loc.T(Txt.WatermarkRotation),
                Minimum = -180,
                Maximum = 180,
                SmallChange = 1,
                Suffix = "\u00B0"
            };
            _offsetXSlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 382, 300, 30),
                Label = Loc.T(Txt.WatermarkOffsetX),
                Minimum = -100,
                Maximum = 100,
                SmallChange = 1
            };
            _offsetYSlider = new SkinSlider
            {
                Bounds = new Rectangle(0, 412, 300, 30),
                Label = Loc.T(Txt.WatermarkOffsetY),
                Minimum = -100,
                Maximum = 100,
                SmallChange = 1
            };
            pageMark.Controls.AddRange(new Control[]
            {
                _opacitySlider, _scaleSlider, _rotationSlider, _offsetXSlider, _offsetYSlider
            });

            var previewHint = new Label
            {
                Text = Loc.T(Txt.WatermarkPreviewHint),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = false,
                Bounds = new Rectangle(0, 446, 300, 40)
            };
            pageMark.Controls.Add(previewHint);

            PageLabel(pageMark, Loc.T(Txt.WatermarkPreview), Theme.BaseFont, "secondary", 336, 64);
            _watermarkPreview = new WatermarkPreviewPanel
            {
                // 内页留白 14px 后接近 A4 比例，预览与合并后的纸张观感一致
                Bounds = new Rectangle(336, 90, 282, 380)
            };
            pageMark.Controls.Add(_watermarkPreview);

            // 预览里直接拖动/滚轮/右键调整后，把结果回写到滑块与设置对象
            _watermarkPreview.SpecEdited += spec =>
            {
                if (_loading) return;

                _working.WatermarkScale = spec.ScalePercent;
                _working.WatermarkRotation = spec.RotationDegrees;
                _working.WatermarkOffsetX = spec.OffsetXPercent;
                _working.WatermarkOffsetY = spec.OffsetYPercent;

                _loading = true;
                _scaleSlider.Value = spec.ScalePercent;
                _rotationSlider.Value = spec.RotationDegrees;
                _offsetXSlider.Value = spec.OffsetXPercent;
                _offsetYSlider.Value = spec.OffsetYPercent;
                _loading = false;

                NotifyApplied();
            };

            // 导航与内容页之外的底部按钮
            _resetButton = new RoundedButton
            {
                Text = Loc.T(Txt.Reset),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(20, 582, 112, 38),
                Radius = 6
            };
            _closeSaveButton = new RoundedButton
            {
                Text = Loc.T(Txt.Close),
                Role = ButtonRole.Primary,
                Bounds = new Rectangle(ClientSize.Width - 24 - 96, 582, 96, 38),
                Radius = 6,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _resetButton.ApplyTheme();
            _closeSaveButton.ApplyTheme();
            _resetButton.Width = Math.Max(112,
                TextRenderer.MeasureText(_resetButton.Text, _resetButton.Font).Width + 30);

            Controls.AddRange(new Control[]
            {
                _closeButton, _liveHint, navCard, _resetButton, _closeSaveButton
            });

            WireEvents();
            LoadWorkingToUi();
            ShowPage(CurrentPage);

            Theme.ApplySkin(SkinPalette.FromId(_working.SkinId));
            SkinManager.Apply(this);
            ResumeLayout(true);
            Invalidate(true);
        }

        // ---------------- 辅助创建 ----------------

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

        private Label PageLabel(Panel page, string text, Font font, string role, int x, int y)
        {
            Label label = MakeLabel(text, font, role, x, y);
            page.Controls.Add(label);
            return label;
        }

        /// <summary>分区标题右侧的灰色说明文字，超宽时自动截断在内容区右侧。</summary>
        private void PageHint(Panel page, string text, Label section, int maxRight)
        {
            var hint = new Label
            {
                Text = text,
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = true
            };

            int sectionWidth = section.PreferredWidth;
            int x = section.Left + sectionWidth + 18;
            if (x + hint.PreferredWidth > maxRight)
                x = Math.Max(section.Left + sectionWidth + 18, maxRight - hint.PreferredWidth);
            hint.Location = new Point(x, section.Top + 7);
            page.Controls.Add(hint);
        }

        // ---------------- 事件 ----------------

        private void WireEvents()
        {
            for (int i = 0; i < _modeRadios.Length; i++)
            {
                int index = i;
                _modeRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_modeRadios[index].Checked) return;
                    UncheckOthers(_modeRadios, index);
                    _working.NamingMode = (NamingMode)index;
                    UpdateEnabledStates();
                    UpdatePreview();
                    NotifyApplied();
                };
            }

            _phraseInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                _working.NamePhrase = _phraseInput.Text;
                UpdatePreview();
                NotifyApplied();
            };
            _customInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                _working.CustomName = _customInput.Text;
                UpdatePreview();
                NotifyApplied();
            };

            for (int i = 0; i < _soundRadios.Length; i++)
            {
                int index = i;
                _soundRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_soundRadios[index].Checked) return;
                    UncheckOthers(_soundRadios, index);
                    _working.SoundId = SoundLibrary.All[index];
                    if (_checkSound.Checked) SoundLibrary.Play(_working.SoundId);
                    NotifyApplied();
                };
            }

            for (int i = 0; i < _languageRadios.Length; i++)
            {
                int index = i;
                _languageRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_languageRadios[index].Checked) return;
                    UncheckOthers(_languageRadios, index);
                    SwitchLanguage(Loc.All[index]);
                };
            }

            for (int i = 0; i < _outputRadios.Length; i++)
            {
                int index = i;
                _outputRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_outputRadios[index].Checked) return;
                    UncheckOthers(_outputRadios, index);
                    _working.OutputLocationMode = (OutputLocationMode)index;
                    UpdateEnabledStates();
                    NotifyApplied();
                };
            }

            _outputInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                _working.CustomOutputFolder = _outputInput.Text;
                NotifyApplied();
            };

            _outputBrowse.Click += (_, _) =>
            {
                ChooseOutputFolder();
                NotifyApplied();
            };

            _checkNotify.Click += (_, _) =>
            {
                if (_loading) return;
                _working.ShowNotification = _checkNotify.Checked;
                NotifyApplied();
            };
            _checkSound.Click += (_, _) =>
            {
                if (_loading) return;
                _working.PlaySound = _checkSound.Checked;
                UpdateEnabledStates();
                if (_checkSound.Checked) SoundLibrary.Play(_working.SoundId);
                NotifyApplied();
            };
            _checkOpen.Click += (_, _) =>
            {
                if (_loading) return;
                _working.OpenAfterMerge = _checkOpen.Checked;
                NotifyApplied();
            };
            _checkQuotes.Click += (_, _) =>
            {
                if (_loading) return;
                _working.ShowQuotes = _checkQuotes.Checked;
                UpdateEnabledStates();
                NotifyApplied();
            };

            _checkQuoteAutoRefresh.Click += (_, _) =>
            {
                if (_loading) return;
                _working.QuoteAutoRefresh = _checkQuoteAutoRefresh.Checked;
                UpdateEnabledStates();
                NotifyApplied();
            };

            _quoteIntervalInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                if (!int.TryParse(_quoteIntervalInput.Text.Trim(), out int seconds)) return;
                if (seconds < AppSettings.MinQuoteRefreshSeconds || seconds > AppSettings.MaxQuoteRefreshSeconds) return;

                _working.QuoteRefreshSeconds = seconds;
                NotifyApplied();
            };

            for (int i = 0; i < _convertFormatRadios.Length; i++)
            {
                int index = i;
                _convertFormatRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_convertFormatRadios[index].Checked) return;
                    UncheckOthers(_convertFormatRadios, index);
                    _working.ConvertFormat = (ConvertImageFormat)index;
                    UpdateEnabledStates();
                    UpdatePreview();
                    NotifyApplied();
                };
            }

            for (int i = 0; i < _convertPpiRadios.Length; i++)
            {
                int index = i;
                _convertPpiRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_convertPpiRadios[index].Checked) return;
                    UncheckOthers(_convertPpiRadios, index);
                    _working.ConvertPpi = ConvertPpiPresets[index];
                    _loading = true;
                    _convertPpiInput.Text = string.Empty;
                    _loading = false;
                    NotifyApplied();
                };
            }

            _convertPpiInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                if (!int.TryParse(_convertPpiInput.Text.Trim(), out int ppi)) return;
                if (ppi < PdfImageEngine.MinPpi || ppi > PdfImageEngine.MaxPpi) return;

                _working.ConvertPpi = ppi;
                _loading = true;
                foreach (var radio in _convertPpiRadios) radio.Checked = false;
                _loading = false;
                NotifyApplied();
            };

            _convertQualitySlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.ConvertJpegQuality = _convertQualitySlider.Value;
                NotifyApplied();
            };

            _checkConvertTransparent.Click += (_, _) =>
            {
                if (_loading) return;
                _working.ConvertTransparentBackground = _checkConvertTransparent.Checked;
                NotifyApplied();
            };

            _checkConvertSubfolder.Click += (_, _) =>
            {
                if (_loading) return;
                _working.ConvertCreateSubfolder = _checkConvertSubfolder.Checked;
                NotifyApplied();
            };

            _checkWatermark.Click += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkEnabled = _checkWatermark.Checked;
                UpdateEnabledStates();
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _checkWatermarkPdfImage.Click += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkForPdfImage = _checkWatermarkPdfImage.Checked;
                UpdateEnabledStates();
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _checkWatermarkImagePdf.Click += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkForImagePdf = _checkWatermarkImagePdf.Checked;
                UpdateEnabledStates();
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _checkWatermarkWordPdf.Click += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkForWordPdf = _checkWatermarkWordPdf.Checked;
                UpdateEnabledStates();
                UpdateWatermarkPreview();
                NotifyApplied();
            };

            for (int i = 0; i < _presetRadios.Length; i++)
            {
                int index = i;
                _presetRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_presetRadios[index].Checked) return;
                    UncheckOthers(_presetRadios, index);
                    _working.WatermarkPreset = index;
                    UpdateEnabledStates();
                    UpdateWatermarkPreview();
                    NotifyApplied();
                };
            }

            _watermarkInput.TextChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkCustomText = _watermarkInput.Text;
                UpdateWatermarkPreview();
                NotifyApplied();
            };

            for (int i = 0; i < _anchorRadios.Length; i++)
            {
                int index = i;
                _anchorRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_anchorRadios[index].Checked) return;
                    UncheckOthers(_anchorRadios, index);
                    _working.WatermarkAnchor = (WatermarkAnchor)index;
                    UpdateWatermarkPreview();
                    NotifyApplied();
                };
            }

            for (int i = 0; i < _tintRadios.Length; i++)
            {
                int index = i;
                _tintRadios[i].CheckedChanged += (_, _) =>
                {
                    if (_loading || !_tintRadios[index].Checked) return;
                    UncheckOthers(_tintRadios, index);
                    _working.WatermarkTint = (WatermarkTint)index;
                    UpdateWatermarkPreview();
                    NotifyApplied();
                };
            }

            _opacitySlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkOpacity = _opacitySlider.Value;
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _scaleSlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkScale = _scaleSlider.Value;
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _rotationSlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkRotation = _rotationSlider.Value;
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _offsetXSlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkOffsetX = _offsetXSlider.Value;
                UpdateWatermarkPreview();
                NotifyApplied();
            };
            _offsetYSlider.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _working.WatermarkOffsetY = _offsetYSlider.Value;
                UpdateWatermarkPreview();
                NotifyApplied();
            };

            _resetButton.Click += (_, _) => ResetToDefaults();
            _closeSaveButton.Click += (_, _) => CloseSettings();
        }

        private static void UncheckOthers(SkinRadioButton[] radios, int index)
        {
            for (int j = 0; j < radios.Length; j++)
            {
                if (j != index && radios[j].Checked) radios[j].Checked = false;
            }
        }

        // ---------------- 数据同步 ----------------

        private void LoadWorkingToUi()
        {
            _loading = true;
            for (int i = 0; i < _modeRadios.Length; i++)
                _modeRadios[i].Checked = (int)_working.NamingMode == i;

            _phraseInput.Text = _working.NamePhrase ?? string.Empty;
            _customInput.Text = _working.CustomName ?? string.Empty;

            foreach (var card in _skinCards)
                card.Selected = string.Equals(card.Palette.Id, _working.SkinId, StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < _soundRadios.Length; i++)
                _soundRadios[i].Checked = string.Equals(
                    SoundLibrary.All[i], _working.SoundId, StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < _languageRadios.Length; i++)
                _languageRadios[i].Checked = Loc.All[i] == _working.Language;

            for (int i = 0; i < _outputRadios.Length; i++)
                _outputRadios[i].Checked = (int)_working.OutputLocationMode == i;

            _outputInput.Text = _working.CustomOutputFolder ?? string.Empty;

            _checkNotify.Checked = _working.ShowNotification;
            _checkSound.Checked = _working.PlaySound;
            _checkOpen.Checked = _working.OpenAfterMerge;
            _checkQuotes.Checked = _working.ShowQuotes;
            _checkQuoteAutoRefresh.Checked = _working.QuoteAutoRefresh;
            _quoteIntervalInput.Text = Math.Clamp(_working.QuoteRefreshSeconds,
                AppSettings.MinQuoteRefreshSeconds, AppSettings.MaxQuoteRefreshSeconds).ToString();

            for (int i = 0; i < _convertFormatRadios.Length; i++)
                _convertFormatRadios[i].Checked = (int)_working.ConvertFormat == i;

            int currentPpi = Math.Clamp(_working.ConvertPpi, PdfImageEngine.MinPpi, PdfImageEngine.MaxPpi);
            int ppiPreset = Array.IndexOf(ConvertPpiPresets, currentPpi);
            for (int i = 0; i < _convertPpiRadios.Length; i++)
                _convertPpiRadios[i].Checked = i == ppiPreset;
            _convertPpiInput.Text = ppiPreset >= 0 ? string.Empty : currentPpi.ToString();

            _convertQualitySlider.Value = Math.Clamp(_working.ConvertJpegQuality, 1, 100);
            _checkConvertTransparent.Checked = _working.ConvertTransparentBackground;
            _checkConvertSubfolder.Checked = _working.ConvertCreateSubfolder;

            _checkWatermark.Checked = _working.WatermarkEnabled;
            _checkWatermarkPdfImage.Checked = _working.WatermarkForPdfImage;
            _checkWatermarkImagePdf.Checked = _working.WatermarkForImagePdf;
            _checkWatermarkWordPdf.Checked = _working.WatermarkForWordPdf;
            for (int i = 0; i < _presetRadios.Length; i++)
                _presetRadios[i].Checked = _working.WatermarkPreset == i;
            _watermarkInput.Text = _working.WatermarkCustomText ?? string.Empty;
            for (int i = 0; i < _anchorRadios.Length; i++)
                _anchorRadios[i].Checked = (int)_working.WatermarkAnchor == i;
            for (int i = 0; i < _tintRadios.Length; i++)
                _tintRadios[i].Checked = (int)_working.WatermarkTint == i;

            _opacitySlider.Value = _working.WatermarkOpacity;
            _scaleSlider.Value = _working.WatermarkScale;
            _rotationSlider.Value = _working.WatermarkRotation;
            _offsetXSlider.Value = _working.WatermarkOffsetX;
            _offsetYSlider.Value = _working.WatermarkOffsetY;

            _loading = false;

            UpdateEnabledStates();
            UpdatePreview();
            UpdateWatermarkPreview();
        }

        private void UpdateEnabledStates()
        {
            _phraseInput.Enabled = _working.NamingMode == NamingMode.FirstPlusPhrase;
            _phraseInput.ApplyTheme();
            _customInput.Enabled = _working.NamingMode == NamingMode.Custom;
            _customInput.ApplyTheme();

            bool customOutput = _working.OutputLocationMode == OutputLocationMode.Custom;
            _outputInput.Enabled = customOutput;
            _outputInput.ApplyTheme();
            _outputBrowse.Enabled = customOutput;

            _checkConvertTransparent.Enabled = _working.ConvertFormat == ConvertImageFormat.Png;
            _checkConvertTransparent.Invalidate();

            foreach (var radio in _soundRadios)
            {
                radio.Enabled = _working.PlaySound;
                radio.Invalidate();
            }

            bool quotesOn = _working.ShowQuotes;
            _checkQuoteAutoRefresh.Enabled = quotesOn;
            _checkQuoteAutoRefresh.Invalidate();
            _quoteIntervalInput.Enabled = quotesOn && _working.QuoteAutoRefresh;
            _quoteIntervalInput.ApplyTheme();

            bool watermarkOn = _working.WatermarkAnyEnabled;
            bool customText = _working.WatermarkPreset == 4 && watermarkOn;
            _watermarkInput.Enabled = customText;
            _watermarkInput.ApplyTheme();

            foreach (var radio in _presetRadios)
            {
                radio.Enabled = watermarkOn;
                radio.Invalidate();
            }
            foreach (var radio in _tintRadios)
            {
                radio.Enabled = watermarkOn;
                radio.Invalidate();
            }
            foreach (var slider in new[] { _opacitySlider, _scaleSlider, _rotationSlider, _offsetXSlider, _offsetYSlider })
            {
                slider.Enabled = watermarkOn;
                slider.ApplyTheme();
            }
            foreach (var radio in _anchorRadios)
            {
                radio.Enabled = watermarkOn;
                radio.Invalidate();
            }
        }

        private void ChooseOutputFolder()
        {
            string start = (_working.CustomOutputFolder ?? string.Empty).Trim();
            if (start.Length == 0 && _working.LastOutputFolder.Trim().Length > 0)
                start = _working.LastOutputFolder.Trim();

            var dialog = new FolderBrowserDialog
            {
                Description = Loc.T(Txt.FolderDesc),
                UseDescriptionForTitle = true,
                SelectedPath = start,
                ShowNewFolderButton = true
            };
            try
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                _outputInput.Text = dialog.SelectedPath;
                _working.CustomOutputFolder = dialog.SelectedPath;
            }
            finally
            {
                dialog.Dispose();
            }
        }

        private void UpdatePreview()
        {
            DateTime now = DateTime.Now;
            string mergeStem = SanitizeStem(_working.BuildNameStem(string.Empty, now));
            string convertStem = SanitizeStem(_working.BuildNameStem(string.Empty, now, forConversion: true));
            string extension = PdfImageEngine.Extension(_working.ConvertFormat);
            _preview.Text = Loc.T(Txt.PreviewFormat, mergeStem) + Environment.NewLine +
                Loc.T(Txt.ConvertNamePreview, convertStem + "_p001" + extension);
        }

        private static string SanitizeStem(string stem)
        {
            foreach (char invalid in System.IO.Path.GetInvalidFileNameChars())
                stem = stem.Replace(invalid, '_');
            return stem;
        }

        private string ResolveWatermarkText()
        {
            switch (_working.WatermarkPreset)
            {
                case 1: return Loc.T(Txt.WatermarkPreset2);
                case 2: return Loc.T(Txt.WatermarkPreset3);
                case 3: return Loc.T(Txt.WatermarkPreset4);
                case 4: return (_working.WatermarkCustomText ?? string.Empty).Trim();
                default: return Loc.T(Txt.WatermarkPreset1);
            }
        }

        private void UpdateWatermarkPreview()
        {
            string text = ResolveWatermarkText();
            if (text.Length == 0) text = Loc.T(Txt.WatermarkCustomPlaceholder);
            _watermarkPreview.Spec = new WatermarkSpec(
                text,
                _working.WatermarkTint,
                _working.WatermarkOpacity,
                _working.WatermarkScale,
                _working.WatermarkRotation,
                _working.WatermarkOffsetX,
                _working.WatermarkOffsetY,
                _working.WatermarkAnchor);
            _watermarkPreview.EnabledMark = _working.WatermarkAnyEnabled;
            _watermarkPreview.Invalidate();
        }

        private void SelectSkin(SkinCard selected)
        {
            if (_loading) return;

            foreach (var card in _skinCards)
                card.Selected = card == selected;

            _working.SkinId = selected.Palette.Id;
            ApplyThemeDeep();
            NotifyApplied(immediate: true);
        }

        private void ApplyThemeDeep()
        {
            Theme.ApplySkin(SkinPalette.FromId(_working.SkinId));
            BackColor = Theme.Background;
            SkinManager.Apply(this);
            foreach (var nav in _navItems)
                nav.RefreshTheme();
            _watermarkPreview.Invalidate();
            Invalidate(true);
            RebuildBackdrop();
        }

        private void ResetToDefaults()
        {
            Language previousLanguage = _working.Language;
            _working.ResetToDefaults();
            LoadWorkingToUi();
            ApplyThemeDeep();
            NotifyApplied(immediate: true);

            if (_working.Language != previousLanguage)
                ReloadForLanguage();
        }

        /// <summary>切换语言并原地重建界面，窗口不关闭、不重新居中。</summary>
        internal void SwitchLanguage(Language language)
        {
            _working.Language = language;
            Loc.SetLanguage(language);
            NotifyApplied(immediate: true);
            ReloadForLanguage();
        }

        /// <summary>语言变化后原地重建界面：窗口位置、尺寸、当前页与滚动位置都保持不变。</summary>
        private void ReloadForLanguage()
        {
            int page = CurrentPage;
            _applyTimer.Stop();
            _loading = true;
            SuspendLayout();

            while (Controls.Count > 0)
            {
                Control control = Controls[0];
                Controls.RemoveAt(0);
                control.Dispose();
            }

            // 背景位图由 GlassPaint 的缓存持有，这里只解除引用，不能 Dispose，
            // 否则缓存里会留下已释放的位图，下次绘制时抛 “Parameter is not valid”。
            _backdropSharp = null;
            _backdropFrosted = null;
            _backdropKey = string.Empty;

            BuildUi();
            ShowPage(page);
            ResumeLayout(true);
            RebuildBackdrop();
            Invalidate(true);
        }

        /// <summary>把当前控件值同步到工作副本，并通知主窗口立即应用与保存。</summary>
        private void NotifyApplied(bool trim = false, bool immediate = false)
        {
            if (_loading) return;
            _working.NamePhrase = trim ? _phraseInput.Text.Trim() : _phraseInput.Text;
            _working.CustomName = trim ? _customInput.Text.Trim() : _customInput.Text;
            _working.CustomOutputFolder = trim ? _outputInput.Text.Trim() : _outputInput.Text;
            _working.WatermarkCustomText = trim ? _watermarkInput.Text.Trim() : _watermarkInput.Text;

            // 拖动滑条会连续触发几十次改动：合并成一次回调，避免主窗口反复重建壁纸造成卡顿
            _applyPending = true;
            if (immediate)
            {
                FlushApply();
                return;
            }

            _applyTimer.Stop();
            _applyTimer.Start();
        }

        /// <summary>把挂起的改动立刻提交给主窗口（关闭设置窗口、换肤、重置时使用）。</summary>
        private void FlushApply()
        {
            _applyTimer.Stop();
            if (!_applyPending) return;
            _applyPending = false;
            Applied?.Invoke(_working.Clone());
        }

        private void CloseSettings()
        {
            if (_working.NamingMode == NamingMode.Custom && _customInput.Text.Trim().Length == 0)
                _customInput.Text = Loc.T(_working.Language, Txt.DefaultDocName);
            NotifyApplied(true, immediate: true);
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 关闭前把挂起的改动落地：例如切换语言后窗口会立即重建，不能等去抖计时器
            FlushApply();
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _applyTimer.Dispose();
            base.Dispose(disposing);
        }

        private void ShowPage(int index)
        {
            CurrentPage = Math.Clamp(index, 0, _pages.Length - 1);
            for (int i = 0; i < _pages.Length; i++)
                _pages[i].Visible = i == CurrentPage;
            for (int i = 0; i < _navItems.Length; i++)
                _navItems[i].Selected = i == CurrentPage;
        }

        // ---------------- 绘制 ----------------

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
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 标题带使用半透明玻璃，而不是整块实色
            var headerRect = new Rectangle(0, 0, Width, HeaderHeight);
            using (var headerBrush = new SolidBrush(Theme.GlassStrong))
                g.FillRectangle(headerBrush, headerRect);

            using (var titleBrush = new SolidBrush(Theme.TextPrimary))
                g.DrawString(Loc.T(Txt.SettingsTitle), Theme.DialogTitleFont, titleBrush, 24f, 17f);

            using (var headerLine = new Pen(Theme.GlassBorder, 1f))
                g.DrawLine(headerLine, 0, HeaderHeight, Width, HeaderHeight);

            using (var divider = new Pen(Theme.Separator, 1f))
                g.DrawLine(divider, 20, 570, Width - 20, 570);

            using (var pen = new Pen(Theme.GlassBorder, 1f))
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RebuildBackdrop();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildBackdrop();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RebuildBackdrop();
        }

        Bitmap? IGlassScene.SharpBackdrop => _backdropSharp;
        Bitmap? IGlassScene.FrostedBackdrop => _backdropFrosted;

        private void RebuildBackdrop()
        {
            if (!IsHandleCreated) return;
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            // 壁纸只由「皮肤 + 尺寸」决定：命中缓存就直接复用，OnHandleCreated / OnShown /
            // OnResize 连续触发时也只做一次实际绘制，窗口弹出不再卡顿
            string key = Theme.Current.Id + "@" + ClientSize.Width + "x" + ClientSize.Height;
            if (_backdropSharp != null && _backdropKey == key) return;

            GlassPaint.Backdrop backdrop = GlassPaint.GetBackdrop(ClientSize, Theme.Current);
            _backdropSharp = backdrop.Sharp;
            _backdropFrosted = backdrop.Frosted;
            _backdropKey = key;

            GlassPaint.ApplyWindowChrome(this, Theme.IsDark);
            Invalidate(true);
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

        /// <summary>左侧一级目录条目：扁平圆角高亮，选中时左侧显示强调色条。</summary>
        private sealed class SettingsNavItem : Control
        {
            private bool _hover;
            private bool _selected;

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

            public SettingsNavItem()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
                BackColor = Color.Transparent;
                Font = Theme.BaseFont;
                Cursor = Cursors.Hand;
                TabStop = true;
            }

            public void RefreshTheme()
            {
                Font = Theme.BaseFont;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
                using GraphicsPath path = Theme.RoundedRect(rect, 8);

                if (_selected)
                {
                    using var fill = new SolidBrush(Color.FromArgb(48, Theme.Accent));
                    g.FillPath(fill, path);
                }
                else if (_hover)
                {
                    using var fill = new SolidBrush(Theme.GlassHover);
                    g.FillPath(fill, path);
                }

                if (_selected)
                {
                    using var bar = new SolidBrush(Theme.Accent);
                    g.FillRectangle(bar, 0, 6, 3, Height - 12);
                }

                TextRenderer.DrawText(g, Text, Font,
                    new Rectangle(16, 0, Width - 24, Height),
                    _selected ? Theme.Accent : Theme.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
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

        /// <summary>扁平滑块：左侧标签、中间轨道、右侧数值，支持拖动、滚轮与方向键。</summary>
        private sealed class SkinSlider : Control
        {
            private const int LabelWidth = 64;
            private const int ValueWidth = 48;
            private const int ThumbRadius = 7;
            private int _minimum;
            private int _maximum = 100;
            private int _value;
            private bool _dragging;
            private readonly TextBox _editor;
            private bool _editing;

            public string Label { get; set; } = string.Empty;
            public string Suffix { get; set; } = "%";
            public int SmallChange { get; set; } = 1;

            public event EventHandler? ValueChanged;

            public SkinSlider()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
                BackColor = Color.Transparent;
                Font = Theme.SmallFont;
                Height = 30;
                TabStop = true;
                Cursor = Cursors.Hand;

                // 数值区域可直接点开输入：滑块的精度不够时（例如精确到 37% 或 -12°）手输更快
                _editor = new TextBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = HorizontalAlignment.Right,
                    Font = Theme.SmallFont,
                    Visible = false
                };
                _editor.KeyDown += EditorKeyDown;
                _editor.LostFocus += (_, _) => CommitEditor();
                Controls.Add(_editor);
            }

            public int Minimum
            {
                get => _minimum;
                set
                {
                    _minimum = value;
                    if (_maximum < _minimum) _maximum = _minimum;
                    if (_value < _minimum) _value = _minimum;
                    Invalidate();
                }
            }

            public int Maximum
            {
                get => _maximum;
                set
                {
                    _maximum = Math.Max(value, _minimum);
                    if (_value > _maximum) _value = _maximum;
                    Invalidate();
                }
            }

            public int Value
            {
                get => _value;
                set
                {
                    int clamped = Math.Clamp(value, _minimum, _maximum);
                    if (clamped == _value) return;
                    _value = clamped;
                    if (_editing) _editor.Text = clamped.ToString();
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public void ApplyTheme()
            {
                Font = Theme.SmallFont;
                if (_editor != null)
                {
                    _editor.Font = Theme.SmallFont;
                    _editor.BackColor = Theme.Background;
                    _editor.ForeColor = Theme.TextPrimary;
                }
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color textColor = Enabled ? Theme.TextSecondary : Theme.TextMuted;
                TextRenderer.DrawText(g, Label, Font,
                    new Rectangle(0, 0, LabelWidth, Height), textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                int trackLeft = LabelWidth + 4;
                int trackRight = Width - ValueWidth - 6;
                if (trackRight <= trackLeft + 8) return;

                int trackY = Height / 2;
                float fraction = _maximum == _minimum
                    ? 0f
                    : (_value - _minimum) / (float)(_maximum - _minimum);
                int thumbX = trackLeft + (int)Math.Round((trackRight - trackLeft) * fraction);
                Color active = Enabled ? Theme.Accent : Theme.TextMuted;

                var trackRect = new Rectangle(trackLeft, trackY - 2, trackRight - trackLeft, 4);
                using (GraphicsPath path = Theme.RoundedRect(trackRect, 2))
                using (var brush = new SolidBrush(Theme.Separator))
                    g.FillPath(brush, path);

                if (thumbX - trackLeft > 3)
                {
                    var activeRect = new Rectangle(trackLeft, trackY - 2, thumbX - trackLeft, 4);
                    using GraphicsPath path = Theme.RoundedRect(activeRect, 2);
                    using var brush = new SolidBrush(active);
                    g.FillPath(brush, path);
                }

                if (Focused)
                {
                    using var ring = new SolidBrush(Color.FromArgb(60, active));
                    g.FillEllipse(ring, thumbX - ThumbRadius - 4, trackY - ThumbRadius - 4,
                        (ThumbRadius + 4) * 2, (ThumbRadius + 4) * 2);
                }

                using (var thumb = new SolidBrush(active))
                    g.FillEllipse(thumb, thumbX - ThumbRadius, trackY - ThumbRadius,
                        ThumbRadius * 2, ThumbRadius * 2);

                if (!_editing)
                {
                    string valueText = _value + Suffix;
                    TextRenderer.DrawText(g, valueText, Font,
                        new Rectangle(Width - ValueWidth, 0, ValueWidth, Height),
                        Enabled ? Theme.TextPrimary : Theme.TextMuted,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                }
            }

            protected override void OnEnabledChanged(EventArgs e)
            {
                base.OnEnabledChanged(e);
                if (!Enabled) CancelEdit();
                Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                if (!Enabled || e.Button != MouseButtons.Left) return;

                // 点右侧数值区域 → 直接键入；点轨道 → 拖动
                if (e.X >= Width - ValueWidth - 10)
                {
                    BeginEdit();
                    return;
                }

                Focus();
                _dragging = true;
                SetFromX(e.X);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (_dragging) SetFromX(e.X);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                _dragging = false;
                base.OnMouseUp(e);
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                if (!Enabled) return;
                Step(e.Delta > 0 ? SmallChange * 5 : -SmallChange * 5);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (!Enabled) return;

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        Step(-SmallChange);
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        Step(SmallChange);
                        e.Handled = true;
                        break;
                    case Keys.PageDown:
                        Step(-SmallChange * 10);
                        e.Handled = true;
                        break;
                    case Keys.PageUp:
                        Step(SmallChange * 10);
                        e.Handled = true;
                        break;
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                Invalidate();
                base.OnGotFocus(e);
            }

            protected override void OnLostFocus(EventArgs e)
            {
                Invalidate();
                base.OnLostFocus(e);
            }

            private void Step(int delta)
            {
                Value = _value + delta;
            }

            private void SetFromX(int x)
            {
                int trackLeft = LabelWidth + 4;
                int trackRight = Width - ValueWidth - 6;
                if (trackRight <= trackLeft) return;

                float fraction = (x - trackLeft) / (float)(trackRight - trackLeft);
                Value = _minimum + (int)Math.Round(fraction * (_maximum - _minimum));
            }

            private void BeginEdit()
            {
                if (_editing || !Enabled) return;

                _editing = true;
                _editor.Bounds = new Rectangle(Width - ValueWidth - 6, (Height - 22) / 2, ValueWidth + 6, 22);
                _editor.BackColor = Theme.Background;
                _editor.ForeColor = Theme.TextPrimary;
                _editor.Text = _value.ToString();
                _editor.Visible = true;
                _editor.BringToFront();
                _editor.Focus();
                _editor.SelectAll();
                Invalidate();
            }

            private void CommitEditor()
            {
                if (!_editing) return;

                string raw = _editor.Text.Trim().TrimEnd('%', '°', ' ');
                if (int.TryParse(raw, out int parsed))
                    Value = parsed;

                CancelEdit();
            }

            private void CancelEdit()
            {
                if (!_editing) return;
                _editing = false;
                _editor.Visible = false;
                Invalidate();
            }

            private void EditorKeyDown(object? sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CommitEditor();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CancelEdit();
                    e.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// 水印效果预览：白纸 + 当前参数的水印示意；未启用时淡化显示。
        /// 支持直接操作：拖动改位置、滚轮改大小、右键拖动改角度、双击复位。
        /// </summary>
        private sealed class WatermarkPreviewPanel : Control
        {
            private Point _lastPoint;
            private bool _dragging;
            private bool _rotating;

            public WatermarkSpec Spec { get; set; } =
                new(Loc.T(Txt.WatermarkPreset1), WatermarkTint.Gray);

            public bool EnabledMark { get; set; }

            /// <summary>用户在预览里直接调整参数后回调，由设置页同步到滑块与设置对象。</summary>
            public event Action<WatermarkSpec>? SpecEdited;

            public WatermarkPreviewPanel()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
                BackColor = Color.Transparent;
                TabStop = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var paper = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
                using (GraphicsPath path = Theme.RoundedRect(paper, 8))
                {
                    using (var fill = new SolidBrush(Color.White))
                        g.FillPath(fill, path);

                    g.SetClip(path);
                    if (EnabledMark)
                    {
                        var inner = new Rectangle(14, 14, Width - 28, Height - 28);
                        WatermarkPainter.RenderPreview(g, inner, Spec);
                    }
                    g.ResetClip();

                    using var pen = new Pen(Theme.Border, 1f);
                    g.DrawPath(pen, path);
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                if (!EnabledMark) return;

                if (e.Button == MouseButtons.Left) _dragging = true;
                else if (e.Button == MouseButtons.Right) _rotating = true;
                else return;

                _lastPoint = e.Location;
                Capture = true;
                Cursor = _rotating ? Cursors.SizeNS : Cursors.SizeAll;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (!_dragging && !_rotating) return;

                if (_dragging)
                {
                    int stepX = (int)Math.Round((e.X - _lastPoint.X) * 100.0 / Math.Max(1, Width));
                    int stepY = (int)Math.Round((e.Y - _lastPoint.Y) * 100.0 / Math.Max(1, Height));
                    if (stepX == 0 && stepY == 0) return;

                    Spec = Spec with
                    {
                        OffsetXPercent = Math.Clamp(Spec.OffsetXPercent + stepX, -100, 100),
                        OffsetYPercent = Math.Clamp(Spec.OffsetYPercent + stepY, -100, 100)
                    };
                }
                else
                {
                    int delta = _lastPoint.Y - e.Y;
                    if (delta == 0) return;

                    Spec = Spec with
                    {
                        RotationDegrees = Math.Clamp(Spec.RotationDegrees + delta, -180, 180)
                    };
                }

                _lastPoint = e.Location;
                Invalidate();
                SpecEdited?.Invoke(Spec);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _dragging = false;
                _rotating = false;
                Capture = false;
                Cursor = Cursors.Default;
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                // 未聚焦的控件收不到滚轮事件
                if (EnabledMark && !Focused) Focus();
                Cursor = EnabledMark ? Cursors.SizeAll : Cursors.Default;
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                if (!EnabledMark) return;

                int step = e.Delta > 0 ? 5 : -5;
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                {
                    Spec = Spec with
                    {
                        RotationDegrees = Math.Clamp(Spec.RotationDegrees + step, -180, 180)
                    };
                }
                else
                {
                    Spec = Spec with
                    {
                        ScalePercent = Math.Clamp(Spec.ScalePercent + step, 10, 400)
                    };
                }

                Invalidate();
                SpecEdited?.Invoke(Spec);
            }

            protected override void OnMouseDoubleClick(MouseEventArgs e)
            {
                base.OnMouseDoubleClick(e);
                if (!EnabledMark || e.Button != MouseButtons.Left) return;

                Spec = Spec with { OffsetXPercent = 0, OffsetYPercent = 0 };
                Invalidate();
                SpecEdited?.Invoke(Spec);
            }
        }
    }
}
