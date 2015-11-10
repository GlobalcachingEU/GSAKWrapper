using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter.Functions.CoordinateFunctions
{
    public class Waypoint: Functor
    {
        public override object Execute(object[] args, ExecutionContext ctx)
        {
            ArgumentChecker checker = new ArgumentChecker(this.GetType().Name);
            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode))
            {
                string codeToSearch = (args.Length == 0)
                    ? Settings.Settings.Default.ActiveGeocacheCode
                    : args[0].ToString();

                if (codeToSearch.Length == 0)
                {
                    codeToSearch = Settings.Settings.Default.ActiveGeocacheCode;
                }

                // Geocache Code
                if (codeToSearch == Settings.Settings.Default.ActiveGeocacheCode)
                {
                    return GetGeocachePostion(Settings.Settings.Default.ActiveGeocacheCode);
                }

                // Waypoints
                return GetWaypointPostion(codeToSearch);
            }
            return "";
        }

        private object GetWaypointPostion(string wpt)
        {
            try
            {
                //todo
                /*
                double lat, lon;
                if (wpt.Lat != null)
                {
                    lat = (double)wpt.Lat;
                }
                else
                {
                    return "";
                }
                if (wpt.Lon != null)
                {
                    lon = (double)wpt.Lon;
                }
                else
                {
                    return "";
                }
                return Utils.Conversion.GetCoordinatesPresentation(lat, lon);
                 * */
            }
            catch
            {
            }
            return "";
        }

        private string GetGeocachePostion(string code)
        {
            try
            {//todo
                /*
                double lat, lon;
                if (geocache.CustomLat != null)
                {
                    lat = (double)geocache.CustomLat;
                }
                else
                {
                    lat = (double)geocache.Lat;
                }

                if (geocache.CustomLon != null)
                {
                    lon = (double)geocache.CustomLon;
                }
                else
                {
                    lon = (double)geocache.Lon;
                }

                return Utils.Conversion.GetCoordinatesPresentation(lat, lon);
                 * */
            }
            catch
            {
            }
            return "";
        }


    }
}
