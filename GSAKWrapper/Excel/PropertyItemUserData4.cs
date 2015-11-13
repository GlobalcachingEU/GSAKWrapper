using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemUserData4 : PropertyItem
    {
        public PropertyItemUserData4()
            : base("UserData4")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.User4;
        }
    }
}
