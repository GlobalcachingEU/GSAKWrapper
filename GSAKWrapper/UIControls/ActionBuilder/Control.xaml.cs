using GSAKWrapper.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl, IDisposable, INotifyPropertyChanged
    {
        private object _clickedObject = null;
        private Point _clickPosition;

        public event PropertyChangedEventHandler PropertyChanged;

        public Control()
        {
            InitializeComponent();

            DataContext = this;

            Localization.TranslationManager.Instance.LanguageChanged += Instance_LanguageChanged;

            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                Type[] types = asm.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsClass
                        && t != typeof(ActionImplementationText)
                        && t != typeof(ActionImplementationYesNo)
                        && t != typeof(ActionImplementationNumericValue)
                        && t != typeof(ActionImplementationDate)
                        && t != typeof(ActionExport)
                        && (t.BaseType == typeof(ActionImplementationCondition) 
                            || t.BaseType == typeof(ActionImplementationText)
                            || t.BaseType == typeof(ActionImplementationYesNo)
                            || t.BaseType == typeof(ActionImplementationNumericValue)
                            || t.BaseType == typeof(ActionImplementationDate)
                            || t.BaseType == typeof(ActionImplementationExecuteOnce)
                            || t.BaseType == typeof(ActionImplementationAction)
                            || t.BaseType == typeof(ActionExport)
                            )
                        )
                    {
                        ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
                        ActionImplementation obj = (ActionImplementation)constructor.Invoke(null);

                        //exception for the start
                        if (obj is ActionStart)
                        {
                            //skip. auto
                        }
                        else
                        {
                            Button b = new Button();
                            b.Tag = obj;
                            b.Height = 25;
                            b.PreviewMouseLeftButtonDown += b_PreviewMouseLeftButtonDown;
                            b.PreviewMouseMove += b_PreviewMouseMove;
                            b.PreviewMouseLeftButtonUp += b_PreviewMouseLeftButtonUp;
                            switch (obj.SearchTypeTarget)
                            {
                                case SearchType.Action:
                                    actionPanel.Children.Add(b);
                                    break;
                                case SearchType.ExecuteOnce:
                                    oncePanel.Children.Add(b);
                                    break;
                                case SearchType.Attributes:
                                    conditionAttributesPanel.Children.Add(b);
                                    break;
                                case SearchType.Children:
                                    conditionChildrenPanel.Children.Add(b);
                                    break;
                                case SearchType.Custom:
                                    conditionCustomPanel.Children.Add(b);
                                    break;
                                case SearchType.Dates:
                                    conditionDatesPanel.Children.Add(b);
                                    break;
                                case SearchType.Logs:
                                    conditionLogsPanel.Children.Add(b);
                                    break;
                                case SearchType.Other:
                                    conditionOtherPanel.Children.Add(b);
                                    break;
                                case SearchType.Where:
                                    conditionWherePanel.Children.Add(b);
                                    break;
                                case SearchType.Extra:
                                    conditionExtraPanel.Children.Add(b);
                                    break;
                                case SearchType.General:
                                default:
                                    conditionPanel.Children.Add(b);
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            Instance_LanguageChanged(this, EventArgs.Empty);

            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActionBuilderActiveFlowName))
            {
                ActiveActionFlow = (from a in Manager.Instance.ActionFlows where a.Name == Settings.Settings.Default.ActionBuilderActiveFlowName select a).FirstOrDefault();
            }
        }

        private void SaveData()
        {
            actionBuilderEditor1.CommitData();
            Manager.Instance.Save();
        }

        private AsyncDelegateCommand _executeCommand;
        public AsyncDelegateCommand ExecuteCommand
        {
            get
            {
                if (_executeCommand==null)
                {
                    _executeCommand = new AsyncDelegateCommand(param => ExecuteActiveFlowAsync(),
                        param => IsFlowActive);
                }
                return _executeCommand;
            }
        }
        public async Task ExecuteActiveFlowAsync()
        {
            if (ActiveActionFlow!=null)
            {
                SaveData();
                await Manager.Instance.RunActionFow(ActiveActionFlow);
                actionBuilderEditor1.UpdateLabels();
            }
        }

        void b_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Type t = (sender as Button).Tag.GetType();
            ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
            ActionImplementation obj = (ActionImplementation)constructor.Invoke(null);
            obj.ID = Guid.NewGuid().ToString("N");
            ActiveActionFlow.Actions.Add(obj);
            actionBuilderEditor1.AddActionControl(obj);
            _clickedObject = null;
        }

        void b_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_clickedObject == sender)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Button b = sender as Button;
                    if (b != null)
                    {
                        Point p = e.GetPosition(b);
                        if (p.X != _clickPosition.X || p.Y != _clickPosition.Y)
                        {
                            ActionImplementation ai = b.Tag as ActionImplementation;
                            if (ai != null)
                            {
                                _clickedObject = null;
                                e.Handled = true;
                                DragDrop.DoDragDrop(b, b.Tag.GetType().ToString(), DragDropEffects.Move);
                            }
                        }
                    }
                }
            }
        }

        void b_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _clickedObject = sender;
            _clickPosition = e.GetPosition(sender as Button);
        }

        void Instance_LanguageChanged(object sender, EventArgs e)
        {
            var containerPanels = new List<Panel>() { conditionPanel, actionPanel, oncePanel, conditionDatesPanel, conditionOtherPanel, conditionLogsPanel, conditionChildrenPanel, conditionAttributesPanel, conditionWherePanel, conditionCustomPanel, conditionExtraPanel };
            foreach (var p in containerPanels)
            {
                var pnlButtons = new List<Button>();
                foreach (UIElement cnt in p.Children)
                {
                    if (cnt is Button)
                    {
                        Button b = cnt as Button;
                        if (b.Tag is ActionImplementation)
                        {
                            b.Content = Localization.TranslationManager.Instance.Translate((b.Tag as ActionImplementation).Name);
                            pnlButtons.Add(b);
                        }
                    }
                }
                p.Children.Clear();
                var lst = pnlButtons.OrderBy(x => x.Content);
                foreach (var a in lst)
                {
                    p.Children.Add(a);
                }
            }
            if (ActiveActionFlow != null)
            {
                foreach (var a in ActiveActionFlow.Actions)
                {
                    a.SelectedLanguageChanged();
                }
            }
        }

        public void Dispose()
        {
            actionBuilderEditor1.CommitData();
            actionBuilderEditor1.Clear(null);
            SaveData();
            Localization.TranslationManager.Instance.LanguageChanged -= Instance_LanguageChanged;
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

        public ActionFlow _activeActionFlow = null;
        public ActionFlow ActiveActionFlow
        {
            get { return _activeActionFlow; }
            set
            {
                if (SetProperty(ref _activeActionFlow, value))
                {
                    Settings.Settings.Default.ActionBuilderActiveFlowName = _activeActionFlow == null ? "" : _activeActionFlow.Name;
                }
                IsFlowActive = _activeActionFlow != null;
            }
        }

        private bool _isFlowActive = false;
        public bool IsFlowActive
        {
            get { return _isFlowActive; }
            set { SetProperty(ref _isFlowActive, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inputDialog.Show(Localization.TranslationManager.Instance.Translate("Name").ToString());
            inputDialog.DialogClosed +=newDialog_DialogClosed;
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
                        if ((from a in Manager.Instance.ActionFlows where string.Compare(a.Name, s, true) == 0 select a).Count() == 0)
                        {
                            ActionFlow af = new ActionFlow();
                            af.Name = inputDialog.InputText;
                            Settings.Settings.Default.ActionBuilderFlowID++;
                            af.ID = string.Format("actbuildf{0}", Settings.Settings.Default.ActionBuilderFlowID);
                            af.Actions = new List<ActionImplementation>();
                            var obj = new ActionStart();
                            obj.ID = Guid.NewGuid().ToString("N");
                            af.Actions.Add(obj);
                            Manager.Instance.ActionFlows.Add(af);
                            ActiveActionFlow = af;

                            SaveData();
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveActionFlow == null)
            {
                actionBuilderEditor1.CommitData();
                actionBuilderEditor1.Clear(null);
            }
            else
            {
                actionBuilderEditor1.CommitData();
                actionBuilderEditor1.Clear(ActiveActionFlow.Actions);
                ActionImplementation startAction = (from a in ActiveActionFlow.Actions where a is ActionStart select a).FirstOrDefault();
                if (startAction != null)
                {
                    if (startAction.UIActionControl != null)
                    {
                        startAction.UIActionControl.Title.Content = string.Format("{0}\r\n{1}", Localization.TranslationManager.Instance.Translate("Start"), ActiveActionFlow.Name);
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ActiveActionFlow != null)
            {
                if (System.Windows.MessageBox.Show(string.Format(Localization.TranslationManager.Instance.Translate("Delete_flow_") as string, ActiveActionFlow.Name), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ActionFlow af = ActiveActionFlow;
                    ActiveActionFlow = null;
                    Manager.Instance.ActionFlows.Remove(af);
                    SaveData();
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ActiveActionFlow != null)
            {
                inputDialog.Show(Localization.TranslationManager.Instance.Translate("Name").ToString());
                inputDialog.DialogClosed += renameDialog_DialogClosed;
            }
        }

        private void renameDialog_DialogClosed(object sender, EventArgs e)
        {
            inputDialog.DialogClosed -= renameDialog_DialogClosed;
            if (ActiveActionFlow != null)
            {
                if (inputDialog.DialogResult)
                {
                    if (!string.IsNullOrEmpty(inputDialog.InputText))
                    {
                        string s = inputDialog.InputText.Trim();
                        if (s.Length > 0)
                        {
                            if ((from a in Manager.Instance.ActionFlows where string.Compare(a.Name, s, true) == 0 select a).Count() == 0)
                            {
                                ActiveActionFlow.Name = s;
                                SaveData();
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            SaveData();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".gwf"; // Default file extension
            dlg.Filter = "GSAKWrapper Flows (.gwf)|*.gwf"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                try
                {
                    var aflws = Manager.Instance.GetActionFlows(System.IO.File.ReadAllText(dlg.FileName));
                    foreach (var naf in aflws)
                    {
                        //check if ID is already in list
                        //check for ID and name
                        ActionImplementation startAction = (from sa in naf.Actions where sa is ActionStart select sa).FirstOrDefault();
                        if (startAction != null)
                        {
                            ActionFlow found = null;
                            bool doAdd = false;
                            foreach (var af in Manager.Instance.ActionFlows)
                            {
                                ActionImplementation startAct = (from sa in af.Actions where sa is ActionStart select sa).FirstOrDefault();
                                if (startAct != null)
                                {
                                    if (startAct.ID == startAction.ID)
                                    {
                                        found = af;
                                        break;
                                    }
                                }
                            }
                            if (found != null)
                            {
                                if (System.Windows.MessageBox.Show(string.Format(Localization.TranslationManager.Instance.Translate("Overwrite_flow_") as string, found.Name), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    if (ActiveActionFlow == found)
                                    {
                                        ActiveActionFlow = null;
                                    }
                                    Manager.Instance.ActionFlows.Remove(found);
                                    doAdd = true;
                                }
                            }
                            else
                            {
                                doAdd = true;
                            }
                            if (doAdd)
                            {
                                //insert new
                                //but first check name
                                int index = 0;
                                while ((from a in Manager.Instance.ActionFlows where a.Name.ToLower() == naf.Name.ToLower() select a).Count() > 0)
                                {
                                    index++;
                                    naf.Name = string.Format("{0}{1}", naf.Name, index);
                                }

                                Manager.Instance.ActionFlows.Add(naf);
                            }
                        }
                    }
                    SaveData();
                }
                catch
                {
                    System.Windows.MessageBox.Show("Unable to load the file.", "Error");
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".gwf"; // Default file extension
            dlg.Filter = "GSAKWrapper Flows (.gwf)|*.gwf"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                try
                {
                    System.IO.File.WriteAllText(dlg.FileName, Manager.Instance.GetFlowXml(ActiveActionFlow) ?? "");
                }
                catch
                {
                    System.Windows.MessageBox.Show("Unable to save the file.", "Error");
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".gwf"; // Default file extension
            dlg.Filter = "GSAKWrapper Flows (.gwf)|*.gwf"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                try
                {
                    System.IO.File.WriteAllText(dlg.FileName, Settings.Settings.Default.ActionBuilderFlowsXml ?? "");
                }
                catch
                {
                    System.Windows.MessageBox.Show("Unable to save the file.", "Error");
                }
            }
        }


    }
}
