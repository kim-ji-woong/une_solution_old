using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalFireSensorDBWatcher
{
    public class ExternalFireSensor
    {
        public enum SensorState { UNKNOWN = -1, NORMAL = 0, ALARM };

        private int m_nID = 0;
        private string m_strTagName = "";
        private int m_nSensorZoneID = 0;
        private int m_nSensorTagInfoID = 0;
        private SensorState m_state = SensorState.UNKNOWN;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TagName
        {
            get { return m_strTagName; }
            set { m_strTagName = value; }
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

        public SensorState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public static SensorState ToSensorState(int value)
        {
            if (value < (int)SensorState.UNKNOWN || value > (int)SensorState.ALARM)
                return SensorState.UNKNOWN;

            return (SensorState)value;
        }
    }
}
