using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public string SelectedCulture
        {
            get { return GetProperty(""); }
            set { SetProperty(value); }
        }
    }
}
