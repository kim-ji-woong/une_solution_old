using System;
using dnsTcpLib2;
using System.Text;

namespace FireSensorServer.Network.Client
{
    public class ClientDataSiemens : ClientData
    {
        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const byte ACK = 0x86;

        private const byte ClearAll = 0x91;
        private const byte DetectFire = 0x92;
        private const byte ClearSensor = 0x93;
        private const byte DetectError = 0x94;
        private const byte ClearError = 0x95;

        private byte[] m_remainBytes = null;

        public ClientDataSiemens(ServerServiceProvider provider, ConnectionState state)
        {
            m_provider = provider;
            m_state = state;
        }

        public override bool OnReceive(ConnectionState state, byte[] bytes)
        {
            if (bytes == null)
                return false;

            ProcessSensorData(bytes);
            return true;
        }

        private bool ProcessSensorData(byte[] bytes)
        {
            if (m_remainBytes != null)
                bytes = AddRemainBytes(bytes);

            int bytesLength = bytes.Length;

            if (bytesLength < 7)
            {
                if (bytesLength == 4)
                {
                    return Hello(bytes);
                }
                else 
                    return false;
            }

            if (bytes[0] != STX)
                return false;

            int times = (int)(bytes[1] - 0x80);
            int remains = (int)(bytes[2] - 0x80);
            int dataLength = times * 128 + remains - 4;

            byte opCode = bytes[5];

            if (dataLength < 20 || bytesLength < dataLength + 8)
            {
                m_remainBytes = bytes;
                return false;
            }

            if (bytes[dataLength + 7] != ETX)
            {
                System.Diagnostics.Trace.WriteLine("Data Error. ETX is missing");
                return false;
            }

            Encoding encEUC_KR = Encoding.GetEncoding("euc-kr");
            string strData = encEUC_KR.GetString(bytes, 7, dataLength).Replace("\u0080", "");
            string[] datas = strData.Split(',');

            if (datas.Length < 4)
            {
                System.Diagnostics.Trace.WriteLine("Data Error. Data Count : " + datas.Length.ToString());
                return false;
            }

            DateTime dtEvent;

            try
            {
                dtEvent = Convert.ToDateTime(datas[0].Trim());
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Data Error. Time Parsing : " + e.Message);
                return false;
            }

            int tagNo = GetTagNo(datas[1]);
            string strPosition = datas[2];
            string strMessage = datas[3];

            if (bytesLength > dataLength + 8)
            {
                int len = bytesLength - (dataLength + 8);
                m_remainBytes = new byte[len];

                for (int i=0;i<len;i++)
                {
                    m_remainBytes[i] = bytes[dataLength + 8 + i];
                }
            }

            NetworkManager.EventType eventType = GetEventType(opCode);

            if (eventType != NetworkManager.EventType.Unknown)
                NetworkManager.Instance.SendSensorData(tagNo, eventType, NetworkManager.ClientType.Siemens, dtEvent, strPosition, strMessage, datas[1]);

            return true;
        }

        private bool Hello(byte[] bytes)
        {
            //STX 0x02
            //Seq Number Default: 0x81
            //ACK 0x86
            //ETX 0x03

            if (bytes[0] == STX && bytes[2] == ACK && bytes[3] == ETX)
            {
                // 응답 보내기
                byte[] helloBytes = new byte[4];
                helloBytes[0] = STX;
                helloBytes[0] = 0x81;
                helloBytes[0] = 0x86;
                helloBytes[0] = 0x03;

                m_state.Write(helloBytes, 0, helloBytes.Length);
                return true;
            }


            return false;
        }

        private NetworkManager.EventType GetEventType(byte opCode)
        {
            if (opCode == ClearAll)
                return NetworkManager.EventType.ClearAll;
            else if (opCode == DetectFire)
                return NetworkManager.EventType.DetectSensor;
            else if (opCode == ClearSensor)
                return NetworkManager.EventType.ClearSensor;
            else if (opCode == DetectError)
                return NetworkManager.EventType.DetectError;
            else if (opCode == ClearError)
                return NetworkManager.EventType.ClearError;
            /* 테스트 (감시, 감시 복구)
             * else if (opCode == 0x96)
                return NetworkManager.EventType.DetectSensor;
            else if (opCode == 0x97)
                return NetworkManager.EventType.ClearSensor;*/

            return NetworkManager.EventType.Unknown;
        }

        private int GetTagNo(string strAddress)
        {
            int no = 0;
            int len = strAddress.Length;

            for (int i=0;i<len;i++)
            {
                char ch = strAddress[i];

                if (ch >= '0' && ch <= '9')
                {
                    int data = (int)(ch - '0');
                    no = no * 10 + data;
                }
                else if (ch == '.')
                    break;
            }

            return no;
        }

        private byte[] AddRemainBytes(byte[] bytes)
        {
            int len1 = bytes.Length;
            int len2 = m_remainBytes.Length;
            byte[] newBytes = new byte[len1 + len2];

            for (int i=0;i<len1;i++)
            {
                newBytes[i] = bytes[i];
            }

            for (int i=len1;i<len1+len2;i++)
            {
                newBytes[i] = m_remainBytes[i - len1];
            }

            m_remainBytes = null;
            return newBytes;
        }
    }
}
