using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using DBUtility2;
using System.IO;
using System.Runtime.InteropServices;
using libTTS;

namespace BroadcastServer
{
    public class TrayManager
    {
        private enum RunMode { Run = 0, Stop };

        private const string FILE_NAME = "site.ini";

        private WebDBManager m_dbMgr = null;
        private RunMode m_mode = RunMode.Run;

        private ITTSManager m_ttsMgr = null;

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

        private System.Timers.Timer m_timer = new System.Timers.Timer();

        private System.Windows.Forms.ToolStripMenuItem tsMenuChangeSiteID;
        private System.Windows.Forms.ToolStripMenuItem tsMenuStopBroadcast;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRunBroadcast;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;

        public TrayManager()
        {
            CreateNotifyicon();
            ReadFile();

            //m_contextMenu.Items.Remove(tsMenuChangeSiteID);

            m_ttsMgr = TTSFactory.MakeInstance();

            m_timer.Interval = 1000;
            m_timer.Elapsed += OnTimer;
            m_timer.Start();
        }

        private void ReadFile()
        {
            if (File.Exists(FILE_NAME))
            {
                StreamReader reader = new StreamReader(FILE_NAME);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    int nSiteID;

                    if (int.TryParse(strLine, out nSiteID))
                    {
                        m_dbMgr = new WebDBManager(nSiteID);
                        tsMenuStopBroadcast.Enabled = true;
                        break;
                    }
                }

                reader.Close();
            }
        }

        private void WriteFile(int nSiteID)
        {
            StreamWriter writer = new StreamWriter(FILE_NAME);
            writer.Write(nSiteID);
            writer.Close();

            if (m_mode == RunMode.Run)
            {
                if (m_dbMgr == null)
                {
                    tsMenuRunBroadcast.Enabled = tsMenuStopBroadcast.Enabled = false;
                }
                else
                {
                    tsMenuStopBroadcast.Enabled = true;
                    tsMenuRunBroadcast.Enabled = false;
                }
            }
            else// if (m_mode == RunMode.Stop)
            {
                if (m_dbMgr == null)
                {
                    tsMenuRunBroadcast.Enabled = tsMenuStopBroadcast.Enabled = false;
                }
                else
                {
                    tsMenuStopBroadcast.Enabled = false;
                    tsMenuRunBroadcast.Enabled = true;
                }
            }
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuChangeSiteID = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuStopBroadcast = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRunBroadcast = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuChangeSiteID,
            this.tsMenuStopBroadcast,
            this.tsMenuRunBroadcast,
            this.toolStripSeparator1,
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 120);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::BroadcastServer.Properties.Resources.AppSDMS64;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "TTS 방송 서버";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuChangeSiteID
            // 
            this.tsMenuChangeSiteID.Name = "tsMenuChangeSiteID";
            this.tsMenuChangeSiteID.Size = new System.Drawing.Size(180, 22);
            this.tsMenuChangeSiteID.Text = "Site ID 변경";
            this.tsMenuChangeSiteID.Click += new System.EventHandler(this.tsMenuChangeSiteID_Click);
            // 
            // tsMenuStopBroadcast
            // 
            this.tsMenuStopBroadcast.Enabled = false;
            this.tsMenuStopBroadcast.Name = "tsMenuStopBroadcast";
            this.tsMenuStopBroadcast.Size = new System.Drawing.Size(180, 22);
            this.tsMenuStopBroadcast.Text = "방송 중단";
            this.tsMenuStopBroadcast.Click += new System.EventHandler(this.tsMenuStopBroadcast_Click);
            // 
            // tsMenuRunBroadcast
            // 
            this.tsMenuRunBroadcast.Enabled = false;
            this.tsMenuRunBroadcast.Name = "tsMenuRunBroadcast";
            this.tsMenuRunBroadcast.Size = new System.Drawing.Size(180, 22);
            this.tsMenuRunBroadcast.Text = "방송 재개";
            this.tsMenuRunBroadcast.Click += new System.EventHandler(this.tsMenuRunBroadcast_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_contextMenu.Show();
        }

        private void tsMenuChangeSiteID_Click(object sender, EventArgs e)
        {
            PointInter pt;

            if (GetCursorPos(out pt))
            {
                int nSiteID = 0;

                if (m_dbMgr != null)
                    nSiteID = m_dbMgr.SiteID;

                FormSite frm = new FormSite(nSiteID);
                frm.Location = new Point(pt.X - frm.Size.Width, pt.Y - frm.Size.Height);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.SiteID != nSiteID)
                    {
                        if (frm.SiteID == 0)
                            m_dbMgr = null;
                        else
                            m_dbMgr = new WebDBManager(frm.SiteID);

                        WriteFile(frm.SiteID);
                    }
                }
            }
        }

        private void tsMenuStopBroadcast_Click(object sender, EventArgs e)
        {
            m_mode = RunMode.Stop;

            tsMenuStopBroadcast.Enabled = false;
            tsMenuRunBroadcast.Enabled = m_dbMgr != null;

            m_ttsMgr.StopSpeech();
        }

        private void tsMenuRunBroadcast_Click(object sender, EventArgs e)
        {
            m_mode = RunMode.Run;

            tsMenuRunBroadcast.Enabled = false;
            tsMenuStopBroadcast.Enabled = m_dbMgr != null;
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_mode == RunMode.Run)
            {
                WebDBManager dbMgr = m_dbMgr;

                if (dbMgr != null)
                {
                    m_ttsMgr.CheckRequest(dbMgr);
                }
            }
        }
    }
}
