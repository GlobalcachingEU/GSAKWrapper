using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionInIgnoreListOfDatabase : ActionImplementationCondition
    {
        public const string STR_NAME = "InIgnoreListOfDatabase";

        public enum Option
        {
            Yes,
            No,
        }

        private Option _option = Option.Yes;
        private string _database = "";
        public ActionInIgnoreListOfDatabase()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Option.Yes.ToString());
            }
            if (Values.Count < 2)
            {
                Values.Add("");
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Option)).ToList();
            ComboBox cb = CreateComboBox(opts.ToArray(), Values[0]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            var opts2 = Utils.GSAK.AvailableDatabases.ToArray();
            cb = CreateComboBox(opts2, (from a in opts2 where a == Values[1] select a).FirstOrDefault() ?? "");
            sp.Children.Add(cb);
            return sp;
        }
        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.Equal | ActionImplementation.Operator.NotEqual;
            }
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
            cb = sp.Children[1] as ComboBox;
            Values[1] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _option = Option.Yes;
            _database = "";
            if (Values.Count > 0)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[0]);
            }
            if (Values.Count > 1)
            {
                _database = Values[1];
            }
            if (!string.IsNullOrEmpty(_database))
            {
                var opts = Utils.GSAK.AvailableDatabases.ToArray();
                _database = (from a in opts where a == _database select a).FirstOrDefault() ?? "";
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (!string.IsNullOrEmpty(_database))
            {
                string target = "target";
                DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Utils.GSAK.GetFullDatabasePath(_database), target));
                switch (_option)
                {
                    case Option.Yes:
                        if (op == Operator.Equal)
                        {
                            SelectGeocachesOnWhereClause(inputTableName, targetTableName, "1=1", innerJoins: string.Format("inner join {0}.Ignore on {1}.gccode={0}.Ignore.iCode", target, inputTableName));
                        }
                        else if (op == Operator.NotEqual)
                        {
                            SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0}.Ignore.iCode is null", target), innerJoins: string.Format("left join {0}.Ignore on {1}.gccode={0}.Ignore.iCode", target, inputTableName));
                        }
                        break;
                    case Option.No:
                        if (op == Operator.Equal)
                        {
                            SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0}.Ignore.iCode is null", target), innerJoins: string.Format("left join {0}.Ignore on {1}.gccode={0}.Ignore.iCode", target, inputTableName));
                        }
                        else if (op == Operator.NotEqual)
                        {
                            SelectGeocachesOnWhereClause(inputTableName, targetTableName, "1=1", innerJoins: string.Format("inner join {0}.Ignore on {1}.gccode={0}.Ignore.iCode", target, inputTableName));
                        }
                        break;
                }
                DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
            }
        }
    }
}
