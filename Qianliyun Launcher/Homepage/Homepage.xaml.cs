using System.Windows.Controls;
using NLog;

namespace Qianliyun_Launcher.Homepage
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Homepage()
        {
            InitializeComponent();
            Logger.Debug("Homepage control initialized");
        }
    }
}
