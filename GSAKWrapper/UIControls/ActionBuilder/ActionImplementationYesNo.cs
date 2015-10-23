using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationYesNo : ActionImplementationCondition
    {
        public enum Option
        {
            Yes,
            No,
            NotSet
        }

        private Option _option = Option.Yes;
        private string fieldName = "";
        private string joins = "";
        private bool _allowNotSet = true;
        public ActionImplementationYesNo(string name, string fieldCompare, string joinStatement = "", bool allowNotSet = false)
            : base(name)
        {
            fieldName = fieldCompare;
            joins = joinStatement;
            _allowNotSet = allowNotSet;
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Option.Yes.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Option)).ToList();
            if (!_allowNotSet)
            {
                opts.Remove(Option.NotSet.ToString());
            }
            ComboBox cb = CreateComboBox(opts.ToArray(), Values[0]);
            cb.IsEditable = false;
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
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _option = Option.Yes;
            if (Values.Count > 0)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[0]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            switch (_option)
            {
                case Option.Yes:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = 1", fieldName), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> 1", fieldName), innerJoins: joins);
                    }
                    break;
                case Option.No:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = 0", fieldName), innerJoins: joins);
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> 0", fieldName), innerJoins: joins);
                    }
                    break;
                case Option.NotSet:
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
