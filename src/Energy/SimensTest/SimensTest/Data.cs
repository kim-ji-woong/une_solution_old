using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorTester
{
    public class BuildingGroup
    {
        private int m_nID = -1;
        private string m_strBuildingGroupName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }
    }

    public class Building
    {
        private int m_nID = -1;
        private string m_strBuildingName = "";
        private BuildingGroup m_buildingGroup = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }
    }

    public class Zone
    {
        private int m_nID = -1;
        private string m_strZoneName = "";
        private bool m_isOutdoor = false;
        private Building m_building = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public bool IsOutdoor
        {
            get { return m_isOutdoor; }
            set { m_isOutdoor = value; }
        }

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }
    }

    public class EquipmentZone
    {
        private int m_nID = -1;
        private string m_strEquipZoneName = "";
        private List<Zone> m_linkedZones = new List<Zone>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strEquipZoneName; }
            set { m_strEquipZoneName = value; }
        }

        public List<Zone> LinkedZones
        {
            get { return m_linkedZones; }
        }
    }

    public class SensorTag
    {
        public enum SensorType
        {
            화재센서 = 0,
            PSM센서 = 11,
            화재감지기_A = 101,
            화재감지기_B = 102,
            가스방출신호,
            수동조작함신호,
            광선식,
            지멘스자탐,
            감시,
            감지선,
            아날로그식연기,
            Unknown
        }

        private int m_nID = -1;
        private int m_nReceiverID = -1;
        private int m_nSensorTagID = -1;
        private string m_strSensorName = "";
        private string m_strSensorDisplayName = "";
        private SensorType m_sensorType = SensorType.Unknown;
        private SensorZone m_sensorZone = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ReceiverID
        {
            get { return m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int SensorTagID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public string SensorDisplayName
        {
            get { return m_strSensorDisplayName; }
            set { m_strSensorDisplayName = value; }
        }

        public SensorType TagType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        public SensorZone SensorZone
        {
            get { return m_sensorZone; }
            set { m_sensorZone = value; }
        }
    }

    public class SensorZone
    {
        private int m_nID = -1;
        private EquipmentZone m_equipZone = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        private int m_nSensorData = -1;
        public int SensorData
        {
            get { return m_nSensorData; }
            set { m_nSensorData = value; }
        }
    }
}
