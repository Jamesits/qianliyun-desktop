using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;
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

        #region exception handler
        void AppDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Logger.Fatal(ex.Message);
        }

        private void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Fatal(e.Exception.Message);
            // Prevent default unhandled exception processing
            // e.Handled = true;
        }
        #endregion

        [STAThread]
        private async void ApplicationStart(object sender, StartupEventArgs e)
        {
            Logger.Debug("App Init");

            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // register global exception process methods
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledExceptionHandler;
            DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

            // parse arguments
            State.Args = e.Args;
            if (Util.StringInArray("/noshutup", State.Args, true)) State.IsDebugMode = true;

            InitDebugEnv();

            // launch log window
            Logger.Debug("Launching logging window");
            State._logWindow = new LogWindow.LogWindow();
            if (State.IsDebugMode) State._logWindow.Show();

            // basic information
            var sb = $@"Operation System Information
----------------------------
Name = {OSVersionInfo.Name}
Edition = {OSVersionInfo.Edition}
Service Pack = {OSVersionInfo.ServicePack}
Version = {OSVersionInfo.VersionString}
ProcessorBits = {OSVersionInfo.ProcessorBits}
OSBits = {OSVersionInfo.OSBits}
ProgramBits = {OSVersionInfo.ProgramBits}";
            Logger.Debug(sb);
            Logger.Debug("Assembly: {0}", State.AssemblyName);
            Logger.Debug("AppName: {0}", State.AppName);
            Logger.Debug("Application Version: {0}", State.AppVersionString);
            Logger.Debug("Application GUID is {0}", State.AppGuid);
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
