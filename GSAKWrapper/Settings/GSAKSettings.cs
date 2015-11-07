using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public string GSAKSettingsPath
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }

        public string DatabaseFolderPath
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }

        public string SelectedDatabase
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }
    }
}
