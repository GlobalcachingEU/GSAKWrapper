﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSAKWrapper.UIControls.ActionBuilder;

namespace GSAKWrapper
{
    public class ActionScript
    {
        //check https://github.com/GlobalcachingEU/GSAKWrapper/wiki for help

        public ActionScript()
        {
        }

        public bool PrepareRun(ActionImplementation owner, Database.DBCon db, string tableName)
        {
            return true;
        }

        public void FinalizeRun(ActionImplementation owner)
        {
        }

    }
}
