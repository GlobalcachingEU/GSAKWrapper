using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemPlacedBy : PropertyItem
    {
        public PropertyItemPlacedBy()
            : base("PlacedBy")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.PlacedBy;
        }
    }
}
