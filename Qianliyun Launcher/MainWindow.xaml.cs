using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using Qianliyun_Launcher.BroadcastCapture.View;
using Qianliyun_Launcher.LoadingPage;
using Qianliyun_Launcher.Properties;
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

        private Settings ApplicationConfig => Properties.Settings.Default;
        private StateManager State => StateManager.Instance;

        #region Tabs
        private UserControl _loadingPage;
        private UserControl _homepage;
        private UserControl _broadcast;
        private UserControl _tag;
        private UserControl _notImplPage;
        private UserControl _debugPanel;
        #endregion

        #region MVVM Data Bindings

        public bool IsDebugMode
        {
            get => State.IsDebugMode;
            set {
                State.IsDebugMode = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        public static RoutedCommand HotkeyRoutedCommand = new RoutedCommand();
        private int _debugModeToggleHitCount;
        private Timer _debugModeToggleHitCountResetTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            Logger.Debug("Property changed: {0}", propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            // register hotkey
            HotkeyRoutedCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            _debugModeToggleHitCountResetTimer = new Timer(x => { _debugModeToggleHitCount = 0; }, null, 0, 1000);

            // display loading progressbar
            _switchToTab<Loading>(ref _loadingPage);

            // prepare controls
            Logger.Debug("Preparing controls");

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
            // _bgWindow.Close();
        }

        private void ToggleDebugMode(object sender, ExecutedRoutedEventArgs e)
        {
            _debugModeToggleHitCount++;
            if (_debugModeToggleHitCount != 5) return;
            Logger.Info("Toggling debug mode");
            IsDebugMode = !IsDebugMode;
        }
    }

}
