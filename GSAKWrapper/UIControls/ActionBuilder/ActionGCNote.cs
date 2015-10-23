using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionGCNote : ActionImplementationText
    {
        public const string STR_NAME = "GCNote";
        public ActionGCNote()
            : base(STR_NAME, "GcNote")
        {
        }
    }
}
