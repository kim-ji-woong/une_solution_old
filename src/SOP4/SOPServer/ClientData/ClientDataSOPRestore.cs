using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;
using System.Collections;

namespace SDMSServer
{
    public class ClientDataSOPRestore : ClientData
    {
        public ClientDataSOPRestore(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.SOP_RESTORE;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
			if (nHeader == TCP_ID.END_RESTORE)
			{
				SendAllRestart();
			}
            return true;
        }

		private void SendAllRestart()
		{
			Thread t = new Thread(SendAllRestartThread);
			t.Start();
		}

		private void SendAllRestartThread()
		{
			Thread.Sleep(7000);
			NetworkServer.Instance.ServiceProvider.SendAllRestart();
		}


    }
}
