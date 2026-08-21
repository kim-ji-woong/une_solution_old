using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using SDMS;

namespace IntegratedManagement4
{
    public class ClientDataUnknown : ClientData
    {
        public ClientDataUnknown(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.UNKNOWN;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.WHO_I_AM)
            {
                if (SetClientType(bytes, state))
                {
                    ClientData client = (ClientData)state.Tag;                      
                    if (client != null)
                    {
                        if (!ProcessFirstConnection(client, state))
                            return false;
                    }
                }

                return true;
            }

            return true;
        }

        private bool SetClientType(byte[] bytes, ConnectionState state)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            int nClientType = BitConverter.ToInt32(bytes, 11);

            if (nClientType <= (int)ClientData.ClientType.ALL || nClientType >= (int)ClientData.ClientType.UNKNOWN)
                return false;

            client = NewClientData((ClientData.ClientType)nClientType);
            client.PingCount = 0;
            client.ConnectionState = state;
            state.Tag = client;

            m_provider.SetClientData(state, client);

            // WhoIAm과 함께 전달되는 나머지 Parameter들을 client.ReceivedData에 저장하여
            // ProcessFirstConnection()에서 처리할 수 있도록 한다.
            if (bytes.Length > 15)
            {
                int len = bytes.Length - 15;
                client.ReceivedData = new byte[len];
                System.Buffer.BlockCopy(bytes, 15, client.ReceivedData, 0, len);
            }

            return true;
        }

        private ClientData NewClientData(ClientData.ClientType type)
        {
            // 다른 Connection정보가 할당될 수 있으므로 반드시 lock이 필요함
            // skkim 2014-03-27
            SDMSServer.DdMonitor.Enter(m_provider.LockObject, true);
            ClientData result = null;
            {
                
                switch (type)
                {
                    case ClientType.SDMS_CLIENT:
                        result = new ClientDataSDMS(this.m_provider);
                        break;

                    case ClientType.SOP_SIMULATOR:
                        result = new ClientDataSOPSimulator(this.m_provider);
                        break;
                }
               
            }
            SDMSServer.DdMonitor.Exit(m_provider.LockObject, true);
            return result;
        }
    }
}
