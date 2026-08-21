using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TeamReader
{
    // 자체 DB Manager
    public class OwnDataManager
    {
        private WebDBManager m_dbMgr = null;
        // RegularTeam ID, RegularTeam
        private Dictionary<int, RegularTeam> m_dicRegularTeam = new Dictionary<int,RegularTeam>();
        // 사번, CompanyMember
        private Dictionary<string, CompanyMember> m_dicCompanyMember = new Dictionary<string, CompanyMember>();
        private int m_nMaxRegularTeamID = -1;
        private int m_nMaxCompanyMemberID = 0;

        public OwnDataManager()
        {
            m_dbMgr = new WebDBManager();
        }

        public bool Load()
        {
            m_dicRegularTeam.Clear();
            m_dicCompanyMember.Clear();

            if (m_dbMgr == null)
                return false;

            if (!LoadRegularTeam())
                return false;
            if (!LoadCompanyMember())
                return false;

            return true;
        }

        private bool LoadCompanyMember()
        {
            string strSQL = "select id, MemberName, RegularTeamID, LevelID, PositionID, MemberID, OfficePhoneNumber, PhoneNumber from CompanyMember";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");

                if (!m_dicRegularTeam.ContainsKey(nTeamID))
                    continue;

                CompanyMember member = new CompanyMember();

                member.ID = nID;
                member.MemberName = strMemberName;
                member.Team = m_dicRegularTeam[nTeamID];
                member.LevelID = nLevelID;
                member.PositionID = nPositionID;
                member.MemberID = strMemberID;
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.PhoneNumber = strPhoneNumber;

                m_dicCompanyMember[strMemberID] = member;

                if (m_nMaxCompanyMemberID < nID)
                    m_nMaxCompanyMemberID = nID;
            }

            return true;
        }

        private bool LoadRegularTeam()
        {
            string strSQL = "select ID, TeamName, ParentTeamID from RegularTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                RegularTeam team = new RegularTeam();

                team.ID = nID;
                team.TeamName = strTeamName;
                team.TeamCode = nParentTeamID.ToString();

                m_dicRegularTeam[nID] = team;

                if (m_nMaxRegularTeamID < nID)
                    m_nMaxRegularTeamID = nID;
            }

            foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeam)
            {
                int nParentTeamID = -1;
                RegularTeam team = pair.Value;

                try
                {
                    nParentTeamID = int.Parse(team.TeamCode.ToString());
                }
                catch (Exception)
                {
                    nParentTeamID = -1;
                }

                team.TeamCode = "";

                if (nParentTeamID > 0)
                {
                    if (m_dicRegularTeam.ContainsKey(nParentTeamID))
                    {
                        RegularTeam teamParent = m_dicRegularTeam[nParentTeamID];
                        team.ParentTeam = teamParent;
                    }
                }
            }

            return true;
        }

        public bool UpdateData(CustomerDataManager customerMgr)
        {
            // customer측 팀에 해당하는 Own 팀 객체 데이터
            Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O = new Dictionary<RegularTeam, RegularTeam>();
            // Own 팀에 해당하는 Customer 팀 데이터
            Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C = new Dictionary<RegularTeam, RegularTeam>();
            // Own 팀원에 해당하는 Customer 팀원 데이터
            Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C = new Dictionary<CompanyMember, CompanyMember>();

            // Customer에 있는 것을 Own으로 옮기기
            if (!UpdateRegularTeam(customerMgr, dicTeamLinkC2O, dicTeamLinkO2C))
                return false;
            if (!UpdateCompanyMember(customerMgr, dicTeamLinkC2O, dicMemberLinkO2C))
                return false;
            ////////////////////////////////////////

            // Own에 있는것 가운데 Customer에 없는것 지우기
            RemoveUnregisterdMember(dicMemberLinkO2C);
            RemoveUnregisterdTeam(dicTeamLinkO2C);
            ////////////////////////////////////////

            return true;
        }

        private void RemoveUnregisterdMember(Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C)
        {
            string strIDs = "";

            foreach (KeyValuePair<string, CompanyMember> pair in m_dicCompanyMember)
            {
                if (!dicMemberLinkO2C.ContainsKey(pair.Value))
                {
                    if (strIDs.Length == 0)
                        strIDs = pair.Value.ID.ToString();
                    else
                        strIDs += ", " + pair.Value.ID.ToString();
                }
            }

            if (strIDs.Length == 0)
                return;

            string strSQL = string.Format("delete from CompanyMember where id in ({0})", strIDs);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void RemoveUnregisterdTeam(Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C)
        {
            string strIDs = "";

            foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeam)
            {
                if (!dicTeamLinkO2C.ContainsKey(pair.Value))
                {
                    if (strIDs.Length == 0)
                        strIDs = pair.Key.ToString();
                    else
                        strIDs += ", " + pair.Key.ToString();
                }
            }

            if (strIDs.Length == 0)
                return;

            string strSQL = string.Format("delete from RegularTeam where id in ({0})", strIDs);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private bool UpdateCompanyMember(CustomerDataManager customerMgr, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C)
        {
            foreach (KeyValuePair<string, CompanyMember> pair in customerMgr.CompanyMembers)
            {
                CompanyMember memberTrg = pair.Value;

                if (!UpdateMember(memberTrg, dicTeamLinkC2O, dicMemberLinkO2C))
                    return false;
            }

            return true;
        }

        private bool UpdateMember(CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C)
        {
            CompanyMember memberSrc = FindCompanyMember(memberTrg, dicTeamLinkC2O);

            if (memberSrc != null)
            {
                UpdateMember(memberSrc, memberTrg, dicTeamLinkC2O);
            }
            else
            {
                memberSrc = InsertNewMember(memberTrg, dicTeamLinkC2O);

                if (memberSrc == null)
                    return false;

                m_dicCompanyMember[memberSrc.MemberID] = memberSrc;
            }

            dicMemberLinkO2C[memberSrc] = memberTrg;
            return true;
        }

        private CompanyMember InsertNewMember(CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLink)
        {
            if (!dicTeamLink.ContainsKey(memberTrg.Team))
                return null;

            CompanyMember member = new CompanyMember();

            member.ID = m_nMaxCompanyMemberID + 1;
            member.LevelID = memberTrg.LevelID;
            member.MemberID = memberTrg.MemberID;
            member.MemberName = memberTrg.MemberName;
            member.OfficePhoneNumber = memberTrg.OfficePhoneNumber;
            member.PhoneNumber = memberTrg.PhoneNumber;
            member.PositionID = memberTrg.PositionID;
            member.Team = dicTeamLink[memberTrg.Team];
            member.Title = memberTrg.Title;

            string strFormat = "insert into CompanyMember (id, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, OfficePhoneNumber, PhoneNumber) ";
            strFormat += "values ({0}, '{1}', {2}, {3}, {4}, '{5}', NULL, NULL, '{6}', '{7}')";

            string strSQL = string.Format(strFormat, member.ID, member.MemberName, member.Team.ID, member.LevelID, member.PositionID, member.MemberID, member.OfficePhoneNumber, member.PhoneNumber);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return null;

            m_nMaxCompanyMemberID = member.ID;
            return member;
        }

        private void UpdateMember(CompanyMember memberSrc, CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLink)
        {
            bool isChanged = false;

            if (memberSrc.LevelID != memberTrg.LevelID)
            {
                memberSrc.LevelID = memberTrg.LevelID;
                isChanged = true;
            }

            if (memberSrc.MemberName != memberTrg.MemberName)
            {
                memberSrc.MemberName = memberTrg.MemberName;
                isChanged = true;
            }

            if (memberSrc.OfficePhoneNumber != memberTrg.OfficePhoneNumber)
            {
                memberSrc.OfficePhoneNumber = memberTrg.OfficePhoneNumber;
                isChanged = true;
            }

            if (memberSrc.PhoneNumber != memberTrg.PhoneNumber)
            {
                memberSrc.PhoneNumber = memberTrg.PhoneNumber;
                isChanged = true;
            }

            if (memberSrc.PositionID != memberTrg.PositionID)
            {
                memberSrc.PositionID = memberTrg.PositionID;
                isChanged = true;
            }

            if (!dicTeamLink.ContainsKey(memberTrg.Team))
                return;

            RegularTeam teamSrc = dicTeamLink[memberTrg.Team];

            if (memberSrc.Team != teamSrc)
            {
                memberSrc.Team = teamSrc;
                isChanged = true;
            }

            memberSrc.Title = memberTrg.Title;

            if (isChanged)
            {
                string strSQL = string.Format("Update CompanyMember set MemberName = '{0}', RegularTeamID = {1}, LevelID = {2}, PositionID = {3}, OfficePhoneNumber = '{4}', PhoneNumber = '{5}' where id = {6}",
                    memberSrc.MemberName, memberSrc.Team.ID, memberSrc.LevelID, memberSrc.PositionID, memberSrc.OfficePhoneNumber, memberSrc.PhoneNumber, memberSrc.ID);

                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        private CompanyMember FindCompanyMember(CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLink)
        {
            /*if (memberTrg.Team == null)
                return null;

            if (!dicTeamLink.ContainsKey(memberTrg.Team))
                return null;*/

            if (m_dicCompanyMember.ContainsKey(memberTrg.MemberID))
                return m_dicCompanyMember[memberTrg.MemberID];

            return null;
        }

        private bool UpdateRegularTeam(CustomerDataManager customerMgr, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C)
        {
            ArrayList arrCheckedTeam = new ArrayList();
            
            foreach (KeyValuePair<string, RegularTeam> pair in customerMgr.RegularTeams)
            {
                RegularTeam teamTrg = pair.Value;
                if (!UpdateTeam(teamTrg, arrCheckedTeam, dicTeamLinkC2O, dicTeamLinkO2C))
                    return false;
            }

            return true;
        }

        private bool UpdateTeam(RegularTeam teamTrg, ArrayList arrCheckedTeam, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C)
        {
            RegularTeam _team = teamTrg;

            ArrayList arrTeamDepth = new ArrayList();
            arrTeamDepth.Add(_team);

            while (_team.ParentTeam != null)
            {
                arrTeamDepth.Insert(0, _team.ParentTeam);
                _team = _team.ParentTeam;
            }

            //RegularTeam teamParent = null;

            foreach (RegularTeam team in arrTeamDepth)
            {
                if (arrCheckedTeam.Contains(team))
                    continue;

                RegularTeam teamSrc = FindTeam(team, team.ParentTeam);//teamParent);

                if (teamSrc != null)
                {
                    teamSrc.TeamCode = team.TeamCode;
                }
                else// if (teamSrc == null)
                {
                    RegularTeam teamParent = null;

                    if (team.ParentTeam != null && dicTeamLinkC2O.ContainsKey(team.ParentTeam))
                        teamParent = dicTeamLinkC2O[team.ParentTeam];

                    teamSrc = InsertNewTeam(team, teamParent);//team.ParentTeam);

                    if (teamSrc == null)
                        return false;
                    else
                        m_dicRegularTeam[teamSrc.ID] = teamSrc;
                }

                dicTeamLinkC2O[team] = teamSrc;
                dicTeamLinkO2C[teamSrc] = team;
                arrCheckedTeam.Add(team);
            }

            return true;
        }

        private RegularTeam InsertNewTeam(RegularTeam teamTrg, RegularTeam teamParent)
        {
            RegularTeam team = new RegularTeam();

            team.ID = m_nMaxRegularTeamID + 1;
            team.ParentTeam = teamParent;
            team.TeamCode = teamTrg.TeamCode;
            team.TeamName = teamTrg.TeamName;

            string strSQL = string.Format("insert into RegularTeam (id, TeamName, ParentTeamID) values ({0}, '{1}', {2})",
                team.ID, team.TeamName, teamParent == null ? "NULL" : teamParent.ID.ToString());

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return null;

            m_nMaxRegularTeamID = team.ID;
            return team;
        }

        private RegularTeam FindTeam(RegularTeam teamTrg, RegularTeam teamParent)
        {
            foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeam)
            {
                RegularTeam team = pair.Value;

                if (RegularTeam.IsSame(team.ParentTeam, teamParent) && team.TeamName == teamTrg.TeamName)
                    return team;
                //if (team.ParentTeam == teamParent && team.TeamName == teamTrg.TeamName)
                //    return team;
            }

            return null;
        }
    }
}
