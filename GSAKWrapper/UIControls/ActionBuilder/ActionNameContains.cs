using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionNameContains : ActionImplementationText
    {
        public const string STR_NAME = "NameContains";
        public ActionNameContains()
            : base(STR_NAME, "Name")
        {
        }
    }
}
