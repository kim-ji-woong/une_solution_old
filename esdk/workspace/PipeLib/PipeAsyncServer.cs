using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;

namespace Pipelib
{  
    public class PipeAsyncServer
    {

        private bool m_bUseStreamReader = true;
        public bool UseStreamReader
        {
            get { return m_bUseStreamReader; }
            set { m_bUseStreamReader = value; }
        }

        private NamedPipeServerStream pipeServer = null;
     
        private string _pipeName;
        
        private PipeControl m_Contorl = null;

        private PassivePipeServer m_PassiveServer = null;

        public PipeAsyncServer(PipeControl contorl)
        {
            m_Contorl = contorl;
        }
        
        private bool m_bPassiveMode = false;
        private PipeDirection m_dir = PipeDirection.In;
        public PipeAsyncServer(PassivePipeServer contorl, PipeDirection dir)
        {
            m_dir = dir;
            m_PassiveServer = contorl;
            m_bPassiveMode = true;
        }

        public void Dispose()
        {
            if(pipeServer != null)
            {     
                if(pipeServer.IsConnected == true)
                {                    
                    pipeServer.Disconnect();
                    pipeServer.Close();
                    pipeServer.Dispose();
                }
            }
        }

        public void Listen(string PipeName)
        {
            try
            {
                _pipeName = PipeName;
                pipeServer = new NamedPipeServerStream(PipeName, m_dir, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);                                
            }
            catch (Exception)
            {
            }
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                NamedPipeServerStream pipeStream = (NamedPipeServerStream)iar.AsyncState;
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
            }
            catch (Exception ex)
            {
            }
        }

        private string szCmd = "";
        

        public void Send(string szMsg)
        {
            if( pipeServer != null && (m_dir == PipeDirection.Out || m_dir == PipeDirection.InOut))
            {     
                try
                {
                    if (pipeServer.IsConnected == true)
                    {
                        byte[] _buffer = Encoding.UTF8.GetBytes(szMsg + "\n\r");
                        pipeServer.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeServer);
                    }
                    szCmd = szMsg;
                }
                catch(Exception)
                {
                    try
                    {
                        pipeServer.Close();
                    }
                    catch(Exception)
                    { }
                    
                    pipeServer = null;
                    pipeServer = new NamedPipeServerStream(_pipeName, m_dir, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);  
                }
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;    
                if( m_dir == PipeDirection.In || m_dir == PipeDirection.InOut)
                {
                    // WaitConnection종료
                    pipeServer.EndWaitForConnection(iar);

                    string stringData = null;
                    if(m_bUseStreamReader == false)
                    {
                        byte[] buffer = new byte[512];
                        pipeServer.Read(buffer, 0, 512);
                        stringData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        stringData = stringData.Trim('\0');      
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(pipeServer))
                        {
                            stringData = reader.ReadLine();                           
                        }      
                    }

                    if (m_bPassiveMode == false)
                    {
                        if (m_Contorl != null && stringData != null)
                            m_Contorl.Recive(stringData);
                    }
                    else
                    {
                        if (m_PassiveServer != null && stringData != null)
                        {
                            m_PassiveServer.Recive(stringData);
                        }
                    }                              

                    pipeServer.Close();
                    pipeServer = null;
                    pipeServer = new NamedPipeServerStream(_pipeName, m_dir, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);                    
                }     
                else
                {
                    pipeServer.EndWaitForConnection(iar);
                }                
            }
            catch
            {
                return;
            }
        }
    }
}
