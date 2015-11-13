using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemGeocacheId : PropertyItem
    {
        public PropertyItemGeocacheId()
            : base("GeocacheId")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.CacheId;
        }
    }
}
