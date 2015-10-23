using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLastGPXDate : ActionImplementationDate
    {
        public const string STR_NAME = "LastGPXDate";
        public ActionLastGPXDate()
            : base(STR_NAME, "LastGPXDate")
        {
        }
    }
}
