using System;
using System.Text;
using System.Windows.Forms;
using TcpLib2;

namespace BroadcastServer
{
	/// <SUMMARY>
	/// EchoServiceProvider. Just replies messages received from the clients.
	/// </SUMMARY>
	public class ServiceProvider : TcpServiceProvider
	{
        public ServiceProvider()
        {
        }

		public override object Clone()
		{
            return new ServiceProvider();
		}

		public override void OnAcceptConnection(ConnectionState state)
		{
#if SERVICE
			BroadcastServerService.Instance.OnAccept(state);
#else
			BroadcastServer.FormMain.Instance.OnAccept(state);
#endif
		}

		public override bool OnReceiveData(ConnectionState state)
		{
            if (!base.OnReceiveData(state))
                return false;

#if SERVICE
			//BroadcastServer.BroadcastServerService.Instance.OnReceive(state, ReceivedData);
#else
			BroadcastServer.FormMain.Instance.OnReceive(state, ReceivedData);
#endif

			return true;
		}

		public override void OnDropConnection(ConnectionState state)
		{
#if SERVICE
			BroadcastServer.BroadcastServerService.Instance.OnDropConnection(state);
#else
			BroadcastServer.FormMain.Instance.OnDropConnection(state);
#endif
		}
	}
}
