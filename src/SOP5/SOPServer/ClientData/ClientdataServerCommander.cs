using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;

namespace SDMSServer
{
    public class ClientdataServerCommander : ClientData
    {
        public ClientdataServerCommander(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.SERVER_COMMANDER;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.SERVER_COMMAND)
            {
                ProcessServerCommand(bytes, arrDatas);
            }

            return true;
        }

        private void ProcessServerCommand(byte[] bytes, ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount <= 0)
                return;

            int nCommand = (int)(byte)arrDatas[0];

            if (nCommand == (int)ServerCommandType.RUN_SDMS)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.INTEGRATE_MANAGE);
            }
            else if (nCommand == (int)ServerCommandType.UPDATE_SYSTEM)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.INTEGRATE_MANAGE);
                m_provider.SendData(bytes, false, TCP_CLIENT.SERVER_COMMANDER);
            }
        }
    }
}
