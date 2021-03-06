﻿using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements.Infrastructure;
using Microsoft.Win32.SafeHandles;
using NLog;
using Clipboard = System.Windows.Clipboard;

namespace Qianliyun_Launcher.DeepDarkWin32Fantasy
{
    class InteropUtil
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void CreateConsole()
        {
            // https://stackoverflow.com/questions/15604014/no-console-output-when-using-allocconsole-and-target-architecture-x86
            Logger.Info("Allocating new console");
            PInvoke.AllocConsole();
            // stdout's handle seems to always be equal to 7
            IntPtr currentStdout = PInvoke.GetStdHandle(PInvoke.STD_OUTPUT_HANDLE);
            // if (currentStdout != defaultStdoutHandle)
            SafeFileHandle safeFileHandle = new SafeFileHandle(currentStdout, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = Encoding.GetEncoding(65001);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding) {AutoFlush = true};
            Console.SetOut(standardOutput);
            Logger.Info("New console prepared");
        }

        public static void ClickWithMouse(int x, int y)
        {
            //Call the imported function with the cursor's current position
            Logger.Debug("Clicking on screen position ({0}, {1})", x, y);
            Cursor.Position = new Point(x, y);
            PInvoke.mouse_event(PInvoke.MOUSEEVENTF_LEFTDOWN | PInvoke.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        public static void Click(IntPtr hWnd, int x, int y)
        {
            Logger.Debug("Sending mouse Click event to control {0} relative position ({1}, {2})", hWnd, x, y);
            PInvoke.SendMessage(hWnd, PInvoke.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            PInvoke.SendMessage(hWnd, PInvoke.WM_LBUTTONDOWN, 1, (y << 16) | (x & 0xffff));
            PInvoke.SendMessage(hWnd, PInvoke.WM_LBUTTONUP, 0, (y << 16) | (x & 0xffff));
        }

        // Click an element inside a parent element, where child element do not have a handle
        public static void Click(AutomationElement MessageReceiver, AutomationElement RelativeChild, int x, int y)
        {
            Logger.Debug("Clicking child element {1} relative position ({2}, {3}) relative parent {0}", MessageReceiver.Name, RelativeChild.Name, x, y);
            var hWnd = MessageReceiver.Properties.NativeWindowHandle;
            x = x + (int)(RelativeChild.BoundingRectangle.X - MessageReceiver.BoundingRectangle.X);
            y = y + (int)(RelativeChild.BoundingRectangle.Y - MessageReceiver.BoundingRectangle.Y);
            Click(hWnd, x, y);
        }

        public static void Write(IntPtr hWnd, string text)
        {
            Logger.Debug("Setting textbox {0} content to be \"{1}\"", hWnd, text);
            // activate it
            PInvoke.SendMessage(hWnd, PInvoke.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            // this fails on Chinese...
            //SetWindowTextW(hWnd, text);
            Clipboard.SetText(text);
            // let's hope nobody modifies it...
            PInvoke.SendMessage(hWnd, PInvoke.WM_PASTE, IntPtr.Zero, IntPtr.Zero);
            // cleanup
            Clipboard.Flush();
        }

        public static void ScrollToBottom(IntPtr hWnd)
        {
            Logger.Debug("Scrolling window {0} to the bottom", hWnd);
            PInvoke.SendMessage(hWnd, PInvoke.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            PInvoke.SendMessage(hWnd, PInvoke.WM_VSCROLL, new IntPtr(PInvoke.SB_BOTTOM), IntPtr.Zero);
        }
    }
}
