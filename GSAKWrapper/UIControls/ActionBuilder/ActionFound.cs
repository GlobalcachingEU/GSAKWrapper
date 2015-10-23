using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionFound : ActionImplementationYesNo
    {
        public const string STR_NAME = "Found";
        public ActionFound()
            : base(STR_NAME, "Found")
        {
        }
    }
}
