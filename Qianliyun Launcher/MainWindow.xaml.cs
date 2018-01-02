using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using Qianliyun_Launcher.BroadcastCapture.View;
using Qianliyun_Launcher.LoadingPage;
using Qianliyun_Launcher.QianniuTag;

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
        private readonly GlobalStatus _status;

        #region Windows
        private readonly BackgroundWindow _bgWindow;
        #endregion

        #region Tabs
        private UserControl _homepage;
        private UserControl _broadcast;
        private UserControl _tag;
        private UserControl _notImplPage;
        private UserControl _debugPanel;
        #endregion

        #region MVVM Data Bindings
        public bool IsDebugMode => (bool)Properties.Settings.Default["debug"];
        #endregion
        
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
            _broadcast = new BroadcastCaptureUI();
            _tag = new QianniuTagUI();

            // Done, goto homepage
            Logger.Debug("Loading Homepage");
            _switchToTab<Homepage.Homepage>(ref _homepage);
        }

        private ref UserControl _switchToTab<T>(ref UserControl stor) where T : UserControl, new()
        {
            if (!(stor is T))
            {
                Logger.Debug("Creating new tab {0}", typeof(T).FullName);
                stor = new T();
            }
            Logger.Debug("Switching to tab {0}", typeof(T).FullName);
            Page.Content = stor;
            return ref stor;
        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<Homepage.Homepage>(ref _homepage);
        }

        private void ButtonCapture_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<BroadcastCaptureUI>(ref _broadcast);
        }

        private void ButtonTagging_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<QianniuTagUI>(ref _tag);
        }

        private void ButtonUpgrade_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<NotImplementdPage.NotImplementedPage>(ref _notImplPage);
        }

        private void ButtonSMS_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<NotImplementdPage.NotImplementedPage>(ref _notImplPage);
        }

        private void ButtonContact_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<NotImplementdPage.NotImplementedPage>(ref _notImplPage);
        }

        private void ButtonData_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<NotImplementdPage.NotImplementedPage>(ref _notImplPage);
        }

        private void ButtonDebug_OnClick(object sender, RoutedEventArgs e)
        {
            _switchToTab<DebugPanel.DebugPanel>(ref _debugPanel);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _bgWindow.Close();
        }
    }

}
