﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemLatOriginal : PropertyItem
    {
        public PropertyItemLatOriginal()
            : base("LatOriginal")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.LatOriginal;
        }
    }
}
