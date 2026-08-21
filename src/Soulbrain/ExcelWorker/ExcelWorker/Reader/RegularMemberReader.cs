using System.Collections.Generic;
using TeamEditor.DAL;
using TeamEditor.Model.Sop.Team;
using SDMS.Model.Sensor;
using dnsDBUtil;

namespace ExcelWorker.Reader
{
    using Rollback;

    public class RegularMemberReader : ExcelReader
    {
        public const string JobLevelProperty = "JobLevel";
        private static readonly string AES_key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private const string TeamNameTag = "부서";
        private const string MemberNameTag = "이름";
        private const string MemberIDTag = "사번";
        private const string PhoneNumberTag = "휴대폰";
        private const string JobLevelTag = "직급";
        private const string EmailTag = "이메일";

        private const int m_nTempTeamID = 0;

        private class RegularTeam : Regular
        {
            private List<RegularTeam> m_children = new List<RegularTeam>();
            private RegularTeam m_parentTeam = null;

            public RegularTeam ParentTeam
            {
                get { return m_parentTeam; }
                set
                {
                    if (m_parentTeam != value)
                    {
                        if (m_parentTeam != null)
                            m_parentTeam.m_children.Remove(this);

                        if (value != null)
                            value.m_children.Add(this);

                        m_parentTeam = value;
                    }
                }
            }

            // 하위 팀을 포함하여 팀원이 한명이라도 존재하는가?
            public bool HasChildMembers(Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers)
            {
                if (HasChildMembers(this, dicTeamMembers))
                    return true;

                return false;
            }

            private bool HasChildMembers(RegularTeam team, Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers)
            {
                List<RegularMember> members;

                if (dicTeamMembers.TryGetValue(team, out members) && members.Count > 0)
                    return true;

                foreach (RegularTeam childTeam in m_children)
                {
                    if (HasChildMembers(childTeam, dicTeamMembers))
                        return true;
                }

                return false;
            }
        }

        private DataManager m_teamDataManager = null;

        public RegularMemberReader(string strFilePath)
            : base(strFilePath)
        {
            WebDBManager dbMgr = (WebDBManager)m_dataManager.GetDBManager();
            m_teamDataManager = new DataManager(dbMgr.DatabaseName, (int)dbMgr.DatabaseType, dbMgr.SiteID, dbMgr.WebServerURL);
        }

        protected override bool UpdateData(List<SheetData> sheetDatas)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            Dictionary<string, RegularMember> dicIDMembers;
            Dictionary<string, RegularMember> dicPhoneNumberMembers;
            Dictionary<string, int> dicJobLevels;

            Dictionary<Regular, List<RegularMember>> dicTeamMembers = ReadDB(m_teamDataManager, out dicIDMembers, out dicPhoneNumberMembers, out dicJobLevels, out strErrorMessage);

            if (dicTeamMembers == null)
                return false;

            return CheckData(dicTeamMembers, dicIDMembers, dicPhoneNumberMembers, dicJobLevels, sheetDatas, out strErrorMessage);
        }

        private bool CheckData(Dictionary<Regular, List<RegularMember>> dicTeamMembers, Dictionary<string, RegularMember> dicIDMembers, Dictionary<string, RegularMember> dicPhoneNumberMembers, Dictionary<string, int> dicJobLevels, List<SheetData> sheetDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            Dictionary<string, int> dicColumnIndex = new Dictionary<string, int>();
            Dictionary<RegularTeam, List<RegularMember>> dicSheetRegularTeamMembers = new Dictionary<RegularTeam, List<RegularMember>>();
            Dictionary<string, RegularTeam> dicTeamPath = MakeTeamPath(dicTeamMembers);

            foreach (SheetData sheet in sheetDatas)
            {
                MakeSheetRegularTeamMembers(sheet, dicSheetRegularTeamMembers, dicTeamPath, dicIDMembers, dicPhoneNumberMembers, dicJobLevels, dicColumnIndex);
                // 첫번째 Sheet만 사용한다.
                break;
            }

            string strTeamIDs, strMemberIDs;
            GetNotDeletingList(dicSheetRegularTeamMembers, dicIDMembers, dicPhoneNumberMembers, out strTeamIDs, out strMemberIDs);

            RollbackManager rollback = new RollbackManager();

            if (DeleteRegularMembers(strMemberIDs, rollback) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            // 삭제되지 않는 직원들은 임시팀을 만든다음 임시팀 소속으로 둔다.
            if (SetTempRegularTeam(strMemberIDs, rollback) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            if (DeleteRegularTeams(strTeamIDs, rollback) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            if (AddRegularTeams(dicSheetRegularTeamMembers.Keys, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            if (UpdateRegularMembers(strMemberIDs, dicSheetRegularTeamMembers, rollback) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            // 임시팀 삭제
            if (DeleteTempRegularTeam() == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            if (AddRegularMembers(dicSheetRegularTeamMembers, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_dataManager, m_teamDataManager);
                return false;
            }

            return true;
        }

        private bool DeleteTempRegularTeam()
        {
            string strErrorMessage;
            return m_teamDataManager.GetDeleteManager().DeleteRegular(m_nTempTeamID, out strErrorMessage);
        }

        // 삭제되지 않는 직원들은 임시팀을 만든다음 임시팀 소속으로 둔다.
        private bool SetTempRegularTeam(string strMemberIDs, RollbackManager rollback)
        {
            if (strMemberIDs.Length == 0)
                return true;

            string strErrorMessage;

            Regular tempTeam = m_teamDataManager.GetSelectManager().SelectRegular(m_nTempTeamID, out strErrorMessage);

            if (strErrorMessage != null && strErrorMessage.Length > 0)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            if (tempTeam == null)
            {
                tempTeam = new Regular();
                tempTeam.ID = m_nTempTeamID;
                tempTeam.ParentTeamID = null;
                tempTeam.TeamName = "Temp";

                if (m_teamDataManager.GetCreateManager().AddRegular(tempTeam) == false)
                    return false;
                else
                {
                    List<Regular> deleteTeams = new List<Regular>();
                    deleteTeams.Add(tempTeam);

                    rollbackData.SetDeleteRegulars(deleteTeams);
                }
            }

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", RegularMember.GetFieldName(RegularMember.Fields.ID, out isNullable), strMemberIDs);
            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return false;

            List<RegularMember> updateMembers = new List<RegularMember>();
            rollbackData.SetUpdateRegularMembers(updateMembers);

            foreach (RegularMember member in members)
            {
                RegularMember updateMember = new RegularMember();
                updateMember.ID = member.ID;
                updateMember.Email = member.Email;
                updateMember.JobLevelID = member.JobLevelID;
                updateMember.JobPositionID = member.JobPositionID;
                updateMember.MemberID = member.MemberID;
                updateMember.MemberName = member.MemberName;
                updateMember.OfficePhoneNumber = member.OfficePhoneNumber;
                updateMember.PhoneNumber = member.PhoneNumber;
                updateMember.RegularID = m_nTempTeamID;

                if (m_teamDataManager.GetUpdateManager().UpdateRegularMember(updateMember, out strErrorMessage) == false)
                    return false;
                else
                    updateMembers.Add(member);
            }

            return true;
        }

        private bool UpdateRegularMembers(string strMemberIDs, Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers, RollbackManager rollback)
        {
            if (strMemberIDs.Length == 0)
                return true;

            string strErrorMessage;
            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", RegularMember.GetFieldName(RegularMember.Fields.ID, out isNullable), strMemberIDs);

            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return false;

            // BackUp을 위한 데이터
            Dictionary<int, RegularMember> dicOriginMembers = new Dictionary<int, RegularMember>();

            foreach (RegularMember member in members)
            {
                dicOriginMembers[member.ID] = member;
            }

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> updateMembers = new List<RegularMember>();
            rollbackData.SetUpdateRegularMembers(updateMembers);

            foreach (KeyValuePair<RegularTeam, List<RegularMember>> pair in dicTeamMembers)
            {
                foreach (RegularMember member in pair.Value)
                {
                    if (member.ID > 0)
                    {
                        member.RegularID = pair.Key.ID;
                        member.PhoneNumber = EncryptPhoneNumber(member.PhoneNumber);

                        if (m_teamDataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage) == false)
                            return false;
                        else
                        {
                            RegularMember _member;

                            if (dicOriginMembers.TryGetValue(member.ID, out _member))
                                updateMembers.Add(_member);
                        }
                    }
                }
            }

            return true;
        }

        private bool AddRegularMembers(Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers, RollbackManager rollback, out string strErrorMessage)
        {
            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> rollbackMembers = new List<RegularMember>();
            rollbackData.SetDeleteRegularMembers(rollbackMembers);

            int nID = m_teamDataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);
            
            foreach (KeyValuePair<RegularTeam, List<RegularMember>> pair in dicTeamMembers)
            {
                foreach (RegularMember member in pair.Value)
                {
                    if (member.ID <= 0)
                    {
                        member.ID = nID++;
                        member.RegularID = pair.Key.ID;
                        member.PhoneNumber = EncryptPhoneNumber(member.PhoneNumber);

                        if (m_teamDataManager.GetCreateManager().AddRegularMember(member) == false)
                            return false;
                        else
                            rollbackMembers.Add(member);
                    }
                }
            }

            return true;
        }

        private bool AddRegularTeams(ICollection<RegularTeam> teams, RollbackManager rollback, out string strErrorMessage)
        {
            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Regular> rollbackTeams = new List<Regular>();
            rollbackData.SetDeleteRegulars(rollbackTeams);

            int nID = m_teamDataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);
            bool complete = false;

            while (complete == false)
            {
                complete = true;

                foreach (RegularTeam team in teams)
                {
                    if (team.ID <= 0)
                    {
                        if (team.ParentTeam == null || (team.ParentTeam != null && team.ParentTeam.ID > 0))
                        {
                            team.ID = nID++;

                            if (team.ParentTeam != null)
                                team.ParentTeamID = team.ParentTeam.ID;

                            if (m_teamDataManager.GetCreateManager().AddRegular(team) == false)
                                return false;
                            else
                                rollbackTeams.Add(team);
                        }
                        else
                            complete = false;
                    }
                }
            }

            return true;
        }

        private bool DeleteRegularTeams(string strNotDeletingTeamIDs, RollbackManager rollback)
        {
            string strErrorMessage;
            List<Regular> teams = null;

            if (strNotDeletingTeamIDs.Length == 0)
                strNotDeletingTeamIDs = m_nTempTeamID.ToString();
            else
                strNotDeletingTeamIDs += "," + m_nTempTeamID.ToString();

            if (strNotDeletingTeamIDs.Length > 0)
            {
                bool isNullable;
                string strCondition = string.Format("{0} not in ({1})", Regular.GetFieldName(Regular.Fields.ID, out isNullable), strNotDeletingTeamIDs);

                teams = m_teamDataManager.GetSelectManager().SelectRegulars(null, strCondition, out strErrorMessage);
            }
            else
            {
                teams = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);
            }

            if (teams == null)
                return false;

            string strTeamIDs = "";

            foreach (Regular team in teams)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.ID.ToString();
                else
                    strTeamIDs += "," + team.ID.ToString();
            }

            if (DeleteFacilityManagers((int)FacilityManager.MemberTypes.RegularTeam, strTeamIDs, rollback) == false)
                return false;

            if (DeleteTemporaryMembers(strTeamIDs, null, rollback) == false)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Regular> rollbackTeams = new List<Regular>();
            rollbackData.SetInsertRegulars(rollbackTeams);

            foreach (Regular team in teams)
            {
                if (m_teamDataManager.GetDeleteManager().DeleteRegular(team.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackTeams.Add(team);
            }

            return true;
        }

        private bool DeleteRegularMembers(string strNotDeletingMemberIDs, RollbackManager rollback)
        {
            string strErrorMessage;
            List<RegularMember> members = null;

            if (strNotDeletingMemberIDs.Length > 0)
            {
                bool isNullable;
                string strCondition = string.Format("{0} not in ({1})", RegularMember.GetFieldName(RegularMember.Fields.ID, out isNullable), strNotDeletingMemberIDs);

                members = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);
            }
            else
            {
                members = m_teamDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);
            }

            if (members == null)
                return false;

            string strMemberIDs = "";

            foreach (RegularMember member in members)
            {
                if (strMemberIDs.Length == 0)
                    strMemberIDs = member.ID.ToString();
                else
                    strMemberIDs += "," + member.ID.ToString();
            }

            if (DeleteFacilityManagers((int)FacilityManager.MemberTypes.RegularMember, strMemberIDs, rollback) == false)
                return false;

            if (DeleteTemporaryMembers(null, strMemberIDs, rollback) == false)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> rollbackMembers = new List<RegularMember>();
            rollbackData.SetInsertRegularMembers(rollbackMembers);

            foreach (RegularMember member in members)
            {
                if (m_teamDataManager.GetDeleteManager().DeleteRegularMember(member.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }

            return true;
        }

        private bool DeleteTemporaryMembers(string strRegularTeamIDs, string strRegularMemberIDs, RollbackManager rollback)
        {
            string strCondition = "";
            bool isNullable;

            if (strRegularTeamIDs != null && strRegularTeamIDs.Length > 0)
            {
                strCondition = string.Format("{0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularID, out isNullable), strRegularTeamIDs);
            }

            if (strRegularMemberIDs != null && strRegularMemberIDs.Length > 0)
            {
                if (strCondition.Length == 0)
                    strCondition = string.Format("{0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularMemberID, out isNullable), strRegularMemberIDs);
                else
                    strCondition += string.Format(" and {0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularMemberID, out isNullable), strRegularMemberIDs);
            }

            if (strCondition.Length == 0)
                return true;

            string strErrorMessage;
            List<TemporaryMember> members = m_teamDataManager.GetSelectManager().SelectTemporaryMembers(null, strCondition, out strErrorMessage);

            if (members == null)
                return false;

            string strMemberIDs = "";

            foreach (TemporaryMember member in members)
            {
                if (strMemberIDs.Length == 0)
                    strMemberIDs = member.ID.ToString();
                else
                    strMemberIDs += "," + member.ID.ToString();
            }

            if (DeleteFacilityManagers((int)FacilityManager.MemberTypes.TemporaryMember, strMemberIDs, rollback) == false)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<TemporaryMember> rollbackMembers = new List<TemporaryMember>();
            rollbackData.SetInsertTemporaryMembers(rollbackMembers);

            foreach (TemporaryMember member in members)
            {
                if (m_teamDataManager.GetDeleteManager().DeleteTemporaryMember(member.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }

            return true;
        }

        private bool DeleteFacilityManagers(int memberType, string memberIDs, RollbackManager rollback)
        {
            if (memberIDs.Length == 0)
                return true;

            bool isNullable;
            string strErrorMessage;

            string strCondition = string.Format("{0} = {1} and {2} in ({3})",
                FacilityManager.GetFieldName(FacilityManager.Fields.MemberType, out isNullable),
                memberType,
                FacilityManager.GetFieldName(FacilityManager.Fields.MemberID, out isNullable),
                memberIDs);

            List<FacilityManager> managers = m_dataManager.GetSelectManager().SelectFacilityManagers(null, strCondition, out strErrorMessage);

            if (managers == null)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<FacilityManager> rollbackManagers = new List<FacilityManager>();
            rollbackData.SetInsertFacilityManagers(rollbackManagers);

            foreach (FacilityManager manager in managers)
            {
                if (m_dataManager.GetDeleteManager().DeleteFacilityManager(null, strCondition, out strErrorMessage) == false)
                    return false;
                else
                    rollbackManagers.Add(manager);
            }

            strCondition = string.Format("{0} = {1} and {2} in ({3})",
                BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.MemberType, out isNullable),
                memberType,
                BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.MemberID, out isNullable),
                memberIDs);

            List<BuildingFacilityManager> buildingManagers = m_dataManager.GetSelectManager().SelectBuildingFacilityManagers(null, strCondition, out strErrorMessage);

            if (buildingManagers == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<BuildingFacilityManager> rollbackBuildingManagers = new List<BuildingFacilityManager>();
            rollbackData.SetInsertBuildingFacilityManagers(rollbackBuildingManagers);

            foreach (BuildingFacilityManager manager in buildingManagers)
            {
                if (m_dataManager.GetDeleteManager().DeleteBuildingFacilityManager(null, strCondition, out strErrorMessage) == false)
                    return false;
                else
                    rollbackBuildingManagers.Add(manager);
            }

            strCondition = string.Format("{0} = {1} and {2} in ({3})",
                EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.MemberType, out isNullable),
                memberType,
                EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.MemberID, out isNullable),
                memberIDs);

            List<EquipZoneFacilityManager> equipZoneManagers = m_dataManager.GetSelectManager().SelectEquipZoneFacilityManagers(null, strCondition, out strErrorMessage);

            if (equipZoneManagers == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<EquipZoneFacilityManager> rollbackEquipZoneManagers = new List<EquipZoneFacilityManager>();
            rollbackData.SetInsertEquipZoneFacilityManagers(rollbackEquipZoneManagers);

            foreach (EquipZoneFacilityManager manager in equipZoneManagers)
            {
                if (m_dataManager.GetDeleteManager().DeleteEquipZoneFacilityManager(null, strCondition, out strErrorMessage) == false)
                    return false;
                else
                    rollbackEquipZoneManagers.Add(manager);
            }

            return true;
        }

        private void GetNotDeletingList(Dictionary<RegularTeam, List<RegularMember>> dicSheetRegularTeamMembers, Dictionary<string, RegularMember> dicIDMembers, Dictionary<string, RegularMember> dicPhoneNumberMembers, out string strTeamIDs, out string strMemberIDs)
        {
            strTeamIDs = strMemberIDs = "";
            RegularMember _member;

            Dictionary<int, int> dicNotDeletingTeamIDs = new Dictionary<int, int>();

            foreach (KeyValuePair<RegularTeam, List<RegularMember>> pair in dicSheetRegularTeamMembers)
            {
                CheckNotDeletingTeam(pair.Key, dicNotDeletingTeamIDs);

                foreach (RegularMember member in pair.Value)
                {
                    if (member.MemberID != null && member.MemberID.Length > 0)
                    {
                        if (dicIDMembers.TryGetValue(member.MemberID, out _member))
                            member.ID = _member.ID;
                    }

                    if (dicPhoneNumberMembers.TryGetValue(member.PhoneNumber, out _member))
                        member.ID = _member.ID;

                    if (member.ID > 0)
                    {
                        if (strMemberIDs.Length == 0)
                            strMemberIDs = member.ID.ToString();
                        else
                            strMemberIDs += "," + member.ID.ToString();
                    }
                }
            }

            foreach (KeyValuePair<int, int> pair in dicNotDeletingTeamIDs)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = pair.Key.ToString();
                else
                    strTeamIDs += "," + pair.Key.ToString();
            }
        }

        private void CheckNotDeletingTeam(RegularTeam team, Dictionary<int, int> dicNotDeletingTeamIDs)
        {
            if (team.ID > 0)
            {
                dicNotDeletingTeamIDs[team.ID] = team.ID;
            }

            if (team.ParentTeam != null)
                CheckNotDeletingTeam(team.ParentTeam, dicNotDeletingTeamIDs);
        }

        private Dictionary<string, RegularTeam> MakeTeamPath(Dictionary<Regular, List<RegularMember>> dicTeamMembers)
        {
            // Key : Team ID
            Dictionary<int, Regular> dicTeams = new Dictionary<int, Regular>();

            foreach (KeyValuePair<Regular, List<RegularMember>> pair in dicTeamMembers)
            {
                dicTeams[pair.Key.ID] = pair.Key;
            }

            Dictionary<int, RegularTeam> dicRegularTeams = new Dictionary<int, RegularTeam>();
            Dictionary<string, RegularTeam> dicRegularTeamPaths = new Dictionary<string, RegularTeam>();

            foreach (KeyValuePair<int, Regular> pair in dicTeams)
            {
                string strTeamPath = GetTeamPath(pair.Value, dicTeams);

                RegularTeam team = new RegularTeam();
                team.ID = pair.Value.ID;
                team.ParentTeamID = pair.Value.ParentTeamID;
                team.TeamName = pair.Value.TeamName;

                dicRegularTeamPaths[strTeamPath] = team;
                dicRegularTeams[team.ID] = team;
            }

            foreach (KeyValuePair<int, RegularTeam> pair in dicRegularTeams)
            {
                RegularTeam team = pair.Value;

                while (team.ParentTeamID != null && team.ParentTeamID > 0 && team.ParentTeam == null)
                {
                    RegularTeam parent;

                    if (dicRegularTeams.TryGetValue((int)team.ParentTeamID, out parent) == false)
                        break;

                    team.ParentTeam = parent;
                    team = parent;
                }
            }

            return dicRegularTeamPaths;
        }

        public static string GetTeamPath(Regular team, Dictionary<int, Regular> dicTeams)
        {
            string strTeamPath = team.TeamName;

            while (team.ParentTeamID != null && team.ParentTeamID > 0)
            {
                Regular parent;

                if (dicTeams.TryGetValue((int)team.ParentTeamID, out parent) == false)
                    break;

                strTeamPath = parent.TeamName + "/" + strTeamPath;
                team = parent;
            }

            return strTeamPath;
        }

        private void MakeSheetRegularTeamMembers(SheetData sheet, Dictionary<RegularTeam, List<RegularMember>> dicSheetRegularTeamMembers, Dictionary<string, RegularTeam> dicTeamPath, Dictionary<string, RegularMember> dicIDMembers, Dictionary<string, RegularMember> dicPhoneNumberMembers, Dictionary<string, int> dicJobLevels, Dictionary<string, int> dicColumnIndex)
        {
            if (dicColumnIndex.Count == 0)
            {
                foreach (KeyValuePair<int, string> pair in sheet.Titles)
                {
                    if (pair.Value.StartsWith(TeamNameTag))
                        dicColumnIndex[TeamNameTag] = pair.Key;
                    else if (pair.Value.StartsWith(MemberNameTag))
                        dicColumnIndex[MemberNameTag] = pair.Key;
                    else if (pair.Value.StartsWith(MemberIDTag))
                        dicColumnIndex[MemberIDTag] = pair.Key;
                    else if (pair.Value.StartsWith(PhoneNumberTag))
                        dicColumnIndex[PhoneNumberTag] = pair.Key;
                    else if (pair.Value.StartsWith(JobLevelTag))
                        dicColumnIndex[JobLevelTag] = pair.Key;
                    else if (pair.Value.StartsWith(EmailTag))
                        dicColumnIndex[EmailTag] = pair.Key;
                }
            }

            List<string> teamNames = GetColumnValues(TeamNameTag, sheet, dicColumnIndex);
            List<string> memberNames = GetColumnValues(MemberNameTag, sheet, dicColumnIndex);
            List<string> memberIDs = GetColumnValues(MemberIDTag, sheet, dicColumnIndex);
            List<string> phoneNumbers = GetColumnValues(PhoneNumberTag, sheet, dicColumnIndex);
            List<string> jobLevels = GetColumnValues(JobLevelTag, sheet, dicColumnIndex);
            List<string> emails = GetColumnValues(EmailTag, sheet, dicColumnIndex);

            // 엑셀파일에 같은 사람이 두번이상 기입되지 않았는지 검사
            Dictionary<string, RegularMember> dicIDMembers2 = new Dictionary<string, RegularMember>();
            Dictionary<string, RegularMember> dicPhoneNumberMembers2 = new Dictionary<string, RegularMember>();

            int nValueCount = teamNames.Count;

            for (int i=0;i<nValueCount;i++)
            {
                string strTeamPath = teamNames[i];
                string strMemberName = memberNames[i];
                string strMemberID = memberIDs[i];
                string strPhoneNumber = phoneNumbers[i];
                string strJobLevel = jobLevels[i];
                string strEmail = emails[i];

                if (strTeamPath == null || strMemberName == null || strPhoneNumber == null || strEmail == null)
                    continue;

                strTeamPath = strTeamPath.Trim();
                strMemberName = strMemberName.Trim();
                strPhoneNumber = TrimPhoneNumber(strPhoneNumber);
                strEmail = strEmail.Trim();

                if (strMemberID != null)
                    strMemberID = strMemberID.Trim();

                if (strJobLevel != null)
                    strJobLevel = strJobLevel.Trim();

                RegularTeam team = GetRegularTeam(strTeamPath, dicTeamPath, dicSheetRegularTeamMembers);
                List<RegularMember> members = dicSheetRegularTeamMembers[team];

                RegularMember member;

                if (strMemberID != null && dicIDMembers.TryGetValue(strMemberID, out member))
                {
                    // 같은 사람이 중복으로 기입되었는지 검사
                    if (dicIDMembers2.ContainsKey(strMemberID) || dicPhoneNumberMembers2.ContainsKey(strPhoneNumber))
                        continue;

                    member.RegularID = team.ID;
                    member.Email = strEmail.Length > 0 ? strEmail : null;
                    member.MemberID = strMemberID;
                    member.PhoneNumber = strPhoneNumber;

                    SetJobLevelID(member, strJobLevel, dicJobLevels);

                    members.Add(member);
                }
                else if (dicPhoneNumberMembers.TryGetValue(strPhoneNumber, out member))
                {
                    // 같은 사람이 중복으로 기입되었는지 검사
                    if (dicPhoneNumberMembers2.ContainsKey(strPhoneNumber))
                        continue;

                    member.RegularID = team.ID;
                    member.Email = strEmail.Length > 0 ? strEmail : null;
                    member.MemberID = strMemberID;
                    member.PhoneNumber = strPhoneNumber;

                    SetJobLevelID(member, strJobLevel, dicJobLevels);

                    members.Add(member);
                }
                else
                {
                    member = new RegularMember();

                    member.Email = strEmail.Length > 0 ? strEmail : null;
                    SetJobLevelID(member, strJobLevel, dicJobLevels);

                    member.MemberID = strMemberID;
                    member.MemberName = strMemberName;
                    member.PhoneNumber = strPhoneNumber;
                    member.RegularID = team.ID;

                    members.Add(member);
                }

                if (strMemberID != null)
                    dicIDMembers2[strMemberID] = member;

                dicPhoneNumberMembers2[strPhoneNumber] = member;
            }
        }

        private void SetJobLevelID(RegularMember member, string strJobLevel, Dictionary<string, int> dicJobLevels)
        {
            if (strJobLevel != null && strJobLevel.Length > 0)
            {
                int nJobLevelID;

                if (dicJobLevels.TryGetValue(strJobLevel, out nJobLevelID))
                    member.JobLevelID = nJobLevelID;
                else
                {
                    string strErrorMessage;
                    int nID = m_teamDataManager.GetSelectManager().GetMaxID(Options.TableName, out strErrorMessage) - 1;

                    if (strErrorMessage != null && strErrorMessage.Length > 0)
                        return;

                    int nID2 = m_teamDataManager.GetSelectManager().GetMaxID(Options.TableName, out strErrorMessage, "PropertyName = '" + JobLevelProperty + "'") - 1;

                    if (strErrorMessage != null && strErrorMessage.Length > 0)
                        return;

                    int nPropertyID = 0;

                    if (nID2 > 0)
                    {
                        List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions("ID = " + nID2.ToString(), out strErrorMessage);

                        if (options == null)
                            return;

                        if (options.Count > 0)
                            nPropertyID = options[options.Count - 1].PropertyID + 1;
                    }

                    Options option = new Options();

                    option.ID = nID + 1;
                    option.PropertyID = nPropertyID;
                    option.PropertyName = "JobLevel";
                    option.PropertyValue = strJobLevel;

                    if (m_teamDataManager.GetCreateManager().AddOptions(option) == false)
                        return;

                    dicJobLevels[strJobLevel] = nPropertyID;
                    member.JobLevelID = nPropertyID;
                }
            }
        }

        // 숫자 이외의 나머지 문자들을 모두 제거한다.
        private string TrimPhoneNumber(string strPhoneNumber)
        {
            string phoneNumber = "";
            int len = strPhoneNumber.Length;

            for (int i=0;i<len;i++)
            {
                char ch = strPhoneNumber[i];

                if (ch >= '0' && ch <= '9')
                {
                    phoneNumber += ch;
                }
            }

            return phoneNumber;
        }

        private RegularTeam GetRegularTeam(string strTeamPath, Dictionary<string, RegularTeam> dicTeamPath, Dictionary<RegularTeam, List<RegularMember>> dicSheetRegularTeamMembers)
        {
            RegularTeam team;

            if (dicTeamPath.TryGetValue(strTeamPath, out team))
            {
                if (dicSheetRegularTeamMembers.ContainsKey(team) == false)
                    dicSheetRegularTeamMembers[team] = new List<RegularMember>();

                int nIndex = strTeamPath.LastIndexOf('/');

                if (nIndex >= 0)
                    GetRegularTeam(strTeamPath.Substring(0, nIndex), dicTeamPath, dicSheetRegularTeamMembers);

                return team;
            }

            string[] teamNames = strTeamPath.Split('/');
            int nTeamNameCount = teamNames.Length;

            string strPrevTeamName = "";
            RegularTeam teamPrevParent = null;

            for (int i=0;i<nTeamNameCount;i++)
            {
                string teamName = teamNames[i].Trim();
                string strTeamName = strPrevTeamName + teamName;

                if (dicTeamPath.TryGetValue(strTeamName, out team) == false)
                {
                    RegularTeam _team = new RegularTeam();
                    _team.ParentTeam = teamPrevParent;
                    _team.TeamName = teamName;

                    teamPrevParent = _team;
                    dicTeamPath[strTeamName] = _team;
                    dicSheetRegularTeamMembers[_team] = new List<RegularMember>();
                }
                else
                {
                    if (dicSheetRegularTeamMembers.ContainsKey(team) == false)
                        dicSheetRegularTeamMembers[team] = new List<RegularMember>();

                    teamPrevParent = team;
                }

                strPrevTeamName = strTeamName + "/";
            }

            return teamPrevParent;
        }

        private List<string> GetColumnValues(string strTag, SheetData sheet, Dictionary<string, int> dicColumnIndex)
        {
            int nIndex;

            if (dicColumnIndex.TryGetValue(strTag, out nIndex) == false)
                return null;

            List<string> columnValues;

            if (sheet.ColumnDatas.TryGetValue(nIndex, out columnValues) == false)
                return null;

            return columnValues;
        }

        private Dictionary<Regular, List<RegularMember>> ReadDB(DataManager dataManager, out Dictionary<string, RegularMember> dicIDMembers, out Dictionary<string, RegularMember> dicPhoneNumberMembers, out Dictionary<string, int> dicJobLevels, out string strErrorMessage)
        {
            strErrorMessage = null;
            dicIDMembers = dicPhoneNumberMembers = null;
            dicJobLevels = null;

            List<Regular> teams = dataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

            if (teams == null)
                return null;

            List<RegularMember> members = dataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (members == null)
                return null;

            Dictionary<int, Regular> dicTeams = new Dictionary<int, Regular>();
            Dictionary<Regular, List<RegularMember>> dicTeamMembers = new Dictionary<Regular, List<RegularMember>>();

            foreach (Regular team in teams)
            {
                if (dicTeamMembers.ContainsKey(team) == false)
                {
                    dicTeamMembers[team] = new List<RegularMember>();
                    dicTeams[team.ID] = team;
                }
            }

            dicPhoneNumberMembers = new Dictionary<string, RegularMember>();
            dicIDMembers = new Dictionary<string, RegularMember>();

            // 이메일 중복검사용
            Dictionary<string, RegularMember> dicEmails = new Dictionary<string, RegularMember>();

            foreach (RegularMember member in members)
            {
                // 전화번호와 이메일은 필수사항이다.
                if (member.PhoneNumber == null || member.PhoneNumber.Length == 0 ||
                    member.Email == null || member.Email.Length == 0)
                    continue;

                member.PhoneNumber = TrimPhoneNumber(DecryptPhoneNumber(member.PhoneNumber));

                if (dicPhoneNumberMembers.ContainsKey(member.PhoneNumber))
                    continue;
                if (dicEmails.ContainsKey(member.Email))
                    continue;

                Regular team;

                if (dicTeams.TryGetValue(member.RegularID, out team) == false)
                    continue;

                List<RegularMember> _members;

                if (dicTeamMembers.TryGetValue(team, out _members) == false)
                    continue;

                _members.Add(member);

                if (member.MemberID != null && member.MemberID.Length > 0)
                    dicIDMembers[member.MemberID] = member;

                dicPhoneNumberMembers[member.PhoneNumber] = member;
                dicEmails[member.Email] = member;
            }

            dicJobLevels = GetJobLevels(dataManager);

            return dicTeamMembers;
        }

        public static string DecryptPhoneNumber(string strPhoneNumber)
        {
            return AES256Cipher.AES_decrypt(strPhoneNumber, AES_key);
        }

        public static string EncryptPhoneNumber(string strPhoneNumber)
        {
            return AES256Cipher.AES_encrypt(strPhoneNumber, AES_key);
        }

        // Key : 직급명
        // Value : 직급 ID
        private Dictionary<string, int> GetJobLevels(DataManager dataManager)
        {
            string strErrorMessage;
            string strCondition = "PropertyName = '" + JobLevelProperty + "'";

            List<Options> options = dataManager.GetSelectManager().SelectOptions(strCondition, out strErrorMessage);

            if (options == null)
                return null;

            Dictionary<string, int> dicJobLevels = new Dictionary<string, int>();

            foreach (Options option in options)
            {
                if (option.PropertyValue == null)
                    continue;

                string strJobLevelName = option.PropertyValue.Trim();

                if (strJobLevelName.Length > 0)
                    dicJobLevels[strJobLevelName] = option.PropertyID;
            }

            return dicJobLevels;
        }
    }
}
