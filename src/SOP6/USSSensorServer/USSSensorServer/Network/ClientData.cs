using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using libUSS;
using System.Collections;

namespace USSFireSensorServer.Network
{
    public class ClientData
    {
        private DateTime m_dtLastMessage;
        private string m_strEndPoint = "";
        private Dictionary<byte, byte> m_dicEventType = new Dictionary<byte, byte>();
        private USSServiceProvider m_provider = null;
        private int m_nPrevIntensity = -1;
        // Key : Sensor ID
        private Dictionary<int, float> m_dicPrevWindSpeed = new Dictionary<int, float>();

        public DateTime LastMessageTime
        {
            get { return m_dtLastMessage; }
            set { m_dtLastMessage = value; }
        }

        public string EndPoint
        {
            get { return m_strEndPoint; }
            set { m_strEndPoint = value; }
        }

        public int PrevIntensity
        {
            get { return m_nPrevIntensity; }
            set { m_nPrevIntensity = value; }
        }

        public ClientData(USSServiceProvider provider)
        {
            m_provider = provider;
            m_dtLastMessage = DateTime.Now;
        }

        public bool OnReceive(ConnectionState state, byte[] bytes)
        {
            m_dtLastMessage = DateTime.Now;

            short header;
            ArrayList arrDatas = BinaryHelper.ReadBytes(bytes, out header);

            if (header == Header.REQUEST_SELECT_EVENT_TYPE)
            {
                ProcessRequestSelectEventType(arrDatas, state, bytes);
            }
            else if (header == Header.ARE_YOU_THERE)
            {
                m_provider.SendData(Header.I_AM_HERE, state);
            }

            return true;
        }

        private void ProcessRequestSelectEventType(ArrayList arrDatas, ConnectionState state, byte[] bytes)
        {
            if (arrDatas.Count == 2 && arrDatas[0] is short)
            {
                if (arrDatas[1] is byte)
                {
                    byte eventType = (byte)arrDatas[1];
                    m_dicEventType[eventType] = eventType;
                }
                else if (arrDatas[1] is byte[])
                {
                    byte[] eventTypes = (byte[])arrDatas[1];

                    foreach (byte eventType in eventTypes)
                    {
                        m_dicEventType[eventType] = eventType;
                    }
                }
                else
                    return;

                if (m_provider != null)
                {
                    byte[] header = BitConverter.GetBytes(Header.RESPONSE_SELECT_EVENT_TYPE);
                    int headerCount = header.Length;

                    for (int i = 0; i < headerCount; i++)
                    {
                        bytes[i] = header[i];
                    }

                    if (m_provider.Send(bytes, 0, bytes.Length, state))
                    {
                        m_provider.SetClientInfo(state, m_dicEventType.Keys.ToList());
                    }
                }
            }
        }

        public bool HasEvent(byte eventType)
        {
            return m_dicEventType.ContainsKey(eventType);
        }

        public float GetPrevWindSpeed(int nSensorID)
        {
            float fWindSpeed;

            if (m_dicPrevWindSpeed.TryGetValue(nSensorID, out fWindSpeed))
                return fWindSpeed;

            return -1.0f;
        }

        public void SetPrevWindSpeed(int nSensorID, float fWindSpeed)
        {
            m_dicPrevWindSpeed[nSensorID] = fWindSpeed;
        }
    }
}
