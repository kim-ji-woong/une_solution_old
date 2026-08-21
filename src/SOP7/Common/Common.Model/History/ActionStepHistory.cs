using System;

namespace Common.Model.History
{
    public class ActionStepHistory
    {
        public enum Fields { ID, ActionStepID, RealMode, BeginTime, EndTime, LastAccessedTime, DetectEndTime, DetectTime, Position, LastAccessedUserID, StartOption, DisasterOption, SensorZoneHistoryID, Description };

        private int m_nID = -1;
        private int m_nActionStepID = -1;
        // 실제상황인가?
        private bool? m_realMode = null;
        private DateTime m_dtBegin;
        private DateTime? m_dtEnd = null;
        private DateTime? m_dtLastAccessed = null;
        private DateTime? m_dtDetectEnd = null;
        private DateTime? m_dtDetect = null;
        private string m_strPosition = null;
        private int? m_nLastAccessedUserID = null;
        // SOP시작 옵션 : 0:None 1:SMS 2:Broadcast 4:Reserve1 8:Reserve2
        private int? m_nStartOption = null;
        private string m_strDisasterOption = null;
        private int? m_nSensorZoneHistoryID = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public bool? RealMode
        {
            get { return m_realMode; }
            set { m_realMode = value; }
        }

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime? EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public DateTime? LastAccessedTime
        {
            get { return m_dtLastAccessed; }
            set { m_dtLastAccessed = value; }
        }

        public DateTime? DetectEndTime
        {
            get { return m_dtDetectEnd; }
            set { m_dtDetectEnd = value; }
        }

        public DateTime? DetectTime
        {
            get { return m_dtDetect; }
            set { m_dtDetect = value; }
        }

        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        public int? LastAccessedUserID
        {
            get { return m_nLastAccessedUserID; }
            set { m_nLastAccessedUserID = value; }
        }

        // SOP시작 옵션 : 0:None 1:SMS 2:Broadcast 4:Reserve1 8:Reserve2
        public int? StartOption
        {
            get { return m_nStartOption; }
            set { m_nStartOption = value; }
        }

        public string DisasterOption
        {
            get { return m_strDisasterOption; }
            set { m_strDisasterOption = value; }
        }

        public int? SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopHistoryActionStep"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.ActionStepID ||
                field == Fields.BeginTime)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
