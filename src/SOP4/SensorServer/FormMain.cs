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

namespace SensorServer
{



    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;

            if (bEnableLog == true)
                file = new System.IO.StreamWriter(szFullPath + "//SensorServer2.log");
#if WIN
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
#endif
        }

        private static log4net.ILog logger = null;
		private System.Timers.Timer tmrTimer = null;

		private NetworkServer server = null;
        private NetworkClient client = null;

		private static StreamWriter file = null;

		private static bool bEnableLog = true;
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true && file != null)
				file.WriteLine(szMsg);

		}
     #if WIN

    
		private ConManager mDBConMan = null;		

        private void button1_Click(object sender, EventArgs e)
        {
            SensorService.WriteLine("DB Wait Timer");
            if (mDBConMan.OpenConnection())
            {
                SensorService.WriteLine("Open Connection");


                server = new NetworkServer();

                DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

                client = new NetworkClient(dbMgr, null, NetworkServer.Instance.SiteID);

                server.NetworkServerLoad();

                mDBConMan.CloseConnection();

                btnConnect.Enabled = false;
            }		
        }

        private void FormMain_Load(object sender, EventArgs e)
        {


            labelSOPServerConnection.Text = "";
            mDBConMan = new ConManager();

            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        

            if (server != null)
            {
                server.NetworkServerClosing();
            }

            if (client != null)
            {
                client.ReleaseThread();
            }

            if (file != null)
            {
                file.Close();
            }
        }

        public void SetServerConnection(string strIP, bool isConnected)
        {
            string strMsg = "";
            
            if (!isConnected)
                strMsg = "SOP Server(" + strIP + ")와의 접속을 시도하고 있습니다.";
            else
                strMsg = "SOP Server(" + strIP + ")와 연결되었습니다.";

            this.Invoke((MethodInvoker)delegate
            {
                labelSOPServerConnection.Text = strMsg;
            });
        }
#endif
    }

}
