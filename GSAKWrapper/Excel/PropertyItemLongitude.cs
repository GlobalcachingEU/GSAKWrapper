using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLongitude : PropertyItem
    {
        public PropertyItemLongitude()
            : base("Longitude")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Longitude;
        }
    }
}
