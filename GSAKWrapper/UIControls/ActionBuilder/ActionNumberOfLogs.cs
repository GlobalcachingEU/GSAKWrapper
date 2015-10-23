using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionNumberOfLogs : ActionImplementationNumericValue
    {
        public const string STR_NAME = "NumberOfLogs";
        public ActionNumberOfLogs()
            : base(STR_NAME, "NumberOfLogs")
        {
        }
    }
}
