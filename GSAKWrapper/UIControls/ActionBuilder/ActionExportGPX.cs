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
        public class ExportGPXSettings
        {
            public string FileName { get; set; }
            public Version Version { get; set; }
        }

        public const string STR_NAME = "ExportGPX";
        public ActionExportGPX()
            : base(STR_NAME)
        {
        }

        private ExportGPXSettings GetSettingsFromValue(string v)
        {
            var result = new ExportGPXSettings();
            //todo: parse v
            return result;
        }

        private string GetSettingsFromValue(ExportGPXSettings s)
        {
            var result = "";
            //todo: 
            return result;
        }

        protected override string SetSettings(string currentSettings)
        {
            var s = GetSettingsFromValue(currentSettings);
            //todo: show dialog with model s
            return GetSettingsFromValue(s);
        }

        protected override object PrepareSettings(string settings)
        {
            return settings;
        }

        protected override void PerformExport(object settings)
        {
            var gpxSetting = GetSettingsFromValue(settings as string);
            //test
            gpxSetting.FileName = @"c:\test.gpx";
            gpxSetting.Version = Utils.GPXGenerator.V101; 
            bool canceled = false;
            try
            {
                using (var db = new NPoco.Database(this.DatabaseConnection.Connection, NPoco.DatabaseType.SQLite))
                {
                    double minLat = 0, minLon = 0, maxLat = 0, maxLon = 0;
                    var dr = DatabaseConnection.ExecuteReader(string.Format("select Min(Latitude), Max(Latitude), Min(Longitude), Max(Longitude) from Caches inner join {0} on Caches.Code={0}.gccode", ActionInputTableName));
                    if (dr.Read())
                    {
                        minLat = Utils.Conversion.StringToDouble(dr.GetString(0));
                        maxLat = Utils.Conversion.StringToDouble(dr.GetString(1));
                        minLon = Utils.Conversion.StringToDouble(dr.GetString(2));
                        maxLon = Utils.Conversion.StringToDouble(dr.GetString(3));
                    }
                    dr.Close();
                    var gcList = db.Fetch<string>(string.Format("select gccode from {0}", ActionInputTableName));
                    using (Utils.ProgressBlock progress = new Utils.ProgressBlock("ExportingGPX", "CreatingFile", gcList.Count, 0, true))
                    {
                        using (System.IO.TemporaryFile gpxFile = new System.IO.TemporaryFile(false))
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(gpxFile.Path, false, Encoding.UTF8))
                            {
                                Utils.GPXGenerator gpxGenerator = new Utils.GPXGenerator(
                                    db
                                    , gcList
                                    , gpxSetting.Version
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
                                        if (!progress.Update("CreatingFile", gpxGenerator.Count, i + 1))
                                        {
                                            canceled = true;
                                            break;
                                        }
                                        nextUpdate = DateTime.Now.AddSeconds(1);
                                    }
                                }
                                //finalize
                                sw.Write(gpxGenerator.Finish());
                            }

                            if (!canceled)
                            {
                                System.IO.File.Copy(gpxFile.Path, gpxSetting.FileName, true);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
