using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertServer.Network
{
    class ClientProvider : TcpLib2.ClientServiceProvider
    {
        public override void OnReceiveData()
        {
            NetworkManager.Instance.OnReceive();

        }

        public override void OnDropConnection()
        {
            //Form1.Instance.OnDropConnection();
        }
    }
}
