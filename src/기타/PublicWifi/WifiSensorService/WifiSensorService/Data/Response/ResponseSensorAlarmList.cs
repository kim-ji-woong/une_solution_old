using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WifiSensorService.Data.Response
{
    public class ResponseSensorAlarmList : MessageResult
    {
        private List<AlarmData> m_alarmList = new List<AlarmData>();

        public List<AlarmData> SensorAlarmList
        {
            get { return m_alarmList; }
            set { m_alarmList = value; }
        }

        public ResponseSensorAlarmList()
            : base()
        {
        }

        public ResponseSensorAlarmList(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }

    public class AlarmData
    {
        private string m_strID = "";
        private bool m_isActive = false;
        private string m_regdate = "";
        private string m_strSensorType = "";
        private string m_strAlarmType = "";

        public string Id
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public bool Active
        {
            get { return m_isActive; }
            set { m_isActive = value; }
        }

        public string Regdate
        {
            get { return m_regdate; }
            set { m_regdate = value; }
        }

        public string Stype
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public string Atype
        {
            get { return m_strAlarmType; }
            set { m_strAlarmType = value; }
        }
    }
}
