using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Threading;
using System.Collections;

namespace AirQualityServer.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;

        private int m_nClientType = SOPWebServer.ClientType.PSM_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.AIR_QUALITY;

        private WebDBManager m_dbMgr = null;
        private SensorManager m_sensorManager = null;

        private int m_nPort = -1;
        private DateTime m_dtLastMessage = new DateTime();

        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;
        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        private static NetworkWebManager m_manager = null;

        public static NetworkWebManager Instance
        {
            get
            {
                return m_manager;
            }
        }

        public NetworkWebManager(WebDBManager dbMgr, SensorManager sensorManager)
        {
            m_manager = this;
            m_dbMgr = dbMgr;
            m_sensorManager = sensorManager;

            int nPort = GetServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Name = "ConnectionThread";
            t.Start();
        }

        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;

            while (!m_shutdownThread)
            {
                if (m_isConnected == true)
                {
                    TimeSpan span = DateTime.Now - m_dtLastMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        if (SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null) == false)
                        {
                            m_isConnected = false;
                            m_postBox.Dispose();
                            m_postBox = null;
                        }
                    }
                }

                if (m_isConnected == false)
                {
                    int nPort = GetServerPort();

                    if (m_postBox == null || (m_postBox != null && m_postBox.Port != nPort))
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
                            m_sensorManager.InitReceiverState();
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = m_dbMgr.WebServerURL;
                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            m_nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return m_nPort;
        }

        public void OnMessage(int header, byte[] messages)
        {
            if (m_shutdownThread)
                return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);
            m_dtLastMessage = DateTime.Now;

            if (header == SOPWebServer.Header.ARE_YOU_THERE)
            {
                SendData(SOPWebServer.Header.I_AM_HERE);
            }
            else if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
                m_postBox.Dispose();
                m_postBox = null;
            }
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[6];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            SendMessage(header, bytes);
        }

        #region SendMessage
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
                    //WriteLineLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastMessage = DateTime.Now;
                    //WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, int data4)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);
                arrDatas.Add(data4);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, string strData)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);
                arrDatas.Add(strData);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }
        #endregion

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nAlarmLevel, bool isReal)
        {
            int nSensorType = (int)UnE.Sensor.IFacility.FacilityType.PSM_SENSOR;

            if (nSensorZoneID < 0)
                return false;

            if (!m_isConnected)
                return false;

            int nHeader = 0;

            if (isReal == false)
                nHeader = SOPWebServer.Header.SENSOR_DATA_TEST;
            else
                nHeader = SOPWebServer.Header.SENSOR_DATA;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nAlarmLevel);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(nHeader, bytes);
        }

        public bool SendAllReceiverState(bool isConnected, int nReceiverID)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nReceiverID);
            arrDatas.Add(isConnected ? 1 : 0);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.ALL_RECEIVER_STATE, bytes);
        }
    }
}
