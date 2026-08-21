namespace SensorMaker.BLL.Models.Data.Sensor
{
    using SDMS.Model.Sensor;
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
        private string m_strMaterialName = "";
        private string m_strUnitName = null;
        private int? m_nBuildingID = null;
        private int? m_nEquipZoneID = null;
        private int m_nSensorType = (int)dnsData.Sensor.Facility.FacilityType.ETC;

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

        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        public string UnitName
        {
            get { return m_strUnitName; }
            set { m_strUnitName = value; }
        }

        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int? EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }


        public EtcSensor()
        {
        }

        public EtcSensor(ETC etc)
        {
            this.ID = etc.ID;
            this.Name = etc.Name;
            this.PositionName = etc.PositionName;
            this.X = etc.X;
            this.Y = etc.Y;
            this.Z = etc.Z;
            this.CurrentData = etc.CurrentData;
            this.ZoneID = etc.ZoneID;
            this.Department = etc.Department;
            this.DepartmentPhoneNumber = etc.DepartmentPhoneNumber;
            this.Enabled = etc.Enabled;
            this.Status = etc.Status;
            this.UniqueKey = etc.UniqueKey;
            this.MaterialType = etc.MaterialType;
        }
    }
}
