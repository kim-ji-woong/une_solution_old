using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SecomEventReceiver
{
    partial class SecomService : ServiceBase
    {
        private DataManager m_dataMgr = null;
        private NetworkWebManager m_netMgr = null;
        private S1NetworkServer m_sensorServer = null;

        public SecomService()
        {
            InitializeComponent();

            m_dataMgr = new DataManager();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_netMgr = new NetworkWebManager();
            m_sensorServer = new S1NetworkServer(DataManager.Instance.DBManager);
            m_sensorServer.NetworkServerLoad();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_netMgr.ReleaseThread();
            m_sensorServer.NetworkServerClosing();
        }
    }
}
