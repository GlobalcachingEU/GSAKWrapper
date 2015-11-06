using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionExportGPX : ActionExport
    {
        public class ExportGPXSettings
        {
            public string FileName { get; set; }
            public Version Version { get; set; }
            public bool AddChildWaypoints { get; set; }

            public ExportGPXSettings()
            {
                FileName = "";
                Version = Utils.GPXGenerator.V101;
                AddChildWaypoints = true;
            }
        }

        public const string STR_NAME = "ExportGPX";
        public ActionExportGPX()
            : base(STR_NAME)
        {
        }

        private ExportGPXSettings GetSettingsFromValue(string v)
        {
            var result = new ExportGPXSettings();
            if (!string.IsNullOrEmpty(v))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(v);

                    XmlNodeList nl = doc.SelectNodes("/settings/setting");
                    foreach (XmlNode n in nl)
                    {
                        switch (n.Attributes["name"].Value)
                        {
                            case "FileName":
                                result.FileName = n.Attributes["value"].Value;
                                break;
                            case "Version":
                                result.Version = Version.Parse(n.Attributes["value"].Value);
                                break;
                            case "AddChildWaypoints":
                                result.AddChildWaypoints = bool.Parse(n.Attributes["value"].Value);
                                break;
                        }
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        private string GetSettingsFromValue(ExportGPXSettings s)
        {
            var result = "";
            if (s != null)
            {
                var doc = new XmlDocument();
                var root = doc.CreateElement("settings");
                doc.AppendChild(root);

                var q = doc.CreateElement("setting");
                var attr = doc.CreateAttribute("name");
                var txt = doc.CreateTextNode("FileName");
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                attr = doc.CreateAttribute("value");
                txt = doc.CreateTextNode(s.FileName ?? "");
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                root.AppendChild(q);

                q = doc.CreateElement("setting");
                attr = doc.CreateAttribute("name");
                txt = doc.CreateTextNode("Version");
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                attr = doc.CreateAttribute("value");
                txt = doc.CreateTextNode(s.Version.ToString());
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                root.AppendChild(q);

                q = doc.CreateElement("setting");
                attr = doc.CreateAttribute("name");
                txt = doc.CreateTextNode("AddChildWaypoints");
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                attr = doc.CreateAttribute("value");
                txt = doc.CreateTextNode(s.AddChildWaypoints.ToString());
                attr.AppendChild(txt);
                q.Attributes.Append(attr);
                root.AppendChild(q);

                var sb = new StringBuilder();
                var tr = new System.IO.StringWriter(sb);
                var wr = new XmlTextWriter(tr);
                wr.Formatting = Formatting.None;
                doc.Save(wr);
                wr.Close();

                result = sb.ToString();
            }
            return result;
        }

        protected override string SetSettings(string currentSettings)
        {
            var s = GetSettingsFromValue(currentSettings);
            var dlg = new Dialogs.WindowExportGPXSettings(s);
            dlg.ShowDialog();
            return GetSettingsFromValue(s);
        }

        protected override object PrepareSettings(string settings)
        {
            return GetSettingsFromValue(settings as string);
        }

        protected override void PerformExport(object settings)
        {
            var gpxSetting = settings as ExportGPXSettings;
            if (gpxSetting != null && !string.IsNullOrEmpty(gpxSetting.FileName))
            {
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

                                        if (gpxSetting.AddChildWaypoints)
                                        {
                                            //write child waypoints
                                            string s = gpxGenerator.WaypointData();
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                sw.WriteLine(s);
                                            }
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
                                    if (gpxSetting.FileName.ToLower().EndsWith(".zip"))
                                    {
                                        using (FileStream zipToOpen = new FileStream(gpxSetting.FileName, FileMode.Create))
                                        {
                                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                                            {
                                                ZipArchiveEntry gpxEntry = archive.CreateEntry("geocaches.gpx");
                                                using (StreamWriter writer = new StreamWriter(gpxEntry.Open()))
                                                {
                                                    writer.Write(File.ReadAllText(gpxFile.Path));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.Copy(gpxFile.Path, gpxSetting.FileName, true);
                                    }
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
}
