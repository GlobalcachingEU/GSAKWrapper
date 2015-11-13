using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemShortHtm : PropertyItem
    {
        public PropertyItemShortHtm()
            : base("ShortHtm")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.ShortHtm != 0;
        }
    }
}
