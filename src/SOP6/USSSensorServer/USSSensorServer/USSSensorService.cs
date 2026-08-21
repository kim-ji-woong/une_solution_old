using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace USSFireSensorServer
{
    using Network;

    partial class USSSensorService : ServiceBase, IServiceOwner
    {
        private NetworkManager m_netMgr = null;

        public USSSensorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_netMgr = new NetworkManager(this, null);
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_netMgr.Close();
        }

        public void OnConnect(string strIP, bool ussServer)
        {
            if (ussServer)
            {
                if (strIP.Length > 0)
                    WriteLog(string.Format("USS Server({0})와의 접속이 성공하였습니다.", strIP));
                else
                    WriteLog(string.Format("USS Server와의 접속이 성공하였습니다."));
            }
            else
            {
                WriteLog(string.Format("SOP Server와의 접속이 성공하였습니다."));
            }
        }

        public void OnDropConnection(string strIP, bool ussServer)
        {
            if (ussServer)
            {
                if (strIP.Length > 0)
                    WriteLog(string.Format("USS Server({0})와의 접속이 끊어졌습니다.", strIP));
                else
                    WriteLog(string.Format("USS Server와의 접속이 끊어졌습니다."));
            }
            else
            {
                WriteLog(string.Format("SOP Server와의 접속이 끊어졌습니다."));
            }
        }

        private void WriteLog(string strLog)
        {
            if (m_netMgr != null)
                m_netMgr.WriteLog(strLog);
        }
    }
}
