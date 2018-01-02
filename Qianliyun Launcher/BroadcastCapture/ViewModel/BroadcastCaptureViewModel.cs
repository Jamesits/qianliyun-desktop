using System;
using System.Collections.ObjectModel;
using System.Windows;
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

        public bool HasInjectedJs { get; set; }

        private bool _isCapturing;

        public bool IsCapturing
        {
            get => _isCapturing;
            set
            {
                _isCapturing = value;
                RaisePropertyChanged();
            }
        }


        public BroadcastCaptureViewModel()
        {
            CaptureProperties = new Capture();
            ResultEntries = new ObservableCollection<CaptureResultEntry>();
            Logger.Debug("Initialized capture result storage");
        }

        public void Log(string message) => Logger.Debug(message);

        public void SetMetadata(string title, string user, string url)
        {
            Logger.Debug("SetMetadata {0}, {1}, {2}", title, user, url);
            CaptureProperties.Title = title;
            CaptureProperties.Broadcaster = user;
            CaptureProperties.URL = url;
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
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ResultEntries.Add(new CaptureResultEntry
                    {
                        Username = username,
                        UserAction = useraction,
                        Content = content,
                        Time = DateTime.Now,
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
            Messenger.Default.Send(new NotificationMessage("Saved."));
            Logger.Debug("Saved");
        }

        public void LoadEntries()
        {
            // load entries here
            RaisePropertyChanged(() => ResultEntries);
            Messenger.Default.Send(new NotificationMessage("Loaded."));
            Logger.Debug("Loaded");
        }
    }
}
