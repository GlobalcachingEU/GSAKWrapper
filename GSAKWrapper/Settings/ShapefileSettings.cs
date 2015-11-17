using GSAKWrapper.DataTypes;
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
        public List<ShapefileItem> GetShapeFileItems()
        {
            return _settingsStorage.GetShapeFileItems();
        }

        public void StoreShapeFileItems(List<ShapefileItem> items)
        {
            _settingsStorage.StoreShapeFileItems(items);
        }

        public int ShapefileWindowHeight
        {
            get { return int.Parse(GetProperty("300")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileWindowWidth
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileTestWindowHeight
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileTestWindowWidth
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileTestWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int ShapefileTestWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public GridLength ShapefileTestWindowLeftPanelWidth
        {
            get { return new GridLength(double.Parse(GetProperty("250"), CultureInfo.InvariantCulture)); }
            set { SetProperty(value.Value.ToString(CultureInfo.InvariantCulture)); }
        }

    }
}
