using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelib
{
    //public delegate void DelegateMessage(string Reply);

    public class PassivePipeServer : IDisposable
    {
        public event DelegateMessage OnReciveMessage;

        private string m_szPipeName = "";
        private string m_szReciverPipeName = "Reciver";
        private string m_szSenderPipeName = "Sender";

        private PipeAsyncServer m_Sender = null;
        private PipeAsyncServer m_Reciver = null;

        private bool m_bServerMode = true;

        public bool ServerMode
        {
            get { return m_bServerMode; }
        }

        public PassivePipeServer(bool bServer, string szPipeName)
        {
            m_bServerMode = bServer;

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
            
            m_Sender = new PipeAsyncServer(this, System.IO.Pipes.PipeDirection.Out);
            m_Reciver = new PipeAsyncServer(this, System.IO.Pipes.PipeDirection.In);
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
                //Invoke((Action)(() =>
                //{

                //    var message = String.Format("Original Message:=\n\n{0}\nServer details:=\n\nSending to pipe:={1}\nListening on pipe:={2}",
                //            szMessage,
                //            txtSendPipeName.Text,
                //            txtListenPipeName.Text);

                //    MessageBox.Show(this, message, "Message Receieved");
                //}));
            }
        }

        public void Send(string szMsg)
        {
            m_Sender.Send(szMsg);
        }

        public void BeginPipe()
        {
            m_Reciver.Listen(m_szReciverPipeName);
            m_Sender.Listen(m_szSenderPipeName);
        }

        public void StopPipe()
        {
            // m_Sender.ReleaseClient();
            // m_Reciver.ReleaseServer();
        }
    }
}
