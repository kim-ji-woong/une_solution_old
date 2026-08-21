using System;
using System.Collections.Generic;
using System.Text;

namespace SafetyServer.BLL.Data.Models
{
    public class MobileUser
    {
        private int m_nID = -1;
        private string m_strName = "";
        private string m_strMemberID = "";
        private string m_strTeamName = "";
        private string m_strJobLevelName = "";
        private int? m_nZoneID = null;
        private float? m_x = null;
        private float? m_y = null;
        private bool m_loginStatus = false;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string JobLevelName
        {
            get { return m_strJobLevelName; }
            set { m_strJobLevelName = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public float? X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public float? Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public bool LoginStatus
        {
            get { return m_loginStatus; }
            set { m_loginStatus = value; }
        }
    }
}
