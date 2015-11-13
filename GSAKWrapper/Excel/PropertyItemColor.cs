using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemColor : PropertyItem
    {
        public PropertyItemColor()
            : base("Color")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Color;
        }
    }
}
