using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
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

        public API()
        {
            Logger.Debug("Init API");
        }

        #region login

        public async Task Login(string username, SecureString password, bool stayLoggedIn = true)
        {
            if (State.IsLoggedIn)
            {
                Logger.Warn("State reports it has been logged in.");
                return;
            }

            try
            {
                // show login window
                var loginRequest = await State.HTTPClient.PostAsync("login.php", new
                    {
                        username,
                        password,
                        machine_key = (string)ApplicationConfig["MachineGUID"],
                    }).AsResponse();
                var body = AsyncHelpers.RunSync(() => loginRequest.Message.Content.ReadAsStringAsync());
                Logger.Debug("HTTP {0} \nResponse header: {1}\nResponse body: {2}", loginRequest.Status, loginRequest.Message, body);

                // we need to save JSESSIONID cookie
                var loginCredential = loginRequest.Message.Headers.Where(x => x.Key == "Set-Cookie").ToList()[0].Value.Where(x => x.StartsWith("JS")).ToList()[0].Split(';')[0];
                Logger.Debug("Login credential: {0}", loginCredential);

                // flush key information in memory
                loginRequest = null;
                password.Clear();
                System.GC.Collect();

                if (!stayLoggedIn) return;
                Logger.Debug("Saving login status");
                State.IsLoggedIn = true;
                ApplicationConfig["cookie"] = loginCredential;
                ApplicationConfig.Save();
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions[0] is ApiException)
                {
                    Logger.Fatal("Login network error: {0}", e);
                    MessageBox.Show(e.InnerException?.Message ?? e.Message, "错误");
                }
            }

        }

        public void ClearLoginStatus()
        {
            Logger.Debug("Clearing login status");
            State.IsLoggedIn = false;
            State.Cookie = null;
            ApplicationConfig["LoginCredential"] = null;
            ApplicationConfig.Save();
        }

        public bool verifyCachedLoginCredential()
        {
            return false;
        }

        #endregion

    }
}
