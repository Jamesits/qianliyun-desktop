using System;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using NLog;

namespace Qianliyun_Launcher.BroadcastCapture
{
    /// <summary>
    /// Interaction logic for BroadcastCaptureUI.xaml
    /// </summary>
    public partial class BroadcastCaptureUI : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private GlobalStatus status;
        private CaptureResultStorage result;

        private const string captureJs = @"
            if (window.location.host == ""taobaolive.taobao.com"") {
                // PC version
                var targetNode = document.getElementsByClassName('comment-inner')[0];
                var config = { attributes: false, childList: true };
                var callback = function(mutationsList) {
                    for(var mutation of mutationsList) {
                        if (mutation.type == 'childList') {
                            for (var node of mutation.addedNodes) {
                                name = node.innerText.split("" "", 2)[0];
                                action = node.innerText.split("" "", 2)[1];
                                capture.addEntry(name, action);
                            }
                        }
                    }
                };
            } else if (window.location.host == ""hudong.m.taobao.com"") {
                // Mobile version
                var targetNode = document.getElementById('J_Msg_List');
                var config = { attributes: false, childList: true };
                var callback = function(mutationsList) {
                    for(var mutation of mutationsList) {
                        if (mutation.type == 'childList') {
                            for (var node of mutation.addedNodes) {
                                name = node.innerText.split("" "", 2)[0];
                                action = node.innerText.split("" "", 2)[1];
                                capture.addEntry(name, action);
                            }
                        }
                    }
                };
            }

            var observer = new MutationObserver(callback);
            observer.observe(targetNode, config);
        ";

        private const string stopCaptureJs = @"
            observer.disconnect();
        ";

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

        public BroadcastCaptureUI(GlobalStatus status)
        {
            InitializeComponent();
            this.status = status;
            result = new CaptureResultStorage("test", "testname", "testurl");
            // this.CaptureBrowser.ShowDevTools();
            CaptureBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            CaptureBrowser.FrameLoadStart += CaptureBrowser_FrameLoadStart;
            CaptureBrowser.LoadingStateChanged += CaptureBrowserOnLoadingStateChanged;
            CaptureBrowser.RegisterAsyncJsObject("capture", result, BindingOptions.DefaultBinder);
            //Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
            CaptureBrowser.LoadingStateChanged += (sender, args) =>
            {
                //Wait for the Page to finish loading
                if (args.IsLoading == false)
                {
                    Logger.Debug("Page loading finished");
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
        }

        private void CaptureBrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            
        }

        private void CaptureBrowser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Logger.Debug("URL changed to {0}", e.Url);
                AddressBar.Text = e.Url;
            });
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
            Logger.Debug("Start capturing");
            CaptureBrowser.ExecuteScriptAsync(captureJs);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("Stop capturing");
            CaptureBrowser.ExecuteScriptAsync(stopCaptureJs);
        }
    }
}
