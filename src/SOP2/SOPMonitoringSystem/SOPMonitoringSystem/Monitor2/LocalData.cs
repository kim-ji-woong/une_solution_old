using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SOPDisasterSystem
{    
    public class FireEquipment
    {
        private int nID = 0;
        public int ID
        {
            get { return nID; }
            set { nID = value; }
        }
        private string szEquipID = "";
        public string EquipID
        {
            get { return szEquipID; }
            set { szEquipID = value; }
        }
        private int nZoneID = 0;
        public int ZoneID
        {
            get { return nZoneID; }
            set { nZoneID = value; }
        }
        private string szBuildingID = "";
        public string BuildingID
        {
            get { return szBuildingID; }
            set { szBuildingID = value; }
        }
        private int nFloorIdx = -999;
        public int FloorIndex
        {
            get { return nFloorIdx; }
            set { nFloorIdx = value; }
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
		//private int m_nFloorIndex = -1;
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

		//public int FloorIndex
		//{
		//	get { return m_nFloorIndex; }
		//	set { m_nFloorIndex = value; }
		//}

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

    // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기를 위한 Zone
    public class EquipmentZone : Object
    {
        // 소화설비(FireSensor, SpringCooler, 압력센서...), 발신기
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, OTHER_TYPE, UNKOWN };

        private int m_nID = -1;
        private string m_strName = "";
        private ArrayList m_arrLinkedZoneList = new ArrayList();
        private EquipZoneType m_type = EquipZoneType.UNKOWN;
		private string m_szBroadcastName = "";
				

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

		public string BroadcastName
		{
			get { return m_szBroadcastName; }
			set { m_szBroadcastName = value; }
		}
    }
}
