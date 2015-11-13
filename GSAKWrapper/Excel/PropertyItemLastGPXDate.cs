using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLastGPXDate : PropertyItem
    {
        public PropertyItemLastGPXDate()
            : base("LastGPXDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LastGPXDate;
        }
    }
}
