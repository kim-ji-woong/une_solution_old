using System;
using System.Collections.Generic;
using System.Collections;
using DBUtility2;

namespace UnE.Spatial
{
   

    public class BuildingGroup : IComparable
    {
        private float m_TextCenterY = 0.0f;
        private float m_TextCenterX = 0.0f;

        private ArrayList m_arBuildingList = new ArrayList();

        private string m_strBuildingGroupName = "";
        private string m_strDisplayName = "";
        private int m_nSiteID = -1;
        private int m_nID = -1;
        private string m_strSiteName = "";

        private BuildingGroup m_parentBuildingGroup = null;

        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }

        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
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

        public BuildingGroup Parent
        {
            get { return m_parentBuildingGroup; }
            set { m_parentBuildingGroup = value; }
        }

        public override string ToString()
        {
            return m_strBuildingGroupName;
        }

        public int CompareTo(object obj)
        {
            BuildingGroup group = (BuildingGroup)obj;
            return this.DisplayName.CompareTo(group.DisplayName);
        }
    }

    public class Building : IComparable
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
        private string m_strDisplayText = "";

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
            if (m_strDisplayText == null || m_strDisplayText == "")
                return m_strDisplayText;
            else
                return m_strDisplayText;
            //if (szBroadcastName == null || szBroadcastName == "")
            //    return szBroadcastName;
            //else
            //    return szBroadcastName;
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

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        protected string szBroadcastName;

        public string BroadcastName
        {
            get { return szBroadcastName; }
            set { szBroadcastName = value; }
        }

        public int CompareTo(object obj)
        {
            Building building = (Building)obj;
            return this.DisplayText.CompareTo(building.DisplayText);
        }
    }

    public class Zone : IComparable
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
            if (szBroadcastName == null || szBroadcastName == "")
                return m_strZoneName;
            else
                return szBroadcastName;
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

        private float dAzimuth = 0.0f;
        public float Azimuth
        {
            get { return dAzimuth; }
            set { dAzimuth = value; }
        }

        private string m_strDisplayText = "";
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public int CompareTo(object obj)
        {
            Zone zone = (Zone)obj;
            return this.DisplayText.CompareTo(zone.DisplayText);
        }
    }

    public class EquipmentZone : IComparable
    {
        // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기
        // -1:미사용, 0 : 소화설비 Zone, 1 : 발신기 Zone, 2 : 유해화학물질 영역, 3 : 지진센서용, 4 : 공기질센서용, 9 : Unkown 
        public enum EquipZoneType
        {
            NOTUSED = -1,
            SENSOR_TYPE = 0,
            FA_TYPE = 1,
            PSM_TYPE = 2,
            EARTHQUAKE_TYPE = 3,
            AIR_QUALITY_TYPE = 4,
            SECURITY_TYPE =9,
            UNKOWN
        };

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

        private string szBroadcastName;

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

        private string m_strDisplayText = "";
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public int CompareTo(object obj)
        {
            EquipmentZone equipZone = (EquipmentZone)obj;
            return this.DisplayText.CompareTo(equipZone.DisplayText);
        }

        public static EquipZoneType ToEquipZoneType(int nEquipZoneType)
        {
            foreach (EquipZoneType type in Enum.GetValues(typeof(EquipZoneType)))
            {
                if (nEquipZoneType == (int)type)
                    return type;
            }

            return EquipZoneType.NOTUSED;
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
            if (m_fFloorIndex >= 10000)
                return "";

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

    public class _3DText
    {
        private int m_nID = -1;
        private string m_strTextName = "";
        private string m_strDisplayText = "";
        private VariousData<System.Drawing.Color> m_textColor = null;
        private VariousData<float> m_textFontHeight = null;
        private float m_TextCenterY = 0.0f;
        private float m_TextCenterX = 0.0f;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strTextName; }
            set { m_strTextName = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public VariousData<System.Drawing.Color> TextColor
        {
            get { return m_textColor; }
            set { m_textColor = value; }
        }

        public VariousData<float> TextFontHeight
        {
            get { return m_textFontHeight; }
            set { m_textFontHeight = value; }
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
    }

    public class EquipmentZoneObjectList
    {
        private EquipmentZone m_Zone = null;

        public EquipmentZone Zone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }

        private List<Sensor.ISensor> m_arSensorList = new List<Sensor.ISensor>();

        public List<Sensor.ISensor> SensorList
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

}


/// <summary>
/// 방재장비 위치
/// </summary>
public class DisasterPreventionEquipmentLocation
{
    private int m_nID = -1;
    private string m_strName = String.Empty;
    private int m_nIndex = -1;

    /// <summary>
    /// 방재장비 DB ID
    /// </summary>
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    /// <summary>
    /// 방재장비 이름
    /// </summary>
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int Index
    {
        get { return m_nIndex; }
        set { m_nIndex = value; }
    }


    public override string ToString()
    {
        return m_strName;
    }
}

/// <summary>
/// 방재장비 유형
/// </summary>
public class DisasterPreventionEquipmentType
{
    private int m_nID = -1;
    private string m_strName = String.Empty;
    private int m_nIndex = -1;

    /// <summary>
    /// 방재장비 위치 DB ID
    /// </summary>
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    /// <summary>
    /// 방재장비 위치 이름
    /// </summary>
    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int Index
    {
        get { return m_nIndex; }
        set { m_nIndex = value; }
    }


    public override string ToString()
    {
        return m_strName;
    }
}

/// <summary>
/// 방재장비
/// </summary>
public class DisasterPreventionEquipment
{
    public enum STATUS { NON = 0, NEW = 1, UPD = 2, DEL = 3 }

    private int m_nID = -1;
    private DisasterPreventionEquipmentType m_Type = null;
    private DisasterPreventionEquipmentLocation m_Location = null;
    private string m_strName = String.Empty;
    private int m_nQuantity = 0;
    private string m_strDescription = String.Empty;
    private int m_nIndex = -1;

    private STATUS m_status = STATUS.NON;

    /// <summary>
    /// 방재장비 ID
    /// </summary>
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    /// <summary>
    /// 방재장비 유형 객체
    /// </summary>
    public DisasterPreventionEquipmentType Type
    {
        get { return m_Type; }
        set { m_Type = value; }
    }

    /// <summary>
    /// 방재장비 위치 객체
    /// </summary>
    public DisasterPreventionEquipmentLocation Location
    {
        get { return m_Location; }
        set { m_Location = value; }
    }

    /// <summary>
    /// 방재장비 이름
    /// </summary>
    public string Name
    {
        get { return m_strName;}
        set { m_strName = value;}
    }

    /// <summary>
    /// 방재장비 수량
    /// </summary>
    public int Quantity
    {
        get { return m_nQuantity; }
        set { m_nQuantity = value; }
    }

    /// <summary>
    /// 방재장비 비고(메모)
    /// </summary>
    public string Description
    {
        get { return m_strDescription; }
        set { m_strDescription = value; }
    }

    /// <summary>
    /// 정렬 순서
    /// </summary>
    public int Index
    {
        get { return m_nIndex; }
        set { m_nIndex = value; }
    }

    /// <summary>
    /// 현재 사용자 계정에서의 데이터 상태
    /// </summary>
    public STATUS Status
    {
        get { return m_status; }
        set { m_status = value; }
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
public class DataTeamControlRoom : DataTeam
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

    /*private bool m_bTeamLeader = false;

    public bool TeamLeader
    {
        get { return m_bTeamLeader; }
        set { m_bTeamLeader = value; }
    }*/

    private DataTeam m_team = null;

    public DataTeam Team
    {
        get { return m_team; }
        set { m_team = value; }
    }

    // 한 개인이 여러팀에 속해있을때 각 팀에 따라 팀장일수도 팀원일수도 있다.
    /*private Dictionary<DataTeam, bool> m_dicTeamLeaders = new Dictionary<DataTeam, bool>();

    public Dictionary<DataTeam, bool> TeamLeaders
    {
        get { return m_dicTeamLeaders; }
    }

    public DataTeam GetFirstTeam()
    {
        foreach (KeyValuePair<DataTeam, bool> pair in m_dicTeamLeaders)
        {
            return pair.Key;
        }
            
        return null;
    }*/

    public override string ToString()
    {
        return m_szName;
    }
}

public class DataCompanyMember : IComparable
{
    private int m_nID = -1;
    private string m_strMemberName = "";
    //private DataTeam m_team = null;
    private int m_nLevelID = -1;
    //private int m_nPositionID = -1;
    private string m_strMemberID = "";
    private string m_strPhoneNumber = "";
    private string m_strOfficePhoneNumber = "";
    private Dictionary<DataTeam, int> m_dicTeamPositions = new Dictionary<DataTeam, int>();

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

    /*public DataTeam Team
    {
        get { return m_team; }
        set { m_team = value; }
    }*/

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

    public Dictionary<DataTeam, int> TeamPositions
    {
        get { return m_dicTeamPositions; }
    }

    /*public bool IsTeamLeader
    {
        get { return m_nPositionID == 2; }
    }*/

    public int GetFirstTeamPosition()
    {
        foreach (KeyValuePair<DataTeam, int> pair in m_dicTeamPositions)
        {
            return pair.Value;
        }

        return -1;
    }

    public DataTeam GetFirstTeam()
    {
        foreach (KeyValuePair<DataTeam, int> pair in m_dicTeamPositions)
        {
            return pair.Key;
        }

        return null;
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
        if (obj.GetType() != typeof(DataCompanyMember))
            return -1;
        DataCompanyMember member = (DataCompanyMember)obj;
        int nPosition = this.GetFirstTeamPosition();

        if (nPosition != member.GetFirstTeamPosition())
            return nPosition == 2 ? -1 : 1;

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

namespace UnE.Sensor
{
    using UnE.Spatial;

    public abstract class IFacility
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
            CORONA = 22,                        // 코로나
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

        public static string GetFacilityTypeString(FacilityType nType)
        {
            if (nType == IFacility.FacilityType.FIRE_SENSOR ||
                nType == IFacility.FacilityType.FireSensor_TypeA ||
                nType == IFacility.FacilityType.FireSensor_TypeB)
                return "화재센서";
            else if (nType == IFacility.FacilityType.COOLER_SENSOR)
                return "스프링쿨러";
            else if (nType == IFacility.FacilityType.PRESSURE_SENSOR)
                return "펌프압력";
            else if (nType == IFacility.FacilityType.PSM_SENSOR)
                return "유해화학물질 센서";
            else if (nType == IFacility.FacilityType.AIR_QUAILITY)
                return "공기질 센서";
            else if (nType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                return "온도/습도 센서";
            else if (nType == IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT)
                return "방재장비";
            else if (nType == IFacility.FacilityType.FireSensor_Monitoring)
                return "감시";
            else if (nType == IFacility.FacilityType.FireSensor_SensingLine)
                return "감지선";
            else if (nType == IFacility.FacilityType.FireSensor_AnalogSmokeType)
                return "연기감지기";
            else if (nType == IFacility.FacilityType.FireSensor_MonitoringType)
                return "감시센서";
            else if (nType == IFacility.FacilityType.CCTV)
                return "CCTV";
            else if (nType == IFacility.FacilityType.FE)
                return "소화기";
            else if (nType == IFacility.FacilityType.HD)
                return "소화전";
            else if (nType == IFacility.FacilityType.FA)
                return "발신기";
            else if (nType == IFacility.FacilityType.FR)
                return "수신기";
            else if (nType == IFacility.FacilityType.FireSensor_GasEmission)
                return "가스방출";
            else if (nType == IFacility.FacilityType.FireSensor_ManualControl)
                return "수동조작함";
            else if (nType == IFacility.FacilityType.FireSensor_SiemensType)
                return "지멘스자탐";
            else if (nType == IFacility.FacilityType.FireSensor_LightType)
                return "광선식";
            else if (nType >= IFacility.FacilityType.Intrusion_S1 && nType <= IFacility.FacilityType.EmergencyBell_S1)
                return "SVMS";
            else if (nType >= IFacility.FacilityType.GeneralIntrusionT1_S1 && nType <= IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1)
                return "S1Access";
            else if (nType == IFacility.FacilityType.ExternalAlarmBell)
                return "외부 비상벨";
            else if (nType >= IFacility.FacilityType.SecomFire && nType <= IFacility.FacilityType.SecomWomenAlarmBell)
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
            else if (nType == FacilityType.CORONA)
                return "코로나";

            return "";
        }
    
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
            if (type >= FacilityType.FIREWALL && type <= FacilityType.CORONA)
                return true;

            return false;
        }

        public static bool IsPSMSensorType(FacilityType type)
        {
            return type == FacilityType.PSM_SENSOR;
        }

        public static bool IsEarthquakeSensorType(FacilityType type)
        {
            return type == FacilityType.Earthquake;
        }

        private static Dictionary<int, FacilityType> m_dicFacilityType = null;

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

        protected string m_strIconPath = null;
        abstract public string IconPath
        {
            get;
            set;
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

        public virtual IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            return null;
        }

        public virtual void UpdateDBData()
        {
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

    public class SensorReactionHistory
    {
        private Zone zone = null;
        private int m_ZoneID = -1;
        private int nReactionCount = -1;
        private int nMulFunctionCount = -1;
        private int nFireCount = -1;
        private int nManualFireCount = -1;
        private int nEquipID = -1;

        public int EquipID
        {
            get { return nEquipID; }
            set { nEquipID = value; }
        }

        private int nReactionID = -1;

        public int ReactionID
        {
            get { return nReactionID; }
            set { nReactionID = value; }
        }

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


    public class PopupFactoryHelper
    {
        internal static IPopupFactory m_FactoryInstance = null;
        public static void SetFactory(IPopupFactory instance)
        {
            m_FactoryInstance = instance;
        }

        public static IPopupFactory GetFactory()
        {
            return m_FactoryInstance;
        }
    }

    public interface IPopupFactory
    {
        IPOIPopup CreatePopup(ISensorTooltipOwner view, ISensor sensor, int nType);
        IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility sensor);
        IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility equip, IFacility.FacilityType type);
    }
       
    // Pump Pressuer Sensor
    public abstract class ISensor : IFacility
    {
        private int m_zoneID = -1;
        public int ZoneID
        {
            get { return m_zoneID; }
            set { m_zoneID = value; }
        }

        private int m_zoneIDDB = -1;
        public int ZoneIDDB
        {
            get { return m_zoneIDDB; }
            set { m_zoneIDDB = value; }
        }

        private int m_EquipZoneID = -1;        
        public int EquipZoneID
        {
            get { return m_EquipZoneID; }
            set { m_EquipZoneID = value; }
        }

        private int m_EquipZoneIDDB = -1;
        public int EquipZoneDB
        {
            get { return m_EquipZoneIDDB; }
            set { m_EquipZoneIDDB = value; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }

        private bool m_bDeActivate = false;
        public bool DeActivate
        {
            get { return m_bDeActivate; }
            set { m_bDeActivate = value; }
        }

        private string m_szDescription = "";
        public string Description
        {
            get { return m_szDescription; }
            set { m_szDescription = value; }
        }

        private int m_nSensorData = -1;
        public int SensorData
        {
            get { return m_nSensorData; }
            set { m_nSensorData = value; }
        }

        private bool m_bInitSensor = true;
        public bool InitSensor
        {
            get { return m_bInitSensor; }
            set { m_bInitSensor = value; }
        }

        private int m_nOrgID = -1;
        public int OrgSensorID
        {
            get { return m_nOrgID; }
            set { m_nOrgID = value; }
        }

        private bool m_soundOn = true;
        public bool SoundOn
        {
            get { return m_soundOn; }
            set { m_soundOn = value; }
        }

        override public IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            return null;
        }

        public string TypeString
        {
            get
            {
                if (Type == FacilityType.FIRE_SENSOR || (Type >= FacilityType.FireSensor_TypeA && Type <= FacilityType.FireSensor_MonitoringType))
                    return "화재 탐지";
                else if (Type == FacilityType.COOLER_SENSOR)
                    return "소화 센서";
                else if (Type == FacilityType.PRESSURE_SENSOR)
                    return "압력 센서";
                else if (Type == FacilityType.PSM_SENSOR)
                    return "유해화학물질 누출감지 센서";
                else if (Type == FacilityType.CCTV)
                    return "CCTV";
                else if (Type >= FacilityType.FE && Type <= FacilityType.FR)
                    return "소방시설";
                else if (Type == FacilityType.DISASTER_PREVENTION_EQUIPMENT)
                    return "방재장비";

                else if (Type == IFacility.FacilityType.FireF1_S1)
                    return "S1Access 화재 탐지";
                else if (Type == IFacility.FacilityType.Fire_S1)
                    return "S1SVMS 화재 탐지";

                else if (Type == IFacility.FacilityType.Intrusion_S1 ||
                         Type == IFacility.FacilityType.Loiter_S1 ||
                        Type == IFacility.FacilityType.Collapse_S1 ||
                        Type == IFacility.FacilityType.Theft_S1 ||
                        Type == IFacility.FacilityType.Neglect_S1 ||
                        Type == IFacility.FacilityType.VirtualFence_S1 ||
                        Type == IFacility.FacilityType.EmergencyBell_S1 ||
                        Type == IFacility.FacilityType.GeneralIntrusionT1_S1 ||
                        Type == IFacility.FacilityType.GeneralIntrusionT2_S1 ||
                        Type == IFacility.FacilityType.InternalIntrusionT3_S1 ||
                        Type == IFacility.FacilityType.VaultIntrusionT4_S1 ||
                        Type == IFacility.FacilityType.CustomerEmergencyC1_S1 ||
                        Type == IFacility.FacilityType.CustomerEmergencyC2_S1)
                    return "S1 방범 센서";

                else if (Type == FacilityType.DOOR)
                    return "출입문";
                else if (Type == FacilityType.FIREWALL)
                    return "방화벽";
                else if (Type == FacilityType.SUBMERGENCY)
                    return "침수";
                else if (Type == FacilityType.BLACKOUT)
                    return "정전";
                else if (Type == FacilityType.Earthquake)
                    return "지진";
                else if (Type == FacilityType.STRONG_WIND)
                    return "강풍";

                return "Unknown";
            }
        }

        protected string m_szPositionName = "";
        public string PositionName
        {
            get { return m_szPositionName; }
            set { m_szPositionName = value; }
        }

        protected string m_szDepartment = "";
        public string Department
        {
            get { return m_szDepartment; }
            set { m_szDepartment = value; }
        }

        protected string m_szPhoneNumber = "";
        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }

        public override void UpdateDBData()
        {
            m_EquipZoneIDDB = m_EquipZoneID;
            base.UpdateDBData();
        }
    }

    public partial class FireAlarm : ISensor
    {
        private FireEquipment m_AlarmStation = null;

        public FireEquipment AlarmStation
        {
            get { return m_AlarmStation; }
            set { m_AlarmStation = value; }
        }

        public override IFacility.FacilityType Type
        {
            get { return FacilityType.FA; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_ALARMSTA;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "발신기";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "발신기"; }
        }
    }

    public partial class PumpPressureSensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.PRESSURE_SENSOR; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_PERSURE;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "펌프압력";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "펌프압력"; }
        }
    }
    // Sping Cooler
    public partial class SpringCooler : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.COOLER_SENSOR; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_COOLER;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "스프링쿨러";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "스프링쿨러"; }
        }
    }
    // Fire Detector
    public partial class FireSensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.FIRE_SENSOR; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_DETECTOR;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "화재탐지";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "화재탐지"; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }
    }

    // Fire Detector
    public partial class SecuritySensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return m_SubType;  }
        }

        private IFacility.FacilityType m_SubType = FacilityType.Intrusion_S1;
        public IFacility.FacilityType SubType
        {
            get { return m_SubType; }
            set { m_SubType = value; }
        }
        
        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_DETECTOR;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "보안센서";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "보안센서"; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }
    }

    // Fire Detector
    public partial class SmokeSensor : FireSensor
    {

    }

    public class EarthquakeSensor : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.Earthquake; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_DETECTOR;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "지진탐지";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "지진탐지"; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }
    }

    public partial class PSMSensorForPOI : ISensor
    {
        public override IFacility.FacilityType Type
        {
            get { return FacilityType.PSM_SENSOR; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_DETECTOR;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "가스탐지";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return "가스탐지"; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }

    }

    public partial class EtcSensor : ISensor
    {
        private IFacility.FacilityType m_type = FacilityType.NONE;
        private string m_strDisconnectionPath = "";

        public EtcSensor()
        {
        }

        public EtcSensor(IFacility.FacilityType type)
        {
            m_type = type;
        }

        public override IFacility.FacilityType Type
        {
            get { return m_type; }
        }

        override public int GetLayerID()
        {
            return SDMS.ID.ID_LAYER_DETECTOR;
        }

        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                    return "";

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get { return m_strDisconnectionPath; }
        }

        private string m_szSensorName = "";
        public string SensorName
        {
            get { return m_szSensorName; }
            set { m_szSensorName = value; }
        }

        public void SetIconPath(string strPath)
        {
            m_strIconPath = strPath;
        }

        public void SetDisconnectionPath(string strPath)
        {
            m_strDisconnectionPath = strPath;
        }

        public void SetSensorType(IFacility.FacilityType type)
        {
            m_type = type;
        }
    }

    public partial class CCTV : IFacility
    {
        public enum LOD { DISCONNECTED = -1, LOW = 0, DEFAULT = 1, IMPORTANT, VERY_IMPORTANT };

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

        // CCTVCtrl에서 사용하는 값 추가함. skkim 2015-05-26
        private string szPassword = "";
        private string szUserName = "guest";
        private int nChannel = 0;
        private int nStream = 0;
        private int nType = 0;
        private int nHttpPort = 0;
        private string szURL = "";

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        private string m_strIPDB = "0.0.0.0";

        private short m_nPortDB = -1;
        private string m_strAccessKeyDB = "BNC-3220HR-W";
        private short m_nPlaybackModeDB = 0;
        private short m_nUseRepositoryDB = 0;
        private byte[] m_bytesDB = new byte[4] { 0, 0, 0, 0 };
        private LOD m_lodDB = LOD.DEFAULT;


        private int nHttpPortDB = 0;
        private int nTypeDB = 0;
        private int nStreamDB = 0;
        private int nChannelDB = 0;
        private string szUserNameDB = "guest";
        private string szPasswordDB = "";
        private string szURLDB = "";
        // m_nReversePTZ : 0보다 작으면 PTZ를 사용하지 않음
        private int m_nReversePTZ = 0;

        /// </summary>
        ///
        public CCTV()
        {
            m_nPort = m_nPortDB = DefaultPortNo;
        }

        public override IFacility.FacilityType Type
        {
            get { return FacilityType.CCTV; }
        }

        override public int GetLayerID()
        {
            if (m_lod == LOD.LOW)
                return SDMS.ID.ID_LAYER_CCTVLOW;
            else if (m_lod == LOD.DISCONNECTED)
                return SDMS.ID.ID_LAYER_CCTV_DISCONNECTED;

            return SDMS.ID.ID_LAYER_CCTV;
        }
        public static string IconName()
        {
            return "CCTV";
        }
        public override string IconPath
        {
            get
            {
                if (m_strIconPath == null)
                {
                    if (LODType == LOD.DISCONNECTED)
                        return DisconnectIconPath;
                    else
                        return "CCTV";
                }

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            //get { return FormMain.EnginPath() + "\\Media\\icons\\cctv.ico"; }
            get { return "CCTV"; }
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

        public int ReversePTZ
        {
            get { return m_nReversePTZ; }
            set { m_nReversePTZ = value; }
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

        public int HttpPort
        {
            get { return nHttpPort; }
            set { nHttpPort = value; }
        }

        public int CCTVType
        {
            get { return nType; }
            set { nType = value; }
        }

        public int Stream
        {
            get { return nStream; }
            set { nStream = value; }
        }

        public int Channel
        {
            get { return nChannel; }
            set { nChannel = value; }
        }

        public string UserName
        {
            get { return szUserName; }
            set { szUserName = value; }
        }

        public string Password
        {
            get { return szPassword; }
            set { szPassword = value; }
        }

        public string URL
        {
            get { return szURL; }
            set { szURL = value; }
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

        public int HttpPortDB
        {
            get { return nHttpPortDB; }
            set { nHttpPortDB = value; }
        }

        public int CCTVTypeDB
        {
            get { return nTypeDB; }
            set { nTypeDB = value; }
        }

        public int StreamDB
        {
            get { return nStreamDB; }
            set { nStreamDB = value; }
        }

        public int ChannelDB
        {
            get { return nChannelDB; }
            set { nChannelDB = value; }
        }

        public string UserNameDB
        {
            get { return szUserNameDB; }
            set { szUserNameDB = value; }
        }

        public string PasswordDB
        {
            get { return szPasswordDB; }
            set { szPasswordDB = value; }
        }

        public string URLDB
        {
            get { return szURLDB; }
            set { szURLDB = value; }
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

            nHttpPortDB = nHttpPort;
            nTypeDB = nType;
            nStreamDB = nStream;
            nChannelDB = nChannel;
            szUserNameDB = szUserName;
            szPasswordDB = szPassword;
            szURLDB = szURL;
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

        private string m_strTagID = "";
        public string TagID
        {
            get { return m_strTagID; }
            set { m_strTagID = value; }
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
                if (m_strIconPath == null)
                {
                    if (m_type == FacilityType.FE)
                        return "소화기";
                    else if (m_type == FacilityType.HD)
                        return "소화전";
                    else if (m_type == FacilityType.FA)
                        return "수신기";

                    return "";
                }

                return m_strIconPath;
            }
            set { m_strIconPath = value; }
        }

        public override string DisconnectIconPath
        {
            get
            {
                if (m_type == FacilityType.FE)
                    return "소화기";
                else if (m_type == FacilityType.HD)
                    return "소화전";
                else if (m_type == FacilityType.FA)
                    return "수신기";

                return "";
            }
        }

        public override int GetLayerID()
        {
            if (m_type == FacilityType.FE)
                return SDMS.ID.ID_LAYER_FIREEXT;
            else if (m_type == FacilityType.HD)
                return SDMS.ID.ID_LAYER_FIREHYD;
            else if (m_type == FacilityType.FA)
                return SDMS.ID.ID_LAYER_ALARMSTA;

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
                else if (m_type == FacilityType.FR)
                    return "수신기";
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

        void Hide();

        void MoveTarget(int xTarget, int yTarget);

        bool IsVisible();

        void Close();

        bool LayerVisible
        {
            get;
            set;
        }

        IntPtr Handle
        {
            get;
        }

        UnE.Sensor.ISensor Sensor
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
        private IFacility m_facility = null;
        private IPOIPopup m_popup = null;

        // 1 이면 BaseView, 2이면 ImageView
        private int m_nViewType = 1;
        public int ViewType
        {
            get { return m_nViewType; }
            set { m_nViewType = value; }
        }

        /// <summary>
        /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
        private float xDB = 0.0f, yDB = 0.0f, zDB = 0.0f;

        private Zone m_zoneDB = null;
        /// </summary>

        private System.Windows.Forms.Control m_Parent = null;

        public System.Windows.Forms.Control ParentView
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set
            {
                if (value == 1764)
                {
                    int i = 0;
                    i++;
                }
                m_nID = value;
            }
        }

        public virtual IFacility.FacilityType Type
        {
            get { return m_facility == null ? IFacility.FacilityType.NONE : m_facility.Type; }
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

        public IFacility Facility
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
    
    public class FacilityManager
    {
        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = -1;
        private IFacility.FacilityType m_type = IFacility.FacilityType.NONE;
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

        // 0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam), 4(RegularCompany), 5(ExternalCompany), 6(당직자)
        // 7(교대 근무자)
        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public IFacility.FacilityType Type
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
        private class ArrayListEx : ArrayList
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

        private IFacility.FacilityType m_type = IFacility.FacilityType.NONE;
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
            m_arrControlRoomMembers = new ArrayListEx(this);
        }

        public IFacility.FacilityType Type
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

        private int m_nState = -1;

        public int State
        {
            get { return m_nState; }
            set { m_nState = value; }
        }

        private DateTime m_dtUpdateTime;

        public System.DateTime UpdateTime
        {
            get { return m_dtUpdateTime; }
            set { m_dtUpdateTime = value; }
        }

        private ReciverType m_nType = 0;
        public ReciverType Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        private int m_nReceiverID = 0;
        public int ReceiverID
        {
            get { return m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        // Key : TagNo
        private Dictionary<int, Circuit> m_dicCircuits = new Dictionary<int, Circuit>();
        public Dictionary<int, Circuit> Circuits
        {
            get { return m_dicCircuits; }
        }

        public enum ReciverType
        {
            UNKNOWN = -1,
            화재수신반 = 1,
            유해물질수신반 = 2,
            SVMS이벤트서버 = 3,
            아신화재감시 = 4,
            외부비상벨서버 = 5,
            Acess연결서버 = 6,
            Secom서버 = 7
        }

        public static ReciverType ToReciverType(int nReceiverType)
        {
            foreach (ReciverType type in Enum.GetValues(typeof(ReciverType)))
            {
                if (nReceiverType == (int)type)
                    return type;
            }

            return ReciverType.UNKNOWN;
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

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nTagNum = -1;
        public int TagNum
        {
            get { return m_nTagNum; }
            set { m_nTagNum = value; }
        }

        public IFacility.FacilityType m_sensorType = IFacility.FacilityType.NONE;
        public IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
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

    public class SensorZoneUpdateData
    {
        private ISensor m_sensorZone = null;
        private int m_equipZoneOrigin = 0;
        private int m_equipZoneChanged = 0;
        private int nZoneID = -1;

        public int Zone
        {
            get { return nZoneID; }
            set { nZoneID = value; }
        }

        public ISensor SensorZone
        {
            get { return m_sensorZone; }
            set { m_sensorZone = value; }
        }

        public int OriginEquipZone
        {
            get { return m_equipZoneOrigin; }
            set { m_equipZoneOrigin = value; }
        }

        public int ChangedEquipZone
        {
            get { return m_equipZoneChanged; }
            set { m_equipZoneChanged = value; }
        }

        public SensorZoneUpdateData()
        {
        }

        public SensorZoneUpdateData(ISensor sensorZone, int equipZoneOrigin, int equipZoneChanged)
        {
            m_sensorZone = sensorZone;
            m_equipZoneOrigin = equipZoneOrigin;
            m_equipZoneChanged = equipZoneChanged;

            if (equipZoneChanged == 0)
            {
                nZoneID = -1;
            }
            else
            {
                if (sensorZone.POI != null)
                {
                    nZoneID = (sensorZone.POI.Zone != null ? sensorZone.POI.Zone.ID : -1);
                }
            }

        }
    }

    public interface ISensorTooltipOwner
    {        
        bool IsTemporaryHiddenPOI(POI poi);

        void EnablePOI(int nID, string szType, bool bEnable);

        void AddToolTipControl(System.Windows.Forms.Control c);

        void ShowIconPOI(int nID,string szType, bool bShow);
        void ShowIconPOIFile(string strPOIType, string strFilePath, List<int> poiIDs, List<string> poiTypes, List<bool> poiVisibles);

        void AddPOI(POI poi);
        void AddPOIFile(string strPOIType, string strFilePath, List<POI> pois);
        // strPOIType이 빈문자열이면 전체 POI들을 모두 지운다.
        void ClearPOI(string strPOIType);

        // poi의 아이콘을 strPOIType으로 바꾼다.
        void ChangePOIIcon(POI poi, string strPOIType);
        // poi의 아이콘을 strPOIType으로 바꾼다.
        void ChangePOIIcons(List<POI> pois, List<string> poiTypes);
        // strPOIType의 POI들을 원래 모양대로 모두 되돌려 놓는다.
        // strPOIType이 빈문자열이면 모든 POI Icon들을 모두 원래대로 되돌려 놓는다.
        void RollBackPOIIcon(string strPOIType);

        void ClearPOISelection();

        void SelectPOI(int nID, string szType);

        POI FindPOI(int nID, string szType);

        POI FindPOI(string szKey);

        System.Collections.ArrayList SelectedPOIList
        {
            get;
        }
    }


}

namespace UnE.Sensor
{   
    public partial class FireSensor : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.FIRE_SENSOR);
        }
    }

    public partial class PSMSensorForPOI : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.PSM_SENSOR);
        }
    }

    public partial class SmokeSensor : FireSensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.FireSensor_AnalogSmokeType);
        }
    }

    public partial class SpringCooler : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.COOLER_SENSOR);
        }
    }

    public partial class PumpPressureSensor : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.PRESSURE_SENSOR);
        }
    }

    public partial class FireAlarm : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, FacilityType.FA);
        }
    }

    public partial class SecuritySensor : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, m_SubType);
        }
    }

    public partial class CCTV : IFacility
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view,  IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (IFacility)this);
        }
    }

    public partial class FireEquipment : IFacility
    {            
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (IFacility)this, m_type);
        }
    }

    public partial class EtcSensor : ISensor
    {
        public override IPOIPopup CreatePopup(ISensorTooltipOwner view, IPopupFactory iFactory)
        {
            iFactory = PopupFactoryHelper.GetFactory();
            if (iFactory == null)
                return null;
            return iFactory.CreatePopup(view, (ISensor)this, m_type);
        }
    }
}
