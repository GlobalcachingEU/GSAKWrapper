using GSAKWrapper.FlowSequences;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GSAKWrapper.Dialogs
{
    /// <summary>
    /// Interaction logic for WindowFlowSequenceEditor.xaml
    /// </summary>
    public partial class WindowFlowSequenceEditor : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> AvailableDatabases { get; set; }

        public WindowFlowSequenceEditor()
        {
            AvailableDatabases = new List<string>();
            //todo fill

            InitializeComponent();
            DataContext = this;
        }

        private FlowSequence _selectedFlowSequence = null;
        public FlowSequence SelectedFlowSequence
        {
            get { return _selectedFlowSequence; }
            set { SetProperty(ref _selectedFlowSequence, value); IsSequenceFlowActive = SelectedFlowSequence != null; }
        }

        public bool IsSequenceFlowActive
        {
            get { return SelectedFlowSequence!=null; }
            set 
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("IsSequenceFlowActive"));
                }
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
