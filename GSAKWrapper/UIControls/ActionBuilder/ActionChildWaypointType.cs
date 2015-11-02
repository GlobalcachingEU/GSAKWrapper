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
    public class ActionChildWaypointType : ActionImplementationCondition
    {
        public const string STR_NAME = "ChildWaypointType";
        private string _value = "Parking Area";
        private double _count = 0;
        public ActionChildWaypointType()
            : base(STR_NAME)
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("Parking Area");
            }
            if (Values.Count < 2)
            {
                Values.Add("0");
            }
            StackPanel sp = new StackPanel();
            var opts = (from a in ApplicationData.Instance.WaypointTypes select a.Name).ToArray();
            ComboBox cb = CreateComboBox(opts, (from a in ApplicationData.Instance.WaypointTypes where a.Name == Values[0] select a.Name).FirstOrDefault() ?? "Parking Area");
            cb.IsEditable = false;
            sp.Children.Add(cb);
            TextBox tb = new TextBox();
            tb.Text = Values[1];
            sp.Children.Add(tb);
            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            var s = cb.Text;
            Values[0] = (from a in ApplicationData.Instance.WaypointTypes where a.Name == s select a.Name).FirstOrDefault() ?? "Parking Area";
            TextBox tb = sp.Children[1] as TextBox;
            Values[1] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "Parking Area";
            _count = 0;
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            if (Values.Count > 1)
            {
                _count = Utils.Conversion.StringToDouble(Values[1]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') = {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') <> {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.Larger)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') > {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.LargerOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') >= {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.Less)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') < {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.LessOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Waypoints where Waypoints.cParent=Caches.Code and cType='{0}') <= {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }

        }
    }
}
