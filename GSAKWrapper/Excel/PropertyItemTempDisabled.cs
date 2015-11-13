using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemTempDisabled : PropertyItem
    {
        public PropertyItemTempDisabled()
            : base("TempDisabled")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.TempDisabled != 0;
        }
    }
}
