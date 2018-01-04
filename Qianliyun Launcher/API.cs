﻿using System;
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

        private static StateManager State => StateManager.Instance;

        public API()
        {
            Logger.Debug("Init API");
        }

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
                loginRequest = null;
                System.GC.Collect();

                Logger.Debug("Saving login status");
                State.IsLoggedIn = true;
                State.SaveLoginStatus = stayLoggedIn;
                State.Cookie = loginCredential;
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
                c = null;
                password.Clear();
                password = null;
                System.GC.Collect();
            }

        }

        public void ClearLoginStatus()
        {
            Logger.Debug("Clearing login status");
            State.IsLoggedIn = false;
            State.Cookie = null;
        }

        public bool verifyCachedLoginCredential()
        {
            return true;
        }

        #endregion

    }
}
