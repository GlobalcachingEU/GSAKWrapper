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
        public string FlowSequencesXml
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }

        public int FlowSequenceEditorHeight
        {
            get { return int.Parse(GetProperty("500")); }
            set { SetProperty(value.ToString()); }
        }

        public int FlowSequenceEditorWidth
        {
            get { return int.Parse(GetProperty("700")); }
            set { SetProperty(value.ToString()); }
        }

        public int FlowSequenceEditorTop
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public int FlowSequenceEditorLeft
        {
            get { return int.Parse(GetProperty("10")); }
            set { SetProperty(value.ToString()); }
        }

        public GridLength FlowSequenceEditorLeftPanelWidth
        {
            get { return new GridLength(double.Parse(GetProperty("250"), CultureInfo.InvariantCulture)); }
            set { SetProperty(value.Value.ToString(CultureInfo.InvariantCulture)); }
        }
    }
}
