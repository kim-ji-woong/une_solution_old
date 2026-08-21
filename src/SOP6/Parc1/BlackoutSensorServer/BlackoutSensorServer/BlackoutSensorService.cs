using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BlackoutSensorServer.Network;
using BlackoutSensorServer.RabbitMQ;

namespace BlackoutSensorServer
{
    partial class BlackoutSensorService : ServiceBase
    {
        private NetworkWebManager m_netMgr = null;
        private RabbitMQService m_rabbitmqService = null;
        
        public BlackoutSensorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_netMgr = new NetworkWebManager();
            
            string strRabbitmqIP = System.Configuration.ConfigurationManager.AppSettings["rabbitmqServerIP"].ToString().Trim();            
            m_rabbitmqService = new RabbitMQService(strRabbitmqIP, m_netMgr);            
        }
        
        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.

            m_netMgr.Close();
            m_rabbitmqService.Close();
        }
    }
}
