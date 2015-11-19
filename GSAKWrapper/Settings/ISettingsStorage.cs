using GSAKWrapper.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public interface ISettingsStorage: IDisposable
    {
        void StoreSetting(string name, string value);
        Hashtable LoadSettings();

        //integrity of settings
        bool IsStorageOK { get; }

        //formula solver
        string GetFormula(string gcCode);
        void SetFormula(string gcCode, string formula);

        //collections
        List<GeocacheCollection> GetGeocacheCollections();
        GeocacheCollection GetCollection(string name, bool createIfNotExists = false);
        List<GeocacheCollectionItem> GetCollectionItems(int id);
        void DeleteGeocacheCollection(int id);
        void DeleteGeocacheCollectionItem(int colid, string code);

        //Shapefiles
        List<ShapefileItem> GetShapeFileItems();
        void StoreShapeFileItems(List<ShapefileItem> items);

        //Scripts
        List<ScriptItem> GetScriptItems();
        ScriptItem GetScriptItem(string name);
        void StoreScriptItem(ScriptItem item);
        void DeleteScriptItem(string name);
    }
}
