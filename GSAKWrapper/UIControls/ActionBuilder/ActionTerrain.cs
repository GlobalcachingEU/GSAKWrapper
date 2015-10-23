using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionTerrain : ActionImplementationNumericValue
    {
        public const string STR_NAME = "Terrain";
        public ActionTerrain()
            : base(STR_NAME, "Terrain")
        {
        }
    }
}
