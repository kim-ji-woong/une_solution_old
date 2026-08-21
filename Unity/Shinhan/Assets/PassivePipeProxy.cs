using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Text;

public class PassivePipeProxy : MonoBehaviour
{
    private static PassivePipeProxy m_Instance = null;
    public static PassivePipeProxy Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private string m_szPipeName = "UnityPipeOutside";
    private Pipelib.PassivePipeClient m_PipeClient = null;

    public void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && proxy.UserObject != null)
        {
            //proxy.UserObject.SetVariable("SendMessage", new Action<string>(SendServer));
            proxy.UserObject.SetVariable("ClosePipe", new Action(ClosePipe));
            proxy.UserObject.SetVariable("OpenPipe", new Action(OpenPipe));
        }
    }

    private void Awake()
    {
        m_Instance = this;

        AddPythonFunction();

        OpenPipe();
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        ClosePipe();
    }

    public void SendServer(string szMsg)
    {
        try
        {            
            if (m_PipeClient != null)
            {
                m_PipeClient.Send(szMsg);
                //UnityEngine.Debug.Log("SendComplete : " + szMsg);
            }
        }
        catch (Exception exx)
        {
            UnityEngine.Debug.LogError(exx.Message);
        }
    } 

    private void OpenPipe()
    {
        UnityEngine.Debug.Log("Create PipeClient");
        m_PipeClient = new Pipelib.PassivePipeClient(m_szPipeName);
        m_PipeClient.OnReciveMessage += OnMessage;
        m_PipeClient.BeginPipe();
        UnityEngine.Debug.Log("Done. PipeClient Created");
    }
    

    
    private void  Update()
    {
        ArrayList arList = (ArrayList)m_arListMessage.Clone();
        m_arListMessage = new ArrayList();

        foreach (string szCmd in arList)
        {
            System.Diagnostics.Trace.WriteLine("Call Function : " + szCmd);
            Action<string> delayAction = new Action<string>(PythonProxy.Instance.Call);
            delayAction.Invoke(szCmd);
        }        
    }

    private ArrayList m_arListMessage = new ArrayList();

    private void OnMessage(string szMessage)
    {
        if(szMessage != null && szMessage != "")
        {
            if(szMessage.StartsWith("CMD:"))
            {
                string szCMD = szMessage.Replace("CMD:", "");

                //Action<string> delayAction = new Action<string>(PythonProxy.Instance.Call);
                //delayAction.Invoke(szCMD);
                m_arListMessage.Add(szCMD);
            }

            UnityEngine.Debug.Log("Received Message : " + szMessage);
            //System.Diagnostics.Trace.WriteLine("Received Message : " + szMessage);
        }
        
    }

    private void ClosePipe()
    {
        if (m_PipeClient != null)
        {
            m_PipeClient.StopPipe();
            m_PipeClient.Dispose();
            m_PipeClient = null;
        }
    }


    private void OnApplicationQuit()
    {
        ClosePipe();
    }
}
