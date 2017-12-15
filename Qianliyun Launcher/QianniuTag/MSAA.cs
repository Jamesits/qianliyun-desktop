using System;
using Accessibility;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;


namespace MSAALayer
{
    public class MSAA
    {
        private static List<IntPtr> topWindowHwndList = new List<IntPtr>();

        static MSAA()
        {
        }

        public static IntPtr GetTopWindow(string wClass, string wCaption)
        {
            if (string.IsNullOrEmpty(string.Concat(wClass, wCaption)))
            {
                return (IntPtr)0;
            }
            else if (string.IsNullOrEmpty(wClass))
            {
                return Win32.FindWindowByCaption((IntPtr)0, wCaption);
            }
            else if (string.IsNullOrEmpty(wCaption))
            {
                return Win32.FindWindowByClass(wClass, (IntPtr)0);
            }
            else
            {
                return Win32.FindWindow(wClass, wCaption);
            }
        }

        public static IAccessible[] GetAccessibleChildren(IAccessible objAccessible)
        {
            int childCount = 0;

            try
            {
                childCount = objAccessible.accChildCount;
            }
            catch (Exception ex)
            {
                childCount = 0;
            }

            var accObjects = new IAccessible[childCount];
            int count = 0;

            if (childCount != 0)
            {
                Win32.AccessibleChildren(objAccessible, 0, childCount, accObjects, ref count);
            }

            return accObjects;
        }

        public static IAccessible GetObjectByName(IAccessible objParent, Regex objName, bool ignoreInvisible)
        {
            IAccessible objToReturn = default(IAccessible);

            if (objParent != null)
            {
                IAccessible[] children = GetAccessibleChildren(objParent);
                foreach (IAccessible child in children)
                {
                    string childName = null;
                    string childState = string.Empty;

                    try
                    {
                        childName = child.get_accName(0);
                        childState = GetStateText(Convert.ToUInt32(child.get_accState(0)));
                    }
                    catch (Exception)
                    {
                    }

                    if (ignoreInvisible)
                    {
                        if (childName != null 
                            && objName.Match(childName).Success
                            && !childState.Contains("invisible"))
                        {
                            return child;
                        }
                    }
                    else
                    {
                        if (childName != null 
                            && objName.Match(childName).Success)
                        {
                            return child;
                        }
                    }
                    
                    if (ignoreInvisible)
                    {
                        if (!childState.Contains("invisible"))
                        {
                            objToReturn = GetObjectByName(child, objName, ignoreInvisible);
                            if (objToReturn != default(IAccessible))
                            {
                                return objToReturn;
                            }
                        }
                    }
                    else
                    {
                        objToReturn = GetObjectByName(child, objName, ignoreInvisible);
                        if (objToReturn != default(IAccessible))
                        {
                            return objToReturn;
                        }
                    }
                    
                }
            }

            return objToReturn;

            
        }

        public static IAccessible GetObjectByNameAndRole(IAccessible objParent, Regex objName, string roleText, bool ignoreInvisible)
        {
            IAccessible objToReturn = default(IAccessible);

            if (objParent != null)
            {
                IAccessible[] children = GetAccessibleChildren(objParent);
                foreach (IAccessible child in children)
                {
                    string childName = null;
                    string childState = string.Empty;
                    string childRole = string.Empty;

                    try
                    {
                        childName = child.get_accName(0);
                        childState = GetStateText(Convert.ToUInt32(child.get_accState(0)));
                        childRole = GetRoleText(Convert.ToUInt32(child.get_accRole(0)));
                    }
                    catch (Exception)
                    {
                    }

                    if (ignoreInvisible)
                    {
                        if (!string.IsNullOrEmpty(childName)
                            && objName.Match(childName).Success
                            && childRole == roleText
                            && !childState.Contains("invisible"))
                        {
                            return child;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(childName)
                            && objName.Match(childName).Success
                            && childRole == roleText)
                        {
                            return child;
                        }
                    }
                    
                    if (ignoreInvisible)
                    {
                        if (!childState.Contains("invisible"))
                        {
                            objToReturn = GetObjectByNameAndRole(child, objName, roleText, ignoreInvisible);
                            if (objToReturn != default(IAccessible))
                            {
                                return objToReturn;
                            }
                        }
                    }
                    else
                    {
                        objToReturn = GetObjectByNameAndRole(child, objName, roleText, ignoreInvisible);
                        if (objToReturn != default(IAccessible))
                        {
                            return objToReturn;
                        }
                    }
                    
                }
            }

            return objToReturn;


        }

        public static void GetAccessibleObjectListByRole(IAccessible objParent, string roleText,
                                            ref List<IAccessible> accessibleObjList, bool ignoreInvisible)
        {
            if (objParent != null)
            {
                IAccessible[] children = GetAccessibleChildren(objParent);
                foreach (IAccessible child in children)
                {
                    string roleTextInner = null;
                    string childState = string.Empty;

                    try
                    {
                        uint roleId = Convert.ToUInt32(child.get_accRole(0));
                        roleTextInner = GetRoleText(roleId);
                        childState = GetStateText(Convert.ToUInt32(child.get_accState(0)));
                    }
                    catch (Exception)
                    {
                    }

                    if (ignoreInvisible)
                    {
                        if (roleTextInner == roleText && !childState.Contains("invisible"))
                        {
                            accessibleObjList.Add(child);
                        }
                    }
                    else
                    {
                        if (roleTextInner == roleText)
                        {
                            accessibleObjList.Add(child);
                        }
                    }

                    if (ignoreInvisible)
                    {
                        if (!childState.Contains("invisible"))
                        {
                            GetAccessibleObjectListByRole(child, roleText, ref accessibleObjList, ignoreInvisible);
                        }
                    }
                    else
                    {
                        GetAccessibleObjectListByRole(child, roleText, ref accessibleObjList, ignoreInvisible);
                    }
                }
            }
        }

        public static IAccessible GetTopWindowAccessibleObject(string className, string caption)
        {
            IntPtr handle = GetTopWindow(className, caption);
            try
            {
                return GetAccessibleObjectFromHandle(handle);
            }
            catch (Exception ex)
            {

            }

            return default(IAccessible);
        }

        public static IAccessible GetTopWindowAccessibleObject(Regex windowName)
        {
            foreach (IAccessible accWindowObject in GetTopWindowAccessibleList())
            {
                try
                {
                    string accWindowName = accWindowObject.get_accName(0);

                    if (!string.IsNullOrEmpty(accWindowName))
                    {
                        if (windowName.Match(accWindowObject.get_accName(0)).Success)
                        {
                            return accWindowObject;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return default(IAccessible);
        }

        public static IAccessible GetAccessibleObjectFromHandle(IntPtr hwnd)
        {
            object accObject = new object();
            IAccessible objAccessible = default(IAccessible);
            var guidAccessible = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
            if (hwnd != (IntPtr)0)
            {
                Win32.AccessibleObjectFromWindow(hwnd, 0, ref guidAccessible, ref accObject);
                objAccessible = (IAccessible)accObject;
            }
            return objAccessible;
        }

        private static List<IAccessible> GetTopWindowAccessibleList()
        {
            GetTopWindowList();
            List<IAccessible> windowList = new List<IAccessible>();
            foreach (IntPtr hwnd in topWindowHwndList)
            {
                windowList.Add(GetAccessibleObjectFromHandle(hwnd));
            }

            return windowList;
        }

        private static void GetTopWindowList()
        {
            topWindowHwndList.Clear();
            Win32.EnumWindows(new Win32.EnumWindowsProc(EnumerateTopWindows), (IntPtr)0);

        }

        private static bool EnumerateTopWindows(IntPtr handle, IntPtr lParam)
        {
            topWindowHwndList.Add(handle);
            return true;
        }

        public static string GetStateText(uint stateID)
        {
            return MSAAState.GetStateText(stateID);
        }


        internal static string GetRoleText(uint roleId)
        {
            uint maxLength = 1024;
            var roleText = new StringBuilder((int)maxLength);

            Win32.GetRoleText(roleId, roleText, maxLength);

            return roleText.ToString();
        }

        internal static IntPtr GetHandle(IAccessible _accessible)
        {
            IntPtr hwnd = IntPtr.Zero;
            Win32.WindowFromAccessibleObject(_accessible, ref hwnd);
            return hwnd;
        }
    }
}
