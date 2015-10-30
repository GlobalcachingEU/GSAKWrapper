using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.FlowSequences
{
    public class FlowSequenceItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FlowSequenceItem()
        {
        }

        private string _flowName = "";
        public string FlowName
        {
            get { return _flowName; }
            set { SetProperty(ref _flowName, value); }
        }

        private string _database = "";
        public string Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        public override string ToString()
        {
            return FlowName ?? "";
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
