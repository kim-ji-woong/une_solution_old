using System;
using System.Collections.Generic;

namespace dnsData.Sensor
{
    public abstract class Facility
    {
        public static List<Facility.FacilityType> UseFacilityType = new List<Facility.FacilityType>();

        // 모든 Facility 및 소방설비와 센서들의 Type 정보를 기록
        public enum FacilityType
        {
            NONE = -1,
            FIRE_SENSOR = 0,        // 화재탐지센서(100번 ~ 199번)
            COOLER_SENSOR = 1,      // 스프링쿨러
            PRESSURE_SENSOR = 2,    // 펌프압력센서
            CCTV = 3,
            FE = 4,                 // 소화기(Fire Extinguisher)
            HD = 5,                 // 소화전(Hydrant)
            FA = 6,                 // 발신기(Fire Alarm)
            FR = 7,                 // 수신반(Fire Receiver)
            PSM_SENSOR = 11,        // 유해화학물질 누출감지 센서
            DISASTER_PREVENTION_EQUIPMENT = 12, // 방재장비
            AIR_QUAILITY = 13,                  // 공기질 센서
            TEMPERATURE_HUMIDITY = 14,          // 온도/습도 센서
            FIREWALL = 15,                      // 방화벽
            DOOR = 16,                          // 출입문
            BLACKOUT = 17,                      // 정전
            STRONG_WIND = 18,                   // 강풍
            SUBMERGENCY = 19,                   // 침수
            TERROR = 20,                        // 테러
            ETC = 21,                           // 기타
            Earthquake = 50,                    // 지진 센서
            FireSensor_TypeA = 101,             // 화재감지기 A
            FireSensor_TypeB = 102,             // 화재감지기 B
            FireSensor_GasEmission = 103,       // 가스 방출신호
            FireSensor_ManualControl = 104,     // 수동조작함 신호
            FireSensor_LightType = 105,         // 광선식
            FireSensor_SiemensType = 106,       // 지멘스 자탐
            FireSensor_Monitoring = 107,        // 감시
            FireSensor_SensingLine = 108,       // 감지선
            FireSensor_AnalogSmokeType = 109,   // 아날로그식 연기
            FireSensor_MonitoringType = 110,     // 감시센서


            // Soulbrain 공장설비
            Temp = 200,
            Humi = 201,
            CO2 = 202,
            TVOC = 203,
            Dust_PM1 = 204,
            Dust_PM2 = 205,
            Dust_PM10 = 206,
            AirPress = 207,
            Inclin_X = 208,
            Inclin_Y = 209,
            Vib_X = 210,
            Vib_Y = 211,
            Vib_Z = 212,
            Noise = 213,
            BLE_Count = 214,
            HF = 215,
            CO = 216,
            O2 = 217,
            Value = 218,
            mA = 219,
            Contact = 220,
            Relay = 221,
            HCL = 222,
            CH3C = 223,
            N2H4 = 224,
            CA = 225,
            EA = 226,
            VOC = 227,
            H2O2 = 228,
            THC = 229,
            HNO3 = 230,
            CL = 231,
            TOLUENE = 232,
            F2 = 233,
            NH3 = 234,
            LNG = 235,
            PGMEA = 236,
            H2S = 237,


            Security_Sensor = 899,              // 방범센서
                                                // 서울대학교 e재난 시스템 - S1시스템 통합으로 추가됨
                                                // skkim     2017-03-14
            Intrusion_S1 = 900,                    // SVMS 침입
            Loiter_S1 = 901,                       // SVMS 배회
            Collapse_S1 = 902,                     // SVMS 쓰러짐
            Theft_S1 = 903,                        // SVMS 도난
            Neglect_S1 = 904,                      // SVMS 방치
            VirtualFence_S1 = 905,                 // SVMS 가상펜스
            Fire_S1 = 906,                         // SVMS 화재
            EmergencyBell_S1 = 907,                // SVMS 비상벨
            GeneralIntrusionT1_S1 = 1001,          // S1Access 일반침입1
            GeneralIntrusionT2_S1 = 1002,          // S1Access 일반 침입2
            InternalIntrusionT3_S1 = 1003,         // S1Access 내부침입
            VaultIntrusionT4_S1 = 1004,            // S1Access 금고침입
            FireF1_S1 = 2000,                      // S1Access 화재
            CustomerEmergencyC1_S1 = 2100,         // S1Access 고객비상
            CustomerEmergencyC2_S1 = 2110,         // S1Access 고객 비상
            RescueQQ_S1 = 2200,                    // S1Access 구급
            GasG1_S1 = 2300,                       // S1Access 가스
            BlackoutAbnormalityU1_S1 = 3000,       // S1Access 정전이상
            LeakAbnormalityU4_S1 = 3004,           // S1Access 누수이상
            SynthesisAlertAbnormalityU8_S1 = 3008, // S1Access 종합경보반 이상
            ExternalAlarmBell = 4000,              // 외부 비상벨

            SecomFire = 5000,                       // SECOM 화재
            SecomExternalAlarmBell = 5001,          // SECOM 외부 비상벨
            SecomWomenAlarmBell = 5002              // SECOM 여자화장실 비상벨
        };

        public static string GetNFacilityTypeString(int facilityType)
        {
            return GetFacilityTypeString(ToFacilityType(facilityType));
        }

        public static string GetFacilityTypeString(FacilityType nType)
        {
            if (nType == Facility.FacilityType.FIRE_SENSOR ||
                nType == Facility.FacilityType.FireSensor_TypeA ||
                nType == Facility.FacilityType.FireSensor_TypeB)
                return "화재센서";
            else if (nType == Facility.FacilityType.COOLER_SENSOR)
                return "스프링쿨러";
            else if (nType == Facility.FacilityType.PRESSURE_SENSOR)
                return "펌프압력";
            else if (nType == Facility.FacilityType.PSM_SENSOR)
                return "유해화학물질 센서";
            else if (nType == Facility.FacilityType.AIR_QUAILITY)
                return "공기질 센서";
            else if (nType == Facility.FacilityType.TEMPERATURE_HUMIDITY)
                return "온도/습도 센서";
            else if (nType == Facility.FacilityType.DISASTER_PREVENTION_EQUIPMENT)
                return "방재장비";
            else if (nType == Facility.FacilityType.FireSensor_Monitoring)
                return "감시";
            else if (nType == Facility.FacilityType.FireSensor_SensingLine)
                return "감지선";
            else if (nType == Facility.FacilityType.FireSensor_AnalogSmokeType)
                return "연기감지기";
            else if (nType == Facility.FacilityType.FireSensor_MonitoringType)
                return "감시센서";
            else if (nType == Facility.FacilityType.CCTV)
                return "CCTV";
            else if (nType == Facility.FacilityType.FE)
                return "소화기";
            else if (nType == Facility.FacilityType.HD)
                return "소화전";
            else if (nType == Facility.FacilityType.FA)
                return "발신기";
            else if (nType == Facility.FacilityType.FR)
                return "수신기";
            else if (nType == Facility.FacilityType.FireSensor_GasEmission)
                return "가스방출";
            else if (nType == Facility.FacilityType.FireSensor_ManualControl)
                return "수동조작함";
            else if (nType == Facility.FacilityType.FireSensor_SiemensType)
                return "지멘스자탐";
            else if (nType == Facility.FacilityType.FireSensor_LightType)
                return "광선식";
            else if (nType == Facility.FacilityType.Intrusion_S1)
                return "지능형영상(침입)";
            else if (nType == Facility.FacilityType.Loiter_S1)
                return "지능형영상(배회)";
            else if (nType == Facility.FacilityType.Collapse_S1)
                return "지능형영상(쓰러짐)";
            else if (nType == Facility.FacilityType.Theft_S1)
                return "지능형영상(도난)";
            else if (nType == Facility.FacilityType.Neglect_S1)
                return "지능형영상(방치)";
            else if (nType == Facility.FacilityType.VirtualFence_S1)
                return "지능형영상(가상펜스)";
            else if (nType == Facility.FacilityType.Fire_S1)
                return "지능형영상(화재)";
            else if (nType >= Facility.FacilityType.GeneralIntrusionT1_S1 && nType <= Facility.FacilityType.SynthesisAlertAbnormalityU8_S1)
                return "S1Access";
            else if (nType == Facility.FacilityType.ExternalAlarmBell)
                return "외부 비상벨";
            else if (nType >= Facility.FacilityType.SecomFire && nType <= Facility.FacilityType.SecomWomenAlarmBell)
                return "세콤";
            else if (nType == FacilityType.FIREWALL)
                return "방화벽";
            else if (nType == FacilityType.DOOR)
                return "출입문";
            else if (nType == FacilityType.BLACKOUT)
                return "정전";
            else if (nType == FacilityType.STRONG_WIND)
                return "강풍";
            else if (nType == FacilityType.TERROR)
                return "테러";
            else if (nType == FacilityType.SUBMERGENCY)
                return "침수";
            else if (nType == FacilityType.Earthquake)
                return "지진";
            else if (nType == FacilityType.ETC)
                return "기타";
            else if (nType == FacilityType.Temp)
                return "온도";
            else if (nType == FacilityType.Humi)
                return "습도";
            else if (nType == FacilityType.CO2)
                return "이산화탄소";
            else if (nType == FacilityType.TVOC)
                return "TVOC";
            else if (nType == FacilityType.Dust_PM1)
                return "미세먼지(PM 1.0)";
            else if (nType == FacilityType.Dust_PM2)
                return "미세먼지(PM 2.5)";
            else if (nType == FacilityType.Dust_PM10)
                return "미세먼지(PM 10)";
            else if (nType == FacilityType.AirPress)
                return "기압";
            else if (nType == FacilityType.Inclin_X)
                return "기울기(X)";
            else if (nType == FacilityType.Inclin_Y)
                return "기울기(Y)";
            else if (nType == FacilityType.Vib_X)
                return "진동(X)";
            else if (nType == FacilityType.Vib_Y)
                return "진동(Y)";
            else if (nType == FacilityType.Vib_Z)
                return "진동(Z)";
            else if (nType == FacilityType.Noise)
                return "소음";
            else if (nType == FacilityType.BLE_Count)
                return "BLE Count";
            else if (nType == FacilityType.HF)
                return "불화수소";
            else if (nType == FacilityType.CO)
                return "일산화탄소";
            else if (nType == FacilityType.O2)
                return "산소";
            else if (nType == FacilityType.Value)
                return "ESH_v5.1 측정값";
            else if (nType == FacilityType.mA)
                return "mA";
            else if (nType == FacilityType.Contact)
                return "접점";
            else if (nType == FacilityType.Relay)
                return "릴레이";
            else if (nType == FacilityType.HCL)
                return "염화수소";
            else if (nType == FacilityType.CH3C)
                return "초산";
            else if (nType == FacilityType.N2H4)
                return "하이드라진";
            else if (nType == FacilityType.CA)
                return "CA Gas";
            else if (nType == FacilityType.EA)
                return "에틸알콜";
            else if (nType == FacilityType.VOC)
                return "VOC";
            else if (nType == FacilityType.H2O2)
                return "과수";
            else if (nType == FacilityType.THC)
                return "에탄올";
            else if (nType == FacilityType.HNO3)
                return "질산";
            else if (nType == FacilityType.CL)
                return "염소가스";
            else if (nType == FacilityType.TOLUENE)
                return "톨루엔";
            else if (nType == FacilityType.F2)
                return "불소";
            else if (nType == FacilityType.NH3)
                return "암모니아";
            else if (nType == FacilityType.LNG)
                return "액화천연가스";
            else if (nType == FacilityType.PGMEA)
                return "유기가스";
            else if (nType == FacilityType.H2S)
                return "황화수소";

            return "";
        }

        #region 
        public static bool IsSecurityType(FacilityType type)
        {
            if ((type >= FacilityType.Security_Sensor && type <= FacilityType.EmergencyBell_S1) ||
                (type >= FacilityType.GeneralIntrusionT1_S1 && type <= FacilityType.VaultIntrusionT4_S1) ||
                type == FacilityType.CustomerEmergencyC1_S1 || type == FacilityType.CustomerEmergencyC2_S1 ||
                type == FacilityType.RescueQQ_S1 || type == FacilityType.GasG1_S1 ||
                type == FacilityType.BlackoutAbnormalityU1_S1 || type == FacilityType.LeakAbnormalityU4_S1 ||
                type == FacilityType.SynthesisAlertAbnormalityU8_S1 || type == FacilityType.ExternalAlarmBell ||
                type == FacilityType.SecomExternalAlarmBell || type == FacilityType.SecomWomenAlarmBell)
                return true;

            return false;
        }

        public static bool IsFireSensorType(FacilityType type)
        {
            if (type == FacilityType.FIRE_SENSOR ||
                (type >= FacilityType.FireSensor_TypeA && type <= FacilityType.FireSensor_MonitoringType) ||
                type == FacilityType.Fire_S1 ||
                type == FacilityType.FireF1_S1 ||
                type == FacilityType.SecomFire)
                return true;

            return false;
        }

        public static bool IsETCSensorType(FacilityType type)
        {
            if ((type >= FacilityType.FIREWALL && type <= FacilityType.ETC) ||
                type == FacilityType.Temp ||
                type == FacilityType.Humi ||
                type == FacilityType.CO2 ||
                type == FacilityType.TVOC ||
                type == FacilityType.Dust_PM1 ||
                type == FacilityType.Dust_PM2 ||
                type == FacilityType.Dust_PM10 ||
                type == FacilityType.AirPress ||
                type == FacilityType.Inclin_X ||
                type == FacilityType.Inclin_Y ||
                type == FacilityType.Vib_X ||
                type == FacilityType.Vib_Y ||
                type == FacilityType.Vib_Z ||
                type == FacilityType.Noise ||
                type == FacilityType.BLE_Count ||
                type == FacilityType.O2 ||
                type == FacilityType.Value ||
                type == FacilityType.mA ||
                type == FacilityType.Contact ||
                type == FacilityType.Relay)
                return true;

            return false;
        }

        public static bool IsPSMSensorType(FacilityType type)
        {
            if (type == FacilityType.PSM_SENSOR ||
                type == FacilityType.HF ||
                type == FacilityType.CO ||
                type == FacilityType.HCL ||
                type == FacilityType.CH3C ||
                type == FacilityType.N2H4 ||
                type == FacilityType.CA ||
                type == FacilityType.EA ||
                type == FacilityType.VOC ||
                type == FacilityType.H2O2 ||
                type == FacilityType.THC ||
                type == FacilityType.HNO3 ||
                type == FacilityType.CL ||
                type == FacilityType.TOLUENE ||
                type == FacilityType.F2 ||
                type == FacilityType.NH3 ||
                type == FacilityType.LNG ||
                type == FacilityType.PGMEA ||
                type == FacilityType.H2S) 
                return true;

            return false;
        }

        public static bool IsSVMSSensorType(FacilityType type)
        {
            if (type == FacilityType.Intrusion_S1 ||
                type == FacilityType.Loiter_S1 ||
                type == FacilityType.Collapse_S1 ||
                type == FacilityType.Theft_S1 ||
                type == FacilityType.Neglect_S1 ||
                type == FacilityType.VirtualFence_S1 ||
                type == FacilityType.Fire_S1 ||
                type == FacilityType.EmergencyBell_S1)
                return true;

            return false;
        }

        public static List<int> GetFireTypeAllNumberToList()
        {
            List<int> fires = new List<int>();
            fires.Add((int)FacilityType.FIRE_SENSOR);
            fires.Add((int)FacilityType.FireSensor_TypeA);
            fires.Add((int)FacilityType.FireSensor_TypeB);
            fires.Add((int)FacilityType.FireSensor_GasEmission);
            fires.Add((int)FacilityType.FireSensor_ManualControl);
            fires.Add((int)FacilityType.FireSensor_LightType);
            fires.Add((int)FacilityType.FireSensor_SiemensType);
            fires.Add((int)FacilityType.FireSensor_Monitoring);
            fires.Add((int)FacilityType.FireSensor_SensingLine);
            fires.Add((int)FacilityType.FireSensor_AnalogSmokeType);
            fires.Add((int)FacilityType.FireSensor_MonitoringType);
            fires.Add((int)FacilityType.Fire_S1);
            fires.Add((int)FacilityType.FireF1_S1);
            fires.Add((int)FacilityType.SecomFire);

            return fires;
        }

        public static List<int> GetETCTypeAllNumberToList()
        {
            List<int> etcs = new List<int>();
            etcs.Add((int)FacilityType.FIREWALL);
            etcs.Add((int)FacilityType.DOOR);
            etcs.Add((int)FacilityType.BLACKOUT);
            etcs.Add((int)FacilityType.STRONG_WIND);
            etcs.Add((int)FacilityType.SUBMERGENCY);
            etcs.Add((int)FacilityType.TERROR);
            etcs.Add((int)FacilityType.ETC);
            etcs.Add((int)FacilityType.Temp);
            etcs.Add((int)FacilityType.Humi);
            etcs.Add((int)FacilityType.CO2);
            etcs.Add((int)FacilityType.TVOC);
            etcs.Add((int)FacilityType.Dust_PM1);
            etcs.Add((int)FacilityType.Dust_PM2);
            etcs.Add((int)FacilityType.Dust_PM10);
            etcs.Add((int)FacilityType.AirPress);
            etcs.Add((int)FacilityType.Inclin_X);
            etcs.Add((int)FacilityType.Inclin_Y);
            etcs.Add((int)FacilityType.Vib_X);
            etcs.Add((int)FacilityType.Vib_Y);
            etcs.Add((int)FacilityType.Vib_Z);
            etcs.Add((int)FacilityType.Noise);
            etcs.Add((int)FacilityType.BLE_Count);
            etcs.Add((int)FacilityType.O2);
            etcs.Add((int)FacilityType.Value);
            etcs.Add((int)FacilityType.mA);
            etcs.Add((int)FacilityType.Contact);
            etcs.Add((int)FacilityType.Relay);

            return etcs;
        }

        public static List<int> GetPSMTypeAllNumberToList()
        {
            List<int> psms = new List<int>();
            psms.Add((int)FacilityType.PSM_SENSOR);
            psms.Add((int)FacilityType.HF);
            psms.Add((int)FacilityType.CO);
            psms.Add((int)FacilityType.HCL);
            psms.Add((int)FacilityType.CH3C);
            psms.Add((int)FacilityType.N2H4);
            psms.Add((int)FacilityType.CA);
            psms.Add((int)FacilityType.EA);
            psms.Add((int)FacilityType.VOC);
            psms.Add((int)FacilityType.H2O2);
            psms.Add((int)FacilityType.THC);
            psms.Add((int)FacilityType.HNO3);
            psms.Add((int)FacilityType.CL);
            psms.Add((int)FacilityType.TOLUENE);
            psms.Add((int)FacilityType.F2);
            psms.Add((int)FacilityType.NH3);
            psms.Add((int)FacilityType.LNG);
            psms.Add((int)FacilityType.PGMEA);
            psms.Add((int)FacilityType.H2S);

            return psms;
        }

        public static List<int> GetSVMSTypeAllNumberToList()
        {
            List<int> psms = new List<int>();
            psms.Add((int)FacilityType.Intrusion_S1);
            psms.Add((int)FacilityType.Loiter_S1);
            psms.Add((int)FacilityType.Collapse_S1);
            psms.Add((int)FacilityType.Theft_S1);
            psms.Add((int)FacilityType.Neglect_S1);
            psms.Add((int)FacilityType.VirtualFence_S1);
            psms.Add((int)FacilityType.Fire_S1);
            psms.Add((int)FacilityType.EmergencyBell_S1);

            return psms;
        }

        public static bool IsEarthquakeSensorType(FacilityType type)
        {
            return type == FacilityType.Earthquake;
        }
        #endregion

        public enum FacilitySubType
        {
            //NULL:일반, 0:열, 1:연기, 2:불꽃
            None = -1,
            Heat = 0,
            Smoke = 1,
            Spark = 2
        }

        public static string GetFacilitySubTypeString(FacilitySubType nType)
        {
            switch (nType)
            {
                case FacilitySubType.None:
                    return "일반";
                case FacilitySubType.Heat:
                    return "열";
                case FacilitySubType.Smoke:
                    return "연기";
                case FacilitySubType.Spark:
                    return "불꽃";
            }

            return "";
        }

        private static Dictionary<int, FacilityType> m_dicFacilityType = null;

        protected int m_nID = -1;
        
        public bool m_bConnected = false;

        public bool Connected
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }

        abstract public FacilityType Type
        {
            get;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // nFacilityType : DB 스키마에 정의된 값
        public static FacilityType ToFacilityType(int nFacilityType)
        {
            if (m_dicFacilityType == null)
            {
                m_dicFacilityType = new Dictionary<int, FacilityType>();

                foreach (FacilityType type in Enum.GetValues(typeof(FacilityType)))
                {
                    m_dicFacilityType[(int)type] = type;
                }
            }

            FacilityType fType;

            if (m_dicFacilityType.TryGetValue(nFacilityType, out fType))
                return fType;

            return FacilityType.NONE;
        }
    }
}
