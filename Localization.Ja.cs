using System.Collections.Generic;

namespace DocHub
{
    internal static class LocJa
    {
        public static readonly Dictionary<Txt, string> Table = new()
        {
            [Txt.AppName] = "DocHub",
            [Txt.Tagline] = "文書のハブ — 結合も変換もワンクリック",
            [Txt.LocalBadge] = "ローカル処理 · ファイルは外部に送信されません",

            [Txt.Ok] = "OK",
            [Txt.Close] = "閉じる",
            [Txt.Cancel] = "キャンセル",
            [Txt.Yes] = "はい",
            [Txt.No] = "いいえ",
            [Txt.Save] = "保存",
            [Txt.Reset] = "既定に戻す",
            [Txt.Browse] = "参照",
            [Txt.PreviewWord] = "試聴",
            [Txt.Settings] = "設定",
            [Txt.About] = "バージョン情報",
            [Txt.Skin] = "スキン",
            [Txt.RailMerge] = "結合",

            [Txt.DocsTitle] = "ファイルリスト",
            [Txt.DocCountFormat] = "{0} 個のファイル",
            [Txt.ListHint] = "ドラッグで並べ替え · 空白をドラッグして範囲選択 · 複数ファイルをまとめてドロップ",
            [Txt.AddDocs] = "ファイルを追加",
            [Txt.MoveUp] = "上へ",
            [Txt.MoveDown] = "下へ",
            [Txt.MoveTop] = "先頭へ",
            [Txt.MoveBottom] = "末尾へ",
            [Txt.Remove] = "削除",
            [Txt.Clear] = "クリア",
            [Txt.OutputFile] = "出力ファイル",
            [Txt.SaveTo] = "保存先",
            [Txt.ChooseFolder] = "保存先を選択",
            [Txt.StartMerge] = "結合を開始",
            [Txt.MergingButton] = "結合中…",
            [Txt.ReadyStatus] = "準備完了 · リストの上から順に結合します",
            [Txt.NamePlaceholderFirst] = "先頭ファイル名を自動使用",
            [Txt.NamePlaceholderPhrase] = "先頭ファイル名 + 任意の語句",
            [Txt.NamePlaceholderTime] = "結合日時（自動生成）",
            [Txt.NamePlaceholderCustom] = "ファイル名を入力",
            [Txt.EmptyTitle] = "Word / PDF / 画像ファイルをここにドロップ",
            [Txt.EmptySubtitle] = "または右上の「ファイルを追加」からまとめて選択",
            [Txt.EmptyFormats] = ".doc · .docx · .rtf · .pdf · .jpg · .png · .bmp · .tiff",

            [Txt.TipAdd] = "結合する Word / PDF / 画像ファイルをまとめて選択",
            [Txt.TipUp] = "選択したファイルを 1 つ上へ移動",
            [Txt.TipDown] = "選択したファイルを 1 つ下へ移動",
            [Txt.TipRemove] = "リストから削除します。元のファイルは残ります",
            [Txt.TipClear] = "ファイルリストをすべてクリア",
            [Txt.TipBrowse] = "結合後の PDF の保存先を選択",
            [Txt.TipList] = "ここにファイルをドロップ、または項目をドラッグして並べ替え",
            [Txt.TipStart] = "リスト順に結合して PDF を作成",
            [Txt.TipSettings] = "命名、テーマ、完了音、言語",
            [Txt.TipAbout] = "バージョン、更新履歴、利用規約",
            [Txt.TipCancelBox] = "範囲選択を解除し、選択前の状態に戻します（Esc）",
            [Txt.TrayOpen] = "メイン画面を開く",
            [Txt.TrayExit] = "終了",

            [Txt.DialogTitleAdd] = "結合する文書を選択",
            [Txt.FolderDesc] = "結合後の PDF の保存先を選択",
            [Txt.FilterSupport] = "対応ファイル",
            [Txt.FilterWord] = "Word 文書",
            [Txt.FilterPdf] = "PDF ファイル",
            [Txt.FilterImage] = "画像ファイル",
            [Txt.FilterAll] = "すべてのファイル",

            [Txt.StatusAdded] = "{0} 個のファイルを追加 · リスト順に結合",
            [Txt.StatusSkipped] = "（重複または未対応のファイル {0} 件をスキップ）",
            [Txt.StatusNothing] = "追加されたファイルはありません：重複、存在しない、または未対応の形式です",
            [Txt.StatusOrder] = "順序を変更しました · 現在のリスト順に結合",
            [Txt.StatusRemoved] = "リストから削除：{0}",
            [Txt.StatusRemovedMulti] = "{0} 件をリストから削除しました",
            [Txt.StatusMovedTop] = "先頭へ移動 · リスト順に結合します",
            [Txt.StatusMovedBottom] = "末尾へ移動 · リスト順に結合します",
            [Txt.StatusCleared] = "リストをクリアしました",
            [Txt.StatusFolderChosen] = "保存先を選択しました",

            [Txt.MsgClearTitle] = "リストをクリア",
            [Txt.MsgClearText] = "ファイルリストをすべてクリアしますか？\n\n元のファイルは削除されません。",
            [Txt.MsgMergeBusy] = "結合中です。完了してからウィンドウを閉じてください。",
            [Txt.MsgNoWordTitle] = "Microsoft Word が見つかりません",
            [Txt.MsgNoWordText] = "Word 文書を変換するには Microsoft Word が必要です。\n\nMicrosoft Word をインストールしてください。PDF と画像のみの結合は可能です。",
            [Txt.MsgOverwriteTitle] = "上書き確認",
            [Txt.MsgOverwriteText] = "出力ファイルは既に存在します：\n{0}\n\n上書きしますか？",

            [Txt.StatusPreparing] = "準備中…",
            [Txt.StatusProgress] = "{0}% · {1}",
            [Txt.StatusDone] = "結合完了 · 全 {0} ページ · {1}",
            [Txt.BalloonTitle] = "結合完了",
            [Txt.BalloonText] = "全 {0} ページ · {1}\n{2}",
            [Txt.OpenFailed] = "PDF を開けませんでした。保存先を確認してください。",
            [Txt.FailStatus] = "結合に失敗しました。ファイルを確認して再試行してください",
            [Txt.FailTitle] = "結合失敗",
            [Txt.FailPrefix] = "結合に失敗しました：{0}",

            [Txt.ErrCannotStartTitle] = "結合を開始できません",
            [Txt.ErrNeedDocs] = "先に結合する文書を追加してください。",
            [Txt.ErrNeedName] = "出力ファイル名を入力してください。",
            [Txt.ErrBadChars] = "ファイル名に次の文字は使用できません：\\ / : * ? \" < > |",
            [Txt.ErrNeedFolder] = "保存先を選択してください。",
            [Txt.ErrFolderMissing] = "保存先が存在しません：\n{0}",
            [Txt.ErrPathInvalid] = "出力パスが無効です。",
            [Txt.ErrSameSource] = "出力ファイルが元の文書と同名のため、元ファイルを上書きします。\n\nファイル名または保存先を変更してください。",
            [Txt.ElapsedSeconds] = "{0:0.0} 秒",
            [Txt.ElapsedMinutes] = "{0} 分 {1} 秒",

            [Txt.SettingsTitle] = "設定",
            [Txt.NamingSection] = "出力ファイル名",
            [Txt.NamingHint] = "結合後にこの方法でファイル名を生成します",
            [Txt.NamingFirst] = "先頭ファイルのファイル名を使用",
            [Txt.NamingPhrase] = "先頭ファイル名 + 任意の語句",
            [Txt.NamingTime] = "結合日時（自動生成）",
            [Txt.NamingCustom] = "任意の名前を使用",
            [Txt.PhrasePlaceholder] = "例：最終版",
            [Txt.CustomPlaceholder] = "例：2026年度契約書まとめ",
            [Txt.PreviewFormat] = "プレビュー：{0}.pdf",
            [Txt.SkinSection] = "外観テーマ",
            [Txt.SkinHint] = "外観を選択（変更はすぐに反映されます）",
            [Txt.DoneSection] = "結合完了時",
            [Txt.DoneNotify] = "タスクバー右下に通知を表示",
            [Txt.DoneSound] = "完了音を再生",
            [Txt.DoneOpen] = "生成した PDF を自動で開く（既定はオフ）",
            [Txt.SoundSection] = "完了音",
            [Txt.SoundWater] = "水滴",
            [Txt.SoundDing] = "ディンドン",
            [Txt.SoundQQ] = "QQ",
            [Txt.SoundChime] = "チャイム",
            [Txt.SoundSoft] = "ソフト",
            [Txt.SoundOff] = "オフ",
            [Txt.SoundHint] = "先に完了音をオンにしてから音を選択してください。選択すると自動再生されます",
            [Txt.LanguageSection] = "表示言語",
            [Txt.LanguageHint] = "切り替えるとすぐに反映されます",
            [Txt.QuoteSection] = "今日の一言",
            [Txt.QuoteHint] = "UI 言語に合わせて切り替わります",
            [Txt.QuoteToggle] = "結合ページに名言や映画の台詞を表示",
            [Txt.QuoteAutoRefresh] = "名言を自動で切り替え",
            [Txt.QuoteRefreshInterval] = "切り替え間隔",
            [Txt.QuoteRefreshSeconds] = "秒",
            [Txt.QuoteRefreshHint] = "有効にすると設定した間隔で自動的に次の一言に切り替わります。オフの場合はカードをクリックして切り替えます",
            [Txt.TipQuote] = "クリックで次の一言",
            [Txt.OutputSection] = "保存先",
            [Txt.OutputHint] = "結合後の PDF をどこに保存するか",
            [Txt.OutputFirst] = "先頭ファイルと同じフォルダー",
            [Txt.OutputLast] = "前回保存した場所",
            [Txt.OutputCustom] = "任意のフォルダー",
            [Txt.OutputCustomPlaceholder] = "フォルダーを選択または入力",
            [Txt.SkinBlueName] = "クラシックブルー",
            [Txt.SkinBlueDesc] = "爽やかで透明感",
            [Txt.SkinNightName] = "ナイトモード",
            [Txt.SkinNightDesc] = "没入感のあるダーク",
            [Txt.SkinCloudName] = "クラウドホワイト",
            [Txt.SkinCloudDesc] = "温かい紙の質感",
            [Txt.SkinSkeuoName] = "スキューモーフィズム",
            [Txt.SkinSkeuoDesc] = "革と紙の質感",
            [Txt.SkinNeumorphName] = "ニューモーフィズム",
            [Txt.SkinNeumorphDesc] = "柔らかな立体感",
            [Txt.SettingsLiveHint] = "すべての変更は即時反映されます",

            [Txt.AboutTitle] = "バージョン情報",
            [Txt.VersionLine] = "{0} · Word / PDF / 画像の結合・変換ツール",
            [Txt.AboutIntro] = "すべての処理はこの端末内で完結し、ファイルは外部に送信されません",
            [Txt.BrandDefinition] = "DocHub：文書のハブ — 結合も変換もワンクリック",
            [Txt.Feature1] = "Word の全形式、PDF、画像をまとめて結合、サイズ制限なし",
            [Txt.Feature2] = "元のレイアウト、ページ番号、画像サイズ、向きをそのまま保持",
            [Txt.Feature3] = "ドラッグで並べ替え、進捗をリアルタイム表示、出力順を完全に制御",
            [Txt.Feature4] = "完全オフライン。ファイルがサーバーに送信されることはありません",
            [Txt.ChangelogTitle] = "更新履歴",
            [Txt.Changelog300] = "3.0.0 · 2026-11-03",
            [Txt.ChangelogItems300] =
                "#新機能\n" +
                "· インストーラーに完全な .NET ランタイムを同梱し、.NET 未導入の PC でもそのまま使用可能\n" +
                "· 簡易 PDF ビューアーを内蔵：PDF リーダーがない PC でも結合結果を確認可能\n" +
                "· バージョン情報に作者名と連絡先を追加（cado@163.com）\n" +
                "#改善\n" +
                "· 設定項目は変更後すぐに反映、「保存」ボタンは不要\n" +
                "· 効果音は選択すると自動再生、「試聴」ボタンは不要\n" +
                "· 結合速度を向上：画像を並列変換し、結果を直接出力ファイルへ書き込み\n" +
                "· インストール後の本体は軽量なまま、ランタイムはインストーラーと一緒に配布\n" +
                "#修正\n" +
                "· アイコンの図形がやや上寄りだったため、垂直中央に調整\n" +
                "· Word 未導入時に PDF と画像を結合できない問題を修正：LibreOffice に対応、または Word を飛ばして続行可能",
            [Txt.Changelog200] = "2.0.0 · 2026-11-03",
            [Txt.ChangelogItems200] =
                "#新機能\n" +
                "· JPG、PNG、BMP、TIFF などの画像を直接結合でき、元のサイズと向きを保持\n" +
                "· Word、PDF、画像を同じリストで混在結合し、1 つの PDF に出力\n" +
                "· 画面の言語はシステム言語に自動で合わせ、インストール直後から対応言語で表示\n" +
                "#改善\n" +
                "· 画面と案内の表記を「ファイル」に統一、リストのバッジは PDF / WORD / 画像を区別\n" +
                "· 設定画面を全面的にフラット化：単色の区分、細い区切り線、グラデーションと影を廃止\n" +
                "· 更新履歴を「新機能 / 改善 / 修正」で分類し、要点が見やすく\n" +
                "#修正\n" +
                "· PDF や画像のみの結合時に Word 不足の警告が出る問題を修正",
            [Txt.Changelog130] = "1.3.0 · 2026-11-03",
            [Txt.ChangelogItems130] =
                "#新機能\n" +
                "· 保存先の設定を追加：先頭ファイルのフォルダー、前回保存した場所、任意のパス\n" +
                "· ファイルリストで右クリックによる削除・上へ・下へに対応\n" +
                "· インストーラー版とポータブル版（非圧縮）の 2 種類で配布\n" +
                "#改善\n" +
                "· 設定は保存後すぐに反映され、再起動は不要\n" +
                "· 進捗バーがテーマ配色に合わせてパーセントを表示\n" +
                "· ファイルリストを刷新：カード形式の行、種類バッジ、ファイル情報\n" +
                "#修正\n" +
                "· リストのちらつきと揺れを修正\n" +
                "· 同名ファイルの上書きを修正し、A-1.pdf のように自動連番",
            [Txt.Changelog120] = "1.2.0 · 2026-11-03",
            [Txt.ChangelogItems120] =
                "#新機能\n" +
                "· 新しいフラットアイコン。文字なしでよりシンプルに\n" +
                "· 完了音を 5 種類内蔵：水滴、ディンドン、QQ など、試聴可能\n" +
                "· バージョン情報に「利用規約」を追加\n" +
                "· 繁体字中国語、英語、日本語、ロシア語、韓国語の画面に対応、即時切り替え可能\n" +
                "#改善\n" +
                "· 角丸、影、余白、ホバー表現など UI を全面ブラッシュアップ",
            [Txt.Changelog110] = "1.1.0 · 2026-11-03",
            [Txt.ChangelogItems110] =
                "#新機能\n" +
                "· 4 種類の命名方法：先頭ファイル名、先頭ファイル名＋任意語句、結合日時、任意の名前\n" +
                "· NetEase 風テーマ 4 種：クラシックブルー、NetEase レッド、ナイト、クラウドホワイト\n" +
                "#改善\n" +
                "· 「設定」と「バージョン情報」を追加、アイコンをフラットなスタイルに変更\n" +
                "· 結合後に PDF を自動で開かず、右下の通知と完了音に変更（設定可能）",
            [Txt.Changelog100] = "1.0.0 · 2026-11-02",
            [Txt.ChangelogItems100] =
                "#新機能\n" +
                "· 初回リリース：Word 全形式 + PDF 結合、一括読み込み、ドラッグ並べ替え、進捗表示",
            [Txt.ContactAuthor] = "作者：AI",
            [Txt.Copyright] = "© 2026 AI · ローカル処理で安心",
            [Txt.EulaButton] = "利用規約",
            [Txt.EulaTitle] = "利用規約",
            [Txt.EulaIntro] = "DocHub をご利用になる前に、以下の規約をお読みください。",
            [Txt.EulaBody] =
                "1. 用途\n" +
                "DocHub は、Word / PDF / 画像をローカルで結合・変換するデスクトップツールです。本機にインストールされた " +
                "Microsoft Word を使って Word 文書を PDF に書き出し、画像は元のサイズでページに配置し、" +
                "指定した順序でページを結合します。\n\n" +
                "2. ファイルについて\n" +
                "すべての処理は本機内で行われます。文書のアップロード、送信、バックアップは行わず、個人情報や" +
                "文書の内容も収集しません。\n\n" +
                "3. 利用者の責任\n" +
                "結合する文書について適法な利用権をお持ちであることをご確認ください。結合前に重要なファイルを" +
                "バックアップしてください。ファイルの破損、上書き、不適切な使用による損失について、作者は責任を負いません。\n\n" +
                "4. 動作環境\n" +
                "Word 文書の結合には、本機にインストールされた Microsoft Word または LibreOffice が必要です。" +
                "本ソフトは Office を提供・認証しません。\n\n" +
                "5. 保証の否認\n" +
                "本ソフトは現状のまま提供されます。十分なテストを行っていますが、すべての環境ですべての文書が" +
                "完全に結合されることを保証するものではありません。\n\n" +
                "6. 規約の変更\n" +
                "本規約はバージョン更新に伴い変更される場合があります。継続してご利用いただく場合、変更後の規約に" +
                "同意したものとみなされます。",
            [Txt.EulaAgree] = "読み取り、同意します",

            [Txt.MergeNoDocs] = "先に結合する文書を追加してください。",
            [Txt.MergeFileMissing] = "ファイルが存在しないか、移動されています：\n{0}",
            [Txt.MergeWordMissing] =
                "Microsoft Word または LibreOffice が見つからないため、Word 文書を変換できません。\n\n" +
                "Microsoft Word または LibreOffice をインストールしてください。PDF と画像のみの結合には影響しません。",
            [Txt.MergeWordStartFail] = "Microsoft Word を起動できません。正しくインストールされ、正常に開けることを確認してください。",
            [Txt.MergeWordNotInstalled] = "Microsoft Word または LibreOffice が見つかりません。インストールしてから再試行してください。",
            [Txt.MergeProcessing] = "処理中（{0}/{1}）：{2}",
            [Txt.MergeStartingWord] = "Word 変換エンジンを起動中…",
            [Txt.MergeMergingPages] = "ページを結合中…",
            [Txt.MergeMerging] = "結合中（{0}/{1}）：{2}",
            [Txt.MergePdfUnreadable] = "PDF「{0}」を読み取れません：破損、暗号化、または未対応の形式の可能性があります。",
            [Txt.MergeNoPages] = "結合できるページがありません。",
            [Txt.MergePdfError] = "PDF の結合中にエラーが発生しました。出力ファイルは作成されていません。",
            [Txt.MergeWriteError] =
                "出力ファイルに書き込めません：\n{0}\n\n他のプログラムで開かれていないこと、保存先が書き込み可能であることを確認してください。\n\n技術情報：{1}",
            [Txt.MergeNoPermission] = "出力ファイルへの書き込み権限がありません：\n{0}\n\n別の保存先を選択して再試行してください。",
            [Txt.MergeFailedGeneric] = "結合に失敗しました：{0}",
            [Txt.MergeTempFail] = "結合用の一時フォルダーを作成できません。\n\nセキュリティソフトが書き込みを阻止しているか、一時フォルダーの権限に問題があります。\n本プログラムを許可リストに追加するか、管理者として実行してから再試行してください。",
            [Txt.MergeDone] = "結合完了",
            [Txt.MergeWordOpenFail] = "「{0}」を開けません。",
            [Txt.MergeWordConvertEmpty] = "「{0}」の変換後、有効なページがありませんでした。",
            [Txt.MergeWordConvertFail] =
                "Word 文書「{0}」を変換できません。\n\n" +
                "暗号化や破損がないこと、本機の Microsoft Word で正常に開けることを確認してください。\n\n技術情報：{1}",
            [Txt.MergePasswordTitle] = "ファイルは暗号化されています",
            [Txt.MergePasswordHint] = "このファイルを開くパスワードを入力してください。今回の結合でのみ使用します。",
            [Txt.MergePasswordPlaceholder] = "パスワード",
            [Txt.MergePasswordWrong] = "パスワードが正しくありません。もう一度入力してください。",
            [Txt.MergeImageConvertFail] =
                "画像「{0}」を読み込めません。\n\n" +
                "画像が破損していないこと、JPG・PNG・BMP・TIFF などの一般的な形式であることを確認してください。\n\n技術情報：{1}",

            [Txt.MsgSkipWordTitle] = "Word ファイルを変換できません",
            [Txt.MsgSkipWordText] =
                "Microsoft Word または LibreOffice が見つからないため、{0} 個の Word ファイルを変換できません。\n\n" +
                "これらの Word ファイルを飛ばして、残りの PDF と画像の結合を続行しますか？",
            [Txt.StatusSkippedWord] = "{0} 個の Word ファイルをスキップしました（Word / LibreOffice 未導入）",

            [Txt.ViewResult] = "結合結果を表示",
            [Txt.PdfViewerTitle] = "PDF ビューアー",
            [Txt.PrevPage] = "前のページ",
            [Txt.NextPage] = "次のページ",
            [Txt.ZoomIn] = "拡大",
            [Txt.ZoomOut] = "縮小",
            [Txt.ViewerPageFormat] = "{0} / {1} ページ",
            [Txt.ViewerNoReader] = "PDF リーダーが見つからないため、内蔵ビューアーを使用しています",
            [Txt.ViewerLoadFailed] = "PDF ファイルを開けません：\n{0}",

            [Txt.SettingsNavLook] = "外観",
            [Txt.SettingsNavNaming] = "名前と保存",
            [Txt.SettingsNavDone] = "完了通知",
            [Txt.SettingsNavWatermark] = "透かし",
            [Txt.WatermarkSection] = "文書の透かし",
            [Txt.WatermarkHint] = "透かしを付ける機能を選択します。スタイルは3機能で共通、初期状態はオフ",
            [Txt.WatermarkEnable] = "透かしを追加",
            [Txt.WatermarkForMerge] = "文書結合",
            [Txt.WatermarkForPdfImage] = "PDF → 画像",
            [Txt.WatermarkForImagePdf] = "画像 → PDF",
            [Txt.WatermarkForWordPdf] = "Word → PDF",
            [Txt.WatermarkPresets] = "定型文字",
            [Txt.WatermarkCustom] = "カスタム",
            [Txt.WatermarkPreset1] = "正本",
            [Txt.WatermarkPreset2] = "副本",
            [Txt.WatermarkPreset3] = "機密",
            [Txt.WatermarkPreset4] = "極秘",
            [Txt.WatermarkPreview] = "プレビュー",
            [Txt.WatermarkCustomPlaceholder] = "透かし文字を入力",
            [Txt.WatermarkPreviewHint] = "プレビュー内でドラッグ移動、ホイールで拡大縮小、右ドラッグで回転。数値も直接入力できます。",
            [Txt.WatermarkTint] = "色",
            [Txt.WatermarkTintGray] = "グレー",
            [Txt.WatermarkTintRed] = "赤",
            [Txt.WatermarkTintBlue] = "青",
            [Txt.WatermarkPosition] = "位置",
            [Txt.WatermarkPosTile] = "タイル",
            [Txt.WatermarkPosTopLeft] = "左上",
            [Txt.WatermarkPosTopRight] = "右上",
            [Txt.WatermarkPosCenter] = "中央",
            [Txt.WatermarkPosBottomLeft] = "左下",
            [Txt.WatermarkPosBottomRight] = "右下",
            [Txt.WatermarkOpacity] = "不透明度",
            [Txt.WatermarkScale] = "サイズ",
            [Txt.WatermarkRotation] = "傾き角度",
            [Txt.WatermarkOffsetX] = "水平位置",
            [Txt.WatermarkOffsetY] = "垂直位置",
            [Txt.ToastTitle] = "結合完了",
            [Txt.ToastOpen] = "表示",
            [Txt.Changelog310] = "3.1.0 · 2026-11-03",
            [Txt.ChangelogItems310] =
                "#追加\n" +
                "· 設定画面を「左メニュー + 右コンテンツ」の 2 階層に刷新\n" +
                "· 文書透かしを追加：正本/副本/機密/極秘の定型、カスタム文字・スタイル・色に対応\n" +
                "#改善\n" +
                "· インストール先を整理：小型ランチャー + app フォルダ\n" +
                "· 結合完了の通知をテーマに合わせた右下カードに変更\n",
            [Txt.Changelog400] = "4.0.0 · 2026-11-03",
            [Txt.ChangelogItems400] =
                "#追加\n" +
                "· ファイルリストで複数選択の一括操作に対応、右クリックで「先頭へ」「末尾へ」\n" +
                "#改善\n" +
                "· ファイルリストの行デザインを整理し、選択状態を見やすく\n" +
                "· ウィンドウのサイズ変更時に再レイアウトし、状態行が「結合開始」ボタンに重なる不具合を修正\n" +
                "· 言語切替時に既定の出力名もその言語へ同期\n" +
                "· 既定設定に戻すと表示言語も簡体字中国語に戻る\n" +
                "· インストーラーの既定インストール先を C:\\Program Files (x86)\\DocHub に変更\n",
            [Txt.Changelog430] = "4.3.0 · 2026-11-04",
            [Txt.ChangelogItems430] =
                "#新機能\n" +
                "· メイン画面のナビゲーションを右上へ移動：外観・透かし・完了通知・設定・情報をフラットなアイコンで並べ、クリックで該当ページへ直接移動\n" +
                "#修正\n" +
                "· 透かしが大きすぎたり画面端に寄せたりすると切れる問題を修正：ページ内に収まるよう自動縮小し、結合後も全体が表示されます\n" +
                "· プレビューで透かしが枠からはみ出さないようにしました\n" +
                "#改善\n" +
                "· 「しずく」「QQ」の通知音を再合成し、より澄んだアタックと自然な余韻に。各通知音の音量も統一\n" +
                "· メイン画面の左サイドバーを廃止し、よりすっきりした UI に\n",
            [Txt.Changelog420] = "4.2.0 · 2026-11-04",
            [Txt.ChangelogItems420] =
                "#最適化\n" +
                "· 設定・バージョン情報ウィンドウが即座に開きます：すりガラス背景をスキンとサイズごとにキャッシュし、事前に生成\n" +
                "· 透かしプレビューのドラッグが滑らかに：タイル表示を事前生成したテクスチャ描画に変更し、最小倍率時の1フレームを190msから1ms未満に短縮\n" +
                "#改善\n" +
                "· 左側ナビゲーションを再設計：アイコンと名称の階層を明確にし、選択項目はアクセント色のピルと左側バーで表示\n" +
                "· バージョン番号をセマンティックバージョニング（SemVer）に準拠：体験と性能の改善はマイナーバージョンを更新\n",
            [Txt.Changelog412] = "4.1.2 · 2026-11-04",
            [Txt.ChangelogItems412] =
                "#新機能\n" +

                "· 透かし位置のプリセット：タイル / 左上 / 右上 / 中央 / 左下 / 右下\n" +

                "#改善\n" +

                "· 透かし設定の動作が軽快に：連続ドラッグは一度だけ反映し、プレビューの描画をキャッシュ\n" +

                "· メイン画面の左サイドバーに機能名（結合 / 設定 / 情報）を表示し、選択項目をテーマ色で強調\n",

            [Txt.Changelog411] = "4.1.1 · 2026-11-04",
            [Txt.ChangelogItems411] =
                "#修正\n" +
                "· インストーラーが既定フォルダーへ強制変更しなくなり、指定したインストール先がそのまま使われます\n" +
                "· 画像の結合に失敗する問題を修正：WebP / HEIC などはシステムのデコーダーで処理し、通常の画像の互換性も向上\n" +
                "#改善\n" +
                "· 透かしの各パラメーターを直接入力可能。プレビューでドラッグ移動、ホイールで拡大縮小、右ドラッグで回転\n" +
                "· 「スタイル」選択を廃止し、斜めタイル表示に統一\n",

            [Txt.Changelog410] = "4.1.0 · 2026-11-04",
            [Txt.ChangelogItems410] =
                "#追加\n" +
                "· 透かしの不透明度・サイズ・位置・傾きを自由に調整でき、設定画面で即時プレビュー\n" +
                "· インストーラーを独自実装に変更：現在のユーザーのフォルダーにインストールし、管理者権限は不要、Program Files へは書き込みません\n" +
                "· スタートメニューに「文枢をアンインストール」ショートカットを追加\n" +
                "#改善\n" +
                "· 設定画面を縦に拡大し、透かしの調整をスライダーに変更して操作しやすく\n",

            [Txt.NavMergeTitle] = "文書結合",
            [Txt.NavMergeSubtitle] = "PDF に結合",
            [Txt.NavConvertTitle] = "PDF→画像",
            [Txt.NavConvertSubtitle] = "ページごとに画像へ",
            [Txt.MergePageTitle] = "文書結合",
            [Txt.MergePageSubtitle] = "リスト順に Word / PDF / 画像を 1 つの PDF に結合",
            [Txt.ConvertPageTitle] = "PDF→画像",
            [Txt.ConvertPageSubtitle] = "PDF の各ページを画像に書き出し · 元ファイルは読み取り専用で変更しません",
            [Txt.ConvertFilesTitle] = "PDF ファイル",
            [Txt.ConvertCountFormat] = "{0} 個のファイル",
            [Txt.ConvertListHint] = "空白をドラッグして範囲選択 · PDF 追加時にページ数を自動取得",
            [Txt.ConvertAdd] = "PDF を追加",
            [Txt.ConvertDialogTitle] = "変換する PDF を選択",
            [Txt.ConvertEmptyTitle] = "PDF をここにドロップ",
            [Txt.ConvertEmptySubtitle] = "または「PDF を追加」からまとめて選択",
            [Txt.ConvertEmptyFormats] = ".pdf",
            [Txt.ConvertPageCountFormat] = "{0} ページ",
            [Txt.ConvertPageCountUnknown] = "ページ数を取得中…",
            [Txt.ConvertPageCountFailed] = "読み取れません",
            [Txt.ConvertSettingsTitle] = "書き出し設定",
            [Txt.ConvertFormat] = "画像形式",
            [Txt.ConvertPpi] = "解像度（PPI）",
            [Txt.ConvertPpiCustom] = "カスタム",
            [Txt.ConvertPpiHint] = "PPI は画像の精細さを決め、ページの物理サイズは変わりません",
            [Txt.ConvertQuality] = "JPEG 品質",
            [Txt.ConvertBackground] = "透過背景",
            [Txt.ConvertBackgroundHint] = "透過は PNG のみ。他の形式は白背景になります",
            [Txt.ConvertPageRange] = "ページ範囲",
            [Txt.ConvertPageRangeAll] = "すべて",
            [Txt.ConvertPageRangePlaceholder] = "例：1-3,5,8-10",
            [Txt.ConvertPageModeAll] = "すべて",
            [Txt.ConvertPageModeOdd] = "奇数",
            [Txt.ConvertPageModeEven] = "偶数",
            [Txt.ConvertPageModeCustom] = "カスタム",
            [Txt.ConvertSubfolder] = "PDF ごとにフォルダーを作成",
            [Txt.ConvertSubfolderHint] = "一括変換では自動的に有効になり、フォルダー名にページ指定が付きます",
            [Txt.ConvertOutput] = "保存先",
            [Txt.ConvertNamePreview] = "名前プレビュー：{0}",
            [Txt.ConvertEstimateFormat] = "約 {0} × {1} px",
            [Txt.ConvertPreviewTitle] = "ページ {0} のプレビュー",
            [Txt.ConvertPreviewEmpty] = "PDF を選択すると選択中のページを表示します",
            [Txt.ConvertStart] = "変換を開始",
            [Txt.ConvertRunning] = "変換中…",
            [Txt.ConvertCancel] = "キャンセル",
            [Txt.ConvertCancelling] = "キャンセル中…",
            [Txt.ConvertReady] = "準備完了 · リスト順に画像を書き出します",
            [Txt.ConvertProcessing] = "変換中（{0}/{1}）：{2}",
            [Txt.ConvertPageStatus] = "{0} / {1} ページ",
            [Txt.ConvertDone] = "変換完了 · {0} 枚の画像 · {1}",
            [Txt.ConvertDonePartial] = "完了 {0} 件 · 失敗 {1} 件 · 画像 {2} 枚",
            [Txt.ConvertCancelled] = "キャンセルしました · {0} 枚書き出し済み",
            [Txt.ConvertFailStatus] = "変換に失敗しました。ファイルを確認して再試行してください",
            [Txt.ConvertFailTitle] = "変換失敗",
            [Txt.ConvertOpenFolder] = "フォルダーを開く",
            [Txt.ConvertOutputSubfolder] = "出力：{0}",
            [Txt.ConvertNoFiles] = "先に変換する PDF を追加してください。",
            [Txt.ConvertNeedFolder] = "保存先を選択してください。",
            [Txt.ConvertBadRange] = "ページ範囲の形式が正しくありません。1-3,5 のように入力してください。",
            [Txt.ConvertRangeOut] = "ページ範囲が文書のページ数（{0} ページ）を超えています。",
            [Txt.ConvertNoPages] = "変換できるページがありません。",
            [Txt.ConvertRangeNeeded] = "ページ範囲を入力するか、「すべて」を選択してください。",
            [Txt.ConvertNoMatchingPages] = "現在のページ指定に変換対象のページがありません。",
            [Txt.ConvertUnreadable] = "PDF「{0}」を読み取れません：破損、暗号化、または未対応の形式の可能性があります。",
            [Txt.ConvertPpiTooLarge] = "選択した PPI ではページが大きすぎます（{0} × {1} px）。PPI を下げてください。",
            [Txt.ConvertWriteError] = "画像を書き込めません：\n{0}\n\n技術情報：{1}",
            [Txt.ConvertMemory] = "メモリ不足のためこのページを描画できません。PPI を下げて再試行してください。",
            [Txt.ConvertSkippedNonPdf] = "PDF 以外のファイル {0} 件を無視しました",
            [Txt.ConvertDuplicate] = "重複ファイル {0} 件をスキップしました",
            [Txt.ConvertAdded] = "PDF ファイル {0} 件を追加しました",
            [Txt.ConvertNothingAdded] = "追加されたファイルはありません：重複、存在しない、または PDF 以外です",
            [Txt.MsgConvertBusy] = "変換中です。完了してからウィンドウを閉じてください。",
            [Txt.SettingsNavConvert] = "PDF→画像",
            [Txt.ConvertSection] = "PDF→画像の既定値",
            [Txt.ConvertDefaultsHint] = "変換ページの初期値です。変換ページでの変更は記憶されます",
            [Txt.ConvertDefaultFormat] = "既定の画像形式",
            [Txt.ConvertDefaultPpi] = "既定の PPI",
            [Txt.ConvertDefaultQuality] = "既定の JPEG 品質",
            [Txt.ConvertDefaultBackground] = "既定で透過背景",
            [Txt.ConvertDefaultSubfolder] = "既定で PDF ごとにフォルダーを作成",
            [Txt.ConvertWatermarkNote] = "透かしは出力画像にのみ合成され、元の PDF は読み取り専用のままです。「透かし」ページで有効化できます",
            [Txt.NamingHintShared] = "結合と PDF→画像で共有する出力命名",
            [Txt.DoneOpenShared] = "完了後に結果を自動で開く（結合は PDF、変換はフォルダー）",
            [Txt.TipNavMerge] = "文書結合：リスト順に 1 つの PDF に結合",
            [Txt.TipNavConvert] = "PDF→画像：PDF の各ページを画像に書き出し",
            [Txt.TipAppearance] = "外観設定を開く",
            [Txt.TipNotifications] = "完了通知設定を開く",
            [Txt.TipWatermark] = "透かし設定を開く",
            [Txt.NavImagePdfTitle] = "画像→PDF",
            [Txt.NavImagePdfSubtitle] = "画像を個別または 1 つの PDF に",
            [Txt.ImagePdfPageTitle] = "画像→PDF",
            [Txt.ImagePdfPageSubtitle] = "画像を個別に出力、または 1 つの PDF に結合 · ローカル処理",
            [Txt.ImagePdfFilesTitle] = "画像ファイル",
            [Txt.ImagePdfCountFormat] = "{0} 個のファイル",
            [Txt.ImagePdfListHint] = "空白をドラッグして範囲選択 · リスト順に変換",
            [Txt.ImagePdfAdd] = "画像を追加",
            [Txt.ImagePdfDialogTitle] = "変換する画像を選択",
            [Txt.ImagePdfEmptyTitle] = "画像をここにドロップ",
            [Txt.ImagePdfEmptySubtitle] = "または「画像を追加」からまとめて選択",
            [Txt.ImagePdfEmptyFormats] = ".jpg .jpeg .png .bmp .tif .tiff .gif .webp",
            [Txt.ImagePdfSettingsTitle] = "PDF 設定",
            [Txt.ImagePdfPageSize] = "ページサイズ",
            [Txt.ImagePdfPageSizeAuto] = "自動",
            [Txt.ImagePdfOrientation] = "ページの向き",
            [Txt.ImagePdfOrientationAuto] = "自動",
            [Txt.ImagePdfOrientationPortrait] = "縦",
            [Txt.ImagePdfOrientationLandscape] = "横",
            [Txt.ImagePdfMargin] = "余白",
            [Txt.ImagePdfMarginNone] = "なし",
            [Txt.ImagePdfMarginSmall] = "10 mm",
            [Txt.ImagePdfMarginLarge] = "20 mm",
            [Txt.ImagePdfFitHint] = "自動は画像に合わせたページ\n固定サイズは比率を保って収めます",
            [Txt.ImagePdfMergeOption] = "1 つの PDF に結合",
            [Txt.ImagePdfNameLabel] = "ファイル名",
            [Txt.ImagePdfNamePlaceholderMerge] = "空欄なら結合時の命名",
            [Txt.ImagePdfNamePlaceholderSeparate] = "空欄なら画像の元の名前",
            [Txt.ImagePdfNamePreview] = "名前プレビュー：{0}",
            [Txt.ImagePdfNamePreviewMulti] = "{0} ほか計 {1} 個の PDF",
            [Txt.ImagePdfPageCountFormat] = "{0} 枚の画像",
            [Txt.ImagePdfOutputCount] = "{0} 個の PDF",
            [Txt.ImagePdfPreviewTitle] = "プレビュー",
            [Txt.ImagePdfPreviewEmpty] = "画像を選択するとプレビューを表示します",
            [Txt.ImagePdfStart] = "変換を開始",
            [Txt.ImagePdfRunning] = "変換中…",
            [Txt.ImagePdfCancel] = "キャンセル",
            [Txt.ImagePdfCancelling] = "キャンセル中…",
            [Txt.ImagePdfReady] = "準備完了 · 各画像を個別の PDF に出力します",
            [Txt.ImagePdfReadyMerge] = "準備完了 · リスト順に 1 つの PDF に結合します",
            [Txt.ImagePdfProcessing] = "変換中（{0}/{1}）：{2}",
            [Txt.ImagePdfDone] = "変換完了 · {0} ページ · {1}",
            [Txt.ImagePdfDoneFiles] = "変換完了 · {0} 個の PDF · {1}",
            [Txt.ImagePdfDonePartial] = "変換完了 · 成功 {0} 件、失敗 {1} 件 · {2}",
            [Txt.ImagePdfCancelled] = "キャンセルしました · {0} 枚処理済み",
            [Txt.ImagePdfFailStatus] = "変換に失敗しました。画像を確認して再試行してください",
            [Txt.ImagePdfFailTitle] = "変換失敗",
            [Txt.ImagePdfNoFiles] = "先に変換する画像を追加してください。",
            [Txt.ImagePdfNeedFolder] = "保存先を選択してください。",
            [Txt.ImagePdfUnreadable] = "画像「{0}」を読み取れません：破損、または未対応の形式の可能性があります。",
            [Txt.ImagePdfWriteError] = "PDF を書き込めません：\n{0}\n\n技術情報：{1}",
            [Txt.ImagePdfFailGeneric] = "PDF を作成できません：{0}",
            [Txt.ImagePdfAdded] = "画像 {0} 枚を追加しました",
            [Txt.ImagePdfSkippedNonImage] = "画像以外のファイル {0} 件を無視しました",
            [Txt.ImagePdfNothingAdded] = "追加されたファイルはありません：重複、存在しない、または画像以外です",
            [Txt.MsgImagePdfBusy] = "画像を変換中です。完了してからウィンドウを閉じてください。",
            [Txt.TipNavImagePdf] = "画像→PDF：既定は個別出力、必要なら 1 つの PDF に結合",
            [Txt.Feature5] = "PDF を PNG / JPEG / BMP / TIFF に書き出し。既定は JPEG・300PPI・品質 100%",
            [Txt.Feature6] = "画像を一括で PDF 化：既定は個別出力、1 つに結合可能、ファイル名も変更可能",
            [Txt.Changelog452] = "4.5.2 · 2026-11-05",
            [Txt.ChangelogItems452] =
                "#新機能\n" +
                "· 画像→PDF は既定で画像ごとに 1 つの PDF を出力\n" +
                "· 「1 つの PDF に結合」スイッチを追加し、必要に応じて従来の結合出力に切替可能\n" +
                "· 出力ファイル名を変更可能。個別出力時は _001、_002 の連番を付加\n" +
                "#改善\n" +
                "· 個別出力時に 1 枚失敗しても残りの画像を処理し、成功 / 失敗数を集計\n" +
                "#修正\n" +
                "· PDF→画像は元 PDF を読み取り専用で扱い、文字・画像・ページを変更しません\n",
            [Txt.NavWordPdfTitle] = "Word → PDF",
            [Txt.NavWordPdfSubtitle] = "Word を一括で PDF に変換",
            [Txt.WordPdfPageTitle] = "Word → PDF",
            [Txt.WordPdfPageSubtitle] = "Word を個別または 1 つの PDF に変換 · ローカル処理",
            [Txt.WordPdfFilesTitle] = "Word 文書",
            [Txt.WordPdfCountFormat] = "{0} 個の文書",
            [Txt.WordPdfListHint] = "空白をドラッグで範囲選択 · リスト順に変換",
            [Txt.WordPdfAdd] = "Word を追加",
            [Txt.WordPdfDialogTitle] = "変換する Word 文書を選択",
            [Txt.WordPdfEmptyTitle] = "Word をここにドロップ",
            [Txt.WordPdfEmptySubtitle] = "または「Word を追加」で複数選択",
            [Txt.WordPdfEmptyFormats] = ".doc .docx .docm .dot .dotx .dotm .rtf .odt",
            [Txt.WordPdfSettingsTitle] = "変換設定",
            [Txt.WordPdfMergeOption] = "1 つのファイルに結合",
            [Txt.WordPdfMergeHint] = "オフの場合は各 Word を同名の PDF として個別に出力",
            [Txt.WordPdfNameLabel] = "ファイル名",
            [Txt.WordPdfNamePlaceholder] = "空欄の場合は最初の Word のファイル名を使用",
            [Txt.WordPdfSeparateHint] = "PDF は Word と同じ名前で出力され、重複時は連番",
            [Txt.WordPdfOutput] = "保存先",
            [Txt.WordPdfNamePreview] = "出力：{0}",
            [Txt.WordPdfNamePreviewMulti] = "Word と同じ名前の PDF を {0} 個出力",
            [Txt.WordPdfStart] = "変換を開始",
            [Txt.WordPdfRunning] = "変換中…",
            [Txt.WordPdfCancel] = "キャンセル",
            [Txt.WordPdfCancelling] = "キャンセル中…",
            [Txt.WordPdfReady] = "準備完了 · 各 Word を個別に PDF 出力",
            [Txt.WordPdfReadyMerge] = "準備完了 · リスト順に 1 つの PDF に結合",
            [Txt.WordPdfEngineWord] = "Microsoft Word",
            [Txt.WordPdfEngineLibre] = "LibreOffice",
            [Txt.WordPdfProcessing] = "変換中（{0}/{1}）：{2}",
            [Txt.WordPdfMerging] = "PDF ページを結合中…",
            [Txt.WordPdfDone] = "変換完了 · {0} ページ · {1}",
            [Txt.WordPdfDoneFiles] = "変換完了 · {0} 個の PDF · {1}",
            [Txt.WordPdfCancelled] = "キャンセルしました · {0} 件完了",
            [Txt.WordPdfFailStatus] = "変換に失敗しました。文書を確認して再試行してください",
            [Txt.WordPdfFailTitle] = "変換失敗",
            [Txt.WordPdfNoFiles] = "変換する Word 文書を追加してください。",
            [Txt.WordPdfNeedFolder] = "保存先を選択してください。",
            [Txt.WordPdfNoEngine] = "Microsoft Word または LibreOffice が見つからないため、Word を変換できません。",
            [Txt.WordPdfNonWord] = "対応していない Word 文書です：{0}",
            [Txt.WordPdfWriteError] = "PDF を書き込めません：{0}",
            [Txt.WordPdfFailGeneric] = "Word の変換に失敗しました：{0}",
            [Txt.WordPdfAdded] = "{0} 個の文書を追加しました",
            [Txt.WordPdfSkippedNonWord] = "{0} 個の Word 以外のファイルを無視しました",
            [Txt.WordPdfNothingAdded] = "追加されたファイルはありません：重複、存在しない、または Word 以外",
            [Txt.WordPdfOutputCount] = "{0} 個の PDF",
            [Txt.MsgWordPdfBusy] = "Word を変換中です。完了するまでウィンドウを閉じないでください。",
            [Txt.TipNavWordPdf] = "Word → PDF：個別に出力、または 1 つのファイルに結合",
            [Txt.Feature7] = "Word を一括で PDF に変換。既定では元のファイル名で個別出力し、1 ファイルへの結合や出力への透かしも可能",
            [Txt.Changelog4521] = "4.5.21 · 2026-11-10",
            [Txt.ChangelogItems4521] =
                "#メンテナンス\n" +
                "· アプリ情報とプログラムのメタデータから追加のクレジットを削除し、作者を AI に統一\n" +
                "· リポジトリのアドレスを GitHub（github.com/swbths/DocHub）に更新\n",
            [Txt.Changelog4520] = "4.5.20 · 2026-11-10",
            [Txt.ChangelogItems4520] =
                "#改善\n" +
                "· 今日の一言が起動時と実行中にランダム表示され、順番どおりの切り替えではなくなり、連続で同じ句は出ません\n" +
                "· 自動切り替えの間隔が秒単位になり、既定値は 30 秒、設定で 5～86400 秒の範囲で変更できます\n",
            [Txt.Changelog4519] = "4.5.19 · 2026-11-10",
            [Txt.ChangelogItems4519] =
                "#改善\n" +
                "· アプリ内のすべてのダイアログが現在のスキンに追従。確認・警告・エラー・問い合わせの各ダイアログで背景色、文字、角丸、ボタンを統一\n" +
                "· パスワード入力も同じ枠なしスキンウィンドウになり、ダークスキンで白いシステムタイトルバーが表示されない\n",
            [Txt.Changelog4518] = "4.5.18 · 2026-11-10",
            [Txt.ChangelogItems4518] =
                "#新機能\n" +
                "· 左側に「Word → PDF」ワークスペースを追加。.doc/.docx/.docm/.dot/.dotx/.dotm/.rtf/.odt の一括変換に対応\n" +
                "· 既定では元のファイル名で PDF を個別出力し、重複時は連番。任意で「1 つのファイルに結合」可能\n" +
                "· 透かし設定に「Word → PDF」を追加。出力 PDF のみに適用し、元の文書は変更しない\n" +
                "· Microsoft Word または LibreOffice を自動で利用し、どちらもない場合は明確に案内\n",
            [Txt.Changelog4517] = "4.5.17 · 2026-11-06",
            [Txt.ChangelogItems4517] =
                "#改善\n" +
                "· 名言ライブラリから漢詩・漢文を削除し、すべて現代の名言に置き換え\n" +
                "#追加\n" +
                "· 国内外の名作映画の台詞と著名人の名言を追加。UI言語ごとに66件、6言語合計396件\n" +
                "· 『フォレスト・ガンプ』『ショーシャンクの空に』『ゴッドファーザー』『タイタニック』『ナタ 魔童降臨』『グランド・マスター』などの台詞を収録\n" +
                "· スティーブ・ジョブズ、ネルソン・マンデラ、ガンディー、キング牧師、ベーコン、チェーホフなどの名言を追加\n",
            [Txt.Changelog4516] = "4.5.16 · 2026-11-06",
            [Txt.ChangelogItems4516] =
                "#改善\n" +
                "· 「今日のひとこと」カードをすりガラス調に刷新：半透明の下地、上部のハイライト、角丸の縁取りでガラス基調のUIに統一\n" +
                "· カード左側の赤い縦線を廃止し、ナイトテーマでも赤い装飾が表示されないように修正\n" +
                "· 長い文章は文字サイズを自動で縮小し、途中で切れずに全文を表示\n" +
                "#追加\n" +
                "· 名言ライブラリを 348 件に拡充（各UI言語 58 件）、世界の著名人と名作映画の台詞を収録\n",
            [Txt.Changelog4515] = "4.5.15 · 2026-11-06",
            [Txt.ChangelogItems4515] =
                "#追加\n" +
                "· 「今日の一言」に自動切り替えを追加し、外観設定で間隔（既定 5 分）を変更できます\n" +
                "· 自動切り替えをオフにしても、カードをクリックすればすぐに次の一言に変わります\n",
            [Txt.Changelog4514] = "4.5.14 · 2026-11-06",
            [Txt.ChangelogItems4514] =
                "#修正\n" +
                "· 表示言語を切り替えた後、設定ウィンドウが赤い×だらけになったり未処理例外が発生したりしなくなりました\n" +
                "· ガラス背景キャッシュが誤って解放された場合は自動的に再構築され、同様の問題を防ぎます\n",
            [Txt.Changelog4513] = "4.5.13 · 2026-11-06",
            [Txt.ChangelogItems4513] =
                "#追加\n" +
                "· 結合ページ右側に「今日の一言」カードを追加し、世界の名言や映画の台詞を出典付きで表示します\n" +
                "· 簡体中文・繁體中文・English・日本語・한국어・Русский の 6 言語の語録を収録し、UI 言語の切り替えに連動します\n" +
                "· 外観設定に「結合ページに名言や映画の台詞を表示」スイッチを追加。カードをクリックすると次の一言へ切り替わります\n",
            [Txt.Changelog4512] = "4.5.12 · 2026-11-06",
            [Txt.ChangelogItems4512] =
                "#変更\n" +
                "· 透かし設定と完了通知の入口を左下へ移動し、スキン / 設定 / 情報と並べました\n" +
                "· 結合ページ右側のヒント文を進捗バーの直下へ移動しました\n" +
                "#修正\n" +
                "· 透かし入口のヒントから古い「文書結合のみ」の説明を削除しました\n",
            [Txt.Changelog4511] = "4.5.11 · 2026-11-06",
            [Txt.ChangelogItems4511] =
                "#変更\n" +
                "· 「ターミナル」スキンを削除しました。使用中の設定は自動的にクラシックブルーへ戻ります\n" +
                "#修正\n" +
                "· 右上のウォーターマークボタンのアイコンが 19px で欠けて表示される問題を修正しました\n",
            [Txt.Changelog4510] = "4.5.10 · 2026-11-06",
            [Txt.ChangelogItems4510] =
                "#修正\n" +
                "· 通常の左クリック（Ctrl+クリックを含む）では現在のファイルだけが選択され、前の行の選択はすぐに消えます\n" +
                "· 複数選択が残るのは範囲選択と Shift+クリックのみです\n",
            [Txt.Changelog459] = "4.5.9 · 2026-11-06",
            [Txt.ChangelogItems459] =
                "#修正\n" +
                "· 通常の左クリックではクリックしたファイルだけが選択され、前の行の選択が残らなくなりました。行内の上へ / 下へ / 削除ボタンも同じです\n" +
                "· Ctrl / Shift による複数選択は従来どおりです\n",
            [Txt.Changelog458] = "4.5.8 · 2026-11-06",
            [Txt.ChangelogItems458] =
                "#新機能\n" +
                "· スキン入口を左下の設定の上へ移動し、スキン / 設定 / バージョン情報をフラットな線画アイコンに統一\n" +
                "· ファイル一覧に「範囲選択を解除」ボタンを追加。選択前の状態に戻せ、Esc でも解除できます\n" +
                "· 削除、クリア、範囲選択の解除などの一覧ボタンにホバー ヒントを追加\n" +
                "#改善\n" +
                "· マウス左ドラッグで範囲選択する際に一覧が跳ねる問題を修正\n" +
                "#変更\n" +
                "· HUD SF スキンを削除。保存済みの設定はクラシックブルーに自動で戻ります\n",
            [Txt.Changelog457] = "4.5.7 · 2026-11-06",
            [Txt.ChangelogItems457] =
                "#新機能\n" +
                "· ファイル一覧の空白をドラッグして範囲選択でき、3 つのワークスペースすべてで複数選択に対応\n" +
                "· Ctrl を押しながら範囲選択すると既存の選択に追加\n" +
                "· 新しいスキン 4 種：スキューモーフィズム、ニューモーフィズム、ターミナル、HUD SF\n" +
                "#改善\n" +
                "· 範囲選択したファイルは Delete でまとめて削除\n" +
                "· NetEase Red スキンを削除。保存済みの設定はクラシックブルーに自動で戻ります\n",
            [Txt.Changelog456] = "4.5.6 · 2026-11-06",
            [Txt.ChangelogItems456] =
                "#改善\n" +
                "· PDF → 画像の既定の出力サブフォルダーに現在のページ指定（「レポート_偶数」「レポート_すべて」など）が付き、複数回の変換結果を区別しやすくなりました\n",
            [Txt.Changelog455] = "4.5.5 · 2026-11-06",
            [Txt.ChangelogItems455] =
                "#新機能\n" +
                "· PDF→画像で、すべて・奇数ページ・偶数ページ・カスタム範囲を選んで書き出せます\n" +
                "· カスタム範囲は 1-3,5,8-10 形式に対応し、元のページ番号でファイルを命名します\n" +
                "#改善\n" +
                "· プレビューとファイル名例が、現在のページ指定で最初に書き出すページに追従します\n" +
                "· 旧版で保存したページ範囲は自動的に「カスタム」へ移行します\n",
            [Txt.Changelog454] = "4.5.4 · 2026-11-05",
            [Txt.ChangelogItems454] =
                "#新機能\n" +
                "· フラットな新ブランドアイコン：ブルーの書類スタック＋琥珀色の統合バッジ\n" +
                "· 機能アイコンをフラットなツートーンに刷新し、小さいサイズでも見やすく統一\n" +
                "#修正\n" +
                "· 言語切り替え時に設定ウィンドウをその場で更新し、閉じ直しや中央への移動をなくしました\n",
            [Txt.Changelog453] = "4.5.3 · 2026-11-05",
            [Txt.ChangelogItems453] =
                "#新機能\n" +
                "· 透かし設定を文書結合・PDF→画像・画像→PDF ごとに適用可能。3 つのスイッチは既定でオフ\n" +
                "· PDF→画像は出力画像に透かしを合成でき、元 PDF は読み取り専用のまま\n" +
                "#改善\n" +
                "· PDF→画像の既定を 300PPI・JPEG・品質 100% に変更\n" +
                "· 完了音は既定でオフ。オフの間は音の選択肢をグレー表示し、オンにすると選択可能\n",
            [Txt.Changelog451] = "4.5.1 · 2026-11-05",
            [Txt.ChangelogItems451] =
                "#新機能\n" +
                "· 「画像→PDF」ワークスペースを追加：リスト順に画像を 1 つの PDF に\n" +
                "· ページサイズは A4 / A3 / A5 / B5 / Letter または画像に合わせるを選択、向きと余白に対応\n" +
                "· 命名・保存先・「完了後に開く」は文書結合の設定を共有\n" +
                "#改善\n" +
                "· Qoder ダークテーマを削除\n" +
                "#修正\n" +
                "· クラシックブルー、NetEase レッドのテーマで左側ナビの文字コントラストが低い問題を修正\n",
            [Txt.Changelog440] = "4.4.0 · 2026-11-05",
            [Txt.ChangelogItems440] =
                "#新機能\n" +
                "· 左側カード式ナビゲーション：文書結合と PDF→画像の 2 ワークスペース、Ctrl+1 / Ctrl+2 で切替\n" +
                "· PDF→画像を追加：PDF を PNG / JPEG / BMP / TIFF にページ単位で書き出し、形式・PPI・品質・ページ範囲を設定可能\n" +
                "· 一括変換、リアルタイムプレビュー、キャンセル、ファイル単位の失敗集計に対応。画像は選択した DPI 情報を保持\n" +
                "· Qoder ダークテーマを追加し、新規インストールの既定に\n" +
                "#改善\n" +
                "· Qoder スタイルで UI を刷新：カード型ナビゲーション、ページ見出し、単一のアクセント色と低コントラストの枠線\n" +
                "· 結合と変換で命名・保存先・完了通知を共有。変換専用の「前回の場所」も保持\n" +
                "· 出力名と保存先を直接編集可能。フォルダーのドロップは再帰スキャンに変更\n" +
                "· 設定に「PDF→画像」ページを追加し、透かしページに「文書結合のみ」と明記\n",

            [Txt.Changelog450] = "4.5.0 · 2026-11-05",
            [Txt.ChangelogItems450] =
                "#新機能\n" +
                "· 「文枢 DocHub」に名称変更：文書のハブ、結合も変換もワンクリック\n" +
                "· バージョン情報にブランドの定義と PDF→画像機能の説明を追加\n" +
                "#改善\n" +
                "· 左側ナビの「文書変換」を「PDF→画像」に変更し、ページ見出し・設定項目と統一\n" +
                "#修正\n" +
                "· 変換ページで「PDF を追加」ボタンがヒント文字に隠れる問題を修正\n" +
                "· ファイル数表示が削除・クリアボタンに重なる問題を修正\n" +
                "· アップグレード時に既存の設定を自動移行\n",

            [Txt.DefaultDocName] = "結合文書",
            [Txt.DefaultConvertName] = "変換画像",
            [Txt.MergeTimePrefix] = "結合_",
            [Txt.ConvertTimePrefix] = "変換_",
            [Txt.MergeCollideSuffix] = "_結合"
        };
    }
}
