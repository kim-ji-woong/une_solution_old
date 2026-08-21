using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections.Concurrent;
using FireSensorServer.Data;
using SOPWebClient;

namespace FireSensorServer.Network
{
    public class ClientProvider : ClientServiceProvider
    {
        private const byte BEGIN_BYTE = 0x47; //'G'
        private const int BLOCK_LENGTH = 13;

        private NetworkWebManager m_netWebManager = null;
        private int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        // 지난번에 받은 패킷이 완전하지 않을 경우 지난 패킷을 보관했다가 나머지 패킷을 수신하면 합친다.
        private byte[] m_arrTempReceived = null;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(NetworkWebManager mgr)
        {
            m_netWebManager = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
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
                            ProcessData(bytes);
                        }
                    }

                    m_isReadingProcess = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("OnReceiveData Error : " + e.Message);
            }
        }

        public override void OnDropConnection()
        {
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
            while (GetBytesBlock(bytes, ref nIndex, ref nBeginIndex, ref nEndIndex))
            {
                ProcessData(bytes, nBeginIndex, nEndIndex);
            }
        }

        private void ProcessData(byte[] bytes, int nBeginIndex, int nEndIndex)
        {
            try
            {
                WriteBinaryLog(bytes, nBeginIndex, nEndIndex - nBeginIndex);

                string strUniqueCode1 = Encoding.ASCII.GetString(bytes, nBeginIndex, 1);     // G 회사 식별
                string strUniqueCode2 = Encoding.ASCII.GetString(bytes, nBeginIndex + 1, 1); // X 제품 식별
                string strCommandCode = Encoding.ASCII.GetString(bytes, nBeginIndex + 2, 1); // [A~Z] F: 화재 정보 G: 가스 정보 S: 감시 정보 T: 단선 정보 R: 복구 신호

                int nReceiverID = AsciiToInt(bytes, nBeginIndex + 3, 2);  // 수신기 번호
                int nUnitID = AsciiToInt(bytes, nBeginIndex + 5, 2);      // 유닛 번호
                string split = Encoding.ASCII.GetString(bytes, nBeginIndex + 7, 1); // 구분자
                int nSystemID = AsciiToInt(bytes, nBeginIndex + 8, 1); // 계통 번호
                int nLineID = AsciiToInt(bytes, nBeginIndex + 9, 3);   // 회선 번호

                string strOccurInfo = Encoding.ASCII.GetString(bytes, nBeginIndex + 12, 1); // 발생 정보 ‘N': 발생 F': 복구

                string strLog = string.Format("CommandCode({0}), ReceiverID({1}), UnitID({2}), SystemID({3}), LineID({4}), OccurInfo({5})",
                    strCommandCode, nReceiverID, nUnitID, nSystemID, nLineID, strOccurInfo);

                Logger.Instance.Write(strLog);
                System.Diagnostics.Trace.WriteLine(strLog);

                if (strCommandCode == "F" && strOccurInfo == "N")
                {
                    // 화재 발생
                    ProcessFire(nReceiverID, nUnitID, nSystemID, nLineID);
                }
                // 어반브릭스는 개별 복구 없음
                //else if (strCommandCode == "R" && strOccurInfo == "F")
                //{
                //    // 화재 복구
                //    ProcessClear(nReceiverID, nUnitID, nSystemID, nLineID);
                //}
                else if (strCommandCode == "R" && strOccurInfo == "N")
                {
                    // 전체 복구
                    ProcessAllClear();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[ERROR] ClientProvider.cs > void ProcessData(byte[], int, int) :" + ex.Message);
            }
        }

        public void ProcessFire(int nReceiverID, int nUnitID, int nSystemID, int nLineID)
        {
            SensorInfo sensor = Data.DataManager.Instance.FindSensorTag(nReceiverID, nUnitID, nSystemID, nLineID);
            if (sensor == null)
                return;

             m_netWebManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 0, 1);
        }

        public void ProcessClear(int nReceiverID, int nUnitID, int nSystemID, int nLineID)
        {
            SensorInfo sensor = Data.DataManager.Instance.FindSensorTag(nReceiverID, nUnitID, nSystemID, nLineID);
            if (sensor == null)
                return;

            m_netWebManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 0, 0);
        }

        private void ProcessAllClear()
        {
            m_netWebManager.SendAllClear();
        }

        private void WriteBinaryLog(byte[] bytes, int nIndex, int len)
        {
            string strBytesLog = Logger.GetByteString(bytes);
            Logger.Instance.Write("Recv : " + strBytesLog);
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
    }
}
