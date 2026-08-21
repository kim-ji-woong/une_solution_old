using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Dynamic;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Binding;

using Microsoft.Scripting;
using Microsoft.Scripting.Debugging;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using System.Diagnostics;


namespace PreSafe
{
    internal class ScriptProxy
    {
        private static ScriptProxy m_Instance = null;
        public static ScriptProxy Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ScriptProxy();
                return m_Instance;
            }
        }

        private dynamic scope = null;
        public dynamic UserObject
        {
            get { return scope; }
        }

        private ScriptEngine m_Engine = null;
        public ScriptEngine Engine
        {
            get { return m_Engine; }
            set { m_Engine = value; }
        }

        private ScriptScope m_Scope = null;

        public ScriptProxy()
        {
            CreateEngine();
        }

        private void CreateEngine()
        {
            if (m_Engine == null)
            {
                m_Engine = Python.CreateEngine();


                var pc = HostingHelpers.GetLanguageContext(m_Engine) as PythonContext;
                var hooks = pc.SystemState.Get__dict__()["path_hooks"] as List;
                hooks.Clear();

                string szPath = AppDomain.CurrentDomain.BaseDirectory;

                string szFullPath = Directory.GetParent(szPath).FullName;
                string strPath = szFullPath + "\\Lib";

                var paths = m_Engine.GetSearchPaths();
                paths.Add(szFullPath);
                paths.Add(strPath);
                //paths.Add(strToolPath);
                //paths.Add(cPython);
                m_Engine.SetSearchPaths(paths);

                MemoryStream ms = new MemoryStream();

                outputWr = new EventRaisingStreamWriter(ms);
                outputWr.StringWritten += new EventHandler<MyEvtArgs<string>>(sWr_StringWritten);

                m_Engine.Runtime.IO.SetOutput(ms, outputWr);

                //m_Engine.Runtime.IO.RedirectToConsole();
                scope = m_Scope = m_Engine.CreateScope();

                //m_Scope.SetVariable("log", _logger);
                //_logger.AddInfo("User Command Object Initialized");

            }
        }
        private EventRaisingStreamWriter outputWr = null;
        public void ImportModule(string szModuleName)
        {
            try
            {
                m_Engine.Runtime.ImportModule(szModuleName);
            }
            catch (System.Exception ex)
            {
                //_logger.AddFault(ex);
                //Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
        private string m_szResult = "";
        public string Result
        {
            get { return m_szResult; }
            set { m_szResult = value; }
        }

        void sWr_StringWritten(object sender, MyEvtArgs<string> e)
        {
            m_szResult += e.Value;
        }

        public void AddVariable(string szName, object varObj)
        {
            m_Scope.SetVariable(szName, varObj);
        }

        public object GetVariable(string szName)
        {
            return m_Scope.GetVariable(szName);
        }


        private dynamic CompileSourceAndExecute(String code)
        {
            m_szResult = "";
            try
            {
                ScriptSource source = m_Engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                CompiledCode compiled = source.Compile();
                // Executes in the scope of Python
                dynamic dyn = compiled.Execute(m_Scope);
                return dyn;

            }
            catch (System.Exception ex)
            {
                //_logger.AddFault(ex);
                throw ex;
            }


        }


        public dynamic RunPythonScript(string code)
        {
            try
            {
                return CompileSourceAndExecute(code);
            }
            catch (System.IO.IOException ex)
            {
                //_logger.AddFault(ex);
                throw ex;
            }
            catch (IronPython.Runtime.Exceptions.ImportException exx)
            {
                //_logger.AddFault(exx);
                throw exx;
            }
        }

        public void Call(string callActionCode)
        {
            try
            {
                var script = @"" + callActionCode;
                dynamic scope = m_Scope;
                //_logger.AddInfo(callActionCode).Tag = callActionCode;
                m_Engine.Execute(script, m_Scope);
            }
            catch (System.MissingMemberException ex)
            {
                string szMsg = "User Function Object has no attribute '" + callActionCode + "'";
                //_logger.AddError(szMsg).Tag = callActionCode;
                throw ex;
            }
            catch (System.Exception ex)
            {
                //_logger.AddFault(ex).Tag = callActionCode;
                throw ex;
            }
        }

        public object CallAction(string callFuncCode)
        {
            try
            {
                var script = @"proxyFuncResult = " + callFuncCode;

                object proxyFuncResult = new object();
                dynamic scope = m_Scope;
                m_Scope.SetVariable(@"proxyFuncResult", proxyFuncResult);
                m_Engine.Execute(script, m_Scope);
                proxyFuncResult = m_Scope.GetVariable("proxyFuncResult");
                //_logger.AddInfo("# Run proxy function : " + callFuncCode).Tag = callFuncCode;
                //_logger.AddInfo(callFuncCode).Tag = callFuncCode;
                //_logger.AddInfo(callFuncCode + " : " + proxyFuncResult).Tag = callFuncCode;
                return proxyFuncResult;
            }
            catch (System.MissingMemberException ex)
            {
                string szMsg = "User Function Object has no attribute '" + callFuncCode + "'";
                //_logger.AddError(szMsg);
                throw ex;
            }
            catch (System.Exception ex)
            {
                //_logger.AddFault(ex);
                throw ex;
            }
        }

        public object InvokeClassFunc(string className, string functionName, object[] args)
        {
            try
            {
                dynamic szclass = className + "()";
                dynamic instance = m_Engine.Execute(szclass, m_Scope);
                ObjectOperations ops = m_Engine.CreateOperations(m_Scope);
                return ops.InvokeMember(instance, functionName, args);
            }
            catch (System.Exception ex)
            {
                //_logger.AddFault(ex);
                throw ex;
            }
        }

        public class MyEvtArgs<T> : EventArgs
        {
            public T Value
            {
                get;
                private set;
            }
            public MyEvtArgs(T value)
            {
                this.Value = value;
            }
        }


        public class EventRaisingStreamWriter : StreamWriter
        {
            #region Event
            public event EventHandler<MyEvtArgs<string>> StringWritten;
            #endregion

            #region CTOR
            public EventRaisingStreamWriter(Stream s)
                : base(s)
            { }
            #endregion

            #region Private Methods
            private void LaunchEvent(string txtWritten)
            {
                if (StringWritten != null)
                {
                    StringWritten(this, new MyEvtArgs<string>(txtWritten));
                }
            }
            #endregion


            #region Overrides

            public override void Write(string value)
            {
                base.Write(value);
                LaunchEvent(value);
                Debug.WriteLine(value);
            }
            public override void Write(bool value)
            {
                base.Write(value);
                LaunchEvent(value.ToString());
                Debug.WriteLine(value);
            }
            // here override all writing methods...

            #endregion
        }
    }
}

