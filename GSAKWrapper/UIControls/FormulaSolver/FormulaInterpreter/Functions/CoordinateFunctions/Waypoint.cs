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
            string result = "";
            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode)
                && !string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath)
                && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                )
            {
                try
                {
                    var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    if (System.IO.File.Exists(fn))
                    {
                        using (var temp = new Database.DBConSqlite(fn))
                        using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                        {
                            var wp = db.FirstOrDefault<DataTypes.GSAKWaypoints>("select * from Waypoints where cCode=@0", wpt);
                            if (!string.IsNullOrEmpty(wp.cLat) && !string.IsNullOrEmpty(wp.cLon))
                            {
                                result = Utils.Conversion.GetCoordinatesPresentation(Utils.Conversion.StringToDouble(wp.cLat), Utils.Conversion.StringToDouble(wp.cLon));
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        private string GetGeocachePostion(string code)
        {
            string result = "";
            if (!string.IsNullOrEmpty(code)
                && !string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath)
                && !string.IsNullOrEmpty(Settings.Settings.Default.SelectedDatabase)
                )
            {
                try
                {
                    var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    if (System.IO.File.Exists(fn))
                    {
                        using (var temp = new Database.DBConSqlite(fn))
                        using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                        {
                            var wp = db.FirstOrDefault<DataTypes.GSAKCaches>("select * from Caches where Code=@0", code);
                            var wpc = db.FirstOrDefault<DataTypes.GSAKCorrected>("select * from Corrected where kCode=@0", code);
                            if (wpc != null && !string.IsNullOrEmpty(wpc.kAfterLat) && !string.IsNullOrEmpty(wpc.kAfterLon))
                            {
                                result = Utils.Conversion.GetCoordinatesPresentation(Utils.Conversion.StringToDouble(wpc.kAfterLat), Utils.Conversion.StringToDouble(wpc.kAfterLon));
                            }
                            else if (!string.IsNullOrEmpty(wp.Latitude) && !string.IsNullOrEmpty(wp.Longitude))
                            {
                                result = Utils.Conversion.GetCoordinatesPresentation(Utils.Conversion.StringToDouble(wp.Latitude), Utils.Conversion.StringToDouble(wp.Longitude));
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return result;
        }


    }
}
