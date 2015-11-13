using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLongHtm : PropertyItem
    {
        public PropertyItemLongHtm()
            : base("LongHtm")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LongHtm != 0;
        }
    }
}
