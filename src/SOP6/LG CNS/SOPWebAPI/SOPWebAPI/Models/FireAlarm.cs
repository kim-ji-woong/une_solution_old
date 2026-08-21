using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class FireAlarm : Alarm
    {
    }

    public class FireParams
    {
        /// <summary>
        /// 알람의 고유 ID
        /// </summary>
        public string alarmID = "";

        /// <summary>
        /// 알람이 발생한 위치
        /// </summary>
        public string alarmPosition = "";

        /// <summary>
        /// 화재 발생 시각(0000-00-00 00:00:00)
        /// </summary>
        public string alarmTime = "";

        /// <summary>
        /// 위기경보단계(0 : 관심, 1 : 주의 : 2 : 경계, 3 : 심각)
        /// 화재 On일 경우에는 사용되는 값이며, 화재 Off일 경우에는 아무 값이나 입력 가능
        /// </summary>
        public int alarmLevel;

        /// <summary>
        /// 화재 On/Off(1이면 화재발생, 0이면 화재종료)
        /// </summary>
        public int onOff;
    }

    public class Alarm
    {
        private string m_strAlarmID = "";
        private DateTime m_alarmTime = new DateTime();
        private Zone m_zone = null;
        private bool m_isAlarmOn = true;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorTagID = -1;
        private int m_nSensorZoneID = -1;
        private int m_nWebHistoryID = -1;

        public string AlarmID
        {
            get { return m_strAlarmID; }
            set { m_strAlarmID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_alarmTime; }
            set { m_alarmTime = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public bool IsAlarmOn
        {
            get { return m_isAlarmOn; }
            set { m_isAlarmOn = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public int SensorTagID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int WebHistoryID
        {
            get { return m_nWebHistoryID; }
            set { m_nWebHistoryID = value; }
        }
    }
}
