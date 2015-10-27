using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionEmptyDatabase : ActionImplementationExecuteOnce
    {
        public const string STR_NAME = "EmptyDatabase";
        private string _value = "";
        public ActionEmptyDatabase()
            : base(STR_NAME)
        {
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            StackPanel sp = new StackPanel();
            var opts = Utils.GSAK.AvailableDatabases.ToArray();
            ComboBox cb = CreateComboBox(opts, (from a in opts where a == Values[0] select a).FirstOrDefault() ?? "");
            sp.Children.Add(cb);
            return sp;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            if (!string.IsNullOrEmpty(_value))
            {
                var opts = Utils.GSAK.AvailableDatabases.ToArray();
                _value = (from a in opts where a == _value select a).FirstOrDefault() ?? "";
            }
            return base.PrepareRun(db, tableName);
        }

        protected override bool Execute()
        {
            if (!string.IsNullOrEmpty(_value))
            {
                string target = "target";
                var s = Settings.Settings.Default.SelectedDatabase;
                if (string.Compare(_value, s, true)==0)
                {
                    //oops, are you sure?
                    target = "main";
                }
                else
                {
                    DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Utils.GSAK.GetFullDatabasePath(_value), target));
                }
                DatabaseConnection.ExecuteNonQuery(string.Format("delete from {0}.Caches", target));
                if (target != "main")
                {
                    DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                }
            }
            return true;
        }
    }
}
