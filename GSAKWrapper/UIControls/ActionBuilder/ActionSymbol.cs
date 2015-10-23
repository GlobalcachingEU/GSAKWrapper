using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionSymbol : ActionImplementationText
    {
        public const string STR_NAME = "Symbol";
        public ActionSymbol()
            : base(STR_NAME, "Symbol")
        {
        }
    }
}
