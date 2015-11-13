﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCounty : PropertyItem
    {
        public PropertyItemCounty()
            : base("County")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.County;
        }
    }
}
