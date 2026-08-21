using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;


namespace SDMSServer
{
    public class AbnormalSensor
    {
        private int m_nSensorID = -1;
        public int SensorID
        {
            get { return m_nSensorID; }
        }
        private DateTime m_nTime;
        public System.DateTime Time
        {
            get { return m_nTime; }
        }
        public AbnormalSensor(int nSensorID)
        {
            m_nTime = DateTime.Now;
            m_nSensorID = nSensorID;
        }
    }

    public class BroadcastMessage
    {
        protected int mID;
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        protected string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        protected bool bUseSiren;
        public bool UseSiren
        {
            get { return bUseSiren; }
            set { bUseSiren = value; }
        }
        protected int mplayOption;
        public int PlayOption
        {
            get { return mplayOption; }
            set { mplayOption = value; }
        }
        protected int mRepeatCount;
        public int RepeatCount
        {
            get { return mRepeatCount; }
            set { mRepeatCount = value; }
        }

        protected DateTime mAddedTime;
        public System.DateTime AddTime
        {
            get { return mAddedTime; }
            set { mAddedTime = value; }
        }
    }

    /*public class BuildingGroup
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

        private string szDisplayText;
        public string DisplayText
        {
            get { return szDisplayText; }
            set { szDisplayText = value; }
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
    }*/

    public class SensorZone : Object
    {
        private UnE.Spatial.EquipmentZone m_Zone = null;

        public UnE.Spatial.EquipmentZone EquipZone
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
        private UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

        public UnE.Sensor.IFacility.FacilityType Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set { m_isConnected = value; }
        }

        private int m_nSensorData = -1;
        public int SensorData
        {
            get { return m_nSensorData; }
            set { m_nSensorData = value; }
        }

        private int m_nLinkedSensorID = -1;
        public int LinkedSensorID
        {
            get { return m_nLinkedSensorID; }
            set { m_nLinkedSensorID = value; }
        }

        private int m_nZoneID = -1;
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public override string ToString()
        {
            string strResult = "";

            if (type == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR || (type >= UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA && type <= UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType))
                strResult = "화재 탐지";
            else if (type == UnE.Sensor.IFacility.FacilityType.COOLER_SENSOR)
                strResult = "소화 센서";
            else if (type == UnE.Sensor.IFacility.FacilityType.PRESSURE_SENSOR)
                strResult = "압력 센서";
            else if (type == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                strResult = "유해화학물질 누출감지 센서";

            return strResult;
        }
    }

    public class SensorReactionLog
    {
        /*public enum ReactionType
        {
            BEGIN_STATUS = 0,
            RUN_BROADCAST = 10,
            SEND_SMS = 11,
            
            MALFUNCTION = 21,
            NOTIFY_FIRE = 22,
            IGNORE_FIRE = 23,
            TRAINNING_FIRE = 24,
                                    
            RUN_SOP = 30,
            RUN_N_CANCEL_SOP = 31,
            FINISH_SOP = 32,
            IGNORE_SOP = 33,
            END_STATUS = 50,
            
            BEGIN_PSM_STATUS = 60,
            IGNORE_PSM_DETECT = 61,
            CHANGE_PSM_ALARM_DEPTH = 62,
            NOTIFY_PSM = 63,
            PSM_USER_RESET = 64,
            END_PSM_STATUS = 70,
            ETC = 100,


            NOTIFY_SECURITY = 898,
            BEGIN_S1SVMS_STATUS = 899,

            //NOTIFY_Intrusion_S1 = 900,  // SVMS 침입           
           // NOTIFY_Loiter_S1 = 901,     // SVMS 배회
            //NOTIFY_Slip_S1 = 902,   // SVMS 쓰러짐
           // NOTIFY_Steal_S1 = 903,       // SVMS 도난
           // NOTIFY_Abandoned_S1 = 904,           // SVMS 방치
          //  NOTIFY_VirtualFence_S1 = 905,      // SVMS 가상펜스
          //  NOTIFY_Fire_S1 = 906,              // SVMS 화재
          //  NOTIFY_EmergencyBell_S1 = 907,     // SVMS 비상벨
            IGNORE_S1SVMS_STATUS = 919,
            END_S1SVMS_STATUS = 920,

            BEGIN_S1ACCESS_STATUS = 921,
            IGNORE_S1ACCESS_STATUS = 939,
            END_S1ACCESS_STATUS = 940


            
        }*/

        public enum DetectionStatus
        {
            REAL = 1,
            MALFUNCTION = 2,
            TEST = 3
        }

        private int m_nID = -1;
        private int m_nSensorHistoryID = -1;
        private libSensorProcess.ReactionType m_type = libSensorProcess.ReactionType.ETC;
        private DateTime m_time = new DateTime();
        private string m_strMessage = "";
        private string m_strParam1 = "";
        private string m_strParam2 = "";
        private string m_strParam3 = "";
        private string m_strParam4 = "";
        private string m_strParam5 = "";
        private string m_strDescriptionText = "";
        private DetectionStatus m_detectionStatus = DetectionStatus.TEST;

        private string m_strImagePath = "";

        private static Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set {
                
                m_nSensorHistoryID = value;
                TimeHistory history = ServiceProvider.Instance.FindTimeHistory(m_nSensorHistoryID);
                if( history != null)
                {
                    if( history.LastReactionLog != null)
                    {
                        m_detectionStatus = history.LastReactionLog.Status;
                    }                   
                }
            }
        }

        public libSensorProcess.ReactionType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public DateTime LogTime
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Param1
        {
            get { return m_strParam1; }
            set { m_strParam1 = value; }
        }

        public string Param2
        {
            get { return m_strParam2; }
            set { m_strParam2 = value; }
        }

        public string Param3
        {
            get { return m_strParam3; }
            set { m_strParam3 = value; }
        }

        public string Param4
        {
            get { return m_strParam4; }
            set { m_strParam4 = value; }
        }

        public string Param5
        {
            get { return m_strParam5; }
            set { m_strParam5 = value; }
        }

        public string DescriptionText
        {
            get { return m_strDescriptionText; }
            set { m_strDescriptionText = value; }
        }

        public DetectionStatus Status
        {
            get { return m_detectionStatus; }
            set { m_detectionStatus = value; }
        }

        // 로그와 연관된 이미지 파일의 경로
        public string ImagePath
        {
            get { return m_strImagePath; }
            set { m_strImagePath = value; }
        }

        public int GetBytesCount()
        {
            int nBytesCount = sizeof(int) * 3;  // ID, SensorHistoryID, Type
            nBytesCount += sizeof(long);        // LogTime
            // Message, Param1, Param2
            nBytesCount += (m_strMessage.Length + m_strParam1.Length + m_strParam2.Length) * sizeof(char);
            nBytesCount += (m_strParam3.Length + m_strParam4.Length + m_strParam5.Length) * sizeof(char);
            // FieldCount : 7
            return nBytesCount + 5 * 7 + 2;
        }

        public byte[] MakeBytes()
        {

            byte[] idBytes = TcpLib2.TcpHelper.MakeBytes(m_nID);
            byte[] sensorHistoryIDBytes = TcpLib2.TcpHelper.MakeBytes(m_nSensorHistoryID);
            byte[] typeBytes = TcpLib2.TcpHelper.MakeBytes((int)m_type);
            byte[] timeBytes = TcpLib2.TcpHelper.MakeBytes(m_time.ToBinary());
            byte[] messageBytes = TcpLib2.TcpHelper.MakeBytes(m_strMessage);
            byte[] param1Bytes = TcpLib2.TcpHelper.MakeBytes(m_strParam1);
            byte[] param2Bytes = TcpLib2.TcpHelper.MakeBytes(m_strParam2);
            byte[] param3Bytes = TcpLib2.TcpHelper.MakeBytes(m_strParam3);
            byte[] param4Bytes = TcpLib2.TcpHelper.MakeBytes(m_strParam4);
            byte[] param5Bytes = TcpLib2.TcpHelper.MakeBytes(m_strParam5);

            int nBytesCount = idBytes.Length +
                sensorHistoryIDBytes.Length +
                typeBytes.Length +
                timeBytes.Length +
                messageBytes.Length +
                param1Bytes.Length +
                param2Bytes.Length +
                param3Bytes.Length +
                param4Bytes.Length +
                param5Bytes.Length + 6;
            byte[] bytes = new byte[nBytesCount];


            int nIndex = 6;

            bytes[0] = TCP_ID.SENSOR_REACTION_HISTORY_DATA;
            bytes[1] = 0;

            int nChunkCount = 10;
            byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
            System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, idBytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, sensorHistoryIDBytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, typeBytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, timeBytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, messageBytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, param1Bytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, param2Bytes);

            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, param3Bytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, param4Bytes);
            TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, param5Bytes);

            return bytes;
        }

        private static object lockObj = new object();
        public static libSensorProcess.ReactionType ToReactionType(int nType, out bool isSuccess)
        {
            isSuccess = true;

            lock (lockObj)
            {
                if (m_dicReactionType == null)
                {
                    m_dicReactionType = new Dictionary<int, libSensorProcess.ReactionType>();

                    foreach (libSensorProcess.ReactionType type in Enum.GetValues(typeof(libSensorProcess.ReactionType)))
                    {
                        m_dicReactionType[(int)type] = type;
                    }
                }
            }

            libSensorProcess.ReactionType rType;
            if (m_dicReactionType.TryGetValue(nType, out rType))
                return rType;

            isSuccess = false;
            return libSensorProcess.ReactionType.ETC;
        }

        public SensorReactionLog Clone()
        {
            SensorReactionLog log = new SensorReactionLog();

            log.m_nID = m_nID;
            log.m_nSensorHistoryID = m_nSensorHistoryID;
            log.m_type = m_type;
            log.m_time = m_time;
            log.m_strMessage = m_strMessage;
            log.m_strParam1 = m_strParam1;
            log.m_strParam2 = m_strParam2;
            log.m_strParam3 = m_strParam3;
            log.m_strParam4 = m_strParam4;
            log.m_strParam5 = m_strParam5;
            log.Status = m_detectionStatus;
            return log;
        }
    }


    /*public class DataTeam
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
    }*/

    // 당직자를 위한 클래스
    /*public class DataTeamDuty : DataTeam
    {
        public DataTeamDuty()
        {
            TeamName = "당직자";
            ID = 0;
        }
    }*/

    // 교대 근무자(근무표)를 위한 클래스
    /*public class DataTeamControlRoom : DataTeam
    {
        public DataTeamControlRoom()
        {
            TeamName = "교대 근무자";
            ID = 0;
        }

        // nRoomType은 제일 하위 1바이트
        // nControlRoomID는 그 바로 위 1바이트
        // nControlTeamJobPosition은 상위 2바이트를 사용한다.
        // 따라서, nRoomType과 nControlRoomID는 0에서 255 사이의 값만 사용할 수 있다.
        public static int MakeID(int nRoomTypeID, int nControlRoomID, int nControlTeamJobPositionID)
        {
            int nID = (nControlTeamJobPositionID << 16) | (nControlRoomID << 8) | nRoomTypeID;
            return nID;
        }

        public static void GetParams(int nID, out int nRoomTypeID, out int nControlRoomID, out int nControlTeamJobPositionID)
        {
            nRoomTypeID = nID & 0xff;
            nControlRoomID = (nID & 0xff00) >> 8;
            nControlTeamJobPositionID = nID >> 16;
        }

        public int RoomTypeID
        {
            get { return (ID & 0xff); }
        }

        public int ControlRoomID
        {
            get { return ((ID & 0xff00) >> 8); }
        }

        public int ControlTeamJobPositionID
        {
            get { return (ID >> 16); }
        }
    }*/

    public class DataMember
    {
        public virtual int ObjectType
        {
            get { return 0; }
        }

        protected int m_nID = -1;
        public virtual int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        protected string m_szPhoneNumber = "";
        public virtual string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }

        protected bool m_bTeamLeader = false;
        public virtual bool TeamLeader
        {
            get { return m_bTeamLeader; }
            set { m_bTeamLeader = value; }
        }

        /*protected DataTeam m_team = null;
        public virtual DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }*/
    }

    public class DataExternalMember : DataMember
    {
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        private string m_strJobLevel = null;
        private string m_strJobPosition = null;

        public string JobLevel
        {
            get { return m_strJobLevel; }
            set { m_strJobLevel = value; }
        }

        public string JobPosition
        {
            get { return m_strJobPosition; }
            set { m_strJobPosition = value; }
        }

        // Key : 소속팀
        // Value : 해당팀에서 팀장인가?
        /*private Dictionary<DataTeam, bool> m_dicTeamLeaders = new Dictionary<DataTeam, bool>();

        public Dictionary<DataTeam, bool> TeamLeaders
        {
            get { return m_dicTeamLeaders; }
        }

        public DataTeam GetFirstTeam()
        {
            if (m_dicTeamLeaders.Count == 0)
                return null;

            return m_dicTeamLeaders.ElementAt(0).Key;
        }*/

        public override string ToString()
        {
            return m_szName;
        }

        public override int ObjectType
        {
            get { return 2; }
        }
    }

    public class DataCompanyMember : DataMember, IComparable
    {
        private string m_strMemberName = "";

        private int m_nLevelID = -1;
        //private int m_nPositionID = -1;
        private string m_strMemberID = "";

        private string m_strOfficePhoneNumber = "";

        // 한 사람이 여러 팀에 속해있을수 있고, 직위(팀내 역할)는 속해있는 팀마다 다를수 있다.
        private Dictionary<DataTeam, int> m_dicTeamPositions = new Dictionary<DataTeam, int>();

        public override int ObjectType
        {
            get { return 1; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        /*public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }*/

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }

        /*public bool IsTeamLeader
        {
            get { return m_nPositionID == 2; }
        }*/
        public override bool TeamLeader
        {
            get
            {
                foreach (KeyValuePair<DataTeam, int> pair in m_dicTeamPositions)
                {
                    if (pair.Value == 2)
                        return true;
                }

                return false;
            }

            set
            {
            }
        }

        // 한 사람이 여러 팀에 속해있을수 있고, 직위(팀내 역할)는 속해있는 팀마다 다를수 있다.
        public Dictionary<DataTeam, int> TeamPositions
        {
            get { return m_dicTeamPositions; }
        }

        public bool IsTeamLeader(DataTeam team)
        {
            int nPosition;

            if (m_dicTeamPositions.TryGetValue(team, out nPosition))
            {
                return nPosition == 2;
            }

            return false;
        }

        public int CompareTo(object obj)
        {
            DataCompanyMember member = (DataCompanyMember)obj;
            bool isTeamLeader = this.TeamLeader;

            if (isTeamLeader != member.TeamLeader)
                return isTeamLeader ? -1 : 1;

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

    public class FacilityManager
    {
        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = -1;
        private UnE.Sensor.IFacility.FacilityType m_type = UnE.Sensor.IFacility.FacilityType.NONE;
        private int m_nLevelLimit = -1;
        // 이 값이 0보다 크면 ~급 및 그 상위직급만 해당
        //         0이면 ~급만 해당
        //         0보다 작으면 ~급 및 그 하위직급만 해당
        private int m_nUpperLimit = 0;
        private string m_strDescription = "";
        private object m_tag = null;
        private UnE.Spatial.Building m_building = null;
        private UnE.Spatial.Zone m_zone = null;
        private FacilityManagerGroup m_group = null;

        // FacilityType별 DB Table 이름
        private static Dictionary<UnE.Sensor.IFacility.FacilityType, string> m_dicFacilityTypeTable = new Dictionary<UnE.Sensor.IFacility.FacilityType, string>();

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

        // 0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam), 4(RegularCompany), 5(ExternalCompany), 6(당직자)
        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public UnE.Sensor.IFacility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int LevelLimit
        {
            get { return m_nLevelLimit; }
            set { m_nLevelLimit = value; }
        }

        // 이 값이 0보다 크면 ~급 및 그 상위직급만 해당
        //         0이면 ~급만 해당
        //         0보다 작으면 ~급 및 그 하위직급만 해당
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

        public UnE.Spatial.Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public UnE.Spatial.Zone Zone
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

            return true;
        }

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(int nFacilityType, string strTableName)
        {
            UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(nFacilityType);

            if (type == UnE.Sensor.IFacility.FacilityType.NONE)
                return;

            SetFacilityTypeTable(type, strTableName);
        }

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(UnE.Sensor.IFacility.FacilityType type, string strTableName)
        {
            m_dicFacilityTypeTable[type] = strTableName;
        }

        public static string GetFacilityTypeTable(UnE.Sensor.IFacility.FacilityType type)
        {
            string strTableName;

            if (m_dicFacilityTypeTable.TryGetValue(type, out strTableName))
                return strTableName;

            return "";
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

        private UnE.Sensor.IFacility.FacilityType m_type = UnE.Sensor.IFacility.FacilityType.NONE;
        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        private Dictionary<DataTeam, int> m_dicRegularTeams = new Dictionary<DataTeam, int>();*/
        private ArrayList m_arrRegularTeams = null;//new ArrayList();
        private ArrayList m_arrCompanyMembers = null;//new ArrayList();
        private ArrayList m_arrExternalTeams = null;//new ArrayList();
        private ArrayList m_arrExternalCompanyMembers = null;//new ArrayList();
        // 교대 근무자
        private ArrayList m_arrControlRoomMembers = null;

        // 특정 건물의 담당자일 경우 m_building이 값을 가진다.
        private UnE.Spatial.Building m_building = null;

        // 특정 외부영역의 담당자일 경우 m_zone이 값을 가진다.
        private UnE.Spatial.Zone m_zone = null;

        // 특정 Equip 존의 담당자일 경우 m_equipZone이 값을 가진다.
        private UnE.Spatial.EquipmentZone m_equipZone = null;

        public FacilityManagerGroup()
        {
            m_arrRegularTeams = new ArrayListEx(this);
            m_arrCompanyMembers = new ArrayListEx(this);
            m_arrExternalTeams = new ArrayListEx(this);
            m_arrExternalCompanyMembers = new ArrayListEx(this);
            m_arrControlRoomMembers = new ArrayListEx(this);
        }

        public UnE.Sensor.IFacility.FacilityType Type
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

        public ArrayList ControlRoomMembers
        {
            get { return m_arrControlRoomMembers; }
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

            m_arrControlRoomMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrControlRoomMembers)
            {
                m_arrControlRoomMembers.Add(mgr.Clone());
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

            if (!IsSameList(m_arrControlRoomMembers, group.m_arrControlRoomMembers))
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
            else if (mgr.MemberType == 7)
            {
                FacilityManager manager = Contains(ControlRoomMembers, mgr);

                if (manager == null)
                    ControlRoomMembers.Add(mgr);
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

            if (m_arrControlRoomMembers.Count > 0)
                return false;

            return true;
        }

        public UnE.Spatial.Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public UnE.Spatial.Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public UnE.Spatial.EquipmentZone EquipZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }
    }

    /*public abstract class IFacility
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
            FireSensor_MonitoringType = 110,     // 감시센서

            // 서울대학교 e재난 시스템 - S1시스템 통합으로 추가됨
            // skkim     2017-03-14
            SECURITY_SENSOR=899,
            Intrusion_S1 = 900,  // SVMS 침입
            Loiter_S1 = 901,     // SVMS 배회
            Slip_S1 = 902,   // SVMS 쓰러짐
            Steal_S1 = 903,       // SVMS 도난
            Abandoned_S1 = 904,           // SVMS 방치
            VirtualFence_S1 = 905,      // SVMS 가상펜스
            Fire_S1 = 906,              // SVMS 화재
            EmergencyBell_S1 = 907,     // SVMS 비상벨

            GeneralIntrusionT1_S1 = 1001,  // S1Access 일반침입1
            GeneralIntrusionT2_S1 = 1002,   // S1Access 일반 침입2
            InternalIntrusionT3_S1 = 1003,// S1Access 내부침입
            VaultIntrusionT4_S1 = 1004,   // S1Access 금고침입
            FireF1_S1 = 2000,             // S1Access 화재
            CustomerEmergencyC1_S1 = 2100, // S1Access 고객비상
            CustomerEmergencyC2_S1 = 2110,// S1Access 고객 비상
            RescueQQ_S1 = 2200,           // S1Access 구급
            GasG1_S1 = 2300,               // S1Access 가스
            BlackoutAbnormalityU1_S1 = 3000, // S1Access 정전이상
            LeakAbnormalityU4_S1 = 3004,     // S1Access 누수이상
            SynthesisAlertAbnormalityU8_S1 = 3008, // S1Access 종합경보반 이상
            ExternalAlarmBell = 4000        // 외부 비상벨
        };

        // FacilityType별 DB Table 이름
        private static Dictionary<FacilityType, string> m_dicFacilityTypeTable = new Dictionary<FacilityType, string>();
        private static Dictionary<int, FacilityType> m_dicFacilityType = null;
        private static object m_lockObj = new object();

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
            // m_dicFacilityType 초기화 부분은 이중초기화 될수 있으므로 반드시 lock이 걸려야 함.
            // 별도 초기화를 하는것이 좋을듯 하니, 추후 수정 바람. edit by skkim 2016-08-01
            lock (m_lockObj)
            {
                if (m_dicFacilityType == null)
                {
                    m_dicFacilityType = new Dictionary<int, FacilityType>();

                    foreach (FacilityType type in Enum.GetValues(typeof(FacilityType)))
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

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(int nFacilityType, string strTableName)
        {
            FacilityType type = ToFacilityType(nFacilityType);

            if (type == FacilityType.NONE)
                return;

            SetFacilityTypeTable(type, strTableName);
        }

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(FacilityType type, string strTableName)
        {
            m_dicFacilityTypeTable[type] = strTableName;
        }

        public static string GetFacilityTypeTable(FacilityType type)
        {
            string strTableName;

            if (m_dicFacilityTypeTable.TryGetValue(type, out strTableName))
                return strTableName;

            return "";
        }

        public static int ToIntType(FacilityType type)
        {
            return (int)type;
        }
    }

    // Pump Pressuer Sensor
    public abstract class ISensor : IFacility
    {
        public int m_ZoneID = -1;
        public int ZoneID
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
    }

    public partial class PumpPressureSensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.PRESSURE_SENSOR; }
        }
    }

    // Sping Cooler
    public partial class SpringCooler : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.COOLER_SENSOR; }
        }
    }

    // Fire Detector
    public partial class FireSensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.FIRE_SENSOR; }
        }
    }


    public partial class CCTV : IFacility
    {
        private string m_strIP = "0.0.0.0";
        private short m_nPort = 9403;
        private string m_strAccessKey = "BNC-3220HR-W";
        private short m_nPlaybackMode = 0;
        private short m_nUseRepository = 0;
        private byte[] m_bytes = new byte[4] { 0, 0, 0, 0 };

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        private string m_strIPDB = "0.0.0.0";
        private short m_nPortDB = 9403;
        private string m_strAccessKeyDB = "BNC-3220HR-W";
        private short m_nPlaybackModeDB = 0;
        private short m_nUseRepositoryDB = 0;
        private byte[] m_bytesDB = new byte[4] { 0, 0, 0, 0 };
        /// </summary>

        public override IFacility.FacilityType Type
        {
            get { return FacilityType.CCTV; }
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
    }

    public partial class FireEquipment : IFacility
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
    }

    public class PSMTank
    {
        private int m_nTankID = -1;
        private string m_strTankName = "";
        private EquipmentZone m_equipZone = null;
        private string m_strBoundaries = null;
        private PSMMaterial m_material = null;
        private float m_fRemains = -1.0f;
        private float m_fCapacity = -1.0f;
        private string m_strUnitName = "";
        private string m_strBroadcastName = "";
        private string m_strLocationName = "";
        // 초기 이격거리(미터)
        private int m_nEvacInitDistance = -1;
        // 주간 방호 대피거리(미터)
        private int m_nEvacDayDistance = -1;
        // 야간 방호 대피거리(미터)
        private int m_nEvacNightDistance = -1;
        private List<PSMSensor> m_sensorList = new List<PSMSensor>();

        public int ID
        {
            get { return m_nTankID; }
            set { m_nTankID = value; }
        }

        public string Name
        {
            get { return m_strTankName; }
            set { m_strTankName = value; }
        }

        public EquipmentZone EquipZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        public string Boundaries
        {
            get { return m_strBoundaries; }
            set { m_strBoundaries = value; }
        }

        public PSMMaterial Material
        {
            get { return m_material; }
            set { m_material = value; }
        }

        // 잔량
        public float Remains
        {
            get { return m_fRemains; }
            set { m_fRemains = value; }
        }

        // 최대 용량
        public float Capacity
        {
            get { return m_fCapacity; }
            set { m_fCapacity = value; }
        }

        // 용량의 단위
        public string UnitName
        {
            get { return m_strUnitName; }
            set { m_strUnitName = value; }
        }

        public string BroadcastName
        {
            get { return m_strBroadcastName; }
            set { m_strBroadcastName = value; }
        }

        public string LocationName
        {
            get { return m_strLocationName; }
            set { m_strLocationName = value; }
        }

        // 초기 이격거리(미터)
        public int EvacInitDistance
        {
            get { return m_nEvacInitDistance; }
            set { m_nEvacInitDistance = value; }
        }

        // 주간 방호 대피거리(미터)
        public int EvacDayDistance
        {
            get { return m_nEvacDayDistance; }
            set { m_nEvacDayDistance = value; }
        }

        // 야간 방호 대피거리(미터)
        public int EvacNightDistance
        {
            get { return m_nEvacNightDistance; }
            set { m_nEvacNightDistance = value; }
        }

        public List<PSMSensor> LinkedSensorList
        {
            get { return m_sensorList; }
        }

        public void AddSensor(PSMSensor sensor)
        {
            if (sensor == null)
                return;

            if (!m_sensorList.Contains(sensor))
            {
                m_sensorList.Add(sensor);
                sensor.AddTank(this);
            }
        }

        public void RemoveSensor(PSMSensor sensor)
        {
            if (sensor == null)
                return;

            if (m_sensorList.Contains(sensor))
            {
                m_sensorList.Remove(sensor);
                sensor.RemoveTank(this);
            }
        }
    }

    public class PSMMaterial
    {
        private int m_nID = -1;
        private string m_strName = "";
        private string m_strUOM = "";
        // [유해화학물질 특성] 매뉴얼의 Page 번호
        private int m_nManualPageNo = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string UOM
        {
            get { return m_strUOM; }
            set { m_strUOM = value; }
        }

        // [유해화학물질 특성] 매뉴얼의 Page 번호
        public int PageNo
        {
            get { return m_nManualPageNo; }
            set { m_nManualPageNo = value; }
        }
    }

    public class PSMSensor : IFacility
    {
        public enum Status { Unknown = -1, On, Off, Off4Work };

        private string m_strSensorName = "";
        private DBUtility.VariousData<System.Drawing.PointF> m_position = null;
        private float m_fCurrentSensorData = 0.0f;
        // 1단계 알람 한계치
        private float m_fLimitLevel1 = 0.0f;
        // 2단계 알람 한계치
        private float m_fLimitLevel2 = 0.0f;
        // 3단계 알람 한계치
        private float m_fLimitLevel3 = 0.0f;
        // 1단계 알람을 발생시킬 센서 데이터 하한치 초기값
        private float m_fDefLimitLevel1 = -1.0f;
        // 2단계 알람을 발생시킬 센서 데이터 하한치 초기값
        private float m_fDefLimitLevel2 = -1.0f;
        // 3단계 알람을 발생시킬 센서 데이터 하한치 초기값
        private float m_fDefLimitLevel3 = -1.0f;
        private List<PSMTank> m_linkedTanks = new List<PSMTank>();
        private Status m_status = Status.Unknown;
        private DBUtility.VariousData<DateTime> m_beginWorkTime = null;
        private DBUtility.VariousData<DateTime> m_endWorkTime = null;
        private EquipmentZone m_equipZone = null;
        // 1단계 알람신호에 대하여 알람 처리를 할 것인가?
        private bool m_allowReceiveLevel1Alarm = true;
        // 2단계 알람신호에 대하여 알람 처리를 할 것인가?
        private bool m_allowReceiveLevel2Alarm = true;
        // 3단계 알람신호에 대하여 알람 처리를 할 것인가?
        private bool m_allowReceiveLevel3Alarm = true;

        override public FacilityType Type
        {
            get { return FacilityType.PSM_SENSOR; }
        }

        public string Name
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public DBUtility.VariousData<System.Drawing.PointF> Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public float CurrentData
        {
            get { return m_fCurrentSensorData; }
            set { m_fCurrentSensorData = value; }
        }

        // 1단계 알람을 발생시킬 센서 데이터 하한치
        public float LimitLevel1
        {
            get { return m_fLimitLevel1; }
            set { m_fLimitLevel1 = value; }
        }

        // 2단계 알람을 발생시킬 센서 데이터 하한치
        public float LimitLevel2
        {
            get { return m_fLimitLevel2; }
            set { m_fLimitLevel2 = value; }
        }

        // 3단계 알람을 발생시킬 센서 데이터 하한치
        public float LimitLevel3
        {
            get { return m_fLimitLevel3; }
            set { m_fLimitLevel3 = value; }
        }

        // 1단계 알람을 발생시킬 센서 데이터 하한치 초기값
        public float DefLimitLevel1
        {
            get { return m_fDefLimitLevel1; }
            set { m_fDefLimitLevel1 = value; }
        }

        // 2단계 알람을 발생시킬 센서 데이터 하한치 초기값
        public float DefLimitLevel2
        {
            get { return m_fDefLimitLevel2; }
            set { m_fDefLimitLevel2 = value; }
        }

        // 3단계 알람을 발생시킬 센서 데이터 하한치 초기값
        public float DefLimitLevel3
        {
            get { return m_fDefLimitLevel3; }
            set { m_fDefLimitLevel3 = value; }
        }

        public List<PSMTank> LinkedTankList
        {
            get { return m_linkedTanks; }
        }

        public Status SensorStatus
        {
            get { return m_status; }
            set
            {
                if (m_status != value)
                {
                    bool spreadValue = m_status == Status.Off4Work || value == Status.Off4Work;
                    m_status = value;

                    // 특정 센서에 대하여 작업중 표시를 한다는 것은 해당 센서가 감시하는 탱크가 작업중이라는 의미이므로
                    // 같은 탱크를 감시하는 모든 센서들의 상태를 같이 변화시킨다.
                    if (spreadValue)
                    {
                        List<PSMSensor> sensors = GetSameSensors();

                        foreach (PSMSensor sensor in sensors)
                        {
                            sensor.m_status = value;
                        }

                        sensors.Clear();
                    }
                }
            }
        }

        public DBUtility.VariousData<DateTime> BeginWorkTime
        {
            get { return m_beginWorkTime; }
            set
            {
                if (!CheckSameTime(m_beginWorkTime, value))
                {
                    m_beginWorkTime = value;

                    // 같은 탱크들을 감시하는 센서들의 작업 시작시간을 동일하게 맞춘다.
                    List<PSMSensor> sensors = GetSameSensors();

                    foreach (PSMSensor sensor in sensors)
                    {
                        sensor.m_beginWorkTime = value;
                    }

                    sensors.Clear();
                }
            }
        }

        public DBUtility.VariousData<DateTime> EndWorkTime
        {
            get { return m_endWorkTime; }
            set
            {
                if (!CheckSameTime(m_endWorkTime, value))
                {
                    m_endWorkTime = value;

                    // 같은 탱크들을 감시하는 센서들의 작업 종료시간을 동일하게 맞춘다.
                    List<PSMSensor> sensors = GetSameSensors();

                    foreach (PSMSensor sensor in sensors)
                    {
                        sensor.m_endWorkTime = value;
                    }

                    sensors.Clear();
                }
            }
        }

        public EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        // 1단계 알람신호에 대하여 알람 처리를 할 것인가?
        public bool AllowReceiveLevel1Alarm
        {
            get { return m_allowReceiveLevel1Alarm; }
            set { m_allowReceiveLevel1Alarm = value; }
        }

        // 2단계 알람신호에 대하여 알람 처리를 할 것인가?
        public bool AllowReceiveLevel2Alarm
        {
            get { return m_allowReceiveLevel2Alarm; }
            set { m_allowReceiveLevel2Alarm = value; }
        }

        // 3단계 알람신호에 대하여 알람 처리를 할 것인가?
        public bool AllowReceiveLevel3Alarm
        {
            get { return m_allowReceiveLevel3Alarm; }
            set { m_allowReceiveLevel3Alarm = value; }
        }

        private bool CheckSameTime(DBUtility.VariousData<DateTime> time1, DBUtility.VariousData<DateTime> time2)
        {
            if (time1 == null && time2 == null)
                return true;
            else if (time1 == null || time2 == null)
                return false;

            return time1.Data == time2.Data;
        }

        public void AddTank(PSMTank tank)
        {
            if (tank == null)
                return;

            if (!m_linkedTanks.Contains(tank))
            {
                m_linkedTanks.Add(tank);
                tank.AddSensor(this);
            }
        }

        public void RemoveTank(PSMTank tank)
        {
            if (tank == null)
                return;

            if (m_linkedTanks.Contains(tank))
            {
                m_linkedTanks.Remove(tank);
                tank.RemoveSensor(this);
            }
        }

        // 같은 탱크들을 감시하는 Sensor들을 얻어온다.
        public List<PSMSensor> GetSameSensors()
        {
            List<PSMSensor> sensors = new List<PSMSensor>();

            foreach (PSMTank tank in m_linkedTanks)
            {
                foreach (PSMSensor sensor in tank.LinkedSensorList)
                {
                    if (sensor == this)
                        continue;

                    if (!sensors.Contains(sensor))
                        sensors.Add(sensor);
                }
            }

            return sensors;
        }

        public PSMMaterial GetLinkedMaterial()
        {
            foreach (PSMTank tank in m_linkedTanks)
            {
                return tank.Material;
            }

            return null;
        }

        public string GetLinkedLocationName()
        {
            foreach (PSMTank tank in m_linkedTanks)
            {
                return tank.LocationName;
            }

            return "";
        }

        // 방송용 위치이름
        public string GetLinkedBroadcastName()
        {
            foreach (PSMTank tank in m_linkedTanks)
            {
                return tank.BroadcastName;
            }

            return "";
        }

        // 센서가 사용중지 상태인가?
        public bool IsOff(DateTime time)
        {
            if (this.SensorStatus == Status.Off)
                return true;

            if (this.SensorStatus == Status.Off4Work)
            {
                if (m_beginWorkTime != null && m_endWorkTime != null)
                {
                    if (time >= m_beginWorkTime.Data && time <= m_endWorkTime.Data)
                        return true;
                }
            }

            return false;
        }

        // 센서가 사용중지 상태인가?
        public bool IsOff()
        {
            return IsOff(DateTime.Now);
        }

        public static Status ToStatus(int nStatus)
        {
            if (nStatus < (int)Status.On || nStatus > (int)Status.Off4Work)
                return Status.Unknown;

            return (Status)nStatus;
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
        private string m_strBroadcastName = "";
        private string m_strDisplayText = "";

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

        public string BroadcastName
        {
            get { return m_strBroadcastName; }
            set { m_strBroadcastName = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public override string ToString()
        {
            return m_strName;
        }
    }*/

    public class SOPGenUser
    {
        private int m_nID;
        private int m_nMemberID;
        private string m_strUserName;
        private int m_nUserLevel;
        private int m_nTeamID;
        private string m_strPassword;
        private string m_strUserID;

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

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

    }

    // 같은 EquipZone을 공유하며, Type이 같은 Sensor들의 집합
    public class SensorZoneGroup
    {
        private long m_nID = -1;
        private UnE.Spatial.EquipmentZone m_equipZone = null;
        private UnE.Sensor.IFacility.FacilityType m_sensorType = UnE.Sensor.IFacility.FacilityType.NONE;
        // Value : SensorZone별 데이터
        private Dictionary<SensorZone, object> m_dicSensorDatas = new Dictionary<SensorZone, object>();

        // EquipZone ID와 SensorType의 조합
        // 상위 4바이트 : EquipZone ID
        // 하위 4바이트 : SensorType
        public long ID
        {
            get { return m_nID; }
        }

        public UnE.Spatial.EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set
            {
                m_equipZone = value;
                SetID();
            }
        }

        public UnE.Sensor.IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set
            {
                m_sensorType = value;
                SetID();
            }
        }

        public Dictionary<SensorZone, object> SensorDatas
        {
            get { return m_dicSensorDatas; }
        }

        private void SetID()
        {
            long hi = m_equipZone == null ? -1 : m_equipZone.ID;
            long low = ((long)m_sensorType) & 0xffffffff;

            m_nID = (hi << 32) | low;
        }

        public static long ToID(UnE.Spatial.EquipmentZone equipZone, UnE.Sensor.IFacility.FacilityType sensorType)
        {
            int nEquipZoneID = equipZone == null ? -1 : equipZone.ID;
            return ToID(nEquipZoneID, sensorType);
        }

        public static long ToID(int nEquipZoneID, UnE.Sensor.IFacility.FacilityType sensorType)
        {
            long hi = nEquipZoneID;
            long low = (long)sensorType;

            long nID = (hi << 32) | low;
            return nID;
        }

        public static void GetIDInfo(long nID, out int nEquipZoneID, out UnE.Sensor.IFacility.FacilityType sensorType)
        {
            nEquipZoneID = (int)(nID >> 32);
            sensorType = UnE.Sensor.IFacility.ToFacilityType((int)(nID & 0xffffffff));
        }
    }
}
