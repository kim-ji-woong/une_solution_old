using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;
using UnE.Sensor;

namespace PSMSensorServer
{
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
            private bool m_isPSM = false;
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

            public bool PSM
            {
                get { return m_isPSM; }
                set { m_isPSM = value; }
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
        private PostMan m_postManPSM = null;
        private PostMan m_postManEtc = null;
        private WebDBManager m_dbMgr = null;

        private log4net.ILog logger = null;
        private bool m_shutdownThread = false;

        private static NetworkWebClient m_instance = null;
        private DateTime m_dtLogDeleteDate = new DateTime();

        public static NetworkWebClient Instance
        {
            get { return m_instance; }
        }

        public NetworkWebClient(WebDBManager dbMgr)
        {
            m_instance = this;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_dbMgr = dbMgr;
            int nPort = ReadServerPort(m_dbMgr);

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);
            m_postManEtc = new PostMan(this, SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.SIMULATOR);
            m_postManPSM = new PostMan(this, SOPWebServer.ClientType.PSM_SENSOR_SERVER, SOPWebServer.ClientSubType.SENKO);
            m_postManPSM.PSM = true;

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManEtc, nPort);
            SetPostBox(m_postManPSM, nPort);
            //Connect(m_postManFire, nPort);

            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postManFire);

            Thread t3 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t3.Start(m_postManEtc);

            Thread t2 = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t2.Start(m_postManPSM);
        }

        /*public void Connect(object postMan)
        {
            if (postMan != null && postMan is PostMan)
            {
                int nPort = ReadServerPort(m_dbMgr);
                Connect((PostMan)postMan, nPort);
            }
        }

        private void Connect(PostMan postMan, int nPort)
        {
            PostBox postBox = new PostBox();
            postBox.WebServerURL = m_dbMgr.WebServerURL;
            postBox.PostMan = postMan;
            postMan.PostBox = postBox;

            if (nPort > 0)
                postBox.Port = nPort;

            if (postBox.Connect(postMan.ClientType, postMan.ClientSubType) == false)
            {
                System.Diagnostics.Trace.WriteLine("Connect Fail : " + postBox.ErrorMessage);
                Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
                t.Start(postMan);
            }
            else
            {
                postMan.IsConnected = true;
                SendAllReciverState();
            }
        }*/

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

                //Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
                //t.Start(postMan);
            }
            else if (header == SOPWebServer.Header.SERVER_COMMAND)
                ProcessServerCommand(messages);
        }

        private void ProcessServerCommand(byte[] messages)
        {
            ArrayList arrDatas = SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (arrDatas.Count >= 2 && arrDatas[0] is byte && arrDatas[1] is int)
            {
                byte cmd = (byte)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];

                if (cmd == SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_ALARM)
                {
                    ProcessRequestSensorAlarm(nSensorZoneID);
                }
                else if (cmd == SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_RESET)
                {
                    ProcessRequestSensorReset(nSensorZoneID);
                }
                else if (cmd == SOPWebServer.ServerCommandType.REQUEST_PSM_BUZZER && arrDatas.Count >= 3 && arrDatas[2] is int)
                {
                    int nOnOff = (int)arrDatas[2];
                    ProcessRequestBuzzer(nSensorZoneID, nOnOff);
                }
            }
        }

        public void ProcessRequestBuzzer(int nSensorID, int nOnOff)
        {
            PSMSensorManager.Instance.BuzzerSet(nSensorID, nOnOff);

            if (nOnOff == 1)
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 켜기", nSensorID);
                logger.Info(szMsg);
            }
            else
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 정지", nSensorID);
                logger.Info(szMsg);
            }
        }

        public void ProcessRequestSensorReset(int nSensorID)
        {
            PSMSensorManager.Instance.RequestReset(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 리셋", nSensorID);
            logger.Info(szMsg);
        }

        public void ProcessRequestSensorAlarm(int nSensorID)
        {
            PSMSensorManager.Instance.RequestAlarm(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 요청", nSensorID);
            logger.Info(szMsg);
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

        private void ConnectionThread(object arg)
        {
            PostMan postMan = (PostMan)arg;

            while (m_shutdownThread == false)
            {
                if (PSMSensorManager.Instance != null)
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
                                // 화재센서 서버는 Simulator다.
                                if (postMan.PSM)
                                    PSMSensorManager.Instance.InitReceiverState();

                                postMan.IsConnected = true;

                                //if (postMan.PSM)
                                //    SendAllReciverState();
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
                }

                Thread.Sleep(1000);

                DateTime dtNow = DateTime.Now;

                // 새벽 한시에 한번 로그 정리를 한다.
                if (dtNow.Hour == 1)
                {
                    if (dtNow.Year != m_dtLogDeleteDate.Year || dtNow.Month != m_dtLogDeleteDate.Month || dtNow.Day != m_dtLogDeleteDate.Day)
                    {
                        m_dtLogDeleteDate = dtNow;
                        ConnectionLogClient.RemoveLog();
                    }
                }
            }

            /*while (postMan.IsConnected == false)
            {
                if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                {
                    postMan.IsConnected = true;
                    SendAllReciverState();
                    break;
                }

                System.Diagnostics.Trace.WriteLine("Connect Fail : " + postMan.PostBox.ErrorMessage);
                Thread.Sleep(1000);
            }*/
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

            if (m_postManEtc.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManEtc.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManEtc.IsConnected = false;
            }

            if (m_postManPSM.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManPSM.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManPSM.IsConnected = false;
            }

            m_shutdownThread = true;
        }

        public void SendSensorData(int nReciver, int nCircuit, int nChannel, int nData, bool bPSM)
        {
            logger.Debug(string.Format("SendSensorData : {0}, {1}, {2}, {3}, {4}", nReciver, nCircuit, nChannel, nData, bPSM));

            int nReciverType = 1;
            if (bPSM == true)
            {
                nReciverType = 2;
            }
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
            IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);
            bool bFire = false;

            switch (sensorType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                case IFacility.FacilityType.FireSensor_TypeA:
                case IFacility.FacilityType.FireSensor_TypeB:
                case IFacility.FacilityType.FireSensor_GasEmission:
                case IFacility.FacilityType.FireSensor_ManualControl:
                    nSensor = (int)IFacility.FacilityType.FIRE_SENSOR;
                    bPSM = false;
                    bFire = true;
                    break;

                case IFacility.FacilityType.FireSensor_SiemensType:
                case IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case IFacility.FacilityType.PSM_SENSOR:
                    nSensor = (int)sensorType;
                    bPSM = true;
                    break;

                case IFacility.FacilityType.DOOR:
                case IFacility.FacilityType.STRONG_WIND:
                case IFacility.FacilityType.BLACKOUT:
                case IFacility.FacilityType.TERROR:
                case IFacility.FacilityType.FIREWALL:
                case IFacility.FacilityType.TEMPERATURE_HUMIDITY:
                    nSensor = (int)sensorType;
                    bPSM = false;
                    bFire = false;
                    break;
            }

            if (nSensor == -1)
                return false;

            if (nSensorZoneID < 0)
                return false;

            PostMan postMan = null;//bPSM ? m_postManPSM : m_postManFire;
            if (bPSM)
                postMan = m_postManPSM;
            else
            {
                if (bFire)
                    postMan = m_postManFire;
                else
                    postMan = m_postManEtc;
            }

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
        }

        public void SendReceiverState(List<Reciver> changedReceivers)
        {
            if (!m_postManPSM.IsConnected)
                return;

            ArrayList arrDatas = new ArrayList();

            foreach (Reciver receiver in changedReceivers)
            {
                arrDatas.Add(receiver.ID);
                arrDatas.Add(receiver.IsConnected ? 1 : 0);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            if (m_postManPSM.SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes))
            {
                foreach (Reciver receiver in changedReceivers)
                {
                    receiver.PrevConnected = receiver.IsConnected ? 1 : 0;
                }
            }
        }

        public void SendReciverState(int nReciver, bool bConnected)
        {
            if (!m_postManPSM.IsConnected)
                return;

            int nHeader = bConnected == true ? SOPWebServer.Header.RECEIVER_CONNECT : SOPWebServer.Header.RECEIVER_DISCONNECT;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nReciver);
            arrDatas.Add(bConnected == true ? 1 : 0);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManPSM.SendMessage(nHeader, bytes);
        }

        private void SendAllReciverState()
        {
            if (!m_postManPSM.IsConnected)
                return;

            ArrayList arReciverList = PSMNetworkServer.Instance.IOManager.GetPSMReciverList();
            if (arReciverList == null)
                return;

            int nDataCount = arReciverList.Count * 2;
            ArrayList arrDatas = new ArrayList();

            //arrDatas.Add(nDataCount);

            if (arReciverList != null)
            {
                foreach (Reciver receiver in arReciverList)
                {
                    arrDatas.Add(receiver.ID);
                    arrDatas.Add(receiver.IsConnected == true ? 1 : 0);
                }
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_postManPSM.SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }

        public void WriteLog(string strLog)
        {
            logger.Debug(strLog);
        }
    }
}
