using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public Version ApplicationVersion
        {
            get { return Version.Parse(GetProperty("0.0.0.0")); }
            set { SetProperty(value.ToString()); }
        }

        public Version ReleaseVersion
        {
            get { return Version.Parse(GetProperty("0.0.0.0")); }
            set { SetProperty(value.ToString()); }
        }

        public string ApplicationPath
        {
            get { return GetProperty(""); }
            set { SetProperty(value); }
        }

        public string ReleaseUrl
        {
            get { return GetProperty(""); }
            set { SetProperty(value); }
        }

        public bool NewVersionChecked
        {
            get { return bool.Parse(GetProperty(false.ToString())); }
            set { SetProperty(value.ToString()); }
        }

        public int VersionCheckedAtDay
        {
            get { return int.Parse(GetProperty("-1")); }
            set { SetProperty(value.ToString()); }
        }

    }
}
