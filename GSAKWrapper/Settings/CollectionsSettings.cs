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
        public List<GeocacheCollection> GetGeocacheCollections()
        {
            return _settingsStorage.GetGeocacheCollections();
        }

        public GeocacheCollection GetCollection(string name, bool createIfNotExists = false)
        {
            return _settingsStorage.GetCollection(name, createIfNotExists: createIfNotExists);
        }

        public List<GeocacheCollectionItem> GetCollectionItems(int id)
        {
            return _settingsStorage.GetCollectionItems(id);
        }

        public void DeleteGeocacheCollection(int id)
        {
            _settingsStorage.DeleteGeocacheCollection(id);
        }

        public void DeleteGeocacheCollectionItem(int colid, string code)
        {
            _settingsStorage.DeleteGeocacheCollectionItem(colid, code);
        }

    }
}
