using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemArchived : PropertyItem
    {
        public PropertyItemArchived()
            : base("Archived")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Archived != 0;
        }
    }
}
