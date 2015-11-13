using GSAKWrapper.Commands;
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
using System.Windows.Shapes;

namespace GSAKWrapper.Dialogs
{
    /// <summary>
    /// Interaction logic for WindowAddToGeocacheCollection.xaml
    /// </summary>
    public partial class WindowAddToGeocacheCollection : Window, INotifyPropertyChanged
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

        public WindowAddToGeocacheCollection()
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

        private RelayCommand _addSingleCommand;
        public RelayCommand AddSingleCommand
        {
            get
            {
                if (_addSingleCommand == null)
                {
                    _addSingleCommand = new RelayCommand(param => AddSingle(),
                        param => SelectedCollection != null
                        && !string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode)
                        && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                        );
                }
                return _addSingleCommand;
            }
        }
        public void AddSingle()
        {
            if (SelectedCollection != null 
                && !string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode)
                && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                )
            {
                try
                {
                    var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    if (System.IO.File.Exists(fn))
                    {
                        using (var db = new Database.DBConSqlite(fn))
                        {
                            string target = "target";
                            db.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", System.IO.Path.Combine(Settings.Settings.Default.SettingsFolder, "settings.db3"), target));
                            db.ExecuteNonQuery(string.Format("insert or ignore into {1}.GeocacheCollectionItem (CollectionID, GeocacheCode, Name) select {2} as CollectionID, main.Caches.Code as GeocacheCode, main.Caches.Name as Name from main.Caches where main.Caches.Code = '{0}'", Settings.Settings.Default.ActiveGeocacheCode, target, SelectedCollection.CollectionID));
                            db.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                        }
                    }
                }
                catch
                {
                }
                Close();
            }
        }

        private RelayCommand _addUserFlaggedCommand;
        public RelayCommand AddUserFlaggedCommand
        {
            get
            {
                if (_addUserFlaggedCommand == null)
                {
                    _addUserFlaggedCommand = new RelayCommand(param => AddUserFlagged(),
                        param => SelectedCollection != null
                        && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                        );
                }
                return _addUserFlaggedCommand;
            }
        }
        public void AddUserFlagged()
        {
            if (SelectedCollection != null
                && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                )
            {
                try
                {
                    var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    if (System.IO.File.Exists(fn))
                    {
                        using (var db = new Database.DBConSqlite(fn))
                        {
                            string target = "target";
                            db.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", System.IO.Path.Combine(Settings.Settings.Default.SettingsFolder, "settings.db3"), target));
                            db.ExecuteNonQuery(string.Format("insert or ignore into {0}.GeocacheCollectionItem (CollectionID, GeocacheCode, Name) select {1} as CollectionID, main.Caches.Code as GeocacheCode, main.Caches.Name as Name from main.Caches where main.Caches.UserFlag = 1", target, SelectedCollection.CollectionID));
                            db.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                        }
                    }
                }
                catch
                {
                }
                Close();
            }
        }

    }
}
