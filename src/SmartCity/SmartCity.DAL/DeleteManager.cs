using dnsDBUtil;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartCity.DAL
{
    public class DeleteManager : QueryManager, IDeleteManager
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        private WebDBManager m_dbManager = null;

        public DeleteManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool DeleteAccountSession(int nID, out string strErrorMessage)
        {
            strErrorMessage = "";
            string tableName = AccountSession.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, nID);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteAccountSession(Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string tableName = AccountSession.TableName;
            string query = "";
            ArrayList res = null;

            string strCondition = "";

            if (SetCondition<AccountSession.Fields>(ref strCondition, dicConditions, AccountSession.GetFieldName, tableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length != 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }
    }
}
