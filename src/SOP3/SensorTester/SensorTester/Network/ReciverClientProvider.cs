using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;

namespace SensorTester
{
	public class ReciverClientProvider : ClientServiceProvider
	{
        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

		private int m_nEquipZoneID = -1;
		public int EquipZoneID
		{
			get { return m_nEquipZoneID; }
			set { m_nEquipZoneID = value; }
		}

		private string m_strReciverAddress = "";
		public string ReciverAddress
		{
			get { return m_strReciverAddress; }
			set { m_strReciverAddress = value; }
		}
		public override string ToString()
		{
			return "ReciverClientProvider(" + m_strReciverAddress + ")";
		}
		private int m_nPort = 0;
		public int Port
		{
			get { return m_nPort; }
			set { m_nPort = value; }
		}

        public ReciverClientProvider(NetworkManager mgr)
        {
			this.LengthAdd = false;
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);			
        }
				
		public void Connect()
		{
			base.Connect(m_strReciverAddress, m_nPort);
		}

		private bool CheckSum(byte[] buffer)
		{
			if (buffer.Length < 11)
				return false;
			byte sum = (byte)(((buffer[0] + buffer[1] + buffer[2] + buffer[3] + buffer[4] + buffer[5] + buffer[6] + buffer[7] + buffer[8] + buffer[9] + buffer[11]) % (byte)16) + (byte)0x30);
			
			return ( sum == buffer[10] ? true : false);
		}				

        public override void OnReceiveData()
        {
            if (ReceivedData != null)
            {
                m_isReadingProcess = true;

                int nBytesCount = ReceivedData.Count();

                if (nBytesCount > 0)
                {
                    m_nPingCount = 0;

					m_mgr.SensorRecvLog(ReceivedData);

                    if (ReceivedData[0] == SERIAL_ID.POLL)
                    {
						SendData(SERIAL_ID.ACK);
                    }
					if (ReceivedData[0] == SERIAL_ID.STX)
					{
						if (!CheckValidation(ReceivedData))
						{
							m_mgr.WriteLineLog("Check Validation Fail!");
							m_mgr.DumpByteData(ReceivedData);
							goto RETURN;
						}
							
						//
						// 센서 데이터 처리
						// 
						ProcessSensorData(ReceivedData);
					}
                }
            }

        RETURN:
            m_isReadingProcess = false;
        }

		private void ProcessSensorData(byte[] bytes)
		{
			char a1 = (char)bytes[1];
			char a2 = (char)bytes[2];

			StringBuilder sb = new StringBuilder();
			sb.Append(a1);
			sb.Append(a2);
			
			string szBuildingCode = sb.ToString();
			
			byte nData = bytes[4];

			char b2 = (char)bytes[5];
			char c1 = (char)bytes[7];
			char c2 = (char)bytes[8];

			StringBuilder sb2 = new StringBuilder();
			sb2.Append(b2);
			sb2.Append(c1);
			sb2.Append(c2);
			string szTag = sb2.ToString();

			m_mgr.SendSensorData(m_nEquipZoneID, (int)nData, szBuildingCode, szTag);
		}

        private bool CheckValidation(byte[] bytes)
        {
			bool bCheck = CheckSum(bytes);
			if (bCheck == true)
				return true;
            
			return false;
        }

        // header 1 Byte로만 이루어진 데이터
		public void SendData(byte header)
		{
			byte[] bytes = new byte[1];			
			bytes[0] = header;
			if (this.Client.Client.Connected == true)
				m_mgr.Send(bytes, this);
		}
		
        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }
    }
}

