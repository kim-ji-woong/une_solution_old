using System;
using System.Text;
using System.Windows.Forms;

namespace TcpLib2
{
	/// <SUMMARY>
	/// EchoServiceProvider. Just replies messages received from the clients.
	/// </SUMMARY>
	public class DefaultServiceProvider: TcpServiceProvider
	{
        public DefaultServiceProvider()
        {
        }

		public override object Clone()
		{
            return new DefaultServiceProvider();
		}

		public override void OnAcceptConnection(ConnectionState state)
		{
			state.LengthAdd = false;
            ServerTest.Form1.Instance.OnAccept(state);
		}

		public override bool OnReceiveData(ConnectionState state)
		{
            if (!base.OnReceiveData(state))
                return false;

            ServerTest.Form1.Instance.OnReceive(state, state.RecivedBuffer);

            return true;
		}

		public override void OnDropConnection(ConnectionState state)
		{
            ServerTest.Form1.Instance.OnDropConnection(state);
		}
	}
}
