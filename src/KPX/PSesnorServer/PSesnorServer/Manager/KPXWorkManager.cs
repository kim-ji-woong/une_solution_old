using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using JubixNetwork;

namespace PSensorServer
{
    public class KPXWorkManager
    {
        private List<WorkInfo> m_WorkList = new List<WorkInfo>();
        private SortedList<long, WorkInfo> m_dicWorks = new SortedList<long, WorkInfo>();
        
        public int BeginWork(JubixCommand cmd)
        {

            WorkInfo work = new WorkInfo();
            System.Diagnostics.Trace.WriteLine(cmd.TankID + "/ p " + cmd.PipeID);
            work.TankID = cmd.TankID;
            work.PipeID = cmd.PipeID;
            work.BeginCmdHistory = cmd.HistoryID;
            work.Begin = cmd.CreateTime;
            work.AvgCnt = 0;                       

            if( AddWork(work))
            {
                work.BeginWork();
                work.InsertWorkHistory();
                //KPXAlarmChecker.Instance.SetStableValue(work, work.Begin);
            }
            return work.WorkHistoryID;
        }

        internal int BeginWork(JubixCommand cmd, int nLinkData)
        {
            WorkInfo work = new WorkInfo();
            System.Diagnostics.Trace.WriteLine(cmd.TankID + "/ p " + cmd.PipeID);
            work.TankID = cmd.TankID;
            work.PipeID = cmd.PipeID;
            work.BeginCmdHistory = cmd.HistoryID;
            work.Begin = cmd.CreateTime;
            work.AvgCnt = 0;
            work.LinkData = nLinkData;

            if (AddWork(work))
            {
                work.BeginWork();
                work.InsertWorkHistory();
            }
            return work.WorkHistoryID;
        }

        private int BeginWork(int nWorkHistoryID)
        {
            // create workinfo
            WorkInfo work = new WorkInfo();
            // read last history
            work.WorkHistoryID = nWorkHistoryID;
            if( work.ReadLastWorkHistory())
            {
                // add workinfo
                if (AddWork(work))
                {
                    work.BeginWork();
                    work.BeginCheck = true;
                    //KPXAlarmChecker.Instance.SetStableValue(work, work.Begin);
                }
                return work.WorkHistoryID;
            }
            return -1;
        }

        public void EndWork(JubixCommand cmd)
        {
            int nTankID = cmd.TankID;
            int nPipeID = cmd.PipeID;
            WorkInfo info = RemoveWork(nTankID, nPipeID);
            if( info != null)
            {
                info.EndCmdHistory = cmd.HistoryID;
                info.End = cmd.CreateTime;

                info.EndWork();
                info.CloseWorkHistory();
            }
        }

        private bool AddWork(WorkInfo info)
        {
            long nID = DBUtil.ToLong(info.TankID, info.PipeID);
            if (!m_dicWorks.ContainsKey(nID))
            {
                m_WorkList.Add(info);
                m_dicWorks.Add(nID, info);
                return true;
            }   
            return false;

        }

        private WorkInfo RemoveWork(int nTankID, int nPipeID)
        {
            WorkInfo info = null;
            
            long nID = DBUtil.ToLong(nTankID, nPipeID);

            if (m_dicWorks.ContainsKey(nID))
            {
                info = m_dicWorks[nID];
                m_dicWorks.Remove(nID);
                m_WorkList.Remove(info);
            }
            return info;
        }

        // 시작시 한번 호출하여 이전 작업을 로드할 것
        public void ReadAllWorkHistory(List<TankInfo> tankList)
        {
            foreach(TankInfo tank in tankList)
            {
                int nTankID = tank.ID;
                ArrayList arHistories = ReadWorkHistories(nTankID);
                if (arHistories.Count > 0)
                {
                    foreach(int nHistoryID in arHistories)
                    {
                        if (BeginWork(nHistoryID) > 0)
                        {
                            tank.PrevHistoryID = nHistoryID;
                        }   
                    }                    
                }
            }
        }
            

        private void ReadStableValue(WorkInfo info)
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;

            string szTemp = "SELECT StandardPressure, StandardFlow FROM workhistory where TankID = {0} order by BeginTime DESC limit 1";
            string szSQL = string.Format(szTemp, info.TankID);
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return;


            float fPressure = WebDBManager.GetFloatField(arResult[0].ToString(), -1);
            float fFlow = WebDBManager.GetFloatField(arResult[0].ToString(), -1);

            info.StableFlow = fFlow;
            info.StablePressure = fPressure;
            DateTime dt = DateTime.Now;
            info.StablePressureTime = dt;
            info.StableFlowTime = dt;
        }

        private ArrayList ReadWorkHistories(int nTankID)
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            ArrayList arHistories = new ArrayList();
            string szTemp = "SELECT ID FROM workhistory where EndTime is NULL and TankID = {0} order by BeginTime DESC";
            string szSQL = string.Format(szTemp, nTankID);
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return arHistories;
           
            for(int i = 0 ; i < arResult.Count; i++)
            {
                int nHistoryID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                if( nHistoryID >= 0)
                {
                    arHistories.Add(nHistoryID);
                }
            }
            return arHistories;
        }

        private int ReadWorkHistory(int nTankID)
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;

            string szTemp = "SELECT ID FROM workhistory where EndTime is NULL and TankID = {0} order by BeginTime DESC limit 1";
            string szSQL = string.Format(szTemp, nTankID);
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return -1;

            int nHistoryID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
            return nHistoryID;
        }

        internal List<WorkInfo> GetAllWorks()
        {
            List<WorkInfo> workList = new List<WorkInfo>(m_WorkList);
            return workList;
        }

        internal bool IsWorkTank(int nTankID, int nPipeID)
        {
            long nID = DBUtil.ToLong(nTankID, nPipeID);
            if (m_dicWorks.ContainsKey(nID))
                return true;
            return false;
        }

        internal bool FindWorkTank(int nTankID)
        {
            List<WorkInfo> WorkList = new List<WorkInfo>(m_WorkList);
            foreach (WorkInfo work in WorkList)
            {
                if (work.TankID == nTankID)
                    return true;
            }
            return false;
        }

        internal WorkInfo GetWork(int nTankID, int nPipeID)
        {
            List<WorkInfo> WorkList = new List<WorkInfo>(m_WorkList);

            foreach (WorkInfo work in WorkList)
            {
                if (work.TankID == nTankID && work.PipeID == nPipeID)
                    return work;
            }
            return null;
        }

        internal List<WorkInfo> GetWorks(int nTankID)
        {
            List<WorkInfo> WorkList = new List<WorkInfo>(m_WorkList);
            List<WorkInfo> result = new List<WorkInfo>();
            foreach (WorkInfo work in WorkList)
            {
                if (work.TankID == nTankID)
                {
                    result.Add(work);
                }
            }
            return result;
        }

        internal WorkInfo GetWork(int nWorkHistoryID)
        {
            List<WorkInfo> WorkList = new List<WorkInfo>(m_WorkList);

            foreach (WorkInfo work in WorkList)
            {
                if (work.WorkHistoryID == nWorkHistoryID)
                    return work;
            }
            return null;
        }
    }

    
}
