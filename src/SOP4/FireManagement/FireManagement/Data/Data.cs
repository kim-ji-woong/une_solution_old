using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;


namespace FireManagement
{
    public class BuildingGroup : Object
    {
        private int m_nID = -1;
        private string m_strBuildingGroupName = "";

        public override string ToString()
        {
            return m_strBuildingGroupName;
        }

        public void CopyFrom(BuildingGroup buildingGroup)
        {
            m_nID = buildingGroup.m_nID;
            m_strBuildingGroupName = buildingGroup.m_strBuildingGroupName;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }
    }

    public class Building : Object
    {
        private BuildingGroup m_buildingGroup = null;

        private int m_nID = -1;
        private string m_strBuildingName = "";
        // 0은 1층, 1은 2층, 지하는 음수
        private int m_nMinFloorIndex = 0;
        private int m_nMaxFloorIndex = 0;

        private string m_strBuildingID = "";
        private string m_strBuildingCode = "";

        public override string ToString()
        {
            return m_strBuildingName;
        }

        public void CopyFrom(Building building)
        {
            m_buildingGroup = building.m_buildingGroup;
            m_nID = building.m_nID;
            m_strBuildingName = building.m_strBuildingName;
            m_nMinFloorIndex = building.m_nMinFloorIndex;
            m_nMaxFloorIndex = building.m_nMaxFloorIndex;
            m_strBuildingID = building.m_strBuildingID;
            m_strBuildingCode = building.m_strBuildingCode;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public FireManagement.BuildingGroup BuildingGroup
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
    }

    public class Zone : Object, IComparable
    {
        // m_building이 null이면 외부 공간
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

        public int CompareTo(object obj)
        {
            Zone zone = (Zone)obj;

            if (this.m_nFloorIndex + m_fAddFloor > zone.m_nFloorIndex + zone.m_fAddFloor)
                return 1;
            else if (this.m_nFloorIndex + m_fAddFloor < zone.m_nFloorIndex + zone.m_fAddFloor)
                return -1;
            //else
            return 0;
        }

        public void CopyFrom(Zone zone)
        {
            m_building = zone.m_building;
            m_nID = zone.m_nID;
            m_nFloorIndex = zone.m_nFloorIndex;
            m_fAddFloor = zone.m_fAddFloor;
            m_strZoneName = zone.m_strZoneName;
            m_strDXFFilePath = zone.m_strDXFFilePath;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public bool IsOutdoor
        {
            get { return m_building == null; }
        }

        public FireManagement.Building Building
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


        private UnE.Geometry.Polygon polygon;
        public UnE.Geometry.Polygon Polygon
        {
            get { return polygon; }
            set { polygon = value; }
        }

        private UnE.Geometry.Vertex2D mDxfTL = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D DxfTL
        {
            get { return mDxfTL; }
            set { mDxfTL = value; }
        }

        private UnE.Geometry.Vertex2D mDxfBR = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D DxfBR
        {
            get { return mDxfBR; }
            set { mDxfBR = value; }
        }

        private Point mImageTL = new Point();
        public Point ImageTL
        {
            get { return mImageTL; }
            set { mImageTL = value; }
        }

        private Point mImageBR = new Point();
        public Point ImageBR
        {
            get { return mImageBR; }
            set { mImageBR = value; }
        }

        private double mAzimuth = 0.0f;
        public double Azimuth
        {
            get { return mAzimuth; }
            set { mAzimuth = value; }
        }
    }

    public class FireEquipment
    {
        // 소화기(Fire Extinguisher), 소화전(Hydrant), 발신기(Fire Alarm), 수신반(Fire Receiver)
        public enum EquipmentType { FE = 4, HD, FA, FR, UNKNOWN };

        private int m_nID = -1;
        private string m_strRFIDTag = "";
        // 설비 관리번호
        private string m_strEquipID = "";
        // RFID Tag에 대한 별칭
        private string m_strRFIDTagID = "";
        // DXF 도면상에서의 이름
        private string m_strDXFObjID = "";
        private EquipmentType m_equipType = EquipmentType.UNKNOWN;
        private Zone m_zone = null;
        private System.Drawing.PointF m_ptCenter;
        private object m_shapeLinked = null;
        private string m_strDescription = "";
        private bool m_checkUnitFlag = false;

        public static int GetTypeID(EquipmentType type)
        {
            if (type == EquipmentType.FE)
                return 11;
            else if (type == EquipmentType.HD)
                return 12;
            else if (type == EquipmentType.FA)
                return 13;
            else if (type == EquipmentType.FR)
                return 14;

            return 0;
        }

        public static string GetTypeName(EquipmentType type)
        {
            if (type == EquipmentType.FE)
                return "소화기";
            else if (type == EquipmentType.HD)
                return "소화전";
            else if (type == EquipmentType.FA)
                return "발신기";
            else if (type == EquipmentType.FR)
                return "수신반";

            return "";
        }

        public static EquipmentType ToEquipmentType(string strTypeName)
        {
            if (strTypeName == "소화기")
                return EquipmentType.FE;
            else if (strTypeName == "소화전")
                return EquipmentType.HD;
            else if (strTypeName == "발신기")
                return EquipmentType.FA;
            else if (strTypeName == "수신반")
                return EquipmentType.FR;

            return EquipmentType.UNKNOWN;
        }

        public FireEquipment()
        {
        }

        public FireEquipment(FireEquipment equip)
        {
            this.m_nID = equip.m_nID;
            this.m_strRFIDTag = equip.m_strRFIDTag;
            this.m_strEquipID = equip.m_strEquipID;
            this.m_strRFIDTagID = equip.m_strRFIDTagID;
            this.m_strDXFObjID = equip.m_strDXFObjID;
            this.m_equipType = equip.m_equipType;
            this.m_zone = equip.m_zone;
            this.m_ptCenter = equip.m_ptCenter;
            this.m_shapeLinked = equip.m_shapeLinked;
            this.m_strDescription = equip.m_strDescription;
        }

        public bool IsSame(FireEquipment equip)
        {
            if (this.m_nID != equip.m_nID)
                return false;
            if (this.m_strRFIDTag != equip.m_strRFIDTag)
                return false;
            if (this.m_strEquipID != equip.m_strEquipID)
                return false;
            if (this.m_strRFIDTagID != equip.m_strRFIDTagID)
                return false;
            if (this.m_strDXFObjID != equip.m_strDXFObjID)
                return false;
            if (this.m_equipType != equip.m_equipType)
                return false;
            if (this.m_zone != equip.m_zone)
                return false;
            if (this.m_ptCenter != equip.m_ptCenter)
                return false;
            if (this.m_strDescription != equip.m_strDescription)
                return false;
            if (this.m_ptCenter != equip.m_ptCenter)
                return false;

            return true;
        }

        // vPos의 위치로 옮긴다.
        //public void Move(UnE.Geometry.Vertex2D vPos)
        //{
        //    //float fFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
           
        //    double x = (vPos.x);// *fFlag;
        //    double y = (vPos.y);// *fFlag;

        //    m_ptCenter.X = (float)x;
        //    m_ptCenter.Y = (float)y;

        //    if (m_shapeLinked != null)
        //    {
        //        Hatch hatch = (Hatch)m_shapeLinked;
        //        double dMoveX = vPos.x - hatch.Center.X;
        //        double dMoveY = vPos.y - hatch.Center.Y;
        //        m_shapeLinked.Move(dMoveX, dMoveY);
        //    }
        //}

        public void FromCopy(FireEquipment equip)
        {
            m_strRFIDTag = equip.m_strRFIDTag;
            m_strEquipID = equip.m_strEquipID;
            m_strRFIDTagID = equip.m_strRFIDTagID;
            m_strDXFObjID = equip.m_strDXFObjID;
            m_equipType = equip.m_equipType;
            m_zone = equip.m_zone;
            m_ptCenter = equip.m_ptCenter;
            m_shapeLinked = equip.m_shapeLinked;
            m_strDescription = equip.m_strDescription;
        }

        public void SetUnitFlag(float fUnitFlag)
        {
            if (!m_checkUnitFlag)
            {
                Position = new System.Drawing.PointF(Position.X / fUnitFlag, Position.Y / fUnitFlag);
                m_checkUnitFlag = true;
            }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string RFIDTag
        {
            get { return m_strRFIDTag; }
            set { m_strRFIDTag = value; }
        }

        // 설비 관리번호
        public string EquipID
        {
            get { return m_strEquipID; }
            set { m_strEquipID = value; }
        }

        // RFID Tag에 대한 별칭
        public string RFIDTagID
        {
            get { return m_strRFIDTagID; }
            set { m_strRFIDTagID = value; }
        }

        // DXF 도면상에서의 이름
        public string DXFObjID
        {
            get { return m_strDXFObjID; }
            set { m_strDXFObjID = value; }
        }

        public EquipmentType Type
        {
            get { return m_equipType; }
            set { m_equipType = value; }
        }

        public FireManagement.Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public System.Drawing.PointF Position
        {
            get { return m_ptCenter; }
            set { m_ptCenter = value; }
        }

        public object LinkedShape
        {
            get { return m_shapeLinked; }
            set { m_shapeLinked = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class Floor : Object, IComparable
    {
        // 0은 1층, 1은 2층, 지하는 음수
        //private int m_nFloorIndex = 0;
        private float m_fFloorIndex = 0.0f;

        /*public Floor(int nFloorIndex)
        {
            m_nFloorIndex = nFloorIndex;
        }*/
        public Floor(float fFloorIndex)
        {
            m_fFloorIndex = fFloorIndex;
        }
        
        /*public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }*/
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
        /*public int Compare(object obj1, object obj2)
        {
            Floor floor1 = (Floor)obj1;
            Floor floor2 = (Floor)obj2;

            if (floor1.m_nFloorIndex > floor2.m_nFloorIndex)
                return 1;
            else if (floor1.m_nFloorIndex < floor2.m_nFloorIndex)
                return -1;
            //else
                return 0;
        }*/

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

    public class FireEquipmentHistory
    {
        public enum EquipmentStatus { NORMAL = 0, FAULT, REPAIRING, ETC }

        private int m_nID = -1;
        private int m_nEquipmentID = -1;
        private int m_nSOPGenUserID = -1;   // 담당자의 SOPGenUser ID
        private DateTime m_time;
        private EquipmentStatus m_nStatus = EquipmentStatus.NORMAL;
        private string m_strCheckersOpinion = "";
        private string m_strDescription = "";
        // 시스템에 저장되지 않고 실행중에 생성된 History인가?
        private bool m_isNewHistory = true;

        public static string GetStatusText(EquipmentStatus status)
        {
            if (status == EquipmentStatus.NORMAL)
                return "양호";
            else if (status == EquipmentStatus.FAULT)
                return "불량/고장";
            else if (status == EquipmentStatus.REPAIRING)
                return "수리중";
            //else
                return "기타";
        }

        public static EquipmentStatus ToEquipmentStatus(string strStatusText)
        {
            if (strStatusText == "양호")
                return EquipmentStatus.NORMAL;
            else if (strStatusText == "불량/고장")
                return EquipmentStatus.FAULT;
            else if (strStatusText == "수리중")
                return EquipmentStatus.REPAIRING;

            return EquipmentStatus.ETC;
        }

        public FireEquipmentHistory()
        {
        }

        public FireEquipmentHistory(FireEquipmentHistory history)
        {
            m_nID = history.ID;
            m_nEquipmentID = history.m_nEquipmentID;
            m_nSOPGenUserID = history.m_nSOPGenUserID;
            m_time = history.m_time;
            m_nStatus = history.m_nStatus;
            m_strCheckersOpinion = history.m_strCheckersOpinion;
            m_strDescription = history.m_strDescription;
            m_isNewHistory = history.m_isNewHistory;
        }

        public bool IsSame(FireEquipmentHistory history)
        {
            if (m_nID != history.ID)
                return false;
            if (m_nEquipmentID != history.m_nEquipmentID)
                return false;
            if (m_nSOPGenUserID != history.m_nSOPGenUserID)
                return false;
            if (m_time != history.m_time)
                return false;
            if (m_nStatus != history.m_nStatus)
                return false;
            if (m_strCheckersOpinion != history.m_strCheckersOpinion)
                return false;
            if (m_strDescription != history.m_strDescription)
                return false;

            // m_isNewHistory는 비교하지 않는다.
            return true;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int EquipmentID
        {
            get { return m_nEquipmentID; }
            set { m_nEquipmentID = value; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
            set { m_nSOPGenUserID = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public EquipmentStatus Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        public string CheckersOpinion
        {
            get { return m_strCheckersOpinion; }
            set { m_strCheckersOpinion = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        // 시스템에 저장되지 않고 실행중에 생성된 History인가?
        public bool IsNewHistory
        {
            get { return m_isNewHistory; }
            set { m_isNewHistory = value; }
        }
    }

    public class EquipmentZone
    {
        // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, OTHER_TYPE, UNKOWN };

        private ArrayList m_arLinkedZoneList = new ArrayList();
        public ArrayList LinkedZoneList
        {
            get { return m_arLinkedZoneList; }
            set { m_arLinkedZoneList = value; }
        }

        //0 : 센서 Zone, 1 : 발신기 Zone
        private EquipZoneType m_nZoneType = EquipZoneType.UNKOWN;
        public EquipZoneType ZoneType
        {
            get { return m_nZoneType; }
            set { m_nZoneType = value; }
        }

        private int mID = 0;
        
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        private string m_strZoneName = "";

        public override string ToString()
        {
            return m_strZoneName;
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

        private UnE.Geometry.Vertex2D m_vTextCenter = null;
        private UnE.Geometry.Vertex2D TextCenter
        {
            get { return m_vTextCenter; }
            set { m_vTextCenter = value; }
        }

        // EquipmentZone은 여러 Zone에 걸쳐 있을수 있으므로 Zone별로 TextCenter가 다를수 있음
        private Dictionary<Zone, UnE.Geometry.Vertex2D> m_dicZoneTextCenter = new Dictionary<Zone, UnE.Geometry.Vertex2D>();
        public Dictionary<Zone, UnE.Geometry.Vertex2D> ZoneTextCenter
        {
            get { return m_dicZoneTextCenter; }
        }

        // Text를 표시하지 않는 Zone List
        private ArrayList m_arrNotShowingZone = new ArrayList();
        public ArrayList NotShowingZone
        {
            get { return m_arrNotShowingZone; }
        }
    }
}
