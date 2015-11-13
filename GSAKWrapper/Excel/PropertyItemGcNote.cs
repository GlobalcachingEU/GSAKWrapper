using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemGcNote : PropertyItem
    {
        public PropertyItemGcNote()
            : base("GCNote")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.GcNote;
        }
    }
}
