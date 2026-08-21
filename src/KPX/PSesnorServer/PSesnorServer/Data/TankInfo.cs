using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PSensorServer
{
    public class TankInfo
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

        private float m_fPrevLevel = -999.0f;
        public float PrevLevel
        {
            get { return m_fPrevLevel; }
            set 
            {               
                m_fPrevLevel = value; }
        }

        private float m_fLevel = -999.0f;
        public float Level
        {
            get { return m_fLevel; }
            set {
                m_fPrevLevel = m_fLevel;
                m_fLevel = value; }
        }

        private float m_fTemperature = -999.0f;
        public float Temperature
        {
            get { return m_fTemperature; }
            set
            {
                m_fPrevTemperature = m_fTemperature;
                m_fTemperature = value; 
            }
        }
        private float m_fPrevTemperature = -999.0f;
        public float PrevTemperature
        {
            get { return m_fPrevTemperature; }
            set
            {

                m_fPrevTemperature = value;
            }
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

        private float m_fPrevFlow = 0.0f;
        public float PrevFlow
        {
            get { return m_fPrevFlow; }
            set { m_fPrevFlow = value; }
        }

        private float m_fFlow = -999.0f;
        public float Flow
        {
            get { return m_fFlow; }
            set 
            {
                m_fPrevFlow = m_fFlow;
                m_fFlow = value; 
            }
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


        private string m_szTablePrefix;
        public string TablePrefix
        {
            get { return m_szTablePrefix; }
            set { m_szTablePrefix = value; }
        }

        public string GetCurrentTableName()
        {
            DateTime dt = DateTime.Now;
            string szTemp = string.Format("{0}{1:D2}", m_szTablePrefix, dt.Month);
            return szTemp;
        }

        //private float m_fStableRatio;
        //public float StableRatio
        //{
        //    get { return m_fStableRatio; }
        //    set { m_fStableRatio = value; }
        //}

        //private float m_fStableAbsolute;
        //public float StableAbsolute
        //{
        //    get { return m_fStableAbsolute; }
        //    set { m_fStableAbsolute = value; }
        //}

        //private int m_nStableType;
        //public int StableType
        //{
        //    get { return m_nStableType; }
        //    set { m_nStableType = value; }
        //}

        //// 작업 시작 무시 시간
        //private int m_nStableBeginWorkTime;
        //public int StableBeginWorkTime
        //{
        //    get { return m_nStableBeginWorkTime; }
        //    set { m_nStableBeginWorkTime = value; }
        //}

        //// 안정 범위 유지 시간
        //private int m_nStableCTime;
        //public int StableCTime
        //{
        //    get { return m_nStableCTime; }
        //    set { m_nStableCTime = value; }
        //}

        //private int m_nStableCTimeUse;
        //public int StableCTimeUse
        //{
        //    get { return m_nStableCTimeUse; }
        //    set { m_nStableCTimeUse = value; }
        //}

        //// 알람 해제시 
        //private float m_fAlarmInterval;
        //public float AlarmInterval
        //{
        //    get { return m_fAlarmInterval; }
        //    set { m_fAlarmInterval = value; }
        //}
        //private int m_nAlarmIntervalUse;


        // 누유 레벨 체크
        private bool m_bCheckLeak = true;
        public bool CheckLeak
        {
            get { return m_bCheckLeak; }
            set { m_bCheckLeak = value; }
        }

        // 누유 레벨 차
        private float m_fLeakLevel = 2.0f;
        public float LeakLevel
        {
            get { return m_fLeakLevel; }
            set { m_fLeakLevel = value; }
        }

        // 누유 감시 시간
        private float m_nLevelChangeTime = 20;
        public float LevelTime
        {
            get { return m_nLevelChangeTime; }
            set { m_nLevelChangeTime = value; }
        }

        private bool m_bCheckStart = false;
        public bool CheckStart
        {
            get { return m_bCheckStart; }
            set { m_bCheckStart = value; }
        }
        
        private DateTime m_dtLastCheckTime = DateTime.Now;
        public DateTime LastCheckTime
        {
            get { return m_dtLastCheckTime; }
            set 
            {             
                m_dtLastCheckTime = value; 
            }
        }

        private DateTime m_dtBeginCheckTime = DateTime.Now;
        public DateTime BeginCheckTime
        {
            get { return m_dtBeginCheckTime; }
            set
            {
                m_dtBeginCheckTime = value;
            }
        }

        private float m_fLastLeakCheckLevel = 0.0f;
        public float LastLeakCheckLevel
        {
            get { return m_fLastLeakCheckLevel; }
            set { m_fLastLeakCheckLevel = value; }
        }

        private float m_fBeginLeakCheckLevel = 0.0f;
        public float BeginLeakCheckLevel
        {
            get { return m_fBeginLeakCheckLevel; }
            set { m_fBeginLeakCheckLevel = value; }
        }



        public float AutoStartFlow { get; set; }

        public int UseAutoStart { get; set; }
    }
}
