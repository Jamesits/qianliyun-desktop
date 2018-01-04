using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Pathoschild.Http.Client;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher
{
    public class API
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Settings ApplicationConfig => Properties.Settings.Default;
        private static StateManager State => StateManager.Instance;

        public void Login(string username, string password, bool stayLoggedIn = true)
        {
            if ((bool) ApplicationConfig["IsLogined"])
            {
                Logger.Debug("Configuration reports it has been logged in. Connecting using existing credential...");
                
            }

                // show login window
                var loginResult = State.HTTPClient.PostAsync("login.php").WithArguments(new
                {
                    username = username,
                    password = password,
                    machine_key = (string)ApplicationConfig["MachineGUID"],
                }).AsResponse();
            try
            {
                Logger.Debug("HTTP {0} {1}", loginResult.Status, loginResult.Result);
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions[0] is ApiException)
                {
                    Logger.Fatal("Login network error: {0}", e);
                    MessageBox.Show(e.InnerException?.Message ?? e.Message, "错误");
                }
            }
                
            

            if (!stayLoggedIn) return;
            Logger.Debug("Saving login status");
            ApplicationConfig["IsLogined"] = true;
            ApplicationConfig.Save();
        }

        public void ClearLoginStatus()
        {
            Logger.Debug("Clearing login status");
            ApplicationConfig["IsLogined"] = false;
            ApplicationConfig.Save();
        }
    }
}
