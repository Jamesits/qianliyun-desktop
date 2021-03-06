﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CefSharp;
using NLog;
using Qianliyun_Launcher.BroadcastCapture.Model;
using Qianliyun_Launcher.BroadcastCapture.ViewModel;

namespace Qianliyun_Launcher.BroadcastCapture.View
{
    /// <summary>
    /// Interaction logic for BroadcastCaptureUI.xaml
    /// </summary>
    public partial class BroadcastCaptureUI : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public BroadcastCaptureViewModel VM { get; set; }
        public BindingList<CaptureResultEntry> CaptureResults => VM.ResultEntries;
        public bool IsDebugMode => (bool)Properties.Settings.Default["debug"];

        public bool IsCapturing
        {
            get => VM.IsCapturing;
            set
            {
                VM.IsCapturing = value;
                //CaptureBrowser.ExecuteScriptAsync(onReadyJs);
                Logger.Debug("{0} capture", value ? "enabling" : "disabling");
                //CaptureBrowser.ExecuteScriptAsync(captureJs);
            } 
        }

        #region JavaScript
        private const string OnReadyJs = @"
            function ready(fn) {
              if (document.attachEvent ? document.readyState === ""complete"" : document.readyState !== ""loading""){
                fn();
              } else {
                document.addEventListener('DOMContentLoaded', fn);
              }
            }

            function retryForever(fn) {
              return fn().catch(function(err) {
                return retryForever(fn); 
              });
            }
        ";

        private const string GetMetadataJs = @"
            CaptureBridge.log(""Starting getMetadataJs"");
            var title = ""未知"";
            var user = ""未知"";
            if (window.location.host == ""taobaolive.taobao.com"") {
                // PC version
                title = document.getElementsByClassName(""lr-header-title"")[0].innerText;
                user = document.getElementsByClassName(""lr-anchor-nick"")[0].innerText;
            } else if (window.location.host == ""hudong.m.taobao.com"") {
                // Mobile version
                // we cannot get title; use roomid instead
                title = document.getElementsByClassName(""room-no"")[0].innerText.split("": "")[1];
                user = document.getElementsByClassName(""anchor-info"")[0].childNodes[1].innerText;
            }
            CaptureBridge.setMetadata(title, user, window.location.href);
        ";
        
        private const string CaptureJs = @"
            CaptureBridge.log(""Starting captureJs"");
            if (window.alreadyObserving != 1) {
                if (window.location.host == ""taobaolive.taobao.com"") {
                    var targetNode = document.getElementsByClassName('comment-inner')[0];
                    var config = { attributes: false, childList: true };
                    var callback = function(mutationsList) {
                        for(var mutation of mutationsList) {
                            if (mutation.type == 'childList') {
                                for (var node of mutation.addedNodes) {
                                    name = node.innerText.split("" "", 2)[0];
                                    action = node.innerText.split("" "", 2)[1];
                                    CaptureBridge.addEntry(name, action);
                                }
                            }
                        }
                    };
                } else if (window.location.host == ""hudong.m.taobao.com"") {
                    var targetNode = document.getElementById('J_Msg_List');
                    var config = { attributes: false, childList: true };
                    var callback = function(mutationsList) {
                        for(var mutation of mutationsList) {
                            if (mutation.type == 'childList') {
                                for (var node of mutation.addedNodes) {
                                    name = node.innerText.split("" "", 2)[0];
                                    action = node.innerText.split("" "", 2)[1];
                                    CaptureBridge.addEntry(name, action);
                                }
                            }
                        }
                    };
                }
                var observer = new MutationObserver(callback);
                observer.observe(targetNode, config);
                window.alreadyObserving = 1;
            } else CaptureBridge.log(""captureJS is executed twice, quitting"");
        ";

        private const string StopCaptureJs = @"
            observer.disconnect();
        ";
        #endregion

        public class RenderProcessMessageHandler : IRenderProcessMessageHandler
        {
            public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
                Logger.Debug("Javascript context released");
            }

            public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
            {
                Logger.Debug("OnFocusedNodeChanged");
            }

            // Wait for the underlying JavaScript Context to be created. This is only called for the main frame.
            // If the page has no JavaScript, no context will be created.
            void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
                //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
                //frame.ExecuteJavaScriptAsync(script);
                Logger.Debug("Javascript context created");
            }
        }

        public BroadcastCaptureUI()
        {
            VM = new BroadcastCaptureViewModel();
            InitializeComponent();
            CaptureBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            CaptureBrowser.RegisterAsyncJsObject("CaptureBridge", VM, BindingOptions.DefaultBinder);
            CaptureBrowser.FrameLoadStart += (sender, e) =>
            {
                Logger.Debug("Loading new frame {0}", e.Url);
            };
            CaptureBrowser.FrameLoadEnd += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Logger.Debug("URL changed to {0}", CaptureBrowser.Address);
                    AddressBar.Text = CaptureBrowser.Address;
                });
            };
            //Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
            CaptureBrowser.LoadingStateChanged += (sender, args) =>
            {
                //Wait for the Page to finish loading
                if (args.IsLoading == false)
                {
                    Logger.Debug("Page loading finished");
                    if (CaptureBrowser.CanExecuteJavascriptInMainFrame)
                    {
                        Logger.Debug("OK to inject JS");
                        //CaptureBrowser.ExecuteScriptAsync(onReadyJs);
                        CaptureBrowser.ExecuteScriptAsync(GetMetadataJs);
                        CaptureBrowser.ExecuteScriptAsync(CaptureJs);
                    }
                    else
                    {
                        Logger.Warn("Cannot inject JS now");
                    }
                        
                }
            };

            //Wait for the MainFrame to finish loading
            CaptureBrowser.FrameLoadEnd += (sender, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                {
                    // args.Frame.ExecuteJavaScriptAsync("alert('MainFrame finished loading');");
                    Logger.Debug("MainFrame finished loading");
                }
            };

            // auto scroll
            CaptureResults.ListChanged += (sender, e) =>
            {
                if (CaptureResultDataGrid.Items.Count > 0)
                {
                    var border = VisualTreeHelper.GetChild(CaptureResultDataGrid, 0) as Decorator;
                    var scroll = border?.Child as ScrollViewer;
                    scroll?.ScrollToEnd();
                }
            };
        }

        private void BtnGoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Debug("Trying to load {0}", AddressBar.Text);
                CaptureBrowser.Load(AddressBar.Text);
            }
            catch (UriFormatException ex)
            {
                Logger.Fatal(ex);
                throw;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug(IsCapturing ? "Start capturing" : "Stop capturing");
        }

        //private void BtnStop_Click(object sender, RoutedEventArgs e)
        //{
        //    Logger.Debug("Stop capturing");
        //    CaptureBrowser.ExecuteScriptAsync(stopCaptureJs);
        //}

        private void BtnDevtool_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Enable DevTools");
            CaptureBrowser.ShowDevTools();
        }
    }
}
