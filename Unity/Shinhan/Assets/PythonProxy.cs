using UnityEngine;
using System;
using System.Collections;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

public class PythonProxy : MonoBehaviour
{
    private static PythonProxy m_Instance = null;
    public static PythonProxy Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private ScriptScope scope = null;   
    public ScriptScope UserObject
    {
        get { return scope; }
    }

    private ScriptEngine m_Engine = null;
    private ScriptScope m_Scope = null;

    public PythonProxy()
    {
        m_Instance = this;
    }

    void Awake ()
    {
        CreateEngine();
    }

    void Start()
    {
    }
	
	void Update ()
    {
	
	}

    private void CreateEngine()
    {
        if (m_Engine == null)
        {
            m_Engine = Python.CreateEngine();
            scope = m_Scope = m_Engine.CreateScope();
        }
    }

    public void AddVariable(string szName, object varObj)
    {
        if(m_Scope != null)
        {
            m_Scope.SetVariable(szName, varObj);
        }
        
    }

    public object GetVariable(string szName)
    {
        if (m_Scope == null)
            return null;

        return m_Scope.GetVariable(szName);
    }

    private void CompileSourceAndExecute(String code)
    {
        if(m_Engine != null)
        {
            ScriptSource source = m_Engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();
            compiled.Execute(m_Scope);
        }       
    }

    public void RunPythonScript(string code)
    {
        try
        {
            CompileSourceAndExecute(code);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
        }
    }

    public void Call(string callActionCode)
    {
        if (m_Engine == null || m_Scope == null)
            return;

        try
        {
            var script = @"" + callActionCode;
            dynamic scope = m_Scope;
            UnityEngine.Debug.Log(callActionCode);
            m_Engine.Execute(script, m_Scope);
        }
        catch (System.MissingMemberException)
        {
            string szMsg = "User Function Object has no attribute '" + callActionCode + "'";
            UnityEngine.Debug.LogError(szMsg);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
        }
    }

    public object CallAction(string callFuncCode)
    {
        if (m_Engine == null || m_Scope == null)
            return null;

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
            UnityEngine.Debug.Log(callFuncCode + " : " + proxyFuncResult);
            return proxyFuncResult;
        }
        catch (System.MissingMemberException)
        {
            string szMsg = "User Function Object has no attribute '" + callFuncCode + "'";
            UnityEngine.Debug.LogError(szMsg);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
        }
        return null;
    }

    private void OnApplicationQuit()
    {
        m_Engine = null;
        UnityEngine.Debug.LogError("Exit");
    }

}
