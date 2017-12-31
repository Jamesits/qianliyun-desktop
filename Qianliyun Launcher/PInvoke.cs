using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Qianliyun_Launcher
{
    class PInvoke
    {
        #region consts
        public static uint EVENT_SYSTEM_ALERT = 0x0002;
        public static uint WINEVENT_OUTOFCONTEXT = 0;
        public static int WM_GETOBJECT = 0x003d;
        public static int WM_SETTEXT = 0x000C;
        public static int WM_SETFOCUS = 0x0007;
        public static int WM_KILLFOCUS = 0x0008;
        public static int WM_CLOSE = 0x10;
        public static int WM_LBUTTONDOWN = 0x201;
        public static int WM_LBUTTONUP = 0x202;
        public static int WM_KEYDOWN = 0x0100;
        public static int WM_PASTE = 0x0302;
        public static int WM_VSCROLL = 0x115;
        //Mouse actions
        public static uint MOUSEEVENTF_LEFTDOWN = 0x02;
        public static uint MOUSEEVENTF_LEFTUP = 0x04;
        public static uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        public static uint MOUSEEVENTF_RIGHTUP = 0x10;
        public static int SB_BOTTOM = 7;

        public static IntPtr defaultStdoutHandle = new IntPtr(7);
        public static UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        #endregion

        #region types
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        #endregion

        #region functions
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(UInt32 nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
                hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindows);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessageW(IntPtr hwnd, int wMsg, IntPtr wParam, StringBuilder lParam);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

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

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        #endregion
    }
}
