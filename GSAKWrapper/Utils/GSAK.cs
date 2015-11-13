using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Utils
{
    public class GSAK
    {
        public static string ExecutablePath
        {
            get
            {
                string result = null;
                try
                {
                    var gsak = Registry.CurrentUser.OpenSubKey(@"Software\GSAK");
                    if (gsak != null)
                    {
                        result = gsak.GetValue("ExePath") as string;
                        if (!string.IsNullOrEmpty(result))
                        {
                            result = System.IO.Path.GetFullPath(result);
                        }
                        gsak.Dispose();
                    }

                }
                catch
                {
                }
                return result;
            }
        }

        public static string SettingsFolderPath
        {
            get
            {
                string result = null;
                try
                {
                    var gsak = Registry.CurrentUser.OpenSubKey(@"Software\GSAK");
                    if (gsak != null)
                    {
                        var exePath = gsak.GetValue("ExePath") as string;
                        if (exePath != null)
                        {
                            var fexePath = System.IO.Path.GetFullPath(exePath);
                            result = gsak.GetValue(fexePath) as string;
                        }
                        gsak.Dispose();
                    }

                }
                catch
                {
                }
                return result;
            }
        }

        public static string DatabaseFolderPath
        {
            get
            {
                string result = null;
                try
                {
                    var sp = SettingsFolderPath;
                    if (!string.IsNullOrEmpty(sp) && System.IO.Directory.Exists(System.IO.Path.Combine(sp, "data")))
                    {
                        result = System.IO.Path.Combine(sp, "data");
                    }
                }
                catch
                {
                }
                return result;
            }
        }

        public static List<string> AvailableDatabases
        {
            get
            {
                var result = new List<string>();
                if (!string.IsNullOrEmpty(Settings.Settings.Default.DatabaseFolderPath))
                {
                    try
                    {
                        string[] dirs = System.IO.Directory.GetDirectories(Settings.Settings.Default.DatabaseFolderPath);
                        foreach (var d in dirs)
                        {
                            if (System.IO.File.Exists(System.IO.Path.Combine(d, "sqlite.db3")))
                            {
                                var sp = d.Substring(Settings.Settings.Default.DatabaseFolderPath.Length + 1);
                                result.Add(sp);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                return result;
            }
        }

        public static List<DataTypes.GSAKCustomGlobal> GlobalCustomFields
        {
            get
            {
                var result = new List<DataTypes.GSAKCustomGlobal>();
                try
                {
                    if (!string.IsNullOrEmpty(Settings.Settings.Default.GSAKSettingsPath))
                    {
                        string fn = System.IO.Path.Combine(Settings.Settings.Default.GSAKSettingsPath, "gsak.db3");
                        if (System.IO.File.Exists(fn))
                        {
                            using (var temp = new Database.DBConSqlite(fn))
                            {
                                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                                {
                                    result = db.Fetch<DataTypes.GSAKCustomGlobal>("select * from CustomGlobal");
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                return result;
            }
        }

        public static List<string> Locations
        {
            get
            {
                var result = new List<string>();
                try
                {
                    if (!string.IsNullOrEmpty(Settings.Settings.Default.GSAKSettingsPath))
                    {
                        string fn = System.IO.Path.Combine(Settings.Settings.Default.GSAKSettingsPath, "gsak.db3");
                        if (System.IO.File.Exists(fn))
                        {
                            using (var temp = new Database.DBConSqlite(fn))
                            {
                                using (var db = new NPoco.Database(temp.Connection, NPoco.DatabaseType.SQLite))
                                {
                                    var record = db.FirstOrDefault<string>("select Data from Settings where Type='LO' and Description='Locations'");
                                    if (!string.IsNullOrEmpty(record))
                                    {
                                        var lines = record.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach(var l in lines)
                                        {
                                            if (!l.Trim().StartsWith("#"))
                                            {
                                                result.Add(l.Trim());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                return result;
            }
        }


        public static string GetFullDatabasePath(string database)
        {
            return System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, database, "sqlite.db3");
        }

        public static bool IsGSAKRunning
        {
            get
            {
                bool result = false;
                try
                {
                    var procs = System.Diagnostics.Process.GetProcessesByName("gsak");
                    result = procs != null && procs.Length > 0;
                }
                catch
                {
                }
                return result;
            }
        }
    }
}
