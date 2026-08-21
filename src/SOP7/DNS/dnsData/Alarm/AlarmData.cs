using System;
using System.Collections.Generic;
using SDMS.Model.History;

namespace dnsData.Alarm
{
    public class AlarmData
    {
        // 알람에 대한 SOP 처리여부
        // SOP의 진행상태에 대한 정보는 없고, 알람에 대하여 SOP가 실행되었는가 여부를 나타낸다.
        // None : 아직 결정되지 않았다.
        // Run : SOP가 실행되었다.
        // Ignore : 알람에 대하여 SOP를 실행시키지 않기로 하였다.
        public enum SOPProcessType { None, Run, Igonore };

        public enum AlarmType
        {
            NO_ALARM = 0,
            ALARM = 1,
            NOT_CONNECTED = 2,
            CONNECTED = 3,
            PSM_ALARM_1 = 21,
            PSM_ALARM_2 = 22,
            PSM_ALARM_3 = 23
        }

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private int m_nSensorReactionHistoryID = -1;
        public int SensorReactionHistoryID
        {
            get { return m_nSensorReactionHistoryID; }
            set { m_nSensorReactionHistoryID = value; }
        }

        private DateTime dtTime;
        public DateTime TimeStamp
        {
            get { return dtTime; }
            set { dtTime = value; }
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        private SensorReactionHistory.ReactionTypes m_status = SensorReactionHistory.ReactionTypes.ETC;
        public SensorReactionHistory.ReactionTypes Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        private int m_nAlarmDepth = 1;
        public int AlarmDepth
        {
            get { return m_nAlarmDepth; }
            set { m_nAlarmDepth = value; }
        }

        private bool m_isReal = true;
        public bool IsReal
        {
            get { return m_isReal; }
            set { m_isReal = value; }
        }

        private Sensor.Facility.FacilityType m_sensorType = Sensor.Facility.FacilityType.NONE;
        public Sensor.Facility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        // 알람발생시 문자 발송한 전화번호 리스트
        // 중복검사를 빠르게 하기 위하여 List 대신 Dictionary를 사용한다.
        // Key와 Value는 모두 같은 값이다.
        private Dictionary<string, string> m_dicPhoneNumbers = new Dictionary<string, string>();
        public Dictionary<string, string> PhoneNumbers
        {
            get { return m_dicPhoneNumbers; }
        }

        // 알람발생시 이메일 발송한 이메일 리스트
        // 중복검사를 빠르게 하기 위하여 List 대신 Dictionary를 사용한다.
        // Key와 Value는 모두 같은 값이다.
        private Dictionary<string, string> m_dicEmails = new Dictionary<string, string>();
        public Dictionary<string, string> Emails
        {
            get { return m_dicEmails; }
        }

        // 알람발생시 문자 발송한 정규직원의 ID List
        // 중복검사를 빠르게 하기 위하여 List 대신 Dictionary를 사용한다.
        // Key와 Value는 모두 같은 값이다.
        private Dictionary<int, int> m_dicRegularMemberIDs = new Dictionary<int, int>();
        public Dictionary<int, int> RegularMemberIDs
        {
            get { return m_dicRegularMemberIDs; }
        }

        // 알람발생시 문자 발송한 외부직원의 ID List
        // 중복검사를 빠르게 하기 위하여 List 대신 Dictionary를 사용한다.
        // Key와 Value는 모두 같은 값이다.
        private Dictionary<int, int> m_dicExternalMemberIDs = new Dictionary<int, int>();
        public Dictionary<int, int> ExternalMemberIDs
        {
            get { return m_dicExternalMemberIDs; }
        }

        private string m_strReactionHistoryParam1 = "";
        public string ReactionHistoryParam1
        {
            get { return m_strReactionHistoryParam1; }
            set { m_strReactionHistoryParam1 = value; }
        }

        private string m_strReactionHistoryParam2 = "";
        public string ReactionHistoryParam2
        {
            get { return m_strReactionHistoryParam2; }
            set { m_strReactionHistoryParam2 = value; }
        }

        private string m_strReactionHistoryParam3 = "";
        public string ReactionHistoryParam3
        {
            get { return m_strReactionHistoryParam3; }
            set { m_strReactionHistoryParam3 = value; }
        }

        private string m_strReactionHistoryParam4 = "";
        public string ReactionHistoryParam4
        {
            get { return m_strReactionHistoryParam4; }
            set { m_strReactionHistoryParam4 = value; }
        }

        private string m_strReactionHistoryParam5 = "";
        public string ReactionHistoryParam5
        {
            get { return m_strReactionHistoryParam5; }
            set { m_strReactionHistoryParam5 = value; }
        }

        private SOPProcessType m_sopProcess = SOPProcessType.None;
        public SOPProcessType SOPProcess
        {
            get { return m_sopProcess; }
            set { m_sopProcess = value; }
        }

        // 수동신고된 알람인가?
        private bool m_isManual = false;
        public bool IsManual
        {
            get { return m_isManual; }
            set { m_isManual = value; }
        }

        private object m_tag = null;
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public AlarmData Clone()
        {
            AlarmData alarm = new AlarmData();

            alarm.m_nSensorZoneHistoryID = this.m_nSensorZoneHistoryID;
            alarm.m_nSensorReactionHistoryID = this.m_nSensorReactionHistoryID;
            alarm.m_nSensorZoneID = this.m_nSensorZoneID;
            alarm.m_status = this.m_status;
            alarm.m_strMessage = this.m_strMessage;
            alarm.m_nAlarmDepth = this.m_nAlarmDepth;
            alarm.m_isReal = this.m_isReal;
            alarm.dtTime = this.dtTime;
            alarm.m_sensorType = this.m_sensorType;
            alarm.m_sopProcess = this.m_sopProcess;
            alarm.m_isManual = this.m_isManual;
            alarm.m_tag = this.m_tag;

            foreach (KeyValuePair<string, string> pair in this.m_dicPhoneNumbers)
            {
                alarm.m_dicPhoneNumbers[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<int, int> pair in this.m_dicRegularMemberIDs)
            {
                alarm.m_dicRegularMemberIDs[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<int, int> pair in this.m_dicExternalMemberIDs)
            {
                alarm.m_dicExternalMemberIDs[pair.Key] = pair.Value;
            }

            return alarm;
        }
    }
}
