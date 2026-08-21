using System;
using TcpLib2;

namespace BroadcastSimulator
{
    public class ClientData
    {
        protected ServiceProvider m_provider = null;
        protected ConnectionState m_state = null;

        public ServiceProvider ServiceProvider
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
        public bool OnReceive(ConnectionState state, byte[] bytes)
        {
            return true;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        public bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        public void CloseClient()
        {
            try
            {
                ConnectionState state = m_state;

                if (state != null)
                {
                    m_state = null;
                    state.Tag = null;
                    state.EndConnection();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("CloseClient Exception : " + e.Message);
            }
        }

        public bool SendData(byte[] bytes, int offset, int count)
        {
            ConnectionState state = m_state;

            try
            {
                if (state != null && state.Connected)
                {
                    if (state.WriteAsync(bytes, offset, count))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("SendData Exception : " + e.Message);
            }

            return false;
        }
    }
}
