using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulbrainSensorTester.Data
{
    public class BuildingGroup
    {
        int m_nID = -1;
        string m_strGroupName = "";
        int? m_nParentID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public int? ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }
    }

    public class Building
    {
        int m_nID = -1;
        string m_strBuildingName = "";
        int m_nBuildingGroupID = -1;
        string m_strDisplayText = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }
    }

    public class Zone
    {
        int m_nID = -1;
        string m_strZoneName = "";
        int m_nBuildingID = -1;
        int m_nFloorIndex = -1;
        string m_strDisplayText = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }
    }

    public class EquipmentZone
    {
        int m_nID = -1;
        string m_strZoneName = "";
        string m_strLinkedZoneIDList = "";
        string m_strDisplayText = "";
        List<int> m_listZoneID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string LinkedZoneIDList
        {
            get { return m_strLinkedZoneIDList; }
            set { m_strLinkedZoneIDList = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public List<int> ListZoneID
        {
            get { return m_listZoneID; }
            set { m_listZoneID = value; }
        }
    }

    public class FireSensorData
    {
        int m_nID = -1;
        string m_strName = "";
        string m_strPositionName = "";
        int m_nZoneID = -1;
        int m_nEquipZoneID = -1;

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

        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }
    }

    public class ETCSensorData
    {
        int m_nID = -1;
        string m_strName = "";
        int m_nSensorType = -1;
        string m_strPositionName = "";
        int m_nZoneID = -1;
        int m_nEquipZoneID = -1;

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

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }
    }

    public class PSMSensorData
    {
        int m_nID = -1;
        string m_strName = "";
        int m_nMaterialType = -1;
        string m_strPositionName = "";
        int m_nEquipZoneID = -1;

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

        public int MaterialType
        {
            get { return m_nMaterialType; }
            set { m_nMaterialType = value; }
        }

        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }
    }

    public class AlarmData
    {
        private int m_nSensorType = -1;
        private int m_nSensorTagID = -1;
        private int m_nSensorZoneID = -1;
        private string m_strURL = "";
        private string m_strSensorName = "";

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int SensorTagID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }
    }

    public class CommonString
    {
        // 알람 관련 정보
        public const string ALARM_METHOD = "POST";
        

    }
}
