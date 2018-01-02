using System;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NLog;
using static Qianliyun_Launcher.DeepDarkWin32Fantasy.InteropUtil;

namespace Qianliyun_Launcher.QianniuTag
{
    class QianniuTagCore
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region exceptions
        public class QianniuTagCoreBaseException : Exception
        {
            public QianniuTagCoreBaseException() { }
            public QianniuTagCoreBaseException(string message) : base(message) { }
            public QianniuTagCoreBaseException(string message, Exception inner) : base(message, inner) { }
        }

        // 用户需要验证好友请求
        public class UserNeedFriendVerificationException : QianniuTagCoreBaseException
        {
            public UserNeedFriendVerificationException() { }
            public UserNeedFriendVerificationException(string message) : base(message) { }
            public UserNeedFriendVerificationException(string message, Exception inner) : base(message, inner) { }
        }
        
        // 用户拒绝加为好友
        public class UserCannotAddFriendException : QianniuTagCoreBaseException
        {
            public UserCannotAddFriendException() { }
            public UserCannotAddFriendException(string message) : base(message) { }
            public UserCannotAddFriendException(string message, Exception inner) : base(message, inner) { }
        }

        // 用户 tag 已打
        public class TagAlreadyPresentException : QianniuTagCoreBaseException
        {
            public TagAlreadyPresentException(){}
            public TagAlreadyPresentException(string message) : base(message){}
            public TagAlreadyPresentException(string message, Exception inner) : base(message, inner){}
        }

        // 全局 tag 不存在
        public class TagNotExistException : QianniuTagCoreBaseException
        {
            public TagNotExistException() { }
            public TagNotExistException(string message) : base(message) { }
            public TagNotExistException(string message, Exception inner) : base(message, inner) { }
        }

        // 某个控件没有启用 UI Automation 支持
        // ReSharper disable once InconsistentNaming
        public class UIAutomationUnsupportedException : QianniuTagCoreBaseException
        {
            public UIAutomationUnsupportedException() { }
            public UIAutomationUnsupportedException(string message) : base(message) { }
            public UIAutomationUnsupportedException(string message, Exception inner) : base(message, inner) { }
        }

        // 找不到需要的 UI Automation 控件
        // ReSharper disable once InconsistentNaming
        public class UIAutomationElementException : QianniuTagCoreBaseException
        {
            public UIAutomationElementException() { }
            public UIAutomationElementException(string message) : base(message) { }
            public UIAutomationElementException(string message, Exception inner) : base(message, inner) { }
        }

        // 遇到了没有测试的目标程序行为
        // ReSharper disable once InconsistentNaming
        public class UIAutomationNotTestedRouteException : QianniuTagCoreBaseException
        {
            public UIAutomationNotTestedRouteException() { }
            public UIAutomationNotTestedRouteException(string message) : base(message) { }
            public UIAutomationNotTestedRouteException(string message, Exception inner) : base(message, inner) { }
        }
        #endregion

        private static void CancelFriendVerificationDialog()
        {
            using (var automation = new UIA3Automation())
            {
                Logger.Debug("Checking if there is a verification dialog");
                var desktop = automation.GetDesktop();
                var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友").ToList()[0];
                // yes
                // now we cancel
                var cancelBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("消")).ToList()[0].AsButton();
                cancelBtn.Invoke();
            }
        }

        public static void DoTag(string name, string newTag)
        {
            //var searchbox = this.GetChildren()[3].GetChildren()[0].GetChildren()[3].GetChildren()[7].GetChildren()[3].GetChildren()[0].GetChildren()[3];
            //// Click on searchbox
            //SendMessage(searchbox.Properties.Handle, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            //// input to it
            //SendMessage(searchbox.Properties.Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(name));
            //// Click on 在网络中查找
            //// check if there is result
            //// caveat: there is no reliable way to check a result since it runs on a special dialog.
            ////var first_result_pos = this.GetChildren()[3].GetChildren()[0].GetChildren()[3].GetChildren()[4].GetChildren()[3].GetChildren()[1].GetChildren()[3];
            //var searchDialog = new QianniuWindow("SEARCH_WND");
            //Click(searchDialog.Properties.Handle, 150, 20);
            try
            {
                var qianniuApplication = Application.Attach("AliWorkbench.exe");
                using (var automation = new UIA3Automation())
                {
                    try
                    {
                        Logger.Debug("Finding Qianniu Application");
                        var qianniuTopWindows = qianniuApplication.GetAllTopLevelWindows(automation);
                        Logger.Debug("Finding Qianniu Chat Window");
                        var chatWindow = qianniuTopWindows.Where(x => x.Name.EndsWith("接待中心")).ToList()[0];
                        Logger.Debug("Finding Search bar");
                        var searchBar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[5]");

                        // if there is content in searchBar, Click clear;
                        try
                        {
                            var searchBarChilds = searchBar.FindAllChildren().ToList();
                            if (searchBarChilds.Count > 2)
                            {
                                Logger.Debug("Clearing search bar content");
                                var clearBtn = searchBarChilds[2].AsButton();
                                Click(clearBtn.Properties.NativeWindowHandle, 5, 5);
                            }
                            else
                            {
                                Logger.Debug("Nothing in search bar, OK to progress");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                            throw new UIAutomationElementException("Clear search bar content failed", e);
                        }

                        Thread.Sleep(1000);

                        try
                        {
                            Logger.Debug("Finding search textbox");
                            var searchBox = searchBar.FindFirstByXPath("/Edit").AsTextBox();
                            Logger.Debug("Inserting username {0}", name);
                            Write(searchBox.Properties.NativeWindowHandle, name);
                        }
                        catch (Exception e)
                        {
                            // strange things happened
                            Logger.Error(e);
                            throw new UIAutomationNotTestedRouteException("Insert search string into textbox failed", e);
                        }

                        Thread.Sleep(1000);

                        // check if we can get a search result pane
                        try
                        {
                            Logger.Debug("See if we can get a search resule pane...");
                            var searchResultPopup = chatWindow.FindAllChildren()
                                .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                                .ToList()[0];
                            var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[2]");
                            // search in network
                            // TODO: it seems that pressing enter works too. needs verification.
                            Logger.Debug("we need to search in network rather than friend list");
                            Click(searchResultInnerPane.Properties.NativeWindowHandle,
                                searchResultInnerPane.ActualWidth.ToInt() - 15, 21);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            // search text is empty
                            Logger.Error("search text is empty or unknown error");
                            throw new UIAutomationElementException("Local search result popup not present", e);
                        }

                        Thread.Sleep(1000);

                        // wait for search result
                        try
                        {
                            Logger.Debug("find search result pop dialog");
                            var searchResultPopup = chatWindow.FindAllChildren()
                                .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                                .ToList()[0];
                            var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[1]");

                            // check if there is result

                            // Click first entry
                            Logger.Debug("Click on the first entry");
                            //clickWithMouse(
                            //    (int)(searchResultInnerPane.BoundingRectangle.X + searchResultInnerPane.BoundingRectangle.Width / 2),
                            //    (int)searchResultInnerPane.BoundingRectangle.Y + 19);
                            // this is strange: we have to Click on its parent not itself
                            Click(searchResultInnerPane.Parent.Properties.NativeWindowHandle,
                                searchResultInnerPane.ActualWidth.ToInt() / 2, 15);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            // search text is empty
                            Logger.Error("search text is empty or unknown error");
                            throw new UIAutomationElementException("Internet search result popup not present", e);
                        }

                        Thread.Sleep(1000);

                        // Check if this guy needs verification to add friends
                        // TODO: if the user completes disable friend request
                        try
                        {
                            CancelFriendVerificationDialog();
                            Logger.Warn("Verification needed");
                            throw new UserNeedFriendVerificationException(
                                "Needs to pass user friend verification to open chat dialog");
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // OK, let's continue...
                            Logger.Debug("Friend request passed");
                        }

                        Thread.Sleep(1000);

                        try
                        {
                            // now we should have chat dialog opened...
                            Logger.Debug("Trying to find add friend toolbar");
                            // find chat toolbar
                            var friendToolbar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[3]").FindAllChildren()
                                .Where((x => x.ClassName == "ToolBarPlus")).ToList()[0];
                            // Click on add friend button
                            Logger.Debug("Click on add friend button");
                            // TODO: this guy may already be friend. Try to detect?
                            Click(friendToolbar.Properties.NativeWindowHandle, 90, 18);
                            // clickWithMouse((int) friendToolbar.BoundingRectangle.Left + 90, (int) friendToolbar.BoundingRectangle.Top + 15);
                        }
                        catch (Exception e)
                        {
                            Logger.Fatal(e);
                            throw new UIAutomationElementException("Cannot find add friend button", e);
                        }

                        Thread.Sleep(1000);

                        // Check if this guy needs verification to add friends (this is exactly the same as 2 block above)
                        // TODO: if the user completes disable friend request
                        try
                        {
                            CancelFriendVerificationDialog();
                            Logger.Warn("Verification needed");
                            throw new UserNeedFriendVerificationException(
                                "Needs to pass user friend verification to add friend");
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // OK, let's continue...
                            Logger.Debug("Friend request passed");
                        }

                        Thread.Sleep(1000);

                        // Otherwise we should have a "添加好友成功!" dialog here
                        try
                        {
                            Logger.Debug("trying to get a friended dialog");
                            var desktop = automation.GetDesktop();
                            var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友成功!")
                                .ToList()[0];
                            // yes
                            // Click on "完成"
                            var doneBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("成"))
                                .ToList()[0].AsButton();
                            doneBtn.Invoke();
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            // strange things happened
                            // might already be friends...
                            Logger.Warn(e);
                            //return 2;
                        }

                        Thread.Sleep(1000);

                        // Click "客户" on the right panel on chat dialog
                        try
                        {
                            Logger.Debug("Clicking on customer information tab");
                            var userInformationTabBar = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[1]");
                            Click(userInformationTabBar.Properties.NativeWindowHandle, 210, 22);
                        }
                        catch (Exception e)
                        {
                            // strange things happened
                            Logger.Fatal(e);
                            throw new UIAutomationElementException("Cannot find 客户 tab", e);
                        }

                        Thread.Sleep(1000);

                        // now find the right panel - a embedded Chrome
                        try
                        {
                            Logger.Debug("Find the right panel");
                            var rightPanel = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[3]");
                            var customerInformationPanel = rightPanel.FindFirstByXPath("/Pane/Pane/Pane");
                            // TODO: there is two identical documents under customerInformationPanel; check why
                            var customerInformationDocument = customerInformationPanel.FindFirstChild().FindFirstChild();
                            // check if Chrome MSAA API has been enabled
                            if (customerInformationDocument.FindAllChildren().Length == 0)
                            {
                                Logger.Fatal("Chrome MSAA not enabled!");
                                throw new UIAutomationUnsupportedException(
                                    "Legacy Chrome Window don't have MSAA support enabled");
                            }
                            Logger.Debug("Chrome MSAA check passed, continue to next stage");

                            // find Edit after "备注"; following List （tag 总数量）
                            var commentEditControl = customerInformationDocument.FindAllChildren().Last(x =>
                            {
                                try
                                {
                                    return x.ControlType == ControlType.Edit;
                                }
                                catch
                                {
                                    return false;
                                }
                            });
                            Logger.Debug("Got comment edit control");
                            var htmlTopNodes = customerInformationDocument.FindAllChildren();
                            // 备注
                            // 备注输入框
                            // 标签总数量
                            // 标签 1
                            // 标签 2
                            // ...
                            var tagListIndex = htmlTopNodes.ToList().IndexOf(commentEditControl) + 2;
                            var alreadyHadTagCount = Convert.ToInt32(htmlTopNodes[tagListIndex - 1].FindFirstChild()
                                .FindChildAt(2).Name);
                            Logger.Debug("This user have {0} tags", alreadyHadTagCount);
                            var alreadyHadTagList = htmlTopNodes.ToList().Skip(tagListIndex)
                                .Take(alreadyHadTagCount);
                            var alreadyHadTags = alreadyHadTagList.Select(x => x.FindFirstChild().Name).ToList();
                            Logger.Info("User tags: ");
                            foreach (var tag in alreadyHadTags) Logger.Info("\t{0}", tag);

                            // if this user already have this tag -> ignore
                            if (alreadyHadTags.Contains(newTag)) throw new TagAlreadyPresentException();

                            // else Click add tag button
                            try
                            {
                                var addButton = htmlTopNodes.Last().FindChildAt(1).AsButton();
                                addButton.Invoke();
                            }
                            catch (Exception e)
                            {
                                Logger.Warn("Cannot find add tag button, maybe already clicked");
                                Logger.Warn(e);
                            }

                            Thread.Sleep(500);

                            // refresh HTML nodes
                            htmlTopNodes = customerInformationDocument.FindAllChildren();

                            // get a global tag list
                            // after user tags, before last one
                            var alreadyPresentTags = htmlTopNodes.ToList().Skip(tagListIndex + alreadyHadTagCount).Reverse()
                                .Skip(1).Reverse().Select(x => x.FindFirstChild().Name).ToList();
                            Logger.Info("Global tags: ");
                            foreach (var tag in alreadyPresentTags) Logger.Info("\t{0}", tag);
                            if (!alreadyPresentTags.Contains(newTag)) throw new TagNotExistException();

                            // get the tag to be added
                            var newTagText =
                                htmlTopNodes[alreadyPresentTags.IndexOf(newTag) + tagListIndex + alreadyHadTagCount]
                                    .FindFirstChild();
                            // unfortunately we have to emulate a Click
                            Click(customerInformationDocument, newTagText, 5, 5);
                            try
                            {
                                var addTagButton = htmlTopNodes.Last().FindFirstChild().AsButton();
                                addTagButton.Invoke();
                            }
                            catch (Exception e)
                            {
                                throw new UIAutomationElementException("Cannot find add tag confirm button", e);
                            }

                        }
                        catch (QianniuTagCoreBaseException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {

                            // strange things happened
                            Logger.Fatal(e);
                            throw new UIAutomationElementException("Cannot find element in right panel", e);
                        }

                    }
                    catch (QianniuTagCoreBaseException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        throw new UIAutomationNotTestedRouteException("Unknown error", e);
                    }
                }
            }
            catch (Exception e)
            {
                // cannot find AliWorkbench.exe
                Logger.Error(e.Message);
            }
        }
    }
}
