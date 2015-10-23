using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationNumericValue : ActionImplementationCondition
    {
        public enum Option
        {
            Value,
            IsEmpty
        }

        private double _value = 1.0;
        private Option _option = Option.Value;
        private string fieldName = "";
        private string joins = "";
        public ActionImplementationNumericValue(string name, string fieldCompare, string joinStatement = "")
            : base(name)
        {
            fieldName = fieldCompare;
            joins = joinStatement;
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("1.0");
            }
            if (Values.Count < 2)
            {
                Values.Add(Option.Value.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Option));
            ComboBox cb = CreateComboBox(opts, Values[1]);
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
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[1] = cb.Text;
            TextBox tb = sp.Children[1] as TextBox;
            Values[0] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = 0.0;
            _option = Option.Value;
            if (Values.Count > 0)
            {
                _value = Utils.Conversion.StringToDouble(Values[0]);
            }
            if (Values.Count > 1)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[1]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            switch (_option)
            {
                case Option.Value:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    else if (op == Operator.Larger)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} > {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    else if (op == Operator.LargerOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} >= {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    else if (op == Operator.Less)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} < {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    else if (op == Operator.LessOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <= {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)), innerJoins: joins);
                    }
                    break;
                case Option.IsEmpty:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} is NULL", fieldName), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} is not NULL", fieldName), innerJoins: joins);
                    }
                    break;
            }
        }
    }
}
