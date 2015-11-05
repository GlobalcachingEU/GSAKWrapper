using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionExportGPX : ActionExport
    {
        public const string STR_NAME = "Export GPX";
        public ActionExportGPX()
            : base(STR_NAME)
        {
        }

        protected override string SetSettings(string currentSettings)
        {
            return currentSettings;
        }

        protected override object PrepareSettings(string settings)
        {
            return settings;
        }

        protected override void PerformExport(object settings)
        {
            //test
            string filename = @"c:\test.gpx";
            try
            {
                using (var db = new NPoco.Database(this.DatabaseConnection.Connection, NPoco.DatabaseType.SQLite))
                {
                    double minLat = 0, minLon = 0, maxLat = 0, maxLon = 0;
                    var dr = DatabaseConnection.ExecuteReader(string.Format("select Min(Latitude), Max(Latitude), Min(Longitude), Max(Longitude) from Caches inner join {0} on Caches.Code={0}.gccode", ActionInputTableName));
                    if (dr.Read())
                    {
                        //todo
                    }
                    dr.Close();
                    var gcList = db.Fetch<string>(string.Format("select gccode from {0}", ActionInputTableName));
                    using (Utils.ProgressBlock progress = new Utils.ProgressBlock("ExportingGPX", "CreatingFile", gcList.Count, 0))
                    {
                        using (System.IO.TemporaryFile gpxFile = new System.IO.TemporaryFile(false))
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(gpxFile.Path, false, Encoding.UTF8))
                            {
                                Utils.GPXGenerator gpxGenerator = new Utils.GPXGenerator(
                                    db
                                    , gcList
                                    , Utils.GPXGenerator.V101
                                    , minLat
                                    , maxLat
                                    , minLon
                                    , maxLon
                                    );

                                DateTime nextUpdate = DateTime.Now.AddSeconds(1);
                                //generate header
                                sw.Write(gpxGenerator.Start());
                                //preserve mem and do for each cache the export
                                for (int i = 0; i < gpxGenerator.Count; i++)
                                {
                                    //write parent
                                    sw.WriteLine(gpxGenerator.Next());

                                    //write child waypoints
                                    string s = gpxGenerator.WaypointData();
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        sw.WriteLine(s);
                                    }

                                    if (DateTime.Now >= nextUpdate)
                                    {
                                        progress.Update("CreatingFile", gpxGenerator.Count, i + 1);
                                        nextUpdate = DateTime.Now.AddSeconds(1);
                                    }
                                }
                                //finalize
                                sw.Write(gpxGenerator.Finish());
                            }

                            System.IO.File.Copy(gpxFile.Path, filename, true);
                        }
                    }
                }
            }
            catch(Exception e)
            {
            }
        }
    }
}
