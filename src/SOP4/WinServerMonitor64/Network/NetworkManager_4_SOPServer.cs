using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;
using System.Threading;

namespace ServerMonitor
{
    public class NetworkManager_4_SOPServer
    {
        private string m_strServerAddr = "";
        private ClientProvider_4_SOPServer m_provider = null;
        private WebDBManager m_dbMgr = null;

        public NetworkManager_4_SOPServer(WebDBManager dbMgr, int nSiteID)
        {
            m_dbMgr = dbMgr;

            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", nSiteID);
            if (strServerURL == null || strServerURL == "")
                strServerURL = dbMgr.WebServerURL;
           
            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);
			m_strServerAddr = addr[0].ToString();

            m_provider = new ClientProvider_4_SOPServer(this);
			
			//m_strServerAddr = "127.0.0.1";
        }

        private int GetServerPort()
		{
            string strSQL = "Select Port from SDMSServerPort";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nPort;
		}

        public bool SendUpdateInform()
        {
            if (!m_provider.IsConnected)
            {
                if (!m_provider.Connect(m_strServerAddr, GetServerPort()))
                    return false;

                if (!m_provider.IsConnected)
                    return false;
            }

            return true;
        }

        public void OnDropConnection()
        {
            m_provider = new ClientProvider_4_SOPServer(this);
        }
    }
}
