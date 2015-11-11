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

namespace GSAKWrapper.UIControls.FormulaSolver
{
    /// <summary>
    /// Interaction logic for WaypointSelectorWindow.xaml
    /// </summary>
    public partial class WaypointSelectorWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Waypoints { get; private set; }
        private string _selectedWaypoint;
        public string SelectedWaypoint
        {
            get { return _selectedWaypoint; }
            set
            {
                SetProperty(ref _selectedWaypoint, value);
                IsWaypointSelected = _selectedWaypoint != null;
            }
        }

        private bool _isWaypointSelected;
        public bool IsWaypointSelected
        {
            get { return _isWaypointSelected; }
            set { SetProperty(ref _isWaypointSelected, value); }
        }


        public WaypointSelectorWindow()
        {
            InitializeComponent();

            Waypoints = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode)
                && !string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath)
                && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                )
            {
                Waypoints.Add(Settings.Settings.Default.ActiveGeocacheCode);
                try
                {
                    var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    if (System.IO.File.Exists(fn))
                    {
                        using (var temp = new Database.DBConSqlite(fn))
                        using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                        {
                            var lst = db.Fetch<string>("select cCode from Waypoints where cParent=@0", Settings.Settings.Default.ActiveGeocacheCode);
                            foreach (var s in lst)
                            {
                                Waypoints.Add(s);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            DataContext = this;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = SelectedWaypoint!=null;
            Close();
        }

    }
}
