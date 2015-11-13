using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLinkedTo : PropertyItem
    {
        public PropertyItemLinkedTo()
            : base("LinkedTo")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LinkedTo;
        }
    }
}
