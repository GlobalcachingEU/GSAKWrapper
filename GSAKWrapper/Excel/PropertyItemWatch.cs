using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemWatch : PropertyItem
    {
        public PropertyItemWatch()
            : base("Watch")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Watch != 0;
        }
    }
}
