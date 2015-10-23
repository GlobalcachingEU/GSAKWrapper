using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionElevation : ActionImplementationNumericValue
    {
        public const string STR_NAME = "Elevation";
        public ActionElevation()
            : base(STR_NAME, "Elevation")
        {
        }
    }
}
