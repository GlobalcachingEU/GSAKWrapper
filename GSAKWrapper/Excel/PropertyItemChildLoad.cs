using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemChildLoad : PropertyItem
    {
        public PropertyItemChildLoad()
            : base("ChildLoad")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.ChildLoad != 0;
        }
    }
}
