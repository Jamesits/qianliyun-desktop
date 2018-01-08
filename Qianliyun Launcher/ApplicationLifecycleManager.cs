using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using NLog;
using Pathoschild.Http.Client;
using Qianliyun_Launcher.DeepDarkWin32Fantasy;
using Qianliyun_Launcher.Dialogs.LoginDialog;

namespace Qianliyun_Launcher
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static StateManager State => StateManager.Instance;

        #region Windows

        #endregion

        #region debug initialization
        [Conditional("DEBUG")]
        private static void InitDebugEnv()
        { }
        #endregion

        [STAThread]
        private async void ApplicationStart(object sender, StartupEventArgs e)
        {
            Logger.Debug("App Init");
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // parse arguments
            State.Args = e.Args;
            if (Util.StringInArray("/noshutup", State.Args, false)) State.IsDebugMode = true;

            InitDebugEnv();

            // launch log window
            Logger.Debug("Launching logging window");
            State._logWindow = new LogWindow.LogWindow();
            if (State.IsDebugMode) State._logWindow.Show();

            // basic information
            var sb = new StringBuilder(string.Empty);
            sb.AppendLine("Operation System Information");
            sb.AppendLine("----------------------------");
            sb.AppendLine($"Name = {OSVersionInfo.Name}");
            sb.AppendLine($"Edition = {OSVersionInfo.Edition}");
            sb.AppendLine(OSVersionInfo.ServicePack != string.Empty
                ? $"Service Pack = {OSVersionInfo.ServicePack}"
                : "Service Pack = None");
            sb.AppendLine($"Version = {OSVersionInfo.VersionString}");
            sb.AppendLine($"ProcessorBits = {OSVersionInfo.ProcessorBits}");
            sb.AppendLine($"OSBits = {OSVersionInfo.OSBits}");
            sb.AppendLine($"ProgramBits = {OSVersionInfo.ProgramBits}");
            Logger.Debug(sb);
            Logger.Debug("Assembly: {0}", State.AssemblyName);
            Logger.Debug("AppName: {0}", State.AppName);
            Logger.Debug("Application Version: {0}", State.AppVersionString);
            Logger.Info("Machine GUID is {0}", State.MachineKey);

            // prevent multiple instances
            if (!State.IsFirstInstance)
            {
                Logger.Fatal("Multiple instance detected, quitting");
                MessageBox.Show("本程序不支持同时运行");
                Current.Shutdown(1);
            }

            // check login status
            State._loginDialog = new LoginDialog();
            if (!State.SaveLoginStatus || !await State.api.VerifyCachedLoginCredential())
            {
                while (!State.IsLoggedIn)
                {
                    Logger.Info("Try login");
                    var ret = false;
                    try
                    {
                        ret = await State._loginDialog.DoLogin();
                    }
                    catch (ApiException ex)
                    {
                        Logger.Fatal("Login failed: {0}", ex);
                        MessageBox.Show("登录失败：" + ex.Message, "登录失败");
                    }
                    finally
                    {
                        if (!ret) Current.Shutdown(1);
                    }
                }
            }
            else
            {
                State.IsLoggedIn = true;
                Logger.Debug("Logged in using saved credential");
            }

            Logger.Debug("Logged in");

            // pull and populate global config
            await State.api.PopulateAccountInformation();

            // launch background window
            Logger.Debug("Loading background window");
            State._bgWindow = new BackgroundWindow();
            //this.bgWindow.EnableDebugMode();

            // launch main window
            Logger.Debug("Loading Main Window");
            State._mainWindow = new MainWindow();
            State._mainWindow.Show();

            //Re-enable normal shutdown mode.
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = State._mainWindow;
        }
    }
}
