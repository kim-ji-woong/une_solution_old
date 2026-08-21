using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamReader
{
    // 고객사의 DB Manager
    public class CustomerDataManager
    {
        private OracleManager m_dbMgr = null;
        // RegularTeam Code, RegularTeam
        private Dictionary<string, RegularTeam> m_dicRegularTeam = new Dictionary<string, RegularTeam>();
        // 사번, CompanyMember
        private Dictionary<string, CompanyMember> m_dicCompanyMember = new Dictionary<string, CompanyMember>();
        private bool m_isOpened = false;

        public CustomerDataManager()
        {
            char[] arrID = new char[] { 'i', 'n', 's', 'a', '_', 'u', 's', 'e', 'r' };
            char[] arrPW = new char[] { 'i', 'n', 's', 'a', '1', '2', '3' };

            m_dbMgr = new OracleManager(new string(arrID), new string(arrPW), "ORA8");
            m_isOpened = m_dbMgr.OpenConnection();
        }

        public bool Load()
        {
            m_dicRegularTeam.Clear();
            m_dicCompanyMember.Clear();

			if (!m_isOpened)
				m_isOpened = m_dbMgr.OpenConnection();
            if (!m_isOpened)
                return false;

            // 부서장 설정을 위한 임시 변수
            Dictionary<RegularTeam, string> dicTeamLeader = new Dictionary<RegularTeam,string>();

            if (!m_dbMgr.LoadTeamList(m_dicRegularTeam, dicTeamLeader))
                return false;
            if (!m_dbMgr.LoadCompanyMemberList(m_dicCompanyMember, m_dicRegularTeam, dicTeamLeader))
                return false;

            return true;
        }

        public Dictionary<string, RegularTeam> RegularTeams
        {
            get { return m_dicRegularTeam; }
        }

        public Dictionary<string, CompanyMember> CompanyMembers
        {
            get { return m_dicCompanyMember; }
        }
    }
}
