using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;

namespace SensorSimulator
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        //private bool m_isConnected = false;
        private bool shutdownThread = false;
        private DBUtility.WebDBManager m_dbMgr = null;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private void WriteLog(string str)
        {
            if (ConnectionLog.Instance.IsOpened)
                ConnectionLog.Instance.Write(str);
        }

        private void WriteLineLog(string str)
        {
            if (ConnectionLog.Instance.IsOpened)
                ConnectionLog.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            ConnectionLog.Instance.Create("sensorsimulator.log");
        }

        public void RecvLog(byte[] bytes)
        {
            if (!ConnectionLog.Instance.IsOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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
            int nResult = provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                if (!ConnectionLog.Instance.IsOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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

        public NetworkManager(DBUtility.WebDBManager dbMgr)
        {
            InitLog();

            m_dbMgr = dbMgr;

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");
            string strServerURL = m_dbMgr.WebServerURL;

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

            Thread t = new Thread(ConnectionThread);
            t.Start();
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!shutdownThread)
            {
                lock (this)
                {
                    //if (m_isConnected)
                    if (m_provider.IsConnected)
                    {
                        if (m_provider.PingCount > 3)
                        {
                            //m_isConnected = false;
                            m_provider.PingCount = 0;
                            m_provider.Close();
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

                        if (m_nPort > 0)
                            /*m_isConnected = */m_provider.Connect(m_strServerAddr, m_nPort);
                    }
                }

                Thread.Sleep(1000);
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

        public bool SendSensorData(EquipmentZone zone, int nSensorType, byte data)
        {
            //if (!m_isConnected)
            if (!m_provider.IsConnected)
                return false;

            byte[] bytes = new byte[33];

            byte[] sensorTypeBytes = ClientProvider.MakeBytes(nSensorType);
            byte[] zoneIDBytes = ClientProvider.MakeBytes(zone.ID);
            byte[] dataBytes = ClientProvider.MakeBytes((int)data);


			byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.SENSOR_DATA);
			bytes[0] = nHeader[0];
			bytes[1] = nHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(3);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            int nIndex = 6;

            CopyBytes(bytes, ref nIndex, sensorTypeBytes);
            CopyBytes(bytes, ref nIndex, zoneIDBytes);
            CopyBytes(bytes, ref nIndex, dataBytes);

            return Send(bytes, m_provider) > 0;
        }

        private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
        {
            int nLength = bytesSrc.Length;
            System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
            nDestOffset += nLength;
        }
    }
}
