using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionHasTravelBug : ActionImplementationYesNo
    {
        public const string STR_NAME = "HasTravelBug";
        public ActionHasTravelBug()
            : base(STR_NAME, "HasTravelBug")
        {
        }
    }
}
