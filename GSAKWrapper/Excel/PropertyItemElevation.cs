using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemElevation : PropertyItem
    {
        public PropertyItemElevation()
            : base("Elevation")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Elevation;
        }
    }
}
