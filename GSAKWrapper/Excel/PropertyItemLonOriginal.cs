using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLonOriginal : PropertyItem
    {
        public PropertyItemLonOriginal()
            : base("LonOriginal")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LonOriginal;
        }
    }
}
