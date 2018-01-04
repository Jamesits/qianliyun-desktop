using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Pathoschild.Http.Client;
using Qianliyun_Launcher.DeepDarkWin32Fantasy;
using Qianliyun_Launcher.Dialogs.LoginDialog;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static StateManager State => StateManager.Instance;

        #region Windows
        
        #endregion

        [STAThread]
        private async void ApplicationStart(object sender, StartupEventArgs e)
        {
            Logger.Debug("App Init");
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // launch log window
            Logger.Debug("Launching logging window");
            State._logWindow = new LogWindow.LogWindow();
            if (State.IsDebugMode) State._logWindow.Show();

            // basic information
            StringBuilder sb = new StringBuilder(String.Empty);
            sb.AppendLine("Operation System Information");
            sb.AppendLine("----------------------------");
            sb.AppendLine(String.Format("Name = {0}", OSVersionInfo.Name));
            sb.AppendLine(String.Format("Edition = {0}", OSVersionInfo.Edition));
            if (OSVersionInfo.ServicePack != string.Empty)
                sb.AppendLine(String.Format("Service Pack = {0}", OSVersionInfo.ServicePack));
            else
                sb.AppendLine("Service Pack = None");
            sb.AppendLine(String.Format("Version = {0}", OSVersionInfo.VersionString));
            sb.AppendLine(String.Format("ProcessorBits = {0}", OSVersionInfo.ProcessorBits));
            sb.AppendLine(String.Format("OSBits = {0}", OSVersionInfo.OSBits));
            sb.AppendLine(String.Format("ProgramBits = {0}", OSVersionInfo.ProgramBits));
            Logger.Debug(sb);
            Logger.Debug("Assembly: {0}", State.AssemblyName);
            Logger.Debug("AppName: {0}", State.AppName);
            Logger.Debug("Application Version: {0}", State.AppVersionString);
            Logger.Info("Machine GUID is {0}", State.MachineKey);

            // check login status
            State._loginDialog = new LoginDialog();
            if (!State.SaveLoginStatus || !State.api.VerifyCachedLoginCredential())
            {
                while (!State.IsLoggedIn)
                {
                    Logger.Info("Try login");
                    bool ret = false;
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
