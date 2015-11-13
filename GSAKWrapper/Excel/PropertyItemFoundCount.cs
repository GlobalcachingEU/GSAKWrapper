using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemFoundCount : PropertyItem
    {
        public PropertyItemFoundCount()
            : base("FoundCount")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.FoundCount;
        }
    }
}
