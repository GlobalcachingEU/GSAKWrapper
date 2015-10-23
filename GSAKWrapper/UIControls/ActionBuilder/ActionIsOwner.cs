using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionIsOwner : ActionImplementationYesNo
    {
        public const string STR_NAME = "IsOwner";
        public ActionIsOwner()
            : base(STR_NAME, "IsOwner")
        {
        }
    }
}
