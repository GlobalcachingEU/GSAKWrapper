using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLastUserDate : PropertyItem
    {
        public PropertyItemLastUserDate()
            : base("LastUserDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LastUserDate;
        }
    }
}
