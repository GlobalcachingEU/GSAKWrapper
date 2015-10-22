using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationText : ActionImplementationCondition
    {
        public enum Option
        {
            Contains,
            EqualsTo,
            RegEx,
            Match
        }

        private string _value = "";
        private Option _option = Option.Contains;
        private string fieldName = "";
        private string joins = "";
        public ActionImplementationText(string name, string fieldCompare, string joinStatement = "")
            : base(name)
        {
            fieldName = fieldCompare;
            joins = joinStatement;
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("-");
            }
            if (Values.Count < 2)
            {
                Values.Add(Option.Contains.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Option));
            ComboBox cb = CreateComboBox(opts, Values[1]);
            sp.Children.Add(cb);
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            if (Values.Count == 0)
            {
                Values.Add("-");
            }
            tb.Text = Values[0];
            sp.Children.Add(tb);
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
            Values[1] = cb.Text;
            TextBox tb = sp.Children[1] as TextBox;
            Values[0] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            _option = Option.Contains;
            if (Values.Count > 0)
            {
                _value = Values[0];
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
                case Option.Contains:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} like '%{0}%'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not like '%{0}%'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    break;
                case Option.EqualsTo:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} like '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not like '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    break;
                case Option.RegEx:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} REGEX '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not REGEXP '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    break;
                case Option.Match:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} MATCH '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not MATCH '{0}'", fieldName, _value.Replace("'", "''")), innerJoins: joins);
                    }
                    break;
            }
        }
    }
}
