using System;
using System.Collections.Generic;

namespace SDMS.Model.History
{
    /// <summary>
    /// SensorZoneHistory(알람)에 대한 처리이력을 나타낸다.
    /// </summary>
    public class SensorReactionHistory : IIDObject
    {
        public enum Fields { ID, SensorZoneHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5 };

        public enum ReactionTypes
        {
            NONE = -1,
            BEGIN_STATUS = 0,              // 상황 시작
            RUN_BROADCAST = 10,            // 사내 방송 실시         
            SEND_SMS = 11,                 // 문자메시지 발송
            MALFUNCTION = 21,              // 오작동 처리
            NOTIFY_SIGNAL = 22,            // 재난 신고
            IGNORE_SIGNAL = 23,            // 재난 탐지신호 무시

            RUN_SOP = 30,                  // SOP 발동 
            RUN_N_CANCEL_SOP = 31,         // SOP 실행중 취소
            FINISH_SOP = 32,               // SOP 종료
            IGNORE_SOP = 33,               // SOP 실행 안함
            END_STATUS = 50,               // 상황 종료

            CHANGE_ALARM_DEPTH = 62,
            USER_RESET = 64,
            ETC = 100,                     // 기타
            RUN_DETECT_BROADCAST = 101,
            RUN_REPORT_BROADCAST = 102,
            SEND_DETECT_SMS = 111,
            SEND_REPORT_SMS = 112,
            SEND_MALFUNCTION_SMS = 113,
            SEND_REPAIR_SMS = 114,

            TIME_OUT = 1000
        }

        private int m_nID = -1;
        private int m_nSensorZoneHistoryID = -1;
        private ReactionTypes m_reactionType = ReactionTypes.NONE;
        private DateTime m_time = new DateTime();
        private string m_strMessage = null;
        private string m_strParam1 = null;
        private string m_strParam2 = null;
        private string m_strParam3 = null;
        private string m_strParam4 = null;
        private string m_strParam5 = null;

        private static Dictionary<int, ReactionTypes> m_dicReactionType = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public ReactionTypes ReactionType
        {
            get { return m_reactionType; }
            set { m_reactionType = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Param1
        {
            get { return m_strParam1; }
            set { m_strParam1 = value; }
        }

        public string Param2
        {
            get { return m_strParam2; }
            set { m_strParam2 = value; }
        }

        public string Param3
        {
            get { return m_strParam3; }
            set { m_strParam3 = value; }
        }

        public string Param4
        {
            get { return m_strParam4; }
            set { m_strParam4 = value; }
        }

        public string Param5
        {
            get { return m_strParam5; }
            set { m_strParam5 = value; }
        }

        public static string TableName
        {
            get { return "SdmsHistorySensorReaction"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Message ||
                field == Fields.Param1 ||
                field == Fields.Param2 ||
                field == Fields.Param3 ||
                field == Fields.Param4 ||
                field == Fields.Param5)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static ReactionTypes ToReactionType(int nType)
        {
            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, ReactionTypes>();

                foreach (ReactionTypes type in Enum.GetValues(typeof(ReactionTypes)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            ReactionTypes rType;
            if (m_dicReactionType.TryGetValue(nType, out rType))
                return rType;

            return ReactionTypes.ETC;
        }
    }
}
