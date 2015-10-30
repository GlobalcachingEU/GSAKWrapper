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
        public List<string> AvailableActionFlows { get; set; }

        public WindowFlowSequenceEditor()
        {
            AvailableDatabases = Utils.GSAK.AvailableDatabases;
            AvailableActionFlows = (from a in UIControls.ActionBuilder.Manager.Instance.ActionFlows select a.Name).ToList();
            InitializeComponent();
            DataContext = this;
        }

        private FlowSequence _selectedFlowSequence = null;
        public FlowSequence SelectedFlowSequence
        {
            get { return _selectedFlowSequence; }
            set 
            {
                if (SetProperty(ref _selectedFlowSequence, value))
                {
                    IsSequenceFlowActive = SelectedFlowSequence != null;
                    SelectedFlowSequenceItem = null;
                }
            }
        }

        private FlowSequenceItem _selectedFlowSequenceItem = null;
        public FlowSequenceItem SelectedFlowSequenceItem
        {
            get { return _selectedFlowSequenceItem; }
            set { SetProperty(ref _selectedFlowSequenceItem, value); IsSequenceFlowItemActive = SelectedFlowSequenceItem != null; }
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

        public bool IsSequenceFlowItemActive
        {
            get { return SelectedFlowSequenceItem != null; }
            set
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("IsSequenceFlowItemActive"));
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
            inputDialog.Show("");
            inputDialog.DialogClosed += newDialog_DialogClosed;
        }

        private void newDialog_DialogClosed(object sender, EventArgs e)
        {
            inputDialog.DialogClosed -= newDialog_DialogClosed;
            if (inputDialog.DialogResult)
            {
                if ((from a in FlowSequences.Manager.Instance.FlowSequences where string.Compare(a.Name, inputDialog.InputText, true) == 0 select a).Count() == 0)
                {
                    var fs = new FlowSequence();
                    fs.Name = inputDialog.InputText;
                    fs.Database = "";

                    FlowSequences.Manager.Instance.FlowSequences.Add(fs);
                    SelectedFlowSequence = fs;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedFlowSequence != null)
            {
                FlowSequences.Manager.Instance.FlowSequences.Remove(SelectedFlowSequence);
                SelectedFlowSequence = null;
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            if (SelectedFlowSequence != null)
            {
                var fsi = new FlowSequenceItem();
                fsi.Database = "";
                fsi.FlowName = AvailableActionFlows.FirstOrDefault() ?? "";
                SelectedFlowSequence.FlowSequenceItems.Add(fsi);
                SelectedFlowSequenceItem = fsi;
            }
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (SelectedFlowSequence != null && SelectedFlowSequenceItem != null)
            {
                SelectedFlowSequence.FlowSequenceItems.Remove(SelectedFlowSequenceItem);
            }
        }
    }
}
