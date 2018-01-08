using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using Pathoschild.Http.Client;

namespace Qianliyun_Launcher.API
{
    public class API
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static StateManager State => StateManager.Instance;

        public API()
        {
            Logger.Debug("Init API");
        }

        #region wrapper

        private static async Task<T> GetApiObject<T>(string api, string extractedObjName) where T: class, new()
        {
            var retstr = await State.HTTPClient.PostAsync(api).AsString();
            dynamic retobj = JsonConvert.DeserializeObject<ExpandoObject>(retstr, new ExpandoObjectConverter());
            var ret = new T();
            Mapper<T>.Map((ExpandoObject)((IDictionary<string, object>)retobj)[extractedObjName], ret);
            return ret;
        }
        #endregion

        #region login

        public async Task Login(string username, SecureString password, bool stayLoggedIn = false)
        {
            if (State.IsLoggedIn)
            {
                Logger.Warn("State reports it has been logged in.");
                return;
            }

            var c = new NetworkCredential(username, password);

            try
            {
                // show login window
                var loginRequest = await State.HTTPClient.PostAsync("login.php", new
                {
                    username = c.UserName,
                    password = c.Password,
                    machine_key = State.MachineKey,
                });

                var body = await loginRequest.Message.Content.ReadAsStringAsync();
                Logger.Debug("HTTP {0} \nResponse header: {1}\nResponse body: {2}", loginRequest.Status,
                    loginRequest.Message, body);

                // we need to save JSESSIONID cookie
                var loginCredential = loginRequest.Message.Headers.Where(x => x.Key == "Set-Cookie").ToList()[0].Value
                    .Where(x => x.StartsWith("JS")).ToList()[0].Split(';')[0];
                Logger.Debug("Login credential: {0}", loginCredential);

                // flush key information in memory
                Logger.Debug("Flushing memory (1st stage)");
                // ReSharper disable once RedundantAssignment
                loginRequest = null;
                System.GC.Collect();

                Logger.Debug("Saving login status");
                State.IsLoggedIn = true;
                State.SaveLoginStatus = stayLoggedIn;
                State.LoginCredential = loginCredential;
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions[0] is ApiException)
                {
                    Logger.Fatal("Login network error: {0}", e);
                    // MessageBox.Show(e.InnerException?.Message ?? e.Message, "错误");
                }
                throw;
            }
            finally
            {
                Logger.Debug("Flushing memory (2nd stage)");
                // ReSharper disable once RedundantAssignment
                c = null;
                password.Clear();
                // ReSharper disable once RedundantAssignment
                password = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }

        public void ClearLoginStatus()
        {
            Logger.Debug("Clearing login status");
            State.IsLoggedIn = false;
            State.LoginCredential = null;
        }

        public bool VerifyCachedLoginCredential()
        {
            return true;
        }

        #endregion

        // related to subscription, customization and limitation
        #region Account

        public async Task PopulateAccountInformation()
        {
            Logger.Debug("PopulateAccountInformation");
            State.UserInfo = await GetApiObject<UserInfo>("query_user_info.php", "user_info");
        }

        #endregion

    }
}
