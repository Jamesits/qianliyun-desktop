using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qianliyun_Launcher
{
    // singleton used to stor temporary (per execution) state
    public class StateManager : INotifyPropertyChanged
    {
        private static readonly StateManager _instance = new StateManager();

        /// <summary>
        /// Initialises a new empty StateManager object.
        /// </summary>
        private StateManager() { }

        /// <summary>
        /// Gets the single available instance of the application StateManager object.
        /// </summary>
        public StateManager Instance => _instance;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => throw new NotImplementedException();

            remove => throw new NotImplementedException();
        }
    }
}
