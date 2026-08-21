using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace Pipelib
{
    internal class PipeAsyncClientReciver
    {
        private PassivePipeClient m_Server = null;

        private NamedPipeClientStream m_ReciverStream = null;
        private Thread m_ReciveThread = null;
        private bool m_bReadServerMsg = true;
        private string m_szPipeName = "";

        public void Dispose()
        {
            ClosePipe();
        }

        public PipeAsyncClientReciver(PassivePipeClient contorl)
        {
            m_Server = contorl;
                
            //Task.Run(() => StartListeningAsync("TestPipeReciver", OnMessage ));
        }

        internal void Listen(string szPipeName)
        {
            try
            {
                m_szPipeName = szPipeName;
                m_ReciveThread = new Thread(CheckPipeData);

                m_ReciveThread.Name = "PipeReadThread";
                m_ReciveThread.Start(szPipeName);
            }
            catch (Exception)
            {

            }  
        }
        
        private void CheckPipeData(object param)
        {
            string szPipeName = (string)param;
            StartListeningAsync(szPipeName, OnMessage);
        }

        internal void StartListeningAsync(string pipeName, Action<string> messageRecieved)
        {
            try
            {
                while (m_bReadServerMsg)
                {
                    Thread.Sleep(500);

                  
                    if (m_ReciverStream == null || m_ReciverStream.IsConnected == false)
                    {
                        try
                        {
                            m_ReciverStream = new NamedPipeClientStream(".", pipeName, PipeDirection.In, PipeOptions.None);
                            m_ReciverStream.Connect();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                        }
                    }

                    if (m_ReciverStream != null || m_ReciverStream.IsConnected == true)
                    {
                        try
                        {
                            using (var streamReader = new StreamReader(m_ReciverStream))
                            {
                                while (m_bReadServerMsg)
                                {
                                    if (m_ReciverStream != null && m_ReciverStream.IsConnected == true)
                                    {
                                        if (streamReader != null)
                                        {
                                            var message = streamReader.ReadLine();
                                            if (messageRecieved != null)
                                            {
                                                messageRecieved(message);
                                            }
                                        }
                                    }
                                    Thread.Sleep(50);
                                }
                                streamReader.Close();
                            }
                        }
                        catch(Exception)
                        {

                        }
                        
                    }
                }                
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.WriteLine(exception.Message);
            }
        }

        internal void OnMessage(string szMessage)
        {
            if (m_Server != null)
            {                        
                m_Server.Recive(szMessage);
            }
            
        }

        internal void ClosePipe()
        {
            m_bReadServerMsg = false;

            if (m_ReciverStream != null)
            {
                m_ReciverStream.Close();
                m_ReciverStream.Dispose();
                m_ReciverStream = null;
            }
        }
    }
}
