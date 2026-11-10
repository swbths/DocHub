using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DocHub
{
    /// <summary>检测本机是否已关联 PDF 阅读器。</summary>
    internal static class PdfShell
    {
        public static bool HasDefaultReader()
        {
            try
            {
                using var choice = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.pdf\UserChoice");
                string? progId = choice?.GetValue("ProgId") as string;
                if (!string.IsNullOrWhiteSpace(progId)) return true;

                using var ext = Registry.ClassesRoot.OpenSubKey(".pdf");
                progId = ext?.GetValue(null) as string;
                if (!string.IsNullOrWhiteSpace(progId)) return true;

                using var openWith = ext?.OpenSubKey("OpenWithProgids");
                if (openWith != null)
                {
                    foreach (string name in openWith.GetValueNames())
                    {
                        if (!string.IsNullOrWhiteSpace(name)) return true;
                    }
                }

                using var assoc = Registry.ClassesRoot.OpenSubKey(@"SystemFileAssociations\.pdf\shell\open\command");
                return assoc?.GetValue(null) != null;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 简易 PDF 阅读器：仅在系统没有关联 PDF 阅读器时作为后备使用。
    /// 支持翻页与缩放，不抢占用户默认阅读器。
    /// </summary>
    public sealed class PdfViewerForm : Form
    {
        private const int WmNcLButtonDown = 0xA1;
        private const int HtCaption = 2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private readonly string _path;
        private readonly Panel _header;
        private readonly Panel _toolbar;
        private readonly Panel _scroll;
        private readonly PictureBox _canvas;
        private readonly Label _title;
        private readonly Label _subtitle;
        private readonly Label _pageLabel;
        private readonly Label _noteLabel;
        private readonly RoundedButton _prevButton;
        private readonly RoundedButton _nextButton;
        private readonly RoundedButton _zoomInButton;
        private readonly RoundedButton _zoomOutButton;
        private readonly GlyphCloseButton _closeButton;

        private PdfDocument? _document;
        private int _pageIndex;
        private double _zoom = 1.0;
        private bool _rendering;
        private bool _closing;

        public PdfViewerForm(string path)
        {
            _path = path;

            Text = Loc.T(Txt.PdfViewerTitle);
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(920, 760);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;
            BackColor = Theme.Background;
            Font = Theme.BaseFont;
            DoubleBuffered = true;
            Icon = AppIconProvider.Get();

            _header = new Panel
            {
                Bounds = new Rectangle(0, 0, ClientSize.Width, 58),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Theme.HeaderStart
            };
            _title = new Label
            {
                Text = Loc.T(Txt.PdfViewerTitle),
                Font = Theme.DialogTitleFont,
                ForeColor = Theme.HeaderText,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(24, 14)
            };
            _subtitle = new Label
            {
                Text = Path.GetFileName(path),
                Font = Theme.SmallFont,
                ForeColor = Theme.HeaderSubtitle,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(24 + TextRenderer.MeasureText(Loc.T(Txt.PdfViewerTitle),
                    Theme.DialogTitleFont).Width + 16, 20)
            };
            _closeButton = new GlyphCloseButton
            {
                Bounds = new Rectangle(ClientSize.Width - 54, 14, 34, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _closeButton.Click += (_, _) => Close();

            _toolbar = new Panel
            {
                Bounds = new Rectangle(0, 58, ClientSize.Width, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Theme.Background
            };

            _prevButton = new RoundedButton
            {
                Text = Loc.T(Txt.PrevPage),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(24, 10, 82, 30),
                Radius = 6,
                Font = Theme.SmallFont
            };
            _nextButton = new RoundedButton
            {
                Text = Loc.T(Txt.NextPage),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(114, 10, 82, 30),
                Radius = 6,
                Font = Theme.SmallFont
            };
            _pageLabel = new Label
            {
                Text = string.Empty,
                Font = Theme.SmallFont,
                ForeColor = Theme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(214, 17)
            };

            _zoomOutButton = new RoundedButton
            {
                Text = Loc.T(Txt.ZoomOut),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(ClientSize.Width - 180, 10, 72, 30),
                Radius = 6,
                Font = Theme.SmallFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _zoomInButton = new RoundedButton
            {
                Text = Loc.T(Txt.ZoomIn),
                Role = ButtonRole.Secondary,
                Bounds = new Rectangle(ClientSize.Width - 96, 10, 72, 30),
                Radius = 6,
                Font = Theme.SmallFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _prevButton.ApplyTheme();
            _nextButton.ApplyTheme();
            _zoomOutButton.ApplyTheme();
            _zoomInButton.ApplyTheme();

            _scroll = new Panel
            {
                Bounds = new Rectangle(24, 112, ClientSize.Width - 48, ClientSize.Height - 168),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                BackColor = Theme.Separator
            };
            _canvas = new PictureBox
            {
                Location = new Point(12, 12),
                SizeMode = PictureBoxSizeMode.Normal,
                BackColor = Color.White,
                Size = new Size(1, 1)
            };
            _scroll.Controls.Add(_canvas);

            _noteLabel = new Label
            {
                Text = Loc.T(Txt.ViewerNoReader),
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                BackColor = Color.Transparent,
                AutoSize = false,
                Visible = !PdfShell.HasDefaultReader(),
                Location = new Point(24, ClientSize.Height - 38),
                Size = new Size(ClientSize.Width - 48, 20),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _header.Controls.AddRange(new Control[] { _title, _subtitle, _closeButton });
            _toolbar.Controls.AddRange(new Control[] { _prevButton, _nextButton, _pageLabel, _zoomOutButton, _zoomInButton });
            Controls.AddRange(new Control[] { _scroll, _toolbar, _header, _noteLabel });

            _prevButton.Click += (_, _) => GoToPage(_pageIndex - 1);
            _nextButton.Click += (_, _) => GoToPage(_pageIndex + 1);
            _zoomInButton.Click += (_, _) => ChangeZoom(1.25);
            _zoomOutButton.Click += (_, _) => ChangeZoom(0.8);
            _scroll.Resize += (_, _) => CenterCanvas();

            _header.MouseDown += DragWindow;
            _title.MouseDown += DragWindow;
            _subtitle.MouseDown += DragWindow;

            KeyDown += OnViewerKeyDown;
            FormClosing += (_, _) => _closing = true;

            UpdateNav();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await LoadDocumentAsync();
        }

        private void DragWindow(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WmNcLButtonDown, (IntPtr)HtCaption, IntPtr.Zero);
        }

        private async Task LoadDocumentAsync()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Path.GetFullPath(_path));
                _document = await PdfDocument.LoadFromFileAsync(file);

                if (_document.PageCount == 0)
                    throw new InvalidOperationException("PDF has no pages");

                _pageIndex = 0;
                using (var first = _document.GetPage(0))
                    _zoom = FitZoom(first.Size.Width);

                UpdateNav();
                await RenderAsync();
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ViewerLoadFailed, ex.Message),
                    Loc.T(Txt.PdfViewerTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private double FitZoom(double pageWidth)
        {
            double available = Math.Max(240, _scroll.ClientSize.Width - 36);
            double zoom = available / Math.Max(1.0, pageWidth);
            return Math.Clamp(zoom, 0.4, 1.6);
        }

        private async Task RenderAsync()
        {
            if (_document == null || _rendering || _closing) return;

            _rendering = true;
            UpdateNav();
            string tempPath = Path.Combine(Path.GetTempPath(), "DocHub",
                "view_" + Guid.NewGuid().ToString("N") + ".png");
            try
            {
                using var page = _document.GetPage((uint)_pageIndex);
                double width = Math.Max(1.0, page.Size.Width * _zoom);
                double height = Math.Max(1.0, page.Size.Height * _zoom);

                // 限制超高清渲染尺寸，避免高倍缩放时内存暴涨
                if (width > 6000)
                {
                    height *= 6000.0 / width;
                    width = 6000;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);
                using (File.Create(tempPath)) { /* 先创建占位文件供 StorageFile 使用 */ }

                StorageFile tempFile = await StorageFile.GetFileFromPathAsync(tempPath);
                using (IRandomAccessStream stream = await tempFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await page.RenderToStreamAsync(stream, new PdfPageRenderOptions
                    {
                        DestinationWidth = (uint)Math.Round(width),
                        DestinationHeight = (uint)Math.Round(height)
                    });
                }

                using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var decoded = Image.FromStream(fileStream);
                var bitmap = new Bitmap(decoded);

                Image? old = _canvas.Image;
                _canvas.Image = bitmap;
                _canvas.Size = bitmap.Size;
                old?.Dispose();

                CenterCanvas();
                _pageLabel.Text = Loc.T(Txt.ViewerPageFormat, _pageIndex + 1, (int)_document.PageCount);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show(this, Loc.T(Txt.ViewerLoadFailed, ex.Message),
                    Loc.T(Txt.PdfViewerTitle), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { /* 临时文件稍后由系统清理 */ }
                _rendering = false;
                UpdateNav();
            }
        }

        private void CenterCanvas()
        {
            if (_canvas.Image == null) return;
            int x = Math.Max(12, (_scroll.ClientSize.Width - _canvas.Width) / 2);
            _canvas.Location = new Point(x, 12);
        }

        private void GoToPage(int index)
        {
            if (_document == null) return;
            int max = (int)_document.PageCount - 1;
            index = Math.Clamp(index, 0, max);
            if (index == _pageIndex) return;

            _pageIndex = index;
            _scroll.AutoScrollPosition = Point.Empty;
            _ = RenderAsync();
        }

        private void ChangeZoom(double factor)
        {
            if (_document == null || _rendering) return;
            double next = Math.Clamp(_zoom * factor, 0.25, 4.0);
            if (Math.Abs(next - _zoom) < 0.001) return;

            _zoom = next;
            _ = RenderAsync();
        }

        private void UpdateNav()
        {
            bool ready = _document != null && !_rendering;
            _prevButton.Enabled = ready && _pageIndex > 0;
            _nextButton.Enabled = ready && _pageIndex < (int)(_document?.PageCount ?? 0) - 1;
            _zoomInButton.Enabled = ready && _zoom < 4.0;
            _zoomOutButton.Enabled = ready && _zoom > 0.25;
        }

        private void OnViewerKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.PageUp)
            {
                GoToPage(_pageIndex - 1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.PageDown)
            {
                GoToPage(_pageIndex + 1);
                e.Handled = true;
            }
        }
    }
}
