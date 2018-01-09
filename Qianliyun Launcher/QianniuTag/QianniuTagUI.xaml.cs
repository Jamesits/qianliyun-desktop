using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using NLog;
using Qianliyun_Launcher.API;

namespace Qianliyun_Launcher.QianniuTag
{
    /// <summary>
    /// Interaction logic for QianniuTagUI.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class QianniuTagUI : INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static StateManager State => StateManager.Instance;
        public event PropertyChangedEventHandler PropertyChanged;

        #region databindings
        public BindingList<LiveSession> LiveSessions => State.LiveSessions;

        #endregion

        public QianniuTagUI()
        {
            InitializeComponent();
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
            throw new NotImplementedException();
        }
    }
}
