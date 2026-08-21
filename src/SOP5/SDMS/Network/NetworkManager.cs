using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TcpLib2;
using UnE.SOP;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
    public class NetworkManager
    {
        //private log4net.ILog logger = null;
        private object m_LockObj = new object();

        public object Lock
        {
            get { return m_LockObj; }
            set { m_LockObj = value; }
        }

        // 네트웍 분산을 위해 두개로 설정함.
        private ClientProvider m_provider = null;       // 주로 짧은 데이터 사용
        private ClientProvider m_providerSub = null;    // 주로 긴 데이터 사용
        private ClientProviderInternal m_providerInternal = null;

        private int m_nPort = -1;
        private string m_strServerAddr = "";
        public string ServerIP
        {
            get { return m_strServerAddr; }
            set { m_strServerAddr = value; }
        }
        //private bool m_isConnected = false;
        private bool shutdownThread = false;

        private static NetworkManager m_manager = null;

        public static NetworkManager Instance
        {
            get
            {
                if (m_manager == null)
                    m_manager = new SDMS.NetworkManager();
                return m_manager;
            }
        }

        public ClientProvider ClientProvider
        {
            get { return m_provider; }
        }

        public ClientProvider ClientProviderSub
        {
            get { return m_providerSub; }
        }

        public ClientProviderInternal ClientProviderInternal
        {
            get { return m_providerInternal; }
        }

        // 초기화가 완료되기 전까지 SOP Server에는 접속하지 않는다.
        private bool m_waitSOPServer = true;
        public bool WaitForSOPServer
        {
            get { return m_waitSOPServer; }
            set { m_waitSOPServer = false; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

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

        public void RecvLog(byte[] bytes, int nLine)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                int nHeader = BitConverter.ToInt16(bytes, 0);
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), SDMS({2})", (int)nHeader, (int)bytes.Length, nLine);
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

        public int Send(byte[] bytes)
        {
            if (m_provider == null)
                return -1;

            return Send(bytes, m_provider, m_provider.ProviderNum);
        }

        public int Send(byte[] bytes, ClientServiceProvider provider, int nNum)
        {
            lock (this)
            {
                int nResult = provider.Send(bytes, 0, bytes.Length);
                if (nResult > 0)
                {
                    if (!IsLogOpened)
                        return nResult;

                    if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                    {
                        //int nNum = provider.ProviderNum;

                        string szRemotePort = "";
                        try
                        {
                            szRemotePort = provider.Client.Client.LocalEndPoint.ToString();
                        }
                        catch (System.Exception)
                        {
                        }
                        string strLog = string.Format("SendMessage : {0} Header({1}), Length({2}), SDMS({3}) ",
                            szRemotePort, (int)bytes[0], (int)bytes.Length, nNum);
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
        }

        private int m_nSiteID = 1;
        protected NetworkManager()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            InitLog();

            //string strPort = FormMain.Instance.DBManager.LoadIni("sdms_port", "Server Connection Info");
            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if (strServerURL == null || strServerURL == "")
                strServerURL = FormMain.Instance.DBManager.WebServerURL;

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

            if (FormMain.Instance.SimulationMode)
                m_strServerAddr = "127.0.0.1";
            else
                m_strServerAddr = addr[0].ToString();

            //m_strServerAddr = "127.0.0.1";

            m_provider = new ClientProvider(this, 1);
            m_providerSub = new ClientProvider(this, 2);
            m_providerInternal = new ClientProviderInternal(this);

            Thread t;

            //if (!FormMain.Instance.SimulationMode)
            {
                t = new Thread(ConnectionThread);
                t.Name = "SDMS.Connection";
                t.Start();
            }

            // 시간이 경과한 로그 삭제
            t = new Thread(DeleteLog);
            t.Name = "SDMS.LogDelete";
            t.Start();
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLog()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "SDMSClient.log-";
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

        private int m_nServerPort = 0;

        private int GetServerPort()
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select Port from SDMSServerPort WHERE SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            m_nServerPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return m_nServerPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;

            while (!shutdownThread)
            {
                lock (this)
                {
                    if (WaitForSOPServer == false)
                    {
                        if (m_provider.IsConnected)
                        {
                            if (m_provider.PingCount > 5)
                            {
                                m_provider.PingCount = 0;

                                try
                                {
                                    m_provider.Close();
                                }
                                catch (System.Exception)
                                {
                                }
                            }

                            // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                            else if (m_provider.IsReadingProcess)
                            {
                                m_provider.SendData(TCP_ID.I_AM_HERE);
                            }
                            else
                                m_provider.PingCount++;
                        }

                        if (!m_provider.IsConnected)
                        {
                            m_nPort = GetServerPort();

                            try
                            {
                                if (m_nPort > 0)
                                    m_provider.Connect(m_strServerAddr, m_nPort);
                            }
                            catch (System.Exception)
                            {
                            }
                        }

                        Thread.Sleep(300);

                        if (m_providerSub.IsConnected)
                        {
                            if (m_providerSub.PingCount > 5)
                            {
                                WriteLineLog("Ping Sub Close!!" + m_providerSub.PingCount);
                                m_providerSub.PingCount = 0;

                                try
                                {
                                    m_providerSub.Close();
                                }
                                catch (System.Exception)
                                {
                                }
                            }

                            // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                            else if (m_providerSub.IsReadingProcess)
                            {
                                m_providerSub.SendData(TCP_ID.I_AM_HERE);
                            }
                            else
                                m_providerSub.PingCount++;
                        }

                        if (!m_providerSub.IsConnected)
                        {
                            m_nPort = GetServerPort();

                            try
                            {
                                if (m_nPort > 0)
                                    m_providerSub.Connect(m_strServerAddr, m_nPort);
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                    }

                    Thread.Sleep(300);

                    if (m_providerInternal.IsConnected)
                    {
                        if (m_providerInternal.PingCount > 5)
                        {
                            WriteLineLog("Ping Internal Close!!" + m_providerInternal.PingCount);
                            m_providerInternal.PingCount = 0;

                            try
                            {
                                m_providerInternal.Close();
                            }
                            catch (System.Exception)
                            {
                            }
                        }

                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_providerInternal.IsReadingProcess)
                        {
                            m_providerInternal.SendData(TCP_ID.I_AM_HERE);
                        }
                        else
                            m_providerInternal.PingCount++;
                    }

                    if (!m_providerInternal.IsConnected)
                    {
                        int nPort = IntegratedManagement3.InternalMessage.GetInternalServerPort(FormMain.Instance.DBManager, m_nSiteID);

                        try
                        {
                            if (nPort > 0)
                                m_providerInternal.Connect("127.0.0.1", nPort);
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }
                Thread.Sleep(400);

                // 날짜가 경과하면 한달이 지난 로그를 삭제한다.
                if (DateTime.Now.Day != dtPrev.Day)
                    DeleteLog();

                // 20초에 한번씩 진행중인 화재들에 대한 유효성 검사를 실시한다.
                CheckValidProcess(ref dtPrev);
            }
        }

        private void CheckValidProcess(ref DateTime dtPrev)
        {
            DateTime dtCurrent = DateTime.Now;
            TimeSpan span = dtCurrent - dtPrev;

            if (span.TotalSeconds >= 40.0)
            {
                try
                {
                    m_provider.CheckValidProcess();
                }
                catch (System.Exception ex)
                {
                    ConnectionLogEx.Instance.WriteLine(ex);
                }

                dtPrev = dtCurrent;
            }
        }

        public void OnDropConnection(int nNum)
        {           
        }

        public bool SendMessage(int nNum, short header, float data)
        {
            if (m_provider == null)
                return false;

            lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);

                if (nNum == 1)
                {
                    m_provider.SendData((short)header, TCP_TYPE.FLOAT, datas);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, TCP_TYPE.FLOAT, datas);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, string data)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                UTF8Encoding enc = new UTF8Encoding();
                byte[] datas = enc.GetBytes(data);
                if (nNum == 1)
                {
                    m_provider.SendData((short)header, TCP_TYPE.STRING, datas);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, TCP_TYPE.STRING, datas);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, float x, float y, float z)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                List<KeyValuePair<byte, byte[]>> list = new List<KeyValuePair<byte, byte[]>>();

                KeyValuePair<byte, byte[]> pair1 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data1));

                list.Add(pair1);

                KeyValuePair<byte, byte[]> pair2 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data2));

                list.Add(pair2);

                KeyValuePair<byte, byte[]> pair3 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data3));
                list.Add(pair3);

                KeyValuePair<byte, byte[]> pair4 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.FLOAT, BitConverter.GetBytes(x));

                list.Add(pair4);

                KeyValuePair<byte, byte[]> pair5 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.FLOAT, BitConverter.GetBytes(y));

                list.Add(pair5);

                KeyValuePair<byte, byte[]> pair6 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.FLOAT, BitConverter.GetBytes(z));

                list.Add(pair6);

                if (nNum == 1)
                {
                    m_provider.SendData((short)header, list);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, list);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, int data4)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                List<KeyValuePair<byte, byte[]>> list = new List<KeyValuePair<byte, byte[]>>();

                KeyValuePair<byte, byte[]> pair1 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data1));

                list.Add(pair1);

                KeyValuePair<byte, byte[]> pair2 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data2));

                list.Add(pair2);

                KeyValuePair<byte, byte[]> pair3 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data3));

                list.Add(pair3);

                KeyValuePair<byte, byte[]> pair4 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data4));

                list.Add(pair4);

                if (nNum == 1)
                {
                    m_provider.SendData((byte)header, list);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((byte)header, list);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                List<KeyValuePair<byte, byte[]>> list = new List<KeyValuePair<byte, byte[]>>();

                KeyValuePair<byte, byte[]> pair1 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data1));

                list.Add(pair1);

                KeyValuePair<byte, byte[]> pair2 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data2));

                list.Add(pair2);

                KeyValuePair<byte, byte[]> pair3 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data3));

                list.Add(pair3);

                if (nNum == 1)
                {
                    m_provider.SendData((short)header, list);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, list);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, string strData)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                List<KeyValuePair<byte, byte[]>> list = new List<KeyValuePair<byte, byte[]>>();

                KeyValuePair<byte, byte[]> pair1 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data1));

                list.Add(pair1);

                KeyValuePair<byte, byte[]> pair2 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data2));

                list.Add(pair2);

                KeyValuePair<byte, byte[]> pair3 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data3));

                list.Add(pair3);

                KeyValuePair<byte, byte[]> pair4 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.STRING, TcpLib2.TcpHelper.MakeBytes(strData));

                list.Add(pair4);

                if (nNum == 1)
                {
                    m_provider.SendData((short)header, list);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, list);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                List<KeyValuePair<byte, byte[]>> list = new List<KeyValuePair<byte, byte[]>>();

                KeyValuePair<byte, byte[]> pair1 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data1));

                list.Add(pair1);

                KeyValuePair<byte, byte[]> pair2 =
                    new KeyValuePair<byte, byte[]>(TCP_TYPE.INTEGER, BitConverter.GetBytes(data2));

                list.Add(pair2);

                if (nNum == 1)
                {
                    m_provider.SendData((short)header, list);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, list);
                }
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data)
        {
            if (m_provider == null || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                byte[] datas = BitConverter.GetBytes(data);
                if (nNum == 1)
                {
                    m_provider.SendData((short)header, TCP_TYPE.INTEGER, datas);
                }
                else if (nNum == 2)
                {
                    m_providerSub.SendData((short)header, TCP_TYPE.INTEGER, datas);
                }
            }
            return true;
        }

        public void SendChangeEquipZoneCCTV(int nEquipZoneID)
        {
            if (m_provider == null)
                return;

            m_provider.SendChangedConfig(TCP_CLIENT.SDMS_CLIENT, SOP.SDMSConfig.GetPropertyName(SOP.SDMSConfig.ConfigType.EQUIPZONE_CCTV), nEquipZoneID.ToString());
            //SendMessage(1, TCP_ID.CHANGE_EQUIPZONE_CCTV, nEquipZoneID);
        }

        public void SendUpdateFacilityZone(ISensor sz, int nOrgEquipZoneID)
        {
            if (m_provider == null)
                return;
            List<SensorZoneUpdateData> arDatas = new List<SensorZoneUpdateData>();
            arDatas.Add(new SensorZoneUpdateData(sz, nOrgEquipZoneID, sz.EquipZoneID));
            m_provider.SendSensorZoneListInEquipZone(arDatas);

        }

        public void SendRequestReactionLogList()
        {
            if (m_provider == null)
                return;

            m_provider.SendRequestDataList();
        }

        public void SendChangeFacilityManager()
        {
            if (m_provider == null)
                return;

            int nChangedConfig = (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER | (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER | (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER;
            m_provider.SendChangedConfig(TCP_CLIENT.SDMS_CLIENT, SOP.SDMSConfig.PropertyName, nChangedConfig.ToString());
            /*lock (this)
            {
                m_provider.SendData(TCP_ID.CHANGE_FACILITY_MANAGER);
            }*/
        }

        public bool SendRequestRestore()
        {
            if (m_provider == null)
                return false;

            if (m_provider.IsClientDisposed == true || m_provider.IsConnected == false)
                return false;

            lock (this)
            {
                m_provider.SendData(TCP_ID.REQUEST_RESTORE);
            }

            return true;
        }

        public void SendPSMSensorStatus(int nSensorID, byte status, long beginWorkTime, long endWorkTime)
        {
            if (m_provider == null)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SDMSCommandType.CHANGE_PSM_SENSOR_STATUS);
            arrDatas.Add(nSensorID);
            arrDatas.Add(status);
            arrDatas.Add(beginWorkTime);
            arrDatas.Add(endWorkTime);
            arrDatas.Add(FormMain.Instance.SOPGenUserID);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);

            if (m_provider.IsClientDisposed == false)
                this.Send(bytes, m_provider, m_provider.ProviderNum);
        }

        public void SendRefreshSensorLifeTime()
        {
            if (m_provider == null)
                return;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SDMSCommandType.REFRESH_PSM_SENSOR_LIFE_TIME);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);

            if (m_provider.IsClientDisposed == false)
                this.Send(bytes, m_provider, m_provider.ProviderNum);
        }

        public void SendPSMSensorAlarmLevel(UnE.PSM.PSMSensor sensor, float fLevel1, float fLevel2, float fLevel3)
        {
            if (m_provider == null)
                return;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SDMSCommandType.PSM_SENSOR_ALARM_LEVEL);
            arrDatas.Add(sensor.ID);
            arrDatas.Add(fLevel1);
            arrDatas.Add(fLevel2);
            arrDatas.Add(fLevel3);

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);

            if (m_provider.IsClientDisposed == false)
                this.Send(bytes, m_provider, m_provider.ProviderNum);
        }

        /*
         * by hypark
         */
        ClientProvider.changeAction historyAction = null;
        
        public void SendTagChangeDeactivationInfo(Dictionary<int, string> dicCangedData, System.Action callback)     
        {
            if (m_provider == null)
                return;

            ArrayList arrDatas = new ArrayList();
            int lengthOfDic = dicCangedData.Count;
            arrDatas.Add(SDMSCommandType.CHANGE_TAG_ACTIVATION);
            arrDatas.Add(lengthOfDic*2);
            foreach (KeyValuePair<int, string> pair in dicCangedData)
            {
                arrDatas.Add(pair.Key);
                arrDatas.Add(pair.Value);
            }

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);
            historyAction = new ClientProvider.changeAction(callback);
            m_provider.Change += historyAction;

            if (m_provider.IsClientDisposed == false)
                this.Send(bytes, m_provider, m_provider.ProviderNum);
        }

        public void removeChangeEventCallback()
        {
            if (historyAction != null)
            {
                m_provider.Change -= historyAction;
                historyAction = null;
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