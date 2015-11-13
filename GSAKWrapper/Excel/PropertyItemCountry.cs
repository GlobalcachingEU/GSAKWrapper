using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCountry : PropertyItem
    {
        public PropertyItemCountry()
            : base("Country")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Country;
        }
    }
}
