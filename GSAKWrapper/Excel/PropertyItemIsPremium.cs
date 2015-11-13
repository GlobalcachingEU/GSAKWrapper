using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemIsPremium : PropertyItem
    {
        public PropertyItemIsPremium()
            : base("IsPremium")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.IsPremium != 0;
        }
    }
}
