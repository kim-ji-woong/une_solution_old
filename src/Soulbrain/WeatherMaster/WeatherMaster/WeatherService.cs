using System;
using System.Timers;
using System.ServiceProcess;

namespace WeatherMaster
{
    partial class WeatherService : ServiceBase
    {
        private CityReader m_cityReader = new CityReader();
        private SpecialReportReader m_reportReader = new SpecialReportReader();
        private Timer m_timer = null;

        public WeatherService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            // 1분에 한번씩 동작
            m_timer = new Timer(60000);
            m_timer.Elapsed += OnTimer;
            m_timer.Start();

            // 시작과 동시에 한번 실행시킨다.
            OnTimer(null, null);
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_timer.Stop();
        }

        private DateTime m_dtLast = new DateTime();
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            m_cityReader.ReadData();
            m_reportReader.ReadData();

            DateTime dtNow = DateTime.Now;
            if ((dtNow - m_dtLast).TotalDays >= 1)
            {
                Logger.Instance.RemoveOldLogs();
                m_dtLast = DateTime.Now;
            }
        }
    }
}
