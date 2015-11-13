using GSAKWrapper.Excel;
using GSAKWrapper.UIControls;
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
    /// Interaction logic for WindowExportExcelSettings.xaml
    /// </summary>
    public partial class WindowExportExcelSettings : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Sheet> Sheets { get; private set; }
        public ObservableCollection<CheckedListItem<PropertyItem>> Fields { get; private set; }

        private UIControls.ActionBuilder.ActionExportExcel.ExportExcelSettings _activeSettings = null;

        private bool _initializingSheet = false; 
        private Sheet _activeSheet;
        public Sheet ActiveSheet
        {
            get { return _activeSheet; }
            set
            {
                _initializingSheet = true;
                SetProperty(ref _activeSheet, value);
                if (_activeSheet == null)
                {
                    foreach (var f in Fields)
                    {
                        f.IsChecked = false;
                    }
                }
                else
                {
                    fillFields();
                }
                _initializingSheet = false;
                IsRemoveSheetEnabled = _activeSheet != null;
            }
        }

        private CheckedListItem<PropertyItem> _selectedField;
        public CheckedListItem<PropertyItem> SelectedField
        {
            get { return _selectedField; }
            set
            {
                SetProperty(ref _selectedField, value);
                IsMoveUpEnabled = _selectedField != null && _selectedField.IsChecked && Fields.IndexOf(_selectedField) > 0;
                IsMoveDownEnabled = _selectedField != null && _selectedField.IsChecked && Fields.IndexOf(_selectedField) < Fields.Count - 1 && Fields[Fields.IndexOf(_selectedField) + 1].IsChecked;
            }

        }

        private bool _isRemoveSheetEnabled = false;
        public bool IsRemoveSheetEnabled
        {
            get { return _isRemoveSheetEnabled; }
            set { SetProperty(ref _isRemoveSheetEnabled, value); }
        }
        private bool _isMoveUpEnabled = false;
        public bool IsMoveUpEnabled
        {
            get { return _isMoveUpEnabled; }
            set { SetProperty(ref _isMoveUpEnabled, value); }
        }
        private bool _isMoveDownEnabled = false;
        public bool IsMoveDownEnabled
        {
            get { return _isMoveDownEnabled; }
            set { SetProperty(ref _isMoveDownEnabled, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        public WindowExportExcelSettings()
        {
            InitializeComponent();
        }

        public WindowExportExcelSettings(UIControls.ActionBuilder.ActionExportExcel.ExportExcelSettings settings)
        {
            _activeSettings = settings;
            InitializeComponent();

            Sheets = new ObservableCollection<Sheet>();
            Fields = new ObservableCollection<CheckedListItem<PropertyItem>>();

            FileName = settings.FileName;
            foreach (var s in settings.Sheets)
            {
                var ns = new Excel.Sheet();
                ns.Name = s.Name;
                foreach (var item in s.SelectedItems)
                {
                    ns.SelectedItems.Add(item);
                }
                Sheets.Add(ns);
            }

            foreach (var fi in PropertyItem.PropertyItems)
            {
                CheckedListItem<PropertyItem> cli = new CheckedListItem<PropertyItem>(fi, false);
                Fields.Add(cli);
                cli.PropertyChanged += cli_PropertyChanged;
            }

            FileName = settings.FileName;
            if (Sheets.Count > 0)
            {
                ActiveSheet = Sheets[0];
            }

            DataContext = this;
        }

        void cli_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_initializingSheet)
            {
                if (_activeSheet != null && sender is CheckedListItem<PropertyItem>)
                {
                    CheckedListItem<PropertyItem> pi = sender as CheckedListItem<PropertyItem>;
                    if (pi.IsChecked && !_activeSheet.SelectedItems.Contains(pi.Item))
                    {
                        _activeSheet.SelectedItems.Add(pi.Item);
                    }
                    else if (!pi.IsChecked && _activeSheet.SelectedItems.Contains(pi.Item))
                    {
                        _activeSheet.SelectedItems.Remove(pi.Item);
                    }
                }
                fillFields();
                SelectedField = sender as CheckedListItem<PropertyItem>;
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

        private void fillFields()
        {
            if (_activeSheet != null)
            {
                int insertAt = 0;
                foreach (var f in _activeSheet.SelectedItems)
                {
                    CheckedListItem<PropertyItem> fitem = (from a in Fields where a.Item == f select a).FirstOrDefault();
                    int index = Fields.IndexOf(fitem);
                    if (index > insertAt)
                    {
                        Fields.RemoveAt(index);
                        Fields.Insert(insertAt, fitem);
                        fitem.IsChecked = true;
                    }
                    else
                    {
                        fitem.IsChecked = true;
                    }
                    insertAt++;
                }
                for (int i = insertAt; i < Fields.Count; i++)
                {
                    Fields[i].IsChecked = false;
                }
            }
            else
            {
                for (int i = 0; i < Fields.Count; i++)
                {
                    Fields[i].IsChecked = false;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Microsoft Excel File (.xlsx)|*.xlsx"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                FileName = dlg.FileName;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
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
                        Sheet sh = new Sheet();
                        sh.Name = s;
                        Sheets.Add(sh);
                        ActiveSheet = sh;
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ActiveSheet != null)
            {
                Sheets.Remove(ActiveSheet);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (ActiveSheet != null && SelectedField != null && SelectedField.IsChecked && Fields.IndexOf(SelectedField) > 0)
            {
                var rmb = SelectedField;
                int index = Fields.IndexOf(rmb);
                Fields.RemoveAt(index);
                Fields.Insert(index - 1, rmb);
                SelectedField = rmb;

                ActiveSheet.SelectedItems.Clear();
                foreach (var fi in Fields)
                {
                    if (fi.IsChecked)
                    {
                        ActiveSheet.SelectedItems.Add(fi.Item);
                    }
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (ActiveSheet != null && SelectedField != null && SelectedField.IsChecked && Fields.IndexOf(SelectedField) < Fields.Count - 1 && Fields[Fields.IndexOf(SelectedField) + 1].IsChecked)
            {
                var rmb = SelectedField;
                int index = Fields.IndexOf(rmb);
                Fields.RemoveAt(index);
                Fields.Insert(index + 1, rmb);
                SelectedField = rmb;

                ActiveSheet.SelectedItems.Clear();
                foreach (var fi in Fields)
                {
                    if (fi.IsChecked)
                    {
                        ActiveSheet.SelectedItems.Add(fi.Item);
                    }
                }
            }

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            _activeSettings.FileName = FileName;
            _activeSettings.Sheets.Clear();
            foreach (var sheet in Sheets)
            {
                _activeSettings.Sheets.Add(sheet);
            }
            Close();
        }
    }
}
