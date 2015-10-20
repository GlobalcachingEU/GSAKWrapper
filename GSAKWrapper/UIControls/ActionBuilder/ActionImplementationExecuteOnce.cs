using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationExecuteOnce : ActionImplementation
    {
        private bool _hasBeenExecuted = false;

        public ActionImplementationExecuteOnce(string name)
            : base(name)
        {
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _hasBeenExecuted = false;
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (!_hasBeenExecuted)
            {
                _hasBeenExecuted = Execute();
            }
            DatabaseConnection.ExecuteNonQuery(string.Format("insert into {0} select Code as gccode from Caches inner join {1} on Caches.Code = {1}.gccode", targetTableName, inputTableName));
        }

        protected virtual bool Execute()
        {
            return true;
        }

        public override Operator AllowOperators
        {
            get { return Operator.Equal; }
        }

    }
}
