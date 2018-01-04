using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;
using NLog.Windows.Forms;

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
