using System;
using System.Windows;

namespace Qianliyun_Launcher.QianniuTag
{
    /// <summary>
    /// Interaction logic for QianniuTagUI.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class QianniuTagUI
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
            QianniuTagCore.DoTag(username, tag);
        }
    }
}
