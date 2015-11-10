using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter.Functions.TrigonometricFunctions
{
    public class Tan : TrigonometricFunction
    {
        protected override object ExecuteTrigFunction(double arg)
        {
            return Math.Tan(arg);
        }
    }
}
