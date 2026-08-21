using System.ServiceProcess;

namespace FireSensorServer
{
    using Network;

    partial class FireSensorService : ServiceBase
    {
        private NetworkManager m_netManager = null;

        public FireSensorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_netManager = new NetworkManager(null);
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_netManager.Close();
        }
    }
}
