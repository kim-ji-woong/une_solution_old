using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Reflection;
using System.ServiceModel;
using DBUtility2;

namespace SOPWebServer
{
	public partial class SOPWebService : ServiceBase
	{
        private ServiceHost m_serviceHost = null;

        public SOPWebService()
		{
			InitializeComponent();
		}

        #pragma warning disable
        protected override void OnStart(string[] args)
		{
            string strWebServerURL = "", strDBName = "", strID = "", strPW = "";
            int nDBType = 0;
            int nSiteID = ReadSiteID(ref strWebServerURL, ref strDBName, ref nDBType, ref strID, ref strPW);
            PostOffice.Instance.Start(nSiteID, strWebServerURL, strDBName, nDBType, strID, strPW);

            m_serviceHost = new ServiceHost(typeof(PostBoxService));
            m_serviceHost.Open();

            SaveServicePort(m_serviceHost, nSiteID, strWebServerURL, strDBName, nDBType, strID, strPW);
        }

		protected override void OnStop()
		{
            if (m_serviceHost != null)
            {
                PostOffice.Instance.Stop();
                m_serviceHost.Close();
            }
        }

        private int GetPort(string strURL)
        {
            int nIndex = strURL.LastIndexOf(':');

            if (nIndex < 0)
                return 0;

            int nIndex2 = strURL.IndexOf('/', nIndex + 1);

            string strPort = "";

            if (nIndex2 < 0)
                strPort = strURL.Substring(nIndex + 1).Trim();
            else
                strPort = strURL.Substring(nIndex + 1, nIndex2 - nIndex - 1);

            int nPort = 0;

            if (int.TryParse(strPort, out nPort) == false)
                nPort = 80;

            return nPort;
        }

        private void SaveServicePort(ServiceHost host, int nSiteID, string strWebServerURL, string strDBName, int nDBType, string strID, string strPW)
        {
            DirectDBManager dbMgr = DirectDBManager.MakeInstance((DirectDBManager.DBType)nDBType, strWebServerURL, strID, strPW, strDBName);
            dbMgr.SiteID = nSiteID;
            //WebDBManager dbMgr = new WebDBManager(nSiteID);
            //dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;
            //dbMgr.WebServerURL = strWebServerURL;

            if (dbMgr.Connect() == false)
                return;

            foreach (System.ServiceModel.Description.ServiceEndpoint ep in m_serviceHost.Description.Endpoints)
            {
                string strURL = ep.Address.ToString().ToLower();
                int nPort = GetPort(strURL);

                if (strURL.EndsWith("mex"))
                    SaveServerPort(dbMgr, nPort, ServerPort.SOP_WEB_SERVER_MEX);
                else
                    SaveServerPort(dbMgr, nPort, ServerPort.SOP_WEB_SERVER);
            }

            dbMgr.Close();
        }

        private void SaveServerPort(DirectDBManager dbMgr/*WebDBManager dbMgr*/, int nPort, string strPortName)
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + strPortName + "' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = string.Format("Insert into SensorServerPort (Port, SiteID, Name) values ({0}, {1}, '{2}')",
                    nPort, dbMgr.SiteID, strPortName);
                dbMgr.GetResultData(strSQL);
            }
            else
            {
                strSQL = string.Format("Update SensorServerPort set Port = {0} where Name = '{1}' and SiteID = {2}",
                    nPort, strPortName, dbMgr.SiteID);
                dbMgr.GetResultData(strSQL);
            }
        }

        public static int ReadSiteID(ref string strWebServerURL, ref string strDBName, ref int nDBType, ref string strID, ref string strPW)
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString();
            string strEncName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            string strDBType = System.Configuration.ConfigurationManager.AppSettings["dbtype"].ToString();
            string strEncDB = System.Configuration.ConfigurationManager.AppSettings["ip"].ToString();

            strDBName = ServerProcess.Data.MemberManager.Convert(strEncName);
            string strIDnPW = ServerProcess.Data.MemberManager.Convert(strEncDB);

            int nIndex = strIDnPW.IndexOf('|');

            if (nIndex > 0)
            {
                strID = strIDnPW.Substring(0, nIndex);
                strPW = strIDnPW.Substring(nIndex + 1);
            }

            int.TryParse(strDBType, out nDBType);

            int nSiteID = 0;
            int.TryParse(strSiteID, out nSiteID);
            return nSiteID;
        }
        /*private int ReadSiteID(out string strWebServerURL, out string strDBName, out int nDBType)
        {
            strWebServerURL = strDBName = "";
            nDBType = 0;

            DBUtility2.Utility util = new DBUtility2.Utility();
            string strValue = util.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strValue == null || strValue.Length == 0)
                return nSiteID;

            int.TryParse(strValue, out nSiteID);

            strWebServerURL = util.getinivalue("Server Connection Info", "webserver_url");
            strDBName = util.getinivalue("Server Connection Info", "db_name");
            string strDBType = util.getinivalue("Server Connection Info", "db_type");

            int.TryParse(strDBType, out nDBType);

            return nSiteID;
        }*/
    }
}
