using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using Interop.UIAutomationClient;
using MSAALayer;
using IAccessible = Accessibility.IAccessible;

namespace Qianliyun_Launcher.QianniuTag
{

    public class QianniuUIItem : MSAAUIItem
    {
        public QianniuUIItem(string name)
            : base(new Regex(name))
        {
        }

        public QianniuUIItem(IAccessible accessibleObject)
            : base(accessibleObject)
        {
        }

        public QianniuUIItem(string className, string caption)
            : base(className, caption)
        {

        }

        public QianniuUIItem(IAccessible parentAccObject, string name, bool ignoreInvisible)
            : base(parentAccObject, new Regex(name), ignoreInvisible)
        {

        }

        public QianniuUIItem(IAccessible parentAccObject, string name, AccessibleUIItemType uiItemType, bool ignoreInvisible)
            : base(parentAccObject, new Regex(name), uiItemType, ignoreInvisible)
        {

        }

    }

    public class QianniuWindow : QianniuUIItem
    {
        public static List<QianniuWindow> getQianniuChatWindows()
        {
            var qianniuChatWindows = new List<QianniuWindow>();
            var foundWindows = MSAA.GetTopWindowAccessibleObject(new Regex("接待中心"));
            return qianniuChatWindows;
        }


        public QianniuWindow(string name)
            : base(name)
        {
            if (this.Accessible == default(IAccessible))
            {
                throw new Exception("No window found: " + name);
            }

        }


       


    }
}
