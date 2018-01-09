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
#pragma warning disable 4014
            State.api.QueryLiveSessions();
#pragma warning restore 4014
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoTag("朱导来了", "测试");
            
        }

        private static void DoTag(string username, string tag)
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

        private async void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            await State.api.QueryLiveSessions();
        }
    }
}
