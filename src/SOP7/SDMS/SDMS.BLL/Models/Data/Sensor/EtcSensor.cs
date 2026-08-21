namespace SDMS.BLL.Models.Data.Sensor
{
    using Model.Sensor;
    public class EtcSensor : ETC
    {
        private bool m_isIndoor = false;
        // SensorTagInfo 테이블의 ID
        private int? m_nSensorTagID = null;
        private int? m_nSensorZoneID = null;
        // FacilityType
        private int m_nFacilityType = -1;
        //private string m_strStatus = null;
        //private bool? m_enabled = null;

        public bool IsIndoor
        {
            get { return m_isIndoor; }
            set { m_isIndoor = value; }
        }

        // SensorTagInfo 테이블의 ID
        public int? SensorTagInfoID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public int? SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        /*public string Status
        {
            get { return m_strStatus; }
            set { m_strStatus = value; }
        }

        public bool? Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }*/

        public EtcSensor()
        {
        }

        public EtcSensor(ETC etc)
        {
            this.ID = etc.ID;
            this.Name = etc.Name;
            this.MaterialType = etc.MaterialType;
            this.PositionName = etc.PositionName;
            this.X = etc.X;
            this.Y = etc.Y;
            this.Z = etc.Z;
            this.CurrentData = etc.CurrentData;
            this.ZoneID = etc.ZoneID;
            this.Department = etc.Department;
            this.DepartmentPhoneNumber = etc.DepartmentPhoneNumber;
            this.Status = etc.Status;
            this.Enabled = etc.Enabled;
        }
    }
}
