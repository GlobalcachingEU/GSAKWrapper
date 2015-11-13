using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemContainer : PropertyItem
    {
        public PropertyItemContainer()
            : base("Container")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Container;
        }
    }
}
