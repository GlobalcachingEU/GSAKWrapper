using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLogEncoded : ActionImplementationYesNo
    {
        public const string STR_NAME = "LogEncoded";
        public ActionLogEncoded()
            : base(STR_NAME, "Logs.lEncoded", joinStatement: "inner join Logs on Caches.Code=Logs.lParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Logs; } }
    }
}
