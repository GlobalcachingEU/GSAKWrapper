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
        public static string DatabaseFolderPath
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
                            var dbPath = gsak.GetValue(fexePath) as string;
                            if (!string.IsNullOrEmpty(dbPath) && System.IO.Directory.Exists(System.IO.Path.Combine(dbPath, "data")))
                            {
                                result = System.IO.Path.Combine(dbPath, "data");
                            }
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

        public static string GetFullDatabasePath(string database)
        {
            return System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, database, "sqlite.db3");
        }
    }
}
