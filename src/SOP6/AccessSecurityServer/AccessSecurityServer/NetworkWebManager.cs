using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TcpLib2;
using System.Threading;
using DBUtility2;
using System.Data.SqlClient;
using SOPWebClient;
using System.IO;

namespace AccessSecurityServer
{
    public class NetworkWebManager
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private bool m_isPSM = false;
            private DateTime m_dtLastSendMessage = new DateTime();
            private bool m_checkLog = false;

            public PostBox PostBox
            {
                get { return m_postBox; }
                set { m_postBox = value; }
            }

            public int ClientType
            {
                get { return m_nClientType; }
            }

            public int ClientSubType
            {
                get { return m_nClientSubType; }
            }

            public bool IsConnected
            {
                get { return m_isConnected; }
                set { m_isConnected = value; }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public bool PSM
            {
                get { return m_isPSM; }
                set { m_isPSM = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public bool CheckLog
            {
                get { return m_checkLog; }
                set { m_checkLog = value; }
            }

            public PostMan(NetworkWebManager owner, int nClientType, int nClientSubType)
            {
                m_owner = owner;
                m_nClientType = nClientType;
                m_nClientSubType = nClientSubType;
            }

            public void OnMessage(int header, byte[] messages)
            {
                if (m_owner != null)
                    m_owner.OnMessage(header, messages, this);
            }

            public bool SendMessage(int header, byte[] messages)
            {
                if (m_postBox == null || m_isConnected == false)
                {
                    //m_postBox = null;
                    m_isConnected = false;

                    //if (m_owner != null)
                    //    m_owner.Connect(this);
                }
                else
                {
                    bool closeConnection;
                    bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                    if (closeConnection)
                    {
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        //m_postBox = null;
                        m_isConnected = false;

                        //if (m_owner != null)
                        //    m_owner.Connect(this);
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManFire = null;
        private PostMan m_postManSecurity = null;
        
        private string m_strServerAddr = "";
        private bool m_shutdownThread = false;

        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private const string APP_NAME = "AccessEventReceiver";
        //private SqlConnection m_connectionS1Access = null;
        private string m_strS1AccessConnection = "";
        private LocationManagerOwner m_locationManagerOwner = null;

        private static NetworkWebManager m_manager = null;
        public static NetworkWebManager Instance
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NetworkWebManager();
                return m_manager;
            }
        }
        
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public string AccessDBConnectionString
        {
            get { return m_strS1AccessConnection; }
            set { m_strS1AccessConnection = value; }
        }
        /* public SqlConnection AccessDBConnection
        {
            get { return m_connectionS1Access; }
            set { m_connectionS1Access = value; }
        }*/

        public AccessSecurityServer.LocationManagerOwner LocationManagerOwner
        {
            get { return m_locationManagerOwner; }
            set { m_locationManagerOwner = value; }
        }

        private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }
        
        protected NetworkWebManager()
        {
            InitLog();

            if (ReadSiteID())
                m_dbMgr = new WebDBManager(m_nSiteID);

            int nPort = ReadServerPort(m_dbMgr);

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.S1_ACCESS);
            m_postManSecurity = new PostMan(this, SOPWebServer.ClientType.SECURITY_SENSOR_SERVER, SOPWebServer.ClientSubType.S1_ACCESS);

            // Log 관리는 한곳에서만 한다.
            m_postManFire.CheckLog = true;

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManSecurity, nPort);

            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postManFire);

            Thread t2 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t2.Start(m_postManSecurity);
        }

        private int ReadServerPort(WebDBManager dbMgr)
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                PostBox postBox = new PostBox();
                postBox.WebServerURL = m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
            }
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;
        }

        private bool ReadSiteID()
        {
            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                ConnectionLogEx.Instance.WriteLine("Site ID가 지정되지 않았습니다. ini파일을 확인하세요");
                //UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                ConnectionLogEx.Instance.WriteLine("잘못된 Site ID입니다. ini파일을 확인하세요");
                //UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLog()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = APP_NAME + ".log-";
                int len = strKey.Length;

                DateTime dtNow = DateTime.Now;
                int nYear, nMonth, nDay;

                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey);

                    if (nIndex < 0)
                        continue;

                    string strDate = strFile.Substring(nIndex + len);

                    int nIndex1 = strDate.IndexOf('-');
                    int nIndex2 = strDate.LastIndexOf('-');

                    if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                        continue;

                    string strYear = strDate.Substring(0, nIndex1);
                    string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDay = strDate.Substring(nIndex2 + 1);

                    if (!int.TryParse(strYear, out nYear))
                        continue;
                    if (!int.TryParse(strMonth, out nMonth))
                        continue;
                    if (!int.TryParse(strDay, out nDay))
                        continue;

                    if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                        System.IO.File.Delete(strFile);
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread(object arg)
        {
            DateTime dtPrev = DateTime.Now;
            bool firstCheckLocation = true;

            PostMan postMan = (PostMan)arg;

            while (!m_shutdownThread)
            {
                if (postMan.IsConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

                    if (postMan.Port != nPort)
                        SetPostBox(postMan, nPort);

                    if (postMan.PostBox != null)
                    {
                        if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                        {
                            postMan.IsConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - postMan.LastSendMessageTime;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        postMan.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);

                // 날짜가 경과하면 한달이 지난 로그를 삭제한다.
                /*if (DateTime.Now.Day != dtPrev.Day)
                    DeleteLog();*/

                dtPrev = DateTime.Now;

                if (postMan.CheckLog)
                {
                    // 한시간에 한번씩 S1 DB를 감시하여 영역이름에 변경이 있는지 검사한다.
                    if (m_strS1AccessConnection.Length > 0 && (firstCheckLocation || (dtPrev.Minute == 0 && dtPrev.Second == 0)))
                    {
                        LocationManager.Instance.CheckLocation(m_strS1AccessConnection, m_dbMgr, m_locationManagerOwner);
                        firstCheckLocation = false;
                    }
                }
            }
        }
        
        // 알람발생
        public bool SendAlarm(Alarm alarm)
        {
            if (alarm.Device == null)
                return false;

            if (alarm.Device.Location == null)
                return false;

            int nSensorType = (int)alarm.AlarmState;
            int nSensorZoneID = GetAccessSensorZoneID(alarm.Device);

            int nTagSensorType = -1;
            int nSensorTagID = GetAccessSensorTagID(nSensorZoneID, out nTagSensorType);
            if( nSensorTagID > 0)
            {
                SaveTagHistory(0x92, nSensorTagID, nTagSensorType, nSensorZoneID);
            }

            Facility.FacilityType sensorType = Facility.ToFacilityType(nTagSensorType);

            PostMan postMan = null;
            switch (sensorType)
            {
                case Facility.FacilityType.Fire_S1:
                case Facility.FacilityType.FireF1_S1:
                    postMan = m_postManFire;
                    break;

                case Facility.FacilityType.Intrusion_S1:
                case Facility.FacilityType.Loiter_S1:
                case Facility.FacilityType.Slip_S1:
                case Facility.FacilityType.Steal_S1:
                case Facility.FacilityType.Abandoned_S1:
                case Facility.FacilityType.VirtualFence_S1:
                case Facility.FacilityType.EmergencyBell_S1:
                case Facility.FacilityType.GeneralIntrusionT1_S1:
                case Facility.FacilityType.GeneralIntrusionT2_S1:
                case Facility.FacilityType.InternalIntrusionT3_S1:
                case Facility.FacilityType.VaultIntrusionT4_S1:
                case Facility.FacilityType.CustomerEmergencyC1_S1:
                case Facility.FacilityType.CustomerEmergencyC2_S1:
                case Facility.FacilityType.RescueQQ_S1:
                case Facility.FacilityType.GasG1_S1:
                case Facility.FacilityType.BlackoutAbnormalityU1_S1:
                case Facility.FacilityType.LeakAbnormalityU4_S1:
                case Facility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    postMan = m_postManSecurity;
                    break;
            }

            if (postMan == null)
                return false;

            if (!postMan.IsConnected)
                return false;

            int nData = 1;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            postMan.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
            return true;
        }

        // 알람해제
        public bool SendAlarmClear(Device device)
        {
            // Access가 보내오는 alarm Off는 처리하지 않는다.
            return true;
            /*if (device == null)
                return false;

            if (device.Location == null)
                return false;

            int nSensorType = (int)device.AlarmState;
            int nSensorZoneID = GetAccessSensorZoneID(device);

            int nTagSensorType = -1;
            int nSensorTagID = GetAccessSensorTagID(nSensorZoneID, out nTagSensorType);
            if (nSensorTagID > 0)
            {
                SaveTagHistory(0x93, nSensorTagID, nTagSensorType, nSensorZoneID);
            }
            int nData = 0;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SENSOR_DATA, arrDatas);
            Send(bytes, m_provider);
            return true;*/
        }

        // Return 값 : device에 연결된 SensorZone ID
        //             존재하지 않으면 -1을 리턴한다.
        private int GetAccessSensorZoneID(Device device)
        {
            string strSQL = "select SensorZone.ID ";
            strSQL += "from AccessLink_View_External_Device as device, S1Access, SensorZone ";
            strSQL += string.Format("where device.S1AccessID = S1Access.ID and S1Access.ID = SensorZone.OrgSensorID and SensorZone.Type = {0} and deviceID = {1}", (int)device.AlarmState, device.ID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[0].ToString());
            return sensorZoneID == null ? -1 : sensorZoneID.Data;
        }

        private int GetAccessSensorTagID(int nSensorID, out int nSensorType)
        {
            nSensorType = -1;

            if (nSensorID < 0)
                return -1;

            string strSQL = "select ID, SensorType ";
            strSQL += "from SensorTagInfo ";
            strSQL += string.Format("where SensorZoneID = {0}", nSensorID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorTagInfo = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());

            nSensorType = sensorType == null ? -1 : sensorType.Data;

            return sensorTagInfo == null ? -1 : sensorTagInfo.Data;
        }

        private void SaveTagHistory(int nHeader, int nTagID, int nSensorType, int nSensorZoneID)
        {
            int nData = 0;
            int nTagType = 0;
            switch (nHeader)
            {
                case 0x87:
                case 0x88:
                case 0x89:
                    nData = 'N';
                    nTagType = 1;
                    break;

                case 0x91: // 전체복구
                    nData = 'R';
                    nTagType = 0;
                    break;
                case 0x92: // 신호발생
                    nData = 'N';
                    nTagType = 1;
                    break;
                case 0x93: // 신호복구
                    nData = 'F';
                    nTagType = 1;
                    break;
                case 0x94: // 장애발생
                    nData = 'E';
                    nTagType = 2;
                    break;
                case 0x95: // 장애복구
                    nData = 'C';
                    nTagType = 2;
                    break;
                case 0x96: // 감시발생
                    nData = 'N';
                    nTagType = 3;
                    break;
                case 0x97: // 감시복구
                    nData = 'F';
                    nTagType = 3;
                    break;
                case 0x98: // 예비경보발생
                    break;
                case 0x99: // 예비경보복구
                    break;
            }

            string szDate = WebDBManager.MakeDateTimeString(DateTime.Now);  
            string szSQL1 = "SELECT max(ID) FROM SensorTagHistory";
            ArrayList arResult = m_dbMgr.GetResultData(szSQL1, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                if (nTagID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nTagID + "," + nTagType + ",'" + szDate + "'," + nData + "," + nSensorType + "," + m_nSiteID + ")";
      
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    m_dbMgr.GetResultData(strSQL);
                }
            }
        }

        public void OnMessage(int header, byte[] messages, IPostMan postMan)
        {
            //PostMan _postMan = (PostMan)postMan;            
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;
        private string m_strChangeLogPath = ".\\logs\\AccessEventReceiverChanged.log";
        private StreamWriter m_changedLogWriter = null;

        public static ConnectionLogEx Instance
        {
            get
            {
                return (ConnectionLogEx)m_instance;
            }
        }

        public string ChangedLogPath
        {
            get { return m_strChangeLogPath; }
        }

        public static bool MakeInstance()
        {
            if (m_instance == null)
                m_instance = new ConnectionLogEx();

            ConnectionLogEx instance = (ConnectionLogEx)m_instance;
            instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            instance.m_isOpened = true;
            return instance.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);

            return true;
        }

        public void ChangeLog(string str, bool writeTime = true)
        {
            if (m_changedLogWriter == null)
            {
                CheckPath(m_strChangeLogPath);
                m_changedLogWriter = new StreamWriter(m_strChangeLogPath, true, Encoding.UTF8);
            }

            if (writeTime)
            {
                DateTime dtNow = DateTime.Now;
                m_changedLogWriter.Write(string.Format("[{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}] : ", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second));
            }

            m_changedLogWriter.WriteLine(str);
            m_changedLogWriter.Flush();
        }

        private void CheckPath(string strNewPath, string strOldPath = null)
        {
            int nIndex1 = strNewPath.IndexOf('\\');
            int nIndex2 = strNewPath.IndexOf('/');

            if (nIndex1 < 0 && nIndex2 < 0)
                return;

            string strPath = "";
            int nIndex = 0;

            if (nIndex1 < 0)
                nIndex = nIndex2;
            else if (nIndex2 < 0)
                nIndex = nIndex1;
            else if (nIndex1 < nIndex2)
                nIndex = nIndex1;
            else// if (nIndex2 < nIndex1)
                nIndex = nIndex2;

            strPath = strNewPath.Substring(0, nIndex);

            if (strPath == "." || strPath == "..")
            {
                if (strOldPath == null)
                    strOldPath = strPath;
                else
                    strOldPath += "\\" + strPath;

                CheckPath(strNewPath.Substring(nIndex + 1), strOldPath);
            }
            else
            {
                string strDirectory = strOldPath == null ? strPath : strOldPath + "\\" + strPath;

                if (Directory.Exists(strDirectory) == false)
                    Directory.CreateDirectory(strDirectory);

                if (strOldPath == null)
                    strOldPath = strPath;
                else
                    strOldPath += "\\" + strPath;

                CheckPath(strNewPath.Substring(nIndex + 1), strOldPath);
            }
        }
    }
}
