using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemDNF : PropertyItem
    {
        public PropertyItemDNF()
            : base("DNF")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.DNF != 0;
        }
    }
}
