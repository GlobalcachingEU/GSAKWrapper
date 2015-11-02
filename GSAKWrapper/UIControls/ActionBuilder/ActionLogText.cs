using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLogText : ActionImplementationText
    {
        public const string STR_NAME = "LogText";
        public ActionLogText()
            : base(STR_NAME, "LogMemo.lText", joinStatement: "inner join LogMemo on Caches.Code=LogMemo.lParent")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Logs; } }
    }
}
