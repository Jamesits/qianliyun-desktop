﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
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

        // return: recoverable?
        private static async Task<bool> ExceeptionHandler(Exception e)
        {
            State.IsLoggedIn = false;
            while (!State.IsLoggedIn)
            {
                Logger.Info("Try login");
                try
                {
                    if (!await State._loginDialog.DoLogin()) break;
                }
                catch (ApiException ex)
                {
                    Logger.Fatal("Login failed: {0}", ex);
                    MessageBox.Show("登录失败：" + ex.Message, "登录失败");
                }
            }
            return false;
        }

        private static async Task PostApi(string api, object postBody = null)
        {
            try
            {
                if (postBody != null) await State.HTTPClient.PostAsync(api, postBody).AsString();
                else await State.HTTPClient.PostAsync(api).AsString();
            }
            catch (Exception e)
            {
                if (!await ExceeptionHandler(e)) throw;
            }
            
        }


        private static async Task<T> GetApiObject<T>(string api, string extractedObjName, object postBody = null) where T : class, new()
        {
            try
            {
                string retstr;
                if (postBody != null) retstr = await State.HTTPClient.PostAsync(api, postBody).AsString();
                else retstr = await State.HTTPClient.PostAsync(api).AsString();
                return JObject.Parse(retstr).GetValue(extractedObjName).ToObject<T>();
            }
            catch (Exception e)
            {
                if (!await ExceeptionHandler(e)) throw;
                return await(GetApiObject<T>(api, extractedObjName, postBody));
            }
        }

        private static async Task<List<T>> GetApiObjectList<T>(string api, string extractedObjName, object postBody = null) where T : class, new()
        {
            try
            {
                string retstr;
                if (postBody != null) retstr = await State.HTTPClient.PostAsync(api, postBody).AsString();
                else retstr = await State.HTTPClient.PostAsync(api).AsString();
                return JObject.Parse(retstr).GetValue(extractedObjName).Where(x => x != null).Select(x => x.ToObject<T>()).ToList();
            }
            catch (Exception e)
            {
                if (!await ExceeptionHandler(e)) throw;
                return await GetApiObjectList<T>(api, extractedObjName, postBody);
            }
        }
        #endregion

        private static async Task<string> TestApiEndpoint(string api, object postBody)
        {
            try
            {
                var retstr = await State.HTTPClient.PostAsync(api, postBody).AsString();
                Logger.Debug(retstr);
                return retstr;
            }
            catch (ApiException e)
            {
                Logger.Warn(e.Message);
            }
            return null;
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
                // ReSharper disable once RedundantAssignment
                loginRequest = null;
                GC.Collect();

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

        public async Task<bool> VerifyCachedLoginCredential()
        {
            if (State.LoginCredential == null || State.LoginCredential.Length <= 0) return false;
            try
            {
                await State.HTTPClient.PostAsync("query_user_info.php").WithHeader("Cookie", State.LoginCredential).AsString();
                return true;
            }
            catch
            {
                return false;
            }
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

        #region TaobaoUser
        #endregion

        #region BroadcastCaptureSession

        // captured live session list
        public async Task QueryLiveSessions()
        {
            Logger.Debug("QueryLiveSessions");
            // Avoid replacing the whole object!
            Util.ReplaceBindingList(
                State.LiveSessions, 
                await GetApiObjectList<LiveSession>("query_live_session.php", "live_session", new LiveSession())
                );
        }

        // get a live session
        public async Task QueryLiveSession(LiveSession s)
        {
            await GetApiObjectList<LiveSession>("query_live_session.php", "live_session", s);
        }

        public async Task UpdateLiveSession(LiveSession s)
        {
            Logger.Debug("UpdateLiveSession");
            await PostApi("update_live_session.php", s);
        }
        #endregion
    }
}
