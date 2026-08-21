using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVAlarmWatcher
{
    public class CCTVAlarm
    {
        private int m_nID = -1;
        private int m_nCCTVID = -1;
        private int m_nEquipZoneID = -1;
        private string m_strCameraName = "";
        private int m_nSensorZoneID = -1;
        private int m_nSensorTagInfoID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int CCTVID
        {
            get { return m_nCCTVID; }
            set { m_nCCTVID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int SensorTagInfoID
        {
            get { return m_nSensorTagInfoID; }
            set { m_nSensorTagInfoID = value; }
        }

        public static int SensorType
        {
            get { return (int)UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell; }
        }
    }
}
