using Common.Model.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Data
{
    public class UserHistoryData
    {
        private int m_nID = -1;
        private string m_strTime = "";
        private string m_strName = "";
        private string m_strLevel = "";
        private string m_strTeamName = "";
        private string m_strTargetType = "";
        private string m_strActionType = "";
        private string m_strHistoryContent = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
        public string Level
        {
            get { return m_strLevel; }
            set { m_strLevel = value; }
        }
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }
        public string TargetType
        {
            get { return m_strTargetType; }
            set { m_strTargetType = value; }
        }
        public string ActionType
        {
            get { return m_strActionType; }
            set { m_strActionType = value; }
        }
        public string HistoryContent
        {
            get { return m_strHistoryContent; }
            set { m_strHistoryContent = value; }
        }
    }
}
