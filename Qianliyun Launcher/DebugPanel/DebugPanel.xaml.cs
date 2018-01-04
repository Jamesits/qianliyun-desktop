using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using NLog;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher.DebugPanel
{
    /// <summary>
    /// Interaction logic for DebugPanel.xaml
    /// </summary>
    public partial class DebugPanel : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Settings ApplicationConfig => Properties.Settings.Default;
        private StateManager State => StateManager.Instance;

        public DebugPanel()
        {
            InitializeComponent();
            Browser.FrameLoadStart += (sender, e) =>
            {
                Logger.Debug("Loading new frame {0}", e.Url);
            };
            Browser.FrameLoadEnd += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Logger.Debug("URL changed to {0}", Browser.Address);
                    AddressBar.Text = Browser.Address;
                });
            };
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Emulating login");
            State.api.Login("username", "password", false);
        }

        private void BtnClearLoginStatus_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Force clear login status");
            State.api.ClearLoginStatus();
        }

        private void ButtonLogWindow_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Toggling LogWindow visibility");
            if (State._logWindow.IsVisible)
                State._logWindow.Hide();
            else
                State._logWindow.Show();
        }

        private void BtnBrowserGoto_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Debug("Trying to load {0}", AddressBar.Text);
                Browser.Load(AddressBar.Text);
            }
            catch (UriFormatException ex)
            {
                Logger.Fatal(ex);
                throw;
            }
        }

        private void BtnBrowserDevTool_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Enable DevTools");
            Browser.ShowDevTools();
        }
    }
}
