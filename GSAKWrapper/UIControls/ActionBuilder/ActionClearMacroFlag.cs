using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionClearMacroFlag : ActionImplementationExecuteOnce
    {
        public const string STR_NAME = "ClearMacroFlag";
        public ActionClearMacroFlag()
            : base(STR_NAME)
        {
        }
        protected override bool Execute()
        {
            DatabaseConnection.ExecuteNonQuery("update Caches set MacroFlag=0 where MacroFlag=1");
            return true;
        }
    }
}
