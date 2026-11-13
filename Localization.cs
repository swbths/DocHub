using System;
using System.Collections.Generic;
using System.Globalization;

namespace DocHub
{
    public enum Language
    {
        ZhCn = 0,
        ZhTw = 1,
        En = 2,
        Ja = 3,
        Ru = 4,
        Ko = 5
    }

    public enum Txt
    {
        AppName, Tagline, LocalBadge,
        Ok, Close, Cancel, Yes, No, Save, Reset, Browse, PreviewWord, Settings, About, Skin, RailMerge,

        DocsTitle, DocCountFormat, ListHint,
        AddDocs, MoveUp, MoveDown, MoveTop, MoveBottom, Remove, Clear,
        OutputFile, SaveTo, ChooseFolder, StartMerge, MergingButton, ReadyStatus,
        NamePlaceholderFirst, NamePlaceholderPhrase, NamePlaceholderTime, NamePlaceholderCustom,
        EmptyTitle, EmptySubtitle, EmptyFormats,
        TipAdd, TipUp, TipDown, TipRemove, TipClear, TipBrowse, TipList, TipStart, TipSettings, TipAbout,
        TipCancelBox,
        TrayOpen, TrayExit,
        DialogTitleAdd, FolderDesc, FilterSupport, FilterWord, FilterPdf, FilterImage, FilterAll,
        StatusAdded, StatusSkipped, StatusNothing,
        StatusOrder, StatusRemoved, StatusRemovedMulti,
        StatusMovedTop, StatusMovedBottom, StatusCleared, StatusFolderChosen,
        MsgClearTitle, MsgClearText, MsgMergeBusy, MsgNoWordTitle, MsgNoWordText,
        MsgOverwriteTitle, MsgOverwriteText,
        StatusPreparing, StatusProgress, StatusDone,
        BalloonTitle, BalloonText, OpenFailed, FailStatus, FailTitle, FailPrefix,
        ErrCannotStartTitle, ErrNeedDocs, ErrNeedName, ErrBadChars, ErrNeedFolder,
        ErrFolderMissing, ErrPathInvalid, ErrSameSource,
        ElapsedSeconds, ElapsedMinutes,

        SettingsTitle, NamingSection, NamingHint, NamingFirst, NamingPhrase, NamingTime, NamingCustom,
        PhrasePlaceholder, CustomPlaceholder, PreviewFormat,
        SkinSection, SkinHint, DoneSection, DoneNotify, DoneSound, DoneOpen,
        SoundSection, SoundWater, SoundDing, SoundQQ, SoundChime, SoundSoft, SoundOff, SoundHint,
        LanguageSection, LanguageHint,
        QuoteSection, QuoteHint, QuoteToggle, QuoteAutoRefresh, QuoteRefreshInterval,
        QuoteRefreshSeconds, QuoteRefreshHint, TipQuote,
        OutputSection, OutputHint, OutputFirst, OutputLast, OutputCustom, OutputCustomPlaceholder,
        SkinBlueName, SkinBlueDesc,
        SkinNightName, SkinNightDesc, SkinCloudName, SkinCloudDesc,
        SkinSkeuoName, SkinSkeuoDesc, SkinNeumorphName, SkinNeumorphDesc,
        SettingsLiveHint,

        AboutTitle, VersionLine, AboutIntro, BrandDefinition,
        Feature1, Feature2, Feature3, Feature4,
        ContactAuthor,
        ChangelogTitle, Changelog300, ChangelogItems300, Changelog200, ChangelogItems200,
        Changelog130, ChangelogItems130, Changelog120, ChangelogItems120,
        Changelog110, ChangelogItems110, Changelog100, ChangelogItems100, Copyright,
        EulaButton, EulaTitle, EulaIntro, EulaBody, EulaAgree,

        MergeNoDocs, MergeFileMissing, MergeWordMissing, MergeWordStartFail, MergeWordNotInstalled,
        MergeProcessing, MergeStartingWord, MergeMergingPages, MergeMerging,
        MergePdfUnreadable, MergeNoPages, MergePdfError, MergeWriteError, MergeNoPermission,
        MergeFailedGeneric, MergeDone, MergeWordOpenFail, MergeWordConvertEmpty, MergeWordConvertFail,
        MergeImageConvertFail, MergeTempFail,
        MergePasswordTitle, MergePasswordHint, MergePasswordPlaceholder, MergePasswordWrong,
        MsgSkipWordTitle, MsgSkipWordText, StatusSkippedWord,

        ViewResult, PdfViewerTitle, PrevPage, NextPage, ZoomIn, ZoomOut, ViewerPageFormat,
        ViewerNoReader, ViewerLoadFailed,

        SettingsNavLook, SettingsNavNaming, SettingsNavDone, SettingsNavWatermark,
        WatermarkSection, WatermarkHint, WatermarkEnable, WatermarkPresets, WatermarkCustom,
        WatermarkForMerge, WatermarkForPdfImage, WatermarkForImagePdf,
        WatermarkPreset1, WatermarkPreset2, WatermarkPreset3, WatermarkPreset4, WatermarkPreview,
        WatermarkFont,
        WatermarkCustomPlaceholder, WatermarkPreviewHint, WatermarkTint, WatermarkTintGray,
        WatermarkTintRed, WatermarkTintBlue,
        WatermarkPosition, WatermarkPosTile, WatermarkPosTopLeft, WatermarkPosTopRight,
        WatermarkPosCenter, WatermarkPosBottomLeft, WatermarkPosBottomRight,
        WatermarkOpacity, WatermarkScale, WatermarkRotation, WatermarkOffsetX, WatermarkOffsetY,
        ToastTitle, ToastOpen,
        Changelog310, ChangelogItems310, Changelog400, ChangelogItems400,
        Changelog430, ChangelogItems430,
        Changelog420, ChangelogItems420,
        Changelog412, ChangelogItems412,
        Changelog411, ChangelogItems411,
        Changelog410, ChangelogItems410,

        // ---------- v4.4 新外壳与 PDF 转图片 ----------
        NavMergeTitle, NavMergeSubtitle, NavConvertTitle, NavConvertSubtitle,
        MergePageTitle, MergePageSubtitle, ConvertPageTitle, ConvertPageSubtitle,
        ConvertFilesTitle, ConvertCountFormat, ConvertListHint, ConvertAdd, ConvertDialogTitle,
        ConvertEmptyTitle, ConvertEmptySubtitle, ConvertEmptyFormats,
        ConvertPageCountFormat, ConvertPageCountUnknown, ConvertPageCountFailed,
        ConvertSettingsTitle, ConvertFormat, ConvertPpi, ConvertPpiCustom, ConvertPpiHint,
        ConvertQuality, ConvertBackground, ConvertBackgroundHint,
        ConvertPageRange, ConvertPageRangeAll, ConvertPageRangePlaceholder,
        ConvertPageModeAll, ConvertPageModeOdd, ConvertPageModeEven, ConvertPageModeCustom,
        ConvertRangeNeeded, ConvertNoMatchingPages,
        ConvertSubfolder, ConvertSubfolderHint, ConvertOutput,
        ConvertNamePreview, ConvertEstimateFormat, ConvertPreviewTitle, ConvertPreviewEmpty,
        ConvertStart, ConvertRunning, ConvertCancel, ConvertCancelling,
        ConvertReady, ConvertProcessing, ConvertPageStatus,
        ConvertDone, ConvertDonePartial, ConvertCancelled, ConvertFailStatus, ConvertFailTitle,
        ConvertOpenFolder, ConvertOutputSubfolder,
        ConvertNoFiles, ConvertNeedFolder, ConvertBadRange, ConvertRangeOut, ConvertNoPages,
        ConvertUnreadable, ConvertPpiTooLarge, ConvertWriteError, ConvertMemory,
        ConvertSkippedNonPdf, ConvertDuplicate, ConvertAdded, ConvertNothingAdded, MsgConvertBusy,
        SettingsNavConvert, ConvertSection, ConvertDefaultsHint,
        ConvertDefaultFormat, ConvertDefaultPpi, ConvertDefaultQuality,
        ConvertDefaultBackground, ConvertDefaultSubfolder,
        ConvertWatermarkNote, NamingHintShared, DoneOpenShared,
        TipNavMerge, TipNavConvert, TipAppearance, TipNotifications, TipWatermark,
        Feature5,
        Changelog440, ChangelogItems440,
        Changelog450, ChangelogItems450,

        // ---------- v4.5.1 图片转 PDF ----------
        NavImagePdfTitle, NavImagePdfSubtitle,
        ImagePdfPageTitle, ImagePdfPageSubtitle,
        ImagePdfFilesTitle, ImagePdfCountFormat, ImagePdfListHint, ImagePdfAdd, ImagePdfDialogTitle,
        ImagePdfEmptyTitle, ImagePdfEmptySubtitle, ImagePdfEmptyFormats,
        ImagePdfSettingsTitle, ImagePdfPageSize, ImagePdfPageSizeAuto,
        ImagePdfOrientation, ImagePdfOrientationAuto, ImagePdfOrientationPortrait, ImagePdfOrientationLandscape,
        ImagePdfMargin, ImagePdfMarginNone, ImagePdfMarginSmall, ImagePdfMarginLarge,
        ImagePdfFitHint, ImagePdfNamePreview, ImagePdfPageCountFormat,
        ImagePdfMergeOption, ImagePdfNameLabel,
        ImagePdfNamePlaceholderMerge, ImagePdfNamePlaceholderSeparate,
        ImagePdfNamePreviewMulti, ImagePdfDoneFiles, ImagePdfDonePartial,
        ImagePdfOutputCount, ImagePdfReadyMerge,
        ImagePdfPreviewTitle, ImagePdfPreviewEmpty,
        ImagePdfStart, ImagePdfRunning, ImagePdfCancel, ImagePdfCancelling,
        ImagePdfReady, ImagePdfProcessing, ImagePdfDone, ImagePdfCancelled,
        ImagePdfFailStatus, ImagePdfFailTitle,
        ImagePdfNoFiles, ImagePdfNeedFolder, ImagePdfUnreadable, ImagePdfWriteError, ImagePdfFailGeneric,
        ImagePdfAdded, ImagePdfSkippedNonImage, ImagePdfNothingAdded, MsgImagePdfBusy,
        TipNavImagePdf, Feature6,
        Changelog451, ChangelogItems451,

        // ---------- v4.5.18 Word 转 PDF ----------
        WatermarkForWordPdf,
        NavWordPdfTitle, NavWordPdfSubtitle,
        WordPdfPageTitle, WordPdfPageSubtitle,
        WordPdfFilesTitle, WordPdfCountFormat, WordPdfListHint, WordPdfAdd, WordPdfDialogTitle,
        WordPdfEmptyTitle, WordPdfEmptySubtitle, WordPdfEmptyFormats,
        WordPdfSettingsTitle, WordPdfMergeOption, WordPdfMergeHint,
        WordPdfNameLabel, WordPdfNamePlaceholder, WordPdfSeparateHint,
        WordPdfOutput, WordPdfNamePreview, WordPdfNamePreviewMulti,
        WordPdfStart, WordPdfRunning, WordPdfCancel, WordPdfCancelling,
        WordPdfReady, WordPdfReadyMerge, WordPdfEngineWord, WordPdfEngineWps, WordPdfEngineLibre,
        WordPdfProcessing, WordPdfMerging, WordPdfDone, WordPdfDoneFiles, WordPdfCancelled,
        WordPdfFailStatus, WordPdfFailTitle,
        WordPdfNoFiles, WordPdfNeedFolder, WordPdfNoEngine, WordPdfNonWord,
        WordPdfWriteError, WordPdfFailGeneric,
        WordPdfAdded, WordPdfSkippedNonWord, WordPdfNothingAdded, WordPdfOutputCount,
        MsgWordPdfBusy, TipNavWordPdf, Feature7,
        Changelog4518, ChangelogItems4518,
        Changelog4524, ChangelogItems4524,
        Changelog4523, ChangelogItems4523, Changelog4522, ChangelogItems4522, Changelog4521, ChangelogItems4521, Changelog4520, ChangelogItems4520, Changelog4519, ChangelogItems4519,

        Changelog452, ChangelogItems452,
        Changelog453, ChangelogItems453,
        Changelog454, ChangelogItems454,
        Changelog457, ChangelogItems457,
        Changelog4517, ChangelogItems4517,
        Changelog4516, ChangelogItems4516,
        Changelog4515, ChangelogItems4515,
        Changelog4514, ChangelogItems4514,
        Changelog4513, ChangelogItems4513,
        Changelog4512, ChangelogItems4512,
        Changelog4511, ChangelogItems4511,
        Changelog4510, ChangelogItems4510,
        Changelog459, ChangelogItems459,
        Changelog458, ChangelogItems458,
        Changelog456, ChangelogItems456,
        Changelog455, ChangelogItems455,

        DefaultDocName, DefaultConvertName, MergeTimePrefix, ConvertTimePrefix, MergeCollideSuffix
    }

    public static class Loc
    {
        private static readonly Dictionary<Language, Dictionary<Txt, string>> Tables = new()
        {
            { Language.ZhCn, LocZhCn.Table },
            { Language.ZhTw, LocZhTw.Table },
            { Language.En, LocEn.Table },
            { Language.Ja, LocJa.Table },
            { Language.Ru, LocRu.Table },
            { Language.Ko, LocKo.Table }
        };

        public static Language Current { get; private set; } = Language.ZhCn;

        public static readonly Language[] All =
        {
            Language.ZhCn, Language.ZhTw, Language.En, Language.Ja, Language.Ru, Language.Ko
        };

        public static void SetLanguage(Language language)
        {
            if (!Tables.ContainsKey(language)) language = Language.ZhCn;
            Current = language;
            var culture = CultureFor(language);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        /// <summary>按系统界面语言挑选默认界面语言。</summary>
        public static Language DetectSystem()
        {
            try
            {
                CultureInfo culture = CultureInfo.CurrentUICulture;
                string name = culture.Name.Replace('_', '-');
                string lower = name.ToLowerInvariant();

                if (lower.StartsWith("zh"))
                {
                    if (lower.Contains("tw") || lower.Contains("hk") || lower.Contains("mo") ||
                        lower.Contains("hant") || lower.Contains("cht"))
                        return Language.ZhTw;
                    return Language.ZhCn;
                }

                switch (culture.TwoLetterISOLanguageName.ToLowerInvariant())
                {
                    case "ja": return Language.Ja;
                    case "ru": return Language.Ru;
                    case "ko": return Language.Ko;
                    case "en": return Language.En;
                    default: break;
                }

                if (lower.StartsWith("ja")) return Language.Ja;
                if (lower.StartsWith("ru")) return Language.Ru;
                if (lower.StartsWith("ko")) return Language.Ko;
                if (lower.StartsWith("en")) return Language.En;
            }
            catch
            {
                // 读取系统语言失败时退回简体中文
                return Language.ZhCn;
            }
            // 其余语言统一使用英文界面
            return Language.En;
        }

        private static CultureInfo CultureFor(Language language)
        {
            try
            {
                switch (language)
                {
                    case Language.ZhTw: return CultureInfo.GetCultureInfo("zh-TW");
                    case Language.En: return CultureInfo.GetCultureInfo("en-US");
                    case Language.Ja: return CultureInfo.GetCultureInfo("ja-JP");
                    case Language.Ru: return CultureInfo.GetCultureInfo("ru-RU");
                    case Language.Ko: return CultureInfo.GetCultureInfo("ko-KR");
                    default: return CultureInfo.GetCultureInfo("zh-CN");
                }
            }
            catch
            {
                return CultureInfo.InvariantCulture;
            }
        }

        /// <summary>界面语言的自称，始终使用该语言本身。</summary>
        public static string NativeName(Language language)
        {
            switch (language)
            {
                case Language.ZhCn: return "简体中文";
                case Language.ZhTw: return "繁體中文";
                case Language.En: return "English";
                case Language.Ja: return "日本語";
                case Language.Ru: return "Русский";
                case Language.Ko: return "한국어";
                default: return "简体中文";
            }
        }

        public static string T(Txt key, params object[] args)
        {
            string format = Lookup(Current, key);
            if (args.Length == 0) return format;
            try { return string.Format(CultureInfo.CurrentCulture, format, args); }
            catch { return format; }
        }

        public static string T(Language language, Txt key, params object[] args)
        {
            string format = Lookup(language, key);
            if (args.Length == 0) return format;
            try { return string.Format(CultureInfo.CurrentCulture, format, args); }
            catch { return format; }
        }

        public static string SkinName(string skinId)
        {
            switch (skinId)
            {
                case SkinPalette.NightId: return T(Txt.SkinNightName);
                case SkinPalette.CloudWhiteId: return T(Txt.SkinCloudName);
                case SkinPalette.SkeuoId: return T(Txt.SkinSkeuoName);
                case SkinPalette.NeumorphId: return T(Txt.SkinNeumorphName);
                default: return T(Txt.SkinBlueName);
            }
        }

        public static string SkinDescription(string skinId)
        {
            switch (skinId)
            {
                case SkinPalette.NightId: return T(Txt.SkinNightDesc);
                case SkinPalette.CloudWhiteId: return T(Txt.SkinCloudDesc);
                case SkinPalette.SkeuoId: return T(Txt.SkinSkeuoDesc);
                case SkinPalette.NeumorphId: return T(Txt.SkinNeumorphDesc);
                default: return T(Txt.SkinBlueDesc);
            }
        }

        public static string SoundName(string soundId)
        {
            switch (soundId)
            {
                case SoundLibrary.QqId: return T(Txt.SoundQQ);
                case SoundLibrary.ChimeId: return T(Txt.SoundChime);
                case SoundLibrary.SoftId: return T(Txt.SoundSoft);
                case SoundLibrary.WaterId: return T(Txt.SoundWater);
                case SoundLibrary.OffId: return T(Txt.SoundOff);
                default: return T(Txt.SoundDing);
            }
        }

        private static string Lookup(Language language, Txt key)
        {
            if (Tables.TryGetValue(language, out var table) && table.TryGetValue(key, out string? text))
                return text;
            if (Tables.TryGetValue(Language.ZhCn, out var fallback) && fallback.TryGetValue(key, out string? zh))
                return zh;
            return key.ToString();
        }
    }
}
