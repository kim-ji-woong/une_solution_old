using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;
using SDMS;

namespace AlarmSimulator
{
	public class NetworkManager
	{
        // PSMSensorServer와 통신
		private ClientProvider m_provider = null;
        // SOPServer와 통신
        private SOPClientProvider m_sopProvider = null;
		private int m_nPort = -1;
		private string m_strServerAddr = "";
		//private bool m_isConnected = false;
		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();

        public bool ShutDownThread
        {
            set { shutdownThread = value; }
        }

        public ClientProvider ClientProvider
        {
            get { return m_provider; }
        }

		private void WriteLog(object str)
		{
			//if (ConnectionLogEx.Instance.IsOpened)
			//	ConnectionLogEx.Instance.Write(str);
		}

		private void WriteLineLog(object str)
		{
            //if (ConnectionLogEx.Instance.IsOpened)
            //    ConnectionLogEx.Instance.WriteLine(str);
		}

		private void InitLog()
		{
			//ConnectionLogEx.MakeInstance();		
		}

		public void RecvLog(byte[] bytes)
		{
            //if (!ConnectionLogEx.Instance.IsOpened)
			//	return;
            			
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
		

        private int WriteSendLog(int nResult, byte[] bytes, ClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                //if (!ConnectionLogEx.Instance.IsOpened)
                //    return nResult;
                
                string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
                string strBytes = "";

                for (int i=nOffset;i<bytes.Length;i++)
                //foreach (byte b in bytes)
                {
                    byte b = bytes[i];

                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);                

                provider.PingCount = 0;
            }

            return nResult;
        }

		public int Send(byte[] bytes, ClientProvider provider)
		{
			int nResult = provider.Send(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 0);
		}

        public int Send_NoLengthByte(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }


        private int m_nSiteID = 1;

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

		public NetworkManager(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_nSiteID = nSiteID;

			InitLog();  

			m_dbMgr = dbMgr;

            if (strServerAddr == null)
            {
                string strPort = "19000";
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
                strServerAddr = addr[0].ToString();
            }
                       
			m_provider = new ClientProvider(this);
			m_strServerAddr = strServerAddr;

            m_sopProvider = new SOPClientProvider(this);

            Thread t;
            t = new Thread(ConnectionThread);
            t.Name = "ConnectionThread";
            t.Start();
		}

		public int GetServerPort()
		{

            string szServerName = "S1SensorServerPort";

            if( m_nSiteID == 100)
            {
                szServerName = "S1SensorServerPort";
            }
            else if( m_nSiteID == 1 || m_nSiteID == 2)
            {
                szServerName = "SensorServerPort";
            }

            string strSQL = string.Format("Select Port from {2} where SiteID = {0} AND Name='{1}'", m_nSiteID.ToString(), "PSMSensor", szServerName);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
			return 19000;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        
		}

        public int GetSOPServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

		public void OnDropConnection()
		{
            m_msgQueue.AbleToSend = false;
			//lock (this)
			//{
				//m_isConnected = false;
				//m_provider = new ClientProvider(this);
			//}
		}

        public void MessageQueueReady()
        {
            // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
            Thread t = new Thread(MessageQueueReadyThread);
            t.Name = "MessageQueueThread";
            t.Start();
        }

        // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
        private void MessageQueueReadyThread()
        {
            Thread.Sleep(1000);
            m_msgQueue.AbleToSend = true;
        }


		public void SendTestData()
		{
			if (!m_provider.IsConnected)
				return;


            int nSize = 10;
			byte[] bytes = new byte[nSize];
		
			
			
            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_AllReceiverState(bytes));
			//Send(bytes, m_provider);

			//m_provider.PingCount = 0;
		}


		private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
		{
			int nLength = bytesSrc.Length;
			System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
			nDestOffset += nLength;
		}

        public bool SendSensorData(SensorTag sensor, byte msgType)
        {
            string szDate = DateTime.Now.ToString();
            string strSensorTagNo = string.Format("{0:00}-{1:00}-{2}-{3:000}",
                    sensor.ReceiverID,
                    (sensor.SensorTagID / 10000) % 100,
                    (sensor.SensorTagID % 10000) / 1000,
                    sensor.SensorTagID % 1000);//sensor.SensorTagID.ToString();
            string strSensorTagName = sensor.SensorName;
            string strSensorTagType = sensor.TagType.ToString();

            string szData = szDate + "," + strSensorTagNo + "," + strSensorTagName + "," + strSensorTagType;
            byte[] byte2 = ClientProvider.MakeBytes(szData);

            int nLength = byte2.Length + 8;
            byte[] bytes = new byte[nLength];



            bytes[0] = 0x02;
            bytes[1] = (byte)((byte)((nLength - 2) / 128) + 0x80);
            bytes[2] = (byte)((byte)((nLength - 2) % 128) + 0x80);

            bytes[3] = 0x80;
            bytes[4] = 0x80;
            bytes[5] = msgType;

            bytes[6] = 0x80;

            bytes[nLength - 1] = 0x03;


            System.Buffer.BlockCopy(byte2, 0, bytes, 7, byte2.Length);

            this.Send_NoLengthByte(bytes, this.ClientProvider);
            return true;
        }

        public void Connect()
        {
            int nServerPort = GetServerPort();
            this.ClientProvider.Connect(m_strServerAddr, nServerPort);
        }

        public void Close()
        {
            this.ClientProvider.Close();
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!shutdownThread)
            {
                lock (this)
                {
                    if (m_sopProvider.IsConnected)
                    {
                        if (m_sopProvider.PingCount > 5)
                        {
                            m_sopProvider.PingCount = 0;

                            try
                            {
                                //m_log.WriteLine("PING COUNT EXCEPTION");
                                m_sopProvider.Close();
                            }
                            catch (System.Exception)
                            {

                            }

                        }
                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_sopProvider.IsReadingProcess)
                            m_sopProvider.SendData(TCP_ID.I_AM_HERE);
                        else
                            m_sopProvider.PingCount++;
                    }

                    if (!m_sopProvider.IsConnected)
                    {
                        int nPort = GetSOPServerPort();

                        try
                        {
                            if (nPort > 0)
                            {
                                m_sopProvider.Connect(m_strServerAddr, nPort);
                            }
                        }
                        catch (System.Exception)
                        {

                        }

                    }
                }

                Thread.Sleep(1000);
            }
        }

        public int Send(byte[] bytes, SOPClientProvider provider)
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

            /*if (nResult > 0)
            {
                if (!IsLogOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), EarthquakeSensorServer", (int)bytes[0], (int)bytes.Length);


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
            }*/

            return nResult;
        }

        public void SendEarthquakeSignal(int nSensorID, float fMagnitude, int nIntensity, int nAlarmLevel, string strPosition, DateTime time)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorID);
            arrDatas.Add(fMagnitude);
            arrDatas.Add(nIntensity);
            arrDatas.Add(nAlarmLevel);
            arrDatas.Add(strPosition);
            arrDatas.Add(time.ToBinary());

            byte[] bytes = SOPClientProvider.MakeBytes(TCP_ID.EARTHQUAKE_SENSOR_DETECT, arrDatas);
            Send(bytes, m_sopProvider);
        }
	}
}
