using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public int GeocacheCollectionWindowHeight
        {
            get { return int.Parse(GetProperty("600")); }
            set { SetProperty(value.ToString()); }
        }

        public int GeocacheCollectionWindowWidth
        {
            get { return int.Parse(GetProperty("600")); }
            set { SetProperty(value.ToString()); }
        }

        public int GeocacheCollectionWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int GeocacheCollectionWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public GridLength GeocacheCollectionWindowLeftPanelWidth
        {
            get { return new GridLength(double.Parse(GetProperty("300"), CultureInfo.InvariantCulture)); }
            set { SetProperty(value.Value.ToString(CultureInfo.InvariantCulture)); }
        }
    }
}
