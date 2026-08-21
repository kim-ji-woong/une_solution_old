using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;

namespace RestoreService
{
    public class ClientProvider : ClientServiceProvider
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

        public ClientProvider(NetworkManager mgr)
        {
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
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

                    if (!CheckValidation(ReceivedData))
                        goto RETURN;

					NetworkManager.Instance.RecvLog(ReceivedData);
					
                    if (ReceivedData[0] == TCP_ID.ARE_YOU_THERE)
                    {
                        SendData(TCP_ID.I_AM_HERE);
                    }
                    else if (ReceivedData[0] == TCP_ID.WHO_ARE_YOU)
                    {
                        SendWhoIam();
                    }
					else if (ReceivedData[0] == TCP_ID.BEGEIN_RESTORE)
					{
						//NetworkManager.Instance.WriteLineLog("BeingRestore");
						m_mgr.BeginRestore();
					}
                }
            }

        RETURN:
            m_isReadingProcess = false;
        }

        private void SendWhoIam()
        {
            byte[] bytes = new byte[15];
            byte[] dataBytes = MakeBytes((int)TCP_CLIENT.SOP_RESTORE);

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.WHO_I_AM);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            m_mgr.Send(bytes, this);
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 6)
                return false;

            int nChunkCount = (int)bytes[1];
            int nIndex = 6;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (length < nIndex + 5)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (length < nIndex + 5 + nDataLength)
                    return false;

                nIndex += 5 + nDataLength;
            }

            return true;
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

			if (this.Client.Client.Connected == true)
				m_mgr.Send(bytes, this);
		}

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            //byte[] dataBytes = new byte[data.Length * sizeof(char)];


            //System.Buffer.BlockCopy(data.ToCharArray(), 0, dataBytes, 0, dataBytes.Length);
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        /*public void Close()
        {
            this.Client.Close();
        }*/
    }
}
