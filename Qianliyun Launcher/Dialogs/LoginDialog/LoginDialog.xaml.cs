using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using NLog;
using Qianliyun_Launcher.Properties;

namespace Qianliyun_Launcher.Dialogs.LoginDialog
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Settings ApplicationConfig => Properties.Settings.Default;
        private StateManager State => StateManager.Instance;

        public LoginDialog()
        {
            InitializeComponent();

            // check if machine GUID has been generated
            if (((string)ApplicationConfig["MachineGUID"]) == "")
            {
                ApplicationConfig["MachineGUID"] = Guid.NewGuid().ToString();
                ApplicationConfig.Save();
                Logger.Debug("Generated new MachineGUID {0}", (string)ApplicationConfig["MachineGUID"]);
            }
            Logger.Info("Machine GUID is {0}", (string)ApplicationConfig["MachineGUID"]);
        }

        private bool checkCachedLoginCredential()
        {
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
