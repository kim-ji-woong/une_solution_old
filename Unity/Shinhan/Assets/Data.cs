using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMS
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

        private Triangulator polygon;

        public Triangulator Polygon
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
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, OTHER_TYPE, UNKOWN };

        private Floor m_Floor = new Floor();

        public SDMS.Floor Floor
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

        public SDMS.Zone LinkedZone
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


        private Triangulator polygon;

        public Triangulator Polygon
        {
            get { return polygon; }
            set { polygon = value; }
        }


        public int CompareTo(object obj)
        {
            EquipmentZone equipZone = (EquipmentZone)obj;
            return this.DisplayText.CompareTo(equipZone.DisplayText);
        }
    }

    public class Floor : Object, IComparable
    {
        // 0은 1층, 1은 2층, 지하는 음수
        private float m_fFloorIndex = 0.0f;

        private SDMS.Zone m_zone = null;

        public SDMS.Zone Zone
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
           

    // 자료구조와 상관없이 3D 화면상에 표시하고자 하는 Text를 위한 클래스
    public class _3DText
    {
        private int m_nID = -1;
        private string m_strTextName = "";
        private string m_strDisplayText = "";
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

}