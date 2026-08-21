using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Collections.Concurrent;
using System.Collections;
using DBUtility2;
using ServerProcess.Client;
using ServerProcess.Data;

namespace SOPWebServer
{
    public class PostOffice : ServerProcess.IPostOffice
    {
        private class ClientAddress
        {
            private string m_strIP = "";
            private int m_nPort = -1;

            public string IP
            {
                get { return m_strIP; }
                set { m_strIP = value; }
            }
                
            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }
        }

        private static PostOffice m_instance = new PostOffice();

        private System.Timers.Timer m_timer = null;
        private bool m_systemClose = false;

        private List<BaseClient> m_clients = new List<BaseClient>();
        private int m_nSiteID = 1;
        private DirectDBManager m_dbMgr = null;
        //private WebDBManager m_dbMgr = null;

        private LoginServer m_loginServer = null;
        private IMainWindow m_mainWindow = null;

        public static PostOffice Instance
        {
            get { return m_instance; }
        }

        public IMainWindow MainWindow
        {
            get { return m_mainWindow; }
            set { m_mainWindow = value; }
        }

        public bool SystemClose
        {
            get { return m_systemClose; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
        }

        private PostOffice()
        {
            AgentFactory.Factory factory = AgentFactory.BaseFactory.GetFactory();

            factory.Logger = Logger.Instance;
            factory.ProcessManager = new ProcessManager(factory);
            factory.SMSManager = new SMSManager(factory);
            factory.BroadcastManager = new BroadcastManager(factory);

            m_clients.Add(new FireSensorServer(factory, this));
            m_clients.Add(new PSMSensorServer(factory, this));
            m_clients.Add(new SecuritySensorServer(factory, this));
            m_clients.Add(new SDMSServer(factory, this));
            //m_clients.Add(new SOPSimulatorServer(factory, this));
            m_clients.Add(new SOPManagerServer(factory, this));
            m_clients.Add(new ServerCommander(factory, this));
            m_clients.Add(new EarthquakeSensorServer(factory, this));
            m_clients.Add(new EtcSensorServer(factory, this));
            m_clients.Add(new TempHumidityServer(factory, this));

            m_loginServer = new LoginServer(factory, this);
            m_clients.Add(m_loginServer);

            m_timer = new System.Timers.Timer(1000);
            m_timer.Elapsed += OnTimer;
        }

        /*public void Start(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);

            Initialize();
        }*/

        public void Start(int nSiteID, string strWebServerURL, string strDBName, int nDBType, string strID, string strPW)
        {
            m_dbMgr = DirectDBManager.MakeInstance((DirectDBManager.DBType)nDBType, strWebServerURL, strID, strPW, strDBName);
            m_dbMgr.SiteID = nSiteID;
            /*m_dbMgr = new WebDBManager(strDBName, nSiteID);
            m_dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;
            m_dbMgr.WebServerURL = strWebServerURL;*/

            Initialize();
        }

        private void Initialize()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            ServerProcess.DdMonitor.Logger = GetLogger();

            SetSOPSimulatorServer(dbMgr);

            MemberManager.Instance.Initialize(dbMgr);
            SensorZoneManager.Instance.Initialize(dbMgr);
            PSMManager.Instance.Initialize(dbMgr, this);
            MemberManager.Instance.LoadFacilityManager(dbMgr);
            ReceiverManager.Instance.Initialize(dbMgr);
            FacilityManager.ReadFacilityTypes(dbMgr);
            AlarmManager.Instance.ReadSensorHistory(dbMgr);

            //IOManager.Instance.Initialize(m_dbMgr);
            //SensorManager.Instance.Initialize(m_dbMgr);

            foreach (BaseClient client in m_clients)
            {
                client.OnLoad(dbMgr);
            }

            dbMgr.Close();

            m_timer.Start();
        }

        private void SetSOPSimulatorServer(DirectDBManager dbMgr)
        {
            string strSQL = "Select * from ControlUser";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            AgentFactory.Factory factory = AgentFactory.BaseFactory.GetFactory();

            if (arrResult == null)
            {
                ServerProcess.Client.SOPSimulatorServer2 simulatorServer = new ServerProcess.Client.SOPSimulatorServer2(factory, this);
                m_clients.Add(simulatorServer);
                SOPSimulatorManager.ServerInstance = simulatorServer;
            }
            else
            {
                ServerProcess.Client.SOPSimulatorLegacyServer simulatorServer = new ServerProcess.Client.SOPSimulatorLegacyServer(factory, this);
                m_clients.Add(simulatorServer);
                SOPSimulatorManager.ServerInstance = simulatorServer;
            }
        }

        public void Stop()
        {
            m_systemClose = true;
            m_timer.Stop();

            foreach (BaseClient client in m_clients)
            {
                client.CloseConnection();
            }
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            long current = DateTime.Now.ToBinary();
            byte[] currentBytes = BitConverter.GetBytes(current);
            
            foreach (BaseClient client in m_clients)
            {
                client.OnTimer();
            }

            Logger.Instance.RemoveOldLogs();
        }

        private ClientAddress GetClientAddress(OperationContext ctx)
        {
            try
            {
                System.ServiceModel.Channels.MessageProperties properties = ctx.IncomingMessageProperties;
                System.ServiceModel.Channels.RemoteEndpointMessageProperty endpoint = properties[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty;

                ClientAddress addr = new ClientAddress();

                if (endpoint.Address == "::1")
                    addr.IP = "127.0.0.1";
                else
                    addr.IP = endpoint.Address;

                addr.Port = endpoint.Port;
                return addr;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("GetClientAddress Error : " + e.Message);
            }

            return new ClientAddress();
        }

        public bool AddClient(int nClientType, int nClientSubType, OperationContext ctx, out string strIP, out int nPort)
        {
            strIP = "";
            nPort = 0;

            if (m_systemClose || ctx == null)
                return false;

            ClientAddress addr = GetClientAddress(ctx);
            GetLogger().Write(string.Format("AddClient({0}:{1}), ClientType({2}), ClientSubType({3}), session({4})",
                addr.IP, addr.Port,
                nClientType, nClientSubType, ctx.SessionId));

            strIP = addr.IP;
            nPort = addr.Port;

            foreach (BaseClient client in m_clients)
            {
                if (client.ClientType == nClientType)
                {
                    return client.AddClient(nClientType, nClientSubType, ctx, addr.IP, addr.Port);
                }
            }

            return false;
        }

        public int ReceiveMail(OperationContext ctx, int header, byte[] messages)
        {
            if (m_systemClose)
                return ErrorMessageType.SERVICE_IS_CLOSED;

            if (ctx == null)
                return ErrorMessageType.NULL_CLIENT_CONTEXT;

            ClientData data = GetClientData(ctx);

            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.SOP_CURRENT_SELECT_MISSION)
            {
                // ARE_YOU_THRERE는 로그에 기록하지 않는다.
                // SOP_CURRENT_SELECT_MISSION는 SOPSimulator에서 Ping과 같은 빈도로 호출되기 때문에 별도로 로그에 기록하지 않도록 한다.
                RecvLog(data, ctx, header, messages);
            }

            if (data != null)
            {
                ArrayList arrDatas = messages == null ? new ArrayList() : BinaryHelper.ReadBytes(messages);

                foreach (BaseClient client in m_clients)
                {
                    if (client.ClientType == data.ClientType)
                        return client.OnReceive(data, ctx, header, messages, arrDatas);
                }
            }
            else
            {
                ClientAddress addr = GetClientAddress(ctx);
                string strIP = addr == null ? "" : addr.IP;

                if (messages == null)
                {
                    GetLogger().Write(string.Format("Unknown Client, header[{0}], message length[0], session[{1}], {2}", header, ctx.SessionId, strIP));
                }
                else
                {
                    GetLogger().Write(string.Format("Unknown Client, header[{0}], message length[{1}], session[{2}], {3}", header, messages.Count(), ctx.SessionId, strIP));
                }
            }

            return ErrorMessageType.UNKNOWN_CLIENT;
        }

        private ClientData GetClientData(OperationContext ctx)
        {
            foreach (BaseClient client in m_clients)
            {
                ClientData data = client.GetClientData(ctx);

                if (data != null)
                    return data;
            }

            return null;
        }

        public ServerProcess.IPostMan GetPostMan(OperationContext ctx)
        {
            IPostMan postMan = ctx.GetCallbackChannel<IPostMan>();
            return new PostMan(postMan);
        }

        public AgentFactory.ILogger GetLogger()
        {
            return Logger.Instance;
        }

        private void RecvLog(ClientData data, OperationContext ctx, int header, byte[] messages)
        {
            // 정기적으로 보내는 신호는 굳이 기록하지 않는다.
            if (header == SOPWebServer.Header.ARE_YOU_THERE || header == SOPWebServer.Header.I_AM_HERE || header == SOPWebServer.Header.CONFIRM_HAS_CONTROL)
                return;

            if (data == null)
                return;

            string strClientType = "Unknown", strClientSubType = "Unknown";

            if (data != null)
            {
                strClientType = SOPWebServer.ClientType.ToString(data.ClientType);
                strClientSubType = SOPWebServer.ClientSubType.ToString(data.ClientSubType);
            }

            string strLog = "";

            if (messages == null)
            {
                strLog = string.Format("RecvMessage from {4}:{5} : Header({0}), Length({1}) from ClientType({2}), ClientSubType({3})",
                    header, 0,
                    strClientType, strClientSubType, data.IP, data.Port);
            }
            else
            {
                strLog = string.Format("RecvMessage from {5}:{6} : Header({0}), Length({1}) from ClientType({2}), ClientSubType({3})\r\n{4}",
                    header, messages.Count(),
                    strClientType, strClientSubType,
                    Logger.GetByteString(messages),
                    data.IP, data.Port);
            }

            GetLogger().Write(strLog);
        }

        public void OnRemoveClient(ClientData client)
        {
            if (m_mainWindow != null)
                m_mainWindow.RemoveClient(client.IP, client.Port);
        }

        public void RemoveClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            foreach (BaseClient client in m_clients)
            {
                if (client.RemoveClient(nClientType, nClientSubType, strIP, nPort))
                    break;
            }
        }

        public void AddClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            if (m_mainWindow != null)
                m_mainWindow.AddClient(nClientType, nClientSubType, strIP, nPort);
        }

        public int SendMessageToClient(int nClientType, int header, byte[] bytes, ArrayList arrDatas)
        {
            foreach (BaseClient client in m_clients)
            {
                if (client.ClientType == nClientType)
                {
                    return client.OnReceive(null, null, header, bytes, arrDatas);
                }
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }
    }

    public interface IMainWindow
    {
        void AddClient(int nClientType, int nClientSubType, string strIP, int nPort);
        void RemoveClient(string strIP, int nPort);
    }
}
