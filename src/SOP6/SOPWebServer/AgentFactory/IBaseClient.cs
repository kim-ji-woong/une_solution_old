using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentFactory
{
    public interface IBaseClient
    {
        void SendClientData(int header, byte[] bytes, IClientData client);
        void SendClientData(int header, byte[] bytes, int nClientType, int nClientSubType, IClientData exceptClient = null);
    }

    public interface IClientData
    {
        int ClientType
        {
            get;
            set;
        }

        int ClientSubType
        {
            get;
            set;
        }

        string IP
        {
            get;
            set;
        }

        int Port
        {
            get;
            set;
        }
    }
}
