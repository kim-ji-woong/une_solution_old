using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;


namespace Pipelib
{

    public class PipeServer : IDisposable
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

        public event OnDataRecive OnDataRecive;

        public PipeServer()
        {
        }

        public void Dispose()
        {
            ReleaseServer();
        }


        private NamedPipeServerStream m_PipeServer = null;
        private NamedPipeServerStream CreateServer(string szName)
        {
            m_PipeServer = new NamedPipeServerStream(szName, PipeDirection.InOut, m_nNumThread);
            return m_PipeServer;
        }

        private void ServerThread(object data)
        {            
            // 파이프 생성
            NamedPipeServerStream pipeServer = CreateServer(m_szPipeName);           
            // 접속 대기 - 블로킹모드임
            pipeServer.WaitForConnection();

            // 접속이 된경우 Provider생성
            m_Provider = new PipeClientProvider(pipeServer);
            if (OnDataRecive != null)
            {
                m_Provider.OnDataRecive += OnDataRecive;
            }
            
            while (!m_bExitThread)
            {  
                try
                {
                    // 접속이 끊어지거나 pipe가 없어진경우
                    if (pipeServer == null || pipeServer.IsConnected != true)
                    {
                        // 파이프가 없어진경우 새로 만든다.
                        if (pipeServer == null)
                        {
                            pipeServer = CreateServer(m_szPipeName);
                            if (m_Provider != null)
                                m_Provider.Dispose();
                            m_Provider = new PipeClientProvider(pipeServer);
                            if (OnDataRecive != null)
                            {
                                m_Provider.OnDataRecive += OnDataRecive;
                            }
                        }
                        // 접속을 대기한다.- 블로킹모드
                        pipeServer.WaitForConnection();
                    }
                    else
                    {
                        if (pipeServer.IsConnected == true)
                        {
                            // 다음 입력을 기다린다.- 블로킹모드                        
                            m_Provider.ReadyCommand();
                        }
                    }
                    
                    // 다음 명령까지 100ms 대기(독점방지)
                    Thread.Sleep(100);                                   
                }
                catch (IOException e)
                {
                    pipeServer = null;
                    Console.WriteLine("ERROR: {0}", e.Message);
                }                
            }
            pipeServer.Close();
        }

        public bool BeginServer(string szPipeName)        
        {
            m_szPipeName = szPipeName;

            if( m_ServerThread == null)
            {
                m_ServerThread = new Thread(ServerThread);
                m_ServerThread.Name = "PipeServerThread";
                m_ServerThread.Start();
            }
            return false;
        }

        public bool ReleaseServer()
        {
            m_bExitThread = true;

            if (m_Provider != null)
            {
                m_Provider.Dispose();
            }

            if (m_PipeServer != null)
            {
                try
                {
                    {
                        using (NamedPipeClientStream npcs = new NamedPipeClientStream(m_szPipeName))
                        {
                            npcs.Connect(100);
                        }
                    }
                    m_PipeServer.Close();
                }
                catch(Exception)
                { }                
            }

            if(m_ServerThread != null && m_ServerThread.IsAlive == true)
            {
                try
                {
                    m_ServerThread.Abort();
                }
                catch(Exception)
                {

                }
                
            }

            return false;
        }
    }
}