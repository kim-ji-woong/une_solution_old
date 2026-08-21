using System.Collections.Generic;

namespace SDMS.BLL.Models.Response
{
    public class ResponseEquipZoneSensorList : MessageResult
    {
        private int m_nEquipZoneID = -1;
        private string m_strEquipZoneName = "";
        private string m_strSensorType = "";
        private List<int> m_sensorIDs = new List<int>();

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string EquipZoneName
        {
            get { return m_strEquipZoneName; }
            set { m_strEquipZoneName = value; }
        }

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public List<int> SensorIDs
        {
            get { return m_sensorIDs; }
            set { m_sensorIDs = value; }
        }
    }
}
