using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemOwnerId : PropertyItem
    {
        public PropertyItemOwnerId()
            : base("OwnerId")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.OwnerId;
        }
    }
}
