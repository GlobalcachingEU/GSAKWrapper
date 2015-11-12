using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GSAKWrapper.UIControls.GeocacheCollections
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Collections.GeocacheCollection> AvailableCollections { get; set; }

        private List<Collections.GeocacheCollectionItem> _availableCollectionItems;
        public List<Collections.GeocacheCollectionItem> AvailableCollectionItems
        {
            get { return _availableCollectionItems; }
            set { SetProperty(ref _availableCollectionItems, value); }
        }

        private Collections.GeocacheCollection _selectedCollection = null;
        public Collections.GeocacheCollection SelectedCollection
        {
            get { return _selectedCollection; }
            set 
            {
                if (SetProperty(ref _selectedCollection, value))
                {
                    IsCollectionSelected = SelectedCollection != null;
                    if (_selectedCollection != null)
                    {
                        AvailableCollectionItems = Collections.Manager.Instance.GetCollectionItems(_selectedCollection.CollectionID);
                    }
                    else
                    {
                        AvailableCollectionItems = null;
                    }
                }
            }
        }

        private Collections.GeocacheCollectionItem _selectedCollectionItem = null;
        public Collections.GeocacheCollectionItem SelectedCollectionItem
        {
            get { return _selectedCollectionItem; }
            set
            {
                if (SetProperty(ref _selectedCollectionItem, value))
                {
                    IsCollectionItemSelected = SelectedCollectionItem != null;
                    if (SelectedCollectionItem != null)
                    {
                        //make it active in GSAK, but only if GSAK already running, but wrapper not executed from within gsak
                        if (Utils.GSAK.IsGSAKRunning && !Settings.Settings.Default.ExecutedWithParameters)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(System.IO.Path.Combine(Settings.Settings.Default.GSAKExecutablePath, "gsak.exe"), string.Format("gsak://%FF/Search/{0}", SelectedCollectionItem.GeocacheCode));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public bool IsCollectionSelected
        {
            get { return SelectedCollection != null; }
            set
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("IsCollectionSelected"));
                }
            }
        }

        public bool IsCollectionItemSelected
        {
            get { return SelectedCollectionItem != null; }
            set
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("IsCollectionItemSelected"));
                }
            }
        }
        
        public Control()
        {
            InitializeComponent();

            AvailableCollections = new ObservableCollection<Collections.GeocacheCollection>();
            if (Settings.Settings.ApplicationRunning)
            {
                var cl = Collections.Manager.Instance.GetGeocacheCollections();
                foreach (var c in cl)
                {
                    AvailableCollections.Add(c);
                }
            }

            DataContext = this;
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            bool result = false;
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                result = true;
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
            return result;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedCollection != null)
            {
                Collections.Manager.Instance.DeleteGeocacheCollection(SelectedCollection.CollectionID);
                AvailableCollections.Remove(SelectedCollection);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (SelectedCollection != null && SelectedCollectionItem!=null)
            {
                Collections.Manager.Instance.DeleteGeocacheCollectionItem(SelectedCollectionItem.CollectionID, SelectedCollectionItem.GeocacheCode);
                AvailableCollectionItems = Collections.Manager.Instance.GetCollectionItems(SelectedCollection.CollectionID);
            }
        }

    }
}
