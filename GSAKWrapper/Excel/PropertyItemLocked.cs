using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLocked : PropertyItem
    {
        public PropertyItemLocked()
            : base("Locked")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Lock != 0;
        }
    }
}
