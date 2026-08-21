namespace SafetyServer.BLL.Data.Request
{
    public class ReportAreaAlarm
    {
        private string m_strUserID = null;
        private string m_strCameraID = null;
        private string m_strTime = null;
        private int m_nAlarmLevel = (int)AlarmLevel.Safety;
        private string m_strNotifications = null;

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
    }
}
