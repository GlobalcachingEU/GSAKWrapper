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
    public class ActionImplementationDate : ActionImplementationCondition
    {
        public enum Option
        {
            Value,
            IsEmpty
        }

        private DateTime _value = DateTime.Now.Date;
        private Option _option = Option.Value;
        private string fieldName = "";
        private string joins = "";
        public ActionImplementationDate(string name, string fieldCompare, string joinStatement = "")
            : base(name)
        {
            fieldName = fieldCompare;
            joins = joinStatement;
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(DateTime.Now.Date.ToString("yyyy-MM-dd"));
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
            DatePicker dp = new DatePicker();
            dp.HorizontalAlignment = HorizontalAlignment.Center;
            dp.SelectedDate = DateTime.ParseExact(Values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
            sp.Children.Add(dp);
            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[1] = cb.Text;
            DatePicker dp = sp.Children[1] as DatePicker;
            Values[0] = (dp.SelectedDate != null ? ((DateTime)dp.SelectedDate).Date : DateTime.Now.Date).ToString("yyyy-MM-dd");
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = DateTime.Now.Date;
            _option = Option.Value;
            if (Values.Count > 0)
            {
                _value = DateTime.ParseExact(Values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
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
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
                    }
                    else if (op == Operator.Larger)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} > '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
                    }
                    else if (op == Operator.LargerOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} >= '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
                    }
                    else if (op == Operator.Less)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} < '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
                    }
                    else if (op == Operator.LessOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <= '{1}'", fieldName, _value.ToString("yyyy-MM-dd")), innerJoins: joins);
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
