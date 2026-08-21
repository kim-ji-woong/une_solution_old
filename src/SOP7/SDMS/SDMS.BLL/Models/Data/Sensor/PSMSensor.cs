using System.Collections.Generic;

namespace SDMS.BLL.Models.Data.Sensor
{
    using Model.Sensor;
    using Model.Spatial;

    public class PSMSensor : PSM
    {
        private bool m_isIndoor = false;
        private List<Zone> m_linkedZones = new List<Zone>();
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

        public List<Zone> LinkedZones
        {
            get { return m_linkedZones; }
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

        public PSMSensor()
        {
        }

        public PSMSensor(PSM psm)
        {
            this.ID = psm.ID;
            this.Name = psm.Name;
            this.PositionName = psm.PositionName;
            this.X = psm.X;
            this.Y = psm.Y;
            this.Z = psm.Z;
            this.ZoneID = psm.ZoneID;
            this.EquipZoneID = psm.EquipZoneID;
            this.CurrentData = psm.CurrentData;
            this.LimitLevel1 = psm.LimitLevel1;
            this.LimitLevel2 = psm.LimitLevel2;
            this.LimitLevel3 = psm.LimitLevel3;
            this.UseLimitLevel1 = psm.UseLimitLevel1;
            this.UseLimitLevel2 = psm.UseLimitLevel2;
            this.UseLimitLevel3 = psm.UseLimitLevel3;
            this.EquipZoneID = psm.EquipZoneID;
            this.Department = psm.Department;
            this.DepartmentPhoneNumber = psm.DepartmentPhoneNumber;
            this.Status = psm.Status;
            this.Enabled = psm.Enabled;
        }
    }
}
