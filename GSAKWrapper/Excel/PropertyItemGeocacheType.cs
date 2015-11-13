using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemGeocacheType : PropertyItem
    {
        public PropertyItemGeocacheType()
            : base("GeocacheType")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return (from a in ApplicationData.Instance.GeocacheTypes where a.GSAK == gc.Caches.CacheType select a.Name).FirstOrDefault() ?? gc.Caches.CacheType;
        }
    }
}
