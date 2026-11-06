using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DocHubSetup
{
    internal static class Program
    {
        internal const string AppName = "文枢";
        internal const string AppPublisher = "AI";
        internal const string AppVersion = "4.5.6";
        internal const string UninstallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\DocHub";

        /// <summary>旧版（合了么 / HeLeMe）的卸载登记项，安装时顺带清理。</summary>
        internal const string LegacyUninstallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\HeLeMe";

        /// <summary>安装根目录下的精简启动器（与主程序同名，位于不同目录）。</summary>
        internal const string LauncherName = "DocHub.exe";

        /// <summary>主程序位于安装根目录的 app 子目录。</summary>
        internal const string AppSubDir = "app";

        internal const string AppExeName = "DocHub.exe";

        /// <summary>内嵌载荷资源名后缀（由 tools\package.ps1 生成 installer\payload.zip 后嵌入）。</summary>
        internal const string PayloadSuffix = "payload.zip";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteFileW(string fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AttachConsole(int processId);

        [STAThread]
        private static int Main(string[] args)
        {
            // 安装程序本身从网上下载后会带「来自网络」标记，启动后立刻清掉自身标记，
            // 之后再次运行不会再被系统（附件管理器）拦截。
            SelfUnblock();

            Options options;
            try
            {
                options = Options.Parse(args);
            }
            catch (Exception ex)
            {
                WriteConsole("参数错误：" + ex.Message);
                return ExitCodes.InvalidArguments;
            }

            // 无界面模式：仅解压载荷（供自动化测试与离线部署），不写注册表、不建快捷方式
            if (options.ExtractOnly)
                return RunExtractOnly(options.TargetDir);

            // 无界面模式：完整静默安装（当前用户级）
            if (options.Silent)
                return RunSilent(options);

            // 主题清单需要在临时目录落一个激活上下文文件；个别环境（%TEMP% 不可写、
            // 受管控的临时目录）会在这里抛异常。失败时继续用系统默认样式显示界面，
            // 不能让整个安装程序因此无提示退出。
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch (Exception ex)
            {
                WriteConsole("界面初始化降级：" + ex.Message);
            }

            try
            {
                Application.Run(new SetupForm(options.TargetDir));
                return ExitCodes.Success;
            }
            catch (Exception ex)
            {
                // 窗体都起不来时至少留一句可读的提示，而不是静默退出
                try
                {
                    MessageBox.Show("安装程序无法启动：" + ex.Message + Environment.NewLine + Environment.NewLine +
                                    "可改用绿色版压缩包，解压即用，不写系统目录、不写注册表。",
                        AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    // 连提示框都不可用时不作处理
                }

                WriteConsole("界面模式失败：" + ex.Message);
                return ExitCodes.Failure;
            }
        }

        /// <summary>进程退出码约定：0 成功，1 失败，2 参数错误，3 部分文件未写入。</summary>
        internal static class ExitCodes
        {
            internal const int Success = 0;
            internal const int Failure = 1;
            internal const int InvalidArguments = 2;
            internal const int PartialFailure = 3;
        }

        // ---------------------------------------------------------------- 无界面模式

        private static int RunExtractOnly(string? targetDir)
        {
            AttachParentConsole();

            if (string.IsNullOrWhiteSpace(targetDir))
            {
                WriteConsole("用法：--extract <目标目录>（把安装内容解压到指定目录，不写注册表）");
                return ExitCodes.InvalidArguments;
            }

            try
            {
                // 与完整安装保持同一套规则：用户指定的目录原样使用，只在路径无法识别时退回默认目录。
                string directory = InstallerEngine.SanitizeInstallDir(targetDir, out bool rewritten);
                if (rewritten)
                    WriteConsole("路径无法识别，已改用默认目录：" + directory);

                Directory.CreateDirectory(directory);

                int written;
                int failed;
                InstallerEngine.ExtractPayload(directory, out written, out failed, WriteConsole);
                InstallerEngine.ClearZoneIdentifiers(directory, out int scanned);

                WriteConsole("解压目录：" + directory);
                WriteConsole("已解压文件数：" + written);
                WriteConsole("写入失败数：" + failed);
                WriteConsole("已检查解除锁定标记的文件数：" + scanned);

                return failed == 0 ? ExitCodes.Success : ExitCodes.PartialFailure;
            }
            catch (Exception ex)
            {
                WriteConsole("解压失败：" + ex.Message);
                return ExitCodes.Failure;
            }
        }

        private static int RunSilent(Options options)
        {
            AttachParentConsole();

            bool userChosen = !string.IsNullOrWhiteSpace(options.TargetDir);
            string requested = userChosen ? options.TargetDir! : InstallerEngine.DefaultInstallDir();

            string installDir = InstallerEngine.SanitizeInstallDir(requested, out bool rewritten);
            if (rewritten)
                WriteConsole("路径无法识别，已改用默认目录：" + installDir);

            try
            {
                InstallOutcome outcome = InstallerEngine.Install(
                    installDir,
                    options.DesktopShortcut,
                    message => WriteConsole(message),
                    !userChosen);

                WriteConsole("安装目录：" + outcome.InstallDir);
                WriteConsole("已写入文件数：" + outcome.FilesWritten);
                foreach (string note in outcome.Notes)
                    WriteConsole("提示：" + note);

                if (outcome.Failures.Count > 0)
                {
                    WriteConsole("以下文件未能写入：");
                    foreach (string failure in outcome.Failures)
                        WriteConsole("  " + failure);
                    return ExitCodes.PartialFailure;
                }

                WriteConsole("安装完成。");
                return ExitCodes.Success;
            }
            catch (Exception ex)
            {
                WriteConsole("安装失败：" + ex.Message);
                return ExitCodes.Failure;
            }
        }

        /// <summary>清掉安装程序自身的 Zone.Identifier（相当于对本 exe「解除锁定」）。</summary>
        private static void SelfUnblock()
        {
            try
            {
                string? path = Environment.ProcessPath;
                if (string.IsNullOrEmpty(path)) return;
                DeleteFileW(path + ":Zone.Identifier");
            }
            catch
            {
                // 没有该数据流或权限不足时保持原样
            }
        }

        /// <summary>WinExe 默认没有控制台，附加到父进程控制台后即可把静默模式的信息打印出来。</summary>
        private static void AttachParentConsole()
        {
            const int AttachParentProcess = -1;
            try
            {
                if (AttachConsole(AttachParentProcess))
                    Console.Out.Flush();
            }
            catch
            {
                // 没有父控制台（例如资源管理器双击）时保持无输出
            }
        }

        private static void WriteConsole(string message)
        {
            try
            {
                Console.Out.WriteLine(message);
                Console.Out.Flush();
            }
            catch
            {
                // 无控制台可用时不再输出
            }
        }
    }

    /// <summary>命令行参数：兼容 NSIS 风格的 /S 与 /D=，并提供 --extract / --silent / --dir。</summary>
    internal sealed class Options
    {
        internal bool ExtractOnly { get; private set; }
        internal bool Silent { get; private set; }
        internal string? TargetDir { get; private set; }
        internal bool DesktopShortcut { get; private set; } = true;

        internal static Options Parse(string[] args)
        {
            var options = new Options();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i] ?? string.Empty;
                if (arg.Length == 0) continue;

                if (Matches(arg, "--extract") || Matches(arg, "-extract"))
                {
                    options.ExtractOnly = true;
                    string? value = ReadValue(arg, args, ref i);
                    if (value != null) options.TargetDir = value;
                    continue;
                }

                if (Matches(arg, "--silent") || Matches(arg, "/S") || Matches(arg, "-silent"))
                {
                    options.Silent = true;
                    continue;
                }

                if (Matches(arg, "--dir") || Matches(arg, "-dir"))
                {
                    options.TargetDir = ReadValue(arg, args, ref i);
                    continue;
                }

                if (Matches(arg, "--no-desktop"))
                {
                    options.DesktopShortcut = false;
                    continue;
                }

                // NSIS 兼容：/D=D:\Apps\DocHub（不带引号，且必须是最后一个参数）
                if (arg.StartsWith("/D=", StringComparison.OrdinalIgnoreCase) && arg.Length > 3)
                {
                    options.TargetDir = arg.Substring(3);
                    continue;
                }
            }

            return options;
        }

        private static bool Matches(string arg, string expected)
        {
            return string.Equals(arg, expected, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取 "--dir 路径" 或 "--dir=路径" 两种写法。
        /// 只有明确的开关 token（--xxx / -xxx / NSIS 的 /S、/D=）才算「没有值」，
        /// 因此 `--dir /tmp/x` 这类以斜杠开头的 POSIX 路径不会被静默丢弃。
        /// </summary>
        private static string? ReadValue(string arg, string[] args, ref int index)
        {
            int separator = arg.IndexOf('=');
            if (separator > 0 && separator < arg.Length - 1)
                return arg.Substring(separator + 1).Trim('"');

            if (index + 1 < args.Length)
            {
                string next = args[index + 1];
                if (!IsSwitchToken(next))
                {
                    index++;
                    return next.Trim('"');
                }
            }

            return null;
        }

        private static bool IsSwitchToken(string token)
        {
            if (token.Length == 0) return false;
            if (token.StartsWith("--", StringComparison.Ordinal)) return true;
            if (token.StartsWith("-", StringComparison.Ordinal)) return true;
            if (string.Equals(token, "/S", StringComparison.OrdinalIgnoreCase)) return true;
            if (token.StartsWith("/D=", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }

    /// <summary>一次安装的结果：失败清单与提示信息一次性交给界面，避免逐文件弹框。</summary>
    internal sealed class InstallOutcome
    {
        internal InstallOutcome(string installDir)
        {
            InstallDir = installDir;
        }

        internal string InstallDir { get; }

        internal List<string> Failures { get; } = new List<string>();

        internal List<string> Notes { get; } = new List<string>();

        internal int FilesWritten { get; set; }

        internal bool Succeeded => Failures.Count == 0;
    }

    /// <summary>
    /// 安装内核：安装到当前用户目录，全程不需要管理员权限，写入失败只记录清单而不弹框。
    /// </summary>
    internal static class InstallerEngine
    {
        private const int MaxWriteAttempts = 3;
        private const int WriteAttemptDelayMs = 200;
        private const int KillRounds = 3;
        private const int KillRoundDelayMs = 300;
        private const int KillWaitMs = 3000;

        /// <summary>结果页最多逐条列出的失败文件数，其余用汇总行说明。</summary>
        private const int MaxReportedFailures = 10;

        /// <summary>默认安装目录：%LOCALAPPDATA%\Programs\DocHub。任何情况下都不回退到 Program Files。</summary>
        internal static string DefaultInstallDir()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(localAppData))
            {
                string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                localAppData = Path.Combine(profile, "AppData", "Local");
            }

            return Path.Combine(localAppData, "Programs", "DocHub");
        }

        /// <summary>
        /// 规范化安装目录：空 → 默认目录；无法解析的路径 → 默认目录。
        /// 用户明确指定的位置（包括 Program Files）一律保留，不再自动改写：
        /// 早期版本会把 Program Files 悄悄改回默认目录，导致「改了目录却没生效」。
        /// </summary>
        internal static string SanitizeInstallDir(string? rawPath, out bool rewritten)
        {
            rewritten = false;

            if (string.IsNullOrWhiteSpace(rawPath))
                return DefaultInstallDir();

            string candidate;
            try
            {
                candidate = Path.GetFullPath(rawPath.Trim().Trim('"'));
            }
            catch
            {
                rewritten = true;
                return DefaultInstallDir();
            }

            string trimmed = candidate.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (trimmed.Length == 0) return candidate;

            // 盘符根目录（D:\）去掉分隔符会变成相对盘符（D:），必须保留分隔符
            if (trimmed.EndsWith(":", StringComparison.Ordinal))
                trimmed += Path.DirectorySeparatorChar;

            return trimmed;
        }

        /// <summary>判断路径是否落在系统程序目录（不区分大小写，含 ProgramFiles / ProgramFilesX86 / ProgramW6432）。</summary>
        internal static bool IsUnderProgramFiles(string path)
        {
            string[] roots =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetEnvironmentVariable("ProgramW6432") ?? string.Empty
            };

            string normalizedPath = NormalizeForCompare(path);
            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                string normalizedRoot = NormalizeForCompare(root);
                if (normalizedRoot.Length == 0) continue;

                if (normalizedPath.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase) ||
                    normalizedPath.StartsWith(normalizedRoot + "\\", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 有的机器把 Program Files 整个搬到别的盘（如 D:\Program Files (x86)），
            // 环境变量仍指向 C:，只按环境变量判断会漏掉，这里再按目录名兜底。
            foreach (string segment in normalizedPath.Split('\\'))
            {
                if (segment.Equals("Program Files", StringComparison.OrdinalIgnoreCase) ||
                    segment.Equals("Program Files (x86)", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static string NormalizeForCompare(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.TrimEnd('\\', '/').Replace('/', '\\');
        }

        /// <summary>
        /// 准备安装目录：先重试首选目录，仍不可写时依次换到其它位置，返回实际可用的那个。
        /// 除了用户目录，还会尝试公用目录、安装包所在目录和其它固定磁盘的根目录：
        /// 个别机器整片用户目录都被安全软件保护，只有其它盘符可写。
        /// </summary>
        internal static string PrepareInstallDirectory(string preferred, Action<string>? report, out bool relocated,
            bool allowRelocate = true)
        {
            // 用户自己填/选的目录：只尝试它本身。以前无论用户填什么都可能被悄悄换到别的目录，
            // 表现就是「改了安装目录却不生效」，这里直接报错让用户决定下一步。
            if (!allowRelocate)
            {
                Exception? error = TryPrepareDirectory(preferred);
                if (error == null)
                {
                    relocated = false;
                    return preferred;
                }

                string detailLog = WriteSetupLog(preferred + " → " + error.GetType().Name + ": " + error.Message);
                throw new IOException(
                    "无法写入指定的安装目录：" + preferred + Environment.NewLine + Environment.NewLine +
                    error.GetType().Name + ": " + error.Message + Environment.NewLine + Environment.NewLine +
                    (IsUnderProgramFiles(preferred)
                        ? "该位置在 Program Files 下，普通用户通常没有写入权限。请改到其它目录（例如 D:\\Apps\\DocHub），或以管理员身份运行安装程序。"
                        : "请换一个有写入权限的目录，或暂时关闭安全软件的「文件实时防护」后重试。") +
                    (detailLog.Length == 0 ? string.Empty : Environment.NewLine + Environment.NewLine + "详细日志：" + detailLog));
            }

            var candidates = new List<string> { preferred };

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string publicDir = Environment.GetEnvironmentVariable("PUBLIC") ?? string.Empty;

            AddCandidate(candidates, Path.Combine(localAppData, "DocHub"));
            AddCandidate(candidates, Path.Combine(localAppData, "Programs", "DocHub"));
            AddCandidate(candidates, Path.Combine(profile, "DocHub"));
            AddCandidate(candidates, Path.Combine(roamingAppData, "DocHub"));
            AddCandidate(candidates, Path.Combine(publicDir, "DocHub"));
            AddCandidate(candidates, Path.Combine(AppContext.BaseDirectory, "DocHub"));

            foreach (DriveInfo drive in SafeGetDrives())
            {
                try
                {
                    if (drive.DriveType != DriveType.Fixed || !drive.IsReady) continue;
                    if (drive.AvailableFreeSpace < 200L * 1024 * 1024) continue;
                    AddCandidate(candidates, Path.Combine(drive.RootDirectory.FullName, "DocHub"));
                }
                catch
                {
                    // 个别磁盘读取元数据就会抛异常，忽略即可
                }
            }

            var failures = new List<string>();
            foreach (string candidate in candidates)
            {
                Exception? lastError = TryPrepareDirectory(candidate);
                if (lastError == null)
                {
                    relocated = !string.Equals(candidate, preferred, StringComparison.OrdinalIgnoreCase);
                    if (relocated) report?.Invoke("原目录无法写入，改用：" + candidate);
                    return candidate;
                }

                failures.Add(candidate + " → " + lastError.GetType().Name + ": " + lastError.Message);
            }

            relocated = false;

            string detail = failures.Count == 0
                ? string.Join("、", candidates)
                : string.Join(Environment.NewLine, failures);

            // 同时写一份日志到安装包旁边（不可写时退到 TEMP），方便排查
            string logPath = WriteSetupLog(detail);

            throw new IOException(
                "无法创建安装目录，已尝试：" + string.Join("、", candidates) +
                Environment.NewLine + Environment.NewLine + detail +
                (logPath.Length == 0 ? string.Empty : Environment.NewLine + Environment.NewLine + "详细日志：" + logPath));
        }

        /// <summary>创建目录并真实写入一个探测文件；成功返回 null，失败返回最后一次异常。</summary>
        private static Exception? TryPrepareDirectory(string directory)
        {
            Exception? lastError = null;

            for (int attempt = 0; attempt < MaxWriteAttempts; attempt++)
            {
                try
                {
                    Directory.CreateDirectory(directory);

                    // 真正写一个文件再做判断：某些安全软件允许建目录但拒绝写入文件
                    string probe = Path.Combine(directory, ".dochub-write-test");
                    File.WriteAllText(probe, "test");
                    File.Delete(probe);
                    return null;
                }
                catch (Exception ex)
                {
                    // 可能只是被短暂占用，稍等再试；多次失败后由调用方决定换目录还是报错
                    lastError = ex;
                    Thread.Sleep(WriteAttemptDelayMs);
                }
            }

            return lastError;
        }

        private static IEnumerable<DriveInfo> SafeGetDrives()
        {
            try
            {
                return DriveInfo.GetDrives();
            }
            catch
            {
                return Array.Empty<DriveInfo>();
            }
        }

        private static bool IsElevatedProcess()
        {
            try
            {
                using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                return new System.Security.Principal.WindowsPrincipal(identity)
                    .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static string WriteSetupLog(string detail)
        {
            string content = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + Environment.NewLine +
                             "version: " + Program.AppVersion + Environment.NewLine +
                             "user: " + Environment.UserDomainName + "\\" + Environment.UserName + Environment.NewLine +
                             "elevated: " + IsElevatedProcess() + Environment.NewLine +
                             "cwd: " + Environment.CurrentDirectory + Environment.NewLine +
                             detail + Environment.NewLine;

            string[] targets =
            {
                Path.Combine(AppContext.BaseDirectory, "setup-error.log"),
                Path.Combine(Path.GetTempPath(), "dochub-setup-error.log")
            };

            foreach (string target in targets)
            {
                try
                {
                    File.AppendAllText(target, content);
                    return target;
                }
                catch
                {
                    // 换下一个位置
                }
            }

            return string.Empty;
        }

        private static void AddCandidate(List<string> candidates, string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate)) return;
            if (IsUnderProgramFiles(candidate)) return;

            foreach (string existing in candidates)
            {
                if (string.Equals(existing, candidate, StringComparison.OrdinalIgnoreCase)) return;
            }

            candidates.Add(candidate);
        }

        // ------------------------------------------------------------ 主流程

        /// <summary>完整安装：结束进程 → 处理旧版本 → 解压 → 解除网络标记 → 快捷方式 → HKCU 卸载信息。</summary>
        internal static InstallOutcome Install(string preferredDir, bool desktopShortcut, Action<string>? report,
            bool allowRelocate = true)
        {
            // 默认目录不可写时（多为安全软件拦截 AppData\Local\Programs）自动换到备用位置；
            // 用户自己指定/选择过的目录不允许被替换，写不进去就如实报错。
            string installDir = PrepareInstallDirectory(preferredDir, report, out bool relocated, allowRelocate);

            var outcome = new InstallOutcome(installDir);
            if (relocated)
                outcome.Notes.Add("原目录无法写入，已改为安装到：" + installDir);

            report?.Invoke("正在关闭运行中的程序…");
            TerminateRunningProcesses();

            // 关闭进程后再处理旧版本：旧卸载器同样需要程序不再占用文件
            HandlePreviousInstallation(installDir, outcome, report);

            report?.Invoke("正在复制程序文件…");
            // 解压失败必须进入 outcome.Failures，否则「只写入了部分文件」会被当成成功
            InstallerEngine.ExtractPayload(installDir, out int written, out _, report, outcome.Failures);
            outcome.FilesWritten = written;

            report?.Invoke("正在解除文件锁定标记…");
            ClearZoneIdentifiers(installDir, out _);

            report?.Invoke("正在创建快捷方式…");
            CreateShortcuts(installDir, desktopShortcut, outcome);

            report?.Invoke("正在登记卸载信息…");
            WriteUninstallRegistry(installDir, outcome);

            return outcome;
        }

        /// <summary>
        /// 结束正在运行的程序：否则覆盖 exe 会失败。多轮尝试以避开进程刚退出、文件句柄尚未释放的瞬间。
        /// 无权结束的实例直接跳过，交给后面的多次写入逻辑记录失败清单。
        /// </summary>
        internal static void TerminateRunningProcesses()
        {
            string[] names = { "DocHub", Program.AppName };

            for (int round = 0; round < KillRounds; round++)
            {
                bool found = false;
                foreach (string name in names)
                {
                    Process[] processes;
                    try
                    {
                        processes = Process.GetProcessesByName(name);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (Process process in processes)
                    {
                        using (process)
                        {
                            try
                            {
                                if (process.HasExited) continue;
                                process.Kill();
                                found = true;
                                process.WaitForExit(KillWaitMs);
                            }
                            catch
                            {
                                // 无权限结束的实例（例如以其他账户运行）：跳过并继续
                            }
                        }
                    }
                }

                if (!found) break;
                Thread.Sleep(KillRoundDelayMs);
            }
        }

        /// <summary>
        /// 旧版本处理：HKCU 记录的其他目录先静默卸载；HKLM 的机器级旧安装只提示，不尝试删除（当前无管理员权限）。
        /// </summary>
        private static void HandlePreviousInstallation(string installDir, InstallOutcome outcome, Action<string>? report)
        {
            try
            {
                foreach (string keyPath in new[] { Program.UninstallKey, Program.LegacyUninstallKey })
                {
                    using RegistryKey? userKey = Registry.CurrentUser.OpenSubKey(keyPath);
                    string? previous = userKey?.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrEmpty(previous) && !SamePath(previous!, installDir))
                    {
                        string? uninstaller = FindUninstaller(previous!);
                        string? command = userKey?.GetValue("UninstallString") as string;
                        string commandExe = string.IsNullOrEmpty(command) ? string.Empty : ExtractCommandPath(command!);

                        if (uninstaller != null)
                        {
                            report?.Invoke("正在卸载旧版本…");
                            RunSilentUninstaller(uninstaller, false);
                        }
                        else if (commandExe.Length > 0 && File.Exists(commandExe))
                        {
                            report?.Invoke("正在卸载旧版本…");
                            RunSilentUninstaller(commandExe, command!.Contains("--uninstall", StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            outcome.Notes.Add("检测到旧版本目录 " + previous + "，未找到卸载程序，已跳过清理。");
                        }
                    }
                }

                foreach (string keyPath in new[] { Program.UninstallKey, Program.LegacyUninstallKey })
                {
                    using RegistryKey? machineKey = Registry.LocalMachine.OpenSubKey(keyPath);
                    string? machineDir = machineKey?.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrEmpty(machineDir))
                    {
                        outcome.Notes.Add("检测到旧版机器级安装（" + machineDir +
                            "），请在「设置 → 应用」中卸载旧版本后重新安装。");
                    }
                }
            }
            catch
            {
                // 注册表读取失败不影响本次安装
            }
        }

        /// <summary>从注册表 UninstallString 中取出可执行文件路径（兼容带引号与带参数两种写法）。</summary>
        private static string ExtractCommandPath(string commandLine)
        {
            string text = commandLine.Trim();
            if (text.StartsWith("\"", StringComparison.Ordinal))
            {
                int end = text.IndexOf('"', 1);
                return end > 1 ? text.Substring(1, end - 1) : text.Trim('"');
            }

            int space = text.IndexOf(' ');
            return space > 0 ? text.Substring(0, space) : text;
        }

        private static string? FindUninstaller(string directory)
        {
            string[] candidates = { "uninst.exe", "uninstall.exe", "Uninstall.exe" };
            foreach (string name in candidates)
            {
                string path = Path.Combine(directory, name);
                if (File.Exists(path)) return path;
            }

            return null;
        }

        /// <summary>调用旧版卸载程序（NSIS 版使用 /S 静默，新版使用 --uninstall --yes），最多等待 60 秒。</summary>
        private static void RunSilentUninstaller(string uninstallerPath, bool customArgs)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = uninstallerPath,
                    Arguments = customArgs ? "--uninstall --yes" : "/S",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(uninstallerPath) ?? Path.GetTempPath()
                };

                using Process? process = Process.Start(startInfo);
                process?.WaitForExit(60000);
            }
            catch
            {
                // 卸载程序无法运行时保持现状，本次安装照常继续
            }
        }

        private static bool SamePath(string left, string right)
        {
            try
            {
                string a = NormalizeForCompare(Path.GetFullPath(left));
                string b = NormalizeForCompare(Path.GetFullPath(right));
                return a.Equals(b, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // ------------------------------------------------------------ 载荷解压

        /// <summary>
        /// 解压内嵌载荷。每个文件最多写 3 次（FileShare.None 独占打开），
        /// 全部尝试都失败时只累加失败清单，绝不弹出逐文件对话框。
        /// </summary>
        /// <param name="failureList">
        /// 非空时把失败项（最多 10 条明细 + 1 条汇总）追加进去，供调用方判定「部分失败」并写进汇总页。
        /// </param>
        internal static void ExtractPayload(string installDir, out int written, out int failed,
            Action<string>? report, List<string>? failureList = null)
        {
            written = 0;
            failed = 0;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string? resource = null;
            foreach (string name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(Program.PayloadSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    resource = name;
                    break;
                }
            }

            if (resource == null)
                throw new InvalidOperationException("安装包内容缺失（未找到内嵌载荷）。");

            using Stream? stream = assembly.GetManifestResourceStream(resource);
            if (stream == null)
                throw new InvalidOperationException("安装包内容缺失（载荷无法读取）。");

            // 越界防护（Zip Slip）：条目必须落在安装目录内部，否则一律拒绝写入
            string root = Path.GetFullPath(installDir).TrimEnd(Path.DirectorySeparatorChar)
                          + Path.DirectorySeparatorChar;

            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            int total = archive.Entries.Count;
            int index = 0;
            int reported = 0;
            int failureStart = failureList?.Count ?? 0;

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                index++;
                if (index % 40 == 0) report?.Invoke("正在复制程序文件… " + index + " / " + total);

                // 保留目录结构：自包含发布包含运行库文件与子目录
                string relative = entry.FullName.Replace('/', Path.DirectorySeparatorChar);

                string target;
                if (!TryResolveTarget(installDir, root, relative, out target))
                {
                    failed++;
                    report?.Invoke("已阻止可疑路径：" + entry.FullName);
                    if (failureList != null && reported < MaxReportedFailures)
                    {
                        reported++;
                        failureList.Add("程序文件（已阻止可疑路径）：" + entry.FullName);
                    }

                    continue;
                }

                if (entry.Name.Length == 0)
                {
                    try
                    {
                        Directory.CreateDirectory(target);
                    }
                    catch
                    {
                        // 目录创建失败会体现为后续文件写入失败，这里不单独计数
                    }

                    continue;
                }

                if (TryWriteEntry(entry, target))
                {
                    written++;
                }
                else
                {
                    failed++;
                    string shown = entry.FullName.Replace('/', '\\');
                    report?.Invoke("未能写入：" + shown);
                    if (failureList != null && reported < MaxReportedFailures)
                    {
                        reported++;
                        failureList.Add("程序文件：" + shown);
                    }
                }
            }

            // 汇总一条并插到本段失败明细之前：保证「残缺安装」一定出现在结果页最上方，
            // 不会因为逐条明细超过显示上限而被省略
            if (failed > 0 && failureList != null)
            {
                failureList.Insert(failureStart,
                    "程序文件：" + failed + " 个未能写入（成功 " + written + " 个，共 " + (written + failed) + " 个）");
            }
        }

        /// <summary>
        /// 把 zip 条目名解析成安装目录内的绝对路径；越界（.. / 绝对路径 / 非法字符）返回 false。
        /// </summary>
        private static bool TryResolveTarget(string installDir, string root, string relative, out string target)
        {
            target = string.Empty;

            try
            {
                target = Path.GetFullPath(Path.Combine(installDir, relative));
            }
            catch
            {
                // 条目名含非法字符（例如绝对路径里的 ':'）时 GetFullPath 会抛异常
                return false;
            }

            return target.StartsWith(root, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryWriteEntry(ZipArchiveEntry entry, string target)
        {
            for (int attempt = 1; attempt <= MaxWriteAttempts; attempt++)
            {
                try
                {
                    string? folder = Path.GetDirectoryName(target);
                    if (folder != null) Directory.CreateDirectory(folder);

                    // FileShare.None：避免安全软件或旧进程持有句柄时写到一半
                    using Stream input = entry.Open();
                    using var output = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None);
                    input.CopyTo(output);
                    return true;
                }
                catch (IOException)
                {
                    // 文件被占用或磁盘忙：稍后再写一次
                }
                catch (UnauthorizedAccessException)
                {
                    // 目录只读或写入被拒：稍后再写一次
                }

                if (attempt < MaxWriteAttempts) Thread.Sleep(WriteAttemptDelayMs);
            }

            // 最后一次可能在 CopyTo 中途失败，留下半截文件；删掉它，避免误以为已装好
            TryDeleteFile(target);
            return false;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // 文件仍被占用时保持原样，交由失败清单提示
            }
        }

        /// <summary>
        /// 递归清除所有文件的 Zone.Identifier（不限扩展名），相当于批量「解除锁定」。
        /// 安装产物来自带「来自网络」标记的载荷，未清除时首次启动会被系统再次拦截。
        /// </summary>
        internal static void ClearZoneIdentifiers(string directory, out int scanned)
        {
            scanned = 0;
            if (!Directory.Exists(directory)) return;

            try
            {
                foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                {
                    scanned++;
                    try
                    {
                        Program.DeleteFileW(file + ":Zone.Identifier");
                    }
                    catch
                    {
                        // 没有该数据流或权限不足时保持原样
                    }
                }
            }
            catch
            {
                // 目录不可枚举时保持原状
            }
        }

        // ------------------------------------------------------------ 快捷方式

        /// <summary>快捷方式写在当前用户下：桌面用 DesktopDirectory，开始菜单用 Programs，均不需要管理员权限。</summary>
        private static void CreateShortcuts(string installDir, bool desktopShortcut, InstallOutcome outcome)
        {
            string launcher = Path.Combine(installDir, Program.LauncherName);
            string appExe = Path.Combine(installDir, Program.AppSubDir, Program.AppExeName);

            try
            {
                string startMenuFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Programs), Program.AppName);

                CreateShortcut(Path.Combine(startMenuFolder, Program.AppName + ".lnk"),
                    launcher, installDir, string.Empty, Program.AppName);

                // 卸载入口与旧版 NSIS 安装保持一致，便于用户在开始菜单里直接卸载
                CreateShortcut(Path.Combine(startMenuFolder, "卸载 " + Program.AppName + ".lnk"),
                    appExe, Path.GetDirectoryName(appExe) ?? installDir, "--uninstall", "卸载 " + Program.AppName);
            }
            catch (Exception ex)
            {
                outcome.Failures.Add("开始菜单快捷方式：" + Describe(ex));
            }

            if (!desktopShortcut) return;

            try
            {
                CreateShortcut(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        Program.AppName + ".lnk"),
                    launcher, installDir, string.Empty, Program.AppName);
            }
            catch (Exception ex)
            {
                outcome.Failures.Add("桌面快捷方式：" + Describe(ex));
            }
        }

        /// <summary>取最内层异常的信息：COM 调用失败会被包装成 TargetInvocationException，外层信息没有诊断价值。</summary>
        private static string Describe(Exception exception)
        {
            Exception innermost = exception;
            while (innermost.InnerException != null) innermost = innermost.InnerException;

            string message = innermost.Message;
            if (innermost is COMException com)
                message += "（HRESULT=0x" + com.HResult.ToString("X8") + "）";

            return message;
        }

        private static void CreateShortcut(string linkPath, string target, string workingDir,
            string arguments, string description)
        {
            string? folder = Path.GetDirectoryName(linkPath);
            if (folder != null) Directory.CreateDirectory(folder);

            RemoveQuietly(linkPath);

            // 优先用 shell32 的 ShellLink 对象（不依赖 Windows Script Host，某些受管控的
            // 系统会禁用 WSH）；失败时退回 WScript.Shell，两条路都失败才视为失败。
            try
            {
                ShellLinkWriter.Create(linkPath, target, workingDir, arguments, description);
                return;
            }
            catch (Exception shellLinkError)
            {
                try
                {
                    ShellLinkWriter.CreateWithWScript(linkPath, target, workingDir, arguments, description);
                    return;
                }
                catch (Exception wscriptError)
                {
                    throw new InvalidOperationException(
                        Describe(shellLinkError) + " / " + Describe(wscriptError));
                }
            }
        }

        private static void RemoveQuietly(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // 旧快捷方式被占用时保留，后续写入会覆盖
            }
        }

        // ------------------------------------------------------------ 卸载信息（HKCU）

        /// <summary>
        /// 卸载信息写 HKCU（当前用户级安装），字段与旧版 NSIS 安装保持一致，
        /// 卸载交给安装目录内的 app\DocHub.exe --uninstall，无需额外复制卸载器。
        /// </summary>
        private static void WriteUninstallRegistry(string installDir, InstallOutcome outcome)
        {
            try
            {
                string appExe = Path.Combine(installDir, Program.AppSubDir, Program.AppExeName);
                string launcher = Path.Combine(installDir, Program.LauncherName);
                string displayIcon = File.Exists(launcher) ? launcher : appExe;
                int estimatedSizeKb = EstimateSizeKb(installDir);

                using RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.UninstallKey, true)
                    ?? throw new InvalidOperationException("无法写入当前用户的卸载登记项。");

                key.SetValue("DisplayName", Program.AppName);
                key.SetValue("DisplayVersion", Program.AppVersion);
                key.SetValue("Publisher", Program.AppPublisher);
                key.SetValue("DisplayIcon", displayIcon);
                key.SetValue("InstallLocation", installDir);
                key.SetValue("UninstallString", "\"" + appExe + "\" --uninstall");
                key.SetValue("QuietUninstallString", "\"" + appExe + "\" --uninstall --yes");
                key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                if (estimatedSizeKb > 0)
                    key.SetValue("EstimatedSize", estimatedSizeKb, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                outcome.Failures.Add("卸载登记项：" + ex.Message);
            }
        }

        private static int EstimateSizeKb(string directory)
        {
            long total = 0;
            try
            {
                foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        total += new FileInfo(file).Length;
                    }
                    catch
                    {
                        // 单个文件无法读取时跳过
                    }
                }
            }
            catch
            {
                return 0;
            }

            return (int)((total + 1023) / 1024);
        }
    }

    /// <summary>
    /// 写 .lnk 快捷方式。优先使用 shell32 的 ShellLink 对象（IShellLinkW + IPersistFile），
    /// 不依赖 Windows Script Host —— 部分受管控的系统会禁用 WSH；WSH 可用时作为后备方案。
    /// </summary>
    internal static class ShellLinkWriter
    {
        internal static void Create(string linkPath, string target, string workingDir,
            string arguments, string description)
        {
            var link = (IShellLinkW)new ShellLinkCoClass();
            try
            {
                link.SetPath(target);
                link.SetWorkingDirectory(workingDir);
                link.SetDescription(description);
                link.SetIconLocation(target, 0);
                if (arguments.Length > 0) link.SetArguments(arguments);

                ((IPersistFile)link).Save(linkPath, true);
            }
            finally
            {
                if (Marshal.IsComObject(link)) Marshal.ReleaseComObject(link);
            }
        }

        internal static void CreateWithWScript(string linkPath, string target, string workingDir,
            string arguments, string description)
        {
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null) throw new InvalidOperationException("系统缺少 WScript.Shell 组件。");

            object? shell = Activator.CreateInstance(shellType);
            if (shell == null) throw new InvalidOperationException("无法创建 WScript.Shell 对象。");

            try
            {
                object? link = shellType.InvokeMember("CreateShortcut",
                    BindingFlags.InvokeMethod, null, shell, new object[] { linkPath });
                if (link == null) throw new InvalidOperationException("WScript.Shell 未返回快捷方式对象。");

                Type linkType = link.GetType();
                linkType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, link, new object[] { target });
                linkType.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, link, new object[] { workingDir });
                linkType.InvokeMember("IconLocation", BindingFlags.SetProperty, null, link, new object[] { target });
                linkType.InvokeMember("Description", BindingFlags.SetProperty, null, link, new object[] { description });
                if (arguments.Length > 0)
                {
                    linkType.InvokeMember("Arguments", BindingFlags.SetProperty, null, link, new object[] { arguments });
                }

                linkType.InvokeMember("Save", BindingFlags.InvokeMethod, null, link, null);
            }
            finally
            {
                if (Marshal.IsComObject(shell)) Marshal.ReleaseComObject(shell);
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLinkCoClass
        {
        }

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, IntPtr pfd, uint fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short wHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int iShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int iIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hwnd, uint fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig] int IsDirty();
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                [MarshalAs(UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }
    }

    internal sealed class SetupForm : Form
    {
        /// <summary>结果页逐条列出的失败项上限，超出的折叠成一行。</summary>
        private const int MaxFailureLines = 10;

        private readonly TextBox _pathBox;
        private readonly CheckBox _desktopBox;
        private readonly CheckBox _launchBox;
        private readonly Button _installButton;
        private readonly Button _cancelButton;
        private readonly Button _browseButton;
        private readonly Label _statusLabel;
        private readonly Label _hintLabel;
        private readonly TextBox _logBox;

        private bool _busy;
        private bool _finished;
        private string _installDir;

        internal SetupForm(string? initialDirectory)
        {
            Text = Program.AppName + " 安装程序";
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath ?? Application.ExecutablePath);
            }
            catch
            {
                // 无法取出图标时使用默认图标
            }

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(620, 540);
            BackColor = Color.White;
            Font = new Font("Microsoft YaHei UI", 9f);
            AutoScaleMode = AutoScaleMode.None;

            _installDir = InstallerEngine.SanitizeInstallDir(initialDirectory, out bool rewritten);

            var title = new Label
            {
                Text = Program.AppName,
                Font = new Font("Microsoft YaHei UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(38, 44, 56),
                AutoSize = true,
                Location = new Point(28, 22)
            };

            var subtitle = new Label
            {
                Text = "Word / PDF / 图片 无损合并工具 · v" + Program.AppVersion,
                Font = new Font("Microsoft YaHei UI", 9f),
                ForeColor = Color.FromArgb(130, 138, 150),
                AutoSize = true,
                Location = new Point(31, 66)
            };

            var pathLabel = new Label
            {
                Text = "安装位置",
                AutoSize = true,
                Location = new Point(30, 102)
            };

            _pathBox = new TextBox
            {
                Bounds = new Rectangle(30, 124, 440, 26),
                Text = _installDir
            };
            _pathBox.Leave += (_, _) => ApplyPathBoxText();

            _browseButton = new Button
            {
                Text = "浏览…",
                Bounds = new Rectangle(484, 122, 90, 28)
            };
            _browseButton.Click += (_, _) => BrowseFolder();

            _hintLabel = new Label
            {
                Text = HintFor(_installDir, rewritten),
                ForeColor = Color.FromArgb(110, 118, 132),
                AutoSize = false,
                Bounds = new Rectangle(30, 160, 556, 40)
            };

            _desktopBox = new CheckBox
            {
                Text = "创建桌面快捷方式",
                Bounds = new Rectangle(30, 208, 240, 24),
                Checked = true
            };

            _launchBox = new CheckBox
            {
                Text = "安装完成后启动",
                Bounds = new Rectangle(30, 234, 240, 24),
                Checked = true
            };

            _statusLabel = new Label
            {
                Text = "准备就绪。",
                ForeColor = Color.FromArgb(72, 118, 255),
                AutoSize = false,
                Bounds = new Rectangle(30, 274, 556, 22)
            };

            _logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 251),
                Bounds = new Rectangle(30, 300, 556, 170),
                Visible = false
            };

            _cancelButton = new Button
            {
                Text = "取消",
                Bounds = new Rectangle(410, 490, 84, 32),
                DialogResult = DialogResult.Cancel
            };
            _cancelButton.Click += (_, _) => { if (!_busy) Close(); };

            _installButton = new Button
            {
                Text = "安装",
                Bounds = new Rectangle(504, 490, 84, 32),
                BackColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _installButton.FlatAppearance.BorderSize = 0;
            _installButton.Click += async (_, _) => await InstallButtonClicked();

            AcceptButton = _installButton;

            Controls.AddRange(new Control[]
            {
                title, subtitle, pathLabel, _pathBox, _browseButton, _hintLabel,
                _desktopBox, _launchBox, _statusLabel, _logBox, _cancelButton, _installButton
            });
        }

        private static string HintFor(string directory, bool rewritten)
        {
            if (rewritten)
            {
                return "路径无法识别，已改用默认目录（%LOCALAPPDATA%\\Programs\\DocHub）。";
            }

            if (InstallerEngine.IsUnderProgramFiles(directory))
            {
                return "该位置在 Program Files 下，普通用户通常无法写入：安装可能失败。" +
                       "建议改到当前用户目录或其它磁盘，或以管理员身份运行安装程序。";
            }

            return "默认安装到当前用户目录（%LOCALAPPDATA%\\Programs\\DocHub），无需管理员权限；" +
                   "也可以改成任意有写入权限的目录。";
        }

        /// <summary>路径可编辑：用户填什么就装到哪里，只在路径无法识别时退回默认目录。</summary>
        private void ApplyPathBoxText()
        {
            if (_busy) return;

            _installDir = InstallerEngine.SanitizeInstallDir(_pathBox.Text.Trim(), out bool rewritten);
            _pathBox.Text = _installDir;
            _hintLabel.Text = HintFor(_installDir, rewritten);
        }

        private void BrowseFolder()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "选择安装位置",
                SelectedPath = _installDir,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            _pathBox.Text = dialog.SelectedPath;
            ApplyPathBoxText();
        }

        private async Task InstallButtonClicked()
        {
            // 安装结束后按钮变为「关闭」
            if (_finished)
            {
                Close();
                return;
            }

            if (_busy) return;

            ApplyPathBoxText();
            string installDir = _installDir;
            bool desktop = _desktopBox.Checked;

            // 只有仍在使用默认目录时才允许自动换位置；用户指定过的目录必须原样使用
            bool allowRelocate = string.Equals(installDir, InstallerEngine.DefaultInstallDir(),
                StringComparison.OrdinalIgnoreCase);

            SetBusy(true);
            _logBox.Visible = false;
            _statusLabel.ForeColor = Color.FromArgb(72, 118, 255);
            _statusLabel.Text = "正在准备安装…";

            InstallOutcome outcome;
            try
            {
                outcome = await Task.Run(() => InstallerEngine.Install(installDir, desktop, ReportProgress, allowRelocate));
            }
            catch (Exception ex)
            {
                SetBusy(false);
                _statusLabel.ForeColor = Color.FromArgb(200, 56, 56);
                _statusLabel.Text = "安装未完成：" + ex.Message;
                _logBox.Text = "安装未能完成：" + ex.Message + Environment.NewLine + Environment.NewLine +
                               "已尝试多个用户目录但仍无法写入，通常是安全软件拦截或该目录权限异常。" + Environment.NewLine +
                               "可在下方路径框把安装目录改到其它位置（例如 D:\\Apps\\DocHub）后重试，" + Environment.NewLine +
                               "或改用绿色版压缩包（解压即用，不写系统目录）。";
                _logBox.Visible = true;
                return;
            }

            SetBusy(false);
            ShowOutcome(outcome);
        }

        private void ReportProgress(string message)
        {
            if (IsDisposed || !IsHandleCreated) return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (IsDisposed) return;
                    _statusLabel.Text = message;
                }));
            }
            catch
            {
                // 窗体已在关闭过程中，不再更新进度
            }
        }

        private void ShowOutcome(InstallOutcome outcome)
        {
            _finished = true;
            _installButton.Text = "关闭";
            _pathBox.ReadOnly = true;
            _browseButton.Enabled = false;
            _desktopBox.Enabled = false;

            bool launchAfterInstall = _launchBox.Checked && outcome.Succeeded;
            _launchBox.Enabled = false;

            if (outcome.Succeeded)
            {
                // 自动换过位置时，路径框同步显示实际安装目录
                _installDir = outcome.InstallDir;
                _pathBox.Text = outcome.InstallDir;

                _statusLabel.ForeColor = Color.FromArgb(36, 150, 96);
                _statusLabel.Text = "安装完成，已写入 " + outcome.FilesWritten + " 个文件。";

                var lines = new List<string>();
                lines.Add("安装目录：" + outcome.InstallDir);
                lines.Add("可在「设置 → 应用 → 已安装的应用」中卸载「" + Program.AppName + "」。");
                foreach (string note in outcome.Notes) lines.Add("提示：" + note);

                _logBox.Text = string.Join(Environment.NewLine, lines);
                _logBox.Visible = true;

                if (launchAfterInstall) LaunchApplication(outcome.InstallDir);
                return;
            }

            _statusLabel.ForeColor = Color.FromArgb(200, 56, 56);
            _statusLabel.Text = "安装未完全成功：共 " + outcome.Failures.Count + " 项需要处理，其余文件已安装完成。";

            var failureLines = new List<string>();
            failureLines.Add("以下内容未能写入（最多列出 " + MaxFailureLines + " 条）：");
            int shown = 0;
            foreach (string failure in outcome.Failures)
            {
                if (shown >= MaxFailureLines)
                {
                    failureLines.Add("……其余 " + (outcome.Failures.Count - shown) + " 条已省略。");
                    break;
                }

                shown++;
                failureLines.Add(shown + ". " + failure);
            }

            failureLines.Add(string.Empty);
            failureLines.Add("已成功写入的文件保留在：" + outcome.InstallDir);
            foreach (string note in outcome.Notes) failureLines.Add("提示：" + note);
            failureLines.Add(string.Empty);
            failureLines.Add("可尝试：暂时关闭杀毒软件 / 安全软件的文件实时防护后重新运行安装程序；");
            failureLines.Add("或改用绿色版压缩包，解压即用，不写系统目录、不写注册表。");

            _logBox.Text = string.Join(Environment.NewLine, failureLines);
            _logBox.Visible = true;
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            _pathBox.Enabled = !busy;
            _browseButton.Enabled = !busy;
            _desktopBox.Enabled = !busy;
            _launchBox.Enabled = !busy;
            _installButton.Enabled = !busy;
            _cancelButton.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        /// <summary>用资源管理器拉起启动器：安装程序本身不再提权，启动的进程就是普通用户权限。</summary>
        private static void LaunchApplication(string installDir)
        {
            string launcher = Path.Combine(installDir, Program.LauncherName);
            string target = File.Exists(launcher)
                ? launcher
                : Path.Combine(installDir, Program.AppSubDir, Program.AppExeName);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    WorkingDirectory = installDir,
                    // 不用 ShellExecute：避免文件万一仍带「来自网络」标记时被系统拦截弹窗
                    UseShellExecute = false
                });
            }
            catch
            {
                // 启动失败不影响安装结果
            }
        }
    }
}
