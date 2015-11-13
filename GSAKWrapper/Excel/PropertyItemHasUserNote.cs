using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemHasUserNote : PropertyItem
    {
        public PropertyItemHasUserNote()
            : base("HasUserNote")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.HasUserNote != 0;
        }
    }
}
