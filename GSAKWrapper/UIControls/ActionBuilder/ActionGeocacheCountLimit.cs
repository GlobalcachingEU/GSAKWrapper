using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionGeocacheCountLimit : ActionImplementationCondition
    {
        public const string STR_NAME = "GeocacheCountLimit";

        public enum SortOption
        {
            Ascending,
            Descending
        }
        public enum FieldOption
        {
            DistanceFromCenter,
            GeocacheName
        }

        private int _value = 2000;
        private FieldOption _fieldoption = FieldOption.DistanceFromCenter;
        private SortOption _sortoption = SortOption.Ascending;

        public ActionGeocacheCountLimit()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("2000");
            }
            if (Values.Count < 2)
            {
                Values.Add(SortOption.Descending.ToString());
            }
            if (Values.Count < 3)
            {
                Values.Add(FieldOption.DistanceFromCenter.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts2 = Enum.GetNames(typeof(FieldOption));
            ComboBox cb = CreateComboBox(opts2, Values[2]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            var opts = Enum.GetNames(typeof(SortOption));
            cb = CreateComboBox(opts, Values[1]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Text = Values[0];
            sp.Children.Add(tb);
            return sp;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[1] as ComboBox;
            Values[1] = cb.Text;
            TextBox tb = sp.Children[2] as TextBox;
            Values[0] = tb.Text;
            cb = sp.Children[0] as ComboBox;
            Values[2] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = 2000;
            _sortoption = SortOption.Ascending;
            _fieldoption = FieldOption.DistanceFromCenter;
            if (Values.Count > 0)
            {
                int.TryParse(Values[0], out _value);
            }
            if (Values.Count > 1)
            {
                _sortoption = (SortOption)Enum.Parse(typeof(SortOption), Values[1]);
            }
            if (Values.Count > 2)
            {
                _fieldoption = (FieldOption)Enum.Parse(typeof(FieldOption), Values[2]);
            }
            return base.PrepareRun(db, tableName);
        }
        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.LessOrEqual | Operator.Larger;
            }
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            var fieldName = "";
            var orderdir = "asc";
            switch (_fieldoption)
            {
                case FieldOption.GeocacheName:
                    fieldName = "main.Caches.Name";
                    break;
                default:
                case FieldOption.DistanceFromCenter:
                    fieldName = "main.Caches.Distance";
                    break;
            }
            switch (_sortoption)
            {
                case SortOption.Ascending:
                    orderdir = "asc";
                    break;
                case SortOption.Descending:
                    orderdir = "desc";
                    break;
            }
            if (op == Operator.LessOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("1=1 order by {0} {1} limit {2}", fieldName, orderdir, _value));
            }
            else if (op == Operator.Larger)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("1=1 order by {0} {1} limit -1 offset {2}", fieldName, orderdir, _value));
            }
        }

    }
}
