using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>图片转 PDF 页：图片队列、纸张设置、预览、进度与取消。</summary>
    public sealed class ImagePdfWorkspace : UserControl
    {
        private const int SideWidth = 306;
        private const int PreviewHeight = 112;
        private const string ReorderFormat = "DocHub.ImagePdfReorder";

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
        private readonly Label _pageSizeLabel;
        private readonly SegmentedControl _pageSizeControl;
        private readonly Label _orientationLabel;
        private readonly SegmentedControl _orientationControl;
        private readonly Label _marginLabel;
        private readonly SegmentedControl _marginControl;
        private readonly Label _fitHint;
        private readonly SkinCheckBox _mergeBox;
        private readonly Label _nameLabel;
        private readonly RoundedInput _nameInput;
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

        private CancellationTokenSource? _cts;
        private Bitmap? _previewImage;
        private int _previewGeneration;
        private bool _running;
        private bool _suppressEvents;
        private bool _manualFolder;
        private int _dragSourceIndex = -1;
        private Point _dragStart;
        private bool _dragArmed;
        private string _lastOutputPath = string.Empty;
        private string _lastOutputFolder = string.Empty;

        public event EventHandler? BusyChanged;

        public bool IsBusy => _running;

        public ImagePdfWorkspace(AppSettings settings)
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
                ShowMoveActions = true,
                ShowDragHandle = true,
                ShowPageCount = false,
                SelectionMode = SelectionMode.MultiExtended,
                AllowDrop = true
            };

            _emptyPanel = new EmptyStatePanel
            {
                TitleOverride = Loc.T(Txt.ImagePdfEmptyTitle),
                SubtitleOverride = Loc.T(Txt.ImagePdfEmptySubtitle),
                FormatsOverride = Loc.T(Txt.ImagePdfEmptyFormats),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _preview = new ImagePreviewBox { EmptyText = Loc.T(Txt.ImagePdfPreviewEmpty) };
            _previewInfo = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);
            _namePreview = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);

            _settingsTitle = MakeLabel(string.Empty, Theme.SectionFont, "section", 16, 18);
            _pageSizeLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _pageSizeControl = new SegmentedControl();
            _orientationLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _orientationControl = new SegmentedControl();
            _marginLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _marginControl = new SegmentedControl();
            _fitHint = MakeLabel(string.Empty, Theme.SmallFont, "muted", 16, 0);
            _mergeBox = new SkinCheckBox();
            _nameLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _nameInput = new RoundedInput { MaxLength = 80 };
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
                Icon = IconKind.Image,
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
                _settingsTitle, _pageSizeLabel, _pageSizeControl,
                _orientationLabel, _orientationControl, _marginLabel, _marginControl,
                _fitHint, _mergeBox, _nameLabel, _nameInput, _saveLabel,
                _folderInput, _browseButton, _divider,
                _statusLabel, _progress, _openFolderButton, _startButton
            });
            Controls.Add(_listCard);
            Controls.Add(_sideCard);

            _list.ActionUp += (_, _) => MoveSelected(-1);
            _list.ActionDown += (_, _) => MoveSelected(1);
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
            _list.MouseDown += OnListMouseDown;
            _list.MouseMove += OnListMouseMove;
            _list.MouseUp += (_, _) => _dragArmed = false;
            _list.DragEnter += OnListDragOver;
            _list.DragOver += OnListDragOver;
            _list.DragDrop += OnListDragDrop;

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

            _pageSizeControl.SelectionChanged += (_, _) => OnSettingsChanged();
            _orientationControl.SelectionChanged += (_, _) => OnSettingsChanged();
            _marginControl.SelectionChanged += (_, _) => OnSettingsChanged();
            _mergeBox.CheckedChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _settings.ImagePdfMergeIntoOne = _mergeBox.Checked;
                _settings.Save();
                UpdateNamePlaceholder();
                UpdateReadyStatus();
                UpdateOutputInfo();
            };
            _nameInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _settings.ImagePdfCustomName = _nameInput.Text.Trim();
                UpdateOutputInfo();
            };
            _nameInput.Box.Leave += (_, _) =>
            {
                if (_running) return;
                _settings.ImagePdfCustomName = _nameInput.Text.Trim();
                _settings.Save();
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
            _pageSizeControl.ApplyTheme();
            _orientationControl.ApplyTheme();
            _marginControl.ApplyTheme();
            Invalidate(true);
        }

        public void ApplyLanguage()
        {
            Font = Theme.BaseFont;
            _sectionLabel.Text = Loc.T(Txt.ImagePdfFilesTitle);
            _sectionLabel.Font = Theme.SectionFont;
            _countLabel.Font = Theme.SmallFont;
            _hintLabel.Text = Loc.T(Txt.ImagePdfListHint);
            _hintLabel.Font = Theme.SmallFont;

            _addButton.Text = Loc.T(Txt.ImagePdfAdd);
            _addButton.Font = Theme.ButtonFont;
            _addButton.ApplyTheme();
            _browseButton.Text = Loc.T(Txt.Browse);
            _browseButton.Font = Theme.ButtonFont;
            _browseButton.ApplyTheme();

            _emptyPanel.TitleOverride = Loc.T(Txt.ImagePdfEmptyTitle);
            _emptyPanel.SubtitleOverride = Loc.T(Txt.ImagePdfEmptySubtitle);
            _emptyPanel.FormatsOverride = Loc.T(Txt.ImagePdfEmptyFormats);
            _preview.EmptyText = Loc.T(Txt.ImagePdfPreviewEmpty);
            _preview.Invalidate();

            _settingsTitle.Text = Loc.T(Txt.ImagePdfSettingsTitle);
            _settingsTitle.Font = Theme.SectionFont;
            _pageSizeLabel.Text = Loc.T(Txt.ImagePdfPageSize);
            _orientationLabel.Text = Loc.T(Txt.ImagePdfOrientation);
            _marginLabel.Text = Loc.T(Txt.ImagePdfMargin);
            _fitHint.Text = Loc.T(Txt.ImagePdfFitHint);
            _mergeBox.Text = Loc.T(Txt.ImagePdfMergeOption);
            _nameLabel.Text = Loc.T(Txt.ImagePdfNameLabel);
            UpdateNamePlaceholder();
            _saveLabel.Text = Loc.T(Txt.ConvertOutput);
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _openFolderButton.Text = Loc.T(Txt.ConvertOpenFolder);
            _openFolderButton.Font = Theme.ButtonFont;
            _openFolderButton.ApplyTheme();

            foreach (Label label in new[] { _pageSizeLabel, _orientationLabel, _marginLabel, _nameLabel, _saveLabel })
                label.Font = Theme.BaseFont;
            _fitHint.Font = Theme.SmallFont;
            _mergeBox.Font = Theme.BaseFont;
            _statusLabel.Font = Theme.BaseFont;
            _previewInfo.Font = Theme.SmallFont;
            _namePreview.Font = Theme.SmallFont;

            _pageSizeControl.Segments = new[]
            {
                Loc.T(Txt.ImagePdfPageSizeAuto), "A4", "A3", "A5", "B5", "Letter"
            };
            _orientationControl.Segments = new[]
            {
                Loc.T(Txt.ImagePdfOrientationAuto),
                Loc.T(Txt.ImagePdfOrientationPortrait),
                Loc.T(Txt.ImagePdfOrientationLandscape)
            };
            _marginControl.Segments = new[]
            {
                Loc.T(Txt.ImagePdfMarginNone), "10 mm", "20 mm"
            };

            _countLabel.Text = Loc.T(Txt.ImagePdfCountFormat, _documents.Count);
            if (!_running) UpdateReadyStatus();
            _startButton.Text = _running ? Loc.T(Txt.ImagePdfRunning) : Loc.T(Txt.ImagePdfStart);
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
            RefreshOptionControls();
            UpdateOutputInfo();
            UpdateButtons();
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

            int sectionWidth = TextRenderer.MeasureText(_sectionLabel.Text, _sectionLabel.Font).Width + 2;
            _sectionLabel.Bounds = new Rectangle(20, 12, Math.Max(80, sectionWidth), 26);
            int countLeft = _sectionLabel.Right + 10;
            _countLabel.Bounds = new Rectangle(countLeft, 18,
                Math.Max(0, _removeButton.Left - 10 - countLeft), 20);

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
            y += 34;

            _pageSizeLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 22;
            _pageSizeControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 38;

            _orientationLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 22;
            _orientationControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 38;

            _marginLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 22;
            _marginControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 34;

            _fitHint.Bounds = new Rectangle(16, y, inner, 38);
            y += 46;

            _mergeBox.Bounds = new Rectangle(16, y, inner, 24);
            y += 28;
            int nameLabelWidth = Math.Min(96,
                TextRenderer.MeasureText(_nameLabel.Text, _nameLabel.Font).Width + 8);
            _nameLabel.Bounds = new Rectangle(16, y, nameLabelWidth, 36);
            _nameInput.Bounds = new Rectangle(16 + nameLabelWidth, y,
                Math.Max(60, inner - nameLabelWidth), 36);
            y += 44;

            _saveLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 22;
            int browseWidth = FitWidth(_browseButton.Text, _browseButton.Font, 28, 58);
            _browseButton.Bounds = new Rectangle(16 + inner - browseWidth, y, browseWidth, 36);
            _folderInput.Bounds = new Rectangle(16, y, Math.Max(60, inner - browseWidth - 8), 36);
            y += 44;

            _divider.Bounds = new Rectangle(16, y, inner, 1);
            y += 10;

            _statusLabel.Bounds = new Rectangle(16, y, inner, 36);
            y += 42;
            _progress.Bounds = new Rectangle(16, y, inner, 22);

            const int startHeight = 48;
            _startButton.Bounds = new Rectangle(16, height - 18 - startHeight, inner, startHeight);
            _openFolderButton.Bounds = new Rectangle(16, _startButton.Top - 38, inner, 30);
        }

        private static int FitWidth(string text, Font font, int padding, int minimum)
        {
            int width = TextRenderer.MeasureText(text, font).Width + padding;
            return Math.Max(minimum, width);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible) LayoutCards();
        }

        // ---------------- 设置与队列 ----------------

        private ImagePdfPageSize CurrentPageSize =>
            (ImagePdfPageSize)Math.Clamp(_pageSizeControl.SelectedIndex, 0, 5);

        private ImagePdfOrientation CurrentOrientation =>
            (ImagePdfOrientation)Math.Clamp(_orientationControl.SelectedIndex, 0, 2);

        private int CurrentMarginMm => _marginControl.SelectedIndex switch
        {
            1 => 10,
            2 => 20,
            _ => 0
        };

        private void RefreshOptionControls()
        {
            _pageSizeControl.SelectedIndex = (int)_settings.ImagePdfPageSize;
            _orientationControl.SelectedIndex = (int)_settings.ImagePdfOrientation;
            _marginControl.SelectedIndex = _settings.ImagePdfMarginMm switch
            {
                >= 15 => 2,
                >= 5 => 1,
                _ => 0
            };

            _suppressEvents = true;
            _mergeBox.Checked = _settings.ImagePdfMergeIntoOne;
            _nameInput.Text = _settings.ImagePdfCustomName;
            _suppressEvents = false;
            UpdateNamePlaceholder();
        }

        private void UpdateNamePlaceholder()
        {
            _nameInput.Box.PlaceholderText = Loc.T(_settings.ImagePdfMergeIntoOne
                ? Txt.ImagePdfNamePlaceholderMerge
                : Txt.ImagePdfNamePlaceholderSeparate);
        }

        private void UpdateReadyStatus()
        {
            if (_running) return;
            SetStatus(Loc.T(_settings.ImagePdfMergeIntoOne
                ? Txt.ImagePdfReadyMerge
                : Txt.ImagePdfReady));
        }

        private void OnSettingsChanged()
        {
            _settings.ImagePdfPageSize = CurrentPageSize;
            _settings.ImagePdfOrientation = CurrentOrientation;
            _settings.ImagePdfMarginMm = CurrentMarginMm;
            _settings.Save();
            UpdateOutputInfo();
        }

        private void ShowAddDialog()
        {
            if (_running) return;
            using var dialog = new OpenFileDialog
            {
                Title = Loc.T(Txt.ImagePdfDialogTitle),
                Filter = $"{Loc.T(Txt.FilterImage)}|{BuildImageFilter()}|{Loc.T(Txt.FilterAll)}|*.*",
                Multiselect = true,
                CheckFileExists = true,
                RestoreDirectory = true
            };
            if (_documents.Count > 0)
                dialog.InitialDirectory = Path.GetDirectoryName(_documents[^1].Path);

            if (dialog.ShowDialog(this) == DialogResult.OK)
                AddFiles(dialog.FileNames);
        }

        private static string BuildImageFilter()
        {
            return string.Join(";", MergeEngine.ImageExtensions.Select(extension => "*" + extension));
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
                            if (!MergeEngine.IsImageFile(file)) continue;
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
                if (!MergeEngine.IsImageFile(path)) { skipped++; continue; }
                if (TryAddFile(path, existing)) added++;
                else skipped++;
            }

            if (added > 0)
            {
                RefreshList(_documents.Count - 1);
                string message = Loc.T(Txt.ImagePdfAdded, added);
                int nonImage = paths.Count(p => File.Exists(p) && !MergeEngine.IsImageFile(p));
                if (nonImage > 0) message += "  ·  " + Loc.T(Txt.ImagePdfSkippedNonImage, nonImage);
                SetStatus(message);
            }
            else
            {
                UpdateButtons();
                SetStatus(skipped > 0 ? Loc.T(Txt.ImagePdfNothingAdded) : Loc.T(Txt.ImagePdfReady));
            }
        }

        private bool TryAddFile(string path, HashSet<string> existing)
        {
            try
            {
                if (!File.Exists(path) || !MergeEngine.IsImageFile(path)) return false;
                string full = Path.GetFullPath(path);
                if (!existing.Add(full)) return false;

                _documents.Add(new DocumentItem(full));
                return true;
            }
            catch
            {
                return false;
            }
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
            SetStatus(Loc.T(Txt.ImagePdfReady));
        }

        private void ClearAll()
        {
            if (_running || _documents.Count == 0) return;
            if (ThemedMessageBox.Show(this, Loc.T(Txt.MsgClearText), Loc.T(Txt.MsgClearTitle),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            _documents.Clear();
            _manualFolder = false;
            RefreshList();
            SetStatus(Loc.T(Txt.ImagePdfReady));
        }

        private void MoveSelected(int delta)
        {
            if (_running) return;
            int index = _list.SelectedIndex;
            int target = index + delta;
            if (index < 0 || target < 0 || target >= _documents.Count) return;

            DocumentItem item = _documents[index];
            _documents.RemoveAt(index);
            _documents.Insert(target, item);
            RefreshList(target);
        }

        private void MoveItem(int from, int to)
        {
            if (_running || from == to) return;
            if (from < 0 || from >= _documents.Count || to < 0 || to >= _documents.Count) return;

            DocumentItem item = _documents[from];
            _documents.RemoveAt(from);
            _documents.Insert(to, item);
            RefreshList(to);
        }

        private void RefreshList(int selectIndex = -1)
        {
            int previous = _list.SelectedIndex;

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (DocumentItem item in _documents)
                _list.Items.Add(item);
            _list.EndUpdate();

            _countLabel.Text = Loc.T(Txt.ImagePdfCountFormat, _documents.Count);
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
            _mergeBox.Enabled = !_running;
            _nameInput.Enabled = !_running;
            _pageSizeControl.Enabled = !_running;
            _orientationControl.Enabled = !_running && CurrentPageSize != ImagePdfPageSize.Auto;
            _marginControl.Enabled = !_running && CurrentPageSize != ImagePdfPageSize.Auto;
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

            if (_settings.LastOutputFolder.Trim().Length > 0)
                return _settings.LastOutputFolder.Trim();

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
            _settings.LastOutputFolder = dialog.SelectedPath;
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

            if (item == null)
            {
                SetPreview(null, 0, 0, null);
                return;
            }

            string path = item.Path;
            Task.Run(() =>
            {
                Bitmap? bitmap = ImagePdfEngine.LoadPreview(path, out int width, out int height, out string error);
                return (bitmap, width, height, error);
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
                                SetPreview(null, 0, 0, item.FileName);
                            return;
                        }

                        var output = task.Result;
                        if (IsDisposed || generation != _previewGeneration)
                        {
                            output.bitmap?.Dispose();
                            return;
                        }

                        if (output.bitmap == null)
                            SetPreview(null, 0, 0, item.FileName);
                        else
                            SetPreview(output.bitmap, output.width, output.height, null);
                    }));
                }
                catch
                {
                    // 窗口已关闭
                }
            });
        }

        private void SetPreview(Bitmap? bitmap, int width, int height, string? failureFile)
        {
            _previewImage?.Dispose();
            _previewImage = bitmap;
            _preview.Image = bitmap;
            _preview.Invalidate();

            if (bitmap == null)
            {
                _previewInfo.Text = failureFile == null
                    ? string.Empty
                    : Loc.T(Txt.ImagePdfUnreadable, failureFile);
            }
            else
            {
                _previewInfo.Text = width > 0 && height > 0
                    ? $"{Loc.T(Txt.ImagePdfPreviewTitle)}  ·  {width} × {height} px"
                    : Loc.T(Txt.ImagePdfPreviewTitle);
            }
        }

        private void UpdateOutputInfo()
        {
            _namePreview.Text = Loc.T(Txt.ImagePdfNamePreview, BuildPreviewName());
            _namePreview.ForeColor = Theme.TextMuted;
            UpdateButtons();
        }

        private string BuildPreviewName()
        {
            bool merge = _settings.ImagePdfMergeIntoOne;
            string custom = SanitizeStem(_settings.ImagePdfCustomName);

            if (merge)
            {
                string mergedStem = custom.Length > 0 ? custom : BuildSharedStem();
                return mergedStem + ".pdf";
            }

            if (_documents.Count == 0)
                return (custom.Length > 0 ? custom : BuildSharedStem()) + ".pdf";

            if (custom.Length > 0)
            {
                return _documents.Count == 1
                    ? custom + ".pdf"
                    : Loc.T(Txt.ImagePdfNamePreviewMulti, custom + "_001.pdf", _documents.Count);
            }

            string first = SanitizeStem(Path.GetFileNameWithoutExtension(_documents[0].Path));
            if (first.Length == 0) first = Loc.T(Txt.DefaultDocName);
            return _documents.Count == 1
                ? first + ".pdf"
                : Loc.T(Txt.ImagePdfNamePreviewMulti, first + ".pdf", _documents.Count);
        }

        /// <summary>合并输出时沿用共享命名（首个文件 / 短语 / 时间 / 自定义名称）。</summary>
        private string BuildSharedStem()
        {
            string? first = _documents.Count > 0 ? _documents[0].Path : null;
            string stem = SanitizeStem(_settings.BuildNameStem(first, DateTime.Now));
            return stem.Length == 0 ? Loc.T(Txt.DefaultDocName) : stem;
        }

        private static string SanitizeStem(string? text)
        {
            string stem = text ?? string.Empty;
            foreach (char invalid in Path.GetInvalidFileNameChars())
                stem = stem.Replace(invalid, '_');
            return stem.Trim().TrimEnd('.');
        }

        // ---------------- 转换 ----------------

        private void StartOrCancel()
        {
            if (_running)
            {
                _cts?.Cancel();
                _startButton.Enabled = false;
                _startButton.Text = Loc.T(Txt.ImagePdfCancelling);
                return;
            }

            StartConversion();
        }

        private void StartConversion()
        {
            if (_documents.Count == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ImagePdfNoFiles), Loc.T(Txt.ImagePdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = _folderInput.Text.Trim();
            if (folder.Length == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ImagePdfNeedFolder), Loc.T(Txt.ImagePdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(folder))
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ErrFolderMissing, folder), Loc.T(Txt.ImagePdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] files = _documents.Select(d => d.Path).ToArray();
            bool merge = _settings.ImagePdfMergeIntoOne;
            string custom = SanitizeStem(_settings.ImagePdfCustomName);
            string mergedOutput = string.Empty;
            string[] separateOutputs = Array.Empty<string>();

            try
            {
                if (merge)
                {
                    string stem = custom.Length > 0
                        ? custom
                        : SanitizeStem(_settings.BuildNameStem(files[0], DateTime.Now));
                    if (stem.Length == 0) stem = Loc.T(Txt.DefaultDocName);
                    mergedOutput = MakeUniqueOutputPath(
                        Path.GetFullPath(Path.Combine(folder, stem + ".pdf")));
                }
                else
                {
                    var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    separateOutputs = new string[files.Length];
                    for (int i = 0; i < files.Length; i++)
                    {
                        string stem = custom.Length > 0
                            ? (files.Length == 1 ? custom : $"{custom}_{i + 1:000}")
                            : SanitizeStem(Path.GetFileNameWithoutExtension(files[i]));
                        if (stem.Length == 0) stem = Loc.T(Txt.DefaultDocName);

                        string path = MakeUniqueOutputPath(
                            Path.GetFullPath(Path.Combine(folder, stem + ".pdf")), reserved);
                        reserved.Add(path);
                        separateOutputs[i] = path;
                    }
                }
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ErrPathInvalid, ex), Loc.T(Txt.ImagePdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var options = new ImagePdfOptions
            {
                PageSize = CurrentPageSize,
                Orientation = CurrentOrientation,
                MarginMm = CurrentMarginMm,
                Watermark = _settings.WatermarkForImagePdf
                    ? _settings.CreateWatermarkSpec()
                    : null
            };

            _settings.ImagePdfPageSize = options.PageSize;
            _settings.ImagePdfOrientation = options.Orientation;
            _settings.ImagePdfMarginMm = options.MarginMm;
            _settings.ImagePdfMergeIntoOne = merge;
            _settings.LastOutputFolder = folder;
            _settings.Save();

            SetRunning(true);
            _progress.Value = 0;
            _openFolderButton.Visible = false;
            _startButton.Text = Loc.T(Txt.ImagePdfCancel);
            _startButton.Icon = IconKind.Close;
            _startButton.ApplyTheme();
            SetStatus(Loc.T(Txt.ImagePdfProcessing, 0, files.Length, Path.GetFileName(files[0])));

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            var progress = new Progress<ImagePdfProgress>(p =>
            {
                if (!_running) return;
                _progress.Value = p.Percent;
                string file = p.FileIndex >= 0 && p.FileIndex < files.Length
                    ? Path.GetFileName(files[p.FileIndex])
                    : string.Empty;
                SetStatus(Loc.T(Txt.ImagePdfProcessing, p.FileIndex + 1, files.Length, file));
            });

            var thread = new Thread(() =>
            {
                try
                {
                    if (merge)
                    {
                        ImagePdfResult result = ImagePdfEngine.Run(files, mergedOutput,
                            options, progress, token);
                        BeginInvoke(new Action(() => FinishConversion(result, folder)));
                    }
                    else
                    {
                        ImagePdfBatchResult result = ImagePdfEngine.RunSeparate(files,
                            separateOutputs, options, progress, token);
                        BeginInvoke(new Action(() => FinishSeparateConversion(result, folder)));
                    }
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

        private static string MakeUniqueOutputPath(string desired, ISet<string>? reserved = null)
        {
            if (!File.Exists(desired) && (reserved == null || !reserved.Contains(desired)))
                return desired;

            string folder = Path.GetDirectoryName(desired) ?? Environment.CurrentDirectory;
            string stem = Path.GetFileNameWithoutExtension(desired);
            string extension = Path.GetExtension(desired);

            for (int index = 1; index < 1000; index++)
            {
                string candidate = Path.Combine(folder, $"{stem}-{index}{extension}");
                if (!File.Exists(candidate) && (reserved == null || !reserved.Contains(candidate)))
                    return candidate;
            }

            return Path.Combine(folder, $"{stem}-{DateTime.Now:yyyyMMddHHmmss}{extension}");
        }

        private void SetRunning(bool running)
        {
            _running = running;
            UseWaitCursor = running;
            UpdateButtons();
            BusyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FinishConversion(ImagePdfResult result, string folder)
        {
            ResetRunState();

            string elapsed = FormatElapsed(result.Elapsed);
            if (result.Cancelled)
            {
                _progress.Value = 0;
                SetStatus(Loc.T(Txt.ImagePdfCancelled, result.PageCount));
            }
            else
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.ImagePdfDone, result.PageCount, elapsed));
            }

            _lastOutputPath = result.OutputPath;
            _lastOutputFolder = folder;
            _openFolderButton.Visible = !result.Cancelled && Directory.Exists(folder);
            _openFolderButton.Enabled = true;
            _settings.LastOutputFolder = folder;
            _settings.Save();

            if (!result.Cancelled)
            {
                ShowCompletion(Loc.T(Txt.ImagePdfDone, result.PageCount, elapsed),
                    Path.GetFileName(result.OutputPath),
                    () => OpenOutput(_lastOutputPath));
                if (_settings.OpenAfterMerge) OpenOutput(result.OutputPath);
            }
        }

        private void FinishSeparateConversion(ImagePdfBatchResult result, string folder)
        {
            ResetRunState();

            string elapsed = FormatElapsed(result.Elapsed);
            int completed = result.OutputPaths.Count;
            if (result.Cancelled)
            {
                _progress.Value = 0;
                SetStatus(Loc.T(Txt.ImagePdfCancelled, completed));
            }
            else if (result.FailedCount > 0)
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.ImagePdfDonePartial, completed, result.FailedCount, elapsed));
                WriteFailureLog(new ImagePdfException(
                    result.FirstError ?? Loc.T(Txt.ImagePdfFailStatus)));
            }
            else
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.ImagePdfDoneFiles, completed, elapsed));
            }

            _lastOutputPath = completed > 0 ? result.OutputPaths[completed - 1] : string.Empty;
            _lastOutputFolder = folder;
            _openFolderButton.Visible = completed > 0 && Directory.Exists(folder);
            _openFolderButton.Enabled = true;
            _settings.LastOutputFolder = folder;
            _settings.Save();

            if (!result.Cancelled && completed > 0)
            {
                string message = result.FailedCount > 0
                    ? Loc.T(Txt.ImagePdfDonePartial, completed, result.FailedCount, elapsed)
                    : Loc.T(Txt.ImagePdfDoneFiles, completed, elapsed);
                ShowCompletion(message, Loc.T(Txt.ImagePdfOutputCount, completed),
                    () => OpenFolder(folder));
                if (_settings.OpenAfterMerge) OpenFolder(folder);
            }
        }

        private void ResetRunState()
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.ImagePdfStart);
            _startButton.Icon = IconKind.Image;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0;
        }

        private void ShowCompletion(string message, string detail, Action onClick)
        {
            if (_settings.PlaySound)
                SoundLibrary.Play(_settings.SoundId);

            if (!_settings.ShowNotification || IsDisposed) return;

            var toast = new ToastForm(Loc.T(Txt.ImagePdfPageTitle), message, detail,
                Screen.FromControl(this).WorkingArea, onClick);
            toast.ShowToast();
        }

        private void FinishFailure(Exception exception)
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.ImagePdfStart);
            _startButton.Icon = IconKind.Image;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0;
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.ImagePdfFailStatus));
            WriteFailureLog(exception);

            string message = exception is ImagePdfException
                ? exception.Message
                : Loc.T(Txt.FailPrefix, exception.Message);
            ThemedMessageBox.Show(this, message, Loc.T(Txt.ImagePdfFailTitle),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OpenOutput(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch
            {
                // 打开默认程序失败时忽略
            }
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

        private static void WriteFailureLog(Exception exception)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "last-image-pdf-error.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                    $"version: {Application.ProductVersion}{Environment.NewLine}" +
                    exception + Environment.NewLine);
            }
            catch
            {
                // 日志失败不影响结果
            }
        }

        // ---------------- 拖放与排序 ----------------

        private void OnListMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _running) return;
            _dragStart = e.Location;
            _dragSourceIndex = _list.IndexFromPoint(e.Location);
            _dragArmed = _dragSourceIndex >= 0;
        }

        private void OnListMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_dragArmed || _running || e.Button != MouseButtons.Left) return;
            int dx = Math.Abs(e.X - _dragStart.X);
            int dy = Math.Abs(e.Y - _dragStart.Y);
            if (dx < 6 && dy < 6) return;

            _dragArmed = false;
            var data = new DataObject();
            data.SetData(ReorderFormat, _dragSourceIndex);
            _list.DoDragDrop(data, DragDropEffects.Move);
        }

        private void OnListDragOver(object? sender, DragEventArgs e)
        {
            if (_running)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            bool reorder = e.Data != null && e.Data.GetDataPresent(ReorderFormat);
            e.Effect = reorder
                ? DragDropEffects.Move
                : HasFiles(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnListDragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            if (e.Data.GetDataPresent(ReorderFormat))
            {
                int from = (int)(e.Data.GetData(ReorderFormat) ?? -1);
                Point point = _list.PointToClient(new Point(e.X, e.Y));
                int to = _list.IndexFromPoint(point);
                if (to < 0) to = _documents.Count - 1;
                MoveItem(from, to);
                return;
            }

            if (HasFiles(e.Data))
            {
                var paths = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (paths != null) AddFiles(paths);
            }
        }

        private void EnableFileDrop(Control control)
        {
            if (control == _list)
                return;

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
