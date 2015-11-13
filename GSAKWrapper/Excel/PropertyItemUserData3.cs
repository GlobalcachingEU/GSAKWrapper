using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemUserData3 : PropertyItem
    {
        public PropertyItemUserData3()
            : base("UserData3")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.User3;
        }
    }
}
