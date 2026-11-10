# 文枢 · DocHub

**DocHub: the document hub — merge and convert in one click.**

A Windows desktop tool that merges **Word, PDF and image** files into a **single lossless PDF** in the order you choose, converts **PDFs to images** in batches, converts **images to PDF** (separately or merged), and converts **Word to PDF** in batches.

Everything runs locally — no uploads, no telemetry.

- Version: `4.5.21` (2026-11-10)
- Platform: Windows 10 2004 (10.0.19041) or later, x64
- Stack: .NET 10 + WinForms + [PDFsharp](https://github.com/empira/PDFsharp) 6.2.4

---

## Features

### Lossless merging

| Type | Formats | How it is handled |
| --- | --- | --- |
| PDF | `.pdf` | Pages are copied as-is — no re-layout, no rasterisation, no quality loss |
| Word | `.doc` `.docx` `.docm` `.dot` `.dotx` `.dotm` `.rtf` `.odt` | Exported to PDF by local Word or LibreOffice, then merged |
| Images | `.jpg` `.jpeg` `.jfif` `.png` `.bmp` `.tif` `.tiff` `.gif` `.webp` `.heic` etc. | Placed on a single page at their original size, no scaling or cropping; HEIC / AVIF require the matching Windows decoder extension |

Image conversion runs in parallel across CPU cores, and merge progress is shown live.

### PDF to images

| Item | Details |
| --- | --- |
| Input | `.pdf`, one or many files or a whole folder (scanned recursively); reorder or remove rows |
| Output formats | PNG / JPEG / BMP / TIFF; JPEG by default |
| Resolution | 300 PPI by default; presets 72 / 96 / 150 / 300 / 600 PPI, or custom 36–1200 PPI, with an estimated pixel size shown live |
| JPEG quality | 100% by default, 1–100, JPEG only |
| Transparent background | PNG only; other formats use a white background |
| Page range | All pages / odd pages / even pages / custom ranges such as `1-3,5,8-10` |
| Output location | Same folder as the source / last used folder / chosen folder; each PDF gets its own subfolder by default, named with the page selection (e.g. `report_Even`) |
| Image names | Shared naming modes with merging (first file name / custom phrase / timestamp / custom name); pages are zero-padded, e.g. `name_p001.png` |
| Live preview | The first page selected by the current page mode is rendered automatically and refreshes when format, PPI or background changes |

- Encrypted PDFs prompt for the open password first; it is used for this run only and never written to disk
- A failed file does not stop the batch; successes and failures are summarised at the end
- The Start button turns into Cancel while running; cancelling keeps the images already exported
- Watermarks can be enabled for PDF to images on the Watermark settings page; they are drawn on output images only and the source PDF stays read-only
- The source PDF is opened read-only: its text, images and pages are never modified and the file stays byte-identical

### Images to PDF

| Item | Details |
| --- | --- |
| Input | `.jpg` `.jpeg` `.png` `.bmp` `.tif` `.tiff` `.gif` `.webp` etc.; multi-select, folder drop (recursive), drag to reorder, remove |
| Output | One PDF per image by default; enable **Merge into one PDF** to combine them in list order |
| Page size | Auto / A4 / A3 / A5 / B5 / Letter |
| Orientation | Auto / portrait / landscape; Auto follows the image aspect |
| Margins | None / 10 mm / 20 mm |
| Fitting | Auto sizes the page from pixels and DPI; fixed sizes scale proportionally and centre without cropping or stretching |
| Output location | Shared with merging: first image's folder / last used folder / chosen folder; duplicate names get `-1`, `-2` |
| Naming | Original image name by default; a custom name is supported, with `_001`, `_002` suffixes in separate mode; leave blank when merging to use the shared merge naming |

- Separate output is the default; a failed image does not stop the rest and the summary reports successes and failures
- Runs on a background thread and can be cancelled; separate mode keeps completed PDFs, while a cancelled merge leaves no partial file
- EXIF orientation is applied and transparent images are flattened onto white
- Corrupt or undecodable images produce a clear message without affecting the other checks

### Word to PDF

| Item | Details |
| --- | --- |
| Input | `.doc` `.docx` `.docm` `.dot` `.dotx` `.dotm` `.rtf` `.odt`; multi-select, folder drop (recursive), drag to reorder, remove |
| Output | One same-name PDF per Word file by default, with `-1`, `-2` appended on collisions; enable **Merge into one file** to combine them in list order |
| Naming | Separate output uses the original Word file name; merged output takes a custom name that defaults to the first Word file and falls back automatically when left blank |
| Engine | Uses local Microsoft Word or LibreOffice automatically; the Start button is disabled with a clear hint when neither is installed |
| Output location | The first Word file's folder by default; browse to choose and the last location is remembered |
| Watermark | Enable **Word to PDF** on the Watermark settings page to draw the watermark on output PDFs only; source Word files are never modified |

- Encrypted Word files prompt for the open password before starting; it is used for this run only and never written to disk
- Conversion runs on a background thread and can be cancelled; cancelled or failed runs clean up incomplete PDFs produced by that run

### File list

- Drop files, drop a folder (recursively collected), or use **Add documents**
- Reorder by **dragging rows**
- **Multi-select** then right-click: move up / move down / **move to top** / **move to bottom** / remove
- Clear all, live counts, duplicates skipped automatically

### Password-protected files

- Encrypted PDFs: a password dialog appears before merging, wrong passwords can be retyped, and the merge continues only after a successful check
- Encrypted Word (`.docx` `.docm` `.dotx` `.dotm`): detected from the file structure and prompted the same way
- Passwords are used for the current merge only — never written to disk or settings

### Watermark

- Targets: enable **Document merge**, **PDF to images**, **Images to PDF** and **Word to PDF** separately; all four switches default to off and share one style
- Text: 4 presets (Original / Copy / Confidential / Top Secret) plus any custom text
- Look: diagonal tiling, or pinned to top left / top right / centre / bottom left / bottom right; in grey, red or blue
- Controls: **opacity 0–100%**, **size 10–400%**, **rotation −180° to 180°**, **horizontal/vertical offset ±100%** — drag the slider or type a value directly
- Interactive preview: drag to move, scroll to resize, right-drag to rotate, double-click to reset; the watermark is generated as a transparent layer scaled to the page, so it stays readable and adds no compression noise
- Anti-clipping: oversized or edge-dragged watermarks are auto-scaled by their rotated bounding box and kept inside the page, so merged documents never show half a watermark
- For PDF to images the watermark is written into the output images only; the source PDF is never modified

### Output and naming

- Naming: first file name / custom phrase / timestamp / custom name
- Location: same folder as the first file / last used folder / chosen folder
- Built-in PDF viewer (page turning and zoom) via **View result**

### Experience

- Card-style sidebar: Merge / PDF to Images / Images to PDF / Word to PDF, with Watermark, Notifications, Skins, Settings and About pinned to the bottom
- All four file lists support drag reordering, marquee selection on empty space and Shift-click ranges; a plain click (including Ctrl+click) keeps only the current file, so the previous highlight disappears at once. Selected files can be removed in bulk, and Cancel Selection (or Esc) restores the selection from before the box drag
- 5 skins: Classic Blue (default), Night, Cloud White, Skeuomorphic, Neumorphic (light/dark colours switch with the skin; sidebar and body contrast are verified against WCAG)
- 6 UI languages: 简体中文, 繁體中文, English, 日本語, 한국어, Русский
- A frosted-glass "Daily quote" card on the merge page: 396 movie lines and famous quotes across all six languages (66 each) with their sources, following the UI language; plays randomly at startup and while running, rotating automatically every 30 seconds by default with a configurable interval (5–86400 seconds) under Appearance, click the card for another one immediately, and switch it off under Appearance
- Flat sidebar entries at the bottom left (Watermark, Notifications, Skins, Settings, About) with tooltips, each opening its settings page directly
- `Ctrl+1` / `Ctrl+2` / `Ctrl+3` / `Ctrl+4` switches pages, `Ctrl+O` adds files
- Completion feedback: system notification / 5 sounds (sound is off by default; sound choices are greyed out until enabled) / open the result automatically
- Every in-app dialog (confirmations, warnings, errors, questions and the password prompt) follows the active skin — dark skins no longer show white system pop-ups
- Tray support; every setting applies instantly and is saved to `%APPDATA%\DocHub\settings.json`

---

## Installation

### Installer (recommended)

Run `文枢_安装程序_v4.5.21.exe`:

- Installs to `%LOCALAPPDATA%\Programs\DocHub` (per-user) by default — **no administrator rights needed**, nothing written to `C:\Program Files`, no UAC prompt; the folder can be changed and the path you enter is used as-is (picking a path under `C:\Program Files` shows a warning that normal users usually cannot write there)
- Creates Start-menu and desktop shortcuts, plus an **Uninstall DocHub** entry in the Start-menu folder
- Appears under **Settings → Apps → Installed apps** and can be uninstalled from there
- Reinstalling closes the running app and silently removes the previous version first
- If an older **machine-wide** build is installed (`C:\Program Files (x86)\DocHub`), uninstall it from **Settings → Apps → Installed apps** before installing this version: the installer is per-user and has no administrator rights, so it cannot clean up the old machine-level folder or its HKLM uninstall entry (it warns instead of deleting anything)
- Uninstall removes the program folder, registry entry and shortcuts

Silent mode (deployment and automated tests; the installer is built in this repository and does not use NSIS):

```bat
:: Full silent install (per-user, default folder %LOCALAPPDATA%\Programs\DocHub)
文枢_安装程序_v4.5.21.exe --silent

:: Custom folder (a path under Program Files falls back to the per-user folder)
文枢_安装程序_v4.5.21.exe --silent --dir D:\Apps\DocHub

:: Extract the payload only: no registry, no shortcuts — handy for verifying contents
文枢_安装程序_v4.5.21.exe --extract D:\temp\dochub_payload

:: Silent uninstall
"%LOCALAPPDATA%\Programs\DocHub\app\DocHub.exe" --uninstall --yes
```

Exit codes: `0` success, `1` failure, `2` bad arguments, `3` some files could not be written (the installer reports the whole list once instead of prompting per file).

### Portable

Extract `文枢_绿色版_v4.5.21.zip` and run `DocHub.exe` from its root. Nothing is written to the registry and no shortcuts are created. The build is self-contained — **no .NET runtime is required**.

---

## Usage

1. Choose **Merge** in the sidebar
2. Add or drop the Word / PDF / image files you want to merge
3. Arrange the order (drag, or right-click move to top/bottom)
4. Enter passwords if prompted for encrypted files
5. To add a watermark, tick Document merge on the Watermark settings page and tune opacity, size, rotation and position
6. Choose naming and output location in Settings
7. Click **Start merge**, then **View result** when it finishes

PDF to images:

1. Choose **PDF to Images** in the sidebar
2. Add one or more PDFs (or drop a folder, scanned recursively)
3. Pick format, PPI (custom allowed) and page mode (all / odd / even / custom range); defaults are JPEG / 300 PPI / quality 100%, and PNG can use a transparent background
4. Choose the output folder and click **Start conversion**; failures are summarised and cancelling keeps the images already exported
5. To stamp output images, tick PDF to images on the Watermark settings page; the source PDF is never modified

Images to PDF:

1. Choose **Images to PDF** in the sidebar
2. Add or drop images (folder drops are scanned recursively) and drag rows to set the order
3. Pick page size (Auto / A4 / A3 / A5 / B5 / Letter), orientation and margins
4. Each image gets its own PDF by default; tick **Merge into one PDF** to combine them
5. Leave the file name blank to use the original image names, or enter a custom name (separate files get sequence suffixes)
6. Confirm the output folder and click **Start conversion**; open the PDF or folder when it finishes

> Word input requires **Microsoft Word** or **LibreOffice** on the machine; merging only PDFs or images, PDF-to-images or images-to-PDF does not.
> A legacy `.doc` protected with an open password cannot be detected automatically — save it as `.docx` in Word first.

---

## Build from source

Requirements:

- .NET SDK 10 (`dotnet --version` ≥ 10.0)
- .NET Framework 4.x (used by `csc.exe` to compile the small launcher)
- No third-party packaging tool: the installer is a .NET project in this repository (`installer\Setup.csproj`) and is compiled by the same script

```powershell
# One command: self-contained publish + launcher + portable zip + in-repo .NET installer
powershell -ExecutionPolicy Bypass -File tools\package.ps1
```

Outputs in `dist\`:

- `文枢_安装程序_v4.5.21.exe`
- `文枢_绿色版_v4.5.21.zip`

Build the app only:

```powershell
dotnet build DocHub.csproj -c Release
```

> The script publishes into `build\layout\app\` (with `DocHub.exe`), keeping the same `DocHub.exe` launcher at the root (an ASCII file name avoids write failures caused by non-ASCII executable names); the whole layout is then zipped into `installer\payload.zip`, embedded into the installer, and every layout file has its internet mark stripped. `build\` and `payload.zip` are cleaned up afterwards.

---

## Project layout

```
DocHub.csproj           Main project (net10.0-windows10.0.19041.0 / win-x64 / self-contained)
Program.cs              Entry point, single instance and command-line handling
MainForm.cs             Main window: sidebar navigation, merge page, drag & drop, progress, tray
ConvertWorkspace.cs     PDF-to-images page: file list, preview, format / PPI / page-range options, batch conversion
ImagePdfWorkspace.cs    Images-to-PDF page: image list, preview, page size / orientation / margins, batch combine
ImagePdfEngine.cs       Images-to-PDF engine: EXIF correction, white flattening, page fitting, PDF output
MergeEngine.cs          Merge engine: PDF page copy, Word to PDF, image to one page, encrypted files
PdfImageEngine.cs       PDF-to-image engine: Windows.Data.Pdf rendering + PNG/JPEG/BMP/TIFF encoding
WatermarkPainter.cs     Watermark rendering: GDI+ transparent layer over PDF pages, plus preview
SettingsForm.cs         Settings: appearance / naming & output / conversion / completion / watermark
PasswordPromptForm.cs   Password dialog for encrypted files
PdfViewerForm.cs        Built-in PDF viewer
AboutForm.cs            About and changelog
EulaForm.cs / ToastForm.cs   First-run agreement, lightweight toasts
UiControls.cs / GlassEffects.cs   Custom controls and acrylic glass effects
AppSettings.cs          Settings model and skin palettes (JSON persistence)
Quotes.cs               Quote and movie-line library for the 6 languages
Localization*.cs        Text for the 6 languages
SoundLibrary.cs         Completion sounds
SelfTest.cs             Headless self-test (`--selftest`): sidebar contrast, images-to-PDF page sizes / separate / merged / cancel, source PDF integrity
Uninstaller.cs          In-app uninstall flow (`--uninstall`), used as the registered uninstall entry
tools\package.ps1       Packaging script (publish + launcher + portable zip + installer)
tools\make_icon.ps1     Generates app.ico
tools\make_samples.ps1  Generates sample documents for testing
installer\Program.cs    Installer: per-user install, one-shot failure report, mark cleanup, HKCU entry
installer\Setup.csproj  Installer project (self-contained single file, embeds payload.zip)
installer\launcher\Launcher.cs   Launcher source (compiled to DocHub.exe)
```

Merge pipeline in short: Word → temporary PDF, images → one-page PDF, PDFs imported as-is; then pages are written into the output document in list order and the watermark layer is applied if enabled.

---

## FAQ

**"No Word converter found"**
Install Microsoft Word or LibreOffice and retry, or choose to skip Word files and merge the remaining PDFs/images.

**Merging feels slow**
Word conversion depends on Office start-up, so the first file is the slowest; with only PDFs/images the time is mostly disk writes.

**"Windows Security Center: Opening these files might be harmful — Your Internet security settings blocked one or more files" when double-clicking the installer**
This is the Windows zone check for files downloaded from the internet (the file carries a "downloaded from the internet" mark, `Zone.Identifier`). It is not a virus warning and has nothing to do with the app itself. Any one of these works:
1. Right-click the installer → **Properties** → tick **Unblock** at the bottom → **OK**, then double-click again (one-time, cleanest);
2. Just click **OK** in the prompt: installation continues, and the installer clears the mark from itself on start-up so it never appears again;
3. Files fetched from a command line have no mark — e.g. in PowerShell run `Invoke-WebRequest '<installer link>' -OutFile "$env:TEMP\dochub_setup.exe"`, then start `$env:TEMP\dochub_setup.exe`.

> Note: that first prompt comes from the OS policy for a *marked executable*; as long as the file was downloaded and is not code-signed, the very first double-click may still be interrupted once. What the installer guarantees is that the installed product (`%LOCALAPPDATA%\Programs\DocHub`) and every launch afterwards never prompt. To avoid it completely, use the portable zip.
> Portable build: extract with 7-Zip / Bandizip (Windows' built-in "Extract All" copies the mark onto the extracted files). The launcher `DocHub.exe` strips the mark from every file under the install folder and `app\` before starting the app, so at most the launcher itself can be prompted once.

**Will the installed app prompt again when launched?**
No. After extraction the installer recursively clears the internet mark from **every** file in the install folder (any extension, including `app\lib`), and the app does the same once more on start-up; the portable launcher performs the same cleanup before launching.

**"Microsoft Defender SmartScreen prevented an unrecognized app from starting"**
This is the standard Windows warning for unsigned binaries, not a virus alert. Click **More info → Run anyway** to continue. The project is open source and has no code-signing certificate, so the prompt stays; the installer carries full version information you can verify under **Properties → Details**.
If the file was downloaded from the internet, you can also unblock it first: right-click the installer → **Properties** → tick **Unblock** → **OK**.

**"Merge failed: Access to the path … is denied"**
The temporary folder needed for merging could not be created — usually a security suite (Tencent PC Manager, 360, Huorong) blocking writes from unknown programs. Add `DocHub.exe` to its trust list (or temporarily disable real-time protection) and retry. PDF-only merges need no temporary folder and keep working; only Word/image inputs require it.

**The installer does not react when double-clicked**
Copy it to the desktop or another ordinary folder and run it there — some managed folders block installer start-up.

**"Cannot open the file for writing" during install (abort / retry / ignore), or the install hangs on one file**
Since 4.1.0 the installer is an in-repo .NET program and **never** shows per-file abort/retry/ignore dialogs: a failed write is retried three times, then skipped and collected; at the end the installer lists up to 10 failures at once and keeps the files it did write.
When you see that list:
1. Close the running program completely (including the tray icon) — the installer also terminates `DocHub.exe` itself;
2. Temporarily disable the real-time file protection of your security suite (360, Tencent PC Manager, Huorong, …), run the installer again, then re-enable it;
3. Or just use the portable zip: unzip and run, no system folder and no registry involved.

**"Installation could not be completed: Access to the path …\Programs\DocHub is denied"**
When the preferred folder is blocked by a security suite or a permission policy, the installer **automatically tries other locations in order**: `%LOCALAPPDATA%\DocHub`, `%LOCALAPPDATA%\Programs\DocHub`, `%USERPROFILE%\DocHub`, `%APPDATA%\DocHub`, the shared folder `C:\Users\Public\DocHub`, a `DocHub` folder next to the installer, and finally `X:\DocHub` on other fixed drives. Each candidate is verified by actually writing a probe file first. On success the result page states the real location ("the original folder was not writable, installed to …" style), and shortcuts plus the uninstall entry point follow it.
If every location is refused, the installer writes `setup-error.log` next to itself (or in `%TEMP%`) listing the concrete error for each folder. Point the install folder to another drive and retry, temporarily disable the suite's real-time protection, or simply use the portable zip.

**Can I install into `C:\Program Files`?**
You neither need to nor should. The installer writes to the per-user folder `%LOCALAPPDATA%\Programs\DocHub` — **no Program Files writes, no UAC**; even if you type a `C:\Program Files\...` path it switches back to the per-user folder and explains why on screen.

**Leftovers after uninstalling**
A normal uninstall removes the folder, registry entry and shortcuts. If files were deleted manually, run the uninstaller once more from Installed apps to clean the entry.

---

## Feedback

- Author: AI
- Third-party components and licenses: see [NOTICE.md](NOTICE.md)
- Repository: <https://github.com/swbths/DocHub>
- This is a personal learning and utility project; issues and suggestions are welcome.

## Acknowledgements

[PDFsharp](https://github.com/empira/PDFsharp) (PDF reading/writing and merging), [.NET](https://dotnet.microsoft.com/). See [NOTICE.md](NOTICE.md) for the full component list and license texts.
