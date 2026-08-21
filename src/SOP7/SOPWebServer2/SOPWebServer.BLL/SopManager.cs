using AgentFactory.BLL;
using dnsSopID;
using SOPWebServer.BLL.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SOPWebServer.BLL
{
    public class SopManager
    {
        private MainManager m_mainManager = null;
        private Server.SopServer m_sopServer= null;

        public SopManager(MainManager mainManager, Factory factory)
        {
            m_mainManager = mainManager;
            m_sopServer = new Server.SopServer(mainManager, factory);
        }

        public Result OnReceive(int header, string strClientInfo, ArrayList arrDatas)
        {
            if (header > 0)
                return m_sopServer.OnReceive(header, strClientInfo, arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
        }
    }
}
