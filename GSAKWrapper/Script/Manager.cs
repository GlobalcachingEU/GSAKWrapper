using csscript;
using CSScriptLibrary;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Script
{
    public class Manager
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        public const int ScriptTypeFilter = 1;
        public const int ScriptTypeAction = 2;

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            CSScriptLibrary.CSScript.ShareHostRefAssemblies = true;
            CSScriptLibrary.CSScript.CacheEnabled = true;
            CSScriptLibrary.CSScript.GlobalSettings.AddSearchDir(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        public string CompileScript(string script)
        {
            string result = "Success";
            try
            {
                CSScript.CompileCode(script);
            }
            catch (CompilerException ce)
            {
                StringBuilder sb = new StringBuilder();
                CompilerErrorCollection errors = (CompilerErrorCollection)ce.Data["Errors"];

                foreach (CompilerError err in errors)
                {
                    sb.AppendLine(string.Format("({1},{2}): {3} {4}: {5}",
                                        err.FileName,
                                        err.Line,
                                        err.Column,
                                        err.IsWarning ? "warning" : "error",
                                        err.ErrorNumber,
                                        err.ErrorText));
                }
                result = sb.ToString();
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        public IFilterScript LoadFilterScript(string code)
        {
            return CSScript.LoadCode(code)
                             .CreateInstance("GSAKWrapper.FilterScript")
                             .AlignToInterface<IFilterScript>();
        }

        public IActionScript LoadActionScript(string code)
        {
            return CSScript.LoadCode(code)
                             .CreateInstance("GSAKWrapper.ActionScript")
                             .AlignToInterface<IActionScript>();
        }
    }
}
