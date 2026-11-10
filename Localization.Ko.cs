using System.Collections.Generic;

namespace DocHub
{
    internal static class LocKo
    {
        public static readonly Dictionary<Txt, string> Table = new()
        {
            [Txt.AppName] = "DocHub",
            [Txt.Tagline] = "문서 허브 — 병합과 변환을 한 번에",
            [Txt.LocalBadge] = "로컬 처리 · 파일은 외부로 전송되지 않습니다",

            [Txt.Ok] = "확인",
            [Txt.Close] = "닫기",
            [Txt.Cancel] = "취소",
            [Txt.Yes] = "예",
            [Txt.No] = "아니요",
            [Txt.Save] = "저장",
            [Txt.Reset] = "기본값 복원",
            [Txt.Browse] = "찾아보기",
            [Txt.PreviewWord] = "미리 듣기",
            [Txt.Settings] = "설정",
            [Txt.About] = "정보",
            [Txt.Skin] = "스킨",
            [Txt.RailMerge] = "병합",

            [Txt.DocsTitle] = "파일 목록",
            [Txt.DocCountFormat] = "{0}개 파일",
            [Txt.ListHint] = "끌어서 순서 변경 · 빈 곳을 드래그해 범위 선택 · 여러 파일을 한 번에 놓기",
            [Txt.AddDocs] = "파일 추가",
            [Txt.MoveUp] = "위로",
            [Txt.MoveDown] = "아래로",
            [Txt.MoveTop] = "맨 위로",
            [Txt.MoveBottom] = "맨 아래로",
            [Txt.Remove] = "삭제",
            [Txt.Clear] = "비우기",
            [Txt.OutputFile] = "출력 파일",
            [Txt.SaveTo] = "저장 위치",
            [Txt.ChooseFolder] = "저장 위치 선택",
            [Txt.StartMerge] = "병합 시작",
            [Txt.MergingButton] = "병합 중…",
            [Txt.ReadyStatus] = "준비 완료 · 목록 순서대로 위에서 아래로 병합",
            [Txt.NamePlaceholderFirst] = "첫 파일 이름 자동 사용",
            [Txt.NamePlaceholderPhrase] = "첫 파일 이름 + 사용자 문구",
            [Txt.NamePlaceholderTime] = "병합 시간 (자동 생성)",
            [Txt.NamePlaceholderCustom] = "파일 이름 입력",
            [Txt.EmptyTitle] = "Word / PDF / 이미지 파일을 여기에 끌어다 놓으세요",
            [Txt.EmptySubtitle] = "또는 오른쪽 위의 「파일 추가」를 클릭하여 한 번에 선택하세요",
            [Txt.EmptyFormats] = ".doc · .docx · .rtf · .pdf · .jpg · .png · .bmp · .tiff",

            [Txt.TipAdd] = "병합할 Word / PDF / 이미지 파일을 한 번에 선택",
            [Txt.TipUp] = "선택한 파일을 한 단계 위로 이동",
            [Txt.TipDown] = "선택한 파일을 한 단계 아래로 이동",
            [Txt.TipRemove] = "목록에서 제거하며 원본 파일은 삭제되지 않습니다",
            [Txt.TipClear] = "파일 목록 전체 비우기",
            [Txt.TipBrowse] = "병합된 PDF의 저장 위치 선택",
            [Txt.TipList] = "여기에 파일을 끌어다 놓거나 항목을 끌어서 순서를 바꾸세요",
            [Txt.TipStart] = "목록 순서대로 병합하여 PDF 생성",
            [Txt.TipSettings] = "이름 방식, 테마, 완료음, 언어",
            [Txt.TipAbout] = "버전, 업데이트 기록, 사용자 약관",
            [Txt.TipCancelBox] = "박스 선택을 취소하고 선택 전 상태로 되돌립니다(Esc)",
            [Txt.TrayOpen] = "주 창 열기",
            [Txt.TrayExit] = "종료",

            [Txt.DialogTitleAdd] = "병합할 파일 선택",
            [Txt.FolderDesc] = "병합된 PDF의 저장 위치 선택",
            [Txt.FilterSupport] = "지원 파일",
            [Txt.FilterWord] = "Word 문서",
            [Txt.FilterPdf] = "PDF 파일",
            [Txt.FilterImage] = "이미지 파일",
            [Txt.FilterAll] = "모든 파일",

            [Txt.StatusAdded] = "{0}개 파일 추가 · 목록 순서대로 병합",
            [Txt.StatusSkipped] = " (중복 또는 지원하지 않는 파일 {0}개 건너뜀)",
            [Txt.StatusNothing] = "추가된 파일이 없습니다: 중복, 존재하지 않거나 지원하지 않는 형식",
            [Txt.StatusOrder] = "순서 변경됨 · 현재 목록 순서대로 병합",
            [Txt.StatusRemoved] = "목록에서 제거: {0}",
            [Txt.StatusRemovedMulti] = "목록에서 {0}개를 제거했습니다",
            [Txt.StatusMovedTop] = "맨 위로 이동 · 목록 순서대로 병합합니다",
            [Txt.StatusMovedBottom] = "맨 아래로 이동 · 목록 순서대로 병합합니다",
            [Txt.StatusCleared] = "목록을 비웠습니다",
            [Txt.StatusFolderChosen] = "저장 위치를 선택했습니다",

            [Txt.MsgClearTitle] = "목록 비우기",
            [Txt.MsgClearText] = "파일 목록 전체를 비우시겠습니까?\n\n원본 파일은 삭제되지 않습니다.",
            [Txt.MsgMergeBusy] = "파일을 병합 중입니다. 완료된 후 창을 닫아 주세요.",
            [Txt.MsgNoWordTitle] = "Microsoft Word를 찾을 수 없음",
            [Txt.MsgNoWordText] = "Word 문서를 변환하려면 Microsoft Word가 필요합니다.\n\nMicrosoft Word를 설치하세요. PDF와 이미지만 병합하는 것은 가능합니다.",
            [Txt.MsgOverwriteTitle] = "덮어쓰시겠습니까?",
            [Txt.MsgOverwriteText] = "출력 파일이 이미 존재합니다:\n{0}\n\n덮어쓰시겠습니까?",

            [Txt.StatusPreparing] = "준비 중…",
            [Txt.StatusProgress] = "{0}% · {1}",
            [Txt.StatusDone] = "병합 완료 · 총 {0}페이지 · {1}",
            [Txt.BalloonTitle] = "병합 완료",
            [Txt.BalloonText] = "총 {0}페이지 · {1}\n{2}",
            [Txt.OpenFailed] = "PDF를 열 수 없습니다. 저장 위치에서 직접 확인해 주세요.",
            [Txt.FailStatus] = "병합 실패. 파일을 확인한 후 다시 시도하세요",
            [Txt.FailTitle] = "병합 실패",
            [Txt.FailPrefix] = "병합 실패: {0}",

            [Txt.ErrCannotStartTitle] = "병합을 시작할 수 없음",
            [Txt.ErrNeedDocs] = "먼저 병합할 문서를 추가하세요.",
            [Txt.ErrNeedName] = "출력 파일 이름을 입력하세요.",
            [Txt.ErrBadChars] = "파일 이름에는 다음 문자를 사용할 수 없습니다: \\ / : * ? \" < > |",
            [Txt.ErrNeedFolder] = "저장 위치를 선택하세요.",
            [Txt.ErrFolderMissing] = "저장 위치가 존재하지 않습니다:\n{0}",
            [Txt.ErrPathInvalid] = "출력 경로가 올바르지 않습니다.",
            [Txt.ErrSameSource] = "출력 파일이 원본 문서와 이름이 같아 원본을 덮어쓰게 됩니다.\n\n파일 이름 또는 저장 위치를 변경하세요.",
            [Txt.ElapsedSeconds] = "{0:0.0}초",
            [Txt.ElapsedMinutes] = "{0}분 {1}초",

            [Txt.SettingsTitle] = "설정",
            [Txt.NamingSection] = "출력 파일 이름",
            [Txt.NamingHint] = "병합 후 이 방식으로 파일 이름을 생성합니다",
            [Txt.NamingFirst] = "맨 위 파일의 파일 이름 사용",
            [Txt.NamingPhrase] = "맨 위 파일 이름 + 사용자 문구",
            [Txt.NamingTime] = "병합 시간 (자동 생성)",
            [Txt.NamingCustom] = "사용자 이름 사용",
            [Txt.PhrasePlaceholder] = "예: 최종본",
            [Txt.CustomPlaceholder] = "예: 2026년 계약서 모음",
            [Txt.PreviewFormat] = "이름 미리보기: {0}.pdf",
            [Txt.SkinSection] = "테마",
            [Txt.SkinHint] = "인터페이스 스타일 선택 — 즉시 적용됩니다",
            [Txt.DoneSection] = "병합 완료 후",
            [Txt.DoneNotify] = "작업 표시줄 오른쪽 아래에 알림 표시",
            [Txt.DoneSound] = "완료음 재생",
            [Txt.DoneOpen] = "생성된 PDF 자동 열기 (기본값 꺼짐)",
            [Txt.SoundSection] = "완료음",
            [Txt.SoundWater] = "물방울",
            [Txt.SoundDing] = "딩동",
            [Txt.SoundQQ] = "QQ",
            [Txt.SoundChime] = "차임",
            [Txt.SoundSoft] = "부드러움",
            [Txt.SoundOff] = "끔",
            [Txt.SoundHint] = "완료음을 먼저 켠 뒤 효과음을 선택하세요. 선택하면 바로 재생됩니다",
            [Txt.LanguageSection] = "인터페이스 언어",
            [Txt.LanguageHint] = "전환 즉시 적용됩니다",
            [Txt.QuoteSection] = "오늘의 한마디",
            [Txt.QuoteHint] = "UI 언어에 맞춰 바뀝니다",
            [Txt.QuoteToggle] = "병합 페이지에 명언이나 영화 대사 표시",
            [Txt.QuoteAutoRefresh] = "명언 자동 전환",
            [Txt.QuoteRefreshInterval] = "전환 간격",
            [Txt.QuoteRefreshSeconds] = "초",
            [Txt.QuoteRefreshHint] = "켜면 설정한 간격마다 자동으로 다음 문장으로 바뀝니다. 끄면 카드를 클릭해 바꿀 수 있습니다",
            [Txt.TipQuote] = "클릭하여 다른 문장 보기",
            [Txt.OutputSection] = "저장 위치",
            [Txt.OutputHint] = "병합된 PDF를 저장할 위치",
            [Txt.OutputFirst] = "맨 위 파일의 폴더",
            [Txt.OutputLast] = "마지막 저장 위치",
            [Txt.OutputCustom] = "사용자 지정 폴더",
            [Txt.OutputCustomPlaceholder] = "폴더를 선택하거나 경로 입력",
            [Txt.SkinBlueName] = "클래식 블루",
            [Txt.SkinBlueDesc] = "산뜻하고 맑음",
            [Txt.SkinNightName] = "야간 모드",
            [Txt.SkinNightDesc] = "몰입형 다크",
            [Txt.SkinCloudName] = "클라우드 화이트",
            [Txt.SkinCloudDesc] = "따뜻한 종이 질감",
            [Txt.SkinSkeuoName] = "스큐어모피즘",
            [Txt.SkinSkeuoDesc] = "가죽과 종이 질감",
            [Txt.SkinNeumorphName] = "뉴모피즘",
            [Txt.SkinNeumorphDesc] = "부드러운 입체감",
            [Txt.SettingsLiveHint] = "모든 변경 사항이 즉시 적용됩니다",

            [Txt.AboutTitle] = "정보",
            [Txt.VersionLine] = "{0} · Word / PDF / 이미지 병합·변환 도구",
            [Txt.AboutIntro] = "모든 처리는 이 PC에서 완료되며 파일은 외부로 전송되지 않습니다",
            [Txt.BrandDefinition] = "DocHub: 문서 허브 — 병합과 변환을 한 번에",
            [Txt.Feature1] = "Word 모든 형식, PDF, 이미지를 한 번에 병합, 크기 제한 없음",
            [Txt.Feature2] = "원본 레이아웃, 페이지 번호, 이미지 크기와 방향 그대로 유지",
            [Txt.Feature3] = "끌어서 순서 변경, 실시간 진행률, 출력 순서 완전 제어",
            [Txt.Feature4] = "완전 오프라인 처리, 파일은 어떤 서버에도 업로드되지 않습니다",
            [Txt.ChangelogTitle] = "업데이트 기록",
            [Txt.Changelog300] = "3.0.0 · 2026-11-03",
            [Txt.ChangelogItems300] =
                "#새 기능\n" +
                "· 설치 프로그램에 완전한 .NET 런타임을 포함하여 .NET이 없는 PC에서도 설치 후 바로 사용 가능\n" +
                "· 간단한 PDF 뷰어를 내장하여 PDF 리더가 없는 PC에서도 병합 결과를 확인 가능\n" +
                "· 정보 페이지에 제작자 표시와 연락처 추가 (cado@163.com)\n" +
                "#개선\n" +
                "· 설정 항목을 변경하면 즉시 적용되며 「저장」 버튼이 필요 없습니다\n" +
                "· 완료음을 선택하면 자동으로 재생되며 「미리 듣기」 버튼이 필요 없습니다\n" +
                "· 병합 속도 향상: 이미지를 병렬 변환하고 결과를 대상 파일에 직접 기록\n" +
                "· 설치 후 본 프로그램은 가볍게 유지되고 런타임은 설치 프로그램과 함께 배포\n" +
                "#수정\n" +
                "· 아이콘 그래픽이 위로 치우친 문제를 수정하여 수직 중앙에 정렬\n" +
                "· Word가 없을 때 PDF와 이미지를 병합할 수 없는 문제 수정: LibreOffice 지원 또는 Word 건너뛰기",
            [Txt.Changelog200] = "2.0.0 · 2026-11-03",
            [Txt.ChangelogItems200] =
                "#새 기능\n" +
                "· JPG, PNG, BMP, TIFF 등 이미지를 직접 병합하고 원본 크기와 방향 유지\n" +
                "· Word, PDF, 이미지를 같은 목록에서 혼합 병합하여 하나의 PDF로 출력\n" +
                "· 설치 직후 인터페이스 언어가 시스템 언어를 자동으로 따름\n" +
                "#개선\n" +
                "· 앱 전체 문구를 「파일」로 통일, 목록 배지는 PDF / WORD / 이미지를 구분\n" +
                "· 설정 화면을 전면 플랫화: 단색 구역, 얇은 구분선, 그라데이션과 그림자 제거\n" +
                "· 업데이트 기록을 「새 기능 / 개선 / 수정」으로 분류하여 더 명확하게\n" +
                "#수정\n" +
                "· PDF나 이미지만 병합할 때 Word가 없다는 경고가 표시되는 문제 수정",
            [Txt.Changelog130] = "1.3.0 · 2026-11-03",
            [Txt.ChangelogItems130] =
                "#새 기능\n" +
                "· 저장 위치 설정 추가: 맨 위 파일 폴더, 마지막 저장 위치, 사용자 지정 경로\n" +
                "· 파일 목록에서 우클릭 삭제·위로·아래로 이동 지원\n" +
                "· 설치 프로그램과 압축하지 않은 포터블 패키지로 배포\n" +
                "#개선\n" +
                "· 설정 저장 후 즉시 적용되며 재시작이 필요 없습니다\n" +
                "· 진행률 표시줄이 테마 색상을 따르고 백분율을 표시합니다\n" +
                "· 파일 목록 개편: 카드형 행, 유형 배지, 파일 정보\n" +
                "#수정\n" +
                "· 목록 깜빡임과 흔들림 수정\n" +
                "· 같은 이름 파일 덮어쓰기 문제 수정, A-1.pdf처럼 자동 번호 부여",
            [Txt.Changelog120] = "1.2.0 · 2026-11-03",
            [Txt.ChangelogItems120] =
                "#새 기능\n" +
                "· 새 플랫 아이콘, 글자 없이 더 간결한 형태\n" +
                "· 완료음 5종 내장: 물방울, 딩동, QQ 등, 미리 듣기 지원\n" +
                "· 정보 페이지에 「사용자 약관」 추가\n" +
                "· 번체 중국어, 영어, 일본어, 러시아어, 한국어 인터페이스 추가, 즉시 전환\n" +
                "#개선\n" +
                "· 모서리, 그림자, 여백, 호버 피드백 등 UI 전반 개선",
            [Txt.Changelog110] = "1.1.0 · 2026-11-03",
            [Txt.ChangelogItems110] =
                "#새 기능\n" +
                "· 4가지 이름 방식: 첫 파일 이름, 첫 파일 이름+문구, 병합 시간, 사용자 이름\n" +
                "· NetEase 스타일 테마 4종: 클래식 블루, NetEase 레드, 야간, 클라우드 화이트\n" +
                "#개선\n" +
                "· 「설정」과 「정보」 추가, 아이콘을 플랫 스타일로 변경\n" +
                "· 병합 후 PDF를 자동으로 열지 않고 오른쪽 아래 알림과 완료음으로 변경, 설정 가능",
            [Txt.Changelog100] = "1.0.0 · 2026-11-02",
            [Txt.ChangelogItems100] =
                "#새 기능\n" +
                "· 첫 버전: Word 모든 형식 + PDF 병합, 한 번에 불러오기, 끌어서 정렬, 진행률 표시",
            [Txt.ContactAuthor] = "제작자: AI · 기획: chao",
            [Txt.Copyright] = "© 2026 AI · 로컬 처리로 더 안전하게",
            [Txt.EulaButton] = "사용자 약관",
            [Txt.EulaTitle] = "사용자 약관",
            [Txt.EulaIntro] = "DocHub를 사용하기 전에 아래 약관을 읽어 주세요.",
            [Txt.EulaBody] =
                "1. 용도\n" +
                "DocHub는 Word / PDF / 이미지 파일을 로컬에서 병합·변환하는 데스크톱 도구입니다. PC에 설치된 Microsoft Word 또는 LibreOffice를 " +
                "사용하여 Word 문서를 PDF로 내보내고, 이미지는 원본 크기로 페이지에 배치한 뒤 지정한 순서대로 병합합니다.\n\n" +
                "2. 파일\n" +
                "모든 처리는 이 PC에서 이루어집니다. 문서를 업로드, 전송, 백업하지 않으며 개인정보나 문서 내용을 " +
                "수집하지 않습니다.\n\n" +
                "3. 책임\n" +
                "병합하려는 문서에 대한 합법적인 사용 권한이 있는지 확인하십시오. 병합 전에 중요한 파일을 " +
                "백업하십시오. 파일 손상, 덮어쓰기, 부적절한 사용으로 인한 손실에 대해 작성자는 책임지지 않습니다.\n\n" +
                "4. 실행 환경\n" +
                "Word 문서를 병합하려면 Microsoft Word 또는 LibreOffice가 설치되어 있어야 합니다. 이 소프트웨어는 " +
                "Office를 제공하거나 인증하지 않습니다.\n\n" +
                "5. 보증 부인\n" +
                "이 소프트웨어는 있는 그대로 제공됩니다. 신중히 테스트하지만 모든 환경에서 모든 문서가 완벽하게 " +
                "병합된다고 보장하지 않습니다.\n\n" +
                "6. 약관 변경\n" +
                "이 약관은 버전 업데이트에 따라 변경될 수 있습니다. 계속 사용하면 변경된 약관에 동의하는 것으로 " +
                "간주됩니다.",
            [Txt.EulaAgree] = "읽었으며 동의합니다",

            [Txt.MergeNoDocs] = "먼저 병합할 문서를 추가하세요.",
            [Txt.MergeFileMissing] = "파일이 없거나 이동되었습니다:\n{0}",
            [Txt.MergeWordMissing] =
                "Microsoft Word 또는 LibreOffice를 찾을 수 없어 Word 문서를 변환할 수 없습니다.\n\n" +
                "Microsoft Word 또는 LibreOffice를 설치하세요. PDF와 이미지만 병합하는 것은 영향이 없습니다.",
            [Txt.MergeWordStartFail] = "Microsoft Word를 시작할 수 없습니다. Word가 올바르게 설치되어 정상적으로 열리는지 확인하세요.",
            [Txt.MergeWordNotInstalled] = "Microsoft Word 또는 LibreOffice를 찾을 수 없습니다. 설치한 후 다시 시도하세요.",
            [Txt.MergeProcessing] = "처리 중 ({0}/{1}): {2}",
            [Txt.MergeStartingWord] = "Word 변환 엔진 시작 중…",
            [Txt.MergeMergingPages] = "페이지 병합 중…",
            [Txt.MergeMerging] = "병합 중 ({0}/{1}): {2}",
            [Txt.MergePdfUnreadable] = "PDF「{0}」을 읽을 수 없습니다: 파일이 손상되었거나 암호화되었거나 지원되지 않는 형식일 수 있습니다.",
            [Txt.MergeNoPages] = "병합할 페이지가 없습니다.",
            [Txt.MergePdfError] = "PDF 병합 중 오류가 발생했습니다. 출력 파일이 생성되지 않았습니다.",
            [Txt.MergeWriteError] =
                "출력 파일에 쓸 수 없습니다:\n{0}\n\n다른 프로그램에서 열려 있지 않은지, 저장 위치에 쓸 수 있는지 확인하세요.\n\n기술 정보: {1}",
            [Txt.MergeNoPermission] = "출력 파일에 대한 쓰기 권한이 없습니다:\n{0}\n\n다른 저장 위치를 선택한 후 다시 시도하세요.",
            [Txt.MergeFailedGeneric] = "병합 실패: {0}",
            [Txt.MergeTempFail] = "병합용 임시 폴더를 만들 수 없습니다.\n\n보안 프로그램이 파일 쓰기를 차단했거나 임시 폴더 권한에 문제가 있습니다.\n이 프로그램을 허용 목록에 추가하거나 관리자 권한으로 실행한 후 다시 시도하세요.",
            [Txt.MergeDone] = "병합 완료",
            [Txt.MergeWordOpenFail] = "「{0}」을 열 수 없습니다.",
            [Txt.MergeWordConvertEmpty] = "「{0}」 변환 후 유효한 페이지가 없습니다.",
            [Txt.MergeWordConvertFail] =
                "Word 문서「{0}」을 변환할 수 없습니다.\n\n" +
                "파일이 암호화되지 않았고 손상되지 않았으며 PC의 Microsoft Word에서 정상적으로 열리는지 확인하세요.\n\n기술 정보: {1}",
            [Txt.MergePasswordTitle] = "암호로 보호된 파일",
            [Txt.MergePasswordHint] = "이 파일을 여는 암호를 입력하세요. 이번 병합에서만 사용됩니다.",
            [Txt.MergePasswordPlaceholder] = "암호",
            [Txt.MergePasswordWrong] = "암호가 올바르지 않습니다. 다시 입력하세요.",
            [Txt.MergeImageConvertFail] =
                "이미지「{0}」을 읽을 수 없습니다.\n\n" +
                "이미지가 손상되지 않았고 JPG, PNG, BMP, TIFF 등 일반적인 형식인지 확인하세요.\n\n기술 정보: {1}",

            [Txt.MsgSkipWordTitle] = "Word 파일을 변환할 수 없습니다",
            [Txt.MsgSkipWordText] =
                "Microsoft Word 또는 LibreOffice를 찾을 수 없어 {0}개의 Word 파일을 변환할 수 없습니다.\n\n" +
                "이 Word 파일을 건너뛰고 나머지 PDF와 이미지 병합을 계속하시겠습니까?",
            [Txt.StatusSkippedWord] = "Word 파일 {0}개를 건너뛰었습니다 (Word / LibreOffice 미설치)",

            [Txt.ViewResult] = "병합 결과 보기",
            [Txt.PdfViewerTitle] = "PDF 뷰어",
            [Txt.PrevPage] = "이전",
            [Txt.NextPage] = "다음",
            [Txt.ZoomIn] = "확대",
            [Txt.ZoomOut] = "축소",
            [Txt.ViewerPageFormat] = "{0} / {1} 페이지",
            [Txt.ViewerNoReader] = "PDF 리더가 없어 내장 뷰어를 사용합니다",
            [Txt.ViewerLoadFailed] = "PDF 파일을 열 수 없습니다:\n{0}",

            [Txt.SettingsNavLook] = "모양",
            [Txt.SettingsNavNaming] = "이름 및 저장",
            [Txt.SettingsNavDone] = "완료 알림",
            [Txt.SettingsNavWatermark] = "워터마크",
            [Txt.WatermarkSection] = "문서 워터마크",
            [Txt.WatermarkHint] = "워터마크를 적용할 기능을 선택하세요. 스타일은 세 기능이 공유하며 기본값은 꺼짐",
            [Txt.WatermarkEnable] = "워터마크 추가",
            [Txt.WatermarkForMerge] = "문서 병합",
            [Txt.WatermarkForPdfImage] = "PDF → 이미지",
            [Txt.WatermarkForImagePdf] = "이미지 → PDF",
            [Txt.WatermarkForWordPdf] = "Word → PDF",
            [Txt.WatermarkPresets] = "기본 문구",
            [Txt.WatermarkCustom] = "사용자 지정",
            [Txt.WatermarkPreset1] = "원본",
            [Txt.WatermarkPreset2] = "사본",
            [Txt.WatermarkPreset3] = "기밀",
            [Txt.WatermarkPreset4] = "극비",
            [Txt.WatermarkPreview] = "미리보기",
            [Txt.WatermarkCustomPlaceholder] = "워터마크 문구 입력",
            [Txt.WatermarkPreviewHint] = "미리보기에서 드래그로 이동, 휠로 크기 조절, 오른쪽 드래그로 회전. 값을 직접 입력할 수도 있습니다.",
            [Txt.WatermarkTint] = "색상",
            [Txt.WatermarkTintGray] = "회색",
            [Txt.WatermarkTintRed] = "빨강",
            [Txt.WatermarkTintBlue] = "파랑",
            [Txt.WatermarkPosition] = "위치",
            [Txt.WatermarkPosTile] = "타일",
            [Txt.WatermarkPosTopLeft] = "왼쪽 위",
            [Txt.WatermarkPosTopRight] = "오른쪽 위",
            [Txt.WatermarkPosCenter] = "가운데",
            [Txt.WatermarkPosBottomLeft] = "왼쪽 아래",
            [Txt.WatermarkPosBottomRight] = "오른쪽 아래",
            [Txt.WatermarkOpacity] = "불투명도",
            [Txt.WatermarkScale] = "크기",
            [Txt.WatermarkRotation] = "기울기 각도",
            [Txt.WatermarkOffsetX] = "가로 위치",
            [Txt.WatermarkOffsetY] = "세로 위치",
            [Txt.ToastTitle] = "병합 완료",
            [Txt.ToastOpen] = "보기",
            [Txt.Changelog310] = "3.1.0 · 2026-11-03",
            [Txt.ChangelogItems310] =
                "#추가\n" +
                "· 설정 화면을 왼쪽 메뉴 + 오른쪽 내용의 2단 구조로 개편\n" +
                "· 문서 워터마크 추가: 원본/사본/기밀/극비 기본 문구, 사용자 지정 문구·스타일·색상 지원\n" +
                "#개선\n" +
                "· 설치 폴더 정리: 작은 실행기 + app 폴더 구조\n" +
                "· 병합 완료 알림을 테마에 맞춘 오른쪽 아래 카드로 변경\n",
            [Txt.Changelog400] = "4.0.0 · 2026-11-03",
            [Txt.ChangelogItems400] =
                "#추가\n" +
                "· 파일 목록에서 여러 개를 선택해 일괄 처리, 마우스 오른쪽 버튼으로 「맨 위로」/「맨 아래로」\n" +
                "#개선\n" +
                "· 파일 목록 행 디자인을 정돈하고 선택 상태를 더 분명하게\n" +
                "· 창 크기 변경 시 레이아웃을 재계산하여 상태 줄이 「병합 시작」 버튼을 가리는 문제 수정\n" +
                "· 인터페이스 언어를 바꾸면 기본 출력 이름도 해당 언어로 변경\n" +
                "· 기본 설정으로 복원하면 언어도 중국어 간체로 복원\n" +
                "· 설치 프로그램의 기본 설치 경로를 C:\\Program Files (x86)\\DocHub 으로 변경\n",
            [Txt.Changelog430] = "4.3.0 · 2026-11-04",
            [Txt.ChangelogItems430] =
                "#새 기능\n" +
                "· 메인 화면 내비게이션을 오른쪽 위로 이동: 모양·워터마크·완료 알림·설정·정보를 평면 아이콘으로 배치하고, 클릭하면 해당 설정 페이지로 바로 이동\n" +
                "#수정\n" +
                "· 워터마크가 너무 크거나 가장자리로 밀릴 때 잘리던 문제 수정: 페이지 안에 맞도록 자동 축소되어 병합 후에도 온전하게 표시\n" +
                "· 미리 보기에서 워터마크가 미리 보기 틀을 벗어나지 않도록 개선\n" +
                "#개선\n" +
                "· 「물방울」「QQ」알림음을 다시 합성해 더 맑은 어택과 자연스러운 잔향으로 다듬고, 알림음 간 음량 통일\n" +
                "· 메인 화면 왼쪽 사이드바를 제거해 UI를 더 깔끔하게\n",
            [Txt.Changelog420] = "4.2.0 · 2026-11-04",
            [Txt.ChangelogItems420] =
                "#최적화\n" +
                "· 설정 및 정보 창이 즉시 열립니다: 반투명 배경을 스킨과 크기별로 캐시하고 미리 생성합니다\n" +
                "· 워터마크 미리보기 드래그가 부드러워졌습니다: 타일링을 미리 만든 텍스처 채우기로 바꿔 최소 배율 기준 프레임당 190ms를 1ms 이내로 단축\n" +
                "#개선\n" +
                "· 왼쪽 탐색 재설계: 아이콘과 이름의 계층이 명확해지고 선택 항목은 강조색 알약과 왼쪽 막대로 표시\n" +
                "· 버전 번호를 시맨틱 버전(SemVer) 규칙에 따라 갱신: 경험과 성능 개선은 마이너 버전 상승\n",
            [Txt.Changelog412] = "4.1.2 · 2026-11-04",
            [Txt.ChangelogItems412] =
                "#새 기능\n" +

                "· 워터마크 위치 프리셋: 타일 / 왼쪽 위 / 오른쪽 위 / 가운데 / 왼쪽 아래 / 오른쪽 아래\n" +

                "#개선\n" +

                "· 워터마크 설정 끊김 개선: 연속 드래그는 한 번만 반영하고 미리보기 그리기 캐시\n" +

                "· 메인 창 왼쪽 사이드바에 기능 이름(병합 / 설정 / 정보) 표시, 선택 항목은 테마 색으로 강조\n",

            [Txt.Changelog411] = "4.1.1 · 2026-11-04",
            [Txt.ChangelogItems411] =
                "#수정\n" +
                "· 설치 프로그램이 기본 폴더로 강제 변경하지 않고 지정한 설치 경로를 그대로 사용합니다\n" +
                "· 이미지 병합 실패 문제 수정: WebP / HEIC 등은 시스템 디코더로 처리하고 일반 이미지 호환성도 향상\n" +
                "#개선\n" +
                "· 워터마크 값을 직접 입력 가능. 미리보기에서 드래그 이동, 휠 크기 조절, 오른쪽 드래그 회전\n" +
                "· 「스타일」 선택을 제거하고 대각선 타일 효과로 통일\n",

            [Txt.Changelog410] = "4.1.0 · 2026-11-04",
            [Txt.ChangelogItems410] =
                "#추가\n" +
                "· 워터마크의 불투명도·크기·위치·기울기를 임의로 조정할 수 있고 설정 화면에서 실시간 미리보기\n" +
                "· 설치 프로그램을 자체 구현으로 변경: 현재 사용자 폴더에 설치하며 관리자 권한이 필요 없고 Program Files에 기록하지 않습니다\n" +
                "· 시작 메뉴에 「文枢 제거」 바로가기 추가\n" +
                "#개선\n" +
                "· 설정 창을 세로로 키우고 워터마크 조정을 슬라이더로 변경하여 더 쉽게 조작\n",

            [Txt.NavMergeTitle] = "문서 병합",
            [Txt.NavMergeSubtitle] = "PDF로 병합",
            [Txt.NavConvertTitle] = "PDF → 이미지",
            [Txt.NavConvertSubtitle] = "페이지별 이미지로 저장",
            [Txt.MergePageTitle] = "문서 병합",
            [Txt.MergePageSubtitle] = "목록 순서대로 Word / PDF / 이미지를 하나의 PDF로 병합",
            [Txt.ConvertPageTitle] = "PDF → 이미지",
            [Txt.ConvertPageSubtitle] = "PDF의 모든 페이지를 이미지로 내보내기 · 원본 PDF는 읽기 전용",
            [Txt.ConvertFilesTitle] = "PDF 파일",
            [Txt.ConvertCountFormat] = "{0}개 파일",
            [Txt.ConvertListHint] = "빈 곳을 드래그해 범위 선택 · PDF 추가 시 페이지 수 자동",
            [Txt.ConvertAdd] = "PDF 추가",
            [Txt.ConvertDialogTitle] = "변환할 PDF 선택",
            [Txt.ConvertEmptyTitle] = "PDF를 여기에 끌어다 놓으세요",
            [Txt.ConvertEmptySubtitle] = "또는 「PDF 추가」를 클릭하여 여러 개 선택",
            [Txt.ConvertEmptyFormats] = ".pdf",
            [Txt.ConvertPageCountFormat] = "{0}페이지",
            [Txt.ConvertPageCountUnknown] = "페이지 수 읽는 중…",
            [Txt.ConvertPageCountFailed] = "읽을 수 없음",
            [Txt.ConvertSettingsTitle] = "내보내기 설정",
            [Txt.ConvertFormat] = "이미지 형식",
            [Txt.ConvertPpi] = "해상도(PPI)",
            [Txt.ConvertPpiCustom] = "사용자 지정",
            [Txt.ConvertPpiHint] = "PPI는 선명도를 결정하며 페이지의 물리적 크기는 바뀌지 않습니다",
            [Txt.ConvertQuality] = "JPEG 품질",
            [Txt.ConvertBackground] = "투명 배경",
            [Txt.ConvertBackgroundHint] = "투명은 PNG만 지원하며 다른 형식은 흰색 배경입니다",
            [Txt.ConvertPageRange] = "페이지 범위",
            [Txt.ConvertPageRangeAll] = "전체",
            [Txt.ConvertPageRangePlaceholder] = "예: 1-3,5,8-10",
            [Txt.ConvertPageModeAll] = "전체",
            [Txt.ConvertPageModeOdd] = "홀수",
            [Txt.ConvertPageModeEven] = "짝수",
            [Txt.ConvertPageModeCustom] = "직접",
            [Txt.ConvertSubfolder] = "PDF마다 별도 폴더 만들기",
            [Txt.ConvertSubfolderHint] = "일괄 변환 시 자동으로 켜지며 폴더 이름에 페이지 선택이 붙습니다",
            [Txt.ConvertOutput] = "저장 위치",
            [Txt.ConvertNamePreview] = "이름 미리보기: {0}",
            [Txt.ConvertEstimateFormat] = "예상 {0} × {1} px",
            [Txt.ConvertPreviewTitle] = "페이지 {0} 미리보기",
            [Txt.ConvertPreviewEmpty] = "PDF를 선택하면 선택한 페이지를 표시합니다",
            [Txt.ConvertStart] = "변환 시작",
            [Txt.ConvertRunning] = "변환 중…",
            [Txt.ConvertCancel] = "취소",
            [Txt.ConvertCancelling] = "취소 중…",
            [Txt.ConvertReady] = "준비 완료 · 목록 순서대로 이미지 내보내기",
            [Txt.ConvertProcessing] = "변환 중 ({0}/{1}): {2}",
            [Txt.ConvertPageStatus] = "{0} / {1} 페이지",
            [Txt.ConvertDone] = "변환 완료 · 이미지 {0}장 · {1}",
            [Txt.ConvertDonePartial] = "완료 {0}개 · 실패 {1}개 · 이미지 {2}장",
            [Txt.ConvertCancelled] = "취소됨 · {0}장 내보냄",
            [Txt.ConvertFailStatus] = "변환 실패. 파일을 확인한 후 다시 시도하세요",
            [Txt.ConvertFailTitle] = "변환 실패",
            [Txt.ConvertOpenFolder] = "폴더 열기",
            [Txt.ConvertOutputSubfolder] = "출력: {0}",
            [Txt.ConvertNoFiles] = "먼저 변환할 PDF를 추가하세요.",
            [Txt.ConvertNeedFolder] = "저장 위치를 선택하세요.",
            [Txt.ConvertBadRange] = "페이지 범위 형식이 올바르지 않습니다. 1-3,5 형식으로 입력하세요.",
            [Txt.ConvertRangeOut] = "페이지 범위가 문서 페이지 수({0}페이지)를 초과합니다.",
            [Txt.ConvertNoPages] = "변환할 페이지가 없습니다.",
            [Txt.ConvertRangeNeeded] = "페이지 범위를 입력하거나 「전체」를 선택하세요.",
            [Txt.ConvertNoMatchingPages] = "현재 페이지 설정에 변환할 페이지가 없습니다.",
            [Txt.ConvertUnreadable] = "PDF「{0}」을 읽을 수 없습니다: 파일이 손상되었거나 암호화되었거나 지원되지 않는 형식일 수 있습니다.",
            [Txt.ConvertPpiTooLarge] = "선택한 PPI에서는 페이지가 너무 큽니다({0} × {1} px). PPI를 낮추세요.",
            [Txt.ConvertWriteError] = "이미지를 쓸 수 없습니다:\n{0}\n\n기술 정보: {1}",
            [Txt.ConvertMemory] = "메모리가 부족하여 이 페이지를 렌더링할 수 없습니다. PPI를 낮춘 후 다시 시도하세요.",
            [Txt.ConvertSkippedNonPdf] = "PDF가 아닌 파일 {0}개를 무시했습니다",
            [Txt.ConvertDuplicate] = "중복 파일 {0}개를 건너뛰었습니다",
            [Txt.ConvertAdded] = "PDF 파일 {0}개를 추가했습니다",
            [Txt.ConvertNothingAdded] = "추가된 파일이 없습니다: 중복, 존재하지 않거나 PDF가 아닌 파일",
            [Txt.MsgConvertBusy] = "변환 중입니다. 완료된 후 창을 닫아 주세요.",
            [Txt.SettingsNavConvert] = "PDF → 이미지",
            [Txt.ConvertSection] = "PDF → 이미지 기본값",
            [Txt.ConvertDefaultsHint] = "변환 페이지의 초기값입니다. 변환 페이지에서 변경한 값은 기억됩니다",
            [Txt.ConvertDefaultFormat] = "기본 이미지 형식",
            [Txt.ConvertDefaultPpi] = "기본 PPI",
            [Txt.ConvertDefaultQuality] = "기본 JPEG 품질",
            [Txt.ConvertDefaultBackground] = "기본 투명 배경",
            [Txt.ConvertDefaultSubfolder] = "기본으로 PDF마다 폴더 만들기",
            [Txt.ConvertWatermarkNote] = "워터마크는 출력 이미지에만 합성되며 원본 PDF는 읽기 전용으로 유지됩니다. '워터마크' 페이지에서 켤 수 있습니다",
            [Txt.NamingHintShared] = "병합과 PDF → 이미지가 공유하는 출력 이름 방식",
            [Txt.DoneOpenShared] = "완료 후 결과 자동 열기 (병합은 PDF, 변환은 폴더)",
            [Txt.TipNavMerge] = "문서 병합: 목록 순서대로 하나의 PDF로",
            [Txt.TipNavConvert] = "PDF → 이미지: PDF의 각 페이지를 이미지로 내보내기",
            [Txt.TipAppearance] = "모양 설정 열기",
            [Txt.TipNotifications] = "완료 알림 설정 열기",
            [Txt.TipWatermark] = "워터마크 설정 열기",
            [Txt.NavImagePdfTitle] = "이미지 → PDF",
            [Txt.NavImagePdfSubtitle] = "이미지를 개별 또는 하나의 PDF로",
            [Txt.ImagePdfPageTitle] = "이미지 → PDF",
            [Txt.ImagePdfPageSubtitle] = "이미지를 개별 출력하거나 하나의 PDF로 병합 · 로컬 처리",
            [Txt.ImagePdfFilesTitle] = "이미지 파일",
            [Txt.ImagePdfCountFormat] = "{0}개 파일",
            [Txt.ImagePdfListHint] = "빈 곳을 드래그해 범위 선택 · 목록 순서대로 변환",
            [Txt.ImagePdfAdd] = "이미지 추가",
            [Txt.ImagePdfDialogTitle] = "변환할 이미지 선택",
            [Txt.ImagePdfEmptyTitle] = "이미지를 여기에 끌어다 놓으세요",
            [Txt.ImagePdfEmptySubtitle] = "또는 「이미지 추가」를 클릭하여 여러 개 선택",
            [Txt.ImagePdfEmptyFormats] = ".jpg .jpeg .png .bmp .tif .tiff .gif .webp",
            [Txt.ImagePdfSettingsTitle] = "PDF 설정",
            [Txt.ImagePdfPageSize] = "페이지 크기",
            [Txt.ImagePdfPageSizeAuto] = "자동",
            [Txt.ImagePdfOrientation] = "페이지 방향",
            [Txt.ImagePdfOrientationAuto] = "자동",
            [Txt.ImagePdfOrientationPortrait] = "세로",
            [Txt.ImagePdfOrientationLandscape] = "가로",
            [Txt.ImagePdfMargin] = "여백",
            [Txt.ImagePdfMarginNone] = "없음",
            [Txt.ImagePdfMarginSmall] = "10 mm",
            [Txt.ImagePdfMarginLarge] = "20 mm",
            [Txt.ImagePdfFitHint] = "자동은 페이지가 이미지에 맞춰짐\n고정 크기는 비율을 유지해 맞춤",
            [Txt.ImagePdfMergeOption] = "하나의 PDF로 병합",
            [Txt.ImagePdfNameLabel] = "파일 이름",
            [Txt.ImagePdfNamePlaceholderMerge] = "비우면 병합 이름 사용",
            [Txt.ImagePdfNamePlaceholderSeparate] = "비우면 원본 이미지 이름 사용",
            [Txt.ImagePdfNamePreview] = "이름 미리보기: {0}",
            [Txt.ImagePdfNamePreviewMulti] = "{0} 외 총 {1}개 PDF",
            [Txt.ImagePdfPageCountFormat] = "이미지 {0}장",
            [Txt.ImagePdfOutputCount] = "PDF {0}개",
            [Txt.ImagePdfPreviewTitle] = "미리보기",
            [Txt.ImagePdfPreviewEmpty] = "이미지를 선택하면 미리보기를 표시합니다",
            [Txt.ImagePdfStart] = "변환 시작",
            [Txt.ImagePdfRunning] = "변환 중…",
            [Txt.ImagePdfCancel] = "취소",
            [Txt.ImagePdfCancelling] = "취소 중…",
            [Txt.ImagePdfReady] = "준비 완료 · 이미지마다 개별 PDF 출력",
            [Txt.ImagePdfReadyMerge] = "준비 완료 · 목록 순서대로 하나의 PDF로 병합",
            [Txt.ImagePdfProcessing] = "변환 중 ({0}/{1}): {2}",
            [Txt.ImagePdfDone] = "변환 완료 · {0}페이지 · {1}",
            [Txt.ImagePdfDoneFiles] = "변환 완료 · PDF {0}개 · {1}",
            [Txt.ImagePdfDonePartial] = "변환 완료 · 성공 {0}개, 실패 {1}개 · {2}",
            [Txt.ImagePdfCancelled] = "취소됨 · 이미지 {0}장 처리",
            [Txt.ImagePdfFailStatus] = "변환 실패. 이미지를 확인한 후 다시 시도하세요",
            [Txt.ImagePdfFailTitle] = "변환 실패",
            [Txt.ImagePdfNoFiles] = "먼저 변환할 이미지를 추가하세요.",
            [Txt.ImagePdfNeedFolder] = "저장 위치를 선택하세요.",
            [Txt.ImagePdfUnreadable] = "이미지「{0}」을 읽을 수 없습니다: 파일이 손상되었거나 지원되지 않는 형식일 수 있습니다.",
            [Txt.ImagePdfWriteError] = "PDF를 쓸 수 없습니다:\n{0}\n\n기술 정보: {1}",
            [Txt.ImagePdfFailGeneric] = "PDF를 생성할 수 없습니다: {0}",
            [Txt.ImagePdfAdded] = "이미지 {0}장을 추가했습니다",
            [Txt.ImagePdfSkippedNonImage] = "이미지가 아닌 파일 {0}개를 무시했습니다",
            [Txt.ImagePdfNothingAdded] = "추가된 파일이 없습니다: 중복, 존재하지 않거나 이미지가 아닌 파일",
            [Txt.MsgImagePdfBusy] = "이미지를 변환 중입니다. 완료된 후 창을 닫아 주세요.",
            [Txt.TipNavImagePdf] = "이미지 → PDF: 기본은 개별 출력, 필요 시 하나의 PDF로 병합",
            [Txt.Feature5] = "PDF 페이지를 PNG / JPEG / BMP / TIFF로 내보내기. 기본값 JPEG·300PPI·품질 100%",
            [Txt.Feature6] = "이미지를 일괄 PDF로 변환: 기본은 개별 출력, 하나로 병합 가능, 파일 이름 사용자 지정",
            [Txt.Changelog452] = "4.5.2 · 2026-11-05",
            [Txt.ChangelogItems452] =
                "#추가\n" +
                "· 이미지 → PDF가 기본으로 이미지마다 별도의 PDF를 출력\n" +
                "· 「하나의 PDF로 병합」 스위치를 추가해 기존 병합 출력으로 전환 가능\n" +
                "· 출력 파일 이름을 지정할 수 있으며 개별 출력 시 _001, _002 번호가 붙음\n" +
                "#개선\n" +
                "· 개별 출력 시 한 장이 실패해도 나머지를 계속 처리하고 성공/실패 수를 요약\n" +
                "#수정\n" +
                "· PDF → 이미지는 원본 PDF를 읽기 전용으로만 사용하며 텍스트·이미지·페이지를 변경하지 않음\n",
            [Txt.NavWordPdfTitle] = "Word → PDF",
            [Txt.NavWordPdfSubtitle] = "Word 일괄 PDF 변환",
            [Txt.WordPdfPageTitle] = "Word → PDF",
            [Txt.WordPdfPageSubtitle] = "Word를 개별 또는 하나의 PDF로 변환 · 로컬 처리",
            [Txt.WordPdfFilesTitle] = "Word 문서",
            [Txt.WordPdfCountFormat] = "{0}개 문서",
            [Txt.WordPdfListHint] = "빈 영역을 드래그하여 선택 · 목록 순서대로 변환",
            [Txt.WordPdfAdd] = "Word 추가",
            [Txt.WordPdfDialogTitle] = "변환할 Word 문서 선택",
            [Txt.WordPdfEmptyTitle] = "Word를 여기에 끌어다 놓으세요",
            [Txt.WordPdfEmptySubtitle] = "또는 「Word 추가」를 클릭하여 여러 개 선택",
            [Txt.WordPdfEmptyFormats] = ".doc .docx .docm .dot .dotx .dotm .rtf .odt",
            [Txt.WordPdfSettingsTitle] = "변환 설정",
            [Txt.WordPdfMergeOption] = "하나의 파일로 병합",
            [Txt.WordPdfMergeHint] = "끄면 각 Word가 같은 이름의 PDF로 개별 출력됩니다",
            [Txt.WordPdfNameLabel] = "파일 이름",
            [Txt.WordPdfNamePlaceholder] = "비워 두면 첫 Word 파일 이름 사용",
            [Txt.WordPdfSeparateHint] = "PDF 이름은 Word와 같으며 중복 시 번호가 붙습니다",
            [Txt.WordPdfOutput] = "저장 위치",
            [Txt.WordPdfNamePreview] = "출력: {0}",
            [Txt.WordPdfNamePreviewMulti] = "Word와 같은 이름의 PDF {0}개",
            [Txt.WordPdfStart] = "변환 시작",
            [Txt.WordPdfRunning] = "변환 중…",
            [Txt.WordPdfCancel] = "취소",
            [Txt.WordPdfCancelling] = "취소 중…",
            [Txt.WordPdfReady] = "준비됨 · 각 Word를 개별 PDF로 출력",
            [Txt.WordPdfReadyMerge] = "준비됨 · 목록 순서대로 하나의 PDF로 병합",
            [Txt.WordPdfEngineWord] = "Microsoft Word",
            [Txt.WordPdfEngineLibre] = "LibreOffice",
            [Txt.WordPdfProcessing] = "변환 중 ({0}/{1}): {2}",
            [Txt.WordPdfMerging] = "PDF 페이지 병합 중…",
            [Txt.WordPdfDone] = "변환 완료 · {0}페이지 · {1}",
            [Txt.WordPdfDoneFiles] = "변환 완료 · PDF {0}개 · {1}",
            [Txt.WordPdfCancelled] = "취소됨 · {0}개 완료",
            [Txt.WordPdfFailStatus] = "변환에 실패했습니다. 문서를 확인 후 다시 시도하세요",
            [Txt.WordPdfFailTitle] = "변환 실패",
            [Txt.WordPdfNoFiles] = "먼저 변환할 Word 문서를 추가하세요.",
            [Txt.WordPdfNeedFolder] = "저장 위치를 선택하세요.",
            [Txt.WordPdfNoEngine] = "Microsoft Word 또는 LibreOffice가 없어 Word를 변환할 수 없습니다.",
            [Txt.WordPdfNonWord] = "지원하지 않는 Word 문서입니다: {0}",
            [Txt.WordPdfWriteError] = "PDF를 쓸 수 없습니다: {0}",
            [Txt.WordPdfFailGeneric] = "Word 변환 실패: {0}",
            [Txt.WordPdfAdded] = "문서 {0}개 추가됨",
            [Txt.WordPdfSkippedNonWord] = "Word가 아닌 파일 {0}개 무시됨",
            [Txt.WordPdfNothingAdded] = "추가된 파일이 없습니다: 중복, 없음 또는 Word 아님",
            [Txt.WordPdfOutputCount] = "PDF {0}개",
            [Txt.MsgWordPdfBusy] = "Word 변환 중입니다. 완료될 때까지 창을 닫지 마세요.",
            [Txt.TipNavWordPdf] = "Word → PDF: 개별 출력 또는 하나의 파일로 병합",
            [Txt.Feature7] = "Word 일괄 PDF 변환: 기본적으로 원본 파일 이름으로 개별 출력, 하나의 파일 병합과 출력 워터마크 지원",
            [Txt.Changelog4520] = "4.5.20 · 2026-11-10",
            [Txt.ChangelogItems4520] =
                "#개선\n" +
                "· 오늘의 한마디가 시작 시와 실행 중에 무작위로 표시되며, 순서대로 넘어가지 않고 연속으로 같은 문장이 나오지 않습니다\n" +
                "· 자동 전환 간격이 초 단위로 바뀌어 기본 30초이며, 설정에서 5~86400초로 지정할 수 있습니다\n",
            [Txt.Changelog4519] = "4.5.19 · 2026-11-10",
            [Txt.ChangelogItems4519] =
                "#개선\n" +
                "· 앱의 모든 대화 상자가 현재 스킨을 따릅니다. 확인, 경고, 오류, 질문 창이 스킨의 색상, 글꼴, 모서리, 버튼 스타일을 사용합니다\n" +
                "· 비밀번호 입력 창도 같은 테두리 없는 스킨 창으로 바뀌어 어두운 스킨에서 흰색 시스템 제목 표시줄이 나타나지 않습니다\n",
            [Txt.Changelog4518] = "4.5.18 · 2026-11-10",
            [Txt.ChangelogItems4518] =
                "#추가\n" +
                "· 왼쪽에 「Word → PDF」 작업 공간 추가. .doc/.docx/.docm/.dot/.dotx/.dotm/.rtf/.odt 일괄 변환 지원\n" +
                "· 기본적으로 원본 파일 이름으로 PDF를 개별 출력하고 중복 시 번호 부여. 「하나의 파일로 병합」 선택 가능\n" +
                "· 워터마크 설정에 「Word → PDF」 대상 추가. 출력 PDF에만 적용되며 원본 문서는 변경되지 않음\n" +
                "· Microsoft Word 또는 LibreOffice를 자동으로 사용하며 둘 다 없으면 명확히 안내\n",
            [Txt.Changelog4517] = "4.5.17 · 2026-11-06",
            [Txt.ChangelogItems4517] =
                "#개선\n" +
                "· 명언 라이브러리에서 고전 시가와 한문을 삭제하고 모두 현대 명언으로 교체\n" +
                "#추가\n" +
                "· 국내외 명작 영화 대사와 유명인 명언 추가. UI 언어별 66개, 6개 언어 합계 396개\n" +
                "· 《포레스트 검프》《쇼생크 탈출》《대부》《타이타닉》《나타지마동강세》《일대종사》 등의 영화 대사 수록\n" +
                "· 스티브 잡스, 넬슨 만델라, 간디, 마틴 루터 킹, 베이컨, 체호프 등의 명언 추가\n",
            [Txt.Changelog4516] = "4.5.16 · 2026-11-06",
            [Txt.ChangelogItems4516] =
                "#개선\n" +
                "· '오늘의 한마디' 카드를 반투명 유리(프로스트 글라스) 스타일로 개편: 반투명 배경, 상단 하이라이트, 둥근 테두리로 유리 테마와 통일\n" +
                "· 카드 왼쪽의 빨간 세로선을 제거하여 야간 테마에서도 붉은 장식이 나타나지 않도록 수정\n" +
                "· 긴 문장은 글자 크기를 자동으로 줄여 잘리지 않고 전체가 표시\n" +
                "#추가\n" +
                "· 명언 라이브러리를 348개로 확장(UI 언어별 58개), 세계 명사와 명작 영화 대사를 수록\n",
            [Txt.Changelog4515] = "4.5.15 · 2026-11-06",
            [Txt.ChangelogItems4515] =
                "#추가\n" +
                "· \"오늘의 한마디\"에 자동 전환을 추가하여 모양 설정에서 전환 간격(기본 5분)을 지정할 수 있습니다\n" +
                "· 자동 전환을 끄더라도 카드를 클릭하면 즉시 다음 문장으로 바뀝니다\n",
            [Txt.Changelog4514] = "4.5.14 · 2026-11-06",
            [Txt.ChangelogItems4514] =
                "#수정\n" +
                "· UI 언어를 전환한 뒤 설정 창이 빨간 X로 가득 차거나 처리되지 않은 예외가 발생하던 문제를 수정했습니다\n" +
                "· 유리 배경 캐시가 실수로 해제되면 자동으로 다시 생성되어 같은 문제가 재발하지 않습니다\n",
            [Txt.Changelog4513] = "4.5.13 · 2026-11-06",
            [Txt.ChangelogItems4513] =
                "#추가\n" +
                "· 병합 페이지 오른쪽에 \"오늘의 한마디\" 카드를 추가하여 세계 명언과 영화 대사를 출처와 함께 표시합니다\n" +
                "· 간체 중국어, 번체 중국어, English, 日本語, 한국어, Русский 6개 언어 문장을 내장하고 UI 언어 전환에 맞춰 교체합니다\n" +
                "· 모양 설정에 \"병합 페이지에 명언이나 영화 대사 표시\" 스위치를 추가했습니다. 카드를 클릭하면 다음 문장으로 바뀝니다\n",
            [Txt.Changelog4512] = "4.5.12 · 2026-11-06",
            [Txt.ChangelogItems4512] =
                "#변경\n" +
                "· 워터마크 설정과 완료 알림 항목을 왼쪽 아래로 옮겨 스킨 / 설정 / 정보와 함께 배치했습니다\n" +
                "· 병합 페이지 오른쪽 안내 문구를 진행 표시줄 바로 아래로 올렸습니다\n" +
                "#수정\n" +
                "· 워터마크 항목 안내에서 오래된 \"문서 병합에만 적용\" 문구를 삭제했습니다\n",
            [Txt.Changelog4511] = "4.5.11 · 2026-11-06",
            [Txt.ChangelogItems4511] =
                "#변경\n" +
                "· 터미널 스킨을 제거했습니다. 사용 중이던 설정은 자동으로 클래식 블루로 돌아갑니다\n" +
                "#수정\n" +
                "· 오른쪽 위 워터마크 버튼 아이콘이 19px에서 잘려 보이던 문제를 수정했습니다\n",
            [Txt.Changelog4510] = "4.5.10 · 2026-11-06",
            [Txt.ChangelogItems4510] =
                "#수정\n" +
                "· 일반 왼쪽 클릭(Ctrl+클릭 포함)은 현재 파일만 선택되며 이전 행의 선택은 즉시 사라집니다\n" +
                "· 범위 선택과 Shift+클릭만 다중 선택을 유지합니다\n",
            [Txt.Changelog459] = "4.5.9 · 2026-11-06",
            [Txt.ChangelogItems459] =
                "#수정\n" +
                "· 일반 왼쪽 클릭 시 클릭한 파일만 선택되고 이전 행의 선택이 남지 않습니다. 행 안의 위로 / 아래로 / 삭제 버튼도 동일합니다\n" +
                "· Ctrl / Shift 다중 선택은 그대로 유지됩니다\n",
            [Txt.Changelog458] = "4.5.8 · 2026-11-06",
            [Txt.ChangelogItems458] =
                "#추가\n" +
                "· 스킨 입구를 왼쪽 아래 설정 위로 옮기고 스킨 / 설정 / 정보를 플랫 라인 아이콘으로 통일\n" +
                "· 파일 목록에 「선택 취소」 버튼을 추가. 박스 선택 전 상태로 되돌리며 Esc도 동작합니다\n" +
                "· 삭제, 모두 지우기, 선택 취소 등 목록 버튼에 마우스 오버 팁 추가\n" +
                "#개선\n" +
                "· 마우스 왼쪽 버튼으로 범위 선택할 때 목록이 튀는 문제를 수정\n" +
                "#변경\n" +
                "· HUD SF 스킨을 제거했으며 저장된 설정은 클래식 블루로 자동 전환\n",
            [Txt.Changelog457] = "4.5.7 · 2026-11-06",
            [Txt.ChangelogItems457] =
                "#추가\n" +
                "· 파일 목록에서 빈 곳을 드래그해 범위 선택할 수 있으며 세 작업 공간 모두 지원\n" +
                "· Ctrl을 누른 채 범위 선택하면 기존 선택에 추가\n" +
                "· 새로운 스킨 4종: 스큐어모피즘, 뉴모피즘, 터미널, HUD SF\n" +
                "#개선\n" +
                "· 범위 선택한 파일은 Delete로 한 번에 삭제\n" +
                "· NetEase Red 스킨을 제거했으며 저장된 설정은 클래식 블루로 자동 전환\n",
            [Txt.Changelog456] = "4.5.6 · 2026-11-06",
            [Txt.ChangelogItems456] =
                "#개선\n" +
                "· PDF → 이미지의 기본 출력 하위 폴더에 현재 페이지 선택(예: \"보고서_짝수\", \"보고서_전체\")이 붙어 여러 번 변환한 결과를 구분하기 쉬워졌습니다\n",
            [Txt.Changelog455] = "4.5.5 · 2026-11-06",
            [Txt.ChangelogItems455] =
                "#추가\n" +
                "· PDF → 이미지에서 전체, 홀수 페이지, 짝수 페이지 또는 직접 지정한 범위로 내보낼 수 있습니다\n" +
                "· 직접 범위는 1-3,5,8-10 형식을 지원하며 원본 페이지 번호로 파일 이름을 지정합니다\n" +
                "#개선\n" +
                "· 미리보기와 파일 이름 예시가 현재 페이지 모드의 첫 번째 이미지를 따릅니다\n" +
                "· 이전 버전에 저장된 페이지 범위는 자동으로 「직접」 모드로 이전됩니다\n",
            [Txt.Changelog454] = "4.5.4 · 2026-11-05",
            [Txt.ChangelogItems454] =
                "#새 기능\n" +
                "· 새로운 플랫 브랜드 아이콘: 파란 문서 스택 + 앰버 병합 배지\n" +
                "· 기능 아이콘을 플랫 투톤 스타일로 재작업해 작은 크기에서도 선명하고 일관되게 표시\n" +
                "#수정\n" +
                "· 언어를 전환할 때 설정 창이 제자리에서 새로 고쳐지며, 닫았다 다시 열거나 화면 중앙으로 이동하지 않음\n",
            [Txt.Changelog453] = "4.5.3 · 2026-11-05",
            [Txt.ChangelogItems453] =
                "#추가\n" +
                "· 워터마크를 문서 병합, PDF → 이미지, 이미지 → PDF에 각각 적용할 수 있으며 세 스위치는 기본적으로 꺼짐\n" +
                "· PDF → 이미지는 출력 이미지에 워터마크를 합성할 수 있고 원본 PDF는 읽기 전용으로 유지\n" +
                "#개선\n" +
                "· PDF → 이미지 기본값을 300PPI, JPEG, 품질 100%로 변경\n" +
                "· 완료음은 기본적으로 꺼져 있으며, 꺼져 있는 동안 효과음 선택이 비활성화됨\n",
            [Txt.Changelog451] = "4.5.1 · 2026-11-05",
            [Txt.ChangelogItems451] =
                "#추가\n" +
                "· 「이미지 → PDF」 작업 공간 추가: 목록 순서대로 이미지를 하나의 PDF로\n" +
                "· 페이지 크기 A4 / A3 / A5 / B5 / Letter 또는 이미지에 맞춤, 방향과 여백 지원\n" +
                "· 이름·저장 위치·「완료 후 열기」는 문서 병합 설정을 공유\n" +
                "#개선\n" +
                "· Qoder 다크 테마 제거\n" +
                "#수정\n" +
                "· 클래식 블루, NetEase 레드 테마에서 왼쪽 탐색 글자 대비가 낮은 문제 수정\n",
            [Txt.Changelog440] = "4.4.0 · 2026-11-05",
            [Txt.ChangelogItems440] =
                "#추가\n" +
                "· 왼쪽 카드형 탐색: 문서 병합과 PDF → 이미지 두 작업 공간, Ctrl+1 / Ctrl+2로 전환\n" +
                "· PDF → 이미지 추가: PDF 페이지를 PNG / JPEG / BMP / TIFF로 내보내고 형식·PPI·품질·페이지 범위 설정 가능\n" +
                "· 일괄 변환, 실시간 미리보기, 취소, 파일별 실패 요약 지원. 이미지는 선택한 DPI 정보를 유지\n" +
                "· Qoder 다크 테마 추가, 새 설치 시 기본 적용\n" +
                "#개선\n" +
                "· Qoder 스타일로 UI 재구성: 카드형 탐색, 페이지 제목, 단일 강조색과 낮은 대비의 테두리\n" +
                "· 병합과 변환이 이름·저장 위치·완료 알림 설정을 공유하고, 변환은 별도의 「마지막 위치」 사용\n" +
                "· 출력 이름과 저장 폴더를 직접 편집 가능. 폴더 끌어오기는 하위 폴더까지 검색\n" +
                "· 설정에 「PDF → 이미지」 페이지 추가, 워터마크 페이지에 문서 병합에만 적용된다고 명시\n",

            [Txt.Changelog450] = "4.5.0 · 2026-11-05",
            [Txt.ChangelogItems450] =
                "#추가\n" +
                "· 「文枢 DocHub」로 이름 변경: 문서 허브, 병합과 변환을 한 번에\n" +
                "· 정보 페이지에 브랜드 정의와 PDF → 이미지 기능 설명 추가\n" +
                "#개선\n" +
                "· 왼쪽 탐색의 「문서 변환」을 「PDF → 이미지」로 변경하고 페이지 제목·설정과 통일\n" +
                "#수정\n" +
                "· 변환 페이지에서 「PDF 추가」 버튼이 안내 문구에 가려지는 문제 수정\n" +
                "· 파일 개수 문구가 삭제·비우기 버튼을 덮는 문제 수정\n" +
                "· 업그레이드 시 기존 설정을 자동으로 이전\n",

            [Txt.DefaultDocName] = "병합 문서",
            [Txt.DefaultConvertName] = "변환 이미지",
            [Txt.MergeTimePrefix] = "병합_",
            [Txt.ConvertTimePrefix] = "변환_",
            [Txt.MergeCollideSuffix] = "_병합"
        };
    }
}
