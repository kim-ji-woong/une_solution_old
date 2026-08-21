using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;


namespace Pipelib
{
    public class PipeClient : IDisposable
    {
        private int m_nNumThread = 4;

        private string m_szPipeName = "DefaultPipe";

        private PipeClientProvider m_Provider = null;
        public PipeClientProvider Provider
        {
            get { return m_Provider; }
        }

        private Thread m_ServerThread = null;
        private bool m_bExitThread = false;

        public PipeClient()
        {
        }

        public void Dispose()
        {
            ReleaseClient();
        }
        
        private NamedPipeClientStream m_pipeClient = null;
        private NamedPipeClientStream ConnectServer(string szName)
        {         
            m_pipeClient = new NamedPipeClientStream(".", szName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
            
            return m_pipeClient;
        }

        private void ClientThread(object data)
        {
            // 파이프 생성
            NamedPipeClientStream pipeClient = ConnectServer(m_szPipeName);

            
            // 접속이 된경우 Provider생성
            m_Provider = new PipeClientProvider(pipeClient);


            while (!m_bExitThread)
            {
                try
                {
                    if (pipeClient !=  null && pipeClient.IsConnected == false)
                        pipeClient.Connect();

                    // 접속이 끊어지거나 pipe가 없어진경우
                    if (pipeClient == null || pipeClient.IsConnected != true)
                    {
                        // 파이프가 없어진경우 새로 만든다.
                        if (pipeClient == null)
                        {
                            pipeClient = ConnectServer(m_szPipeName);
                            if (m_Provider != null)
                                m_Provider.Dispose();
                            m_Provider = new PipeClientProvider(pipeClient);
                        }
                    }

                    // 다음 명령까지 1초 대기(독점방지)
                    for (int i = 0; i < 10; i++)
                        Thread.Sleep(100);
                }
                catch (IOException e)
                {
                    pipeClient = null;
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
            pipeClient.Close();
        }

        public bool BeginClient(string szPipeName)
        {
            m_szPipeName = szPipeName;

            if (m_ServerThread == null)
            {
                m_ServerThread = new Thread(ClientThread);
                m_ServerThread.Name = "PipeClientThread";
                m_ServerThread.Start();
            }
            return false;
        }

        public bool ReleaseClient()
        {
            m_bExitThread = true;

            m_bExitThread = true;

            if (m_Provider != null)
            {
                m_Provider.Dispose();
            }

            if (m_pipeClient != null)
            {
                try
                {                    
                    m_pipeClient.Close();
                }
                catch (Exception)
                { }
            }


            if (m_ServerThread != null && m_ServerThread.IsAlive == true)
            {
                try
                {
                    m_ServerThread.Abort();
                }
                catch (Exception)
                {

                }
            }

            return false;
        }

        public void SendCommand(string szCmd)
        {
            if (m_Provider != null && IsConnected == true)
            {
                m_Provider.WriteCommand(szCmd);
            }
        }

        public bool IsConnected
        {
            get
            {
                if (m_pipeClient == null)
                    return false;
                return m_pipeClient.IsConnected;
            }
        }
    }
}
