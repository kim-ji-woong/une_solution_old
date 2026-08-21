using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;
using System.Diagnostics;
using JubixNetwork;

namespace S1SensorServer
{
    public abstract class ClientData
    {
        public enum ClientType
        {
            ALL = 0,
            KPXSensorServer = 1,
            UNKNOWN
        };

        private int m_nPingCount = 0;
        private ClientType m_type = ClientType.UNKNOWN;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        protected NetworkServiceProvider m_provider = null;
        protected ConnectionState m_state = null;

        public byte[] ReceivedData
        {
            get { return m_arrReceived; }
            set { m_arrReceived = value; }
        }

        public byte[] TempData
        {
            get { return m_arrTemp; }
            set { m_arrTemp = value; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public NetworkServiceProvider ServiceProvider
        {
            get { return m_provider; }
            set { m_provider = value; }
        }

        public TcpLib2.ConnectionState ConnectionState
        {
            get { return m_state; }
            set { m_state = value; }
        }

        

        // bytes는 length byte가 제거되었음
        protected abstract bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, JubixNetwork.JubixMessage msg);

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected virtual bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        protected bool ProcessFirstConnection(ClientData data, ConnectionState state)
        {
            return data.ProcessFirstConnection(state);
        }

        public virtual void Close()
        {

        }

        public virtual bool OnReceiveData(ConnectionState state, byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            
            
            WriteByteArray(bytes);
            
     
            int nHeader;
            JubixNetwork.JubixMessage msg = JubixNetwork.JubixMessage.ReadDataValue(bytes, out nHeader);
     
            if (nHeader < 0)
                return false;
            else if (nHeader == 0)
                return true;

            bool bResult = OnReceive(state, bytes, nHeader, msg);           
            return bResult;
        }

        private void WriteByteArray(byte[] bytes)
        {
            Debug.Write("{");
            for (int i = 0; i < bytes.Length; i++)
            {
                Debug.Write(string.Format("{0:X}", bytes[i]));
                Debug.Write(" ");
            }
            Debug.WriteLine("}");
        }

    }
}
