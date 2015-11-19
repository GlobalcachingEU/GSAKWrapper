using GSAKWrapper.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public int ScriptEditorWindowHeight
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int ScriptEditorWindowWidth
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int ScriptEditorWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int ScriptEditorWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public GridLength ScriptEditorWindowLeftPanelWidth
        {
            get { return new GridLength(double.Parse(GetProperty("150"), CultureInfo.InvariantCulture)); }
            set { SetProperty(value.Value.ToString(CultureInfo.InvariantCulture)); }
        }

        public List<ScriptItem> GetScriptItems()
        {
            return _settingsStorage.GetScriptItems();
        }

        public ScriptItem GetScriptItem(string name)
        {
            return _settingsStorage.GetScriptItem(name);
        }

        public void StoreScriptItem(ScriptItem item)
        {
            _settingsStorage.StoreScriptItem(item);
        }

        public void DeleteScriptItem(string name)
        {
            _settingsStorage.DeleteScriptItem(name);
        }

    }
}
