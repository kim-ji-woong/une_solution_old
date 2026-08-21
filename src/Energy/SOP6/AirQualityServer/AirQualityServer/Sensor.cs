using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirQualityServer
{
    public class Sensor
    {
        private string m_strSensorName = "";
        private int m_nID = 0;
        private string m_strTagName = "";
        private float m_fValue = 0.0f;
        private object m_tag = null;
        private bool m_isInverse = false;
        private int m_nSensorTagInfoID = -1;
        private int m_nSensorZoneID = -1;
        private bool m_isConnected = false;

        public string Name
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

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

        public float Value
        {
            get { return m_fValue; }
            set { m_fValue = value; }
        }

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public bool IsInverse
        {
            get { return m_isInverse; }
            set { m_isInverse = value; }
        }

        public int SensorTagInfoID
        {
            get { return m_nSensorTagInfoID; }
            set { m_nSensorTagInfoID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public bool IsConnected
        {
            get { return m_isConnected; }
            set { m_isConnected = value; }
        }
    }
}
