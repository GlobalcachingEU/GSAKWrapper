using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemSymbol : PropertyItem
    {
        public PropertyItemSymbol()
            : base("Symbol")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Symbol;
        }
    }
}
