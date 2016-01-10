using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
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
    public class ActionExportGGZ : ActionExport
    {
        public class ExportGPXSettings
        {
            public string FileName { get; set; }
            public Version Version { get; set; }

            public ExportGPXSettings()
            {
                FileName = "";
                Version = Utils.GPXGenerator.V101;
            }
        }

        public class GeocacheEntryInfo
        {
            //from Caches
            public string Code { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Name { get; set; }
            public string CacheType { get; set; }
            public double Difficulty { get; set; }
            public double Terrain { get; set; }
            public int Found { get; set; }
            public string Container { get; set; }

            //from Corrected
            public string kAfterLat { get; set; }
            public string kAfterLon { get; set; }

            [NPoco.Ignore]
            public int FileLen { get; set; }
        }

        public const string STR_NAME = "ExportGGZ";
        public ActionExportGGZ()
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
            var dlg = new Dialogs.WindowExportGGZSettings(s);
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
                        if (DatabaseConnection.CurrentDataReader != null && !DatabaseConnection.CurrentDataReader.IsClosed)
                        {
                            DatabaseConnection.CurrentDataReader.Close();
                        }
                        var gcList = db.Fetch<GeocacheEntryInfo>(string.Format("select Code, Name, CacheType, Difficulty, Terrain, Found, Container, Latitude, Longitude, kAfterLat, kAfterLon from Caches inner join {0} on Caches.Code={0}.gccode left join Corrected on Caches.Code = Corrected.kCode", ActionInputTableName));
                        minLat = (from a in gcList select Utils.Conversion.StringToDouble(a.Latitude)).Min();
                        maxLat = (from a in gcList select Utils.Conversion.StringToDouble(a.Latitude)).Max();
                        minLon = (from a in gcList select Utils.Conversion.StringToDouble(a.Longitude)).Min();
                        maxLon = (from a in gcList select Utils.Conversion.StringToDouble(a.Longitude)).Max();
                        DateTime dt = DateTime.Now.AddSeconds(2); 
                        using (Utils.ProgressBlock progress = new Utils.ProgressBlock("ExportingGPX", "CreatingFile", gcList.Count, 0, true))
                        {
                            using (System.IO.TemporaryFile gpxFile = new System.IO.TemporaryFile(false))
                            {
                                using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(gpxFile.Path)))
                                {
                                    s.SetLevel(9); // 0-9, 9 being the highest compression
                                    s.UseZip64 = UseZip64.Off;

                                    int totalGeocaches = gcList.Count;
                                    int totalProcessed = 0;
                                    int fileIndex = 1;
                                    int geocacheIndex = 0;
                                    int gpxSizeLimit = 4500000; //appr. 4.5MB

                                    XmlDocument doc = new XmlDocument();
                                    XmlDeclaration pi = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                                    doc.InsertBefore(pi, doc.DocumentElement);
                                    XmlElement root = doc.CreateElement("ggz");
                                    doc.AppendChild(root);
                                    XmlAttribute attr = doc.CreateAttribute("xmlns");
                                    XmlText txt = doc.CreateTextNode("http://www.opencaching.com/xmlschemas/ggz/1/0");
                                    attr.AppendChild(txt);
                                    root.Attributes.Append(attr);

                                    XmlElement el = doc.CreateElement("time");
                                    txt = doc.CreateTextNode(string.Format("{0}Z", DateTime.Now.ToUniversalTime().ToString("s")));
                                    el.AppendChild(txt);
                                    root.AppendChild(el);

                                    Utils.GPXGenerator gpxGenerator = new Utils.GPXGenerator(
                                        db
                                        , (from a in gcList select a.Code).ToList()
                                        , gpxSetting.Version
                                        , minLat
                                        , maxLat
                                        , minLon
                                        , maxLon
                                        );

                                    while (gcList.Count > 0)
                                    {
                                        XmlElement elFile = doc.CreateElement("file");
                                        root.AppendChild(elFile);

                                        el = doc.CreateElement("name");
                                        txt = doc.CreateTextNode(string.Format("{0}_{1}.gpx", System.IO.Path.GetFileNameWithoutExtension(gpxSetting.FileName), fileIndex));
                                        el.AppendChild(txt);
                                        elFile.AppendChild(el);

                                        XmlElement elCRC = doc.CreateElement("crc");
                                        elFile.AppendChild(elCRC);

                                        el = doc.CreateElement("time");
                                        txt = doc.CreateTextNode(string.Format("{0}Z", DateTime.Now.ToUniversalTime().ToString("s")));
                                        el.AppendChild(txt);
                                        elFile.AppendChild(el);

                                        //create GPX wpt entries until max size is reached
                                        List<GeocacheEntryInfo> gpxBatchList = new List<GeocacheEntryInfo>();
                                        List<GeocacheEntryInfo> geiList = new List<GeocacheEntryInfo>();
                                        geocacheIndex = 0;
                                        minLat = (from a in gcList select Utils.Conversion.StringToDouble(a.Latitude)).Min();
                                        maxLat = (from a in gcList select Utils.Conversion.StringToDouble(a.Latitude)).Max();
                                        minLon = (from a in gcList select Utils.Conversion.StringToDouble(a.Longitude)).Min();
                                        maxLon = (from a in gcList select Utils.Conversion.StringToDouble(a.Longitude)).Max();
                                        gpxGenerator.SetGeocacheList((from a in gcList select a.Code).ToList(), minLat, maxLat, minLon, maxLon);
                                        StringBuilder sb = new StringBuilder();
                                        gpxGenerator.Start();
                                        while (sb.Length < gpxSizeLimit && geocacheIndex < gpxGenerator.Count)
                                        {
                                            gpxBatchList.Add(gcList[geocacheIndex]);
                                            string gpxText = gpxGenerator.Next();

                                            gcList[geocacheIndex].FileLen = System.Text.UTF8Encoding.UTF8.GetBytes(gpxText).Length + 2;
                                            geiList.Add(gcList[geocacheIndex]);

                                            sb.AppendLine(gpxText);

                                            totalProcessed++;
                                            geocacheIndex++;

                                            if (DateTime.Now >= dt)
                                            {
                                                if (!progress.Update("CreatingFile", totalGeocaches, totalProcessed))
                                                {
                                                    canceled = true;
                                                    break;
                                                }
                                                dt = DateTime.Now.AddSeconds(2);
                                            }
                                        }
                                        sb.AppendLine(gpxGenerator.Finish());
                                        //insert gpx header
                                        minLat = (from a in gpxBatchList select Utils.Conversion.StringToDouble(a.Latitude)).Min();
                                        maxLat = (from a in gpxBatchList select Utils.Conversion.StringToDouble(a.Latitude)).Max();
                                        minLon = (from a in gpxBatchList select Utils.Conversion.StringToDouble(a.Longitude)).Min();
                                        maxLon = (from a in gpxBatchList select Utils.Conversion.StringToDouble(a.Longitude)).Max();
                                        gpxGenerator.SetGeocacheList((from a in gpxBatchList select a.Code).ToList(), minLat, maxLat, minLon, maxLon);
                                        string gpxHeader = gpxGenerator.Start();
                                        sb.Insert(0, gpxHeader);
                                        gcList.RemoveRange(0, gpxBatchList.Count);

                                        //add gpx to zip
                                        byte[] data;
                                        using (System.IO.TemporaryFile tmp = new System.IO.TemporaryFile(true))
                                        {
                                            using (System.IO.StreamWriter sw = System.IO.File.CreateText(tmp.Path))
                                            {
                                                sw.Write(sb.ToString());
                                            }
                                            data = File.ReadAllBytes(tmp.Path);
                                        }
                                        string fn = string.Format("data/{0}_{1}.gpx", System.IO.Path.GetFileNameWithoutExtension(gpxSetting.FileName), fileIndex);
                                        ZipEntry entry = new ZipEntry(fn);
                                        entry.DateTime = DateTime.Now;
                                        s.PutNextEntry(entry);
                                        s.Write(data, 0, data.Length);

                                        Crc32 crc = new Crc32();
                                        crc.Update(data);
                                        //txt = doc.CreateTextNode(crc16.ComputeChecksum(data).ToString("X8"));
                                        txt = doc.CreateTextNode(crc.Value.ToString("X8"));
                                        elCRC.AppendChild(txt);

                                        int curPos = System.Text.UTF8Encoding.UTF8.GetBytes(gpxHeader).Length;
                                        for (int i = 0; i < geiList.Count; i++)
                                        {
                                            GeocacheEntryInfo gei = geiList[i];

                                            XmlElement chgEl = doc.CreateElement("gch");
                                            elFile.AppendChild(chgEl);

                                            el = doc.CreateElement("code");
                                            txt = doc.CreateTextNode(gei.Code ?? "");
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("name");
                                            txt = doc.CreateTextNode(gpxGenerator.validateXml(gei.Name ?? ""));
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("type");
                                            txt = doc.CreateTextNode((from a in ApplicationData.Instance.GeocacheTypes where a.GSAK == gei.CacheType select a.GPXTag).FirstOrDefault() ?? "");
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("lat");
                                            txt = doc.CreateTextNode(gei.kAfterLat ?? gei.Latitude);
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("lon");
                                            txt = doc.CreateTextNode(gei.kAfterLon ?? gei.Longitude);
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("file_pos");
                                            txt = doc.CreateTextNode(curPos.ToString());
                                            curPos += gei.FileLen;
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            el = doc.CreateElement("file_len");
                                            txt = doc.CreateTextNode(gei.FileLen.ToString());
                                            el.AppendChild(txt);
                                            chgEl.AppendChild(el);

                                            XmlElement ratingsEl = doc.CreateElement("ratings");
                                            chgEl.AppendChild(ratingsEl);

                                            el = doc.CreateElement("awesomeness");
                                            txt = doc.CreateTextNode("3.0");
                                            el.AppendChild(txt);
                                            ratingsEl.AppendChild(el);

                                            el = doc.CreateElement("difficulty");
                                            txt = doc.CreateTextNode(gei.Difficulty.ToString("0.#").Replace(',', '.'));
                                            el.AppendChild(txt);
                                            ratingsEl.AppendChild(el);

                                            el = doc.CreateElement("size");
                                            switch ((from a in ApplicationData.Instance.GeocacheContainers where a.Name == gei.Container select a.ID).FirstOrDefault())
                                            {
                                                case 1:
                                                    txt = doc.CreateTextNode("2.0");
                                                    break;
                                                case 5:
                                                    txt = doc.CreateTextNode("2.0");
                                                    break;
                                                case 6:
                                                    txt = doc.CreateTextNode("2.0");
                                                    break;
                                                case 2:
                                                    txt = doc.CreateTextNode("2.0");
                                                    break;
                                                case 3:
                                                    txt = doc.CreateTextNode("4.0");
                                                    break;
                                                case 4:
                                                    txt = doc.CreateTextNode("5.0");
                                                    break;
                                                case 8:
                                                    txt = doc.CreateTextNode("3.0");
                                                    break;
                                                default:
                                                    txt = doc.CreateTextNode("3.0");
                                                    break;
                                            }
                                            el.AppendChild(txt);
                                            ratingsEl.AppendChild(el);

                                            el = doc.CreateElement("terrain");
                                            txt = doc.CreateTextNode(gei.Terrain.ToString("0.#").Replace(',', '.'));
                                            el.AppendChild(txt);
                                            ratingsEl.AppendChild(el);

                                            if (gei.Found != 0)
                                            {
                                                el = doc.CreateElement("found");
                                                txt = doc.CreateTextNode("true");
                                                el.AppendChild(txt);
                                                chgEl.AppendChild(el);
                                            }
                                        }

                                        fileIndex++;
                                    }

                                    //add index file
                                    // index\com\garmin\geocaches\v0\index.xml
                                    /*
                                    <gch>
                                      <code>GC12345</code>
                                      <name>Cache name</name>
                                      <type>Traditional Cache</type>
                                      <lat>33.550217</lat>
                                      <lon>-117.660617</lon>
                                      <file_pos>5875</file_pos>
                                      <file_len>5783</file_len>
                                      <ratings>
                                         <awesomeness>3.0</awesomeness>
                                         <difficulty>1.5</difficulty>
                                         <size>5.0</size>
                                         <terrain>1.5</terrain>
                                      </ratings>
                                      <found>true</found>
                                    </gch>
                                     * 
                                     1 = Nano (not supported, unfortunately, by GC.com yet)
                                    2 = Micro
                                    3 = Small
                                    4 = Regular
                                    5 = Large 
                                     * 
                                     */
                                    using (System.IO.TemporaryFile tmp = new System.IO.TemporaryFile(true))
                                    {
                                        using (TextWriter sw = new StreamWriter(tmp.Path, false, Encoding.UTF8)) //Set encoding
                                        {
                                            doc.Save(sw);
                                        }
                                        byte[] data = File.ReadAllBytes(tmp.Path);
                                        ZipEntry entry = new ZipEntry("index/com/garmin/geocaches/v0/index.xml");
                                        entry.DateTime = DateTime.Now;
                                        s.PutNextEntry(entry);
                                        s.Write(data, 0, data.Length);
                                    }

                                    s.Finish();
                                    s.Close();
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
}
