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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Qianliyun_Launcher.LoadingPage;
using Qianliyun_Launcher.Homepage;

namespace Qianliyun_Launcher
{

    public partial class ImageButton : Button
    {

        [Description("The image displayed in the button if there is an Image control in the template " +
            "whose Source property is template-bound to the ImageButton Source property."), Category("Common Properties")]
        public ImageSource Source
        {
            get { return base.GetValue(SourceProperty) as ImageSource; }
            set { base.SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton));

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Homepage.Homepage homepage;
        private BroadcastCapture.BroadcastCaptureUI broadcast;

        private GlobalStatus status;
        private BackgroundWindow bgWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.bgWindow = new BackgroundWindow();
            this.status = this.bgWindow.Status;
            this.Page.Content = new Loading();

            // prepare controls
            homepage = new Homepage.Homepage();
            broadcast = new BroadcastCapture.BroadcastCaptureUI(status);
            
        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = homepage;
        }

        private void ButtonCapture_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void ButtonTagging_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void ButtonUpgrade_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void ButtonSMS_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void ButtonContact_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void ButtonData_OnClick(object sender, RoutedEventArgs e)
        {
            this.Page.Content = broadcast;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.bgWindow.Close();
        }
    }

}
