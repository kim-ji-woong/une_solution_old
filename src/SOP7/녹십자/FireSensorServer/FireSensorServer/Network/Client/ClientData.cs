using System.Collections;
using dnsTcpLib2;

namespace FireSensorServer.Network.Client
{
    public abstract class ClientData
    {
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        protected ServerServiceProvider m_provider = null;
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

        public ServerServiceProvider ServiceProvider
        {
            get { return m_provider; }
            set { m_provider = value; }
        }

        public ConnectionState ConnectionState
        {
            get { return m_state; }
            set { m_state = value; }
        }


        // bytes는 length byte가 제거되었음
        public abstract bool OnReceive(ConnectionState state, byte[] bytes);

        public virtual void Close()
        {
        }
    }
}
