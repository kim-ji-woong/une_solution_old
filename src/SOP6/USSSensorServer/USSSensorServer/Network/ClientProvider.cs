using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;
using libUSS;

namespace USSFireSensorServer.Network
{
    public class ClientProvider : ClientServiceProvider
    {
        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        private string m_strServerIP = "";
        private int m_nPort = 0;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public string ServerIP
        {
            get { return m_strServerIP; }
        }

        public ClientProvider(NetworkManager mgr, string strServerIP, int nPort)
        {
            m_mgr = mgr;
            m_strServerIP = strServerIP;
            m_nPort = nPort;

            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            this.LengthAdd = false;
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

                    short header;
                    ArrayList arrDatas = BinaryHelper.ReadBytes(ReceivedData, out header);
                    OnReceive(header, ReceivedData, arrDatas);
                }
            }

            RETURN:
            m_isReadingProcess = false;
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 4)
                return false;

            int nChunkCount = (int)bytes[1];
            int nIndex = 4;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (length < nIndex + 3)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (length < nIndex + 3 + nDataLength)
                    return false;

                nIndex += 3 + nDataLength;
            }

            return true;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection(m_strServerIP);
        }

        private void OnReceive(short header, byte[] bytes, ArrayList arrDatas)
        {
            if (header == Header.ARE_YOU_THERE)
                SendData(Header.I_AM_HERE);
            else
            {
                m_mgr.RecvUSSLog(bytes);

                if (header == Header.FIRE_SENSOR_DATA)
                {
                    if (arrDatas.Count >= 3 && arrDatas[0] is byte && arrDatas[1] is int && arrDatas[2] is DateTime)
                    {
                        byte on = (byte)arrDatas[0];
                        int nSensorTagID = (int)arrDatas[1];
                        DateTime timeStamp = (DateTime)arrDatas[2];

                        m_mgr.OnFireSignal(on == (byte)1, nSensorTagID, timeStamp);
                    }
                }
            }
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[4];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes((short)0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];

            if (this.Client.Client.Connected == true)
                m_mgr.SendUSS(bytes);
        }

        public bool Connect()
        {
            return Connect(m_strServerIP, m_nPort);
        }
    }

    public interface IServiceOwner
    {
        void OnConnect(string strIP, bool ussServer);
        void OnDropConnection(string strIP, bool ussServer);
    }
}
