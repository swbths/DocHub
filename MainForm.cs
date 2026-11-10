using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DocHub
{
    public sealed class MainForm : Form, IGlassScene
    {
        private const string ReorderFormat = "DocHub.DocumentReorder";
        private const int SidebarWidth = 236;
        private const int PageHeaderHeight = 84;
        private const int SideWidth = 306;

        private readonly AppSettings _settings;
        private readonly SideNavPanel _sidebar;
        private readonly Panel _brandDivider;
        private readonly Panel _sideDivider;
        private readonly NavCard _navMerge;
        private readonly NavCard _navConvert;
        private readonly NavCard _navImagePdf;
        private readonly NavCard _navWordPdf;
        private readonly SideLink _sideSkin;
        private readonly SideLink _sideSettings;
        private readonly SideLink _sideAbout;
        private readonly SideLink _sideWatermark;
        private readonly SideLink _sideDone;
        private readonly Label _pageTitle;
        private readonly Label _pageSubtitle;
        private readonly ConvertWorkspace _convertPage;
        private readonly ImagePdfWorkspace _imagePdfPage;
        private readonly WordPdfWorkspace _wordPdfPage;
        private readonly GlassCard _listCard;
        private readonly GlassCard _outputCard;
        private readonly LogoBadge _logo;
        private readonly Label _appTitle;
        private readonly Label _versionLabel;
        private readonly Label _sectionLabel;
        private readonly Label _countLabel;
        private readonly Label _hintLabel;
        private readonly QuoteCard _quoteCard;
        private readonly Label _outputLabel;
        private readonly Label _folderLabel;
        private readonly Label _pdfSuffix;
        private readonly Label _statusLabel;
        private readonly Label _localLabel;
        private readonly Panel _divider;
        private readonly RoundedButton _addButton;
        private readonly IconButton _upButton;
        private readonly IconButton _downButton;
        private readonly IconButton _removeButton;
        private readonly IconButton _clearButton;
        private readonly IconButton _cancelBoxButton;
        private readonly RoundedButton _browseButton;
        private readonly RoundedButton _startButton;
        private readonly RoundedButton _viewResultButton;
        private readonly DocumentListBox _list;
        private readonly EmptyStatePanel _emptyPanel;
        private readonly Panel _dropLine;
        private readonly RoundedInput _nameInput;
        private readonly RoundedInput _folderInput;
        private readonly SlimProgressBar _progress;
        private readonly System.Windows.Forms.Timer _backdropTimer = new() { Interval = 140 };
        private readonly System.Windows.Forms.Timer _quoteTimer = new();
        private readonly ToolTip _toolTip = new();
        private readonly ContextMenuStrip _contextMenu = new();
        private readonly ToolStripMenuItem _menuTop;
        private readonly ToolStripMenuItem _menuBottom;
        private readonly ToolStripMenuItem _menuUp;
        private readonly ToolStripMenuItem _menuDown;
        private readonly ToolStripMenuItem _menuRemove;
        private ToolStripMenuItem? _trayOpenItem;
        private ToolStripMenuItem? _trayExitItem;

        private readonly List<DocumentItem> _documents = new();
        private int _activePage;
        private bool _manualFolder;
        private bool _suppressEvents;
        private bool _running;
        private int _dragSourceIndex = -1;
        private Point _dragStart;
        private bool _dragArmed;
        private NotifyIcon? _tray;
        private ToastForm? _toast;
        private string _lastOutputPath = string.Empty;
        private Bitmap? _backdropSharp;
        private Bitmap? _backdropFrosted;

        public MainForm()
        {
            _settings = AppSettings.Load();
            Loc.SetLanguage(_settings.Language);
            Theme.RefreshFonts();
            Theme.ApplySkin(SkinPalette.FromId(_settings.SkinId));

            Text = Loc.T(Txt.AppName);
            ClientSize = new Size(1200, 760);
            MinimumSize = new Size(1080, 760);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = true;
            AllowDrop = true;
            Icon = AppIconProvider.Get();

            _sidebar = new SideNavPanel
            {
                Bounds = new Rectangle(0, 0, SidebarWidth, ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            _logo = new LogoBadge
            {
                Bounds = new Rectangle(20, 16, 34, 34)
            };

            _appTitle = new Label
            {
                Text = Loc.T(Txt.AppName),
                Font = Theme.SectionFont,
                ForeColor = Theme.HeaderText,
                BackColor = Color.Transparent,
                Tag = "header",
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds = new Rectangle(62, 12, 158, 24)
            };

            string versionText = "v4.0.0";
            try
            {
                Version? version = typeof(MainForm).Assembly.GetName().Version;
                if (version != null)
                {
                    versionText = $"v{Math.Max(0, version.Major)}.{Math.Max(0, version.Minor)}." +
                                  $"{Math.Max(0, version.Build)}";
                }
            }
            catch
            {
                // 读取程序集版本失败时保留固定文本
            }

            _versionLabel = new Label
            {
                Text = versionText,
                Font = Theme.BadgeFont,
                ForeColor = Theme.HeaderSubtitle,
                BackColor = Color.Transparent,
                Tag = "header",
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds = new Rectangle(63, 34, 158, 16)
            };

            _brandDivider = new Panel
            {
                Bounds = new Rectangle(20, 66, SidebarWidth - 40, 1),
                BackColor = Theme.Separator
            };

            _navMerge = new NavCard
            {
                Icon = IconKind.Merge,
                Bounds = new Rectangle(16, 80, SidebarWidth - 32, 66)
            };
            _navConvert = new NavCard
            {
                Icon = IconKind.Convert,
                Bounds = new Rectangle(16, 154, SidebarWidth - 32, 66)
            };
            _navImagePdf = new NavCard
            {
                Icon = IconKind.Image,
                Bounds = new Rectangle(16, 228, SidebarWidth - 32, 66)
            };
            _navWordPdf = new NavCard
            {
                Icon = IconKind.WordPdf,
                Bounds = new Rectangle(16, 302, SidebarWidth - 32, 66)
            };

            _sideDivider = new Panel
            {
                Height = 1,
                BackColor = Theme.Separator
            };
            _sideSkin = new SideLink { Icon = IconKind.Palette };
            _sideSettings = new SideLink { Icon = IconKind.Settings };
            _sideAbout = new SideLink { Icon = IconKind.Info };
            _sideWatermark = new SideLink { Icon = IconKind.Watermark };
            _sideDone = new SideLink { Icon = IconKind.Bell };

            _navMerge.Title = Loc.T(Txt.NavMergeTitle);
            _navMerge.Subtitle = Loc.T(Txt.NavMergeSubtitle);
            _navConvert.Title = Loc.T(Txt.NavConvertTitle);
            _navConvert.Subtitle = Loc.T(Txt.NavConvertSubtitle);
            _navImagePdf.Title = Loc.T(Txt.NavImagePdfTitle);
            _navImagePdf.Subtitle = Loc.T(Txt.NavImagePdfSubtitle);
            _navWordPdf.Title = Loc.T(Txt.NavWordPdfTitle);
            _navWordPdf.Subtitle = Loc.T(Txt.NavWordPdfSubtitle);
            _sideSkin.Text = Loc.T(Txt.Skin);
            _sideSettings.Text = Loc.T(Txt.Settings);
            _sideAbout.Text = Loc.T(Txt.About);
            _sideWatermark.Text = Loc.T(Txt.SettingsNavWatermark);
            _sideDone.Text = Loc.T(Txt.SettingsNavDone);

            _sidebar.Controls.AddRange(new Control[]
            {
                _logo, _appTitle, _versionLabel, _brandDivider,
                _navMerge, _navConvert, _navImagePdf, _navWordPdf, _sideDivider,
                _sideWatermark, _sideDone, _sideSkin, _sideSettings, _sideAbout
            });

            _pageTitle = new Label
            {
                Font = Theme.SectionFont,
                ForeColor = Theme.TextPrimary,
                BackColor = Color.Transparent,
                Tag = "title",
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _pageSubtitle = new Label
            {
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.AddRange(new Control[]
            {
                _sidebar, _pageTitle, _pageSubtitle
            });

            _listCard = new GlassCard();
            _outputCard = new GlassCard { Radius = 16, Strong = true };

            _sectionLabel = MakeLabel(Loc.T(Txt.DocsTitle), Theme.SectionFont, "section", 20, 14);
            _countLabel = MakeLabel(Loc.T(Txt.DocCountFormat, 0), Theme.SmallFont, "muted", 120, 20);

            _hintLabel = new Label
            {
                Text = Loc.T(Txt.ListHint),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.TopLeft,
                Bounds = new Rectangle(18, 404, 262, 64),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _divider = new Panel
            {
                Bounds = new Rectangle(16, 156, 274, 1),
                BackColor = Theme.Separator,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _addButton = new RoundedButton
            {
                Text = Loc.T(Txt.AddDocs),
                Role = ButtonRole.Primary,
                Font = Theme.ButtonFont,
                Radius = 9,
                Icon = IconKind.Plus,
                IconSize = 14,
                IconGap = 6,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _addButton.ApplyTheme();

            _upButton = CreateIconButton(IconKind.Up);
            _downButton = CreateIconButton(IconKind.Down);
            _removeButton = CreateIconButton(IconKind.Trash);
            _clearButton = CreateIconButton(IconKind.Clear);
            _cancelBoxButton = CreateIconButton(IconKind.MarqueeOff);
            _cancelBoxButton.Enabled = false;

            _list = new DocumentListBox
            {
                Bounds = new Rectangle(10, 50, 546, 564),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowDrop = true,
                SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            };
            _emptyPanel = new EmptyStatePanel
            {
                Bounds = _list.Bounds,
                Anchor = _list.Anchor
            };
            _dropLine = new Panel
            {
                Height = 2,
                BackColor = Theme.Accent,
                Visible = false,
                Tag = "accentBar"
            };

            _contextMenu.ShowImageMargin = false;
            _contextMenu.Font = Theme.BaseFont;
            _contextMenu.Renderer = new SkinMenuRenderer();
            _menuTop = new ToolStripMenuItem(Loc.T(Txt.MoveTop), null, (_, _) => MoveToEdge(true));
            _menuBottom = new ToolStripMenuItem(Loc.T(Txt.MoveBottom), null, (_, _) => MoveToEdge(false));
            _menuUp = new ToolStripMenuItem(Loc.T(Txt.MoveUp), null, (_, _) => MoveSelected(-1));
            _menuDown = new ToolStripMenuItem(Loc.T(Txt.MoveDown), null, (_, _) => MoveSelected(1));
            _menuRemove = new ToolStripMenuItem(Loc.T(Txt.Remove), null, (_, _) => RemoveSelected());
            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _menuTop, _menuBottom, new ToolStripSeparator(),
                _menuUp, _menuDown, new ToolStripSeparator(), _menuRemove
            });
            _contextMenu.Opening += (_, _) => UpdateContextMenu();

            _outputLabel = MakeLabel(Loc.T(Txt.OutputFile), Theme.BaseFont, "secondary", 16, 18);
            _nameInput = new RoundedInput
            {
                Bounds = new Rectangle(16, 42, 240, 36)
            };
            _nameInput.MaxLength = 120;
            _nameInput.ReadOnly = _settings.NamingMode != NamingMode.Custom;
            _nameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _pdfSuffix = MakeLabel(".pdf", Theme.InputFont, "secondary", 262, 51);
            _folderLabel = MakeLabel(Loc.T(Txt.SaveTo), Theme.BaseFont, "secondary", 16, 92);
            _folderInput = new RoundedInput
            {
                Bounds = new Rectangle(16, 116, 214, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _folderInput.ReadOnly = false;

            _browseButton = CreateSecondaryButton(Loc.T(Txt.Browse), 238, 116, 52, 36);
            _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            _statusLabel = new Label
            {
                Bounds = new Rectangle(16, 200, 274, 40),
                AutoSize = false,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.TopLeft,
                Font = Theme.BaseFont,
                ForeColor = Theme.TextSecondary,
                BackColor = Color.Transparent,
                Tag = "secondary",
                Text = Loc.T(Txt.ReadyStatus)
            };

            _progress = new SlimProgressBar
            {
                Bounds = new Rectangle(16, 244, 274, 22),
                StrongGlass = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _startButton = new RoundedButton
            {
                Bounds = new Rectangle(16, 476, 274, 48),
                Text = Loc.T(Txt.StartMerge),
                Role = ButtonRole.Primary,
                Font = Theme.PrimaryButtonFont,
                Radius = 11,
                Icon = IconKind.Merge,
                IconSize = 16,
                IconGap = 8,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _startButton.ApplyTheme();

            _viewResultButton = CreateSecondaryButton(Loc.T(Txt.ViewResult), 16, 430, 274, 30);
            _viewResultButton.Role = ButtonRole.Ghost;
            _viewResultButton.ApplyTheme();
            _viewResultButton.Visible = false;

            _localLabel = new Label
            {
                Text = Loc.T(Txt.LocalBadge),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                Tag = "muted",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(16, 448, 274, 18),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _quoteCard = new QuoteCard
            {
                Bounds = new Rectangle(16, 330, 274, 150),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Visible = _settings.ShowQuotes
            };

            _listCard.Controls.AddRange(new Control[]
            {
                _sectionLabel, _countLabel, _addButton, _upButton, _downButton,
                _removeButton, _clearButton, _cancelBoxButton, _list
            });
            _listCard.Controls.Add(_emptyPanel);
            _emptyPanel.BringToFront();
            _listCard.Controls.Add(_dropLine);
            _dropLine.BringToFront();
            _outputCard.Controls.AddRange(new Control[]
            {
                _outputLabel, _nameInput, _pdfSuffix, _folderLabel, _folderInput,
                _browseButton, _divider, _statusLabel, _progress, _startButton,
                _viewResultButton, _localLabel, _hintLabel, _quoteCard
            });

            _convertPage = new ConvertWorkspace(_settings)
            {
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _convertPage.BusyChanged += (_, _) => UpdateNavigationEnabled();

            _imagePdfPage = new ImagePdfWorkspace(_settings)
            {
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _imagePdfPage.BusyChanged += (_, _) => UpdateNavigationEnabled();

            _wordPdfPage = new WordPdfWorkspace(_settings)
            {
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _wordPdfPage.BusyChanged += (_, _) => UpdateNavigationEnabled();

            Controls.Add(_listCard);
            Controls.Add(_outputCard);
            Controls.Add(_convertPage);
            Controls.Add(_imagePdfPage);
            Controls.Add(_wordPdfPage);

            // 窗口缩放时按卡片实际宽度重新排布，避免状态行等内容压到按钮上
            _listCard.Resize += (_, _) => LayoutQueueCard();
            _outputCard.Resize += (_, _) => LayoutSideCard();

            // 让窗口的各个区域都能接收文件拖入，而不只是列表
            EnableFileDrop(_listCard);
            EnableFileDrop(_outputCard);

            // 列表行内操作
            _list.ActionUp += (_, _) => MoveSelected(-1);
            _list.ActionDown += (_, _) => MoveSelected(1);
            _list.ActionRemove += (_, _) => RemoveSelected();

            _backdropTimer.Tick += (_, _) =>
            {
                _backdropTimer.Stop();
                RebuildBackdrop();
            };

            _quoteTimer.Tick += (_, _) =>
            {
                if (_settings.ShowQuotes && _settings.QuoteAutoRefresh && _quoteCard.Visible)
                    _quoteCard.NextQuote();
            };

            ConfigureToolTips();
            WireEvents();
            LayoutLocalizedControls();
            UpdateNamePreview();
            RefreshList();
            ApplyTheme();
            SwitchPage(0);
            InitializeTray();
        }

        private static int FitWidth(string text, Font font, int padding, int minimum)
        {
            int width = TextRenderer.MeasureText(text, font).Width + padding;
            return Math.Max(minimum, width);
        }

        private IconButton CreateIconButton(IconKind icon)
        {
            return new IconButton
            {
                Icon = icon,
                Bounds = new Rectangle(0, 0, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
        }

        /// <summary>页头：左侧页面标题，右侧留空给窗口按钮。</summary>
        private void LayoutHeader()
        {
            int right = ClientSize.Width - 24;
            int titleLeft = SidebarWidth + 24;
            int titleWidth = Math.Max(180, right - titleLeft);
            _pageTitle.Bounds = new Rectangle(titleLeft, 18, titleWidth, 28);
            _pageSubtitle.Bounds = new Rectangle(titleLeft, 47, titleWidth, 22);
        }

        /// <summary>左侧卡片导航：品牌、四个入口、底部水印/提醒/皮肤/设置/关于。</summary>
        private void LayoutSidebar()
        {
            int height = ClientSize.Height;
            if (height <= 0) return;

            _sidebar.Bounds = new Rectangle(0, 0, SidebarWidth, height);
            _brandDivider.Bounds = new Rectangle(20, 66, SidebarWidth - 40, 1);
            _navMerge.Bounds = new Rectangle(16, 80, SidebarWidth - 32, 66);
            _navConvert.Bounds = new Rectangle(16, 154, SidebarWidth - 32, 66);
            _navImagePdf.Bounds = new Rectangle(16, 228, SidebarWidth - 32, 66);
            _navWordPdf.Bounds = new Rectangle(16, 302, SidebarWidth - 32, 66);

            const int linkHeight = 34;
            _sideAbout.Bounds = new Rectangle(16, height - 18 - linkHeight, SidebarWidth - 32, linkHeight);
            _sideSettings.Bounds = new Rectangle(16, _sideAbout.Top - 4 - linkHeight, SidebarWidth - 32, linkHeight);
            _sideSkin.Bounds = new Rectangle(16, _sideSettings.Top - 4 - linkHeight, SidebarWidth - 32, linkHeight);
            _sideDone.Bounds = new Rectangle(16, _sideSkin.Top - 4 - linkHeight, SidebarWidth - 32, linkHeight);
            _sideWatermark.Bounds = new Rectangle(16, _sideDone.Top - 4 - linkHeight, SidebarWidth - 32, linkHeight);
            _sideDivider.Bounds = new Rectangle(20, _sideWatermark.Top - 12, SidebarWidth - 40, 1);
        }

        /// <summary>合并页两张卡片与转换页容器的位置。</summary>
        private void LayoutMergeCards()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            int pageWidth = ClientSize.Width - SidebarWidth;
            int cardHeight = Math.Max(120, ClientSize.Height - PageHeaderHeight - 18);
            int sideWidth = Math.Min(SideWidth,
                Math.Max(220, pageWidth - 24 - 20 - 16 - 260));
            int listWidth = Math.Max(200, pageWidth - 24 - 20 - 16 - sideWidth);

            _listCard.Bounds = new Rectangle(SidebarWidth + 24, PageHeaderHeight, listWidth, cardHeight);
            _outputCard.Bounds = new Rectangle(SidebarWidth + 24 + listWidth + 16, PageHeaderHeight, sideWidth, cardHeight);
            _convertPage.Bounds = new Rectangle(SidebarWidth, PageHeaderHeight, pageWidth,
                Math.Max(120, ClientSize.Height - PageHeaderHeight));
            _imagePdfPage.Bounds = _convertPage.Bounds;
            _wordPdfPage.Bounds = _convertPage.Bounds;
        }

        /// <summary>按当前语言的实际文字宽度排布，避免长语言下按钮被截断或标签互相重叠。</summary>
        private void LayoutLocalizedControls()
        {
            LayoutSidebar();
            LayoutHeader();
            LayoutMergeCards();
            LayoutQueueCard();
            LayoutSideCard();
        }

        private void SwitchPage(int page)
        {
            if (_running || _convertPage.IsBusy || _imagePdfPage.IsBusy || _wordPdfPage.IsBusy) return;

            _activePage = Math.Clamp(page, 0, 3);
            _navMerge.Selected = _activePage == 0;
            _navConvert.Selected = _activePage == 1;
            _navImagePdf.Selected = _activePage == 2;
            _navWordPdf.Selected = _activePage == 3;
            _listCard.Visible = _outputCard.Visible = _activePage == 0;
            _convertPage.Visible = _activePage == 1;
            _imagePdfPage.Visible = _activePage == 2;
            _wordPdfPage.Visible = _activePage == 3;
            UpdatePageHeader();

            if (_activePage == 1)
            {
                _convertPage.ApplySettings();
                _convertPage.Focus();
            }
            else if (_activePage == 2)
            {
                _imagePdfPage.ApplySettings();
                _imagePdfPage.Focus();
            }
            else if (_activePage == 3)
            {
                _wordPdfPage.ApplySettings();
                _wordPdfPage.Focus();
            }
        }

        private void UpdatePageHeader()
        {
            switch (_activePage)
            {
                case 1:
                    _pageTitle.Text = Loc.T(Txt.ConvertPageTitle);
                    _pageSubtitle.Text = Loc.T(Txt.ConvertPageSubtitle);
                    break;
                case 2:
                    _pageTitle.Text = Loc.T(Txt.ImagePdfPageTitle);
                    _pageSubtitle.Text = Loc.T(Txt.ImagePdfPageSubtitle);
                    break;
                case 3:
                    _pageTitle.Text = Loc.T(Txt.WordPdfPageTitle);
                    _pageSubtitle.Text = Loc.T(Txt.WordPdfPageSubtitle);
                    break;
                default:
                    _pageTitle.Text = Loc.T(Txt.MergePageTitle);
                    _pageSubtitle.Text = Loc.T(Txt.MergePageSubtitle);
                    break;
            }
        }

        private void UpdateNavigationEnabled()
        {
            bool busy = _running || _convertPage.IsBusy || _imagePdfPage.IsBusy || _wordPdfPage.IsBusy;
            _navMerge.Enabled = !busy;
            _navConvert.Enabled = !busy;
            _navImagePdf.Enabled = !busy;
            _navWordPdf.Enabled = !busy;
            _sideSkin.Enabled = !busy;
            _sideSettings.Enabled = !busy;
            _sideAbout.Enabled = !busy;
            _sideWatermark.Enabled = !busy;
            _sideDone.Enabled = !busy;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1) || keyData == (Keys.Control | Keys.NumPad1))
            {
                SwitchPage(0);
                return true;
            }

            if (keyData == (Keys.Control | Keys.D2) || keyData == (Keys.Control | Keys.NumPad2))
            {
                SwitchPage(1);
                return true;
            }

            if (keyData == (Keys.Control | Keys.D3) || keyData == (Keys.Control | Keys.NumPad3))
            {
                SwitchPage(2);
                return true;
            }

            if (keyData == (Keys.Control | Keys.D4) || keyData == (Keys.Control | Keys.NumPad4))
            {
                SwitchPage(3);
                return true;
            }

            if (keyData == (Keys.Control | Keys.O))
            {
                if (_activePage == 0)
                {
                    if (!_running) ShowAddDialog();
                }
                else if (_activePage == 1)
                {
                    _convertPage.PromptAddFiles();
                }
                else if (_activePage == 2)
                {
                    _imagePdfPage.PromptAddFiles();
                }
                else
                {
                    _wordPdfPage.PromptAddFiles();
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>队列卡：标题、计数、四个图标操作与「添加文件」全部右对齐。</summary>
        private void LayoutQueueCard()
        {
            int width = _listCard.ClientSize.Width;
            int height = _listCard.ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            int addWidth = FitWidth(_addButton.Text, _addButton.Font, 52, 112);
            _addButton.Bounds = new Rectangle(width - 16 - addWidth, 12, addWidth, 34);

            const int gap = 4;
            int x = _addButton.Left - 14;
            x -= 30;
            _cancelBoxButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _clearButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _removeButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _downButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _upButton.Bounds = new Rectangle(x, 14, 30, 30);

            _countLabel.Location = new Point(_sectionLabel.Right + 10, 20);

            _list.Bounds = new Rectangle(10, 50, Math.Max(60, width - 20), Math.Max(60, height - 60));
            _emptyPanel.Bounds = _list.Bounds;

            RefreshListBackdrop();
        }

        /// <summary>右侧输出卡：字段自上而下，状态与主按钮固定在底部。</summary>
        private void LayoutSideCard()
        {
            int width = _outputCard.ClientSize.Width;
            int height = _outputCard.ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            int inner = Math.Max(80, width - 32);
            int y = 18;

            _outputLabel.Location = new Point(16, y);
            y += 24;
            _nameInput.Bounds = new Rectangle(16, y, Math.Max(60, inner - 34), 36);
            _pdfSuffix.Location = new Point(_nameInput.Right + 6, y + 9);
            y += 50;

            _folderLabel.Location = new Point(16, y);
            y += 24;
            int browseWidth = FitWidth(_browseButton.Text, _browseButton.Font, 28, 58);
            _browseButton.Bounds = new Rectangle(16 + inner - browseWidth, y, browseWidth, 36);
            _folderInput.Bounds = new Rectangle(16, y, Math.Max(60, inner - browseWidth - 8), 36);
            y += 46;

            _divider.Bounds = new Rectangle(16, y, inner, 1);
            y += 16;

            _statusLabel.Bounds = new Rectangle(16, y, inner, 40);
            y += 46;
            _progress.Bounds = new Rectangle(16, y, inner, 22);

            const int startHeight = 48;
            _startButton.Bounds = new Rectangle(16, height - 18 - startHeight, inner, startHeight);
            _localLabel.Bounds = new Rectangle(16, _startButton.Top - 26, inner, 16);
            _viewResultButton.Bounds = new Rectangle(16, _localLabel.Top - 38, inner, 30);
            _hintLabel.Bounds = new Rectangle(18,
                Math.Min(y + 34, _viewResultButton.Top - 76), Math.Max(60, inner - 4), 68);

            // 名言卡片占据提示文字与底部按钮之间的空白；空间不足时自动隐藏
            int quoteTop = _hintLabel.Bottom + 18;
            int quoteHeight = Math.Min(172, _viewResultButton.Top - 14 - quoteTop);
            _quoteCard.Bounds = new Rectangle(16, quoteTop, inner, Math.Max(0, quoteHeight));
            _quoteCard.Visible = _settings.ShowQuotes && quoteHeight >= 128;
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

        private RoundedButton CreatePrimaryButton(string text, int x, int y, int width, int height)
        {
            var button = new RoundedButton
            {
                Text = text,
                Bounds = new Rectangle(x, y, width, height),
                Role = ButtonRole.Primary,
                Font = Theme.ButtonFont,
                Radius = 9,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            button.ApplyTheme();
            return button;
        }

        private RoundedButton CreateSecondaryButton(string text, int x, int y, int width, int height)
        {
            var button = new RoundedButton
            {
                Text = text,
                Bounds = new Rectangle(x, y, width, height),
                Role = ButtonRole.Secondary,
                Font = Theme.ButtonFont,
                Radius = 9,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            button.ApplyTheme();
            return button;
        }

        private void ConfigureToolTips()
        {
            _toolTip.SetToolTip(_addButton, Loc.T(Txt.TipAdd));
            _toolTip.SetToolTip(_upButton, Loc.T(Txt.TipUp));
            _toolTip.SetToolTip(_downButton, Loc.T(Txt.TipDown));
            _toolTip.SetToolTip(_removeButton, Loc.T(Txt.TipRemove));
            _toolTip.SetToolTip(_clearButton, Loc.T(Txt.TipClear));
            _toolTip.SetToolTip(_browseButton, Loc.T(Txt.TipBrowse));
            _toolTip.SetToolTip(_list, Loc.T(Txt.TipList));
            _toolTip.SetToolTip(_startButton, Loc.T(Txt.TipStart));
            _toolTip.SetToolTip(_cancelBoxButton, Loc.T(Txt.TipCancelBox));
            _toolTip.SetToolTip(_quoteCard, Loc.T(Txt.TipQuote));
            _toolTip.SetToolTip(_sideWatermark, Loc.T(Txt.TipWatermark));
            _toolTip.SetToolTip(_sideDone, Loc.T(Txt.TipNotifications));
            _toolTip.SetToolTip(_navMerge, Loc.T(Txt.TipNavMerge));
            _toolTip.SetToolTip(_navConvert, Loc.T(Txt.TipNavConvert));
            _toolTip.SetToolTip(_navImagePdf, Loc.T(Txt.TipNavImagePdf));
            _toolTip.SetToolTip(_navWordPdf, Loc.T(Txt.TipNavWordPdf));
            _toolTip.SetToolTip(_sideSkin, Loc.T(Txt.TipAppearance));
            _toolTip.SetToolTip(_sideSettings, Loc.T(Txt.TipSettings));
            _toolTip.SetToolTip(_sideAbout, Loc.T(Txt.TipAbout));
        }

        private void InitializeTray()
        {
            _tray = new NotifyIcon
            {
                Icon = AppIconProvider.Get(),
                Text = Loc.T(Txt.AppName),
                Visible = true
            };

            var menu = new ContextMenuStrip
            {
                ShowImageMargin = false,
                Font = Theme.BaseFont,
                Renderer = new SkinMenuRenderer()
            };
            _trayOpenItem = new ToolStripMenuItem(Loc.T(Txt.TrayOpen), null, (_, _) => RestoreWindow());
            _trayExitItem = new ToolStripMenuItem(Loc.T(Txt.TrayExit), null, (_, _) =>
            {
                if (_tray != null) _tray.Visible = false;
                Close();
            });
            menu.Items.Add(_trayOpenItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(_trayExitItem);
            _tray.ContextMenuStrip = menu;
            _tray.DoubleClick += (_, _) => RestoreWindow();
        }

        private void RestoreWindow()
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            Show();
            Activate();
            BringToFront();
        }

        private void WireEvents()
        {
            _addButton.Click += (_, _) => ShowAddDialog();
            _upButton.Click += (_, _) => MoveSelected(-1);
            _downButton.Click += (_, _) => MoveSelected(1);
            _removeButton.Click += (_, _) => RemoveSelected();
            _clearButton.Click += (_, _) => ClearAll();
            _browseButton.Click += (_, _) => BrowseFolder();
            _startButton.Click += (_, _) => StartMerge();
            _viewResultButton.Click += (_, _) => OpenOutput(_lastOutputPath);
            _sideWatermark.Click += (_, _) => OpenSettings(4);
            _sideDone.Click += (_, _) => OpenSettings(3);
            _navMerge.Click += (_, _) => SwitchPage(0);
            _navConvert.Click += (_, _) => SwitchPage(1);
            _navImagePdf.Click += (_, _) => SwitchPage(2);
            _navWordPdf.Click += (_, _) => SwitchPage(3);
            _sideSkin.Click += (_, _) => OpenSettings(0);
            _sideSettings.Click += (_, _) => OpenSettings(1);
            _sideAbout.Click += (_, _) => OpenAbout();
            _cancelBoxButton.Click += (_, _) =>
            {
                _list.CancelMarqueeSelection();
                UpdateButtons();
            };

            _list.SelectedIndexChanged += (_, _) => UpdateButtons();
            _list.MarqueeStateChanged += (_, _) => UpdateButtons();
            _list.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Delete) RemoveSelected();
            };

            _nameInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                // 直接编辑输出名时自动切换为自定义命名
                _settings.NamingMode = NamingMode.Custom;
                _settings.CustomName = _nameInput.Text;
                UpdateNamePreview();
                UpdateButtons();
            };

            _folderInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _manualFolder = true;
            };

            _list.MouseDown += (_, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    int hit = _list.IndexFromPoint(e.Location);
                    if (hit < 0)
                    {
                        _list.ClearSelected();
                    }
                    else if (!_list.GetSelected(hit))
                    {
                        // 右键点在未选中的行上：改为只选中这一行；已在多选中的行保持多选
                        _list.ClearSelected();
                        _list.SetSelected(hit, true);
                    }
                    return;
                }

                if (e.Button != MouseButtons.Left || _running) return;
                _dragStart = e.Location;
                _dragSourceIndex = _list.IndexFromPoint(e.Location);
                _dragArmed = _dragSourceIndex >= 0;
            };
            _list.ContextMenuStrip = _contextMenu;
            _list.MouseMove += (_, e) =>
            {
                if (!_dragArmed || _running || e.Button != MouseButtons.Left) return;
                int dx = Math.Abs(e.X - _dragStart.X);
                int dy = Math.Abs(e.Y - _dragStart.Y);
                if (dx < 6 && dy < 6) return;

                _dragArmed = false;
                var data = new DataObject();
                data.SetData(ReorderFormat, _dragSourceIndex);
                _list.DoDragDrop(data, DragDropEffects.Move);
            };
            _list.MouseUp += (_, _) => _dragArmed = false;

            _list.DragEnter += OnListDragEnter;
            _list.DragOver += OnListDragOver;
            _list.DragDrop += OnListDragDrop;
            _list.DragLeave += (_, _) => _dropLine.Visible = false;
            AllowDrop = true;
            DragEnter += (_, e) =>
            {
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            };
            DragDrop += (_, e) =>
            {
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var paths = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                    if (paths != null) AddFiles(paths);
                }
            };

            FormClosing += (_, e) =>
            {
                if (_running)
                {
                    e.Cancel = true;
                    ThemedMessageBox.Show(this,
                        Loc.T(Txt.MsgMergeBusy),
                        Loc.T(Txt.AppName), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_convertPage.IsBusy)
                {
                    e.Cancel = true;
                    ThemedMessageBox.Show(this,
                        Loc.T(Txt.MsgConvertBusy),
                        Loc.T(Txt.AppName), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_imagePdfPage.IsBusy)
                {
                    e.Cancel = true;
                    ThemedMessageBox.Show(this,
                        Loc.T(Txt.MsgImagePdfBusy),
                        Loc.T(Txt.AppName), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_wordPdfPage.IsBusy)
                {
                    e.Cancel = true;
                    ThemedMessageBox.Show(this,
                        Loc.T(Txt.MsgWordPdfBusy),
                        Loc.T(Txt.AppName), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _settings.Save();
            };

            FormClosed += (_, _) =>
            {
                if (_toast != null)
                {
                    _toast.HideToast();
                    _toast = null;
                }

                if (_tray == null) return;
                _tray.Visible = false;
                _tray.Dispose();
                _tray = null;
            };
        }

        /// <summary>设置窗口内的任何改动都会立即回调到这里，无需再点保存。</summary>
        private void OpenSettings(int page)
        {
            using var dialog = new SettingsForm(_settings, Math.Clamp(page, 0, 4));
            dialog.Applied += ApplySettingsLive;
            dialog.ShowDialog(this);
        }

        private void ApplySettingsLive(AppSettings updated)
        {
            bool languageChanged = _settings.Language != updated.Language;
            bool skinChanged = !string.Equals(_settings.SkinId, updated.SkinId, StringComparison.OrdinalIgnoreCase);
            bool namingChanged = _settings.NamingMode != updated.NamingMode ||
                !string.Equals(_settings.NamePhrase, updated.NamePhrase, StringComparison.Ordinal) ||
                !string.Equals(_settings.CustomName, updated.CustomName, StringComparison.Ordinal);
            bool convertDefaultsChanged =
                _settings.ConvertFormat != updated.ConvertFormat ||
                _settings.ConvertPpi != updated.ConvertPpi ||
                _settings.ConvertJpegQuality != updated.ConvertJpegQuality ||
                _settings.ConvertTransparentBackground != updated.ConvertTransparentBackground;
            OutputLocationMode previousMode = _settings.OutputLocationMode;
            string previousCustomFolder = _settings.CustomOutputFolder;

            _settings.CopyFrom(updated);
            // 默认输出名（如「合并文档」）跟随界面语言一起切换
            if (languageChanged) _settings.SyncDefaultName(_settings.Language);
            _settings.Save();

            bool outputChanged = _settings.OutputLocationMode != previousMode ||
                (_settings.OutputLocationMode == OutputLocationMode.Custom &&
                 !string.Equals(_settings.CustomOutputFolder, previousCustomFolder, StringComparison.OrdinalIgnoreCase));

            if (outputChanged)
                _manualFolder = false;

            // 只有换肤/换语言才需要重建主题与磨砂壁纸；
            // 拖动水印参数时会连续回调，每次都重建会让设置窗口明显卡顿
            if (languageChanged || skinChanged)
            {
                Theme.ApplySkin(SkinPalette.FromId(_settings.SkinId));
                if (languageChanged) ApplyLanguage();
                ApplyTheme();

                // 换肤后设置窗口的壁纸要重新生成，趁用户还在主界面时后台先做好
                GlassPaint.PrewarmBackdrop(SettingsForm.DialogSize, Theme.Current);
            }

            if (outputChanged || languageChanged) RefreshFolder();
            if (namingChanged || outputChanged || languageChanged) UpdateNamePreview();
            _convertPage.ApplySettings();
            if (convertDefaultsChanged) _convertPage.ApplyConvertDefaults();
            _imagePdfPage.ApplySettings();
            _wordPdfPage.ApplySettings();
            LayoutSideCard();
            UpdateQuoteTimer();
            UpdateButtons();
        }

        /// <summary>语言、字体与文案即时刷新，无需重启程序。</summary>
        private void ApplyLanguage()
        {
            Loc.SetLanguage(_settings.Language);
            Theme.RefreshFonts();

            Text = Loc.T(Txt.AppName);
            Font = Theme.BaseFont;

            _appTitle.Text = Loc.T(Txt.AppName);
            _appTitle.Font = Theme.SectionFont;
            _versionLabel.Font = Theme.BadgeFont;

            _navMerge.Title = Loc.T(Txt.NavMergeTitle);
            _navMerge.Subtitle = Loc.T(Txt.NavMergeSubtitle);
            _navConvert.Title = Loc.T(Txt.NavConvertTitle);
            _navConvert.Subtitle = Loc.T(Txt.NavConvertSubtitle);
            _navImagePdf.Title = Loc.T(Txt.NavImagePdfTitle);
            _navImagePdf.Subtitle = Loc.T(Txt.NavImagePdfSubtitle);
            _navWordPdf.Title = Loc.T(Txt.NavWordPdfTitle);
            _navWordPdf.Subtitle = Loc.T(Txt.NavWordPdfSubtitle);
            _sideSkin.Text = Loc.T(Txt.Skin);
            _sideSettings.Text = Loc.T(Txt.Settings);
            _sideAbout.Text = Loc.T(Txt.About);
            _sideWatermark.Text = Loc.T(Txt.SettingsNavWatermark);
            _sideDone.Text = Loc.T(Txt.SettingsNavDone);
            _sideSkin.ApplyTheme();
            _sideSettings.ApplyTheme();
            _sideAbout.ApplyTheme();
            _sideWatermark.ApplyTheme();
            _sideDone.ApplyTheme();

            _pageTitle.Font = Theme.SectionFont;
            _pageSubtitle.Font = Theme.SmallFont;
            UpdatePageHeader();

            _sectionLabel.Text = Loc.T(Txt.DocsTitle);
            _sectionLabel.Font = Theme.SectionFont;
            _countLabel.Font = Theme.SmallFont;
            _hintLabel.Text = Loc.T(Txt.ListHint);
            _hintLabel.Font = Theme.SmallFont;
            _quoteCard.SetLanguage(_settings.Language);
            _localLabel.Text = Loc.T(Txt.LocalBadge);
            _localLabel.Font = Theme.SmallFont;

            _addButton.Text = Loc.T(Txt.AddDocs);
            _upButton.Text = Loc.T(Txt.MoveUp);
            _downButton.Text = Loc.T(Txt.MoveDown);
            _removeButton.Text = Loc.T(Txt.Remove);
            _clearButton.Text = Loc.T(Txt.Clear);
            foreach (Control button in new Control[] { _addButton, _upButton, _downButton, _removeButton, _clearButton })
                button.Font = Theme.ButtonFont;

            _outputLabel.Text = Loc.T(Txt.OutputFile);
            _outputLabel.Font = Theme.BaseFont;
            _folderLabel.Text = Loc.T(Txt.SaveTo);
            _folderLabel.Font = Theme.BaseFont;
            _pdfSuffix.Font = Theme.InputFont;
            _browseButton.Text = Loc.T(Txt.Browse);
            _browseButton.Font = Theme.ButtonFont;
            _viewResultButton.Text = Loc.T(Txt.ViewResult);
            _viewResultButton.Font = Theme.ButtonFont;
            _startButton.Text = _running ? Loc.T(Txt.MergingButton) : Loc.T(Txt.StartMerge);
            _startButton.Font = Theme.PrimaryButtonFont;
            _statusLabel.Font = Theme.BaseFont;

            _nameInput.Box.Font = Theme.InputFont;
            _folderInput.Box.Font = Theme.InputFont;
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);

            _list.Font = Theme.ItemFont;
            _menuTop.Text = Loc.T(Txt.MoveTop);
            _menuBottom.Text = Loc.T(Txt.MoveBottom);
            _menuUp.Text = Loc.T(Txt.MoveUp);
            _menuDown.Text = Loc.T(Txt.MoveDown);
            _menuRemove.Text = Loc.T(Txt.Remove);
            _contextMenu.Font = Theme.BaseFont;
            if (_trayOpenItem != null) _trayOpenItem.Text = Loc.T(Txt.TrayOpen);
            if (_trayExitItem != null) _trayExitItem.Text = Loc.T(Txt.TrayExit);
            if (_tray != null) _tray.Text = Loc.T(Txt.AppName);

            ConfigureToolTips();
            SetStatus(Loc.T(Txt.ReadyStatus));

            _convertPage.ApplyLanguage();
            _imagePdfPage.ApplyLanguage();
            _wordPdfPage.ApplyLanguage();
            LayoutLocalizedControls();
            UpdateNamePreview();
            RefreshList(_list.SelectedIndex);
            UpdateButtons();
        }

        private void OpenAbout()
        {
            using var dialog = new AboutForm();
            dialog.ShowDialog(this);
        }

        private void ApplyTheme()
        {
            BackColor = Theme.Background;
            SkinManager.Apply(this);
            _appTitle.ForeColor = Theme.HeaderText;
            _versionLabel.ForeColor = Theme.HeaderSubtitle;
            _brandDivider.BackColor = Color.FromArgb(70, Theme.Separator);
            _sideDivider.BackColor = Color.FromArgb(70, Theme.Separator);
            _divider.BackColor = Theme.Separator;
            _navMerge.ApplyTheme();
            _navConvert.ApplyTheme();
            _navImagePdf.ApplyTheme();
            _navWordPdf.ApplyTheme();
            _sideSkin.ApplyTheme();
            _sideSettings.ApplyTheme();
            _sideAbout.ApplyTheme();
            _sideWatermark.ApplyTheme();
            _sideDone.ApplyTheme();
            _convertPage.ApplyTheme();
            _imagePdfPage.ApplyTheme();
            _wordPdfPage.ApplyTheme();
            _list.ApplyTheme();
            _emptyPanel.Invalidate();
            _progress.Invalidate();
            _quoteCard.ApplyTheme();
            GlassPaint.ApplyWindowChrome(this, Theme.IsDark);
            Invalidate(true);
            RebuildBackdrop();
        }

        // ---------------- 磨砂玻璃 ----------------

        Bitmap? IGlassScene.SharpBackdrop => _backdropSharp;
        Bitmap? IGlassScene.FrostedBackdrop => _backdropFrosted;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!IsHandleCreated || _backdropSharp == null)
            {
                base.OnPaintBackground(e);
                return;
            }
            GlassPaint.Paint(this, e.Graphics, Theme.GlassTint, true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RebuildBackdrop();
            UpdateQuoteTimer();

            // 后台预生成设置窗口的壁纸（约 70ms），避免第一次点开侧栏「设置」时现算而卡一下
            GlassPaint.PrewarmBackdrop(SettingsForm.DialogSize, Theme.Current);
        }

        /// <summary>按设置启用/停用名言自动换句，并同步刷新间隔。</summary>
        private void UpdateQuoteTimer()
        {
            int seconds = Math.Clamp(_settings.QuoteRefreshSeconds,
                AppSettings.MinQuoteRefreshSeconds, AppSettings.MaxQuoteRefreshSeconds);
            int interval = seconds * 1000;
            if (_quoteTimer.Interval != interval) _quoteTimer.Interval = interval;

            if (_settings.ShowQuotes && _settings.QuoteAutoRefresh)
                _quoteTimer.Start();
            else
                _quoteTimer.Stop();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildBackdrop();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsHandleCreated) return;
            LayoutSidebar();
            LayoutHeader();
            LayoutMergeCards();
            _backdropTimer.Stop();
            _backdropTimer.Start();
        }

        /// <summary>重建壁纸与模糊层；尺寸变化或换肤后调用。</summary>
        private void RebuildBackdrop()
        {
            if (!IsHandleCreated) return;
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            Bitmap sharp = GlassPaint.BuildWallpaper(ClientSize, Theme.Current);
            Bitmap frosted = GlassPaint.Frost(sharp);

            Bitmap? oldSharp = _backdropSharp;
            Bitmap? oldFrosted = _backdropFrosted;
            _backdropSharp = sharp;
            _backdropFrosted = frosted;
            oldSharp?.Dispose();
            oldFrosted?.Dispose();

            RefreshListBackdrop();
            Invalidate(true);
        }

        /// <summary>ListBox 是原生控件、不支持透明，取所在区域的磨砂平均色作为底色。</summary>
        private void RefreshListBackdrop()
        {
            if (!IsHandleCreated) return;
            _list.BackColor = GlassPaint.SampleOpaque(_list, Theme.GlassFill);
            _emptyPanel.Invalidate();
        }

        private void EnableFileDrop(Control control)
        {
            if (control == _list) return;
            if (control is TextBox)
            {
                control.AllowDrop = false;
                return;
            }

            control.AllowDrop = true;
            control.DragEnter += OnAnyDragEnter;
            control.DragOver += OnAnyDragOver;
            control.DragDrop += OnAnyDragDrop;

            foreach (Control child in control.Controls)
                EnableFileDrop(child);
        }

        private static bool HasFiles(IDataObject? data)
        {
            return data != null && data.GetDataPresent(DataFormats.FileDrop);
        }

        private void OnAnyDragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = HasFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnAnyDragOver(object? sender, DragEventArgs e)
        {
            e.Effect = HasFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnAnyDragDrop(object? sender, DragEventArgs e)
        {
            if (!HasFiles(e.Data)) return;
            var paths = (string[]?)e.Data!.GetData(DataFormats.FileDrop);
            if (paths != null) AddFiles(paths);
        }

        private void ShowAddDialog()
        {
            using var dialog = new OpenFileDialog
            {
                Title = Loc.T(Txt.DialogTitleAdd),
                Filter = BuildOpenDialogFilter(),
                Multiselect = true,
                CheckFileExists = true,
                RestoreDirectory = true
            };
            if (_documents.Count > 0)
                dialog.InitialDirectory = Path.GetDirectoryName(_documents[^1].Path);

            if (dialog.ShowDialog(this) == DialogResult.OK)
                AddFiles(dialog.FileNames);
        }

        private static string BuildOpenDialogFilter()
        {
            const string wordPattern = "*.doc;*.docx;*.docm;*.dot;*.dotx;*.dotm;*.rtf;*.odt;*.wps;*.wpt";
            const string imagePattern = "*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.bmp;*.dib;*.tif;*.tiff;*.gif;*.webp;*.heic;*.heif;*.avif";
            return $"{Loc.T(Txt.FilterSupport)}|{wordPattern};*.pdf;{imagePattern}|" +
                   $"{Loc.T(Txt.FilterWord)}|{wordPattern}|" +
                   $"{Loc.T(Txt.FilterPdf)}|*.pdf|" +
                   $"{Loc.T(Txt.FilterImage)}|{imagePattern}|" +
                   $"{Loc.T(Txt.FilterAll)}|*.*";
        }

        private void AddFiles(IEnumerable<string> paths)
        {
            int added = 0;
            int skipped = 0;
            var existing = new HashSet<string>(
                _documents.Select(d => d.Path), StringComparer.OrdinalIgnoreCase);

            foreach (string raw in paths)
            {
                string path = raw;
                if (Directory.Exists(path))
                {
                    try
                    {
                        var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                        foreach (var file in files)
                        {
                            if (!MergeEngine.IsSupportedFile(file)) continue;
                            if (TryAddFile(file, existing)) added++;
                            else skipped++;
                        }
                    }
                    catch
                    {
                        skipped++;
                    }
                    continue;
                }

                if (TryAddFile(path, existing)) added++;
                else skipped++;
            }

            if (added > 0)
            {
                RefreshList(_documents.Count - 1);
                string message = Loc.T(Txt.StatusAdded, added);
                if (skipped > 0) message += Loc.T(Txt.StatusSkipped, skipped);
                SetStatus(message);
            }
            else
            {
                UpdateButtons();
                if (skipped > 0)
                    SetStatus(Loc.T(Txt.StatusNothing));
            }
        }

        private bool TryAddFile(string path, HashSet<string> existing)
        {
            try
            {
                if (!File.Exists(path)) return false;
                if (!MergeEngine.IsSupportedFile(path)) return false;

                string full = Path.GetFullPath(path);
                if (existing.Contains(full)) return false;

                existing.Add(full);
                _documents.Add(new DocumentItem(full));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>当前选中的行号（升序）；没有任何选中时回退到当前行。</summary>
        private int[] GetSelectedIndices()
        {
            var indices = new List<int>();
            foreach (int index in _list.SelectedIndices)
                indices.Add(index);
            if (indices.Count == 0 && _list.SelectedIndex >= 0)
                indices.Add(_list.SelectedIndex);
            indices.Sort();
            return indices.ToArray();
        }

        /// <summary>批量选中一段连续的行，并把视图滚动到可见位置。</summary>
        private void SelectRange(int start, int count)
        {
            if (_documents.Count == 0) return;
            start = Math.Clamp(start, 0, _documents.Count - 1);
            count = Math.Clamp(count, 1, _documents.Count - start);

            _list.ClearSelected();
            for (int i = start; i < start + count; i++)
                _list.SetSelected(i, true);
            _list.TopIndex = Math.Max(0, Math.Min(start, _list.Items.Count - 1));
        }

        private void MoveSelected(int offset)
        {
            int[] selected = GetSelectedIndices();
            if (selected.Length == 0) return;

            if (offset < 0)
            {
                if (selected[0] == 0) return;
                foreach (int index in selected)
                {
                    DocumentItem item = _documents[index];
                    _documents.RemoveAt(index);
                    _documents.Insert(index - 1, item);
                }
            }
            else
            {
                if (selected[selected.Length - 1] >= _documents.Count - 1) return;
                for (int k = selected.Length - 1; k >= 0; k--)
                {
                    int index = selected[k];
                    DocumentItem item = _documents[index];
                    _documents.RemoveAt(index);
                    _documents.Insert(index + 1, item);
                }
            }

            int start = offset < 0 ? selected[0] - 1 : selected[0] + 1;
            RefreshList(start);
            SelectRange(start, selected.Length);
            SetStatus(Loc.T(Txt.StatusOrder));
        }

        /// <summary>把选中的文件整体移到列表最前或最后，保持它们之间的原有顺序。</summary>
        private void MoveToEdge(bool top)
        {
            int[] selected = GetSelectedIndices();
            if (selected.Length == 0) return;

            var moved = new List<DocumentItem>();
            var remaining = new List<DocumentItem>();
            int cursor = 0;
            foreach (DocumentItem document in _documents)
            {
                if (Array.IndexOf(selected, cursor) >= 0) moved.Add(document);
                else remaining.Add(document);
                cursor++;
            }

            if (top && remaining.Count == 0) return;
            int start = top ? 0 : remaining.Count;
            if (!top && start == selected[0]) return;

            _documents.Clear();
            if (top)
            {
                _documents.AddRange(moved);
                _documents.AddRange(remaining);
            }
            else
            {
                _documents.AddRange(remaining);
                _documents.AddRange(moved);
            }

            RefreshList(start);
            SelectRange(start, moved.Count);
            SetStatus(Loc.T(top ? Txt.StatusMovedTop : Txt.StatusMovedBottom));
        }

        private void RemoveSelected()
        {
            int[] selected = GetSelectedIndices();
            if (selected.Length == 0) return;

            string name = _documents[selected[0]].FileName;
            int first = selected[0];
            for (int k = selected.Length - 1; k >= 0; k--)
                _documents.RemoveAt(selected[k]);

            int next = Math.Min(first, _documents.Count - 1);
            RefreshList(next);
            SetStatus(selected.Length > 1
                ? Loc.T(Txt.StatusRemovedMulti, selected.Length)
                : Loc.T(Txt.StatusRemoved, name));
        }

        private void ClearAll()
        {
            if (_documents.Count == 0) return;

            if (ThemedMessageBox.Show(this, Loc.T(Txt.MsgClearText),
                    Loc.T(Txt.MsgClearTitle), MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            _documents.Clear();
            _manualFolder = false;
            RefreshList();
            SetStatus(Loc.T(Txt.StatusCleared));
        }

        private void BrowseFolder()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = Loc.T(Txt.FolderDesc),
                UseDescriptionForTitle = true,
                SelectedPath = _folderInput.Text,
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _manualFolder = true;
                _folderInput.Text = dialog.SelectedPath;
                _settings.LastOutputFolder = dialog.SelectedPath;
                _settings.Save();
                SetStatus(Loc.T(Txt.StatusFolderChosen));
            }
        }

        /// <summary>按设置解析合并后的保存目录：最上方文档 / 上一次位置 / 自定义。</summary>
        private string ResolveOutputFolder()
        {
            switch (_settings.OutputLocationMode)
            {
                case OutputLocationMode.LastUsed:
                    if (_settings.LastOutputFolder.Trim().Length > 0)
                        return _settings.LastOutputFolder.Trim();
                    break;
                case OutputLocationMode.Custom:
                    if (_settings.CustomOutputFolder.Trim().Length > 0)
                        return _settings.CustomOutputFolder.Trim();
                    break;
            }

            return _documents.Count > 0
                ? Path.GetDirectoryName(_documents[0].Path) ?? Environment.CurrentDirectory
                : string.Empty;
        }

        /// <summary>重新计算保存目录；用户手动选择过则保留用户选择。</summary>
        private void RefreshFolder()
        {
            if (_manualFolder && _folderInput.Text.Trim().Length > 0) return;
            SetFolderSilently(ResolveOutputFolder());
        }

        private void RefreshList(int selectIndex = -1)
        {
            int previous = _list.SelectedIndex;

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var document in _documents)
                _list.Items.Add(document);
            _list.EndUpdate();

            _countLabel.Text = Loc.T(Txt.DocCountFormat, _documents.Count);
            _emptyPanel.Visible = _documents.Count == 0;

            RefreshFolder();

            if (_documents.Count > 0)
            {
                int index = selectIndex >= 0
                    ? Math.Min(selectIndex, _documents.Count - 1)
                    : Math.Clamp(previous, 0, _documents.Count - 1);
                _list.SelectedIndex = index;
            }

            UpdateNamePreview();
            UpdateButtons();
        }

        private void UpdateNamePreview()
        {
            string? firstPath = _documents.Count > 0 ? _documents[0].Path : null;
            string stem = _settings.BuildNameStem(firstPath, DateTime.Now);
            stem = SanitizeFileName(stem);
            SetNameSilently(MakeSafeDefaultName(stem));

            _nameInput.ReadOnly = false;
            _nameInput.Box.PlaceholderText = PlaceholderForMode(_settings.NamingMode);
            _nameInput.ApplyTheme();
        }

        private static string PlaceholderForMode(NamingMode mode)
        {
            switch (mode)
            {
                case NamingMode.FirstPlusPhrase:
                    return Loc.T(Txt.NamePlaceholderPhrase);
                case NamingMode.MergeTime:
                    return Loc.T(Txt.NamePlaceholderTime);
                case NamingMode.Custom:
                    return Loc.T(Txt.NamePlaceholderCustom);
                default:
                    return Loc.T(Txt.NamePlaceholderFirst);
            }
        }

        private void SetNameSilently(string text)
        {
            _suppressEvents = true;
            _nameInput.Text = text;
            _suppressEvents = false;
        }

        private void SetFolderSilently(string text)
        {
            _suppressEvents = true;
            _folderInput.Text = text;
            _suppressEvents = false;
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
                name = name.Replace(invalid, '_');
            name = name.Trim();
            return name.Length == 0 ? Loc.T(Txt.DefaultDocName) : name;
        }

        private string MakeSafeDefaultName(string stem)
        {
            // 最终文件名由 BuildOutputPath 做同名自动编号，这里只展示基础名称
            return stem;
        }

        private void UpdateButtons()
        {
            int[] selected = GetSelectedIndices();
            bool hasDocuments = _documents.Count > 0;
            bool interactive = hasDocuments && !_running;
            bool hasSelection = selected.Length > 0;

            _addButton.Enabled = !_running;
            _upButton.Enabled = interactive && hasSelection && selected[0] > 0;
            _downButton.Enabled = interactive && hasSelection &&
                selected[selected.Length - 1] < _documents.Count - 1;
            _removeButton.Enabled = interactive && hasSelection;
            _clearButton.Enabled = interactive;
            _cancelBoxButton.Enabled = interactive && _list.HasMarqueeSelection;
            _browseButton.Enabled = !_running;
            _nameInput.Enabled = !_running;
            _list.Enabled = !_running;
            _startButton.Enabled = !_running && hasDocuments;
            _viewResultButton.Enabled = !_running && _viewResultButton.Visible;
        }

        private void UpdateContextMenu()
        {
            int[] selected = GetSelectedIndices();
            bool enabled = selected.Length > 0 && !_running;
            _menuTop.Enabled = enabled && selected[0] > 0;
            _menuBottom.Enabled = enabled && selected[selected.Length - 1] < _documents.Count - 1;
            _menuUp.Enabled = _menuTop.Enabled;
            _menuDown.Enabled = _menuBottom.Enabled;
            _menuRemove.Enabled = enabled;
        }

        private void SetStatus(string text)
        {
            _statusLabel.Text = text;
        }

        private void StartMerge()
        {
            if (_running || _documents.Count == 0) return;

            if (_settings.NamingMode == NamingMode.MergeTime)
                UpdateNamePreview();

            string outputPath;
            try
            {
                outputPath = BuildOutputPath();
            }
            catch (InvalidOperationException ex)
            {
                ThemedMessageBox.Show(this, ex.Message, Loc.T(Txt.ErrCannotStartTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int wordFiles = _documents.Count(d => d.IsWord);
            bool skipWord = false;
            if (wordFiles > 0 && !MergeEngine.IsWordAvailable)
            {
                if (wordFiles == _documents.Count)
                {
                    ThemedMessageBox.Show(this,
                        Loc.T(Txt.MergeWordMissing),
                        Loc.T(Txt.MsgNoWordTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ThemedMessageBox.Show(this,
                        Loc.T(Txt.MsgSkipWordText, wordFiles),
                        Loc.T(Txt.MsgSkipWordTitle), MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return;

                skipWord = true;
            }

            var files = _documents.Select(d => d.Path).ToArray();

            // 带密码的 PDF / Word 先取得打开密码，用户取消则放弃本次合并
            if (!TryCollectPasswords(files, out Dictionary<string, string>? passwords))
                return;

            WatermarkSpec? watermark = BuildWatermarkSpec();
            SetRunningUi(true);
            _viewResultButton.Visible = false;
            LayoutSideCard();
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.StatusPreparing));

            var progress = new Progress<MergeProgress>(p =>
            {
                if (!_running) return;
                _progress.Value = p.Percent;
                _statusLabel.Text = Loc.T(Txt.StatusProgress, p.Percent, p.Status);
            });

            var thread = new Thread(() =>
            {
                try
                {
                    var result = MergeEngine.Run(files, outputPath, progress, skipWord, watermark, passwords);
                    BeginInvoke(new Action(() => FinishSuccess(result)));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() => FinishFailure(ex)));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 合并前逐个询问加密文件的打开密码；用户取消时返回 false，由调用方放弃本次合并。
        /// </summary>
        private bool TryCollectPasswords(string[] files, out Dictionary<string, string>? passwords)
        {
            passwords = null;

            foreach (string file in files)
            {
                bool isPdf = MergeEngine.IsPdfFile(file);
                bool needsPassword = isPdf
                    ? MergeEngine.PdfNeedsPassword(file)
                    : MergeEngine.WordNeedsPassword(file);
                if (!needsPassword) continue;

                string? entered = null;
                while (true)
                {
                    using var dialog = new PasswordPromptForm(
                        Path.GetFileName(file),
                        entered == null ? null : Loc.T(Txt.MergePasswordWrong));

                    if (dialog.ShowDialog(this) != DialogResult.OK) return false;
                    entered = dialog.Password;

                    // Word 的密码只能由 Word 打开时校验，这里仅校验 PDF
                    if (!isPdf || MergeEngine.VerifyPdfPassword(file, entered)) break;
                }

                passwords ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                passwords[file] = entered!;
            }

            return true;
        }

        private string BuildOutputPath()
        {
            if (_documents.Count == 0)
                throw new InvalidOperationException(Loc.T(Txt.ErrNeedDocs));

            string name = _nameInput.Text.Trim();
            if (name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                name = name[..^4].TrimEnd();

            if (name.Length == 0)
                throw new InvalidOperationException(Loc.T(Txt.ErrNeedName));

            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                if (name.Contains(invalid))
                    throw new InvalidOperationException(Loc.T(Txt.ErrBadChars));
            }

            string folder = _folderInput.Text.Trim();
            if (folder.Length == 0)
                throw new InvalidOperationException(Loc.T(Txt.ErrNeedFolder));
            if (!Directory.Exists(folder))
                throw new InvalidOperationException(Loc.T(Txt.ErrFolderMissing, folder));

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(Path.Combine(folder, name + ".pdf"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(Loc.T(Txt.ErrPathInvalid), ex);
            }

            return MakeUniqueOutputPath(fullPath);
        }

        /// <summary>同名时自动编号：A.pdf 已存在则输出 A-1.pdf、A-2.pdf……</summary>
        private static string MakeUniqueOutputPath(string desired)
        {
            if (!File.Exists(desired)) return desired;

            string folder = Path.GetDirectoryName(desired) ?? Environment.CurrentDirectory;
            string stem = Path.GetFileNameWithoutExtension(desired);
            string extension = Path.GetExtension(desired);

            for (int index = 1; index < 1000; index++)
            {
                string candidate = Path.Combine(folder, $"{stem}-{index}{extension}");
                if (!File.Exists(candidate)) return candidate;
            }

            return Path.Combine(folder, $"{stem}-{DateTime.Now:yyyyMMddHHmmss}{extension}");
        }

        private void SetRunningUi(bool running)
        {
            _running = running;
            if (running)
            {
                _startButton.Text = Loc.T(Txt.MergingButton);
                _startButton.Enabled = false;
                _addButton.Enabled = false;
                _upButton.Enabled = false;
                _downButton.Enabled = false;
                _removeButton.Enabled = false;
                _clearButton.Enabled = false;
                _cancelBoxButton.Enabled = false;
                _browseButton.Enabled = false;
                _nameInput.Enabled = false;
                _list.Enabled = false;
                UseWaitCursor = true;
            }
            else
            {
                _startButton.Text = Loc.T(Txt.StartMerge);
                UseWaitCursor = false;
                UpdateButtons();
            }

            UpdateNavigationEnabled();
        }

        private void FinishSuccess(MergeResult result)
        {
            SetRunningUi(false);
            _progress.Value = 100;
            string elapsed = FormatElapsed(result.Elapsed);
            string status = Loc.T(Txt.StatusDone, result.PageCount, elapsed);
            if (result.SkippedWordCount > 0)
                status += "  " + Loc.T(Txt.StatusSkippedWord, result.SkippedWordCount);
            SetStatus(status);

            _lastOutputPath = result.OutputPath;
            _viewResultButton.Visible = true;
            _viewResultButton.Enabled = true;
            LayoutSideCard();
            LayoutLocalizedControls();

            _settings.LastOutputFolder = Path.GetDirectoryName(result.OutputPath) ?? string.Empty;
            _settings.Save();
            RefreshFolder();

            ShowCompletionNotification(result);
            if (_settings.OpenAfterMerge)
                OpenOutput(result.OutputPath);
        }

        private void ShowCompletionNotification(MergeResult result)
        {
            if (_settings.PlaySound)
                SoundLibrary.Play(_settings.SoundId);

            if (!_settings.ShowNotification || IsDisposed) return;

            if (_toast != null)
            {
                _toast.HideToast();
                _toast = null;
            }

            string fileName = Path.GetFileName(result.OutputPath);
            string[] lines = Loc.T(Txt.BalloonText, result.PageCount, FormatElapsed(result.Elapsed), fileName).Split('\n');
            string subtitle = lines[0].TrimEnd('\r');
            string fileLine = lines.Length > 1 ? lines[1].TrimEnd('\r') : fileName;

            var toast = new ToastForm(Loc.T(Txt.ToastTitle), subtitle, fileLine,
                Screen.FromControl(this).WorkingArea, () => OpenOutput(_lastOutputPath));
            toast.FormClosed += (_, _) => _toast = null;
            _toast = toast;
            toast.ShowToast();
        }

        /// <summary>按设置解析水印；文档合并未开启或自定义文字为空时不添加水印。</summary>
        private WatermarkSpec? BuildWatermarkSpec()
        {
            return _settings.WatermarkEnabled ? _settings.CreateWatermarkSpec() : null;
        }

        /// <summary>
        /// 优先调用系统关联的 PDF 阅读器；本机没有 PDF 阅读器时，才使用内置简易阅读器。
        /// </summary>
        private void OpenOutput(string path)
        {
            if (!File.Exists(path))
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.OpenFailed),
                    Loc.T(Txt.AppName), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (PdfShell.HasDefaultReader())
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                    return;
                }
                catch
                {
                    // 关联的阅读器不可用时回退到内置阅读器
                }
            }

            using var viewer = new PdfViewerForm(path);
            viewer.ShowDialog(this);
        }

        private void FinishFailure(Exception exception)
        {
            SetRunningUi(false);
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.FailStatus));

            string message = exception is MergeException
                ? exception.Message
                : Loc.T(Txt.FailPrefix, exception.Message);
            WriteFailureLog(exception);
            ThemedMessageBox.Show(this, message, Loc.T(Txt.FailTitle), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>把合并失败的完整异常写到 %APPDATA%\DocHub\last-error.log，便于排查无法复现的问题。</summary>
        private void WriteFailureLog(Exception exception)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);

                string files = _documents.Count == 0
                    ? "（无）"
                    : string.Join(Environment.NewLine, _documents.Select(d => d.Path));

                File.WriteAllText(Path.Combine(folder, "last-error.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                    $"version: {Application.ProductVersion}{Environment.NewLine}" +
                    $"files:{Environment.NewLine}{files}{Environment.NewLine}{Environment.NewLine}" +
                    exception + Environment.NewLine);
            }
            catch
            {
                // 写日志失败不影响错误提示
            }
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            if (elapsed.TotalSeconds < 60)
                return Loc.T(Txt.ElapsedSeconds, elapsed.TotalSeconds);
            return Loc.T(Txt.ElapsedMinutes, (int)elapsed.TotalMinutes, elapsed.Seconds);
        }

        private void OnListDragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(ReorderFormat))
                e.Effect = DragDropEffects.Move;
            else if (HasFiles(e.Data))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void OnListDragOver(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(ReorderFormat))
            {
                e.Effect = DragDropEffects.Move;
                Point point = _list.PointToClient(new Point(e.X, e.Y));
                ShowDropLine(GetInsertIndex(point));
            }
            else if (HasFiles(e.Data))
            {
                e.Effect = DragDropEffects.Copy;
                _dropLine.Visible = false;
            }
            else
            {
                e.Effect = DragDropEffects.None;
                _dropLine.Visible = false;
            }
        }

        private void OnListDragDrop(object? sender, DragEventArgs e)
        {
            _dropLine.Visible = false;

            if (e.Data != null && e.Data.GetDataPresent(ReorderFormat))
            {
                int from;
                try { from = (int)e.Data.GetData(ReorderFormat)!; }
                catch { return; }

                Point point = _list.PointToClient(new Point(e.X, e.Y));
                MoveDocument(from, GetInsertIndex(point));
                return;
            }

            if (HasFiles(e.Data))
            {
                var paths = (string[]?)e.Data!.GetData(DataFormats.FileDrop);
                if (paths != null) AddFiles(paths);
            }
        }

        private void ShowDropLine(int index)
        {
            int y = index <= 0
                ? 2
                : index >= _documents.Count
                    ? _list.Height - 3
                    : index * _list.ItemHeight;
            var bounds = new Rectangle(_list.Left + 10, _list.Top + y, _list.Width - 20, 2);
            if (_dropLine.Bounds != bounds) _dropLine.Bounds = bounds;
            if (!_dropLine.Visible)
            {
                _dropLine.BringToFront();
                _dropLine.Visible = true;
            }
        }

        private int GetInsertIndex(Point point)
        {
            int index = _list.IndexFromPoint(point);
            if (index < 0)
            {
                if (point.Y < 0) return 0;
                return _list.Items.Count;
            }

            Rectangle rectangle = _list.GetItemRectangle(index);
            return point.Y >= rectangle.Top + rectangle.Height / 2 ? index + 1 : index;
        }

        private void MoveDocument(int from, int to)
        {
            if (from < 0 || from >= _documents.Count) return;

            to = Math.Clamp(to, 0, _documents.Count);
            if (to > from) to--;

            if (to == from) return;

            var item = _documents[from];
            _documents.RemoveAt(from);
            _documents.Insert(to, item);
            RefreshList(to);
            SetStatus(Loc.T(Txt.StatusOrder));
        }
    }
}
