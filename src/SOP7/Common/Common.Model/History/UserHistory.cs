using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model.History
{
    public class UserHistory
    {
        public enum Fields { ID, Time, UserID, TargetType, ActionType, HistoryContent }

        private int m_nID = -1;
        private DateTime m_dtTime = new DateTime();
        private int m_nUserID = -1;
        private int m_nTargetType = -1;
        private int m_nActionType = -1;
        private string m_strHistoryContent = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public int TargetType
        {
            get { return m_nTargetType; }
            set { m_nTargetType = value; }
        }

        public int ActionType
        {
            get { return m_nActionType; }
            set { m_nActionType = value; }
        }

        public string HistoryContent
        {
            get { return m_strHistoryContent; }
            set { m_strHistoryContent = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {            
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "CommonUserHistory"; }
        }
    }
}
