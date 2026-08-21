using CrisisAlertServer.Data;
using CrisisAlertServer.Network;
using CrisisAlertServer.Weather;
using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertServer
{
    public partial class FormMain : Form
    {
        private WebServiceManager m_webServiceMgr = null;
        public WebServiceManager WebManager
        {
            get { return m_webServiceMgr; }
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        //private Timer m_timerReload = null;
        private bool m_startService = false;

        private NetworkManager m_netMgr = null;
        public NetworkManager NetworkManager
        {
            get { return m_netMgr; }
        }

        private SMSSendManager m_smsMgr = null;
        public SMSSendManager SMSManager
        {
            get { return m_smsMgr; }
        }

        private WeatherManager m_weawherMgr = null;
        public WeatherManager WeatherManager
        {
            get { return m_weawherMgr; }
        }

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();

            string strWebDBServerURL = ConfigurationManager.AppSettings.Get("WebDBServerURL");
            if (strWebDBServerURL == null || strWebDBServerURL.Length == 0)
                strWebDBServerURL = "http://127.0.0.1";

            m_dbMgr = new WebDBManager(1);
            m_dbMgr.WebServerURL = strWebDBServerURL;
            m_dbMgr.DatabaseName = "SmartCity";
            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            m_dataMgr = new DataManager(m_dbMgr);
            m_webServiceMgr = new WebServiceManager();

            m_netMgr = new NetworkManager(this);
            m_smsMgr = new SMSSendManager(this);
            m_weawherMgr = new WeatherManager(this);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_startService == false)
            {
                WriteMessage("서버 시작");
                lbState.Text = "위기경보수준 서버 실행 중";

                m_startService = true;

                m_weawherMgr.StartThread();
                m_netMgr.StartThread();
                m_smsMgr.StartThread();
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            if (m_startService == true)
            {
                WriteMessage("서버 중지");
                lbState.Text = "위기경보수준 서버 중지";

                m_startService = false;

                m_weawherMgr.StopThread();
                m_netMgr.StopThread();
                m_smsMgr.StopThread();
            }     
        }

        private void WriteMessage(string strMessage)
        {
            string strResult = textBoxResult.Text;

            if (strResult == "")
                textBoxResult.Text = strMessage;
            else
                textBoxResult.Text = strResult + "\r\n" + strMessage; 
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_weawherMgr.Shutdown();
            m_netMgr.Shutdown();
            m_smsMgr.Shutdown();
        }

        public void ShowTextMessage(string strMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (textBoxResult.Text.Length == 0)
                    textBoxResult.AppendText(strMessage);
                else
                    textBoxResult.AppendText("\r\n" + strMessage);
            });
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 처음 자동 실행
            //btnStart_Click(null, null);
        }
    }
}
