﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemFound : PropertyItem
    {
        public PropertyItemFound()
            : base("Found")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Found != 0;
        }
    }
}
