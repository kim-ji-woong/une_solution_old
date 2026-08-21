using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1SensorUpdate
{
    public class Zone
    {
        private int m_nZoneID = -1;
        private string m_strZoneName = "";

        public int ID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public string Name
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }
    }

    public class EquipmentZone
    {
        private int m_nEquipZoneID = -1;
        private string m_strEquipZoneName = "";
        // Link된 첫번째 Zone
        private Zone m_linkedZone = null;

        public int ID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string Name
        {
            get { return m_strEquipZoneName; }
            set { m_strEquipZoneName = value; }
        }

        // Link된 첫번째 Zone
        public Zone LinkedZone
        {
            get { return m_linkedZone; }
            set { m_linkedZone = value; }
        }
    }

    public class AccessDevice
    {
        private int m_nDeviceID = -1;
        private int m_nS1AccessID = -1;
        private string m_strDeviceName = "";

        public int ID
        {
            get { return m_nDeviceID; }
            set { m_nDeviceID = value; }
        }

        public int S1AccessID
        {
            get { return m_nS1AccessID; }
            set { m_nS1AccessID = value; }
        }

        public string Name
        {
            get { return m_strDeviceName; }
            set { m_strDeviceName = value; }
        }
    }
}
