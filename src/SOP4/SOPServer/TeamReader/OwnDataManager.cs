using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using SOP;
using UnE.Sensor;

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

        // Key : JobLevel Table의 ID
        private Dictionary<int, string> m_dicJobLevelName = new Dictionary<int, string>();
        // 아직 직급이 정해지지 않았거나 알수 없는 상태
        private int m_nUnknownJobLevelID = -1;

        private int m_nSiteID = 1;
        public OwnDataManager(WebDBManager db)
        {
            m_nSiteID = SDMSServer.NetworkServer.Instance.SiteID;

			m_dbMgr = db;
        }

        public bool IsValidJobLevelID(int nLevelID)
        {
            return m_dicJobLevelName.ContainsKey(nLevelID);
        }

        public int GetUnknownLevelID()
        {
            return m_nUnknownJobLevelID;
        }

        public bool Load()
        {
            m_dicJobLevelName.Clear();
            m_dicRegularTeam.Clear();
            m_dicCompanyMember.Clear();

            if (m_dbMgr == null)
                return false;

            /*if (!LoadJobLevel())
                return false;*/
            if (!LoadRegularTeam())
                return false;

            Dictionary<int, CompanyMember> allMembers = LoadCompanyMember();

            if (allMembers == null)
                return false;

            if (!LoadRegularMembers(allMembers))
                return false;

            return true;
        }

        private bool LoadRegularMembers(Dictionary<int, CompanyMember> allMembers)
        {
            string strSQL = "select RegularTeamID, CompanyMemberID, PositionID from RegularMemberList";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            RegularTeam team;
            CompanyMember member;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                // SiteID가 m_nSiteID와 다른 직원은 여기서 걸러진다.
                if (!m_dicRegularTeam.TryGetValue(nTeamID, out team))
                    continue;

                if (!allMembers.TryGetValue(nMemberID, out member))
                    continue;

                team.Members.Add(member);
                m_dicCompanyMember[member.MemberID] = member;

                member.TeamPositions[team] = nPositionID;
            }

            return true;
        }

        private bool LoadJobLevel()
        {
            m_nUnknownJobLevelID = -1;

            string strSQL = "select id, LevelName from JobLevel";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            int nMinID = -1;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strJobName = WebDBManager.GetStringField(arrResult[i + 1], "");

                if (i == 0)
                    nMinID = nID;
                else if (nMinID > nID)
                    nMinID = nID;

                m_dicJobLevelName[nID] = strJobName;

                if (strJobName.Contains("알수없음") ||
                    strJobName.Contains("알수 없음") ||
                    strJobName.Contains("알 수 없음") ||
                    strJobName.Contains("Unknown"))
                    m_nUnknownJobLevelID = nID;
            }

            if (m_nUnknownJobLevelID < 0)
                m_nUnknownJobLevelID = nMinID;

            return true;
        }

        private Dictionary<int, CompanyMember> LoadCompanyMember()
        {
            string strSQL = "select id, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber from CompanyMember";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, CompanyMember> members = new Dictionary<int, CompanyMember>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                //int nTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                //int nPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");

                if (nLevelID < 0)
                {
                    // nLevelID가 0보다 작은 직원은 삭제된 직원이다.
                    continue;
                }

                //if (!m_dicRegularTeam.ContainsKey(nTeamID))
                //    continue;

                CompanyMember member = new CompanyMember();

                member.ID = nID;
                member.MemberName = strMemberName;
                //member.Team = m_dicRegularTeam[nTeamID];
                member.LevelID = nLevelID;
                //member.PositionID = nPositionID;
                member.MemberID = strMemberID;
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.PhoneNumber = strPhoneNumber;

                members[member.ID] = member;
                //m_dicCompanyMember[strMemberID] = member;

                if (m_nMaxCompanyMemberID < nID)
                    m_nMaxCompanyMemberID = nID;
            }

            return members;
        }



        private bool LoadRegularTeam()
        {

            string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult = SDMSServer.DataManager.ExecuteTeamList(m_dbMgr, nTeamID);
            /*string strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            ArrayList arrResult = m_dbMgr.GetStoredProcedureData(strSQL, 0);*/
            if (arrResult == null)
                return false;

            //string strSQL = "select ID, TeamName, ParentTeamID from RegularTeam";
            //ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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
            lock (SDMSServer.NetworkServer.Instance.MemberCriticalSection)
            {
                // customer측 팀에 해당하는 Own 팀 객체 데이터
                Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O = new Dictionary<RegularTeam, RegularTeam>();
                // Own 팀에 해당하는 Customer 팀 데이터
                Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C = new Dictionary<RegularTeam, RegularTeam>();
                // Own 팀원에 해당하는 Customer 팀원 데이터
                Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C = new Dictionary<CompanyMember, CompanyMember>();

                int nChangedConfigData = 0;

                // Customer에 있는 것을 Own으로 옮기기
                if (!UpdateRegularTeam(customerMgr, dicTeamLinkC2O, dicTeamLinkO2C, ref nChangedConfigData))
                    return false;
                if (!UpdateCompanyMember(customerMgr, dicTeamLinkC2O, dicMemberLinkO2C, ref nChangedConfigData))
                    return false;
                ////////////////////////////////////////

                // Own에 있는것 가운데 Customer에 없는것 지우기
                RemoveUnregisterdMember(dicMemberLinkO2C, ref nChangedConfigData);
                RemoveUnregisterdTeam(dicTeamLinkO2C, ref nChangedConfigData);
                ////////////////////////////////////////

                if (nChangedConfigData > 0)
                {
                    // Data가 바뀌었으니 Client들에게 알려준다.
                    SDMSServer.NetworkServer.Instance.ServiceProvider.SendChangedConfig(nChangedConfigData, SDMS.TCP_CLIENT.SDMS_CLIENT);
                    SDMSServer.NetworkServer.Instance.ServiceProvider.SendChangedConfig(nChangedConfigData, SDMS.TCP_CLIENT.SOP_SIMULATOR);
                    //SDMSServer.NetworkServer.Instance.ServiceProvider.SendChangedConfig(nChangedConfigData, SDMSServer.TCP_CLIENT.SOP_SIMULATOR);
                }
            }

            return true;
        }

        private void RemoveUnregisterdMember(Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C, ref int nChangedConfigData)
        {
            string strIDs = "";
            ArrayList arrRemoveIDs = new ArrayList();

            foreach (KeyValuePair<string, CompanyMember> pair in m_dicCompanyMember)
            {
                if (!dicMemberLinkO2C.ContainsKey(pair.Value))
                {
                    if (strIDs.Length == 0)
                        strIDs = pair.Value.ID.ToString();
                    else
                        strIDs += ", " + pair.Value.ID.ToString();

                    arrRemoveIDs.Add(pair.Value.ID);
                }
            }

            if (strIDs.Length == 0)
                return;

            if (!RemoveEquipZoneFacilityManager(strIDs, arrRemoveIDs, 0, ref nChangedConfigData))
                return;
            if (!RemoveBuildingFacilityManager(strIDs, arrRemoveIDs, 0, ref nChangedConfigData))
                return;
            if (!RemoveFacilityManager(strIDs, arrRemoveIDs, 0, ref nChangedConfigData))
                return;
            //if (!RemoveDuty(strIDs))
            //    return;
            if (!RemoveSOPGenUser(ref strIDs, arrRemoveIDs))
                return;
            if (!RemoveRegularMemberListFromMemberIDs(strIDs))
                return;

            if (strIDs.Length > 0)
            {
                string strSQL = string.Format("delete from CompanyMember where id in ({0})", strIDs);

                if (m_dbMgr.GetResultData(strSQL, 0) != null)
                    nChangedConfigData |= (int)SDMSConfig.ConfigType.COMPANY_MEMBER;
            }
        }

        private bool RemoveRegularMemberListFromMemberIDs(string strIDs)
        {
            string strSQL = string.Format("delete from RegularMemberList where CompanyMemberID in ({0})", strIDs);
            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool RemoveRegularMemberListFromTeamIDs(string strIDs)
        {
            string strSQL = string.Format("delete from RegularMemberList where RegularTeamID in ({0})", strIDs);
            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        // arrCompanyMemberIDs에 링크되어 있는 SOPGenUser는 삭제하지 않고 남겨두며
        // 해당 CompanyMember의 경우 LevelID를 -1로 둔다.
        private bool RemoveSOPGenUser(ref string strCompanyMemberIDs, ArrayList arrCompanyMemberIDs)
        {
            RegularTeam rootTeam = GetRootParentTeam();

            if (rootTeam == null)
                return false;

            string strSQL = "select MemberID from SOPGenUser where MemberID in (" + strCompanyMemberIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strIDs = "";

                foreach (object data in arrResult)
                {
                    int nID = WebDBManager.GetIntField(data.ToString(), -1);

                    if (nID > 0)
                    {
                        if (strIDs.Length == 0)
                            strIDs = nID.ToString();
                        else
                            strIDs += ", " + nID.ToString();

                        // 삭제되지 못한 직원 때문에 지워져야될 팀이 삭제되지 못할 수 있으므로, nID에 해당하는 직원의 팀은
                        // RootTeam으로 바꿔놓는다. 
                        strSQL = "Update CompanyMember set LevelID = -1, RegularTeamID = " + rootTeam.ID.ToString();
                        strSQL += " where id = " + nID.ToString();

                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return false;

                        arrCompanyMemberIDs.Remove(nID);
                    }
                }

                if (strIDs.Length > 0)
                {
                    strCompanyMemberIDs = "";

                    foreach (int nID in arrCompanyMemberIDs)
                    {
                        if (strCompanyMemberIDs.Length == 0)
                            strCompanyMemberIDs = nID.ToString();
                        else
                            strCompanyMemberIDs += ", " + nID.ToString();
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    // 한꺼번에 strIDs를 이용하여 업데이트 하지 않고 개별 Member들의 과거 이력을 남기도록 한다.
                    //strSQL = "Update CompanyMember set LevelID = -1 where id in (" + strIDs + ")";
                    //arrResult = m_dbMgr.GetResultData(strSQL, 0);

                    //return arrResult != null;
                }
            }

            return true;
        }

        /*private bool RemoveDuty(string strCompanyMemberIDs)
        {
            string strSQL = "delete from Duty where MemberID in (" + strCompanyMemberIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            return arrResult != null;
        }*/

        // isDeleted : 삭제된 데이터가 존재하는가?
        private bool RemoveEquipZoneFacilityManager(string strMemberIDs, ArrayList arrMemberIDs, int nMemberType, ref int nChangedConfigData)
        {
            bool isDeleted = SDMSServer.DataManager.Instance.RemoveEquipZoneFacilityManagers(arrMemberIDs, IFacility.FacilityType.FIRE_SENSOR, nMemberType);

            if (isDeleted)
                nChangedConfigData |= (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER;

            string strSQL = "delete from EquipZoneFacilityManager where MemberType = " + nMemberType.ToString() + " and MemberID in (" + strMemberIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            return arrResult != null;
        }

        // isDeleted : 삭제된 데이터가 존재하는가?
        private bool RemoveBuildingFacilityManager(string strMemberIDs, ArrayList arrMemberIDs, int nMemberType, ref int nChangedConfigData)
        {
            bool isDeleted = SDMSServer.DataManager.Instance.RemoveBuildingFacilityManagers(arrMemberIDs, IFacility.FacilityType.FIRE_SENSOR, nMemberType);

            if (isDeleted)
                nChangedConfigData |= (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER;

            string strSQL = "delete from BuildingFacilityManager where MemberType = " + nMemberType.ToString() + " and MemberID in (" + strMemberIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            return arrResult != null;
        }

        // isDeleted : 삭제된 데이터가 존재하는가?
        private bool RemoveFacilityManager(string strMemberIDs, ArrayList arrMemberIDs, int nMemberType, ref int nChangedConfigData)
        {
            bool isDeleted = SDMSServer.DataManager.Instance.RemoveEntireFacilityManagers(arrMemberIDs, IFacility.FacilityType.FIRE_SENSOR, nMemberType);

            if (isDeleted)
                nChangedConfigData |= (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER;

            string strSQL = "delete from FacilityManager where MemberType = " + nMemberType.ToString() + " and MemberID in (" + strMemberIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            return arrResult != null;
        }

        private void RemoveUnregisterdTeam(Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C, ref int nChangedConfigData)
        {
            ArrayList arrRemovedTeamIDs = new ArrayList();
            string strIDs = "";

            foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeam)
            {
                if (!dicTeamLinkO2C.ContainsKey(pair.Value))
                {
                    arrRemovedTeamIDs.Add(pair.Key);

                    if (strIDs.Length == 0)
                        strIDs = pair.Key.ToString();
                    else
                        strIDs += ", " + pair.Key.ToString();
                }
            }

            if (arrRemovedTeamIDs.Count == 0)
                return;
            else
                nChangedConfigData |= (int)SDMSConfig.ConfigType.REGULAR_TEAM;

            UpdateTemporaryTeam(arrRemovedTeamIDs, ref nChangedConfigData, true);
            UpdateTemporaryTeam(arrRemovedTeamIDs, ref nChangedConfigData, false);

            if (!RemoveEquipZoneFacilityManager(strIDs, arrRemovedTeamIDs, 1, ref nChangedConfigData))
                return;
            if (!RemoveBuildingFacilityManager(strIDs, arrRemovedTeamIDs, 1, ref nChangedConfigData))
                return;
            if (!RemoveFacilityManager(strIDs, arrRemovedTeamIDs, 1, ref nChangedConfigData))
                return;
            if (!RemoveRegularMemberListFromTeamIDs(strIDs))
                return;

            string strSQL = string.Format("delete from RegularTeam where id in ({0})", strIDs);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        // arrRemovedTeamIDs : 삭제될 Regular Team ID들
        private void UpdateTemporaryTeam(ArrayList arrRemovedTeamIDs, ref int nChangedConfigData, bool isTemporaryNormalTeam)
        {
            string strTableName = "";
            int nConfigData = 0;

            if (isTemporaryNormalTeam)
            {
                strTableName = "TemporaryNormalTeam";
                nConfigData = (int)SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM;
            }
            else
            {
                strTableName = "TemporaryEmergencyTeam";
                nConfigData = (int)SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM;
            }

            string strSQL = "select id, RegularTeamLink from " + strTableName + " where RegularTeamLink is not null";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            string strFormat = "Update " + strTableName + " set RegularTeamLink = '{0}' where id = {1}";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamLink = WebDBManager.GetStringField(arrResult[i + 1], "");

                string[] arrTeamIDs = strTeamLink.Split(',');

                string strNewTeamLink = "";
                bool isChanged = false;
                int nTeamID;

                foreach (string strID in arrTeamIDs)
                {
                    string _strID = strNewTeamLink.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                    _strID = _strID.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                    if (!int.TryParse(_strID, out nTeamID))
                        continue;

                    int id = nTeamID >= 0 ? nTeamID : -nTeamID;

                    if (!arrRemovedTeamIDs.Contains(id))
                    {
                        if (strNewTeamLink.Length == 0)
                            strNewTeamLink = nTeamID.ToString();
                        else
                            strNewTeamLink += ", " + nTeamID.ToString();
                    }
                    else
                        isChanged = true;
                }

                if (isChanged)
                {
                    nChangedConfigData |= nConfigData;

                    strSQL = string.Format(strFormat, strNewTeamLink, nID);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return;
                }
            }
        }

        private bool UpdateCompanyMember(CustomerDataManager customerMgr, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C, ref int nChangedConfigData)
        {
            foreach (KeyValuePair<string, CompanyMember> pair in customerMgr.CompanyMembers)
            {
                CompanyMember memberTrg = pair.Value;

                if (!UpdateMember(memberTrg, dicTeamLinkC2O, dicMemberLinkO2C, ref nChangedConfigData))
                    return false;
            }

            return true;
        }

        private bool UpdateMember(CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<CompanyMember, CompanyMember> dicMemberLinkO2C, ref int nChangedConfigData)
        {
            CompanyMember memberSrc = FindCompanyMember(memberTrg, dicTeamLinkC2O);

            if (memberSrc != null)
            {
                UpdateMember(memberSrc, memberTrg, dicTeamLinkC2O, ref nChangedConfigData);
            }
            else
            {
                memberSrc = InsertNewMember(memberTrg, dicTeamLinkC2O);

                if (memberSrc == null)
                    return false;

                m_dicCompanyMember[memberSrc.MemberID] = memberSrc;
                nChangedConfigData |= (int)SDMSConfig.ConfigType.COMPANY_MEMBER;
            }

            dicMemberLinkO2C[memberSrc] = memberTrg;
            return true;
        }

        private CompanyMember InsertNewMember(CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLink)
        {
            //if (!dicTeamLink.ContainsKey(memberTrg.Team))
            //    return null;

            /*if (!IsValidJobLevelID(memberTrg.LevelID))
                memberTrg.LevelID = GetUnknownLevelID();*/

            CompanyMember member = new CompanyMember();

            member.ID = m_nMaxCompanyMemberID + 1;
            member.LevelID = memberTrg.LevelID;
            member.MemberID = memberTrg.MemberID;
            member.MemberName = memberTrg.MemberName;
            member.OfficePhoneNumber = memberTrg.OfficePhoneNumber;
            member.PhoneNumber = memberTrg.PhoneNumber;

            RegularTeam team;

            foreach (KeyValuePair<RegularTeam, int> pair in memberTrg.TeamPositions)
            {
                if (!dicTeamLink.TryGetValue(pair.Key, out team))
                    return null;

                member.TeamPositions[pair.Key] = pair.Value;
            }

            //member.PositionID = memberTrg.PositionID;
            //member.Team = dicTeamLink[memberTrg.Team];
            //member.Title = memberTrg.Title;

            string strFormat = "insert into CompanyMember (id, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber) ";
            strFormat += "values ({0}, '{1}', {2}, '{3}', '{4}', '{5}')";

            string strSQL = string.Format(strFormat, member.ID, member.MemberName, member.LevelID, member.MemberID, member.OfficePhoneNumber, member.PhoneNumber);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return null;

            if (!InsertRegularMemberList(member))
                return null;

            m_nMaxCompanyMemberID = member.ID;
            return member;
        }

        private bool InsertRegularMemberList(CompanyMember member)
        {
            foreach (KeyValuePair<RegularTeam, int> pair in member.TeamPositions)
            {
                string strFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID) values ({0}, {1}, {2})";
                string strSQL = string.Format(strFormat, pair.Key.ID, member.ID, pair.Value);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            return true;
        }

        private void UpdateMember(CompanyMember memberSrc, CompanyMember memberTrg, Dictionary<RegularTeam, RegularTeam> dicTeamLink, ref int nChangedConfigData)
        {
            bool isChanged = false;

            if (!IsValidJobLevelID(memberTrg.LevelID))
                memberTrg.LevelID = GetUnknownLevelID();

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

            int nPositionCount1 = memberSrc.TeamPositions.Count;
            int nPositionCount2 = memberTrg.TeamPositions.Count;

            if (nPositionCount1 != nPositionCount2)
                isChanged = true;
            else
            {
                RegularTeam team;
                int nPositionID;

                foreach (KeyValuePair<RegularTeam, int> pair in memberTrg.TeamPositions)
                {
                    if (!dicTeamLink.TryGetValue(pair.Key, out team))
                    {
                        isChanged = true;
                        break;
                    }

                    if (!memberSrc.TeamPositions.TryGetValue(team, out nPositionID))
                    {
                        isChanged = true;
                        break;
                    }

                    if (nPositionID != pair.Value)
                    {
                        isChanged = true;
                        break;
                    }
                }
            }

            /*if (memberSrc.PositionID != memberTrg.PositionID)
            {
                memberSrc.PositionID = memberTrg.PositionID;
                isChanged = true;
            }*/

            /*if (!dicTeamLink.ContainsKey(memberTrg.Team))
                return;

            RegularTeam teamSrc = dicTeamLink[memberTrg.Team];

            if (memberSrc.Team != teamSrc)
            {
                memberSrc.Team = teamSrc;
                isChanged = true;
            }*/

            //memberSrc.Title = memberTrg.Title;

            if (isChanged)
            {
                string strSQL = string.Format("Update CompanyMember set MemberName = '{0}', LevelID = {1}, OfficePhoneNumber = '{2}', PhoneNumber = '{3}' where id = {4}",
                    memberSrc.MemberName, memberSrc.LevelID, memberSrc.OfficePhoneNumber, memberSrc.PhoneNumber, memberSrc.ID);

                m_dbMgr.GetResultData(strSQL, 0);
                nChangedConfigData |= (int)SDMSConfig.ConfigType.COMPANY_MEMBER;

                RemoveRegularMemberListFromMemberIDs(memberSrc.ID.ToString());
                InsertRegularMemberList(memberSrc);
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

        private bool UpdateRegularTeam(CustomerDataManager customerMgr, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C, ref int nChangedConfigData)
        {
            ArrayList arrCheckedTeam = new ArrayList();
            
            foreach (KeyValuePair<string, RegularTeam> pair in customerMgr.RegularTeams)
            {
                RegularTeam teamTrg = pair.Value;
                if (!UpdateTeam(teamTrg, arrCheckedTeam, dicTeamLinkC2O, dicTeamLinkO2C, ref nChangedConfigData))
                    return false;
            }

            return true;
        }

        private bool UpdateTeam(RegularTeam teamTrg, ArrayList arrCheckedTeam, Dictionary<RegularTeam, RegularTeam> dicTeamLinkC2O, Dictionary<RegularTeam, RegularTeam> dicTeamLinkO2C, ref int nChangedConfigData)
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
                    {
                        m_dicRegularTeam[teamSrc.ID] = teamSrc;
                        nChangedConfigData |= (int)SDMSConfig.ConfigType.REGULAR_TEAM;
                    }
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

        // 최상위 팀
        private RegularTeam GetRootParentTeam()
        {
            foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeam)
            {
                RegularTeam team = pair.Value;

                if (team.ParentTeam == null)
                    return team;
            }

            return null;
        }
    }
}
