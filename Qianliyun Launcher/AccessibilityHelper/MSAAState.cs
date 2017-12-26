using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSAALayer
{
    internal class MSAAStateConstants
    {
        public static uint STATE_SYSTEM_ALERT_HIGH = 0x10000000;
        public static uint STATE_SYSTEM_ALERT_LOW = 0x04000000;
        public static uint STATE_SYSTEM_ALERT_MEDIUM = 0x08000000;
        public static uint STATE_SYSTEM_ANIMATED = 0x00004000;
        public static uint STATE_SYSTEM_BUSY = 0x00000800;
        public static uint STATE_SYSTEM_CHECKED = 0x00000010;
        public static uint STATE_SYSTEM_COLLAPSED = 0x00000400;
        public static uint STATE_SYSTEM_DEFAULT = 0x00000100;
        public static uint STATE_SYSTEM_EXPANDED = 0x00000200;
        public static uint STATE_SYSTEM_EXTSELECTABLE = 0x02000000;
        public static uint STATE_SYSTEM_FLOATING = 0x00001000;
        public static uint STATE_SYSTEM_FOCUSABLE = 0x00100000;
        public static uint STATE_SYSTEM_FOCUSED = 0x00000004;
        public static uint STATE_SYSTEM_HASPOPUP = 0x40000000;
        public static uint STATE_SYSTEM_HOTTRACKED = 0x00000080;
        public static uint STATE_SYSTEM_INVISIBLE = 0x00008000;
        public static uint STATE_SYSTEM_LINKED = 0x00400000;
        public static uint STATE_SYSTEM_MARQUEED = 0x00002000;
        public static uint STATE_SYSTEM_MIXED = 0x00000020;
        public static uint STATE_SYSTEM_MOVEABLE = 0x00040000;
        public static uint STATE_SYSTEM_MULTISELECTABLE = 0x01000000;
        public static uint STATE_SYSTEM_NORMAL = 0x00000000;
        public static uint STATE_SYSTEM_OFFSCREEN = 0x00010000;
        public static uint STATE_SYSTEM_PRESSED = 0x00000008;
        public static uint STATE_SYSTEM_READONLY = 0x00000040;
        public static uint STATE_SYSTEM_SELECTABLE = 0x00200000;
        public static uint STATE_SYSTEM_SELECTED = 0x00000002;
        public static uint STATE_SYSTEM_SELFVOICING = 0x00080000;
        public static uint STATE_SYSTEM_SIZEABLE = 0x00020000;
        public static uint STATE_SYSTEM_TRAVERSED = 0x00800000;
        public static uint STATE_SYSTEM_UNAVAILABLE = 0x00000001;
        public static uint STATE_SYSTEM_VALID = 0x1FFFFFFF;
    }

    public class MSAAState
    {

        static MSAAState()
        {
        }

        public static string GetStateText(uint stateID)
        {
            uint maxLength = 1024;
            var focusableStateText = new StringBuilder((int)maxLength);
            var sizeableStateText = new StringBuilder((int)maxLength);
            var moveableStateText = new StringBuilder((int)maxLength);
            var invisibleStateText = new StringBuilder((int)maxLength);
            var unavailableStateText = new StringBuilder((int)maxLength);
            var hasPopupStateText = new StringBuilder((int)maxLength);

            if (stateID ==
                (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE
                | MSAAStateConstants.STATE_SYSTEM_SIZEABLE
                | MSAAStateConstants.STATE_SYSTEM_MOVEABLE))
            {
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE, focusableStateText, maxLength);
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_SIZEABLE, sizeableStateText, maxLength);
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_MOVEABLE, moveableStateText, maxLength);

                return focusableStateText + "," + sizeableStateText + "," + moveableStateText;
            }

            if (stateID ==
               (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE
               | MSAAStateConstants.STATE_SYSTEM_INVISIBLE))
            {
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE, focusableStateText, maxLength);
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_INVISIBLE, invisibleStateText, maxLength);

                return focusableStateText + "," + invisibleStateText;
            }

            if (stateID ==
               (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE
               | MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE))
            {
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE, focusableStateText, maxLength);
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE, unavailableStateText, maxLength);

                return focusableStateText + "," + unavailableStateText;
            }

            if (stateID ==
            (MSAAStateConstants.STATE_SYSTEM_HASPOPUP
            | MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE))
            {
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_HASPOPUP, hasPopupStateText, maxLength);
                Win32.GetStateText(MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE, unavailableStateText, maxLength);

                return hasPopupStateText + "," + unavailableStateText;
            }

            var stateText = new StringBuilder((int)maxLength);
            Win32.GetStateText(stateID, stateText, maxLength);
            return stateText.ToString();
        }



    }
}
