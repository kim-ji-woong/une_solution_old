using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.IO;

namespace UnE.CCTV
{
    public class CCTV
    {
        private static short m_nDefaultPort = 9400;
        public static short DefaultPortNo
        {
            get { return m_nDefaultPort; }
        }

        private string m_strIP = "0.0.0.0";
        private short m_nPort = -1;
        private string m_strAccessKey = "BNC-3220HR-W";
        private short m_nPlaybackMode = 0;
        private short m_nUseRepository = 0;
        private byte[] m_bytes = new byte[4] { 0, 0, 0, 0 };

        // CCTVCtrl에서 사용하는 값 추가함. skkim 2015-05-26
        private string szPassword = "";
        private string szUserName = "guest";
        private int nChannel = 0;
        private int nStream = 0;
        private int nType = 0;
        private int nHttpPort = 0;
        private string szURL = "";
        // 큰 해상도 및 FPS의 URL
        private string m_strBigURL = "";
        // 작은 해상도 및 FPS의 URL
        private string m_strSmallURL = "";

        public CCTV()
        {
            m_nPort = DefaultPortNo;
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 라이브 모드 (0 : Live, 1 : Playback)
        public short PlayBackMode
        {
            get { return m_nPlaybackMode; }
            set { m_nPlaybackMode = value; }
        }

        // 리포지토리와 연동함 (0 : 사용하지 않음, 1: 사용함(사용할 시 위 리포지토리 연결 부분 정의))
        public short UseRepository
        {
            get { return m_nUseRepository; }
            set { m_nUseRepository = value; }
        }

        public string AccessKey
        {
            get { return m_strAccessKey; }
            set { m_strAccessKey = value; }
        }

        public string IPAddress
        {
            get { return m_strIP; }
            set
            {
                m_strIP = value;
                ToByteArray(m_strIP, ref m_bytes);
            }
        }

        private bool ToByteArray(string strIP, ref byte[] arrBytes)
        {
            arrBytes[0] = 0;
            arrBytes[1] = 0;
            arrBytes[2] = 0;
            arrBytes[3] = 0;

            int nIndex1 = strIP.IndexOf('.');
            if (nIndex1 < 0)
                return false;

            int nIndex2 = strIP.IndexOf('.', nIndex1 + 1);
            if (nIndex2 < 0)
                return false;

            int nIndex3 = strIP.IndexOf('.', nIndex2 + 1);
            if (nIndex3 < 0)
                return false;

            try
            {
                int n1 = int.Parse(strIP.Substring(0, nIndex1));
                int n2 = int.Parse(strIP.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1));
                int n3 = int.Parse(strIP.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1));
                int n4 = int.Parse(strIP.Substring(nIndex3 + 1));

                if (n1 < 0 || n1 > 255 || n2 < 0 || n2 > 255 || n3 < 0 || n3 > 255 || n4 < 0 || n4 > 255)
                    return false;

                arrBytes[0] = (byte)n1;
                arrBytes[1] = (byte)n2;
                arrBytes[2] = (byte)n3;
                arrBytes[3] = (byte)n4;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public byte[] IPBytes
        {
            get { return m_bytes; }
            set
            {
                m_bytes[0] = value[0];
                m_bytes[1] = value[1];
                m_bytes[2] = value[2];
                m_bytes[3] = value[3];

                m_strIP = string.Format("{0}.{1}.{2}.{3}", (int)m_bytes[0], (int)m_bytes[1], (int)m_bytes[2], (int)m_bytes[3]);
            }
        }

        public short PortNo
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public int HttpPort
        {
            get { return nHttpPort; }
            set { nHttpPort = value; }
        }

        public int CCTVType
        {
            get { return nType; }
            set { nType = value; }
        }

        public int Stream
        {
            get { return nStream; }
            set { nStream = value; }
        }

        public int Channel
        {
            get { return nChannel; }
            set { nChannel = value; }
        }

        public string UserName
        {
            get { return szUserName; }
            set { szUserName = value; }
        }

        public string Password
        {
            get { return szPassword; }
            set { szPassword = value; }
        }

        public string URL
        {
            get { return szURL; }
            set { szURL = value; }
        }

        // 큰 해상도 및 FPS의 URL
        public string BigURL
        {
            get { return m_strBigURL; }
            set { m_strBigURL = value; }
        }

        // 작은 해상도 및 FPS의 URL
        public string SmallURL
        {
            get { return m_strSmallURL; }
            set { m_strSmallURL = value; }
        }

        private int m_bReversPtz = 0;
        public int ReversePTZ
        {
            get { return m_bReversPtz; }
            set { m_bReversPtz = value; }
        }

        public bool EnableDoubleClickEvent()
        {
            if (CCTVType == (int)UnE.Control.CCTVTypes.IDIS_NVR ||
                CCTVType == (int)UnE.Control.CCTVTypes.ITX_NVR ||
                CCTVType == (int)UnE.Control.CCTVTypes.None ||
                CCTVType == (int)UnE.Control.CCTVTypes.NotSet)
                return false;

            return true;
        }
    }

    public class CCTVLoader
    {
        private int m_nSiteID = 2;

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private DBUtility2.WebDBManager m_dbMgr = null;
        public DBUtility2.WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        public CCTVLoader(int nSiteID)
        {
            m_nSiteID = nSiteID;

            m_dbMgr = new DBUtility2.WebDBManager(m_nSiteID);
            /*if (m_nSiteID == 2)
                m_dbMgr = new WebDBManager("SOP4");
            else if (m_nSiteID == 1)
                m_dbMgr = new WebDBManager("SOP3");*/
        }

        public CCTV LoadCCTV(int nID)
        {
            WebDBManager dbMgr = m_dbMgr;

            //System.Windows.Forms.MessageBox.Show(dbMgr.WebServerURL + " " + dbMgr.DatabaseName);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT cv.ID, cv.CameraName, cv.IPAddr, cv.Port, cv.X, cv.Y, cv.Z, cv.ZoneID, cv.LOD ");
            sb.Append(",cv.HTTPPort, cv.Type,cv.Stream,cv.Channel,cv.UserID,cv.Password,cv.URL  , cv.ReversePTZ, cv.SmallURL, cv.BigURL ");
            sb.Append(" FROM CCTV as cv JOIN Zone as z ON z.ID = cv.ZoneID ");
            sb.AppendFormat(" WHERE z.SiteID = {0} and cv.ID = {1} ORDER BY cv.Id", m_nSiteID, nID);

            string strSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //System.Windows.Forms.MessageBox.Show(strSQL);
            //System.Windows.Forms.MessageBox.Show(""+ (arrResult == null));
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            //System.Windows.Forms.MessageBox.Show("" + nResultCount);
            if( nResultCount == 19)
            {
                //int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strCameraName = WebDBManager.GetStringField(arrResult[1], "");
                string strIPAddr = WebDBManager.GetStringField(arrResult[2], "");
                int nPort = WebDBManager.GetIntField(arrResult[3].ToString(), -1);
                float x = WebDBManager.GetFloatField(arrResult[4].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[5].ToString(), 0.0f);
                float z = WebDBManager.GetFloatField(arrResult[6].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[7].ToString(), -1);
                int nLOD = WebDBManager.GetIntField(arrResult[8].ToString(), -1);

                int nHttpPort =WebDBManager.GetIntField(arrResult[9].ToString(), 80);
                string szType = WebDBManager.GetStringField(arrResult[10].ToString(), "");
                int nStream = WebDBManager.GetIntField(arrResult[11].ToString(), 0);
                int nChannel = WebDBManager.GetIntField(arrResult[12].ToString(), 0);
                string szUserName = WebDBManager.GetStringField(arrResult[13], "guest");
                string szPassword = WebDBManager.GetStringField(arrResult[14], "");
                string szURL = WebDBManager.GetStringField(arrResult[15], "");
                int nReversPtz = WebDBManager.GetIntField(arrResult[16].ToString(), 0);
                string strSmallURL = WebDBManager.GetStringField(arrResult[17], "");
                string strBigURL = WebDBManager.GetStringField(arrResult[18], "");

                int nType = UnE.Control.CCTVCtrl.GetCCTVType(szType);
                //int nType = GetCCTVType(szType);

                CCTV cctv = new CCTV();

                cctv.ID = nID;



                string strSendText = strCameraName;
                string strReceiveText = strCameraName;
                //string strIP = strIPAddr;
                //if( nType == 3)
                //{
                //    byte[] pbSource = Encoding.UTF8.GetBytes(strSendText);
                //    byte[] pbDest = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(51949), pbSource);
                //    pbSource = Encoding.Convert(Encoding.GetEncoding(51949), Encoding.UTF8, pbDest);
                //    char[] psUnicode = UTF8Encoding.UTF8.GetChars(pbSource);
                //    strReceiveText = new string(psUnicode);
                //}
                

                cctv.AccessKey = strReceiveText;
                cctv.IPAddress = strIPAddr;
                cctv.PortNo = (short)nPort;  
                cctv.Channel = nChannel;
                cctv.Stream = nStream;
                cctv.UserName = szUserName;
                cctv.Password = szPassword;
                cctv.CCTVType = nType;
                cctv.ReversePTZ = nReversPtz;
                cctv.URL = szURL;
                cctv.SmallURL = strSmallURL;
                cctv.BigURL = strBigURL;

                //System.Windows.Forms.MessageBox.Show("" + cctv.CCTVType);

                return cctv;
            }
            return null;
        }

        public CCTV LoadCCTV(string strFilePath)
        {
            int nLineIndex = 0;
            VariousData<int> id = null, port = null, zoneID = null, isIndoor = null, lod = null, stream = null, channel = null, reversePTZ = null;
            VariousData<float> x = null, y = null, z = null;
            string strCameraName = null, strIPAddr = null, strDescription = null, strPositionName = null, strType = null, strUserID = null, strPassword = null;
            string strURL = null, strBigURL = null, strSmallURL = null;

            StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (nLineIndex == 0)
                    id = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 1)
                    strCameraName = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 2)
                    strIPAddr = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 3)
                    port = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 4)
                    strPositionName = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 5)
                    x = WebDBManager.GetFloatField(strLine);
                else if (nLineIndex == 6)
                    y = WebDBManager.GetFloatField(strLine);
                else if (nLineIndex == 7)
                    z = WebDBManager.GetFloatField(strLine);
                else if (nLineIndex == 8)
                    zoneID = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 9)
                    isIndoor = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 10)
                    lod = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 11)
                    strDescription = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 13)
                    strType = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 14)
                    stream = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 15)
                    channel = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 16)
                    strUserID = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 17)
                    strPassword = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 18)
                    strURL = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 19)
                    reversePTZ = WebDBManager.GetIntField(strLine);
                else if (nLineIndex == 20)
                    strBigURL = WebDBManager.GetStringField(strLine);
                else if (nLineIndex == 21)
                    strSmallURL = WebDBManager.GetStringField(strLine);

                nLineIndex++;
            }

            reader.Close();
            File.Delete(strFilePath);

            if (id == null || strIPAddr == null || port == null || x == null || y == null || z == null || zoneID == null || isIndoor == null || lod == null)
                return null;

            int nType = UnE.Control.CCTVCtrl.GetCCTVType(strType);

            CCTV cctv = new CCTV();

            cctv.ID = id.Data;
            cctv.AccessKey = strCameraName;
            cctv.IPAddress = strIPAddr;
            cctv.PortNo = (short)port.Data;
            cctv.Channel = channel == null ? 0 : channel.Data;
            cctv.Stream = stream == null ? 0 : stream.Data;
            cctv.UserName = strUserID == null ? "guest" : strUserID;
            cctv.Password = strPassword == null ? "" : strPassword;
            cctv.CCTVType = nType;
            cctv.ReversePTZ = reversePTZ == null ? 0 : reversePTZ.Data;
            cctv.URL = strURL == null ? "" : strURL;
            cctv.SmallURL = strSmallURL == null ? "" : strSmallURL;
            cctv.BigURL = strBigURL == null ? "" : strBigURL;

            return cctv;
        }

        public String GetDefaultPreset(int pEquipZone, int pPositionIndex)
        {
            WebDBManager dbMgr = m_dbMgr;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PRESET" + CCTVLoader.ChangeEquipZoneIndex(pPositionIndex) + " ");
            sb.Append("FROM EquipZoneCCTV ");
            sb.Append("WHERE EquipZoneID = " + pEquipZone.ToString());
            
            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());            
            if (arrResult == null) return "";

            int nResultCount = arrResult.Count;
            if (nResultCount == 1)
            {                
                return WebDBManager.GetStringField(arrResult[0], "");
            }

            return "";
        }

        public static int ChangeEquipZoneIndex(int pPositionIndex)
        {
            if (pPositionIndex == 0) return 5;
            else if (pPositionIndex == 1) return 1;
            else if (pPositionIndex == 2) return 4;
            else if (pPositionIndex == 3) return 6;
            else if (pPositionIndex == 4) return 2;
            else if (pPositionIndex == 5) return 3;
            else return -1;
        }

        /*private int GetCCTVType(string szType)
        {
            if (szType == "Axis")
                return 1;
            else if (szType == "NVS")
                return 2;
            else if (szType == "XpressStrm")
                return 3;
            else if (szType == "UDP")
                return 4;
            else if (szType == "Panasonic")
                return 5;
            else if (szType == "iPolis")
                return 6;
            else if (szType == "IPVideo")
                return 7;
            else if (szType == "HIK")
                return 8;
            else if (szType == "NVT")
                return 9;
            else if (szType == "MediaPlayer")
                return 10;
            else if (szType == "IDIS")
                return 11;
            else if (szType == "RTSP")
                return 12;

            return 0;
        }*/
    }
}
