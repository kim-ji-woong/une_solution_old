using TcpLib2;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;

namespace BroadcastServer.Network
{
    public class ServiceProvider : TcpServiceProvider
    {
        private ConcurrentDictionary<ConnectionState, ClientData> m_dicClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        private IServiceOwner m_serviceOwner = null;

        public IServiceOwner ServiceOwner
        {
            get { return m_serviceOwner; }
            set { m_serviceOwner = value; }
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            ClientData data = new ClientData();
            data.ConnectionState = state;
            state.Tag = data;
            state.LengthAdd = false;

            if (m_dicClients.TryAdd(state, data))
            {
                data.ProcessFirstConnection(state);
                System.Diagnostics.Trace.WriteLine("OnAccept : " + state.IPAddress + ":" + state.PortNo.ToString());

                if (m_serviceOwner != null)
                    m_serviceOwner.OnAccept(state.IPAddress + ":" + state.PortNo.ToString());

                // 새로 접속한 클라이언트를 제외한 나머지 클라이언트들과의 접속을 모두 끊는다.
                CloseAll(data);
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            ClientData data = null;
            m_dicClients.TryRemove(state, out data);
            System.Diagnostics.Trace.WriteLine("OnDropConnection : " + state.IPAddress + ":" + state.PortNo.ToString());

            if (m_serviceOwner != null)
                m_serviceOwner.OnDropConnection(state.IPAddress + ":" + state.PortNo.ToString());
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            if (!base.OnReceiveData(state))
                return false;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            //WriteByteArray(state.RecivedBuffer);

            bool bResult = client.OnReceive(state, state.RecivedBuffer);
            state.RecivedBuffer = null;
            return bResult;
        }

        private void CloseAll(ClientData exceptClient)
        {
            List<ClientData> clients = new List<ClientData>();
            clients.AddRange(m_dicClients.Values);
            
            foreach (ClientData client in clients)
            {
                if (client == exceptClient)
                    continue;

                client.CloseClient();
            }
        }

        public bool SendMessage(byte cmd, byte[] datas)
        {
            ClientData client = GetFirstClient();

            if (client == null)
                return false;

            string strEquipmentID = ConfigurationManager.AppSettings.Get("equipmentID");
            string strMyID = ConfigurationManager.AppSettings.Get("myID");

            if (strEquipmentID == null || strEquipmentID.Length == 0 ||
                strMyID == null || strMyID.Length == 0)
                return false;

            int nEquipID, nMyID;

            if (int.TryParse(strEquipmentID.Trim(), out nEquipID) == false ||
                int.TryParse(strMyID.Trim(), out nMyID) == false)
                return false;

            int nDataLength = datas.Length;

            int len = 8 + nDataLength - 1;
            byte[] bytes = new byte[len];

            // STX
            bytes[0] = 0x02;
            bytes[1] = (byte)len;
            bytes[2] = (byte)nEquipID;
            bytes[3] = (byte)nMyID;
            bytes[4] = cmd;

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[5 + i] = datas[i];
            }

            byte xor = bytes[0];

            for (int i=1;i<len-2;i++)
            {
                xor = (byte)(xor ^ bytes[i]);
            }

            bytes[5 + nDataLength] = xor;
            // ETX
            bytes[len - 1] = 0x03;

            if (client.SendData(bytes, 0, len) == false)
            {
                OnDropConnection(client.ConnectionState);
                return false;
            }

            return true;
        }

        private ClientData GetFirstClient()
        {
            List<ConnectionState> states = new List<ConnectionState>();
            states.AddRange(m_dicClients.Keys);

            ClientData client = null;

            foreach (ConnectionState state in states)
            {
                if (state.Connected == false)
                {
                    m_dicClients.TryRemove(state, out client);
                    continue;
                }

                if (m_dicClients.TryGetValue(state, out client))
                {
                    return client;
                }
            }

            return null;
        }

        public override object Clone()
        {
            return this;
        }
    }

    public interface IServiceOwner
    {
        void OnAccept(string strConnectionInfo);
        void OnDropConnection(string strConnectionInfo);
    }
}
