using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientTest
{
    public class ClientProvider : TcpLib2.ClientServiceProvider
    {
        public override void OnReceiveData()
        {
            Form1.Instance.OnReceive();
        }

        public override void OnDropConnection()
        {
            Form1.Instance.OnDropConnection();
        }
    }
}
