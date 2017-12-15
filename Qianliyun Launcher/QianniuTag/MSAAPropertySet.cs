using System;
using System.Drawing;
using Accessibility;

namespace MSAALayer
{
    public class MSAAPropertySet
    {
        IAccessible _accessible = default(IAccessible);
        string _name = string.Empty;
        string _state = string.Empty;
        string _role = string.Empty;
        string _value = string.Empty;
        string _defaultAction = string.Empty;
        Rectangle _location = new Rectangle();
        IntPtr _handle = IntPtr.Zero;


        public string Name
        {
            get{return _name;}
        }

        public string State
        {
            get { return _state; }
        }

        public bool IsEnabled
        {
            get { return !State.Contains("unavailable"); }
        }

        public bool IsVisible
        {
            get { return !State.Contains("invisible"); }
        }

        public bool IsExist
        {
            get { return Handle != IntPtr.Zero; }
        }

        public string Role
        {
            get { return _role; }
        }

        public string Value
        {
            get { return _value; }
        }

        public Rectangle Location
        {
            get { return _location; }
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public string DefaultAction
        {
            get { return _defaultAction; }
        }

        public MSAAPropertySet(IAccessible accessibleObject)
        {
            _accessible = accessibleObject;
            
            if(_accessible != null && _accessible != default(IAccessible))
            {
                SetAccessibleProperties();
            }
        }

        public void Refresh()
        {
            if (_accessible != null && _accessible != default(IAccessible))
            {
                SetAccessibleProperties();
            }
        }

        private void SetAccessibleProperties()
        {
            //Here we are consuming the COM Exceptions which happens in case 
            //the property/Method we need is not available with IAccessible Object.

            try
            {
                _name = _accessible.get_accName(0);
            }
            catch (Exception ex)
            {
            }

            try
            {
                _value = _accessible.get_accValue(0);
            }
            catch (Exception ex)
            {
            }

            try
            {
                uint stateId = Convert.ToUInt32(_accessible.get_accState(0));
                _state = MSAA.GetStateText(stateId);
            }
            catch (Exception ex)
            {
            }

            try
            {
                uint roleId = Convert.ToUInt32(_accessible.get_accRole(0));
                _role = MSAA.GetRoleText(roleId);
            }
            catch (Exception ex)
            {
            }


            _handle = MSAA.GetHandle(_accessible);

            try
            {
                _defaultAction = _accessible.get_accDefaultAction(0);
            }
            catch (Exception ex)
            {
            }

            SetLocation(_accessible);
        }

        private void SetLocation(IAccessible accObject)
        {
            if (accObject != null)
            {
                int x1, y1;
                int width;
                int hieght;

                accObject.accLocation(out x1, out y1, out width, out hieght, 0);
                _location = new Rectangle(x1, y1, x1 + width, y1 + hieght);
            }
        }
    }
}
