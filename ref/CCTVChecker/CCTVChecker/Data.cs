using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CCTVChecker
{
    public class ID
    {
        public const int ID_LAYER_DETECTOR = 5100;
        public const int ID_LAYER_COOLER = 5101;
        public const int ID_LAYER_PERSURE = 5102;
        public const int ID_LAYER_CCTV = 5103;
        public const int ID_LAYER_FIREEXT = 5104;
        public const int ID_LAYER_FIREHYD = 5105;
        public const int ID_LAYER_ALARMSTA = 5106;
        public const int ID_LAYER_RECIVER = 5107;
        public const int ID_LAYER_TEXTPOI = 5108;
        public const int ID_LAYER_CCTVLOW = 5109;
    }

    public class BuildingGroup
    {          
        
        private float m_TextCenterY = 0.0f;
        private float m_TextCenterX = 0.0f;

        private ArrayList m_arBuildingList = new ArrayList();

        private string m_strBuildingGroupName = "";
        private int m_nSiteID = -1;
        private int m_nID = -1;
        private string m_strSiteName = "";
        
        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }

        public int GroupID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }

        public float TextCenterX
        {
            get { return m_TextCenterX; }
            set { m_TextCenterX = value; }
        }

        public float TextCenterY
        {
            get { return m_TextCenterY; }
            set { m_TextCenterY = value; }
        }

        public System.Collections.ArrayList BuildingList
        {
            get { return m_arBuildingList; }
        }

        public override string ToString()
        {
            return m_strBuildingGroupName;
        }
    }   

    public class Building
    {
        private BuildingGroup m_buildingGroup = null;
        private ArrayList m_arFloorList = new ArrayList();
		private ArrayList m_arEquipZoneList = new ArrayList();
        
        private string m_strBuildingName = "";
        // 0은 1층, 1은 2층, 지하는 음수
        private int m_nMinFloorIndex = 0;
        private int m_nMaxFloorIndex = 0;

        private string m_strBuildingID = "";
        private string m_strBuildingCode = "";

        private int m_ID = -1;

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public System.Collections.ArrayList FloorList
        {
            get { return m_arFloorList; }
            set { m_arFloorList = value; }
        }
		public System.Collections.ArrayList EquipZoneList
		{
			get { return m_arEquipZoneList; }
			set { m_arEquipZoneList = value; }
		}

        public override string ToString()
        {
            return m_strBuildingName;
        }

        public BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int MinFloorIndex
        {
            get { return m_nMinFloorIndex; }
            set { m_nMinFloorIndex = value; }
        }

        public int MaxFloorIndex
        {
            get { return m_nMaxFloorIndex; }
            set { m_nMaxFloorIndex = value; }
        }

        public string BuildingID
        {
            get { return m_strBuildingID; }
            set { m_strBuildingID = value; }
        }

        public string BuildingCode
        {
            get { return m_strBuildingCode; }
            set { m_strBuildingCode = value; }
        }
        protected string szBroadcastName;
        public string BroadcastName
        {
            get { return szBroadcastName; }
            set { szBroadcastName = value; }
        }
    }

    public class Zone
    {
        private Floor m_Floor = new Floor();
        public Floor Floor
        {
            get { return m_Floor; }
            set { m_Floor = value; }
        }

        // m_building이 null이면 외부 공간
        private Building m_building = null;
        
        private int mID = 0;
        private float m_fAddFloor = 0.0f;

        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        private int m_nFloorIndex = -1;
        private string m_strZoneName = "";

        public override string ToString()
        {
            return m_strZoneName;
        }

        public bool IsOutdoor
        {
            get { return m_building == null; }
        }

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
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

        private UnE.Geometry.Polygon polygon;
        public UnE.Geometry.Polygon Polygon
        {
            get { return polygon; }
            set { polygon = value; }
        }

        public string szBroadcastName;
        public string BroadcastName
        {
            get { return szBroadcastName; }
            set { szBroadcastName = value; }
        }
        public float AddFloor
        {
            get { return m_fAddFloor; }
            set { m_fAddFloor = value; }
        }

        private string strDXFFilePath = "";
        public string DXFFilePath
        {
            get { return strDXFFilePath; }
            set { strDXFFilePath = value; }
        }

        private string strDXFFileName = "";
        public string DXFFileName
        {
            get { return strDXFFileName; }
            set { strDXFFileName = value; }
        }
    }

	public class EquipmentZone
	{
        // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, OTHER_TYPE, UNKOWN };

		private Floor m_Floor = new Floor();
		public Floor Floor
		{
			get { return m_Floor; }
			set { m_Floor = value; }
		}

		private ArrayList m_arLinkedZoneList = new ArrayList();
		public System.Collections.ArrayList LinkedZoneList
		{
			get { return m_arLinkedZoneList; }
			set { m_arLinkedZoneList = value; }
		}
        
		private Zone m_LinkedZone = null;
		public Zone LinkedZone
		{
			get { return m_LinkedZone; }
			set 
			{
				m_LinkedZone = value;
				if (m_LinkedZone != null)
				{
					m_Floor = m_LinkedZone.Floor;
					m_building = m_LinkedZone.Building;
					m_nFloorIndex = m_LinkedZone.FloorIndex;
					m_fAddFloor = m_LinkedZone.AddFloor;
				}
				else
				{
					m_building = null;
					m_nFloorIndex = -1;
					m_fAddFloor = 0.0f;
				}
			}
		}
		//0 : 센서 Zone, 1 : 발신기 Zone
        private EquipZoneType m_nZoneType = EquipZoneType.UNKOWN;
		public EquipZoneType ZoneType
		{
			get { return m_nZoneType; }
			set { m_nZoneType = value; }
		}

		// m_building이 null이면 외부 공간
		private Building m_building = null;

		private int mID = 0;
		private float m_fAddFloor = 0.0f;

		public int ID
		{
			get { return mID; }
			set { mID = value; }
		}
		private int m_nFloorIndex = -1;
		private string m_strZoneName = "";

		public override string ToString()
		{
			return m_strZoneName;
		}

		public bool IsOutdoor
		{
			get { return m_building == null; }
		}

		public Building Building
		{
			get { return m_building; }
			set { m_building = value; }
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

		private UnE.Geometry.Polygon polygon;
		public UnE.Geometry.Polygon Polygon
		{
			get { return polygon; }
			set { polygon = value; }
		}

		public string szBroadcastName;
		public string BroadcastName
		{
			get { return szBroadcastName; }
			set { szBroadcastName = value; }
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
        private float m_fFloorIndex = 0.0f;
        private Zone m_zone = null;

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public Floor(float fFloorIndex = 0.0f)
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

    public class SensorReactionHistory
    {
        private Zone zone = null;
        private int m_ZoneID = -1;
        private int nReactionCount = -1;
        private int nMulFunctionCount = -1;
        private int nFireCount = -1;
        private int nManualFireCount = -1;

        private string strBuildingGroupName = "";
        private string strBuildingName = "";
        private string strZoneName = "";
        private string strAddFloor = "";
        private float nFloorIndex = -1;
        private int nType = -1;
        private int nBuildingID = -1;

        public int BuildingID
        {
            get { return nBuildingID; }
            set { nBuildingID = value; }
        }
        private DateTime dtTime = new DateTime();

        public DateTime DtTime
        {
            get { return dtTime; }
            set { dtTime = value; }
        }
        
        public int Type
        {
            get { return nType; }
            set { nType = value; }
        }

        public int ZoneID
        {
            get { return m_ZoneID; }
            set { m_ZoneID = value; }
        }
        public Zone Zone
        {
            get { return zone; }
            set { zone = value; }
        }

        public string BuildingGroupName
        {
            get { return strBuildingGroupName; }
            set { strBuildingGroupName = value; }
        }
        public string BuildingName
        {
            get { return strBuildingName; }
            set { strBuildingName = value; }
        }
        public string ZoneName
        {
            get { return strZoneName; }
            set { strZoneName = value; }
        }

        public int ReactionCount
        {
            get { return nReactionCount; }
            set { nReactionCount = value; }
        }
        public int MulFunctionCount
        {
            get { return nMulFunctionCount; }
            set { nMulFunctionCount = value; }
        }
        public int FireCount
        {
            get { return nFireCount; }
            set { nFireCount = value; }
        }

        public int ManualFireCount
        {
            get { return nManualFireCount; }
            set { nManualFireCount = value; }
        }
        public string AddFloor
        {
            get { return strAddFloor; }
            set { strAddFloor = value; }
        }
        public float FloorIndex
        {
            get { return nFloorIndex; }
            set { nFloorIndex = value; }
        }
    }
  
    public abstract class Facility
    {
        // 자탐센서(Detector), 스프링쿨러, 펌프압력센서, CCTV, 소화기(Fire Extinguisher), 소화전(Hydrant), 발신기(Fire Alarm)
        public enum FacilityType { NONE = 0, FIRE_SENSOR, COOLER_SENSOR, PRESSURE_SENSOR, CCTV, FE, HD, FA };

        protected int m_nID = -1;
        protected POI m_poi = null;

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
        abstract public int GetLayerID();

        abstract public string IconPath
        {
            get;
        }
		abstract public string DisconnectIconPath
		{
			get;
		}

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public POI POI
        {
            get { return m_poi; }
            set
            {
                if (m_poi != value)
                {
                    if (m_poi != null)
                        m_poi.SetNullFacility();

                    m_poi = value;

                    if (m_poi != null)
                        m_poi.Facility = this;
                }
            }
        }

        public void SetNullPOI()
        {
            m_poi = null;
        }

        //public virtual IPOIPopup CreatePopup(BaseViewEx view)
        //{
         //   return null;
        //}

        public virtual void UpdateDBData()
        {
        }

        // nFacilityType : DB 스키마에 정의된 값
        public static FacilityType ToFacilityType(int nFacilityType)
        {
            if (nFacilityType == 0)
                return FacilityType.FIRE_SENSOR;
            else if (nFacilityType == 1)
                return FacilityType.COOLER_SENSOR;
            else if (nFacilityType == 2)
                return FacilityType.PRESSURE_SENSOR;
            else if (nFacilityType == 3)
                return FacilityType.CCTV;
            else if (nFacilityType == 4)
                return FacilityType.FE;
            else if (nFacilityType == 5)
                return FacilityType.HD;
            else if (nFacilityType == 6)
                return FacilityType.FA;

            return FacilityType.NONE;
        }

        public static int ToIntType(FacilityType type)
        {
            if (type == FacilityType.FIRE_SENSOR)
                return 0;
            else if (type == FacilityType.COOLER_SENSOR)
                return 1;
            else if (type == FacilityType.PRESSURE_SENSOR)
                return 2;
            else if (type == FacilityType.CCTV)
                return 3;
            else if (type == FacilityType.FE)
                return 4;
            else if (type == FacilityType.HD)
                return 5;
            else if (type == FacilityType.FA)
                return 6;

            return -1;
        }
    }

    // Pump Pressuer Sensor
    public abstract class SensorZone : Facility
    {
        public int m_ZoneID = -1;
        public int EquipZoneID
        {
            get { return m_ZoneID; }
            set { m_ZoneID = value; }
        }
        
        public string m_szDescription = "";
        public string Description
        {
            get { return m_szDescription; }
            set { m_szDescription = value; }
        }

        public int m_nSensorData = -1;
        public int SensorData
        {
            get { return m_nSensorData; }
            set { m_nSensorData = value; }
        }
        public bool m_bInitSensor = true;
        public bool InitSensor
        {
            get { return m_bInitSensor; }
            set { m_bInitSensor = value; }
        }

        public int m_nOrgID = -1;
        public int OrgSensorID
        {
            get { return m_nOrgID; }
            set { m_nOrgID = value; }
        }

		//override public IPOIPopup CreatePopup(BaseViewEx view)
		//{
		//	return null;
		//}

		public string TypeString
		{
			get
			{
				if (Type == FacilityType.FIRE_SENSOR)
					return "화재센서";
				else if (Type == FacilityType.COOLER_SENSOR)
					return "스프링쿨러";
				else if (Type == FacilityType.PRESSURE_SENSOR)
					return "압력센서";

				return "Unknown";
			}
		}
    }

	public partial class FireAlarm : SensorZone
	{
		private FireEquipment m_AlarmStation = null;
		public FireEquipment AlarmStation
		{
			get { return m_AlarmStation; }
			set { m_AlarmStation = value; }
		}
		public override Facility.FacilityType Type
		{
			get { return FacilityType.FA; }
		}
		override public int GetLayerID()
		{
            return CCTVChecker.ID.ID_LAYER_ALARMSTA;
		}
		public override string IconPath
		{
			get { return ""; }
		}
		public override string DisconnectIconPath
		{
			get { return ""; }
		}
	}

    public partial class PumpPressureSensor : SensorZone 
    {
        public override Facility.FacilityType Type
        {
            get { return FacilityType.PRESSURE_SENSOR; }
        }
        override public int GetLayerID()
        {
            return CCTVChecker.ID.ID_LAYER_PERSURE;
        }
        public override string IconPath
        {
            get { return ""; }
        }
		public override string DisconnectIconPath
		{
			get { return ""; }
		}
    }

    // Sping Cooler
    public partial class SpringCooler : SensorZone
    {
        public override Facility.FacilityType Type
        {
            get { return FacilityType.COOLER_SENSOR; }
        }
        override public int GetLayerID()
        {
            return CCTVChecker.ID.ID_LAYER_COOLER;
        }
        public override string IconPath
        {
            get { return ""; }
        }
		public override string DisconnectIconPath
		{
			get { return ""; }
		}
    }

    // Fire Detector
    public partial class FireSensor : SensorZone
    {
        public override Facility.FacilityType Type
        {
            get { return FacilityType.FIRE_SENSOR; }
        }
        override public int GetLayerID()
        {
            return CCTVChecker.ID.ID_LAYER_DETECTOR;
        }
        public override string IconPath
        {
            get { return ""; }
        }
		public override string DisconnectIconPath
		{
			get { return ""; }
		}
    }


    public partial class CCTV : Facility
    {
        public enum LOD { LOW = 0, DEFAULT = 1, IMPORTANT, VERY_IMPORTANT };

        private static short m_nDefaultPort = 9400;
        public static short DefaultPortNo
        {
            get { return m_nDefaultPort; }
        }

        private string m_strIP = "0.0.0.0";
        private short m_nPort = -1;
        private string m_strAccessKey = "BNC-3220HR-W";
        private short m_nPlaybackMode = 0;
        private short m_nUseRepository = 0;
        private byte[] m_bytes = new byte[4] { 0, 0, 0, 0 };
        private LOD m_lod = LOD.DEFAULT;

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        private string m_strIPDB = "0.0.0.0";
        private short m_nPortDB = -1;
        private string m_strAccessKeyDB = "BNC-3220HR-W";
        private short m_nPlaybackModeDB = 0;
        private short m_nUseRepositoryDB = 0;
        private byte[] m_bytesDB = new byte[4] { 0, 0, 0, 0 };
        private LOD m_lodDB = LOD.DEFAULT;
        /// </summary>
        /// 
        public CCTV()
        {
            m_nPort = m_nPortDB = DefaultPortNo;
        }

        public override Facility.FacilityType Type
        {
            get { return FacilityType.CCTV; }
        }

        override public int GetLayerID()
        {
            if (m_lod == LOD.LOW)
                return CCTVChecker.ID.ID_LAYER_CCTVLOW;

            return CCTVChecker.ID.ID_LAYER_CCTV;
        }

        public override string IconPath
        {
            get { return ""; }
        }
		public override string DisconnectIconPath
		{
			get { return ""; }
			//get { return FormMain.EnginPath() + "\\Media\\icons\\cctv_disconnected.ico"; }
		}

        // 라이브 모드 (0 : Live, 1 : Playback)
        public short PlayBackMode
        {
            get { return m_nPlaybackMode; }
            set { m_nPlaybackMode = value; }
        }

        // 리포지토리와 연동함 (0 : 사용하지 않음, 1: 사용함(사용할 시 위 리포지토리 연결 부분 정의))
        public short UseRepository
        {
            get { return m_nUseRepository; }
            set { m_nUseRepository = value; }
        }

        public string AccessKey
        {
            get { return m_strAccessKey; }
            set { m_strAccessKey = value; }
        }

        public string IPAddress
        {
            get { return m_strIP; }
            set
            {
                m_strIP = value;
                ToByteArray(m_strIP, ref m_bytes);
            }
        }

        private bool ToByteArray(string strIP, ref byte[] arrBytes)
        {
            arrBytes[0] = 0;
            arrBytes[1] = 0;
            arrBytes[2] = 0;
            arrBytes[3] = 0;

            int nIndex1 = strIP.IndexOf('.');
            if (nIndex1 < 0)
                return false;

            int nIndex2 = strIP.IndexOf('.', nIndex1 + 1);
            if (nIndex2 < 0)
                return false;

            int nIndex3 = strIP.IndexOf('.', nIndex2 + 1);
            if (nIndex3 < 0)
                return false;

            try
            {
                int n1 = int.Parse(strIP.Substring(0, nIndex1));
                int n2 = int.Parse(strIP.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1));
                int n3 = int.Parse(strIP.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1));
                int n4 = int.Parse(strIP.Substring(nIndex3 + 1));

                if (n1 < 0 || n1 > 255 || n2 < 0 || n2 > 255 || n3 < 0 || n3 > 255 || n4 < 0 || n4 > 255)
                    return false;

                arrBytes[0] = (byte)n1;
                arrBytes[1] = (byte)n2;
                arrBytes[2] = (byte)n3;
                arrBytes[3] = (byte)n4;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public byte[] IPBytes
        {
            get { return m_bytes; }
            set
            {
                m_bytes[0] = value[0];
                m_bytes[1] = value[1];
                m_bytes[2] = value[2];
                m_bytes[3] = value[3];

                m_strIP = string.Format("{0}.{1}.{2}.{3}", (int)m_bytes[0], (int)m_bytes[1], (int)m_bytes[2], (int)m_bytes[3]);
            }
        }

        public short PortNo
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public LOD LODType
        {
            get { return m_lod; }
            set { m_lod = value; }
        }

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        // 라이브 모드 (0 : Live, 1 : Playback)
        public short PlayBackModeDB
        {
            get { return m_nPlaybackModeDB; }
            set { m_nPlaybackModeDB = value; }
        }

        // 리포지토리와 연동함 (0 : 사용하지 않음, 1: 사용함(사용할 시 위 리포지토리 연결 부분 정의))
        public short UseRepositoryDB
        {
            get { return m_nUseRepositoryDB; }
            set { m_nUseRepositoryDB = value; }
        }

        public string AccessKeyDB
        {
            get { return m_strAccessKeyDB; }
            set { m_strAccessKeyDB = value; }
        }

        public string IPAddressDB
        {
            get { return m_strIPDB; }
            set
            {
                m_strIPDB = value;
                ToByteArray(m_strIPDB, ref m_bytesDB);
            }
        }

        public byte[] IPBytesDB
        {
            get { return m_bytesDB; }
            set
            {
                m_bytesDB[0] = value[0];
                m_bytesDB[1] = value[1];
                m_bytesDB[2] = value[2];
                m_bytesDB[3] = value[3];

                m_strIPDB = string.Format("{0}.{1}.{2}.{3}", (int)m_bytesDB[0], (int)m_bytesDB[1], (int)m_bytesDB[2], (int)m_bytesDB[3]);
            }
        }

        public short PortNoDB
        {
            get { return m_nPortDB; }
            set { m_nPortDB = value; }
        }

        public LOD LODTypeDB
        {
            get { return m_lodDB; }
            set { m_lodDB = value; }
        }
        /// </summary>
        /// 
        public override void UpdateDBData()
        {
            IPBytesDB = IPBytes;
            AccessKeyDB = AccessKey;
            PortNoDB = PortNo;
            PlayBackModeDB = PlayBackMode;
            UseRepositoryDB = UseRepository;
            LODTypeDB = LODType;
        }
    }

    public partial class FireEquipment : Facility
    {
        public enum EquipmentStatus { NORMAL = 0, FAULT, FIXING, ETC, UNKNOWN };

        private string szEquipID = "";
        public string EquipID
        {
            get { return szEquipID; }
            set { szEquipID = value; }
        }
        private Zone m_zone = null;
        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }
        private float mX = 0.0f;
        public float X
        {
            get { return mX; }
            set { mX = value; }
        }
        private float mY = 0.0f;
        public float Y
        {
            get { return mY; }
            set { mY = value; }
        }
        private float mZ = 0.0f;
        public float Z
        {
            get { return mZ; }
            set { mZ = value; }
        }
        private string szDescription = "";
        public string Description
        {
            get { return szDescription; }
            set { szDescription = value; }
        }
        private FacilityType m_type = FacilityType.NONE;
        public override FacilityType Type
        {
            get { return m_type; }
        }

        private string m_strRFID = "";
        public string RFIDTag
        {
            get { return m_strRFID; }
            set { m_strRFID = value; }
        }

        private DateTime m_timeLastChecked = new DateTime();
        public DateTime LastCheckedTime
        {
            get { return m_timeLastChecked; }
            set { m_timeLastChecked = value; }
        }

        private EquipmentStatus m_status = EquipmentStatus.UNKNOWN;
        public EquipmentStatus Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        private string m_strCheckersOpinion = "";
        public string CheckersOpinion
        {
            get { return m_strCheckersOpinion; }
            set { m_strCheckersOpinion = value; }
        }

        public override string IconPath
        {
            get
            {
                return "";
            }
        }
		public override string DisconnectIconPath
		{
			get
			{				
				return "";
			}
		}

        public override int GetLayerID()
        {
            if (m_type == FacilityType.FE)
                return CCTVChecker.ID.ID_LAYER_FIREEXT;
            else if (m_type == FacilityType.HD)
                return CCTVChecker.ID.ID_LAYER_FIREHYD;
            else if (m_type == FacilityType.FA)
                return CCTVChecker.ID.ID_LAYER_ALARMSTA;

            return -1;
        }

        public void SetType(FacilityType type)
        {
            m_type = type;
        }

        public string TypeString
        {
            get
            {
                if (m_type == FacilityType.FE)
                    return "소화기";
                else if (m_type == FacilityType.HD)
                    return "소화전";
                else if (m_type == FacilityType.FA)
                    return "발신기";

                return "Unknown";
            }
        }

        public string StatusString
        {
            get
            {
                if (m_status == EquipmentStatus.NORMAL)
                    return "상태 양호";
                else if (m_status == EquipmentStatus.FAULT)
                    return "설비 불량";
                else if (m_status == EquipmentStatus.FIXING)
                    return "수리중";
                else if (m_status == EquipmentStatus.ETC)
                    return "기타";

                return "상태정보 없음";
            }
        }

		// group id가 -1이 아닌경우만 화면에 표현한다.
		private int m_nGroupID = -1;
		public int GroupID
		{
			get { return m_nGroupID; }
			set { m_nGroupID = value; }
		}
    }

    public interface IPOIPopup
    {
        // xTarget, yTarget : Target POI의 좌표
        void Show(int xTarget, int yTarget);
        void Hide(bool absolutely);
        void MoveTarget(int xTarget, int yTarget);
        bool IsVisible();
        void Close();

        bool LayerVisible
        {
            get;
            set;
        }
    }

    public class POI
    {
        private int m_nID = -1;
        private float x = 0.0f, y = 0.0f, z = 0.0f;
        private Zone m_zone = null;
        private bool m_isIndoor = false;
        private Facility m_facility = null;
        private IPOIPopup m_popup = null;

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        private float xDB = 0.0f, yDB = 0.0f, zDB = 0.0f;
        private Zone m_zoneDB = null;
        /// </summary>

        //private Core.BaseView m_Parent = null;
        //public Core.BaseView ParentView
        //{
        //    get { return m_Parent; }
        //    set { m_Parent = value; }
        //}

        public int ID
        {
            get { return m_nID; }
            set {
				if (value == 1764)
				{
					int i = 0;
					i++;
				}
				m_nID = value; 
			}
        }

        public Facility.FacilityType Type
        {
            get { return m_facility == null ? Facility.FacilityType.NONE : m_facility.Type; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public Facility Facility
        {
            get { return m_facility; }
            set
            {
                if (m_facility != value)
                {
                    if (m_facility != null)
                        m_facility.SetNullPOI();

                    m_facility = value;

                    if (m_facility != null)
                        m_facility.POI = this;
                }
            }
        }

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        public float XDB
        {
            get { return xDB; }
            set { xDB = value; }
        }

        public float YDB
        {
            get { return yDB; }
            set { yDB = value; }
        }

        public float ZDB
        {
            get { return zDB; }
            set { zDB = value; }
        }

        public Zone ZoneDB
        {
            get { return m_zoneDB; }
            set { m_zoneDB = value; }
        }
        /// </summary>

        public void SetNullFacility()
        {
            m_facility = null;
        }

        public bool IsIndoor
        {
            get { return m_isIndoor; }
            set { m_isIndoor = value; }
        }

        public IPOIPopup Popup
        {
            get { return m_popup; }
            set { m_popup = value; }
        }

        public void UpdateDBData()
        {
            xDB = x;
            yDB = y;
            zDB = z;
            m_zoneDB = m_zone;

            if (m_facility != null)
                m_facility.UpdateDBData();
        }
    }

    public class DataTeam
    {
        private int m_nID = -1;
        private string m_szTeamName = "";
        private DataTeam m_teamParent = null;
        private bool m_bExternal = false;
        private ArrayList m_arrChildTeams = new ArrayList();
        private string m_strCompanyName = "";
        private bool m_isCompany = false;

        public bool External
        {
            get { return m_bExternal; }
            set { m_bExternal = value; }
        }
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }
        public DataTeam ParentTeam
        {
            get { return m_teamParent; }
            set
            {
                if (m_teamParent != null)
                    m_teamParent.RemoveChild(this);

                m_teamParent = value;

                if (m_teamParent != null)
                    m_teamParent.AddChild(this);
            }
        }

        public ArrayList ChildTeams
        {
            get { return m_arrChildTeams; }
        }

        public string CompanyName
        {
            get { return m_strCompanyName; }
            set { m_strCompanyName = value; }
        }

        // Team이 아닌 Company인가?
        public bool IsCompany
        {
            get { return m_isCompany; }
            set { m_isCompany = value; }
        }

        protected void RemoveChild(DataTeam team)
        {
            if (team != null)
                m_arrChildTeams.Remove(team);
        }

        protected void AddChild(DataTeam team)
        {
            if (!m_arrChildTeams.Contains(team))
                m_arrChildTeams.Add(team);
        }

        public override string ToString()
        {
            return m_szTeamName;
        }
    }

    public class DataExternalMember
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        private string m_szPhoneNumber = "";
        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }
        bool m_bTeamLeader = false;
        public bool TeamLeader
        {
            get { return m_bTeamLeader; }
            set { m_bTeamLeader = value; }
        }

        private DataTeam m_team = null;
        public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public override string ToString()
        {
            return m_szName;
        }
    }

    public class DataCompanyMember : IComparable
    {
        private int m_nID = -1;
        private string m_strMemberName = "";
        private DataTeam m_team = null;
        private int m_nLevelID = -1;
        private int m_nPositionID = -1;
        private string m_strMemberID = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }

        public bool IsTeamLeader
        {
            get { return m_nPositionID == 2; }
        }

        public int CompareTo(object obj)
        {
            DataCompanyMember member = (DataCompanyMember)obj;

            if (this.IsTeamLeader != member.IsTeamLeader)
                return IsTeamLeader ? -1 : 1;

            if (this.m_nLevelID > member.m_nLevelID)
                return 1;
            else if (this.m_nLevelID < member.m_nLevelID)
                return -1;

            return this.m_strMemberID.CompareTo(member.m_strMemberID);
        }

        public override string ToString()
        {
            return m_strMemberName;
        }
    }

    public class EquipmentZoneObjectList
    {
        private EquipmentZone m_Zone = null;
		public EquipmentZone Zone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }
        private ArrayList m_arSensorList = new ArrayList();
        public System.Collections.ArrayList SensorList
        {
            get { return m_arSensorList; }
            set { m_arSensorList = value; }
        }

        private ArrayList m_arFireEquipmentList = new ArrayList();
        public System.Collections.ArrayList FireEquipmentList
        {
            get { return m_arFireEquipmentList; }
            set { m_arFireEquipmentList = value; }
        }
    }

    public class FacilityManager
    {
        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = -1;
        private Facility.FacilityType m_type = Facility.FacilityType.NONE;
        private int m_nLevelLimit = -1;
        private string m_strDescription = "";
        private object m_tag = null;
        private Building m_building = null;
        private Zone m_zone = null;
        private FacilityManagerGroup m_group = null;
		private EquipmentZone m_equipZone = null;
		
		public EquipmentZone EquipZone
		{
			get { return m_equipZone; }
			set { m_equipZone = value; }
		}

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // 0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam), 4(RegularCompany), 5(ExternalCompany)
        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public Facility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int LevelLimit
        {
            get { return m_nLevelLimit; }
            set { m_nLevelLimit = value; }
        }

        /*// 이 값이 true이면 ~급 및 그 상위직급만 해당
        //         false이면 ~급 및 그 하위직급만 해당
		private bool m_bUpperLimit = true;
		public bool UpperLimit
		{
			get { return m_bUpperLimit; }
			set { m_bUpperLimit = value; }
		}*/
        // 이 값이 0보다 크면 ~급 및 그 상위직급만 해당
        //         0이면 ~급만 해당
        //         0보다 작으면 ~급 및 그 하위직급만 해당
        private int m_nUpperLimit = 0;
        public int UpperLimit
        {
            get { return m_nUpperLimit; }
            set { m_nUpperLimit = value; }
        }
	

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public FacilityManagerGroup Group
        {
            get { return m_group; }
            set { m_group = value; }
        }

        public FacilityManager Clone()
        {
            FacilityManager mgr = new FacilityManager();

            mgr.m_nID = this.m_nID;
            mgr.m_nMemberID = this.m_nMemberID;
            mgr.m_nMemberType = this.m_nMemberType;
            mgr.m_type = this.m_type;
            mgr.m_nLevelLimit = this.m_nLevelLimit;
            mgr.m_strDescription = this.m_strDescription;
			mgr.m_nUpperLimit = this.m_nUpperLimit;
            mgr.m_tag = this.m_tag;

            return mgr;
        }

        public void CopyFrom(FacilityManager mgr)
        {
            this.m_nID = mgr.m_nID;
            this.m_nMemberID = mgr.m_nMemberID;
            this.m_nMemberType = mgr.m_nMemberType;
            this.m_type = mgr.m_type;
            this.m_nLevelLimit = mgr.m_nLevelLimit;
            this.m_strDescription = mgr.m_strDescription;
			this.m_nUpperLimit = mgr.m_nUpperLimit;
            this.m_tag = mgr.m_tag;
        }

        public bool IsSame(FacilityManager mgr)
        {
            if (this.m_nID != mgr.m_nID)
                return false;

            if (this.m_nMemberID != mgr.m_nMemberID)
                return false;

            if (this.m_nMemberType != mgr.m_nMemberType)
                return false;

            if (this.m_type != mgr.m_type)
                return false;

            if (this.m_nLevelLimit != mgr.m_nLevelLimit)
                return false;

            if (this.m_strDescription != mgr.m_strDescription)
                return false;

            if (this.m_tag != mgr.m_tag)
                return false;

			if (this.m_nUpperLimit != mgr.m_nUpperLimit)
				return false;

            return true;
        }
    }

    public class FacilityManagerGroup
    {
        class ArrayListEx : ArrayList
        {
            private FacilityManagerGroup m_group = null;

            public ArrayListEx(FacilityManagerGroup group)
            {
                m_group = group;
            }

            public override int Add(object value)
            {
                /*if (Count >= 1)
                {
                    int a = 3;
                }*/

                if (value.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)value;
                    mgr.Group = m_group;
                }

                return base.Add(value);
            }

            public override void Remove(object obj)
            {
                base.Remove(obj);

                if (m_group != null && obj.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)obj;
                    mgr.Group = null;
                }
            }

            public override void RemoveAt(int index)
            {
                object obj = base[index];

                base.RemoveAt(index);

                if (m_group != null && obj.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)obj;
                    mgr.Group = null;
                }
            }
        }

        private Facility.FacilityType m_type = Facility.FacilityType.NONE;
        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        private Dictionary<DataTeam, int> m_dicRegularTeams = new Dictionary<DataTeam, int>();*/
        private ArrayList m_arrRegularTeams = null;//new ArrayList();
        private ArrayList m_arrCompanyMembers = null;//new ArrayList();
        private ArrayList m_arrExternalTeams = null;//new ArrayList();
        private ArrayList m_arrExternalCompanyMembers = null;//new ArrayList();

        // 특정 건물의 담당자일 경우 m_building이 값을 가진다.
        private Building m_building = null;
        // 특정 외부영역의 담당자일 경우 m_zone이 값을 가진다.
        private Zone m_zone = null;
		// 특정 Equip 존의 담당자일 경우 m_equipZone이 값을 가진다.
		private EquipmentZone m_equipZone = null;
		
        public FacilityManagerGroup()
        {
            m_arrRegularTeams = new ArrayListEx(this);
            m_arrCompanyMembers = new ArrayListEx(this);
            m_arrExternalTeams = new ArrayListEx(this);
            m_arrExternalCompanyMembers = new ArrayListEx(this);
        }

        public Facility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public ArrayList RegularTeams
        {
            get { return m_arrRegularTeams; }
        }

        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        public Dictionary<DataTeam, int> RegularTeams
        {
            get { return m_dicRegularTeams; }
        }*/

        public ArrayList CompanyMembers
        {
            get { return m_arrCompanyMembers; }
        }

        public ArrayList ExternalTeams
        {
            get { return m_arrExternalTeams; }
        }

        public ArrayList ExternalCompanyMembers
        {
            get { return m_arrExternalCompanyMembers; }
        }

        public void CopyFrom(FacilityManagerGroup group)
        {
            m_type = group.m_type;

            m_arrRegularTeams.Clear();
            foreach (FacilityManager mgr in group.m_arrRegularTeams)
            {
                m_arrRegularTeams.Add(mgr.Clone());
            }

            m_arrCompanyMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrCompanyMembers)
            {
                m_arrCompanyMembers.Add(mgr.Clone());
            }

            m_arrExternalTeams.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalTeams)
            {
                m_arrExternalTeams.Add(mgr.Clone());
            }

            m_arrExternalCompanyMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalCompanyMembers)
            {
                m_arrExternalCompanyMembers.Add(mgr.Clone());
            }

            /*m_dicRegularTeams.Clear();
            foreach (KeyValuePair<DataTeam, int> pair in mgr.m_dicRegularTeams)
            {
                m_dicRegularTeams[pair.Key] = pair.Value;
            }

            m_arrCompanyMembers.Clear();
            foreach (DataCompanyMember member in mgr.m_arrCompanyMembers)
            {
                m_arrCompanyMembers.Add(member);
            }

            m_arrExternalTeams.Clear();
            foreach (DataTeam team in mgr.m_arrExternalTeams)
            {
                m_arrExternalTeams.Add(team);
            }

            m_arrExternalCompanyMembers.Clear();
            foreach (DataExternalMember member in mgr.m_arrExternalCompanyMembers)
            {
                m_arrExternalCompanyMembers.Add(member);
            }*/
        }

        protected bool IsSameList(ArrayList arr1, ArrayList arr2)
        {
            if (arr1.Count != arr2.Count)
                return false;

            foreach (FacilityManager mgr in arr1)
            {
                bool find = false;

                foreach (FacilityManager mgr2 in arr2)
                {
                    if (mgr.IsSame(mgr2))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                    return false;
            }

            return true;
        }

        public bool IsSame(FacilityManagerGroup group)
        {
            if (group == null)
                return false;

            if (m_type != group.m_type)
                return false;

            if (!IsSameList(m_arrRegularTeams, group.m_arrRegularTeams))
                return false;

            if (!IsSameList(m_arrCompanyMembers, group.m_arrCompanyMembers))
                return false;

            if (!IsSameList(m_arrExternalTeams, group.m_arrExternalTeams))
                return false;

            if (!IsSameList(m_arrExternalCompanyMembers, group.m_arrExternalCompanyMembers))
                return false;

            /*if (m_dicRegularTeams.Count != mgr.m_dicRegularTeams.Count)
                return false;

            foreach (KeyValuePair<DataTeam, int> pair in mgr.m_dicRegularTeams)
            {
                if (!m_dicRegularTeams.ContainsKey(pair.Key))
                    return false;

                if (m_dicRegularTeams[pair.Key] != pair.Value)
                    return false;
            }

            if (m_arrCompanyMembers.Count != mgr.m_arrCompanyMembers.Count)
                return false;

            foreach (DataCompanyMember member in mgr.m_arrCompanyMembers)
            {
                if (!m_arrCompanyMembers.Contains(member))
                    return false;
            }

            if (m_arrExternalTeams.Count != mgr.m_arrExternalTeams.Count)
                return false;

            foreach (DataTeam team in mgr.m_arrExternalTeams)
            {
                if (!m_arrExternalTeams.Contains(team))
                    return false;
            }

            if (m_arrExternalCompanyMembers.Count != mgr.m_arrExternalCompanyMembers.Count)
                return false;

            foreach (DataExternalMember member in mgr.m_arrExternalCompanyMembers)
            {
                if (!m_arrExternalCompanyMembers.Contains(member))
                    return false;
            }*/

            return true;
        }

        private FacilityManager Contains(ArrayList arrManagers, FacilityManager mgr)
        {
            foreach (FacilityManager manager in arrManagers)
            {
                if (manager.MemberType == mgr.MemberType &&
                    manager.Type == mgr.Type &&
                    manager.Tag == mgr.Tag)
                    return manager;
            }

            return null;
        }

        public void AddManager(FacilityManager mgr)
        {
            if (mgr.MemberType == 0)
            {
                FacilityManager manager = Contains(CompanyMembers, mgr);

                if (manager == null)
                    CompanyMembers.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 1 || mgr.MemberType == 4)
            {
                FacilityManager manager = Contains(RegularTeams, mgr);

                if (manager == null)
                    RegularTeams.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 2)
            {
                FacilityManager manager = Contains(ExternalCompanyMembers, mgr);

                if (manager == null)
                    ExternalCompanyMembers.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 3 || mgr.MemberType == 5)
            {
                FacilityManager manager = Contains(ExternalTeams, mgr);

                if (manager == null)
                    ExternalTeams.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
        }

        public bool IsEmpty()
        {
            if (m_arrRegularTeams.Count > 0)
                return false;

            if (m_arrCompanyMembers.Count > 0)
                return false;

            if (m_arrExternalTeams.Count > 0)
                return false;

            if (m_arrExternalCompanyMembers.Count > 0)
                return false;

            return true;
        }

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

		public EquipmentZone EquipZone
		{
			get { return m_equipZone; }
			set { m_equipZone = value; }
		}
    }

    /*public class Config
    {
        public enum ConfigType { FACILITY_MANGER = 1 };
    }*/
}
