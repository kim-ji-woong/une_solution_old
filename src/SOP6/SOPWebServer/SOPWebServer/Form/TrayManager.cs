using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ServiceModel;

namespace SOPWebServer
{
    public class TrayManager : IMainWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        private NotifyIcon m_icon = null;
        private ContextMenuStrip m_contextMenu = null;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ToolStripMenuItem tsMenuShowClientList;
        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;

        private ServiceHost m_serviceHost = null;

        private FormClientList m_frmClientList = null;
        private List<FormClientList.ClientData> m_clientDatas = new List<FormClientList.ClientData>();

        public TrayManager()
        {
            CreateNotifyicon();
            FormMain.InitServiceHost(ref m_serviceHost, this);
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuShowClientList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            
            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuShowClientList,
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::SOPWebServer.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "SOP Web Server";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuShowClientList
            // 
            this.tsMenuShowClientList.Name = "tsMenuShowClientList";
            this.tsMenuShowClientList.Size = new System.Drawing.Size(180, 22);
            this.tsMenuShowClientList.Text = "접속현황 보기";
            this.tsMenuShowClientList.Click += new System.EventHandler(this.tsMenuShowClientList_Click);
            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
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
            FormMain.CloseService(m_serviceHost);
            Application.Exit();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
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