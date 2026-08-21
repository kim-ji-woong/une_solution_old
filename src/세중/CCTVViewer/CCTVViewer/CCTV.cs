using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVViewer
{
    public class CCTV
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strCameraName = string.Empty;
        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        private float m_fX = 0;
        public float X
        {
            get { return m_fX; }
            set { m_fX = value; }
        }

        private float m_fY = 0;
        public float Y
        {
            get { return m_fY; }
            set { m_fY = value; }
        }

        private float m_fZ = 0;
        public float Z
        {
            get { return m_fZ; }
            set { m_fZ = value; }
        }

        private bool m_bIsInDoor = false;
        public bool IsInDoor
        {
            get { return m_bIsInDoor; }
            set { m_bIsInDoor = value; }
        }

        private int m_nHTTPPort = 80;
        public int HTTPPort
        {
            get { return m_nHTTPPort; }
            set { m_nHTTPPort = value; }
        }

        private string strType = string.Empty;
        public string Type
        {
            get { return strType; }
            set { strType = value; }
        }

        private int m_nStream = -1;
        public int Stream
        {
            get { return m_nStream; }
            set { m_nStream = value; }
        }

        public string m_strURL = string.Empty;
        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        private string m_strDescription = string.Empty;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        private int m_nChipVersion = 0;
        public int ChipVersion
        {
            get { return m_nChipVersion; }
            set { m_nChipVersion = value; }
        }

        #region CCTV 동작을 위한 필수 정보

        private string m_strServerIP = string.Empty;
        public string ServerIP
        {
            get { return m_strServerIP; }
            set { m_strServerIP = value; }
        }

        private int m_nServerControlPort = 80;
        public int ServerControlPort
        {
            get { return m_nServerControlPort; }
            set { m_nServerControlPort = value; }
        }

        private int m_nServerVideoPort = 80;
        public int ServerVideoPort
        {
            get { return m_nServerVideoPort; }
            set { m_nServerVideoPort = value; }
        }

        private int m_nServerAudioTransmitPort = 80;
        public int ServerAudioTransmitPort
        {
            get { return m_nServerAudioTransmitPort; }
            set { m_nServerAudioTransmitPort = value; }
        }

        private int m_nServerAudioReceivePort = 80;
        public int ServerAudioReceivePort
        {
            get { return m_nServerAudioReceivePort; }
            set { m_nServerAudioReceivePort = value; }
        }

        private int m_nVideoChannel = 0;
        public int VideoChannel
        {
            get { return m_nVideoChannel; }
            set { m_nVideoChannel = value; }
        }

        private string m_strUserID = string.Empty;
        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        private string m_strUserPassword = string.Empty;
        public string UserPassword
        {
            get { return m_strUserPassword; }
            set { m_strUserPassword = value; }
        }

        #endregion CCTV 동작을 위한 필수 정보


        public CCTV(
            int nID,
            string strCameraName,
            string strServerIP,
            int nServerControlPort,
            int nServerVideoPort,
            int nServerAudioTransmitPort,
            int nServerAudioReceivePort,
            float fX,
            float fY,
            float fZ,
            bool bIsInDoor,
            int nHTTPPort,
            string strType,
            int nStream,
            int nVideoChannel,
            string strUserID,
            string strUserPassword,
            string strURL,
            int nChipVersion,
            string strDescription)
        {
            ID = nID;
            CameraName = strCameraName;
            ServerIP = strServerIP;
            ServerControlPort = nServerControlPort;
            ServerVideoPort = nServerVideoPort;
            ServerAudioTransmitPort = nServerAudioTransmitPort;
            ServerAudioReceivePort = nServerAudioReceivePort;
            X = fX;
            Y = fY;
            Z = fZ;
            IsInDoor = bIsInDoor;
            HTTPPort = nHTTPPort;
            Type = strType;
            Stream = nStream;
            VideoChannel = nVideoChannel;
            UserID = strUserID;
            UserPassword = strUserPassword;
            URL = strURL;
            ChipVersion = nChipVersion;
            Description = strDescription;
        }

    }
}