using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility;
using JubixNetwork;

namespace PSensorServer
{
    public class KPXLevelMeterManager
    {
        private int m_nSiteID = 500;
        private DBUtility.WebDBManager mDBMgr = null;
        private Thread m_LevelCheckThread = null;

        private GasDetector.LevelMeterManager dm = new GasDetector.LevelMeterManager();
   
        private static KPXLevelMeterManager m_instance = null;
        public static KPXLevelMeterManager Instance
        {
            get { return m_instance; }
        }

        public KPXLevelMeterManager()
        {
            m_nSiteID = KPXServerManager.Instance.SiteID;
            mDBMgr = new WebDBManager(m_nSiteID);
            m_instance = this;

            System.Diagnostics.Trace.WriteLine("Create KPXLevelMeterManager");
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
            catch (Exception)
            { }

            LoadTank();

            m_LevelCheckThread = new Thread(CheckValue);
            m_LevelCheckThread.Name = "Level value Check";
            m_LevelCheckThread.Start();
        }

        private void GasLevelMeter_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if (mNotifyAction != null)
            {
                mNotifyAction.Invoke(nComm, nAlarmUnit, fValue, nChannel, nStatus);
            }
        }

        private bool m_bSimulation = false;
        public bool Simulation
        {
            get { return m_bSimulation; }
        }
            
        internal void SimulationMode(bool bSet)
        {
            m_bSimulation = bSet;

        }


        public void StopServer()
        {
            try
            {
                m_bReleaseThread = true;
                if (m_LevelCheckThread != null)
                    m_LevelCheckThread.Join(2000);
                m_LevelCheckThread = null;
            }
            catch (Exception)
            { }

            dm.OnNotifyAlarm -= GasLevelMeter_OnNotifyAlarm;
            dm.End();

            mNotifyAction = null;
        }
        
        private List<TankInfo> m_TankList = new List<TankInfo>();
        public List<TankInfo> TankList
        {
            get { return m_TankList; }
        }

        // Tank ID별 탱크정보
        private SortedList<int, TankInfo> m_dicTankList = new SortedList<int, TankInfo>();
        public SortedList<int, TankInfo> DicTankList
        {
            get { return m_dicTankList; }
        }

        // Level Address 별 탱크 정보
        private SortedList<int, TankInfo> m_TankInfo = new SortedList<int, TankInfo>();
        public SortedList<int, TankInfo> DicLeveTank
        {
            get { return m_TankInfo; }
        }
        
        public TankInfo FindTank(int nTankID)
        {
            TankInfo info = null;
            m_TankInfo.TryGetValue(nTankID, out info);
            return info;
        }

        private void LoadTank()
        {
            string szSQL = "SELECT ID, Name, LiquidType, Capacity, HighLevel, MinTemp, MaxTemp, Density, Status, " +
                          " LevelAddress, TempAddress, GrossVolumeAddress, NetVolumeAddress, MassAddress, FlowAddress, PressureAddress, HistoryTableName, " +
                          " IsLeak, LeakLevel, LevelTime,WorkAutoStartFlow,UseAutoStartWork  FROM tank";

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count - 21; i += 22)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    string szName = WebDBManager.GetStringField(arResult[i + 1]);
                    string szLiquidType = WebDBManager.GetStringField(arResult[i + 2]);
                    float fCapacity = WebDBManager.GetFloatField(arResult[i + 3].ToString(), 0.0f);
                    float fHighLevel = WebDBManager.GetFloatField(arResult[i + 4].ToString(), 0.0f);
                    float fMinTemp = WebDBManager.GetFloatField(arResult[i + 5].ToString(), 0.0f);
                    float fMaxTemp = WebDBManager.GetFloatField(arResult[i + 6].ToString(), 0.0f);
                    float fDensity = WebDBManager.GetFloatField(arResult[i + 7].ToString(), 0.0f);
                    int nStatus = WebDBManager.GetIntField(arResult[i + 8].ToString(), -1);
                    int nLevelAddress = WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);
                    int nTempAddress = WebDBManager.GetIntField(arResult[i + 10].ToString(), -1);
                    int nGrossVolumeAddress = WebDBManager.GetIntField(arResult[i + 11].ToString(), -1);
                    int nNetVolumeAddress = WebDBManager.GetIntField(arResult[i + 12].ToString(), -1);
                    int nMassAddress = WebDBManager.GetIntField(arResult[i + 13].ToString(), -1);
                    int nFlowAddress = WebDBManager.GetIntField(arResult[i + 14].ToString(), -1);
                    int nPressureAddress = WebDBManager.GetIntField(arResult[i + 15].ToString(), -1);

                    string szTableName = WebDBManager.GetStringField(arResult[i + 16].ToString());

                    int isLeakCheck = WebDBManager.GetIntField(arResult[i + 17].ToString(), 1);
                    float fLeakLevel = WebDBManager.GetFloatField(arResult[i + 18].ToString(), 1.0f);
                    int fLeakCheckTime = WebDBManager.GetIntField(arResult[i + 19].ToString(), 600);

                    float fAutoStartFlow = WebDBManager.GetFloatField(arResult[i + 20].ToString(), -999.0f);
                    int nUseAutoStart = WebDBManager.GetIntField(arResult[i + 21].ToString(), 0);
                    
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
                    info.NetVolumeAddress = nNetVolumeAddress - 1;
                    info.MassAddress = nMassAddress - 1;
                    info.FlowAddress = nFlowAddress - 1;
                    info.PressureAddress = nPressureAddress - 1;

                    info.AutoStartFlow = fAutoStartFlow;
                    info.UseAutoStart = nUseAutoStart;
                   
                    // Data테이블 명 추가
                    info.TablePrefix = szTableName;

                    m_TankList.Add(info);
                    m_TankInfo.Add(nLevelAddress, info);
                    m_dicTankList.Add(info.ID, info);
                }
            }
        }

        //private void LoadTankAlarmOptions()
        //{
        //    //string szSQL = "SELECT ID, HighLevel, MinTemp, MaxTemp FROM tank";
        //    string szSQL = "SELECT ID, HighLevel, MinTemp, MaxTemp, StableRatio, StableAbsolute, StableType, StableBeginWorkM, " +
        //         " StableCTime, StableCTimeUse, AlarmInterval, AlarmIntervalUse FROM tank";

        //    ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
        //    if (arResult != null && arResult.Count > 0)
        //    {
        //        for (int i = 0; i < arResult.Count - 11; i += 12)
        //        {
        //            int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
        //            float fHighLevel = WebDBManager.GetFloatField(arResult[i + 1].ToString(), 0.0f);
        //            float fMinTemp = WebDBManager.GetFloatField(arResult[i + 2].ToString(), 0.0f);
        //            float fMaxTemp = WebDBManager.GetFloatField(arResult[i + 3].ToString(), 0.0f);

        //            float fStableRatio = WebDBManager.GetFloatField(arResult[i + 4].ToString(), 5);
        //            float fStableAbsolute = WebDBManager.GetFloatField(arResult[i + 5].ToString(), 1);
        //            int nStableType = WebDBManager.GetIntField(arResult[i + 6].ToString(), 0);
        //            int nStableBeginWorkM = WebDBManager.GetIntField(arResult[i + 7].ToString(), 15);
        //            int nStableCTime = WebDBManager.GetIntField(arResult[i + 8].ToString(), 2);
        //            int nStableCTimeUse = WebDBManager.GetIntField(arResult[i + 9].ToString(),1);
        //            int nAlarmInterval = WebDBManager.GetIntField(arResult[i + 10].ToString(), 30);
        //            int nAlarmIntervalUse = WebDBManager.GetIntField(arResult[i + 11].ToString(), 1);

        //            TankInfo info = GetTank(nID);

        //            if (info == null)
        //                continue;

        //            info.HighLevel = fHighLevel;
        //            info.MinTemp = fMinTemp;
        //            info.MaxTemp = fMaxTemp;

        //            info.StableRatio = fStableRatio;
        //            info.StableAbsolute = fStableAbsolute;
        //            info.StableType = nStableType;
        //            info.StableBeginWorkTime = nStableBeginWorkM;
        //            info.StableCTime = nStableCTime;
        //            info.StableCTimeUse = nStableCTimeUse;
        //            info.AlarmInterval = nAlarmInterval;
        //            if(nAlarmIntervalUse == 0)
        //            {
        //                info.AlarmInterval = 0;
        //            }
        //        }
        //    }
        //}

        private TankInfo GetTank(int nTankID)
        {
            TankInfo info = null;
            m_dicTankList.TryGetValue(nTankID, out info);
            return info;
        }

        private bool m_bReleaseThread = false;
        
        // PLC모든 번지에 대해 데이터 입력이 1회이상 완료된 경우 true가 된다.
        // 정상적인 데이터가 입력되어 체크가 가능한 시점에 true가 된다.
        private bool m_bDataEnabled = false;

        public bool DataEnabled
        {
            get { return m_bDataEnabled; }
            set { m_bDataEnabled = value; }
        }

        private void CheckValue()
        {
            int nCount = 0;
          
            while (!m_bReleaseThread)
            {
                int nUnitID = 1;
                bool bOnline = dm.GetOnline(nUnitID);
                for (int i = 0; i < m_TankList.Count; i++)
                {
                    nUnitID = 1;
                    TankInfo info = (TankInfo)m_TankList[i];
                    int nDelta = 0;
                    
                    if (info.LevelAddress >= 132)
                    {
                        nUnitID = 5;
                        nDelta = 132;
                    }
                    else if (info.LevelAddress >= 108)
                    {
                        nUnitID = 4;
                        nDelta = 108;
                    }
                    else if (info.LevelAddress >= 72)
                    {
                        nUnitID = 3;
                        nDelta = 72;

                    }
                    else if (info.LevelAddress >= 36)
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

                    if (info.PressureAddress >= 132)
                    {
                        nUnitID = 5;
                        nDelta = 132;
                    }
                    else if (info.PressureAddress >= 108)
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

                    if (m_bSimulation == false)
                    {
                        info.Level = fLevel;
                        info.Temperature = fTemp;
                        info.GrossVolume = fGrossVolume;
                        info.NetVolume = fNetVolume;
                        info.Mass = fMass;
                        info.Flow = fFlow;

                        if (info.PressureAddress >= 0)
                            info.Pressure = fPressure;
                    }
                    

                    if (m_bDataEnabled == true)
                    {
                        //SaveTankHistory(info);
                        //CheckAlarm(info);
                    }

                    if (m_bSimulation == false)
                        UpdateTankValue(info);


                    if (m_bReleaseThread == true)
                        break;
                }

                if (nCount == 20)
                    m_bDataEnabled = true;
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

        private void UpdateTankValue(TankInfo info)
        {
            string szTemp = "UPDATE tank SET Mass={0},GrossVolume={1}, " +
                    " NetVolume={2}, Pressure={3} WHERE ID = {4}";

            string szPressure = (info.PressureAddress >= 0 ? info.Pressure.ToString() : "NULL");
            string szSQL = string.Format(szTemp, info.Mass, info.GrossVolume, info.NetVolume, szPressure, info.ID);
            mDBMgr.GetResultData(szSQL, 0);
        }        
    }   
}
