﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemHasTravelBug : PropertyItem
    {
        public PropertyItemHasTravelBug()
            : base("HasTravelBug")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.HasTravelBug != 0;
        }
    }
}
