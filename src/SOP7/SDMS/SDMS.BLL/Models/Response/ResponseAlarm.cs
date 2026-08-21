using SDMS.BLL.Models.Alarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL.Models.Response
{
    public class ResponseAlarm
    {
        private List<AlarmData> m_alarmDatas = new List<AlarmData>();
        public List<AlarmData> AlarmDatas
        {
            get { return m_alarmDatas; }
            set { m_alarmDatas = value; }
        }

        private List<AlarmData> m_allAlarmDatas = new List<AlarmData>();
        public List<AlarmData> AllAlarmDatas
        {
            get { return m_allAlarmDatas; }
            set { m_allAlarmDatas = value; }
        }
    }
}
