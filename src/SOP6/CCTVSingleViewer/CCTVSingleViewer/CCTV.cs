using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVSingleViewer
{
    public class CCTV
    {
        public enum CCTVType { Unknown = 0, RTSP, Divisys, WESP };

        private int m_nID = 0;
        private string m_strCameraName = "";
        private string m_strBuildingName = "";
        private string m_strFloorName = "";
        private string m_strZoneName = "";
        private string m_strChannelNormalURL = "";
        private string m_strChannelBigURL = "";
        private string m_strChannelSmallURL = "";
        private CCTVType m_type = CCTVType.Unknown;

        private string m_strUserID = "";
        private string m_strPW = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string Password
        {
            get { return m_strPW; }
            set { m_strPW = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public string FloorName
        {
            get { return m_strFloorName; }
            set { m_strFloorName = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string ChannelNormalURL
        {
            get { return m_strChannelNormalURL; }
            set { m_strChannelNormalURL = value; }
        }

        public string ChannelBigURL
        {
            get { return m_strChannelBigURL; }
            set { m_strChannelBigURL = value; }
        }

        public string ChannelSmallURL
        {
            get { return m_strChannelSmallURL; }
            set { m_strChannelSmallURL = value; }
        }

        public CCTVType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
}
