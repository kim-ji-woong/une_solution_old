namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorTagInfo
    /// Sensor와 EquipmentZone간의 연결관계를 표현한다.
    /// 특정 센서가 동작하면 어떤 영역을 표시해야 하는지를 의미한다.
    /// </summary>
    public class TagInfo : IIDObject
    {
        public enum Fields { ID, SensorServerID, TagNo, SensorZoneID, Activate, Description };

        private int m_nID = -1;
        // SensorServerInfo ID
        private int m_nSensorServerID = -1;
        // TagNo
        private int m_nTagNo = -1;
        // SensorZone ID
        private int m_nSensorZoneID = -1;
        // 센서 신호 활성화인가 ?
        private bool m_isActivate = false;
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /// <summary>
        /// SensorServerInfo ID
        /// </summary>
        public int SensorServerID
        {
            get { return m_nSensorServerID; }
            set { m_nSensorServerID = value; }
        }

        /// <summary>
        /// Tag No
        /// </summary>
        public int TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }

        /// <summary>
        /// SensorZone ID
        /// </summary>
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        // 센서 신호가 활성화인가 ?
        public bool IsActivate
        {
            get { return m_isActivate; }
            set { m_isActivate = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorTagInfo"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
