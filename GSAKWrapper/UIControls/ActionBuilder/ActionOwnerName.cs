using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionOwnerName : ActionImplementationText
    {
        public const string STR_NAME = "OwnerName";
        public ActionOwnerName()
            : base(STR_NAME, "OwnerName")
        {
        }
    }
}
