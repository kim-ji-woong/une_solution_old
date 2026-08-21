using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamEditor.IDAL;
using TeamEditor.Model;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.DAL
{
    public class CreateManager : QueryManager, ICreate
    {
        private DataManager m_dataMgr = null;

        public CreateManager(DataManager dataMgr)
        {
            m_dataMgr = dataMgr;
            m_dbManager = m_dataMgr.GetDBManager() as WebDBManager;
        }

        public bool AddRegular(Regular regular, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<Regular.Fields, object> dicFieldDatas = new Dictionary<Regular.Fields, object>();
            dicFieldDatas[Regular.Fields.ID] = regular.ID;
            dicFieldDatas[Regular.Fields.ParentTeamID] = regular.ParentTeamID;
            dicFieldDatas[Regular.Fields.TeamName] = regular.TeamName;

            int nFieldCount;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Regular.GetTableName(),
                GetFieldNames<Regular.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                return true;
                /*Regular team = m_dataMgr.GetSelectManager().SelectRegular(regular.ID, out strErrorMessage);

                if (team != null)
                    return true;*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        public bool AddRegular(Regular regular)
        {
            string strErrorMessage = "";
            return AddRegular(regular, out strErrorMessage);
        }

        public bool AddRegular(int? nID, int? nParentTeamID, string strTeamName, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<Regular.Fields, object> dicFieldDatas = new Dictionary<Regular.Fields, object>();
            dicFieldDatas[Regular.Fields.ParentTeamID] = nParentTeamID;
            dicFieldDatas[Regular.Fields.TeamName] = strTeamName;

            int nFieldCount;
            string strSQL;

            if (nID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Regular.GetTableName(),
                 GetFieldNames<Regular.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                Regular.GetTableName(),
                GetFieldNames<Regular.Fields>(out nFieldCount),
                nID,
                GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                return true;
                /*if (nID != null)
                {
                    //Regular team = m_dataMgr.GetSelectManager().SelectRegular((int)nID, out strErrorMessage);
                    Regular team = new Regular();
                    team.ID = (int)nID;
                    team.ParentTeamID = nParentTeamID;
                    team.TeamName = strTeamName;
                    
                    //if (team != null)
                    return true;
                }
                else
                {
                    List<Regular> regulars = m_dataMgr.GetSelectManager().SelectRegulars(dicFieldDatas, out strErrorMessage);

                    if (regulars == null || regulars.Count == 0)
                        return false;

                    return true;
                }*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        public RegularMember AddRegularMember(RegularMember member, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<RegularMember.Fields, object> dicFieldDatas = new Dictionary<RegularMember.Fields, object>();
            dicFieldDatas[RegularMember.Fields.ID] = member.ID;
            dicFieldDatas[RegularMember.Fields.Email] = member.Email;
            dicFieldDatas[RegularMember.Fields.JobLevelID] = member.JobLevelID;
            dicFieldDatas[RegularMember.Fields.JobPositionID] = member.JobPositionID;
            dicFieldDatas[RegularMember.Fields.MemberID] = member.MemberID;
            dicFieldDatas[RegularMember.Fields.MemberName] = member.MemberName;
            dicFieldDatas[RegularMember.Fields.OfficePhoneNumber] = member.OfficePhoneNumber;
            dicFieldDatas[RegularMember.Fields.PhoneNumber] = member.PhoneNumber;
            dicFieldDatas[RegularMember.Fields.RegularID] = member.RegularID;
            dicFieldDatas[RegularMember.Fields.StatusID] = member.StatusID;

            int nFieldCount;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                RegularMember.GetTableName(),
                GetFieldNames<RegularMember.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                string strCondition = string.Format("order by {0} desc", RegularMember.Fields.ID);
                List<RegularMember> createMembers = m_dataMgr.GetSelectManager().SelectRegularMembers(null, strCondition, out strErrorMessage);
                if (createMembers == null || createMembers.Count == 0)
                {
                    return null;
                }

                if (IsSameRegularMember(createMembers[0], member))
                    return createMembers[0];
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return null;
        }

        public bool AddRegularMember(RegularMember member)
        {
            string strErrorMessage = "";
            RegularMember createMember = AddRegularMember(member, out strErrorMessage);
            return createMember != null;
        }

        public bool AddRegularMember(int? nID, string strEmail, int? nJobLevelID, int? nJobPositionID, string strMemberID, string strMemberName, string strOfficePhoneNumber, string strPhoneNumber, int nRegularID, int nStatusID, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<RegularMember.Fields, object> dicFieldDatas = new Dictionary<RegularMember.Fields, object>();
            dicFieldDatas[RegularMember.Fields.Email] = strEmail;
            dicFieldDatas[RegularMember.Fields.JobLevelID] = nJobLevelID;
            dicFieldDatas[RegularMember.Fields.JobPositionID] = nJobPositionID;
            dicFieldDatas[RegularMember.Fields.MemberID] = strMemberID;
            dicFieldDatas[RegularMember.Fields.MemberName] = strMemberName;
            dicFieldDatas[RegularMember.Fields.OfficePhoneNumber] = strOfficePhoneNumber;
            dicFieldDatas[RegularMember.Fields.PhoneNumber] = strPhoneNumber;
            dicFieldDatas[RegularMember.Fields.RegularID] = nRegularID;
            dicFieldDatas[RegularMember.Fields.StatusID] = nStatusID;

            int nFieldCount;
            string strSQL;
            string strAdditionalConditions = null;

            if (nID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                RegularMember.GetTableName(),
                GetFieldNames<RegularMember.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                RegularMember.GetTableName(),
                GetFieldNames<RegularMember.Fields>(out nFieldCount),
                nID,
                GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                return true;
                /*if (nID != null)
                {
                    //RegularMember regularMember = m_dataMgr.GetSelectManager().SelectRegularMember((int)nID, out strErrorMessage);
                    RegularMember member = new RegularMember();
                    member.ID = (int)nID;
                    member.Email = strEmail;
                    member.JobLevelID = nJobLevelID;
                    member.JobPositionID = nJobPositionID;
                    member.MemberID = strMemberID;
                    member.MemberName = strMemberName;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;
                    member.RegularID = nRegularID;
                    member.StatusID = nStatusID;

                    //if (regularMember != null)
                        return true;
                }
                else
                {
                    List<RegularMember> regularMembers = m_dataMgr.GetSelectManager().SelectRegularMembers(dicFieldDatas, strAdditionalConditions, out strErrorMessage);

                    if (regularMembers == null || regularMembers.Count == 0)
                    {
                        return false;
                    }

                    return true;
                }*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        public bool AddTemporary(Temporary temporary, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strSQL = string.Format("Insert into SopTeamTemporary (ID, ParentTeamID, TeamName, IsNormal, SiteID) values ({0}, {1}, '{2}', {3}, {4})"
                , temporary.ID, temporary.ParentTeamID == null ? "null" : temporary.ParentTeamID.ToString(), temporary.TeamName, temporary.IsNormal ? 1 : 0, m_dbManager.SiteID);

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
                
            return true;
        }

        public bool AddTemporary(Temporary temporary)
        {
            string strErrorMessage = "";
            return AddTemporary(temporary, out strErrorMessage);
        }

        public bool AddTemporary(int? nID, int? nParentTeamID, string strTeamName, bool bIsNormal, int nSiteID, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<Temporary.Fields, object> dicFieldDatas = new Dictionary<Temporary.Fields, object>();
            dicFieldDatas[Temporary.Fields.ParentTeamID] = nParentTeamID;
            dicFieldDatas[Temporary.Fields.TeamName] = strTeamName;
            dicFieldDatas[Temporary.Fields.IsNormal] = bIsNormal ? 1 : 0;
            dicFieldDatas[Temporary.Fields.SiteID] = nSiteID;

            int nFieldCount;
            string strSQL;

            if (nID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Temporary.GetTableName(),
                GetFieldNames<Temporary.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                Temporary.GetTableName(),
                GetFieldNames<Temporary.Fields>(out nFieldCount),
                nID,
                GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                return true;
                /*if (nID != null)
                {
                    //Temporary temporary = m_dataMgr.GetSelectManager().SelectTemporary((int)nID, out strErrorMessage);
                    Temporary temporary = new Temporary();
                    temporary.ID = (int)nID;
                    temporary.ParentTeamID = nParentTeamID;
                    temporary.TeamName = strTeamName;
                    temporary.IsNormal = bIsNormal;
                    temporary.SiteID = nSiteID;

                    //if (temporary != null)
                        return true;
                }
                else
                {
                    List<Temporary> temporaries = m_dataMgr.GetSelectManager().SelectTemporaries(dicFieldDatas, out strErrorMessage);

                    if (temporaries == null || temporaries.Count == 0)
                        return false;

                    return true;
                }*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        private bool IsSameRegularMember(RegularMember data, RegularMember orgData)
        {
            if (data.Email == orgData.Email &&
                data.JobLevelID == orgData.JobLevelID &&
                data.JobPositionID == orgData.JobPositionID &&                
                data.MemberID == orgData.MemberID &&
                data.MemberName == orgData.MemberName &&
                data.OfficePhoneNumber == orgData.OfficePhoneNumber &&
                data.PhoneNumber == orgData.PhoneNumber &&
                data.RegularID == orgData.RegularID &&
                data.StatusID == orgData.StatusID)
                return true;

            return false;
        }

        private RegularMember GetRegularMember(RegularMember orgMember, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", RegularMember.Fields.ID, id);

            List<RegularMember> datas = m_dataMgr.GetSelectManager().SelectRegularMembers(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (RegularMember data in datas)
            {
                if (IsSameRegularMember(data, orgMember))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetRegularMember(orgMember, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = "직원 데이터 추가 실패";
            return null;
        }

        public bool AddTemporaryMember(TemporaryMember temporaryMember, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strSQL = string.Format("Insert into SopTeamTemporaryMember (ID, DisplaySOPName, TeamID, RegularID, RegularMemberID, IsNormal, Role) " +
                "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                temporaryMember.ID, temporaryMember.DisplaySOPName, temporaryMember.TeamID, (temporaryMember.RegularID == null) ? "Null" : temporaryMember.RegularID.ToString(), (temporaryMember.RegularMemberID == null) ? "Null" : temporaryMember.RegularMemberID.ToString(), temporaryMember.IsNormal, (temporaryMember.Role == null) ? "Null" : temporaryMember.Role.ToString());

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool AddTemporaryMember(TemporaryMember temporaryMember)
        {
            string strErrorMessage = "";
            return AddTemporaryMember(temporaryMember, out strErrorMessage);
        }

        public bool AddTemporaryMember(int? nID, string strDisplaySOPName, int nTeamID, int? nRegularID, int? nRegularMemberID, int nIsNormal, int? nRole, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<TemporaryMember.Fields, object> dicFieldDatas = new Dictionary<TemporaryMember.Fields, object>();
            dicFieldDatas[TemporaryMember.Fields.DisplaySOPName] = strDisplaySOPName;
            dicFieldDatas[TemporaryMember.Fields.TeamID] = nTeamID;
            dicFieldDatas[TemporaryMember.Fields.RegularID] = nRegularID;
            dicFieldDatas[TemporaryMember.Fields.RegularMemberID] = nRegularMemberID;
            dicFieldDatas[TemporaryMember.Fields.IsNormal] = nIsNormal;
            dicFieldDatas[TemporaryMember.Fields.Role] = nRole;

            int nFieldCount;
            string strSQL;

            if (nID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                TemporaryMember.GetTableName(),
                GetFieldNames<TemporaryMember.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                TemporaryMember.GetTableName(),
                GetFieldNames<TemporaryMember.Fields>(out nFieldCount),
                nID,
                GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                return true;
                /*if (nID != null)
                {
                    //TemporaryMember temporaryMember = m_dataMgr.GetSelectManager().SelectTemporaryMember((int)nID, out strErrorMessage);
                    TemporaryMember temporaryMember = new TemporaryMember();
                    temporaryMember.ID = (int)nID;
                    temporaryMember.DisplaySOPName = strDisplaySOPName;
                    temporaryMember.TeamID = nTeamID;
                    temporaryMember.RegularID = nRegularID;
                    temporaryMember.RegularMemberID = nRegularMemberID;
                    temporaryMember.IsNormal = nIsNormal;
                    temporaryMember.Role = nRole;

                    //if (temporaryMember != null)
                        return true;
                }
                else
                {
                    List<TemporaryMember> temporaryMembers = m_dataMgr.GetSelectManager().SelectTemporaryMembers(dicFieldDatas, out strErrorMessage);

                    if (temporaryMembers == null || temporaryMembers.Count == 0)
                        return false;

                    return true;
                }*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        public bool AddOptions(Options options, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strSQL = string.Format("Insert into SopTeamOptions (ID, PropertyID, PropertyName, PropertyValue) values ({0}, {1}, '{2}', '{3}')",
                options.ID, options.PropertyID, options.PropertyName, options.PropertyValue);

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
                
            return true;
        }

        public bool AddOptions(Options options)
        {
            string strErrorMessage = "";
            return AddOptions(options, out strErrorMessage);
        }

        public bool AddOptions(int? nID, int nPropertyID, string strPropertyName, string strPropertyValue, out string strErrorMessage)
        {
            strErrorMessage = "";

            Dictionary<Options.Fields, object> dicFieldDatas = new Dictionary<Options.Fields, object>();
            dicFieldDatas[Options.Fields.PropertyID] = nPropertyID;
            dicFieldDatas[Options.Fields.PropertyName] = strPropertyName;
            dicFieldDatas[Options.Fields.PropertyValue] = strPropertyValue;

            int nFieldCount;
            string strSQL;

            if (nID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Options.TableName,
                GetFieldNames<Options.Fields>(out nFieldCount),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                Options.TableName,
                GetFieldNames<Options.Fields>(out nFieldCount),
                nID,
                GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);
            //string strAdditionalConditions = "";

            if (arrResult != null)
            {
                return true;
                /*if (nID != null)
                {
                    //Options option = m_dataMgr.GetSelectManager().SelectOptions((int)nID, out strErrorMessage);
                    Options option = new Options();
                    option.ID = (int)nID;
                    option.PropertyID = nPropertyID;
                    option.PropertyName = strPropertyName;
                    option.PropertyValue = strPropertyValue;

                    //if (option != null)
                        return true;
                }
                else
                {
                    List<Options> options = m_dataMgr.GetSelectManager().SelectOptions(dicFieldDatas, strAdditionalConditions, out strErrorMessage);

                    if (options == null || options.Count == 0)
                        return false;

                    return true;
                }*/
            }

            strErrorMessage = m_dbManager.LastErrorMessage;
            return false;
        }

        public bool AddRegularMemberList()
        {
            //strSQL = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ";
            //strSQL += string.Format("({0}, {1}, {2}, {3}, {4})",
            //    m_member.Team.TeamID, nID,
            //    nSavePositionID);

            //if (dbMgr.GetBatchData(strSQL) == null)
            //{
            //    dbMgr.BatchRollback();
            //    return;
            //}

            return true;
        }

        public bool AddTemporaryMemberList()
        {
            //string strFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
            //strFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            //string strSQL = string.Format(strFormat, nID, strMemberName, nTeamID,
            //    isNormal ? 1 : 0,
            //    nMemberID, strTeamLeader, (int)memberType, strMemberCount,
            //    managerType == TemporaryMember.ManagerType.NONE ? "NULL" : ((int)managerType).ToString());

            return true;
        }

        public bool AddFacilityManager()
        {
            //string strInsertFormat = "Insert into FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID) ";
            //strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";

            return true;
        }
        
        public bool AddEquipZoneFacilityManager()
        {
            //string strInsertFormat = "Insert into EquipZoneFacilityManager (ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit) ";
            //strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            return true;
        }

        public bool AddBuildingFacilityManager()
        {
            //string strInsertFormat = "Insert into BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID) ";
            //strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            return true;
        }
    }
}
