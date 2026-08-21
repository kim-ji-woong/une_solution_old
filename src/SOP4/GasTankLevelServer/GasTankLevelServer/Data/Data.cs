using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GasLevelServer
{
    public static class Facility
    {
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
            FireSensor_TypeA = 101,             // 화재감지기 A
            FireSensor_TypeB = 102,             // 화재감지기 B
            FireSensor_GasEmission = 103,       // 가스 방출신호
            FireSensor_ManualControl = 104,     // 수동조작함 신호
            FireSensor_LightType = 105,         // 광선식
            FireSensor_SiemensType = 106,       // 지멘스 자탐
            FireSensor_Monitoring = 107,        // 감시
            FireSensor_SensingLine = 108,       // 감지선
            FireSensor_AnalogSmokeType = 109,   // 아날로그식 연기
            FireSensor_MonitoringType = 110     // 감시센서
        };

        private static Dictionary<int, FacilityType> m_dicFacilityType = null;

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


	public class Reciver
	{
		private int m_nID;
		public int ID
		{
			get { return m_nID; }
			set { m_nID = value; }
		}

        private int m_nSlaveID = -1;
        public int SlaveID
        {
            get { return m_nSlaveID; }
            set { m_nSlaveID = value; }
        }

		private string m_strAddress;
		public string Address
		{
			get { return m_strAddress; }
			set { m_strAddress = value; }
		}  

		private string m_szName = "";
		public string Place
		{
			get { return m_szName; }
			set { m_szName = value; }
		}

        public override string ToString()
        {
            return (m_nID.ToString() + ". " + m_szName);
        }

		private int m_nTimeout = 3000;
		public int Timeout
		{
			get { return m_nTimeout; }
			set { m_nTimeout = value; }
		}
		private Dictionary<int, Circuit> m_dicCurcuitList = new Dictionary<int, Circuit>();
		public Dictionary<int, Circuit> Curcuits
		{
			get { return m_dicCurcuitList; }
			set { m_dicCurcuitList = value; }
		}

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set { m_isConnected = value; }
        }

        private int m_nReciverID = -1;

        public int ReciverID
        {
            get { return m_nReciverID; }
            set { m_nReciverID = value; }
        }

        private int m_nType = 0;
        public int ReciverType
        {
            get { return m_nType; }
            set { m_nType = value; }
        }
	}

	public class Circuit
	{
		private int m_nReciverID = -1;
		public int ReciverID
		{
			get { return m_nReciverID; }
			set { m_nReciverID = value; }
		}

		private int m_nID = -1;
		public int ID
		{
			get { return m_nID; }
			set { m_nID = value; }
		}

        private int m_nSlaveID = -1;
        public int SlaveID
        {
            get { return m_nSlaveID; }
            set { m_nSlaveID = value; }
        }

		private int m_nTagNum = -1;
		public int TagNum
		{
			get { return m_nTagNum; }
			set { m_nTagNum = value; }
		}

		public int m_nTankCount = 0;
        public int TankCount
		{
            get { return m_nTankCount; }
            set { m_nTankCount = value; }
		}

		private string m_szName = "";
		public string Name
		{
			get { return m_szName; }
			set { m_szName = value; }
		}

        public override string ToString()
        {
            return (m_nTagNum.ToString() + ". " + m_szName);
        }
	}

}
