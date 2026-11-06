using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

// 精简启动器：安装目录顶层只保留这个约 20KB 的 exe，真正的主程序与运行库放在 app\ 下。
// 由 tools\package.ps1 使用 .NET Framework 的 csc.exe 编译（C# 5 语法，勿使用新语法）。
internal static class Launcher
{
    // 递归清除标记时的文件数上限：避免极端情况下启动被拖慢
    private const int MaxFilesToUnblock = 2000;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteFileW(string fileName);

    [STAThread]
    private static void Main(string[] args)
    {
        string root = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
        string appDir = Path.Combine(root, "app");
        string exe = Path.Combine(appDir, "DocHub.exe");

        if (!File.Exists(exe)) return;

        // 从网上下载的压缩包解压出来的文件带「来自网络」标记（Zone.Identifier），
        // 双击时 Windows 会弹出「你的 Internet 安全设置阻止打开一个或多个文件」。
        // 这里在启动内层程序之前，把安装目录与 app 子目录里所有文件的标记先清掉，
        // 内层程序加载运行库时就不会再被系统拦截（不依赖文件扩展名）。
        UnblockDirectory(root);
        UnblockDirectory(appDir);

        string arguments = string.Empty;
        if (args.Length > 0)
        {
            string[] quoted = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                quoted[i] = args[i].IndexOf(' ') >= 0 ? "\"" + args[i] + "\"" : args[i];
            }
            arguments = string.Join(" ", quoted);
        }

        var start = new ProcessStartInfo(exe, arguments);
        start.WorkingDirectory = appDir;
        start.UseShellExecute = false;

        // 被安全软件拦截、文件损坏或权限不足时 Process.Start 会抛异常；
        // 启动器是最外层的壳，不能把未处理异常抛给 Windows（会弹 WER 对话框），
        // 这里静默退出，用户重新双击即可。
        try
        {
            Process.Start(start);
        }
        catch
        {
            // 无需处理：不给用户增加一个看不懂的系统错误框
        }
    }

    /// <summary>递归清除目录下所有文件的 Zone.Identifier（相当于批量「解除锁定」）。失败时保持原样。</summary>
    private static void UnblockDirectory(string directory)
    {
        if (!Directory.Exists(directory)) return;

        string[] files;
        try
        {
            files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        }
        catch
        {
            return;
        }

        int count = 0;
        for (int i = 0; i < files.Length && count < MaxFilesToUnblock; i++)
        {
            count++;
            Unblock(files[i]);
        }
    }

    /// <summary>删除文件的 Zone.Identifier 备用数据流（相当于「解除锁定」）。失败时保持原样。</summary>
    private static void Unblock(string path)
    {
        try
        {
            DeleteFileW(path + ":Zone.Identifier");
        }
        catch
        {
            // 无需处理：没有该数据流或权限不足时保持原样
        }
    }
}
