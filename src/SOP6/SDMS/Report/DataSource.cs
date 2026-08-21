using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS
{
    namespace Report
    {
        public class DetectPageGridData
        {
            private int m_nNo = 0;
            private DateTime m_timeStamp = new DateTime();
            private string m_strSensorType = "";
            private string m_strSensorName = "";
            private string m_strBuildingGroup = "";
            private string m_strBuilding = "";
            private string m_strFloor = "";
            private string m_strLocation = "";
            private string m_strMemo = "";
            private string m_strStatus = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public string SensorType
            {
                get { return m_strSensorType; }
                set { m_strSensorType = value; }
            }

            public string SensorName
            {
                get { return m_strSensorName; }
                set { m_strSensorName = value; }
            }

            public string BuildingGroup
            {
                get { return m_strBuildingGroup; }
                set { m_strBuildingGroup = value; }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Floor
            {
                get { return m_strFloor; }
                set { m_strFloor = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public string Memo
            {
                get { return m_strMemo; }
                set { m_strMemo = value; }
            }

            public string Status
            {
                get { return m_strStatus; }
                set { m_strStatus = value; }
            }
        }

        public class DetectPSMPageGridData
        {
            private int m_nNo = 0;
            private DateTime m_timeStamp = new DateTime();
            private string m_strMaterial = "";
            private string m_strSensorName = "";
            private string m_strBuilding = "";
            private string m_strLocation = "";
            private string m_strAlarmDepth = "";
            private string m_strStatus = "";
            private string m_strMemo = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public string Material
            {
                get { return m_strMaterial; }
                set { m_strMaterial = value; }
            }

            public string SensorName
            {
                get { return m_strSensorName; }
                set { m_strSensorName = value; }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public string AlarmDepth
            {
                get { return m_strAlarmDepth; }
                set { m_strAlarmDepth = value; }
            }

            public string DetectTrend
            {
                get { return "상세보기"; }
            }

            public string Status
            {
                get { return m_strStatus; }
                set { m_strStatus = value; }
            }

            public string Memo
            {
                get { return m_strMemo; }
                set { m_strMemo = value; }
            }
        }

        public class DetectPageEarthquakeGridData
        {
            private int m_nNo = 0;
            private DateTime m_timeStamp = new DateTime();
            // 진도 또는 규모
            private string m_strSensorData = "";
            private string m_strAlarmDepth = "";
            private string m_strMemo = "";
            private string m_strStatus = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public string SensorData
            {
                get { return m_strSensorData; }
                set { m_strSensorData = value; }
            }

            public string AlarmDepth
            {
                get { return m_strAlarmDepth; }
                set { m_strAlarmDepth = value; }
            }

            public string Memo
            {
                get { return m_strMemo; }
                set { m_strMemo = value; }
            }

            public string Status
            {
                get { return m_strStatus; }
                set { m_strStatus = value; }
            }
        }

        public class DetectPageTHGridData
        {
            private int m_nNo = 0;
            private DateTime m_timeStamp = new DateTime();
            private string m_strSensorType = "";
            private string m_strSensorName = "";
            private string m_strAlarmType = "";
            private string m_strLocation = "";
            private string m_strMemo = "";
            private string m_strStatus = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public string SensorType
            {
                get { return m_strSensorType; }
                set { m_strSensorType = value; }
            }

            public string SensorName
            {
                get { return m_strSensorName; }
                set { m_strSensorName = value; }
            }

            public string AlarmType
            {
                get { return m_strAlarmType; }
                set { m_strAlarmType = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public string Memo
            {
                get { return m_strMemo; }
                set { m_strMemo = value; }
            }

            public string Status
            {
                get { return m_strStatus; }
                set { m_strStatus = value; }
            }
        }

        public class NotOperationPageGridData
        {
            private int m_nNo = 0;
            private string m_strSensorType = "";
            private string m_strBuildingGroup = "";
            private string m_strBuilding = "";
            private string m_strFloor = "";
            private string m_strDetect = "";
            private string m_strFire = "";
            private string m_strMalfunction = "";
            private string m_strFieldRecovery = "";
            private string m_strMalfunctionRate = "";
            private string m_strManager = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public string SensorType
            {
                get { return m_strSensorType; }
                set { m_strSensorType = value; }
            }

            public string BuildingGroup
            {
                get { return m_strBuildingGroup; }
                set { m_strBuildingGroup = value; }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Floor
            {
                get { return m_strFloor; }
                set { m_strFloor = value; }
            }

            public string Detect
            {
                get { return m_strDetect; }
                set { m_strDetect = value; }
            }

            public string Fire
            {
                get { return m_strFire; }
                set { m_strFire = value; }
            }

            public string Malfunction
            {
                get { return m_strMalfunction; }
                set { m_strMalfunction = value; }
            }

            public string FieldRecovery
            {
                get { return m_strFieldRecovery; }
                set { m_strFieldRecovery = value; }
            }

            public string MalfunctionRate
            {
                get { return m_strMalfunctionRate; }
                set { m_strMalfunctionRate = value; }
            }

            public string Manager
            {
                get { return m_strManager; }
                set { m_strManager = value; }
            }
        }

        public class NotOperationPSMPageGridData
        {
            private int m_nNo = 0;
            private UnE.PSM.PSMMaterial m_material = null;
            private string m_strBuilding = "";
            private string m_strLocation = "";
            private string m_strDetect = "";
            private string m_strReport = "";
            private string m_strSystemRecovery = "";
            private string m_strFieldRecovery = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public UnE.PSM.PSMMaterial Material
            {
                get { return m_material; }
                set { m_material = value; }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public string Detect
            {
                get { return m_strDetect; }
                set { m_strDetect = value; }
            }

            public string Report
            {
                get { return m_strReport; }
                set { m_strReport = value; }
            }

            public string SystemRecovery
            {
                get { return m_strSystemRecovery; }
                set { m_strSystemRecovery = value; }
            }

            public string FieldRecovery
            {
                get { return m_strFieldRecovery; }
                set { m_strFieldRecovery = value; }
            }
        }

        public class NotOperationEarthquakePageGridData
        {
            private int m_nNo = 0;
            private string m_strSensorType = "";
            private string m_strBuildingGroup = "";
            private string m_strBuilding = "";
            private string m_strFloor = "";
            private string m_strDetect = "";
            private string m_strFire = "";
            private string m_strMalfunction = "";
            private string m_strFieldRecovery = "";
            private string m_strMalfunctionRate = "";
            private string m_strManager = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public string SensorType
            {
                get { return m_strSensorType; }
                set { m_strSensorType = value; }
            }

            public string BuildingGroup
            {
                get { return m_strBuildingGroup; }
                set { m_strBuildingGroup = value; }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Floor
            {
                get { return m_strFloor; }
                set { m_strFloor = value; }
            }

            public string Detect
            {
                get { return m_strDetect; }
                set { m_strDetect = value; }
            }

            public string Fire
            {
                get { return m_strFire; }
                set { m_strFire = value; }
            }

            public string Malfunction
            {
                get { return m_strMalfunction; }
                set { m_strMalfunction = value; }
            }

            public string FieldRecovery
            {
                get { return m_strFieldRecovery; }
                set { m_strFieldRecovery = value; }
            }

            public string MalfunctionRate
            {
                get { return m_strMalfunctionRate; }
                set { m_strMalfunctionRate = value; }
            }

            public string Manager
            {
                get { return m_strManager; }
                set { m_strManager = value; }
            }
        }

        public class SMSPageGridData
        {
            private int m_nNo = 0;
            private DateTime m_dtTimeStamp = new DateTime();
            private string m_strLocation = "";
            private string m_strSendCount = "";
            private string m_strSendText = "";

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_dtTimeStamp; }
                set { m_dtTimeStamp = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public string SendCount
            {
                get { return m_strSendCount; }
                set { m_strSendCount = value; }
            }

            public string SendText
            {
                get { return m_strSendText; }
                set { m_strSendText = value; }
            }
        }
    }

    namespace Admin
    {
        public class SensorListGridData
        {
            private int m_nNo = 0;
            private string m_strType = "";
            private string m_strName = "";
            private string m_strStatus = "";
            private string m_strBuilding = "";
            private string m_strFloor = "";
            private string m_strDescription = "";
            // 이 값은 FormSensorList.SensorType을 따른다.
            private int m_nSensorTypeID = 0;
            private UnE.Spatial.EquipmentZone m_equipZone = null;
            private UnE.Spatial.Zone m_zone = null;

            public int No
            {
                get { return m_nNo; }
                set { m_nNo = value; }
            }

            public string Type
            {
                get { return m_strType; }
                set { m_strType = value; }
            }

            public string Name
            {
                get { return m_strName; }
                set { m_strName = value; }
            }

            public string Status
            {
                get { return m_strStatus; }
                set { /*m_strStatus = value;*/ }
            }

            public string Building
            {
                get { return m_strBuilding; }
                set { m_strBuilding = value; }
            }

            public string Floor
            {
                get { return m_strFloor; }
                set { m_strFloor = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            // 이 값은 FormSensorList.SensorType을 따른다.
            public int SensorTypeID
            {
                get { return m_nSensorTypeID; }
                set { m_nSensorTypeID = value; }
            }

            public UnE.Spatial.EquipmentZone EquipmentZone
            {
                get { return m_equipZone; }
                set { m_equipZone = value; }
            }

            public UnE.Spatial.Zone Zone
            {
                get { return m_zone; }
                set { m_zone = value; }
            }
        }

        /*
         * 추가 : 센서 동작 관리를 위한 GridView의 DataSource용
         *  2018.2. 26 by hypark
         */          
        public class SensorMgrListGridData
        {
            private int no = 0;
            private string sensorType = "";
            private string sensorTagName = "";
            private string buildingGroupName = "";

            private string buildingName = "";
            private string equipmentZoneName = "";
            private int tagID = 0;
            private bool sensorDeActivated = false;

            private int sensorTypeID = 0;
            private UnE.Spatial.EquipmentZone equipZone = null;
            private UnE.Spatial.Zone zone = null;

            
            public int No
            {
                get { return no; }
                set { no = value; }
            }

            public string Type
            {
                get { return sensorType; }
                set { sensorType = value; }
            }

            public string Name
            {
                get { return sensorTagName; }
                set { sensorTagName = value; }
            }
            public string BuildingGroupName
            {
                get { return buildingGroupName; }
                set { buildingGroupName = value; }
            }
            
            public string BuildingName
            {
                get { return buildingName; }
                set { buildingName = value; }
            }
            public string EZoneName
            {
                get { return equipmentZoneName; }
                set { equipmentZoneName = value; }
            }

            public int TagID
            {
                get { return tagID; }
                set { tagID = value; }
            }
            public bool SensorDeActivated
            {
                get { return sensorDeActivated; }
                set { sensorDeActivated = value; }
            }


            public int SensorTypeID
            {
                get { return sensorTypeID; }
                set { sensorTypeID = value; }
            }
           
            public UnE.Spatial.EquipmentZone EquipmentZone
            {
                get { return equipZone; }
                set { equipZone = value; }
            }

            public UnE.Spatial.Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }
        }

    }
}
