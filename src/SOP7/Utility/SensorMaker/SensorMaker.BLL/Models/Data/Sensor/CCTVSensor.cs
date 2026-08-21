using System.Collections.Generic;

namespace SensorMaker.BLL.Models.Data.Sensor
{
    using SDMS.Model.CCTV;

    public class CCTVSensor : CCTV
    {
        private int? m_nBuildingID = null;
        private int? m_nEquipZoneID = null;
        private List<int> m_equipZoneIDs = new List<int>();        
        private int m_nSensorType = (int)dnsData.Sensor.Facility.FacilityType.Security_Sensor;

        public string Name
        {
            get { return this.CameraName; }
            set { this.CameraName = value; }
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

        //EquipZoneCCTV
        public List<int> EquipZoneIDs
        {
            get { return m_equipZoneIDs; }
            set { m_equipZoneIDs = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public CCTVSensor()
        {
        }

        public CCTVSensor(CCTV cctv)
        {
            this.ID = cctv.ID;
            this.CameraName = cctv.CameraName;
            this.PositionName = cctv.PositionName;
            this.UniqueKey = cctv.UniqueKey;
            this.X = cctv.X;
            this.Y = cctv.Y;
            this.Z = cctv.Z;
            this.ZoneID = cctv.ZoneID;
            this.IsIndoor = cctv.IsIndoor;
            this.Type = cctv.Type;
            this.Channel = cctv.Channel;
            this.UserID = cctv.UserID;
            this.Password = cctv.Password;
            this.URL = cctv.URL;
            this.BigURL = cctv.BigURL;
            this.SmallURL = cctv.SmallURL;
            this.Enabled = cctv.Enabled;            
            this.CameraIP = cctv.CameraIP;
            this.CameraCompanyName = cctv.CameraCompanyName;
            this.CameraModelName = cctv.CameraModelName;
            this.Description = cctv.Description;
        }
    }
}
