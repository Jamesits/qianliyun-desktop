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
using System.Windows.Shapes;

namespace Qianliyun_Launcher
{
    /// <summary>
    /// Interaction logic for BackgroundWindow.xaml
    /// </summary>
    public partial class BackgroundWindow : Window
    {
        public BackgroundWindow()
        {
            InitializeComponent();
            this.Hide();

            // Initialize application

            // show main form
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
