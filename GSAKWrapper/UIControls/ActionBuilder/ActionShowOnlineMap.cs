using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionShowOnlineMap : ActionImplementationAction
    {
        private SynchronizationContext _context = null;

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
        }

        public class JSCallBack
        {
            public JSCallBack()
            {
            }

            public void GeocacheClicked(string code)
            {
                //make it active in GSAK, but only if GSAK already running, but wrapper not executed from within gsak
                if (Utils.GSAK.IsGSAKRunning && !Settings.Settings.Default.ExecutedWithParameters)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(System.IO.Path.Combine(Settings.Settings.Default.GSAKExecutablePath, "gsak.exe"), string.Format("gsak://%FF/Search/{0}", code.Split(new char[]{'-'}, 2)[0].Trim()));
                    }
                    catch
                    {
                    }
                }
            }
        }

        public const string STR_NAME = "ShowOnlineMap";
        public ActionShowOnlineMap()
            : base(STR_NAME)
        {
            _context = SynchronizationContext.Current;
            if (_context == null)
            {
                _context = new SynchronizationContext();
            }

        }

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();
            //generate map HTML
            string html = Utils.ResourceHelper.GetEmbeddedTextFile("/Resources/ActionShowOnlineMap.html");

            //icons
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("var foundIcon = new google.maps.MarkerImage(\"{0}\");", Utils.ResourceHelper.GetEmbeddedHtmlImageData("/Resources/Map/found.png")));
            sb.AppendLine(string.Format("var unknownIcon = new google.maps.MarkerImage(\"{0}\");", Utils.ResourceHelper.GetEmbeddedHtmlImageData("/Resources/Map/0.png")));
            sb.AppendLine(string.Format("var myownIcon = new google.maps.MarkerImage(\"{0}\");", Utils.ResourceHelper.GetEmbeddedHtmlImageData("/Resources/Map/myown.png")));
            foreach (var gctype in ApplicationData.Instance.GeocacheTypes)
            {
                sb.AppendLine(string.Format("var gct{0}Icon = new google.maps.MarkerImage(\"{1}\");", gctype.ID.ToString().Replace("-", "_"), Utils.ResourceHelper.GetEmbeddedHtmlImageData(string.Format("/Resources/Map/{0}.png", gctype.ID))));
                sb.AppendLine(string.Format("var gct{0}IconC = new google.maps.MarkerImage(\"{1}\");", gctype.ID.ToString().Replace("-", "_"), Utils.ResourceHelper.GetEmbeddedHtmlImageData(string.Format("/Resources/Map/c{0}.png", gctype.ID))));
            }
            foreach (var wptype in ApplicationData.Instance.WaypointTypes)
            {
                sb.AppendLine(string.Format("var wpt{0}Icon = new google.maps.MarkerImage(\"{1}\");", wptype.ID.ToString().Replace("-", "_"), Utils.ResourceHelper.GetEmbeddedHtmlImageData(string.Format("/Resources/Map/{0}.gif", wptype.ID))));
            }
            html = html.Replace("//icons", sb.ToString());

            sb.Length = 0;
            using (var db = new NPoco.Database(DatabaseConnection.Connection, NPoco.DatabaseType.SQLite))
            {
                var gcl = db.Fetch<GeocachePoco>(string.Format("select Code, Name, CacheType, Found, IsOwner, Latitude, Longitude, kAfterLat, kAfterLon from Caches inner join {0} on Caches.Code={0}.gccode left join Corrected on Caches.Code=Corrected.kCode", ActionInputTableName));
                foreach (var gc in gcl)
                {
                    var gcicon = "gct0Icon";
                    if (gc.IsOwner != 0)
                    {
                        gcicon = "myownIcon";
                    }
                    else if (gc.Found != 0)
                    {
                        gcicon = "foundIcon";
                    }
                    else
                    {
                        var gctype = (from a in ApplicationData.Instance.GeocacheTypes where a.GSAK == gc.CacheType select a).FirstOrDefault();
                        if (gctype != null)
                        {
                            if (gc.kAfterLat != null)
                            {
                                gcicon = string.Format("gct{0}IconC", gctype.ID);
                            }
                            else
                            {
                                gcicon = string.Format("gct{0}Icon", gctype.ID);
                            }
                        }
                    }
                    sb.AppendFormat("markers.push(addClickListener(new MarkerWithLabel({{position: new google.maps.LatLng({1},{2}),icon:{3},title:'{0}',labelContent:'{0}',labelAnchor: new google.maps.Point(10, 0),labelClass:'labels'}})));", string.Format("{0}-{1}", gc.Code, gc.Name.Replace("'", "").Replace("\\", "")), gc.kAfterLat == null ? gc.Latitude : gc.kAfterLat, gc.kAfterLon == null ? gc.Longitude : gc.kAfterLon, gcicon);
                }
                sb.AppendLine();
                sb.AppendLine("markerClusterer = new MarkerClusterer(map, markers, clusterOptions);");
                if (gcl.Count > 0)
                {
                    var dr = DatabaseConnection.ExecuteReader(string.Format("select AVG(Latitude), AVG(Longitude) from Caches inner join {0} on Caches.Code={0}.gccode left join Corrected on Caches.Code=Corrected.kCode", ActionInputTableName));
                    if (dr.Read())
                    {
                        var lat = dr.GetDouble(0);
                        var lon = dr.GetDouble(1);
                        sb.AppendLine(string.Format("map.setCenter(new google.maps.LatLng({0}, {1}));", lat.ToString(CultureInfo.InvariantCulture), lon.ToString(CultureInfo.InvariantCulture)));
                    }
                }
            }
            html = html.Replace("//geocachelist", sb.ToString());

            _context.Send(new SendOrPostCallback(delegate(object state)
            {
                var wnd = new Dialogs.WindowWebBrowser(html, new WebBrowserControl.JSCallback() { Name = "bound", Instance = new JSCallBack() });
                wnd.Show();
            }), null);

            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
