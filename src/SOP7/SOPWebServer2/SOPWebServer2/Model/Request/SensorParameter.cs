using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOPWebServer2.Model.Request
{
    public class SensorParameter
    {
        private int m_nHeader = 0;
        private string m_strClientInfo = "";
        private List<string> m_values = new List<string>();

        public int Header
        {
            get { return m_nHeader; }
            set { m_nHeader = value; }
        }

        public string ClientInfo
        {
            get { return m_strClientInfo; }
            set { m_strClientInfo = value; }
        }

        public List<string> Values
        {
            get { return m_values; }
            set { m_values = value; }
        }
    }

    public class PSMSensorData
    {
        private int m_nSensorID = 0;
        private float m_fSensorData = 0;

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public float SensorData
        {
            get { return m_fSensorData; }
            set { m_fSensorData = value; }
        }
    }

    public class PSMSensorDatas
    {
        private List<PSMSensorData> m_sensorDatas = new List<PSMSensorData>();

        public List<PSMSensorData> Datas
        {
            get { return m_sensorDatas; }
            set { m_sensorDatas = value; }
        }
    }
}
