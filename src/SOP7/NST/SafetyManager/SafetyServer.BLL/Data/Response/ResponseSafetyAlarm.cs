using System.Collections.Generic;
using SafetyServer.BLL.Data.Models;

namespace SafetyServer.BLL.Data.Response
{
    public class ResponseSafetyAlarm
    {
        private List<SafetyAlarm> m_alarmDatas = new List<SafetyAlarm>();
        public List<SafetyAlarm> AlarmDatas
        {
            get { return m_alarmDatas; }
            set { m_alarmDatas = value; }
        }

        private List<SafetyAlarm> m_allAlarmDatas = new List<SafetyAlarm>();
        public List<SafetyAlarm> AllAlarmDatas
        {
            get { return m_allAlarmDatas; }
            set { m_allAlarmDatas = value; }
        }
    }
}
