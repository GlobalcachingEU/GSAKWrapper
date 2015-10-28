using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionChildWaypointCode : ActionImplementationText
    {
        public const string STR_NAME = "ChildWaypointCode";
        public ActionChildWaypointCode()
            : base(STR_NAME, "Waypoints.cParent", joinStatement: "inner join Waypoints on Caches.Code=Waypoints.cParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }
    }
}
