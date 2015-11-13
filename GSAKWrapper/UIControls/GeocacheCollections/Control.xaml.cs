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

        public ObservableCollection<DataTypes.GeocacheCollection> AvailableCollections { get; set; }

        private List<DataTypes.GeocacheCollectionItem> _availableCollectionItems;
        public List<DataTypes.GeocacheCollectionItem> AvailableCollectionItems
        {
            get { return _availableCollectionItems; }
            set { SetProperty(ref _availableCollectionItems, value); }
        }

        private DataTypes.GeocacheCollection _selectedCollection = null;
        public DataTypes.GeocacheCollection SelectedCollection
        {
            get { return _selectedCollection; }
            set 
            {
                if (SetProperty(ref _selectedCollection, value))
                {
                    IsCollectionSelected = SelectedCollection != null;
                    if (_selectedCollection != null)
                    {
                        AvailableCollectionItems = Settings.Settings.Default.GetCollectionItems(_selectedCollection.CollectionID);
                    }
                    else
                    {
                        AvailableCollectionItems = null;
                    }
                }
            }
        }

        private DataTypes.GeocacheCollectionItem _selectedCollectionItem = null;
        public DataTypes.GeocacheCollectionItem SelectedCollectionItem
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

            AvailableCollections = new ObservableCollection<DataTypes.GeocacheCollection>();
            if (Settings.Settings.ApplicationRunning)
            {
                var cl = Settings.Settings.Default.GetGeocacheCollections();
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
                Settings.Settings.Default.DeleteGeocacheCollection(SelectedCollection.CollectionID);
                AvailableCollections.Remove(SelectedCollection);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (SelectedCollection != null && SelectedCollectionItem!=null)
            {
                Settings.Settings.Default.DeleteGeocacheCollectionItem(SelectedCollectionItem.CollectionID, SelectedCollectionItem.GeocacheCode);
                AvailableCollectionItems = Settings.Settings.Default.GetCollectionItems(SelectedCollection.CollectionID);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inputDialog.Show(Localization.TranslationManager.Instance.Translate("Name").ToString());
            inputDialog.DialogClosed += newDialog_DialogClosed;
        }

        private void newDialog_DialogClosed(object sender, EventArgs e)
        {
            inputDialog.DialogClosed -= newDialog_DialogClosed;
            if (inputDialog.DialogResult)
            {
                if (!string.IsNullOrEmpty(inputDialog.InputText))
                {
                    string s = inputDialog.InputText.Trim();
                    if (s.Length > 0)
                    {
                        var c = Settings.Settings.Default.GetCollection(s);
                        if (c == null)
                        {
                            c = Settings.Settings.Default.GetCollection(s, createIfNotExists: true);
                            if (c != null)
                            {
                                AvailableCollections.Add(c);
                                SelectedCollection = c;
                            }
                        }
                    }
                }
            }
        }

    }
}
