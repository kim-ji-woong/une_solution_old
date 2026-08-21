namespace SDMS.Model.Sensor.Option
{
    public class Etc
    {
        public enum Fields { SensorType, DataType, CloseAlarmSeconds, DelaySeconds, SiteID };

        // FacilityType(SensorType)
        private int m_nSensorType = -1;
        // dnsSOPID.DATA_TYPE 참조 : 1(int), 3(float), 7(string)
        private int m_nDataType = -1;
        private int? m_nCloseAlarmSeconds = null;
        private int? m_nDelaySeconds = null;
        private int m_nSiteID = -1;

        /// <summary>
        /// FacilityType(SensorType)
        /// </summary>
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        /// <summary>
        /// dnsSOPID.DATA_TYPE 참조 : 1(int), 3(float), 7(string)
        /// </summary>
        public int DataType
        {
            get { return m_nDataType; }
            set { m_nDataType = value; }
        }

        public int? CloseAlarmSeconds
        {
            get { return m_nCloseAlarmSeconds; }
            set { m_nCloseAlarmSeconds = value; }
        }

        public int? DelaySeconds
        {
            get { return m_nDelaySeconds; }
            set { m_nDelaySeconds = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.CloseAlarmSeconds ||
                field == Fields.DelaySeconds)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "OptionEtcSensor"; }
        }
    }
}
