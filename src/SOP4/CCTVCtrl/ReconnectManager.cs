using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnE.Control
{
    // 예외적인 상황이 발생하여 화면이 정지되거나 접속이 끊어졌을때 재접속을 하도록 도와주는 Class
    public class ReconnectManager
    {
        private DateTime m_dtClose = new DateTime();
        // Close() 요청이 발생한 후 실제로 처리되기까지의 유예 시간(초)
        private double m_closeTime = 1.5;
        private Thread m_threadConnect = null;
        private IReconnectManagerOwner m_owner = null;
        
        public ReconnectManager(IReconnectManagerOwner owner)
        {
            m_owner = owner;
        }

        public void Close()
        {
            m_dtClose = DateTime.Now;
        }

        public void ReleaseThread()
        {
            m_threadConnect = null;
        }

        public void OnStop()
        {
            TimeSpan span = DateTime.Now - m_dtClose;

            // 정상적인 CCTV 종료
            if (span.TotalSeconds <= m_closeTime)
                return;
            else if (m_threadConnect == null)
            {
                m_threadConnect = new Thread(new ThreadStart(ReconnectThread));
                m_threadConnect.Start();
            }
        }

        public void OnFail()
        {
            TimeSpan span = DateTime.Now - m_dtClose;

            // 정상적인 CCTV 종료
            if (span.TotalSeconds <= m_closeTime)
                return;
            else if (m_threadConnect == null)
            {
                m_threadConnect = new Thread(new ThreadStart(ReconnectThread));
                m_threadConnect.Start();
            }
        }

        private void ReconnectThread()
        {
            if (m_owner != null)
            {
                while (m_owner.IsConnected == false && m_threadConnect != null)
                {
                    m_owner.Connect();
                    Thread.Sleep(3000);
                }
            }

            m_threadConnect = null;
        }
    }

    public interface IReconnectManagerOwner
    {
        bool IsConnected
        {
            get;
        }

        void Connect();
    }
}
