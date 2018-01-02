using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private static readonly StateManager _instance = new StateManager();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GlobalStatus _status;

        private Settings ApplicationConfig => Properties.Settings.Default;

        private readonly IClient HTTPClient;

        #region states

        public bool IsLoggedIn { get; set; }
        #endregion

        /// <summary>
        /// Initialises a new empty StateManager object.
        /// </summary>
        private StateManager()
        {
            HTTPClient = new FluentClient((string)ApplicationConfig["APIBaseURL"]);
        }

        /// <summary>
        /// Gets the single available instance of the application StateManager object.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public static StateManager Instance => _instance;

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

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => throw new NotImplementedException();

            remove => throw new NotImplementedException();
        }
    }
}
