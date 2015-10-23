using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLatitude : ActionImplementationNumericValue
    {
        public const string STR_NAME = "Latitude";
        public ActionLatitude()
            : base(STR_NAME, "Latitude")
        {
        }
    }
}
