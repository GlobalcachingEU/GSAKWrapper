using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionHasCorrected : ActionImplementationYesNo
    {
        public const string STR_NAME = "HasCorrected";
        public ActionHasCorrected()
            : base(STR_NAME, "HasCorrected")
        {
        }
    }
}
