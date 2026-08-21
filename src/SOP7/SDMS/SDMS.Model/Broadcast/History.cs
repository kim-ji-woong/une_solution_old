using System;

namespace SDMS.Model.Broadcast
{
    public class History : IIDObject
    {
        public enum Fields { ID, Text, UseSiren, PlayOption, RepeatCount, RequestTime, ExecuteTime, SiteID };

        private int m_nID = -1;
        private string m_strText = "";
        private bool m_useSiren = false;
        // Broadcast.PlayType
        private int m_nPlayOption = (int)Broadcast.PlayType.STOP;
        // 반복횟수가 1이면 한번만 방송한다.
        private int m_nRepeatCount = 1;
        private DateTime m_requestTime = new DateTime();
        private DateTime m_executeTime = new DateTime();
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool UseSiren
        {
            get { return m_useSiren; }
            set { m_useSiren = value; }
        }

        // PlayType
        public int PlayOption
        {
            get { return m_nPlayOption; }
            set { m_nPlayOption = value; }
        }

        // 반복횟수가 1이면 한번만 방송한다.
        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
        }

        public DateTime RequestTime
        {
            get { return m_requestTime; }
            set { m_requestTime = value; }
        }

        public DateTime ExecuteTime
        {
            get { return m_executeTime; }
            set { m_executeTime = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsBroadcastHistory"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
