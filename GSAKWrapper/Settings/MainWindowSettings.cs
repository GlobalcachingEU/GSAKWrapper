using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public int MainWindowHeight
        {
            get { return int.Parse(GetProperty("800")); }
            set { SetProperty(value.ToString()); }
        }

        public int MainWindowWidth
        {
            get { return int.Parse(GetProperty("800")); }
            set { SetProperty(value.ToString()); }
        }

        public int MainWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int MainWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }
    }
}
