using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertServer.Data
{
    public class SensorData
    {
        public static string ChangeLevelNumToType(string strLevel)
        {
            string strRet = CommonString.RiskLevel_Normal;

            if (strLevel == "0")
                strRet = CommonString.RiskLevel_Normal;
            else if (strLevel == "1")
                strRet = CommonString.RiskLevel_Attention;
            else if (strLevel == "2")
                strRet = CommonString.RiskLevel_Caution;
            else if (strLevel == "3")
                strRet = CommonString.RiskLevel_Alert;
            else if (strLevel == "4")
                strRet = CommonString.RiskLevel_Serious;
            else
                strRet = CommonString.RiskLevel_Normal;

            return strRet;
        }

        private string ChangeLevelKorToType(string strLevel)
        {
            string strRet = CommonString.RiskLevel_Normal;

            if (strLevel == CommonString.RiskLevel_Normal_Kor)
            {
                strRet = CommonString.RiskLevel_Normal;
            }
            else if (strLevel == CommonString.RiskLevel_Attention_Kor)
            {
                strRet = CommonString.RiskLevel_Attention;
            }
            else if (strLevel == CommonString.RiskLevel_Caution_Kor)
            {
                strRet = CommonString.RiskLevel_Caution;
            }
            else if (strLevel == CommonString.RiskLevel_Alert_Kor)
            {
                strRet = CommonString.RiskLevel_Alert;
            }
            else if (strLevel == CommonString.RiskLevel_Serious_Kor)
            {
                strRet = CommonString.RiskLevel_Serious;
            }

            return strRet;
        }
    }

    public class FloodData
    {
        private string m_strGroupID = "";
        private string m_strSensorID = "";
        private DateTime m_dtMeasureTime;
        private float m_strWaterLevel = 0;

        public string GroupID
        {
            get { return m_strGroupID; }
            set { m_strGroupID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public DateTime MeasureTime
        {
            get { return m_dtMeasureTime; }
            set { m_dtMeasureTime = value; }
        }

        public float WaterLevel
        {
            get { return m_strWaterLevel; }
            set { m_strWaterLevel = value; }
        }
    }

    public class FloodNewData
    {
        private DateTime m_dtObserveTime;
        private string m_strDistrictCode = "";
        private string m_strFall = "";

        public DateTime ObserveTime
        {
            get { return m_dtObserveTime; }
            set { m_dtObserveTime = value; }
        }

        public string DistrictCode
        {
            get { return m_strDistrictCode; }
            set { m_strDistrictCode = value; }
        }

        public string Fall
        {
            get { return m_strFall; }
            set { m_strFall = value; }
        }
    }

    public class FloodSensor
    {
        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = CommonString.RiskLevel_Normal;
        private string m_strAddr = "";
        private DateTime m_dtMeasureTime;
        private float m_fDepth = 0;
        private float m_fFlow = 0;
        private string m_strMessage = "";

        private int m_nUserModifity = 0;
        public int UserModifity
        {
            get { return m_nUserModifity; }
            set { m_nUserModifity = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        public DateTime MeasureTime
        {
            get { return m_dtMeasureTime; }
            set { m_dtMeasureTime = value; }
        }

        public float Depth
        {
            get { return m_fDepth; }
            set { m_fDepth = value; }
        }

        public float Flow
        {
            get { return m_fFlow; }
            set { m_fFlow = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }

    public class FloodSensorFallData
    {
        string m_strSensorID = "";
        Dictionary<int, TimeFallData> m_dicTimeFalls = new Dictionary<int, TimeFallData>();

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public Dictionary<int, TimeFallData> DicTimeFalls
        {
            get { return m_dicTimeFalls; }
            set { m_dicTimeFalls = value; }
        }
    }

    public class TimeFallData
    {
        int m_nFall = 0;
        Dictionary<int, float> m_dicFalls = new Dictionary<int, float>();

        public int Fall
        {
            get { return m_nFall; }
            set { m_nFall = value; }
        }

        public Dictionary<int, float> DicFalls
        {
            get { return m_dicFalls; }
            set { m_dicFalls = value; }
        }
    }

    public class FloodSensorLevelData
    {
        string m_strSensorID = "";
        List<FallLevelData> m_listFallLevel = new List<FallLevelData>();

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public List<FallLevelData> FallLevels
        {
            get { return m_listFallLevel; }
            set { m_listFallLevel = value; }
        }

        public string CheckSensorLevel(float fDepth)
        {
            foreach (FallLevelData levelData in m_listFallLevel)
            {
                if (fDepth > levelData.OverValue && fDepth <= levelData.LowerValue)
                    return levelData.Level.ToString();
            }

            return RiskLevel.Normal.ToString();
        }
    }

    public class FallLevelData
    {
        float m_fOverValue = 0;
        float m_fLowerValue = 0;
        RiskLevel m_Level = RiskLevel.Normal;

        public float OverValue
        {
            get { return m_fOverValue; }
            set { m_fOverValue = value; }
        }

        public float LowerValue
        {
            get { return m_fLowerValue; }
            set { m_fLowerValue = value; }
        }

        public RiskLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
    }



    public class CollapseSensor
    {
        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = CommonString.RiskLevel_Normal;
        private string m_strAddr = "";
        private DateTime m_dtMeasureTime;
        private string m_strMessage = "";

        private int m_nUserModifity = 0;
        public int UserModifity
        {
            get { return m_nUserModifity; }
            set { m_nUserModifity = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        public DateTime MeasureTime
        {
            get { return m_dtMeasureTime; }
            set { m_dtMeasureTime = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }

    public class CollapseData
    {
        private string m_strSensorID = "";
        private short m_nSlopeID = 0;
        private DateTime m_dtEvelDate;
        private string m_strLevel = "";

        private string m_strRainfall = "";

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public short SlopeID
        {
            get { return m_nSlopeID; }
            set { m_nSlopeID = value; }
        }

        public DateTime EvelDate
        {
            get { return m_dtEvelDate; }
            set { m_dtEvelDate = value; }
        }

        public string Level
        {
            get { return m_strLevel; }
            set { m_strLevel = value; }
        }

        public string Rainfall
        {
            get { return m_strRainfall; }
            set { m_strRainfall = value; }
        }
    }

    public class FireSensor
    {
        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = CommonString.RiskLevel_Normal;
        private string m_strAddr = "";
        private DateTime m_dtOccurTime;
        private DateTime m_dtCloseTime;
        private bool m_bAfterFire = false;
        private DateTime m_dtAlarmPeriodStart;
        private DateTime m_dtAlarmPeriodEnd;
        private DateTime m_dtWeakStart;
        private DateTime m_dtWeakEnd;
        private int m_nInitReact = 0;
        private int m_nDemander = 0;
        private int m_nDeathToll = 0;
        private string m_strMessage = "";

        private int m_nUserModifity = 0;
        public int UserModifity
        {
            get { return m_nUserModifity; }
            set { m_nUserModifity = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        public DateTime OccurTime
        {
            get { return m_dtOccurTime; }
            set { m_dtOccurTime = value; }
        }

        public DateTime CloseTime
        {
            get { return m_dtCloseTime; }
            set { m_dtCloseTime = value; }
        }

        public bool AfterFire
        {
            get { return m_bAfterFire; }
            set { m_bAfterFire = value; }
        }

        public DateTime AlarmPeriodStart
        {
            get { return m_dtAlarmPeriodStart; }
            set { m_dtAlarmPeriodStart = value; }
        }

        public DateTime AlarmPeriodEnd
        {
            get { return m_dtAlarmPeriodEnd; }
            set { m_dtAlarmPeriodEnd = value; }
        }

        public DateTime WeakStart
        {
            get { return m_dtWeakStart; }
            set { m_dtWeakStart = value; }
        }

        public DateTime WeakEnd
        {
            get { return m_dtWeakEnd; }
            set { m_dtWeakEnd = value; }
        }

        public int InitReact
        {
            get { return m_nInitReact; }
            set { m_nInitReact = value; }
        }

        public int Demander
        {
            get { return m_nDemander; }
            set { m_nDemander = value; }
        }

        public int DeathToll
        {
            get { return m_nDeathToll; }
            set { m_nDeathToll = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }

    public class FireData
    {
        private int m_nEventID = 0;
        private string m_strOccurType = "";
        private DateTime m_dtOccurTime;
        private float m_nLatitude = 0;
        private float m_nLongitude = 0;
        private int m_nDangerRange = 0;
        private string m_strDangerStep = "";
        private int m_nBuildingId = 0;
        private short m_nEventFinishYn = 0;

        public int EventID
        {
            get { return m_nEventID; }
            set { m_nEventID = value; }
        }

        public string OccurType
        {
            get { return m_strOccurType; }
            set { m_strOccurType = value; }
        }

        public DateTime OccurTime
        {
            get { return m_dtOccurTime; }
            set { m_dtOccurTime = value; }
        }

        public float Latitude
        {
            get { return m_nLatitude; }
            set { m_nLatitude = value; }
        }

        public float Longitude
        {
            get { return m_nLongitude; }
            set { m_nLongitude = value; }
        }

        public int DangerRange
        {
            get { return m_nDangerRange; }
            set { m_nDangerRange = value; }
        }

        public string DangerStep
        {
            get { return m_strDangerStep; }
            set { m_strDangerStep = value; }
        }

        public int BuildingId
        {
            get { return m_nBuildingId; }
            set { m_nBuildingId = value; }
        }

        public short EventFinishYn
        {
            get { return m_nEventFinishYn; }
            set { m_nEventFinishYn = value; }
        }
    }

    public class HeatData
    {
        int m_nEventID = -1;
        int m_nGroupID = -1;
        short m_nUniqueID = -1;
        double m_dLatitude = 0;
        double m_dLongitude = 0;
        DateTime m_dtMeasureTime;
        string m_strTemperature = "";
        string m_strHumidity = "";
        string m_strDust = "";
        int m_nDirection = 0;
        int m_nVelocity = 0;
        int m_nGrade = 0;
        int m_nWorkStatus = 0;
        double m_dPrevTemperature = 0;
        DateTime m_dtRegDate;

        public int EventID
        {
            get { return m_nEventID; }
            set { m_nEventID = value; }
        }

        public int GroupID
        {
            get { return m_nGroupID; }
            set { m_nGroupID = value; }
        }

        public short UniqueID
        {
            get { return m_nUniqueID; }
            set { m_nUniqueID = value; }
        }

        public double Latitude
        {
            get { return m_dLatitude; }
            set { m_dLatitude = value; }
        }

        public double Longitude
        {
            get { return m_dLongitude; }
            set { m_dLongitude = value; }
        }

        public DateTime MeasureTime
        {
            get { return m_dtMeasureTime; }
            set { m_dtMeasureTime = value; }
        }

        public string Temperature
        {
            get { return m_strTemperature; }
            set { m_strTemperature = value; }
        }

        public string Humidity
        {
            get { return m_strHumidity; }
            set { m_strHumidity = value; }
        }

        /*
        public string Dust
        {
            get { return m_strDust; }
            set { m_strDust = value; }
        }

        public int Direction
        {
            get { return m_nDirection; }
            set { m_nDirection = value; }
        }

        public int Velocity
        {
            get { return m_nVelocity; }
            set { m_nVelocity = value; }
        }
        */
        public int Grade
        {
            get { return m_nGrade; }
            set { m_nGrade = value; }
        }

        public int WorkStatus
        {
            get { return m_nWorkStatus; }
            set { m_nWorkStatus = value; }
        }

        public double PrevTemperature
        {
            get { return m_dPrevTemperature; }
            set { m_dPrevTemperature = value; }
        }

        public DateTime RegDate
        {
            get { return m_dtRegDate; }
            set { m_dtRegDate = value; }
        }
    }

    public class HeatSensor
    {
        private int m_nID = -1;
        private string m_strSensorID = "";
        private int m_nGroupID = -1;
        private int m_nUniqueID = -1;
        private string m_strState = "";
        private string m_strAddr = "";
        private DateTime m_dtOccurTime;
        private float m_fTemperature = 0;
        private float m_fHumidity = 0;
        private float m_fDirection = 0;
        private float m_fSpeed = 0;
        private DateTime m_dtMeasPeriodStart;
        private DateTime m_dtMeasPeriodEnd;
        private DateTime m_dtPreliminaryDate;
        private DateTime m_dtAdvisoryDate;
        private DateTime m_dtAlertDate;
        private int m_nDeathToll = 0;
        private string m_strMessage = "";
        private int m_nUserModifity = 0;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public int GroupID
        {
            get { return m_nGroupID; }
            set { m_nGroupID = value; }
        }

        public int UniqueID
        {
            get { return m_nUniqueID; }
            set { m_nUniqueID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        public DateTime OccurTime
        {
            get { return m_dtOccurTime; }
            set { m_dtOccurTime = value; }
        }

        public float Temperature
        {
            get { return m_fTemperature; }
            set { m_fTemperature = value; }
        }

        public float Humidity
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public float Direction
        {
            get { return m_fDirection; }
            set { m_fDirection = value; }
        }

        public float Speed
        {
            get { return m_fSpeed; }
            set { m_fSpeed = value; }
        }

        public DateTime MeasPeriodStart
        {
            get { return m_dtMeasPeriodStart; }
            set { m_dtMeasPeriodStart = value; }
        }

        public DateTime MeasPeriodEnd
        {
            get { return m_dtMeasPeriodEnd; }
            set { m_dtMeasPeriodEnd = value; }
        }

        public DateTime PreliminaryDate
        {
            get { return m_dtPreliminaryDate; }
            set { m_dtPreliminaryDate = value; }
        }

        public DateTime AdvisoryDate
        {
            get { return m_dtAdvisoryDate; }
            set { m_dtAdvisoryDate = value; }
        }

        public DateTime AlertDate
        {
            get { return m_dtAlertDate; }
            set { m_dtAlertDate = value; }
        }

        public int DeathToll
        {
            get { return m_nDeathToll; }
            set { m_nDeathToll = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public int UserModifity
        {
            get { return m_nUserModifity; }
            set { m_nUserModifity = value; }
        }
    }

    public class FloodInfo
    {
        private bool m_bIsRaining = false;
        private List<RainInfo> m_listRainInfo = new List<RainInfo>();
        private Dictionary<int, RainInfo> m_dicRainInfos = new Dictionary<int, RainInfo>();

        public bool IsRaining
        {
            get { return m_bIsRaining; }
            set { m_bIsRaining = value; }
        }

        public List<RainInfo> ListRainInfos
        {
            get { return m_listRainInfo; }
            set { m_listRainInfo = value; }
        }

        public bool AddRainInfo(string strFall)
        {
            float fFall;
            int nFall;

            if (strFall == null || (float.TryParse(strFall, out fFall) == false))
                return false;

            // 10mm 단위로 계산(30mm부터 계산)
            if (30 > fFall)
            {   // 10mm 이하는 강수량 체크 안함.
                m_bIsRaining = false;
                return true;
            } else if (fFall >= 30 && fFall < 40)
            {
                m_bIsRaining = true;
                nFall = 30;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 40 && fFall < 50)
            {
                m_bIsRaining = true;
                nFall = 40;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 50 && fFall < 60)
            {
                m_bIsRaining = true;
                nFall = 50;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 60 && fFall < 70)
            {
                m_bIsRaining = true;
                nFall = 60;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 70 && fFall < 80)
            {
                m_bIsRaining = true;
                nFall = 70;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 80 && fFall < 90)
            {
                m_bIsRaining = true;
                nFall = 80;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }
            else if (fFall >= 90)
            {
                m_bIsRaining = true;
                nFall = 90;

                if (!m_dicRainInfos.ContainsKey(nFall))
                {   // 등록된 강수량이 아니라면 강수량 등록
                    RainInfo info = new RainInfo();
                    info.Fall = nFall;
                    info.CreateTime = DateTime.Now;

                    m_dicRainInfos[nFall] = info;
                }
            }

            return true;
        }

        public static double InitFallTime(double dTotalMinutes)
        {
            if (dTotalMinutes < 10)
                dTotalMinutes = 0;
            else if (dTotalMinutes >= 10 && dTotalMinutes < 20)
                dTotalMinutes = 10;
            else if (dTotalMinutes >= 20 && dTotalMinutes < 30)
                dTotalMinutes = 20;
            else if (dTotalMinutes >= 30 && dTotalMinutes < 40)
                dTotalMinutes = 30;
            else if (dTotalMinutes >= 40 && dTotalMinutes < 50)
                dTotalMinutes = 40;
            else if (dTotalMinutes >= 50 && dTotalMinutes < 60)
                dTotalMinutes = 50;
            else if (dTotalMinutes >= 60 && dTotalMinutes < 70)
                dTotalMinutes = 60;
            else if (dTotalMinutes >= 70 && dTotalMinutes < 80)
                dTotalMinutes = 70;
            else if (dTotalMinutes >= 80 && dTotalMinutes < 90)
                dTotalMinutes = 80;
            else if (dTotalMinutes >= 90 && dTotalMinutes < 100)
                dTotalMinutes = 90;
            else if (dTotalMinutes >= 100 && dTotalMinutes < 110)
                dTotalMinutes = 100;
            else if (dTotalMinutes >= 110 && dTotalMinutes < 120)
                dTotalMinutes = 110;
            else if (dTotalMinutes >= 120 && dTotalMinutes < 130)
                dTotalMinutes = 120;
            else if (dTotalMinutes >= 130 && dTotalMinutes < 140)
                dTotalMinutes = 130;
            else if (dTotalMinutes >= 140 && dTotalMinutes < 150)
                dTotalMinutes = 140;
            else if (dTotalMinutes >= 150 && dTotalMinutes < 160)
                dTotalMinutes = 150;
            else if (dTotalMinutes >= 160 && dTotalMinutes < 170)
                dTotalMinutes = 160;
            else if (dTotalMinutes >= 170 && dTotalMinutes < 180)
                dTotalMinutes = 170;
            else if (dTotalMinutes >= 180)
                dTotalMinutes = 180;

            return dTotalMinutes;
        }
    }

    public class RainInfo
    {
        private int m_nFall = 0;
        private DateTime m_dtCreateTime;

        public int Fall
        {
            get { return m_nFall; }
            set { m_nFall = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreateTime; }
            set { m_dtCreateTime = value; }
        }
    }

    
}
