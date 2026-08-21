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
            정전센서 = 17,
            화재감지기_A = 101,
            화재감지기_B = 102,
            가스방출신호,
            수동조작함신호,
            광선식,
            지멘스자탐,
            감시,
            감지선,
            아날로그식연기,
            
             // 서울대학교 e재난 시스템 - S1시스템 통합으로 추가됨
            // skkim     2017-03-14
            SVMS침입 = 900,  // SVMS 침입
            SVMS배회 = 901,     // SVMS 배회
            SVMS쓰러짐 = 902,   // SVMS 쓰러짐
            SVMS도난 = 903,       // SVMS 도난
            SVMS방치 = 904,           // SVMS 방치
            SVMS가상펜스 = 905,      // SVMS 가상펜스
            SVMS화재 = 906,              // SVMS 화재
            SVMS비상벨 = 907,     // SVMS 비상벨

            Access일반칩입1 = 1001,  // S1Access 일반침입1
            Access일반칩입2 = 1002,   // S1Access 일반 침입2
            Access내부침입 = 1003,// S1Access 내부침입
            Access금고침입 = 1004,   // S1Access 금고침입
            Access화재 = 2000,             // S1Access 화재
            Access고객비상1 = 2100, // S1Access 고객비상
            Access고객비상2 = 2110,// S1Access 고객 비상
            Access구급 = 2200,           // S1Access 구급
            Access가스 = 2300,               // S1Access 가스
            Access정전이상 = 3000, // S1Access 정전이상
            Access누수이상 = 3004,     // S1Access 누수이상
            Access종합경보반이상 = 3008, // S1Access 종합경보반 이상
            EMPOLL외곽비상벨=4000,
            Unknown
        }

        private int m_nID = -1;
        private int m_nReceiverID = -1;
        private int m_nSensorTagNo = -1;
        private int m_nSensorTagID = -1;
        private string m_strSensorName = "";
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

        public int SensorTagNo
        {
            get { return m_nSensorTagNo; }
            set { m_nSensorTagNo = value; }
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
