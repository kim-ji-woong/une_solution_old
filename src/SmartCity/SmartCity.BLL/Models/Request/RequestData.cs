namespace SmartCity.BLL.Models.Request
{
    public class RequestData
    {
        private RequestLogin m_requestLogin = null;
        private RequestSessionLogin m_requestSessionLogin = null;
        private RequestChangePassword m_requestChangePassword = null;
        private RequestLogout m_requestLogout = null;
        private RequestCheckUserID m_requestCheckUserID = null;
        private RequestCheckCode m_requestCheckCode = null;
        private RequestPWDFind m_requestPWDFind = null;
        private RequestFirstSensor m_requestFirstSensor = null;
        private RequestSensorInfo m_requestSensorInfo = null;
        private RequestFacilityTypeSensors m_requestFacilityTypeSensors = null;
        private RequestAlarmList m_requestAlarmList = null;
        private RequestManualList m_requestManualList = null;

        public RequestLogin RequestLogin
        {
            get { return m_requestLogin; }
            set { m_requestLogin = value; }
        }

        public RequestSessionLogin RequestSessionLogin
        {
            get { return m_requestSessionLogin; }
            set { m_requestSessionLogin = value; }
        }

        public RequestChangePassword RequestChangePassword
        {
            get { return m_requestChangePassword; }
            set { m_requestChangePassword = value; }
        }

        public RequestLogout RequestLogout
        {
            get { return m_requestLogout; }
            set { m_requestLogout = value; }
        }

        public RequestCheckUserID RequestCheckUserID
        {
            get { return m_requestCheckUserID; }
            set { m_requestCheckUserID = value; }
        }

        public RequestCheckCode RequestCheckCode
        {
            get { return m_requestCheckCode; }
            set { m_requestCheckCode = value; }
        }

        public RequestPWDFind RequestPWDFind
        {
            get { return m_requestPWDFind; }
            set { m_requestPWDFind = value; }
        }

        public RequestFirstSensor RequestFirstSensor
        {
            get { return m_requestFirstSensor; }
            set { m_requestFirstSensor = value; }
        }

        public RequestSensorInfo RequestSensorInfo
        {
            get { return m_requestSensorInfo; }
            set { m_requestSensorInfo = value; }
        }

        public RequestFacilityTypeSensors RequestFacilityTypeSensors
        {
            get { return m_requestFacilityTypeSensors; }
            set { m_requestFacilityTypeSensors = value; }
        }

        public RequestAlarmList RequestAlarmList
        {
            get { return m_requestAlarmList; }
            set { m_requestAlarmList = value; }
        }
        
        public RequestManualList RequestManualList
        {
            get { return m_requestManualList; }
            set { m_requestManualList = value; }
        }
    }

    public class RequestLogin
    {
        private string m_strValue = "";
        private string m_strKey = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestSessionLogin
    {
        private string m_strKey = "";

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestChangePassword
    {
        private string m_strValue = "";
        private string m_strKey = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestLogout
    {
        private string m_strKey = "";

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestCheckUserID
    {
        private string m_strUserID = "";

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }

    public class RequestCheckCode
    {
        private string m_strValue = "";
        private string m_strKey = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestPWDFind
    {
        private string m_strValue = "";
        private string m_strKey = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class RequestFirstSensor
    {
        private int m_nFacilityType = -1;

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }

    public class RequestSensorInfo
    {
        private int m_nID = -1;
        private int m_nFacilityType = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        } 

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }

    public class RequestFacilityTypeSensors
    {
        private int m_nFacilityType = -1;

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }

    public class RequestAlarmList
    {
        private int m_nFacilityType = -1;

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }

    public class RequestManualList
    {
        private int m_nFacilityType = -1;

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }
}
