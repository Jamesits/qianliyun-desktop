using System;
using System.Runtime.InteropServices;
using System.Text;
using Accessibility;

namespace MSAALayer
{
    internal class Win32
    {

        #region Win32 API Declarations
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowByClass(string lpClassName, IntPtr zero);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowByCaption(IntPtr zero, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hwnd);

        [DllImport("oleacc.dll")]
        public static extern uint GetRoleText(uint dwRole, [Out] StringBuilder lpszRole, uint cchRoleMax);

        [DllImport("oleacc.dll")]
        public static extern uint GetStateText(uint dwStateBit, [Out] StringBuilder lpszStateBit, uint cchStateBitMax);

        [DllImport("oleacc.dll")]
        public static extern uint WindowFromAccessibleObject(IAccessible pacc, ref IntPtr phwnd);

        [DllImport("oleacc.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object AccessibleObjectFromWindow(int hwnd, int dwId, ref Guid riid);

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(
             IntPtr hwnd,
             uint id,
             ref Guid iid,
             [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject); 

        [DllImport("oleacc.dll")]
        public static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren,
                                                     [Out()] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] object[] rgvarChildren, ref int pcObtained);
        
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        #endregion

    }
}
