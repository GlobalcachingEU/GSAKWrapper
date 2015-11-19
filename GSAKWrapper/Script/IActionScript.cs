using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSAKWrapper.UIControls.ActionBuilder;

namespace GSAKWrapper.Script
{
    public interface IActionScript
    {
        bool PrepareRun(ActionImplementation owner, Database.DBCon db, string tableName);
        void FinalizeRun(ActionImplementation owner);
    }
}
