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
        private string m_strURL = "";
        private DBBackup dbBackup = null;

        public HitService()
        {
            InitializeComponent();
            dbBackup = new DBBackup();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_strURL = System.Configuration.ConfigurationManager.AppSettings.Get("url");

            if (m_strURL != null && m_strURL.Length > 0)
            {
                m_timer = new System.Timers.Timer();
                // 10분 주기
                m_timer.Interval = 1000 * 60 * 10;
                m_timer.Elapsed += OnTimer;
                m_timer.Start();

                OnTimer(null, null);
            }
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadData(m_strURL);
            }
            catch (Exception ex)
            {
                DBBackup.WriteLog("[ERROR] OnTimer : " + ex.Message);
            }

            dbBackup.Run();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            if (m_timer != null)
                m_timer.Stop();

            DBBackup.WriteLog("HitPage End");

            if (DBBackup.swLog != null)
                DBBackup.swLog.Close();
        }
    }
}
