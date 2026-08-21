using System;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Binding;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;

namespace UBMLViewer
{
    public class ScriptProxy
    {
        private static ScriptProxy m_Instance = null;
        public static ScriptProxy Instance
        {
            get 
            {
                if( m_Instance == null)
                    m_Instance = new ScriptProxy(FormMain.Instance);
                return m_Instance; 
            }
        }

        private dynamic scope = null;
        public dynamic UserFunction
        {
            get { return scope.System; }
        }
        public dynamic UserObject
        {
            get { return scope;  }
        }

        private ScriptEngine m_Engine = null;
        private ScriptRuntime m_Runtime = null;
        private ScriptScope m_Scope = null;

        private PythonLogger _logger = new PythonLogger();
        public PythonLogger Logger
        {
            get { return _logger; }
        }

        private FormMain m_MainForm = null;

        private ScriptProxy(FormMain mainForm)
        {
            m_MainForm = mainForm;
            CreateEngine();
        }
        
        private void CreateEngine()
        {
            if (m_Engine == null)
            {
                m_Engine = Python.CreateEngine();
                scope = m_Scope = m_Engine.CreateScope();
              
                m_Scope.SetVariable("log", _logger);
                //scope.Main = m_MainForm;
                //scope.System = CreateProxy();
                //scope.View3D = PageBackstageHome.Instance.View3D;
                _logger.AddInfo("User Command Object Initialized");

                
            }
        }

        public void AddVariable(string szName, object varObj)
        {
            m_Scope.SetVariable(szName, varObj);
        }

        public object GetVariable(string szName)
        {
            return m_Scope.GetVariable(szName);
        }

        private void CompileSourceAndExecute(String code)
        {
            ScriptSource source = m_Engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();
            // Executes in the scope of Python
            compiled.Execute(m_Scope);
        }


        public void RunPythonScript(string code)
        {
            try
            {
                CompileSourceAndExecute(code);
            }
            catch (Exception ex)
            {
                _logger.AddFault(ex);
            }
        }

        public void Call(string callActionCode)
        {
            try
            {
                var script = @"" + callActionCode;               
                dynamic scope = m_Scope;
                _logger.AddInfo(callActionCode).Tag = callActionCode;
                m_Engine.Execute(script, m_Scope);                
            }
            catch (System.MissingMemberException ex)
            {
                string szMsg = "User Function Object has no attribute '" + callActionCode + "'";
                _logger.AddError(szMsg).Tag = callActionCode;
            }
            catch (System.Exception ex)
            {
                _logger.AddFault(ex).Tag = callActionCode;
            }
        }

        public object CallAction(string callFuncCode)
        {
            try
            {
                var script = @"
proxyFuncResult = " + callFuncCode;

                object proxyFuncResult = new object();
                dynamic scope = m_Scope;
                m_Scope.SetVariable(@"proxyFuncResult", proxyFuncResult);
                m_Engine.Execute(script, m_Scope);
                proxyFuncResult = m_Scope.GetVariable("proxyFuncResult");
                //_logger.AddInfo("# Run proxy function : " + callFuncCode).Tag = callFuncCode;
                //_logger.AddInfo(callFuncCode).Tag = callFuncCode;
                _logger.AddInfo(callFuncCode + " : " + proxyFuncResult).Tag = callFuncCode;
                return proxyFuncResult;
            }
            catch (System.MissingMemberException ex)
            {
                string szMsg = "User Function Object has no attribute '" + callFuncCode + "'";
                _logger.AddError(szMsg);
            }
            catch (System.Exception ex)
            {
               
                _logger.AddFault(ex);
            }
            return null;
        }

        public object InvokeClassFunc(string className, string functionName, object[] args)
        {
            try
            {
                var szclass = className + "()";
                var instance = m_Engine.Execute(szclass, m_Scope);
                var ops = m_Engine.CreateOperations(m_Scope);
                return ops.InvokeMember(instance, functionName, args);   
            }
            catch (System.Exception ex)
            {
                _logger.AddFault(ex);
            }
            return null;         
        }

        private object CreateProxy()
        {
            dynamic proxy = new ExpandoObject();
            return proxy;
        }
    }
}
