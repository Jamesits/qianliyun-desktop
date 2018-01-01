using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Qianliyun_Launcher.BroadcastCapture
{
    public class Capture
    {
        public String GUID;
        public String name;
        public String URL;
    }

    public class CaptureResultEntry: ObservableObject
    {
        private string _username;
        private string _userAction;
        private string _content;
        private DateTime _time;
        public string Username { get => _username;
            set { Set(() => Username, ref _username, value); } }
        public string UserAction { get => _username;
            set
            {
                Set(() => UserAction, ref _userAction, value);
            } }
        public string Content { get => _content;
            set { Set(() => Content, ref _content, value); } }
        public DateTime Time { get => _time;
            set { Set(() => Time, ref _time, value); } }
    }

    public class CaptureResultStorage: ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Capture CaptureProperties;
        public ObservableCollection<CaptureResultEntry> ResultEntries;

        public CaptureResultStorage(string guid, string name, string url)
        {
            CaptureProperties = new Capture { GUID = guid, name = name, URL = url};
            ResultEntries = new ObservableCollection<CaptureResultEntry>();
            Logger.Debug("Initialized capture result storage for GUID {0}, name {1}", guid, name);
        }

        public void addEntry(string username, string useraction)
        {
            var content = "";
            if (username.EndsWith(":"))
            {
                username = username.TrimEnd(':');
                content = useraction;
                useraction = "发言";
            }
            ResultEntries.Add(new CaptureResultEntry {Username = username, UserAction = useraction, Content = content, Time = DateTime.Now});
            Logger.Debug("New record: {0} {1}", username, useraction);
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

    public class CaptureResultViewModelLocator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public CaptureResultViewModelLocator()
        {
            Logger.Debug("Initialized");
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<CaptureResultStorage>();
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);
        }

        public CaptureResultStorage CaptureResultStorage => ServiceLocator.Current.GetInstance<CaptureResultStorage>();

        private void NotifyUserMethod(NotificationMessage message)
        {
            Logger.Debug("Notify user with message {0}", message.Notification);
            MessageBox.Show(message.Notification);
        }
    }
}
