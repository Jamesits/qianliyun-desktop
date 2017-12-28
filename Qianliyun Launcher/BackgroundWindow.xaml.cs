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
using Qianliyun_Launcher.BroadcastCapture;

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
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public GlobalStatus Status = new GlobalStatus();

        public BackgroundWindow()
        {
            InitializeComponent();
            this.Hide();

            // Initialize application
            var pricipal = new System.Security.Principal.WindowsPrincipal(
                System.Security.Principal.WindowsIdentity.GetCurrent());
            if (pricipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                RegistryKey registrybrowser = Registry.LocalMachine.OpenSubKey
                    (@"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                string myProgramName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var currentValue = registrybrowser.GetValue(myProgramName);
                if (currentValue == null || (int)currentValue != 0x00002af9)
                    registrybrowser.SetValue(myProgramName, 0x00002af9, RegistryValueKind.DWord);
            }

        }

        public void EnableDebugMode()
        {
            //AllocConsole();
            //Console.WriteLine("Debug mode on");
        }

        public void DisableDebugMode()
        {
            //FreeConsole();
        }
    }
}
