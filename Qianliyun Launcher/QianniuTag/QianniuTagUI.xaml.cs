using System;
using System.Windows;
using System.Windows.Controls;

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
            doTag("朱导来了", "测试");
            
        }

        private void doTag(String username, String tag)
        {
            QianniuTagCore.doTag(username, tag);
        }
    }
}
