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

namespace Qianliyun_Launcher.BroadcastCapture
{
    /// <summary>
    /// Interaction logic for BroadcastCaptureUI.xaml
    /// </summary>
    public partial class BroadcastCaptureUI : UserControl
    {
        private GlobalStatus status;
        public BroadcastCaptureUI(GlobalStatus status)
        {
            InitializeComponent();
            this.status = status;
            this.CaptureList.ItemsSource = status.CaptureList;
            var capture = new Capture();
            capture.GUID = "123";
            capture.name = "name";
            status.CaptureList.Add(capture);

        }
    }
}
