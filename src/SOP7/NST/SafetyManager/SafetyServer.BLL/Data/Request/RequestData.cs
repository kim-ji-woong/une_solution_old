namespace SafetyServer.BLL.Data.Request
{
    // 알람해제, 관심, 주의, 경계, 심각
    public enum AlarmLevel { Safety = 0, Attention, Caution, Alert, Emergency}

    public class RequestData
    {
        private bool? m_requestSpatialInfo = null;
        private RequestReportManualAlarm m_requestReportManualAlarm = null;
        private LoginEvent m_loginEvent = null;
        private RequestUserPosition m_requestUserPosition = null;
        private UpdateUserPosition m_updateUserPosition = null;
        private ReportAreaAlarm m_reportAreaAlarm = null;
        private ReportNoEquipmentAlarm m_reportNoEquipmentAlarm = null;
        private RequestFieldUserPosition m_requestFieldUserPosition = null;
        private bool? m_requestMobileUserList = null;

        public bool? RequestSpatialInfo
        {
            get { return m_requestSpatialInfo; }
            set { m_requestSpatialInfo = value; }
        }

        public RequestReportManualAlarm ReportManualAlarm
        {
            get { return m_requestReportManualAlarm; }
            set { m_requestReportManualAlarm = value; }
        }

        public LoginEvent LoginEvent
        {
            get { return m_loginEvent; }
            set { m_loginEvent = value; }
        }

        public RequestUserPosition RequestUserPosition
        {
            get { return m_requestUserPosition; }
            set { m_requestUserPosition = value; }
        }

        public UpdateUserPosition UpdateUserPosition
        {
            get { return m_updateUserPosition; }
            set { m_updateUserPosition = value; }
        }

        public ReportAreaAlarm ReportAreaAlarm
        {
            get { return m_reportAreaAlarm; }
            set { m_reportAreaAlarm = value; }
        }

        public ReportNoEquipmentAlarm ReportNoEquipmentAlarm
        {
            get { return m_reportNoEquipmentAlarm; }
            set { m_reportNoEquipmentAlarm = value; }
        }

        public RequestFieldUserPosition RequestFieldUserPosition
        {
            get { return m_requestFieldUserPosition; }
            set { m_requestFieldUserPosition = value; }
        }

        public bool? RequestMobileUserList
        {
            get { return m_requestMobileUserList; }
            set { m_requestMobileUserList = value; }
        }
    }
}
