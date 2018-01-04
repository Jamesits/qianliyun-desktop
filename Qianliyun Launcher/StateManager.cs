using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Pathoschild.Http.Client;
using Qianliyun_Launcher.Dialogs.LoginDialog;
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
        public IClient HTTPClient;
        public API.API api;
        #endregion

        #region System Info

        public string AssemblyName => Assembly.GetEntryAssembly().GetName().Name;
        public string AppName => Assembly.GetEntryAssembly().FullName;
        public Version AppVersion =>  Assembly.GetEntryAssembly().GetName().Version;
        public string AppVersionString => String.Format("{0}.{1}.{2} build {3}", AppVersion.Major, AppVersion.Minor, AppVersion.Revision, AppVersion.Build);

        #endregion

        #region states

        public string APIBaseURL => (string) ApplicationConfig["APIBaseURL"];

        private bool? _isDebugMode;
        public bool IsDebugMode
        {
            get => _isDebugMode?? (bool)ApplicationConfig["debug"];
            set {
                _isDebugMode = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLoggedIn { get; set; }

        public bool SaveLoginStatus
        {
            get => (bool)ApplicationConfig["SaveLoginStatus"];
            set
            {
                ApplicationConfig["SaveLoginStatus"] = value;
                ApplicationConfig.Save();
                NotifyPropertyChanged();
            }
        }

        private string _loginCredential;

        public string LoginCredential
        {
            get => _loginCredential ?? (string)ApplicationConfig["LoginCredential"];
            set
            {
                _loginCredential = value;
                if (SaveLoginStatus || value == null)
                {
                    ApplicationConfig["LoginCredential"] = value;
                    ApplicationConfig.Save();
                }
                NotifyPropertyChanged();
            }
        }

        public string MachineKey
        {
            get {
                // check if machine GUID has been generated
                if (((string)ApplicationConfig["MachineGUID"]).Length < 1)
                {
                    ApplicationConfig["MachineGUID"] = Guid.NewGuid().ToString();
                    ApplicationConfig.Save();
                    Logger.Debug("Generated new MachineGUID {0}", (string)ApplicationConfig["MachineGUID"]);
                }
                return (string)ApplicationConfig["MachineGUID"];
            }
        }
        #endregion

        #region windows
        public BackgroundWindow _bgWindow;
        public MainWindow _mainWindow;
        public LogWindow.LogWindow _logWindow;
        public LoginDialog _loginDialog;
        #endregion

        /// <summary>
        /// Initialises a new empty StateManager object.
        /// </summary>
        private StateManager()
        {
            Logger.Debug("Creating StateManager");

            // set initial states
            
            // init global objects
            HTTPClient = new FluentClient(APIBaseURL);
            api = new API.API();

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
