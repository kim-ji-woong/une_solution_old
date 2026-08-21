using System;

namespace Dashboard.DAL
{
    using Dashboard.Model;
    using dnsDBUtil;
    using IDAL;
    using System.Collections.Generic;

    public class DeleteManager : QueryManager, IDelete
    {
        private DataManager m_dataManager = null;

        public DeleteManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool DeleteCurrentWorkPermit(string strPlantPrcsID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}", CurrentWorkPermit.TableName, CurrentWorkPermit.GetFieldName(CurrentWorkPermit.Fields.PLANT_PRCS_ID, out isNullable), strPlantPrcsID);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCurrentWorkPermit(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<CurrentWorkPermit.Fields>(ref strCondition, dicConditions, CurrentWorkPermit.GetFieldName, CurrentWorkPermit.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", CurrentWorkPermit.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }
    }
}
