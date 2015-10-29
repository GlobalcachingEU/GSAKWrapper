using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Threading;
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
using System.Xml;

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

#if DEBUG
                Localization.TranslationManager.Instance.CreateOrUpdateXmlFiles();
#endif
                Settings.Settings.Default.ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Settings.Settings.Default.ApplicationPath = Assembly.GetExecutingAssembly().Location;

                AvailableDatabases = new ObservableCollection<string>();

                if (string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath))
                {
                    Settings.Settings.Default.DatabaseFolderPath = Utils.GSAK.DatabaseFolderPath;
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

                Settings.Settings.Default.NewVersionChecked = false;
                Settings.Settings.Default.PropertyChanged += Default_PropertyChanged;

                if (Settings.Settings.Default.ReleaseVersion > Settings.Settings.Default.ApplicationVersion)
                {
                    newVersionUrl.Visibility = System.Windows.Visibility.Visible;
                }

                if (Settings.Settings.Default.VersionCheckedAtDay != DateTime.Now.Day)
                {
                    var thrd = new Thread(new ThreadStart(this.CheckForNewVersionThreadMethod));
                    thrd.IsBackground = true;
                    thrd.Start();
                }
            }
        }

        public void CheckForNewVersionThreadMethod()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(
                       delegate
                       {
                           return true;
                       });
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";
                    var s = webClient.DownloadString("https://raw.githubusercontent.com/GlobalcachingEU/GSAKWrapper/master/Release");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(s);
                    XmlElement root = doc.DocumentElement;
                    XmlNodeList strngs = root.SelectNodes("item");
                    if (strngs != null)
                    {
                        foreach (XmlNode sn in strngs)
                        {
                            var n = sn.Attributes["name"].InnerText;
                            var v = sn.Attributes["value"].InnerText;
                            if (n == "version")
                            {
                                Settings.Settings.Default.ReleaseVersion = Version.Parse(v);
                            }
                            else if (n == "url")
                            {
                                Settings.Settings.Default.ReleaseUrl = v;
                            }
                        }
                    }
                    Settings.Settings.Default.VersionCheckedAtDay = DateTime.Now.Day;
                    Settings.Settings.Default.NewVersionChecked = true;
                }
            }
            catch
            {
            }
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
                case "NewVersionChecked":

                    if (Settings.Settings.Default.ReleaseVersion > Settings.Settings.Default.ApplicationVersion)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            newVersionUrl.Visibility = System.Windows.Visibility.Visible;
                        }));
                    }
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
                    var newList = Utils.GSAK.AvailableDatabases;
                    foreach (var sp in newList)
                    {
                        if (!AvailableDatabases.Contains(sp))
                        {
                            AvailableDatabases.Add(sp);
                        }
                        else
                        {
                            curList.Remove(sp);
                        }
                    }
                    foreach (var s in curList)
                    {
                        AvailableDatabases.Remove(s);
                    }
                    if (!string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase))
                    {
                        if (!AvailableDatabases.Contains(Settings.Settings.Default.SelectedDatabase))
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

        private void menux27_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.AboutWindow dlg = new Dialogs.AboutWindow(this);
            dlg.ShowDialog();
        }

        private void menua37_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/GlobalcachingEU/GSAKWrapper/wiki");
            }
            catch
            {
            }
        }

        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            Localization.TranslationManager.Instance.CurrentLanguage = new CultureInfo("de-DE");
        }

        private void MenuItem_Click_16(object sender, RoutedEventArgs e)
        {
            Localization.TranslationManager.Instance.CurrentLanguage = new CultureInfo("fr-FR");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
