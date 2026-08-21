using System;
using System.Collections.Generic;

namespace SDMS.Model.Alarm
{
    public class CurrentAlarm
    {
        public enum Fields { SensorZoneHistoryID, SensorType, AlarmType, TimeStamp, SopStatus, AlarmDepth, AlarmSensorZoneIDs };

        public enum AlarmTypes { Detect = 0, Report };

        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorType = -1;
        private int m_nAlarmType = -1;        
        // 신호탐지 시간 혹은 재난신고 시간
        private DateTime m_timeStamp = new DateTime();
        // SOP 실행 상태 (-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중)
        private int m_nSopStatus = -1;
        private int m_nAlarmDepth = 0;
        // 하나의 알람에 여러 센서들이 연관되어 있을수 있다.
        // 현재 알람상태인 모든 센서들의 ID가 담겨있다.
        private List<int> m_alarmSensorZoneIDs = new List<int>();

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int AlarmType
        {
            get { return m_nAlarmType; }
            set { m_nAlarmType = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public int SopStatus
        {
            get { return m_nSopStatus; }
            set { m_nSopStatus = value; }
        }

        public int AlarmDepth
        {
            get { return m_nAlarmDepth; }
            set { m_nAlarmDepth = value; }
        }

        // 하나의 알람에 여러 센서들이 연관되어 있을수 있다.
        // 현재 알람상태인 모든 센서들의 ID가 담겨있다.
        public List<int> AlarmSensorZoneIDs
        {
            get { return m_alarmSensorZoneIDs; }
            set { m_alarmSensorZoneIDs = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsAlarmCurrent"; }
        }
    }
}
