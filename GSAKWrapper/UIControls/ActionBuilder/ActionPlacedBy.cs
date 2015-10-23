using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionPlacedBy : ActionImplementationText
    {
        public const string STR_NAME = "PlacedBy";
        public ActionPlacedBy()
            : base(STR_NAME, "PlacedBy")
        {
        }
    }
}
