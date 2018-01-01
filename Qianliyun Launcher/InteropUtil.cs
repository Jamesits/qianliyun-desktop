using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements.Infrastructure;
using Microsoft.Win32.SafeHandles;
using NLog;
using static Qianliyun_Launcher.PInvoke;
using Clipboard = System.Windows.Clipboard;

namespace Qianliyun_Launcher
{
    class InteropUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void CreateConsole()
        {
            // https://stackoverflow.com/questions/15604014/no-console-output-when-using-allocconsole-and-target-architecture-x86
            logger.Info("Allocating new console");
            AllocConsole();
            // stdout's handle seems to always be equal to 7
            IntPtr currentStdout = GetStdHandle(STD_OUTPUT_HANDLE);
            // if (currentStdout != defaultStdoutHandle)
            SafeFileHandle safeFileHandle = new SafeFileHandle(currentStdout, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = Encoding.GetEncoding(65001);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding) {AutoFlush = true};
            Console.SetOut(standardOutput);
            logger.Info("New console prepared");
        }

        public static void clickWithMouse(int x, int y)
        {
            //Call the imported function with the cursor's current position
            logger.Debug("Clicking on screen position ({0}, {1})", x, y);
            Cursor.Position = new Point(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        public static void click(IntPtr hWnd, int x, int y)
        {
            logger.Debug("Sending mouse click event to control {0} relative position ({1}, {2})", hWnd, x, y);
            SendMessage(hWnd, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hWnd, WM_LBUTTONDOWN, 1, (y << 16) | (x & 0xffff));
            SendMessage(hWnd, WM_LBUTTONUP, 0, (y << 16) | (x & 0xffff));
        }

        // click an element inside a parent element, where child element do not have a handle
        public static void click(AutomationElement MessageReceiver, AutomationElement RelativeChild, int x, int y)
        {
            logger.Debug("Clicking child element {1} relative position ({2}, {3}) relative parent {0}", MessageReceiver.Name, RelativeChild.Name, x, y);
            var hWnd = MessageReceiver.Properties.NativeWindowHandle;
            x = x + (int)(RelativeChild.BoundingRectangle.X - MessageReceiver.BoundingRectangle.X);
            y = y + (int)(RelativeChild.BoundingRectangle.Y - MessageReceiver.BoundingRectangle.Y);
            click(hWnd, x, y);
        }

        public static void write(IntPtr hWnd, string text)
        {
            logger.Debug("Setting textbox {0} content to be \"{1}\"", hWnd, text);
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

        public static void ScrollToBottom(IntPtr hWnd)
        {
            logger.Debug("Scrolling window {0} to the bottom", hWnd);
            SendMessage(hWnd, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hWnd, WM_VSCROLL, new IntPtr(SB_BOTTOM), IntPtr.Zero);
        }
    }
}
