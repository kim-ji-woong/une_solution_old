using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using System.Collections;
using UnE.Sensor;
using DBUtility2;
using System.Threading;

namespace SH_Temp_Humidity_Server.Network
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
                        //if (m_owner != null)
                        //    m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postMan = null;
        private DirectDBManagerEx m_dbMgr = null;
        private bool m_shutdownThread = false;

        private VariousData<bool> m_receiverConnected = null;

        public NetworkWebClient(DirectDBManagerEx dbMgr)
        {
            m_postMan = new PostMan(this, SOPWebServer.ClientType.TEMPERATURE_HUMIDITY_SERVER, SOPWebServer.ClientSubType.WOORIZEN);

            m_dbMgr = dbMgr;
            int nPort = ReadServerPort();

            SetPostBox(m_postMan, nPort);

            Thread t = new Thread(new ParameterizedThreadStart(ConnectionThread));
            t.Start(m_postMan);
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
                postBox.WebServerURL = strWebServerURL;//m_dbMgr.WebServerURL;
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
                    m_receiverConnected = null;
                    int nPort = ReadServerPort();

                    if (postMan.Port != nPort)
                        SetPostBox(postMan, nPort);

                    if (postMan.PostBox != null)
                    {
                        if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                        {
                            postMan.IsConnected = true;
                            SendReceiverInfo(AlarmManager.Instance.ReceiverID, true, true);
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
            }
        }

        public void Close()
        {
            if (m_postMan.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postMan.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postMan.IsConnected = false;
            }

            m_shutdownThread = true;
        }

        public bool SendSensorData(AlarmData alarm, bool isAlarm)
        {
            if (m_postMan.IsConnected)
            {
                ArrayList arrDatas = new ArrayList();

                arrDatas.Add((int)IFacility.FacilityType.TEMPERATURE_HUMIDITY);
                arrDatas.Add(alarm.Sensor.SensorTagInfoID);
                arrDatas.Add(alarm.Sensor.SensorZoneID);
                arrDatas.Add(isAlarm ? alarm.AlarmType.ID : -alarm.AlarmType.ID);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                return m_postMan.SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
            }

            return false;
        }

        public bool SendSensorDatas(List<KeyValuePair<AlarmData, bool>> alarmDatas)
        {
            if (m_postMan.IsConnected)
            {
                int nDataCount = alarmDatas.Count;

                if (nDataCount == 0)
                    return true;

                ArrayList arrDatas = new ArrayList();

                arrDatas.Add((int)IFacility.FacilityType.TEMPERATURE_HUMIDITY);
                arrDatas.Add(nDataCount);

                for (int i = 0; i < nDataCount; i++)
                {
                    AlarmData alarm = alarmDatas[i].Key;
                    bool isAlarm = alarmDatas[i].Value;

                    arrDatas.Add(alarm.Sensor.SensorTagInfoID);
                    arrDatas.Add(alarm.Sensor.SensorZoneID);
                    arrDatas.Add(isAlarm ? alarm.AlarmType.ID : -alarm.AlarmType.ID);
                }

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                return m_postMan.SendMessage(SOPWebServer.Header.SENSOR_DATAS, bytes);
            }

            return false;
        }

        public bool SendReceiverInfo(int nReceiverID, bool isConnected, bool absolutely = false)
        {
            if (absolutely == false)
            {
                if (m_receiverConnected == null)
                    m_receiverConnected = new VariousData<bool>(isConnected);
                else if (m_receiverConnected.Data == isConnected)
                    return true;
            }

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nReceiverID);

            int nHeader = isConnected ? SOPWebServer.Header.RECEIVER_CONNECT : SOPWebServer.Header.RECEIVER_DISCONNECT;
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            bool result = m_postMan.SendMessage(nHeader, bytes);

            if (result)
            {
                if (m_receiverConnected == null)
                    m_receiverConnected = new VariousData<bool>(isConnected);
                else
                    m_receiverConnected.Data = isConnected;
            }

            return result;
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
    }
}
