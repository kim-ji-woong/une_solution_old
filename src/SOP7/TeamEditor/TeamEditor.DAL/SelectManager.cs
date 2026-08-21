using dnsDBUtil;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.DAL
{
    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataMgr = null;

        public SelectManager(DataManager dataMgr)
        {
            m_dataMgr = dataMgr;
            m_dbManager = dataMgr.GetDBManager() as WebDBManager;
        }

        public string ReadSiteName()
        {
            return "";
        }

        public List<RegularMember> SelectRegularMembers(out string strErrorMessage)
        {
            return SelectRegularMembers(null, out strErrorMessage);
            /*List<RegularMember> listRegularMembers = new List<RegularMember>();
            strErrorMessage = "";

            string strSQL = "select ID, RegularID, MemberName, MemberID, OfficePhoneNumber, PhoneNumber, JobLevelID, JobPositionID, Email ";
            strSQL += "from SopTeamRegularMember ";
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return listRegularMembers;
            
            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nRegularMemberID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                int nCompanyID = WebDBManager.GetIntField(arrResults[i + 1].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strMemberID = WebDBManager.GetStringField(arrResults[i + 3].ToString(), "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResults[i + 4], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResults[i + 5], "");
                int nJobLevelID = WebDBManager.GetIntField(arrResults[i + 6].ToString(), -1);
                int nJobPositionID = WebDBManager.GetIntField(arrResults[i + 7].ToString(), -1);
                string strEmail = WebDBManager.GetStringField(arrResults[i + 8], "");

                RegularMember regularMember = new RegularMember();
                regularMember.ID = nRegularMemberID;
                regularMember.RegularID = nCompanyID;
                regularMember.MemberName = strMemberName;
                regularMember.MemberID = strMemberID;
                regularMember.OfficePhoneNumber = strOfficePhoneNumber;
                regularMember.PhoneNumber = strPhoneNumber;
                regularMember.Email = strEmail;

                if (nJobLevelID == -1)
                    regularMember.JobLevelID = null;
                else
                    regularMember.JobLevelID = nJobLevelID;

                if (nJobPositionID == -1)
                    regularMember.JobPositionID = null;
                else
                    regularMember.JobPositionID = nJobPositionID;

                listRegularMembers.Add(regularMember);
            }

            return listRegularMembers;*/
        }

        public List<RegularMember> SelectRegularMembers(Dictionary<RegularMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectRegularMembers(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<RegularMember> SelectRegularMembers(Dictionary<RegularMember.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            List<RegularMember> listRegularMembers = new List<RegularMember>();
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<RegularMember.Fields>(out nFieldCount), RegularMember.GetTableName());
            string strCondition = "";

            if (SetCondition<RegularMember.Fields>(ref strCondition, dicConditions, RegularMember.GetFieldName, RegularMember.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                RegularMember model = ReadRegularMember(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    listRegularMembers.Add(model);
            }

            return listRegularMembers;
        }

        public RegularMember SelectRegularMember(int nID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<RegularMember.Fields>(out nFieldCount), RegularMember.GetTableName(), nID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                RegularMember model = ReadRegularMember(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
            /*strErrorMessage = "";

            string strSQL = "select ID, RegularID, MemberName, MemberID, OfficePhoneNumber, PhoneNumber, JobLevelID, JobPositionID, Email from SopTeamRegularMember where ID =" + nID;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;
            if (nResultCount != 9)
                return null;

            int nRegularMemberID = WebDBManager.GetIntField(arrResults[0].ToString(), -1);
            int nCompanyID = WebDBManager.GetIntField(arrResults[1].ToString(), -1);
            string strMemberName = WebDBManager.GetStringField(arrResults[2], "");
            string strMemberID = WebDBManager.GetStringField(arrResults[3].ToString(), "");
            string strOfficePhoneNumber = WebDBManager.GetStringField(arrResults[4], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResults[5], "");
            int nJobLevelID = WebDBManager.GetIntField(arrResults[6].ToString(), -1);
            int nJobPositionID = WebDBManager.GetIntField(arrResults[7].ToString(), -1);
            string strEmail = WebDBManager.GetStringField(arrResults[8], "");

            RegularMember regularMember = new RegularMember();
            regularMember.ID = nRegularMemberID;
            regularMember.RegularID = nCompanyID;
            regularMember.MemberName = strMemberName;
            regularMember.MemberID = strMemberID;
            regularMember.OfficePhoneNumber = strOfficePhoneNumber;
            regularMember.PhoneNumber = strPhoneNumber;
            regularMember.Email = strEmail;

            if (nJobLevelID == -1)
                regularMember.JobLevelID = null;
            else
                regularMember.JobLevelID = nJobLevelID;

            if (nJobPositionID == -1)
                regularMember.JobPositionID = null;
            else
                regularMember.JobPositionID = nJobPositionID;
            
            return regularMember;*/
        }

        public List<RegularMember> SelectRegularMembers(string strCondition, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<RegularMember.Fields>(out nFieldCount), RegularMember.GetTableName());

            if (strCondition != null && strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<RegularMember> members = new List<RegularMember>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                RegularMember model = ReadRegularMember(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    members.Add(model);
            }

            return members;
            /*List<RegularMember> listRegularMembers = new List<RegularMember>();
            strErrorMessage = "";

            string strSQL = "select ID, RegularID, MemberName, MemberID, OfficePhoneNumber, PhoneNumber, JobLevelID, JobPositionID, Email from SopTeamRegularMember";
            if (strCondition != null && strCondition.Length > 0)
            {
                strSQL += " where " + strCondition;
            }

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return listRegularMembers;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nRegularMemberID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                int nCompanyID = WebDBManager.GetIntField(arrResults[i + 1].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strMemberID = WebDBManager.GetStringField(arrResults[i + 3].ToString(), "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResults[i + 4], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResults[i + 5], "");
                int nJobLevelID = WebDBManager.GetIntField(arrResults[i + 6].ToString(), -1);
                int nJobPositionID = WebDBManager.GetIntField(arrResults[i + 7].ToString(), -1);
                string strEmail = WebDBManager.GetStringField(arrResults[i + 8], "");

                RegularMember regularMember = new RegularMember();
                regularMember.ID = nRegularMemberID;
                regularMember.RegularID = nCompanyID;
                regularMember.MemberName = strMemberName;
                regularMember.MemberID = strMemberID;
                regularMember.OfficePhoneNumber = strOfficePhoneNumber;
                regularMember.PhoneNumber = strPhoneNumber;
                regularMember.Email = strEmail;

                if (nJobLevelID == -1)
                    regularMember.JobLevelID = null;
                else
                    regularMember.JobLevelID = nJobLevelID;

                if (nJobPositionID == -1)
                    regularMember.JobPositionID = null;
                else
                    regularMember.JobPositionID = nJobPositionID;

                listRegularMembers.Add(regularMember);
            }

            return listRegularMembers;*/
        }

        private RegularMember ReadRegularMember(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            RegularMember model = new RegularMember();
            bool isNullable;

            foreach (RegularMember.Fields field in RegularMember.Fields.GetValues(typeof(RegularMember.Fields)))
            {
                string strFieldName = RegularMember.GetFieldName(field, out isNullable);

                if (field == RegularMember.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == RegularMember.Fields.Email)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Email = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Email = str;
                }
                else if (field == RegularMember.Fields.JobLevelID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.JobLevelID = null;
                    }
                    else
                        model.JobLevelID = data.Data;
                }
                else if (field == RegularMember.Fields.JobPositionID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.JobPositionID = null;
                    }
                    else
                        model.JobPositionID = data.Data;
                }
                else if (field == RegularMember.Fields.MemberID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.MemberID = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MemberID = str;
                }
                else if (field == RegularMember.Fields.MemberName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.MemberName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MemberName = str;
                }
                else if (field == RegularMember.Fields.OfficePhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.OfficePhoneNumber = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.OfficePhoneNumber = str;
                }
                else if (field == RegularMember.Fields.PhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.OfficePhoneNumber = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PhoneNumber = str;
                }
                else if (field == RegularMember.Fields.RegularID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.RegularID = data.Data;
                }
                else if (field == RegularMember.Fields.StatusID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.StatusID = data.Data;
                }

                index++;
            }

            return model;
        }

        public List<Regular> SelectRegulars(out string strErrorMessage)
        {
            List<Regular> listRegulars = new List<Regular>();
            strErrorMessage = null;

            string strSQL = "select ID, TeamName, ParentTeamID from SopTeamRegular";
            
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return listRegulars;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);

                Regular regular = new Regular();
                regular.ID = nID;
                regular.TeamName = strTeamName;

                if (nParentTeamID == -1)
                    regular.ParentTeamID = null;
                else
                    regular.ParentTeamID = nParentTeamID;

                listRegulars.Add(regular);
            }

            return listRegulars;

        }

        public Regular SelectRegular(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Regular.Fields>(out nFieldCount), Regular.GetTableName(), id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Regular model = ReadRegular(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectRegulars(dicConditions, null, out strErrorMessage);
        }

        public List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectRegulars(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            List<Regular> listRegulars = new List<Regular>();
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Regular.Fields>(out nFieldCount), Regular.GetTableName());
            string strCondition = "";

            if (SetCondition<Regular.Fields>(ref strCondition, dicConditions, Regular.GetFieldName, Regular.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Regular model = ReadRegular(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    listRegulars.Add(model);
            }

            return listRegulars;
        }

        private Regular ReadRegular(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Regular model = new Regular();
            bool isNullable;

            foreach (Regular.Fields field in Regular.Fields.GetValues(typeof(Regular.Fields)))
            {
                string strFieldName = Regular.GetFieldName(field, out isNullable);

                if (field == Regular.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ID = data.Data;
                }
                else if (field == Regular.Fields.TeamName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TeamName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TeamName = str;
                }
                else if (field == Regular.Fields.ParentTeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.ParentTeamID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ParentTeamID = data.Data;
                }

                index++;
            }

            return model;
        }

        /*
        public List<CompanyMember> SelectExternalCompanyMembers(out string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public List<CompanyMember> SelectTemporaryEmergencyMembers(out string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public List<CompanyMember> SelectTemporaryNormalMembers(out string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public List<Company> SelectUserDefinedTeams(int nSiteID, out string strErrorMessage)
        {
            throw new NotImplementedException();
        }
        */


        public int GetMaxID(string strTableName, out string strErrorMessage, string strCondition = "")
        {
            int nID = -1;
            strErrorMessage = "";

            string strSQL = "Select max(ID) from " + strTableName;
            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return nID;
            }

            nID = arrResults.Count == 0 ? 1 : WebDBManager.GetIntField(arrResults[0].ToString(), 0) + 1;

            return nID;
        }

        public Options SelectOptions(int nID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Options.Fields>(out nFieldCount), Options.TableName, nID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Options model = ReadOptions(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Options> SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectOptions(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Options> SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            List<Options> options = new List<Options>();
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Options.Fields>(out nFieldCount), Options.TableName);
            string strCondition = "";

            if (SetCondition<Options.Fields>(ref strCondition, dicConditions, Options.GetFieldName, Options.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Options model = ReadOptions(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    options.Add(model);
            }

            return options;
        }

        private Options ReadOptions(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Options model = new Options();
            bool isNullable;

            foreach (Options.Fields field in Options.Fields.GetValues(typeof(Options.Fields)))
            {
                string strFieldName = Options.GetFieldName(field, out isNullable);

                if (field == Options.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ID = data.Data;
                }
                else if (field == Options.Fields.PropertyID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.PropertyID = data.Data;
                }
                else if (field == Options.Fields.PropertyName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PropertyName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PropertyName = str;
                }
                else if (field == Options.Fields.PropertyValue)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PropertyValue = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PropertyValue = str;
                }

                index++;
            }

            return model;
        }

        public List<Options> SelectOptions(out string strErrorMessage)
        {
            List<Options> listOptions = new List<Options>();
            strErrorMessage = "";

            string strSQL = "select ID, PropertyID, PropertyName, PropertyValue ";
            strSQL += "from SopTeamOptions ";
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return listOptions;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                int nPropertyID = WebDBManager.GetIntField(arrResults[i + 1].ToString(), -1);
                string strPropertyName = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strPropertyValue = WebDBManager.GetStringField(arrResults[i + 3], "");

                Options options = new Options();
                options.ID = nID;
                options.PropertyID = nPropertyID;
                options.PropertyName = strPropertyName;
                options.PropertyValue = strPropertyValue;

                listOptions.Add(options);
            }

            return listOptions;
        }

        public List<Options> SelectOptions(string strCondition, out string strErrorMessage)
        {
            List<Options> listOptions = new List<Options>();
            strErrorMessage = "";

            string strSQL = "select ID, PropertyID, PropertyName, PropertyValue ";
            strSQL += "from SopTeamOptions ";

            if (strCondition != null && strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return listOptions;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                int nPropertyID = WebDBManager.GetIntField(arrResults[i + 1].ToString(), -1);
                string strPropertyName = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strPropertyValue = WebDBManager.GetStringField(arrResults[i + 3], "");

                Options options = new Options();
                options.ID = nID;
                options.PropertyID = nPropertyID;
                options.PropertyName = strPropertyName;
                options.PropertyValue = strPropertyValue;

                listOptions.Add(options);
            }

            return listOptions;
        }

        public Temporary SelectTemporary(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Temporary.Fields>(out nFieldCount), Temporary.GetTableName(), id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Temporary model = ReadTemporary(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Temporary ReadTemporary(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Temporary model = new Temporary();
            bool isNullable;

            foreach (Temporary.Fields field in Temporary.Fields.GetValues(typeof(Temporary.Fields)))
            {
                string strFieldName = Temporary.GetFieldName(field, out isNullable);

                if (field == Temporary.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ID = data.Data;
                }
                else if (field == Temporary.Fields.TeamName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TeamName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TeamName = str;
                }
                else if (field == Temporary.Fields.ParentTeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.ParentTeamID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ParentTeamID = data.Data;
                }
                else if (field == Temporary.Fields.IsNormal)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.IsNormal = (data.Data == 1) ? true : false;
                }
                else if (field == Temporary.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.SiteID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SiteID = data.Data;
                }

                index++;
            }

            return model;
        }

        public List<Temporary> SelectTemporaries(Dictionary<Temporary.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectTemporaries(dicConditions, null, null, out strErrorMessage);
        }

        public List<Temporary> SelectTemporaries(Dictionary<Temporary.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            List<Temporary> listTemporarys = new List<Temporary>();
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Temporary.Fields>(out nFieldCount), Temporary.GetTableName());
            string strCondition = "";

            if (SetCondition<Temporary.Fields>(ref strCondition, dicConditions, Temporary.GetFieldName, Temporary.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Temporary model = ReadTemporary(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    listTemporarys.Add(model);
            }

            return listTemporarys;
        }

        public TemporaryMember SelectTemporaryMember(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<TemporaryMember.Fields>(out nFieldCount), TemporaryMember.GetTableName(), id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                TemporaryMember model = ReadTemporaryMember(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectTemporaryMembers(dicConditions, null, out strErrorMessage);
        }

        public List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectTemporaryMembers(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            List<TemporaryMember> listRegulars = new List<TemporaryMember>();
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<TemporaryMember.Fields>(out nFieldCount), TemporaryMember.GetTableName());
            string strCondition = "";

            if (SetCondition<TemporaryMember.Fields>(ref strCondition, dicConditions, TemporaryMember.GetFieldName, TemporaryMember.GetTableName(), ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                TemporaryMember model = ReadTemporaryMember(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    listRegulars.Add(model);
            }

            return listRegulars;
        }

        private TemporaryMember ReadTemporaryMember(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            TemporaryMember model = new TemporaryMember();
            bool isNullable;

            foreach (TemporaryMember.Fields field in TemporaryMember.Fields.GetValues(typeof(TemporaryMember.Fields)))
            {
                string strFieldName = TemporaryMember.GetFieldName(field, out isNullable);

                if (field == TemporaryMember.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ID = data.Data;
                }
                else if (field == TemporaryMember.Fields.DisplaySOPName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DisplaySOPName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplaySOPName = str;
                }
                else if (field == TemporaryMember.Fields.TeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.TeamID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TeamID = data.Data;
                }
                else if (field == TemporaryMember.Fields.RegularID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.RegularID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RegularID = data.Data;
                }
                else if (field == TemporaryMember.Fields.RegularMemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.RegularMemberID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RegularMemberID = data.Data;
                }
                else if (field == TemporaryMember.Fields.IsNormal)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsNormal = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsNormal = data.Data;
                }
                else if (field == TemporaryMember.Fields.Role)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Role = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Role = data.Data;
                }

                index++;
            }

            return model;
        }

        /// <summary>
        /// SopTeamRegular
        /// SopTeamRegularMember
        /// SopTeamTemporary
        /// SopTeamTemporaryMember
        /// </summary>
        /// <param name="teamID">SopTeamTemporary:ID</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public List<RegularmemberTemporarymember> JoinRegularMemberTemporaryMember(int temporaryID, bool isNormal, out string strErrorMessage)
        {
            strErrorMessage = "";

            StringBuilder sb = new StringBuilder();
            sb.Append("Select te.ID, te.Role, te.DisplaySOPName, te.IsNormal, te.TeamID ");
            sb.Append("     , (Select TeamName From SopTeamTemporary as t Where t.ID=te.TeamID) as TemporaryName ");
            sb.Append("     , te.RegularID, (Select TeamName From SopTeamRegular as r Where r.ID = te.RegularID) as RegularTeamName ");
            sb.Append("     , te.RegularMemberID, (Select MemberName From SopTeamRegularMember as rm Where rm.ID = te.RegularMemberID) as RegularMemberName ");
            sb.Append("  From SopTeamTemporaryMember as te ");
            sb.AppendFormat(" Where te.TeamID = {0}", temporaryID);
            sb.AppendFormat("   And te.IsNormal = {0}", (isNormal) ? 1 : 0);

            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());
            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            List<RegularmemberTemporarymember> temporaryMembers = new List<RegularmemberTemporarymember>();

            int nResultCount = arrResults.Count;
            if (nResultCount == 0)
                return temporaryMembers;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                int nTemporaryMemberID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                //string strRole = WebDBManager.GetStringField(arrResults[i + 1]);
                int? nRole = WebDBManager.GetIntField(arrResults[i + 1].ToString(), -1);
                string strDisplaySOPName = WebDBManager.GetStringField(arrResults[i + 2]);
                int nIsNormal = WebDBManager.GetIntField(arrResults[i + 3].ToString(), -1);
                int nTemporaryID = WebDBManager.GetIntField(arrResults[i + 4].ToString(), -1);
                string strTemporaryName = WebDBManager.GetStringField(arrResults[i + 5]);
                int nRegularID = WebDBManager.GetIntField(arrResults[i + 6].ToString(), -1);
                string strRegularName = WebDBManager.GetStringField(arrResults[i + 7]);
                int nRegularMemberID = WebDBManager.GetIntField(arrResults[i + 8].ToString(), -1);
                string strRegularMemberName = WebDBManager.GetStringField(arrResults[i + 9]);

                RegularmemberTemporarymember member = new RegularmemberTemporarymember();
                member.TemporaryMemberID = nTemporaryMemberID;
                member.Role = nRole == -1 ? null : nRole;
                member.DisplaySOPName = strDisplaySOPName;
                member.IsNormal = (nIsNormal == 1) ? true : false;
                member.TemporaryID = nTemporaryID;
                member.TemporaryName = strTemporaryName;
                member.RegularID = nRegularID;
                member.RegularName = strRegularName;
                member.RegularMemberID = nRegularMemberID;
                member.RegularMemberName= strRegularMemberName;

                temporaryMembers.Add(member);
            }

            return temporaryMembers;
        }
    }
}
