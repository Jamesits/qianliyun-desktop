using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NLog;
using Qianliyun_Launcher.BroadcastCapture;
using static Qianliyun_Launcher.PInvoke;
using static Qianliyun_Launcher.InteropUtil;

namespace Qianliyun_Launcher
{
    public class GlobalStatus
    {
        public BindingList<Capture> CaptureList;

        public GlobalStatus()
        {
            CaptureList = new BindingList<Capture>();
        }
    }
    /// <summary>
    /// Interaction logic for BackgroundWindow.xaml
    /// </summary>
    public partial class BackgroundWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static PInvoke.WinEventDelegate procDelegate = new PInvoke.WinEventDelegate(WinEventProc);
        public GlobalStatus Status = new GlobalStatus();
        private IntPtr hhook;

        public BackgroundWindow()
        {
            logger.Debug("BackgroundWindow initialization");
            InitializeComponent();
            this.Hide();

            // Initialize application
            //logger.Debug("Setting IE Control compatibility mode");
            //var pricipal = new System.Security.Principal.WindowsPrincipal(
            //    System.Security.Principal.WindowsIdentity.GetCurrent());
            //if (pricipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            //{
            //    RegistryKey registrybrowser = Registry.LocalMachine.OpenSubKey
            //        (@"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
            //    string myProgramName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //    var currentValue = registrybrowser.GetValue(myProgramName);
            //    if (currentValue == null || (int)currentValue != 0x00002af9)
            //        registrybrowser.SetValue(myProgramName, 0x00002af9, RegistryValueKind.DWord);
            //}

            // register accessibility hook
            logger.Info("Registering Chrome Accessibility hook");
            hhook = SetWinEventHook(EVENT_SYSTEM_ALERT, EVENT_SYSTEM_ALERT, IntPtr.Zero,
                procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

        }

        ~BackgroundWindow()
        {
            logger.Info("Unregistering Chrome Accessibility hook");
            UnhookWinEvent(hhook);
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject == 1)
            {
                // notify Chrome that Accessibility should be enabled
                logger.Info("Received accessibility probe message, replying");
                SendMessage(hwnd, WM_GETOBJECT, 0, 0);
            }
        }

        public void EnableDebugMode()
        {
            logger.Info("Enabling debug mode");
            CreateConsole();
        }

        public void DisableDebugMode()
        {
            logger.Info("Disabling debug mode");
            FreeConsole();
        }
    }
}
