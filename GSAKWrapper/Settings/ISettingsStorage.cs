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
        bool CreateBackup();
        List<string> AvailableBackups { get; }
        bool RemoveBackup(string id);
        bool PrepareRestoreBackup(string id);
    }
}
