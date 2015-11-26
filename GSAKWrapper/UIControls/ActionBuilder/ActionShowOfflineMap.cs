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
    public class ActionShowOfflineMap : ActionImplementationAction
    {
        private SynchronizationContext _context = null;

        public const string STR_NAME = "ShowOfflineMap";
        public ActionShowOfflineMap()
            : base(STR_NAME)
        {
            _context = SynchronizationContext.Current;
            if (_context == null)
            {
                _context = new SynchronizationContext();
            }

        }

        public override UIElement GetUIElement()
        {
            Button b = new Button();
            b.Content = Localization.TranslationManager.Instance.Translate("ShowMap");
            b.Click += b_Click;
            return b;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new Dialogs.WindowOSMOfflineMap(null, null, null);
            wnd.Show();
        }

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();

            List<GSAKWrapper.MapProviders.GeocachePoco> gcl = null;
            double? cLat = null;
            double? cLon = null;
            using (var db = new NPoco.Database(DatabaseConnection.Connection, NPoco.DatabaseType.SQLite))
            {
                gcl = db.Fetch<GSAKWrapper.MapProviders.GeocachePoco>(string.Format("select Code, Name, CacheType, Found, IsOwner, Latitude, Longitude, kAfterLat, kAfterLon from Caches inner join {0} on Caches.Code={0}.gccode left join Corrected on Caches.Code=Corrected.kCode", ActionInputTableName));
            }
            if (gcl.Count > 0)
            {
                var dr = DatabaseConnection.ExecuteReader(string.Format("select AVG(Latitude), AVG(Longitude) from Caches inner join {0} on Caches.Code={0}.gccode left join Corrected on Caches.Code=Corrected.kCode", ActionInputTableName));
                if (dr.Read())
                {
                    cLat = dr.GetDouble(0);
                    cLon = dr.GetDouble(1);
                }
            }

            _context.Send(new SendOrPostCallback(delegate(object state)
            {
                var wnd = new Dialogs.WindowOSMOfflineMap(gcl, cLat, cLon);
                wnd.Show();
            }), null);

            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
