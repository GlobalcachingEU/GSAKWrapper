using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationAction : ActionImplementation
    {
        public ActionImplementationAction(string name)
            : base(name)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Action; } }


        public override Operator AllowOperators
        {
            get { return 0; }
        }

    }
}
