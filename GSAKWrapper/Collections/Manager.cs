using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Collections
{
    public class Manager
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        public string DatabaseFilePath { get; private set; }

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            DatabaseFilePath = System.IO.Path.Combine(Settings.Settings.Default.SettingsFolder, "collections.db3");

            if (Settings.Settings.ApplicationRunning)
            {
                try
                {
                    using (var db = new Database.DBConSqlite(DatabaseFilePath))
                    {
                        if (!db.TableExists("GeocacheCollection"))
                        {
                            db.ExecuteNonQuery("create table 'GeocacheCollection' (CollectionID integer not null, Name text not null)");
                            db.ExecuteNonQuery("create index idx_gckey on GeocacheCollection (CollectionID)");
                            db.ExecuteNonQuery("create index idx_gcname on GeocacheCollection (Name)");
                        }
                        if (!db.TableExists("GeocacheCollectionItem"))
                        {
                            db.ExecuteNonQuery("create table 'GeocacheCollectionItem' (CollectionID integer not null, GeocacheCode text not null, Name text not null)");
                            db.ExecuteNonQuery("create unique index idx_gcikey on GeocacheCollectionItem (CollectionID, GeocacheCode)");

                            db.ExecuteNonQuery("CREATE TRIGGER Delete_GeocacheCollection Delete ON GeocacheCollection BEGIN delete from GeocacheCollectionItem where CollectionID = old.CollectionID; END");
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        public List<GeocacheCollection> GetGeocacheCollections()
        {
            List<GeocacheCollection> result = null;
            try
            {
                using (var temp = new Database.DBConSqlite(DatabaseFilePath))
                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                {
                    result = db.Fetch<GeocacheCollection>();
                }
            }
            catch
            {
                result = new List<GeocacheCollection>();
            }
            return result;
        }

        public GeocacheCollection GetCollection(string name, bool createIfNotExists = false)
        {
            GeocacheCollection result = null;
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    using (var temp = new Database.DBConSqlite(DatabaseFilePath))
                    using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
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
                catch
                {
                }
            }
            return result;
        }

        public List<GeocacheCollectionItem> GetCollectionItems(int id)
        {
            List<GeocacheCollectionItem> result = null;
            try
            {
                using (var temp = new Database.DBConSqlite(DatabaseFilePath))
                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                {
                    result = db.Fetch<GeocacheCollectionItem>("where CollectionID = @0", id);
                }
            }
            catch
            {
            }
            return result;
        }

        public void DeleteGeocacheCollection(int id)
        {
            try
            {
                using (var temp = new Database.DBConSqlite(DatabaseFilePath))
                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                {
                    db.Execute("delete from GeocacheCollection where CollectionID = @0", id);
                }
            }
            catch
            {
            }
        }

        public void DeleteGeocacheCollectionItem(int colid, string code)
        {
            try
            {
                using (var temp = new Database.DBConSqlite(DatabaseFilePath))
                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                {
                    db.Execute("delete from GeocacheCollectionItem where CollectionID = @0 and GeocacheCode = @1", colid, code);
                }
            }
            catch
            {
            }
        }

    }
}
