using AgentFactory.BLL;

namespace SafetyServer.BLL
{
    public class MainManager
    {
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;

        private SpatialManager m_spatialManager = null;
        private Process.MemberManager m_memberManager = null;
        private SensorManager m_sensorManager = null;
        private IAlarmManager m_alarmManager = null;
        private BaseBroadcastManager m_broadcastManager = null;
        private BaseProcessManager m_processManager = null;
        private BaseSMSManager m_smsManager = null;

        public SDMS.IDAL.IDataManager SDMSDataManager
        {
            get { return m_sdmsDataManager; }
            set { m_sdmsDataManager = value; }
        }

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
            set { m_commonDataManager = value; }
        }

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_teamDataManager; }
            set { m_teamDataManager = value; }
        }

        public Process.MemberManager MemberManager
        {
            get { return m_memberManager; }
        }

        public IAlarmManager AlarmManager
        {
            get { return m_alarmManager; }
        }

        public BaseProcessManager ProcessManager
        {
            get { return m_processManager; }
        }

        public SensorManager SensorManager
        {
            get { return m_sensorManager; }
        }

        public MainManager(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_sdmsDataManager = sdmsDataManager;
            m_commonDataManager = commonDataManager;
            m_teamDataManager = teamDataManager;

            m_spatialManager = new SpatialManager(m_sdmsDataManager, m_teamDataManager, m_commonDataManager);

            m_memberManager = new Process.MemberManager(this);
            m_memberManager.Initialize();

            Factory factory = BaseFactory.GetFactory();

            m_smsManager = new Process.SMSManager(factory, this);
            m_broadcastManager = new Process.BroadcastManager(factory, this);
            m_sensorManager = new SensorManager(this, factory);
            m_sensorManager.Initialize();

            m_alarmManager = new Process.AlarmManager(this, m_sensorManager);
            m_processManager = new Process.ProcessManager(factory, this);

            m_sensorManager.OnLoad();
        }

        public SpatialManager GetSpatialManager()
        {
            return m_spatialManager;
        }
    }
}
