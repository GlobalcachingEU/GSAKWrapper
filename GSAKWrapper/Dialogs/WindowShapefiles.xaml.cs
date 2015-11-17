using GSAKWrapper.DataTypes;
using GSAKWrapper.Shapefiles;
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
    /// Interaction logic for WindowShapefiles.xaml
    /// </summary>
    public partial class WindowShapefiles : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<ShapeFileInfo> _listData;

        private ShapeFileInfo _selectedRecord;
        public ShapeFileInfo SelectedRecord
        {
            get { return _selectedRecord; }
            set
            {
                if (SetProperty(ref _selectedRecord, value))
                {
                    IsRecordSelected = _selectedRecord != null;
                }
            }
        }

        public bool IsRecordSelected
        {
            get { return _selectedRecord != null; }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsRecordSelected"));
                }
            }
        }

        public WindowShapefiles()
        {
            InitializeComponent();

            if (Settings.Settings.ApplicationRunning)
            {
                _listData = new ObservableCollection<ShapeFileInfo>();
                var sfls = Settings.Settings.Default.GetShapeFileItems();
                foreach (var si in sfls)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(si.FileName) && System.IO.File.Exists(si.FileName))
                        {
                            var sf = new ShapeFileInfo();
                            sf.Enabled = si.Enabled != 0;
                            sf.Filename = si.FileName;
                            sf.TableName = si.TableName;
                            sf.TCoord = si.CoordType;
                            sf.TArea = si.AreaType;
                            sf.Prefix = si.NamePrefix;
                            sf.Encoding = si.Encoding;

                            using (ShapeFile s = new ShapeFile(sf.Filename))
                            {
                                sf.TableNames = s.GetFields().ToList();
                            }
                            sf.TAreas = Enum.GetNames(typeof(AreaType)).ToList();
                            sf.TCoords = Enum.GetNames(typeof(ShapeFile.CoordType)).ToList();
                            sf.TEncodings = new List<string>() { "utf-8", "ISO-8859-1" };

                            _listData.Add(sf);
                        }
                    }
                    catch
                    {
                    }
                }

                DataContext = this;
                dataGrid.ItemsSource = _listData;
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

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            int i = dataGrid.SelectedIndex;
            _listData[i].TableName = (sender as ComboBox).SelectedItem as string;
        }

        private void ComboBox_DropDownClosed_1(object sender, EventArgs e)
        {
            int i = dataGrid.SelectedIndex;
            _listData[i].TCoord = (sender as ComboBox).SelectedItem as string;

        }

        private void ComboBox_DropDownClosed_2(object sender, EventArgs e)
        {
            int i = dataGrid.SelectedIndex;
            _listData[i].TArea = (sender as ComboBox).SelectedItem as string;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".shp"; // Default file extension
            dlg.Filter = "Shapefile (.shp)|*.shp"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                var sf = new ShapeFileInfo();
                sf.Enabled = true;
                sf.Filename = dlg.FileName;
                sf.TableName = "";
                sf.TCoord = ShapeFile.CoordType.WGS84.ToString();
                sf.TArea = AreaType.Other.ToString();
                sf.Prefix = "";
                sf.Encoding = "utf-8";
                sf.TEncodings = new List<string>() { "utf-8", "ISO-8859-1" };

                using (ShapeFile s = new ShapeFile(sf.Filename))
                {
                    sf.TableNames = s.GetFields().ToList();
                }
                sf.TAreas = Enum.GetNames(typeof(AreaType)).ToList();
                sf.TCoords = Enum.GetNames(typeof(ShapeFile.CoordType)).ToList();

                _listData.Add(sf);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var sfls = new List<ShapefileItem>();
            foreach (ShapeFileInfo sf in _listData)
            {
                if (!string.IsNullOrEmpty(sf.TableName))
                {
                    var item = new ShapefileItem();
                    item.Enabled = sf.Enabled ? 1 : 0;
                    item.Encoding = sf.Encoding;
                    item.FileName = sf.Filename;
                    item.NamePrefix = sf.Prefix;
                    item.TableName = sf.TableName;
                    item.AreaType = sf.TArea;
                    item.CoordType = sf.TCoord;
                    sfls.Add(item);
                }
            }
            Settings.Settings.Default.StoreShapeFileItems(sfls);
            Manager.Instance.Initialize();
            DialogResult = true;
            Close();
        }

        private void ComboBox_DropDownClosed_3(object sender, EventArgs e)
        {
            int i = dataGrid.SelectedIndex;
            _listData[i].Encoding = (sender as ComboBox).SelectedItem as string;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dlg = new WindowShapeFileTest(SelectedRecord);
            dlg.ShowDialog();
        }
    }
}
