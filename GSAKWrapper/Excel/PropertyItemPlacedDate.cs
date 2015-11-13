﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemPlacedDate : PropertyItem
    {
        public PropertyItemPlacedDate()
            : base("PlacedDate")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.PlacedDate;
        }
    }
}
