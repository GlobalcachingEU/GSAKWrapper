using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter.Exception;

namespace GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter
{
    public class ArgumentCountException : FormulaSolverException
    {
        public ArgumentCountException(string msg) : base(msg) { }
        public ArgumentCountException(string msg, System.Exception innerException) : base(msg, innerException) { }
    }
}
