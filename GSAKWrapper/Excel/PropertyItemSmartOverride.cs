﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemSmartOverride : PropertyItem
    {
        public PropertyItemSmartOverride()
            : base("SmartOverride")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.SmartOverride != 0;
        }
    }
}
