using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemUserData2 : PropertyItem
    {
        public PropertyItemUserData2()
            : base("UserData2")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.User2;
        }
    }
}
