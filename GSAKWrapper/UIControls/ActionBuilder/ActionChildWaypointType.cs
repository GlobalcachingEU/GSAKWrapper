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
    public class ActionChildWaypointType : ActionImplementationCondition
    {
        public const string STR_NAME = "ChildWaypointType";
        private string _value = "Parking Area";
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
            StackPanel sp = new StackPanel();
            var opts = (from a in ApplicationData.Instance.WaypointTypes select a.Name).ToArray();
            ComboBox cb = CreateComboBox(opts, (from a in ApplicationData.Instance.WaypointTypes where a.Name == Values[0] select a.Name).FirstOrDefault() ?? "Parking Area");
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
            var s = cb.Text;
            Values[0] = (from a in ApplicationData.Instance.WaypointTypes where a.Name == s select a.Name).FirstOrDefault() ?? "Parking Area";
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "Parking Area";
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
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("cType = '{0}'", _value), innerJoins: "inner join Waypoints on Caches.Code=Waypoints.cParent");
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("cType <> '{0}'", _value), innerJoins: "inner join Waypoints on Caches.Code=Waypoints.cParent");
            }
        }
    }
}
