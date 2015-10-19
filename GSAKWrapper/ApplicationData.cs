using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GSAKWrapper
{
    public class ApplicationData
    {
        private static ApplicationData _uniqueInstance = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow MainWindow { get; set; }

        private int _activityCounter = 0;
        public void BeginActiviy()
        {
            _activityCounter++;
            if (_activityCounter == 1)
            {
                UIIsIdle = false;
            }
        }
        public void EndActiviy()
        {
            _activityCounter--;
            if (_activityCounter == 0)
            {
                UIIsIdle = true;
            }
        }

        private bool _uiIsIdle = true;
        public bool UIIsIdle
        {
            get { return _uiIsIdle; }
            set
            {
                if (_uiIsIdle != value)
                {
                    SetProperty(ref _uiIsIdle, value);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ApplicationData()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                //use: 
                //<properties:ApplicationData x:Key="ApplicationData" />
                //{Binding Databases, Source={x:Static p:ApplicationData.Instance}}
                //{Binding Source={x:Static p:ApplicationData.Instance}, Path=ActiveDatabase.GeocacheCollection}
                System.Diagnostics.Debugger.Break();
            }
#endif
        }

        public static ApplicationData Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    _uniqueInstance = new ApplicationData();
                }
                return _uniqueInstance;
            }
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

    }
}
