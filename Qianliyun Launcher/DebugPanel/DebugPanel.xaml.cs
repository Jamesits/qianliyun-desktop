﻿using System;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using NLog;

namespace Qianliyun_Launcher.DebugPanel
{
    /// <summary>
    /// Interaction logic for DebugPanel.xaml
    /// </summary>
    public partial class DebugPanel : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private StateManager State => StateManager.Instance;

        public DebugPanel()
        {
            InitializeComponent();
            Browser.FrameLoadStart += (sender, e) =>
            {
                Logger.Debug("Loading new frame {0}", e.Url);
            };
            Browser.FrameLoadEnd += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Logger.Debug("URL changed to {0}", Browser.Address);
                    AddressBar.Text = Browser.Address;
                });
            };
        }

        private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Emulating login");
            await State._loginDialog.DoLogin();
        }

        private void BtnClearLoginStatus_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Force clear login status");
            State.api.ClearLoginStatus();
        }

        private void ButtonLogWindow_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Toggling LogWindow visibility");
            if (State._logWindow.IsVisible)
                State._logWindow.Hide();
            else
                State._logWindow.Show();
        }

        private void BtnBrowserGoto_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Debug("Trying to load {0}", AddressBar.Text);
                Browser.Load(AddressBar.Text);
            }
            catch (UriFormatException ex)
            {
                Logger.Fatal(ex);
                throw;
            }
        }

        private void BtnBrowserDevTool_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Enable DevTools");
            Browser.ShowDevTools();
        }

        private void BtnToggleDebugMode_OnClick(object sender, RoutedEventArgs e)
        {
            State.IsDebugMode = !State.IsDebugMode;
        }

        private async void BtnPopulateAccountInformation_OnClick(object sender, RoutedEventArgs e)
        {
            await State.api.PopulateAccountInformation();
        }

        private async void BtnQueryLiveSessions_OnClick(object sender, RoutedEventArgs e)
        {
            await State.api.QueryLiveSessions();
        }
    }
}
