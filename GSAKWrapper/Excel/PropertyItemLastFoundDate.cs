using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLastFoundDate : PropertyItem
    {
        public PropertyItemLastFoundDate()
            : base("LastFoundDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LastFoundDate;
        }
    }
}
