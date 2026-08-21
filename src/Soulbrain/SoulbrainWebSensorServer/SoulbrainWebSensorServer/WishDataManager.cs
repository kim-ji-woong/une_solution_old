using Dashboard.Model;
using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulbrainWebSensorServer
{
    public class WishDataManager
    {
        DirectDBManager m_wishDBManager = null;
        Dashboard.DAL.DataManager m_dashboardDataManager = null;

        //private LogManager m_logMgr = new LogManager();

        public WishDataManager(DirectDBManager wishDBManager, Dashboard.DAL.DataManager dashboardDataManager)
        {
            m_wishDBManager = wishDBManager;
            m_dashboardDataManager = dashboardDataManager;
        }

        public List<CurrentWorkPermit> ReadCurrentWorkPermitData(out string strErrorMessage)
        {
            strErrorMessage = "";
            List<CurrentWorkPermit> currentWorkPermits = new List<CurrentWorkPermit>();

            try
            {
                if (m_wishDBManager.Connect() == true) 
                {
                    string strSQL = "Select GENERAL_CNT, FIRE_CNT, HIGH_CNT, ELEC_CNT, CLOSENESS_CNT, CRANE_CNT, DIGG_CNT, RADI_CNT, TOTAL_CNT, PLANT_PRCS_ID From SWOV_CURRENT_WORK_PERMIT";
                    ArrayList arrResult = m_wishDBManager.GetResultData(strSQL);

                    if (arrResult == null)
                    {
                        m_wishDBManager.Close();
                        strErrorMessage = "WISH DB에서 SWOV_CURRENT_WORK_PERMIT 테이블을 조회 할 수 없습니다.";
                        return null;
                    }

                    int nCount = arrResult.Count;
                    DateTime dtNow = DateTime.Now;

                    for (int i = 0; i < nCount; i += 10)
                    {
                        int nGeneralCnt = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                        int nFireCnt = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                        int nHighCnt = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                        int nElecCnt = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                        int nClosenessCnt = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                        int nCraneCnt = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                        int nDiggCnt = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                        int nRadiCnt = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                        int nTotalCnt = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                        string strPlantPrcsID = WebDBManager.GetStringField(arrResult[i + 9], "");

                        CurrentWorkPermit currentWork = new CurrentWorkPermit();
                        currentWork.GENERAL_CNT = nGeneralCnt;
                        currentWork.FIRE_CNT = nFireCnt;
                        currentWork.HIGH_CNT = nHighCnt;
                        currentWork.ELEC_CNT = nElecCnt;
                        currentWork.CLOSENESS_CNT = nClosenessCnt;
                        currentWork.CRANE_CNT = nCraneCnt;
                        currentWork.DIGG_CNT = nDiggCnt;
                        currentWork.RADI_CNT = nRadiCnt;
                        currentWork.TOTAL_CNT = nTotalCnt;
                        currentWork.PLANT_PRCS_ID = strPlantPrcsID;
                        currentWork.UpdateTime = dtNow;

                        currentWorkPermits.Add(currentWork);
                    }

                    m_wishDBManager.Close();
                }

            }
            catch (Exception e)
            {
                strErrorMessage = "ReadCurrentWorkPermitData 실패(예외처리: " + e.Message + " )";
                //m_logMgr.Log_Info(strErrorMessage);
                Logger.Instance.Write(strErrorMessage);
                return null;
            }

            return currentWorkPermits;
        }
        

        public bool WriteCurrentWorkPermitData(List<CurrentWorkPermit> currentWorkPermits, out string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (currentWorkPermits == null)
                {
                    strErrorMessage = "CurrentWorkPermit 데이터가 없습니다.";
                    return false;
                }

                foreach (CurrentWorkPermit currentWork in currentWorkPermits)
                {
                    CurrentWorkPermit data = m_dashboardDataManager.GetSelectManager().SelectCurrentWorkPermit(currentWork.PLANT_PRCS_ID, out strErrorMessage);

                    // 해당 값이 없거나 
                    if (data == null)
                    {   // 조회가 되지 않는다면 추가
                        CurrentWorkPermit workPermit = m_dashboardDataManager.GetCreateManager().CreateCurrentWorkPermit(currentWork.GENERAL_CNT, currentWork.FIRE_CNT, currentWork.HIGH_CNT, currentWork.ELEC_CNT, currentWork.CLOSENESS_CNT, currentWork.CRANE_CNT, currentWork.DIGG_CNT, currentWork.RADI_CNT, currentWork.TOTAL_CNT, currentWork.PLANT_PRCS_ID, currentWork.UpdateTime);

                        //if (workPermit == null)
                        //{
                        //    strErrorMessage = "CreateCurrentWorkPermit 실패.";
                        //    return false;
                        //}
                    }
                    else
                    {   // 조회가 된다면 업데이트
                        if (m_dashboardDataManager.GetUpdateManager().UpdateCurrentWorkPermit(currentWork, out strErrorMessage) == false)
                        {
                            //return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                strErrorMessage = "ReadCurrentWorkPermitData 실패(예외처리: " + e.Message + " )";
                //m_logMgr.Log_Info(strErrorMessage);
                Logger.Instance.Write(strErrorMessage);
                return false;
            }

            return true;
        }

        public bool ReloadCurrentWorkPermitData(out string strErrorMessage)
        {
            strErrorMessage = "";

            List<CurrentWorkPermit> currentWorkPermits = ReadCurrentWorkPermitData(out strErrorMessage);

            if (currentWorkPermits == null)
                return false;

            if (WriteCurrentWorkPermitData(currentWorkPermits, out strErrorMessage) == false)
            {
                return false;
            }

            return true;
        }
    }
}
