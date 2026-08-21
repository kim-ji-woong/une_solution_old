using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using System.Collections;
using DBUtility2;
using System.Threading;

namespace SOPManager.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;

        private bool m_isConnected = false;
        private int m_nPort = -1;

        private bool m_shutdownThread = false;
        private DateTime m_dtLastReceive = DateTime.Now;

        public NetworkWebManager(WebDBManager dbMgr)
        {
            //m_provider = new ClientProvider(this);
            //m_providerInternal = new ClientProviderInternal(this);

            m_dbMgr = dbMgr;

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(ConnectionThread);
            t.Start();
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private string GetSOPWebServerURL()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'SOPWebServerURL' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return m_dbMgr.WebServerURL;

            string strWebServerURL = WebDBManager.GetStringField(arrResult[0]);

            if (strWebServerURL == null)
                return m_dbMgr.WebServerURL;

            return strWebServerURL;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = GetSOPWebServerURL();
                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        private void ConnectionThread()
        {
            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_postBox == null || m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.SOP_MANAGER, SOPWebServer.ClientSubType.SOP_MANAGER))
                        {
                            m_isConnected = true;
                            m_dtLastReceive = DateTime.Now;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastReceive;

                    if (span.TotalSeconds >= 3.0)
                    {
                        m_isConnected = false;
                        m_postBox.Dispose();
                        m_postBox = null;
                    }
                }
                
                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        public void OnMessage(int header, byte[] messages)
        {
            m_dtLastReceive = DateTime.Now;

            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CHANGE_CONFIG)
                ProcessChangedConfig(arrDatas);
        }

        private void ProcessChangedConfig(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                int nClientType = (int)arrDatas[0];
                string strPropertyName = (string)arrDatas[1];
                string strPropertyValue = (string)arrDatas[2];

                if (nClientType == SOPWebServer.ClientType.SDMS && strPropertyName == SOP.SDMSConfig.PropertyName)
                {
                    int nConfigValue;

                    if (int.TryParse(strPropertyValue, out nConfigValue))
                    {
                        bool realoadTeam = false;

                        if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) == (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                            realoadTeam = true;

                        if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                            realoadTeam = true;

                        if (realoadTeam)
                            FormMain.Instance.ReloadTeams();
                    }
                }
            }
        }

        public void SendDeletedActionStepIDs(string strActionStepIDs)
        {
            if (m_isConnected == false)
                return;

            int nActionStepHistoryID;
            ArrayList arrDatas = new ArrayList();

            string[] tokens = strActionStepIDs.Split(',');

            foreach (string strToken in tokens)
            {
                if (int.TryParse(strToken.Trim(), out nActionStepHistoryID))
                    arrDatas.Add(nActionStepHistoryID);
            }

            if (arrDatas.Count > 0)
            {
                // false이면 ActionStep
                bool isActionStepHistory = false;
                int nCount = arrDatas.Count;
                arrDatas.Insert(0, isActionStepHistory);
                arrDatas.Insert(1, nCount);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(SOPWebServer.Header.DELETE_ACTIONSTEP_HISTORY, bytes);
            }
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
                return false;
            }

            bool closeConnection;
            bool result = m_postBox.SendMessage(header, messages, out closeConnection);

            if (closeConnection)
            {
                m_isConnected = false;
            }

            return result;
        }
    }
}
