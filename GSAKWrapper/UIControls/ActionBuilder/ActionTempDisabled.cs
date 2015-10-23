using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionTempDisabled : ActionImplementationYesNo
    {
        public const string STR_NAME = "TempDisabled";
        public ActionTempDisabled()
            : base(STR_NAME, "TempDisabled")
        {
        }
    }
}
