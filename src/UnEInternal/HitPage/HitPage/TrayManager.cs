using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Net;

namespace HitPage
{
    public class TrayManager
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

        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;
        private string m_strURL = "";
        private Timer m_timer = null;

        private DBBackup dbBackup = null;

        public TrayManager()
        {
            try
            {
                CreateNotifyicon();

                dbBackup = new DBBackup();

                m_strURL = System.Configuration.ConfigurationManager.AppSettings.Get("url");

                if (m_strURL != null && m_strURL.Length > 0)
                {
                    m_timer = new Timer();
                    // 10분 주기
                    m_timer.Interval = 1000 * 60 * 10;
                    m_timer.Tick += OnTimer;
                    m_timer.Start();

                    OnTimer(null, null);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            try
            {
                KillProcess("chrome");
                System.Diagnostics.Process.Start("chrome", m_strURL);
                //WebClient client = new WebClient();
                //client.DownloadData(m_strURL);
            }
            catch (Exception ex)
            {
                DBBackup.WriteLog("[ERROR] OnTimer : " + ex.Message);
            }

            dbBackup.Run();
        }

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName && process.HasExited == false)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    { }
                    //break;
                }
            }
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::HitPage.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "HitPage";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            DBBackup.WriteLog("HitPage End");

            if (DBBackup.swLog != null)
                DBBackup.swLog.Close();

            Application.Exit();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }
    }
}
