using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSAKWrapper.UIControls.ActionBuilder;

namespace GSAKWrapper
{
    public class FilterScript
    {
        //check https://github.com/GlobalcachingEU/GSAKWrapper/wiki for help

        public FilterScript()
        {
        }

        public bool PrepareRun(ActionImplementation owner, Database.DBCon db, string tableName)
        {
            return true;
        }

        public void Process(ActionImplementation owner, ActionImplementation.Operator op, string inputTableName, string targetTableName)
        {
        }

        public void FinalizeRun(ActionImplementation owner)
        {
        }

    }
}
