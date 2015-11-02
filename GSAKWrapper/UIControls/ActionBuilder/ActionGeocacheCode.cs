using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionGeocacheCode : ActionImplementationText
    {
        public const string STR_NAME = "GeocacheCode";
        public ActionGeocacheCode()
            : base(STR_NAME, "Code")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Other; } }
    }
}
