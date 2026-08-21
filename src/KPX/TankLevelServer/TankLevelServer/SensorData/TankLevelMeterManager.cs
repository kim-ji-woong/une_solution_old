using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility;
using JubixNetwork;

namespace GasLevelServer
{
    public class TankLevelMeterManager
    {
        private int m_nSiteID = 1;
        private DBUtility.WebDBManager mDBMgr = null;
        private Thread m_LevelCheckThread = null;

        private GasDetector.LevelMeterManager dm = new GasDetector.LevelMeterManager();
        public GasDetector.LevelMeterManager Detector
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
            { }

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


        private ArrayList m_TankList = new ArrayList();
        private SortedList<int, TankInfo> m_TankInfo = new SortedList<int, TankInfo>();

        private void LoadTank()
        {
            string szSQL = "SELECT ID, Name, LiquidType, Capacity, HighLevel, MinTemp, MaxTemp, Density, Status, "+ 
                          " LevelAddress, TempAddress, GrossVolumeAddress, NetVolumeAddress, MassAddress, FlowAddress, PressureAddress " +
                          " FROM tank";

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if( arResult != null && arResult.Count > 0)
            {
                for( int i = 0 ; i < arResult.Count - 15; i += 16)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    string szName = WebDBManager.GetStringField(arResult[i+1]);
                    string szLiquidType = WebDBManager.GetStringField(arResult[i+2]);
                    float fCapacity = WebDBManager.GetFloatField(arResult[i+3].ToString(), 0.0f);
                    float fHighLevel = WebDBManager.GetFloatField(arResult[i+4].ToString(), 0.0f);
                    float fMinTemp = WebDBManager.GetFloatField(arResult[i+5].ToString(), 0.0f);
                    float fMaxTemp = WebDBManager.GetFloatField(arResult[i+6].ToString(), 0.0f);
                    float fDensity = WebDBManager.GetFloatField(arResult[i+7].ToString(), 0.0f);
                    int nStatus = WebDBManager.GetIntField(arResult[i+8].ToString(), -1);
                    int nLevelAddress = WebDBManager.GetIntField(arResult[i+9].ToString(), -1);
                    int nTempAddress = WebDBManager.GetIntField(arResult[i+10].ToString(), -1);
                    int nGrossVolumeAddress = WebDBManager.GetIntField(arResult[i+11].ToString(), -1);
                    int nNetVolumeAddress = WebDBManager.GetIntField(arResult[i+12].ToString(), -1);
                    int nMassAddress = WebDBManager.GetIntField(arResult[i+13].ToString(), -1);
                    int nFlowAddress = WebDBManager.GetIntField(arResult[i+14].ToString(), -1);
                    int nPressureAddress = WebDBManager.GetIntField(arResult[i +15].ToString(), -1);
                                        
                    TankInfo info = new TankInfo();
                    info.ID = nID;
                    info.Name = szName;
                    info.LiquidType = szLiquidType;
                    info.Capacity = fCapacity;
                    info.HighLevel = fHighLevel;
                    info.MinTemp = fMinTemp;
                    info.MaxTemp = fMaxTemp;
                    info.Density = fDensity;
                    info.Status = nStatus;
                    info.LevelAddress = nLevelAddress - 1;
                    info.TempAddress = nTempAddress - 1;
                    info.GrossVolumeAddress = nGrossVolumeAddress - 1;
                    info.NetVolumeAddress = nNetVolumeAddress -1;
                    info.MassAddress = nMassAddress -1;
                    info.FlowAddress = nFlowAddress -1;
                    info.PressureAddress = nPressureAddress -1 ;

                    m_TankList.Add(info);
                    m_TankInfo.Add(nLevelAddress, info);
                    m_dicTankList.Add(info.ID, info);
                }
            }
        }

        private void LoadTankAlarmOptions()
        {
            string szSQL = "SELECT ID, HighLevel, MinTemp, MaxTemp FROM tank";

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count - 3; i += 4)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    float fHighLevel = WebDBManager.GetFloatField(arResult[i + 1].ToString(), 0.0f);
                    float fMinTemp = WebDBManager.GetFloatField(arResult[i + 2].ToString(), 0.0f);
                    float fMaxTemp = WebDBManager.GetFloatField(arResult[i + 3].ToString(), 0.0f);

                    TankInfo info = GetTank(nID);

                    if (info == null)
                        continue;

                    info.HighLevel = fHighLevel;
                    info.MinTemp = fMinTemp;
                    info.MaxTemp = fMaxTemp;
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
            m_CommandThread = new Thread(ProcessCommand);
            m_CommandThread.Start();

        }

        private void ProcessCommand()
        {
            while (!m_bExitThread)
            {
                //if(ReadOption())
                {
                    JubixNetwork.PipeSensorManager.Instance.ChangeOption();
                }

                ReadAllTankCommand();
                Queue<JubixNetwork.JubixCommand> que = this.m_QueCmd;
                if (que != null && que.Count > 0)
                {
                    int nCount = que.Count;
                    int nProcessCount = 0;
                    while (que.Count > 0)
                    {
                        JubixNetwork.JubixCommand cmd = que.Dequeue();

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
        private bool ProcessCommand(JubixNetwork.JubixCommand cmd)
        {
            if (cmd.Command == 2 || cmd.Command == 3)
            {
                //int nTankID = cmd.TankID;
                //if (nTankID > 0)
                {
                    ClearTankAlarm(cmd);
                    return true;
                }
            }         
            return false;
        }

        private void SaveLevelServerInfo(int nReciverID, bool bOnline)
        {
        }        

        private bool m_bReleaseThread = false;

        private void CheckValue()
        {
            int nCount = 0;
            bool bFirst = true;
            while (!m_bReleaseThread)
            {

                DateTime dt = DateTime.Now;
                
                int nUnitID = 1;
                bool bOnline = dm.GetOnline(nUnitID);
                for (int i = 0; i < m_TankList.Count; i++ )
                {
                    nUnitID = 1;
                    TankInfo info = (TankInfo)m_TankList[i];
                    int nDelta = 0;

                    if(info.LevelAddress >= 108)
                    {
                        nUnitID = 4;
                        nDelta = 108;
                    }
                    else if (info.LevelAddress >= 72)
                    {
                        nUnitID = 3;
                        nDelta = 72;

                    }
                    else if(info.LevelAddress >= 36)
                    {
                        nUnitID = 2;
                        nDelta = 36;
                    }

                    float fLevel = dm.GetLevel(nUnitID, info.LevelAddress - nDelta);
                    float fTemp = dm.GetLevel(nUnitID, info.TempAddress - nDelta);
                    float fGrossVolume = dm.GetLevel(nUnitID, info.GrossVolumeAddress - nDelta);
                    float fNetVolume = dm.GetLevel(nUnitID, info.NetVolumeAddress - nDelta);
                    float fMass = dm.GetLevel(nUnitID, info.MassAddress - nDelta);
                    float fFlow = dm.GetLevel(nUnitID, info.FlowAddress - nDelta);


                    if (info.PressureAddress >= 108)
                    {
                        nUnitID = 4;
                        nDelta = 108;
                    }
                    else if (info.PressureAddress >= 72)
                    {
                        nUnitID = 3;
                        nDelta = 72;

                    }
                    else if (info.PressureAddress >= 36)
                    {
                        nUnitID = 2;
                        nDelta = 36;
                    }
                    
                    float fPressure = dm.GetLevel(nUnitID, info.PressureAddress - nDelta);


                    //System.Diagnostics.Trace.WriteLine("CheckValue : Level " + fLevel);
                    //System.Diagnostics.Trace.WriteLine("CheckValue : Temp " + fTemp);
                   // System.Diagnostics.Trace.WriteLine("CheckValue : Gross " + fGrossVolume);
                    //System.Diagnostics.Trace.WriteLine("CheckValue : Net " + fNetVolume);
                   // System.Diagnostics.Trace.WriteLine("CheckValue : Mass " + fMass);
                   // System.Diagnostics.Trace.WriteLine("CheckValue : Flow " + fFlow);

                    info.Level = fLevel;
                    info.Temperature = fTemp;
                    info.GrossVolume = fGrossVolume;
                    info.NetVolume = fNetVolume;
                    info.Mass = fMass;
                    info.Flow = fFlow;

                    if (info.PressureAddress >= 0)
                        info.Pressure = fPressure;

                    if (bFirst == false)
                    {
                        CheckAlarm(info);
                    }
                    
                    SaveValue(info);

                    SaveFlowHistory(info, dt);
                    

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

                // Tank별 알람 옵션은 바뀔수 있으므로 계속 새로 읽는다.
                LoadTankAlarmOptions();
            }
        }

        private string GetKeyTimeString(DateTime dt)
        {
            string szResult = string.Format("{0:D2}{1:D2}{2:D2}{3:D2}", dt.Day, dt.Hour, dt.Minute, dt.Second);
            return szResult;
        }

        private void SaveFlowHistory(TankInfo info, DateTime dt)
        {
            string szTableName = string.Format("zzz_flowhistory_{0}_{1:D2}", info.ID, dt.Month);

            string szTemp = "INSERT INTO {0} ( KeyTime, flow, temperture, level ) values ({1}, {2}, {3}, {4})";
            string keyTime = GetKeyTimeString(dt);
            string szSQL = string.Format(szTemp,szTableName, keyTime, info.Flow, info.Temperature, info.Level);
            mDBMgr.GetResultData(szSQL, 0);
        }

        private void SaveValue(TankInfo info)
        {
            string szTemp = "UPDATE tank SET Level={0},Temperature={1},Mass={2},Flow={3},GrossVolume={4}, " +
                    " NetVolume={5}, Pressure={6}, Status= {7} WHERE ID = {8}";
            
            string szPressure = (info.PressureAddress >= 0 ? info.Pressure.ToString() : "NULL");
            string szSQL = string.Format(szTemp, info.Level, info.Temperature, info.Mass, info.Flow, info.GrossVolume, info.NetVolume, szPressure, info.Status, info.ID);
            mDBMgr.GetResultData(szSQL, 0);
        }       

        private void CheckAlarm(TankInfo info)
        {
            bool m_bCheck = false;
            float temp = info.Temperature;
            if( temp >= info.MaxTemp)
            {
                // create over tempeture              
                
                if (info.Status == 3)
                    info.Status = 6;
                if (info.Status <= 4)
                    info.Status = 4;

                int nID = SaveAlarmHistory(info.ID, info.Status , 2, new VariousData<float>(temp), null);
                UpdateRecentTankTempAlarm(info.ID, nID);

                m_bCheck = true;
            }

            // 절대온도 0은 -273.15도
            if( temp < info.MinTemp && temp > -274.0f)
            {
                // create under tempeture
               

                if (info.Status == 3)
                    info.Status = 7;
                if (info.Status <= 4)
                    info.Status = 5;

                int nID = SaveAlarmHistory(info.ID, info.Status, 1, new VariousData<float>(temp), null);
                UpdateRecentTankTempAlarm(info.ID, nID);

                m_bCheck = true;
            }

            float fLevel = info.Level;
            if (fLevel >= info.HighLevel)
            {
                
                if (info.Status <= 3)
                    info.Status = 3;

                if (info.Status == 5)
                    info.Status = 7;
                if (info.Status == 4)
                    info.Status = 6;

                int nID = SaveAlarmHistory(info.ID, info.Status, 0, null, new VariousData<float>(fLevel));
                UpdateRecentTankLevelAlarm(info.ID, nID);


                m_bCheck = true;
            }

            if (m_bCheck == false)
                info.Status = 2;
        }

        private void UpdateRecentTankLevelAlarm(int nTankID, int nHistoryID)
        {
            string szTemp = "UPDATE recenttankalarmhistory SET LevelAlarmHistoryID = {0} WHERE TankID = {1}";
            string szSQL = string.Format(szTemp, nHistoryID, nTankID);
            mDBMgr.GetResultData(szSQL, 0);
        }

        private void UpdateRecentTankTempAlarm(int nTankID, int nHistoryID)
        {
            string szTemp = "UPDATE recenttankalarmhistory SET TempAlarmHistoryID = {0} WHERE TankID = {1}";
            string szSQL = string.Format(szTemp, nHistoryID, nTankID);
            mDBMgr.GetResultData(szSQL, 0);
        }

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select MAX(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }
        
        
        //1	신호없음
        //2	정상상태
        //3	레벨알람
        //4	온도알람
        //5	온도레벨알람

        private int SaveAlarmHistory(int nTankID, int nAlarmType, int nStatus, VariousData<float> temp, VariousData<float> level)
        {
            int nAlarm = CheckAlarmStatus(nTankID, nStatus);
            if (nAlarm >= 0)
                return nAlarm;

            string strTemp = temp == null ? "NULL" : string.Format("{0:F1}", temp.Data);
            string strLevel = level == null ? "NULL" : string.Format("{0:F1}", level.Data);

            //int nStatus = (nAlarmType == 0 ? 3 : 4);
            int nMaxID = GetMaxID("tankalarmhistory", mDBMgr) + 1;
            string szTemp = "INSERT INTO tankalarmhistory (ID, TankID, BeginTime, EndTime, Status, AlarmTerminator, AlarmType, TempLog, LevelLog ) " +
                 " VALUES ({0},{1},'{2}',NULL,{3},NULL,{4}, {5}, {6})";
            string szDate = WebDBManager.MakeDateTimeString(DateTime.Now);
            string szSQL = string.Format(szTemp, nMaxID, nTankID, szDate, nAlarmType, nAlarmType, strTemp, strLevel);

            mDBMgr.GetResultData(szSQL, 0);

            return nMaxID;
        }

        private int CheckAlarmStatus(int nTankID, int nStatus)
        {
            if (nStatus == 0)
            {
                string szTemp = "SELECT LevelAlarmHistoryID FROM recenttankalarmhistory WHERE TankID = {0}";
                string szSQL = string.Format(szTemp, nTankID);
                ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
                if( arResult != null && arResult.Count > 0)
                {
                    int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                    return nID;
                }
            }              
            else
            {
                string szTemp = "SELECT TempAlarmHistoryID FROM recenttankalarmhistory WHERE TankID = {0}";
                string szSQL = string.Format(szTemp, nTankID);
                ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
                if (arResult != null && arResult.Count > 0)
                {
                    int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                    return nID;
                }
            }
            return -1;
        }


        private Queue<JubixCommand> m_QueCmd = new Queue<JubixCommand>();
        public Queue<JubixCommand> CommandQue
        {
            get { return m_QueCmd; }
        }

        public void ReadAllTankCommand()
        {
            m_QueCmd.Clear();
            // read all command
            string szSQL = "SELECT cmd.ID, cmd.CommandType, cmd.TimeStamp, cmd.UserID, cmh.ID, cmd.TankID  FROM command as cmd " +
                           "inner join commandhistory as cmh on cmh.CmdID = cmd.ID " +
                           " WHERE cmd.CommandType = 2 OR cmd.CommandType = 3";
            
            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return;

            for (int i = 0; i < arResult.Count - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nCmd = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arResult[i + 2]);
                int nUserID = WebDBManager.GetIntField(arResult[i + 3].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);
                VariousData<int> nTankID = WebDBManager.GetIntField(arResult[i + 5].ToString());
                if (nID > 0)
                {
                    JubixCommand cmd = new JubixCommand();
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
                    m_QueCmd.Enqueue(cmd);
                }
            }
        }

        public void RemoveTankCommand(JubixCommand cmd)
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

        private SortedList<int, TankInfo> m_dicTankList = new SortedList<int, TankInfo>();
        public void ClearTankAlarm(JubixCommand cmd)
        {
            if (cmd == null)
                return;

            TankInfo sensor = null;
            if (m_dicTankList.TryGetValue(cmd.TankID, out sensor))
            {
                if(cmd.Command == 2)
                {
                    string szTemp = "UPDATE recenttankalarmhistory SET LevelAlarmHistoryID = NULL WHERE TankID = {0}";
                    string szSQL = string.Format(szTemp, cmd.TankID);
                    mDBMgr.GetResultData(szSQL, 0);

                    if( sensor.Status == 7)
                    {
                        sensor.Status = 5;
                    }
                    if (sensor.Status == 6)
                    {
                        sensor.Status = 4;
                    }
                    if (sensor.Status == 3)
                    {
                        sensor.Status = 0;
                    }
                }
                else if(cmd.Command == 3)
                {
                    string szTemp = "UPDATE recenttankalarmhistory SET TempAlarmHistoryID = NULL WHERE TankID = {0}";
                    string szSQL = string.Format(szTemp, cmd.TankID);
                    mDBMgr.GetResultData(szSQL, 0);

                    if (sensor.Status == 6)
                        sensor.Status = 3;
                    if (sensor.Status == 7)
                        sensor.Status = 3;
                    if (sensor.Status == 4)
                        sensor.Status = 0;                   
                    if (sensor.Status == 5)
                        sensor.Status = 0;
                }
                string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
                string szTemp2 = "UPDATE tankalarmhistory SET EndTime= '{0}', AlarmTerminator={1} WHERE TankID = {2} AND EndTime is NULL";
                string szSQL2 = string.Format(szTemp2, szEndDate, (cmd.UserID > 0 ? cmd.UserID.ToString() : "NULL"), cmd.TankID);
                mDBMgr.GetResultData(szSQL2, 0);
                
            }
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

        private string m_szName;
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        private string m_szLiquidType;
        public string LiquidType
        {
            get { return m_szLiquidType; }
            set { m_szLiquidType = value; }
        }

        private float m_fCapacity;
        public float Capacity
        {
            get { return m_fCapacity; }
            set { m_fCapacity = value; }
        }

        private float m_fHighLevel;
        public float HighLevel
        {
            get { return m_fHighLevel; }
            set { m_fHighLevel = value; }
        }

        private float m_fMinTemp;

        public float MinTemp
        {
            get { return m_fMinTemp; }
            set { m_fMinTemp = value; }
        }

        private float m_fMaxTemp;
        public float MaxTemp
        {
            get { return m_fMaxTemp; }
            set { m_fMaxTemp = value; }
        }

        private float m_fLevel;
        public float Level
        {
            get { return m_fLevel; }
            set { m_fLevel = value; }
        }

        private float m_fTemperature;
        public float Temperature
        {
            get { return m_fTemperature; }
            set { m_fTemperature = value; }
        }
        private float m_fDensity;
        public float Density
        {
            get { return m_fDensity; }
            set { m_fDensity = value; }
        }
        private float m_fMass;
        public float Mass
        {
            get { return m_fMass; }
            set { m_fMass = value; }
        }

        private float m_fFlow;
        public float Flow
        {
            get { return m_fFlow; }
            set { m_fFlow = value; }
        }

        private float m_fGrossVolume;
        public float GrossVolume
        {
            get { return m_fGrossVolume; }
            set { m_fGrossVolume = value; }
        }

        private float m_fNetVolume;
        public float NetVolume
        {
            get { return m_fNetVolume; }
            set { m_fNetVolume = value; }
        }

        private float m_fPressure;
        public float Pressure
        {
            get { return m_fPressure; }
            set { m_fPressure = value; }
        }

        private int m_nStatus;
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        private int m_nLevelAddress;
        public int LevelAddress
        {
            get { return m_nLevelAddress; }
            set { m_nLevelAddress = value; }
        }

        private int m_nTempAddress;
        public int TempAddress
        {
            get { return m_nTempAddress; }
            set { m_nTempAddress = value; }
        }

        private int m_nGrossVolumeAddress;
        public int GrossVolumeAddress
        {
            get { return m_nGrossVolumeAddress; }
            set { m_nGrossVolumeAddress = value; }
        }

        private int m_nNetVolumeAddress;
        public int NetVolumeAddress
        {
            get { return m_nNetVolumeAddress; }
            set { m_nNetVolumeAddress = value; }
        }

        private int m_nMassAddress;
        public int MassAddress
        {
            get { return m_nMassAddress; }
            set { m_nMassAddress = value; }
        }

        private int m_nFlowAddress;
        public int FlowAddress
        {
            get { return m_nFlowAddress; }
            set { m_nFlowAddress = value; }
        }

        private int m_nPressureAddress;
        public int PressureAddress
        {
            get { return m_nPressureAddress; }
            set { m_nPressureAddress = value; }
        }


        private int m_nPrevHistoryID = -1;
        public int PrevHistoryID
        {
            get { return m_nPrevHistoryID; }
            set { m_nPrevHistoryID = value; }
        }

        private int m_nPrevEventType = 0;
        public int PrevEventType
        {
            get { return m_nPrevEventType; }
            set { m_nPrevEventType = value; }
        }


    }
}
