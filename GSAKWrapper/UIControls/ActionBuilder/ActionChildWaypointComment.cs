using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionChildWaypointComment : ActionImplementationText
    {
        public const string STR_NAME = "ChildWaypointComment";
        public ActionChildWaypointComment()
            : base(STR_NAME, "WayMemo.cComment", joinStatement: "inner join WayMemo on Caches.Code=WayMemo.cParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Children; } }
    }
}
