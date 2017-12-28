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
using Application = FlaUI.Core.Application;
using Clipboard = System.Windows.Clipboard;

namespace Qianliyun_Launcher.QianniuTag
{
    class QianniuTagCore
    {
        private const int WM_SETTEXT = 0x000C;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KILLFOCUS = 0x0008;
        private const int WM_CLOSE = 0x10;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_PASTE = 0x0302;
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(
            string lpClassName,
            string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string lpszClass,
            string lpszWindows);
        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessageW(IntPtr hwnd, int wMsg, IntPtr wParam, StringBuilder lParam);

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

        /// <summary>
        ///     Changes the text of the specified window's title bar (if it has one). If the specified window is a control, the
        ///     text of the control is changed. However, SetWindowText cannot change the text of a control in another application.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633546%28v=vs.85%29.aspx for more
        ///     information
        ///     </para>
        /// </summary>
        /// <param name="hwnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window or control whose text is to be changed.</param>
        /// <param name="lpString">C++ ( lpString [in, optional]. Type: LPCTSTR )<br />The new title or control text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.<br />
        ///     To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     If the target window is owned by the current process, <see cref="SetWindowText" /> causes a WM_SETTEXT message to
        ///     be sent to the specified window or control. If the control is a list box control created with the WS_CAPTION style,
        ///     however, <see cref="SetWindowText" /> sets the text for the control, not for the list box entries.<br />To set the
        ///     text of a control in another process, send the WM_SETTEXT message directly instead of calling
        ///     <see cref="SetWindowText" />. The <see cref="SetWindowText" /> function does not expand tab characters (ASCII code
        ///     0x09). Tab characters are displayed as vertical bar(|) characters.<br />For an example go to
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
        ///     Sending a
        ///     Message.
        ///     </see>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetWindowTextW(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        public static void clickWithMouse(int x, int y)
        {
            //Call the imported function with the cursor's current position
            Cursor.Position = new System.Drawing.Point(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        public static void click(IntPtr hWnd, int x, int y)
        {
            SendMessage(hWnd, WM_LBUTTONDOWN, 1, (y << 16) | (x & 0xffff));
            Thread.Sleep(100);
            SendMessage(hWnd, WM_LBUTTONUP, 0, (y << 16) | (x & 0xffff));
        }

        public static void write(IntPtr hWnd, string text)
        {
            // activate it
            SendMessage(hWnd, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            // this fails on Chinese...
            //SetWindowTextW(hWnd, text);
            Clipboard.SetText(text);
            // let's hope nobody modifies it...
            SendMessage(hWnd, WM_PASTE, IntPtr.Zero, IntPtr.Zero);
            // cleanup
            Clipboard.Flush();
        }

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

                var searchBox = searchBar.FindFirstByXPath("/Edit").AsTextBox();

                write(searchBox.Properties.NativeWindowHandle, name);

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
                    
                }

                // no
                // now we should have chat dialog opened...
                // find chat toolbar
                var friendToolbar = chatWindow.FindFirstByXPath("/Pane[last()]/Pane[3]").FindAllChildren().Where((x => x.ClassName == "ToolBarPlus")).ToList()[0];
                // click on add friend button
                // TODO: doesn't work either
                // click(friendToolbar.Properties.NativeWindowHandle, 90, 15);
                clickWithMouse((int)friendToolbar.BoundingRectangle.Left + 90, (int)friendToolbar.BoundingRectangle.Top + 15);

            }


            return 0;
        }
    }
}
