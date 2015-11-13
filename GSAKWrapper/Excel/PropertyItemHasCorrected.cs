using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemHasCorrected : PropertyItem
    {
        public PropertyItemHasCorrected()
            : base("HasCorrected")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.HasCorrected != 0;
        }
    }
}
