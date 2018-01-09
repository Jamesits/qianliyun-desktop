using System;
using System.Collections.ObjectModel;
using System.Windows;
using NLog;

namespace Qianliyun_Launcher.LogWindow
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public ObservableCollection<LogEntry> LogEntries { get; set; }

        private static LogWindow _logWindow;
        private int _index;

        public LogWindow()
        {
            Logger.Debug("Initializing logWindow");
            _logWindow = this;
            InitializeComponent();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
        }

        public static void AddEntry(string loglevel, string module, string message)
        {
            _logWindow?.Dispatcher.BeginInvoke((Action) (() => _logWindow?.LogEntries.Insert(0, new LogEntry()
                {
                    Index = _logWindow._index++,
                    DateTime = DateTime.Now,
                    Message = $"{loglevel,-5} | {module,-20}: {message}"
                })
            ));
        }
    }
}
