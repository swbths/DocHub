using System.Collections.Generic;

namespace DocHub
{
    internal static class LocEn
    {
        public static readonly Dictionary<Txt, string> Table = new()
        {
            [Txt.AppName] = "DocHub",
            [Txt.Tagline] = "The document hub — merge and convert in one click",
            [Txt.LocalBadge] = "Offline · Files never leave this device",

            [Txt.Ok] = "OK",
            [Txt.Close] = "Close",
            [Txt.Cancel] = "Cancel",
            [Txt.Yes] = "Yes",
            [Txt.No] = "No",
            [Txt.Save] = "Save",
            [Txt.Reset] = "Restore defaults",
            [Txt.Browse] = "Browse",
            [Txt.PreviewWord] = "Preview",
            [Txt.Settings] = "Settings",
            [Txt.About] = "About",
            [Txt.Skin] = "Skins",
            [Txt.RailMerge] = "Merge",

            [Txt.DocsTitle] = "Files",
            [Txt.DocCountFormat] = "{0} files",
            [Txt.ListHint] = "Drag to reorder · Drag empty space to box-select · Drop multiple files at once",
            [Txt.AddDocs] = "Add files",
            [Txt.MoveUp] = "Up",
            [Txt.MoveDown] = "Down",
            [Txt.MoveTop] = "To top",
            [Txt.MoveBottom] = "To bottom",
            [Txt.Remove] = "Remove",
            [Txt.Clear] = "Clear",
            [Txt.OutputFile] = "Output",
            [Txt.SaveTo] = "Save to",
            [Txt.ChooseFolder] = "Choose a folder",
            [Txt.StartMerge] = "Start merge",
            [Txt.MergingButton] = "Merging…",
            [Txt.ReadyStatus] = "Ready · merged top to bottom",
            [Txt.NamePlaceholderFirst] = "Auto: first file name",
            [Txt.NamePlaceholderPhrase] = "First file name + custom phrase",
            [Txt.NamePlaceholderTime] = "Merge time (auto-generated)",
            [Txt.NamePlaceholderCustom] = "Enter a custom file name",
            [Txt.EmptyTitle] = "Drop Word / WPS / PDF / image files here",
            [Txt.EmptySubtitle] = "or click “Add files” at the top right",
            [Txt.EmptyFormats] = ".doc · .docx · .rtf · .wps · .wpt · .pdf · .jpg · .png · .bmp · .tiff",

            [Txt.TipAdd] = "Select Word / WPS / PDF / image files to merge",
            [Txt.TipUp] = "Move the selected file one step up",
            [Txt.TipDown] = "Move the selected file one step down",
            [Txt.TipRemove] = "Remove from the list; the source file is kept",
            [Txt.TipClear] = "Clear the whole file list",
            [Txt.TipBrowse] = "Choose where to save the merged PDF",
            [Txt.TipList] = "Drop files here, or drag list items to reorder",
            [Txt.TipStart] = "Merge in list order and create a PDF",
            [Txt.TipSettings] = "Naming, appearance and post-merge behavior",
            [Txt.TipAbout] = "Version, changelog and user agreement",
            [Txt.TipCancelBox] = "Cancel box selection and restore the previous selection (Esc)",
            [Txt.TrayOpen] = "Open DocHub",
            [Txt.TrayExit] = "Exit",

            [Txt.DialogTitleAdd] = "Choose documents to merge",
            [Txt.FolderDesc] = "Choose where to save the merged PDF",
            [Txt.FilterSupport] = "Supported files",
            [Txt.FilterWord] = "Word / WPS documents",
            [Txt.FilterPdf] = "PDF files",
            [Txt.FilterImage] = "Image files",
            [Txt.FilterAll] = "All files",

            [Txt.StatusAdded] = "Added {0} file(s) · merged in list order",
            [Txt.StatusSkipped] = " ({0} duplicate or unsupported files skipped)",
            [Txt.StatusNothing] = "Nothing added: duplicates, missing or unsupported files",
            [Txt.StatusOrder] = "Order updated · merged in the current list order",
            [Txt.StatusRemoved] = "Removed from the list: {0}",
            [Txt.StatusRemovedMulti] = "Removed {0} items from the list",
            [Txt.StatusMovedTop] = "Moved to top · merged in list order",
            [Txt.StatusMovedBottom] = "Moved to bottom · merged in list order",
            [Txt.StatusCleared] = "List cleared",
            [Txt.StatusFolderChosen] = "Save location selected",

            [Txt.MsgClearTitle] = "Clear list",
            [Txt.MsgClearText] = "Clear the whole file list?\n\nSource files will not be deleted.",
            [Txt.MsgMergeBusy] = "A merge is in progress. Please wait until it finishes before closing.",
            [Txt.MsgNoWordTitle] = "Microsoft Word not found",
            [Txt.MsgNoWordText] = "Microsoft Word is required to convert Word documents.\n\nPlease install Microsoft Word; PDF and image files can still be merged.",
            [Txt.MsgOverwriteTitle] = "Overwrite?",
            [Txt.MsgOverwriteText] = "The output file already exists:\n{0}\n\nOverwrite it?",

            [Txt.StatusPreparing] = "Preparing…",
            [Txt.StatusProgress] = "{0}% · {1}",
            [Txt.StatusDone] = "Merge complete · {0} pages · {1}",
            [Txt.BalloonTitle] = "Merge complete",
            [Txt.BalloonText] = "{0} pages · {1}\n{2}",
            [Txt.OpenFailed] = "Could not open the PDF. Please check the save location manually.",
            [Txt.FailStatus] = "Merge failed. Check the files and try again.",
            [Txt.FailTitle] = "Merge failed",
            [Txt.FailPrefix] = "Merge failed: {0}",

            [Txt.ErrCannotStartTitle] = "Cannot start merging",
            [Txt.ErrNeedDocs] = "Please add documents to merge first.",
            [Txt.ErrNeedName] = "Please enter an output file name.",
            [Txt.ErrBadChars] = "The file name cannot contain: \\ / : * ? \" < > |",
            [Txt.ErrNeedFolder] = "Please choose a save location.",
            [Txt.ErrFolderMissing] = "Save location does not exist:\n{0}",
            [Txt.ErrPathInvalid] = "Invalid output path.",
            [Txt.ErrSameSource] = "The output file has the same name as a source document and would overwrite it.\n\nPlease change the file name or save location.",
            [Txt.ElapsedSeconds] = "{0:0.0} s",
            [Txt.ElapsedMinutes] = "{0} min {1} s",

            [Txt.SettingsTitle] = "Settings",
            [Txt.NamingSection] = "Output file name",
            [Txt.NamingHint] = "How the file name is generated after merging",
            [Txt.NamingFirst] = "Use the first file's name",
            [Txt.NamingPhrase] = "First file name + custom phrase",
            [Txt.NamingTime] = "Merge time (auto-generated)",
            [Txt.NamingCustom] = "Use a custom name",
            [Txt.PhrasePlaceholder] = "e.g. Final",
            [Txt.CustomPlaceholder] = "e.g. Contracts 2026",
            [Txt.PreviewFormat] = "Preview: {0}.pdf",
            [Txt.SkinSection] = "Appearance",
            [Txt.SkinHint] = "Pick a look — changes apply instantly",
            [Txt.DoneSection] = "When merging finishes",
            [Txt.DoneNotify] = "Show a notification in the taskbar corner",
            [Txt.DoneSound] = "Play a completion sound",
            [Txt.DoneOpen] = "Open the merged PDF automatically (off by default)",
            [Txt.SoundSection] = "Sound",
            [Txt.SoundWater] = "Water drop",
            [Txt.SoundDing] = "Ding-dong",
            [Txt.SoundQQ] = "QQ",
            [Txt.SoundChime] = "Chime",
            [Txt.SoundSoft] = "Soft",
            [Txt.SoundOff] = "Off",
            [Txt.SoundHint] = "Turn on the completion sound first, then choose a tone; selecting plays it",
            [Txt.LanguageSection] = "Language",
            [Txt.LanguageHint] = "Applies immediately",
            [Txt.QuoteSection] = "Daily quote",
            [Txt.QuoteHint] = "Follows the interface language",
            [Txt.QuoteToggle] = "Show a quote or movie line on the merge page",
            [Txt.QuoteAutoRefresh] = "Auto-rotate quotes",
            [Txt.QuoteRefreshInterval] = "Interval",
            [Txt.QuoteRefreshSeconds] = "sec",
            [Txt.QuoteRefreshHint] = "When enabled, a new quote appears at the chosen interval; when off, click the card to change it",
            [Txt.TipQuote] = "Click for another",
            [Txt.OutputSection] = "Save location",
            [Txt.OutputHint] = "Where the merged PDF is saved",
            [Txt.OutputFirst] = "Folder of the top file",
            [Txt.OutputLast] = "Last saved location",
            [Txt.OutputCustom] = "Custom folder",
            [Txt.OutputCustomPlaceholder] = "Choose or type a folder path",
            [Txt.SkinBlueName] = "Classic Blue",
            [Txt.SkinBlueDesc] = "Crisp and clear",
            [Txt.SkinNightName] = "Night",
            [Txt.SkinNightDesc] = "Immersive dark",
            [Txt.SkinCloudName] = "Cloud White",
            [Txt.SkinCloudDesc] = "Warm paper feel",
            [Txt.SkinSkeuoName] = "Skeuomorphic",
            [Txt.SkinSkeuoDesc] = "Leather and paper",
            [Txt.SkinNeumorphName] = "Neumorphic",
            [Txt.SkinNeumorphDesc] = "Soft depth",
            [Txt.SettingsLiveHint] = "All changes apply immediately",

            [Txt.AboutTitle] = "About",
            [Txt.VersionLine] = "{0} · Merge and convert Word, WPS, PDF and images",
            [Txt.AboutIntro] = "Everything runs locally. Files are never uploaded.",
            [Txt.BrandDefinition] = "DocHub: the document hub — merge and convert in one click",
            [Txt.Feature1] = "Merge any Word / WPS format, PDF and images in one batch, no size limit",
            [Txt.Feature2] = "Keeps original layout, page numbers, image size and orientation",
            [Txt.Feature3] = "Drag to reorder, live progress, full control over output order",
            [Txt.Feature4] = "Fully offline; files never leave this device",
            [Txt.ChangelogTitle] = "Changelog",
            [Txt.Changelog300] = "3.0.0 · 2026-11-03",
            [Txt.ChangelogItems300] =
                "#New\n" +
                "· The installer bundles the full .NET runtime, so PCs without .NET can install and run it\n" +
                "· Built-in simple PDF viewer for PCs with no PDF reader installed\n" +
                "· Author credit and contact details added to About (cado@163.com)\n" +
                "#Improved\n" +
                "· Settings now apply instantly — no Save button required\n" +
                "· Sounds play automatically when selected — no Preview button needed\n" +
                "· Faster merging: images convert in parallel and results are written straight to the target\n" +
                "· The installed app stays small; the runtime ships with the installer\n" +
                "#Fix\n" +
                "· Fixed the icon artwork sitting slightly above center; it is now vertically centered\n" +
                "· Fixed merging PDF/images when Word is absent: LibreOffice is supported, or Word files can be skipped",
            [Txt.Changelog200] = "2.0.0 · 2026-11-03",
            [Txt.ChangelogItems200] =
                "#New\n" +
                "· Merge JPG, PNG, BMP, TIFF and other images directly, keeping original size and orientation\n" +
                "· Word, PDF and images can be mixed in one list and exported as a single PDF\n" +
                "· Interface language defaults to the system language right after installation\n" +
                "#Improved\n" +
                "· Unified \"file\" wording across the app; the list badges now distinguish PDF / WORD / IMG\n" +
                "· Flat redesign of Settings: solid sections, thin dividers, no gradients or shadows\n" +
                "· Changelog grouped into New / Improved / Fixed for clearer reading\n" +
                "#Fix\n" +
                "· Fixed a false \"Word missing\" prompt when merging only PDF or image files",
            [Txt.Changelog130] = "1.3.0 · 2026-11-03",
            [Txt.ChangelogItems130] =
                "#New\n" +
                "· New save-location options: top file folder, last saved location, custom path\n" +
                "· File list now supports right-click delete, move up/down\n" +
                "· Ships as a Windows installer and an uncompressed portable package\n" +
                "#Improved\n" +
                "· Settings apply immediately after saving, no restart needed\n" +
                "· Progress bar follows the theme and shows a percentage\n" +
                "· Redesigned file list: card rows, type badges and file details\n" +
                "#Fix\n" +
                "· Fixed list flicker and jumping\n" +
                "· Fixed overwriting when names collide; files are numbered as A-1.pdf",
            [Txt.Changelog120] = "1.2.0 · 2026-11-03",
            [Txt.ChangelogItems120] =
                "#New\n" +
                "· New flat app icon, no lettering, cleaner silhouette\n" +
                "· 5 built-in completion sounds: water drop, ding-dong, QQ and more, with preview\n" +
                "· New User Agreement in About\n" +
                "· Traditional Chinese, English, Japanese, Russian and Korean interfaces, switch instantly\n" +
                "#Improved\n" +
                "· Polished interface details: corners, shadows, spacing and hover feedback",
            [Txt.Changelog110] = "1.1.0 · 2026-11-03",
            [Txt.ChangelogItems110] =
                "#New\n" +
                "· 4 naming modes: first file, first name + phrase, merge time, custom\n" +
                "· 4 NetEase-style skins: Classic Blue, NetEase Red, Night, Cloud White\n" +
                "#Improved\n" +
                "· New Settings and About entries; the icon switched to a flat style\n" +
                "· Does not open the PDF by default — shows a tray notification and plays a sound",
            [Txt.Changelog100] = "1.0.0 · 2026-11-02",
            [Txt.ChangelogItems100] =
                "#New\n" +
                "· First release: any Word format + PDF merge, batch import, drag reorder, progress",
            [Txt.ContactAuthor] = "Author: AI",
            [Txt.Copyright] = "© 2026 AI · Local processing, peace of mind",
            [Txt.EulaButton] = "User Agreement",
            [Txt.EulaTitle] = "User Agreement",
            [Txt.EulaIntro] = "Please read the following terms before using DocHub.",
            [Txt.EulaBody] =
                "1. Scope\n" +
                "DocHub is a local desktop tool for merging and converting Word, WPS, PDF and image files. It converts Word / WPS files " +
                "to PDF using the Microsoft Word, WPS Office or LibreOffice installed on your computer, lays out images at their original " +
                "size, then combines the pages in the order you choose.\n\n" +
                "2. Your files\n" +
                "All processing happens on this device. The app does not upload, transmit or back up any " +
                "document, and does not collect personal information or document content.\n\n" +
                "3. Responsibilities\n" +
                "You are responsible for ensuring that you have the right to use and merge the documents you " +
                "select. Please back up important files before merging. The author is not liable for any loss " +
                "caused by file corruption, overwriting or misuse.\n\n" +
                "4. Environment\n" +
                "Merging Word documents requires Microsoft Word or LibreOffice to be installed locally. " +
                "The app does not provide or activate Office.\n\n" +
                "5. No warranty\n" +
                "The software is provided \"as is\". Although we test carefully, we do not guarantee that every " +
                "document will merge perfectly in every environment.\n\n" +
                "6. Changes\n" +
                "These terms may be updated with new versions. Continued use means you accept the updated terms.",
            [Txt.EulaAgree] = "I have read and agree",

            [Txt.MergeNoDocs] = "Please add documents to merge first.",
            [Txt.MergeFileMissing] = "File missing or moved:\n{0}",
            [Txt.MergeWordMissing] =
                "Microsoft Word, WPS Office or LibreOffice was not found, so Word / WPS documents cannot be converted.\n\n" +
                "Install one of them first; PDF and image merges are unaffected.",
            [Txt.MergeWordStartFail] =
                "Could not start the Word / WPS conversion engine. Make sure it is installed and opens normally.",
            [Txt.MergeWordNotInstalled] =
                "Microsoft Word, WPS Office or LibreOffice was not found. Please install one of them and try again.",
            [Txt.MergeProcessing] = "Processing ({0}/{1}): {2}",
            [Txt.MergeStartingWord] = "Starting the Word / WPS conversion engine…",
            [Txt.MergeMergingPages] = "Merging pages…",
            [Txt.MergeMerging] = "Merging ({0}/{1}): {2}",
            [Txt.MergePdfUnreadable] =
                "Cannot read PDF \"{0}\": the file may be damaged, encrypted or unsupported.",
            [Txt.MergeNoPages] = "The documents contain no pages to merge.",
            [Txt.MergePdfError] = "An error occurred while merging PDFs. No output file was created.",
            [Txt.MergeWriteError] =
                "Cannot write the output file:\n{0}\n\nMake sure it is not open in another program and that " +
                "the location is writable.\n\nTechnical details: {1}",
            [Txt.MergeNoPermission] =
                "No permission to write the output file:\n{0}\n\nPlease choose another location and try again.",
            [Txt.MergeFailedGeneric] = "Merge failed: {0}",
            [Txt.MergeTempFail] = "Cannot create the temporary working folder.\n\nA security suite may be blocking file writes, or its permissions are broken.\nAdd this program to the allow-list, or run it as administrator, then try again.",
            [Txt.MergeDone] = "Merge complete",
            [Txt.MergeWordOpenFail] = "Cannot open \"{0}\".",
            [Txt.MergeWordConvertEmpty] = "\"{0}\" produced no valid pages after conversion.",
            [Txt.MergeWordConvertFail] =
                "Cannot convert Word / WPS document \"{0}\".\n\nMake sure it is not encrypted or damaged and that " +
                "Microsoft Word or WPS Office can open it normally.\n\nTechnical details: {1}",
            [Txt.MergePasswordTitle] = "Protected file",
            [Txt.MergePasswordHint] = "Enter the password that opens this file. It is used for this merge only.",
            [Txt.MergePasswordPlaceholder] = "Password",
            [Txt.MergePasswordWrong] = "Incorrect password. Please try again.",
            [Txt.MergeImageConvertFail] =
                "Cannot read image \"{0}\".\n\nMake sure the image is not damaged. " +
                "JPG / PNG / BMP / TIFF / GIF / WebP are supported directly; " +
                "HEIC / HEIF / AVIF need the matching decoder extension installed in Windows.\n\n" +
                "Technical details: {1}",

            [Txt.MsgSkipWordTitle] = "Word / WPS files cannot be converted",
            [Txt.MsgSkipWordText] =
                "Microsoft Word, WPS Office or LibreOffice was not found, so {0} Word / WPS file(s) cannot be converted.\n\n" +
                "Skip these Word / WPS files and merge the remaining PDF and image files?",
            [Txt.StatusSkippedWord] = "Skipped {0} Word / WPS file(s) — Word / WPS / LibreOffice not installed",

            [Txt.ViewResult] = "View merged PDF",
            [Txt.PdfViewerTitle] = "PDF viewer",
            [Txt.PrevPage] = "Previous",
            [Txt.NextPage] = "Next",
            [Txt.ZoomIn] = "Zoom in",
            [Txt.ZoomOut] = "Zoom out",
            [Txt.ViewerPageFormat] = "Page {0} of {1}",
            [Txt.ViewerNoReader] = "No PDF reader found on this PC — using the built-in viewer",
            [Txt.ViewerLoadFailed] = "Cannot open this PDF:\n{0}",

            [Txt.SettingsNavLook] = "Appearance",
            [Txt.SettingsNavNaming] = "Naming & Output",
            [Txt.SettingsNavDone] = "Notifications",
            [Txt.SettingsNavWatermark] = "Watermark",
            [Txt.WatermarkSection] = "Document watermark",
            [Txt.WatermarkHint] = "Choose which features get a watermark; one shared style, off by default",
            [Txt.WatermarkEnable] = "Add watermark",
            [Txt.WatermarkForMerge] = "Document merge",
            [Txt.WatermarkForPdfImage] = "PDF to images",
            [Txt.WatermarkForImagePdf] = "Images to PDF",
            [Txt.WatermarkForWordPdf] = "Word to PDF",
            [Txt.WatermarkPresets] = "Preset text",
            [Txt.WatermarkCustom] = "Custom",
            [Txt.WatermarkPreset1] = "ORIGINAL",
            [Txt.WatermarkPreset2] = "COPY",
            [Txt.WatermarkPreset3] = "CONFIDENTIAL",
            [Txt.WatermarkPreset4] = "SECRET",
            [Txt.WatermarkPreview] = "Preview",
            [Txt.WatermarkCustomPlaceholder] = "Enter watermark text",
            [Txt.WatermarkPreviewHint] = "Drag in the preview to move, scroll to resize, right-drag to rotate; values can also be typed.",
            [Txt.WatermarkTint] = "Color",
            [Txt.WatermarkTintGray] = "Gray",
            [Txt.WatermarkTintRed] = "Red",
            [Txt.WatermarkTintBlue] = "Blue",
            [Txt.WatermarkPosition] = "Position",
            [Txt.WatermarkPosTile] = "Tiled",
            [Txt.WatermarkPosTopLeft] = "Top left",
            [Txt.WatermarkPosTopRight] = "Top right",
            [Txt.WatermarkPosCenter] = "Centre",
            [Txt.WatermarkPosBottomLeft] = "Bottom left",
            [Txt.WatermarkPosBottomRight] = "Bottom right",
            [Txt.WatermarkOpacity] = "Opacity",
            [Txt.WatermarkScale] = "Size",
            [Txt.WatermarkRotation] = "Rotation",
            [Txt.WatermarkOffsetX] = "Horizontal offset",
            [Txt.WatermarkOffsetY] = "Vertical offset",
            [Txt.ToastTitle] = "Merge complete",
            [Txt.ToastOpen] = "View",
            [Txt.Changelog310] = "3.1.0 · 2026-11-03",
            [Txt.ChangelogItems310] =
                "#Added\n" +
                "· Settings rebuilt as a two-level layout: left navigation, right content\n" +
                "· New document watermark: built-in ORIGINAL / COPY / CONFIDENTIAL / SECRET, plus custom text, style and color\n" +
                "#Improved\n" +
                "· Install folder tidied: small launcher plus an app folder\n" +
                "· Completion notice now a themed bottom-right card\n",
            [Txt.Changelog400] = "4.0.0 · 2026-11-03",
            [Txt.ChangelogItems400] =
                "#Added\n" +
                "· File list supports multi-select batch actions, with one-click \"To top\" / \"To bottom\" in the context menu\n" +
                "#Improved\n" +
                "· Cleaner file list rows with a clearer selected state\n" +
                "· Layout reflows on window resize; the status line no longer covers the Start button\n" +
                "· Default output name follows the UI language when switching languages\n" +
                "· Restoring defaults also switches the language back to Simplified Chinese\n" +
                "· Installer now defaults to C:\\Program Files (x86)\\DocHub\n",
            [Txt.Changelog430] = "4.3.0 · 2026-11-04",
            [Txt.ChangelogItems430] =
                "#New\n" +
                "· Main-window navigation moved to the top-right: Appearance, Watermark, Notifications, Settings and About as flat icons that jump straight to their settings page\n" +
                "#Fixed\n" +
                "· Oversized or edge-dragged watermarks are no longer cut off: they auto-scale and stay inside the page, so merged documents show them in full\n" +
                "· The watermark preview no longer spills outside the preview frame\n" +
                "#Refined\n" +
                "· Re-synthesised the Water drop and QQ alert sounds with crisper attacks, more natural tails and consistent loudness\n" +
                "· Removed the left sidebar for a cleaner main window\n",
            [Txt.Changelog420] = "4.2.0 · 2026-11-04",
            [Txt.ChangelogItems420] =
                "#Improved\n" +
                "· Settings and About open instantly: the frosted backdrop is cached per skin and size and prepared in advance\n" +
                "· Watermark preview dragging is smooth: tiling now fills a pre-rendered texture, cutting the worst case from 190 ms to under 1 ms per frame\n" +
                "#Refined\n" +
                "· Sidebar redesigned: clearer icon and label hierarchy, with an accent pill and left bar for the selected item\n" +
                "· Version numbers now follow Semantic Versioning (SemVer): experience and performance work bumps the minor version\n",
            [Txt.Changelog412] = "4.1.2 · 2026-11-04",
            [Txt.ChangelogItems412] =
                "#New\n" +

                "· Watermark position presets: tiled / top left / top right / centre / bottom left / bottom right\n" +

                "#Improved\n" +

                "· Watermark settings no longer stutter: continuous drags commit once; preview drawing caches fonts and metrics\n" +

                "· The main window sidebar shows feature names (Merge / Settings / About) with accent-coloured selection\n",

            [Txt.Changelog411] = "4.1.1 · 2026-11-04",
            [Txt.ChangelogItems411] =
                "#Fixed\n" +
                "· The installer no longer forces the default folder: custom install paths are used as entered\n" +
                "· Fixed images failing to merge: WebP / HEIC etc. are decoded by the system codecs; plain images are more tolerant too\n" +
                "#Improved\n" +
                "· Watermark values can be typed in directly; drag to move, scroll to resize, right-drag to rotate in the preview\n" +
                "· Removed the style selector; the tiled look is now the single effect\n",

            [Txt.Changelog410] = "4.1.0 · 2026-11-04",
            [Txt.ChangelogItems410] =
                "#Added\n" +
                "· Watermark now supports any opacity, size, position and tilt angle, with a live preview\n" +
                "· Installer rebuilt as an in-house installer: installs into the per-user folder, needs no administrator rights and never writes to Program Files\n" +
                "· New \"Uninstall\" shortcut in the Start menu folder\n" +
                "#Improved\n" +
                "· Taller settings window; watermark controls are now sliders for quicker adjustment\n",

            [Txt.NavMergeTitle] = "Document merge",
            [Txt.NavMergeSubtitle] = "Merge to PDF",
            [Txt.NavConvertTitle] = "PDF to Images",
            [Txt.NavConvertSubtitle] = "Export pages as images",
            [Txt.MergePageTitle] = "Document merge",
            [Txt.MergePageSubtitle] = "Combine Word / WPS / PDF / images into one PDF in list order",
            [Txt.ConvertPageTitle] = "PDF to Images",
            [Txt.ConvertPageSubtitle] = "Export every PDF page as an image · source is read-only and never edited",
            [Txt.ConvertFilesTitle] = "PDF files",
            [Txt.ConvertCountFormat] = "{0} files",
            [Txt.ConvertListHint] = "Drag empty space to box-select · Page counts load automatically",
            [Txt.ConvertAdd] = "Add PDFs",
            [Txt.ConvertDialogTitle] = "Choose PDF files to convert",
            [Txt.ConvertEmptyTitle] = "Drop PDF files here",
            [Txt.ConvertEmptySubtitle] = "or click “Add PDFs” to select several",
            [Txt.ConvertEmptyFormats] = ".pdf",
            [Txt.ConvertPageCountFormat] = "{0} pages",
            [Txt.ConvertPageCountUnknown] = "Reading page count…",
            [Txt.ConvertPageCountFailed] = "Unreadable",
            [Txt.ConvertSettingsTitle] = "Export settings",
            [Txt.ConvertFormat] = "Image format",
            [Txt.ConvertPpi] = "Resolution (PPI)",
            [Txt.ConvertPpiCustom] = "Custom",
            [Txt.ConvertPpiHint] = "PPI controls sharpness, not the page's physical size",
            [Txt.ConvertQuality] = "JPEG quality",
            [Txt.ConvertBackground] = "Transparent background",
            [Txt.ConvertBackgroundHint] = "PNG only; other formats use a white background",
            [Txt.ConvertPageRange] = "Page range",
            [Txt.ConvertPageRangeAll] = "All",
            [Txt.ConvertPageRangePlaceholder] = "e.g. 1-3,5,8-10",
            [Txt.ConvertPageModeAll] = "All",
            [Txt.ConvertPageModeOdd] = "Odd",
            [Txt.ConvertPageModeEven] = "Even",
            [Txt.ConvertPageModeCustom] = "Custom",
            [Txt.ConvertSubfolder] = "Separate folder per PDF",
            [Txt.ConvertSubfolderHint] = "Forced on for batches; folder names include the page selection",
            [Txt.ConvertOutput] = "Save location",
            [Txt.ConvertNamePreview] = "Name preview: {0}",
            [Txt.ConvertEstimateFormat] = "About {0} × {1} px",
            [Txt.ConvertPreviewTitle] = "Page {0} preview",
            [Txt.ConvertPreviewEmpty] = "Select a PDF to preview the selected page",
            [Txt.ConvertStart] = "Start conversion",
            [Txt.ConvertRunning] = "Converting…",
            [Txt.ConvertCancel] = "Cancel",
            [Txt.ConvertCancelling] = "Cancelling…",
            [Txt.ConvertReady] = "Ready · images exported in list order",
            [Txt.ConvertProcessing] = "Converting ({0}/{1}): {2}",
            [Txt.ConvertPageStatus] = "Page {0} of {1}",
            [Txt.ConvertDone] = "Conversion complete · {0} images · {1}",
            [Txt.ConvertDonePartial] = "{0} done · {1} failed · {2} images",
            [Txt.ConvertCancelled] = "Cancelled · {0} images exported",
            [Txt.ConvertFailStatus] = "Conversion failed. Check the files and try again.",
            [Txt.ConvertFailTitle] = "Conversion failed",
            [Txt.ConvertOpenFolder] = "Open folder",
            [Txt.ConvertOutputSubfolder] = "Output: {0}",
            [Txt.ConvertNoFiles] = "Please add PDF files to convert first.",
            [Txt.ConvertNeedFolder] = "Please choose a save location.",
            [Txt.ConvertBadRange] = "Invalid page range. Use a format such as 1-3,5.",
            [Txt.ConvertRangeOut] = "The page range exceeds the document ({0} pages).",
            [Txt.ConvertNoPages] = "The document has no pages to convert.",
            [Txt.ConvertRangeNeeded] = "Enter a page range or choose All.",
            [Txt.ConvertNoMatchingPages] = "No pages match the current page selection.",
            [Txt.ConvertUnreadable] = "Cannot read PDF \"{0}\": the file may be damaged, encrypted or unsupported.",
            [Txt.ConvertPpiTooLarge] = "This PPI makes the page too large ({0} × {1} px). Lower the PPI.",
            [Txt.ConvertWriteError] = "Cannot write the image:\n{0}\n\nTechnical details: {1}",
            [Txt.ConvertMemory] = "Not enough memory to render this page. Lower the PPI and try again.",
            [Txt.ConvertSkippedNonPdf] = "Ignored {0} non-PDF file(s)",
            [Txt.ConvertDuplicate] = "Skipped {0} duplicate file(s)",
            [Txt.ConvertAdded] = "Added {0} PDF file(s)",
            [Txt.ConvertNothingAdded] = "Nothing added: duplicates, missing or non-PDF files",
            [Txt.MsgConvertBusy] = "A conversion is in progress. Please wait for it to finish before closing.",
            [Txt.SettingsNavConvert] = "PDF to Images",
            [Txt.ConvertSection] = "PDF to Images defaults",
            [Txt.ConvertDefaultsHint] = "The conversion page starts with these values; changes made there are remembered",
            [Txt.ConvertDefaultFormat] = "Default image format",
            [Txt.ConvertDefaultPpi] = "Default PPI",
            [Txt.ConvertDefaultQuality] = "Default JPEG quality",
            [Txt.ConvertDefaultBackground] = "Transparent background by default",
            [Txt.ConvertDefaultSubfolder] = "Separate folder per PDF by default",
            [Txt.ConvertWatermarkNote] = "Watermark is drawn on output images only; the source PDF stays read-only. Enable it on the Watermark page",
            [Txt.NamingHintShared] = "Shared output naming for merging and PDF to Images",
            [Txt.DoneOpenShared] = "Open the result automatically (PDF for merge, folder for conversion)",
            [Txt.TipNavMerge] = "Document merge: combine files into one PDF",
            [Txt.TipNavConvert] = "PDF to Images: export every PDF page as an image",
            [Txt.TipAppearance] = "Open appearance settings",
            [Txt.TipNotifications] = "Open completion notification settings",
            [Txt.TipWatermark] = "Open watermark settings",
            [Txt.NavImagePdfTitle] = "Images to PDF",
            [Txt.NavImagePdfSubtitle] = "Images one by one or merged to PDF",
            [Txt.ImagePdfPageTitle] = "Images to PDF",
            [Txt.ImagePdfPageSubtitle] = "Export images separately or merge them into one PDF · processed locally",
            [Txt.ImagePdfFilesTitle] = "Image files",
            [Txt.ImagePdfCountFormat] = "{0} files",
            [Txt.ImagePdfListHint] = "Drag empty space to box-select · Converted in list order",
            [Txt.ImagePdfAdd] = "Add images",
            [Txt.ImagePdfDialogTitle] = "Choose images to convert",
            [Txt.ImagePdfEmptyTitle] = "Drop images here",
            [Txt.ImagePdfEmptySubtitle] = "or click \"Add images\" to select several",
            [Txt.ImagePdfEmptyFormats] = ".jpg .jpeg .png .bmp .tif .tiff .gif .webp",
            [Txt.ImagePdfSettingsTitle] = "PDF settings",
            [Txt.ImagePdfPageSize] = "Page size",
            [Txt.ImagePdfPageSizeAuto] = "Auto",
            [Txt.ImagePdfOrientation] = "Orientation",
            [Txt.ImagePdfOrientationAuto] = "Auto",
            [Txt.ImagePdfOrientationPortrait] = "Portrait",
            [Txt.ImagePdfOrientationLandscape] = "Landscape",
            [Txt.ImagePdfMargin] = "Margin",
            [Txt.ImagePdfMarginNone] = "None",
            [Txt.ImagePdfMarginSmall] = "10 mm",
            [Txt.ImagePdfMarginLarge] = "20 mm",
            [Txt.ImagePdfFitHint] = "Auto matches the page to the image\nFixed sizes scale to fit without cropping",
            [Txt.ImagePdfMergeOption] = "Merge into one PDF",
            [Txt.ImagePdfNameLabel] = "File name",
            [Txt.ImagePdfNamePlaceholderMerge] = "Blank = shared merge naming",
            [Txt.ImagePdfNamePlaceholderSeparate] = "Blank = original image name",
            [Txt.ImagePdfNamePreview] = "Name preview: {0}",
            [Txt.ImagePdfNamePreviewMulti] = "{0} and {1} PDFs total",
            [Txt.ImagePdfPageCountFormat] = "{0} images",
            [Txt.ImagePdfOutputCount] = "{0} PDFs",
            [Txt.ImagePdfPreviewTitle] = "Preview",
            [Txt.ImagePdfPreviewEmpty] = "Select an image to see a preview",
            [Txt.ImagePdfStart] = "Start conversion",
            [Txt.ImagePdfRunning] = "Converting…",
            [Txt.ImagePdfCancel] = "Cancel",
            [Txt.ImagePdfCancelling] = "Cancelling…",
            [Txt.ImagePdfReady] = "Ready · each image becomes its own PDF",
            [Txt.ImagePdfReadyMerge] = "Ready · images are merged in list order",
            [Txt.ImagePdfProcessing] = "Converting ({0}/{1}): {2}",
            [Txt.ImagePdfDone] = "Done · {0} pages · {1}",
            [Txt.ImagePdfDoneFiles] = "Done · {0} PDFs · {1}",
            [Txt.ImagePdfDonePartial] = "Done · {0} succeeded, {1} failed · {2}",
            [Txt.ImagePdfCancelled] = "Cancelled · {0} images processed",
            [Txt.ImagePdfFailStatus] = "Conversion failed. Check the images and try again",
            [Txt.ImagePdfFailTitle] = "Conversion failed",
            [Txt.ImagePdfNoFiles] = "Add images to convert first.",
            [Txt.ImagePdfNeedFolder] = "Choose where to save.",
            [Txt.ImagePdfUnreadable] = "Cannot read image \"{0}\": the file may be damaged or in an unsupported format.",
            [Txt.ImagePdfWriteError] = "Cannot write the PDF:\n{0}\n\nTechnical details: {1}",
            [Txt.ImagePdfFailGeneric] = "Cannot create the PDF: {0}",
            [Txt.ImagePdfAdded] = "Added {0} image(s)",
            [Txt.ImagePdfSkippedNonImage] = "Ignored {0} non-image file(s)",
            [Txt.ImagePdfNothingAdded] = "Nothing added: duplicate, missing or non-image files",
            [Txt.MsgImagePdfBusy] = "Converting images. Wait for it to finish before closing the window.",
            [Txt.TipNavImagePdf] = "Images to PDF: separate PDFs by default, or merge into one",
            [Txt.Feature5] = "Export PDF pages to PNG / JPEG / BMP / TIFF; defaults to JPEG at 300 PPI, quality 100%",
            [Txt.Feature6] = "Batch images to PDF: separate files by default, optional merge, custom file names",
            [Txt.Changelog452] = "4.5.2 · 2026-11-05",
            [Txt.ChangelogItems452] =
                "#New\n" +
                "· Images to PDF now creates one PDF per image by default\n" +
                "· Added a \"Merge into one PDF\" switch to restore combined output\n" +
                "· Output file names are customizable; separate files get _001, _002 suffixes\n" +
                "#Improved\n" +
                "· In separate mode a failed image no longer stops the batch; successes and failures are summarised\n" +
                "#Fixed\n" +
                "· PDF to Images now reads the source PDF only; its text, images and pages stay unchanged\n",
            [Txt.NavWordPdfTitle] = "Word to PDF",
            [Txt.NavWordPdfSubtitle] = "Batch convert Word / WPS to PDF",
            [Txt.WordPdfPageTitle] = "Word to PDF",
            [Txt.WordPdfPageSubtitle] = "Convert Word / WPS separately or merge into one PDF · Local processing",
            [Txt.WordPdfFilesTitle] = "Word / WPS files",
            [Txt.WordPdfCountFormat] = "{0} files",
            [Txt.WordPdfListHint] = "Drag on empty space to marquee · converted in list order",
            [Txt.WordPdfAdd] = "Add Word",
            [Txt.WordPdfDialogTitle] = "Select Word / WPS documents to convert",
            [Txt.WordPdfEmptyTitle] = "Drop Word / WPS files here",
            [Txt.WordPdfEmptySubtitle] = "or click Add Word to select multiple files",
            [Txt.WordPdfEmptyFormats] = ".doc .docx .docm .dot .dotx .dotm .rtf .odt .wps .wpt",
            [Txt.WordPdfSettingsTitle] = "Conversion settings",
            [Txt.WordPdfMergeOption] = "Merge into one file",
            [Txt.WordPdfMergeHint] = "When off, each Word file is saved as a PDF with the same name",
            [Txt.WordPdfNameLabel] = "File name",
            [Txt.WordPdfNamePlaceholder] = "Leave empty to use the first Word file name",
            [Txt.WordPdfSeparateHint] = "PDF names match the Word files; duplicates are numbered",
            [Txt.WordPdfOutput] = "Save to",
            [Txt.WordPdfNamePreview] = "Output: {0}",
            [Txt.WordPdfNamePreviewMulti] = "{0} PDFs with the same names as the Word files",
            [Txt.WordPdfStart] = "Start conversion",
            [Txt.WordPdfRunning] = "Converting…",
            [Txt.WordPdfCancel] = "Cancel",
            [Txt.WordPdfCancelling] = "Cancelling…",
            [Txt.WordPdfReady] = "Ready · each Word file becomes its own PDF",
            [Txt.WordPdfReadyMerge] = "Ready · merged into one PDF in list order",
            [Txt.WordPdfEngineWord] = "Microsoft Word",
            [Txt.WordPdfEngineWps] = "WPS Office",
            [Txt.WordPdfEngineLibre] = "LibreOffice",
            [Txt.WordPdfProcessing] = "Converting ({0}/{1}): {2}",
            [Txt.WordPdfMerging] = "Merging PDF pages…",
            [Txt.WordPdfDone] = "Done · {0} pages · {1}",
            [Txt.WordPdfDoneFiles] = "Done · {0} PDFs · {1}",
            [Txt.WordPdfCancelled] = "Cancelled · {0} completed",
            [Txt.WordPdfFailStatus] = "Conversion failed. Check the documents and try again.",
            [Txt.WordPdfFailTitle] = "Conversion failed",
            [Txt.WordPdfNoFiles] = "Add Word documents to convert first.",
            [Txt.WordPdfNeedFolder] = "Choose a save location.",
            [Txt.WordPdfNoEngine] = "Microsoft Word, WPS Office or LibreOffice is required to convert Word / WPS files.",
            [Txt.WordPdfNonWord] = "Not a supported Word / WPS document: {0}",
            [Txt.WordPdfWriteError] = "Could not write the PDF: {0}",
            [Txt.WordPdfFailGeneric] = "Could not convert the Word document: {0}",
            [Txt.WordPdfAdded] = "Added {0} document(s)",
            [Txt.WordPdfSkippedNonWord] = "Ignored {0} non-Word file(s)",
            [Txt.WordPdfNothingAdded] = "Nothing added: files are duplicates, missing, or not Word documents",
            [Txt.WordPdfOutputCount] = "{0} PDFs",
            [Txt.MsgWordPdfBusy] = "Word conversion is running. Please wait before closing the window.",
            [Txt.TipNavWordPdf] = "Word / WPS to PDF: save each file separately or merge into one",
            [Txt.Feature7] = "Batch Word / WPS to PDF with original file names by default, optional single-file merge and output watermark",
            [Txt.Changelog4523] = "4.5.23 · 2026-11-10",
            [Txt.ChangelogItems4523] =
                "# Added\n" +
                "· Document merge now supports WPS Writer formats (.wps, .wpt), mixable with Word, PDF and images\n" +
                "· Merge and convert pages now mention WPS; WPS-only batches automatically pick an available engine\n",
            [Txt.Changelog4522] = "4.5.22 · 2026-11-10",
            [Txt.ChangelogItems4522] =
                "# Added\n" +
                "· Word to PDF now supports WPS Writer formats (.wps, .wpt)\n" +
                "· WPS files are converted by WPS Office when available; Word documents still use Microsoft Word\n",
            [Txt.Changelog4521] = "4.5.21 · 2026-11-10",
            [Txt.ChangelogItems4521] =
                "# Maintenance\n" +
                "· Removed the extra credit from the About page and program metadata; the author is AI\n" +
                "· Updated the repository address to GitHub (github.com/swbths/DocHub)\n",
            [Txt.Changelog4520] = "4.5.20 · 2026-11-10",
            [Txt.ChangelogItems4520] =
                "# Improved\n" +
                "· Daily quotes now play randomly at startup and while the app runs instead of rotating in order, and never repeat back-to-back\n" +
                "· The auto-rotate interval is now in seconds, defaulting to 30 and customizable from 5 to 86400 seconds\n",
            [Txt.Changelog4519] = "4.5.19 · 2026-11-10",
            [Txt.ChangelogItems4519] =
                "# Improved\n" +
                "· All in-app dialogs now follow the active skin: confirmations, warnings, errors and questions use its colours, typography, corners and buttons\n" +
                "· The password prompt now uses the same borderless skin window, so dark skins no longer show a white system title bar\n",
            [Txt.Changelog4518] = "4.5.18 · 2026-11-10",
            [Txt.ChangelogItems4518] =
                "# New\n" +
                "· Added a Word to PDF workspace for .doc/.docx/.docm/.dot/.dotx/.dotm/.rtf/.odt batch conversion\n" +
                "· Each Word file is exported as a PDF with its original name by default; duplicates are numbered, with an optional merge into one file\n" +
                "· Added a Word to PDF target in Watermark settings that only affects output PDFs, never the source files\n" +
                "· Reuses Microsoft Word or LibreOffice automatically and shows a clear hint when neither is available\n",
            [Txt.Changelog4517] = "4.5.17 · 2026-11-06",
            [Txt.ChangelogItems4517] =
                "# Improved\n" +
                "· Removed classical Chinese poetry and literary Chinese from the quote library and replaced them with modern quotes\n" +
                "# Added\n" +
                "· Added classic movie lines and famous quotes from China and abroad: 66 per UI language, 396 in total\n" +
                "· Includes lines from Forrest Gump, The Shawshank Redemption, The Godfather, Titanic, Nezha and The Grandmaster\n" +
                "· Adds quotes from Steve Jobs, Nelson Mandela, Gandhi, Martin Luther King Jr., Francis Bacon and Anton Chekhov\n",
            [Txt.Changelog4516] = "4.5.16 · 2026-11-06",
            [Txt.ChangelogItems4516] =
                "# Improved\n" +
                "· The daily quote card now uses a frosted-glass look: translucent fill, top sheen and rounded outline that match the glass theme\n" +
                "· Removed the red vertical bar, so the night skin no longer shows a harsh red accent\n" +
                "· Long quotes automatically shrink their font size and stay fully visible\n" +
                "# Added\n" +
                "· Quote library expanded to 348 entries (58 per UI language), covering world figures and classic movie lines\n",
            [Txt.Changelog4515] = "4.5.15 · 2026-11-06",
            [Txt.ChangelogItems4515] =
                "#Added\n" +
                "· Daily quotes can now rotate automatically, with a custom interval under Appearance (default: 5 minutes)\n" +
                "· When auto-rotate is off, click the card to change the quote instantly\n",
            [Txt.Changelog4514] = "4.5.14 · 2026-11-06",
            [Txt.ChangelogItems4514] =
                "#Fixed\n" +
                "· The Settings window no longer turns into a wall of red X marks or raises an unhandled exception after switching the UI language\n" +
                "· The glass backdrop cache now rebuilds itself if its bitmaps are released by mistake, preventing the same problem from recurring\n",
            [Txt.Changelog4513] = "4.5.13 · 2026-11-06",
            [Txt.ChangelogItems4513] =
                "#Added\n" +
                "· A \"Daily quote\" card on the merge page shows famous quotes and movie lines with their sources\n" +
                "· Six quote collections (简体中文, 繁體中文, English, 日本語, 한국어, Русский) that follow the interface language\n" +
                "· A new \"Show a quote or movie line on the merge page\" toggle under Appearance; click the card for another quote\n",
            [Txt.Changelog4512] = "4.5.12 · 2026-11-06",
            [Txt.ChangelogItems4512] =
                "#Changed\n" +
                "· Watermark and Notifications moved to the bottom-left sidebar next to Skins, Settings and About\n" +
                "· The hint text on the merge page now sits directly below the progress bar\n" +
                "#Fixed\n" +
                "· Removed the outdated \"merge only\" note from the watermark tooltip\n",
            [Txt.Changelog4511] = "4.5.11 · 2026-11-06",
            [Txt.ChangelogItems4511] =
                "#Changed\n" +
                "· Removed the Terminal skin; configs using it fall back to Classic Blue automatically\n" +
                "#Fixed\n" +
                "· Fixed the top-right watermark button icon being clipped at 19 px\n",
            [Txt.Changelog4510] = "4.5.10 · 2026-11-06",
            [Txt.ChangelogItems4510] =
                "#Fixed\n" +
                "· A plain left click (including Ctrl+click) keeps only the current file, so the previous highlight disappears immediately\n" +
                "· Only box selection and Shift-click preserve multi-select\n",
            [Txt.Changelog459] = "4.5.9 · 2026-11-06",
            [Txt.ChangelogItems459] =
                "#Fixed\n" +
                "· A plain left click now keeps only the clicked file selected instead of leaving the previous row highlighted; the row Up / Down / Remove buttons behave the same\n" +
                "· Ctrl / Shift multi-select is unchanged\n",
            [Txt.Changelog458] = "4.5.8 · 2026-11-06",
            [Txt.ChangelogItems458] =
                "#New\n" +
                "· The skin entry moved to the lower-left sidebar, above Settings; Skins, Settings and About now share flat line icons\n" +
                "· File lists have a Cancel Selection button that restores the selection from before the box drag; Esc works too\n" +
                "· Hover tooltips added to Delete, Clear, Cancel Selection and other list buttons\n" +
                "#Improved\n" +
                "· Fixed the list jumping while dragging a marquee selection with the left mouse button\n" +
                "#Changed\n" +
                "· Removed the HUD Sci-Fi skin; saved settings fall back to Classic Blue\n",
            [Txt.Changelog457] = "4.5.7 · 2026-11-06",
            [Txt.ChangelogItems457] =
                "#New\n" +
                "· File lists support marquee selection: drag on empty space to select many files, in all three workspaces\n" +
                "· Hold Ctrl while marquee-selecting to add files to the current selection\n" +
                "· Four new skins: Skeuomorphic, Neumorphic, Terminal and HUD Sci-Fi\n" +
                "#Improved\n" +
                "· Delete removes all selected files at once\n" +
                "· Removed the NetEase Red skin; saved settings fall back to Classic Blue\n",
            [Txt.Changelog456] = "4.5.6 · 2026-11-06",
            [Txt.ChangelogItems456] =
                "#Improved\n" +
                "· The default output subfolder for PDF to images now includes the page selection, such as \"report_Even\" or \"report_All\", so repeated runs are easy to tell apart\n",
            [Txt.Changelog455] = "4.5.5 · 2026-11-06",
            [Txt.ChangelogItems455] =
                "#New\n" +
                "· PDF to images can export all pages, odd pages, even pages or a custom page range\n" +
                "· Custom ranges accept formats such as 1-3,5,8-10 and keep the original page numbers in file names\n" +
                "#Improved\n" +
                "· The preview and name example now follow the first page selected by the current mode\n" +
                "· Page ranges saved by older versions are migrated to Custom automatically\n",
            [Txt.Changelog454] = "4.5.4 · 2026-11-05",
            [Txt.ChangelogItems454] =
                "#New\n" +
                "· New flat brand icon: blue document stack with an amber merge badge\n" +
                "· Functional icons now use a flat two-tone style for a more consistent, legible look at small sizes\n" +
                "#Fixed\n" +
                "· Switching language now refreshes the Settings window in place instead of closing and re-centering it\n",
            [Txt.Changelog453] = "4.5.3 · 2026-11-05",
            [Txt.ChangelogItems453] =
                "#New\n" +
                "· Watermark settings now support Document merge, PDF to images and Images to PDF separately; all three switches default to off\n" +
                "· PDF to images can stamp watermarks on output images while the source PDF stays read-only\n" +
                "#Improved\n" +
                "· PDF to images now defaults to 300 PPI, JPEG format and quality 100%\n" +
                "· Completion sound is off by default; sound options are greyed out until it is turned on\n",
            [Txt.Changelog451] = "4.5.1 · 2026-11-05",
            [Txt.ChangelogItems451] =
                "#New\n" +
                "· New Images to PDF workspace: combine images into one PDF in list order\n" +
                "· Page size A4 / A3 / A5 / B5 / Letter or match image, with orientation and margins\n" +
                "· Naming, save location and \"open when done\" reuse the document merge settings\n" +
                "#Improved\n" +
                "· Removed the Qoder dark skin\n" +
                "#Fixed\n" +
                "· Sidebar text contrast on the Classic Blue and NetEase Red skins\n",
            [Txt.Changelog440] = "4.4.0 · 2026-11-05",
            [Txt.ChangelogItems440] =
                "#New\n" +
                "· Card-style left navigation with Document merge and PDF to Images workspaces, switchable with Ctrl+1 / Ctrl+2\n" +
                "· New PDF to Images: export PDF pages to PNG / JPEG / BMP / TIFF with format, PPI, quality and page range\n" +
                "· Batch conversion, live preview, cancel and per-file failure summary; images keep the selected DPI metadata\n" +
                "· New Qoder Dark skin, used by default for new installs\n" +
                "#Improved\n" +
                "· Interface rebuilt in the Qoder style: card navigation, page headers, one accent color and lower-contrast borders\n" +
                "· Merge and conversion share naming, save location and completion settings; conversion keeps its own last folder\n" +
                "· Output name and save folder can be edited directly; folder drops now scan recursively\n" +
                "· New PDF to Images page in Settings; the watermark page notes that it only affects merging\n",

            [Txt.Changelog450] = "4.5.0 · 2026-11-05",
            [Txt.ChangelogItems450] =
                "#New\n" +
                "· Renamed to DocHub (文枢): the document hub — merge and convert in one click\n" +
                "· The About page now shows the brand definition and the PDF-to-image capability\n" +
                "#Improved\n" +
                "· Left navigation renamed from Document convert to PDF to Images, matching the page title and settings\n" +
                "#Fixed\n" +
                "· The Add PDF button is no longer covered by the hint text on the conversion page\n" +
                "· The file counter no longer covers the remove / clear buttons\n" +
                "· Existing settings are migrated automatically when upgrading\n",

            [Txt.DefaultDocName] = "Merged document",
            [Txt.DefaultConvertName] = "Converted images",
            [Txt.MergeTimePrefix] = "Merged_",
            [Txt.ConvertTimePrefix] = "Converted_",
            [Txt.MergeCollideSuffix] = "_merged"
        };
    }
}
