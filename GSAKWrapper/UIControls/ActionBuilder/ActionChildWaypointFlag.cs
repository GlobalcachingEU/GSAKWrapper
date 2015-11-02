using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionChildWaypointFlag : ActionImplementationYesNo
    {
        public const string STR_NAME = "ChildWaypointFlag";
        public ActionChildWaypointFlag()
            : base(STR_NAME, "Waypoints.cFlag", joinStatement: "inner join Waypoints on Caches.Code=Waypoints.cParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }
    }
}
