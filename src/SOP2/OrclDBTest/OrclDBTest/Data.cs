using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrclDBTest
{
    public class RegularTeam
    {
        private string m_strTeamCode = "";
        private string m_strParentTeamCode = "";
        private string m_strTeamName = "";
        private int m_nTeamID = -1;
        private string m_strTeamManagerName = "";

        public string TeamCode
        {
            get { return m_strTeamCode; }
            set { m_strTeamCode = value; }
        }

        public string ParentTeamCode
        {
            get { return m_strParentTeamCode; }
            set { m_strParentTeamCode = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamManager
        {
            get { return m_strTeamManagerName; }
            set { m_strTeamManagerName = value; }
        }
    }

    public class RegularTeamMember
    {
        private string m_strEMPNO = ""; // --사번
        private RegularTeam m_team = null;
        private string m_strLEVELNO = ""; // -- 직급
        private string m_strMAILNO = ""; // --메일주소
        private string m_strNAME = ""; // --이름
        private string m_strTelNo = ""; // 회사 전화번호
        private string m_strHP = ""; // --핸드폰 번호
        private string m_strTITLE = ""; // --직책
        private bool m_isTeamLeader = false; // --부서장여부

        // 사번
        public string EMPNO
        {
            get { return m_strEMPNO; }
            set { m_strEMPNO = value; }
        }

        public RegularTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        // 직급
        public string LEVELNO
        {
            get { return m_strLEVELNO; }
            set { m_strLEVELNO = value; }
        }

        public string MailAddress
        {
            get { return m_strMAILNO; }
            set { m_strMAILNO = value; }
        }

        public string NAME
        {
            get { return m_strNAME; }
            set { m_strNAME = value; }
        }

        public string TelNo
        {
            get { return m_strTelNo; }
            set { m_strTelNo = value; }
        }

        public string HandPhoneNumber
        {
            get { return m_strHP; }
            set { m_strHP = value; }
        }

        public string Title
        {
            get { return m_strTITLE; }
            set { m_strTITLE = value; }
        }

        public bool IsTeamLeader
        {
            get { return m_isTeamLeader; }
            set { m_isTeamLeader = value; }
        }
    }
}
