using System.Collections.Generic;

namespace DocHub
{
    internal static class LocZhTw
    {
        public static readonly Dictionary<Txt, string> Table = new()
        {
            [Txt.AppName] = "文樞",
            [Txt.Tagline] = "文件中樞，一鍵合轉 · Word / PDF / 圖片 合併與轉換",
            [Txt.LocalBadge] = "本機處理 · 檔案不外傳",

            [Txt.Ok] = "確定",
            [Txt.Close] = "關閉",
            [Txt.Cancel] = "取消",
            [Txt.Yes] = "是",
            [Txt.No] = "否",
            [Txt.Save] = "儲存",
            [Txt.Reset] = "恢復預設",
            [Txt.Browse] = "瀏覽",
            [Txt.PreviewWord] = "試聽",
            [Txt.Settings] = "設定",
            [Txt.About] = "關於",
            [Txt.Skin] = "面板",
            [Txt.RailMerge] = "合併",

            [Txt.DocsTitle] = "檔案清單",
            [Txt.DocCountFormat] = "{0} 個檔案",
            [Txt.ListHint] = "拖曳排序 · 空白處拖出選框可多選 · 支援批次拖入 Word / PDF / 圖片",
            [Txt.AddDocs] = "加入檔案",
            [Txt.MoveUp] = "上移",
            [Txt.MoveDown] = "下移",
            [Txt.MoveTop] = "置頂",
            [Txt.MoveBottom] = "置底",
            [Txt.Remove] = "刪除",
            [Txt.Clear] = "清空",
            [Txt.OutputFile] = "輸出檔案",
            [Txt.SaveTo] = "儲存到",
            [Txt.ChooseFolder] = "選擇儲存位置",
            [Txt.StartMerge] = "開始合併",
            [Txt.MergingButton] = "正在合併…",
            [Txt.ReadyStatus] = "就緒 · 依清單順序由上到下合併",
            [Txt.NamePlaceholderFirst] = "自動取第一個文件名",
            [Txt.NamePlaceholderPhrase] = "第一個文件名 + 自訂語句",
            [Txt.NamePlaceholderTime] = "合併時間（自動產生）",
            [Txt.NamePlaceholderCustom] = "輸入自訂文件名",
            [Txt.EmptyTitle] = "拖曳 Word / PDF / 圖片檔案到這裡",
            [Txt.EmptySubtitle] = "或點擊右上角「加入檔案」批次選擇",
            [Txt.EmptyFormats] = ".doc · .docx · .rtf · .pdf · .jpg · .png · .bmp · .tiff",

            [Txt.TipAdd] = "批次選擇要合併的 Word / PDF / 圖片檔案",
            [Txt.TipUp] = "將選取的檔案上移一位",
            [Txt.TipDown] = "將選取的檔案下移一位",
            [Txt.TipRemove] = "從清單移除，不會刪除來源檔案",
            [Txt.TipClear] = "清空整個檔案清單",
            [Txt.TipBrowse] = "選擇合併後 PDF 的儲存位置",
            [Txt.TipList] = "可拖曳檔案到這裡，也可以拖曳清單項目調整順序",
            [Txt.TipStart] = "依清單順序合併並產生 PDF",
            [Txt.TipSettings] = "命名方式、面板、提示音與語言",
            [Txt.TipAbout] = "版本資訊、更新紀錄與使用者條款",
            [Txt.TipCancelBox] = "取消框選，還原為框選前的選擇（Esc）",
            [Txt.TrayOpen] = "開啟主畫面",
            [Txt.TrayExit] = "結束",

            [Txt.DialogTitleAdd] = "選擇要合併的文件",
            [Txt.FolderDesc] = "選擇合併後 PDF 的儲存位置",
            [Txt.FilterSupport] = "支援的檔案",
            [Txt.FilterWord] = "Word 文件",
            [Txt.FilterPdf] = "PDF 檔案",
            [Txt.FilterImage] = "圖片檔案",
            [Txt.FilterAll] = "所有檔案",

            [Txt.StatusAdded] = "已加入 {0} 個檔案 · 依清單順序合併",
            [Txt.StatusSkipped] = "（略過 {0} 個重複或不受支援的檔案）",
            [Txt.StatusNothing] = "沒有新增檔案：檔案重複、不存在或格式不受支援",
            [Txt.StatusOrder] = "順序已調整 · 依目前清單順序合併",
            [Txt.StatusRemoved] = "已從清單移除：{0}",
            [Txt.StatusRemovedMulti] = "已從清單移除 {0} 個文件",
            [Txt.StatusMovedTop] = "已置頂 · 選取的檔案排到最前",
            [Txt.StatusMovedBottom] = "已置底 · 選取的檔案排到最後",
            [Txt.StatusCleared] = "清單已清空",
            [Txt.StatusFolderChosen] = "已選擇儲存位置",

            [Txt.MsgClearTitle] = "清空清單",
            [Txt.MsgClearText] = "確定清空整個文件清單嗎？\n\n來源檔案不會被刪除。",
            [Txt.MsgMergeBusy] = "正在合併文件，請等待完成後再關閉視窗。",
            [Txt.MsgNoWordTitle] = "缺少 Microsoft Word",
            [Txt.MsgNoWordText] = "未偵測到 Microsoft Word，無法轉換 Word 文件。\n\n請先安裝 Microsoft Word；純 PDF 與圖片仍可直接合併。",
            [Txt.MsgOverwriteTitle] = "覆寫確認",
            [Txt.MsgOverwriteText] = "目標檔案已存在：\n{0}\n\n是否覆寫？",

            [Txt.StatusPreparing] = "正在準備…",
            [Txt.StatusProgress] = "{0}% · {1}",
            [Txt.StatusDone] = "合併完成 · 共 {0} 頁 · 耗時 {1}",
            [Txt.BalloonTitle] = "合併完成",
            [Txt.BalloonText] = "共 {0} 頁 · 耗時 {1}\n{2}",
            [Txt.OpenFailed] = "無法開啟 PDF，請手動前往儲存位置查看。",
            [Txt.FailStatus] = "合併失敗，請檢查檔案後重試",
            [Txt.FailTitle] = "合併失敗",
            [Txt.FailPrefix] = "合併失敗：{0}",

            [Txt.ErrCannotStartTitle] = "無法開始合併",
            [Txt.ErrNeedDocs] = "請先加入要合併的文件。",
            [Txt.ErrNeedName] = "請填寫輸出檔名。",
            [Txt.ErrBadChars] = "輸出檔名不能包含下列字元：\\ / : * ? \" < > |",
            [Txt.ErrNeedFolder] = "請選擇儲存位置。",
            [Txt.ErrFolderMissing] = "儲存位置不存在：\n{0}",
            [Txt.ErrPathInvalid] = "輸出路徑無效。",
            [Txt.ErrSameSource] = "輸出檔案與某個來源文件同名，繼續會覆寫來源檔案。\n\n請修改輸出檔名或儲存位置。",
            [Txt.ElapsedSeconds] = "{0:0.0} 秒",
            [Txt.ElapsedMinutes] = "{0} 分 {1} 秒",

            [Txt.SettingsTitle] = "設定",
            [Txt.NamingSection] = "輸出檔案命名",
            [Txt.NamingHint] = "合併完成後依此方式產生檔名",
            [Txt.NamingFirst] = "依最上方檔案的檔名",
            [Txt.NamingPhrase] = "最上方檔名 + 自訂語句",
            [Txt.NamingTime] = "合併時間（自動產生）",
            [Txt.NamingCustom] = "使用自訂名稱",
            [Txt.PhrasePlaceholder] = "例如：定稿",
            [Txt.CustomPlaceholder] = "例如：2026年度合約彙總",
            [Txt.PreviewFormat] = "命名預覽：{0}.pdf",
            [Txt.SkinSection] = "外觀面板",
            [Txt.SkinHint] = "選擇介面風格，切換後立即生效",
            [Txt.DoneSection] = "合併完成後",
            [Txt.DoneNotify] = "在電腦右下角顯示完成提示",
            [Txt.DoneSound] = "播放完成提示音",
            [Txt.DoneOpen] = "完成後自動開啟產生的 PDF（預設關閉）",
            [Txt.SoundSection] = "提示音",
            [Txt.SoundWater] = "水滴",
            [Txt.SoundDing] = "叮咚",
            [Txt.SoundQQ] = "QQ",
            [Txt.SoundChime] = "清脆",
            [Txt.SoundSoft] = "柔和",
            [Txt.SoundOff] = "靜音",
            [Txt.SoundHint] = "先開啟提示音，再選擇音效；選取即自動播放",
            [Txt.LanguageSection] = "介面語言",
            [Txt.LanguageHint] = "切換後立即生效",
            [Txt.QuoteSection] = "每日一言",
            [Txt.QuoteHint] = "隨介面語言切換",
            [Txt.QuoteToggle] = "在合併頁顯示名言或電影台詞",
            [Txt.QuoteAutoRefresh] = "自動換一句",
            [Txt.QuoteRefreshInterval] = "換句間隔",
            [Txt.QuoteRefreshSeconds] = "秒",
            [Txt.QuoteRefreshHint] = "開啟後每隔設定時間自動換一句；關閉後僅點擊卡片換句",
            [Txt.TipQuote] = "點擊換一句",
            [Txt.OutputSection] = "儲存位置",
            [Txt.OutputHint] = "合併後的 PDF 儲存在哪裡",
            [Txt.OutputFirst] = "最上方檔案所在的資料夾",
            [Txt.OutputLast] = "上一次儲存的位置",
            [Txt.OutputCustom] = "自訂資料夾",
            [Txt.OutputCustomPlaceholder] = "選擇或輸入資料夾路徑",
            [Txt.SkinBlueName] = "經典藍",
            [Txt.SkinBlueDesc] = "清爽通透",
            [Txt.SkinNightName] = "夜間模式",
            [Txt.SkinNightDesc] = "沉浸暗色",
            [Txt.SkinCloudName] = "雲白",
            [Txt.SkinCloudDesc] = "暖白紙感",
            [Txt.SkinSkeuoName] = "擬物風",
            [Txt.SkinSkeuoDesc] = "皮革紙張質感",
            [Txt.SkinNeumorphName] = "新擬態",
            [Txt.SkinNeumorphDesc] = "柔和立體",
            [Txt.SettingsLiveHint] = "所有修改即時生效",

            [Txt.AboutTitle] = "關於",
            [Txt.VersionLine] = "{0} · Word / PDF / 圖片 合併與轉換工具",
            [Txt.AboutIntro] = "所有處理均在本機完成，檔案不外傳",
            [Txt.BrandDefinition] = "文樞：文件中樞，一鍵合轉",
            [Txt.Feature1] = "Word 全格式、PDF 與圖片混搭合併，批次匯入無大小限制",
            [Txt.Feature2] = "保留原排版、頁碼、圖片尺寸與方向，不做二次排版",
            [Txt.Feature3] = "拖曳排序、即時進度，輸出順序完全可控",
            [Txt.Feature4] = "本機離線處理，檔案不會上傳到任何伺服器",
            [Txt.ChangelogTitle] = "更新紀錄",
            [Txt.Changelog300] = "3.0.0 · 2026-11-03",
            [Txt.ChangelogItems300] =
                "#新增\n" +
                "· 安裝程式內建完整執行階段，未安裝 .NET 的電腦也能直接安裝使用\n" +
                "· 內建簡易 PDF 閱讀器：本機沒有 PDF 閱讀器時也能檢視合併結果\n" +
                "· 關於頁新增作者署名與聯絡方式（cado@163.com）\n" +
                "#優化\n" +
                "· 設定項目修改後立即生效，無需再按「儲存」\n" +
                "· 提示音選取即自動播放，無需再按「試聽」\n" +
                "· 合併速度提升：圖片平行轉換，結果直接寫入目標檔案\n" +
                "· 安裝後的主程式保持精簡，執行階段隨安裝程式一起散佈\n" +
                "#修復\n" +
                "· 修復圖示圖形重心偏上，調整為垂直置中\n" +
                "· 修復未安裝 Word 時無法合併 PDF 與圖片的問題：支援 LibreOffice，或跳過 Word 繼續合併",
            [Txt.Changelog200] = "2.0.0 · 2026-11-03",
            [Txt.ChangelogItems200] =
                "#新增\n" +
                "· 支援 JPG、PNG、BMP、TIFF 等圖片直接合併，保持原始尺寸與方向\n" +
                "· Word、PDF、圖片可在同一清單混合合併，統一輸出為 PDF\n" +
                "· 介面語言預設跟隨系統語言，安裝後即為對應語言\n" +
                "#優化\n" +
                "· 主介面與提示統一為「檔案」描述，清單區分 PDF / WORD / 圖片\n" +
                "· 設定介面全面扁平化：純色分區、細線分隔，移除漸層與陰影\n" +
                "· 更新紀錄依「新增 / 優化 / 修復」分類展示，重點更清晰\n" +
                "#修復\n" +
                "· 修復僅含 PDF 或圖片時仍提示缺少 Word 的問題",
            [Txt.Changelog130] = "1.3.0 · 2026-11-03",
            [Txt.ChangelogItems130] =
                "#新增\n" +
                "· 新增儲存位置設定：最上方檔案位置、上一次儲存位置、自訂路徑\n" +
                "· 檔案清單支援右鍵刪除、上移、下移\n" +
                "· 提供電腦安裝程式與綠色版（不壓縮）兩種封裝\n" +
                "#優化\n" +
                "· 設定儲存後立即生效，無需重新啟動軟體\n" +
                "· 進度條跟隨主題配色，並顯示合併百分比\n" +
                "· 檔案清單煥新：卡片式列、類型徽章與檔案資訊\n" +
                "#修復\n" +
                "· 修復清單抖動與閃爍\n" +
                "· 修復同名檔案覆寫問題，自動編號為 A-1.pdf",
            [Txt.Changelog120] = "1.2.0 · 2026-11-03",
            [Txt.ChangelogItems120] =
                "#新增\n" +
                "· 全新扁平化應用程式圖示，無文字，造型更簡潔\n" +
                "· 5 種內建完成提示音：水滴、叮咚、QQ 等，支援試聽\n" +
                "· 關於頁新增《使用者條款》\n" +
                "· 新增繁體中文、英文、日文、俄文、韓文介面，可即時切換\n" +
                "#優化\n" +
                "· 全面美化介面細節：圓角、陰影、留白與互動回饋",
            [Txt.Changelog110] = "1.1.0 · 2026-11-03",
            [Txt.ChangelogItems110] =
                "#新增\n" +
                "· 4 種命名方式：首檔名、首檔名＋自訂語句、合併時間、自訂名稱\n" +
                "· 4 套網易雲風面板：經典藍、網易紅、夜間模式、雲白\n" +
                "#優化\n" +
                "· 新增「設定」與「關於」入口，圖示改為扁平風格\n" +
                "· 合併完成預設不開啟 PDF，改為右下角提示與提示音，可自訂",
            [Txt.Changelog100] = "1.0.0 · 2026-11-02",
            [Txt.ChangelogItems100] =
                "#新增\n" +
                "· 首個版本：Word 全格式與 PDF 混搭合併、批次匯入、拖曳排序、進度顯示",
            [Txt.ContactAuthor] = "作者：AI · 構思：chao",
            [Txt.Copyright] = "© 2026 AI · 本機處理更安心",
            [Txt.EulaButton] = "使用者條款",
            [Txt.EulaTitle] = "使用者條款",
            [Txt.EulaIntro] = "使用「文樞」前，請閱讀以下條款。",
            [Txt.EulaBody] =
                "一、用途說明\n" +
                "「文樞」是一款在本機執行的 Word / PDF / 圖片 合併與轉換工具。軟體呼叫本機安裝的 Microsoft Word " +
                "將 Word 文件匯出為 PDF，圖片依原始尺寸排成頁面，再依您指定的順序逐頁合併。\n\n" +
                "二、您的檔案\n" +
                "所有處理均在本機完成。軟體不會上傳、傳輸或備份任何文件，也不會蒐集您的個人資訊或文件內容。\n\n" +
                "三、使用責任\n" +
                "您應確保對所合併的文件擁有合法使用權。合併前請自行備份重要檔案。因檔案損壞、被覆寫或使用不當" +
                "造成的損失，作者不承擔責任。\n\n" +
                "四、執行環境\n" +
                "合併 Word 文件需要本機安裝 Microsoft Word 或 LibreOffice。本軟體不提供、不啟用 Office。\n\n" +
                "五、免責聲明\n" +
                "軟體依現狀提供。我們會盡力測試，但不保證在任何環境下所有文件都能完美合併。\n\n" +
                "六、條款更新\n" +
                "本條款可能隨版本更新而調整，繼續使用即表示您接受更新後的條款。",
            [Txt.EulaAgree] = "我已閱讀並同意",

            [Txt.MergeNoDocs] = "請先加入要合併的文件。",
            [Txt.MergeFileMissing] = "檔案不存在或已被移動：\n{0}",
            [Txt.MergeWordMissing] =
                "未偵測到 Microsoft Word 或 LibreOffice，無法轉換 Word 文件。\n\n" +
                "請安裝 Microsoft Word 或 LibreOffice 後再合併 Word 檔案；純 PDF 與圖片可直接合併。",
            [Txt.MergeWordStartFail] = "無法啟動 Microsoft Word。請確認 Word 已正確安裝並可正常開啟。",
            [Txt.MergeWordNotInstalled] = "未偵測到 Microsoft Word 或 LibreOffice，請安裝後再試。",
            [Txt.MergeProcessing] = "正在處理（{0}/{1}）：{2}",
            [Txt.MergeStartingWord] = "正在啟動 Word 轉換引擎…",
            [Txt.MergeMergingPages] = "正在合併頁面…",
            [Txt.MergeMerging] = "正在合併（{0}/{1}）：{2}",
            [Txt.MergePdfUnreadable] = "無法讀取 PDF「{0}」：檔案可能已損壞、加密或格式不受支援。",
            [Txt.MergeNoPages] = "文件中沒有可合併的頁面。",
            [Txt.MergePdfError] = "合併 PDF 時發生錯誤，輸出檔案未產生。",
            [Txt.MergeWriteError] =
                "無法寫入輸出檔案：\n{0}\n\n請確認檔案未被其他程式占用，且儲存位置可寫入。\n\n技術資訊：{1}",
            [Txt.MergeNoPermission] = "沒有權限寫入輸出檔案：\n{0}\n\n請更換儲存位置後重試。",
            [Txt.MergeFailedGeneric] = "合併失敗：{0}",
            [Txt.MergeTempFail] = "無法建立合併用的暫存目錄。\n\n可能是安全軟體阻止了檔案寫入，或暫存目錄權限異常。\n請把本程式加入安全軟體白名單，或以系統管理員身分執行後重試。",
            [Txt.MergeDone] = "合併完成",
            [Txt.MergeWordOpenFail] = "無法開啟「{0}」。",
            [Txt.MergeWordConvertEmpty] = "「{0}」轉換後沒有取得有效頁面。",
            [Txt.MergeWordConvertFail] =
                "無法轉換 Word 文件「{0}」。\n\n" +
                "請確認該檔案沒有加密、沒有損壞，並且本機 Microsoft Word 可以正常開啟它。\n\n技術資訊：{1}",
            [Txt.MergePasswordTitle] = "檔案已加密",
            [Txt.MergePasswordHint] = "請輸入開啟此檔案的密碼，僅在本次合併中使用。",
            [Txt.MergePasswordPlaceholder] = "開啟密碼",
            [Txt.MergePasswordWrong] = "密碼不正確，請重新輸入。",
            [Txt.MergeImageConvertFail] =
                "無法讀取圖片「{0}」。\n\n" +
                "請確認圖片沒有損壞。JPG / PNG / BMP / TIFF / GIF / WebP 可直接合併；" +
                "HEIC / HEIF / AVIF 需要系統已安裝對應的解碼擴充。\n\n技術資訊：{1}",

            [Txt.MsgSkipWordTitle] = "Word 檔案無法轉換",
            [Txt.MsgSkipWordText] =
                "未偵測到 Microsoft Word 或 LibreOffice，{0} 個 Word 檔案無法轉換。\n\n" +
                "是否跳過這些 Word 檔案，繼續合併其餘 PDF 與圖片？",
            [Txt.StatusSkippedWord] = "已跳過 {0} 個 Word 檔案（未安裝 Word / LibreOffice）",

            [Txt.ViewResult] = "檢視合併文件",
            [Txt.PdfViewerTitle] = "PDF 閱讀器",
            [Txt.PrevPage] = "上一頁",
            [Txt.NextPage] = "下一頁",
            [Txt.ZoomIn] = "放大",
            [Txt.ZoomOut] = "縮小",
            [Txt.ViewerPageFormat] = "第 {0} / {1} 頁",
            [Txt.ViewerNoReader] = "本機未偵測到 PDF 閱讀器，已啟用內建閱讀器",
            [Txt.ViewerLoadFailed] = "無法開啟 PDF 檔案：\n{0}",

            [Txt.SettingsNavLook] = "外觀",
            [Txt.SettingsNavNaming] = "命名與儲存",
            [Txt.SettingsNavDone] = "完成提醒",
            [Txt.SettingsNavWatermark] = "浮水印",
            [Txt.WatermarkSection] = "文件浮水印",
            [Txt.WatermarkHint] = "勾選需要添加浮水印的功能；樣式三項共用，預設關閉",
            [Txt.WatermarkEnable] = "添加浮水印",
            [Txt.WatermarkForMerge] = "文件合併",
            [Txt.WatermarkForPdfImage] = "PDF 轉圖片",
            [Txt.WatermarkForImagePdf] = "圖片轉 PDF",
            [Txt.WatermarkForWordPdf] = "Word 轉 PDF",
            [Txt.WatermarkPresets] = "預置文字",
            [Txt.WatermarkCustom] = "自訂",
            [Txt.WatermarkPreset1] = "正本",
            [Txt.WatermarkPreset2] = "副本",
            [Txt.WatermarkPreset3] = "機密",
            [Txt.WatermarkPreset4] = "絕密",
            [Txt.WatermarkPreview] = "效果預覽",
            [Txt.WatermarkCustomPlaceholder] = "輸入浮水印文字",
            [Txt.WatermarkPreviewHint] = "預覽中拖動可移動、滾輪縮放、右鍵拖動旋轉；數值也可直接鍵入",
            [Txt.WatermarkTint] = "浮水印顏色",
            [Txt.WatermarkTintGray] = "灰",
            [Txt.WatermarkTintRed] = "紅",
            [Txt.WatermarkTintBlue] = "藍",
            [Txt.WatermarkPosition] = "位置",
            [Txt.WatermarkPosTile] = "平鋪",
            [Txt.WatermarkPosTopLeft] = "左上",
            [Txt.WatermarkPosTopRight] = "右上",
            [Txt.WatermarkPosCenter] = "居中",
            [Txt.WatermarkPosBottomLeft] = "左下",
            [Txt.WatermarkPosBottomRight] = "右下",
            [Txt.WatermarkOpacity] = "不透明度",
            [Txt.WatermarkScale] = "大小",
            [Txt.WatermarkRotation] = "傾斜角度",
            [Txt.WatermarkOffsetX] = "水平位置",
            [Txt.WatermarkOffsetY] = "垂直位置",
            [Txt.ToastTitle] = "合併完成",
            [Txt.ToastOpen] = "檢視",
            [Txt.Changelog310] = "3.1.0 · 2026-11-03",
            [Txt.ChangelogItems310] =
                "#新增\n" +
                "· 設定介面改為「左側目錄 + 右側內容」兩級結構\n" +
                "· 新增文件浮水印：內建正本/副本/機密/絕密，可自訂文字、風格與顏色\n" +
                "#優化\n" +
                "· 安裝目錄分層：精簡「DocHub.exe」啟動器 + app 程式目錄，頂層更清爽\n" +
                "· 合併完成提醒改為與介面面板一致的右下角卡片\n",
            [Txt.Changelog400] = "4.0.0 · 2026-11-03",
            [Txt.ChangelogItems400] =
                "#新增\n" +
                "· 檔案清單支援多選批次操作，右鍵可一鍵「置頂」或「置底」\n" +
                "#優化\n" +
                "· 檔案清單列樣式更清爽，選取狀態更醒目\n" +
                "· 視窗縮放時自動重排版面，修正狀態列壓住「開始合併」按鈕\n" +
                "· 切換介面語言後，預設輸出名稱同步改為對應語言\n" +
                "· 恢復預設設定時，語言一併恢復為簡體中文\n" +
                "· 安裝程式預設安裝到 C:\\Program Files (x86)\\DocHub\n",
            [Txt.Changelog430] = "4.3.0 · 2026-11-04",
            [Txt.ChangelogItems430] =
                "#新增\n" +
                "· 主介面導覽移到右上角：外觀、浮水印、完成提醒、設定、關於以扁平圖示排列，點擊直達對應設定頁\n" +
                "#修復\n" +
                "· 浮水印過大或拖到頁面邊緣時會被裁掉：現在自動等比縮放並限制在頁面內，合併後完整顯示\n" +
                "· 預覽中的浮水印不再溢出預覽框\n" +
                "#最佳化\n" +
                "· 重新合成「水滴」「QQ」提示音，起音更清脆、餘韻更自然，並統一各提示音音量\n" +
                "· 移除主介面左側欄，介面更簡潔\n",
            [Txt.Changelog420] = "4.2.0 · 2026-11-04",
            [Txt.ChangelogItems420] =
                "#最佳化\n" +
                "· 設定、關於視窗秒開：磨砂背景依面板與尺寸快取，並於主界面顯示後提前產生\n" +
                "· 水印預覽拖曳更順暢：平鋪改為預先產生貼圖填滿，最低縮放下單幀由 190ms 降至 1ms 以內\n" +
                "#改進\n" +
                "· 左側導覽重新排版：圖示與名稱層級更清楚，選取狀態為強調色膠囊 + 左側直條\n" +
                "· 版本號依語意化版本（SemVer）規則迭代：體驗與效能改進升次版本號\n",
            [Txt.Changelog412] = "4.1.2 · 2026-11-04",
            [Txt.ChangelogItems412] =
                "#新增\n" +

                "· 浮水印位置可預設：平鋪 / 左上 / 右上 / 居中 / 左下 / 右下\n" +

                "#優化\n" +

                "· 浮水印設定不再卡頓：連續拖動只提交一次，預覽繪製快取字型與度量\n" +

                "· 主介面左側導航顯示功能名稱（合併 / 設定 / 關於），選中項以主題色高亮\n",

            [Txt.Changelog411] = "4.1.1 · 2026-11-04",
            [Txt.ChangelogItems411] =
                "#修正\n" +
                "· 安裝程式不再強制改回預設目錄：輸入或瀏覽的自訂安裝位置會原樣生效\n" +
                "· 修正匯入圖片後無法合併的問題：WebP / HEIC 等格式改由系統解碼器處理，一般圖片相容性也更好\n" +
                "#優化\n" +
                "· 浮水印各參數支援直接鍵入數值；預覽中拖動可移動、滾輪縮放、右鍵拖動旋轉\n" +
                "· 移除「浮水印風格」選項，統一為斜紋平鋪效果\n",

            [Txt.Changelog410] = "4.1.0 · 2026-11-04",
            [Txt.ChangelogItems410] =
                "#新增\n" +
                "· 浮水印支援任意不透明度、大小、位置與傾斜方向，設定頁即時預覽\n" +
                "· 安裝程式改為自研安裝程式，安裝到目前使用者目錄，無需系統管理員權限、不寫入 Program Files\n" +
                "· 開始選單新增「解除安裝 文樞」捷徑\n" +
                "#優化\n" +
                "· 設定介面加高，浮水印參數改為滑桿調節，操作更直觀\n" +
                "· 啟動器更名為 DocHub.exe，修正安裝時提示「無法開啟要寫入的檔案」\n" +
                "· 安裝與解除安裝前自動關閉執行中的程式，升級安裝不再占用檔案\n",

            [Txt.NavMergeTitle] = "文件合併",
            [Txt.NavMergeSubtitle] = "合併為 PDF",
            [Txt.NavConvertTitle] = "PDF 轉圖片",
            [Txt.NavConvertSubtitle] = "逐頁匯出圖片",
            [Txt.MergePageTitle] = "文件合併",
            [Txt.MergePageSubtitle] = "依清單順序將 Word / PDF / 圖片合成一個 PDF",
            [Txt.ConvertPageTitle] = "PDF 轉圖片",
            [Txt.ConvertPageSubtitle] = "將 PDF 的每一頁匯出為圖片 · 唯讀來源檔，不修改原文",
            [Txt.ConvertFilesTitle] = "PDF 檔案",
            [Txt.ConvertCountFormat] = "{0} 個檔案",
            [Txt.ConvertListHint] = "空白處拖曳可框選 · 加入 PDF 後自動讀取頁數",
            [Txt.ConvertAdd] = "加入 PDF",
            [Txt.ConvertDialogTitle] = "選擇要轉換的 PDF",
            [Txt.ConvertEmptyTitle] = "拖曳 PDF 到這裡",
            [Txt.ConvertEmptySubtitle] = "或點擊「加入 PDF」批次選擇",
            [Txt.ConvertEmptyFormats] = ".pdf",
            [Txt.ConvertPageCountFormat] = "{0} 頁",
            [Txt.ConvertPageCountUnknown] = "讀取頁數…",
            [Txt.ConvertPageCountFailed] = "無法讀取",
            [Txt.ConvertSettingsTitle] = "匯出設定",
            [Txt.ConvertFormat] = "圖片格式",
            [Txt.ConvertPpi] = "解析度（PPI）",
            [Txt.ConvertPpiCustom] = "自訂",
            [Txt.ConvertPpiHint] = "PPI 決定圖片清晰度，不改變頁面尺寸",
            [Txt.ConvertQuality] = "JPEG 品質",
            [Txt.ConvertBackground] = "透明背景",
            [Txt.ConvertBackgroundHint] = "僅 PNG 支援透明，其他格式為白底",
            [Txt.ConvertPageRange] = "頁碼範圍",
            [Txt.ConvertPageRangeAll] = "全部",
            [Txt.ConvertPageRangePlaceholder] = "例：1-3,5,8-10",
            [Txt.ConvertPageModeAll] = "全部",
            [Txt.ConvertPageModeOdd] = "奇數頁",
            [Txt.ConvertPageModeEven] = "偶數頁",
            [Txt.ConvertPageModeCustom] = "自訂",
            [Txt.ConvertSubfolder] = "每個 PDF 個別建立資料夾",
            [Txt.ConvertSubfolderHint] = "批次轉換時自動開啟；資料夾名稱附上頁碼模式",
            [Txt.ConvertOutput] = "儲存位置",
            [Txt.ConvertNamePreview] = "命名預覽：{0}",
            [Txt.ConvertEstimateFormat] = "預計 {0} × {1} px",
            [Txt.ConvertPreviewTitle] = "第 {0} 頁預覽",
            [Txt.ConvertPreviewEmpty] = "選擇 PDF 後顯示目前頁碼的預覽",
            [Txt.ConvertStart] = "開始轉換",
            [Txt.ConvertRunning] = "正在轉換…",
            [Txt.ConvertCancel] = "取消轉換",
            [Txt.ConvertCancelling] = "正在取消…",
            [Txt.ConvertReady] = "就緒 · 依清單順序匯出圖片",
            [Txt.ConvertProcessing] = "正在轉換（{0}/{1}）：{2}",
            [Txt.ConvertPageStatus] = "第 {0}/{1} 頁",
            [Txt.ConvertDone] = "轉換完成 · 共 {0} 張圖片 · 耗時 {1}",
            [Txt.ConvertDonePartial] = "完成 {0} 個 · 失敗 {1} 個 · 共 {2} 張圖片",
            [Txt.ConvertCancelled] = "已取消 · 已匯出 {0} 張圖片",
            [Txt.ConvertFailStatus] = "轉換失敗，請檢查檔案後重試",
            [Txt.ConvertFailTitle] = "轉換失敗",
            [Txt.ConvertOpenFolder] = "開啟資料夾",
            [Txt.ConvertOutputSubfolder] = "輸出：{0}",
            [Txt.ConvertNoFiles] = "請先加入要轉換的 PDF 檔案。",
            [Txt.ConvertNeedFolder] = "請選擇儲存位置。",
            [Txt.ConvertBadRange] = "頁碼範圍格式不正確，請使用如 1-3,5 的寫法。",
            [Txt.ConvertRangeOut] = "頁碼範圍超出文件頁數（共 {0} 頁）。",
            [Txt.ConvertNoPages] = "文件中沒有可轉換的頁面。",
            [Txt.ConvertRangeNeeded] = "請填寫頁碼範圍，或選擇「全部」。",
            [Txt.ConvertNoMatchingPages] = "目前頁碼設定下沒有可轉換的頁面。",
            [Txt.ConvertUnreadable] = "無法讀取 PDF「{0}」：檔案可能已損壞、加密或格式不受支援。",
            [Txt.ConvertPpiTooLarge] = "所選 PPI 會讓頁面過大（{0} × {1} px），請降低 PPI。",
            [Txt.ConvertWriteError] = "無法寫入圖片：\n{0}\n\n技術資訊：{1}",
            [Txt.ConvertMemory] = "記憶體不足，無法轉譯此頁面。請降低 PPI 後重試。",
            [Txt.ConvertSkippedNonPdf] = "已忽略 {0} 個非 PDF 檔案",
            [Txt.ConvertDuplicate] = "已略過 {0} 個重複檔案",
            [Txt.ConvertAdded] = "已加入 {0} 個 PDF 檔案",
            [Txt.ConvertNothingAdded] = "沒有新增檔案：檔案重複、不存在或不是 PDF",
            [Txt.MsgConvertBusy] = "正在轉換文件，請等待完成後再關閉視窗。",
            [Txt.SettingsNavConvert] = "PDF 轉圖片",
            [Txt.ConvertSection] = "PDF 轉圖片預設值",
            [Txt.ConvertDefaultsHint] = "轉換頁會以這些值作為初始設定，在轉換頁修改後會記住",
            [Txt.ConvertDefaultFormat] = "預設圖片格式",
            [Txt.ConvertDefaultPpi] = "預設 PPI",
            [Txt.ConvertDefaultQuality] = "預設 JPEG 品質",
            [Txt.ConvertDefaultBackground] = "預設透明背景",
            [Txt.ConvertDefaultSubfolder] = "預設每個 PDF 建立資料夾",
            [Txt.ConvertWatermarkNote] = "浮水印疊加在輸出圖片上，來源 PDF 始終唯讀；可在「浮水印」頁開啟",
            [Txt.NamingHintShared] = "合併與 PDF 轉圖片共用的輸出命名方式",
            [Txt.DoneOpenShared] = "完成後自動開啟結果（合併開啟 PDF，轉換開啟資料夾）",
            [Txt.TipNavMerge] = "文件合併：依清單順序合成一個 PDF",
            [Txt.TipNavConvert] = "PDF 轉圖片：將 PDF 每一頁匯出為圖片",
            [Txt.TipAppearance] = "開啟外觀設定",
            [Txt.TipNotifications] = "開啟完成提醒設定",
            [Txt.TipWatermark] = "開啟浮水印設定",
            [Txt.NavImagePdfTitle] = "圖片轉 PDF",
            [Txt.NavImagePdfSubtitle] = "圖片逐張或合併轉 PDF",
            [Txt.ImagePdfPageTitle] = "圖片轉 PDF",
            [Txt.ImagePdfPageSubtitle] = "圖片逐張輸出或合併為一個 PDF · 本機處理",
            [Txt.ImagePdfFilesTitle] = "圖片檔案",
            [Txt.ImagePdfCountFormat] = "{0} 個檔案",
            [Txt.ImagePdfListHint] = "空白處拖曳可框選 · 依清單順序轉換",
            [Txt.ImagePdfAdd] = "加入圖片",
            [Txt.ImagePdfDialogTitle] = "選擇要轉換的圖片",
            [Txt.ImagePdfEmptyTitle] = "拖曳圖片到這裡",
            [Txt.ImagePdfEmptySubtitle] = "或點擊「加入圖片」批次選擇",
            [Txt.ImagePdfEmptyFormats] = ".jpg .jpeg .png .bmp .tif .tiff .gif .webp",
            [Txt.ImagePdfSettingsTitle] = "PDF 設定",
            [Txt.ImagePdfPageSize] = "頁面大小",
            [Txt.ImagePdfPageSizeAuto] = "自動",
            [Txt.ImagePdfOrientation] = "頁面方向",
            [Txt.ImagePdfOrientationAuto] = "自動",
            [Txt.ImagePdfOrientationPortrait] = "直向",
            [Txt.ImagePdfOrientationLandscape] = "橫向",
            [Txt.ImagePdfMargin] = "頁面邊距",
            [Txt.ImagePdfMarginNone] = "無",
            [Txt.ImagePdfMarginSmall] = "10 mm",
            [Txt.ImagePdfMarginLarge] = "20 mm",
            [Txt.ImagePdfFitHint] = "自動時頁面跟隨圖片\n固定紙張依比例適配，不裁切",
            [Txt.ImagePdfMergeOption] = "合併為一個 PDF",
            [Txt.ImagePdfNameLabel] = "檔案名稱",
            [Txt.ImagePdfNamePlaceholderMerge] = "留空則使用合併命名",
            [Txt.ImagePdfNamePlaceholderSeparate] = "留空則使用圖片原檔名",
            [Txt.ImagePdfNamePreview] = "命名預覽：{0}",
            [Txt.ImagePdfNamePreviewMulti] = "{0} 等 {1} 個 PDF",
            [Txt.ImagePdfPageCountFormat] = "{0} 張圖片",
            [Txt.ImagePdfOutputCount] = "{0} 個 PDF",
            [Txt.ImagePdfPreviewTitle] = "預覽",
            [Txt.ImagePdfPreviewEmpty] = "選擇圖片後顯示預覽",
            [Txt.ImagePdfStart] = "開始轉換",
            [Txt.ImagePdfRunning] = "正在轉換…",
            [Txt.ImagePdfCancel] = "取消轉換",
            [Txt.ImagePdfCancelling] = "正在取消…",
            [Txt.ImagePdfReady] = "就緒 · 每張圖片單獨輸出 PDF",
            [Txt.ImagePdfReadyMerge] = "就緒 · 依清單順序合併為一個 PDF",
            [Txt.ImagePdfProcessing] = "正在轉換（{0}/{1}）：{2}",
            [Txt.ImagePdfDone] = "轉換完成 · {0} 頁 · 耗時 {1}",
            [Txt.ImagePdfDoneFiles] = "轉換完成 · {0} 個 PDF · 耗時 {1}",
            [Txt.ImagePdfDonePartial] = "轉換完成 · 成功 {0} 個，失敗 {1} 個 · 耗時 {2}",
            [Txt.ImagePdfCancelled] = "已取消 · 已處理 {0} 張圖片",
            [Txt.ImagePdfFailStatus] = "轉換失敗，請檢查圖片後重試",
            [Txt.ImagePdfFailTitle] = "轉換失敗",
            [Txt.ImagePdfNoFiles] = "請先加入要轉換的圖片。",
            [Txt.ImagePdfNeedFolder] = "請選擇儲存位置。",
            [Txt.ImagePdfUnreadable] = "無法讀取圖片「{0}」：檔案可能已損壞或格式不受支援。",
            [Txt.ImagePdfWriteError] = "無法寫入 PDF：\n{0}\n\n技術資訊：{1}",
            [Txt.ImagePdfFailGeneric] = "無法產生 PDF：{0}",
            [Txt.ImagePdfAdded] = "已加入 {0} 張圖片",
            [Txt.ImagePdfSkippedNonImage] = "已忽略 {0} 個非圖片檔案",
            [Txt.ImagePdfNothingAdded] = "沒有新增檔案：檔案重複、不存在或不是圖片",
            [Txt.MsgImagePdfBusy] = "正在轉換圖片，請等待完成後再關閉視窗。",
            [Txt.TipNavImagePdf] = "圖片轉 PDF：每張單獨輸出，或合併為一個 PDF",
            [Txt.Feature5] = "PDF 逐頁匯出為 PNG / JPEG / BMP / TIFF，預設 JPEG 300PPI、品質 100%",
            [Txt.Feature6] = "圖片批次轉 PDF：預設每張單獨輸出，可合併為一個 PDF，檔名可自訂",
            [Txt.Changelog452] = "4.5.2 · 2026-11-05",
            [Txt.ChangelogItems452] =
                "#新增\n" +
                "· 圖片轉 PDF 預設每張圖片單獨輸出一個 PDF\n" +
                "· 新增「合併為一個 PDF」開關，可依需求恢復合併輸出\n" +
                "· 輸出檔名可自訂；逐張輸出時自動追加 _001、_002 序號\n" +
                "#優化\n" +
                "· 逐張輸出時單張失敗會繼續處理其餘圖片，並彙總成功 / 失敗數量\n" +
                "#修復\n" +
                "· PDF 轉圖片全程唯讀來源檔，轉換前後原 PDF 的文字、圖像與頁面保持一致\n",
            [Txt.NavWordPdfTitle] = "Word 轉 PDF",
            [Txt.NavWordPdfSubtitle] = "Word 批次轉 PDF",
            [Txt.WordPdfPageTitle] = "Word 轉 PDF",
            [Txt.WordPdfPageSubtitle] = "Word 逐份或合併轉 PDF · 本機處理",
            [Txt.WordPdfFilesTitle] = "Word 文件",
            [Txt.WordPdfCountFormat] = "{0} 個文件",
            [Txt.WordPdfListHint] = "空白處拖曳可框選 · 依列表順序轉換",
            [Txt.WordPdfAdd] = "新增 Word",
            [Txt.WordPdfDialogTitle] = "選擇要轉換的 Word 文件",
            [Txt.WordPdfEmptyTitle] = "拖曳 Word 到這裡",
            [Txt.WordPdfEmptySubtitle] = "或點擊「新增 Word」批次選擇",
            [Txt.WordPdfEmptyFormats] = ".doc .docx .docm .dot .dotx .dotm .rtf .odt",
            [Txt.WordPdfSettingsTitle] = "轉換設定",
            [Txt.WordPdfMergeOption] = "合併成一個檔案",
            [Txt.WordPdfMergeHint] = "關閉時每個 Word 單獨輸出同名 PDF",
            [Txt.WordPdfNameLabel] = "檔案名稱",
            [Txt.WordPdfNamePlaceholder] = "留空則使用第一個 Word 的檔案名稱",
            [Txt.WordPdfSeparateHint] = "輸出檔案名稱與 Word 相同，重名自動編號",
            [Txt.WordPdfOutput] = "儲存位置",
            [Txt.WordPdfNamePreview] = "輸出：{0}",
            [Txt.WordPdfNamePreviewMulti] = "{0} 個 PDF，檔案名稱與 Word 相同",
            [Txt.WordPdfStart] = "開始轉換",
            [Txt.WordPdfRunning] = "轉換中…",
            [Txt.WordPdfCancel] = "取消",
            [Txt.WordPdfCancelling] = "正在取消…",
            [Txt.WordPdfReady] = "準備就緒 · 每份 Word 單獨輸出 PDF",
            [Txt.WordPdfReadyMerge] = "準備就緒 · 依列表順序合併為一個 PDF",
            [Txt.WordPdfEngineWord] = "Microsoft Word",
            [Txt.WordPdfEngineLibre] = "LibreOffice",
            [Txt.WordPdfProcessing] = "正在轉換（{0}/{1}）：{2}",
            [Txt.WordPdfMerging] = "正在合併 PDF 頁面…",
            [Txt.WordPdfDone] = "轉換完成 · {0} 頁 · {1}",
            [Txt.WordPdfDoneFiles] = "轉換完成 · {0} 個 PDF · {1}",
            [Txt.WordPdfCancelled] = "已取消 · 已完成 {0} 份",
            [Txt.WordPdfFailStatus] = "轉換失敗，請檢查文件後重試",
            [Txt.WordPdfFailTitle] = "轉換失敗",
            [Txt.WordPdfNoFiles] = "請先新增要轉換的 Word 文件。",
            [Txt.WordPdfNeedFolder] = "請選擇儲存位置。",
            [Txt.WordPdfNoEngine] = "未偵測到 Microsoft Word 或 LibreOffice，無法轉換 Word。",
            [Txt.WordPdfNonWord] = "不是支援的 Word 文件：{0}",
            [Txt.WordPdfWriteError] = "無法寫入 PDF：{0}",
            [Txt.WordPdfFailGeneric] = "轉換 Word 失敗：{0}",
            [Txt.WordPdfAdded] = "已新增 {0} 個文件",
            [Txt.WordPdfSkippedNonWord] = "已忽略 {0} 個非 Word 檔案",
            [Txt.WordPdfNothingAdded] = "沒有新增檔案：檔案重複、不存在或不是 Word",
            [Txt.WordPdfOutputCount] = "{0} 個 PDF",
            [Txt.MsgWordPdfBusy] = "正在轉換 Word，請等待完成後再關閉視窗。",
            [Txt.TipNavWordPdf] = "Word 轉 PDF：每份單獨輸出，或合併為一個檔案",
            [Txt.Feature7] = "Word 批次轉 PDF：預設依原檔名逐份輸出，可合併為一個檔案，並可為輸出加上浮水印",
            [Txt.Changelog4520] = "4.5.20 · 2026-11-10",
            [Txt.ChangelogItems4520] =
                "#優化\n" +
                "· 每日一言在啟動時與執行中隨機播放，不再依序輪換，連續兩次也不會重複\n" +
                "· 自動換句間隔改為秒，預設 30 秒，可在設定中自訂（5–86400 秒）\n",
            [Txt.Changelog4519] = "4.5.19 · 2026-11-10",
            [Txt.ChangelogItems4519] =
                "#優化\n" +
                "· 應用程式內所有提示框改用目前面板：確認、警告、錯誤與詢問視窗統一使用面板的底色、文字、圓角與按鈕樣式\n" +
                "· 密碼輸入框改為同一套無邊框面板視窗，深色面板下不再出現系統白色標題列\n",
            [Txt.Changelog4518] = "4.5.18 · 2026-11-10",
            [Txt.ChangelogItems4518] =
                "#新增\n" +
                "· 左側新增「Word 轉 PDF」工作區，支援 .doc/.docx/.docm/.dot/.dotx/.dotm/.rtf/.odt 批次轉換\n" +
                "· 預設依原檔名逐份輸出 PDF，重名自動編號；可開啟「合併成一個檔案」\n" +
                "· 浮水印設定新增「Word 轉 PDF」目標，僅作用於輸出 PDF，不改動來源文件\n" +
                "· 自動沿用 Microsoft Word 或 LibreOffice 完成轉換，缺少兩者時給出明確提示\n",
            [Txt.Changelog4517] = "4.5.17 · 2026-11-06",
            [Txt.ChangelogItems4517] =
                "#優化\n" +
                "· 移除名言庫中的古詩詞與文言文，全部替換為現代名言\n" +
                "#新增\n" +
                "· 新增國內外經典電影台詞與名人名言，每個介面語言 66 條、六語言合計 396 條\n" +
                "· 收錄《阿甘正傳》《刺激1995》《教父》《鐵達尼號》《哪吒之魔童降世》《一代宗師》等影片台詞\n" +
                "· 新增賈伯斯、曼德拉、甘地、馬丁·路德·金、培根、契訶夫等名人名言\n",
            [Txt.Changelog4516] = "4.5.16 · 2026-11-06",
            [Txt.ChangelogItems4516] =
                "#優化\n" +
                "· 「每日一言」卡片改為毛玻璃質感：半透明底色、頂部高光與圓角描邊，與整體玻璃風格統一\n" +
                "· 移除卡片左側的紅色豎線，夜間模式下不再出現刺眼的紅色裝飾\n" +
                "· 長句會自動縮小字號並完整顯示，避免被截斷\n" +
                "#新增\n" +
                "· 名言庫擴充至 348 條，每種介面語言各 58 條，涵蓋中外名人與經典電影台詞\n",
            [Txt.Changelog4515] = "4.5.15 · 2026-11-06",
            [Txt.ChangelogItems4515] =
                "#新增\n" +
                "· 每日一言新增自動換句，可在外觀設定中自訂換句間隔（預設 5 分鐘）\n" +
                "· 關閉自動換句後，仍可點擊卡片立即換一句\n",
            [Txt.Changelog4514] = "4.5.14 · 2026-11-06",
            [Txt.ChangelogItems4514] =
                "#修復\n" +
                "· 切換介面語言後，設定視窗不再整窗顯示紅叉或彈出未處理例外\n" +
                "· 玻璃背景快取被誤釋放後會自動重建，避免同類問題再次出現\n",
            [Txt.Changelog4513] = "4.5.13 · 2026-11-06",
            [Txt.ChangelogItems4513] =
                "#新增\n" +
                "· 合併頁右側新增「每日一言」卡片，展示世界名言與電影台詞並註明出處\n" +
                "· 內建簡體中文、繁體中文、English、日本語、한국어、Русский 六套語錄，切換介面語言時同步更換\n" +
                "· 外觀設定新增「在合併頁顯示名言或電影台詞」開關，點擊卡片可換一句\n",
            [Txt.Changelog4512] = "4.5.12 · 2026-11-06",
            [Txt.ChangelogItems4512] =
                "#調整\n" +
                "· 浮水印設定與完成提醒入口移到左下角，與面板、設定、關於排在一起\n" +
                "· 合併頁右側的提示文字上移，緊跟在進度條下方\n" +
                "#修復\n" +
                "· 刪除浮水印入口提示中「僅檔案合併生效」的舊說明\n",
            [Txt.Changelog4511] = "4.5.11 · 2026-11-06",
            [Txt.ChangelogItems4511] =
                "#調整\n" +
                "· 移除終端風面板，正在使用該面板的設定會自動回退到經典藍\n" +
                "#修復\n" +
                "· 修正右上角浮水印按鈕圖示在 19px 尺寸下顯示不全的問題\n",
            [Txt.Changelog4510] = "4.5.10 · 2026-11-06",
            [Txt.ChangelogItems4510] =
                "#修復\n" +
                "· 滑鼠一般左鍵點選（包括 Ctrl+點選）只保留目前檔案，上一列的選取狀態立即消失\n" +
                "· 只有框選與 Shift 連選保留多選\n",
            [Txt.Changelog459] = "4.5.9 · 2026-11-06",
            [Txt.ChangelogItems459] =
                "#修復\n" +
                "· 滑鼠一般左鍵點選檔案時只保留目前檔案，不再殘留上一列的選取狀態；列內上移 / 下移 / 移除按鈕同樣生效\n" +
                "· Ctrl / Shift 多選行為維持不變\n",
            [Txt.Changelog458] = "4.5.8 · 2026-11-06",
            [Txt.ChangelogItems458] =
                "#新增\n" +
                "· 面板入口移到左下角，位於設定上方，面板 / 設定 / 關於統一為扁平線性圖示\n" +
                "· 檔案清單新增「取消框選」按鈕，可一鍵還原框選前的選擇，按 Esc 同樣有效\n" +
                "· 刪除、清空、取消框選等清單按鈕增加滑鼠懸停提示\n" +
                "#優化\n" +
                "· 修正滑鼠左鍵框選時清單跳動的問題，框選過程更穩定\n" +
                "#調整\n" +
                "· 移除 HUD 科幻面板，已選該面板的設定會自動回退到經典藍\n",
            [Txt.Changelog457] = "4.5.7 · 2026-11-06",
            [Txt.ChangelogItems457] =
                "#新增\n" +
                "· 待處理區支援滑鼠框選：在清單空白處拖出選框即可批次選取檔案，三個工作區皆可使用\n" +
                "· 按住 Ctrl 框選會將新檔案追加到既有選擇\n" +
                "· 新增四套面板：擬物風、新擬態、終端風、HUD 科幻\n" +
                "#優化\n" +
                "· 框選後可批次移除選取的檔案\n" +
                "· 移除網易紅面板，已選該面板的設定會自動回退到經典藍\n",
            [Txt.Changelog456] = "4.5.6 · 2026-11-06",
            [Txt.ChangelogItems456] =
                "#優化\n" +
                "· PDF 轉圖片的預設輸出子資料夾會附上目前頁碼模式，如「報告_偶數頁」「報告_全部」，方便區分多次轉換結果\n",
            [Txt.Changelog455] = "4.5.5 · 2026-11-06",
            [Txt.ChangelogItems455] =
                "#新增\n" +
                "· PDF 轉圖片支援依全部、奇數頁、偶數頁或自訂頁碼範圍匯出\n" +
                "· 自訂範圍支援 1-3,5,8-10，並依真實頁碼命名輸出檔案\n" +
                "#優化\n" +
                "· 預覽與命名範例會跟隨目前頁碼模式顯示第一張圖片\n" +
                "· 舊版已填寫的頁碼範圍會自動遷移為「自訂」模式\n",
            [Txt.Changelog454] = "4.5.4 · 2026-11-05",
            [Txt.ChangelogItems454] =
                "#新增\n" +
                "· 全新扁平品牌圖示：品牌藍文件疊層 + 琥珀色合併徽標\n" +
                "· 應用內功能圖示改為扁平雙色風格，導覽與按鈕更統一、小尺寸更清晰\n" +
                "#修復\n" +
                "· 切換語言時設定視窗原地重新整理，不再關閉重開或跳回螢幕中央\n",
            [Txt.Changelog453] = "4.5.3 · 2026-11-05",
            [Txt.ChangelogItems453] =
                "#新增\n" +
                "· 浮水印設定可分別作用於文件合併、PDF 轉圖片、圖片轉 PDF，三個開關預設關閉\n" +
                "· PDF 轉圖片可在輸出圖片上疊加浮水印，來源 PDF 維持唯讀不變\n" +
                "#優化\n" +
                "· PDF 轉圖片預設改為 300PPI、JPEG 格式、品質 100%\n" +
                "· 完成提示音預設關閉；關閉時音效選項自動置灰，開啟後才可選擇\n",
            [Txt.Changelog451] = "4.5.1 · 2026-11-05",
            [Txt.ChangelogItems451] =
                "#新增\n" +
                "· 新增「圖片轉 PDF」工作區：圖片依清單順序合成一個 PDF\n" +
                "· 頁面大小可選 A4 / A3 / A5 / B5 / Letter 或跟隨圖片，支援頁面方向與邊距\n" +
                "· 命名、儲存位置與「完成後自動開啟」沿用文件合併設定\n" +
                "#優化\n" +
                "· 移除 Qoder 深色面板\n" +
                "#修復\n" +
                "· 修復經典藍、網易紅面板下左側功能欄文字對比度不足的問題\n",
            [Txt.Changelog440] = "4.4.0 · 2026-11-05",
            [Txt.ChangelogItems440] =
                "#新增\n" +
                "· 左側卡片式導覽：文件合併、PDF 轉圖片兩個工作區，可用 Ctrl+1 / Ctrl+2 切換\n" +
                "· 新增 PDF 轉圖片：PDF 逐頁匯出為 PNG / JPEG / BMP / TIFF，格式、PPI、品質、頁碼範圍均可設定\n" +
                "· 轉換支援批次、即時預覽、取消與逐檔失敗彙總；圖片保留所選 DPI 中繼資料\n" +
                "· 新增 Qoder 深色面板，新安裝預設使用\n" +
                "#優化\n" +
                "· 介面依 Qoder 風格重做：左側導覽卡片、頁首標題、單一強調色與更低對比的描邊\n" +
                "· 合併與轉換共用命名、儲存位置、完成提醒等設定；轉換使用獨立的「上次位置」\n" +
                "· 輸出名稱與儲存目錄可直接編輯；資料夾拖入改為遞迴掃描\n" +
                "· 設定頁新增「PDF 轉圖片」頁，浮水印頁註明僅對文件合併生效\n",

            [Txt.Changelog450] = "4.5.0 · 2026-11-05",
            [Txt.ChangelogItems450] =
                "#新增\n" +
                "· 軟體更名為「文樞 DocHub」：文件中樞，一鍵合轉\n" +
                "· 關於頁展示品牌定義，並補充 PDF 轉圖片能力說明\n" +
                "#優化\n" +
                "· 左側導覽「文件轉換」更名為「PDF 轉圖片」，與頁面標題、設定項保持一致\n" +
                "#修復\n" +
                "· 修復轉換頁「新增 PDF」按鈕被提示文字遮擋的問題\n" +
                "· 修復檔案計數文字壓住刪除/清空按鈕的問題\n" +
                "· 升級安裝保留原有設定：自動遷移舊版設定目錄\n",

            [Txt.DefaultDocName] = "合併文件",
            [Txt.DefaultConvertName] = "轉換圖片",
            [Txt.MergeTimePrefix] = "合併_",
            [Txt.ConvertTimePrefix] = "轉換_",
            [Txt.MergeCollideSuffix] = "_合併"
        };
    }
}
