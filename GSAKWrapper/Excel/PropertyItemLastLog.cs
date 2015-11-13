using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLastLog : PropertyItem
    {
        public PropertyItemLastLog()
            : base("LastLog")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LastLog;
        }
    }
}
