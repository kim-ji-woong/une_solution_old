using System;

namespace Dashboard.DAL
{
    using Dashboard.Model;
    using dnsDBUtil;
    using IDAL;
    using System.Collections;
    using System.Collections.Generic;

    public class CreateManager : QueryManager, ICreate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }

        public CurrentWorkPermit CreateCurrentWorkPermit(int nGeneralCnt, int nFireCnt, int nHighCnt, int nElecCnt, int nClosenessCnt, int nCraneCnt, int nDiggCnt, int nRadiCnt, int nTotalCnt, string strPlantPrcsID, DateTime dtUpdate)
        {
            Dictionary<CurrentWorkPermit.Fields, object> dicFieldDatas = new Dictionary<CurrentWorkPermit.Fields, object>();
            dicFieldDatas[CurrentWorkPermit.Fields.GENERAL_CNT] = nGeneralCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.FIRE_CNT] = nFireCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.HIGH_CNT] = nHighCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.ELEC_CNT] = nElecCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.CLOSENESS_CNT] = nClosenessCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.CRANE_CNT] = nCraneCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.DIGG_CNT] = nDiggCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.RADI_CNT] = nRadiCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.TOTAL_CNT] = nTotalCnt;
            dicFieldDatas[CurrentWorkPermit.Fields.PLANT_PRCS_ID] = strPlantPrcsID;
            dicFieldDatas[CurrentWorkPermit.Fields.UpdateTime] = dtUpdate;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                CurrentWorkPermit.TableName,
                GetFieldNames<CurrentWorkPermit.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                CurrentWorkPermit data = new CurrentWorkPermit();
                data.GENERAL_CNT = nGeneralCnt;
                data.FIRE_CNT = nFireCnt;
                data.HIGH_CNT = nHighCnt;
                data.ELEC_CNT = nElecCnt;
                data.CLOSENESS_CNT = nClosenessCnt;
                data.CRANE_CNT = nCraneCnt;
                data.DIGG_CNT = nDiggCnt;
                data.RADI_CNT = nRadiCnt;
                data.TOTAL_CNT = nTotalCnt;
                data.PLANT_PRCS_ID = strPlantPrcsID;
                data.UpdateTime = dtUpdate;

                return data;
                /*string strErrorMessage;
                List<CurrentWorkPermit> currents = m_dataManager.GetSelectManager().SelectCurrentWorkPermits(dicFieldDatas, null, out strErrorMessage);

                if (currents == null || currents.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                return currents[0];*/
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }
    }
}
