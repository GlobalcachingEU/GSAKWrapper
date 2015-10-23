using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionClearUserFlag : ActionImplementationExecuteOnce
    {
        public const string STR_NAME = "ClearUserFlag";
        public ActionClearUserFlag()
            : base(STR_NAME)
        {
        }
        protected override bool Execute()
        {
            DatabaseConnection.ExecuteNonQuery("update Caches set UserFlag=0 where UserFlag=1");
            return true;
        }
    }
}
