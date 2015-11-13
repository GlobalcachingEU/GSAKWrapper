using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemFoundByMeDate : PropertyItem
    {
        public PropertyItemFoundByMeDate()
            : base("FoundByMeDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.FoundByMeDate;
        }
    }
}
