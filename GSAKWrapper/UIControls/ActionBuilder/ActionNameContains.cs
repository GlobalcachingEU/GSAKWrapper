using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionNameContains : ActionImplementationCondition
    {
        public const string STR_NAME = "NameContains";
        private string _value = "";
        public ActionNameContains()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            if (Values.Count == 0)
            {
                Values.Add("-");
            }
            tb.Text = Values[0];
            return tb;
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
            TextBox tb = uiElement as TextBox;
            Values[0] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Name like '%{0}%'", _value.Replace("'", "''")));
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Name not like '%{0}%'", _value.Replace("'", "''")));
            }
        }
    }
}
