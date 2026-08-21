namespace SDMS.BLL.Models.Data.Sensor
{
    using Model.Sensor;

    public class FireSensor : Fire
    {
        private bool m_isIndoor = false;
        // SensorTagInfo 테이블의 ID
        private int? m_nSensorTagID = null;
        private int? m_nSensorZoneID = null;
        // SensorTagInfo 테이블의 TagNo
        private int? m_nTagNo = null;

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

        // SensorTagInfo 테이블의 TagNo
        public int? TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }

        public FireSensor()
        {
        }

        public FireSensor(Fire fire)
        {
            this.ID = fire.ID;
            this.Name = fire.Name;
            this.PositionName = fire.PositionName;
            this.X = fire.X;
            this.Y = fire.Y;
            this.Z = fire.Z;
            this.ZoneID = fire.ZoneID;
            this.Department = fire.Department;
            this.DepartmentPhoneNumber = fire.DepartmentPhoneNumber;
            this.SensorSubType = fire.SensorSubType;
        }
    }
}
