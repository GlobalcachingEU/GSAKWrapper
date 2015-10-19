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

namespace GSAKWrapper.Settings
{
    public class SqliteSettingsStorage: ISettingsStorage
    {
        private Database.DBCon _dbcon = null;
        private Hashtable _availableKeys;

        public static bool ApplicationRunning = false;
        private string RootSettingsFolder;

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

        public bool CreateBackup()
        {
            bool result = false;
            try
            {
                File.Copy(Path.Combine(RootSettingsFolder, "settings.db3"), Path.Combine(RootSettingsFolder, string.Format("settings.{0}.db3", DateTime.Now.ToString("s").Replace(" ", "-").Replace(":", "-"))), true);
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public List<string> AvailableBackups 
        {
            get
            {
                List<string> result;
                try
                {
                    string[] fls = Directory.GetFiles(RootSettingsFolder, "settings.*.db3");
                    result = (from s in fls select Path.GetFileName(s)).OrderBy(x => x).ToList();
                }
                catch
                {
                    result = new List<string>();
                }
                return result;
            }
        }

        public bool RemoveBackup(string id)
        {
            bool result = false;
            try
            {
                File.Delete(Path.Combine(RootSettingsFolder, id));
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public bool PrepareRestoreBackup(string id)
        {
            bool result = false;
            lock (this)
            {
                bool restoreConnection = _dbcon != null;
                try
                {
                    if (restoreConnection)
                    {
                        //copy settings.db3 to settings.db3.bak
                        if (_dbcon != null)
                        {
                            _dbcon.Dispose();
                            _dbcon = null;
                        }
                        File.Copy(Path.Combine(RootSettingsFolder, "settings.db3"), Path.Combine(RootSettingsFolder, "settings.db3.bak"), true);
                    }
                    File.Copy(Path.Combine(RootSettingsFolder, id), Path.Combine(RootSettingsFolder, "settings.db3"), true);

                    if (restoreConnection)
                    {
                        //connect to previous settings file, so the backup is not overwritten.
                        //application needs to restart
                        //after restart the backup is used
                        string sf = Path.Combine(RootSettingsFolder, "settings.db3.bak");
                        _dbcon = new Database.DBConSqlite(sf);
                    }

                    result = true;
                }
                catch
                {
                    if (restoreConnection)
                    {
                        string sf = Path.Combine(RootSettingsFolder, "settings.db3");
                        if (_dbcon != null)
                        {
                            _dbcon.Dispose();
                            _dbcon = null;
                        }
                        _dbcon = new Database.DBConSqlite(sf);
                    }
                }
            }
            return result;
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

    }
}
