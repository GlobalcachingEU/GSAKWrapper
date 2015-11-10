using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter
{
    public abstract class Expression : GCToken
    {
        public abstract object GetValue(ExecutionContext ctx);
    }
}
