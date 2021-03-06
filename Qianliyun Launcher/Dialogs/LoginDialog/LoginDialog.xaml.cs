﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace Qianliyun_Launcher.Dialogs.LoginDialog
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private StateManager State => StateManager.Instance;

        private readonly SemaphoreSlim _loginStartedSemaphore = new SemaphoreSlim(0, 1);

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public LoginDialog()
        {
            InitializeComponent();
        }

        // return true: login process finished
        // return false: user cancelled
        public async Task<bool> DoLogin()
        {
            bool flag;
            try
            {
                Show();
                await _loginStartedSemaphore.WaitAsync(_cts.Token);

                Logger.Debug("Login action Username {0} Password * Autologin {1}", Username.Text, Autologin.IsChecked ?? false);
                await State.api.Login(Username.Text, Password.SecurePassword,
                    Autologin.IsChecked ?? false);
                flag = true;
            }
            catch (OperationCanceledException)
            {
                Logger.Warn("User cancelled login");
                flag = false;
            }
            finally
            {
                // wipe memory
                Password.Clear();
                GC.Collect();
                Hide();
            }
            return flag;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _loginStartedSemaphore.Release();
            }
            catch (SemaphoreFullException)
            {
                Logger.Warn("Clicked too much times of login button");
            }
            
        }

        private void UserCancelling(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        private void LoginDialog_OnClosing(object sender, CancelEventArgs e)
        {
            UserCancelling(sender, null);
        }
    }
}
