using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemResolution : PropertyItem
    {
        public PropertyItemResolution()
            : base("Resolution")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Resolution;
        }
    }
}
