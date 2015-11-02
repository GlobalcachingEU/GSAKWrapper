using GSAKWrapper.Commands;
using GSAKWrapper.FlowSequences;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        internal delegate void ProcessArgDelegate(string[] args);
        internal static ProcessArgDelegate ProcessArg;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> AvailableDatabases { get; set; }

        public class ProgramArguments
        {
            public string Database { get; set; }
            public string Flow { get; set; }
            public string Sequence { get; set; }
        }

        private FlowSequence _activeFlowSequence = null;
        public FlowSequence ActiveFlowSequence
        {
            get { return _activeFlowSequence; }
            set 
            {
                if (SetProperty(ref _activeFlowSequence, value))
                {
                    IsFlowSequenceActive = ActiveFlowSequence != null;
                }
            }
        }

        public bool IsFlowSequenceActive
        {
            get { return ActiveFlowSequence != null; }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsFlowSequenceActive"));
                }
            }
        }
        
        public MainWindow()
        {
            ProcessArg = delegate(string[] args)
            {
                //process arguments
                System.Windows.MessageBox.Show("Please close GSAKWrapper before using the GSAK macro to execute GSAKWrapper.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    while (availableBackups.Count > 10)
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
            }
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            var pa = ProgressCommandLineArguments(Environment.GetCommandLineArgs());
            if (!string.IsNullOrEmpty(pa.Database))
            {
                var d = (from a in AvailableDatabases where string.Compare(a, pa.Database, true) == 0 select a).FirstOrDefault();
                if (d != null)
                {
                    Settings.Settings.Default.SelectedDatabase = d;
                }
                else
                {
                    System.Windows.MessageBox.Show(string.Format("Database '{0}' not found", pa.Database), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (!string.IsNullOrEmpty(pa.Flow))
            {
                var fl = (from a in UIControls.ActionBuilder.Manager.Instance.ActionFlows where string.Compare(a.Name, pa.Flow, true) == 0 select a).FirstOrDefault();
                if (fl != null)
                {
                    flowBuilder.ActiveActionFlow = fl;
                    await UIControls.ActionBuilder.Manager.Instance.RunActionFow(fl);
                    Close();
                }
                else
                {
                    System.Windows.MessageBox.Show(string.Format("Flow '{0}' not found", pa.Database), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else if (!string.IsNullOrEmpty(pa.Sequence))
            {
                var sq = (from a in FlowSequences.Manager.Instance.FlowSequences where string.Compare(a.Name, pa.Sequence, true) == 0 select a).FirstOrDefault();
                if (sq != null)
                {
                    ActiveFlowSequence = sq;
                    await FlowSequences.Manager.Instance.RunFowSequence(sq);
                    Close();
                }
                else
                {
                    System.Windows.MessageBox.Show(string.Format("Sequence '{0}' not found", pa.Database), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {

#if DEBUG
                //if (Settings.Settings.Default.VersionCheckedAtDay != DateTime.Now.Day)
#else
                if (Settings.Settings.Default.VersionCheckedAtDay != DateTime.Now.Day)
#endif
                {
                    var thrd = new Thread(new ThreadStart(this.CheckForNewVersionThreadMethod));
                    thrd.IsBackground = true;
                    thrd.Start();
                }
            }
        }

        private ProgramArguments ProgressCommandLineArguments(string[] args)
        {
            var result = new ProgramArguments();
            foreach (var p in args)
            {
                var parts = p.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    if (parts[0] == "-d")
                    {
                        result.Database = parts[1];
                    }
                    else if (parts[0] == "-f")
                    {
                        result.Flow = parts[1];
                    }
                    else if (parts[0] == "-s")
                    {
                        result.Sequence = parts[1];
                    }
                }
            }
            return result;
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
#if DEBUG
                    //var s = webClient.DownloadString("https://raw.githubusercontent.com/GlobalcachingEU/GSAKWrapper/master/Release");
                    Thread.Sleep(2000);
                    var s = @"<?xml version=""1.0"" encoding=""utf-8""?>
<items>
  <item name=""version"" value=""0.1.1.0"" />
  <item name=""url"" value=""https://github.com/GlobalcachingEU/GSAKWrapper/releases"" />
</items>
";
#else
                    var s = webClient.DownloadString("https://raw.githubusercontent.com/GlobalcachingEU/GSAKWrapper/master/Release");
#endif
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
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            newVersionUrl.Visibility = System.Windows.Visibility.Visible;
                        }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
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

        private void Button_EditFlowSequences(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.WindowFlowSequenceEditor();
            dlg.ShowDialog();
            FlowSequences.Manager.Instance.Save();
        }

        private AsyncDelegateCommand _executeSequenceCommand;
        public AsyncDelegateCommand ExecuteSequenceCommand
        {
            get
            {
                if (_executeSequenceCommand == null)
                {
                    _executeSequenceCommand = new AsyncDelegateCommand(param => ExecuteActiveFlowAsync(),
                        param => ActiveFlowSequence!=null);
                }
                return _executeSequenceCommand;
            }
        }
        public async Task ExecuteActiveFlowAsync()
        {
            if (ActiveFlowSequence != null)
            {
                await FlowSequences.Manager.Instance.RunFowSequence(ActiveFlowSequence);
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

        private void menua47_Click(object sender, RoutedEventArgs e)
        {
            //default macro
            System.Diagnostics.Process.Start(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Settings.Settings.Default.ApplicationPath), "GSAKWrapper.gsk"));
            e.Handled = true;
        }

        private void menux48_Click(object sender, RoutedEventArgs e)
        {
            if (flowBuilder.ActiveActionFlow != null)
            {
                //create a flow specific macro
                var txt = GetTemplateGSKFile();
                try
                {
                    var fn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Settings.Settings.Default.ApplicationPath),"GSAKWrapper - flow.gsk");
                    if (!string.IsNullOrEmpty(txt))
                    {
                        txt = txt.Replace("# MacFileName = GSAKWrapper.gsk", string.Format("# MacFileName = GSAKWrapper - {0}.gsk", flowBuilder.ActiveActionFlow.Name));
                        txt = txt.Replace("$execParam=\"-d=\" + Quote($_CurrentDatabase)", string.Format("$execParam\"-d=\" + Quote($_CurrentDatabase) + \" -f=\" + Quote(\"{0}\")", flowBuilder.ActiveActionFlow.Name));
                    }
                    System.IO.File.WriteAllText(fn, txt);
                    System.Diagnostics.Process.Start(fn);
                }
                catch
                {
                }
            }
        }

        private void menux49_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveFlowSequence != null)
            {
                //create a sequence specific macro
                var txt = GetTemplateGSKFile();
                try
                {
                    var fn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Settings.Settings.Default.ApplicationPath), "GSAKWrapper - sequence.gsk");
                    if (!string.IsNullOrEmpty(txt))
                    {
                        txt = txt.Replace("# MacFileName = GSAKWrapper.gsk", string.Format("# MacFileName = GSAKWrapper - {0}.gsk", ActiveFlowSequence.Name));
                        txt = txt.Replace("$execParam=\"-d=\" + Quote($_CurrentDatabase)", string.Format("$execParam\"-d=\" + Quote($_CurrentDatabase) + \" -s=\" + Quote(\"{0}\")", ActiveFlowSequence.Name));
                    }
                    System.IO.File.WriteAllText(fn, txt);
                    System.Diagnostics.Process.Start(fn);
                }
                catch
                {
                }
            }
        }

        private string GetTemplateGSKFile()
        {
            string result = null;
            try
            {
                var fn = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Settings.Settings.Default.ApplicationPath), "GSAKTemplateWrapper.gsk");
                result = System.IO.File.ReadAllText(fn);
            }
            catch
            {
            }
            return result;
        }
    }
}
