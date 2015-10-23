using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionPlacedDate : ActionImplementationDate
    {
        public const string STR_NAME = "PlacedDate";
        public ActionPlacedDate()
            : base(STR_NAME, "PlacedDate")
        {
        }
    }
}
