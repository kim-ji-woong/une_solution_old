using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;

namespace ClientSample
{
    public class ClientProvider : ClientServiceProvider
    {
        private bool m_isReadingProcess = false;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public ClientProvider()
        {
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }

        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

        public override void OnDropConnection()
        {
            FormMain.Instance.SetState(false);
            m_arrTemp = null;
        }

        private bool OnReceive(byte[] bytes)
        {
            if (bytes != null)
            {
                m_isReadingProcess = true;

                m_arrReceived = bytes;

                if (m_arrTemp != null)
                {
                    int nReceivedCount = m_arrReceived.Length;
                    int nTempCount = m_arrTemp.Length;

                    byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                    Array.Copy(m_arrTemp, arrBuffer, nTempCount);
                    Array.Copy(m_arrReceived, 0, arrBuffer, nTempCount, nReceivedCount);

                    m_arrReceived = arrBuffer;
                    m_arrTemp = null;
                }

                int nBytesCount = m_arrReceived.Count();

                if (nBytesCount > 0)
                {
                    if (!CheckValidation(m_arrReceived))
                    {
                        m_arrTemp = m_arrReceived;
                        m_isReadingProcess = false;
                        return false;
                    }

                    //int nHeader = (int)BitConverter.ToInt16(m_arrReceived, 0);
                    short nHeader;
                    ArrayList arrDatas = TcpHelper.ReadBytes(m_arrReceived, out nHeader);

                    if (arrDatas == null)
                        return false;

                    if (nHeader == FireSimulator.TCP_ID.ARE_YOU_THERE)
                    {
                        SendIAmHere();
                    }
                    else if (nHeader == FireSimulator.TCP_ID.REPORT_FIRE)
                    {
                        ProcessReportFire(arrDatas);
                    }
                    else if (nHeader == FireSimulator.TCP_ID.CLEAR_FIRE)
                    {
                        ProcessClearFire(arrDatas);
                    }
                }
            }

            m_isReadingProcess = false;
            return true;
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 6)
                return false;

            int nChunkCount = (int)BitConverter.ToInt16(bytes, 2);
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

            if (length > nIndex)
            {
                byte[] bytes1 = new byte[nIndex];
                byte[] bytes2 = new byte[length - nIndex];

                Array.Copy(bytes, bytes1, nIndex);
                Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

                OnReceive(bytes1);

                if (!OnReceive(bytes2))
                    return false;

                m_arrReceived = null;
                return false;
            }

            return true;
        }

        private void SendIAmHere()
        {
            if (this.IsConnected == false)
                return;

            byte[] bytes = new byte[6] { (byte)FireSimulator.TCP_ID.I_AM_HERE, 0, 0, 0, 0, 0 };

            if (this.IsClientDisposed == false)
                Send(bytes, 0, bytes.Length);
        }

        private void ProcessReportFire(ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is long)
            {
                string strProjectName = (string)arrDatas[0];
                string strLevelID = (string)arrDatas[1];
                string strSpaceID = (string)arrDatas[2];
                DateTime time = DateTime.FromBinary((long)arrDatas[3]);

                FormMain.Instance.ReportFire(strLevelID, strSpaceID, time);
            }
        }

        private void ProcessClearFire(ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is long)
            {
                string strProjectName = (string)arrDatas[0];
                string strLevelID = (string)arrDatas[1];
                string strSpaceID = (string)arrDatas[2];
                DateTime time = DateTime.FromBinary((long)arrDatas[3]);

                FormMain.Instance.RemoveFire(strLevelID, strSpaceID, time);
            }
        }
    }
}
