namespace SafetyServer.BLL.Data.Request
{
    // 수동신고
    public class RequestReportManualAlarm
    {
        // 신고자의 ID
        private string m_strReporterID = null;
        private AccidentType m_accidentType = null;
        // ZoneID가 null이 아니면 특정 Zone에 재난 발생
        private int? m_nZoneID = null;
        // BuildingID가 null이 아니면 특정 Building에 재난 발생
        private int? m_nBuildingID = null;
        // 신고메시지
        private string m_strNotifications = null;

        public string ReporterID
        {
            get { return m_strReporterID; }
            set { m_strReporterID = value; }
        }

        public AccidentType accident_type
        {
            get { return m_accidentType; }
            set { m_accidentType = value; }
        }

        public int? FieldID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        // 신고메시지
        public string Notifications
        {
            get { return m_strNotifications; }
            set { m_strNotifications = value; }
        }
    }

    public class AccidentType
    {
        private bool m_isFire = true;

        public bool Fire
        {
            get { return m_isFire; }
            set { m_isFire = value; }
        }

        public override string ToString()
        {
            if (m_isFire)
                return "Fire";

            return "";
        }
    }
}
