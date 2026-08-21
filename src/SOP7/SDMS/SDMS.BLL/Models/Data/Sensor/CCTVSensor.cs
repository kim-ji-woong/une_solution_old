namespace SDMS.BLL.Models.Data.Sensor
{
    using Model.CCTV;

    public class CCTVSensor : CCTV
    {
        public string Name
        {
            get { return this.CameraName; }
            set { this.CameraName = value; }
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
            this.URL = cctv.URL;
            this.BigURL = cctv.BigURL;
            this.SmallURL = cctv.SmallURL;
            this.CameraIP = cctv.CameraIP;
            this.CameraCompanyName = cctv.CameraCompanyName;
            this.CameraModelName = cctv.CameraModelName;
            this.Description = cctv.Description;
        }
    }
}
