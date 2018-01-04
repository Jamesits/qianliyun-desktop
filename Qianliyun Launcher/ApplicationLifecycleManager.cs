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
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // basic information
            Logger.Debug("Assembly: {0}", Assembly.GetEntryAssembly().GetName().Name);
            Logger.Info("Machine GUID is {0}", State.MachineKey);

            // launch log window
            Logger.Debug("Launching logging window");
            State._logWindow = new LogWindow.LogWindow();
            if (State.IsDebugMode) State._logWindow.Show();

            // check login status
            State._loginDialog = new LoginDialog();
            while (!State.IsLoggedIn || !State.api.verifyCachedLoginCredential())
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
                    MessageBox.Show(ex.Message, "网络故障");
                }
                finally
                {
                    if (!ret) Current.Shutdown(1);
                }
            }

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
