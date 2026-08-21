using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace SOPChecker
{
    public class ServerManager
    {
        protected DBUtility.Utility m_ini = null;

        private static ServerManager m_Instance = null;
        public static ServerManager Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new ServerManager();
                return m_Instance;
            }
        }


        protected SortedList<int, ServerInfo> m_ServerList = new SortedList<int, ServerInfo>();
        public List<ServerInfo> GetServerList()
        {
            List<ServerInfo> result = new List<ServerInfo>();
            result.AddRange(m_ServerList.Values);
            return result;
        }

        protected ServerManager()
        {
            m_ini = new DBUtility.Utility("SOPChecker.ini");
            ReadServerInfo();
        }

        protected virtual void ReadServerInfo()
        {
            string szSection = "Server Info";
            string szServerList = m_ini.getinivalue(szSection, "server");
            if(!string.IsNullOrEmpty(szServerList))
            {
                string[] servers = szServerList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (servers != null && servers.Length > 0)
                {
                    for( int i = 0 ; i < servers.Length ; i++)
                    {
                        string szTemp1 = m_ini.getinivalue(servers[i], "ID");
                        string szTemp2 = m_ini.getinivalue(servers[i], "Path");
                        string szTemp3 = m_ini.getinivalue(servers[i], "File");
                        string szTemp4 = m_ini.getinivalue(servers[i], "Service");
                        string szTemp5 = m_ini.getinivalue(servers[i], "Name");

                        int nID = -1;
                        int.TryParse(szTemp1, out nID);

                        int nService = -1;
                        int.TryParse(szTemp4, out nService);

                        if( nID >= 0)
                        {
                            ServerInfo info = new ServerInfo(szTemp5, szTemp2, szTemp3, nID, nService == 1 ? true : false);
                            m_ServerList.Add(nID, info);
                        }
                    }
                }
            }
        }

        internal ServerInfo GetServer(int nServerID)
        {
            ServerInfo result = null;
            m_ServerList.TryGetValue(nServerID, out result);
            return result;
        }

        internal int GetServerState(ServerInfo info)
        {
            if( info != null)
            {
                if(info.IsService == true)
                {       
                    string szServerName = info.ServerName;
                    bool bResult = false;
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        if (ServiceManager.IsRunningSerivce(szServerName))
                        {
                            bResult = true;
                        }
                    });

                    if (bResult == true)
                        return 1;
                }
                else
                {
                    string szProcessName = info.ServerName;
                    if (ProcessManager.Instance.RunCheckProcess(szProcessName))
                    {
                        return 1;
                    }
                }        
            }
            return 0;
        }
    }

    public class ServerInfo 
    {
        public ServerInfo()
        {
        }

        public ServerInfo(string szServerName, string szFilePath, string szFileName, int nServerID, bool bService)
        {
            m_nServerID = nServerID;
            m_szServerName = szServerName;
            m_szFilePath = szFilePath;
            m_szFileName = szFileName;
            m_bService = bService;
        }

        private int m_nServerID = -1;
        public int ServerID
        {
            get { return m_nServerID; }
            set { m_nServerID = value; }
        }

        private string m_szServerName = "";
        public string ServerName
        {
            get { return m_szServerName; }
            set { m_szServerName = value; }
        }


        private string m_szFilePath = "";
        public string FilePath
        {
            get { return m_szFilePath; }
            set { m_szFilePath = value; }
        }


        private string m_szFileName = "";
        public string FileName
        {
            get { return m_szFileName; }
            set { m_szFileName = value; }
        }

        private bool m_bService = false;
        public bool IsService
        {
            get { return m_bService; }
            set { m_bService = value; }
        }

    }
}
