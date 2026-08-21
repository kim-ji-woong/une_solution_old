using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;

namespace SOPWebServer
{
    public partial class FormTray : Form, IMainWindow
    {
        private ServiceHost m_serviceHost = null;

        private FormClientList m_frmClientList = null;
        private List<FormClientList.ClientData> m_clientDatas = new List<FormClientList.ClientData>();

        public FormTray()
        {
            InitializeComponent();
        }

        private void FormTray_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            FormMain.InitServiceHost(ref m_serviceHost, this);
        }

        private void FormTray_Resize(object sender, EventArgs e)
        {
            this.Hide();
            trayIcon.Visible = true;
        }

        private void FormTray_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain.CloseService(m_serviceHost);
        }

        private void tsMenuShowClientList_Click(object sender, EventArgs e)
        {
            if (m_frmClientList == null || m_frmClientList.IsDisposed)
                m_frmClientList = new FormClientList();

            if (m_frmClientList.Visible == false)
            {
                m_frmClientList.SetClient(m_clientDatas);
                m_frmClientList.Show();
            }
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show();
        }

        public void AddClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            FormClientList.ClientData data = new FormClientList.ClientData();

            data.ClientType = nClientType;
            data.ClientSubType = nClientSubType;
            data.IP = strIP;
            data.Port = nPort;

            m_clientDatas.Add(data);

            if (m_frmClientList != null && m_frmClientList.IsDisposed == false && m_frmClientList.Visible)
                m_frmClientList.AddClient(nClientType, nClientSubType, strIP, nPort);
        }

        public void RemoveClient(string strIP, int nPort)
        {
            foreach (FormClientList.ClientData data in m_clientDatas)
            {
                if (data.IP == strIP && data.Port == nPort)
                {
                    m_clientDatas.Remove(data);
                    break;
                }
            }

            if (m_frmClientList != null && m_frmClientList.IsDisposed == false && m_frmClientList.Visible)
                m_frmClientList.RemoveClient(strIP, nPort);
        }
    }
}
