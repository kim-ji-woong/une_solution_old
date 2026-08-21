using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;


namespace PSensorServer
{
    public class WorkInfo
    {
        private int m_nTankID = -1;
        private int m_nPipeID = -1;
        private DateTime m_dtBegin;
        private DateTime m_dtEnd;
        private bool m_bWorkEnd;

        private float m_fAvgFlow = 0.0f;
        private float m_fAvgPressure = 0.0f;
        private float m_fMaxPressure = 0.0f;
        private float m_fMaxFlow = 0.0f;
        private float m_fMinPressure = 0.0f;
        private float m_fMinFlow = 0.0f;
        private int m_nAvgCnt = 0;

        private int m_nBeginCmdHistory;
        private int m_nEndCmdHistory;

        private int m_nLastWorkHistoryID = -1;

        // 최근 데이터를 이용해서 로드하는 경우 처음부터 사용
        // SaveWorkHistory이후에는 데이터가 입력됨
        private int m_nWorkHistoryID = -1;
        public int WorkHistoryID
        {
            get { return m_nWorkHistoryID; }
            set { m_nWorkHistoryID = value; }
        }

        private float m_fCurrentPressure;
        private float m_fCurrentFlow;

        private float m_fPrevPressure;
        private float m_fPrevFlow;

        private int m_nLinkData = 0;
        public int LinkData
        {
            get { return m_nLinkData; }
            set { m_nLinkData = value; }
        }

        public WorkInfo()
        {
        }

        public void AddSensorValue(float fPressure, float fFlow)
        {

            m_fCurrentFlow = fFlow;
            m_fCurrentPressure = fPressure;

            if( fPressure == -999.0f)
            {
                m_fCurrentPressure = 0.0f;
            }
            if (fFlow == -999.0f)
            {
                m_fCurrentFlow = 0.0f;
            }
            
            CalcAverage();

            UpdateWorkHistory();
            UpdateLastWorkHistory();
        }

        private void CalcAverage()
        {
            float fSumPressure = m_fAvgPressure * m_nAvgCnt;
            float fSumFlow = m_fAvgFlow * m_nAvgCnt;

            m_nAvgCnt++;           

            // 압력값 평균계산
            fSumPressure += m_fCurrentPressure;
            m_fAvgPressure = fSumPressure / m_nAvgCnt;
            if (m_fCurrentPressure > m_fMaxPressure)
                m_fMaxPressure = m_fCurrentPressure;

            if (m_fCurrentPressure < m_fMinPressure)
                m_fMinPressure = m_fCurrentPressure;

            // 유량값 평균계산
            fSumFlow += m_fCurrentFlow;
            m_fAvgFlow = fSumFlow / m_nAvgCnt;
            if (m_fCurrentFlow > m_fMaxFlow)
                m_fMaxFlow = m_fCurrentFlow;

            if (m_fCurrentFlow < m_fMinFlow)
                m_fMinFlow = m_fCurrentFlow;
        }
        
        // 작업 시작에관한 정보
        public void BeginWork()
        {
            // LastWorkHistory에 작업에 대한 ID를 얻어온다.
            m_nLastWorkHistoryID = ReadLastWorkHistoryID();

            BeginLastWorkHistory();
        }

        // 작업 종료에 관한 정보
        public void EndWork()
        {
            m_bWorkEnd = true;
            EndLastWorkHistory();
        }

        private int ReadLastWorkHistoryID()
        {
            string szTemp = "SELECT ID FROM lastworkhistory WHERE TankID = {0} and PipeID {1}";
            string szPipeID = m_nPipeID > 0 ? "=" + m_nPipeID.ToString() : "is NULL";
            string szSQL = string.Format(szTemp, m_nTankID, szPipeID);

            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                return nID;
            }  
            else
            {
                int nID = CreateLastWorkHistory();
                return nID;
            }
        }

        //private int CreateLastWorkHistory()
        //{
        //    WebDBManager dbManager = KPXServerManager.Instance.DBManager;
        //    int nMaxID = DBUtil.GetMaxID("lastworkhistory",dbManager) + 1;
        //    string szTemp = "INSERT INTO lastworkhistory (ID, PipeID, TankID, BeginTime, AvgPressure, MinPressure, MaxPressure) " +
        //        " value ({0}, {1}, {2}, now(), 0, 0, 0)";

        //    string szPipeID = m_nPipeID > 0 ? m_nPipeID.ToString() : "NULL";
        //    string szSQL = string.Format(szTemp, nMaxID, szPipeID, m_nTankID);
        //    dbManager.GetResultData(szSQL, 0);
        //    return nMaxID;
        //}

        private int CreateLastWorkHistory()
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            int nMaxID = DBUtil.GetMaxID("lastworkhistory", dbManager) + 1;
            string szTemp = "INSERT INTO lastworkhistory (ID, PipeID, TankID, BeginTime, AvgPressure, MinPressure, MaxPressure, AnotherLink) " +
                " value ({0}, {1}, {2}, '{4}', 0, 0, 0, {3})";

            string szBeginDate = WebDBManager.MakeDateTimeString(m_dtBegin);

            string szPipeID = m_nPipeID > 0 ? m_nPipeID.ToString() : "NULL";
            string szSQL = string.Format(szTemp, nMaxID, szPipeID, m_nTankID, m_nLinkData, szBeginDate);
            dbManager.GetResultData(szSQL, 0);
            return nMaxID;
        }
   
        // 작업을 DB에 추가
        //public void InsertWorkHistory()
        //{ 
        //    WebDBManager dbManager = KPXServerManager.Instance.DBManager;
        //    int nMaxID = DBUtil.GetMaxID("WorkHistory", dbManager) + 1;

        //    string szBeginDate = WebDBManager.MakeDateTimeString(m_dtBegin); 

        //    // Save workhistory
        //    string szTemp = "INSERT INTO WorkHistory (ID, TankID, PipeID, BeginTime, EndTime, AvgPressure, MinPressure, MaxPressure, AvgCnt," +
        //                    " AvgFlow, MinFlow, MaxFlow, BeginCmdHistoryID, EndCmdHistoryID) " +
        //                    " VALUES ( {0}, {11}, {1}, '{2}', NULL, {3}, {4}, {5}, {6}, {7},{8},{9},{10}, -2)";
        //    string szPipeID = m_nPipeID > 0 ? m_nPipeID.ToString() : "NULL";
        //    string szSQL = string.Format(szTemp, nMaxID, szPipeID, szBeginDate, m_fAvgPressure, m_fMinPressure, m_fMaxPressure, 
        //        m_nAvgCnt, m_fAvgFlow, m_fMinFlow, m_fMaxFlow, m_nBeginCmdHistory, m_nTankID);

        //    ArrayList arResult = dbManager.GetResultData(szSQL, 0);
        //    if( arResult != null)
        //    {
        //        m_nWorkHistoryID = nMaxID;    
        //    }            
        //}

        public void InsertWorkHistory()
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            int nMaxID = DBUtil.GetMaxID("WorkHistory", dbManager) + 1;

            string szBeginDate = WebDBManager.MakeDateTimeString(m_dtBegin);

            // Save workhistory
            string szTemp = "INSERT INTO WorkHistory (ID, TankID, PipeID, BeginTime, EndTime, AvgPressure, MinPressure, MaxPressure, AvgCnt," +
                            " AvgFlow, MinFlow, MaxFlow, BeginCmdHistoryID, EndCmdHistoryID, AnotherLink) " +
                            " VALUES ( {0}, {11}, {1}, '{2}', NULL, {3}, {4}, {5}, {6}, {7},{8},{9},{10}, -2, {12})";
            string szPipeID = m_nPipeID > 0 ? m_nPipeID.ToString() : "NULL";
            string szSQL = string.Format(szTemp, nMaxID, szPipeID, szBeginDate, m_fAvgPressure, m_fMinPressure, m_fMaxPressure,
                m_nAvgCnt, m_fAvgFlow, m_fMinFlow, m_fMaxFlow, m_nBeginCmdHistory, m_nTankID, m_nLinkData);

            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult != null)
            {
                m_nWorkHistoryID = nMaxID;
            }
        }

        // 작업 종료시간 및 평균값,CmdHistory를 저장
        public void CloseWorkHistory()
        {
            string szEndDate = WebDBManager.MakeDateTimeString(m_dtEnd);

            string szTemp = "UPDATE workhistory SET " +
                   " EndTime = '{0}', AvgPressure = {1}, MinPressure = {2}, MaxPressure = {3}, " +
                   " AvgFlow = {4}, MinFlow = {5}, MaxFlow = {6}, AvgCnt = {7},EndCmdHistoryID={8} " +
                   " WHERE ID = {9}";

            string szSQL = string.Format(szTemp, szEndDate, m_fAvgPressure, m_fMinPressure, m_fMaxPressure, m_fAvgFlow, m_fMinFlow, 
                        m_fMaxFlow, m_nAvgCnt, m_nEndCmdHistory, m_nWorkHistoryID);

            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            dbManager.GetResultData(szSQL, 0);           
        }

        private void UpdateWorkHistory()
        {
            //if(m_nWorkHistoryID > 0)
            {
                string szTemp = "UPDATE workhistory SET " +
                    " AvgPressure = {0}, MinPressure = {1}, MaxPressure = {2}, " +
                    " AvgFlow = {3}, MinFlow = {4}, MaxFlow = {5}, AvgCnt = {6} " +
                    " WHERE ID = {7}";

                string szSQL = string.Format(szTemp, m_fAvgPressure, m_fMinPressure, m_fMaxPressure, m_fAvgFlow, 
                                m_fMinFlow, m_fMaxFlow, m_nAvgCnt, m_nWorkHistoryID);

                WebDBManager dbManager = KPXServerManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
            }
        }

        // 프로그램 시작시 이전에 작업중이던 내용을 다시 로드 하는 함수
        public bool ReadLastWorkHistory()
        {
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            //string szTemp = "SELECT TankID, PipeID, BeginTime, AvgPressure, MinPressure, MaxPressure, " +
            //                " AvgFlow, MinFlow, MaxFlow, AvgCnt, BeginCmdHistoryID, StandardFlow, StandardPressure FROM workhistory WHERE ID = {0}";


            string szTemp = "SELECT  wh.TankID, wh.PipeID, wh.BeginTime, wh.AvgPressure, wh.MinPressure, wh.MaxPressure, wh.AvgFlow, wh.MinFlow, wh.MaxFlow,  " +
                            " wh.AvgCnt, wh.BeginCmdHistoryID, lwh.StandardFlow, lwh.StandardPressure, lwh.StandardFlowUpdateTime, lwh.StandardPressureUpdateTime, wh.AnotherLink FROM workhistory as wh " +
                            " INNER JOIN LastworkHistory as lwh ON wh.TankID = lwh.TankID AND wh.BeginTime = lwh.BeginTime WHERE wh.ID = {0}";

            string szSQL = string.Format(szTemp, m_nWorkHistoryID);
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                m_nTankID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                m_nPipeID = WebDBManager.GetIntField(arResult[1].ToString(), -1);

                m_dtBegin = WebDBManager.GetDateTimeField(arResult[2], DateTime.Now);
                
                m_fAvgPressure = WebDBManager.GetFloatField(arResult[3].ToString(), 0.0f);
                m_fMinPressure = WebDBManager.GetFloatField(arResult[4].ToString(), 99999.0f);
                m_fMaxPressure = WebDBManager.GetFloatField(arResult[5].ToString(), -99999.0f);    

                m_fAvgFlow = WebDBManager.GetFloatField(arResult[6].ToString(), 0.0f);
                m_fMinFlow = WebDBManager.GetFloatField(arResult[7].ToString(), 99999.0f);
                m_fMaxFlow = WebDBManager.GetFloatField(arResult[8].ToString(), -99999.0f);                
               
                m_nAvgCnt = WebDBManager.GetIntField(arResult[9].ToString(), 0);
                m_nBeginCmdHistory = WebDBManager.GetIntField(arResult[10].ToString(), -1);

                m_fStableFlow = WebDBManager.GetFloatField(arResult[11].ToString(), 0.0f);
                m_fStablePressure = WebDBManager.GetFloatField(arResult[12].ToString(), 0.0f);

                this.dtFlowUpdateTime = WebDBManager.GetDateTimeField(arResult[13].ToString(), DateTime.Now);
                this.dtPressureUpdateTime = WebDBManager.GetDateTimeField(arResult[14].ToString(), DateTime.Now);

                m_nLinkData = WebDBManager.GetIntField(arResult[15].ToString(), 0);

                return true;
            }

            return false;
        }

        //private void BeginLastWorkHistory()
        //{
        //    //if (m_nLastWorkHistoryID > 0)
        //    {
        //        string szBeginDate = WebDBManager.MakeDateTimeString(m_dtBegin);

        //        string szTemp = "UPDATE LastWorkHistory SET BeginTime = '{0}', EndTime = NULL, MaxPressure = 0, AvgPressure = 0, MinPressure = 0, "+
        //            "  AvgFlow =0, MinFlow = 0, MaxFlow = 0, AvgCnt = 0, EndCmdHistoryID= -2, StandardFlow= -9999, StandardPressure=-9999 WHERE ID = {1}";
                
        //        string szSQL = string.Format(szTemp, szBeginDate, m_nLastWorkHistoryID);
        //        WebDBManager dbManager = KPXServerManager.Instance.DBManager;
        //        dbManager.GetResultData(szSQL, 0);
        //    }
        //}

        private void BeginLastWorkHistory()
        {
            //if (m_nLastWorkHistoryID > 0)
            {
                string szBeginDate = WebDBManager.MakeDateTimeString(m_dtBegin);

                string szTemp = "UPDATE LastWorkHistory SET BeginTime = '{0}', EndTime = NULL, MaxPressure = 0, AvgPressure = 0, MinPressure = 0, " +
                    "  AvgFlow =0, MinFlow = 0, MaxFlow = 0, AvgCnt = 0, EndCmdHistoryID= -2 ,StandardFlow= -9999, StandardPressure=-9999 , AnotherLink={2} "+
                    " WHERE ID = {1} And EndTime is not NULL";

                string szSQL = string.Format(szTemp, szBeginDate, m_nLastWorkHistoryID, m_nLinkData);
                WebDBManager dbManager = KPXServerManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
            }
        }

        private void EndLastWorkHistory()
        {
            string szEndDate = WebDBManager.MakeDateTimeString(m_dtEnd);

            string szTemp = "UPDATE lastworkhistory SET " +
                   " EndTime = '{0}', AvgPressure = {1}, MinPressure = {2}, MaxPressure = {3}, " +
                   " AvgFlow = {4}, MinFlow = {5}, MaxFlow = {6}, AvgCnt = {7},EndCmdHistoryID={8}, StandardFlow= -9999, StandardPressure=-9999  " +
                   " WHERE ID = {9}";

            string szSQL = string.Format(szTemp, szEndDate, m_fAvgPressure, m_fMinPressure, m_fMaxPressure, m_fAvgFlow, m_fMinFlow,
                        m_fMaxFlow, m_nAvgCnt, m_nEndCmdHistory, m_nLastWorkHistoryID);
            WebDBManager dbManager = KPXServerManager.Instance.DBManager;
            dbManager.GetResultData(szSQL, 0);
        }

        private void UpdateLastWorkHistory()
        {
            //if (m_nLastWorkHistoryID > 0)
            {
                string szTemp = "UPDATE lastworkhistory SET " +
                    " AvgPressure = {0}, MinPressure = {1}, MaxPressure = {2}, " +
                    " AvgFlow = {3}, MinFlow = {4}, MaxFlow = {5}, AvgCnt = {6} " +
                    " WHERE ID = {7}";

                string szSQL = string.Format(szTemp, m_fAvgPressure, m_fMinPressure, m_fMaxPressure, m_fAvgFlow,
                                m_fMinFlow, m_fMaxFlow, m_nAvgCnt, m_nLastWorkHistoryID);
                WebDBManager dbManager = KPXServerManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
            }
        }        
                
        public DateTime Begin
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime End
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public bool WorkEnd
        {
            get { return m_bWorkEnd; }
        }

        public float AvgPressure
        {
            get { return m_fAvgPressure; }
            set { m_fAvgPressure = value; }
        }

        public float AvgFlow
        {
            get { return m_fAvgFlow; }
            set { m_fAvgFlow = value; }
        }

        public float MaxPressure
        {
            get { return m_fMaxPressure; }
            set { m_fMaxPressure = value; }
        }

        public float MaxFlow
        {
            get { return m_fMaxFlow; }
            set { m_fMaxFlow = value; }
        }

        public float MinPressure
        {
            get { return m_fMinPressure; }
            set { m_fMinPressure = value; }
        }

        public float MinFlow
        {
            get { return m_fMinFlow; }
            set { m_fMinFlow = value; }
        }

        public int AvgCnt
        {
            get { return m_nAvgCnt; }
            set { m_nAvgCnt = value; }
        }

        public int BeginCmdHistory
        {
            get { return m_nBeginCmdHistory; }
            set { m_nBeginCmdHistory = value; }
        }

        public int EndCmdHistory
        {
            get { return m_nEndCmdHistory; }
            set { m_nEndCmdHistory = value; }
        }

        public int TankID
        {
            get { return m_nTankID; }
            set { m_nTankID = value; }
        }
        public int PipeID
        {
            get { return m_nPipeID; }
            set { m_nPipeID = value; }
        }


        private float m_fStablePressure;
        public float StablePressure 
        {
            get { return m_fStablePressure; }
            set
            
            {
                
                if( value != -999.0f && value != -9999.0f)
                    m_fStablePressure = value; 
                else
                {
                    int i = 0;
                    i = 0;
                }
            }
        }

        private float m_fStableFlow;
        public float StableFlow
        {
            get { return m_fStableFlow; }
            set
            { 
               
                if (value != -999.0f && value != -9999.0f)
                    m_fStableFlow = value;
                else
                {
                    int i = 0;
                    i = 0;
                }
            
            }
        }

        DateTime dtFlowUpdateTime = DateTime.Now;
        public DateTime StableFlowTime 
        {
            get { return dtFlowUpdateTime; }
            set
            {
                dtFlowUpdateTime = value;
            }
        }

        DateTime dtPressureUpdateTime = DateTime.Now;

        private bool mFirstFlowStableCheck = true;
        public bool FirstFlowStableCheck
        {
            get { return mFirstFlowStableCheck; }
            set { mFirstFlowStableCheck = value; }
        }

        private bool mFirstPressureStableCheck = true;
        public bool FirstPressureStableCheck
        {
            get { return mFirstPressureStableCheck; }
            set { mFirstPressureStableCheck = value; }
        }

        public DateTime StablePressureTime
        {
            get { return dtPressureUpdateTime; }
            set
            {
                dtPressureUpdateTime = value;
            }
        }


        public bool BeginCheck
        {
            get; set;
        }

        
    }
}
