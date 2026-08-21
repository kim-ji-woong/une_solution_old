using System.Timers;
using System.ServiceProcess;

namespace AlarmWatcher
{
    partial class ServiceMain : ServiceBase
    {
        private SensorManager m_sensorManager = new SensorManager();
        private Timer m_timer = null;
        private bool m_processing = false;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            // 3초에 한번씩 동작
            m_timer = new Timer(3000);
            m_timer.Elapsed += OnTimer;
            m_timer.Start();

            OnTimer(null, null);
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_timer.Stop();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (m_processing)
                return;

            m_processing = true;
            m_sensorManager.ReadSensorData();
            m_processing = false;
        }
    }
}
