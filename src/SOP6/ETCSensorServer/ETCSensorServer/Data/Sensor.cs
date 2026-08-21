using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;

namespace ETCSensorServer.Data
{
    public class SensorTagInfo
    {
        private int m_nID = -1;
        private int m_nSensorServerID = -1;
        private int m_nTagNo = -1;
        private int m_nTagID = -1;
        private string m_strSensorName = "";
        private IFacility.FacilityType m_sensorType = IFacility.FacilityType.NONE;
        private int m_nEquipZoneID = -1;
        private int m_nSensorZoneID = -1;
        private bool m_activate = true;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorServerID
        {
            get { return m_nSensorServerID; }
            set { m_nSensorServerID = value; }
        }

        public int TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }

        public int TagID
        {
            get { return m_nTagID; }
            set { m_nTagID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public bool Activate
        {
            get { return m_activate; }
            set { m_activate = value; }
        }
    }
}
