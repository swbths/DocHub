using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DocHub
{
    /// <summary>Word 转 PDF 页：Word 队列、逐份/合并输出、进度与取消。</summary>
    public sealed class WordPdfWorkspace : UserControl
    {
        private const int SideWidth = 306;
        private const string ReorderFormat = "DocHub.WordPdfReorder";

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
        private readonly Label _outputPreview;

        private readonly Label _settingsTitle;
        private readonly SkinCheckBox _mergeBox;
        private readonly Label _mergeHint;
        private readonly Label _nameLabel;
        private readonly RoundedInput _nameInput;
        private readonly Label _separateHint;
        private readonly Label _outputLabel;
        private readonly RoundedInput _folderInput;
        private readonly RoundedButton _browseButton;
        private readonly Panel _divider;
        private readonly Label _statusLabel;
        private readonly SlimProgressBar _progress;
        private readonly RoundedButton _openFolderButton;
        private readonly RoundedButton _startButton;

        private readonly List<DocumentItem> _documents = new();

        private CancellationTokenSource? _cts;
        private bool _running;
        private bool _suppressEvents;
        private bool _manualFolder;
        private bool _nameEdited;
        private int _dragSourceIndex = -1;
        private Point _dragStart;
        private bool _dragArmed;
        private string _lastOutputFolder = string.Empty;

        public event EventHandler? BusyChanged;

        public bool IsBusy => _running;

        public WordPdfWorkspace(AppSettings settings)
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
                TitleOverride = Loc.T(Txt.WordPdfEmptyTitle),
                SubtitleOverride = Loc.T(Txt.WordPdfEmptySubtitle),
                FormatsOverride = Loc.T(Txt.WordPdfEmptyFormats),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _outputPreview = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);

            _settingsTitle = MakeLabel(string.Empty, Theme.SectionFont, "section", 16, 18);
            _mergeBox = new SkinCheckBox();
            _mergeHint = MakeLabel(string.Empty, Theme.SmallFont, "muted", 16, 0);
            _nameLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _nameInput = new RoundedInput { MaxLength = 80 };
            _separateHint = MakeLabel(string.Empty, Theme.SmallFont, "muted", 16, 0);
            _outputLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
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
                Icon = IconKind.WordPdf,
                IconSize = 16,
                IconGap = 8,
                Height = 48
            };
            _startButton.ApplyTheme();

            _listCard.Controls.AddRange(new Control[]
            {
                _sectionLabel, _countLabel, _hintLabel, _addButton, _removeButton,
                _clearButton, _cancelBoxButton, _list, _emptyPanel, _outputPreview
            });
            _sideCard.Controls.AddRange(new Control[]
            {
                _settingsTitle, _mergeBox, _mergeHint, _nameLabel, _nameInput,
                _separateHint, _outputLabel, _folderInput, _browseButton, _divider,
                _statusLabel, _progress, _openFolderButton, _startButton
            });
            Controls.Add(_listCard);
            Controls.Add(_sideCard);

            _list.ActionUp += (_, _) => MoveSelected(-1);
            _list.ActionDown += (_, _) => MoveSelected(1);
            _list.ActionRemove += (_, _) => RemoveSelected();
            _list.SelectedIndexChanged += (_, _) => UpdateButtons();
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

            _mergeBox.CheckedChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _settings.WordPdfMergeIntoOne = _mergeBox.Checked;
                _settings.Save();
                if (_mergeBox.Checked && !_nameEdited)
                    SetNameSilently(BuildDefaultMergedName());
                UpdateMergeMode();
                UpdateReadyStatus();
                UpdateOutputInfo();
            };

            _nameInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _nameEdited = _nameInput.Text.Trim().Length > 0;
                UpdateOutputInfo();
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
            _progress.ApplyTheme();
            Invalidate(true);
        }

        public void ApplyLanguage()
        {
            Font = Theme.BaseFont;
            _sectionLabel.Text = Loc.T(Txt.WordPdfFilesTitle);
            _sectionLabel.Font = Theme.SectionFont;
            _countLabel.Font = Theme.SmallFont;
            _hintLabel.Text = Loc.T(Txt.WordPdfListHint);
            _hintLabel.Font = Theme.SmallFont;

            _addButton.Text = Loc.T(Txt.WordPdfAdd);
            _addButton.Font = Theme.ButtonFont;
            _addButton.ApplyTheme();
            _browseButton.Text = Loc.T(Txt.Browse);
            _browseButton.Font = Theme.ButtonFont;
            _browseButton.ApplyTheme();

            _emptyPanel.TitleOverride = Loc.T(Txt.WordPdfEmptyTitle);
            _emptyPanel.SubtitleOverride = Loc.T(Txt.WordPdfEmptySubtitle);
            _emptyPanel.FormatsOverride = Loc.T(Txt.WordPdfEmptyFormats);
            _emptyPanel.Invalidate();

            _settingsTitle.Text = Loc.T(Txt.WordPdfSettingsTitle);
            _settingsTitle.Font = Theme.SectionFont;
            _mergeBox.Text = Loc.T(Txt.WordPdfMergeOption);
            _mergeBox.Font = Theme.BaseFont;
            _mergeHint.Text = Loc.T(Txt.WordPdfMergeHint);
            _mergeHint.Font = Theme.SmallFont;
            _nameLabel.Text = Loc.T(Txt.WordPdfNameLabel);
            _nameLabel.Font = Theme.BaseFont;
            _nameInput.Box.PlaceholderText = Loc.T(Txt.WordPdfNamePlaceholder);
            _separateHint.Text = Loc.T(Txt.WordPdfSeparateHint);
            _separateHint.Font = Theme.SmallFont;
            _outputLabel.Text = Loc.T(Txt.WordPdfOutput);
            _outputLabel.Font = Theme.BaseFont;
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _openFolderButton.Text = Loc.T(Txt.ConvertOpenFolder);
            _openFolderButton.Font = Theme.ButtonFont;
            _openFolderButton.ApplyTheme();
            _statusLabel.Font = Theme.BaseFont;
            _outputPreview.Font = Theme.SmallFont;

            _countLabel.Text = Loc.T(Txt.WordPdfCountFormat, _documents.Count);
            if (!_running) UpdateReadyStatus();
            _startButton.Text = _running ? Loc.T(Txt.WordPdfRunning) : Loc.T(Txt.WordPdfStart);
            _startButton.Font = Theme.PrimaryButtonFont;

            _list.ApplyTheme();
            UpdateMergeMode();
            UpdateOutputInfo();
            RefreshList(_list.SelectedIndex);
            ConfigureToolTips();
            LayoutCards();
        }

        public void ApplySettings()
        {
            _suppressEvents = true;
            _mergeBox.Checked = _settings.WordPdfMergeIntoOne;
            _suppressEvents = false;

            if (!_manualFolder) SetFolderSilently(ResolveOutputFolder());
            if (!_nameEdited) SetNameSilently(BuildDefaultMergedName());
            UpdateMergeMode();
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
                Math.Max(0, _removeButton.Left - 4 - countLeft), 20);

            _hintLabel.Bounds = new Rectangle(20, 50, Math.Max(60, width - 40), 16);

            int listTop = 72;
            int listHeight = Math.Max(80, height - listTop - 34);
            _list.Bounds = new Rectangle(10, listTop, Math.Max(60, width - 20), listHeight);
            _emptyPanel.Bounds = _list.Bounds;
            _outputPreview.Bounds = new Rectangle(12, _list.Bottom + 4,
                Math.Max(60, width - 24), 22);

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
            y += 36;

            _mergeBox.Bounds = new Rectangle(16, y, inner, 24);
            y += 28;
            _mergeHint.Bounds = new Rectangle(16, y, inner, 32);
            y += 38;

            // 合并模式显示文件名输入，逐份模式显示同名输出说明，二者占用同一行
            int nameLabelWidth = Math.Min(96,
                TextRenderer.MeasureText(_nameLabel.Text, _nameLabel.Font).Width + 8);
            _nameLabel.Bounds = new Rectangle(16, y, nameLabelWidth, 36);
            _nameInput.Bounds = new Rectangle(16 + nameLabelWidth, y,
                Math.Max(60, inner - nameLabelWidth), 36);
            _separateHint.Bounds = new Rectangle(16, y, inner, 36);
            y += 44;

            _outputLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 22;
            int browseWidth = FitWidth(_browseButton.Text, _browseButton.Font, 28, 58);
            _browseButton.Bounds = new Rectangle(16 + inner - browseWidth, y, browseWidth, 36);
            _folderInput.Bounds = new Rectangle(16, y, Math.Max(60, inner - browseWidth - 8), 36);
            y += 44;

            _divider.Bounds = new Rectangle(16, y, inner, 1);
            y += 12;

            _statusLabel.Bounds = new Rectangle(16, y, inner, 40);
            y += 46;
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

        // ---------------- 队列 ----------------

        private void ShowAddDialog()
        {
            if (_running) return;
            using var dialog = new OpenFileDialog
            {
                Title = Loc.T(Txt.WordPdfDialogTitle),
                Filter = $"{Loc.T(Txt.FilterWord)}|{BuildWordFilter()}|{Loc.T(Txt.FilterAll)}|*.*",
                Multiselect = true,
                CheckFileExists = true,
                RestoreDirectory = true
            };
            if (_documents.Count > 0)
                dialog.InitialDirectory = Path.GetDirectoryName(_documents[^1].Path);

            if (dialog.ShowDialog(this) == DialogResult.OK)
                AddFiles(dialog.FileNames);
        }

        private static string BuildWordFilter()
        {
            return string.Join(";", MergeEngine.WordExtensions.Select(extension => "*" + extension));
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
                            if (!MergeEngine.IsWordFile(file)) continue;
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
                if (!MergeEngine.IsWordFile(path)) { skipped++; continue; }
                if (TryAddFile(path, existing)) added++;
                else skipped++;
            }

            if (added > 0)
            {
                RefreshList(_documents.Count - 1);
                if (!HasUsableEngine())
                {
                    SetStatus(Loc.T(Txt.WordPdfNoEngine));
                    return;
                }

                string message = Loc.T(Txt.WordPdfAdded, added);
                int nonWord = paths.Count(p => File.Exists(p) && !MergeEngine.IsWordFile(p));
                if (nonWord > 0) message += "  ·  " + Loc.T(Txt.WordPdfSkippedNonWord, nonWord);
                SetStatus(message);
            }
            else
            {
                UpdateButtons();
                SetStatus(skipped > 0 ? Loc.T(Txt.WordPdfNothingAdded) : ReadyText());
            }
        }

        private bool TryAddFile(string path, HashSet<string> existing)
        {
            try
            {
                if (!File.Exists(path) || !MergeEngine.IsWordFile(path)) return false;
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
            SetStatus(ReadyText());
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
            _nameEdited = false;
            RefreshList();
            SetStatus(ReadyText());
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

            _countLabel.Text = Loc.T(Txt.WordPdfCountFormat, _documents.Count);
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
            if (!_nameEdited) SetNameSilently(BuildDefaultMergedName());
            UpdateButtons();
            UpdateOutputInfo();
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
            _nameInput.Enabled = !_running && _settings.WordPdfMergeIntoOne;
            _list.Enabled = !_running;
            _startButton.Enabled = _running || (hasDocuments && HasUsableEngine());
            _openFolderButton.Enabled = !_running && _openFolderButton.Visible;
        }

        private void UpdateMergeMode()
        {
            bool merge = _settings.WordPdfMergeIntoOne;
            _nameLabel.Visible = merge;
            _nameInput.Visible = merge;
            _separateHint.Visible = !merge;
            UpdateButtons();
        }

        private string BuildDefaultMergedName()
        {
            if (_documents.Count == 0) return string.Empty;
            string stem = WordPdfEngine.SanitizeStem(
                Path.GetFileNameWithoutExtension(_documents[0].Path));
            return stem.Length == 0 ? Loc.T(Txt.DefaultDocName) : stem;
        }

        private void SetNameSilently(string text)
        {
            _suppressEvents = true;
            _nameInput.Text = text;
            _suppressEvents = false;
        }

        // ---------------- 保存位置与输出信息 ----------------

        private string ResolveOutputFolder()
        {
            string last = (_settings.LastWordPdfFolder ?? string.Empty).Trim();
            if (last.Length > 0) return last;

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
            _settings.LastWordPdfFolder = dialog.SelectedPath;
            _settings.Save();
            SetStatus(Loc.T(Txt.StatusFolderChosen));
        }

        private void SetFolderSilently(string text)
        {
            _suppressEvents = true;
            _folderInput.Text = text;
            _suppressEvents = false;
        }

        private void UpdateOutputInfo()
        {
            if (_settings.WordPdfMergeIntoOne)
            {
                string stem = WordPdfEngine.SanitizeStem(_nameInput.Text);
                if (stem.Length == 0) stem = BuildDefaultMergedName();
                if (stem.Length == 0) stem = Loc.T(Txt.DefaultDocName);
                _outputPreview.Text = Loc.T(Txt.WordPdfNamePreview, stem + ".pdf");
            }
            else
            {
                _outputPreview.Text = Loc.T(Txt.WordPdfNamePreviewMulti, _documents.Count);
            }

            _outputPreview.ForeColor = Theme.TextMuted;
            UpdateButtons();
        }

        private bool HasUsableEngine() =>
            (_documents.Count > 0
                ? MergeEngine.SelectConverter(_documents.Select(d => d.Path))
                : MergeEngine.DetectedConverter) != WordConverter.None;

        private string ReadyText()
        {
            WordConverter converter = _documents.Count > 0
                ? MergeEngine.SelectConverter(_documents.Select(d => d.Path))
                : MergeEngine.DetectedConverter;
            if (converter == WordConverter.None)
                return Loc.T(Txt.WordPdfNoEngine);

            Txt engineKey = converter switch
            {
                WordConverter.MicrosoftWord => Txt.WordPdfEngineWord,
                WordConverter.WpsOffice => Txt.WordPdfEngineWps,
                _ => Txt.WordPdfEngineLibre
            };
            string mode = Loc.T(_settings.WordPdfMergeIntoOne
                ? Txt.WordPdfReadyMerge
                : Txt.WordPdfReady);
            return mode + "  ·  " + Loc.T(engineKey);
        }

        private void UpdateReadyStatus()
        {
            if (_running) return;
            SetStatus(ReadyText());
        }

        // ---------------- 转换 ----------------

        private void StartOrCancel()
        {
            if (_running)
            {
                _cts?.Cancel();
                _startButton.Enabled = false;
                _startButton.Text = Loc.T(Txt.WordPdfCancelling);
                return;
            }

            StartConversion();
        }

        private void StartConversion()
        {
            if (_documents.Count == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.WordPdfNoFiles), Loc.T(Txt.WordPdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!HasUsableEngine())
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.WordPdfNoEngine), Loc.T(Txt.WordPdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string folder = _folderInput.Text.Trim();
            if (folder.Length == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.WordPdfNeedFolder), Loc.T(Txt.WordPdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(folder))
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ErrFolderMissing, folder), Loc.T(Txt.WordPdfFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] files = _documents.Select(d => d.Path).ToArray();
            if (!TryCollectPasswords(files, out Dictionary<string, string>? passwords))
                return;

            bool merge = _settings.WordPdfMergeIntoOne;
            string? mergedStem = merge ? WordPdfEngine.SanitizeStem(_nameInput.Text) : null;

            var watermark = _settings.WatermarkForWordPdf
                ? _settings.CreateWatermarkSpec()
                : null;

            _settings.WordPdfMergeIntoOne = merge;
            _settings.LastWordPdfFolder = folder;
            _settings.Save();

            SetRunning(true);
            _progress.Value = 0;
            _openFolderButton.Visible = false;
            _startButton.Text = Loc.T(Txt.WordPdfCancel);
            _startButton.Icon = IconKind.Close;
            _startButton.ApplyTheme();
            SetStatus(Loc.T(Txt.WordPdfProcessing, 1, files.Length, Path.GetFileName(files[0])));

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            var progress = new Progress<WordPdfProgress>(p =>
            {
                if (!_running) return;
                _progress.Value = p.Percent;
                if (p.FileIndex >= 0)
                {
                    SetStatus(Loc.T(Txt.WordPdfProcessing,
                        p.FileIndex + 1, p.FileCount, p.FileName));
                }
                else
                {
                    SetStatus(Loc.T(Txt.WordPdfMerging));
                }
            });

            var thread = new Thread(() =>
            {
                try
                {
                    WordPdfBatchResult result = WordPdfEngine.Run(
                        files, folder, merge, mergedStem, watermark, progress, passwords, token);
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

        private bool TryCollectPasswords(string[] files, out Dictionary<string, string>? passwords)
        {
            passwords = null;

            foreach (string file in files)
            {
                if (!MergeEngine.WordNeedsPassword(file)) continue;

                string? entered = null;
                while (true)
                {
                    using var dialog = new PasswordPromptForm(
                        Path.GetFileName(file),
                        entered == null ? null : Loc.T(Txt.MergePasswordWrong));

                    if (dialog.ShowDialog(this) != DialogResult.OK) return false;
                    entered = dialog.Password;
                    break;
                }

                passwords ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                passwords[file] = entered!;
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

        private void FinishConversion(WordPdfBatchResult result, string folder)
        {
            ResetRunState();

            string elapsed = FormatElapsed(result.Elapsed);
            int completed = result.OutputPaths.Count;
            if (result.Cancelled)
            {
                _progress.Value = 0;
                SetStatus(Loc.T(Txt.WordPdfCancelled, completed));
            }
            else if (result.Merged)
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.WordPdfDone, result.PageCount, elapsed));
            }
            else
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.WordPdfDoneFiles, completed, elapsed));
            }

            _lastOutputFolder = folder;
            _openFolderButton.Visible = completed > 0 && Directory.Exists(folder);
            _openFolderButton.Enabled = true;

            if (!result.Cancelled && completed > 0)
            {
                string message = result.Merged
                    ? Loc.T(Txt.WordPdfDone, result.PageCount, elapsed)
                    : Loc.T(Txt.WordPdfDoneFiles, completed, elapsed);
                string detail = result.Merged
                    ? Path.GetFileName(result.OutputPaths[0])
                    : Loc.T(Txt.WordPdfOutputCount, completed);
                ShowCompletion(message, detail, () => OpenFolder(folder));
                if (_settings.OpenAfterMerge) OpenFolder(folder);
            }
        }

        private void ResetRunState()
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.WordPdfStart);
            _startButton.Icon = IconKind.WordPdf;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0 && HasUsableEngine();
        }

        private void ShowCompletion(string message, string detail, Action onClick)
        {
            if (_settings.PlaySound)
                SoundLibrary.Play(_settings.SoundId);

            if (!_settings.ShowNotification || IsDisposed) return;

            var toast = new ToastForm(Loc.T(Txt.WordPdfPageTitle), message, detail,
                Screen.FromControl(this).WorkingArea, onClick);
            toast.ShowToast();
        }

        private void FinishFailure(Exception exception)
        {
            ResetRunState();
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.WordPdfFailStatus));
            WriteFailureLog(exception);

            string message = exception is WordPdfException
                ? exception.Message
                : Loc.T(Txt.FailPrefix, exception.Message);
            ThemedMessageBox.Show(this, message, Loc.T(Txt.WordPdfFailTitle),
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

        private static void WriteFailureLog(Exception exception)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "last-word-pdf-error.log"),
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
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
            base.Dispose(disposing);
        }
    }
}
