using System.Reflection;

// 启动器的版本与厂商信息：写入 exe 的属性页。
// 安全软件对「无版本信息的极小启动器」更容易拦截，补齐元数据可减少误判。
// 注意 .NET 的映射关系：资源管理器「说明/FileDescription」取自 AssemblyTitle，
// 「备注/Comments」取自 AssemblyDescription。
[assembly: AssemblyTitle("文枢 DocHub 启动器")]
[assembly: AssemblyDescription("文枢 DocHub 启动器")]
[assembly: AssemblyCompany("AI")]
[assembly: AssemblyProduct("文枢")]
[assembly: AssemblyCopyright("© 2026 AI")]
[assembly: AssemblyFileVersion("4.5.21")]
[assembly: AssemblyInformationalVersion("4.5.21")]
[assembly: AssemblyVersion("4.5.21")]
