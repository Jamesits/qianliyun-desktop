using System;
using System.Linq;
using System.Threading;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NLog;
using Application = FlaUI.Core.Application;
using static Qianliyun_Launcher.InteropUtil;

namespace Qianliyun_Launcher.QianniuTag
{
    class QianniuTagCore
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
        public class UIAutomationUnsupportedException : QianniuTagCoreBaseException
        {
            public UIAutomationUnsupportedException() { }
            public UIAutomationUnsupportedException(string message) : base(message) { }
            public UIAutomationUnsupportedException(string message, Exception inner) : base(message, inner) { }
        }

        // 找不到需要的 UI Automation 控件
        public class UIAutomationElementException : QianniuTagCoreBaseException
        {
            public UIAutomationElementException() { }
            public UIAutomationElementException(string message) : base(message) { }
            public UIAutomationElementException(string message, Exception inner) : base(message, inner) { }
        }

        // 遇到了没有测试的目标程序行为
        public class UIAutomationNotTestedRouteException : QianniuTagCoreBaseException
        {
            public UIAutomationNotTestedRouteException() { }
            public UIAutomationNotTestedRouteException(string message) : base(message) { }
            public UIAutomationNotTestedRouteException(string message, Exception inner) : base(message, inner) { }
        }
        #endregion

        private static void cancelFriendVerificationDialog()
        {
            using (var automation = new UIA3Automation())
            {
                logger.Debug("Checking if there is a verification dialog");
                var desktop = automation.GetDesktop();
                var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友").ToList()[0];
                // yes
                // now we cancel
                var cancelBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("消")).ToList()[0].AsButton();
                cancelBtn.Invoke();
            }
        }

        public static void doTag(string name, string newTag)
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
                try
                {
                    logger.Debug("Finding Qianniu Application");
                    var QianniuTopWindows = QianniuApplication.GetAllTopLevelWindows(automation);
                    logger.Debug("Finding Qianniu Chat Window");
                    var chatWindow = QianniuTopWindows.Where(x => x.Name.EndsWith("接待中心")).ToList()[0];
                    logger.Debug("Finding Search bar");
                    var searchBar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[5]");

                    // if there is content in searchBar, click clear;
                    try
                    {
                        var searchBarChilds = searchBar.FindAllChildren().ToList();
                        if (searchBarChilds.Count > 2)
                        {
                            logger.Debug("Clearing search bar content");
                            var clearBtn = searchBarChilds[2].AsButton();
                            click(clearBtn.Properties.NativeWindowHandle, 5, 5);
                        }
                        else
                        {
                            logger.Debug("Nothing in search bar, OK to progress");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw new UIAutomationElementException("Clear search bar content failed", e);
                    }

                    Thread.Sleep(1000);

                    try
                    {
                        logger.Debug("Finding search textbox");
                        var searchBox = searchBar.FindFirstByXPath("/Edit").AsTextBox();
                        logger.Debug("Inserting username {0}", name);
                        write(searchBox.Properties.NativeWindowHandle, name);
                    }
                    catch (Exception e)
                    {
                        // strange things happened
                        logger.Error(e);
                        throw new UIAutomationNotTestedRouteException("Insert search string into textbox failed", e);
                    }

                    Thread.Sleep(1000);

                    // check if we can get a search result pane
                    try
                    {
                        logger.Debug("See if we can get a search resule pane...");
                        var searchResultPopup = chatWindow.FindAllChildren()
                            .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                            .ToList()[0];
                        var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[2]");
                        // search in network
                        // TODO: it seems that pressing enter works too. needs verification.
                        logger.Debug("we need to search in network rather than friend list");
                        click(searchResultInnerPane.Properties.NativeWindowHandle,
                            searchResultInnerPane.ActualWidth.ToInt() - 15, 21);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        // search text is empty
                        logger.Error("search text is empty or unknown error");
                        throw new UIAutomationElementException("Local search result popup not present", e);
                    }

                    Thread.Sleep(1000);

                    // wait for search result
                    try
                    {
                        logger.Debug("find search result pop dialog");
                        var searchResultPopup = chatWindow.FindAllChildren()
                            .Where(x => x.FindAllChildren().Length == 2 && x.FindAllChildren()[1].Name == "SEARCH_WND")
                            .ToList()[0];
                        var searchResultInnerPane = searchResultPopup.FindFirstByXPath("/Pane[1]");

                        // check if there is result

                        // click first entry
                        logger.Debug("click on the first entry");
                        //clickWithMouse(
                        //    (int)(searchResultInnerPane.BoundingRectangle.X + searchResultInnerPane.BoundingRectangle.Width / 2),
                        //    (int)searchResultInnerPane.BoundingRectangle.Y + 19);
                        // this is strange: we have to click on its parent not itself
                        click(searchResultInnerPane.Parent.Properties.NativeWindowHandle,
                            searchResultInnerPane.ActualWidth.ToInt() / 2, 15);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        // search text is empty
                        logger.Error("search text is empty or unknown error");
                        throw new UIAutomationElementException("Internet search result popup not present", e);
                    }

                    Thread.Sleep(1000);

                    // Check if this guy needs verification to add friends
                    // TODO: if the user completes disable friend request
                    try
                    {
                        cancelFriendVerificationDialog();
                        logger.Warn("Verification needed");
                        throw new UserNeedFriendVerificationException(
                            "Needs to pass user friend verification to open chat dialog");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // OK, let's continue...
                        logger.Debug("Friend request passed");
                    }

                    Thread.Sleep(1000);

                    try
                    {
                        // now we should have chat dialog opened...
                        logger.Debug("Trying to find add friend toolbar");
                        // find chat toolbar
                        var friendToolbar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[3]").FindAllChildren()
                            .Where((x => x.ClassName == "ToolBarPlus")).ToList()[0];
                        // click on add friend button
                        logger.Debug("Click on add friend button");
                        // TODO: this guy may already be friend. Try to detect?
                        click(friendToolbar.Properties.NativeWindowHandle, 90, 18);
                        // clickWithMouse((int) friendToolbar.BoundingRectangle.Left + 90, (int) friendToolbar.BoundingRectangle.Top + 15);
                    }
                    catch (Exception e)
                    {
                        logger.Fatal(e);
                        throw new UIAutomationElementException("Cannot find add friend button", e);
                    }

                    Thread.Sleep(1000);

                    // Check if this guy needs verification to add friends (this is exactly the same as 2 block above)
                    // TODO: if the user completes disable friend request
                    try
                    {
                        cancelFriendVerificationDialog();
                        logger.Warn("Verification needed");
                        throw new UserNeedFriendVerificationException(
                            "Needs to pass user friend verification to add friend");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // OK, let's continue...
                        logger.Debug("Friend request passed");
                    }

                    Thread.Sleep(1000);

                    // Otherwise we should have a "添加好友成功!" dialog here
                    try
                    {
                        logger.Debug("trying to get a friended dialog");
                        var desktop = automation.GetDesktop();
                        var addFriendVerificationWindow = desktop.FindAllChildren().Where(x => x.Name == "添加好友成功!")
                            .ToList()[0];
                        // yes
                        // click on "完成"
                        var doneBtn = addFriendVerificationWindow.FindAllChildren().Where(x => x.Name.EndsWith("成"))
                            .ToList()[0].AsButton();
                        doneBtn.Invoke();
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        // strange things happened
                        // might already be friends...
                        logger.Warn(e);
                        //return 2;
                    }

                    Thread.Sleep(1000);

                    // click "客户" on the right panel on chat dialog
                    try
                    {
                        logger.Debug("Clicking on customer information tab");
                        var userInformationTabBar = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[1]");
                        click(userInformationTabBar.Properties.NativeWindowHandle, 210, 22);
                    }
                    catch (Exception e)
                    {
                        // strange things happened
                        logger.Fatal(e);
                        throw new UIAutomationElementException("Cannot find 客户 tab", e);
                    }

                    Thread.Sleep(1000);

                    // now find the right panel - a embedded Chrome
                    try
                    {
                        logger.Debug("Find the right panel");
                        var rightPanel = chatWindow.FindFirstByXPath("/Pane[2]/Pane[3]/Pane[5]/Pane[3]");
                        var customerInformationPanel = rightPanel.FindFirstByXPath("/Pane/Pane/Pane");
                        // TODO: there is two identical documents under customerInformationPanel; check why
                        var customerInformationDocument = customerInformationPanel.FindFirstChild().FindFirstChild();
                        // check if Chrome MSAA API has been enabled
                        if (customerInformationDocument.FindAllChildren().Length == 0)
                        {
                            logger.Fatal("Chrome MSAA not enabled!");
                            throw new UIAutomationUnsupportedException(
                                "Legacy Chrome Window don't have MSAA support enabled");
                        }
                        else
                        {
                            logger.Debug("Chrome MSAA check passed, continue to next stage");
                        }

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
                        logger.Debug("Got comment edit control");
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
                        logger.Debug("This user have {0} tags", alreadyHadTagCount);
                        var alreadyHadTagList = htmlTopNodes.ToList().Skip(tagListIndex)
                            .Take(alreadyHadTagCount);
                        var alreadyHadTags = alreadyHadTagList.Select(x => x.FindFirstChild().Name).ToList();
                        logger.Info("User tags: ");
                        foreach (var tag in alreadyHadTags) logger.Info("\t{0}", tag);

                        // if this user already have this tag -> ignore
                        if (alreadyHadTags.Contains(newTag)) throw new TagAlreadyPresentException();

                        // else click add tag button
                        try
                        {
                            var addButton = htmlTopNodes.Last().FindChildAt(1).AsButton();
                            addButton.Invoke();
                        }
                        catch (Exception e)
                        {
                            logger.Warn("Cannot find add tag button, maybe already clicked");
                            logger.Warn(e);
                        }

                        Thread.Sleep(500);

                        // refresh HTML nodes
                        htmlTopNodes = customerInformationDocument.FindAllChildren();

                        // get a global tag list
                        // after user tags, before last one
                        var alreadyPresentTags = htmlTopNodes.ToList().Skip(tagListIndex + alreadyHadTagCount).Reverse()
                            .Skip(1).Reverse().Select(x => x.FindFirstChild().Name).ToList();
                        logger.Info("Global tags: ");
                        foreach (var tag in alreadyPresentTags) logger.Info("\t{0}", tag);
                        if (!alreadyPresentTags.Contains(newTag)) throw new TagNotExistException();

                        // get the tag to be added
                        var newTagText =
                            htmlTopNodes[alreadyPresentTags.IndexOf(newTag) + tagListIndex + alreadyHadTagCount]
                                .FindFirstChild();
                        // unfortunately we have to emulate a click
                        click(customerInformationDocument, newTagText, 5, 5);
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
                    catch (QianniuTagCoreBaseException e)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {

                        // strange things happened
                        logger.Fatal(e);
                        throw new UIAutomationElementException("Cannot find element in right panel", e);
                    }

                }
                catch (QianniuTagCoreBaseException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new UIAutomationNotTestedRouteException("Unknown error", e);
                }
            }
        }
    }
}
