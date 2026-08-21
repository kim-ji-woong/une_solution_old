using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Windows.Forms;

namespace HSMS
{
    //AlarmHistory
    public class AlarmHistory
    {
        //ID
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //WorkerMemberID
        private string m_szWorkerMemberID = "";
        public string WorekrMemberID
        {
            get { return m_szWorkerMemberID; }
            set { m_szWorkerMemberID = value; }
        }

        //TargetSensorID
        private string m_szTargetSensorID = "";
        public string TargetSensorID
        {
            get { return m_szTargetSensorID; }
            set { m_szTargetSensorID = value; }
        }

        //TargetZoneID
        private int m_szTargetZoneID = -1;
        public int TargetZoneID
        {
            get { return m_szTargetZoneID; }
            set { m_szTargetZoneID = value; }
        }

        //AlarmType
        private int m_szAlarmType = -1;
        public int AlarmType
        {
            get { return m_szAlarmType; }
            set { m_szAlarmType = value; }
        }

        //Done
        private bool m_szDone = false;
        public bool Done
        {
            get { return m_szDone; }
            set { m_szDone = value; }
        }

        //SiteID
        private int m_szSiteID = -1;
        public int SiteID
        {
            get { return m_szSiteID; }
            set { m_szSiteID = value; }
        }

        //public override string ToString()
        //{
        //    return m_szName;
        //}
    }

    //AlarmProcessHistory
    public class AlarmProcessHistory
    {
        //ID
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        //AlarmHistoryID
        private int m_szAlarmHistoryID = -1;
        public int AlarmHistoryID
        {
            get { return m_szAlarmHistoryID; }
            set { m_szAlarmHistoryID = value; }
        }

        //time
        private DateTime m_sztime;
        public DateTime Time
        {
            get { return m_sztime; }
            set { m_sztime = value; }
        }

        //ProcessType
        private int m_szProcessType = -1;
        public int ProcessType
        {
            get { return m_szProcessType; }
            set { m_szProcessType = value; }
        }

        //Distance
        private float m_szDistance = 0.0f;
        public float Distance
        {
            get { return m_szDistance; }
            set { m_szDistance = value; }
        }

        //Status
        private string m_szStatus = "";
        public string Status
        {
            get { return m_szStatus; }
            set { m_szStatus = value; }
        }

        //Message
        private string m_szMessage = "";
        public string Message
        {
            get { return m_szMessage; }
            set { m_szMessage = value; }
        }

        //isCritical
        private bool m_szIsCritical = false;
        public bool IsCritical
        {
            get { return m_szIsCritical; }
            set { m_szIsCritical = value; }
        }

        //public override string ToString()
        //{
        //    return m_szName;
        //}
    }

    //
    public class ReportHistory : IComparable
    {
        //No
        private int m_nNo = -1;
        public int No
        {
            get { return m_nNo; }
            set { m_nNo = value; }
        }

        //일시
        private DateTime m_sztime;
        public DateTime Time
        {
            get { return m_sztime; }
            set { m_sztime = value; }
        }

        //유형
        private string m_szType = "";
        public string Type
        {
            get { return m_szType; }
            set { m_szType = value; }
        }

        //위험물이나 차량의 SensorID
        private string m_szSensorID = "";
        public string SensorID
        {
            get { return m_szSensorID; }
            set { m_szSensorID = value; }
        }

        //위험물
        private DataEquip m_szEquipment = null;
        public DataEquip Equipment
        {
            get { return m_szEquipment; }
            set { m_szEquipment = value; }
        }

        //작업자 SensorID
        private string m_szWorkerSensorID = "";
        public string WorkerSensorID
        {
            get { return m_szWorkerSensorID; }
            set { m_szWorkerSensorID = value; }
        }

        //작업자
        private DataWorker m_szWorker = null;
        public DataWorker Worker
        {
            get { return m_szWorker; }
            set { m_szWorker = value; }
        }


        //위험존
        private DataZone m_szZone = null;
        public DataZone Zone
        {
            get { return m_szZone; }
            set { m_szZone = value; }
        }

        //차량
        private DataCar m_szCar = null;
        public DataCar Car
        {
            get { return m_szCar; }
            set { m_szCar = value; }
        }

        private string m_strEtc = "";
        public string Etc
        {
            get { return m_strEtc; }
            set { m_strEtc = value; }
        }

        private string m_szProcessType = null;
        public string ProcessType
        {
            get { return m_szProcessType; }
            set { m_szProcessType = value; }
        }

        private bool m_bCriticalType = false;
        public bool CriticalType
        {
            get { return m_bCriticalType; }
            set { m_bCriticalType = value; }
        }

        public int CompareTo(object obj)
        {
            ReportHistory history = (ReportHistory)obj;

            if (this.m_sztime < history.m_sztime)
                return 1;
            else if (this.m_sztime > history.m_sztime)
                return -1;
            else
            {
                if (this.m_nNo < history.m_nNo)
                    return -1;
                else if (this.m_nNo > history.m_nNo)
                    return 1;
            }
            return 0;
        }

        //public override string ToString()
        //{
        //    return m_szName;
        //}
    }

    public class DataReport
    {
        private int m_nMaxID = 0;
        private int m_nMaxCount = 0;

        private LinkedList<ReportHistory> m_arrAllReportData = null;

        private System.Collections.ArrayList m_arrReportData = new System.Collections.ArrayList();
        public System.Collections.ArrayList ReportDataList
        {
            get { return m_arrReportData; }
            set { m_arrReportData = value; }
        }


        private DBConn m_DBConnection = null;

        Timer tm = null;

        ////AlarmHistory 데이터
        //private Dictionary<int, AlarmHistory> m_dicAlarmHistory = new Dictionary<int, AlarmHistory>();

        ////AlarmProcessHistory 데이터
        //private Dictionary<int, AlarmProcessHistory> m_dicAlarmProcessHistory = new Dictionary<int, AlarmProcessHistory>();

        public void DataClear()
        {
            if (m_arrAllReportData != null)
                m_arrAllReportData.Clear();
        }

        public DataReport()
        {
            m_DBConnection = new DBConn("HSMS");
            m_arrAllReportData = new LinkedList<ReportHistory>();

            tm = new Timer();
            tm.Interval = 60000;
            tm.Enabled = true;
            tm.Start();

            tm.Tick += new EventHandler(tm_Tick);
            //tm_Tick(null, null);

            //LoadAlarmHistory();
            //LoadAlarmProcessHistory();
        }

        public void tm_Tick(object sender, EventArgs e)
        {
            if (m_DBConnection == null)
                return;

            int nSiteID = FormMain.Instance.SiteID;
            SqlConnection connect = m_DBConnection.Connect();

            int nTotalCount = 0;


            //전체 데이터 갯수 가져오기
            string szSQLCount = string.Format("Select count(*) from AlarmHistory Inner Join AlarmProcessHistory "
                + "on AlarmHistory.ID = AlarmProcessHistory.AlarmHistoryID And SiteID = {0} And ProcessType = 1", nSiteID);
            SqlDataReader rd2 = m_DBConnection.ExecuteReader(szSQLCount, connect);
            if( rd2 != null)
            {
                if (rd2.Read())
                {
                    nTotalCount = Convert.ToInt32(rd2[0].ToString().TrimEnd());
                }

                if (nTotalCount - m_nMaxCount == 0)
                    return;

                int nCount = (nTotalCount - m_nMaxCount) / 10;
                m_nMaxCount = nTotalCount;

                if (nTotalCount < 10)
                {
                    SetDetectedHistory();
                }
                else
                {
                    tm.Stop();
                    for (int i = 0; i < nCount + 1; i++)
                    {
                        SetDetectedHistory();
                    }
                    tm.Start();
                }
                rd2.Close();
            }
            connect.Close();
        }

        public void SetDetectedHistory()
        {
            //if (m_DBConnection == null)
            //    return;

            //ArrayList arrDetectHistory = new ArrayList();

            ////날짜 형식을 string으로 변환
            //string strNowDate = "";
            //string strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), "00", "00", "00");

            //if (dtStart.ToShortDateString() == dtEnd.ToShortDateString())
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), dtEnd.Hour, dtEnd.Minute, dtEnd.Second);
            //}
            //else
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            //}


            //int nSiteID = FormMain.Instance.SiteID;
            //SqlConnection connect = m_DBConnection.Connect();

            //string szSQL = string.Format("Select ProcessType, Time, WorkerMemberID, TargetSensorID, TargetZoneID, AlarmType, IsCritical from AlarmHistory Inner Join AlarmProcessHistory "
            //    + "on AlarmHistory.ID = AlarmProcessHistory.AlarmHistoryID And SiteID = {0} And Time Between '{1}' And '{2}' And ProcessType = 1", nSiteID, strBeforeDate, strNowDate);

            //SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            //while (rd.Read())
            //{
            //    int nProcessType = Convert.ToInt32(rd[0].ToString().TrimEnd());

            //    ////알람단계 검사
            //    ////nCritical이 0이면 1단계 알람, 1이면 2단계 알람
            //    string strCritical = rd[6].ToString().TrimEnd();
            //    //int nCritical = Convert.ToInt32(strCritical);

            //    //if (nProcessType == 1)
            //    {
            //        if (strAlarmStep == "1단계 알람")
            //        {
            //            if (strCritical == "True")
            //                continue;
            //        }
            //        else if (strAlarmStep == "2단계 알람")
            //        {
            //            if (strCritical == "False")
            //                continue;
            //        }
            //    }

            //    DateTime dtTime = (DateTime)rd[1];
            //    string strWorkerMemberID = rd[2].ToString().TrimEnd();

            //    string strProcessType = "";
            //    switch (nProcessType)
            //    {
            //        case 1: strProcessType = "알람 발생";
            //            break;
            //        case 2: strProcessType = "알람 진행중";
            //            break;
            //        case 3: strProcessType = "알람 종료";
            //            break;
            //        case 4: strProcessType = "알람종료버튼 Click에 의한 알람 종료";
            //            break;
            //        default:
            //            break;
            //    }

            //    ReportHistory reportHistory = new ReportHistory();
            //    string strTargetSensorID ="";
            //    strTargetSensorID = rd[3].ToString().TrimEnd();
            //    int nTargetZoneID = -1;
            //    if(strTargetSensorID == "")
            //    {
            //        nTargetZoneID = Convert.ToInt32(rd[4].ToString().TrimEnd());
            //        DataZone zone = FormMain.Instance.DataMgr.FindZone(nTargetZoneID);
            //        reportHistory.Zone = zone;
            //    }
            //    else
            //    {
            //        if (ERPManager.Instance.DicSensorEquip.ContainsKey(strTargetSensorID))
            //        {
            //            DataEquip equip = ERPManager.Instance.DicSensorEquip[strTargetSensorID];

            //            reportHistory.Equipment = equip;
            //            reportHistory.SensorID = strTargetSensorID;
            //        }
            //        else
            //        {
            //            DataCar car = ERPManager.Instance.DicSensorCar[strTargetSensorID];
            //            reportHistory.Car = car;
            //            reportHistory.SensorID = strTargetSensorID;
            //        }
            //    }

            //    reportHistory.ProcessType = strProcessType;

            //    int nAlarmType = Convert.ToInt32(rd[5].ToString().TrimEnd());
            //    string strAlarmType = "";
            //    switch (nAlarmType)
            //    {
            //        case 1: strAlarmType = "차량이 작업자를 향해 접근";
            //            break;
            //        case 2: strAlarmType = "작업자가 차량을 향해 접근";
            //            break;
            //        case 3: strAlarmType = "차량과 작업자가 상호 접근";
            //            break;
            //        case 4: strAlarmType = "작업자가 위험설비를 향해 접근";
            //            break;
            //        case 5: strAlarmType = "작업자가 위험존을 향해 접근";
            //            break;
            //    }


            //    DataWorker worker = ERPManager.Instance.DicCompanyWorkers[strWorkerMemberID];

            //    reportHistory.Time = dtTime;
            //    reportHistory.Worker = worker;
            //    reportHistory.WorkerSensorID = worker.Sensor;
            //    reportHistory.Type = strAlarmType;


            //    arrDetectHistory.Add(reportHistory);
            //}
            //rd.Close();
            //connect.Close();

            ////다른 폼에서 참조할 때 m_arrReportHistoryList를 가져다 쓰면됨.
            //m_arrReportHistoryList = arrDetectHistory;

            if (m_DBConnection == null)
                return;

            int nSiteID = FormMain.Instance.SiteID;
            SqlConnection connect = m_DBConnection.Connect();

            System.Diagnostics.Trace.WriteLine("Select Log : " + m_nMaxID.ToString());

            // 데이터 가져오기
            string szSQL = string.Format("Select Top 10 AlarmProcessHistory.ID, ProcessType, Time, WorkerMemberID, TargetSensorID, TargetZoneID, AlarmType, IsCritical from AlarmHistory Inner Join AlarmProcessHistory "
                + "on AlarmHistory.ID = AlarmProcessHistory.AlarmHistoryID And SiteID = {0} And ProcessType = 1 And AlarmProcessHistory.ID > {1}", nSiteID, m_nMaxID);
            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0].ToString().TrimEnd());

                int nProcessType = Convert.ToInt32(rd[1].ToString().TrimEnd());




                DateTime dtTime = (DateTime)rd[2];
                string strWorkerMemberID = rd[3].ToString().TrimEnd();

                string strProcessType = "";
                switch (nProcessType)
                {
                    case 1: strProcessType = "알람 발생";
                        break;
                    case 2: strProcessType = "알람 진행중";
                        break;
                    case 3: strProcessType = "알람 종료";
                        break;
                    case 4: strProcessType = "알람종료버튼 Click에 의한 알람 종료";
                        break;
                    default:
                        break;
                }

                int nAlarmType = Convert.ToInt32(rd[6].ToString().TrimEnd());

                ReportHistory reportHistory = new ReportHistory();
                string strTargetSensorID = "";
                strTargetSensorID = rd[4].ToString().TrimEnd();
                int nTargetZoneID = -1;
                if (strTargetSensorID == "")
                {
                    nTargetZoneID = Convert.ToInt32(rd[5].ToString().TrimEnd());
                    DataZone zone = FormMain.Instance.DataMgr.FindZone(nTargetZoneID);
                    reportHistory.Zone = zone;
                }
                else
                {
                    if (ERPManager.Instance.DicSensorEquip.ContainsKey(strTargetSensorID))
                    {
                        DataEquip equip = ERPManager.Instance.DicSensorEquip[strTargetSensorID];

                        reportHistory.Equipment = equip;
                        reportHistory.SensorID = strTargetSensorID;
                    }
                    else
                    {
                        if (ERPManager.Instance.DicSensorCar.ContainsKey(strTargetSensorID))
                        {
                            DataCar car = ERPManager.Instance.DicSensorCar[strTargetSensorID];
                            reportHistory.Car = car;
                        }
                        else
                            reportHistory.Etc = nAlarmType == 6 ? "일산화탄소" : "메탄가스";

                        reportHistory.SensorID = strTargetSensorID;
                    }
                }

                reportHistory.ProcessType = strProcessType;
               

                string strCritical = rd[7].ToString().TrimEnd();
                if (strCritical == "True")
                    reportHistory.CriticalType = true;
                else
                    reportHistory.CriticalType = false;

                string strAlarmType = "";

                
                if(reportHistory.CriticalType == false)
                {
                    switch (nAlarmType)
                    {
                        case 1: strAlarmType = "차량이 작업자를 충돌";
                            break;
                        case 2: strAlarmType = "작업자가 차량에 충돌";
                            break;
                        case 3: strAlarmType = "차량과 작업자가 상호 충돌";
                            break;
                        case 4: strAlarmType = "작업자가 위험설비에 진입";
                            break;
                        case 5: strAlarmType = "작업자가 위험존으로 진입";
                            break;
                        case 6: strAlarmType = "일산화탄소 누출";
                            break;
                        case 7: strAlarmType = "메탄가스 누출";
                            break;
                    }
                }
                else
                {
                    switch (nAlarmType)
                    {
                        case 1: strAlarmType = "차량이 작업자를 향해 접근";
                            break;
                        case 2: strAlarmType = "작업자가 차량을 향해 접근";
                            break;
                        case 3: strAlarmType = "차량과 작업자가 상호 접근";
                            break;
                        case 4: strAlarmType = "작업자가 위험설비를 향해 접근";
                            break;
                        case 5: strAlarmType = "작업자가 위험존을 향해 접근";
                            break;
                        case 6: strAlarmType = "일산화탄소 누출";
                            break;
                        case 7: strAlarmType = "메탄가스 누출";
                            break;
                    }
                } 

                DataWorker worker = strWorkerMemberID.Length > 0 ? ERPManager.Instance.DicCompanyWorkers[strWorkerMemberID] : null;

                reportHistory.Time = dtTime;
                reportHistory.Worker = worker;
                reportHistory.WorkerSensorID = worker != null ? worker.Sensor : "";
                reportHistory.Type = strAlarmType;


                m_arrAllReportData.AddLast(reportHistory);

                m_nMaxID = nID;


            }
            rd.Close();
            connect.Close();
        }


        //전체 데이터로부터 지정한 날짜와 위험단계 검색
        public void ProcessSearchData(DateTime dtStart, DateTime dtEnd, string strAlarmStep)
        {

            ////알람단계 검사
            ////nCritical이 0이면 1단계 알람, 1이면 2단계 알람

            if (m_arrAllReportData == null)
                return;


            m_arrReportData.Clear();

            if (strAlarmStep == "1단계 알람")
            {
                var Lists = from data in m_arrAllReportData
                            where data.Time >= dtStart && data.Time < dtEnd.AddDays(1) && data.CriticalType == false
                            select data;


                foreach (ReportHistory data in Lists)
                {
                    ReportHistory equip = (ReportHistory)data;
                    m_arrReportData.Add(equip);
                }
            }
            else if(strAlarmStep == "2단계 알람")
            {
                var Lists = from data in m_arrAllReportData
                            where data.Time >= dtStart && data.Time < dtEnd.AddDays(1) && data.CriticalType == true
                            select data;

                foreach (ReportHistory data in Lists)
                {
                    ReportHistory equip = (ReportHistory)data;
                    m_arrReportData.Add(equip);
                }
            }
            else
            {
                var Lists = from data in m_arrAllReportData
                            where data.Time >= dtStart && data.Time < dtEnd.AddDays(1)
                            select data;

                foreach (ReportHistory data in Lists)
                {
                    ReportHistory equip = (ReportHistory)data;
                    m_arrReportData.Add(equip);
                }
            
            }
        }
    }
}
