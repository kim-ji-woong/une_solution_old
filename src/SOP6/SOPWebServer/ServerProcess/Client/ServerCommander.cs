using AgentFactory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerProcess.Client
{
    public class ServerCommander : BaseClient
    {
        private static ServerCommander m_instance = null;

        public static ServerCommander Instance
        {
            get { return m_instance; }
        }

        public ServerCommander() : base()
        {
            m_instance = this;
        }

        public ServerCommander(Factory factory, IPostOffice postOffice) : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.SOPCommander);
        }


        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SOP_COMMANDER; }
        }

        protected override void OnLoadEvent()
        {
            
        }

        protected override int OnReceiveEvent(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SERVER_COMMAND)
                return ProcessServerCommand(messages, data, arrDatas);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessServerCommand(byte[] bytes, ClientData data, ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount > 0)
            {
                int nCommand = (int)(byte)arrDatas[0];

                if (nCommand == (int)SOPWebServer.ServerCommandType.RUN_SDMS)
                {
                    //m_provider.SendData(bytes, false, TCP_CLIENT.INTEGRATE_MANAGE);
                    SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER);
                }
                else if (nCommand == (int)SOPWebServer.ServerCommandType.UPDATE_SYSTEM)
                {
                    //m_provider.SendData(bytes, false, TCP_CLIENT.INTEGRATE_MANAGE);
                    //m_provider.SendData(bytes, false, TCP_CLIENT.SERVER_COMMANDER);
                    //LoginServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER);                   
                    //LoginServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.SOP_COMMANDER);
                    LoginServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER);
                    //LoginServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, data);
                    //LoginServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, data);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }
    }
}
