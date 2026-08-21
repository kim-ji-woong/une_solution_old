using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Collections.Concurrent;
using DBUtility2;
using System.Collections;
using AgentFactory;

namespace ServerProcess.Client
{
    public abstract class BaseClient : IBaseClient
    {
        //protected ConcurrentQueue<ClientData> m_clients = new ConcurrentQueue<ClientData>();
        // Key : Session ID
        private ConcurrentDictionary<string, ClientData> m_dicClients = new ConcurrentDictionary<string, ClientData>();
        protected BaseAgent m_agent = null;
        protected Factory m_agentFactory = null;
        protected DirectDBManager m_dbMgr = null;
        protected IPostOffice m_postOffice = null;

        protected bool m_timerProcessing = false;

        // 기존에 동작하던 Timer가 있어서 OnTimer() 호출이 취소된 회수
        private int m_nTimerCancelCount = 0;
        // OnTimer() 호출이 m_nTimerCancelLimit 이상 취소되면 모든 Client와의 접속을 끊어버린다.
        private const int m_nTimerCancelLimit = 30;

        public abstract int ClientType
        {
            get;
        }

        public BaseClient()
        {
        }

        public BaseClient(Factory factory, IPostOffice postOffice)
        {
            m_agentFactory = factory;
            m_postOffice = postOffice;
        }

        public void SetPostOffice(IPostOffice postOffice)
        {
            m_postOffice = postOffice;
        }

        public void SetAgentFactory(Factory agentFactory)
        {
            m_agentFactory = agentFactory;
        }

        public virtual bool AddClient(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice.SystemClose)
                return false;

            ClientData client;

            if (m_dicClients.TryGetValue(ctx.SessionId, out client))
                return false;

            /*List<ClientData> clientDatas = m_dicClients.Values.ToList();

            foreach (ClientData data in clientDatas)
            {
                // 문자발신기는 같은 IP에 대하여 중복 접속을 허용한다.
                if (nClientSubType == SOPWebServer.ClientSubType.SMS_SENDER)
                    continue;

                if (data.ClientType == nClientType && data.ClientSubType == nClientSubType && data.IP == strIP)
                {
                    // 같은 타입의 클라이언트가 동일한 IP로 둘 이상 접속하게 되면 둘다 접속을 끊어버린다.
                    ClosePostMan(data.PostMan);
                    RemoveClient(data);

                    IPostMan postMan = m_postOffice.GetPostMan(ctx);

                    if (postMan != null)
                        ClosePostMan(postMan);

                    System.Diagnostics.Trace.WriteLine("같은 타입의 클라이언트 접속 감지됨 : " + SOPWebServer.ClientType.ToString(nClientType) + ", " + strIP);
                    return false;
                }
            }*/

            client = MakeClientData(nClientType, nClientSubType, ctx, strIP, nPort);

            if (m_dicClients.TryAdd(ctx.SessionId, client))
            {
                //m_clients.Enqueue(client);
                return true;
            }

            return false;
        }

        private void ClosePostMan(IPostMan postMan)
        {
            IClientChannel channel = postMan.ClientChannel;

            if (channel.State == CommunicationState.Opened)
            {
                postMan.OnRing(SOPWebServer.Header.CLOSE_CONNECTION, null);
                channel.Close();
            }
        }

        protected virtual ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);
                //IPostMan postMan = ctx.GetCallbackChannel<IPostMan>();
                ClientData data = new ClientData(ctx.SessionId, postMan, nClientType, nClientSubType);
                data.IP = strIP;
                data.Port = nPort;
                postMan.ClientData = data;

                return data;
            }

            return null;
        }

        public ClientData GetClientData(OperationContext ctx)
        {
            ClientData client;

            if (m_dicClients.TryGetValue(ctx.SessionId, out client) == false)
                return null;

            return client;
        }

        protected List<ClientData> GetClientDatas()
        {
            return m_dicClients.Values.ToList();
        }

        protected void SendPostMan(int nHeader, byte[] bytes)
        {
            // lock을 사용하지 않고 동기화문제를 해결하기 위하여
            // Dictionary에서 현재 상태의 List를 만든다.
            // 접속이 끊어진 Client는 Dictionary에서 삭제한다.
            List<ClientData> clients = GetClientDatas();
            List<ClientData> removeClients = new List<ClientData>();

            foreach (Client.ClientData client in clients)
            {
                IClientChannel channel = client.PostMan.ClientChannel;

                if (channel.State == CommunicationState.Opened)
                {
                    try
                    {
                        if (nHeader >= 0)
                            client.PostMan.OnRing(nHeader, bytes);
                        else
                            client.PostMan.OnRing(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine("SendPostMan Exception : " + e.Message);
                        removeClients.Add(client);
                    }
                }
                else
                    removeClients.Add(client);
            }

            foreach (ClientData data in removeClients)
            {
                RemoveClient(data);
            }

            removeClients.Clear();
            clients.Clear();

            // lock을 사용하지 않고 동기화문제를 해결하기 위하여
            // Queue에서 데이터를 하나씩 꺼내어 임시저장소에 보낸다.
            // 작업이 완료되면 임시저장소에 있는 것들을 다시 Queue로 보낸다.
            /*List<ClientData> tempClients = new List<ClientData>();
            ClientData client;

            m_dicClients.Clear();

            while (m_clients.Count > 0)
            {
                if (m_clients.TryDequeue(out client))
                {
                    IClientChannel channel = (IClientChannel)client.PostMan.ClientChannel;

                    if (channel.State == CommunicationState.Opened)
                    {
                        if (nHeader >= 0)
                            client.PostMan.OnRing(nHeader, bytes);

                        tempClients.Add(client);
                    }
                }
            }

            foreach (ClientData data in tempClients)
            {
                m_dicClients.TryAdd(data.SessionID, data);
                m_clients.Enqueue(data);
            }*/
        }

        protected virtual void RemoveNotConnectedClients()
        {
            SendPostMan(-1, null);
        }

        protected void RemoveClient(ClientData data)
        {
            ClientData _data;

            if (m_dicClients.TryRemove(data.SessionID, out _data))
            {
                if (m_postOffice != null)
                    m_postOffice.OnRemoveClient(data);

                OnRemoveClient(data);
            }
        }

        public void OnTimer()
        {
            if (m_postOffice.SystemClose)
                return;

            if (m_timerProcessing)
            {
                m_nTimerCancelCount++;

                if (m_nTimerCancelCount >= m_nTimerCancelLimit)
                {
                    // 특정 Client 때문에 Block된 상태이기 때문에, 모든 Client와의 접속을 끊어버린다.
                    OnClose();
                    WriteLog(string.Format("{0} OnClose fromTimerCancel", SOPWebServer.ClientType.ToString(this.ClientType)));
                    m_nTimerCancelCount = 0;
                    m_timerProcessing = false;
                }

                return;
            }

            m_timerProcessing = true;
            m_nTimerCancelCount = 0;

            // 3초에 한번씩 접속이 끊어진 Client들을 정리해준다.
            if (DateTime.Now.Second % 3 == 0)
            {
                RemoveNotConnectedClients();
            }

            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnTimer, null);

            if (processType == BaseAgent.MethodProcessType.Default)
                OnTimerEvent();
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
                m_agent.RunMethod(BaseAgent.MethodType.OnTimer, null);
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                OnTimerEvent();
                m_agent.RunMethod(BaseAgent.MethodType.OnTimer, null);
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnTimer, null);
                OnTimerEvent();
            }

            m_timerProcessing = false;
        }

        public void OnLoad(DirectDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnLoad, null);

            if (processType == BaseAgent.MethodProcessType.Default)
                OnLoadEvent();
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dbMgr);
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                OnLoadEvent();
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dbMgr);
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dbMgr);
                OnLoadEvent();
            }
        }

        public int OnReceive(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
            {
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }
            else if (header == SOPWebServer.Header.SEND_SMS)
            {
                return SendSMS(header, arrDatas);
            }
            else if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                RemoveClient(data);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnReceive, header);

            if (processType == BaseAgent.MethodProcessType.Default)
                return OnReceiveEvent(data, ctx, header, messages, arrDatas);
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
            {
                object result = m_agent.RunMethod(BaseAgent.MethodType.OnReceive, data.ClientSubType, ctx, header, messages, arrDatas);

                if (result != null && result is int)
                    return (int)result;
            }
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                int nResult = OnReceiveEvent(data, ctx, header, messages, arrDatas);
                object result = m_agent.RunMethod(BaseAgent.MethodType.OnReceive, data.ClientSubType, ctx, header, messages, arrDatas);

                if (result != null && result is int)
                    return (int)result;
                else
                    return nResult;
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnReceive, data.ClientSubType, ctx, header, messages, arrDatas);
                return OnReceiveEvent(data, ctx, header, messages, arrDatas);
            }

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        public void CloseConnection()
        {
            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnClose);

            if (processType == BaseAgent.MethodProcessType.Default)
                OnClose();
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
                m_agent.RunMethod(BaseAgent.MethodType.OnClose);
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                OnClose();
                m_agent.RunMethod(BaseAgent.MethodType.OnClose);
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnClose);
                OnClose();
            }
        }

        protected void WriteLog(string strLog)
        {
            if (m_postOffice != null)
            {
                ILogger logger = m_postOffice.GetLogger();

                if (logger != null)
                {
                    logger.Write(strLog);
                }
            }
        }

        private int SendSMS(int header, ArrayList arrDatas)
        {
            if (m_agentFactory == null)
                return SOPWebServer.ErrorMessageType.CAN_NOT_SEND_SMS;

            BaseSMSManager smsMgr = m_agentFactory.SMSManager;

            if (smsMgr == null)
                return SOPWebServer.ErrorMessageType.CAN_NOT_SEND_SMS;

            int nDataCount = arrDatas.Count;

            if (nDataCount < 3)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            if (arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is int)
            {
                string strCaller = (string)arrDatas[0];
                string strMessage = (string)arrDatas[1];
                int nPhoneNumberCount = (int)arrDatas[2];

                if (nPhoneNumberCount != nDataCount - 3)
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                List<string> phoneNumbers = new List<string>();
                
                for (int i=3;i<nDataCount;i++)
                {
                    phoneNumbers.Add(arrDatas[i].ToString());
                }

                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                int nResult = smsMgr.SendSMS(dbMgr, strCaller, phoneNumbers, strMessage, -1);
                dbMgr.Close();

                return nResult;
            }

            return SOPWebServer.ErrorMessageType.CAN_NOT_SEND_SMS;
        }

        protected virtual void OnTimerEvent()
        {
            while (m_agent.TimerDatas.Count > 0)
            {
                ArrayList arrDatas;

                if (m_agent.TimerDatas.TryDequeue(out arrDatas) == false)
                    break;

                if (arrDatas[0] is ClientData)
                //if (arrDatas[0] is OperationContext)
                {
                    // 특정 클라이언트에게 전송
                    ClientData data = (ClientData)arrDatas[0];
                    //OperationContext ctx = (OperationContext)arrDatas[0];
                    int header = (int)arrDatas[1];
                    byte[] bytes = (byte[])arrDatas[2];

                    //ClientData data = GetClientData(ctx);

                    if (data != null)
                    {
                        if (data.PostMan.ClientChannel.State == CommunicationState.Opened)
                            data.PostMan.OnRing(header, bytes);
                        else
                            RemoveClient(data);
                    }
                    else
                        break;
                }
                else if (arrDatas[0] is int)
                {
                    // 특정타입의 모든 클라이언트에게 전송
                    int nClientType = (int)arrDatas[0];
                    int nClientSubType = (int)arrDatas[1];
                    int header = (int)arrDatas[2];
                    byte[] bytes = (byte[])arrDatas[3];

                    ClientData exceptClient = null;

                    if (arrDatas.Count >= 5 && arrDatas[4] is ClientData)
                    {
                        // 특정타입의 모든 클라이언트 가운데 하나만 빼고 모두 전송
                        exceptClient = (ClientData)arrDatas[4];
                    }

                    SendMessageToClient(nClientType, nClientSubType, header, bytes, exceptClient);
                }
            }
        }

        protected void SendMessageToClient(int nClientType, int nClientSubType, int header, byte[] bytes, ClientData exceptClient)
        {
            List<ClientData> clients = m_dicClients.Values.ToList();
            List<ClientData> removeClients = new List<ClientData>();

            foreach (ClientData client in clients)
            {
                if (client != exceptClient)
                {
                    if (nClientType >= 0 && nClientType != client.ClientType)
                        continue;

                    if (nClientSubType >= 0 && nClientSubType != client.ClientSubType)
                        continue;

                    if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                        client.PostMan.OnRing(header, bytes);
                    else
                        removeClients.Add(client);
                }
                /*if (client != exceptClient && client.ClientSubType == nClientSubType)
                    client.PostMan.OnRing(header, bytes);*/
            }

            foreach (ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            clients.Clear();
            removeClients.Clear();
        }

        /*public void SendClientData(int header, byte[] bytes, OperationContext ctx)
        {
            ClientData data = GetClientData(ctx);

            if (data != null)
            {
                ArrayList arrDatas = new ArrayList();

                arrDatas.Add(data);
                arrDatas.Add(header);
                arrDatas.Add(bytes);

                m_agent.TimerDatas.Enqueue(arrDatas);
            }
        }*/

        public void SendClientData(int header, byte[] bytes, IClientData client)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(client);
            arrDatas.Add(header);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);
        }

        public void SendClientData(int header, byte[] bytes, int nClientType, int nClientSubType, IClientData exceptClient = null)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nClientType);
            arrDatas.Add(nClientSubType);
            arrDatas.Add(header);
            arrDatas.Add(bytes);

            if (exceptClient != null)
                arrDatas.Add(exceptClient);

            m_agent.TimerDatas.Enqueue(arrDatas);
        }

        public bool RemoveClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            if (nClientType == this.ClientType)
            {
                List<Client.ClientData> clients = GetClientDatas();

                foreach (Client.ClientData client in clients)
                {
                    if (client.ClientSubType == nClientSubType && client.IP == strIP && client.Port == nPort)
                    {
                        IClientChannel channel = client.PostMan.ClientChannel;

                        if (channel.State == CommunicationState.Opened)
                            client.PostMan.OnRing(SOPWebServer.Header.CLOSE_CONNECTION, null);

                        RemoveClient(client);
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual void OnRemoveClient(ClientData data)
        {
        }

        protected abstract void OnLoadEvent();
        protected abstract int OnReceiveEvent(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas);

        public virtual void OnClose()
        {
            List<KeyValuePair<string, ClientData>> clients = m_dicClients.ToList();

            foreach (KeyValuePair<string, ClientData> pair in clients)
            {
                IClientChannel channel = pair.Value.PostMan.ClientChannel;
                RemoveClient(pair.Value);

                if (channel.State == CommunicationState.Opened)
                {
                    pair.Value.PostMan.OnRing(SOPWebServer.Header.CLOSE_CONNECTION, null);
                    channel.Close();
                }
            }

            //m_dicClients.Clear();

            /*ClientData client;
            m_dicClients.Clear();

            while (m_clients.Count > 0)
            {
                if (m_clients.TryDequeue(out client))
                {
                    IClientChannel channel = (IClientChannel)client;

                    if (channel.State == CommunicationState.Opened)
                    {
                        client.PostMan.OnRing(SOPWebServer.Header.CLOSE_CONNECTION, null);
                        channel.Close();
                    }
                }
            }*/
        }
    }
}
