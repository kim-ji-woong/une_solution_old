using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SensorTester
{
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
        public enum EquipZoneType { SENSOR_TYPE = 0, FA_TYPE, UNKOWN };

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
		private int m_nEquipZoneID;
		public int EquipZoneID
		{
			get { return m_nEquipZoneID; }
			set { m_nEquipZoneID = value; }
		}

		private string m_szUserText = "";
		public string UserText
		{
			get { return m_szUserText; }
			set { m_szUserText = value; }
		}

		public override string ToString()
		{
			return (m_szUserText + "," + m_strAddress);
		}
	}
}
