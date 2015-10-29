﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Settings
{
    public partial class Settings
    {
        public Version ApplicationVersion
        {
            get { return Version.Parse(GetProperty("0.0.0.0")); }
            set { SetProperty(value.ToString()); }
        }

        public string ApplicationPath
        {
            get { return GetProperty(""); }
            set { SetProperty(value); }
        }
    }
}
