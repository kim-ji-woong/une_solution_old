using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SOPMonitoringSystem;
using System.Windows.Forms;
using SDMS;
using UnE.SOP;
using IntegratedManagement3;

namespace SOPMonitoringSystem
{
    public class ClientProviderInternal : ClientServiceProvider
    {
        private NetworkManager m_mgr = null;

        public bool IsReadingProcess
        {
            get { return false; }
        }

        public int PingCount
        {
            get { return 0; }
            set { }
        }

        public ClientProviderInternal(NetworkManager mgr)
        {
            m_mgr = mgr;
        }
        
        public override void OnReceiveData()
        {
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
        }

        public void SendData(short header, ArrayList arrDatas)
        {
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        public new void Close()
        {
            base.Close();
        }
    }
}
