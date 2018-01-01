using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
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

    public class CaptureResultStorage
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Capture CaptureProperties;
        public ObservableCollection<CaptureResultEntry> ResultEntries;

        public CaptureResultStorage(string guid, string name, string url)
        {
            CaptureProperties = new Capture { GUID = guid, name = name, URL = url};
            ResultEntries = new ObservableCollection<CaptureResultEntry>();
            logger.Debug("Initialized capture result storage for GUID {0}, name {1}", guid, name);
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
            logger.Debug("New record: {0} {1}", username, useraction);
        }
    }
}
