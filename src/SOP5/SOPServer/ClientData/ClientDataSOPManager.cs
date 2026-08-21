using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using SDMS;
using System.Collections;

namespace SDMSServer
{
    public class ClientDataSOPManager : ClientData
    {
        private int m_nSiteID = 1;
        public ClientDataSOPManager(ServiceProvider provider)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            ClientType = TCP_CLIENT.SOP_MANAGER;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            return true;
        }
    }
}
