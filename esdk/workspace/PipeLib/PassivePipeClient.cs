using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelib
{
    //public delegate void DelegateMessage(string Reply);

    public class PassivePipeClient : IDisposable
    {
        public event DelegateMessage OnReciveMessage;

        private string m_szPipeName = "";
        private string m_szReciverPipeName = "Reciver";
        private string m_szSenderPipeName = "Sender";

        private PipeAsyncClientSender m_Sender = null;
        private PipeAsyncClientReciver m_Reciver = null;

        private bool m_bServerMode = false;

        public bool ServerMode
        {
            get { return m_bServerMode; }
        }

        public PassivePipeClient(string szPipeName)
        {
            m_szPipeName = szPipeName;
            m_szReciverPipeName = m_szPipeName + "Reciver";
            m_szSenderPipeName = m_szPipeName + "Sender";

            // 서버 모드인경우 이름을 바꾸어 준다.
            if (m_bServerMode == true)
            {
                string szTemp = m_szReciverPipeName;
                m_szReciverPipeName = m_szSenderPipeName;
                m_szSenderPipeName = szTemp;
            }

            m_Sender = new PipeAsyncClientSender();
            m_Reciver = new PipeAsyncClientReciver(this);
        }

        public void Dispose()
        {
            if (m_Reciver != null)
            {
                m_Reciver.Dispose();
            }
            if (m_Sender != null)
            {
                m_Sender.Dispose();
            }
        }

        internal void Recive(string szMsg)
        {
            if (OnReciveMessage != null)
            {
                OnReciveMessage.Invoke(szMsg);
            }
        }

        public void Send(string szMsg)
        {
            m_Sender.Send(szMsg, m_szSenderPipeName);
        }

        public void BeginPipe()
        {
            m_Reciver.Listen(m_szReciverPipeName);
        }

        public void StopPipe()
        {
            // m_Sender.ReleaseClient();
            // m_Reciver.ReleaseServer();
        }
    }
}
