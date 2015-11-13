using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemFTF : PropertyItem
    {
        public PropertyItemFTF()
            : base("FTF")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.FTF != 0;
        }
    }
}
