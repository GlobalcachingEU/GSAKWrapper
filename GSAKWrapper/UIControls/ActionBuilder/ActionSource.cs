using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionSource : ActionImplementationText
    {
        public const string STR_NAME = "Source";
        public ActionSource()
            : base(STR_NAME, "Source")
        {
        }
    }
}
