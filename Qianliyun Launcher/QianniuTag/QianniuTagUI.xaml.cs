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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Qianliyun_Launcher.QianniuTag
{
    /// <summary>
    /// Interaction logic for QianniuTagUI.xaml
    /// </summary>
    public partial class QianniuTagUI : UserControl
    {
        public QianniuTagUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var _qianniuWindow = new QianniuWindow("接待中心");
            var i = 1;
        }
    }
}
