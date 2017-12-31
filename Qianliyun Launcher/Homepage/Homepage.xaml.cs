using System.Windows.Controls;
using NLog;

namespace Qianliyun_Launcher.Homepage
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Homepage()
        {
            InitializeComponent();
            logger.Debug("Homepage control initialized");
        }
    }
}
