using System;
using System.Collections.Concurrent;
using dnsTcpLib2;

namespace FireSensorServer.Network
{
    using Client;

    public class ServerServiceProvider : TcpServiceProvider
    {
        private ConcurrentDictionary<ConnectionState, ClientData> m_arrClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        private Logger m_logger = null;

        public ServerServiceProvider(Logger logger)
        {
            m_logger = logger;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            state.LengthAdd = false;

            ClientData data = new ClientDataSiemens(this, state);
            state.Tag = data;

            if (m_arrClients.TryAdd(state, data))
            {
                if (m_logger != null)
                    m_logger.Write("[" + NetworkManager.GetClientTypeString(NetworkManager.ClientType.Siemens) + "] new Client Connection : " + state.IPAddress + ":" + state.PortNo.ToString());

                NetworkManager.Instance.AddClient(state, NetworkManager.ClientType.Siemens);
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            ClientData data = null;

            if (m_arrClients.TryRemove(state, out data))
            {
                if (m_logger != null)
                    m_logger.Write("[" + NetworkManager.GetClientTypeString(NetworkManager.ClientType.Siemens) + "] close Connection : " + state.IPAddress + ":" + state.PortNo.ToString());

                data.Close();
                NetworkManager.Instance.RemoveClient(state);
            }
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            if (!base.OnReceiveData(state))
                return false;

            byte[] bytes = CopyBytes(state.RecivedBuffer);

            if (bytes == null)
                return false;

            ClientData client;

            if (m_arrClients.TryGetValue(state, out client))
                client.OnReceive(state, bytes);

            WriteRecvLog(state, bytes);
            return true;
        }

        private void WriteRecvLog(ConnectionState state, byte[] bytes)
        {
            if (m_logger == null)
                return;

            string strLog = "";

            int len = bytes.Length;

            for (int i=0;i<len;i++)
            {
                string strBytes = string.Format("{0:X2}", bytes[i]);

                if (i == 0)
                    strLog = strBytes;
                else
                    strLog += " " + strBytes;
            }

            strLog = string.Format("[{4}] Recv from {0}:{1}\r\nBytes Length : {2}\r\n{3}", state.IPAddress, state.PortNo, len, strLog, NetworkManager.GetClientTypeString(NetworkManager.ClientType.Siemens));
            m_logger.Write(strLog);
        }

        private byte[] CopyBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;

            if (len == 0)
                return null;

            byte[] copied = new byte[len];

            for (int i=0;i<len;i++)
            {
                copied[i] = bytes[i];
            }

            return copied;
        }

        public override object Clone()
        {
            return new ServerServiceProvider(m_logger);
        }
    }
}
