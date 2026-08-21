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
    public class UpdateManager : QueryManager, IUpdate
    {
        private DataManager m_dataMgr = null;

        public UpdateManager(DataManager dataMgr)
        {
            m_dataMgr = dataMgr;
            m_dbManager = m_dataMgr.GetDBManager() as WebDBManager;
        }

        public bool UpdateRegularMember(RegularMember member, out string strErrorMessage)
        {
            strErrorMessage = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("Update SopTeamRegularMember SET ");
            sb.AppendFormat("RegularID = {0} ", member.RegularID);
            sb.AppendFormat(", MemberName = '{0}' ", member.MemberName);
            sb.AppendFormat(", MemberID = {0} ", (member.MemberID == null) ? "null" : "'" + member.MemberID + "'");
            sb.AppendFormat(", OfficePhoneNumber = {0} ", (member.OfficePhoneNumber == null) ? "null" : "'" + member.OfficePhoneNumber + "'");
            sb.AppendFormat(", PhoneNumber = {0} ", (member.PhoneNumber == null) ? "null" : "'" + member.PhoneNumber + "'");
            sb.AppendFormat(", JobLevelID = {0} ", (member.JobLevelID == null) ? "null" : member.JobLevelID.ToString());
            sb.AppendFormat(", JobPositionID = {0} ", (member.JobPositionID == null) ? "null" : member.JobPositionID.ToString());
            sb.AppendFormat(", Email = {0} ", (member.Email == null) ? "null" : "'" + member.Email + "'");
            sb.AppendFormat(", StatusID = {0} ", member.StatusID);
            sb.AppendFormat(" where ID = {0}", member.ID);

            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateRegularMember(Dictionary<RegularMember.Fields, object> dicSets, Dictionary<RegularMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<RegularMember.Fields>(ref strSets, dicSets, RegularMember.GetFieldName, RegularMember.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<RegularMember.Fields>(ref strCondition, dicConditions, RegularMember.GetFieldName, RegularMember.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + RegularMember.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateRegular(Dictionary<Regular.Fields, object> dicSets, Dictionary<Regular.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<Regular.Fields>(ref strSets, dicSets, Regular.GetFieldName, Regular.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<Regular.Fields>(ref strCondition, dicConditions, Regular.GetFieldName, Regular.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + Regular.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateRegular(Regular regular, out string strErrorMessage)
        {
            strErrorMessage = "";
            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Update SopTeamRegular SET TeamName = '{0}', ParentTeamID = {1} where ID = {2}", regular.TeamName, regular.ParentTeamID, regular.ID);
            sb.Append("Update SopTeamRegular SET ");
            sb.AppendFormat("TeamName = '{0}' ", regular.TeamName);
            sb.AppendFormat(", ParentTeamID = {0} ", (regular.ParentTeamID == null) ? "null" : regular.ParentTeamID.ToString());
            sb.AppendFormat(" where ID = {0}", regular.ID);

            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateTemporary(Temporary temporary, out string strErrorMessage)
        {
            strErrorMessage = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("Update SopTeamTemporary");
            sb.AppendFormat(" SET TeamName = '{0}'", temporary.TeamName);
            //sb.AppendFormat("   , ParentTeamID = {0}", temporary.ParentTeamID);
            sb.AppendFormat("   , ParentTeamID = {0}", (temporary.ParentTeamID == null) ? "null" : temporary.ParentTeamID.ToString());
            sb.AppendFormat("   , IsNormal = {0} ", (temporary.IsNormal == true) ? 1 : 0);
            sb.AppendFormat("   , SiteID = {0} ", temporary.SiteID);
            sb.AppendFormat("where ID = {0}", temporary.ID);

            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());
            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateTemporary(Dictionary<Temporary.Fields, object> dicSets, Dictionary<Temporary.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<Temporary.Fields>(ref strSets, dicSets, Temporary.GetFieldName, Temporary.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<Temporary.Fields>(ref strCondition, dicConditions, Temporary.GetFieldName, Temporary.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + Temporary.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateTemporaryMember(TemporaryMember temporaryMember, out string strErrorMessage)
        {
            //if (m_teamType == TeamTreeView.TeamType.REGULAR)
            //    strSQL = "Update RegularTeam set " + strSQL + " where ID = " + m_team.TeamID.ToString();

            strErrorMessage = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update SopTeamTemporaryMember SET DisplaySOPName = '{0}', TeamID = {1}, RegularID = {2}, RegularMemberID = {3}, IsNormal = {4}, Role = {5} where ID = {6}", 
                (temporaryMember.DisplaySOPName == null) ? "null" : temporaryMember.DisplaySOPName, 
                temporaryMember.TeamID, 
                (temporaryMember.RegularID == null) ? "Null" : temporaryMember.RegularID.ToString(), 
                (temporaryMember.RegularMemberID == null) ? "Null" : temporaryMember.RegularMemberID.ToString(), 
                temporaryMember.IsNormal, 
                (temporaryMember.Role == null) ? "Null" : temporaryMember.Role.ToString(), 
                temporaryMember.ID);

            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateTemporaryMember(Dictionary<TemporaryMember.Fields, object> dicSets, Dictionary<TemporaryMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCondition = "";
            string strSets = "";

            if (SetData<TemporaryMember.Fields>(ref strSets, dicSets, TemporaryMember.GetFieldName, TemporaryMember.GetTableName(), ref strErrorMessage) == false)
                return false;
            if (SetCondition<TemporaryMember.Fields>(ref strCondition, dicConditions, TemporaryMember.GetFieldName, TemporaryMember.GetTableName(), ref strErrorMessage) == false)
                return false;

            if (strSets.Length == 0)
                return false;

            string strSQL = "Update " + TemporaryMember.GetTableName() + " set " + strSets;

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSQL(string strSQL, out string strErrorMessage)
        {
            strErrorMessage = "";

            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOptions(Options options, out string strErrorMessage)
        {
            strErrorMessage = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update SopTeamOptions SET PropertyID = {0}, PropertyName = '{1}', PropertyValue = '{2}' where ID = {3}", 
                options.PropertyID, options.PropertyName, options.PropertyValue, options.ID);
            ArrayList arrResults = m_dbManager.GetResultData(sb.ToString());

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
