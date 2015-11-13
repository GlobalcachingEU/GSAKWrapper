using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemDistance : PropertyItem
    {
        public PropertyItemDistance()
            : base("Distance")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Distance;
        }
    }
}
