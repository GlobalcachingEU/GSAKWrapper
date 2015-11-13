using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCode : PropertyItem
    {
        public PropertyItemCode()
            : base("GeocacheCode")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Code;
        }
    }
}
