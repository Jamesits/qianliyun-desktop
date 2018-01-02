using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private StateManager State => StateManager.Instance;

        #region Windows
        private BackgroundWindow _bgWindow;
        private MainWindow _mainWindow;
        #endregion

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // check login status
            do
            {
                Thread.Sleep(1000);
            } while (!State.IsLoggedIn);


            // pull and populate global config

            // launch background window
            Logger.Debug("Loading background window");
            _bgWindow = new BackgroundWindow();
            //this.bgWindow.EnableDebugMode();

            // launch main window
            Logger.Debug("Loading Main Window");
            _mainWindow = new MainWindow();

            //Re-enable normal shutdown mode.
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = _mainWindow;
        }
    }
}
