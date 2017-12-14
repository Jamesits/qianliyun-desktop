using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CefSharp;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;
using UserControl = System.Windows.Controls.UserControl;

namespace Qianliyun_Launcher.BroadcastCapture
{
    /// <summary>
    /// Interaction logic for BroadcastCaptureUI.xaml
    /// </summary>
    public partial class BroadcastCaptureUI : UserControl
    {
        private GlobalStatus status;
        private CaptureStorage result = new CaptureStorage();
        public BroadcastCaptureUI(GlobalStatus status)
        {
            InitializeComponent();
            this.status = status;
            // this.CaptureBrowser.ShowDevTools();
            this.CaptureBrowser.FrameLoadStart += CaptureBrowser_FrameLoadStart;
            this.CaptureBrowser.LoadingStateChanged += CaptureBrowserOnLoadingStateChanged;
            this.CaptureBrowser.RegisterAsyncJsObject("capture", result, BindingOptions.DefaultBinder);
        }

        private void CaptureBrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
        }

        private void CaptureBrowser_FrameLoadStart(object sender, CefSharp.FrameLoadStartEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.AddressBar.Text = e.Url;
            });
        }


        private void BtnGoto_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                this.CaptureBrowser.Load(this.AddressBar.Text);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}
