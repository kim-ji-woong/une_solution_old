using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HitPage
{
    partial class HitService : ServiceBase
    {
        private System.Timers.Timer m_timer = null;
        private ConnectionManager m_mgr = null;

        public HitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_mgr = new ConnectionManager();

            int nInterval;
            string strInterval = System.Configuration.ConfigurationManager.AppSettings.Get("interval");

            if (int.TryParse(strInterval, out nInterval))
            {
                m_timer = new System.Timers.Timer();
                m_timer.Interval = nInterval;
                m_timer.Elapsed += OnTimer;
                m_timer.Start();

                OnTimer(null, null);
            }
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timer.Stop();
            m_mgr.CheckConnection();
            m_timer.Start();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            if (m_timer != null)
                m_timer.Stop();
        }
    }
}
