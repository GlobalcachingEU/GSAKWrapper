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
        public int FormulaSolverWindowHeight
        {
            get { return int.Parse(GetProperty("600")); }
            set { SetProperty(value.ToString()); }
        }

        public int FormulaSolverWindowWidth
        {
            get { return int.Parse(GetProperty("600")); }
            set { SetProperty(value.ToString()); }
        }

        public int FormulaSolverWindowTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int FormulaSolverWindowLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public GridLength FormulaSolverWindowLeftPanelWidth
        {
            get { return new GridLength(double.Parse(GetProperty("300"), CultureInfo.InvariantCulture)); }
            set { SetProperty(value.Value.ToString(CultureInfo.InvariantCulture)); }
        }

        public string GetFormula(string gcCode)
        {
            return _settingsStorage.GetFormula(gcCode);
        }
        public void SetFormula(string gcCode, string formula)
        {
            _settingsStorage.SetFormula(gcCode, formula);
        }
    }
}
