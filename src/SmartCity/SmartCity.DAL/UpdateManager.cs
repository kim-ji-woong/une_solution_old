using dnsDBUtil;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SmartCity.DAL
{
    public class UpdateManager : QueryManager, IUpdateManager
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        private WebDBManager m_dbManager = null;

        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool UpdateAccountSession(AccountSession accountSession, out string strErrorMessage)
        {
            Dictionary<AccountSession.Fields, object> dicSets = new Dictionary<AccountSession.Fields, object>();
            dicSets[AccountSession.Fields.SessionKey] = accountSession.SessionKey;
            dicSets[AccountSession.Fields.AccountUserID] = accountSession.AccountUserID;
            dicSets[AccountSession.Fields.CreateDate] = accountSession.CreateDate;
            dicSets[AccountSession.Fields.UpdateDate] = accountSession.UpdateDate;

            Dictionary<AccountSession.Fields, object> dicConditions = new Dictionary<AccountSession.Fields, object>();
            dicConditions[AccountSession.Fields.ID] = accountSession.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<AccountSession.Fields>(ref strSets, dicSets, AccountSession.GetFieldName, AccountSession.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<AccountSession.Fields>(ref strCondition, dicConditions, AccountSession.GetFieldName, AccountSession.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", AccountSession.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateAccountSession(Dictionary<AccountSession.Fields, object> dicSets, Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<AccountSession.Fields>(ref strSets, dicSets, AccountSession.GetFieldName, AccountSession.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<AccountSession.Fields>(ref strCondition, dicConditions, AccountSession.GetFieldName, AccountSession.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", AccountSession.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        bool IUpdateManager.UpdateAccountUser(AccountUser accountUser, out string strErrorMessage)
        {
            Dictionary<AccountUser.Fields, object> dicSets = new Dictionary<AccountUser.Fields, object>();
            dicSets[AccountUser.Fields.UserID] = accountUser.UserID;
            dicSets[AccountUser.Fields.Password] = accountUser.Password;
            dicSets[AccountUser.Fields.NickName] = accountUser.NickName;
            dicSets[AccountUser.Fields.UserLevel] = accountUser.UserLevel;
            dicSets[AccountUser.Fields.FacilityType] = accountUser.FacilityType;

            Dictionary<AccountUser.Fields, object> dicConditions = new Dictionary<AccountUser.Fields, object>();
            dicConditions[AccountUser.Fields.ID] = accountUser.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<AccountUser.Fields>(ref strSets, dicSets, AccountUser.GetFieldName, AccountUser.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<AccountUser.Fields>(ref strCondition, dicConditions, AccountUser.GetFieldName, AccountUser.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", AccountUser.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        bool IUpdateManager.UpdateAccountUser(Dictionary<AccountUser.Fields, object> dicSets, Dictionary<AccountUser.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<AccountUser.Fields>(ref strSets, dicSets, AccountUser.GetFieldName, AccountUser.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<AccountUser.Fields>(ref strCondition, dicConditions, AccountUser.GetFieldName, AccountUser.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", AccountUser.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
