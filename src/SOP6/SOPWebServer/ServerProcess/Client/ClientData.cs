using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProcess.Client
{
    public class ClientData : AgentFactory.IClientData
    {
        private string m_strSessionID = "";
        private IPostMan m_postMan = null;
        private int m_nClientType = 0;
        private int m_nClientSubType = 0;
        private string m_strIP = "";
        private int m_nPort = -1;

        public string SessionID
        {
            get { return m_strSessionID; }
            set { m_strSessionID = value; }
        }

        public IPostMan PostMan
        {
            get { return m_postMan; }
            set { m_postMan = value; }
        }

        public int ClientType
        {
            get { return m_nClientType; }
            set { m_nClientType = value; }
        }

        public int ClientSubType
        {
            get { return m_nClientSubType; }
            set { m_nClientSubType = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public ClientData()
        {
        }

        public ClientData(string strSessionID, IPostMan postMan)
        {
            m_strSessionID = strSessionID;
            m_postMan = postMan;
        }

        public ClientData(string strSessionID, IPostMan postMan, int nClientType, int nClientSubType)
        {
            m_strSessionID = strSessionID;
            m_postMan = postMan;
            m_nClientType = nClientType;
            m_nClientSubType = nClientSubType;
        }
    }
}
