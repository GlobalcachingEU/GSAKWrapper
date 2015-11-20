using GSAKWrapper.UIControls;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace GSAKWrapper.Dialogs
{
    /// <summary>
    /// Interaction logic for WindowScriptEditor.xaml
    /// </summary>
    public partial class WindowScriptEditor : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TreeViewItem _selectedTreeViewItem;
        public TreeViewItem SelectedTreeViewItem
        {
            get { return _selectedTreeViewItem; }
            set
            {
                if (SetProperty(ref _selectedTreeViewItem, value))
                {
                    //IsTreeViewItemSelected = _selectedTreeViewItem != null;
                }
            }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }


        private ICSharpCode.CodeCompletion.CSharpCompletion _completion;
        private int _tempFileCounter = 0;
        private int _scriptTypeToAdd = 0;
        
        public WindowScriptEditor()
        {           
            InitializeComponent();
            StatusText = "Ready";
            DataContext = this;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _completion = new ICSharpCode.CodeCompletion.CSharpCompletion();
            _completion.AddAssembly(Assembly.GetAssembly(typeof(MainWindow)).Location);

            var allscripts = Settings.Settings.Default.GetScriptItems();
            foreach (var scr in allscripts)
            {
                addScriptToTreeView(scr);
            }
        }

        private void addScriptToTreeView(DataTypes.ScriptItem scr)
        {
            TreeViewItem tvi = new TreeViewItem();
            tvi.FontWeight = FontWeights.Normal;
            tvi.Header = scr.Name;
            tvi.MouseDoubleClick += tvi_MouseDoubleClick;
            if (scr.ScriptType == Script.Manager.ScriptTypeFilter)
            {
                RootFilters.Items.Add(tvi);
            }
            else if (scr.ScriptType == Script.Manager.ScriptTypeAction)
            {
                RootActions.Items.Add(tvi);
            }
        }

        void tvi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenScript((sender as TreeViewItem).Header as string);
        }

        private void OpenScript(string scriptName)
        {
            foreach (ClosableTab t in tabs.Items)
            {
                if (t.Title == scriptName)
                {
                    tabs.SelectedItem = t;
                    return;
                }
            }
            _tempFileCounter++;
            var fn = System.IO.Path.Combine(Settings.Settings.Default.SettingsFolder, string.Format("script{0}.cs", _tempFileCounter));
            var scr = Settings.Settings.Default.GetScriptItem(scriptName);
            if (scr != null)
            {
                System.IO.File.WriteAllText(fn, scr.Code);

                var editor = new CodeTextEditor();
                editor.FontFamily = new FontFamily("Consolas");
                editor.FontSize = 12;
                editor.Completion = _completion;
                editor.OpenFile(fn);
                editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
                editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy();

                var tabItem = new ClosableTab();
                tabItem.Content = editor;
                tabItem.Title = scriptName;
                tabs.Items.Add(tabItem);
                tabs.SelectedItem = tabItem;
                tabItem.MouseLeave += tabs_MouseLeave;
            }
        }

        void tabs_MouseLeave(object sender, MouseEventArgs e)
        {
            var t = sender as ClosableTab;
            if (t != null)
            {
                var scr = Settings.Settings.Default.GetScriptItem(t.Title);
                if (scr != null)
                {
                    scr.Code = (t.Content as CodeTextEditor).Text;
                    Settings.Settings.Default.StoreScriptItem(scr);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _scriptTypeToAdd = Script.Manager.ScriptTypeFilter;
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
                        var allscripts = Settings.Settings.Default.GetScriptItems();
                        if ((from a in allscripts where string.Compare(a.Name, s, true) == 0 select a).Count() == 0)
                        {
                            var scr = new DataTypes.ScriptItem();
                            scr.Name = s;
                            scr.ScriptType = _scriptTypeToAdd;
                            StreamResourceInfo sri;
                            if (_scriptTypeToAdd == Script.Manager.ScriptTypeFilter)
                            {
                                sri = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Scripts/FilterScriptTemplate.cs"));
                            }
                            else
                            {
                                sri = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Scripts/ActionScriptTemplate.cs"));
                            }
                            if (sri != null)
                            {
                                using (StreamReader textStreamReader = new StreamReader(sri.Stream))
                                {
                                    scr.Code = textStreamReader.ReadToEnd();
                                }
                            }
                            Settings.Settings.Default.StoreScriptItem(scr);
                            addScriptToTreeView(scr);
                            OpenScript(scr.Name);
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedTreeViewItem != null)
            {
                var p = SelectedTreeViewItem.Parent as TreeViewItem;
                var scrName = SelectedTreeViewItem.Header as string;
                var confirmed =
                    MessageBox.Show(
                        String.Format("Do you really want to delete script {0}?", scrName),
                        "Confirm delete script",
                        MessageBoxButton.YesNo);

                if (confirmed == MessageBoxResult.Yes)
                {
                    Settings.Settings.Default.DeleteScriptItem(scrName);
                    p.Items.Remove(SelectedTreeViewItem);
                }
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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tvi = e.NewValue as TreeViewItem;
            if (tvi == RootActions || tvi == RootFilters)
            {
                tvi = null;
            }
            SelectedTreeViewItem = tvi;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var ct = tabs.SelectedItem as ClosableTab;
            if (ct != null)
            {
                var scr = Settings.Settings.Default.GetScriptItem(ct.Title);
                if (scr != null)
                {
                    StatusText = Script.Manager.Instance.CompileScript(scr.Code);
                }
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            _scriptTypeToAdd = Script.Manager.ScriptTypeAction;
            inputDialog.Show(Localization.TranslationManager.Instance.Translate("Name").ToString());
            inputDialog.DialogClosed += newDialog_DialogClosed;
        }

    }
}
