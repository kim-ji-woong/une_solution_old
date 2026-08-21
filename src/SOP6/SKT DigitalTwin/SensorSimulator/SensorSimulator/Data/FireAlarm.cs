using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorSimulator.Data
{
    public class FireAlarm
    {
        private string m_strEquipCode = "";
        private string m_strEquipStatus = "";
        private string m_eventID = "";
        private DateTime m_eventTime = new DateTime();
        private string m_eventType = "";
        private Zone m_zone = null;
        private bool m_isAlarmOn = true;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorTagID = -1;
        private int m_nSensorZoneID = -1;
        private int m_nWebHistoryID = -1;

        public string EquipCode
        {
            get { return m_strEquipCode; }
            set { m_strEquipCode = value; }
        }

        public string EquipStatus
        {
            get { return m_strEquipStatus; }
            set { m_strEquipStatus = value; }
        }

        public string EventID
        {
            get { return m_eventID; }
            set { m_eventID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_eventTime; }
            set { m_eventTime = value; }
        }

        public string EventType
        {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public bool IsAlarmOn
        {
            get { return m_isAlarmOn; }
            set { m_isAlarmOn = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
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

        public int WebHistoryID
        {
            get { return m_nWebHistoryID; }
            set { m_nWebHistoryID = value; }
        }

        public override string ToString()
        {
            return m_zone == null ? "" : m_zone.Name;
        }
    }
}
