using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionSmartNameContains : ActionImplementationText
    {
        public const string STR_NAME = "SmartNameContains";
        public ActionSmartNameContains()
            : base(STR_NAME, "SmartName")
        {
        }
    }
}
