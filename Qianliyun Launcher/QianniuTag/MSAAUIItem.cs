using System;
using System.Text.RegularExpressions;
using Accessibility;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace MSAALayer
{
    
    public class MSAAUIItem
    {
        #region Private Members

        IAccessible _me = default(IAccessible);
        IAccessible _parent = default(IAccessible);
        MSAAPropertySet _propertySet = null;
        int searchCycles = 15;
        int searchDuration = 2000;
        
        #endregion

        #region Public Properties

        public MSAAPropertySet Properties
        {
            get
            {
                _propertySet.Refresh();
                return _propertySet;
            }
        }

        public IAccessible Accessible
        {
            get { return _me; }
        }

        public IAccessible AccessibleParent
        {
            get { return _parent; }
        }

        #endregion
        
        #region Constructors

        public MSAAUIItem(Regex name)
        {
            for (int searchCycleCount = 0; searchCycleCount < searchCycles; searchCycleCount++)
            {
                _me = MSAA.GetTopWindowAccessibleObject(name);

                if (_me == null || _me == default(IAccessible))
                {
                    Thread.Sleep(searchDuration);
                }
                else
                {
                    _propertySet = new MSAAPropertySet(_me);
                    break;
                }
            }
        }

        public MSAAUIItem(string className, string caption)
        {
            for (int searchCycleCount = 0; searchCycleCount < searchCycles; searchCycleCount++)
            {
                _me = MSAA.GetTopWindowAccessibleObject(className, caption);

                if (_me == null || _me == default(IAccessible))
                {
                    Thread.Sleep(searchDuration);
                }
                else
                {
                    _propertySet = new MSAAPropertySet(_me);
                    break;
                }
            }
        }

        public MSAAUIItem(IAccessible parentAccObject, Regex name, bool ignoreInvisible)
        {
            for (int searchCycleCount = 0; searchCycleCount < searchCycles; searchCycleCount++)
            {
                _me = MSAA.GetObjectByName(parentAccObject, name, ignoreInvisible);
                _parent = parentAccObject;
                
                if (_me == null || _me == default(IAccessible))
                {
                    Thread.Sleep(searchDuration);
                }
                else
                {
                    _propertySet = new MSAAPropertySet(_me);
                    break;
                }
            }
            
        }

        public MSAAUIItem(IAccessible parentAccObject, Regex name, AccessibleUIItemType uiItemType, bool ignoreInvisible)
        {
            for (int searchCycleCount = 0; searchCycleCount < searchCycles; searchCycleCount++)
            {
                _me = MSAA.GetObjectByNameAndRole(parentAccObject, name, MSAARoles.GetRoleText(uiItemType), ignoreInvisible);
                _parent = parentAccObject;

                if (_me == null || _me == default(IAccessible))
                {
                    Thread.Sleep(searchDuration);
                }
                else
                {
                    _propertySet = new MSAAPropertySet(_me);
                    break;
                }
            }
            
            _propertySet = new MSAAPropertySet(_me);
        }

        public MSAAUIItem(IAccessible accObject)
        {
            _me = accObject;
            _propertySet = new MSAAPropertySet(_me);
        }


        #endregion

        #region Public Methods

        public List<MSAAUIItem> GetChildren()
        {
            List<MSAAUIItem> accUiItemList = new List<MSAAUIItem>();
            
            foreach(IAccessible accUIObject in MSAA.GetAccessibleChildren(_me))
            {
                MSAAUIItem accUIItem = new MSAAUIItem(accUIObject);
                accUiItemList.Add(accUIItem);
            }

            return accUiItemList;
        }

        public List<MSAAUIItem> GetChildren(AccessibleUIItemType uiItemType)
        {
            List<MSAAUIItem> accUiItemList = new List<MSAAUIItem>();

            foreach (IAccessible accUIObject in MSAA.GetAccessibleChildren(_me))
            {
                MSAAUIItem accUIItem = new MSAAUIItem(accUIObject);
                
                if(accUIItem.Properties.Role ==  MSAARoles.GetRoleText(uiItemType))
                {
                    accUiItemList.Add(accUIItem);
                }
            }

            return accUiItemList;
        }

        public List<MSAAUIItem> GetAllUIItemsOfType(AccessibleUIItemType uiItemType, bool ignoreInvisible)
        {
            List<MSAAUIItem> accUiItemList = new List<MSAAUIItem>();

            List<IAccessible> accObjectList = new List<IAccessible>();

            MSAA.GetAccessibleObjectListByRole(_me, MSAARoles.GetRoleText(uiItemType), ref accObjectList, ignoreInvisible);

            foreach (IAccessible accUIObject in accObjectList)
            {
                MSAAUIItem accUIItem = new MSAAUIItem(accUIObject);

                if (accUIItem.Properties.Role == MSAARoles.GetRoleText(uiItemType))
                {
                    accUiItemList.Add(accUIItem);
                }
            }

            return accUiItemList;
        }

        public bool Invoke()
        {
            bool defaultActionSuccess = false;

            if (_me != null && _me != default(IAccessible))
            {
                try
                {
                    _me.accDoDefaultAction(0);
                    defaultActionSuccess = true;
                }
                catch (Exception ex)
                {
                    defaultActionSuccess = false;
                }
            }
            return defaultActionSuccess;
        }

        #endregion

        #region Private Methods



        #endregion
    }
}
