using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Threading;
using System.Collections;

namespace MuxFireSensorServer.Network
{
    using Data;

    public class NetworkWebClient
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebClient m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();

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

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public PostMan(NetworkWebClient owner, int nClientType, int nClientSubType)
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
                    m_isConnected = false;
                }
                else
                {
                    bool closeConnection;
                    bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                    if (closeConnection)
                    {
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManFire = null;
        private DirectDBManagerEx m_dbMgr = null;

        private SOPWebClient.Logger logger = null;
        private bool m_shutdownThread = false;

        private static NetworkWebClient m_instance = null;
        private DateTime m_dtLogDeleteDate = new DateTime();

        private List<int> m_receiverIDs = new List<int>();

        public static NetworkWebClient Instance
        {
            get { return m_instance; }
        }

        public NetworkWebClient(DirectDBManagerEx dbMgr, string strLogFolder, string strLogFile)
        {
            m_instance = this;
            logger = new Logger(strLogFolder, strLogFile + "_2", 30);
            //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_dbMgr = dbMgr;
            int nPort = ReadServerPort(m_dbMgr);

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.JOHNSON_CONTROLS);

            SetPostBox(m_postManFire, nPort);
            ReadSensorServerInfo();

            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postManFire);
        }

        private void ReadSensorServerInfo()
        {
            string strSQL = "Select ID from SensorServerInfo where ReciverType = 1 and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            foreach (object result in arrResult)
            {
                VariousData<int> id = WebDBManager.GetIntField(result.ToString());

                if (id != null)
                    m_receiverIDs.Add(id.Data);
            }
        }

        // 서버로부터 받은 데이터
        public void OnMessage(int header, byte[] messages, IPostMan postMan)
        {
            if (messages == null)
                return;

            PostMan _postMan = (PostMan)postMan;
            System.Diagnostics.Trace.WriteLine("OnMessage : " + header.ToString());

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                _postMan.IsConnected = false;
            }
        }

        private int ReadServerPort(DirectDBManagerEx dbMgr)
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
                string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerURL");

                PostBox postBox = new PostBox();
                postBox.WebServerURL = strWebServerURL;// m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
            }
        }

        private void ConnectionThread(object arg)
        {
            PostMan postMan = (PostMan)arg;

            while (m_shutdownThread == false)
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
                            NetworkWebClient.Instance.SendReceiverState(NetworkManager.Instance.IsConnected);
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

                DateTime dtNow = DateTime.Now;

                // 새벽 한시에 한번 로그 정리를 한다.
                if (dtNow.Hour == 1)
                {
                    if (dtNow.Year != m_dtLogDeleteDate.Year || dtNow.Month != m_dtLogDeleteDate.Month || dtNow.Day != m_dtLogDeleteDate.Day)
                    {
                        m_dtLogDeleteDate = dtNow;
                        RemoveLog(dtNow);
                    }
                }
            }
        }

        public void Close()
        {
            if (m_postManFire.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManFire.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManFire.IsConnected = false;
            }

            m_shutdownThread = true;
        }

        public bool SendSensorData(SensorTag sensor, int nSensorData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(sensor.SensorType);
            arrDatas.Add(sensor.ID);
            arrDatas.Add(sensor.SensorZoneID);
            arrDatas.Add(nSensorData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return m_postManFire.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

        public bool SendAllClear()
        {
            return m_postManFire.SendMessage(SOPWebServer.Header.CLEAR_DETECT_ALL, null);
        }

        public void SendReceiverState(bool isConnected)
        {
            ArrayList arrDatas = new ArrayList();

            foreach (int nReceiverID in m_receiverIDs)
            {
                int nCon = isConnected == true ? 1 : 0;
                int nPol = isConnected == true ? 10 : 0;

                arrDatas.Add(nReceiverID);
                arrDatas.Add(nPol + nCon);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManFire.SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }

        /*public void SendSensorData(int nReciver, int nCircuit, int nChannel, int nData)
        {
            logger.Debug(string.Format("SendSensorData : {0}, {1}, {2}, {3}", nReciver, nCircuit, nChannel, nData));

            int nReciverType = 1;
            
            Reciver reciver = PSMNetworkServer.Instance.IOManager.FindReciverForUnitID(nReciver, nReciverType);
            if (reciver != null)
            {
                Circuit curcuit = null;

                if (reciver.ReciverType == 2 && nCircuit >= 16)
                {
                    if (reciver.Curcuits.ContainsKey(nCircuit + nChannel))
                    {
                        // 가성소다와 같은 누액감지 센서는 알람단계가 1단계밖에 없으므로, Channel번호로 센서 신호를 구분한다.
                        curcuit = reciver.Curcuits[nCircuit + nChannel];
                        nCircuit = nCircuit + nChannel;

                        if (nData > 0)
                            nData = 1;
                    }
                    else if (reciver.Curcuits.ContainsKey(nCircuit))
                    {
                        curcuit = reciver.Curcuits[nCircuit];
                    }
                }
                else
                {
                    if (reciver.Curcuits.ContainsKey(nCircuit))
                    {
                        curcuit = reciver.Curcuits[nCircuit];
                    }
                }

                if (curcuit != null)
                {
                    int nCurcuit = curcuit.TagNum;

                    logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "], Channel : " + nChannel.ToString());

                    //int nEquipzoneID = curcuit.TargetZoneID;
                    int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;

                    int nTagNum = curcuit.TagNum;
                    int nSensorType = curcuit.SensorType;
                    //if(nSensorType == 6 && nSensorType == 9)
                    {

                        SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString(), bPSM);
                    }
                    logger.Debug("[SensorType]" + nSensorType);
                }
            }
        }

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag, bool bPSM = false, bool bTest = false)
        {
            int nSensor = -1;
            Facility.FacilityType sensorType = Facility.ToFacilityType(nSensorType);

            switch (sensorType)
            {
                case Facility.FacilityType.FIRE_SENSOR:
                case Facility.FacilityType.FireSensor_TypeA:
                case Facility.FacilityType.FireSensor_TypeB:
                case Facility.FacilityType.FireSensor_GasEmission:
                case Facility.FacilityType.FireSensor_ManualControl:
                    nSensor = (int)Facility.FacilityType.FIRE_SENSOR;
                    bPSM = false;
                    break;

                case Facility.FacilityType.FireSensor_SiemensType:
                case Facility.FacilityType.FireSensor_AnalogSmokeType:
                case Facility.FacilityType.PSM_SENSOR:
                    nSensor = (int)sensorType;
                    bPSM = true;
                    break;
            }

            if (nSensor == -1)
                return false;

            if (nSensorZoneID < 0)
                return false;

            PostMan postMan = bPSM ? m_postManPSM : m_postManFire;

            if (!postMan.IsConnected)
                return false;

            int nHeader = 0;

            if (bTest)
                nHeader = SOPWebServer.Header.SENSOR_DATA_TEST;
            else
                nHeader = SOPWebServer.Header.SENSOR_DATA;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add((int)nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return postMan.SendMessage(nHeader, bytes);
        }*/

        public void WriteLog(string strLog)
        {
            logger.Write(strLog);
            //logger.Debug(strLog);
        }

        private void RemoveLog(DateTime dtNow)
        {
            string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string szFullPath = System.IO.Directory.GetParent(szPath).FullName;

            string[] arrFiles = System.IO.Directory.GetFiles(szFullPath, "*.log");

            int nYear, nMonth, nDay;

            foreach (string strFile in arrFiles)
            {
                int nDotIndex = strFile.LastIndexOf('.');

                if (nDotIndex < 0)
                    continue;

                string strFilePath = strFile.Substring(0, nDotIndex);

                if (strFilePath.Length < 8)
                    continue;

                string strDate = strFilePath.Substring(strFilePath.Length - 8, 8);

                string strYear = strDate.Substring(0, 4);
                string strMonth = strDate.Substring(4, 2);
                string strDay = strDate.Substring(6);

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

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtFile = new DateTime(nYear, nMonth, nDay);
            TimeSpan spant = dtNow - dtFile;
            if (spant.TotalDays > 30.0)
                return true;
            return false;
        }

        public void SendFire(short header, byte[] bytes)
        {
            m_postManFire.SendMessage(header, bytes);
        }
    }
}
