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

        public override bool PrepareRun()
        {
            _hasBeenExecuted = false;
            return base.PrepareRun();
        }

        public override ActionImplementation.Operator Process()
        {
            if (!_hasBeenExecuted)
            {
                _hasBeenExecuted = Execute();
            }
            return Operator.Equal;
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
