namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorZone
    /// Sensor와 EquipmentZone간의 연결관계를 표현한다.
    /// 특정 센서가 동작하면 어떤 영역을 표시해야 하는지를 의미한다.
    /// </summary>
    public class SensorZone : IIDObject
    {
        public enum Fields { ID, SensorType, OrgSensorID, EquipZoneID, IsAlarmStatus, Data };

        private int m_nID = -1;
        // FacilityType(SensorType)
        private int m_nSensorType = -1;
        // Original Sensor ID
        private int? m_nOrgSensorID = -1;
        // EquipmentZone ID
        private int m_nEquipZoneID = -1;
        // 현재 알람 상태인가?
        private bool m_isAlarmStatus = false;
        private int? m_nData = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /// <summary>
        /// FacilityType(SensorType)
        /// </summary>
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        /// <summary>
        /// Original Sensor ID
        /// </summary>
        public int? OrgSensorID
        {
            get { return m_nOrgSensorID; }
            set { m_nOrgSensorID = value; }
        }

        /// <summary>
        /// EquipmentZone ID
        /// </summary>
        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        // 현재 알람 상태인가?
        public bool IsAlarmStatus
        {
            get { return m_isAlarmStatus; }
            set { m_isAlarmStatus = value; }
        }

        public int? Data
        {
            get { return m_nData; }
            set { m_nData = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorZone"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.OrgSensorID ||
                field == Fields.Data)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
