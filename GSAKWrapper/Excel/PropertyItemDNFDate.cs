﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemDNFDate : PropertyItem
    {
        public PropertyItemDNFDate()
            : base("DNFDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.DNFDate;
        }
    }
}
