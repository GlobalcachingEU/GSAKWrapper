using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionCountry : ActionImplementationText
    {
        public const string STR_NAME = "Country";
        public ActionCountry()
            : base(STR_NAME, "Country")
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Other; } }
    }
}
