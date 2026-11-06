using System.Collections.Generic;

namespace DocHub
{
    internal static class LocZhCn
    {
        public static readonly Dictionary<Txt, string> Table = new()
        {
            [Txt.AppName] = "文枢",
            [Txt.Tagline] = "文档中枢，一键合转 · Word / PDF / 图片 合并与转换",
            [Txt.LocalBadge] = "本地处理 · 文件不外传",

            [Txt.Ok] = "确定",
            [Txt.Close] = "关闭",
            [Txt.Cancel] = "取消",
            [Txt.Save] = "保存",
            [Txt.Reset] = "恢复默认",
            [Txt.Browse] = "浏览",
            [Txt.PreviewWord] = "试听",
            [Txt.Settings] = "设置",
            [Txt.About] = "关于",
            [Txt.RailMerge] = "合并",

            [Txt.DocsTitle] = "文件列表",
            [Txt.DocCountFormat] = "{0} 个文件",
            [Txt.ListHint] = "拖动列表项可调整顺序 · 支持批量拖入 Word / PDF / 图片",
            [Txt.AddDocs] = "添加文件",
            [Txt.MoveUp] = "上移",
            [Txt.MoveDown] = "下移",
            [Txt.MoveTop] = "置顶",
            [Txt.MoveBottom] = "置底",
            [Txt.Remove] = "删除",
            [Txt.Clear] = "清空",
            [Txt.OutputFile] = "输出文件",
            [Txt.SaveTo] = "保存到",
            [Txt.ChooseFolder] = "选择保存位置",
            [Txt.StartMerge] = "开始合并",
            [Txt.MergingButton] = "正在合并…",
            [Txt.ReadyStatus] = "就绪 · 按列表顺序从上到下合并",
            [Txt.NamePlaceholderFirst] = "自动取第一个文档名",
            [Txt.NamePlaceholderPhrase] = "第一个文档名 + 自定义语句",
            [Txt.NamePlaceholderTime] = "合并时间（自动生成）",
            [Txt.NamePlaceholderCustom] = "输入自定义文件名",
            [Txt.EmptyTitle] = "拖拽 Word / PDF / 图片到这里",
            [Txt.EmptySubtitle] = "或点击右上角「添加文件」批量选择",
            [Txt.EmptyFormats] = ".doc · .docx · .rtf · .pdf · .jpg · .png · .bmp · .tiff",

            [Txt.TipAdd] = "批量选择要合并的 Word / PDF / 图片文件",
            [Txt.TipUp] = "将选中的文件上移一位",
            [Txt.TipDown] = "将选中的文件下移一位",
            [Txt.TipRemove] = "从列表移除，不会删除源文件",
            [Txt.TipClear] = "清空整个文件列表",
            [Txt.TipBrowse] = "选择合并后 PDF 的保存位置",
            [Txt.TipList] = "可拖拽文件到这里，也可以拖动列表项调整顺序",
            [Txt.TipStart] = "按列表顺序合并并生成 PDF",
            [Txt.TipSettings] = "命名方式、皮肤、提示音与语言",
            [Txt.TipAbout] = "版本信息、更新日志与用户协议",
            [Txt.TrayOpen] = "打开主界面",
            [Txt.TrayExit] = "退出",

            [Txt.DialogTitleAdd] = "选择要合并的文档",
            [Txt.FolderDesc] = "选择合并后 PDF 的保存位置",
            [Txt.FilterSupport] = "支持的文件",
            [Txt.FilterWord] = "Word 文档",
            [Txt.FilterPdf] = "PDF 文件",
            [Txt.FilterImage] = "图片文件",
            [Txt.FilterAll] = "所有文件",

            [Txt.StatusAdded] = "已添加 {0} 个文件 · 按列表顺序合并",
            [Txt.StatusSkipped] = "（跳过 {0} 个重复或不支持的文件）",
            [Txt.StatusNothing] = "没有新增文件：文件重复、不存在或格式不受支持",
            [Txt.StatusOrder] = "顺序已调整 · 按当前列表顺序合并",
            [Txt.StatusRemoved] = "已从列表移除：{0}",
            [Txt.StatusRemovedMulti] = "已从列表移除 {0} 个文档",
            [Txt.StatusMovedTop] = "已置顶 · 选中的文档排到最前",
            [Txt.StatusMovedBottom] = "已置底 · 选中的文档排到最后",
            [Txt.StatusCleared] = "列表已清空",
            [Txt.StatusFolderChosen] = "已选择保存位置",

            [Txt.MsgClearTitle] = "清空列表",
            [Txt.MsgClearText] = "确定清空整个文档列表吗？\n\n源文件不会被删除。",
            [Txt.MsgMergeBusy] = "正在合并文档，请等待完成后再关闭窗口。",
            [Txt.MsgNoWordTitle] = "缺少 Microsoft Word",
            [Txt.MsgNoWordText] = "未检测到 Microsoft Word，无法转换 Word 文档。\n\n请先安装 Microsoft Word；纯 PDF 与图片仍可直接合并。",
            [Txt.MsgOverwriteTitle] = "覆盖确认",
            [Txt.MsgOverwriteText] = "目标文件已存在：\n{0}\n\n是否覆盖？",

            [Txt.StatusPreparing] = "正在准备…",
            [Txt.StatusProgress] = "{0}% · {1}",
            [Txt.StatusDone] = "合并完成 · 共 {0} 页 · 用时 {1}",
            [Txt.BalloonTitle] = "合并完成",
            [Txt.BalloonText] = "共 {0} 页 · 用时 {1}\n{2}",
            [Txt.OpenFailed] = "无法打开 PDF，请手动前往保存位置查看。",
            [Txt.FailStatus] = "合并失败，请检查文件后重试",
            [Txt.FailTitle] = "合并失败",
            [Txt.FailPrefix] = "合并失败：{0}",

            [Txt.ErrCannotStartTitle] = "无法开始合并",
            [Txt.ErrNeedDocs] = "请先添加要合并的文档。",
            [Txt.ErrNeedName] = "请填写输出文件名。",
            [Txt.ErrBadChars] = "输出文件名不能包含下列字符：\\ / : * ? \" < > |",
            [Txt.ErrNeedFolder] = "请选择保存位置。",
            [Txt.ErrFolderMissing] = "保存位置不存在：\n{0}",
            [Txt.ErrPathInvalid] = "输出路径无效。",
            [Txt.ErrSameSource] = "输出文件与某个源文档同名，继续会覆盖源文件。\n\n请修改输出文件名或保存位置。",
            [Txt.ElapsedSeconds] = "{0:0.0} 秒",
            [Txt.ElapsedMinutes] = "{0} 分 {1} 秒",

            [Txt.SettingsTitle] = "设置",
            [Txt.NamingSection] = "输出文件命名",
            [Txt.NamingHint] = "合并完成后按此方式生成文件名",
            [Txt.NamingFirst] = "按最上方文件的文件名",
            [Txt.NamingPhrase] = "最上方文件名 + 自定义语句",
            [Txt.NamingTime] = "合并时间（自动生成）",
            [Txt.NamingCustom] = "使用自定义名称",
            [Txt.PhrasePlaceholder] = "例如：终稿",
            [Txt.CustomPlaceholder] = "例如：2026年度合同汇总",
            [Txt.PreviewFormat] = "命名预览：{0}.pdf",
            [Txt.SkinSection] = "外观皮肤",
            [Txt.SkinHint] = "选择界面风格，切换后立即生效",
            [Txt.DoneSection] = "合并完成后",
            [Txt.DoneNotify] = "在电脑右下角弹出完成提示",
            [Txt.DoneSound] = "播放完成提示音",
            [Txt.DoneOpen] = "完成后自动打开生成的 PDF（默认关闭）",
            [Txt.SoundSection] = "提示音",
            [Txt.SoundWater] = "水滴",
            [Txt.SoundDing] = "叮咚",
            [Txt.SoundQQ] = "QQ",
            [Txt.SoundChime] = "清脆",
            [Txt.SoundSoft] = "柔和",
            [Txt.SoundOff] = "静音",
            [Txt.SoundHint] = "先打开提示音，再选择音效；选中即自动播放",
            [Txt.LanguageSection] = "界面语言",
            [Txt.LanguageHint] = "切换后立即生效",
            [Txt.OutputSection] = "保存位置",
            [Txt.OutputHint] = "合并后的 PDF 保存在哪里",
            [Txt.OutputFirst] = "最上方文件所在的文件夹",
            [Txt.OutputLast] = "上一次保存的位置",
            [Txt.OutputCustom] = "自定义文件夹",
            [Txt.OutputCustomPlaceholder] = "选择或输入文件夹路径",
            [Txt.SkinBlueName] = "经典蓝",
            [Txt.SkinBlueDesc] = "清爽通透",
            [Txt.SkinRedName] = "网易红",
            [Txt.SkinRedDesc] = "经典云音乐",
            [Txt.SkinNightName] = "夜间模式",
            [Txt.SkinNightDesc] = "沉浸暗色",
            [Txt.SkinCloudName] = "云白",
            [Txt.SkinCloudDesc] = "暖白纸感",
            [Txt.SettingsLiveHint] = "所有修改即时生效",

            [Txt.AboutTitle] = "关于",
            [Txt.VersionLine] = "{0} · Word / PDF / 图片 合并与转换工具",
            [Txt.AboutIntro] = "所有处理均在本机完成，文件不外传",
            [Txt.BrandDefinition] = "文枢：文档中枢，一键合转",
            [Txt.Feature1] = "Word 全格式、PDF 与图片混搭合并，批量导入无大小限制",
            [Txt.Feature2] = "保留原排版、页码、图片尺寸与方向，不做二次排版",
            [Txt.Feature3] = "拖拽排序、实时进度，输出顺序完全可控",
            [Txt.Feature4] = "本地离线处理，文档不会上传到任何服务器",
            [Txt.ChangelogTitle] = "更新日志",
            [Txt.Changelog300] = "3.0.0 · 2026-11-03",
            [Txt.ChangelogItems300] =
                "#新增\n" +
                "· 安装包内置完整运行库，未安装 .NET 的电脑也能直接安装使用\n" +
                "· 内置简易 PDF 阅读器：本机没有 PDF 阅读器时也能查看合并结果\n" +
                "· 关于页新增作者署名与联系方式（cado@163.com）\n" +
                "#优化\n" +
                "· 设置项修改后立即生效，无需再点「保存」\n" +
                "· 提示音选中即自动播放，无需再点「试听」\n" +
                "· 合并速度提升：图片并行转换，结果直接写入目标文件\n" +
                "· 安装后的主程序保持精简，运行库随安装包一起分发\n" +
                "#修复\n" +
                "· 修复图标图形重心偏上，调整为垂直居中\n" +
                "· 修复未安装 Word 时无法合并 PDF 与图片的问题：支持 LibreOffice，或跳过 Word 继续合并",
            [Txt.Changelog200] = "2.0.0 · 2026-11-03",
            [Txt.ChangelogItems200] =
                "#新增\n" +
                "· 支持 JPG、PNG、BMP、TIFF 等图片直接合并，保持原始尺寸与方向\n" +
                "· Word、PDF、图片可在同一列表混合合并，统一输出为 PDF\n" +
                "· 界面语言默认跟随系统语言，安装后即为对应语言\n" +
                "#优化\n" +
                "· 主界面与提示统一为「文件」描述，列表区分 PDF / WORD / 图片\n" +
                "· 设置界面全面扁平化：纯色分区、细线分隔，去掉渐变与阴影\n" +
                "· 更新日志按「新增 / 优化 / 修复」分类展示，重点更清晰\n" +
                "#修复\n" +
                "· 修复仅含 PDF 或图片时仍提示缺少 Word 的问题",
            [Txt.Changelog130] = "1.3.0 · 2026-11-03",
            [Txt.ChangelogItems130] =
                "#新增\n" +
                "· 新增保存位置设置：最上方文件位置、上一次保存位置、自定义路径\n" +
                "· 文件列表支持右键删除、上移、下移\n" +
                "· 提供电脑安装程序与绿色版（不压缩）两种打包\n" +
                "#优化\n" +
                "· 设置保存后立即生效，无需重启软件\n" +
                "· 进度条跟随主题配色，并显示合并百分比\n" +
                "· 文件列表焕新：卡片式行、类型徽标与文件信息\n" +
                "#修复\n" +
                "· 修复列表抖动与闪烁\n" +
                "· 修复同名文件覆盖问题，自动编号为 A-1.pdf",
            [Txt.Changelog120] = "1.2.0 · 2026-11-03",
            [Txt.ChangelogItems120] =
                "#新增\n" +
                "· 全新扁平化应用图标，无文字，造型更简洁\n" +
                "· 5 种内置完成提示音：水滴、叮咚、QQ 等，支持试听\n" +
                "· 关于页新增《用户协议》\n" +
                "· 新增繁体中文、英文、日文、俄文、韩文界面，可即时切换\n" +
                "#优化\n" +
                "· 全面美化界面细节：圆角、阴影、留白与交互反馈",
            [Txt.Changelog110] = "1.1.0 · 2026-11-03",
            [Txt.ChangelogItems110] =
                "#新增\n" +
                "· 4 种命名方式：首文件名、首文件名＋自定义语句、合并时间、自定义名称\n" +
                "· 4 套网易云风皮肤：经典蓝、网易红、夜间模式、云白\n" +
                "#优化\n" +
                "· 新增「设置」与「关于」入口，图标改为扁平风格\n" +
                "· 合并完成默认不打开 PDF，改为右下角提示与提示音，可自定义",
            [Txt.Changelog100] = "1.0.0 · 2026-11-02",
            [Txt.ChangelogItems100] =
                "#新增\n" +
                "· 首个版本：Word 全格式与 PDF 混搭合并、批量导入、拖拽排序、进度显示",
            [Txt.ContactAuthor] = "作者：AI · 构思：chao",
            [Txt.Copyright] = "© 2026 AI · 本地处理更安心",
            [Txt.EulaButton] = "用户协议",
            [Txt.EulaTitle] = "用户协议",
            [Txt.EulaIntro] = "使用「文枢」前，请阅读以下条款。",
            [Txt.EulaBody] =
                "一、用途说明\n" +
                "「文枢」是一款在本地运行的 Word / PDF / 图片 合并与转换工具。软件调用本机安装的 Microsoft Word 或 LibreOffice " +
                "将 Word 文档导出为 PDF，图片按原始尺寸排成页面，再按您指定的顺序逐页合并。\n\n" +
                "二、您的文件\n" +
                "所有处理均在本机完成。软件不会上传、传输或备份任何文档，也不会收集您的个人信息或文档内容。\n\n" +
                "三、使用责任\n" +
                "您应确保对所合并的文档拥有合法使用权。合并前请自行备份重要文件。因文件损坏、被覆盖或使用不当" +
                "造成的损失，作者不承担责任。\n\n" +
                "四、运行环境\n" +
                "合并 Word 文档需要本机安装 Microsoft Word 或 LibreOffice。本软件不提供、不激活 Office。\n\n" +
                "五、免责声明\n" +
                "软件按现状提供。我们会尽力测试，但不保证在任何环境下所有文档都能完美合并。\n\n" +
                "六、条款更新\n" +
                "本协议可能随版本更新而调整，继续使用即表示您接受更新后的条款。",
            [Txt.EulaAgree] = "我已阅读并同意",

            [Txt.MergeNoDocs] = "请先添加要合并的文档。",
            [Txt.MergeFileMissing] = "文件不存在或已被移动：\n{0}",
            [Txt.MergeWordMissing] =
                "未检测到 Microsoft Word 或 LibreOffice，无法转换 Word 文档。\n\n" +
                "请安装 Microsoft Word 或 LibreOffice 后再合并 Word 文件；纯 PDF 与图片可以直接合并。",
            [Txt.MergeWordStartFail] = "无法启动 Microsoft Word。请确认 Word 已正确安装并可正常打开。",
            [Txt.MergeWordNotInstalled] = "未检测到 Microsoft Word 或 LibreOffice，请安装后再试。",
            [Txt.MergeProcessing] = "正在处理（{0}/{1}）：{2}",
            [Txt.MergeStartingWord] = "正在启动 Word 转换引擎…",
            [Txt.MergeMergingPages] = "正在合并页面…",
            [Txt.MergeMerging] = "正在合并（{0}/{1}）：{2}",
            [Txt.MergePdfUnreadable] = "无法读取 PDF「{0}」：文件可能已损坏、加密或格式不受支持。",
            [Txt.MergeNoPages] = "文档中没有可合并的页面。",
            [Txt.MergePdfError] = "合并 PDF 时发生错误，输出文件未生成。",
            [Txt.MergeWriteError] =
                "无法写入输出文件：\n{0}\n\n请确认文件未被其他程序占用，且保存位置可写。\n\n技术信息：{1}",
            [Txt.MergeNoPermission] = "没有权限写入输出文件：\n{0}\n\n请更换保存位置后重试。",
            [Txt.MergeFailedGeneric] = "合并失败：{0}",
            [Txt.MergeTempFail] = "无法创建合并用的临时目录。\n\n可能是安全软件阻止了文件写入，或临时目录权限异常。\n请把本程序加入安全软件白名单，或以管理员身份运行后重试。",
            [Txt.MergeDone] = "合并完成",
            [Txt.MergeWordOpenFail] = "无法打开「{0}」。",
            [Txt.MergeWordConvertEmpty] = "「{0}」转换后没有得到有效页面。",
            [Txt.MergeWordConvertFail] =
                "无法转换 Word 文档「{0}」。\n\n" +
                "请确认该文件没有加密、没有损坏，并且本机 Microsoft Word 可以正常打开它。\n\n技术信息：{1}",
            [Txt.MergePasswordTitle] = "文件已加密",
            [Txt.MergePasswordHint] = "请输入打开该文件的密码，仅在本次合并中使用。",
            [Txt.MergePasswordPlaceholder] = "打开密码",
            [Txt.MergePasswordWrong] = "密码不正确，请重新输入。",
            [Txt.MergeImageConvertFail] =
                "无法读取图片「{0}」。\n\n" +
                "请确认图片没有损坏。JPG / PNG / BMP / TIFF / GIF / WebP 可直接合并；" +
                "HEIC / HEIF / AVIF 需要系统已安装对应的解码扩展。\n\n技术信息：{1}",

            [Txt.MsgSkipWordTitle] = "Word 文件无法转换",
            [Txt.MsgSkipWordText] =
                "未检测到 Microsoft Word 或 LibreOffice，{0} 个 Word 文件无法转换。\n\n" +
                "是否跳过这些 Word 文件，继续合并其余 PDF 与图片？",
            [Txt.StatusSkippedWord] = "已跳过 {0} 个 Word 文件（未安装 Word / LibreOffice）",

            [Txt.ViewResult] = "查看合并文档",
            [Txt.PdfViewerTitle] = "PDF 阅读器",
            [Txt.PrevPage] = "上一页",
            [Txt.NextPage] = "下一页",
            [Txt.ZoomIn] = "放大",
            [Txt.ZoomOut] = "缩小",
            [Txt.ViewerPageFormat] = "第 {0} / {1} 页",
            [Txt.ViewerNoReader] = "本机未检测到 PDF 阅读器，已启用内置阅读器",
            [Txt.ViewerLoadFailed] = "无法打开 PDF 文件：\n{0}",

            [Txt.SettingsNavLook] = "外观",
            [Txt.SettingsNavNaming] = "命名与保存",
            [Txt.SettingsNavDone] = "完成提醒",
            [Txt.SettingsNavWatermark] = "水印",
            [Txt.WatermarkSection] = "文档水印",
            [Txt.WatermarkHint] = "勾选需要添加水印的功能；样式三项共用，默认关闭",
            [Txt.WatermarkEnable] = "添加水印",
            [Txt.WatermarkForMerge] = "文档合并",
            [Txt.WatermarkForPdfImage] = "PDF 转图片",
            [Txt.WatermarkForImagePdf] = "图片转 PDF",
            [Txt.WatermarkPresets] = "预置文字",
            [Txt.WatermarkCustom] = "自定义",
            [Txt.WatermarkPreset1] = "正本",
            [Txt.WatermarkPreset2] = "副本",
            [Txt.WatermarkPreset3] = "机密",
            [Txt.WatermarkPreset4] = "绝密",
            [Txt.WatermarkPreview] = "效果预览",
            [Txt.WatermarkCustomPlaceholder] = "输入水印文字",
            [Txt.WatermarkPreviewHint] = "预览中拖动可移动、滚轮缩放、右键拖动旋转；数值也可直接键入",
            [Txt.WatermarkTint] = "水印颜色",
            [Txt.WatermarkTintGray] = "灰",
            [Txt.WatermarkTintRed] = "红",
            [Txt.WatermarkTintBlue] = "蓝",
            [Txt.WatermarkPosition] = "位置",
            [Txt.WatermarkPosTile] = "平铺",
            [Txt.WatermarkPosTopLeft] = "左上",
            [Txt.WatermarkPosTopRight] = "右上",
            [Txt.WatermarkPosCenter] = "居中",
            [Txt.WatermarkPosBottomLeft] = "左下",
            [Txt.WatermarkPosBottomRight] = "右下",
            [Txt.WatermarkOpacity] = "不透明度",
            [Txt.WatermarkScale] = "大小",
            [Txt.WatermarkRotation] = "倾斜角度",
            [Txt.WatermarkOffsetX] = "水平位置",
            [Txt.WatermarkOffsetY] = "垂直位置",
            [Txt.ToastTitle] = "合并完成",
            [Txt.ToastOpen] = "查看",
            [Txt.Changelog310] = "3.1.0 · 2026-11-03",
            [Txt.ChangelogItems310] =
                "#新增\n" +
                "· 设置界面改为「左侧目录 + 右侧内容」两级结构\n" +
                "· 新增文档水印：内置正本/副本/机密/绝密，可自定义文字、风格与颜色\n" +
                "#优化\n" +
                "· 安装目录分层：精简「DocHub.exe」启动器 + app 程序目录，顶层更清爽\n" +
                "· 合并完成提醒改为与界面皮肤一致的右下角卡片\n",
            [Txt.Changelog400] = "4.0.0 · 2026-11-03",
            [Txt.ChangelogItems400] =
                "#新增\n" +
                "· 文件列表支持多选批量操作，右键可一键「置顶」或「置底」\n" +
                "#优化\n" +
                "· 文件列表行样式更清爽，选中状态更醒目\n" +
                "· 窗口缩放时自动重排布局，修复状态行压住「开始合并」按钮\n" +
                "· 切换界面语言后，默认输出名同步改为对应语言\n" +
                "· 恢复默认设置时，语言一并恢复为简体中文\n" +
                "· 安装包默认安装到 C:\\Program Files (x86)\\DocHub\n",
            [Txt.Changelog430] = "4.3.0 · 2026-11-04",
            [Txt.ChangelogItems430] =
                "#新增\n" +
                "· 主界面导航移到右上角：外观、水印、完成提醒、设置、关于以扁平图标排列，点击直达对应设置页\n" +
                "#修复\n" +
                "· 水印过大或拖到页面边缘时会被裁掉：现在自动等比缩放并限制在页面内，合并后完整显示\n" +
                "· 预览中的水印不再溢出预览框\n" +
                "#优化\n" +
                "· 重新合成「水滴」「QQ」提示音，起音更清脆、余韵更自然，并统一各提示音响度\n" +
                "· 移除主界面左侧栏，界面更简洁\n",
            [Txt.Changelog420] = "4.2.0 · 2026-11-04",
            [Txt.ChangelogItems420] =
                "#优化\n" +
                "· 设置、关于窗口秒开：磨砂背景按皮肤与尺寸缓存，并在主界面显示后提前生成\n" +
                "· 水印预览拖动更顺滑：平铺改为预生成贴图填充，最低缩放下单帧从 190ms 降到 1ms 以内\n" +
                "#改进\n" +
                "· 左侧导航重新排版：图标与名称层级更清晰，选中态为强调色胶囊 + 左侧竖条\n" +
                "· 版本号按语义化版本（SemVer）规则迭代：体验与性能改进升次版本号\n",
            [Txt.Changelog412] = "4.1.2 · 2026-11-04",
            [Txt.ChangelogItems412] =
                "#新增\n" +

                "· 水印位置可预设：平铺 / 左上 / 右上 / 居中 / 左下 / 右下\n" +

                "#优化\n" +

                "· 水印设置不再卡顿：连续拖动只提交一次，预览绘制缓存字体与度量\n" +

                "· 主界面左侧导航显示功能名称（合并 / 设置 / 关于），选中项以主题色高亮\n",

            [Txt.Changelog411] = "4.1.1 · 2026-11-04",
            [Txt.ChangelogItems411] =
                "#修复\n" +
                "· 安装程序不再强制改回默认目录：输入或浏览的自定义安装位置会原样生效\n" +
                "· 修复导入图片后无法合并的问题：WebP / HEIC 等格式改由系统解码器处理，普通图片兼容性也更强\n" +
                "#优化\n" +
                "· 水印各参数支持直接键入数值；预览中拖动可移动、滚轮缩放、右键拖动旋转\n" +
                "· 移除「水印风格」选项，统一为斜纹平铺效果\n",

            [Txt.Changelog410] = "4.1.0 · 2026-11-04",
            [Txt.ChangelogItems410] =
                "#新增\n" +
                "· 水印支持任意透明度、大小、位置与倾斜方向，设置页实时预览\n" +
                "· 安装包改为自研安装器，安装到当前用户目录，无需管理员权限、不写入 Program Files\n" +
                "· 开始菜单新增「卸载 文枢」快捷方式\n" +
                "#优化\n" +
                "· 设置界面加高，水印参数改为滑块调节，操作更直观\n" +
                "· 启动器更名为 DocHub.exe，修复安装时提示「无法打开要写入的文件」\n" +
                "· 安装与卸载前自动关闭正在运行的程序，升级安装不再占用文件\n",

            [Txt.NavMergeTitle] = "文档合并",
            [Txt.NavMergeSubtitle] = "合并为 PDF",
            [Txt.NavConvertTitle] = "PDF 转图片",
            [Txt.NavConvertSubtitle] = "逐页导出图片",
            [Txt.MergePageTitle] = "文档合并",
            [Txt.MergePageSubtitle] = "按列表顺序把 Word / PDF / 图片合成一个 PDF",
            [Txt.ConvertPageTitle] = "PDF 转图片",
            [Txt.ConvertPageSubtitle] = "把 PDF 的每一页导出为图片 · 只读源文件，不修改原文",
            [Txt.ConvertFilesTitle] = "PDF 文件",
            [Txt.ConvertCountFormat] = "{0} 个文件",
            [Txt.ConvertListHint] = "添加 PDF 后自动读取页数 · 支持批量转换",
            [Txt.ConvertAdd] = "添加 PDF",
            [Txt.ConvertDialogTitle] = "选择要转换的 PDF",
            [Txt.ConvertEmptyTitle] = "拖拽 PDF 到这里",
            [Txt.ConvertEmptySubtitle] = "或点击「添加 PDF」批量选择",
            [Txt.ConvertEmptyFormats] = ".pdf",
            [Txt.ConvertPageCountFormat] = "{0} 页",
            [Txt.ConvertPageCountUnknown] = "读取页数…",
            [Txt.ConvertPageCountFailed] = "无法读取",
            [Txt.ConvertSettingsTitle] = "导出设置",
            [Txt.ConvertFormat] = "图片格式",
            [Txt.ConvertPpi] = "分辨率（PPI）",
            [Txt.ConvertPpiCustom] = "自定义",
            [Txt.ConvertPpiHint] = "PPI 决定图片清晰度，不改变页面尺寸",
            [Txt.ConvertQuality] = "JPEG 质量",
            [Txt.ConvertBackground] = "透明背景",
            [Txt.ConvertBackgroundHint] = "仅 PNG 支持透明，其他格式为白底",
            [Txt.ConvertPageRange] = "页码范围",
            [Txt.ConvertPageRangeAll] = "全部",
            [Txt.ConvertPageRangePlaceholder] = "例：1-3,5,8-10",
            [Txt.ConvertPageModeAll] = "全部",
            [Txt.ConvertPageModeOdd] = "奇数页",
            [Txt.ConvertPageModeEven] = "偶数页",
            [Txt.ConvertPageModeCustom] = "自定义",
            [Txt.ConvertSubfolder] = "每个 PDF 单独建文件夹",
            [Txt.ConvertSubfolderHint] = "批量转换时自动开启；文件夹名附带页码模式",
            [Txt.ConvertOutput] = "保存位置",
            [Txt.ConvertNamePreview] = "命名预览：{0}",
            [Txt.ConvertEstimateFormat] = "预计 {0} × {1} px",
            [Txt.ConvertPreviewTitle] = "第 {0} 页预览",
            [Txt.ConvertPreviewEmpty] = "选择 PDF 后显示当前页码的预览",
            [Txt.ConvertStart] = "开始转换",
            [Txt.ConvertRunning] = "正在转换…",
            [Txt.ConvertCancel] = "取消转换",
            [Txt.ConvertCancelling] = "正在取消…",
            [Txt.ConvertReady] = "就绪 · 按列表顺序导出图片",
            [Txt.ConvertProcessing] = "正在转换（{0}/{1}）：{2}",
            [Txt.ConvertPageStatus] = "第 {0}/{1} 页",
            [Txt.ConvertDone] = "转换完成 · 共 {0} 张图片 · 用时 {1}",
            [Txt.ConvertDonePartial] = "完成 {0} 个 · 失败 {1} 个 · 共 {2} 张图片",
            [Txt.ConvertCancelled] = "已取消 · 已导出 {0} 张图片",
            [Txt.ConvertFailStatus] = "转换失败，请检查文件后重试",
            [Txt.ConvertFailTitle] = "转换失败",
            [Txt.ConvertOpenFolder] = "打开文件夹",
            [Txt.ConvertOutputSubfolder] = "输出：{0}",
            [Txt.ConvertNoFiles] = "请先添加要转换的 PDF 文件。",
            [Txt.ConvertNeedFolder] = "请选择保存位置。",
            [Txt.ConvertBadRange] = "页码范围格式不正确，请使用如 1-3,5 的写法。",
            [Txt.ConvertRangeOut] = "页码范围超出文档页数（共 {0} 页）。",
            [Txt.ConvertNoPages] = "文档中没有可转换的页面。",
            [Txt.ConvertRangeNeeded] = "请填写页码范围，或选择「全部」。",
            [Txt.ConvertNoMatchingPages] = "当前页码设置下没有可转换的页面。",
            [Txt.ConvertUnreadable] = "无法读取 PDF「{0}」：文件可能已损坏、加密或格式不受支持。",
            [Txt.ConvertPpiTooLarge] = "所选 PPI 会让页面过大（{0} × {1} px），请降低 PPI。",
            [Txt.ConvertWriteError] = "无法写入图片：\n{0}\n\n技术信息：{1}",
            [Txt.ConvertMemory] = "内存不足，无法渲染该页面。请降低 PPI 后重试。",
            [Txt.ConvertSkippedNonPdf] = "已忽略 {0} 个非 PDF 文件",
            [Txt.ConvertDuplicate] = "已跳过 {0} 个重复文件",
            [Txt.ConvertAdded] = "已添加 {0} 个 PDF 文件",
            [Txt.ConvertNothingAdded] = "没有新增文件：文件重复、不存在或不是 PDF",
            [Txt.MsgConvertBusy] = "正在转换文档，请等待完成后再关闭窗口。",
            [Txt.SettingsNavConvert] = "PDF 转图片",
            [Txt.ConvertSection] = "PDF 转图片默认值",
            [Txt.ConvertDefaultsHint] = "转换页会以这些值作为初始设置，在转换页修改后会记住",
            [Txt.ConvertDefaultFormat] = "默认图片格式",
            [Txt.ConvertDefaultPpi] = "默认 PPI",
            [Txt.ConvertDefaultQuality] = "默认 JPEG 质量",
            [Txt.ConvertDefaultBackground] = "默认透明背景",
            [Txt.ConvertDefaultSubfolder] = "默认每个 PDF 建立子文件夹",
            [Txt.ConvertWatermarkNote] = "水印叠加在输出图片上，源 PDF 始终只读；可在「水印」页开启",
            [Txt.NamingHintShared] = "合并与 PDF 转图片共用的输出命名方式",
            [Txt.DoneOpenShared] = "完成后自动打开结果（合并打开 PDF，转换打开文件夹）",
            [Txt.TipNavMerge] = "文档合并：按列表顺序合成一个 PDF",
            [Txt.TipNavConvert] = "PDF 转图片：把 PDF 每一页导出为图片",
            [Txt.TipAppearance] = "打开外观设置",
            [Txt.TipNotifications] = "打开完成提醒设置",
            [Txt.TipWatermark] = "打开水印设置（仅文档合并生效）",
            [Txt.NavImagePdfTitle] = "图片转 PDF",
            [Txt.NavImagePdfSubtitle] = "图片逐张或合并转 PDF",
            [Txt.ImagePdfPageTitle] = "图片转 PDF",
            [Txt.ImagePdfPageSubtitle] = "图片逐张输出或合并为一个 PDF · 本地处理",
            [Txt.ImagePdfFilesTitle] = "图片文件",
            [Txt.ImagePdfCountFormat] = "{0} 个文件",
            [Txt.ImagePdfListHint] = "按列表顺序转换 · 默认每张图片单独输出",
            [Txt.ImagePdfAdd] = "添加图片",
            [Txt.ImagePdfDialogTitle] = "选择要转换的图片",
            [Txt.ImagePdfEmptyTitle] = "拖拽图片到这里",
            [Txt.ImagePdfEmptySubtitle] = "或点击「添加图片」批量选择",
            [Txt.ImagePdfEmptyFormats] = ".jpg .jpeg .png .bmp .tif .tiff .gif .webp",
            [Txt.ImagePdfSettingsTitle] = "PDF 设置",
            [Txt.ImagePdfPageSize] = "页面大小",
            [Txt.ImagePdfPageSizeAuto] = "自动",
            [Txt.ImagePdfOrientation] = "页面方向",
            [Txt.ImagePdfOrientationAuto] = "自动",
            [Txt.ImagePdfOrientationPortrait] = "纵向",
            [Txt.ImagePdfOrientationLandscape] = "横向",
            [Txt.ImagePdfMargin] = "页边距",
            [Txt.ImagePdfMarginNone] = "无",
            [Txt.ImagePdfMarginSmall] = "10 mm",
            [Txt.ImagePdfMarginLarge] = "20 mm",
            [Txt.ImagePdfFitHint] = "自动时页面跟随图片\n固定纸张按比例适配，不裁切",
            [Txt.ImagePdfMergeOption] = "合并为一个 PDF",
            [Txt.ImagePdfNameLabel] = "文件名",
            [Txt.ImagePdfNamePlaceholderMerge] = "留空则使用合并命名",
            [Txt.ImagePdfNamePlaceholderSeparate] = "留空则使用图片原文件名",
            [Txt.ImagePdfNamePreview] = "命名预览：{0}",
            [Txt.ImagePdfNamePreviewMulti] = "{0} 等 {1} 个 PDF",
            [Txt.ImagePdfPageCountFormat] = "{0} 张图片",
            [Txt.ImagePdfOutputCount] = "{0} 个 PDF",
            [Txt.ImagePdfPreviewTitle] = "预览",
            [Txt.ImagePdfPreviewEmpty] = "选择图片后显示预览",
            [Txt.ImagePdfStart] = "开始转换",
            [Txt.ImagePdfRunning] = "转换中…",
            [Txt.ImagePdfCancel] = "取消",
            [Txt.ImagePdfCancelling] = "正在取消…",
            [Txt.ImagePdfReady] = "准备就绪 · 每张图片单独输出 PDF",
            [Txt.ImagePdfReadyMerge] = "准备就绪 · 按列表顺序合并为一个 PDF",
            [Txt.ImagePdfProcessing] = "正在转换（{0}/{1}）：{2}",
            [Txt.ImagePdfDone] = "转换完成 · {0} 页 · {1}",
            [Txt.ImagePdfDoneFiles] = "转换完成 · {0} 个 PDF · {1}",
            [Txt.ImagePdfDonePartial] = "转换完成 · 成功 {0} 个，失败 {1} 个 · {2}",
            [Txt.ImagePdfCancelled] = "已取消 · 已处理 {0} 张图片",
            [Txt.ImagePdfFailStatus] = "转换失败，请检查图片后重试",
            [Txt.ImagePdfFailTitle] = "转换失败",
            [Txt.ImagePdfNoFiles] = "请先添加要转换的图片。",
            [Txt.ImagePdfNeedFolder] = "请选择保存位置。",
            [Txt.ImagePdfUnreadable] = "无法读取图片「{0}」：文件可能已损坏或格式不受支持。",
            [Txt.ImagePdfWriteError] = "无法写入 PDF：\n{0}\n\n技术信息：{1}",
            [Txt.ImagePdfFailGeneric] = "无法生成 PDF：{0}",
            [Txt.ImagePdfAdded] = "已添加 {0} 张图片",
            [Txt.ImagePdfSkippedNonImage] = "已忽略 {0} 个非图片文件",
            [Txt.ImagePdfNothingAdded] = "没有新增文件：文件重复、不存在或不是图片",
            [Txt.MsgImagePdfBusy] = "正在转换图片，请等待完成后再关闭窗口。",
            [Txt.TipNavImagePdf] = "图片转 PDF：每张单独输出，或合并为一个 PDF",
            [Txt.Feature5] = "PDF 逐页导出为 PNG / JPEG / BMP / TIFF，默认 JPEG 300PPI、质量 100%",
            [Txt.Feature6] = "图片批量转 PDF：默认每张单独输出，可合并为一个 PDF，文件名可自定义",
            [Txt.Changelog452] = "4.5.2 · 2026-11-05",
            [Txt.ChangelogItems452] =
                "#新增\n" +
                "· 图片转 PDF 默认每张图片单独输出一个 PDF\n" +
                "· 新增「合并为一个 PDF」开关，可按需恢复合并输出\n" +
                "· 输出文件名可自定义；逐张输出时自动追加 _001、_002 序号\n" +
                "#优化\n" +
                "· 逐张输出时单张失败会继续处理其余图片，并汇总成功 / 失败数量\n" +
                "#修复\n" +
                "· PDF 转图片全程只读源文件，转换前后原 PDF 的文字、图像与页面保持一致\n",
            [Txt.Changelog456] = "4.5.6 · 2026-11-06",
            [Txt.ChangelogItems456] =
                "#优化\n" +
                "· PDF 转图片的默认输出子文件夹会附带当前页码模式，如「报告_偶数页」「报告_全部」，便于区分多次转换结果\n",
            [Txt.Changelog455] = "4.5.5 · 2026-11-06",
            [Txt.ChangelogItems455] =
                "#新增\n" +
                "· PDF 转图片支持按全部、奇数页、偶数页或自定义页码范围导出\n" +
                "· 自定义范围支持 1-3,5,8-10，并按真实页码命名输出文件\n" +
                "#优化\n" +
                "· 预览和命名示例会跟随当前页码模式显示第一张图片\n" +
                "· 旧版已填写的页码范围会自动迁移为「自定义」模式\n",
            [Txt.Changelog454] = "4.5.4 · 2026-11-05",
            [Txt.ChangelogItems454] =
                "#新增\n" +
                "· 全新扁平品牌图标：品牌蓝文档叠层 + 琥珀色合并徽标\n" +
                "· 应用内功能图标改为扁平双色风格，导航与按钮更统一、小尺寸更清晰\n" +
                "#修复\n" +
                "· 切换语言时设置窗口原地刷新，不再关闭重开或跳回屏幕中央\n",
            [Txt.Changelog453] = "4.5.3 · 2026-11-05",
            [Txt.ChangelogItems453] =
                "#新增\n" +
                "· 水印设置支持分别作用于文档合并、PDF 转图片、图片转 PDF，三个开关默认关闭\n" +
                "· PDF 转图片可在输出图片上叠加水印，源 PDF 保持只读不变\n" +
                "#优化\n" +
                "· PDF 转图片默认改为 300PPI、JPEG 格式、质量 100%\n" +
                "· 完成提示音默认关闭；关闭时音效选项自动置灰，打开后才可选择\n",
            [Txt.Changelog451] = "4.5.1 · 2026-11-05",
            [Txt.ChangelogItems451] =
                "#新增\n" +
                "· 新增「图片转 PDF」工作区：图片按列表顺序合成一个 PDF\n" +
                "· 页面大小可选 A4 / A3 / A5 / B5 / Letter 或跟随图片，支持页面方向与页边距\n" +
                "· 命名、保存位置与「完成后打开」沿用文档合并设置\n" +
                "#优化\n" +
                "· 移除 Qoder 深色皮肤\n" +
                "#修复\n" +
                "· 修复经典蓝、网易红皮肤下左侧功能栏文字对比度不足的问题\n",
            [Txt.Changelog440] = "4.4.0 · 2026-11-05",
            [Txt.ChangelogItems440] =
                "#新增\n" +
                "· 左侧卡片式导航：文档合并、PDF 转图片两个工作区，可用 Ctrl+1 / Ctrl+2 切换\n" +
                "· 新增 PDF 转图片：PDF 逐页导出为 PNG / JPEG / BMP / TIFF，格式、PPI、质量、页码范围均可设置\n" +
                "· 转换支持批量、实时预览、取消与逐文件失败汇总；图片保留所选 DPI 元数据\n" +
                "· 新增 Qoder 深色皮肤，新安装默认使用\n" +
                "#优化\n" +
                "· 界面按 Qoder 风格重做：左侧导航卡片、页头标题、单一强调色与更低对比的描边\n" +
                "· 合并与转换共用命名、保存位置、完成提醒等设置；转换使用独立的「上次位置」\n" +
                "· 输出名与保存目录可直接编辑；文件夹拖入改为递归扫描\n" +
                "· 设置页新增「PDF 转图片」页，水印页注明仅对文档合并生效\n",

            [Txt.Changelog450] = "4.5.0 · 2026-11-05",
            [Txt.ChangelogItems450] =
                "#新增\n" +
                "· 软件更名为「文枢 DocHub」：文档中枢，一键合转\n" +
                "· 关于页展示品牌定义，并补充 PDF 转图片能力说明\n" +
                "#优化\n" +
                "· 左侧导航「文档转换」更名为「PDF 转图片」，与页面标题、设置项保持一致\n" +
                "#修复\n" +
                "· 修复转换页「添加 PDF」按钮被提示文字遮挡的问题\n" +
                "· 修复文件计数文字压住删除/清空按钮的问题\n" +
                "· 升级安装保留原有设置：自动迁移旧版配置目录\n",

            [Txt.DefaultDocName] = "合并文档",
            [Txt.DefaultConvertName] = "转换图片",
            [Txt.MergeTimePrefix] = "合并_",
            [Txt.ConvertTimePrefix] = "转换_",
            [Txt.MergeCollideSuffix] = "_合并"
        };
    }
}
