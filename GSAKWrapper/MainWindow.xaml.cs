using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace GSAKWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal delegate void ProcessArgDelegate(String arg);
        internal static ProcessArgDelegate ProcessArg;

        public ObservableCollection<string> AvailableDatabases { get; set; }
        
        public MainWindow()
        {
            ProcessArg = delegate(String arg)
            {
                //process arguments
            };

            this.Initialized += delegate(object sender, EventArgs e)
            {
                //process arguments
                //ArgsRun.Text = (String)Application.Current.Resources[WpfSingleInstance.StartArgKey];
                try
                {
                    Application.Current.Resources.Remove(WpfSingleInstance.StartArgKey);
                }
                catch
                {

                }
            };

            AvailableDatabases = new ObservableCollection<string>();

            if (string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath))
            {
                try
                {
                    var gsak = Registry.CurrentUser.OpenSubKey(@"Software\GSAK");
                    if (gsak != null)
                    {
                        var exePath = gsak.GetValue("ExePath") as string;
                        if (exePath != null)
                        {
                            var fexePath = System.IO.Path.GetFullPath(exePath);
                            var dbPath = gsak.GetValue(fexePath) as string;
                            if (!string.IsNullOrEmpty(dbPath) && System.IO.Directory.Exists(System.IO.Path.Combine(dbPath, "data")))
                            {
                                Settings.Settings.Default.DatabaseFolderPath = System.IO.Path.Combine(dbPath, "data");
                            }
                        }
                        gsak.Dispose();
                    }

                }
                catch
                {
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath))
                {
                    if (!System.IO.Directory.Exists(Settings.Settings.Default.DatabaseFolderPath))
                    {
                        Settings.Settings.Default.DatabaseFolderPath = null;
                    }
                }
            }
            catch
            {
                Settings.Settings.Default.DatabaseFolderPath = null;
            }

            checkSelectedDatabaseExists();

            InitializeComponent();
            DataContext = this;

            Settings.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DatabaseFolderPath":
                    Dispatcher.Invoke(new Action(() =>
                    {
                        checkSelectedDatabaseExists();
                    }));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new GSAKWrapper.Dialogs.FolderPickerDialog();
            if (dlg.ShowDialog() == true)
            {
                Settings.Settings.Default.DatabaseFolderPath = dlg.SelectedPath;
            }
        }

        private void checkSelectedDatabaseExists()
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath) && System.IO.Directory.Exists(Settings.Settings.Default.DatabaseFolderPath))
                {
                    var curList = AvailableDatabases.ToList();
                    string[] dirs = System.IO.Directory.GetDirectories(Settings.Settings.Default.DatabaseFolderPath);
                    foreach (var d in dirs)
                    {
                        if (System.IO.File.Exists(System.IO.Path.Combine(d, "sqlite.db3")))
                        {
                            var sp = d.Substring(Settings.Settings.Default.DatabaseFolderPath.Length+1);
                            if (!AvailableDatabases.Contains(sp))
                            {
                                AvailableDatabases.Add(sp);
                            }
                            else
                            {
                                curList.Remove(sp);
                            }
                        }
                    }
                    foreach (var s in curList)
                    {
                        AvailableDatabases.Remove(s);
                    }
                    if (!string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase))
                    {
                        if (!System.IO.Directory.Exists(System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase))
                            || !System.IO.File.Exists(System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3"))
                            )
                        {
                            Settings.Settings.Default.SelectedDatabase = null;
                        }
                    }
                    if (string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase))
                    {
                        if (AvailableDatabases.Count > 0)
                        {
                            Settings.Settings.Default.SelectedDatabase = AvailableDatabases[0];
                        }
                    }
                }
                else
                {
                    AvailableDatabases.Clear();
                    Settings.Settings.Default.SelectedDatabase = null;
                }
            }
            catch
            {
                AvailableDatabases.Clear();
                Settings.Settings.Default.SelectedDatabase = null;
            }
        }

    }
}
