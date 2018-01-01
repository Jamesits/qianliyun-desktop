using System;
using GalaSoft.MvvmLight;

namespace Qianliyun_Launcher.BroadcastCapture.Model
{
    public class Capture
    {
        // ReSharper disable once InconsistentNaming
        public string GUID { get; set; }
        public string Name { get; set; }
        // ReSharper disable once InconsistentNaming
        public string URL { get; set; }
    }

    public class CaptureResultEntry : ObservableObject
    {
        private string _username;
        private string _userAction;
        private string _content;
        private DateTime _time;
        public string Username
        {
            get => _username;
            set { Set(() => Username, ref _username, value); }
        }
        public string UserAction
        {
            get => _userAction;
            set
            {
                Set(() => UserAction, ref _userAction, value);
            }
        }
        public string Content
        {
            get => _content;
            set { Set(() => Content, ref _content, value); }
        }
        public DateTime Time
        {
            get => _time;
            set { Set(() => Time, ref _time, value); }
        }
    }
}
