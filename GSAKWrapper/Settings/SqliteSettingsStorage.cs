using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Data.Common;
using System.Globalization;
using System.ComponentModel;
using System.Windows;
using System.Data;
using GSAKWrapper.DataTypes;

namespace GSAKWrapper.Settings
{
    public class SqliteSettingsStorage: ISettingsStorage
    {
        private Database.DBCon _dbcon = null;
        private Hashtable _availableKeys;

        public static bool ApplicationRunning = false;
        public string RootSettingsFolder {get; private set; }

        public SqliteSettingsStorage()
        {
            _availableKeys = new Hashtable();
            try
            {
                var sf = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GSAKWrapper");
                RootSettingsFolder = sf;
                if (!Directory.Exists(sf))
                {
                    Directory.CreateDirectory(sf);
                }

                sf = Path.Combine(sf, "settings.db3");
                if (ApplicationRunning)
                {
                    _dbcon = new Database.DBConSqlite(sf);

                    if (!_dbcon.TableExists("settings"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'settings' (item_name text, item_value text)");
                        _dbcon.ExecuteNonQuery("create index idx_key on settings (item_name)");
                    }
                    else
                    {
                        DbDataReader dr = _dbcon.ExecuteReader("select item_name, item_value from settings");
                        while (dr.Read())
                        {
                            _availableKeys[dr[0] as string] = dr[1] as string;
                        }
                    }

                    if (!_dbcon.TableExists("formulasolv"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'formulasolv' (gccode text, formula text)");
                        _dbcon.ExecuteNonQuery("create index idx_form on formulasolv (gccode)");
                    }

                    if (!_dbcon.TableExists("GeocacheCollection"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'GeocacheCollection' (CollectionID integer not null, Name text not null)");
                        _dbcon.ExecuteNonQuery("create index idx_gckey on GeocacheCollection (CollectionID)");
                        _dbcon.ExecuteNonQuery("create index idx_gcname on GeocacheCollection (Name)");
                    }
                    if (!_dbcon.TableExists("GeocacheCollectionItem"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'GeocacheCollectionItem' (CollectionID integer not null, GeocacheCode text not null, Name text not null)");
                        _dbcon.ExecuteNonQuery("create unique index idx_gcikey on GeocacheCollectionItem (CollectionID, GeocacheCode)");

                        _dbcon.ExecuteNonQuery("CREATE TRIGGER Delete_GeocacheCollection Delete ON GeocacheCollection BEGIN delete from GeocacheCollectionItem where CollectionID = old.CollectionID; END");
                    }
                    if (!_dbcon.TableExists("ShapeFileItemV2"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'ShapeFileItemV2' (FileName text, TableName text not null, CoordType text not null, AreaType text not null, NamePrefix text, Encoding text not null, Enabled integer not null)");
                    }
                    if (!_dbcon.TableExists("Scripts"))
                    {
                        _dbcon.ExecuteNonQuery("create table 'Scripts' (Name text not null, ScriptType integer not null, Code text not null)");
                        _dbcon.ExecuteNonQuery("create unique index idx_scname on Scripts (Name)");
                    }

                    object o = _dbcon.ExecuteScalar("PRAGMA integrity_check");
                    if (o as string == "ok")
                    {
                        //what is expected
                    }
                    else
                    {
                        //oeps?
                        _dbcon.Dispose();
                        _dbcon = null;
                    }
                }
            }
            catch//(Exception e)
            {
                _dbcon = null;
            }
        }

        public bool IsStorageOK 
        {
            get
            {
                bool result;
                lock (this)
                {
                    result = _dbcon != null;
                }
                return result;
            }
        }

        public void StoreSetting(string name, string value)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    if (_availableKeys.ContainsKey(name))
                    {
                        _dbcon.ExecuteNonQuery(string.Format("update settings set item_value={1} where item_name='{0}'", name, value == null ? "NULL" : string.Format("'{0}'", value.Replace("'", "''"))));
                    }
                    else
                    {
                        _dbcon.ExecuteNonQuery(string.Format("insert into settings (item_name, item_value) values ('{0}', {1})", name, value == null ? "NULL" : string.Format("'{0}'", value.Replace("'", "''"))));
                    }
                    _availableKeys[name] = value;
                }
            }
        }

        public Hashtable LoadSettings()
        {
            Hashtable result = new Hashtable();
            foreach (DictionaryEntry kp in _availableKeys)
            {
                result.Add(kp.Key as string, kp.Value as string);
            }
            return result;
        }

        public void Dispose()
        {
            if (_dbcon!=null)
            {
                _dbcon.Dispose();
                _dbcon = null;
            }
        }

        public string GetFormula(string gcCode)
        {
            string result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    result = _dbcon.ExecuteScalar(string.Format("select formula from formulasolv where gccode='{0}'", gcCode)) as string;
                }
            }
            return result;
        }

        public void SetFormula(string gcCode, string formula)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    if (string.IsNullOrEmpty(formula))
                    {
                        _dbcon.ExecuteNonQuery(string.Format("delete from formulasolv where gccode='{0}'", gcCode));
                    }
                    else if (_dbcon.ExecuteNonQuery(string.Format("update formulasolv set formula='{1}' where gccode='{0}'", gcCode, formula.Replace("'", "''"))) == 0)
                    {
                        _dbcon.ExecuteNonQuery(string.Format("insert into formulasolv (gccode, formula) values ('{0}', '{1}')", gcCode, formula.Replace("'", "''")));
                    }
                }
            }
        }

        public List<GeocacheCollection> GetGeocacheCollections()
        {
            List<GeocacheCollection> result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        result = db.Fetch<GeocacheCollection>();
                    }
                }
            }
            return result;
        }

        public GeocacheCollection GetCollection(string name, bool createIfNotExists = false)
        {
            GeocacheCollection result = null;
            if (!string.IsNullOrEmpty(name))
            {
                lock (this)
                {
                    if (_dbcon != null)
                    {
                        using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                        {
                            result = db.FirstOrDefault<GeocacheCollection>("where Name like @0", name);
                            if (result == null && createIfNotExists)
                            {
                                //just the lazy way
                                var record = db.FirstOrDefault<GeocacheCollection>("order by CollectionID desc limit 1");
                                result = new GeocacheCollection();
                                result.Name = name;
                                result.CollectionID = record == null ? 1 : record.CollectionID + 1;
                                db.Insert(result);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<GeocacheCollectionItem> GetCollectionItems(int id)
        {
            List<GeocacheCollectionItem> result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        result = db.Fetch<GeocacheCollectionItem>("where CollectionID = @0", id);
                    }
                }
            }
            return result;
        }

        public void DeleteGeocacheCollection(int id)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        db.Execute("delete from GeocacheCollection where CollectionID = @0", id);
                    }
                }
            }
        }

        public void DeleteGeocacheCollectionItem(int colid, string code)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        db.Execute("delete from GeocacheCollectionItem where CollectionID = @0 and GeocacheCode = @1", colid, code);
                    }
                }
            }
        }

        public List<ShapefileItem> GetShapeFileItems()
        {
            List<ShapefileItem> result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        result = db.Fetch<ShapefileItem>("select * from ShapeFileItemV2");
                    }
                }
            }
            return result;
        }

        public void StoreShapeFileItems(List<ShapefileItem> items)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        db.BeginTransaction();
                        try
                        {
                            db.Execute("delete from ShapeFileItemV2");
                            foreach (var s in items)
                            {
                                db.Insert("ShapeFileItemV2", null, s);
                            }
                            db.CompleteTransaction();
                        }
                        catch
                        {
                            db.AbortTransaction();
                        }
                    }
                }
            }
        }

        public List<ScriptItem> GetScriptItems()
        {
            List<ScriptItem> result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        result = db.Fetch<ScriptItem>("select * from Scripts");
                    }
                }
            }
            return result;
        }

        public ScriptItem GetScriptItem(string name)
        {
            ScriptItem result = null;
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        result = db.FirstOrDefault<ScriptItem>("select * from Scripts where Name=@0", name);
                    }
                }
            }
            return result;
        }

        public void StoreScriptItem(ScriptItem item)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        if (db.FirstOrDefault<ScriptItem>("select * from Scripts where Name=@0", item.Name) == null)
                        {
                            db.Insert("Scripts", null, item);
                        }
                        else
                        {
                            db.Update("Scripts", "Name", item);
                        }
                    }
                }
            }
        }

        public void DeleteScriptItem(string name)
        {
            lock (this)
            {
                if (_dbcon != null)
                {
                    using (var db = new NPoco.Database(_dbcon.Connection, NPoco.DatabaseType.SQLite))
                    {
                        db.Execute("delete from Scripts where Name=@0", name);
                    }
                }
            }
        }

    }
}
