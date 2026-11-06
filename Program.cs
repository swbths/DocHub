using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace DocHub
{
    internal static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFileW(string fileName);

        [STAThread]
        private static void Main(string[] args)
        {
            // 绿色版从网上下载的压缩包里解压后，文件会带「来自网络」标记，
            // 双击时 Windows 会弹出「Internet 安全设置阻止打开」的提示。
            // 启动时把这些标记清掉，之后运行不再出现任何系统提示。
            UnblockProgramFiles();

            // 支持库（PDFsharp 等）单独放在程序目录的 lib 子目录，运行时按名称到这里查找
            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                if (name.Name == null) return null;
                string candidate = Path.Combine(AppContext.BaseDirectory, "lib", name.Name + ".dll");
                return File.Exists(candidate) ? context.LoadFromAssemblyPath(candidate) : null;
            };

            if (args.Length > 0 && args[0] == "--uninstall")
            {
                Uninstaller.Run(args);
                return;
            }

            // 静默自检：不创建任何窗口，结果写入 %TEMP%\DocHub-selftest.log
            if (args.Length > 0 && args[0] == "--selftest")
            {
                Environment.ExitCode = SelfTest.Run();
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetDefaultFont(Theme.BaseFont);

            Application.Run(new MainForm());
        }

        /// <summary>清掉程序目录与 lib 子目录里所有文件的 Zone.Identifier（相当于批量「解除锁定」）。</summary>
        private static void UnblockProgramFiles()
        {
            string appDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            UnblockDirectory(appDir);

            // 安装版的主程序位于 <安装目录>\app，启动器 DocHub.exe 在上一级
            string? installDir = Path.GetDirectoryName(appDir);
            if (installDir != null && File.Exists(Path.Combine(installDir, "DocHub.exe")))
                UnblockDirectory(installDir);
        }

        /// <summary>
        /// 递归清除目录下所有文件的网络标记（不限扩展名，覆盖 lib 子目录）。
        /// 文件数上限 2000：正常安装约 240 个文件，超限说明目录异常，直接跳过以免拖慢启动。
        /// </summary>
        private static void UnblockDirectory(string directory)
        {
            const int MaxFiles = 2000;

            try
            {
                if (!Directory.Exists(directory)) return;

                int count = 0;
                foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                {
                    if (count >= MaxFiles) return;
                    count++;

                    try { DeleteFileW(file + ":Zone.Identifier"); } catch { /* 无需处理 */ }
                }
            }
            catch
            {
                // 目录不存在或无权访问时保持原状
            }
        }
    }
}
