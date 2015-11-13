using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemSmartName : PropertyItem
    {
        public PropertyItemSmartName()
            : base("SmartName")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.SmartName;
        }
    }
}
