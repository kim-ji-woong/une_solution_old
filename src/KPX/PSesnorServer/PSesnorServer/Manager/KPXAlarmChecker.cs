using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

using DBUtility;
using JubixNetwork;

namespace PSensorServer
{

    public class KPXAlarmChecker
    {
        private static object m_lock = new object();
        private static KPXAlarmChecker m_Instance = null;
        public static KPXAlarmChecker Instance
        {
            get
            {
                lock (m_lock)
                {
                    if (m_Instance == null)
                        m_Instance = new KPXAlarmChecker();
                    return KPXAlarmChecker.m_Instance;
                }
               
            }
        }

        // KPX : 500
        private int m_nSiteID = 500;
        public int SiteID
        {
            get { return m_nSiteID; }
        }

        private KPXAlarmManager m_alarmManager = new KPXAlarmManager();
        public KPXAlarmManager AlarmManager
        {
            get { return m_alarmManager; }
        }

        private KPXWorkManager m_workManager = new KPXWorkManager();
        public KPXWorkManager WorkManager
        {
            get { return m_workManager; }
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }
        
        private SortedList<int, TankInfo> m_dicSensors = null;
        private List<TankInfo> m_TankList = new List<TankInfo>();
        public List<TankInfo> TankList
        {
            get { return m_TankList; }
        }

        private SortedList<int, PipeSensor> m_dicPipes = null;
        private List<PipeSensor> m_PipeList = new List<PipeSensor>();
        public List<PipeSensor> PipeList
        {
            get { return m_PipeList; }
        }

        private Queue<JubixCommand> m_QueCmd = new Queue<JubixCommand>();
        public Queue<JubixCommand> CommandQue
        {
            get { return m_QueCmd; }
        }

        //private libSMS.IMessageClient client = null;
        //public libSMS.IMessageClient SMSClient
        //{
        //    get { return client; }
        //}
        
        private bool m_bChangedData = false;
        private SortedList<int, AlarmOption> m_AlarmOptions = new SortedList<int, AlarmOption>();

        public SortedList<int, AlarmOption> AlarmOptions
        {
            get { return m_AlarmOptions; }
            set { m_AlarmOptions = value; }
        }

        private SortedList<int, PipeAlarmOption> m_PipeAlarmOptions = new SortedList<int, PipeAlarmOption>();

        public SortedList<int, PipeAlarmOption> PipeAlarmOptions
        {
            get { return m_PipeAlarmOptions; }
            set { m_PipeAlarmOptions = value; }
        }
        
        private KPXAlarmChecker()
        {
            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);

            ReadPipe();
            ReadTank();

            ReadAlarmOption();
            ReadPipeAlarmOption();

            m_bChangedData = false;          

            //client = libSMS.MessageClientFactory.CreateMessageClient(m_nSiteID, "127.0.0.1");
            //client.SendSMS("01052672290", "01043632290", "[KPX]테스트메시지");
        }


        private bool m_bReady = false;
        internal bool ReadyToRead()
        {
            return m_bReady;
        }

        private Thread m_CheckThread = null;
        private bool m_bExitThread = false;

        public void BeginThread()
        {
            m_bExitThread = false;
            m_CheckThread = new Thread(CheckAlarm);
            m_CheckThread.Name = "alarmCheckThread";
            m_CheckThread.Start();

            if (m_alarmManager != null)
                m_alarmManager.BeginThread();


            m_workManager.ReadAllWorkHistory(this.m_TankList);

            m_bReady = true;
        }

        public void ReleaseThread()
        {
            m_bExitThread = true;
            try
            {
                if(m_CheckThread != null)
                {
                    m_CheckThread.Join();
                    m_CheckThread = null;
                }

            }
            catch(Exception)
            { }


            if (m_alarmManager != null)
                m_alarmManager.ReleaseThread();
        }


        public void ReadSiteID()
        {
            DBUtility.Utility iniFile = new DBUtility.Utility("KPXConfig.ini");
            string szSiteID = iniFile.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 500;
            if (szSiteID.Length > 0)
            {
                int.TryParse(szSiteID, out nSiteID);

            }
            m_nSiteID = nSiteID;
        }
        
        private void ReadPipe()
        {
            m_PipeList = JubixNetwork.JubixSensorManager.Instance.SensorList;

            //m_PipeList.Clear();
            //foreach (PipeSensor item in JubixNetwork.JubixSensorManager.Instance.SensorList)
            //{
            //    m_PipeList.Add(item);
            //}            
            //foreach (PipeSensor item in Netowrk.KPXParagonManager.Instance.PipeList)
            //{
            //    m_PipeList.Add(item);
            //}

            m_dicPipes = JubixNetwork.JubixSensorManager.Instance.DicPipeList;
            //foreach (KeyValuePair<int, PipeSensor> item in Netowrk.KPXParagonManager.Instance.DicPipeList)
            //{
            //    m_dicPipes[item.Key] = item.Value;
            //}
        }

        private void ReadTank()
        {
            m_TankList = KPXLevelMeterManager.Instance.TankList;
            m_dicSensors = KPXLevelMeterManager.Instance.DicTankList;
        } 

        private PipeSensor GetPipe(int nPipeID)
        {
            PipeSensor pipe = null;
            m_dicPipes.TryGetValue(nPipeID, out pipe);
            return pipe;
        }


        private TankInfo GetTank(int nTankID)
        {
            TankInfo tank = null;
            m_dicSensors.TryGetValue(nTankID, out tank);
            return tank;
        }
        
        
        public void CheckAlarm()
        {
            while(!m_bExitThread)
            {
                ReadPipe();
                ReadTank(); 

                foreach (PipeSensor info in m_PipeList)
                {
                    info.Status = 0;
                }

                foreach (TankInfo info in m_TankList)
                { 
                    info.Status = 0;
                }                

                DateTime dtTime = DateTime.Now;
                
                // 작업 목록 가져오기
                List<WorkInfo> workList = m_workManager.GetAllWorks();
                if( workList.Count > 0)
                {
                    foreach(WorkInfo info in workList)
                    {
                        if (info.BeginCheck == false)
                            continue;

                        // 압력 유량 값 가져오기
                        int nPipeID = info.PipeID;
                        int nTankID = info.TankID;

                        float pressureValue = 0.0f;
                        float flowValue = 0.0f;

                        bool bAlarmIgnore = false;
                        // 알람 무시 목록에 있는지 체크
                        AlarmIgnore ignore = m_alarmManager.FindAlarmIgnore(nTankID);
                        if( ignore != null)
                        {
                            bAlarmIgnore = true;
                        }

                        AlarmOption option = GetAlarmOption(nTankID);

                        if( option != null)
                        {
                            int nPipeStatus = 0;
                            if (nPipeID > 0)
                            {
                                PipeAlarmOption optionPipe = GetPipeAlarmOption(nPipeID);
                                if (optionPipe == null)
                                    continue;

                                AlarmInfo alrm = new AlarmInfo();
                                alrm.BeginTime = dtTime;
                                alrm.PipeID = nPipeID;
                                alrm.TankID = nTankID;
                                alrm.AlarmType = 0;

                                PipeSensor pipe = GetPipe(nPipeID);                                                                
                                if (pipe != null)
                                {
                                    alrm.RealValue = pipe.CurrentValue;
                                    pressureValue = pipe.CurrentValue;
                                    // 압력 체크
                                    float fPressure = pipe.CurrentValue;
                                    float fPrevPressure = pipe.PrevValue;

                                    float fStablePressure = info.StablePressure;
                                    

                                    if(option.PipeStableCTimeUse == 1)
                                    {
                                        if (CheckStablePressure(info, dtTime, optionPipe))
                                        {
                                            SaveStablePressure(info, fPressure, dtTime);
                                        }

                                        fPrevPressure = fStablePressure;
                                    }
                                    else
                                    {
                                        SaveStablePressure(info, fPrevPressure, dtTime);
                                    }


                                    float fDiff = fPressure - fPrevPressure;

                                    // 이전 압력 값이 있는 경우
                                    if (pipe.PrevValue > 0.0f)
                                    {
                                        // 비율인 경우
                                        if (optionPipe.PipeStableType == 0)
                                        {
                                            float fCheckValue = optionPipe.PipeStableRatio * fPrevPressure / 100;
                                            if (Math.Abs(fDiff) > Math.Abs(fCheckValue))
                                            {
                                                alrm.StandardValue = fPrevPressure;
                                                alrm.StandardRange = fCheckValue;

                                                // 알람
                                                if (fDiff < 0)
                                                {
                                                  
                                                    alrm.AlarmType = (int)(AlarmType.압력하강);
                                                    pipe.Status = (int)(AlarmType.압력하강);
                                                    nPipeStatus = (int)(AlarmType.압력하강);
                                                }
                                                else
                                                {
                                                    alrm.AlarmType = (int)(AlarmType.압력상승);
                                                    pipe.Status = (int)(AlarmType.압력상승);
                                                    nPipeStatus = (int)(AlarmType.압력상승);

                                                }
                                            }
                                        }
                                        // 절대값
                                        else if (optionPipe.PipeStableType == 1)
                                        {
                                            float fCheckValue = optionPipe.PipeStableAbsolute;
                                            if (Math.Abs(fDiff) > Math.Abs(fCheckValue))
                                            {
                                                alrm.StandardValue = fPrevPressure;
                                                alrm.StandardRange = fCheckValue;
                                                // 알람
                                                if (fDiff < 0)
                                                {
                                                    alrm.AlarmType = (int)(AlarmType.압력하강);
                                                    pipe.Status = (int)(AlarmType.압력하강);
                                                    nPipeStatus = (int)(AlarmType.압력하강);
                                                }
                                                else
                                                {
                                                    alrm.AlarmType = (int)(AlarmType.압력상승);
                                                    pipe.Status = (int)(AlarmType.압력상승);
                                                    nPipeStatus = (int)(AlarmType.압력상승);
                                                }
                                            }
                                        }
                                    }
                                }

                                // 배관/탱크 알람 체크                            
                                if (bAlarmIgnore == false)
                                {
                                    AddAlarmInternal(nTankID, nPipeID, alrm, option, dtTime);                                      
                                } 
                            }

                           
                            if(KPXLevelMeterManager.Instance.DataEnabled == true)
                            {
                                if( nTankID > 0)
                                {
                                    AlarmInfo alrm = new AlarmInfo();
                                    alrm.BeginTime = dtTime;
                                    alrm.PipeID = nPipeID;
                                    alrm.TankID = nTankID;
                                    alrm.AlarmType = 0;
                                    // 작업 목록 에 있는 배관 탱크의 Flow 값 체크
                                    TankInfo tank = GetTank(nTankID);
                                    if (tank != null)
                                    { 
                                        tank.Status = 0;
                                        alrm.RealValue = tank.Flow;
                                        // 유량 체크
                                        float fFlow = tank.Flow;
                                        flowValue = fFlow;
                                        float fPrevFlow = tank.PrevFlow;

                                        float fStableFlow = info.StableFlow;

                                        if (option.TankStableCTimeUse == 1)
                                        {
                                            if (CheckStableFlow(info, dtTime, option))
                                            {
                                                if (fFlow != -999.0f)
                                                    SaveStableFlow(info, fFlow, dtTime);
                                                else
                                                    SaveStableFlow(info, -9999.0f, dtTime);
                                            }

                                            fPrevFlow = fStableFlow;
                                        }
                                        else
                                        {
                                            if( fPrevFlow != -999.0f)
                                                SaveStableFlow(info, fPrevFlow, dtTime);
                                            else
                                                SaveStableFlow(info, -9999.0f, dtTime);
                                        }

                                        float fDiff = fFlow - fPrevFlow;

                                        // 이전 유량 값이 있는 경우
                                        if (fPrevFlow != 0.0f && tank.PrevFlow != -999.0f && tank.PrevFlow != -9999.0f)
                                        {
                                            // 비율인 경우
                                            if (option.TankStableType == 0)
                                            {
                                                float fCheckValue = option.TankStableRatio * fPrevFlow / 100;
                                                if (Math.Abs(fDiff) > Math.Abs(fCheckValue))
                                                {
                                                    alrm.StandardValue = fPrevFlow;
                                                    alrm.StandardRange = fCheckValue;
                                                    // 알람
                                                    if (fDiff > 0)
                                                    {
                                                        alrm.AlarmType = (int)(AlarmType.탱크배관유량증가);
                                                        tank.Status = (int)(AlarmType.탱크배관유량증가);

                                                    }
                                                    else
                                                    {
                                                        alrm.AlarmType = (int)(AlarmType.탱크배관유량감소);
                                                        tank.Status = (int)(AlarmType.탱크배관유량감소);
                                                    }
                                                }
                                            }
                                            // 절대값
                                            else if (option.TankStableType == 1)
                                            {
                                                float fCheckValue = option.TankStableAbsolute;

                                                if (Math.Abs(fDiff) > Math.Abs(fCheckValue))
                                                {
                                                    alrm.StandardValue = fPrevFlow;
                                                    alrm.StandardRange = fCheckValue;

                                                    // 알람
                                                    if (fDiff > 0)
                                                    {
                                                        alrm.AlarmType = (int)(AlarmType.탱크배관유량증가);
                                                        tank.Status = (int)(AlarmType.탱크배관유량증가);
                                                    }
                                                    else
                                                    {
                                                        alrm.AlarmType = (int)(AlarmType.탱크배관유량감소);
                                                        tank.Status = (int)(AlarmType.탱크배관유량감소);
                                                    }
                                                }
                                            }
                                        }
                                        tank.Status += nPipeStatus;

                                            
                                    }
                                     
                                    // 배관/탱크 알람 체크                            
                                    if (bAlarmIgnore == false)
                                    {
                                        if (alrm.RealValue != -999.0f && alrm.RealValue != -9999.0f)
                                        {
                                            if (alrm.StandardValue != -999.0f && alrm.StandardValue != -9999.0f)
                                                AddAlarmInternal(nTankID, nPipeID, alrm, option, dtTime);
                                        }
                                    } 
                                }
                                
                            }

                            if (info != null)
                                info.AddSensorValue(pressureValue, flowValue);
                            
                            
                        }                        
                    }
                }

                // 탱크알람 체크                
                foreach (TankInfo info in m_TankList)
                {
                    CheckAlarm(info, dtTime);
                }

                // 알람상태에서 해제상태로 변경될때
                //foreach (TankInfo info in m_TankList)
                //{
                //    if( info.PrevStatus != info.Status && info.Status == 0)
                //    {
                //         UpdateTankStatus(info);
                //    }                    
                //}  

                if (KPXLevelMeterManager.Instance.DataEnabled == true)
                {
                    foreach (TankInfo info in m_TankList)
                    {
                        UpdateTankInfo(info);
                        SaveFlowHistory(info, dtTime);

                        if (KPXLevelMeterManager.Instance.Simulation == true)
                        {
                            info.Flow = info.Flow;
                        }
                    }
                }

                foreach (PipeSensor info in m_PipeList)
                {
                    UpdatePipeInfo(info);
                    SavePipeHistory(info, dtTime);

                    if (KPXLevelMeterManager.Instance.Simulation == true)
                    {
                        info.SetSensorValue(info.CurrentValue*100);
                    }
                }
                
                for(int i  = 0 ;  i < 30 ; i++)
                {
                    if (m_bExitThread == true)
                        break;

                    Thread.Sleep(100);
                }
            }           
        }

        private void SaveFlowHistory(TankInfo info, DateTime dtNow)
        {
            string szTableName = info.GetCurrentTableName();
            string szKeyTemp = DBUtil.GetKeyTimeString(dtNow);

            // 1년 지난 데이터 지우기 - 데이터를 못받는 상황이어도 이전 데이터는 삭제해야함 
            string szTemp2 = "DELETE FROM {0} WHERE KeyTime >= {1}";
            string szSQL2 = string.Format(szTemp2, szTableName, szKeyTemp);
            m_dbMgr.GetResultData(szSQL2, 0);

            // 데이터가 없는 경우 세가지값 모두 -999
            if (info.Flow == -999.0f && info.Temperature == -999.0f && info.Level == -999.0f)
                return;
            
            string szTemp = "INSERT INTO {0} (KeyTime, flow, temperture, level) VALUES ({1}, {2}, {3}, {4})";            
            string szSQL = string.Format(szTemp, szTableName, szKeyTemp, info.Flow, info.Temperature, info.Level);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void SavePipeHistory(PipeSensor info, DateTime dtNow)
        {
            float value = info.CurrentValue;
            WebDBManager dbManager = m_dbMgr;
            
            string szTableName = TablePartition.GetTableNames(info.PipeID, dtNow);
            string szKeyTime = DBUtil.GetKeyTimeString(dtNow);

            // 1년 지난 데이터 지우기
            DateTime dtBefore = dtNow.AddYears(-1);
            string strQuery = string.Format("DELETE FROM {0} WHERE timestamp < '{1}'", szTableName, dtNow.AddMonths(-1).ToString("yyyy-MM") + "-01 00:00:00");
            dbManager.GetResultData(strQuery, 0);

            int nMaxID = DBUtil.GetMaxID(szTableName, dbManager) + 1;
            string szDate = WebDBManager.MakeDateTimeString(dtNow);

            
            string szTemp2 = "INSERT " + szTableName + " (ID, TimeStamp, Pressure, KeyTime) VALUES ({0},'{1}',{2},'{3}')";
            string szSQL2 = string.Format(szTemp2, nMaxID, szDate, value, szKeyTime);

            dbManager.GetResultData(szSQL2, 0); 
        }

        private void UpdateTankStatus(TankInfo info)
        {
            string szTemp = "UPDATE tank SET Status = 0 WHERE ID = {0}";
            string szSQL = string.Format(szTemp, info.ID);
            m_dbMgr.GetResultData(szSQL, 0);
        }
        
        private void UpdateTankInfo(TankInfo info)
        {
            string szTemp = "UPDATE tank SET Status = {0}, Flow = {1}, PrevFlow={2}, Temperature={3}, PrevTemperature={4}, Level={5}, PrevLevel={6} WHERE ID = {7}";
            string szSQL = string.Format(szTemp, info.Status, info.Flow, info.PrevFlow,info.Temperature, info.PrevTemperature, info.Level, info.PrevLevel, info.ID);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void UpdatePipeInfo(PipeSensor info)
        {
            string szTemp = "UPDATE pipe SET Status = {0}, Pressure = {1}, PrevPressure={2} WHERE ID = {3}";
            string szSQL = string.Format(szTemp, info.Status, info.CurrentValue, info.PrevValue, info.PipeID);
            m_dbMgr.GetResultData(szSQL, 0);
        }


        private bool m_bAutoStartTankOnlyWork = true;

        private void CheckAlarm(TankInfo info, DateTime dtTime)
        {  
            // 현재 온도
            float temp = info.Temperature;
            float fLevel = info.Level;
            int nStatus = info.Status;

            AlarmOption option = GetAlarmOption(info.ID);
            if (option == null)
                return;

            if (temp >= info.MaxTemp)
            {
                AlarmType type = AlarmType.온도상승;

                AlarmInfo alrm = new AlarmInfo();
                alrm.BeginTime = dtTime;
                alrm.PipeID = -1;
                alrm.TankID = info.ID;
                alrm.AlarmType = (int)type;
                alrm.RealValue = temp;

                alrm.StandardValue = info.MaxTemp;
                alrm.StandardRange = 0;

                nStatus += (int)type;
                
                AddAlarmInternal(info.ID, -1, alrm, option, dtTime);
                
            }
            // 절대온도 0은 -273.15도
            else if (temp < info.MinTemp && temp > -274.0f)
            {
                AlarmType type = AlarmType.온도하강;

                AlarmInfo alrm = new AlarmInfo();
                alrm.BeginTime = dtTime;
                alrm.PipeID = -1;
                alrm.TankID = info.ID;
                alrm.AlarmType = (int)type;
                alrm.RealValue = temp;
                nStatus += (int)type;

                alrm.StandardValue = info.MinTemp;
                alrm.StandardRange = 0;

                AddAlarmInternal(info.ID, -1, alrm, option, dtTime);
                
            }   
            // 레벨 체크
            if (fLevel >= info.HighLevel)
            {
                AlarmType type = AlarmType.최고레벨;

                AlarmInfo alrm = new AlarmInfo();
                alrm.BeginTime = dtTime;
                alrm.PipeID = -1;
                alrm.TankID = info.ID;
                alrm.AlarmType = (int)type;
                alrm.RealValue = fLevel;
                nStatus += (int)type;

                alrm.StandardValue = info.HighLevel;
                alrm.StandardRange = 0;

                AddAlarmInternal(info.ID, -1, alrm, option, dtTime);                
            }

            if( info.LiquidType == "PO" || info.LiquidType == "황산")
            {
                WorkInfo work = m_workManager.GetWork(info.ID, -1);
                if( work == null)
                {

                    if (info.Flow > info.AutoStartFlow || info.Flow < -info.AutoStartFlow)
                    {
                        if( info.Flow != -999.0f && info.Flow != -9999.0f)
                        {
                            // 작업 시작
                            if (info.UseAutoStart == 1)
                            {
                                int nLinkData = -100;

                                // 황산 -200
                                if (info.LiquidType == "황산")
                                    nLinkData = -200;
                                // PO -100
                                JubixCommand cmd = new JubixCommand();
                                cmd.TankID = info.ID;
                                cmd.PipeID = -1;
                                cmd.UserID = 1;
                                cmd.CreateTime = dtTime;

                                if (m_bAutoStartTankOnlyWork)
                                {
                                    BeginWork(cmd, nLinkData);
                                }
                            }
                        }
                       
                                            

                    }
                }
            }
            
            // ToDo : 유량 체크 추가
            bool bTankLeakCheck = false;

            if (bTankLeakCheck == true)
            {
                if (!m_workManager.FindWorkTank(info.ID))
                {
                    #region 탱크유량 검사
                    //AlarmInfo alrm = new AlarmInfo();
                    //alrm.BeginTime = dtTime;
                    //alrm.PipeID = -1;
                    //alrm.TankID = info.ID;


                    //if( option != null)
                    //{

                    //    TankInfo tank = info;

                    //    alrm.RealValue = tank.Flow;
                    //    // 유량 체크
                    //    float fFlow = tank.Flow;
                    //    float fPrevFlow = tank.PrevFlow;

                    //    float fDiff = fFlow - fPrevFlow;

                    //    // 이전 유량 값이 있는 경우
                    //    if (fPrevFlow > 0.0f)
                    //    {
                    //        // 비율인 경우
                    //        if (option.TankStableType == 0)
                    //        {
                    //            float fCheckValue = option.TankStableRatio * fPrevFlow / 100;
                    //            if (Math.Abs(fDiff) > fCheckValue)
                    //            {
                    //                alrm.StandardValue = fPrevFlow;
                    //                alrm.StandardRange = fCheckValue;

                    //                // 알람
                    //                if (fDiff > 0)
                    //                {
                    //                    alrm.AlarmType = (int)(AlarmType.탱크유량증가);
                    //                    nStatus += (int)(AlarmType.탱크유량증가);
                    //                }
                    //                else
                    //                {
                    //                    alrm.AlarmType = (int)(AlarmType.탱크유량감소);
                    //                    nStatus += (int)(AlarmType.탱크유량감소);
                    //                }
                    //            }
                    //        }
                    //        // 절대값
                    //        else if (option.TankStableType == 1)
                    //        {
                    //            float fCheckValue = option.TankStableAbsolute;
                    //            if (Math.Abs(fDiff) > fCheckValue)
                    //            {

                    //                alrm.StandardValue = fPrevFlow;
                    //                alrm.StandardRange = fCheckValue;

                    //                // 알람
                    //                if (fDiff > 0)
                    //                {
                    //                    alrm.AlarmType = (int)(AlarmType.탱크유량증가);
                    //                    nStatus += (int)(AlarmType.탱크유량증가);
                    //                }
                    //                else
                    //                {
                    //                    alrm.AlarmType = (int)(AlarmType.탱크유량감소);
                    //                    nStatus += (int)(AlarmType.탱크유량감소);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //// Add Alarm
                    //if (option.AlarmIntervalUse == 1)
                    //{
                    //    if (!m_alarmManager.CheckAlarmClearTime(info.ID, -1, (int)alrm.AlarmType,  option.AlarmInterval, dtTime))
                    //    {
                    //        // add alarm
                    //        if (alrm.AlarmType > 0)
                    //        {
                    //            if (m_alarmManager.AddAlarm(alrm))
                    //                SendSMS(info.Name, (AlarmType)alrm.AlarmType);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (alrm.AlarmType > 0)
                    //    {
                    //        if (m_alarmManager.AddAlarm(alrm))
                    //            SendSMS(info.Name, (AlarmType)alrm.AlarmType);
                    //    }
                    //}
                    #endregion

                    AlarmInfo alrm = new AlarmInfo();
                    alrm.BeginTime = dtTime;
                    alrm.PipeID = -1;
                    alrm.TankID = info.ID;

                    // 유량값이 0인경우만 체크할 것인지 여부
                    if (info.CheckLeak == true && info.Flow == 0.0f)
                    {
                        // 알람값 설정
                        if (info.CheckStart == true)
                        {
                            TimeSpan sp = dtTime - info.BeginCheckTime;
                            if (sp.TotalSeconds < info.LevelTime)
                            {
                                float fDiffLevel = Math.Abs(info.BeginLeakCheckLevel - fLevel);
                                if (fDiffLevel > info.LeakLevel)
                                {
                                    // 알람 발생

                                    alrm.StandardValue = info.LeakLevel;
                                    alrm.StandardRange = 0;

                                    alrm.AlarmType = (int)(AlarmType.탱크유량감소);
                                    nStatus += (int)(AlarmType.탱크유량감소);
                                }
                            }
                            else
                            {
                                // 시간이 넘었으므로 다음 체크로 변경
                                info.CheckStart = false;
                            }
                        }
                        else
                        {
                            info.BeginCheckTime = dtTime;
                            info.BeginLeakCheckLevel = fLevel;
                            info.CheckStart = true;
                        }

                        info.LastLeakCheckLevel = fLevel;
                        info.LastCheckTime = dtTime;
                    }

                    AddAlarmInternal(info.ID, -1, alrm, option, dtTime);

                }
                else
                {
                    // 작업중인경우에는 체크하지 않는다.
                    info.LastCheckTime = dtTime;
                    info.CheckStart = false;
                }
            }
            
            info.Status = nStatus;
        }

        private void AddAlarmInternal(int nTankID, int nPipeID, AlarmInfo alrm, AlarmOption option, DateTime dtTime)
        {
            if (alrm.RealValue != -999.0f && alrm.RealValue != -9999.0f)
            {
                if (alrm.StandardValue != -999.0f && alrm.StandardValue != -9999.0f)
                {
                    if (alrm.AlarmType > 0)
                    {
                        // Add Alarm
                        bool bAddAlarm = false;
                        if (option.AlarmIntervalUse == 1)
                        {
                            if (!m_alarmManager.CheckAlarmClearTime(nTankID, nPipeID, (int)alrm.AlarmType, option.AlarmInterval, dtTime))
                            {
                                bAddAlarm = true;
                            }
                        }
                        else
                        {
                            bAddAlarm = true;
                        }

                        if (bAddAlarm == true)
                            m_alarmManager.AddAlarm(alrm);
                    }
                }
            }            
        }
       
        private bool CheckStableFlow(WorkInfo info, DateTime dtTime, AlarmOption otption)
        {
            if(info.FirstFlowStableCheck == true)
            {
                info.FirstFlowStableCheck = false;
                return true;
            }

            TimeSpan span = dtTime - info.StableFlowTime;
            if( span.TotalSeconds >( otption.TankStableCTime * 60))
            {
                // 기능수정 : 알람무시와 정상범위 업데이트는 연계하지 않음, 2017-11-08 skkim
                //return true;
                // 알람 무시 구간에는 업데이트 되지 않음
                if (m_alarmManager.FindAlarmIgnore(info.TankID) == null)
                {
                    return true;
                }
                return true;
            }
            return false;
        }

        private void SaveStableFlow(WorkInfo info, float fFlow, DateTime dtTime)
        {
            info.StableFlow = fFlow;
            info.StableFlowTime = dtTime;
            string szDT = DBUtility.WebDBManager.MakeDateTimeString(dtTime);
            string szTemp = "UPDATE lastworkhistory SET StandardFlow = {0}, StandardFlowUpdateTime='{3}' WHERE TankID = {1} and PipeID {2}";
            string szPipeID  = info.PipeID > 0 ? "= " + info.PipeID.ToString() : "is NULL";
            string szSQL = string.Format(szTemp, fFlow, info.TankID, szPipeID, szDT);
            m_dbMgr.GetResultData(szSQL, 0);            
        }

        private void SaveStablePressure(WorkInfo info, float fPressure, DateTime dtTime)
        {
            info.StablePressure = fPressure;
            info.StablePressureTime = dtTime;
            string szDT = DBUtility.WebDBManager.MakeDateTimeString(dtTime);
            string szTemp = "UPDATE lastworkhistory SET StandardPressure = {0}, StandardPressureUpdateTime='{3}' WHERE TankID = {1} and PipeID {2}";
            string szPipeID = info.PipeID > 0 ? "= " + info.PipeID.ToString() : "is NULL";
            string szSQL = string.Format(szTemp, fPressure, info.TankID, szPipeID, szDT);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private bool CheckStablePressure(WorkInfo info, DateTime dtTime, PipeAlarmOption otption)
        {
            if (info.FirstPressureStableCheck == true)
            {
                info.FirstPressureStableCheck = false;
                return true;
            }

            TimeSpan span = dtTime - info.StablePressureTime;
            if (span.TotalMinutes > (otption.PipeStableCTime))
            {
                if (m_alarmManager.FindAlarmIgnore(info.TankID) == null)
                {
                    return true;
                }
                return true;
            }
            return false;
        }       

        public void ReadAllCommand()
        {
            m_QueCmd.Clear();

            // read all command
            string szSQL = "SELECT cmd.ID, cmd.CommandType, cmd.TimeStamp, cmd.PipeID, cmd.TankID, cmd.UserID, cmd.CommandName, cmd.CommandValue, " + 
                           " cmh.ID, cmh.AlarmHistoryID, alarmOccurType, alarmComment  FROM command AS cmd INNER JOIN commandhistory as cmh ON cmh.CmdID = cmd.ID";
   
            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return;

            for (int i = 0; i < arResult.Count - 9; i += 12)
            {
                int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nCmdType = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arResult[i + 2]);

                VariousData<int> nPipeID = WebDBManager.GetIntField(arResult[i + 3].ToString());
                VariousData<int> nTankID = WebDBManager.GetIntField(arResult[i + 4].ToString());

                int nUserID = WebDBManager.GetIntField(arResult[i + 5].ToString(), -1);

                string szCommandName = WebDBManager.GetStringField(arResult[i + 6].ToString());
                string szCommandValue = WebDBManager.GetStringField(arResult[i + 7].ToString());
                
                int nHistoryID = WebDBManager.GetIntField(arResult[i + 8].ToString(), -1);
                int nAlarmHistoryID = WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);

                int nOccurType = WebDBManager.GetIntField(arResult[i + 10].ToString(), -1);
                string strComment = WebDBManager.GetStringField(arResult[i + 11].ToString());

                if (nID > 0)
                {
                    JubixCommand cmd = new JubixCommand();
                    cmd.ID = nID;
                    cmd.Command = nCmdType;
                    if (dt != null)
                        cmd.CreateTime = dt.Data;
                    cmd.UserID = nUserID;
                    cmd.HistoryID = nHistoryID;

                    if (nPipeID != null)
                    {
                        cmd.PipeID = nPipeID.Data;
                    }

                    if (nTankID != null)
                    {
                        cmd.TankID = nTankID.Data;
                    }

                    cmd.AlarmHistoryID = nAlarmHistoryID;
                    cmd.CommandName = szCommandName;
                    cmd.CommandValue = szCommandValue;
                    cmd.OccurrenceType = nOccurType;
                    cmd.Comment = strComment;

                    m_QueCmd.Enqueue(cmd);
                }
            }
        }


        public void RemoveCommand(JubixCommand cmd)
        {
            if (cmd != null)
            {
                string szTemp = "UPDATE commandhistory SET CmdID = NULL, CommandExecuteTime='{0}', CommandName='{1}', CommandValue='{2}' WHERE ID = {3}";

                string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
                string szSQL = string.Format(szTemp, szEndDate, cmd.CommandName, cmd.CommandValue, cmd.HistoryID);
                m_dbMgr.GetResultData(szSQL, 0);

                string szSQL2 = "DELETE FROM command WHERE ID = " + cmd.ID.ToString();
                m_dbMgr.GetResultData(szSQL2, 0);
            }
        }

        public void ClearAlarm(JubixCommand cmd)
        {
            if (cmd == null)
                return;
           
            m_alarmManager.ClearAlarm(cmd.TankID, cmd.PipeID, cmd.AlarmHistoryID, cmd.UserID, cmd.OccurrenceType, cmd.Comment);
        }
      
        // 각 센서들의 마지막 알람 상태를 읽어온다.
        private void ReadLastSensorAlarms()
        {
            m_alarmManager.ReadAllLastAlarm();    
        }

        private void ChangeAlarmOptionAll(string szFieldName, string szFieldValue)
        {
            foreach(AlarmOption option in m_AlarmOptions.Values)
            {
                int nTankID = option.TankID;
                
                SaveAlarmOptionOne(nTankID, szFieldName, szFieldValue);
               
                float fValue;
                if (float.TryParse(szFieldValue, out fValue))
                {
                    SetPropertyValue(option, szFieldName, fValue);
                }
                m_alarmManager.OnChangedOption(nTankID, option);                   
            }
        }

        private void ChangeTankOptionAll(string szFieldName, string szFieldValue)
        {
            foreach (TankInfo tank in m_dicSensors.Values)
            {
                int nTankID = tank.ID;
                SaveTankOptionOne(nTankID, szFieldName, szFieldValue); 
                float fValue;
                if (float.TryParse(szFieldValue, out fValue))
                {
                    SetPropertyValue(tank, szFieldName, fValue);
                }                
            }
        }

        private void ChangeAlarmPipeOptionAll(string szFieldName, string szFieldValue)
        {
            foreach (PipeAlarmOption option in m_PipeAlarmOptions.Values)
            {
                int nPipeID = option.PipeID;

                SavePipeAlarmOptionOne(nPipeID, szFieldName, szFieldValue);

                float fValue;
                if (float.TryParse(szFieldValue, out fValue))
                {
                    SetPropertyValue(option, szFieldName, fValue);
                }               
            }
        }

        internal bool ChangeOption(JubixCommand cmd)
        {
            // 알람 옵션 구성
            if( cmd.Command == 6 ) // 배관/탱크 옵션
            {                 
                // Tank Option
                int nTankID = cmd.TankID;
                string szFieldName = cmd.CommandName;
                string szFieldValue = cmd.CommandValue;

                if( nTankID == -1)
                {
                    ChangeAlarmOptionAll(szFieldName, szFieldValue);
                }
                else
                {
                    SaveAlarmOptionOne(nTankID, szFieldName, szFieldValue);
                
                    AlarmOption option = GetAlarmOption(nTankID);
                    if (option != null)
                    {
                        float fValue;
                        if (float.TryParse(szFieldValue, out fValue))
                        {
                            SetPropertyValue(option, szFieldName, fValue);
                        }

                        m_alarmManager.OnChangedOption(nTankID, option);
                    }            
                }             
            }
            else if(cmd.Command == 7) // 탱크옵션
            {
                int nTankID = cmd.TankID;
                string szFieldName = cmd.CommandName;
                string szFieldValue = cmd.CommandValue;

                if (nTankID == -1)
                {
                    ChangeTankOptionAll(szFieldName, szFieldValue);
                }
                else
                {
                    SaveTankOptionOne(nTankID, szFieldName, szFieldValue);
                    TankInfo tank = null;
                    if (m_dicSensors.TryGetValue(nTankID, out tank))
                    {
                        float fValue;
                        if (float.TryParse(szFieldValue, out fValue))
                        {
                            SetPropertyValue(tank, szFieldName, fValue);
                        }
                    }
                }                
            }
            else if (cmd.Command == 9) // 배관알람옵션
            {
                // Pipe Option
                int nPipeID = cmd.PipeID;
                string szFieldName = cmd.CommandName;
                string szFieldValue = cmd.CommandValue;

                if (nPipeID == -1)
                {
                    ChangeAlarmPipeOptionAll(szFieldName, szFieldValue);
                }
                else
                {
                    SavePipeAlarmOptionOne(nPipeID, szFieldName, szFieldValue);

                    PipeAlarmOption option = GetPipeAlarmOption(nPipeID);
                    if (option != null)
                    {
                        float fValue;
                        if (float.TryParse(szFieldValue, out fValue))
                        {
                            SetPropertyValue(option, szFieldName, fValue);
                        }

                       // m_alarmManager.OnChangedOption(nPipeID, option);
                    }
                }      
            }
            m_bChangedData = true;
            return true;
        }


        public static void SetPropertyValue(TankInfo tank, string szPropertyName,  float fValue)
        {
            Type dgvType1 = tank.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty(szPropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (pi1.GetValue(tank).GetType() == typeof(float))
            {
                pi1.SetValue(tank, fValue, null);
            }
            else if (pi1.GetValue(tank).GetType() == typeof(int))
            {
                pi1.SetValue(tank, (int)fValue, null);
            }
        }

        public static void SetPropertyValue(AlarmOption option, string szPropertyName, float fValue)
        {
            Type dgvType1 = option.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty(szPropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (pi1.GetValue(option).GetType() == typeof(float))
            {
                pi1.SetValue(option, fValue, null);
            }
            else if (pi1.GetValue(option).GetType() == typeof(int))
            {
                pi1.SetValue(option, (int)fValue, null);
            }
        }

        public static void SetPropertyValue(PipeAlarmOption option, string szPropertyName, float fValue)
        {
            Type dgvType1 = option.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty(szPropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (pi1.GetValue(option).GetType() == typeof(float))
            {
                pi1.SetValue(option, fValue, null);
            }
            else if (pi1.GetValue(option).GetType() == typeof(int))
            {
                pi1.SetValue(option, (int)fValue, null);
            }
        }

        private void SaveTankOptionOne(int nTankID, string szFieldName, string szValue)
        {
            if (nTankID <= 0)
                return;

            string szTemp = "UPDATE tank SET {0} = {1} WHERE ID = {2}";
            string szSQL = string.Format(szTemp, szFieldName, szValue, nTankID);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void SaveAlarmOptionOne(int nAlarm, string szFieldName, string szValue)
        {
            if (nAlarm <= 0)
                return;

            string szTemp = "UPDATE AlarmOptions SET {0} = {1} WHERE ID = {2}";
            string szSQL = string.Format(szTemp, szFieldName, szValue, nAlarm);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void SavePipeAlarmOptionOne(int nAlarm, string szFieldName, string szValue)
        {
            if (nAlarm <= 0)
                return;

            string szTemp = "UPDATE AlarmPipeOptions SET {0} = {1} WHERE ID = {2}";
            string szSQL = string.Format(szTemp, szFieldName, szValue, nAlarm);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        public PipeAlarmOption GetPipeAlarmOption(int nPipeID)
        {
            PipeAlarmOption option = null;
            m_PipeAlarmOptions.TryGetValue(nPipeID, out option);
            return option;  
        }

        public AlarmOption GetAlarmOption(int nTankID)
        {
            AlarmOption option = null;
            m_AlarmOptions.TryGetValue(nTankID, out option);
            return option;            
        }

        private void ReadPipeAlarmOption()
        {
            string szSQL = "SELECT ID, PipeID, PipeStableRatio, PipeStableAbsolute, PipeStableType, PipeStableCTime, PipeStableCTimeUse FROM alarmpipeoptions";

            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nResultCount = arResult.Count;
                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    int nPipeID = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);

                    float fPipeStableRatio = WebDBManager.GetFloatField(arResult[i + 2].ToString(), -1);
                    float fPipeStableAbsolute = WebDBManager.GetFloatField(arResult[i + 3].ToString(), -1);
                    int nPipeStableType = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);
                    int nPipeStableCTime = WebDBManager.GetIntField(arResult[i + 5].ToString(), -1);
                    int nPipeStableCTimeUse = WebDBManager.GetIntField(arResult[i + 6].ToString(), -1);


                    PipeAlarmOption option = null;
                    if (m_PipeAlarmOptions.TryGetValue(nPipeID, out option) == true)
                    {
                        continue;
                    }
                    else
                    {
                        option = new PipeAlarmOption();
                        option.ID = nID;
                        option.PipeID = nPipeID;
                        option.PipeStableRatio = fPipeStableRatio;
                        option.PipeStableAbsolute = fPipeStableAbsolute;
                        option.PipeStableType = nPipeStableType;
                        option.PipeStableCTime = nPipeStableCTime;
                        option.PipeStableCTimeUse = nPipeStableCTimeUse;

                        m_PipeAlarmOptions.Add(nPipeID, option);
                    }
                }
            }
        }

        private void ReadAlarmOption()
        {
            string szSQL = "SELECT ID, TankID, PipeStableRatio, PipeStableAbsolute, PipeStableType, PipeStableCTime,PipeStableCTimeUse, " +
                            " TankStableRatio, TankStableAbsolute, TankStableType,  TankStableCTime, " +
                            " TankStableCTimeUse, StableBeginWorkM,  AlarmInterval, AlarmIntervalUse FROM alarmoptions";

            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nResultCount = arResult.Count;
                for (int i = 0; i < nResultCount - 14; i += 15) 
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    int nTankID = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);

                    float fPipeStableRatio = WebDBManager.GetFloatField(arResult[i + 2].ToString(), -1);
                    float fPipeStableAbsolute = WebDBManager.GetFloatField(arResult[i + 3].ToString(), -1);
                    int nPipeStableType = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);                    
                    int nPipeStableCTime = WebDBManager.GetIntField(arResult[i + 5].ToString(), -1);
                    int nPipeStableCTimeUse = WebDBManager.GetIntField(arResult[i + 6].ToString(), -1);                   

                    float fTankStableRatio = WebDBManager.GetFloatField(arResult[i + 7].ToString(), -1);
                    float fTankStableAbsolute = WebDBManager.GetFloatField(arResult[i + 8].ToString(), -1);
                    int nTankStableType = WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);                   
                    int nTankStableCTime = WebDBManager.GetIntField(arResult[i + 10].ToString(), -1);
                    int nTankStableCTimeUse = WebDBManager.GetIntField(arResult[i + 11].ToString(), -1);

                    int nStableBeginWorkM = WebDBManager.GetIntField(arResult[i + 12].ToString(), -1);
                    int nAlarmInterval = WebDBManager.GetIntField(arResult[i + 13].ToString(), -1);
                    int nAlarmIntervalUse = WebDBManager.GetIntField(arResult[i + 14].ToString(), -1);

                    AlarmOption option = null;
                    if (m_AlarmOptions.TryGetValue(nTankID, out option) == true)
                    {
                        continue;
                    }
                    else
                    {
                        option = new AlarmOption();
                        option.ID = nID;
                        option.TankID = nTankID;
                        option.PipeStableRatio = fPipeStableRatio;
                        option.PipeStableAbsolute = fPipeStableAbsolute;
                        option.PipeStableType = nPipeStableType;
                        option.PipeStableCTime = nPipeStableCTime;
                        option.PipeStableCTimeUse = nPipeStableCTimeUse;

                        option.TankStableRatio = fTankStableRatio;
                        option.TankStableAbsolute = fTankStableAbsolute;
                        option.TankStableType = nTankStableType;                        
                        option.TankStableCTime = nTankStableCTime;
                        option.TankStableCTimeUse = nTankStableCTimeUse;

                        option.StableBeginWorkM = nStableBeginWorkM;
                        option.AlarmInterval = nAlarmInterval;
                        option.AlarmIntervalUse = nAlarmIntervalUse;

                        m_AlarmOptions.Add(nTankID, option);
                    }
                }
            }
        }       

        internal void DoneWork(JubixCommand cmd)
        {
            // Clear AlarmIgnore
            m_alarmManager.ClearAlarm(cmd.TankID, cmd.PipeID, cmd.UserID);
            
            m_alarmManager.RemoveAlarmIgnore(cmd.TankID, cmd.UserID);
            
            m_workManager.EndWork(cmd);   
        }

        internal void BeginWork(JubixCommand cmd, int nLinkData)        
        {
            int nWorkHistoryID = m_workManager.BeginWork(cmd, nLinkData);

            // 남아 있는 알람을 모두 클리어 한다.
            m_alarmManager.ClearAlarm(cmd.TankID, cmd.PipeID, cmd.UserID);

            // 탱크에 지정된 알람 무시 시간을 구성한다.
            AlarmOption option = GetAlarmOption(cmd.TankID);
            int nIgnoreTime = option.StableBeginWorkM;
            // Make AlarmIgonre

            if (nIgnoreTime >= 0)
                m_alarmManager.CreateAlarmIgnore(cmd.TankID, cmd.PipeID, cmd.UserID, nIgnoreTime, nWorkHistoryID);

            
            SetStableValue(-1, -1, nWorkHistoryID, cmd.CreateTime);

            WorkInfo info = m_workManager.GetWork(nWorkHistoryID);
            if (info != null)
            {
                // 황산 PO작업도 
                //SetTankStableValueOtherWork(info.TankID, info.PipeID, cmd.CreateTime);

                info.BeginCheck = true;
            }
        }

        internal void BeginWork(JubixCommand cmd)
        {
            int nWorkHistoryID = m_workManager.BeginWork(cmd);

            // 남아 있는 알람을 모두 클리어 한다.
            m_alarmManager.ClearAlarm(cmd.TankID, cmd.PipeID, cmd.UserID);

            // 탱크에 지정된 알람 무시 시간을 구성한다.
            AlarmOption option = GetAlarmOption(cmd.TankID);            
            int nIgnoreTime = option.StableBeginWorkM;
            // Make AlarmIgonre

            if (nIgnoreTime >= 0)
                m_alarmManager.CreateAlarmIgnore(cmd.TankID, cmd.PipeID, cmd.UserID, nIgnoreTime, nWorkHistoryID);

            SetStableValue(-1, -1, nWorkHistoryID, cmd.CreateTime);


            WorkInfo info = m_workManager.GetWork(nWorkHistoryID);
            if( info != null)
            {
                SetTankStableValueOtherWork(info.TankID, info.PipeID, cmd.CreateTime);

                info.BeginCheck = true;
               
            } 
        }

        public void SetTankStableValueOtherWork(int nTankID, int nPipeID, DateTime dtTime)
        {
            List<WorkInfo> infolist = this.m_workManager.GetWorks(nTankID);
            if (infolist != null)
            {
                foreach (WorkInfo info in infolist)
                {
                    //if (info.PipeID != nPipeID)
                    {
                        SetStableFlowValue(info, dtTime);
                    }
                }
            }
        }

        public void SetStableValue(WorkInfo info, DateTime dt)
        {
            int nTankID = info.TankID;
            int nPipeID = info.PipeID;
            int nWorkHistoryID = info.WorkHistoryID;

            if (info != null)
            {
                TankInfo tank = GetTank(nTankID);
                if (tank != null && tank.Flow != -999.0f)
                {
                    SaveStableFlow(info, tank.Flow, dt);
                   
                }
                else
                {
                    SaveStableFlow(info, -9999, dt);
                }


                PipeSensor pipe = GetPipe(nPipeID);
                SaveStablePressure(info, (pipe == null) ? -9999 : pipe.CurrentValue, dt);                
            }
        }


        internal void SetStableFlowValue(WorkInfo info, DateTime dt)
        {
            int nTankID = info.TankID;
            int nPipeID = info.PipeID;
            int nWorkHistoryID = info.WorkHistoryID;
   
            if (info != null)
            {
                TankInfo tank = GetTank(nTankID);
                if (tank != null && tank.Flow != -999.0f)
                {
                    SaveStableFlow(info, tank.Flow, dt);
                }
                else
                {
                    SaveStableFlow(info, -9999, dt);
                }
            }
        }

        public void SetStableValue(WorkInfo info)
        {
            int nTankID = info.TankID;
            int nPipeID = info.PipeID;
            int nWorkHistoryID = info.WorkHistoryID;

            DateTime dt = DateTime.Now;

            //WorkInfo work = m_workManager.GetWork(nWorkHistoryID);
            if (info != null)
            {
                TankInfo tank = GetTank(nTankID);
                if (tank != null && tank.Flow != -999.0f)
                {
                    SaveStableFlow(info, tank.Flow, dt);
                }
                else
                {
                    SaveStableFlow(info, -9999, dt);
                }


                PipeSensor pipe = GetPipe(nPipeID);
                SaveStablePressure(info, (pipe == null) ? -9999 : pipe.CurrentValue, dt);
            }
        }

        internal void SetStableValue(int nWorkHistoryID)
        {
            DateTime dt = DateTime.Now;

            WorkInfo work = m_workManager.GetWork(nWorkHistoryID);
            if (work != null)
            {
                TankInfo tank = GetTank(work.TankID);
                if (tank != null && tank.Flow != -999.0f)
                {
                    SaveStableFlow(work, tank.Flow, dt);
                }
                else
                {
                    SaveStableFlow(work, -9999, dt);
                }
                
                PipeSensor pipe = GetPipe(work.PipeID);
                SaveStablePressure(work, (pipe == null) ? -9999 : pipe.CurrentValue, dt);                
            } 
        }

        private void SetStableValue(int nTankID, int nPipeID, int nWorkHistoryID, DateTime dt)
        {
            //DateTime dt = DateTime.Now;

            WorkInfo work = m_workManager.GetWork(nWorkHistoryID);
            if (work != null)
            {
                TankInfo tank = GetTank(nTankID);
                if (tank != null && tank.Flow != -999.0f)
                {
                    SaveStableFlow(work, tank.Flow, dt);
                }
                else
                {
                    SaveStableFlow(work, -9999, dt);
                }


                PipeSensor pipe = GetPipe(nPipeID);
                SaveStablePressure(work, (pipe == null) ? -9999 : pipe.CurrentValue, dt);                
            }      
        }

        private bool UseSMS()
        {
            // SMS전송 사용안함
            return false;

            // 문자는 Push서버에서 전송하므로 여기에 전송루틴은 사용하지 않는다.

            //string strSQL = "Select PropertyValue from Options where PropertyName = 'UseSMS' and SiteID = " + KPXServerManager.Instance.SiteID;
            //ArrayList arrResult = JubixSensorManager.Instance.DBManager.GetResultData(strSQL, 0);

            //if (arrResult == null || arrResult.Count == 0)
            //    return false;

            //string strValue = WebDBManager.GetStringField(arrResult[0]).Trim();

            //if (strValue == "0")
            //    return false;
            //else if (strValue == "1")
            //    return true;
            //else if (string.Compare(strValue, "true", true) == 0)
            //    return true;
            //else if (string.Compare(strValue, "false", true) == 0)
            //    return false;

            //return false;
        }

        private string MakeMessage(string szName, AlarmType nStatus, bool bPipe = false)
        {
            string szMsg = "탱크[{0}] 에서 {1}이 감지 되었습니다.";
            if(bPipe == true)
            {
                szMsg = "배관[{0}] 에서 {1}이 감지 되었습니다.";
            }
            string szResult = string.Format(szMsg, szName, nStatus.ToString());
            return szResult;
        }        

        private void SendSMS(string szTankName, AlarmType nStatus, bool bPipe = false)
        {
            if (UseSMS() == false)
                return;

            //DataManager mgr = new DataManager(JubixSensorManager.Instance.DBManager, JubixSensorManager.Instance.SiteID);
            //List<string> phoneNumbers = mgr.GetFacilityManagerPhoneNumberList();

            //if (phoneNumbers == null || phoneNumbers.Count == 0)
            //    return;


            //string strMsg = MakeMessage(szTankName, nStatus, bPipe);

            //string szSendPhoneNumber = GetSendPhoneNumber();
            //foreach (string strPhoneNumber in phoneNumbers)
            //{
            //    libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1");
            //    client.SendSMS(szSendPhoneNumber, strPhoneNumber, strMsg);
            //}
        }

        //private string szCaller = "0522676652";
        //private string GetSendPhoneNumber()
        //{
        //    string strSQL = "Select PropertyValue from Options where PropertyName = 'SmsCaller' and SiteID = " + JubixSensorManager.Instance.SiteID.ToString();
        //    ArrayList arrResult = JubixSensorManager.Instance.DBManager.GetResultData(strSQL, 0);

        //    if (arrResult == null || arrResult.Count == 0)
        //        return szCaller;

        //    string strValue = WebDBManager.GetStringField(arrResult[0]).Trim();
        //    if (strValue == null || strValue == "")
        //        return szCaller;

        //    return strValue;
        //}


        internal void SetPressure(int p, float fPressure)
        {
            //PipeSensor pipe = GetPipe(p);

            foreach (PipeSensor sensor in JubixSensorManager.Instance.SensorList)
            {
                if( sensor.PipeID == p)
                {
                    sensor.SetSensorValue(fPressure);
                    break;
                }
            }

            //foreach (PipeSensor sensor in Netowrk.KPXParagonManager.Instance.PipeList)
            //{
            //    if (sensor.PipeID == p)
            //    {
            //        sensor.SetSensorValue(fPressure);
            //        break;
            //    }
            //}

            //if( pipe != null)
            //{
            //    pipe.SetSensorValue(fPressure);
            //}
        } 
    }
}