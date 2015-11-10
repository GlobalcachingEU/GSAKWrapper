using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter.Exception;

namespace GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter
{
    public class ArgumentRangeException : FormulaSolverException
    {
        public ArgumentRangeException(string msg) : base(msg) { }
        public ArgumentRangeException(string msg, System.Exception innerException) : base(msg, innerException) { }
    }
}
