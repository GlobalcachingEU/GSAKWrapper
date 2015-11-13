using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemShortDescription : PropertyItem
    {
        public PropertyItemShortDescription()
            : base("ShortDescription")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.CacheMemo == null ? "" : gc.CacheMemo.ShortDescription;
        }
    }
}
