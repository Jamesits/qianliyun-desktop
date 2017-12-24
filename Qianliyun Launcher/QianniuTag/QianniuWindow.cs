using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accessibility;
using MSAALayer;

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
        //OfficeRibbon _ribbon = null;
        //OfficeRibbonPropertyPage _propertyPage = null;

        //public Dictionary<string, OfficeToolBar> ToolBars
        //{
        //    get { return _propertyPage.ToolBars; }
        //}


        public QianniuWindow(string name)
            : base(name)
        {
            if (this.Accessible == default(IAccessible))
            {
                throw new Exception("No window found: " + name);
            }

            //_ribbon = new OfficeRibbon(Accessible, "Ribbon", true);

            //if (_ribbon.Accessible == default(IAccessible))
            //{
            //    throw new Exception("Ribbon not found.");
            //}
        }

        //public MSWordWindow(OfficeAppType appType, string caption)
        //    : base(GetClassName(appType), caption)
        //{

        //}

        public void SelectTab(string name)
        {
            //    if (!_ribbon.Tabs.ContainsKey(name))
            //    {
            //        //Just give one more try
            //        _ribbon.ReloadTabs();
            //        if (!_ribbon.Tabs.ContainsKey(name))
            //        {
            //            throw new Exception(name + " Ribbon Tab not found.");
            //        }
            //    }

            //    if (_ribbon.Tabs[name].Invoke())
            //    {
            //        _propertyPage = new OfficeRibbonPropertyPage(_ribbon.Accessible, name,
            //            MSAALayer.AccessibleUIItemType.PropertyPage, true);
            //    }

            //}
        }

        public bool Search(string name)
        {
            var searchbox = this.GetChildren()[3].GetChildren()[0].GetChildren()[3].GetChildren()[7].GetChildren()[3].GetChildren()[0].GetChildren()[3];
            // input text to it
            return true;
        }


    }
}
