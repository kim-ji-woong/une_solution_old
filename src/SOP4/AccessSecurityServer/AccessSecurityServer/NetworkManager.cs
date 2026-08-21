using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TcpLib2;
using SDMS;
using System.Threading;
using DBUtility;
using System.Data.SqlClient;

namespace AccessSecurityServer
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        private bool shutdownThread = false;

        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private const string APP_NAME = "AccessEventReceiver";
        //private SqlConnection m_connectionS1Access = null;
        private string m_strS1AccessConnection = "";
        private LocationManagerOwner m_locationManagerOwner = null;

        private static NetworkManager m_manager = null;
        public static NetworkManager Instance
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NetworkManager();
                return m_manager;
            }
        }

        public ClientProvider ClientProvier
        {
            get { return m_provider; }
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

        public void RecvLog(byte[] bytes)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), " + APP_NAME, (int)bytes[0], (int)bytes.Length);
                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);
            }
        }

        public int Send(byte[] bytes, ClientProvider provider)
        {
            if (provider.IsClientDisposed == true)
                return -1;

            if (provider.IsConnected == false)
            {
                Thread.Sleep(1000);
                if (provider.IsConnected == false)
                    return -1;
            }

            int nResult = provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                if (!IsLogOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), " + APP_NAME, (int)bytes[0], (int)bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);
                }
            }

            return nResult;
        }

        protected NetworkManager()
        {
            InitLog();

            if (ReadSiteID())
                m_dbMgr = new WebDBManager(m_nSiteID);

            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if (strServerURL == null || strServerURL == "")
                strServerURL = "http://127.0.0.1:8080/SOP";

            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

            m_provider = new ClientProvider(this);
            m_strServerAddr = addr[0].ToString();

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();

            // 시간이 경과한 로그 삭제
            /*t = new Thread(DeleteLog);
            t.Name = "LogDelete";
            t.Start();*/
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
            DBUtility.Utility util = new DBUtility.Utility();
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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;
            bool firstCheckLocation = true;

            while (!shutdownThread)
            {
                lock (this)
                {
                    //if (m_isConnected)
                    if (m_provider.IsConnected)
                    {
                        if (m_provider.PingCount > 5)
                        {
                            //m_isConnected = false;
                            m_provider.PingCount = 0;

                            try
                            {
                                ConnectionLogEx.Instance.WriteLine("PING COUNT EXCEPTION");
                                m_provider.Close();
                            }
                            catch (System.Exception)
                            {

                            }

                        }
                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_provider.IsReadingProcess)
                            m_provider.SendData(TCP_ID.I_AM_HERE);
                        else
                            m_provider.PingCount++;
                    }

                    //if (!m_isConnected)
                    if (!m_provider.IsConnected)
                    {
                        m_nPort = GetServerPort();
                        try
                        {
                            if (m_nPort > 0)
                            {
                                /*m_isConnected = */
                                m_provider.Connect(m_strServerAddr, m_nPort);
                            }
                        }
                        catch (System.Exception)
                        {

                        }

                    }
                }
                Thread.Sleep(1000);

                // 날짜가 경과하면 한달이 지난 로그를 삭제한다.
                /*if (DateTime.Now.Day != dtPrev.Day)
                    DeleteLog();*/

                dtPrev = DateTime.Now;

                // 한시간에 한번씩 S1 DB를 감시하여 영역이름에 변경이 있는지 검사한다.
                if (m_strS1AccessConnection.Length > 0 && (firstCheckLocation || (dtPrev.Minute == 0 && dtPrev.Second == 0)))
                {
                    LocationManager.Instance.CheckLocation(m_strS1AccessConnection, m_dbMgr, m_locationManagerOwner);
                    firstCheckLocation = false;
                }
            }
        }

        public void OnDropConnection()
        {
            lock (this)
            {
                //m_isConnected = false;
                m_provider = new ClientProvider(this);
            }
        }

        public bool SendMessage(short header)
        {
            if (m_provider == null)
                return false;

            //lock (this)
            {
                m_provider.SendData(header);
            }
            return true;
        }

        public bool SendMessage(short header, float data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);
                m_provider.SendData(header, TCP_TYPE.INTEGER, datas);
            }
            return true;
        }

        public bool SendMessage(short header, string data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                UTF8Encoding enc = new UTF8Encoding();
                byte[] datas = enc.GetBytes(data);
                m_provider.SendData((short)header, TCP_TYPE.STRING, datas);
            }
            return true;
        }

        public bool SendMessage(short header, int data)
        {
            if (m_provider != null)
                return false;

            //lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);
                m_provider.SendData((short)header, TCP_TYPE.INTEGER, datas);
            }

            return true;
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

            int nData = 1;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SENSOR_DATA, arrDatas);
            Send(bytes, m_provider);
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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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
                int nMaxID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                if (nTagID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nTagID + "," + nTagType + ",'" + szDate + "'," + nData + "," + nSensorType + "," + m_nSiteID + ")";
      
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get
            {
                return (ConnectionLogEx)m_instance;
            }
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
    }
}
