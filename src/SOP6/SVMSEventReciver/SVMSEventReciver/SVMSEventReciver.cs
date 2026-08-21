using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using DBUtility2;
using S1SVMSSDKv2.Info;
using S1SVMSSDKv2.Model.Alarm;
using S1SVMSSDKv2.Model.Etc;

namespace SVMSEventReciver
{
    public class SVMSEventReciver : IDisposable
    {
        private ManagementServer managementServer = null;

        private static log4net.ILog logger = null;
        
        private string SVMSManagementServerIPAddress = "127.0.0.1";
        private ushort SVMSManagementServerPortNumber = 8020;
        private string SVMSManagementServerUserName = "unes";
        private string SVMSManagementServerUserPassword = "q1w2e3r4!";
        private int m_nSVMSDBType = 0;
        private string m_strSVMSDBName = "";

        private string _clientGUID;

        private WebDBManager m_dbMgr = null;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }
        /*private DBUtility.LocalDBManager dbManager = null;

        public DBUtility.LocalDBManager DBManager
        {
            get { return dbManager; }
        }*/

        private static SVMSEventReciver m_Instance = null;
        public static SVMSEventReciver Instance
        {
            get { return SVMSEventReciver.m_Instance; }
        }

        private int m_nSiteID = 100;
        public int SiteID
        {
            get { return m_nSiteID; }
        }

        private IOManager ioMgr = null;
        public IOManager IOManager
        {
            get { return ioMgr; }
        }

        private CameraManager cctvMgr = null;
        public CameraManager CCTVManager
        {
            get { return cctvMgr; }
        }

        private NetworkWebClient client = null;
        public NetworkWebClient Client
        {
            get { return client; }
        }

        private bool m_bReciveFireSignal = true;
        public bool ReciveFireSignal
        {
            get { return m_bReciveFireSignal; }
            set { m_bReciveFireSignal = value; }
        }

        private bool m_bReciveFenceSignal = true;
        public bool ReciveFenceSignal
        {
            get { return m_bReciveFenceSignal; }
            set { m_bReciveFenceSignal = value; }
        }

        private Utility m_ini = new Utility();
        private void ReadSVMSInfo()
        {  
			string strSection = "SVMS Connection info";

			SVMSManagementServerIPAddress = m_ini.getinivalue(strSection, "SVMSServer_IP");
			string szPort  = m_ini.getinivalue(strSection, "SVMSServer_Port");

            if(!ushort.TryParse(szPort, out SVMSManagementServerPortNumber))
            {
                SVMSManagementServerPortNumber = 8020;
            }

            SVMSManagementServerUserName = m_ini.getinivalue(strSection, "SVMSServer_User");
            SVMSManagementServerUserPassword = m_ini.getinivalue(strSection, "SVMSServer_Passwd");

            m_strSVMSDBName = m_ini.getinivalue(strSection, "DBName");
            string strDBType = m_ini.getinivalue(strSection, "DBType");

            if (strDBType.Length > 0)
            {
                int.TryParse(strDBType, out m_nSVMSDBType);
            }
        }

        public SVMSEventReciver(int nSiteID)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            m_Instance = this;

            m_nSiteID = nSiteID;
            
            ReadSVMSInfo();

            //string szDBName = "EDU_" + m_nSiteID.ToString();
            m_dbMgr = new WebDBManager(m_nSiteID);
            //dbManager = new DBUtility.LocalDBManager(szDBName, "mysql", m_nSiteID);

            ioMgr = new IOManager(m_nSiteID);
            cctvMgr = new CameraManager(m_nSiteID, m_dbMgr);
            cctvMgr.LoadCCTV();

            client = new NetworkWebClient(m_dbMgr, null, m_nSiteID);            
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.ShutdownThread = true;
            }

            if (m_dbMgr != null)
            {
                //dbManager.Dispose();
            }

            m_bConnectServer = false;

            //managementServer.Cleanup();
        }

        private bool m_bConnectServer = false;
        public bool IsConnect
        {
            get { return m_bConnectServer; }
        }

        public void OnLaunchCallback(string server, bool isSuccess)
        {
            if (isSuccess == true)
            {
                m_bConnectServer = true;
            }
            else
            {
                m_bConnectServer = false;

                // 자동 재접속 이후도 접속종료 하지 못하는 경우 재 접속한다.
                RequestLaunch();
            }
        }

        private void RequestLaunch()
        {
            if (managementServer != null)
                managementServer.Cleanup();
            
             managementServer = new ManagementServer(SVMSManagementServerIPAddress, SVMSManagementServerPortNumber, SVMSManagementServerUserName, SVMSManagementServerUserPassword, false, 1, SVMSClientType.externalclient);

            if (managementServer != null)
                managementServer.Launch(OnLaunchCallback);

            InitializeSVMSResponse();
        }

        public void ConnectServer()
        {
            RequestLaunch();
           
        }

        public void RequestCameraList()
        {
            guidList.Clear();
            if (managementServer != null)
                managementServer.RequestDeviceCameraList();
        }

        private void InitializeSVMSResponse()
        {            
            managementServer.ClientTypeCompleted += new Action<string, bool, string, XmlNode>(SVMSInitClient);
            //managementServer.NetworkConnectionStatusNotified ConnectCompleted += new Action<bool, XmlNode>(SVMSServerConnect);
            
            managementServer.Disconnected += (s) =>
            {
                //m_bConnectServer = false;
            };

            managementServer.Reconnected += (s, b) =>
            {
                RequestCameraList();              
            };

            managementServer.DeviceGroupListCompleted += (arg1, isSuccess, arg2, deviceGroups, originalActionStructure) =>
            {
                if (isSuccess == true)
                {                   
                }
            };
            
            managementServer.DeviceCameraListCompleted += SVMSEventCameraList;

            managementServer.AddDeviceCameraNotified += (arg1, isSuccess, addDeviceCamera, originalActionStructure) =>
            {
                if (isSuccess == true)
                {                    
                    //RequestCameraList();
                    Console.WriteLine("[DeviceCamera] " + addDeviceCamera.CameraGUID + " added.");
                }
            };

            managementServer.ModifyDeviceCameraNotified += (arg1, isSuccess, modifyDeviceCamera, originalActionStructure) =>
            {
                if (isSuccess == true)
                {
                    //RequestCameraList();
                    Console.WriteLine("[DeviceCamera] " + modifyDeviceCamera.CameraGUID + " modified.");
                }
            };

            managementServer.RemoveDeviceCameraNotified += (arg1, isSuccess, deviceCameraGUID, originalActionStructure) =>
            {
                if (isSuccess == true)
                {
                    //RequestCameraList();
                    Console.WriteLine("[DeviceCamera] " + deviceCameraGUID + " removed.");
                }
            };
                                          
            managementServer.SVMSEventNotified += new Action<string, bool, SVMSEventInformation, XmlNode>(this.SVMSEventNotify);

        }


        public void SVMSInitClient(string serverKey, bool isSuccess,string clientGUID, XmlNode originalActionStructure)
        {           
            if (isSuccess == true)
            {              
                _clientGUID = clientGUID;

                guidList.Clear();             
            }
        }

        public void SVMSServerConnect(bool isSuccess, XmlNode originalActionStructure)
        {
            if (isSuccess == true)
            {
                RequestCameraList();
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("connect fail");
            }
        }


        private SortedList<string, string> guidList = new SortedList<string, string>();

        public void SVMSEventCameraList(string arg1, bool isSuccess,bool isFinished, List<S1SVMSSDKv2.Model.Device.DeviceCamera> deviceCameras, XmlNode originalActionStructure)
        {
            if (isSuccess == true)
            {
                if (isFinished != true)
                {
                    foreach (var deviceCameraItem in deviceCameras)
                    {
                        try
                        {
                            string deviceCameraGUID = deviceCameraItem.CameraGUID;                            
                            string cameraIP = deviceCameraItem.CameraIPAddress;
                            if (cameraIP != "")
                            {
                                cameraIP = deviceCameraItem.CameraRTSPURL;
                            }

                            System.Diagnostics.Trace.WriteLine("GUID: " + deviceCameraGUID);
                            System.Diagnostics.Trace.WriteLine("Camera: " + cameraIP);

                            if (deviceCameraGUID != null)
                            {
                                guidList.Add(deviceCameraGUID, cameraIP);
                            }

                            if (string.IsNullOrEmpty(deviceCameraGUID) == false)
                            {
                                //managementServer.GetIntelligentConfigurationInformation(deviceCameraGUID);
                            }

                            Console.WriteLine("[DeviceCamera] " + deviceCameraItem.CameraGUID);
                        }
                        catch(Exception ex)
                        {
                        }                        
                    }
                }
                else
                {
                    Console.WriteLine("[------------] " + "list up completed.");
                }
            }
        }

        delegate void DeviceCameraAddedCallback(ArrayList lvi);

        private void DeviceCameraAdded(/*ArrayList lvi*/)
        {
            RequestCameraList();

            //if (this.ui_livDeviceCamera.InvokeRequired)
            //{
            //    DeviceCameraAddedCallback d = new DeviceCameraAddedCallback(DeviceCameraAdded);
            //    this.Invoke(d, new object[] { lvi });
            //}
            //else
            //{
            //    this.ui_livDeviceCamera.Items.Add(lvi);
            //}
        }

        private int m_nLastEventType = 0;
        private int m_nLastIntelligentEvent = 0;

        public void SVMSEventNotify(string serverKey, bool isSuccess, SVMSEventInformation SVMSEventInformation, XmlNode originalActionStructure)
        {
            if (isSuccess == true)
            {
                logger.Debug("[SVMSEvent (" + SVMSEventInformation.DeviceGUID + ")] type: →" + SVMSEventInformation.AlarmProperty.Type + " " + SVMSEventInformation.DeviceType);
                int nReciverID = -1;
                int nData = 0;
                bool bFire = false;
                int nType = SVMSEventInformation.AlarmProperty.Type;
                // 시스템 상태값
                if (nType >= 1000 && nType <= 1004)
                {
                    switch (nType)
                    {
                        case 1000: // System on
                            break;
                        case 1001: // System off                                 
                            break;
                        case 1002: // CPU Power over
                            break;
                        case 1003: // system network over
                            break;
                        case 1004: // system memory over
                            break;
                    }

                }
                else
                {
                    //IntPtr ptr = new IntPtr(nType);
                    //IntelligentConfigurationInformation alarm = (IntelligentConfigurationInformation)Marshal.PtrToStructure(ptr, typeof(IntelligentConfigurationInformation));
                    //int nAlarmType = alarm.IntelligentAlgorithmType;

                    // 우리가 처리할 항목 침입 / 배회 / 쓰러짐 / 도난 / 방치 / 가상펜스 / 화재 / 비상벨(DIO)
                    
                    switch (nType)
                    {
                        case 0: // Previous Event clear
                            break;
                        case 2: // Intrusion(침입)
                        case 3: // Loitering (배회)
                        case 4: // Slip( 넘어짐 )
                        case 6: // Steal (도난)
                        case 7: // Abandoned( 방치)
                            nReciverID = 2;
                            nData = 1;
                            break;
                        case 8: // Fence (가상펜스)
                            nReciverID = 5;
                            nData = 1;
                            break;
                        case 100: // Fire (화재)
                            if (m_dbMgr.SiteID == 102)
                            {
                                nReciverID = 2;
                            }
                            else
                                nReciverID = 5;
                            bFire = true;
                            nData = 1;
                            break;
                        case 200: // DIO (카메라 DIO 비상벨)
                            break;
                        default:
                            break;
                    }
                    m_nLastIntelligentEvent = nType;
                }

                System.Diagnostics.Trace.WriteLine("[SVMSEvent (" + SVMSEventInformation.DeviceGUID + ")] type: →" + SVMSEventInformation.AlarmProperty.Type + " " + SVMSEventInformation.DeviceType);

                if (bFire == true && m_bReciveFireSignal == false)
                    return;

                if (bFire == false && m_bReciveFenceSignal == false)
                    return;

                int nEventType = nType;
                string szGUID = SVMSEventInformation.DeviceGUID;
                long nTime = SVMSEventInformation.AlarmProperty.Time;
                int nDeviceTypeNumber = SVMSEventInformation.DeviceType;

                string szCameraIP = "";
                if (guidList.ContainsKey(szGUID))
                {
                    szCameraIP = guidList[szGUID];
                }

                CCTV cctv = null;
                cctv = cctvMgr.GetCCTV(szCameraIP);

                if (cctv != null)
                {
                    System.Diagnostics.Trace.WriteLine("CCTV : " + cctv.AccessKey);
                    System.Diagnostics.Trace.WriteLine("CCTV IP " + cctv.IPAddress);
                    int nID = cctv.ID;
                    Reciver reciver = ioMgr.FindReciver(nReciverID);
                    if (reciver != null)
                    {
                        if (reciver.Curcuits.ContainsKey(nID))
                        {
                            Circuit circuit = reciver.Curcuits[nID];
                            if (circuit != null)
                            {
                                int nTagID = SaveTagHistory(circuit, 0x92);
                                
                                client.SendSensorData(circuit, nData, nTagID);

                                if (SVMSEventInformation.DeviceCameraSnapshotInformationList != null)
                                {
                                    if (SVMSEventInformation.DeviceCameraSnapshotInformationList.Count > 0)
                                    {
                                        DeviceCameraSnapshotInformation snapShot = SVMSEventInformation.DeviceCameraSnapshotInformationList[0];
                                        if (nTagID > 0)
                                            GetSnapShotFile(nTagID, snapShot);
                                    }
                                }

                                DateTime timeUtc = DateTime.FromBinary(nTime);
                                TimeZone oTimeZone = TimeZone.CurrentTimeZone;
                                DateTime cstTime = oTimeZone.ToLocalTime(timeUtc);

                                if (SVMSEventInformation.DeviceCameraRecordInformationList != null)
                                {
                                    if (SVMSEventInformation.DeviceCameraRecordInformationList.Count > 0)
                                    {
                                        DeviceCameraRecordInformation snapShot = SVMSEventInformation.DeviceCameraRecordInformationList[0];

                                        string guid = snapShot.DeviceGUID;
                                        string serverip = snapShot.StreamServerRtspIP;
                                        int nPort = snapShot.StreamServerRtspPort;
                                        string szFileName = snapShot.RecordFilePath;
                                        int nRecordIdx = snapShot.RecIndexNum;
                                        long szRecordStart = snapShot.PostRecordTime;
                                        long szRecordEnd = snapShot.PreRecordTime;

                                        System.Diagnostics.Trace.WriteLine("EventType : " + nEventType);
                                        System.Diagnostics.Trace.WriteLine("Camera : " + guid);
                                        System.Diagnostics.Trace.WriteLine("MainSnapShotFile : " + szFileName);
                                        System.Diagnostics.Trace.WriteLine("StreamServer : " + serverip);
                                        System.Diagnostics.Trace.WriteLine("StreamServerPort : " + nPort);
                                        System.Diagnostics.Trace.WriteLine("RecordIndex : " + nRecordIdx);
                                        System.Diagnostics.Trace.WriteLine("RecordPostTime : " + szRecordStart);
                                        System.Diagnostics.Trace.WriteLine("RecordPreTime : " + szRecordEnd);
                                        System.Diagnostics.Trace.WriteLine("EventTime : " + cstTime);


                                        string url = "rtsp://" + serverip + ":" + nPort + "/" + szFileName + "?g=" + _clientGUID + "&t=externalclient";
                                        System.Diagnostics.Trace.WriteLine("URL : " + url);

                                        string szText = "INSERT INTO SVMSEventHistory (ID,EventType,CameraID,CameraGUID,RTSPServerIP,RTSPServerPort,ClientGUID,RecordFilePath, RecordIndex, RecordTime, RTSPUrl) " +
                                            " VALUES ( {0}, {1}, {2}, '{3}', '{4}', {5}, '{6}', '{7}', {8}, '{9}', '{10}')";

                                        string szDate = DBUtility.LocalDBManager.MakeDateTimeString(cstTime);
                                        string szSQL = string.Format(szText, nTagID, nEventType, 1, szGUID, serverip, nPort, _clientGUID, szFileName, nRecordIdx, szDate, url);
                                        m_dbMgr.GetResultData(szSQL);
                                        //m_dbMgr.GetResultData(szSQL, 0);
                                    }
                                }
                                m_nLastEventType = nType;
                            }
                        }
                    }
                }
            }
        }

        private int SaveTagHistory(Circuit circuit, int nHeader)
        {

            if( circuit == null)
            {
                logger.Debug("선택된 회로가 없습니다.");
                return -1;
            }

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

            int nCircuit = circuit.ID;
            int nReciver = circuit.ReciverID;
            string szDate = DBUtility.LocalDBManager.MakeDateTimeString(DateTime.Now);

            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return -1;
            }

            int nType = circuit.SensorType;
            int nID = -1;
            // get max id
            //DBUtility.LocalDBManager dbMgr = DBManager;
            string szSQL1 = "SELECT max(ID) FROM SensorTagHistory";
            ArrayList arResult = m_dbMgr.GetResultData(szSQL1);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = WebDBManager.GetIntField(arResult[0].ToString(), 0);
                nID = nMaxID + 1;              

                if (nCircuit >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nCircuit + "," + nTagType + ",'" + szDate + "'," + nData + "," + nType + "," + m_nSiteID + ")";
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    m_dbMgr.GetResultData(strSQL);
                    return nID;
                }
               
            }
            return -1;
        }

        public void GetSnapShotFile(int nTagHistoryID, DeviceCameraSnapshotInformation snapshotInformation)
        {
            string szCameraGuid = snapshotInformation.DeviceGUID;
            string szFtpServerIP = snapshotInformation.StreamServerIPAddress;
            string szFtpUserID = snapshotInformation.FTPUserID;
            string szFtpPasswd = "router";
            int nFtpPortNo = snapshotInformation.StreamServerDataServerPortNumber;
            SaveSnapShotInfo(nTagHistoryID, snapshotInformation.SnapshotFileName1, szFtpServerIP, nFtpPortNo.ToString(), szFtpUserID, szFtpPasswd, true);        
        }
        
        public void SaveSnapShotInfo(int nTagHistoryID, string szFileName, string serverIP, string serverPort, string userID, string passwd, bool bPassive)
        {
            if (nTagHistoryID < 0)
                return;

            string szConnectURL = "ftp://" + serverIP + ":" + serverPort + "/" + szFileName;
            System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(szConnectURL);
            request.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(userID, passwd);
            request.UseBinary = true;
            request.UsePassive = true;

            string szText = "INSERT INTO SVMSSnapshotHistory (ID,FTPServerIP,FTPServerPort,ServerFileName,FTPUser,FTPPass,IsPassive,IsBinary, Description) " +
                " VALUES ( {0}, '{1}', {2}, '{3}', '{4}', '{5}', {6}, {7}, '{8}')";
            string szSQL = string.Format(szText, nTagHistoryID, serverIP, serverPort, szFileName, userID, passwd, 1, 1, szConnectURL);
            m_dbMgr.GetResultData(szSQL);

#if TEST

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream("c:\\temp\\test.png", FileMode.Create))
                        {
                            byte[] buffer = new byte[102400];
                            int read = 0;
                            do
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);
                                fs.Write(buffer, 0, read);
                                fs.Flush();
                            } while (!(read == 0));

                            fs.Flush();
                            fs.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);

            }           
            
#endif
                
        }
    }
}
