using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace SOPSimulator.Network
{
    public class ClientProvider : ClientServiceProvider
    {

        public override void OnReceiveData()
        {
            FormMain.Instance.LinkManager.OnReceive();
        }

        public override void OnDropConnection()
        {
            FormMain.Instance.LinkManager.ReConnect();
        }


    }
}
