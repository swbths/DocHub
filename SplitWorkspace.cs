using System;
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
    /// <summary>PDF 拆分页：PDF 队列、按页/每 N 页拆分、进度与取消。</summary>
    public sealed class SplitWorkspace : UserControl
    {
        private const int SideWidth = 306;

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
        private readonly Label _modeLabel;
        private readonly SegmentedControl _modeControl;
        private readonly Label _pagesLabel;
        private readonly RoundedInput _pagesInput;
        private readonly Label _pagesHint;
        private readonly Label _outputLabel;
        private readonly RoundedInput _folderInput;
        private readonly RoundedButton _browseButton;
        private readonly Panel _divider;
        private readonly Label _statusLabel;
        private readonly SlimProgressBar _progress;
        private readonly RoundedButton _openFolderButton;
        private readonly RoundedButton _startButton;

        private readonly List<DocumentItem> _documents = new();
        private readonly Dictionary<string, string> _passwords = new(StringComparer.OrdinalIgnoreCase);

        private CancellationTokenSource? _cts;
        private bool _running;
        private bool _suppressEvents;
        private bool _manualFolder;
        private bool _pagesVisible;
        private string _lastOutputFolder = string.Empty;

        public event EventHandler? BusyChanged;

        public bool IsBusy => _running;

        public SplitWorkspace(AppSettings settings)
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
                TitleOverride = Loc.T(Txt.SplitEmptyTitle),
                SubtitleOverride = Loc.T(Txt.SplitEmptySubtitle),
                FormatsOverride = Loc.T(Txt.SplitEmptyFormats),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _outputPreview = MakeLabel(string.Empty, Theme.SmallFont, "muted", 12, 0);

            _settingsTitle = MakeLabel(string.Empty, Theme.SectionFont, "section", 16, 18);
            _modeLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _modeControl = new SegmentedControl();
            _pagesLabel = MakeLabel(string.Empty, Theme.BaseFont, "secondary", 16, 0);
            _pagesInput = new RoundedInput { MaxLength = 3 };
            _pagesInput.Box.PlaceholderText = "1–500";
            _pagesHint = MakeLabel(string.Empty, Theme.SmallFont, "muted", 16, 0);
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
                Icon = IconKind.Split,
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
                _settingsTitle, _modeLabel, _modeControl, _pagesLabel, _pagesInput, _pagesHint,
                _outputLabel, _folderInput, _browseButton, _divider,
                _statusLabel, _progress, _openFolderButton, _startButton
            });
            Controls.Add(_listCard);
            Controls.Add(_sideCard);

            _list.ActionRemove += (_, _) => RemoveSelected();
            _list.SelectedIndexChanged += (_, _) => UpdateButtons();
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
            _modeControl.SelectionChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                _settings.SplitMode = CurrentMode;
                _settings.Save();
                UpdateModeVisibility();
                LayoutSideCard();
                UpdateOutputInfo();
            };
            _pagesInput.TextChanged += (_, _) =>
            {
                if (_suppressEvents || _running) return;
                if (int.TryParse(_pagesInput.Text.Trim(), out int pages) &&
                    pages >= PdfSplitEngine.MinPagesPerFile && pages <= PdfSplitEngine.MaxPagesPerFile)
                {
                    _settings.SplitPagesPerFile = pages;
                    _settings.Save();
                }
                UpdateOutputInfo();
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
            _progress.ApplyTheme();
            _modeControl.ApplyTheme();
            Invalidate(true);
        }

        public void ApplyLanguage()
        {
            Font = Theme.BaseFont;
            _sectionLabel.Text = Loc.T(Txt.SplitFilesTitle);
            _sectionLabel.Font = Theme.SectionFont;
            _countLabel.Font = Theme.SmallFont;
            _hintLabel.Text = Loc.T(Txt.SplitListHint);
            _hintLabel.Font = Theme.SmallFont;

            _addButton.Text = Loc.T(Txt.SplitAdd);
            _addButton.Font = Theme.ButtonFont;
            _addButton.ApplyTheme();
            _browseButton.Text = Loc.T(Txt.Browse);
            _browseButton.Font = Theme.ButtonFont;
            _browseButton.ApplyTheme();

            _emptyPanel.TitleOverride = Loc.T(Txt.SplitEmptyTitle);
            _emptyPanel.SubtitleOverride = Loc.T(Txt.SplitEmptySubtitle);
            _emptyPanel.FormatsOverride = Loc.T(Txt.SplitEmptyFormats);
            _emptyPanel.Invalidate();

            _settingsTitle.Text = Loc.T(Txt.SplitSettingsTitle);
            _settingsTitle.Font = Theme.SectionFont;
            _modeLabel.Text = Loc.T(Txt.SplitMode);
            _modeLabel.Font = Theme.BaseFont;
            _pagesLabel.Text = Loc.T(Txt.SplitPagesPerFile);
            _pagesLabel.Font = Theme.BaseFont;
            _pagesHint.Text = Loc.T(Txt.SplitPagesPerFileHint);
            _pagesHint.Font = Theme.SmallFont;
            _outputLabel.Text = Loc.T(Txt.SplitOutput);
            _outputLabel.Font = Theme.BaseFont;
            _folderInput.Box.PlaceholderText = Loc.T(Txt.ChooseFolder);
            _openFolderButton.Text = Loc.T(Txt.ConvertOpenFolder);
            _openFolderButton.Font = Theme.ButtonFont;
            _openFolderButton.ApplyTheme();
            _statusLabel.Font = Theme.BaseFont;
            _outputPreview.Font = Theme.SmallFont;

            _modeControl.Segments = new[]
            {
                Loc.T(Txt.SplitModeEveryPage),
                Loc.T(Txt.SplitModeEveryN)
            };
            _modeControl.SelectedIndex = (int)CurrentMode;

            _countLabel.Text = Loc.T(Txt.SplitCountFormat, _documents.Count);
            if (!_running) SetStatus(Loc.T(Txt.SplitReady));
            _startButton.Text = _running ? Loc.T(Txt.SplitRunning) : Loc.T(Txt.SplitStart);
            _startButton.Font = Theme.PrimaryButtonFont;

            _list.ApplyTheme();
            UpdateModeVisibility();
            UpdateOutputInfo();
            RefreshList(_list.SelectedIndex);
            ConfigureToolTips();
            LayoutCards();
        }

        public void ApplySettings()
        {
            if (!_manualFolder) SetFolderSilently(ResolveOutputFolder());
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

            _modeLabel.Bounds = new Rectangle(16, y, inner, 20);
            y += 20;
            _modeControl.Bounds = new Rectangle(16, y, inner, 32);
            y += 38;

            if (_pagesVisible)
            {
                int labelWidth = Math.Min(inner - 80,
                    TextRenderer.MeasureText(_pagesLabel.Text, _pagesLabel.Font).Width + 8);
                _pagesLabel.Bounds = new Rectangle(16, y, Math.Max(60, labelWidth), 36);
                _pagesInput.Bounds = new Rectangle(16 + Math.Max(60, labelWidth), y,
                    Math.Max(60, inner - Math.Max(60, labelWidth)), 36);
                y += 42;
                _pagesHint.Bounds = new Rectangle(16, y, inner, 32);
                y += 38;
            }

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

        // ---------------- 设置与队列 ----------------

        private PdfSplitMode CurrentMode =>
            (PdfSplitMode)Math.Clamp(_modeControl.SelectedIndex, 0, 1);

        private void ResolveDefaults()
        {
            _suppressEvents = true;
            _modeControl.SelectedIndex = (int)_settings.SplitMode;
            _pagesInput.Text = Math.Clamp(_settings.SplitPagesPerFile,
                PdfSplitEngine.MinPagesPerFile, PdfSplitEngine.MaxPagesPerFile).ToString();
            _suppressEvents = false;
            UpdateModeVisibility();
        }

        private void UpdateModeVisibility()
        {
            bool show = CurrentMode == PdfSplitMode.EveryNPages;
            _pagesVisible = show;
            _pagesLabel.Visible = show;
            _pagesInput.Visible = show;
            _pagesHint.Visible = show;
        }

        private void ShowAddDialog()
        {
            if (_running) return;
            using var dialog = new OpenFileDialog
            {
                Title = Loc.T(Txt.SplitDialogTitle),
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
                string message = Loc.T(Txt.SplitAdded, added);
                int nonPdf = paths.Count(p => File.Exists(p) && !MergeEngine.IsPdfFile(p));
                if (nonPdf > 0) message += "  ·  " + Loc.T(Txt.SplitSkippedNonPdf, nonPdf);
                SetStatus(message);
            }
            else
            {
                UpdateButtons();
                SetStatus(skipped > 0 ? Loc.T(Txt.SplitNothingAdded) : Loc.T(Txt.SplitReady));
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
                        UpdateOutputInfo();
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
            SetStatus(Loc.T(Txt.SplitReady));
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
            SetStatus(Loc.T(Txt.SplitReady));
        }

        private void RefreshList(int selectIndex = -1)
        {
            int previous = _list.SelectedIndex;

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (DocumentItem item in _documents)
                _list.Items.Add(item);
            _list.EndUpdate();

            _countLabel.Text = Loc.T(Txt.SplitCountFormat, _documents.Count);
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
            _modeControl.Enabled = !_running;
            _pagesInput.Enabled = !_running && _pagesVisible;
            _list.Enabled = !_running;
            _startButton.Enabled = _running || hasDocuments;
            _openFolderButton.Enabled = !_running && _openFolderButton.Visible;
        }

        // ---------------- 保存位置与输出信息 ----------------

        private string ResolveOutputFolder()
        {
            string last = (_settings.LastSplitFolder ?? string.Empty).Trim();
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
            _settings.LastSplitFolder = dialog.SelectedPath;
            _settings.Save();
            SetStatus(Loc.T(Txt.StatusFolderChosen));
        }

        private void SetFolderSilently(string text)
        {
            _suppressEvents = true;
            _folderInput.Text = text;
            _suppressEvents = false;
        }

        private PdfSplitOptions CurrentOptions()
        {
            int pages = PdfSplitEngine.MinPagesPerFile;
            if (CurrentMode == PdfSplitMode.EveryNPages)
            {
                if (!int.TryParse(_pagesInput.Text.Trim(), out pages))
                    pages = _settings.SplitPagesPerFile;
                pages = Math.Clamp(pages, PdfSplitEngine.MinPagesPerFile, PdfSplitEngine.MaxPagesPerFile);
            }
            return new PdfSplitOptions { Mode = CurrentMode, PagesPerFile = pages };
        }

        private void UpdateOutputInfo()
        {
            if (_documents.Count == 0)
            {
                _outputPreview.Text = Loc.T(Txt.SplitPreviewUnknown);
            }
            else if (_documents.Any(d => d.PageCount <= 0))
            {
                _outputPreview.Text = Loc.T(Txt.SplitPreviewUnknown);
            }
            else
            {
                PdfSplitOptions options = CurrentOptions();
                int total = _documents.Sum(d => PdfSplitEngine.CountOutputs(d.PageCount, options));
                _outputPreview.Text = Loc.T(Txt.SplitPreview, total);
            }

            _outputPreview.ForeColor = Theme.TextMuted;
            UpdateButtons();
        }

        // ---------------- 拆分 ----------------

        private void StartOrCancel()
        {
            if (_running)
            {
                _cts?.Cancel();
                _startButton.Enabled = false;
                _startButton.Text = Loc.T(Txt.SplitCancelling);
                return;
            }

            StartSplit();
        }

        private void StartSplit()
        {
            if (_documents.Count == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.SplitNoFiles), Loc.T(Txt.SplitFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = _folderInput.Text.Trim();
            if (folder.Length == 0)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.SplitNeedFolder), Loc.T(Txt.SplitFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(folder))
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ErrFolderMissing, folder), Loc.T(Txt.SplitFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CurrentMode == PdfSplitMode.EveryNPages &&
                (!int.TryParse(_pagesInput.Text.Trim(), out int pages) ||
                 pages < PdfSplitEngine.MinPagesPerFile || pages > PdfSplitEngine.MaxPagesPerFile))
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.SplitBadPages), Loc.T(Txt.SplitFailTitle),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] files = _documents.Select(d => d.Path).ToArray();
            if (!TryCollectPasswords(files)) return;

            PdfSplitOptions options = CurrentOptions();
            _settings.SplitMode = options.Mode;
            _settings.SplitPagesPerFile = options.PagesPerFile;
            _settings.LastSplitFolder = folder;
            _settings.Save();

            SetRunning(true);
            _progress.Value = 0;
            _openFolderButton.Visible = false;
            _startButton.Text = Loc.T(Txt.SplitCancel);
            _startButton.Icon = IconKind.Close;
            _startButton.ApplyTheme();
            SetStatus(Loc.T(Txt.SplitProcessing, 1, files.Length, Path.GetFileName(files[0])));

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            var progress = new Progress<PdfSplitProgress>(p =>
            {
                if (!_running) return;
                _progress.Value = p.Percent;
                SetStatus(Loc.T(Txt.SplitProcessing,
                    p.FileIndex + 1, p.FileCount, p.FileName));
            });

            var thread = new Thread(() =>
            {
                try
                {
                    PdfSplitResult result = PdfSplitEngine.Run(
                        files, folder, options, progress, token, _passwords);
                    BeginInvoke(new Action(() => FinishSplit(result, folder)));
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

        private void FinishSplit(PdfSplitResult result, string folder)
        {
            ResetRunState();

            string elapsed = FormatElapsed(result.Elapsed);
            if (result.Cancelled)
            {
                SetStatus(Loc.T(Txt.SplitCancelled, result.OutputCount));
            }
            else if (result.FailedCount > 0)
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.SplitDonePartial,
                    result.SucceededCount, result.FailedCount, result.OutputCount));
                WriteFailureLog(result);
                if (result.SucceededCount == 0)
                {
                    string message = result.Files.FirstOrDefault(file => file.Error != null)?.Error
                        ?? Loc.T(Txt.SplitFailStatus);
                    ThemedMessageBox.Show(this, message, Loc.T(Txt.SplitFailTitle),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _progress.Value = 100;
                SetStatus(Loc.T(Txt.SplitDone, result.OutputCount, elapsed));
            }

            _lastOutputFolder = folder;
            _openFolderButton.Visible = Directory.Exists(folder);
            _openFolderButton.Enabled = true;
            _settings.LastSplitFolder = folder;
            _settings.Save();

            if (result.OutputCount > 0 && !result.Cancelled)
            {
                ShowCompletion(result, folder, elapsed);
                if (_settings.OpenAfterMerge) OpenFolder(folder);
            }
        }

        private void ShowCompletion(PdfSplitResult result, string folder, string elapsed)
        {
            if (_settings.PlaySound)
                SoundLibrary.Play(_settings.SoundId);

            if (!_settings.ShowNotification || IsDisposed) return;

            string subtitle = result.FailedCount > 0
                ? Loc.T(Txt.SplitDonePartial,
                    result.SucceededCount, result.FailedCount, result.OutputCount)
                : Loc.T(Txt.SplitDone, result.OutputCount, elapsed);

            var toast = new ToastForm(Loc.T(Txt.SplitPageTitle), subtitle,
                Path.GetFileName(folder), Screen.FromControl(this).WorkingArea,
                () => OpenFolder(folder));
            toast.ShowToast();
        }

        private void ResetRunState()
        {
            _cts?.Dispose();
            _cts = null;
            SetRunning(false);
            _startButton.Text = Loc.T(Txt.SplitStart);
            _startButton.Icon = IconKind.Split;
            _startButton.ApplyTheme();
            _startButton.Enabled = _documents.Count > 0;
        }

        private void FinishFailure(Exception exception)
        {
            ResetRunState();
            _progress.Value = 0;
            SetStatus(Loc.T(Txt.SplitFailStatus));
            WriteFailureLog(exception);

            string message = exception is PdfSplitException
                ? exception.Message
                : Loc.T(Txt.SplitFailGeneric, exception.Message);
            ThemedMessageBox.Show(this, message, Loc.T(Txt.SplitFailTitle),
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

        private static void WriteFailureLog(PdfSplitResult result)
        {
            try
            {
                string folder = Path.GetDirectoryName(AppSettings.FilePath)!;
                Directory.CreateDirectory(folder);
                var failures = result.Files.Where(file => file.Error != null)
                    .Select(file => $"{file.Source}{Environment.NewLine}  {file.Error}");
                File.WriteAllText(Path.Combine(folder, "last-split-error.log"),
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
                File.WriteAllText(Path.Combine(folder, "last-split-error.log"),
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
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                _toolTip.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
