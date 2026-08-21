using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility2;
using System.Collections;
using SOPWebClient;

namespace EarthquakeSensorServer.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private int m_nClientType = SOPWebServer.ClientType.EARTHQUAKE_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.EARTHQUAKE;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();
        private bool m_shutdownThread = false;
        private WebDBManager m_dbMgr = null;
        
        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            //InitLog();

            int nPort = ReadServerPort(m_dbMgr);
            SetPostBox(nPort);

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();
        }

        private void SetPostBox(int nPort)
        {
            m_postBox = new PostBox();
            m_postBox.WebServerURL = m_dbMgr.WebServerURL;
            m_postBox.Port = nPort;
            m_postBox.PostMan = this;
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

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            m_shutdownThread = false;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void SendEarthquakeSignal(int nSensorID, float fMagnitude, int nIntensity, string strPosition, DateTime time, bool isReal)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorID);
            arrDatas.Add(fMagnitude);
            arrDatas.Add(nIntensity);
            
            // 알람단계는 서버에서 계산한다.
            //int nAlarmLevel = 1;
            //arrDatas.Add(nAlarmLevel);
            arrDatas.Add(strPosition);
            arrDatas.Add(time.ToBinary());
            arrDatas.Add(isReal);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT, bytes);            
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
                    //WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                    //WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            string strLog = string.Format("SendMessage : Header({0}), Length({1})", header, (int)bytes.Length);
            string strBytes = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            //WriteLog(strLog + strBytes);
        }

        public void OnMessage(int header, byte[] messages)
        {
            
        }
    }
 }
