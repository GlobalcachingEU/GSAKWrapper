using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionAddToIgnoreList : ActionImplementationAction
    {
        public const string STR_NAME = "AddToIgnoreList";
        private string _value = "";
        public ActionAddToIgnoreList()
            : base(STR_NAME)
        {
        }

        //call this at the end of all actions!
        public override int Priority { get { return 0; } }

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

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();
            string target = "target";
            var s = Settings.Settings.Default.SelectedDatabase;
            if (string.IsNullOrEmpty(_value) || string.Compare(_value, s, true) == 0)
            {
                target = "main";
            }
            else
            {
                DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Utils.GSAK.GetFullDatabasePath(_value), target));
            }

            DatabaseConnection.ExecuteNonQuery(string.Format("insert or ignore into {1}.Ignore (iCode, iName) select main.Caches.Code as iCode, main.Caches.Name as iName from main.Caches inner join {0} on main.Caches.Code = {0}.gccode", ActionInputTableName, target));

            if (target != "main")
            {
                DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
            }
            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
