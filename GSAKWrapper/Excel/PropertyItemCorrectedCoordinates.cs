using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class PropertyItemCorrectedCoordinates : PropertyItem
    {
        public PropertyItemCorrectedCoordinates()
            : base("CorrectedCoordinates")
        {
        }
        public override object GetValue(GeocacheData gc)
        {
            if (gc.Corrected != null && !string.IsNullOrEmpty(gc.Corrected.kAfterLat) && !string.IsNullOrEmpty(gc.Corrected.kAfterLon))
            {
                try
                {
                    return Utils.Conversion.GetCoordinatesPresentation(Utils.Conversion.StringToDouble(gc.Corrected.kAfterLat), Utils.Conversion.StringToDouble(gc.Corrected.kAfterLon));
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
