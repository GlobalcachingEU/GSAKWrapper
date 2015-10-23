using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionFoundCount : ActionImplementationNumericValue
    {
        public const string STR_NAME = "FoundCount";
        public ActionFoundCount()
            : base(STR_NAME, "FoundCount")
        {
        }
    }
}
