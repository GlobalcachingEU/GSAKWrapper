using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLongDescription : PropertyItem
    {
        public PropertyItemLongDescription()
            : base("LongDescription")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.CacheMemo == null ? "" : gc.CacheMemo.LongDescription;
        }
    }
}
