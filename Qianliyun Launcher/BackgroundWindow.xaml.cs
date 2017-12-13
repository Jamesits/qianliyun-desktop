using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Qianliyun_Launcher.BroadcastCapture;

namespace Qianliyun_Launcher
{
    public class GlobalStatus
    {
        public BindingList<Capture> CaptureList;

        public GlobalStatus()
        {
            CaptureList = new BindingList<Capture>();
        }
    }
    /// <summary>
    /// Interaction logic for BackgroundWindow.xaml
    /// </summary>
    public partial class BackgroundWindow : Window
    {
        public GlobalStatus Status = new GlobalStatus();

        public BackgroundWindow()
        {
            InitializeComponent();
            this.Hide();

            // Initialize application

        }

    }
}
