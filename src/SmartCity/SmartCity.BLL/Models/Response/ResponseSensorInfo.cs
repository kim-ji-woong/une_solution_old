using SmartCity.Model;
using System.Collections.Generic;

namespace SmartCity.BLL.Models.Response
{

    public class ResponseSensorInfo : MessageResult
    {
        private Sensor m_Sensor = null;

        public Sensor Sensor
        {
            get { return m_Sensor; }
            set { m_Sensor = value; }
        }
    }

    public class Sensor
    {
        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = "";
        private string m_strAddr = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

    }
}
