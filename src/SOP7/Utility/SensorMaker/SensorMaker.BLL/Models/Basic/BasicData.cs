using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Basic
{
    public class BasicData
    {
        private SensorType m_sensorType = null;
        public SensorType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }
    }

    public class SensorType
    {
        private int m_nID = -1;
        private string m_strName = "";
        private List<SensorSubType> m_subType = new List<SensorSubType>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public List<SensorSubType> SubType
        {
            get { return m_subType; }
            set { m_subType = value; }
        }
    }

    public class SensorSubType
    {
        private int m_nID = -1;
        private string m_strName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }
}
