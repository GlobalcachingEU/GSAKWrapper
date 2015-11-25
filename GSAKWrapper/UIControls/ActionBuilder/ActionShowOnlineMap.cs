using System;
using System.Collections.Generic;
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
            sb.AppendLine(string.Format("var foundIcon = new google.maps.MarkerImage(\"gapp://{0}\");", "Resources/Maps/found.png"));
            foreach (var gctype in ApplicationData.Instance.GeocacheTypes)
            {
                sb.AppendLine(string.Format("var gct{0}Icon = new google.maps.MarkerImage(\"gapp://Resources/Maps/{1}.png\");", gctype.ID));
                sb.AppendLine(string.Format("var gct{0}IconC = new google.maps.MarkerImage(\"gapp://Resources/Maps/{1}.png\");", gctype.ID));
            }
            foreach (var wptype in ApplicationData.Instance.WaypointTypes)
            {
                sb.AppendLine(string.Format("var wpt{0}Icon = new google.maps.MarkerImage(\"gapp://Resources/Maps/{1}.png\");", wptype.ID));
            }
            html = html.Replace("//icons", sb.ToString());
            //todo

            _context.Send(new SendOrPostCallback(delegate(object state)
            {
                var wnd = new Dialogs.WindowWebBrowser(html);
                wnd.Show();
            }), null);

            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
