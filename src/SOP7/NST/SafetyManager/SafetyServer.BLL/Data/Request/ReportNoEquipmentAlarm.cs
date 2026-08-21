namespace SafetyServer.BLL.Data.Request
{
    public class ReportNoEquipmentAlarm
    {
        private string m_strUserID = null;
        private string m_strCameraID = null;
        private string m_strTime = null;
        private int m_nAlarmLevel = (int)AlarmLevel.Safety;
        private string m_strNotifications = null;
        private bool m_helmet = false;
        private bool m_shoes = false;
        private bool m_belt = false;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string CameraID
        {
            get { return m_strCameraID; }
            set { m_strCameraID = value; }
        }

        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }

        public int Level
        {
            get { return m_nAlarmLevel; }
            set { m_nAlarmLevel = value; }
        }

        public string Notifications
        {
            get { return m_strNotifications; }
            set { m_strNotifications = value; }
        }

        public bool Helmet
        {
            get { return m_helmet; }
            set { m_helmet = value; }
        }

        public bool Shoes
        {
            get { return m_shoes; }
            set { m_shoes = value; }
        }

        public bool Belt
        {
            get { return m_belt; }
            set { m_belt = value; }
        }
    }
}
