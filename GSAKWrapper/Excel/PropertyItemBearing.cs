﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemBearing : PropertyItem
    {
        public PropertyItemBearing()
            : base("Bearing")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Bearing;
        }
    }
}