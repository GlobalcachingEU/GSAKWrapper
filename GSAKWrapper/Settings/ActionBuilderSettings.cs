using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public string ActionBuilderFlowsXml
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }

        public string ActionBuilderActiveFlowName
        {
            get { return GetProperty(null); }
            set { SetProperty(value); }
        }

        public int ActionBuilderFlowID
        {
            get { return int.Parse(GetProperty("1")); }
            set { SetProperty(value.ToString()); }
        }
    }
}
