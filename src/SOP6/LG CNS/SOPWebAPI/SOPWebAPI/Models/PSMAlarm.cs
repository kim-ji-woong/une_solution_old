using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class PSMAlarm : Alarm
    {
        private string m_strAlarmMessage = "";

        public string AlarmMessage
        {
            get { return m_strAlarmMessage; }
            set { m_strAlarmMessage = value; }
        }
    }

    public class PSMParams
    {
        /// <summary>
        /// 알람의 고유 ID
        /// </summary>
        public string alarmID = "";

        /// <summary>
        /// 알람명
        /// </summary>
        public string alarmName = "";

        /// <summary>
        /// 알람메시지
        /// </summary>
        public string alarmMsg = "";

        /// <summary>
        /// 알람 발생 시각(0000-00-00 00:00:00)
        /// </summary>
        public string alarmTime = "";

        /// <summary>
        /// 알람 데이터.
        /// 예 : 온도 40도, 암모니아 25ppm 누출...
        /// </summary>
        public string alarmValue = "";

        /// <summary>
        /// 상황 On/Off(1이면 상황발생, 0이면 상황종료) (alarmState)
        /// </summary>
        public string alarmState;

        /// <summary>
        /// 위기경보단계(0 : 관심, 1 : 주의 : 2 : 경계, 3 : 심각)
        /// 상황 On일 경우에는 사용되는 값이며, 상황 Off일 경우에는 아무 값이나 입력 가능
        /// </summary>
        public string alarmLevel;

        /// <summary>
        /// 알람 발생 위치
        /// </summary>
        public string alarmLocation = "";

        /// <summary>
        /// 태그 ID
        /// </summary>
        public string tagID = "";

        /// <summary>
        /// Device ID
        /// </summary>
        public string deviceID = "";

        /// <summary>
        /// DeviceName
        /// </summary>
        public string deviceName = "";
    }
}