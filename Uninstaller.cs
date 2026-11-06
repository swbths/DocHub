using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DocHub
{
    /// <summary>
    /// 由主程序以 --uninstall 参数调用：主程序本身就是安装目录里的精简启动器，
    /// 因此无需再向安装目录复制一份体积很大的卸载程序。
    /// </summary>
    internal static class Uninstaller
    {
        private const string UninstallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\DocHub";
        private const string AppName = "文枢";

        public static void Run(string[] args)
        {
            bool alreadyConfirmed = false;
            foreach (string arg in args)
            {
                if (arg == "--yes") alreadyConfirmed = true;
            }

            string installDir = ResolveInstallDir(
                AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));

            // 安装在 Program Files 下（或注册表项在 HKLM）时，需要管理员权限才能删除，
            // 先向用户确认，再以管理员身份重新执行本卸载流程。
            if (NeedsElevation(installDir) && !IsElevated())
            {
                if (!alreadyConfirmed && !ConfirmUninstall()) return;
                RelaunchElevated();
                return;
            }

            if (!alreadyConfirmed && !ConfirmUninstall()) return;

            try
            {
                DeleteShortcuts();
                DeleteRegistryEntries();

                // 先启动延迟清理，再弹提示框：提示框会阻塞当前进程，
                // 若等用户确认后再清理，安装目录一直被占用，删除容易失败。
                StartDeferredDelete(installDir);
            }
            catch (Exception ex)
            {
                if (!alreadyConfirmed)
                {
                    MessageBox.Show("卸载失败：" + ex.Message, AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // --yes 为静默卸载（「设置 → 应用」与自动化脚本会用到），不弹任何提示框
            if (alreadyConfirmed) return;

            MessageBox.Show("已卸载「" + AppName + "」。", AppName,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static bool ConfirmUninstall()
        {
            return MessageBox.Show(
                "确定要卸载「" + AppName + "」吗？\n\n将删除程序文件、快捷方式和注册表项。",
                AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private static void RelaunchElevated()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "DocHub.exe"),
                    Arguments = "--uninstall --yes",
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch
            {
                // 用户取消 UAC 授权时保持原状
            }
        }

        private static bool IsElevated()
        {
            using var identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>安装目录位于系统程序目录，或注册表项写在 HKLM 时，需要管理员权限。</summary>
        private static bool NeedsElevation(string installDir)
        {
            using (RegistryKey? machineKey = Registry.LocalMachine.OpenSubKey(UninstallKey))
            {
                if (machineKey != null) return true;
            }

            string[] programRoots =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };

            foreach (string root in programRoots)
            {
                if (root.Length == 0) continue;
                if (installDir.StartsWith(root.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 主程序自身占用了安装目录里的文件，必须等本进程退出后才能删除，
        /// 因此交给后台 PowerShell 循环重试。用 -EncodedCommand 传递脚本，
        /// 避免安装路径含中文时批处理出现编码问题。
        /// </summary>
        private static void StartDeferredDelete(string installDir)
        {
            string dir = installDir.Replace("'", "''");
            string script =
                "$dir = '" + dir + "';" +
                "for ($i = 0; $i -lt 40; $i++) {" +
                "  if (-not (Test-Path -LiteralPath $dir)) { break }" +
                "  Remove-Item -LiteralPath $dir -Recurse -Force -ErrorAction SilentlyContinue;" +
                "  Start-Sleep -Milliseconds 500" +
                "}";
            string encoded = Convert.ToBase64String(
                System.Text.Encoding.Unicode.GetBytes(script));

            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -WindowStyle Hidden -EncodedCommand " + encoded,
                WorkingDirectory = Path.GetTempPath(),
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

        /// <summary>
        /// 安装版的主程序位于 app\ 下，安装目录是其上级（含 DocHub.exe 启动器）；
        /// 绿色版直接以程序目录为准。
        /// </summary>
        private static string ResolveInstallDir(string appDir)
        {
            string? parent = Path.GetDirectoryName(appDir);
            if (parent == null) return appDir;
            if (!string.Equals(Path.GetFileName(appDir), "app", StringComparison.OrdinalIgnoreCase))
                return appDir;

            if (File.Exists(Path.Combine(parent, "DocHub.exe")))
                return parent;

            return appDir;
        }

        private static void DeleteShortcuts()
        {
            // 安装包把快捷方式放在「所有用户」下，同时兼容旧版本的用户级快捷方式
            DeleteLink(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                AppName + ".lnk"));
            DeleteLink(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                AppName + ".lnk"));

            DeleteFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), AppName));
            DeleteFolder(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs), AppName));
        }

        private static void DeleteLink(string path)
        {
            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { /* 快捷方式被占用时忽略 */ }
            }
        }

        private static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                try { Directory.Delete(path, true); } catch { /* 忽略 */ }
            }
        }

        private static void DeleteRegistryEntries()
        {
            try { Registry.LocalMachine.DeleteSubKeyTree(UninstallKey, false); } catch { /* 无权限时忽略 */ }
            try { Registry.CurrentUser.DeleteSubKeyTree(UninstallKey, false); } catch { /* 忽略 */ }
        }
    }
}
