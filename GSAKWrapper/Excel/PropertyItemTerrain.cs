﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemTerrain : PropertyItem
    {
        public PropertyItemTerrain()
            : base("Terrain")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            return gc.Caches.Terrain;
        }
    }
}
