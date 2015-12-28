using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionCopyToDatabase : ActionImplementationAction
    {
        public const string STR_NAME = "CopyToDatabase";
        public const string STR_OVERWRITE = "OverwriteExistingData";
        private string _value = "";
        private bool _overwrite = false;
        public ActionCopyToDatabase()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            if (Values.Count <= 1)
            {
                Values.Add(true.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts = Utils.GSAK.AvailableDatabases.ToArray();
            ComboBox cb = CreateComboBox(opts, (from a in opts where a == Values[0] select a).FirstOrDefault() ?? "");
            sp.Children.Add(cb);
            CheckBox cb1 = new CheckBox();
            cb1.Content = Localization.TranslationManager.Instance.Translate(STR_OVERWRITE);
            cb1.IsChecked = bool.Parse(Values[1]);
            sp.Children.Add(cb1);
            return sp;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
            CheckBox cb1 = sp.Children[1] as CheckBox;
            Values[1] = cb1.IsChecked.ToString();
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            _overwrite = false;
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            if (Values.Count > 1)
            {
                bool.TryParse( Values[1], out _overwrite);
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
            if (!string.IsNullOrEmpty(_value))
            {
                TotalProcessTime.Start();
                string target = "target";
                var s = Settings.Settings.Default.SelectedDatabase;
                if (string.Compare(_value, s, true) != 0)
                {
                    DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Utils.GSAK.GetFullDatabasePath(_value), target));
                    CopyTable(target, "Caches", "Code", _overwrite);
                    CopyTable(target, "Attributes", "aCode", _overwrite);
                    CopyTable(target, "CacheMemo", "Code", _overwrite);
                    CopyTable(target, "Corrected", "kCode", _overwrite);
                    CopyTable(target, "Logs", "lParent", _overwrite);
                    CopyTable(target, "LogImages", "iCode", _overwrite);
                    CopyTable(target, "LogMemo", "lParent", _overwrite);
                    CopyTable(target, "Waypoints", "cParent", _overwrite);
                    CopyTable(target, "WayMemo", "cParent", _overwrite);
                    CopyTable(target, "CacheImages", "iCode", _overwrite);
                    DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                }
                TotalProcessTime.Stop();
            }
            base.FinalizeRun();
        }

        private void CopyTable(string target, string table, string CodeFieldName, bool overwrite)
        {
            //just to be sure, we state the columns
            var clmns = new List<string>();
            var dr = DatabaseConnection.ExecuteReader(string.Format("PRAGMA table_info({0})", table));
            while (dr.Read())
            {
                clmns.Add(dr["name"] as string);
            }
            var c = string.Join(", ", clmns.ToArray());
            DatabaseConnection.ExecuteNonQuery(string.Format("insert or {5} into {0}.{1} ({2}) select {2} from {1} inner join {3} on {1}.{4} = {3}.gccode", target, table, c, ActionInputTableName, CodeFieldName, overwrite ? "replace" : "ignore"));
        }
    }
}
