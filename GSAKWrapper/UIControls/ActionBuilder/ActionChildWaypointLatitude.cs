using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionChildWaypointLatitude : ActionImplementationNumericValue
    {
        public const string STR_NAME = "ChildWaypointLatitude";
        public ActionChildWaypointLatitude()
            : base(STR_NAME, "Waypoints.cLat", joinStatement: "inner join Waypoints on Caches.Code=Waypoints.cParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }
    }
}
