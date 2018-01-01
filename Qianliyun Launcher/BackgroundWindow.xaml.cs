using System;
using NLog;
using static Qianliyun_Launcher.PInvoke;
using static Qianliyun_Launcher.InteropUtil;

namespace Qianliyun_Launcher
{
    public class GlobalStatus
    {
    }
    /// <summary>
    /// Interaction logic for BackgroundWindow.xaml
    /// </summary>
    public partial class BackgroundWindow
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly WinEventDelegate ProcDelegate = WinEventProc;
        public GlobalStatus Status = new GlobalStatus();
        private readonly IntPtr _hhook;

        public BackgroundWindow()
        {
            Logger.Debug("BackgroundWindow initialization");
            InitializeComponent();
            Hide();

            // Initialize application
            //Logger.Debug("Setting IE Control compatibility mode");
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
            Logger.Info("Registering Chrome Accessibility hook");
            _hhook = SetWinEventHook(EVENT_SYSTEM_ALERT, EVENT_SYSTEM_ALERT, IntPtr.Zero,
                ProcDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

        }

        ~BackgroundWindow()
        {
            Logger.Info("Unregistering Chrome Accessibility hook");
            UnhookWinEvent(_hhook);
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject == 1)
            {
                // notify Chrome that Accessibility should be enabled
                Logger.Info("Received accessibility probe message, replying");
                SendMessage(hwnd, WM_GETOBJECT, 0, 1);
            }
        }

        public void EnableDebugMode()
        {
            Logger.Info("Enabling debug mode");
            CreateConsole();
        }

        public void DisableDebugMode()
        {
            Logger.Info("Disabling debug mode");
            FreeConsole();
        }
    }
}
