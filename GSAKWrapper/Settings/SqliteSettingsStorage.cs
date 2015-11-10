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


    }
}
