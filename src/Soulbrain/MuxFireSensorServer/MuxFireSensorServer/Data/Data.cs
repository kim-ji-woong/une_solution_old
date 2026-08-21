using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuxFireSensorServer
{
    public class SensorTag
    {
        private int m_nID = 0;
        private int m_nType = 0;
        private int m_nTagNo = 0;
        private int m_nTagID = 0;
        private int m_nSensorZoneID = 0;
        private string m_strName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorType
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        public int TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public string SensorName
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }
}
