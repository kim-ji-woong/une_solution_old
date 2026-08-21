using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SDMS;

namespace AlarmSimulator
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
			//this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
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

                  
                }
            }

        RETURN:
            m_isReadingProcess = false;
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

        public static byte[] MakeBytes(short data)
        {
            byte[] dataBytes = BitConverter.GetBytes(data);
            return dataBytes;
        }

        public static byte[] MakeBytes(string data)
        {
            Encoding enc = System.Text.Encoding.GetEncoding(51949);
            byte[] datas = enc.GetBytes(data);
            return datas;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        public int Send(byte[] buffer, int offset,  int size)
        {
            return base.Send(buffer, offset, size);
        }

        public int Send_NoLengthByte(byte[] buffer, int offset,  int size)
        {
            if (Client != null && Client.Client != null)
            {
                SocketError nErrCode = SocketError.Success;
                int nSendSize = 0;

                nSendSize = Client.Client.Send(buffer, 0, size, SocketFlags.None, out nErrCode);

                if (nErrCode == SocketError.Success)
                    return nSendSize;
            }

            return -1;
        }

     }
}
