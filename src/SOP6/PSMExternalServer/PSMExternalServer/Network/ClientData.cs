using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace PSMExternalServer.Network
{
    public class ClientData
    {
        private ServerServiceProvider m_provider = null;
        private ConnectionState m_state = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        //private byte[] m_arrTemp = null;

        private int m_nPingCount = 0;

        private byte[] m_lastSendBytes = null;
        private DateTime m_dtLastSend;

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public byte[] LastSendBytes
        {
            get { return m_lastSendBytes; }
            set
            {
                m_lastSendBytes = value;
                m_dtLastSend = DateTime.Now;
            }
        }

        public ClientData(ServerServiceProvider provider, ConnectionState state)
        {
            m_provider = provider;
            m_state = state;
        }

        public bool OnReceiveData(ConnectionState state, byte[] bytes)
        {
            if (bytes == null)
                return false;

            m_nPingCount++;

            if (bytes[0] == SERIAL_ID.NACK)
            {
                Resend(state);
            }

            /*List<byte[]> cmdList = GetCommandList(bytes);

            foreach (byte[] cmd in cmdList)
            {
                if (ProcessCommand(state, cmd) == false)
                    return false;
            }*/

            return true;
        }

        // Nack를 받으면 마지막에 보냈었던 데이터를 다시 보낸다.
        // 마지막 메시지를 보낸후 10초가 경과하였으면 다시 보내지 않는다.
        private bool Resend(ConnectionState state)
        {
            if (m_lastSendBytes != null)
            {
                TimeSpan span = DateTime.Now - m_dtLastSend;

                if (span.TotalSeconds > 10.0)
                {
                    m_lastSendBytes = null;
                    return false;
                }

                return m_provider.Send(m_lastSendBytes, 0, m_lastSendBytes.Length, state);
            }

            return false;
        }

        /*private List<byte[]> GetCommandList(byte[] bytes)
        {
            List<byte[]> cmdList = new List<byte[]>();

            int nBytesLength = bytes.Length;

            if (nBytesLength == 0)
                return cmdList;

            int nBeginIndex = 0;            

            if (m_arrTemp != null)
            {
                int nTempLength = m_arrTemp.Length;

                if (nTempLength > 0 && m_arrTemp[0] == SERIAL_ID.STX && bytes[0] != SERIAL_ID.STX)
                {
                    for (int i = 0; i < nBytesLength; i++)
                    {
                        if (bytes[i] == SERIAL_ID.ETX)
                        {
                            byte[] cmd = new byte[nTempLength + i + 1];
                            Array.Copy(m_arrTemp, cmd, nTempLength);
                            Array.Copy(bytes, 0, cmd, nTempLength, i + 1);

                            cmdList.Add(cmd);
                            nBeginIndex = i + 1;
                            break;
                        }
                    }

                    if (cmdList.Count == 0)
                    {
                        byte[] temp = new byte[nTempLength + nBytesLength];
                        Array.Copy(m_arrTemp, temp, nTempLength);
                        Array.Copy(bytes, 0, temp, nTempLength, nBytesLength);

                        m_arrTemp = bytes;
                        return cmdList;
                    }
                    else
                        m_arrTemp = null;
                }
                else
                {
                    // m_arrTemp가 STX로 시작되지 않았거나 bytes가 STX로 시작될 경우 m_arrTemp는 그냥 버린다.
                    m_arrTemp = null;
                }
            }

            int nIndex = -1;

            for (int i=nBeginIndex;i<nBytesLength;i++)
            {
                if (bytes[i] == SERIAL_ID.STX)
                    nIndex = i;
                else if (bytes[i] == SERIAL_ID.ETX)
                {
                    if (nIndex < 0)
                    {
                        // Data Error(STX 없이 ETX만 있다.)
                        return cmdList;
                    }
                    else
                    {
                        int nArrSize = i - nIndex + 1;
                        byte[] cmd = new byte[nArrSize];
                        Array.Copy(bytes, nIndex, cmd, 0, nArrSize);

                        nIndex = -1;
                    }
                }
            }

            if (nIndex >= 0)
            {
                int nArrSize = nBytesLength - nIndex;
                m_arrTemp = new byte[nArrSize];
                Array.Copy(bytes, nIndex, m_arrTemp, 0, nArrSize);
            }

            return cmdList;
        }

        private bool ProcessCommand(ConnectionState state, byte[] bytes)
        {
            return true;
        }*/
    }
}
