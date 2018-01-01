using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using Qianliyun_Launcher.BroadcastCapture;
using Qianliyun_Launcher.LoadingPage;
using Qianliyun_Launcher.QianniuTag;
using BroadcastCaptureUI = Qianliyun_Launcher.BroadcastCapture.View.BroadcastCaptureUI;

namespace Qianliyun_Launcher
{

    public class ImageButton : Button
    {

        [Description("The image displayed in the button if there is an Image control in the template " +
            "whose Source property is template-bound to the ImageButton Source property."), Category("Common Properties")]
        public ImageSource Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton));

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Homepage.Homepage _homepage;
        private readonly BroadcastCaptureUI _broadcast;

        private readonly GlobalStatus _status;
        private readonly BackgroundWindow _bgWindow;
        private readonly QianniuTagUI _tag;

        public MainWindow()
        {
            InitializeComponent();
            Logger.Debug("Loading background window");
            _bgWindow = new BackgroundWindow();
            //this.bgWindow.EnableDebugMode();
            _status = _bgWindow.Status;
            Page.Content = new Loading();

            // prepare controls
            Logger.Debug("Preparing controls");
            _homepage = new Homepage.Homepage();
            _broadcast = new BroadcastCaptureUI(_status);
            _tag = new QianniuTagUI();

            // Done, goto homepage
            Logger.Debug("Loading Homepage");
            Page.Content = _homepage;

        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _homepage;
        }

        private void ButtonCapture_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _broadcast;
        }

        private void ButtonTagging_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _tag;
        }

        private void ButtonUpgrade_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _broadcast;
        }

        private void ButtonSMS_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _broadcast;
        }

        private void ButtonContact_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _broadcast;
        }

        private void ButtonData_OnClick(object sender, RoutedEventArgs e)
        {
            Page.Content = _broadcast;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _bgWindow.Close();
        }
    }

}
