using GSAKWrapper.Script;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionScriptAction : ActionImplementationAction
    {
        public const string STR_NAME = "ScriptAction";

        private string _script = "";
        private object _scriptObject = null;
        private Script.IActionScript _scriptInterface = null;
        public ActionScriptAction()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Action; } }
        public ObservableCollection<string> AvailableScripts { get; set; }

        public List<PropValue> GetPropertyValues()
        {
            var result = new List<PropValue>();
            if (Values.Count > 1 && Values[1].Length > 0)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Values[1]);

                    XmlNodeList nl = doc.SelectNodes("/settings/setting");
                    foreach (XmlNode n in nl)
                    {
                        var pv = new PropValue();
                        pv.Name = n.Attributes["name"].Value;
                        pv.Value = n.Attributes["value"].Value;
                        result.Add(pv);
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        public override UIElement GetUIElement()
        {
            if (AvailableScripts == null)
            {
                AvailableScripts = new ObservableCollection<string>();
            }
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            if (Values.Count < 2)
            {
                Values.Add("");
            }
            StackPanel sp = new StackPanel();
            Button b = new Button();
            b.Content = Localization.TranslationManager.Instance.Translate("ScriptEditor");
            b.Click += b_Click;
            sp.Children.Add(b);

            ComboBox cb = CreateComboBox(new string[] { }, Values[0]);
            cb.DropDownOpened += cb_DropDownOpened;
            cb.DropDownClosed += cb_DropDownClosed;
            cb.IsEditable = true;
            sp.Children.Add(cb);

            b = new Button();
            b.Content = Localization.TranslationManager.Instance.Translate("LoadSettings");
            b.Click += b_Click2;
            sp.Children.Add(b);

            Binding binding = new Binding();
            binding.Source = AvailableScripts;
            BindingOperations.SetBinding(cb, ComboBox.ItemsSourceProperty, binding);

            var g = new Grid();
            sp.Children.Add(g);

            var props = GetPropertyValues();
            if (props.Count > 0)
            {
                var grid = new Grid();

                ColumnDefinition columnDefinition1 = new ColumnDefinition();
                ColumnDefinition columnDefinition2 = new ColumnDefinition();
                columnDefinition1.Width = new GridLength(80);
                columnDefinition2.Width = new GridLength(1, GridUnitType.Star);

                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);

                foreach (var prop in props)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);

                    var tbk = new TextBlock();
                    tbk.Text = prop.Name;
                    grid.Children.Add(tbk);
                    Grid.SetRow(tbk, grid.RowDefinitions.Count - 1);
                    Grid.SetColumn(tbk, 0);

                    var tbe = new TextBox();
                    tbe.Text = prop.Value;
                    grid.Children.Add(tbe);
                    Grid.SetRow(tbe, grid.RowDefinitions.Count - 1);
                    Grid.SetColumn(tbe, 1);
                }

                g.Children.Add(grid);
            }

            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[1] as ComboBox;
            Values[0] = cb.Text;

            var doc = new XmlDocument();
            var root = doc.CreateElement("settings");
            doc.AppendChild(root);

            var g = sp.Children[3] as Grid;
            if (g.Children.Count > 0)
            {
                var grid = g.Children[0] as Grid;
                for (int i = 0; i < grid.Children.Count; i += 2)
                {
                    var tbk = grid.Children[i] as TextBlock;
                    var q = doc.CreateElement("setting");

                    var attr = doc.CreateAttribute("name");
                    var txt = doc.CreateTextNode(tbk.Text);
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);

                    attr = doc.CreateAttribute("value");
                    var ui = grid.Children[i + 1];
                    if (ui is TextBox)
                    {
                        txt = doc.CreateTextNode((ui as TextBox).Text);
                    }
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    root.AppendChild(q);
                }
            }

            var sb = new StringBuilder();
            var tr = new System.IO.StringWriter(sb);
            var wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.None;
            doc.Save(wr);
            wr.Close();

            Values[1] = sb.ToString();
        }

        void cb_DropDownClosed(object sender, EventArgs e)
        {
            var sp = (sender as ComboBox).Parent as StackPanel;
            var cb = sp.Children[1] as ComboBox;
            if (Values[0] != cb.Text)
            {
                var b = sp.Children[2] as Button;
                b_Click2(b, null);
            }
        }

        void b_Click2(object sender, RoutedEventArgs e)
        {
            //clear current settings list
            Values[1] = "";
            var sp = (sender as Button).Parent as StackPanel;
            var cb = sp.Children[1] as ComboBox;
            Values[0] = cb.Text;
            var g = sp.Children[3] as Grid;
            g.Children.Clear();

            //get properties
            var props = Script.Manager.Instance.GetScriptProperties(cb.Text);
            if (props.Count > 0)
            {
                var grid = new Grid();

                ColumnDefinition columnDefinition1 = new ColumnDefinition();
                ColumnDefinition columnDefinition2 = new ColumnDefinition();
                columnDefinition1.Width = new GridLength(100);
                columnDefinition2.Width = new GridLength(1, GridUnitType.Star);

                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);

                foreach (var prop in props)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    grid.RowDefinitions.Add(rowDefinition);

                    var tbk = new TextBlock();
                    tbk.Text = prop.Name;
                    grid.Children.Add(tbk);
                    Grid.SetRow(tbk, grid.RowDefinitions.Count - 1);
                    Grid.SetColumn(tbk, 0);

                    //todo: different per property type!
                    var tbe = new TextBox();
                    tbe.Text = prop.Value == null ? "" : prop.Value.ToString();
                    grid.Children.Add(tbe);
                    Grid.SetRow(tbe, grid.RowDefinitions.Count - 1);
                    Grid.SetColumn(tbe, 1);
                }

                g.Children.Add(grid);
            }

        }
        void b_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.WindowScriptEditor();
            dlg.ShowDialog();
        }
        void cb_DropDownOpened(object sender, EventArgs e)
        {
            AvailableScripts.Clear();
            var opt = (from a in Settings.Settings.Default.GetScriptItems() where a.ScriptType==Script.Manager.ScriptTypeAction orderby a.Name select a.Name).ToArray();
            foreach (var c in opt)
            {
                AvailableScripts.Add(c);
            }
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _script = "";
            _scriptObject = null;
            if (Values.Count > 0)
            {
                _script = Values[0];
            }
            if (!string.IsNullOrEmpty(_script))
            {
                var scr = Settings.Settings.Default.GetScriptItem(_script);
                if (scr != null)
                {
                    _scriptObject = Script.Manager.Instance.LoadFilterScript(scr.Code);
                    if (_scriptObject != null)
                    {
                        _scriptInterface = _scriptObject.AlignToInterface<IActionScript>();
                        if (Values.Count > 1)
                        {
                            var props = GetPropertyValues();
                            Script.Manager.Instance.SetProprtyValues(_scriptObject, props);
                        }
                        _scriptInterface.PrepareRun(this, db, tableName);
                    }
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void FinalizeRun()
        {
            if (_scriptInterface != null)
            {
                TotalProcessTime.Start();
                _scriptInterface.FinalizeRun(this);
                TotalProcessTime.Stop();
            }
            base.FinalizeRun();
        }
    }
}
