using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Accessibility;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NLog;
using Application = FlaUI.Core.Application;
using Clipboard = System.Windows.Clipboard;
using static Qianliyun_Launcher.PInvoke;
using static Qianliyun_Launcher.InteropUtil;

namespace Qianliyun_Launcher.QianniuTag
{
    class QianniuTagCore
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        

        // return value
        // 0: success
        // 1: need verification
        // 2: cannot find panel
        public static int Search(string name)
        {
            //var searchbox = this.GetChildren()[3].GetChildren()[0].GetChildren()[3].GetChildren()[7].GetChildren()[3].GetChildren()[0].GetChildren()[3];
            //// click on searchbox
            //SendMessage(searchbox.Properties.Handle, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            //// input to it
            //SendMessage(searchbox.Properties.Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(name));
            //// click on 在网络中查找
            //// check if there is result
            //// caveat: there is no reliable way to check a result since it runs on a special dialog.
            ////var first_result_pos = this.GetChildren()[3].GetChildren()[0].GetChildren()[3].GetChildren()[4].GetChildren()[3].GetChildren()[1].GetChildren()[3];
            //var searchDialog = new QianniuWindow("SEARCH_WND");
            //click(searchDialog.Properties.Handle, 150, 20);
            var QianniuApplication = Application.Attach("AliWorkbench.exe");
            using (var automation = new UIA3Automation())
            {
                var QianniuTopWindows = QianniuApplication.GetAllTopLevelWindows(automation);
                var chatWindow = QianniuTopWindows.Where(x => x.Name.EndsWith("接待中心")).ToList()[0];
                var searchBar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[5]");
                var searchBarChilds = searchBar.FindAllChildren().ToList();
                // if there is content in searchBar, click clear;
                if (searchBarChilds.Count > 2)
                {
                    var clearBtn = searchBarChilds[2].AsButton();
                    click(clearBtn.Properties.NativeWindowHandle, 5, 5);

                }

                Thread.Sleep(1000);

                try
                {
                    var searchBox = searchBar.FindFirstByXPath("/Edit").AsTextBox();

                    write(searchBox.Properties.NativeWindowHandle, name);
                }
                catch (Exception)
                {
                    // strange things happened
                    logger.Trace(System.Environment.StackTrace);
                    return 2;
                }

                Thread.Sleep(1000);

                // check if we can get a search result pane
                try
                {
                    var searchResultPopup = chatWindow.FindAllChildren()
                        .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                        .ToList()[0];
                    var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[2]");
                    // search in network
                    // TODO: it seems that pressing enter works too. needs verification.
                    click(searchResultInnerPane.Properties.NativeWindowHandle,
                        searchResultInnerPane.ActualWidth.ToInt() - 15, 21);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // search text is empty
                    logger.Trace(System.Environment.StackTrace);
                    return 2;
                }

                Thread.Sleep(1000);

                // wait for search result
                try
                {
                    var searchResultPopup = chatWindow.FindAllChildren()
                        .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                        .ToList()[0];
                    var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[1]");

                    // check if there is result

                    // click first entry
                    clickWithMouse(
                        (int)(searchResultInnerPane.BoundingRectangle.X + searchResultInnerPane.BoundingRectangle.Width / 2),
                        (int)searchResultInnerPane.BoundingRectangle.Y + 19);
                    // TODO: Why this doesn't work
                    //click(searchResultInnerPane.Properties.NativeWindowHandle,
                    //    searchResultInnerPane.ActualWidth.ToInt() / 2, 19);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // search text is empty
                    logger.Trace(System.Environment.StackTrace);
                    return 2;
                }

                Thread.Sleep(1000);

                // Check if this guy needs verification to add friends
                try
                {
                    var desktop = automation.GetDesktop();
                    var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友").ToList()[0];
                    // yes
                    // now we cancel
                    var cancelBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("消")).ToList()[0].AsButton();
                    cancelBtn.Invoke();
                    return 1;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // OK, let's continue...
                    logger.Trace(System.Environment.StackTrace);
                }

                // no
                // now we should have chat dialog opened...
                // find chat toolbar
                var friendToolbar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[3]").FindAllChildren().Where((x => x.ClassName == "ToolBarPlus")).ToList()[0];
                // click on add friend button
                // TODO: this guy may already be friend. Try to detect?
                // TODO: doesn't work either
                // click(friendToolbar.Properties.NativeWindowHandle, 90, 15);
                clickWithMouse((int)friendToolbar.BoundingRectangle.Left + 90, (int)friendToolbar.BoundingRectangle.Top + 15);


                // Check if this guy needs verification to add friends (this is exactly the same as 2 block above)
                try
                {
                    var desktop = automation.GetDesktop();
                    var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友").ToList()[0];
                    // yes
                    // now we cancel
                    var cancelBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("消")).ToList()[0].AsButton();
                    cancelBtn.Invoke();
                    return 1;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // OK, let's continue...
                    logger.Trace(System.Environment.StackTrace);
                }

                // Otherwise we should have a "添加好友成功!" dialog here
                try
                {
                    var desktop = automation.GetDesktop();
                    var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友成功!").ToList()[0];
                    // yes
                    // click on "完成"
                    var doneBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("成")).ToList()[0].AsButton();
                    doneBtn.Invoke();
                    
                }
                catch (ArgumentOutOfRangeException)
                {
                    // strange things happened
                    // might already be friends...
                    logger.Trace(System.Environment.StackTrace);
                    //return 2;
                }

                // click "客户" on the right panel on chat dialog
                try
                {
                    var userInformationTabBar = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[1]");
                    click(userInformationTabBar.Properties.NativeWindowHandle, 210, 22);
                }
                catch (Exception)
                {
                    // strange things happened
                    logger.Trace(System.Environment.StackTrace);
                    return 2;
                }

                // now find the right panel - a embedded Chrome
                try
                {
                    var customerInformationPanel = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[3]")
                        .FindAllDescendants().Where(x => x.Name == "客户插件").ToList()[0];
                    // scroll to the end
                    ScrollToBottom(customerInformationPanel.Properties.NativeWindowHandle);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // strange things happened
                    logger.Trace(System.Environment.StackTrace);
                    return 2;
                }
                

            }


            return 0;
        }
    }
}
