using dnsDBUtil;

namespace WifiSensorService.Data
{
    public class Option
    {
        private WebDBManager m_dbMgr = null;
        // 데이터 보존기한(개월)
        private int m_nLifeTime = 1;
        // 센서로부터 얼마의 시간 이상 데이터를 못받으면 센서가 꺼진것으로 간주할 것인가?(분)
        private int m_nRebootMinutes = 30;
        // 센서가 켜진 이후에 초기 몇 분 동안은 데이터를 무시한다.
        private int m_nWarmingupMinutes = 10;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        // 데이터 보존기한(개월)
        public int LifeTime
        {
            get { return m_nLifeTime; }
            set { m_nLifeTime = value; }
        }

        public int RebootMinutes
        {
            get { return m_nRebootMinutes; }
            set { m_nRebootMinutes = value; }
        }

        public int WarmingupMinutes
        {
            get { return m_nWarmingupMinutes; }
            set { m_nWarmingupMinutes = value; }
        }

        public Option(WebDBManager dbMgr, int nLifeTime, int rebootMinutes, int warmingupMinutes)
        {
            m_dbMgr = dbMgr;
            m_nLifeTime = nLifeTime;
            m_nRebootMinutes = rebootMinutes;
            m_nWarmingupMinutes = warmingupMinutes;
        }
    }
}
