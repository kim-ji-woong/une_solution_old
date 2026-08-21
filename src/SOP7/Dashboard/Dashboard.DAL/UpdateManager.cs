using System;

namespace Dashboard.DAL
{
    using Dashboard.Model;
    using dnsDBUtil;
    using IDAL;
    using System.Collections.Generic;

    public class UpdateManager : QueryManager, IUpdate
    {
        private DataManager m_dataManager = null;

        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool UpdateCurrentWorkPermit(CurrentWorkPermit currentWorkPermit, out string strErrorMessage)
        {
            Dictionary<CurrentWorkPermit.Fields, object> dicSets = new Dictionary<CurrentWorkPermit.Fields, object>();
            dicSets[CurrentWorkPermit.Fields.GENERAL_CNT] = currentWorkPermit.GENERAL_CNT;
            dicSets[CurrentWorkPermit.Fields.FIRE_CNT] = currentWorkPermit.FIRE_CNT;
            dicSets[CurrentWorkPermit.Fields.HIGH_CNT] = currentWorkPermit.HIGH_CNT;
            dicSets[CurrentWorkPermit.Fields.ELEC_CNT] = currentWorkPermit.ELEC_CNT;
            dicSets[CurrentWorkPermit.Fields.CLOSENESS_CNT] = currentWorkPermit.CLOSENESS_CNT;
            dicSets[CurrentWorkPermit.Fields.CRANE_CNT] = currentWorkPermit.CRANE_CNT;
            dicSets[CurrentWorkPermit.Fields.DIGG_CNT] = currentWorkPermit.DIGG_CNT;
            dicSets[CurrentWorkPermit.Fields.RADI_CNT] = currentWorkPermit.RADI_CNT;
            dicSets[CurrentWorkPermit.Fields.TOTAL_CNT] = currentWorkPermit.TOTAL_CNT;
            dicSets[CurrentWorkPermit.Fields.PLANT_PRCS_ID] = currentWorkPermit.PLANT_PRCS_ID;
            dicSets[CurrentWorkPermit.Fields.UpdateTime] = currentWorkPermit.UpdateTime;

            Dictionary<CurrentWorkPermit.Fields, object> dicConditions = new Dictionary<CurrentWorkPermit.Fields, object>();
            dicConditions[CurrentWorkPermit.Fields.PLANT_PRCS_ID] = currentWorkPermit.PLANT_PRCS_ID;

            return UpdateCurrentWorkPermit(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool UpdateCurrentWorkPermit(Dictionary<CurrentWorkPermit.Fields, object> dicSets, Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<CurrentWorkPermit.Fields>(ref strSets, dicSets, CurrentWorkPermit.GetFieldName, CurrentWorkPermit.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<CurrentWorkPermit.Fields>(ref strCondition, dicConditions, CurrentWorkPermit.GetFieldName, CurrentWorkPermit.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", CurrentWorkPermit.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
