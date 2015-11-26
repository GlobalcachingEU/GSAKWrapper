using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public string MapsOSMBinOfflineMapFiles
        {
            get { return GetProperty(""); }
            set { SetProperty(value); }
        }

        public int OSMOfflineMapWindowHeight
        {
            get { return int.Parse(GetProperty("500")); }
            set { SetProperty(value.ToString()); }
        }

        public int OSMOfflineMapWindowWidth
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int OSMOfflineMapWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int OSMOfflineMapWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

    }
}
