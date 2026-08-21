using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;

namespace IntegratedManagement4
{
    public class NetworkServer
    {
        public enum Command
        {
            NONE = 0,
            RUN_SOP_SIMULATOR0,                                 // SOPSimulator.exe의 것 실행
            RUN_SOP_SIMULATOR1,                                 // SOPSimulator1.exe의 것 실행
            CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0,          // SOPSimulator1에서 SOP MonitoringSystem이 실행중인지 확인한 다음
                                                                // 실행중이지 않으면 SOPSimulator.exe의 것 실행
            RESERVE_RUN_SOP_SIMULATOR0,                         // SOPSimulator.exe가 접속하면 SOP MonitoringSystem을 실행하도록 예약
            RESERVE_RUN_SOP_SIMULATOR1,                         // SOPSimulator1.exe가 접속하면 SOP MonitoringSystem을 실행하도록 예약
            RESERVE_CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0   // SOPSimulator1.exe가 접속하면 SOPSimulator1에서 SOP MonitoringSystem이 실행중인지 확인한 다음
                                                                // 실행중이지 않으면 SOPSimulator.exe의 것 실행
        }

		private bool m_bCloseServer = false;
		public bool ClosingServer
		{
			get { return m_bCloseServer; }
		}

        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;
        private int m_nPort = 0;
        private bool m_isOpened = false;

        public ServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

        // Team이나 직원정보, 담당자 정보를 바꾸거나 조회하는 중인가?
        private object m_memberCriticalSection = new object();
        public object MemberCriticalSection
        {
            get { return m_memberCriticalSection; }
        }

        private Command m_lastCmd = Command.NONE;
        public Command LastCommand
        {
            get { return m_lastCmd; }
            set { m_lastCmd = value; }
        }

		public NetworkServer(int nPort)
        {
            m_nPort = nPort;

            m_provider = new ServiceProvider(this);
        }

		public void NetworkServerLoad()
        {
            if (m_nPort > 0)
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_isOpened = m_server.Start();
            }
        }

		public void NetworkServerClosing()
		{
			m_bCloseServer = true;

			m_provider.ReleaseThread();

			if (m_server != null && m_isOpened)
			{
				m_isOpened = false;

                try
                {
                    m_server.Stop();
                }
                catch (Exception)
                {
                }
			}
		}
    }
}

/*namespace SDMSServer
{
    // 사용하지 않음
    public class ConnectionLogEx
    {
        private static ConnectionLogEx m_instance = null;
        public static ConnectionLogEx Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ConnectionLogEx();
                return m_instance;
            }
        }

        private ConnectionLogEx()
        {
        }

        public void WriteLine(string strLog)
        {
        }
    }
}*/