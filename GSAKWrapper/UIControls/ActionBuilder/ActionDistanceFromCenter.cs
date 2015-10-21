using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionDistanceFromCenter : ActionImplementationCondition
    {
        public const string STR_NAME = "ActionDistanceFromCenterKm";
        private double _value = 0.0;
        public ActionDistanceFromCenter()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            if (Values.Count == 0)
            {
                Values.Add("1.0");
            }
            tb.Text = Values[0];
            return tb;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            TextBox tb = uiElement as TextBox;
            Values[0] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            if (Values.Count > 0)
            {
                try
                {
                    _value = Utils.Conversion.StringToDouble(Values[0]);
                }
                catch
                {
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance = {0}", _value));
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance <> {0}", _value));
            }
            else if (op == Operator.Larger)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance > {0}", _value));
            }
            else if (op == Operator.LargerOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance >= {0}", _value));
            }
            else if (op == Operator.Less)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance < {0}", _value));
            }
            else if (op == Operator.LessOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Distance <= {0}", _value));
            }
        }
    }

}
