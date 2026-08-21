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
using System.IO;
using System.Windows.Forms;

using MessageServer;

namespace MessageServer
{
    public partial class FormMain : Form
    {	

        //private DBUtility.WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();
        //public DBUtility.WebDBManager DBManager
        //{
        //    get { return m_dbMgr; }
        //    set { m_dbMgr = value; }
        //}
        private static FormMain m_instance = null;

        private bool m_finishProcess = false;

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

		private MessageBroker server = null;
		private static StreamWriter file = null;

		private static bool bEnableLog = true;
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true && file != null)
				file.WriteLine(szMsg);
		}

        public FormMain()
        {
            m_instance = this;

            int nSiteID = LoadSiteID();
            //m_dbMgr = new DBUtility.WebDBManager(nSiteID);
			server = new MessageBroker();

			InitializeComponent();

			SMSDBManager.Instance.Connect();           
        }

        public int LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

		private void FormMain_Load(object sender, EventArgs e)
		{
			try
			{
				log4net.Config.XmlConfigurator.Configure();
			}
			catch (System.Exception ex)
			{

			}

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			
            MessageService.EnableLog = true;

			server.MessageLoop();
		}
		      

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (server != null)
			{
				server.Close();
			}

			if (file != null)
			{
				file.Close();
			}

			SMSDBManager.Instance.Close();
        }

		private void btnInitMsgQueue_Click(object sender, EventArgs e)
		{

		}		
    }
}
