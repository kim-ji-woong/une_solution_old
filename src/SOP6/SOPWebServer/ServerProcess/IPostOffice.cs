using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ServerProcess
{
    public interface IPostOffice
    {
        bool SystemClose
        {
            get;
        }

        IPostMan GetPostMan(OperationContext ctx);
        AgentFactory.ILogger GetLogger();
        void OnRemoveClient(Client.ClientData client);
        int SendMessageToClient(int nClientType, int header, byte[] bytes, ArrayList arrDatas);
    }
}
