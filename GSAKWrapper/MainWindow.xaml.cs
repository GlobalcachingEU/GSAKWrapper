using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

            if (Settings.Settings.ApplicationRunning)
            {
                string p = System.IO.Path.Combine(new string[] { System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GSAKWrapper" });
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                string fn = System.IO.Path.Combine(p, "settings.db3");

                //create backup first
                List<string> availableBackups = new List<string>();
                if (File.Exists(fn))
                {
                    File.Copy(fn, string.Format("{0}.{1}.bak", fn, DateTime.Now.ToString("yyyyMMddHHmmss")));

                    //keep maximum of X backups
                    availableBackups = Directory.GetFiles(p, "settings.db3.*.bak").OrderBy(x => x).ToList();
                    while (availableBackups.Count > 20)
                    {
                        File.Delete(availableBackups[0]);
                        availableBackups.RemoveAt(0);
                    }
                }

                bool dbOK;
                bool backupUsed = false;
                do
                {
                    try
                    {
                        dbOK = Settings.Settings.Default.IsStorageOK;
                        //if (availableBackups.Count > 2) dbOK = false; //test
                    }
                    catch
                    {
                        dbOK = false;
                    }
                    if (!dbOK)
                    {
                        backupUsed = true;
                        Settings.Settings.Default.PrepareReloadSettings();
                        //delete settings and move to latest
                        File.Delete(fn);
                        if (availableBackups.Count > 0)
                        {
                            File.Move(availableBackups[availableBackups.Count - 1], fn);
                            availableBackups.RemoveAt(availableBackups.Count - 1);
                        }
                        Settings.Settings.Default.ReloadSettings();
                    }

                } while (!dbOK);

                if (backupUsed)
                {
                    System.Windows.MessageBox.Show("The settings file was corrupt and a backup file is restored.", "Settings");
                }
            }

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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Localization.TranslationManager.Instance.CurrentLanguage = CultureInfo.InvariantCulture;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Localization.TranslationManager.Instance.CurrentLanguage = new CultureInfo("en-US");
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            Localization.TranslationManager.Instance.CurrentLanguage = new CultureInfo("nl-NL");
        }

    }
}
