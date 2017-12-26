using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSAALayer
{
    public enum AccessibleUIItemType
    {
        Client,
        ComboBox,
        Dialog,
        EditableText,
        GridDropDownButton,
        List,
        ListItem,
        MenuBar,
        MenuItem,
        PageTab,
        Pane,
        PropertyPage,
        PushButton,
        SplitButton,
        ToolBar,
        DropDown,
        Window,
        TitleBar,
        ScrollBar,
        Grip
    }

    internal class MSAARoles
    {
        public const string Client = "client";
        public const string ComboBox = "combo box";
        public const string Dialog = "dialog";
        public const string EditableText = "editable text";
        public const string GridDropDownButton = "grid drop down button";
        public const string List = "list";
        public const string ListItem = "list item";
        public const string MenuBar = "menu bar";
        public const string MenuItem = "menu item";
        public const string PageTab = "page tab";
        public const string Pane = "pane";
        public const string PropertyPage = "property page";
        public const string PushButton = "push button";
        public const string SplitButton = "split button";
        public const string ToolBar = "tool bar";
        public const string DropDown = "drop down";
        public const string Window = "window";
        public const string TitleBar = "title bar";
        public const string ScrolBar = "scroll bar";
        public const string Grip = "grip";

        public static string GetRoleText(AccessibleUIItemType role)
        {
            switch (role)
            {
                case AccessibleUIItemType.Client:
                    return Client;
                    break;
                case AccessibleUIItemType.ComboBox:
                    return ComboBox;
                    break;
                case AccessibleUIItemType.Dialog:
                    return Dialog;
                    break;
                case AccessibleUIItemType.DropDown:
                    return DropDown;
                    break;
                case AccessibleUIItemType.EditableText:
                    return EditableText;
                    break;
                case AccessibleUIItemType.GridDropDownButton:
                    return GridDropDownButton;
                    break;
                case AccessibleUIItemType.List:
                    return List;
                    break;
                case AccessibleUIItemType.ListItem:
                    return ListItem;
                    break;
                case AccessibleUIItemType.MenuBar:
                    return MenuBar;
                    break;
                case AccessibleUIItemType.MenuItem:
                    return MenuItem;
                    break;
                case AccessibleUIItemType.PageTab:
                    return PageTab;
                    break;
                case AccessibleUIItemType.Pane:
                    return Pane;
                    break;
                case AccessibleUIItemType.PropertyPage:
                    return PropertyPage;
                    break;
                case AccessibleUIItemType.PushButton:
                    return PushButton;
                    break;
                case AccessibleUIItemType.SplitButton:
                    return SplitButton;
                    break;
                case AccessibleUIItemType.TitleBar:
                    return TitleBar;
                    break;
                case AccessibleUIItemType.ToolBar:
                    return ToolBar;
                    break;
                case AccessibleUIItemType.Window:
                    return Window;
                    break;
                case AccessibleUIItemType.ScrollBar:
                    return ScrolBar;
                    break;
                case AccessibleUIItemType.Grip:
                    return Grip;
                    break;
                default:
                    return "";


            }
        }
    }
}
