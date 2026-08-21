using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;
using libUSS;

namespace SampleClient.Network
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
                    OnReceive(header, arrDatas);
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
            m_mgr.OnDropConnection();
        }

        private void OnReceive(short header, ArrayList arrDatas)
        {
            if (header == Header.ARE_YOU_THERE)
                SendData(Header.I_AM_HERE);
            else
            {
                System.Diagnostics.Trace.WriteLine("OnReceive : " + GetReceiveString(header, arrDatas));
                //System.Diagnostics.Trace.WriteLine("OnReceive : " + header);
            }
        }

        private string GetReceiveString(short header, ArrayList arrDatas)
        {
            string strHeader = "";

            if (header == Header.REQUEST_SELECT_EVENT_TYPE)
                strHeader = "REQUEST_SELECT_EVENT_TYPE";
            else if (header == Header.RESPONSE_SELECT_EVENT_TYPE)
                strHeader = "RESPONSE_SELECT_EVENT_TYPE";
            else if (header == Header.FIRE_SENSOR_DATA)
                strHeader = "FIRE_SENSOR_DATA";
            else if (header == Header.POWER_OFF_DATA)
                strHeader = "POWER_OFF_DATA";
            else if (header == Header.EARTH_QUAKE_DATA)
                strHeader = "EARTH_QUAKE_DATA";
            else if (header == Header.WIND_SENSOR_DATA)
                strHeader = "WIND_SENSOR_DATA";
            else
                strHeader = header.ToString();

            string strData = "";

            foreach (object data in arrDatas)
            {
                if (strData.Length == 0)
                    strData = data.ToString();
                else
                    strData += ", " + data.ToString();
            }

            return strHeader + " : " + strData;
    }

        public int Send(byte[] buffer, int offset, int size)
        {
            return base.Send(buffer, offset, size);
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
                m_mgr.Send(bytes);
        }
    }

    public interface IServiceOwner
    {
        void OnConnect();
        void OnDropConnection();
    }
}
