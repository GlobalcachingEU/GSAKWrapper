using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.FlowSequences
{
    public class FlowSequence : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FlowSequenceItem> FlowSequenceItems { get; set; }

        public FlowSequence()
        {
            FlowSequenceItems = new ObservableCollection<FlowSequenceItem>();
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _database = "";
        public string Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        public override string ToString()
        {
            return Name ?? "";
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
