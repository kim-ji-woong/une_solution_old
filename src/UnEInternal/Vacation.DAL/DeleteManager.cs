using System;
using System.Collections.Generic;
using dnsDBUtil;

namespace Vacation.DAL
{
    using IDAL;
    using Model;

    public class DeleteManager : QueryManager, IDeleteManager
    {
        private WebDBManager m_dbMgr = null;
        
        public DeleteManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public bool DeleteCompanyMember(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", CompanyMember.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCompanyMember(Dictionary<CompanyMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions, CompanyMember.GetFieldName, CompanyMember.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + CompanyMember.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteJobLevel(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", JobLevel.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteJobLevel(Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<JobLevel.Fields>(ref strCondition, dicConditions, JobLevel.GetFieldName, JobLevel.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + JobLevel.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteHistory(int memberID, int year, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where MemberID = {1} and Year = {2}", History.GetTableName(), memberID, year);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteHistory(Dictionary<History.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<History.Fields>(ref strCondition, dicConditions, History.GetFieldName, History.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + History.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteRegularTeam(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", RegularTeam.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteRegularTeam(Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<RegularTeam.Fields>(ref strCondition, dicConditions, RegularTeam.GetFieldName, RegularTeam.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + RegularTeam.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteRequest(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Request.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteRequest(Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Request.Fields>(ref strCondition, dicConditions, Request.GetFieldName, Request.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + Request.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteResponse(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Response.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteResponse(Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Response.Fields>(ref strCondition, dicConditions, Response.GetFieldName, Response.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + Response.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteSpecialVacationRequest(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SpecialVacationRequest.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpecialVacationRequest(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SpecialVacationRequest.Fields>(ref strCondition, dicConditions, SpecialVacationRequest.GetFieldName, SpecialVacationRequest.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + SpecialVacationRequest.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteSpecialVacationResponse(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SpecialVacationResponse.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SpecialVacationResponse.Fields>(ref strCondition, dicConditions, SpecialVacationResponse.GetFieldName, SpecialVacationResponse.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + SpecialVacationResponse.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteReservation(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", Reservation.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteReservation(Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Reservation.Fields>(ref strCondition, dicConditions, Reservation.GetFieldName, Reservation.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + Reservation.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteSpecialVacation(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", SpecialVacation.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpecialVacation(Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SpecialVacation.Fields>(ref strCondition, dicConditions, SpecialVacation.GetFieldName, SpecialVacation.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + SpecialVacation.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteOption(int id, out string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID = {1}", VacationOption.GetTableName(), id);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteOption(Dictionary<VacationOption.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<VacationOption.Fields>(ref strCondition, dicConditions, VacationOption.GetFieldName, VacationOption.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + VacationOption.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteExternalLogin(string userID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = '{2}'", ExternalLogin.GetTableName(), ExternalLogin.GetFieldName(ExternalLogin.Fields.UserID, out isNullable), userID);

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteExternalLogin(Dictionary<ExternalLogin.Fields, object> dicConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<ExternalLogin.Fields>(ref strCondition, dicConditions, ExternalLogin.GetFieldName, ExternalLogin.GetTableName(), ref strErrorMessage) == false)
                return false;

            string strSQL = "Delete from " + ExternalLogin.GetTableName();

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
