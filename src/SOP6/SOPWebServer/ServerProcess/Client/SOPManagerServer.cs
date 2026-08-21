using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ServiceModel;
using AgentFactory;

namespace ServerProcess.Client
{
    public class SOPManagerServer : BaseClient
    {
        private static SOPManagerServer m_instance = null;

        public static SOPManagerServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SOP_MANAGER; }
        }

        public SOPManagerServer()
            : base()
        {
            m_instance = this;
        }

        public SOPManagerServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.SOPManager);
        }

        protected override void OnLoadEvent()
        {
        }

        protected override int OnReceiveEvent(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.DELETE_ACTIONSTEP_HISTORY)
                return ProcessDeleteActionStepHistory(arrDatas);

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessDeleteActionStepHistory(ArrayList arrDatas)
        {
            List<int> actionStepHistoryIDs = new List<int>();
            List<int> actionStepIDs = new List<int>();
            int nDataCount = arrDatas.Count;

            if (nDataCount < 2)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            if (arrDatas[0] is bool && arrDatas[1] is int)
            {
                bool isActionStepHistoryID = (bool)arrDatas[0];
                int nCount = (int)arrDatas[1];

                if (nCount >= nDataCount - 1)
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                List<int> list = isActionStepHistoryID ? actionStepHistoryIDs : actionStepIDs;

                for (int i=0;i<nCount;i++)
                {
                    if (arrDatas[i + 2] is int)
                        list.Add((int)arrDatas[i + 2]);
                    else
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                }

                if (nDataCount - nCount > 3)
                {
                    if (arrDatas[nCount + 2] is bool && arrDatas[nCount + 3] is int)
                    {
                        isActionStepHistoryID = (bool)arrDatas[nCount + 2];
                        int nCount2 = (int)arrDatas[nCount + 3];

                        if (nCount2 >= nDataCount - nCount - 3)
                            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                        List<int> list2 = isActionStepHistoryID ? actionStepHistoryIDs : actionStepIDs;

                        for (int i = 0; i < nCount2; i++)
                        {
                            if (arrDatas[i + nCount + 4] is int)
                                list.Add((int)arrDatas[i + nCount + 4]);
                            else
                                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                        }
                    }
                }

                if (Data.SOPSimulatorManager.ServerInstance != null)
                    Data.SOPSimulatorManager.ServerInstance.DeleteActionStepHistory(actionStepHistoryIDs, actionStepIDs);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public void SendChangedConfig(int nConfigData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOP.SDMSConfig.PropertyName);
            arrDatas.Add(nConfigData.ToString());

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_CONFIG, bytes, SOPWebServer.ClientType.SOP_MANAGER, -1);
        }

        protected override void OnTimerEvent()
        {
            base.OnTimerEvent();

            List<Client.ClientData> clients = GetClientDatas();

            if (clients.Count == 0)
                return;

            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                IClientChannel channel = client.PostMan.ClientChannel;

                if (channel.State == CommunicationState.Opened)
                    client.PostMan.OnRing(SOPWebServer.Header.ARE_YOU_THERE, null);
                else
                    removeClients.Add(client);
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();
        }

        protected override void RemoveNotConnectedClients()
        {
            // OnTimerEvent에서 처리한다.
        }
    }
}
