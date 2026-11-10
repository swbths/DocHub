using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>PDF 转图片页：PDF 队列、导出设置、首页预览、进度与取消。</summary>
    public sealed class ConvertWorkspace : UserControl
    {
        private const int SideWidth = 306;
        private const int PreviewHeight = 112;

        private readonly AppSettings _settings;

        private readonly GlassCard _listCard;
        private readonly GlassCard _sideCard;
        private readonly Label _sectionLabel;
        private readonly Label _countLabel;
        private readonly Label _hintLabel;
        private readonly RoundedButton _addButton;
        private readonly IconButton _removeButton;
        private readonly IconButton _clearButton;
        private readonly IconButton _cancelBoxButton;
        private readonly ToolTip _toolTip = new();
        private readonly DocumentListBox _list;
        private readonly EmptyStatePanel _emptyPanel;
        private readonly ImagePreviewBox _preview;
        private readonly Label _previewInfo;
        private readonly Label _namePreview;

        private readonly Label _settingsTitle;
        private readonly Label _formatLabel;
        private readonly SegmentedControl _formatControl;
        private readonly Label _ppiLabel;
        private readonly SegmentedControl _ppiControl;
        private readonly RoundedInput _ppiCustom;
        private readonly Label _qualityLabel;
        private readonly Label _qualityValue;
        private readonly SlimSlider _qualitySlider;
        private readonly SkinCheckBox _transparentBox;
        private readonly SkinCheckBox _subfolderBox;
        private readonly Label _rangeLabel;
        private readonly SegmentedControl _rangeControl;
        private readonly RoundedInput _rangeInput;
        private readonly Label _saveLabel;
        private readonly RoundedInput _folderInput;
        private readonly RoundedButton _browseButton;
        private readonly Panel _divider;
        private readonly Label _statusLabel;
        private readonly SlimProgressBar _progress;
        private readonly RoundedButton _openFolderButton;
        private readonly RoundedButton _startButton;

        private readonly System.Windows.Forms.Timer _previewTimer = new() { Interval = 220 };
        private readonly List<DocumentItem> _documents = new();
        private readonly Dictionary<string, string> _passwords = new(StringComparer.OrdinalIgnoreCase);

        private CancellationTokenSource? _cts;
        private Bitmap? _previewImage;
        private int _previewGeneration;
        private bool _running;
        private bool _suppressEvents;
        private bool _manualFolder;
        // 页面未显示时 Control.Visible 恒为 false，布局必须看这两个独立标记
        private bool _qualityVisible;
        private bool _ppiCustomVisible;
        private string _lastOutputFolder = string.Empty;

        public event EventHandler? BusyChanged;

        public bool IsBusy => _running;

        public ConvertWorkspace(AppSettings settings)
        {
            _settings = settings;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            AllowDrop = true;
            DoubleBuffered = true;
            Font = Theme.BaseFont;

            _listCard = new GlassCard();
            _sideCard = new GlassCard { Strong = true };

            _sectionLabel = MakeLabel(string.Empty, Theme.SectionFont, "section", 20, 14);
            _countLabel = MakeLabel(string.Empty, Theme.SmallFont, "muted", 120, 20);
            _hintLabel = MakeLabel(string.Empty, Theme.SmallFont, "muted", 20, 38);

            _addButton = new RoundedButton
            {
                Role = ButtonRole.Primary,
                Font = Theme.ButtonFont,
                Radius = 9,
                Icon = IconKind.Plus,
                IconSize = 14,
                IconGap = 6,
                Height = 34
            };
            _addButton.ApplyTheme();

            _removeButton = new IconButton { Icon = IconKind.Trash, Size = new Size(30, 30) };
            _clearButton = new IconButton { Icon = IconKind.Clear, Size = new Size(30, 30) };
            _cancelBoxButton = new IconButton
            {
                Icon = IconKind.MarqueeOff,
                Size = new Size(30, 30),
                Enabled = false
            };

            _list = new DocumentListBox
            {
                ShowMoveActions = false,
                ShowDragHandle = false,
                ShowPageCount = true,
                SelectionMode = SelectionMode.MultiExtended,
                AllowDrop = true
            };

            _emptyPanel = new EmptyStatePanel
            {
                TitleOverride = Loc.T(Txt.ConvertEmptyTitle),
                SubtitleOverride = Loc.T(Txt.ConvertEmptySubtitle),
                FormatsOverride = Loc.T(Txt.ConvertEmptyFormats),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _preview = new ImagePreviewBox { EmptyText = Loc.T(Txt.ConvertPreviewEmpty) };
            _previewInfo = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);
            _namePreview = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);

            _settingsTitle = MakeLabel(string.Empty, Theme.SectionFont, "section", 16, 18);
            _formatLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _formatControl = new SegmentedControl();
            _ppiLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _ppiControl = new SegmentedControl();
            _ppiCustom = new RoundedInput { MaxLength = 4, ReadOnly = false };
            _ppiCustom.Box.PlaceholderText = "36–1200";
            _qualityLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _qualityValue = MakeLabel(string.Empty, Theme.SmallFont, "accent", 0, 0);
            _qualityValue.AutoSize = false;
            _qualityValue.TextAlign = ContentAlignment.MiddleRight;
            _qualitySlider = new SlimSlider { Minimum = 1, Maximum = 100, Value = 90 };
            _transparentBox = new SkinCheckBox();
            _subfolderBox = new SkinCheckBox();
            _rangeLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _rangeControl = new SegmentedControl();
            _rangeInput = new RoundedInput { MaxLength = 80 };
            _rangeInput.Box.PlaceholderText = Loc.T(Txt.ConvertPageRangePlaceholder);
            _saveLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _folderInput = new RoundedInput { MaxLength = 260 };
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _browseButton = new RoundedButton
            {
                Role = ButtonRole.Secondary,
                Font = Theme.ButtonFont,
                Radius = 9,
                Height = 36
            };
            _browseButton.ApplyTheme();
            _divider = new Panel { Height = 1, BackColor = Theme.Separator };
            _statusLabel = new Label
            {
                AutoSize = false,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.TopLeft,
                Font = Theme.BaseFont,
                ForeColor = Theme.TextSecondary,
                BackColor = Color.Transparent,
                Tag = "secondary"
            };
            _progress = new SlimProgressBar { StrongGlass = true, Height = 22 };
            _openFolderButton = new RoundedButton
            {
                Role = ButtonRole.Ghost,
                Font = Theme.ButtonFont,
                Radius = 9,
                Height = 30,
                Visible = false
            };
            _openFolderButton.ApplyTheme();
            _startButton = new RoundedButton
            {
                Role = ButtonRole.Primary,
                Font = Theme.PrimaryButtonFont,
                Radius = 11,
                Icon = IconKind.Convert,
                IconSize = 16,
                IconGap = 8,
                Height = 48
            };
            _startButton.ApplyTheme();

            _listCard.Controls.AddRange(new Control[]
            {
                _sectionLabel, _countLabel, _hintLabel, _addButton, _removeButton,
                _clearButton, _cancelBoxButton, _list, _emptyPanel, _preview, _previewInfo, _namePreview
            });
            _sideCard.Controls.AddRange(new Control[]
            {
                _settingsTitle, _formatLabel, _formatControl, _ppiLabel, _ppiControl,
                _ppiCustom, _qualityLabel, _qualityValue, _qualitySlider,
                _transparentBox, _subfolderBox, _rangeLabel, _rangeControl, _rangeInput,
                _saveLabel, _folderInput, _browseButton, _divider, _statusLabel,
                _progress, _openFolderButton, _startButton
            });
            Controls.Add(_listCard);
            Controls.Add(_sideCard);

            _list.ActionRemove += (_, _) => RemoveSelected();
            _list.SelectedIndexChanged += (_, _) =>
            {
                UpdateButtons();
                SchedulePreview();
            };
            _list.MarqueeStateChanged += (_, _) => UpdateButtons();
            _list.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Delete) RemoveSelected();
            };

            _addButton.Click += (_, _) => ShowAddDialog();
            _removeButton.Click += (_, _) => RemoveSelected();
            _clearButton.Click += (_, _) => ClearAll();
            _cancelBoxButton.Click += (_, _) =>
            {
                _list.CancelMarqueeSelection();
                UpdateButtons();
            };
            _browseButton.Click += (_, _) => BrowseFolder();
            _openFolderButton.Click += (_, _) => OpenFolder(_lastOutputFolder);
            _startButton.Click += (_, _) => StartOrCancel();

            _folderInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _manualFolder = true;
            };
            _rangeInput.TextChanged += (_, _) => SchedulePreview();
            _rangeControl.SelectionChanged += (_, _) =>
            {
                UpdateRangeVisibility();
                LayoutSideCard();
                UpdateButtons();
                SchedulePreview();
            };
            _ppiControl.SelectionChanged += (_, _) =>
            {
                SetPpiCustomVisible(IsCustomPpi);
                LayoutSideCard();
                SchedulePreview();
            };
            _formatControl.SelectionChanged += (_, _) =>
            {
                UpdateQualityVisibility();
                UpdateButtons(); // 透明背景仅 PNG 可用
                UpdateOutputInfo();
                LayoutSideCard();
                SchedulePreview();
            };
            _ppiCustom.TextChanged += (_, _) =>
            {
                UpdateOutputInfo();
                SchedulePreview();
            };
            _transparentBox.CheckedChanged += (_, _) => SchedulePreview();
            _subfolderBox.CheckedChanged += (_, _) => UpdateOutputInfo();
            _qualitySlider.ValueChanged += (_, _) =>
            {
                _qualityValue.Text = _qualitySlider.Value.ToString();
            };

            _previewTimer.Tick += (_, _) =>
            {
                _previewTimer.Stop();
                RefreshPreview();
            };

            EnableFileDrop(_listCard);
            EnableFileDrop(_sideCard);

            Resize += (_, _) => LayoutCards();
            _listCard.Resize += (_, _) => LayoutListCard();
            _sideCard.Resize += (_, _) => LayoutSideCard();

            ResolveDefaults();
            ApplyLanguage();
            ApplySettings();
            ApplyTheme();
            RefreshList();
        }

        private static Label MakeLabel(string text, Font font, string role, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = Theme.TextPrimary,
                BackColor = Color.Transparent,
                Tag = role,
                AutoSize = false,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, y)
            };
        }

        // ---------------- 对外接口 ----------------

        public void ApplyTheme()
        {
            BackColor = Color.Transparent;
            _divider.BackColor = Theme.Separator;
            SkinManager.Apply(this);
            _list.ApplyTheme();
            _emptyPanel.Invalidate();
            _preview.ApplyTheme();
            _progress.ApplyTheme();
            _formatControl.ApplyTheme();
            _ppiControl.ApplyTheme();
            _rangeControl.ApplyTheme();
            _qualitySlider.ApplyTheme();
            Invalidate(true);
        }

        public void ApplyLanguage()
        {
            Font = Theme.BaseFont;
            _sectionLabel.Text = Loc.T(Txt.ConvertFilesTitle);
            _sectionLabel.Font = Theme.SectionFont;
            _countLabel.Font = Theme.SmallFont;
            _hintLabel.Text = Loc.T(Txt.ConvertListHint);
            _hintLabel.Font = Theme.SmallFont;

            _addButton.Text = Loc.T(Txt.ConvertAdd);
            _addButton.Font = Theme.ButtonFont;
            _addButton.ApplyTheme();
            _browseButton.Text = Loc.T(Txt.Browse);
            _browseButton.Font = Theme.ButtonFont;
            _browseButton.ApplyTheme();

            _emptyPanel.TitleOverride = Loc.T(Txt.ConvertEmptyTitle);
            _emptyPanel.SubtitleOverride = Loc.T(Txt.ConvertEmptySubtitle);
            _emptyPanel.FormatsOverride = Loc.T(Txt.ConvertEmptyFormats);
            _preview.EmptyText = Loc.T(Txt.ConvertPreviewEmpty);
            _preview.Invalidate();

            _settingsTitle.Text = Loc.T(Txt.ConvertSettingsTitle);
            _settingsTitle.Font = Theme.SectionFont;
            _formatLabel.Text = Loc.T(Txt.ConvertFormat);
            _ppiLabel.Text = Loc.T(Txt.ConvertPpi);
            _qualityLabel.Text = Loc.T(Txt.ConvertQuality);
            _transparentBox.Text = Loc.T(Txt.ConvertBackground);
            _subfolderBox.Text = Loc.T(Txt.ConvertSubfolder);
            _rangeLabel.Text = Loc.T(Txt.ConvertPageRange);
            _rangeControl.Segments = new[]
            {
                Loc.T(Txt.ConvertPageModeAll),
                Loc.T(Txt.ConvertPageModeOdd),
                Loc.T(Txt.ConvertPageModeEven),
                Loc.T(Txt.ConvertPageModeCustom)
            };
            _saveLabel.Text = Loc.T(Txt.ConvertOutput);
            _rangeInput.Box.PlaceholderText = Loc.T(Txt.ConvertPageRangePlaceholder);
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _openFolderButton.Text = Loc.T(Txt.ConvertOpenFolder);
            _openFolderButton.Font = Theme.ButtonFont;
            _openFolderButton.ApplyTheme();

            foreach (Label label in new[] { _formatLabel, _ppiLabel, _qualityLabel, _rangeLabel, _saveLabel })
                label.Font = Theme.BaseFont;
            _qualityValue.Font = Theme.SmallFont;
            _statusLabel.Font = Theme.BaseFont;
            _previewInfo.Font = Theme.SmallFont;
            _namePreview.Font = Theme.SmallFont;

            // 只更新分段文字，保留用户当前选择
            _formatControl.Segments = new[] { "PNG", "JPEG", "BMP", "TIFF" };
            _ppiControl.Segments = new[]
            {
                "72", "96", "150", "300", "600", Loc.T(Txt.ConvertPpiCustom)
            };
            _qualityValue.Text = _qualitySlider.Value.ToString();
            _countLabel.Text = Loc.T(Txt.ConvertCountFormat, _documents.Count);
            if (!_running) SetStatus(Loc.T(Txt.ConvertReady));
            _startButton.Text = _running ? Loc.T(Txt.ConvertRunning) : Loc.T(Txt.ConvertStart);
            _startButton.Font = Theme.PrimaryButtonFont;

            _list.ApplyTheme();
            UpdateOutputInfo();
            RefreshList(_list.SelectedIndex);
            ConfigureToolTips();
            LayoutCards();
        }

        /// <summary>设置窗口里的命名/保存位置等发生变化后刷新显示。</summary>
        public void ApplySettings()
        {
            if (!_manualFolder) SetFolderSilently(ResolveOutputFolder());
            _subfolderBox.Checked = _settings.ConvertCreateSubfolder;
            UpdateOutputInfo();
            UpdateButtons();
        }

        /// <summary>设置窗口修改「转换默认值」后立即同步到工作区，不必重启。</summary>
        public void ApplyConvertDefaults()
        {
            if (_running) return;
            RefreshFormatSegments();
            RefreshPpiSegments();
            RefreshQualityValue();
            LayoutSideCard();
            UpdateButtons();
            SchedulePreview();
        }

        public void CancelRunning()
        {
            if (_running) _cts?.Cancel();
        }

        public void PromptAddFiles()
        {
            if (!_running) ShowAddDialog();
        }

        // ---------------- 布局 ----------------

        private void LayoutCards()
        {
            if (Width <= 0 || Height <= 0) return;

            int left = 24;
            int right = 20;
            int gap = 16;
            int sideWidth = Math.Min(SideWidth, Math.Max(220, Width - left - right - gap - 260));
            int listWidth = Math.Max(200, Width - left - right - gap - sideWidth);
            int height = Math.Max(120, Height - 18);

            _listCard.Bounds = new Rectangle(left, 0, listWidth, height);
            _sideCard.Bounds = new Rectangle(left + listWidth + gap, 0, sideWidth, height);
            LayoutListCard();
            LayoutSideCard();
        }

        private void LayoutListCard()
        {
            int width = _listCard.ClientSize.Width;
            int height = _listCard.ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            int addWidth = FitWidth(_addButton.Text, _addButton.Font, 54, 112);
            _addButton.Bounds = new Rectangle(width - 16 - addWidth, 12, addWidth, 34);

            const int gap = 4;
            int x = _addButton.Left - 14 - 30;
            _cancelBoxButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _clearButton.Bounds = new Rectangle(x, 14, 30, 30);
            x -= 30 + gap;
            _removeButton.Bounds = new Rectangle(x, 14, 30, 30);

            // 标题和计数只能在图标按钮左侧结束：透明标签会绘制父容器背景，
            // 一旦与右侧按钮重叠就会把按钮「擦掉」。
            int sectionWidth = TextRenderer.MeasureText(_sectionLabel.Text, _sectionLabel.Font).Width + 2;
            _sectionLabel.Bounds = new Rectangle(20, 12, Math.Max(80, sectionWidth), 26);
            int countLeft = _sectionLabel.Right + 10;
            _countLabel.Bounds = new Rectangle(countLeft, 18,
                Math.Max(0, _removeButton.Left - 10 - countLeft), 20);

            // 提示单独占标题右侧下方的一整行，不能压住「添加 PDF」按钮。
            _hintLabel.Bounds = new Rectangle(20, 50, Math.Max(60, width - 40), 16);

            int previewArea = PreviewHeight + 62;
            int listTop = 72;
            int listHeight = Math.Max(80, height - listTop - previewArea);
            _list.Bounds = new Rectangle(10, listTop, Math.Max(60, width - 20), listHeight);
            _emptyPanel.Bounds = _list.Bounds;

            int previewTop = _list.Bottom + 10;
            _preview.Bounds = new Rectangle(10, previewTop, Math.Max(60, width - 20), PreviewHeight);
            _previewInfo.Bounds = new Rectangle(12, _preview.Bottom + 4, Math.Max(60, width - 24), 18);
            _namePreview.Bounds = new Rectangle(12, _previewInfo.Bottom, Math.Max(60, width - 24), 18);

            _list.BackColor = GlassPaint.SampleOpaque(_list, Theme.GlassFill);
            _emptyPanel.Invalidate();
        }

        private void LayoutSideCard()
        {
            int width = _sideCard.ClientSize.Width;
            int height = _sideCard.ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            int inner = Math.Max(80, width - 32);
            int y = 18;

            _settingsTitle.Bounds = new Rectangle(16, y, inner, 26);
            y += 30;

            _formatLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 20;
            _formatControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 38;

            _ppiLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 20;
            _ppiControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 34;

            if (_ppiCustomVisible)
            {
                _ppiCustom.Bounds = new Rectangle(16, y, inner, 32);
                y += 38;
            }

            if (_qualityVisible)
            {
                _qualityLabel.Bounds = new Rectangle(16, y, Math.Max(60, inner - 50), 20);
                _qualityValue.Bounds = new Rectangle(16 + Math.Max(60, inner - 50) + 4, y, Math.Min(46, inner - 60), 20);
                y += 20;
                _qualitySlider.Bounds = new Rectangle(16, y, inner, 22);
                y += 26;
            }

            _transparentBox.Bounds = new Rectangle(16, y, inner, 24);
            y += 26;
            _subfolderBox.Bounds = new Rectangle(16, y, inner, 24);
            y += 30;

            _rangeLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 20;
            _rangeControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 36;
            if (IsCustomPageMode)
            {
                _rangeInput.Bounds = new Rectangle(16, y, inner, 34);
                y += 40;
            }

            _saveLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 20;
            int browseWidth = FitWidth(_browseButton.Text, _browseButton.Font, 28, 58);
            _browseButton.Bounds = new Rectangle(16 + inner - browseWidth, y, browseWidth, 36);
            _folderInput.Bounds = new Rectangle(16, y, Math.Max(60, inner - browseWidth - 8), 36);
            y += 44;

            _divider.Bounds = new Rectangle(16, y, inner, 1);
            y += 12;

            const int startHeight = 48;
            _startButton.Bounds = new Rectangle(16, height - 18 - startHeight, inner, startHeight);
            _openFolderButton.Bounds = new Rectangle(16, _startButton.Top - 38, inner, 30);

            // 设置项较多时，状态与进度上移，避免和底部按钮重叠
            int statusY = Math.Min(y, _startButton.Top - 74);
            _statusLabel.Bounds = new Rectangle(16, statusY, inner, 40);
            _progress.Bounds = new Rectangle(16, statusY + 46, inner, 22);
        }

        private static int FitWidth(string text, Font font, int padding, int minimum)
        {
            int width = TextRenderer.MeasureText(text, font).Width + padding;
            return Math.Max(minimum, width);
        }

        // ---------------- 设置与队列 ----------------

        private void RefreshFormatSegments()
        {
            _formatControl.Segments = new[] { "PNG", "JPEG", "BMP", "TIFF" };
            _formatControl.SelectedIndex = (int)_settings.ConvertFormat;
        }

        private void RefreshPpiSegments()
        {
            int ppi = _settings.ConvertPpi;
            int presetIndex = Array.IndexOf(new[] { 72, 96, 150, 300, 600 }, ppi);
            _ppiControl.Segments = new[]
            {
                "72", "96", "150", "300", "600", Loc.T(Txt.ConvertPpiCustom)
            };
            if (presetIndex >= 0)
            {
                _ppiControl.SelectedIndex = presetIndex;
                SetPpiCustomVisible(false);
            }
            else
            {
                _ppiControl.SelectedIndex = 5;
                SetPpiCustomVisible(true);
                _ppiCustom.Text = ppi.ToString();
            }
        }

        private void SetPpiCustomVisible(bool visible)
        {
            _ppiCustomVisible = visible;
            _ppiCustom.Visible = visible;
        }

        private void RefreshQualityValue()
        {
            UpdateQualityVisibility();
            _qualitySlider.Value = Math.Clamp(_settings.ConvertJpegQuality, 1, 100);
            _qualityValue.Text = _qualitySlider.Value.ToString();
            _transparentBox.Checked = _settings.ConvertTransparentBackground;
            _subfolderBox.Checked = _settings.ConvertCreateSubfolder;
        }

        private void UpdateQualityVisibility()
        {
            bool visible = CurrentFormat == ConvertImageFormat.Jpeg;
            _qualityVisible = visible;
            _qualityLabel.Visible = visible;
            _qualityValue.Visible = visible;
            _qualitySlider.Visible = visible;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible) LayoutCards();
        }

        private void SetRangeSilently(string text)
        {
            _suppressEvents = true;
            _rangeInput.Text = text ?? string.Empty;
            _suppressEvents = false;
        }

        private ConvertImageFormat CurrentFormat =>
            (ConvertImageFormat)Math.Clamp(_formatControl.SelectedIndex, 0, 3);

        private bool IsCustomPpi => _ppiControl.SelectedIndex == 5;

        private ConvertPageSelection CurrentPageMode =>
            (ConvertPageSelection)Math.Clamp(_rangeControl.SelectedIndex, 0, 3);

        private bool IsCustomPageMode => CurrentPageMode == ConvertPageSelection.Custom;

        private void UpdateRangeVisibility()
        {
            _rangeInput.Visible = IsCustomPageMode;
        }

        private int CurrentPpi
        {
            get
            {
                if (!IsCustomPpi)
                {
                    int[] presets = { 72, 96, 150, 300, 600 };
                    return presets[Math.Clamp(_ppiControl.SelectedIndex, 0, presets.Length - 1)];
                }

                return int.TryParse(_ppiCustom.Text.Trim(), out int ppi)
                    ? Math.Clamp(ppi, PdfImageEngine.MinPpi, PdfImageEngine.MaxPpi)
                    : _settings.ConvertPpi;
            }
        }

        private void ResolveDefaults()
        {
            RefreshFormatSegments();
            RefreshPpiSegments();
            RefreshQualityValue();
            _rangeControl.SelectedIndex = (int)_settings.ConvertPageMode;
            SetRangeSilently(_settings.ConvertPageRange);
            UpdateRangeVisibility();
            LayoutSideCard();
        }

        private void ShowAddDialog()
        {
            if (_running) return;
            using var dialog = new OpenFileDialog
            {
                Title = Loc.T(Txt.ConvertDialogTitle),
                Filter = $"{Loc.T(Txt.FilterPdf)}|*.pdf|{Loc.T(Txt.FilterAll)}|*.*",
                Multiselect = true,
                CheckFileExists = true,
                RestoreDirectory = true
            };
            if (_documents.Count > 0)
                dialog.InitialDirectory = Path.GetDirectoryName(_documents[^1].Path);

            if (dialog.ShowDialog(this) == DialogResult.OK)
                AddFiles(dialog.FileNames);
        }

        private void AddFiles(IEnumerable<string> paths)
        {
            if (_running) return;

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
                        foreach (string file in files)
                        {
                            if (!MergeEngine.IsPdfFile(file)) continue;
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

                if (!File.Exists(path)) { skipped++; continue; }
                if (!MergeEngine.IsPdfFile(path)) { skipped++; continue; }
                if (TryAddFile(path, existing)) added++;
                else skipped++;
            }

            if (added > 0)
            {
                RefreshList(_documents.Count - 1);
                string message = Loc.T(Txt.ConvertAdded, added);
                int nonPdf = paths.Count(p => File.Exists(p) && !MergeEngine.IsPdfFile(p));
                if (nonPdf > 0) message += "  ·  " + Loc.T(Txt.ConvertSkippedNonPdf, nonPdf);
                SetStatus(message);
            }
            else
            {
                UpdateButtons();
                SetStatus(skipped > 0 ? Loc.T(Txt.ConvertNothingAdded) : Loc.T(Txt.ConvertReady));
            }
        }

        private bool TryAddFile(string path, HashSet<string> existing)
        {
            try
            {
                if (!File.Exists(path) || !MergeEngine.IsPdfFile(path)) return false;
                string full = Path.GetFullPath(path);
                if (!existing.Add(full)) return false;

                var item = new DocumentItem(full) { PageCount = -1 };
                _documents.Add(item);
                ProbePageCount(item);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ProbePageCount(DocumentItem item)
        {
            string path = item.Path;
            Task.Run(() =>
            {
                int? count = PdfImageEngine.TryGetPageCount(path);
                if (!IsHandleCreated || IsDisposed) return;
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (IsDisposed) return;
                        item.PageCount = count ?? -2;
                        int index = _documents.IndexOf(item);
                        if (index >= 0 && index < _list.Items.Count)
                        {
                            _list.Items[index] = item;
                            _list.Invalidate(_list.GetItemRectangle(index));
                        }
                    }));
                }
                catch
                {
                    // 窗口已关闭
                }
            });
        }

        private void RemoveSelected()
        {
            if (_running) return;
            var selected = new List<int>();
            foreach (int i in _list.SelectedIndices) selected.Add(i);
            if (selected.Count == 0) return;
            selected.Sort();

            for (int k = selected.Count - 1; k >= 0; k--)
            {
                if (selected[k] >= 0 && selected[k] < _documents.Count)
                    _documents.RemoveAt(selected[k]);
            }

            RefreshList(Math.Min(selected[0], _documents.Count - 1));
            SetStatus(Loc.T(Txt.ConvertReady));
        }

        private void ClearAll()
        {
            if (_running || _documents.Count == 0) return;
            if (ThemedMessageBox.Show(this, Loc.T(Txt.MsgClearText), Loc.T(Txt.MsgClearTitle),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            _documents.Clear();
            _passwords.Clear();
            _manualFolder = false;
            RefreshList();
            SetStatus(Loc.T(Txt.ConvertReady));
        }

        private void RefreshList(int selectIndex = -1)
        {
            int previous = _list.SelectedIndex;

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (DocumentItem item in _documents)
                _list.Items.Add(item);
            _list.EndUpdate();

            _countLabel.Text = Loc.T(Txt.ConvertCountFormat, _documents.Count);
            _emptyPanel.Visible = _documents.Count == 0;
            _emptyPanel.BringToFront();

            if (_documents.Count > 0)
            {
                int index = selectIndex >= 0
                    ? Math.Min(selectIndex, _documents.Count - 1)
                    : Math.Clamp(previous, 0, _documents.Count - 1);
                _list.SelectedIndex = index;
            }

            if (!_manualFolder) SetFolderSilently(ResolveOutputFolder());
            UpdateButtons();
            UpdateOutputInfo();
            SchedulePreview();
        }

        private void ConfigureToolTips()
        {
            _toolTip.SetToolTip(_removeButton, Loc.T(Txt.TipRemove));
            _toolTip.SetToolTip(_clearButton, Loc.T(Txt.TipClear));
            _toolTip.SetToolTip(_cancelBoxButton, Loc.T(Txt.TipCancelBox));
            _toolTip.SetToolTip(_list, Loc.T(Txt.TipList));
            _toolTip.SetToolTip(_browseButton, Loc.T(Txt.TipBrowse));
        }

        private void UpdateButtons()
        {
            bool hasDocuments = _documents.Count > 0;
            _addButton.Enabled = !_running;
            _removeButton.Enabled = !_running && _list.SelectedIndices.Count > 0;
            _clearButton.Enabled = !_running && hasDocuments;
            _cancelBoxButton.Enabled = !_running && _list.HasMarqueeSelection;
            _browseButton.Enabled = !_running;
            _folderInput.Enabled = !_running;
            _rangeControl.Enabled = !_running;
            _rangeInput.Enabled = !_running && IsCustomPageMode;
            _ppiCustom.Enabled = !_running;
            _formatControl.Enabled = !_running;
            _ppiControl.Enabled = !_running;
            _qualitySlider.Enabled = !_running;
            _transparentBox.Enabled = !_running && CurrentFormat == ConvertImageFormat.Png;
            _subfolderBox.Enabled = !_running;
            _list.Enabled = !_running;
            _startButton.Enabled = _running || hasDocuments;
            _openFolderButton.Enabled = !_running && _openFolderButton.Visible;
        }

        // ---------------- 保存位置 ----------------

        private string ResolveOutputFolder()
        {
            if (_settings.OutputLocationMode == OutputLocationMode.Custom &&
                _settings.CustomOutputFolder.Trim().Length > 0)
                return _settings.CustomOutputFolder.Trim();

            if (_settings.LastConvertFolder.Trim().Length > 0)
                return _settings.LastConvertFolder.Trim();

            return _documents.Count > 0
                ? Path.GetDirectoryName(_documents[0].Path) ?? string.Empty
                : string.Empty;
        }

        private void BrowseFolder()
        {
            if (_running) return;
            using var dialog = new FolderBrowserDialog
            {
                Description = Loc.T(Txt.FolderDesc),
                UseDescriptionForTitle = true,
                SelectedPath = _folderInput.Text,
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            _manualFolder = true;
            SetFolderSilently(dialog.SelectedPath);
            _settings.LastConvertFolder = dialog.SelectedPath;
            _settings.Save();
            SetStatus(Loc.T(Txt.StatusFolderChosen));
        }

        private void SetFolderSilently(string text)
        {
            _suppressEvents = true;
            _folderInput.Text = text;
            _suppressEvents = false;
        }

        // ---------------- 预览与输出信息 ----------------

        private DocumentItem? SelectedDocument =>
            _list.SelectedIndex >= 0 && _list.SelectedIndex < _documents.Count
                ? _documents[_list.SelectedIndex]
                : null;

        /// <summary>当前模式下第一张会导出的页；预览和命名示例都用它。</summary>
        private int GetPreviewPageNumber()
        {
            int pageCount = SelectedDocument?.PageCount ?? 0;
            switch (CurrentPageMode)
            {
                case ConvertPageSelection.Even:
                    return pageCount <= 0 || pageCount >= 2 ? 2 : 1;
                case ConvertPageSelection.Custom:
                    if (PdfImageEngine.TryParsePageRange(_rangeInput.Text,
                            pageCount > 0 ? pageCount : 0,
                            out List<int> pages, out _) && pages.Count > 0)
                        return pages[0];
                    return 1;
                default:
                    return 1;
            }
        }

        private void SchedulePreview()
        {
            _previewTimer.Stop();
            _previewTimer.Start();
            UpdateOutputInfo();
        }

        private void RefreshPreview()
        {
            DocumentItem? item = SelectedDocument;
            int generation = ++_previewGeneration;

            int pageNumber = GetPreviewPageNumber();
            if (item == null)
            {
                SetPreview(null, 0, 0, null, pageNumber);
                return;
            }

            string path = item.Path;
            _passwords.TryGetValue(path, out string? password);
            bool transparent = CurrentFormat == ConvertImageFormat.Png && _transparentBox.Checked;
            int ppi = CurrentPpi;
            int pageIndex = pageNumber - 1;

            Task.Run(() =>
            {
                Bitmap? bitmap = PdfImageEngine.RenderPreview(path, pageIndex, 900, password, transparent);
                uint width = 0;
                uint height = 0;
                if (PdfImageEngine.TryGetPageSize(path, pageIndex, password, out double dipWidth, out double dipHeight))
                    PdfImageEngine.GetTargetSize(dipWidth, dipHeight, ppi, out width, out height);
                return (bitmap, width, height);
            }).ContinueWith(task =>
            {
                if (!IsHandleCreated || IsDisposed) return;
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            if (generation == _previewGeneration)
                                SetPreview(null, 0, 0, item.FileName, pageNumber);
                            return;
                        }

                        var output = task.Result;
                        if (IsDisposed || generation != _previewGeneration)
                        {
                            output.bitmap?.Dispose();
                            return;
                        }
                        SetPreview(output.bitmap, output.width, output.height, item.FileName, pageNumber);
                    }));
                }
                catch
                {
                    // 窗口已关闭
                }
            });
        }

        private void SetPreview(Bitmap? bitmap, uint width, uint height, string? failureFile,
            int pageNumber)
        {
            _previewImage?.Dispose();
            _previewImage = bitmap;
            _preview.Image = bitmap;
            _preview.Checkerboard = CurrentFormat == ConvertImageFormat.Png && _transparentBox.Checked;
            _preview.Invalidate();

            if (bitmap == null)
            {
                _previewInfo.Text = failureFile == null
                    ? string.Empty
                    : Loc.T(Txt.ConvertUnreadable, failureFile);
            }
            else
            {
                string title = Loc.T(Txt.ConvertPreviewTitle, pageNumber);
                _previewInfo.Text = width > 0 && height > 0
                    ? title + "  ·  " + Loc.T(Txt.ConvertEstimateFormat, width, height)
                    : title;
            }
        }

        private void UpdateOutputInfo()
        {
            _namePreview.Text = Loc.T(Txt.ConvertNamePreview, BuildPreviewName());
            _namePreview.ForeColor = Theme.TextMuted;
            UpdateButtons();
        }

        private string BuildPreviewName()
        {
            string? first = _documents.Count > 0 ? _documents[0].Path : null;
            string stem = _settings.BuildNameStem(first, DateTime.Now, forConversion: true);
            foreach (char invalid in Path.GetInvalidFileNameChars())
                stem = stem.Replace(invalid, '_');
            stem = stem.Trim();
            if (stem.Length == 0) stem = Loc.T(Txt.DefaultConvertName);

            int pageNumber = GetPreviewPageNumber();
            int pageDigits = Math.Max(3, (SelectedDocument?.PageCount ?? 0).ToString().Length);
            string extension = PdfImageEngine.Extension(CurrentFormat);
            return $"{stem}_p{pageNumber.ToString("D" + pageDigits)}.{extension}";
        }

        // ---------------- 转换 ----------------

        private void StartOrCancel()
        {
            if (_running)
            {
                _cts?.Cancel();
                _startButton.Enabled = false;
                _startButton.Text = Loc.T(Txt.ConvertCancelling);
                return;
            }

            StartConversion();
        }

        private void StartConversion()
        {
            if (_documents.Count == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ConvertNoFiles), Loc.T(Txt.ConvertFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = _folderInput.Text.Trim();
            if (folder.Length == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ConvertNeedFolder), Loc.T(Txt.ConvertFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConvertPageSelection pageMode = CurrentPageMode;
            string range = _rangeInput.Text.Trim();
            if (pageMode == ConvertPageSelection.Custom)
            {
                if (range.Length == 0)
                {
                    ThemedMessageBox.Show(this, Loc.T(Txt.ConvertRangeNeeded), Loc.T(Txt.ConvertFailTitle),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!PdfImageEngine.TryParsePageRange(range, int.MaxValue, out _, out _))
                {
                    ThemedMessageBox.Show(this, Loc.T(Txt.ConvertBadRange), Loc.T(Txt.ConvertFailTitle),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int ppi = CurrentPpi;
            if (ppi < PdfImageEngine.MinPpi || ppi > PdfImageEngine.MaxPpi)
            {
                ThemedMessageBox.Show(this, $"PPI {PdfImageEngine.MinPpi}–{PdfImageEngine.MaxPpi}",
                    Loc.T(Txt.ConvertFailTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var files = _documents.Select(d => d.Path).ToArray();
            if (!TryCollectPasswords(files)) return;

            var options = new PdfConvertOptions
            {
                Format = CurrentFormat,
                Ppi = ppi,
                JpegQuality = _qualitySlider.Value,
                TransparentBackground = _transparentBox.Checked,
                PageSelection = pageMode,
                PageRange = range,
                CreateSubfolder = _subfolderBox.Checked || _documents.Count > 1,
                Naming = _settings,
                Watermark = _settings.WatermarkForPdfImage
                    ? _settings.CreateWatermarkSpec()
                    : null
            };

            _settings.ConvertFormat = options.Format;
            _settings.ConvertPpi = options.Ppi;
            _settings.ConvertJpegQuality = options.JpegQuality;
            _settings.ConvertTransparentBackground = options.TransparentBackground;
            _settings.ConvertCreateSubfolder = _subfolderBox.Checked;
            _settings.ConvertPageMode = pageMode;
            _settings.ConvertPageRange = range;
            _settings.LastConvertFolder = folder;
            _settings.Save();

            SetRunning(true);
            _progress.Value = 0;
            _openFolderButton.Visible = false;
            _startButton.Text = Loc.T(Txt.ConvertCancel);
            _startButton.Icon = IconKind.Close;
            _startButton.ApplyTheme();
            SetStatus(Loc.T(Txt.ConvertProcessing, 0, files.Length, Path.GetFileName(files[0])));

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            var progress = new Progress<PdfConvertProgress>(p =>
            {
                if (!_running) return;
                _progress.Value = p.Percent;
                string file = p.FileIndex >= 0 && p.FileIndex < files.Length
                    ? Path.GetFileName(files[p.FileIndex])
                    : string.Empty;
                string page = p.PageCount > 0
                    ? "  ·  " + Loc.T(Txt.ConvertPageStatus, p.Page, p.PageCount)
                    : string.Empty;
                SetStatus(Loc.T(Txt.ConvertProcessing, p.FileIndex + 1, files.Length, file) + page);
            });

            var thread = new Thread(() =>
            {
                try
                {
                    PdfConvertResult result = PdfImageEngine.Run(files, folder, options, progress, token, _passwords);
                    BeginInvoke(new Action(() => FinishConversion(result, folder)));
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

        private bool TryCollectPasswords(string[] files)
        {
            foreach (string file in files)
            {
                if (!MergeEngine.PdfNeedsPassword(file)) continue;

                string? entered = null;
                while (true)
                {
                    using var dialog = new PasswordPromptForm(
                        Path.GetFileName(file),
                        entered == null ? null : Loc.T(Txt.MergePasswordWrong));

                    if (dialog.ShowDialog(this) != DialogResult.OK) return false;
                    entered = dialog.Password;

                    if (MergeEngine.VerifyPdfPassword(file, entered)) break;
                }

                _passwords[file] = entered!;
            }

            return true;
        }

        private void SetRunning(bool running)
        {
            _running = running;
            UseWaitCursor = running;
            UpdateButtons();
            BusyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FinishConversion(PdfConvertResult result, string folder)
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.ConvertStart);
            _startButton.Icon = IconKind.Convert;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0;

            string elapsed = FormatElapsed(result.Elapsed);
            if (result.Cancelled)
            {
                _progress.Value = Math.Max(0, Math.Min(100, result.ImageCount));
                SetStatus(Loc.T(Txt.ConvertCancelled, result.ImageCount));
            }
            else if (result.FailedCount > 0)
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.ConvertDonePartial, result.SucceededCount, result.FailedCount, result.ImageCount));
                WriteFailureLog(result);
                if (result.SucceededCount == 0)
                {
                    string message = result.Files.FirstOrDefault(f => f.Error != null)?.Error
                        ?? Loc.T(Txt.ConvertFailStatus);
                    ThemedMessageBox.Show(this, message, Loc.T(Txt.ConvertFailTitle),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.ConvertDone, result.ImageCount, elapsed));
            }

            string lastFolder = result.Files.LastOrDefault(f => f.OutputFolder.Length > 0)?.OutputFolder
                ?? folder;
            _lastOutputFolder = lastFolder;
            _openFolderButton.Visible = Directory.Exists(lastFolder);
            _openFolderButton.Enabled = true;
            _settings.LastConvertFolder = folder;
            _settings.Save();

            if (result.ImageCount > 0 && !result.Cancelled)
            {
                ShowCompletion(result, lastFolder, elapsed);
                if (_settings.OpenAfterMerge) OpenFolder(lastFolder);
            }
        }

        private void ShowCompletion(PdfConvertResult result, string folder, string elapsed)
        {
            if (_settings.PlaySound)
                SoundLibrary.Play(_settings.SoundId);

            if (!_settings.ShowNotification || IsDisposed) return;

            string subtitle = result.FailedCount > 0
                ? Loc.T(Txt.ConvertDonePartial, result.SucceededCount, result.FailedCount, result.ImageCount)
                : Loc.T(Txt.ConvertDone, result.ImageCount, elapsed);

            var toast = new ToastForm(Loc.T(Txt.ConvertPageTitle), subtitle,
                Path.GetFileName(folder), Screen.FromControl(this).WorkingArea,
                () => OpenFolder(folder));
            toast.ShowToast();
        }

        private void FinishFailure(Exception exception)
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.ConvertStart);
            _startButton.Icon = IconKind.Convert;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0;
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.ConvertFailStatus));
            WriteFailureLog(exception);

            string message = exception is PdfConvertException
                ? exception.Message
                : Loc.T(Txt.FailPrefix, exception.Message);
            ThemedMessageBox.Show(this, message, Loc.T(Txt.ConvertFailTitle),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OpenFolder(string folder)
        {
            if (!Directory.Exists(folder)) return;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                });
            }
            catch
            {
                // 打开资源管理器失败时忽略
            }
        }

        private void SetStatus(string text)
        {
            _statusLabel.Text = text;
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            if (elapsed.TotalSeconds < 60)
                return Loc.T(Txt.ElapsedSeconds, elapsed.TotalSeconds);
            return Loc.T(Txt.ElapsedMinutes, (int)elapsed.TotalMinutes, elapsed.Seconds);
        }

        private static void WriteFailureLog(PdfConvertResult result)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);
                var failures = result.Files.Where(f => f.Error != null)
                    .Select(f => $"{f.Source}{Environment.NewLine}  {f.Error}");
                File.WriteAllText(Path.Combine(folder, "last-convert-error.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                    $"version: {Application.ProductVersion}{Environment.NewLine}" +
                    string.Join(Environment.NewLine, failures) + Environment.NewLine);
            }
            catch
            {
                // 日志失败不影响结果
            }
        }

        private static void WriteFailureLog(Exception exception)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "last-convert-error.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                    $"version: {Application.ProductVersion}{Environment.NewLine}" +
                    exception + Environment.NewLine);
            }
            catch
            {
                // 日志失败不影响结果
            }
        }

        // ---------------- 拖放 ----------------

        private void EnableFileDrop(Control control)
        {
            if (control is TextBox)
            {
                control.AllowDrop = false;
                return;
            }

            control.AllowDrop = true;
            control.DragEnter += OnDragEnter;
            control.DragOver += OnDragOver;
            control.DragDrop += OnDrop;

            foreach (Control child in control.Controls)
                EnableFileDrop(child);
        }

        private static bool HasFiles(IDataObject? data) =>
            data != null && data.GetDataPresent(DataFormats.FileDrop);

        private void OnDragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = !_running && HasFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.Effect = !_running && HasFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (_running || !HasFiles(e.Data)) return;
            var paths = (string[]?)e.Data!.GetData(DataFormats.FileDrop);
            if (paths != null) AddFiles(paths);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _previewImage?.Dispose();
                _previewImage = null;
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                _previewTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
