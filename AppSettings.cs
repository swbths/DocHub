using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocHub
{
    public enum NamingMode
    {
        FirstDocument = 0,
        FirstPlusPhrase = 1,
        MergeTime = 2,
        Custom = 3
    }

    public enum OutputLocationMode
    {
        FirstDocument = 0,
        LastUsed = 1,
        Custom = 2
    }

    public enum WatermarkTint
    {
        Gray = 0,
        Red = 1,
        Blue = 2
    }

    /// <summary>水印摆放位置：平铺整页，或固定在页面的某个角/正中。</summary>
    public enum WatermarkAnchor
    {
        Tile = 0,
        TopLeft = 1,
        TopRight = 2,
        Center = 3,
        BottomLeft = 4,
        BottomRight = 5
    }

    /// <summary>PDF 转图片的输出格式。</summary>
    public enum ConvertImageFormat
    {
        Png = 0,
        Jpeg = 1,
        Bmp = 2,
        Tiff = 3
    }

    /// <summary>PDF 转图片的页码选择方式。</summary>
    public enum ConvertPageSelection
    {
        All = 0,
        Odd = 1,
        Even = 2,
        Custom = 3
    }

    /// <summary>图片转 PDF 的纸张大小；自适应表示页面跟随图片比例。</summary>
    public enum ImagePdfPageSize
    {
        Auto = 0,
        A4 = 1,
        A3 = 2,
        A5 = 3,
        B5 = 4,
        Letter = 5
    }

    /// <summary>图片转 PDF 的纸张方向；自动表示跟随图片横竖。</summary>
    public enum ImagePdfOrientation
    {
        Auto = 0,
        Portrait = 1,
        Landscape = 2
    }

    public sealed class AppSettings
    {
        public const int CurrentSchemaVersion = 2;

        /// <summary>设置结构版本；旧配置在读取时按新版默认值迁移一次。</summary>
        public int SchemaVersion { get; set; }
        public NamingMode NamingMode { get; set; } = NamingMode.FirstDocument;
        public string NamePhrase { get; set; } = string.Empty;
        public string CustomName { get; set; } = Loc.T(Txt.DefaultDocName);
        public string SkinId { get; set; } = SkinPalette.ClassicBlueId;
        public bool ShowNotification { get; set; } = true;
        public bool PlaySound { get; set; } = false;
        public string SoundId { get; set; } = SoundLibrary.DingId;
        public Language Language { get; set; } = Loc.DetectSystem();
        public bool OpenAfterMerge { get; set; } = false;
        public OutputLocationMode OutputLocationMode { get; set; } = OutputLocationMode.FirstDocument;
        public string CustomOutputFolder { get; set; } = string.Empty;
        public string LastOutputFolder { get; set; } = string.Empty;

        /// <summary>是否为「文档合并」的输出添加水印。</summary>
        public bool WatermarkEnabled { get; set; } = false;

        /// <summary>是否为「PDF 转图片」的输出图片添加水印（不改动源 PDF）。</summary>
        public bool WatermarkForPdfImage { get; set; } = false;

        /// <summary>是否为「图片转 PDF」的输出 PDF 添加水印。</summary>
        public bool WatermarkForImagePdf { get; set; } = false;
        public int WatermarkPreset { get; set; } = 0;
        public string WatermarkCustomText { get; set; } = string.Empty;
        public WatermarkTint WatermarkTint { get; set; } = WatermarkTint.Gray;

        /// <summary>水印位置：默认平铺整页，可改为左上/右上/正中/左下/右下。</summary>
        public WatermarkAnchor WatermarkAnchor { get; set; } = WatermarkAnchor.Tile;

        /// <summary>水印不透明度，0~100（百分比）。</summary>
        public int WatermarkOpacity { get; set; } = 35;

        /// <summary>水印大小，20~300（百分比，100 为基准尺寸）。</summary>
        public int WatermarkScale { get; set; } = 100;

        /// <summary>水印倾斜方向，-180~180 度。</summary>
        public int WatermarkRotation { get; set; } = 30;

        /// <summary>水印水平位置偏移，-100~100（页面宽度的百分比）。</summary>
        public int WatermarkOffsetX { get; set; } = 0;

        /// <summary>水印垂直位置偏移，-100~100（页面高度的百分比）。</summary>
        public int WatermarkOffsetY { get; set; } = 0;

        // ---------- PDF 转图片 ----------
        public ConvertImageFormat ConvertFormat { get; set; } = ConvertImageFormat.Jpeg;

        /// <summary>输出像素密度，36~1200；只影响清晰度，不改变页面物理尺寸。</summary>
        public int ConvertPpi { get; set; } = 300;

        /// <summary>JPEG 编码质量，1~100。</summary>
        public int ConvertJpegQuality { get; set; } = 100;

        /// <summary>PNG 是否保留透明背景；其它格式始终使用白底。</summary>
        public bool ConvertTransparentBackground { get; set; }

        /// <summary>是否为每个 PDF 单独建立输出文件夹；批量转换时强制开启。</summary>
        public bool ConvertCreateSubfolder { get; set; } = true;

        /// <summary>页码选择方式；自定义时使用 ConvertPageRange。</summary>
        public ConvertPageSelection ConvertPageMode { get; set; } = ConvertPageSelection.All;

        /// <summary>自定义页码范围；支持 1-3,5,8-10。</summary>
        public string ConvertPageRange { get; set; } = string.Empty;

        /// <summary>转换专用的上次保存位置，避免与合并互相覆盖。</summary>
        public string LastConvertFolder { get; set; } = string.Empty;

        // ---------- 图片转 PDF ----------
        /// <summary>纸张大小；默认 A4，自适应时页面与图片同比例。</summary>
        public ImagePdfPageSize ImagePdfPageSize { get; set; } = ImagePdfPageSize.A4;

        /// <summary>纸张方向；自动时纵向图片用纵向页面、横向图片用横向页面。</summary>
        public ImagePdfOrientation ImagePdfOrientation { get; set; } = ImagePdfOrientation.Auto;

        /// <summary>页面四边留白（毫米），0 表示铺满可打印区域。</summary>
        public int ImagePdfMarginMm { get; set; } = 10;

        /// <summary>是否合并为一个 PDF；关闭时每张图片单独输出（默认）。</summary>
        public bool ImagePdfMergeIntoOne { get; set; }

        /// <summary>自定义输出文件名；为空时合并使用共享命名，逐张输出使用图片原名。</summary>
        public string ImagePdfCustomName { get; set; } = string.Empty;

        /// <summary>三个水印目标中是否有任一开启；关闭时水印样式控件整体置灰。</summary>
        [JsonIgnore]
        public bool WatermarkAnyEnabled =>
            WatermarkEnabled || WatermarkForPdfImage || WatermarkForImagePdf;

        /// <summary>按当前水印样式生成参数；自定义文字为空时返回 null。</summary>
        public WatermarkSpec? CreateWatermarkSpec()
        {
            string text = ResolveWatermarkText();
            return text.Length == 0 ? null : new WatermarkSpec(
                text,
                WatermarkTint,
                WatermarkOpacity,
                WatermarkScale,
                WatermarkRotation,
                WatermarkOffsetX,
                WatermarkOffsetY,
                WatermarkAnchor);
        }

        private string ResolveWatermarkText()
        {
            switch (WatermarkPreset)
            {
                case 1: return Loc.T(Txt.WatermarkPreset2);
                case 2: return Loc.T(Txt.WatermarkPreset3);
                case 3: return Loc.T(Txt.WatermarkPreset4);
                case 4: return (WatermarkCustomText ?? string.Empty).Trim();
                default: return Loc.T(Txt.WatermarkPreset1);
            }
        }

        [JsonIgnore]
        public static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DocHub", "settings.json");

        /// <summary>旧版（合了么 / HeLeMe）配置目录；升级后首次启动时迁移，避免用户设置丢失。</summary>
        private static string LegacyFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HeLeMe", "settings.json");

        private static void MigrateLegacySettings()
        {
            try
            {
                if (File.Exists(FilePath) || !File.Exists(LegacyFilePath)) return;
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.Copy(LegacyFilePath, FilePath);
            }
            catch
            {
                // 迁移失败时按默认设置启动，不影响正常使用
            }
        }

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static AppSettings Load()
        {
            try
            {
                MigrateLegacySettings();
                if (!File.Exists(FilePath)) return new AppSettings();
                string json = File.ReadAllText(FilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions);
                if (settings == null) return new AppSettings();
                if (!SkinPalette.Exists(settings.SkinId))
                    settings.SkinId = SkinPalette.ClassicBlueId;
                if (!SoundLibrary.Exists(settings.SoundId))
                    settings.SoundId = SoundLibrary.DingId;
                if (!Enum.IsDefined(settings.Language))
                    settings.Language = Loc.DetectSystem();
                if (!Enum.IsDefined(settings.OutputLocationMode))
                    settings.OutputLocationMode = OutputLocationMode.FirstDocument;
                if (!Enum.IsDefined(settings.WatermarkTint))
                    settings.WatermarkTint = WatermarkTint.Gray;
                if (!Enum.IsDefined(settings.WatermarkAnchor))
                    settings.WatermarkAnchor = WatermarkAnchor.Tile;
                if (settings.WatermarkPreset < 0 || settings.WatermarkPreset > 4)
                    settings.WatermarkPreset = 0;
                settings.WatermarkOpacity = Math.Clamp(settings.WatermarkOpacity, 0, 100);
                settings.WatermarkScale = Math.Clamp(settings.WatermarkScale, 10, 400);
                settings.WatermarkRotation = Math.Clamp(settings.WatermarkRotation, -180, 180);
                settings.WatermarkOffsetX = Math.Clamp(settings.WatermarkOffsetX, -100, 100);
                settings.WatermarkOffsetY = Math.Clamp(settings.WatermarkOffsetY, -100, 100);
                if (!Enum.IsDefined(settings.ConvertFormat))
                    settings.ConvertFormat = ConvertImageFormat.Jpeg;
                if (!Enum.IsDefined(settings.ConvertPageMode))
                    settings.ConvertPageMode = ConvertPageSelection.All;
                settings.ConvertPpi = Math.Clamp(settings.ConvertPpi, 36, 1200);
                settings.ConvertJpegQuality = Math.Clamp(settings.ConvertJpegQuality, 1, 100);
                settings.ConvertPageRange ??= string.Empty;
                settings.LastConvertFolder ??= string.Empty;
                if (!Enum.IsDefined(settings.ImagePdfPageSize))
                    settings.ImagePdfPageSize = ImagePdfPageSize.A4;
                if (!Enum.IsDefined(settings.ImagePdfOrientation))
                    settings.ImagePdfOrientation = ImagePdfOrientation.Auto;
                settings.ImagePdfMarginMm = Math.Clamp(settings.ImagePdfMarginMm, 0, 30);
                settings.ImagePdfCustomName ??= string.Empty;
                if (settings.ImagePdfCustomName.Length > 80)
                    settings.ImagePdfCustomName = settings.ImagePdfCustomName[..80];

                int version = settings.SchemaVersion;

                // v4.5.3：旧默认值（PNG/150PPI/质量90、提示音开启）迁移到新默认值；
                // 用户自定义过的其他组合保持不变，提示音只迁移一次以免覆盖之后的手动开启。
                if (version < 1)
                {
                    if (settings.ConvertFormat == ConvertImageFormat.Png &&
                        settings.ConvertPpi == 150 &&
                        settings.ConvertJpegQuality == 90)
                    {
                        settings.ConvertFormat = ConvertImageFormat.Jpeg;
                        settings.ConvertPpi = 300;
                        settings.ConvertJpegQuality = 100;
                    }

                    settings.PlaySound = false;
                }

                // v4.5.5：旧版只有自定义范围输入框；已填写范围时迁移为「自定义」模式。
                if (version < 2)
                {
                    if (settings.ConvertPageMode == ConvertPageSelection.All &&
                        !string.IsNullOrWhiteSpace(settings.ConvertPageRange))
                        settings.ConvertPageMode = ConvertPageSelection.Custom;
                }

                if (version < CurrentSchemaVersion)
                {
                    settings.SchemaVersion = CurrentSchemaVersion;
                    settings.Save();
                }

                return settings;
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save()
        {
            SchemaVersion = CurrentSchemaVersion;
            try
            {
                string folder = Path.GetDirectoryName(FilePath)!;
                Directory.CreateDirectory(folder);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this, SerializerOptions));
            }
            catch
            {
                // 设置写盘失败时不影响正常使用
            }
        }

        public AppSettings Clone()
        {
            return JsonSerializer.Deserialize<AppSettings>(
                JsonSerializer.Serialize(this, SerializerOptions), SerializerOptions) ?? new AppSettings();
        }

        public void CopyFrom(AppSettings other)
        {
            SchemaVersion = other.SchemaVersion;
            NamingMode = other.NamingMode;
            NamePhrase = other.NamePhrase;
            CustomName = other.CustomName;
            SkinId = other.SkinId;
            ShowNotification = other.ShowNotification;
            PlaySound = other.PlaySound;
            SoundId = other.SoundId;
            Language = other.Language;
            OpenAfterMerge = other.OpenAfterMerge;
            OutputLocationMode = other.OutputLocationMode;
            CustomOutputFolder = other.CustomOutputFolder;
            LastOutputFolder = other.LastOutputFolder;
            WatermarkEnabled = other.WatermarkEnabled;
            WatermarkForPdfImage = other.WatermarkForPdfImage;
            WatermarkForImagePdf = other.WatermarkForImagePdf;
            WatermarkPreset = other.WatermarkPreset;
            WatermarkCustomText = other.WatermarkCustomText;
            WatermarkTint = other.WatermarkTint;
            WatermarkAnchor = other.WatermarkAnchor;
            WatermarkOpacity = other.WatermarkOpacity;
            WatermarkScale = other.WatermarkScale;
            WatermarkRotation = other.WatermarkRotation;
            WatermarkOffsetX = other.WatermarkOffsetX;
            WatermarkOffsetY = other.WatermarkOffsetY;
            ConvertFormat = other.ConvertFormat;
            ConvertPpi = other.ConvertPpi;
            ConvertJpegQuality = other.ConvertJpegQuality;
            ConvertTransparentBackground = other.ConvertTransparentBackground;
            ConvertCreateSubfolder = other.ConvertCreateSubfolder;
            ConvertPageMode = other.ConvertPageMode;
            ConvertPageRange = other.ConvertPageRange;
            LastConvertFolder = other.LastConvertFolder;
            ImagePdfPageSize = other.ImagePdfPageSize;
            ImagePdfOrientation = other.ImagePdfOrientation;
            ImagePdfMarginMm = other.ImagePdfMarginMm;
            ImagePdfMergeIntoOne = other.ImagePdfMergeIntoOne;
            ImagePdfCustomName = other.ImagePdfCustomName;
        }

        public void ResetToDefaults()
        {
            SchemaVersion = CurrentSchemaVersion;
            NamingMode = NamingMode.FirstDocument;
            NamePhrase = string.Empty;
            // 恢复默认时界面语言固定回到简体中文，默认输出名也同步为中文
            Language = Language.ZhCn;
            CustomName = Loc.T(Language.ZhCn, Txt.DefaultDocName);
            SkinId = SkinPalette.ClassicBlueId;
            ShowNotification = true;
            PlaySound = false;
            SoundId = SoundLibrary.DingId;
            OpenAfterMerge = false;
            OutputLocationMode = OutputLocationMode.FirstDocument;
            CustomOutputFolder = string.Empty;
            LastOutputFolder = string.Empty;
            WatermarkEnabled = false;
            WatermarkForPdfImage = false;
            WatermarkForImagePdf = false;
            WatermarkPreset = 0;
            WatermarkCustomText = string.Empty;
            WatermarkTint = WatermarkTint.Gray;
            WatermarkAnchor = WatermarkAnchor.Tile;
            WatermarkOpacity = 35;
            WatermarkScale = 100;
            WatermarkRotation = 30;
            WatermarkOffsetX = 0;
            WatermarkOffsetY = 0;
            ConvertFormat = ConvertImageFormat.Jpeg;
            ConvertPpi = 300;
            ConvertJpegQuality = 100;
            ConvertTransparentBackground = false;
            ConvertCreateSubfolder = true;
            ConvertPageMode = ConvertPageSelection.All;
            ConvertPageRange = string.Empty;
            LastConvertFolder = string.Empty;
            ImagePdfPageSize = ImagePdfPageSize.A4;
            ImagePdfOrientation = ImagePdfOrientation.Auto;
            ImagePdfMarginMm = 10;
            ImagePdfMergeIntoOne = false;
            ImagePdfCustomName = string.Empty;
        }

        public string BuildNameStem(string? firstDocumentPath, DateTime now, bool forConversion = false)
        {
            string documentStem = Loc.T(forConversion ? Txt.DefaultConvertName : Txt.DefaultDocName);
            if (!string.IsNullOrWhiteSpace(firstDocumentPath))
            {
                string name = Path.GetFileNameWithoutExtension(firstDocumentPath);
                if (!string.IsNullOrWhiteSpace(name)) documentStem = name;
            }

            switch (NamingMode)
            {
                case NamingMode.FirstPlusPhrase:
                    string phrase = (NamePhrase ?? string.Empty).Trim();
                    return phrase.Length == 0 ? documentStem : documentStem + "_" + phrase;
                case NamingMode.MergeTime:
                    return Loc.T(forConversion ? Txt.ConvertTimePrefix : Txt.MergeTimePrefix)
                        + now.ToString("yyyyMMdd_HHmmss");
                case NamingMode.Custom:
                    string custom = (CustomName ?? string.Empty).Trim();
                    if (custom.Length == 0) return documentStem;
                    // 共享的自定义名若仍是任一语言的默认值，转换场景改用转换专用默认名
                    if (forConversion && IsDefaultName(custom)) return documentStem;
                    return custom;
                default:
                    return documentStem;
            }
        }

        /// <summary>判断文本是否为某一语言的默认输出名（含空值，合并与转换都算）。</summary>
        public static bool IsDefaultName(string? text)
        {
            string trimmed = (text ?? string.Empty).Trim();
            if (trimmed.Length == 0) return true;
            foreach (Language language in Loc.All)
            {
                if (string.Equals(trimmed, Loc.T(language, Txt.DefaultDocName),
                    StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(trimmed, Loc.T(language, Txt.DefaultConvertName),
                    StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>切换界面语言时，把仍是默认名称的输出名同步成新语言。</summary>
        public void SyncDefaultName(Language language)
        {
            if (NamingMode == NamingMode.Custom && IsDefaultName(CustomName))
                CustomName = Loc.T(language, Txt.DefaultDocName);
        }
    }

    /// <summary>
    /// 一套完整配色：经典蓝、网易红、夜间模式与云白。
    /// </summary>
    public sealed class SkinPalette
    {
        public const string ClassicBlueId = "blue";
        public const string NetEaseRedId = "netease";
        public const string NightId = "night";
        public const string CloudWhiteId = "cloud";

        public string Id { get; init; } = ClassicBlueId;
        public string Name { get; init; } = "经典蓝";
        public string Description { get; init; } = string.Empty;
        public bool IsDark { get; init; }
        public bool HeaderOnLight { get; init; }

        public Color Background { get; init; }
        public Color Card { get; init; }
        public Color Border { get; init; }
        public Color TextPrimary { get; init; }
        public Color TextSecondary { get; init; }
        public Color TextMuted { get; init; }

        public Color Accent { get; init; }
        public Color AccentHover { get; init; }
        public Color AccentPressed { get; init; }
        public Color AccentLight { get; init; }
        public Color AccentText { get; init; }

        public Color InputFill { get; init; }
        public Color InputReadOnlyFill { get; init; }
        public Color ListHover { get; init; }
        public Color ListSelected { get; init; }
        public Color Separator { get; init; }

        public Color SecondaryFill { get; init; }
        public Color SecondaryHover { get; init; }
        public Color SecondaryPressed { get; init; }
        public Color SecondaryText { get; init; }
        public Color PrimaryDisabledFill { get; init; }
        public Color PrimaryDisabledText { get; init; }
        public Color SecondaryDisabledText { get; init; }
        public Color SecondaryDisabledBorder { get; init; }

        public Color HeaderStart { get; init; }
        public Color HeaderEnd { get; init; }
        public Color HeaderText { get; init; }
        public Color HeaderSubtitle { get; init; }
        public Color HeaderBadgeFill { get; init; }
        public Color HeaderBadgeBorder { get; init; }
        public Color HeaderBadgeText { get; init; }
        public Color HeaderButtonFill { get; init; }
        public Color HeaderButtonHover { get; init; }
        public Color HeaderButtonBorder { get; init; }
        public Color LogoStart { get; init; }
        public Color LogoEnd { get; init; }
        public Color LogoText { get; init; }

        public Color ProgressTrack { get; init; }
        public Color ProgressStart { get; init; }

        public Color PdfBadgeFill { get; init; }
        public Color PdfBadgeText { get; init; }
        public Color WordBadgeFill { get; init; }
        public Color WordBadgeText { get; init; }
        public Color ImageBadgeFill { get; init; }
        public Color ImageBadgeText { get; init; }
        public Color NumberFill { get; init; }
        public Color NumberText { get; init; }
        public Color DragDots { get; init; }
        public Color EmptyBorder { get; init; }
        public Color EmptyIconBack { get; init; }
        public Color EmptyIconBorder { get; init; }
        public Color EmptyText { get; init; }

        // ---------- 磨砂玻璃 ----------
        /// <summary>窗口壁纸的渐变起止色（清晰层，只在卡片之间的空隙可见）。</summary>
        public Color WallpaperStart { get; init; }
        public Color WallpaperEnd { get; init; }
        /// <summary>壁纸上的三团柔光，决定玻璃里透出的颜色。</summary>
        public Color WallpaperGlowA { get; init; }
        public Color WallpaperGlowB { get; init; }
        public Color WallpaperGlowC { get; init; }

        /// <summary>玻璃面板填充（半透明，叠在模糊壁纸上）。</summary>
        public Color GlassFill { get; init; }
        /// <summary>更实的玻璃填充，用于侧栏等需要压暗的区域。</summary>
        public Color GlassStrong { get; init; }
        /// <summary>玻璃外描边。</summary>
        public Color GlassBorder { get; init; }
        /// <summary>玻璃边缘高光。</summary>
        public Color GlassEdge { get; init; }
        /// <summary>叠在磨砂层上的整体色调。</summary>
        public Color GlassTint { get; init; }
        /// <summary>玻璃上的悬停底色。</summary>
        public Color GlassHover { get; init; }
        /// <summary>输入框等原生控件使用的不透明近似色。</summary>
        public Color GlassInput { get; init; }
        /// <summary>左侧导航栏的压暗层。</summary>
        public Color RailFill { get; init; }

        public static readonly SkinPalette ClassicBlue = new()
        {
            Id = ClassicBlueId,
            Name = "经典蓝",
            Description = "清爽通透",
            Background = Color.FromArgb(244, 246, 250),
            Card = Color.White,
            Border = Color.FromArgb(228, 233, 241),
            TextPrimary = Color.FromArgb(28, 37, 52),
            TextSecondary = Color.FromArgb(107, 119, 137),
            TextMuted = Color.FromArgb(143, 154, 170),
            Accent = Color.FromArgb(47, 107, 255),
            AccentHover = Color.FromArgb(36, 92, 232),
            AccentPressed = Color.FromArgb(28, 79, 214),
            AccentLight = Color.FromArgb(234, 242, 255),
            AccentText = Color.White,
            InputFill = Color.White,
            InputReadOnlyFill = Color.FromArgb(249, 250, 252),
            ListHover = Color.FromArgb(247, 250, 255),
            ListSelected = Color.FromArgb(237, 244, 255),
            Separator = Color.FromArgb(241, 244, 249),
            SecondaryFill = Color.White,
            SecondaryHover = Color.FromArgb(244, 248, 254),
            SecondaryPressed = Color.FromArgb(235, 243, 255),
            SecondaryText = Color.FromArgb(61, 74, 96),
            PrimaryDisabledFill = Color.FromArgb(191, 210, 247),
            PrimaryDisabledText = Color.White,
            SecondaryDisabledText = Color.FromArgb(183, 193, 208),
            SecondaryDisabledBorder = Color.FromArgb(238, 242, 247),
            HeaderStart = Color.FromArgb(23, 38, 61),
            HeaderEnd = Color.FromArgb(32, 57, 91),
            HeaderText = Color.White,
            HeaderSubtitle = Color.FromArgb(188, 205, 228),
            HeaderBadgeFill = Color.FromArgb(16, 255, 255, 255),
            HeaderBadgeBorder = Color.FromArgb(52, 255, 255, 255),
            HeaderBadgeText = Color.FromArgb(205, 221, 240),
            HeaderButtonFill = Color.FromArgb(30, 255, 255, 255),
            HeaderButtonHover = Color.FromArgb(52, 255, 255, 255),
            HeaderButtonBorder = Color.FromArgb(48, 255, 255, 255),
            LogoStart = Color.FromArgb(94, 158, 255),
            LogoEnd = Color.FromArgb(40, 98, 240),
            LogoText = Color.White,
            ProgressTrack = Color.FromArgb(232, 237, 245),
            ProgressStart = Color.FromArgb(84, 146, 255),
            PdfBadgeFill = Color.FromArgb(255, 236, 238),
            PdfBadgeText = Color.FromArgb(214, 48, 61),
            WordBadgeFill = Color.FromArgb(235, 243, 255),
            WordBadgeText = Color.FromArgb(37, 99, 235),
            ImageBadgeFill = Color.FromArgb(230, 246, 238),
            ImageBadgeText = Color.FromArgb(22, 128, 74),
            NumberFill = Color.FromArgb(237, 243, 255),
            NumberText = Color.FromArgb(78, 110, 172),
            DragDots = Color.FromArgb(199, 208, 221),
            EmptyBorder = Color.FromArgb(214, 223, 235),
            EmptyIconBack = Color.FromArgb(240, 245, 253),
            EmptyIconBorder = Color.FromArgb(203, 218, 240),
            EmptyText = Color.FromArgb(172, 182, 196),

            WallpaperStart = Color.FromArgb(243, 246, 252),
            WallpaperEnd = Color.FromArgb(228, 237, 251),
            WallpaperGlowA = Color.FromArgb(72, 47, 107, 255),
            WallpaperGlowB = Color.FromArgb(48, 122, 90, 250),
            WallpaperGlowC = Color.FromArgb(42, 45, 180, 200),
            GlassFill = Color.FromArgb(150, 255, 255, 255),
            GlassStrong = Color.FromArgb(206, 255, 255, 255),
            GlassBorder = Color.FromArgb(64, 23, 43, 77),
            GlassEdge = Color.FromArgb(196, 255, 255, 255),
            GlassTint = Color.FromArgb(12, 240, 246, 255),
            GlassHover = Color.FromArgb(26, 23, 43, 77),
            GlassInput = Color.FromArgb(255, 251, 252, 254),
            RailFill = Color.FromArgb(120, 255, 255, 255)
        };

        public static readonly SkinPalette NetEaseRed = new()
        {
            Id = NetEaseRedId,
            Name = "网易红",
            Description = "经典云音乐",
            Background = Color.FromArgb(245, 245, 247),
            Card = Color.White,
            Border = Color.FromArgb(231, 231, 234),
            TextPrimary = Color.FromArgb(31, 31, 35),
            TextSecondary = Color.FromArgb(106, 106, 112),
            TextMuted = Color.FromArgb(150, 150, 156),
            Accent = Color.FromArgb(211, 58, 49),
            AccentHover = Color.FromArgb(190, 47, 39),
            AccentPressed = Color.FromArgb(165, 37, 30),
            AccentLight = Color.FromArgb(253, 237, 236),
            AccentText = Color.White,
            InputFill = Color.White,
            InputReadOnlyFill = Color.FromArgb(248, 248, 249),
            ListHover = Color.FromArgb(255, 248, 247),
            ListSelected = Color.FromArgb(253, 236, 234),
            Separator = Color.FromArgb(243, 241, 241),
            SecondaryFill = Color.White,
            SecondaryHover = Color.FromArgb(251, 246, 245),
            SecondaryPressed = Color.FromArgb(247, 237, 236),
            SecondaryText = Color.FromArgb(61, 56, 54),
            PrimaryDisabledFill = Color.FromArgb(237, 181, 177),
            PrimaryDisabledText = Color.White,
            SecondaryDisabledText = Color.FromArgb(189, 182, 180),
            SecondaryDisabledBorder = Color.FromArgb(241, 235, 234),
            HeaderStart = Color.FromArgb(201, 48, 42),
            HeaderEnd = Color.FromArgb(140, 27, 22),
            HeaderText = Color.White,
            HeaderSubtitle = Color.FromArgb(246, 206, 202),
            HeaderBadgeFill = Color.FromArgb(20, 255, 255, 255),
            HeaderBadgeBorder = Color.FromArgb(56, 255, 255, 255),
            HeaderBadgeText = Color.FromArgb(248, 221, 218),
            HeaderButtonFill = Color.FromArgb(26, 255, 255, 255),
            HeaderButtonHover = Color.FromArgb(48, 255, 255, 255),
            HeaderButtonBorder = Color.FromArgb(46, 255, 255, 255),
            LogoStart = Color.FromArgb(242, 92, 82),
            LogoEnd = Color.FromArgb(193, 39, 45),
            LogoText = Color.FromArgb(255, 243, 230),
            ProgressTrack = Color.FromArgb(239, 237, 237),
            ProgressStart = Color.FromArgb(231, 97, 88),
            PdfBadgeFill = Color.FromArgb(253, 236, 235),
            PdfBadgeText = Color.FromArgb(179, 38, 30),
            WordBadgeFill = Color.FromArgb(253, 237, 236),
            WordBadgeText = Color.FromArgb(201, 48, 42),
            ImageBadgeFill = Color.FromArgb(232, 245, 236),
            ImageBadgeText = Color.FromArgb(22, 130, 72),
            NumberFill = Color.FromArgb(253, 236, 235),
            NumberText = Color.FromArgb(179, 38, 30),
            DragDots = Color.FromArgb(211, 203, 203),
            EmptyBorder = Color.FromArgb(227, 225, 227),
            EmptyIconBack = Color.FromArgb(251, 243, 242),
            EmptyIconBorder = Color.FromArgb(234, 213, 211),
            EmptyText = Color.FromArgb(175, 167, 165),

            WallpaperStart = Color.FromArgb(252, 245, 244),
            WallpaperEnd = Color.FromArgb(243, 227, 225),
            WallpaperGlowA = Color.FromArgb(66, 211, 58, 49),
            WallpaperGlowB = Color.FromArgb(46, 230, 140, 90),
            WallpaperGlowC = Color.FromArgb(38, 120, 90, 250),
            GlassFill = Color.FromArgb(150, 255, 255, 255),
            GlassStrong = Color.FromArgb(206, 255, 252, 251),
            GlassBorder = Color.FromArgb(58, 70, 40, 36),
            GlassEdge = Color.FromArgb(196, 255, 255, 255),
            GlassTint = Color.FromArgb(10, 255, 244, 242),
            GlassHover = Color.FromArgb(24, 70, 40, 36),
            GlassInput = Color.FromArgb(255, 255, 252, 251),
            RailFill = Color.FromArgb(122, 255, 250, 249)
        };

        public static readonly SkinPalette Night = new()
        {
            Id = NightId,
            Name = "夜间模式",
            Description = "沉浸暗色",
            IsDark = true,
            Background = Color.FromArgb(18, 19, 22),
            Card = Color.FromArgb(30, 31, 36),
            Border = Color.FromArgb(46, 48, 56),
            TextPrimary = Color.FromArgb(232, 233, 236),
            TextSecondary = Color.FromArgb(167, 170, 178),
            TextMuted = Color.FromArgb(123, 126, 135),
            Accent = Color.FromArgb(233, 67, 58),
            AccentHover = Color.FromArgb(240, 90, 82),
            AccentPressed = Color.FromArgb(201, 50, 43),
            AccentLight = Color.FromArgb(51, 37, 42),
            AccentText = Color.White,
            InputFill = Color.FromArgb(38, 39, 45),
            InputReadOnlyFill = Color.FromArgb(26, 27, 32),
            ListHover = Color.FromArgb(38, 39, 45),
            ListSelected = Color.FromArgb(51, 37, 40),
            Separator = Color.FromArgb(41, 43, 49),
            SecondaryFill = Color.FromArgb(38, 39, 45),
            SecondaryHover = Color.FromArgb(46, 47, 54),
            SecondaryPressed = Color.FromArgb(53, 55, 63),
            SecondaryText = Color.FromArgb(201, 203, 210),
            PrimaryDisabledFill = Color.FromArgb(74, 48, 51),
            PrimaryDisabledText = Color.FromArgb(158, 142, 142),
            SecondaryDisabledText = Color.FromArgb(105, 108, 116),
            SecondaryDisabledBorder = Color.FromArgb(42, 44, 51),
            HeaderStart = Color.FromArgb(27, 28, 33),
            HeaderEnd = Color.FromArgb(38, 40, 47),
            HeaderText = Color.FromArgb(240, 241, 243),
            HeaderSubtitle = Color.FromArgb(148, 151, 160),
            HeaderBadgeFill = Color.FromArgb(26, 255, 255, 255),
            HeaderBadgeBorder = Color.FromArgb(38, 255, 255, 255),
            HeaderBadgeText = Color.FromArgb(174, 177, 185),
            HeaderButtonFill = Color.FromArgb(24, 255, 255, 255),
            HeaderButtonHover = Color.FromArgb(43, 255, 255, 255),
            HeaderButtonBorder = Color.FromArgb(38, 255, 255, 255),
            LogoStart = Color.FromArgb(233, 67, 58),
            LogoEnd = Color.FromArgb(165, 42, 36),
            LogoText = Color.White,
            ProgressTrack = Color.FromArgb(42, 44, 51),
            ProgressStart = Color.FromArgb(239, 95, 86),
            PdfBadgeFill = Color.FromArgb(58, 36, 39),
            PdfBadgeText = Color.FromArgb(255, 138, 128),
            WordBadgeFill = Color.FromArgb(52, 39, 43),
            WordBadgeText = Color.FromArgb(255, 158, 149),
            ImageBadgeFill = Color.FromArgb(35, 52, 44),
            ImageBadgeText = Color.FromArgb(126, 214, 164),
            NumberFill = Color.FromArgb(45, 46, 53),
            NumberText = Color.FromArgb(201, 203, 210),
            DragDots = Color.FromArgb(84, 87, 97),
            EmptyBorder = Color.FromArgb(52, 54, 61),
            EmptyIconBack = Color.FromArgb(38, 39, 45),
            EmptyIconBorder = Color.FromArgb(58, 60, 68),
            EmptyText = Color.FromArgb(108, 111, 120),

            WallpaperStart = Color.FromArgb(7, 9, 14),
            WallpaperEnd = Color.FromArgb(17, 26, 43),
            WallpaperGlowA = Color.FromArgb(118, 70, 110, 240),
            WallpaperGlowB = Color.FromArgb(72, 130, 80, 240),
            WallpaperGlowC = Color.FromArgb(56, 40, 190, 180),
            GlassFill = Color.FromArgb(142, 18, 22, 31),
            GlassStrong = Color.FromArgb(196, 16, 20, 28),
            GlassBorder = Color.FromArgb(40, 255, 255, 255),
            GlassEdge = Color.FromArgb(28, 255, 255, 255),
            GlassTint = Color.FromArgb(0, 6, 8, 12),
            GlassHover = Color.FromArgb(30, 255, 255, 255),
            GlassInput = Color.FromArgb(255, 12, 16, 23),
            RailFill = Color.FromArgb(150, 8, 10, 15)
        };

        public static readonly SkinPalette CloudWhite = new()
        {
            Id = CloudWhiteId,
            Name = "云白",
            Description = "暖白纸感",
            HeaderOnLight = true,
            Background = Color.FromArgb(251, 250, 248),
            Card = Color.White,
            Border = Color.FromArgb(239, 233, 227),
            TextPrimary = Color.FromArgb(44, 38, 34),
            TextSecondary = Color.FromArgb(122, 114, 107),
            TextMuted = Color.FromArgb(163, 155, 147),
            Accent = Color.FromArgb(201, 48, 42),
            AccentHover = Color.FromArgb(179, 42, 36),
            AccentPressed = Color.FromArgb(153, 34, 29),
            AccentLight = Color.FromArgb(252, 241, 239),
            AccentText = Color.White,
            InputFill = Color.White,
            InputReadOnlyFill = Color.FromArgb(247, 244, 240),
            ListHover = Color.FromArgb(253, 249, 246),
            ListSelected = Color.FromArgb(251, 240, 237),
            Separator = Color.FromArgb(242, 237, 232),
            SecondaryFill = Color.White,
            SecondaryHover = Color.FromArgb(251, 247, 243),
            SecondaryPressed = Color.FromArgb(246, 239, 233),
            SecondaryText = Color.FromArgb(74, 64, 58),
            PrimaryDisabledFill = Color.FromArgb(232, 196, 192),
            PrimaryDisabledText = Color.White,
            SecondaryDisabledText = Color.FromArgb(191, 182, 174),
            SecondaryDisabledBorder = Color.FromArgb(241, 234, 227),
            HeaderStart = Color.FromArgb(243, 237, 230),
            HeaderEnd = Color.FromArgb(228, 215, 202),
            HeaderText = Color.FromArgb(59, 46, 38),
            HeaderSubtitle = Color.FromArgb(125, 107, 96),
            HeaderBadgeFill = Color.FromArgb(20, 120, 90, 70),
            HeaderBadgeBorder = Color.FromArgb(42, 120, 90, 70),
            HeaderBadgeText = Color.FromArgb(110, 90, 78),
            HeaderButtonFill = Color.FromArgb(18, 60, 40, 30),
            HeaderButtonHover = Color.FromArgb(32, 60, 40, 30),
            HeaderButtonBorder = Color.FromArgb(26, 60, 40, 30),
            LogoStart = Color.FromArgb(217, 83, 71),
            LogoEnd = Color.FromArgb(176, 40, 31),
            LogoText = Color.FromArgb(255, 254, 248),
            ProgressTrack = Color.FromArgb(239, 234, 228),
            ProgressStart = Color.FromArgb(217, 83, 71),
            PdfBadgeFill = Color.FromArgb(251, 236, 234),
            PdfBadgeText = Color.FromArgb(179, 38, 30),
            WordBadgeFill = Color.FromArgb(251, 237, 234),
            WordBadgeText = Color.FromArgb(201, 48, 42),
            ImageBadgeFill = Color.FromArgb(236, 246, 238),
            ImageBadgeText = Color.FromArgb(24, 130, 76),
            NumberFill = Color.FromArgb(251, 240, 237),
            NumberText = Color.FromArgb(179, 38, 30),
            DragDots = Color.FromArgb(204, 193, 184),
            EmptyBorder = Color.FromArgb(230, 222, 213),
            EmptyIconBack = Color.FromArgb(250, 243, 236),
            EmptyIconBorder = Color.FromArgb(228, 208, 190),
            EmptyText = Color.FromArgb(176, 166, 156),

            WallpaperStart = Color.FromArgb(251, 248, 242),
            WallpaperEnd = Color.FromArgb(242, 233, 220),
            WallpaperGlowA = Color.FromArgb(72, 201, 48, 42),
            WallpaperGlowB = Color.FromArgb(52, 230, 160, 80),
            WallpaperGlowC = Color.FromArgb(38, 120, 90, 250),
            GlassFill = Color.FromArgb(155, 255, 253, 248),
            GlassStrong = Color.FromArgb(210, 255, 253, 248),
            GlassBorder = Color.FromArgb(58, 60, 45, 35),
            GlassEdge = Color.FromArgb(196, 255, 255, 255),
            GlassTint = Color.FromArgb(8, 255, 250, 240),
            GlassHover = Color.FromArgb(26, 60, 45, 35),
            GlassInput = Color.FromArgb(255, 255, 252, 246),
            RailFill = Color.FromArgb(120, 255, 252, 245)
        };

        public static readonly IReadOnlyList<SkinPalette> All = new[]
        {
            ClassicBlue, NetEaseRed, Night, CloudWhite
        };

        public static bool Exists(string? id)
        {
            return All.Any(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        public static SkinPalette FromId(string? id)
        {
            return All.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase))
                   ?? ClassicBlue;
        }
    }
}
