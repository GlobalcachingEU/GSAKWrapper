using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionChildWaypoints : ActionImplementationNumericValue
    {
        public const string STR_NAME = "ChildWaypoints";
        public ActionChildWaypoints()
            : base(STR_NAME, "(select count(1) from Waypoints where cParent=Caches.Code)")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }
    }
}
