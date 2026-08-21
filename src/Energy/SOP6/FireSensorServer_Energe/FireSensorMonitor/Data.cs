using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SensorMonitor
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
        private static object m_lockObj = new object();
        // nFacilityType : DB 스키마에 정의된 값
        public static FacilityType ToFacilityType(int nFacilityType)
        {
            lock(m_lockObj)
            {
                if (m_dicFacilityType == null)
                {

                    m_dicFacilityType = new Dictionary<int, FacilityType>();

                    Array arValues = Enum.GetValues(typeof(FacilityType));
                    foreach (FacilityType type in arValues)
                    {
                        m_dicFacilityType[(int)type] = type;
                    }
                }
            }            

            FacilityType fType;

            if (m_dicFacilityType.TryGetValue(nFacilityType, out fType))
                return fType;

            return FacilityType.NONE;
        }
    }


    class BuildingGroup : Object
    {
        private int m_ID = -1;
        private string m_strBuildingGroupName = "";
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }
        public override string ToString()
        {
            return m_strBuildingGroupName.ToString();
        }
    }

    class Building : Object
    {
        private BuildingGroup m_buildingGroup = null;

        private int m_ID = -1;
        private string m_strBuildName = "";
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        internal BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }
        public string BuildingName
        {
            get { return m_strBuildName; }
            set { m_strBuildName = value; }
        }
        public override string ToString()
        {
            return m_strBuildName.ToString();
        }
    }

    public class Zone : Object
    {
        // m_building이 null이면 외부 공간
        //private Building m_building = null;

        private Building m_building = null;

        private int m_nID = -1;
        private int m_nFloorIndex = 0;
        private string m_strZoneName = "";
        private string m_strDXFFilePath = "";
        // .5층, .2층과 같이 복층을 표기하기 위한 값
        private float m_fAddFloor = 0.0f;

        public override string ToString()
        {
            return m_strZoneName;
        }

        internal Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string DXFFilePath
        {
            get { return m_strDXFFilePath; }
            set { m_strDXFFilePath = value; }
        }

        public float AddFloor
        {
            get { return m_fAddFloor; }
            set { m_fAddFloor = value; }
        }
    }

    public class Floor : Object, IComparable
    {
        // 0은 1층, 1은 2층, 지하는 음수
        //private int m_nFloorIndex = 0;
        private float m_fFloorIndex = 0.0f;

        public Floor(float fFloorIndex)
        {
            m_fFloorIndex = fFloorIndex;
        }

		public float FloorIndex
        {
            get { return m_fFloorIndex; }
            set { m_fFloorIndex = value; }
        }

        public int CompareTo(object obj)
        {
            Floor floor = (Floor)obj;

            if (this.m_fFloorIndex > floor.m_fFloorIndex)
                return 1;
            else if (this.m_fFloorIndex < floor.m_fFloorIndex)
                return -1;
            //else
            return 0;
        }

        public override string ToString()
        {
            string strResult = "";

            if (m_fFloorIndex < 0)
                strResult = string.Format("지하 {0:f1}층", -m_fFloorIndex);
            else
                strResult = string.Format("{0:f1}층", m_fFloorIndex + 1);

            if (strResult.EndsWith(".0층"))
                return strResult.Substring(0, strResult.Length - 3) + "층";

            return strResult;
        }
    }

    public class SensorZone : Object
    {
        private EquipmentZone m_Zone = null;

        public EquipmentZone EquipZone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private int type = 1;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public override string ToString()
        {
            string strResult = "";

            if (type == 1)
                strResult = "화재 탐지";
            else if (type == 2)
                strResult = "소화 센서";
            else
                strResult = "압력 센서";

            return strResult;
        }
    }

    // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기를 위한 Zone
    public class EquipmentZone : Object
    {
        // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, OTHER_TYPE, UNKOWN };

        private int m_nID = -1;
        private string m_strName = "";
        private ArrayList m_arrLinkedZoneList = new ArrayList();
        private EquipZoneType m_type = EquipZoneType.UNKOWN;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string EquipZoneName
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public ArrayList LinkedZoneList
        {
            get { return m_arrLinkedZoneList; }
        }

        public EquipZoneType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public override string ToString()
        {
            return m_strName;
        }
    }

    public class ReciverState
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private Reciver m_Reciver = null;
        public Reciver TargetReciver
        {
            get { return m_Reciver; }
            set { m_Reciver = value; }
        }

        private bool m_bConnected = false;
        public bool Connected
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }

        private DateTime m_dtLastAccess;
        public DateTime LastAccess
        {
            get { return m_dtLastAccess; }
            set { m_dtLastAccess = value; }
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

		private string m_strAddress;
		public string Address
		{
			get { return m_strAddress; }
			set { m_strAddress = value; }
		}

		private int m_nPort;
		public int Port
		{
			get { return m_nPort; }
			set { m_nPort = value; }
		}

		private int m_nBuadRate = 9600;
		public int BuadRate
		{
			get { return m_nBuadRate; }
			set { m_nBuadRate = value; }
		}

		private string m_nMacAddress = "";
		public string MacAddress
		{
			get { return m_nMacAddress; }
			set { m_nMacAddress = value; }
		}

		private int m_nMode = 3;
		public int Mode
		{
			get { return m_nMode; }
			set { m_nMode = value; }
		}

		private int m_nFlowCtrl = 3;
		public int FlowCtrl
		{
			get { return m_nFlowCtrl; }
			set { m_nFlowCtrl = value; }
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
        private Dictionary<string, Curcuit> m_dicCurcuitList = new Dictionary<string, Curcuit>();
		public Dictionary<string, Curcuit> Curcuits
		{
			get { return m_dicCurcuitList; }
			set { m_dicCurcuitList = value; }
		}

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set 
            {
                m_isConnected = value;
                if( m_isConnected == false)
                {
                    m_bRecivePoll = false;
                }
            }
        }

        private bool m_bRecivePoll = false;
        public bool RecivedPoll
        {
            get { return m_bRecivePoll; }
            set 
            { 
                m_bRecivePoll = value;
            }
        }

	}

	public class Curcuit
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
		private int m_nTargetZoneID = -1;
		public int TargetZoneID
		{
			get { return m_nTargetZoneID; }
			set { m_nTargetZoneID = value; }
		}

		private int m_nTagNum = -1;
		public int TagNum
		{
			get { return m_nTagNum; }
			set { m_nTagNum = value; }
		}

        private string m_nTagID = "";
        public string TagID
        {
            get { return m_nTagID; }
            set { m_nTagID = value; }
        }

		//1(화재탐지 센서), 2(소화 센서), 3(압력 센서), 4(발신기)
		//화재 센서(0), 화재감지기 A(1), 화재감지기 B(2), 가스 방출신호(3), 수동조작함 신호(4)
		public int m_nSensorType = 1;
		public int SensorType
		{
			get { return m_nSensorType; }
			set { m_nSensorType = value; }
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
