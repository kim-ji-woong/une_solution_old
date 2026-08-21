using dnsDBUtil;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartCity.DAL
{
    public class CreateManager : QueryManager, ICreateManager
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        private WebDBManager m_dbManager = null;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public AccountSession CreateAccountSession(int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime? dtUpdateDate)
        {
            Dictionary<AccountSession.Fields, object> dicFieldDatas = new Dictionary<AccountSession.Fields, object>();
            dicFieldDatas[AccountSession.Fields.AccountUserID] = nAccountUserID;
            dicFieldDatas[AccountSession.Fields.SessionKey] = strSessionKey;
            dicFieldDatas[AccountSession.Fields.CreateDate] = dtCreateDate;
            dicFieldDatas[AccountSession.Fields.UpdateDate] = dtUpdateDate;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                AccountSession.TableName,
                GetFieldNames<AccountSession.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                string strErrorMessage;
                string strAdditionalConditions = "";
                List<AccountSession> accountSessions = m_dataManager.GetSelectManager().SelectAccountSessions(dicFieldDatas, strAdditionalConditions, out strErrorMessage);

                if (accountSessions == null || accountSessions.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                return accountSessions[0];
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }
    }
}
