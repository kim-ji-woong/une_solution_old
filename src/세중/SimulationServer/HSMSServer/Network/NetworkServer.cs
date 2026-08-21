using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;

namespace HSMSServer
{
    public class NetworkServer
    {
		private bool m_bCloseServer = false;
		public bool ClosingServer
		{
			get { return m_bCloseServer; }
		}
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;

        private static NetworkServer m_instance = null;

        private bool m_finishProcess = false;

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        public static NetworkServer Instance
        {
            get { return m_instance; }
        }

        public HSMSServer.ServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

		public NetworkServer()
        {
            m_instance = this;

            m_provider = new ServiceProvider();

            string strPort = HSMS.DBConn.GetInValue("ip_addr", "SensorServer");

			if (strPort.Length > 0)
			{
				int.TryParse(strPort, out m_nPort);
			}
			else
			{
				m_nPort = 5000;
			}

            m_server = new TcpServer(m_provider, m_nPort);
            m_isOpened = m_server.Start();
        }

		public void Start()
        {
            m_server = new TcpServer(m_provider, m_nPort);
            m_isOpened = m_server.Start();
        }

        public void Stop()
        {
            m_bCloseServer = true;
            m_finishProcess = true;

            if (m_server != null && m_isOpened)
            {
                m_isOpened = false;
                m_server.Stop();
            }
        }

        public void SendLocInfo(LocInfo loc)
        {
            string strSend = "#DEVINFO:IT," + loc.DeviceID + ",0," + loc.X.ToString();
            strSend += "," + loc.Y.ToString() + "," + loc.Latitude.ToString();
            strSend += "," + loc.Longitude.ToString() + "," + loc.MethanGas.ToString();
            strSend += "," + loc.CoGas.ToString();

            m_provider.Send(strSend, null, true);
        }
    }
}
