﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Netch.Controllers;
using Netch.Forms;
using Netch.Interfaces;
using Netch.Services;
using Netch.Utils;
using Serilog;
using Vanara.PInvoke;

namespace Netch
{
    public static class Netch
    {
        public static readonly SingleInstance.SingleInstance SingleInstance = new($"Global\\{nameof(Netch)}");

        public static HWND ConsoleHwnd { get; private set; }

        /// <summary>
        ///     应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Contains(Constants.Parameter.ForceUpdate))
                Flags.AlwaysShowNewVersionFound = true;

            // 设置当前目录
            Directory.SetCurrentDirectory(Global.NetchDir);
            var binPath = Path.Combine(Global.NetchDir, "bin");
            Environment.SetEnvironmentVariable("PATH", $"{Environment.GetEnvironmentVariable("PATH")};{binPath}");

            // 绑定错误捕获
#if !DEBUG
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_OnException;
#endif
            Application.ApplicationExit += Application_OnExit;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Updater.CleanOld(Global.NetchDir);

            CreateDirectories();

            DI.Register();

            var configuration = DI.GetRequiredService<IConfigService>();
            configuration.LoadAsync().Wait();

            if (!SingleInstance.IsFirstInstance)
            {
                SingleInstance.PassArgumentsToFirstInstance(args.Append(Constants.Parameter.Show));
                Environment.Exit(0);
            }

            SingleInstance.ArgumentsReceived.Subscribe(SingleInstance_ArgumentsReceived);

            InitConsole();

            CleanLogs();

            DI.CreateLogger();

            // 加载语言
            i18N.Load(Global.Settings.Language);

            CheckPrebuiltFiles();

            Task.Run(() =>
            {
                Log.Information("系统版本：{Version}", Environment.OSVersion);
                Log.Information("系统语言：{Version}", CultureInfo.CurrentCulture);
                Log.Information("版本: {Version}", $"{UpdateChecker.Owner}/{UpdateChecker.Repo}@{UpdateChecker.Version}");
                Log.Information("主程序 SHA256: {Hash}", $"{Misc.Sha256CheckSum(Global.NetchExecutable)}");
            });

            Application.Run(DI.GetRequiredService<MainForm>());
        }
        private static void CreateDirectories()
        {
            // 预创建目录
            var directories = new[] {"mode\\Custom", "data", "i18n", "logging"};
            foreach (var item in directories)
                if (!Directory.Exists(item))
                    Directory.CreateDirectory(item);
        }

        private static void CheckPrebuiltFiles()
        {
            if (!Directory.Exists("bin") || !Directory.EnumerateFileSystemEntries("bin").Any())
            {
                MessageBoxX.Show(i18N.Translate("Please extract all files then run the program!"));
                Environment.Exit(2);
            }
        }

        private static void CleanLogs()
        {
            // 清理上一次的日志文件，防止淤积占用磁盘空间
            if (Directory.Exists("logging"))
            {
                var directory = new DirectoryInfo("logging");

                foreach (var file in directory.GetFiles())
                    file.Delete();

                foreach (var dir in directory.GetDirectories())
                    dir.Delete(true);
            }
        }

        private static void InitConsole()
        {
            Kernel32.AllocConsole();

            ConsoleHwnd = Kernel32.GetConsoleWindow();
#if RELEASE
            User32.ShowWindow(ConsoleHwnd, ShowWindowCommand.SW_HIDE);
#endif
        }

        private static void Application_OnException(object sender, ThreadExceptionEventArgs e)
        {
            Log.Error(e.Exception, "未处理异常");
        }

        private static void Application_OnExit(object? sender, EventArgs eventArgs)
        {
            Log.CloseAndFlush();
        }

        private static void SingleInstance_ArgumentsReceived(IEnumerable<string> args)
        {
            if (args.Contains(Constants.Parameter.Show))
                Global.MainForm.ShowMainFormToolStripButton_Click(null!, null!);
        }
    }
}