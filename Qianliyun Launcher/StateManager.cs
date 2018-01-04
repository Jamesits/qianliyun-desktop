using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Pathoschild.Http.Client;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher
{
    // singleton used to stor temporary (per execution) state
    public class StateManager : INotifyPropertyChanged
    {
        // put this on the front
        // since we are gonna use it the next line
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the single available instance of the application StateManager object.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public static StateManager Instance => _instance;
        private static readonly StateManager _instance = new StateManager();
        
        private readonly GlobalStatus _status;

        private Settings ApplicationConfig => Properties.Settings.Default;

        #region global objects
        private IClient HTTPClient;
        #endregion

        #region states

        private bool _isDebugMode;
        public bool IsDebugMode
        {
            get => _isDebugMode;
            set {
                _isDebugMode = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsLoggedIn { get; set; }
        #endregion

        #region windows
        public BackgroundWindow _bgWindow;
        public MainWindow _mainWindow;
        public LogWindow.LogWindow _logWindow;
        #endregion

        /// <summary>
        /// Initialises a new empty StateManager object.
        /// </summary>
        private StateManager()
        {
            Logger.Debug("Creating StateManager");

            // set initial states
            IsDebugMode = (bool)ApplicationConfig["debug"];
            // init global objects
            HTTPClient = new FluentClient((string)ApplicationConfig["APIBaseURL"]);

        }

        public void Login(string username, string password, bool stayLoggedIn=true)
        {
            if ((bool)ApplicationConfig["IsLogined"])
            {
                Logger.Debug("Configuration reports it has been logged in. Connecting using existing credential...");
                var loginResult = HTTPClient.PostAsync("login.php").WithArguments(new 
                {
                    username = username,
                    password = password,
                    machine_key = (string)ApplicationConfig["MachineGUID"],
                }).AsResponse();
                Logger.Debug("HTTP {0} {1}", loginResult.Status, loginResult.Result);
            }

            if (!stayLoggedIn) return;
            Logger.Debug("Saving login status");
            ApplicationConfig["IsLogined"] = true;
            ApplicationConfig.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            Logger.Debug("Property changed: {0}", propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
