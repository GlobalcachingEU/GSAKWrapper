using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSAKWrapper.UIControls.ActionBuilder;

namespace GSAKWrapper.Script
{
    public interface IFilterScript
    {
        bool PrepareRun(ActionImplementation owner, Database.DBCon db, string tableName);
        void Process(ActionImplementation owner, ActionImplementation.Operator op, string inputTableName, string targetTableName);
        void FinalizeRun(ActionImplementation owner);
    }
}
