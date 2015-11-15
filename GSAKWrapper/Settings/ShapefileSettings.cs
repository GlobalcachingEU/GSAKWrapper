using GSAKWrapper.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
