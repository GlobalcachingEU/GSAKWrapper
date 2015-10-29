using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLogByUser : ActionImplementationText
    {
        public const string STR_NAME = "LogByUser";
        public ActionLogByUser()
            : base(STR_NAME, "Logs.lBy", joinStatement: "inner join Logs on Caches.Code=Logs.lParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Logs; } }
    }
}
