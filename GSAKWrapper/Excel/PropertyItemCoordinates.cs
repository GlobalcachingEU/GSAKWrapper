using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCoordinates : PropertyItem
    {
        public PropertyItemCoordinates()
            : base("Coordinates")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            if (!string.IsNullOrEmpty(gc.Caches.Latitude) && !string.IsNullOrEmpty(gc.Caches.Longitude))
            {
                try
                {
                    return Utils.Conversion.GetCoordinatesPresentation(Utils.Conversion.StringToDouble(gc.Caches.Latitude), Utils.Conversion.StringToDouble(gc.Caches.Longitude));
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
