using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCreated : PropertyItem
    {
        public PropertyItemCreated()
            : base("Created")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Created;
        }
    }
}
