using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSensorServer.Data
{
    /// <summary>
    /// DB 컬럼
    /// </summary>
    public class SensorInfo
    {
        private int m_nSensorZoneID = 0;
        private int m_nSensorTagInfoID = 0;
        private string m_strSensorName = "";
        private int m_nZoneID = 0;
        private int m_nEquipZoneID = 0;
        private int m_nSensorServerID = 0;
        private int m_nTagNo = 0;

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

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
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

        private int m_nStartUnit = -1;
        public int StartUnit
        {
            get { return m_nStartUnit; }
            set { m_nStartUnit = value; }
        }

        private int m_nLastUnit = -1;
        public int LastUnit
        {
            get { return m_nLastUnit; }
            set { m_nLastUnit = value; }
        }
    }
}
