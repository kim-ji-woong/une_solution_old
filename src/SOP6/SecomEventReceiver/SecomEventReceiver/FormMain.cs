using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecomEventReceiver
{
    public partial class FormMain : Form
    {
        private DataManager m_dataMgr = null;
        private NetworkWebManager m_netMgr = null;
        private S1NetworkServer m_sensorServer = null;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            try
            {
                m_dataMgr = new DataManager();
                SetDBConnection(true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SetDBConnection(bool isConnected)
        {
            if (isConnected)
            {
                lblSecomDBStatus.Text = "Secom DB에 접속되었습니다.";
                lblSecomDBStatus.ForeColor = Color.Green;
            }
            else
            {
                lblSecomDBStatus.Text = "Secom DB에 접속되지 못하였습니다.";
                lblSecomDBStatus.ForeColor = Color.Red;
            }
        }

        public void SetServerConnection(string strServerAddr, bool isConnected)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (isConnected)
                {
                    lblServerStatus.Text = string.Format("Server({0})에 접속되었습니다.", strServerAddr);
                    lblServerStatus.ForeColor = Color.Green;
                }
                else
                {
                    lblServerStatus.Text = string.Format("Server({0})에 접속되지 못하였습니다.", strServerAddr);
                    lblServerStatus.ForeColor = Color.Red;
                }
            });
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_netMgr = new NetworkWebManager();
            m_sensorServer = new S1NetworkServer(DataManager.Instance.DBManager);
            m_sensorServer.NetworkServerLoad();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
            m_sensorServer.NetworkServerClosing();
        }
    }
}
