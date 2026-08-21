using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility; 
using TankModbusLib;

namespace TankServer
{
    public class TankLevelMeterManager
    {
        private int m_nSiteID = 1;
        private DBUtility.WebDBManager mDBMgr = null;
        private Thread m_LevelCheckThread = null;

        private LeakDetectorManager dm = new LeakDetectorManager();
        public LeakDetectorManager Detector
        {
            get { return dm; }
        }

        private static TankLevelMeterManager m_instance = null;
        public static TankLevelMeterManager Instance
        {
            get { return m_instance; }
        }

        public TankLevelMeterManager()
        {
            m_nSiteID = LevelMeterNetworkServer.Instance.SiteID;
            mDBMgr = new WebDBManager(m_nSiteID);         
            m_instance = this;
        }
          
        private Action<int, int, float, int, int> mNotifyAction = null;
        public void BeginServer(Action<int, int, float, int, int> onNotify)
        {
            try
            {
                mNotifyAction = onNotify;
                dm.OnNotifyAlarm += GasLevelMeter_OnNotifyAlarm;
                dm.Start();
            }
            catch(Exception)
            { 

            }

            LoadTank();

            m_LevelCheckThread = new Thread(CheckValue);
            m_LevelCheckThread.Name = "Level value Check";
            m_LevelCheckThread.Start();

            BeginCommander();
        }

        private void GasLevelMeter_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if(mNotifyAction != null)
            {
                mNotifyAction.Invoke(nComm, nAlarmUnit, fValue, nChannel, nStatus);
            }           
        }

        public void StopServer()
        {
            try
            {
                m_bReleaseThread = true;
                if (m_LevelCheckThread != null)
                    m_LevelCheckThread.Join(2000);
            }
            catch(Exception)
            {}            

            dm.OnNotifyAlarm -= GasLevelMeter_OnNotifyAlarm;
            dm.End();

            mNotifyAction = null;

            StopCommander();
        }
        
        public ArrayList m_TankList = new ArrayList();
        private SortedList<int, TankInfo> m_dicTankList = new SortedList<int, TankInfo>();

        private void LoadTank()
        {
            string szSQL = "SELECT ID, Name, IsLeakStatus, LeakEvtStatusAddress, " +
                "(SELECT endtime FROM TANKLEAKHISTORY where tankid=t.id order by id desc limit 1) LastEndTime " + 
                "FROM tank as t WHERE LiquidType = '황산'"; 

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if( arResult != null && arResult.Count > 0)
            {
                for (int i = 0 ; i < arResult.Count; i += 5)
                {
                    int nTankID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    string szName = WebDBManager.GetStringField(arResult[i + 1]);
                    int nIsLeakStatus = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1); 
                    int nLeakEvtStatusAddress = WebDBManager.GetIntField(arResult[i + 3].ToString(), -1);
                    VariousData<DateTime> LastEndTime = WebDBManager.GetDateTimeField(arResult[i + 4]);
                                        
                    TankInfo info = new TankInfo();
                    info.ID = nTankID;
                    info.Name = szName;
                    info.LeakStatus = nIsLeakStatus;
                    info.LeakEvtStatusAddress = nLeakEvtStatusAddress;
                    info.LastAlarm = LastEndTime;
                     
                    int cuNum = -1;
                    if (nTankID == 11)
                        cuNum = 1;
                    else if (nTankID == 12)
                        cuNum = 2;
                    else if (nTankID == 13)
                        cuNum = 3;
                    else if (nTankID == 14)
                        cuNum = 4;
                    else if (nTankID == 15)
                        cuNum = 5;

                    info.CuNum = cuNum;                    

                    m_TankList.Add(info); 
                    m_dicTankList.Add(info.ID, info);
                }
            }
        }

        private void LoadTankAlarmOptions()
        {
            string szSQL = "SELECT TankID, AlarmIntervalUse, AlarmInterval FROM AlarmOptions WHERE TankID in (select id from tank where liquidType='황산')";

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count; i += 3)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    bool bUse = (arResult[i + 1].ToString() == "1") ? true : false;
                    int nInterval = WebDBManager.GetIntField(arResult[i + 2].ToString(), 0); 

                    TankInfo info = GetTank(nID);

                    if (info == null) continue;
                    info.AlarmIntervalUse = bUse;
                    info.AlarmInterval = nInterval; 
                }
            }
        }

        private TankInfo GetTank(int nTankID)
        {
            TankInfo info = null;
            m_dicTankList.TryGetValue(nTankID, out info);
            return info;
        }

        private Thread m_CommandThread;
        private bool m_bExitThread = false;
        private int nSleepTime = 3000;

        internal void BeginCommander()
        {
            m_CommandThread = new Thread(ProcessCommandTh);
            m_CommandThread.Start();
        }

        private void ProcessCommandTh()
        {
            while (!m_bExitThread)
            { 
                ReadAllTankCommand();
                Queue<ModbusCommand> que = this.m_QueCmd;
                if (que != null && que.Count > 0)
                {
                    int nCount = que.Count;
                    int nProcessCount = 0;
                    while (que.Count > 0)
                    {
                        ModbusCommand cmd = que.Dequeue();

                        if (ProcessCommand(cmd) == true)
                        {
                            RemoveTankCommand(cmd);
                        }
                        else
                        {
                            que.Enqueue(cmd);
                        }
                        Thread.Sleep(5);

                        if (2 * nCount < nProcessCount)
                            break;

                        nProcessCount++;
                    }
                }
                 
                for (int i = 0; i < 10; i++)
                {
                    if (m_bExitThread == true)
                        break;
                    Thread.Sleep(nSleepTime / 10);
                }
            }
        }

        internal void StopCommander()
        {        
            m_bExitThread = true;
        }

        //0(Pipe Alarm Off & Siren Off), 1(Siren On), 2(Tank Alarm Off)
        private bool ProcessCommand(ModbusCommand cmd)
        {
            if (cmd.Command == 11)
            {
                SendCommand(cmd);
                ClearTankAlarm(cmd);
                return true; 
            }
            if (cmd.Command == 13)
            {
                SendCommand(cmd); 
                return true;
            }
            return false;
        }

        internal void SendCommand(ModbusCommand cmd)
        { 
            WriteSingleType writeType = WriteSingleType.None;
            if (cmd.Command == 10)
                writeType = WriteSingleType.RealyNAlarmConfigMode;
            else if (cmd.Command == 11)
                writeType = WriteSingleType.BuzzerAlarmStatus;
            else if (cmd.Command == 12)
                writeType = WriteSingleType.ResetMode;
            else if (cmd.Command == 13)
                writeType = WriteSingleType.Reset;

            if (writeType == WriteSingleType.None) return;

            if (m_dicTankList.ContainsKey(cmd.TankID))
            {
                if (m_dicTankList[cmd.TankID].CuNum < 0) return;
                 
                dm.SetControlRegister(m_dicTankList[cmd.TankID].CuNum, 6, (int)writeType, Convert.ToInt32(cmd.CommandValue));
            } 
        }
         
        private bool m_bReleaseThread = false;

        private void CheckValue()
        {
            int nCount = 0;
            bool bFirst = true;
            while (!m_bReleaseThread)
            {
                // Tank별 알람 옵션은 바뀔수 있으므로 계속 새로 읽는다.
                LoadTankAlarmOptions();

                int nUnitID = -1;
                
                for (int i = 0; i < m_TankList.Count; i++)
                { 
                    TankInfo info = (TankInfo)m_TankList[i];
                    //if (info.CuNum != 3) continue; //t
                    nUnitID = info.CuNum;
                    bool bOnline = dm.GetOnline(nUnitID); 
                    if (nUnitID < 0) continue;

                    int nStatus = dm.GetStatus(nUnitID, 0);

                    if (bOnline) 
                        info.LeakMonitoring = 1;
                    else if (!bOnline)
                    {
                        info.LeakMonitoring = 0;
                        nStatus = 0; // 통신 끊어졌을때 알람해제 처리
                    }

                    //if (nStatus == -999) nStatus = 0;
                    // Leak 상태 변경됐을때 알람 발생
                    if (info.LeakStatus != nStatus && nStatus >= 0)
                    {
                        bool chk = false;
                        if (nStatus == 1)
                        {
                            if (info.LastAlarm == null) // 알람중이거나 여태 한번도 알람인적 없음
                                chk = true;
                            else // n분 이내에 같은 알람이 발생하지 않는다
                            {
                                DateTime now = DateTime.Now;
                                TimeSpan ts = now - info.LastAlarm.Data;
                                if (ts.TotalSeconds > info.AlarmInterval)
                                    chk = true;
                            }
                        }

                        if (nStatus == 0 || (nStatus == 1 && chk))
                        {
                            info.LeakStatus = nStatus;
                            CheckAlarm(info);
                        }
                    } 

                    SaveTankValue(info); 
                     
                    if (m_bReleaseThread == true)
                        break;
                }


                if (nCount == 20)
                    bFirst = false;
                else
                    nCount++;

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    if (m_bReleaseThread == true)
                        break;
                } 
            }
        }

        private string GetKeyTimeString(DateTime dt)
        {
            string szResult = string.Format("{0:D2}{1:D2}{2:D2}{3:D2}", dt.Day, dt.Hour, dt.Minute, dt.Second);
            return szResult;
        }

        private void SaveTankValue(TankInfo info)
        {
            string szTemp = "UPDATE tank SET IsLeakStatus={0}, IsLeakMonitoring={1} WHERE ID = {2}";

            string szSQL = string.Format(szTemp, info.LeakStatus, info.LeakMonitoring, info.ID);
            mDBMgr.GetResultData(szSQL, 0);
        }

        #region Leak 상태
        private void CheckAlarm(TankInfo info)
        {
            string szTemp = "";
            string szSQL = "";
            if (info.LeakStatus == 0) // 정상
            {
                // Last Table Null Update
                UpdateRecentTankLeakAlarm(info.ID, -1);

                DateTime now = DateTime.Now;
                // History Table Update
                string szEndDate = WebDBManager.MakeDateTimeString(now);
                if (info.LastAlarm == null)
                    info.LastAlarm = new VariousData<DateTime>();
                info.LastAlarm.Data = now;

                szTemp = "UPDATE TankLeakHistory SET EndTime='{0}', AlarmTerminator=-1, AlarmOccurType=3, AlarmComment='자동종료' WHERE TankID = {1} AND EndTime is NULL";
                szSQL = string.Format(szTemp, szEndDate, info.ID);
                mDBMgr.GetResultData(szSQL, 0); 
            }
            else if (info.LeakStatus == 1) // 누출 (알람발생)
            {  
                // Buzzer 복귀 
                dm.SetControlRegister(info.CuNum, 6, (int)WriteSingleType.BuzzerAlarmStatus, 0);
                 
                info.LastAlarm = null;
                int nHistoryID = InsertTankLeakHistory(info.ID, info.LeakStatus);
                UpdateRecentTankLeakAlarm(info.ID, nHistoryID); 
            } 
        } 
         
        private int InsertTankLeakHistory(int nTankID, int nStatus)
        {
            int nAlarm = CheckLeakStatus(nTankID, nStatus);
            if (nAlarm >= 0)
                return nAlarm;
            
            // 기존 알람 해제처리 안된것 해제
            string szDate = WebDBManager.MakeDateTimeString(DateTime.Now);
            mDBMgr.GetResultData(string.Format("Update TankLeakHistory Set EndTime='{0}' WHERE EndTime IS NULL And TankID={1}", szDate, nTankID), 0);

            int nMaxID = GetMaxID("TankLeakHistory", mDBMgr) + 1;
            string szTemp = "INSERT INTO TankLeakHistory (ID, TankID, BeginTime, EndTime, EvtStatus, LeakPosition) VALUES ({0},{1},'{2}',NULL,{3},0)";
            
            string szSQL = string.Format(szTemp, nMaxID, nTankID, szDate, nStatus, 1);            
            mDBMgr.GetResultData(szSQL, 0);

            return nMaxID;
        }

        private void UpdateRecentTankLeakAlarm(int nTankID, int nHistoryID)
        {
            string szTemp = "UPDATE TankLeak SET HistoryID = {0} WHERE TankID = {1}";
            string szSQL = string.Format(szTemp, (nHistoryID < 0) ? "NULL" : nHistoryID.ToString(), nTankID);
            mDBMgr.GetResultData(szSQL, 0);
        }

        private int CheckLeakStatus(int nTankID, int nStatus)
        {
            string szTemp = "SELECT HistoryID FROM TankLeak WHERE TankID = {0}";
            string szSQL = string.Format(szTemp, nTankID);
            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                return nID;
            }
            return -1;
        }

        public void ClearTankAlarm(ModbusCommand cmd)
        {
            if (cmd == null)
                return; 

            TankInfo sensor = null;
            if (m_dicTankList.TryGetValue(cmd.TankID, out sensor))
            {
                string szTemp = "";
                string szSQL = "";
                if (cmd.Command == 11)
                {
                    // Last Table Null Update
                    UpdateRecentTankLeakAlarm(cmd.TankID, -1); 
                     
                    // History Table Update
                    string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
                    DateTime now = DateTime.Now;
                    szTemp = "UPDATE TankLeakHistory SET EndTime= '{0}', AlarmTerminator={1}, AlarmOccurType={2}, AlarmComment='{3}' WHERE TankID = {4} AND EndTime is NULL";
                    szSQL = string.Format(szTemp, szEndDate, (cmd.UserID > 0 ? cmd.UserID.ToString() : "NULL"), cmd.AlarmOccurType, cmd.AlarmComment, cmd.TankID);
                    mDBMgr.GetResultData(szSQL, 0);

                    szTemp = "UPDATE tank SET IsLeakStatus=0 WHERE ID = {0}";
                    szSQL = string.Format(szTemp, cmd.TankID);
                    mDBMgr.GetResultData(szSQL, 0);

                    // Last Table Null Update
                    UpdateRecentTankLeakAlarm(cmd.TankID, -1);  

                    for (int i = 0; i < m_TankList.Count; i++)
                    {
                        TankInfo info = (TankInfo)m_TankList[i];
                        if (info.ID == cmd.TankID)
                        {
                            info.LeakStatus = 0;
                            info.LastAlarm = new VariousData<DateTime>(now);
                            break;
                        }

                    }
                } 
            }
        }
        #endregion
         
        #region Command
        private Queue<ModbusCommand> m_QueCmd = new Queue<ModbusCommand>();
        public Queue<ModbusCommand> CommandQue
        {
            get { return m_QueCmd; }
        }

        public void ReadAllTankCommand()
        {
            m_QueCmd.Clear();

            string szSQL = "SELECT cmd.ID, cmd.CommandType, cmd.TimeStamp, cmd.UserID, cmh.ID, cmd.TankID, cmd.CommandValue, cmh.AlarmOccurType, cmh.AlarmComment " +
                           "  FROM command as cmd " +
                           " INNER JOIN commandhistory as cmh on cmh.CmdID = cmd.ID " +
                           " WHERE cmd.CommandType in (10, 11, 12, 13) ORDER BY cmd.id";

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return;

            for (int i = 0; i < arResult.Count; i += 9)
            {
                int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nCmd = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arResult[i + 2]);
                int nUserID = WebDBManager.GetIntField(arResult[i + 3].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);
                VariousData<int> nTankID = WebDBManager.GetIntField(arResult[i + 5].ToString());
                string nCmdValue = (arResult[i + 6].ToString() == "null") ? "-1" : arResult[i + 6].ToString();
                int nAlarmOccurType = WebDBManager.GetIntField(arResult[i + 7].ToString(), -1);
                string strAlarmComment = WebDBManager.GetStringField(arResult[i + 8], "");

                if (nID > 0)
                {
                    ModbusCommand cmd = new ModbusCommand();
                    cmd.ID = nID;
                    cmd.Command = nCmd;
                    if (dt != null)
                        cmd.CreateTime = dt.Data;
                    cmd.UserID = nUserID;
                    cmd.HistoryID = nHistoryID;
                    cmd.PipeID = -1;

                    if (nTankID != null)
                    {
                        cmd.TankID = nTankID.Data;
                    }

                    cmd.CommandValue = nCmdValue;
                    cmd.AlarmOccurType = nAlarmOccurType;
                    cmd.AlarmComment = strAlarmComment;
                    m_QueCmd.Enqueue(cmd);
                }
            }
        }

        public void RemoveTankCommand(ModbusCommand cmd)
        {
            if (cmd != null)
            {
                string szTemp = "UPDATE commandhistory SET CmdID = NULL, CommandExecuteTime='{0}' WHERE ID = {1}";
                string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
                string szSQL = string.Format(szTemp, szEndDate, cmd.HistoryID);
                mDBMgr.GetResultData(szSQL, 0);
                string szSQL2 = "DELETE FROM command WHERE ID = " + cmd.ID.ToString();
                mDBMgr.GetResultData(szSQL2, 0);
            }
        } 
        #endregion

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select MAX(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }
    }

    internal class TankInfo
    {
        private int m_nID;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nCuNum;
        public int CuNum
        {
            get { return m_nCuNum; }
            set { m_nCuNum = value; }
        }

        private string m_szName;
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        // 0:정상 1:누출
        private int m_nLeakStatus;
        public int LeakStatus
        {
            get { return m_nLeakStatus; }
            set { m_nLeakStatus = value; }
        }

        // 0:감시안함 1:감시중
        private int m_nLeakMonitoring;
        public int LeakMonitoring
        {
            get { return m_nLeakMonitoring; }
            set { m_nLeakMonitoring = value; }
        }

        private int m_nLeakEvtStatusAddress;
        public int LeakEvtStatusAddress
        {
            get { return m_nLeakEvtStatusAddress; }
            set { m_nLeakEvtStatusAddress = value; }
        }

        private bool m_bAlarmIntervalUse;
        public bool AlarmIntervalUse
        {
            get { return m_bAlarmIntervalUse; }
            set { m_bAlarmIntervalUse = value; }
        }

        private int m_nAlarmInterval;
        public int AlarmInterval
        {
            get { return m_nAlarmInterval; }
            set { m_nAlarmInterval = value; }
        }

        private VariousData<DateTime> m_dtLastAlarm;
        public VariousData<DateTime> LastAlarm
        {
            get { return m_dtLastAlarm; }
            set { m_dtLastAlarm = value; }
        } 
    }
}
