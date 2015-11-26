using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.MapProviders
{
    public class GeocachePoco
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Found { get; set; }
        public int IsOwner { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string kAfterLat { get; set; }
        public string kAfterLon { get; set; }
        public string CacheType { get; set; }

        [NPoco.Ignore]
        public bool ContainsCustomLatLon { get { return kAfterLat != null; } }

        private double? _customLat;
        [NPoco.Ignore]
        public double? CustomLat 
        { 
            get 
            {
                if (kAfterLat != null && _customLat == null)
                {
                    _customLat = Convert.ToDouble(kAfterLat, CultureInfo.InvariantCulture);
                }
                return _customLat; 
            } 
        }

        private double? _customLon;
        [NPoco.Ignore]
        public double? CustomLon
        {
            get
            {
                if (kAfterLon != null && _customLon == null)
                {
                    _customLon = Convert.ToDouble(kAfterLon, CultureInfo.InvariantCulture);
                }
                return _customLon;
            }
        }

        private double? _lat;
        [NPoco.Ignore]
        public double Lat
        {
            get
            {
                if (_lat == null)
                {
                    _lat = Convert.ToDouble(Latitude, CultureInfo.InvariantCulture);
                }
                return (double)_lat;
            }
        }

        private double? _lon;
        [NPoco.Ignore]
        public double Lon
        {
            get
            {
                if (_lon == null)
                {
                    _lon = Convert.ToDouble(Longitude, CultureInfo.InvariantCulture);
                }
                return (double)_lon;
            }
        }

    }
}
