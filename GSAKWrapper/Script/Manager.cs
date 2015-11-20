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

        public object LoadFilterScript(string code)
        {
            return CSScript.LoadCode(code).CreateInstance("GSAKWrapper.FilterScript");
        }

        public object LoadActionScript(string code)
        {
            return CSScript.LoadCode(code).CreateInstance("GSAKWrapper.ActionScript");
        }

        public List<ScriptProperty> GetScriptProperties(object obj)
        {
            var result = new List<ScriptProperty>();
            try
            {
                var props = obj.GetType().GetProperties();
                foreach(var prop in props)
                {
                    var sp = new ScriptProperty();
                    sp.Name = prop.Name;
                    sp.Type = prop.PropertyType;
                    sp.Value = prop.GetValue(obj);
                    result.Add(sp);
                }
            }
            catch
            {
            }
            return result;
        }

        public List<ScriptProperty> GetScriptProperties(string name)
        {
            List<ScriptProperty> result;
            var scr = Settings.Settings.Default.GetScriptItem(name);
            if (scr != null)
            {
                try
                {
                    if (scr.ScriptType == ScriptTypeFilter)
                    {
                        result = GetScriptProperties(LoadFilterScript(scr.Code));
                    }
                    else if (scr.ScriptType == ScriptTypeAction)
                    {
                        result = GetScriptProperties(LoadActionScript(scr.Code));
                    }
                    else
                    {
                        result = new List<ScriptProperty>();
                    }
                }
                catch
                {
                    result = new List<ScriptProperty>();
                }
            }
            else
            {
                result = new List<ScriptProperty>();
            }
            return result;
        }

        public void SetProprtyValues(object scriptObject, List<PropValue> values)
        {
            var props = GetScriptProperties(scriptObject);
            foreach (var prop in props)
            {
                var v = (from a in values where a.Name == prop.Name select a.Value).FirstOrDefault();
                if (v != null)
                {
                    var t = Convert.ChangeType(v, prop.Type);
                    scriptObject.GetType().GetProperty(prop.Name).SetValue(scriptObject, t);
                }
            }
        }
    }
}
