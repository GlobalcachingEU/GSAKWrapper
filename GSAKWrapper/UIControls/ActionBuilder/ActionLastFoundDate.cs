using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLastFoundDate : ActionImplementationDate
    {
        public const string STR_NAME = "LastFoundDate";
        public ActionLastFoundDate()
            : base(STR_NAME, "LastFoundDate")
        {
        }
    }
}
