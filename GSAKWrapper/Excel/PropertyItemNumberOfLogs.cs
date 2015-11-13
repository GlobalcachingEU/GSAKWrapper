using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemNumberOfLogs : PropertyItem
    {
        public PropertyItemNumberOfLogs()
            : base("NumberOfLogs")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.NumberOfLogs;
        }
    }
}
