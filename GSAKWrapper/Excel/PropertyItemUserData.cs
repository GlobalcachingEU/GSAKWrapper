using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemUserData : PropertyItem
    {
        public PropertyItemUserData()
            : base("UserData")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.UserData;
        }
    }
}
