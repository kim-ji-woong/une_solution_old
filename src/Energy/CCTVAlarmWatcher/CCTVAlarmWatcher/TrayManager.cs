using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace CCTVAlarmWatcher
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

        private const int EDNETP_EVENT_LOGIN = 2;
        private const int EDNETP_EVENT_CONNECT = 3;

        private FormMain m_frmOcx = null;
        private string m_strFileName = "config.dat";

        public TrayManager()
        {
            CreateNotifyicon();

            string strTargetFile = System.Configuration.ConfigurationManager.AppSettings.Get("target");

            if (strTargetFile.Length == 0)
            {
                MessageBox.Show("target을 읽어올 수 없습니다.");
            }
            else
            {
                List<NVR> nvrs = ReadNVRDatas();

                if (nvrs == null)
                {
                    MessageBox.Show("CCTV 정보를 읽어올 수 없습니다.");
                }
                else
                {
                    m_frmOcx = new FormMain(nvrs, strTargetFile);
                    m_frmOcx.Show();
                    //m_frmOcx.Hide();
                }
            }
        }

        private List<NVR> ReadNVRDatas()
        {
            if (File.Exists(m_strFileName))
            {
                Dictionary<string, NVR> dicNVRs = new Dictionary<string, NVR>();
                StreamReader reader = new StreamReader(m_strFileName, Encoding.UTF8);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    if (strLine.StartsWith("#"))
                        continue;

                    string[] tokens = strLine.Split(',');
                    string strTag = tokens[0].Trim();

                    if (strTag == "NVR")
                    {
                        ReadNVR(tokens, dicNVRs);
                    }
                    else if (strTag == "CCTV")
                    {
                        ReadCCTV(tokens, dicNVRs);
                    }
                }

                reader.Close();
                return dicNVRs.Values.ToList();
            }

            return null;
        }

        private bool ReadCCTV(string[] tokens, Dictionary<string, NVR> dicNVRs)
        {
            int nTokenCount = tokens.Length;

            if (nTokenCount < 6)
                return false;

            string strID = tokens[1].Trim();
            string strNVRID = tokens[2].Trim();
            string strName = tokens[3].Trim();
            string strChannel = tokens[4].Trim();
            string strFire = tokens[5].Trim();

            int nID, nChannel;

            if (int.TryParse(strID, out nID) == false)
                return false;

            if (int.TryParse(strChannel, out nChannel) == false)
                return false;

            NVR nvr;

            if (dicNVRs.TryGetValue(strNVRID, out nvr) == false)
                return false;

            CCTV cctv = new CCTV();

            cctv.ID = nID;
            cctv.CameraName = strName;
            cctv.Channel = nChannel;

            if (strFire == "0")
                cctv.IsFire = false;
            else if (strFire == "1")
                cctv.IsFire = true;
            else
                return false;

            if (cctv.IsFire)
            {
                if (nTokenCount != 7)
                    return false;

                cctv.FireEventData = tokens[6].Trim();
                nvr.SetFireEventCCTV(cctv.FireEventData, cctv);
            }

            nvr.AddCCTV(cctv);
            return true;
        }

        private bool ReadNVR(string[] tokens, Dictionary<string, NVR> dicNVRs)
        {
            if (tokens.Length != 6)
                return false;

            string strID = tokens[1].Trim();
            string strHost = tokens[2].Trim();
            string strPort = tokens[3].Trim();
            string strUserID = tokens[4].Trim();
            string strPW = tokens[5].Trim();

            int nPort;

            if (int.TryParse(strPort, out nPort) == false)
                return false;

            NVR nvr = new NVR();

            nvr.Host = strHost;
            nvr.Port = nPort;
            nvr.ID = strUserID;
            nvr.Password = strPW;

            dicNVRs[strID] = nvr;
            return true;
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
            m_icon.Icon = global::CCTVAlarmWatcher.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "열화상 CCTV 감시";
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
            Application.Exit();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }
    }
}
