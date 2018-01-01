using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using Qianliyun_Launcher.BroadcastCapture.Model;

namespace Qianliyun_Launcher.BroadcastCapture.ViewModel
{
    public class BroadcastCaptureViewModel: ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Capture CaptureProperties;
        public ObservableCollection<CaptureResultEntry> ResultEntries;

        private bool _hasInjectedJs;

        public bool HasInjectedJs
        {
            get => _hasInjectedJs;
            set
            {
                _hasInjectedJs = value;
                
            }
        }

        private bool _isCapturing = false;

        public bool IsCapturing
        {
            get => _isCapturing;
            set
            {
                _isCapturing = value;
                RaisePropertyChanged("IsCapturing");
                RaisePropertyChanged("CaptureToggleButtonText");
            }
        }

        public string CaptureToggleButtonText => IsCapturing ? "停止采集" : "开始采集";

        public BroadcastCaptureViewModel(string guid, string name, string url)
        {
            CaptureProperties = new Capture { GUID = guid, name = name, URL = url };
            ResultEntries = new ObservableCollection<CaptureResultEntry>();
            Logger.Debug("Initialized capture result storage for GUID {0}, name {1}", guid, name);
        }

        public void Log(string message) => Logger.Debug(message);

        public void SetMetadata(string title, string user, string url)
        {
            Logger.Debug("SetMetadata {0}, {1}, {2}", title, user, url);
        }

        public void AddEntry(string username, string useraction)
        {
            if (IsCapturing)
            {
                var content = "";
                if (username.EndsWith(":"))
                {
                    username = username.TrimEnd(':');
                    content = useraction;
                    useraction = "发言";
                }
                App.Current.Dispatcher.Invoke((Action) delegate
                {
                    ResultEntries.Add(new CaptureResultEntry
                    {
                        Username = username,
                        UserAction = useraction,
                        Content = content,
                        Time = DateTime.Now
                    });
                });
                Logger.Debug("New record: {0} {1}", username, useraction);
            }
            else
            {
                Logger.Debug("Discarded new record: {0} {1}", username, useraction);
            }
            
        }

        public void SaveEntries()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Saved."));
            Logger.Debug("Saved");
        }

        public void LoadEntries()
        {
            // load entries here
            RaisePropertyChanged(() => ResultEntries);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Loaded."));
            Logger.Debug("Loaded");
        }
    }
}
