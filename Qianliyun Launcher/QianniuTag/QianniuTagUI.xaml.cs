using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NLog;
using Qianliyun_Launcher.Annotations;
using Qianliyun_Launcher.API;

namespace Qianliyun_Launcher.QianniuTag
{
    /// <summary>
    /// Interaction logic for QianniuTagUI.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class QianniuTagUI: INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static StateManager State => StateManager.Instance;

        #region databindings
        public BindingList<LiveSession> LiveSessions => State.LiveSessions;

        #endregion

        public QianniuTagUI()
        {
            InitializeComponent();
            PropertyChanged += (state, e) =>
            {
                Logger.Debug("Received PropertyChanged {0}", e.PropertyName);
            };
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            doTag("朱导来了", "测试");
            
        }

        private void doTag(String username, String tag)
        {
            QianniuTagCore.DoTag(username, tag);
        }

        private void BtnTag_OnClick(object sender, RoutedEventArgs e)
        {
            CaptureResultDataGrid.ItemsSource = LiveSessions;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Logger.Debug("PropertyChanged {0}", propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
