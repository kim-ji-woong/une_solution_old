using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ServerProcess
{
    public interface IPostMan
    {
        IClientChannel ClientChannel
        {
            get;
        }

        Client.ClientData ClientData
        {
            get;
            set;
        }

        void OnRing(int header, byte[] messages);
    }
}
