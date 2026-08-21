using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace MuxFireSensorServer.Network
{
    public class ClientProvider : ClientServiceProvider
    {
        private class SensorData
        {
            private bool m_isOn = false;
            private DateTime m_timeStamp;

            public bool IsOn
            {
                get { return m_isOn; }
                set { m_isOn = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }
        }

        private const byte BEGIN_BYTE = 0x41;//'A'
        private const int BLOCK_LENGTH = 72;

        public const int MUXTYPE_1 = 1;
        public const int MUXTYPE_2 = 2;

        private const byte LOG_TYPE_FIRE = 0x07;
        private const byte LOG_TYPE_OP = 0x05;
        private const byte LOG_TYPE_FIRED = 0x06;
        private const byte LOG_TYPE_RECOVERTY = 0x0a;

        private SOPWebClient.Logger logger = null;

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        // 지난번에 받은 패킷이 완전하지 않을 경우 지난 패킷을 보관했다가 나머지 패킷을 수신하면 합친다.
        private byte[] m_arrTempReceived = null;

        private int m_nMuxType = MUXTYPE_1;

        private static Relay.Server m_relayServer = null;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public int MuxType
        {
            get { return m_nMuxType; }
            set { m_nMuxType = value; }
        }

        public static Relay.Server RelayServer
        {
            get { return m_relayServer; }
            set { m_relayServer = value; }
        }

        public ClientProvider(NetworkManager mgr, string strLogFolder, string strLogFile)
        {
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

            if (logger == null)
                logger = new SOPWebClient.Logger(strLogFolder, strLogFile, 30);
        }

        public override void OnReceiveData()
        {
            try
            {
                lock (this)
                {
                    if (ReceivedData != null)
                    {
                        m_isReadingProcess = true;

                        int nBytesCount = ReceivedData.Count();

                        if (nBytesCount > 0)
                        {
                            m_nPingCount = 0;
                            byte[] bytes = ReceivedData;
                            RelaySensorDataData(bytes, 0, bytes.Length);
                            ProcessData(bytes);
                        }
                    }

                    m_isReadingProcess = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("OnReceiveData Error : " + e.Message);
                WriteLog("OnReceiveData Error : " + e.Message);
            }
        }

        public override void OnDropConnection()
        {
            NetworkWebClient.Instance.SendReceiverState(false);
        }

        public void ProcessData(byte[] bytes)
        {

            if (m_arrTempReceived != null)
            {
                int len1 = m_arrTempReceived.Length;
                int len2 = bytes.Length;

                byte[] bytes2 = new byte[len1 + len2];
                System.Buffer.BlockCopy(m_arrTempReceived, 0, bytes2, 0, len1);
                System.Buffer.BlockCopy(bytes, 0, bytes2, len1, len2);

                bytes = bytes2;
            }

            int nIndex = 0, nBeginIndex = -1, nEndIndex = -1;

            WriteLog("ReadBytes");
            WriteBinaryLog(bytes, 0, bytes.Length);

            int index = 1;

            while (GetBytesBlock(bytes, ref nIndex, ref nBeginIndex, ref nEndIndex))
            {
                WriteLog("ReadBlock : " + index++);
                ProcessData(bytes, nBeginIndex, nEndIndex);
            }
        }

        private void ProcessData(byte[] bytes, int nBeginIndex, int nEndIndex)
        {
            WriteBinaryLog(bytes, nBeginIndex, nEndIndex - nBeginIndex);

            byte lType = bytes[nBeginIndex + 1];
            DateTime timeStamp = ToDateTime(bytes, nBeginIndex + 2);
            bool isOn = bytes[nBeginIndex + 8] == 0x01;

            int nReceiverID = 0, nRelayID = 0, nLoopID = 0, nTagID = 0;
            GetReceiverInfo(bytes, nBeginIndex + 11, ref nReceiverID, ref nRelayID, ref nLoopID, ref nTagID);

            string strArea = GetString(bytes, nBeginIndex + 19, 21);
            string strDevice = GetString(bytes, nBeginIndex + 40, 23);
            string strRunning = GetString(bytes, nBeginIndex + 63, 9);

            string strLog = string.Format("Receiver({0}), Loop({1}), Relay({2}), Tag({3}), On({4}), Area : {5}, Device : {6}, Running : {7}, Type : {8}",
                nReceiverID, nLoopID, nRelayID, nTagID, isOn, strArea, strDevice, strRunning, (int)lType);

            WriteLog(strLog);
            System.Diagnostics.Trace.WriteLine(strLog);

            if (m_nMuxType == MUXTYPE_1 && lType == LOG_TYPE_OP && (strDevice.Contains("화재 복구") || strDevice.Contains("화재복구")))
                ProcessAllClear();
            else if (m_nMuxType == MUXTYPE_2 && lType == LOG_TYPE_RECOVERTY)
                ProcessAllClear();
            else if (lType == LOG_TYPE_FIRE || lType == LOG_TYPE_FIRED)
            {
                if (isOn)
                {
                    // 화재신호
                    ProcessFire(nReceiverID, nLoopID, nRelayID, nTagID, isOn, strArea, strDevice, strRunning);
                }
                else
                {
                    // 복구신호
                    ProcessClear(nReceiverID, nLoopID, nRelayID, nTagID, isOn, strArea, strDevice, strRunning);
                }
            }
            /*else if (lType == LOG_TYPE_RECOVERTY)
            {
                // 복구신호
                ProcessClear(nReceiverID, nLoopID, nRelayID, nTagID, isOn, strArea, strDevice, strRunning);
            }*/
        }

        public void ProcessFire(int nReceiverID, int nLoopID, int nRelayID, int nTagID, bool isOn, string strArea, string strDevice, string strRunning)
        {
            SensorTag sensor = SensorManager.Instance.FindSensorTag(nReceiverID, nLoopID, nRelayID, nTagID);

            if (sensor == null)
                return;

            NetworkWebClient.Instance.SendSensorData(sensor, 1);
        }

        public void ProcessClear(int nReceiverID, int nLoopID, int nRelayID, int nTagID, bool isOn, string strArea, string strDevice, string strRunning)
        {
            SensorTag sensor = SensorManager.Instance.FindSensorTag(nReceiverID, nLoopID, nRelayID, nTagID);

            if (sensor == null)
                return;

            NetworkWebClient.Instance.SendSensorData(sensor, 0);
        }

        private void ProcessAllClear()
        {
            // 전체복구 처리해야 함
            NetworkWebClient.Instance.SendAllClear();
        }

        private void RelaySensorDataData(/*int nReceiverID, int nLoopID, int nRelayID, int nTagID, bool isOn, */byte[] bytes, int nBeginIndex, int nEndIndex)
        {
            if (m_relayServer != null)
            {
                /*int nSensorTagID = SensorManager.GetSensorTagID(nReceiverID, nLoopID, nRelayID, nTagID);
                SensorData data = null;
                DateTime dtNow = DateTime.Now;

                if (m_dicSensorDatas.TryGetValue(nSensorTagID, out data))
                {
                    if (data.IsOn == isOn)
                    {
                        TimeSpan span = dtNow - data.TimeStamp;

                        if (span.TotalSeconds < m_timeoutSeconds)
                            return;
                    }

                    data.IsOn = isOn;
                    data.TimeStamp = dtNow;
                }*/

                m_relayServer.Provider.Send(bytes, nBeginIndex, nEndIndex - nBeginIndex);
            }
        }

        private void WriteBinaryLog(byte[] bytes, int nIndex, int len)
        {
            string strBytesLog = SOPWebClient.Logger.GetByteString(bytes, nIndex, len);
            WriteLog("Recv : " + strBytesLog);
        }

        private string GetString(byte[] bytes, int nIndex, int len)
        {
            byte[] trg = null;

            for (int i=nIndex;i<nIndex + len;i++)
            {
                if (bytes[i] == 0x00)
                {
                    if (i == nIndex)
                        return "";

                    trg = new byte[i - nIndex];
                    System.Buffer.BlockCopy(bytes, nIndex, trg, 0, i - nIndex);
                    break;
                }
            }

            if (trg == null)
            {
                trg = new byte[len];
                System.Buffer.BlockCopy(bytes, nIndex, trg, 0, len);
            }

            return Encoding.GetEncoding(51949).GetString(trg);
        }

        private void GetReceiverInfo(byte[] bytes, int nIndex, ref int nReceiverID, ref int nRelayID, ref int nLoopID, ref int nTagID)
        {
            nReceiverID = AsciiToInt(bytes, nIndex, 2);
            nLoopID = AsciiToInt(bytes, nIndex + 2, 1);
            nRelayID = AsciiToInt(bytes, nIndex + 3, 3);
            nTagID = AsciiToInt(bytes, nIndex + 6, 1);
        }

        private int AsciiToInt(byte[] bytes, int nIndex, int len)
        {
            int data = 0;

            for (int i=nIndex;i < nIndex + len;i++)
            {
                data = data * 10 + ((char)bytes[i] - '0');
            }

            return data;
        }

        private DateTime ToDateTime(byte[] bytes, int nIndex)
        {
            int year = ((int)bytes[nIndex]) + 2000;
            int month = (int)bytes[nIndex + 1];
            int day = (int)bytes[nIndex + 2];
            int hour = (int)bytes[nIndex + 3];
            int min = (int)bytes[nIndex + 4];
            int sec = (int)bytes[nIndex + 5];

            return new DateTime(year, month, day, hour, min, sec);
        }

        private bool GetBytesBlock(byte[] bytes, ref int nIndex, ref int nBeginIndex, ref int nEndIndex)
        {
            m_arrTempReceived = null;

            int len = bytes.Length;
            bool find = false;

            for (int i=nIndex;i<len;i++)
            {
                if (bytes[i] == BEGIN_BYTE)
                {
                    nIndex = i;
                    find = true;
                    break;
                }
            }

            if (find == false)
                return false;

            while (nIndex < len)
            {
                if (nIndex == len - 1)
                {
                    m_arrTempReceived = new byte[1];
                    m_arrTempReceived[0] = bytes[nIndex];
                    return false;
                }
                else if (bytes[nIndex + 1] != BEGIN_BYTE)
                    break;
                else
                    nIndex++;
            }

            if (nIndex + BLOCK_LENGTH <= len)
            {
                nBeginIndex = nIndex;
                nEndIndex = nBeginIndex + BLOCK_LENGTH;
                nIndex = nEndIndex;
                return true;
            }

            if (len <= nIndex)
                return false;

            // 처리되지 못한 데이터는 m_arrTempReceived에 남겨둔다.
            m_arrTempReceived = new byte[len - nIndex];
            System.Buffer.BlockCopy(bytes, nIndex, m_arrTempReceived, 0, len - nIndex);
            return false;
        }

        public void WriteLog(string strLog)
        {
            logger.Write(strLog);
        }
    }
}
