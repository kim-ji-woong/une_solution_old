using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace DivisysCCTVInfo
{
    public class TrayManager : IEventOwner
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
        private System.Windows.Forms.ToolStripMenuItem tsMenuTestAlarm;
        private FormCCTV m_frmCCTV = null;

        private Dictionary<string, int> m_dicEvents = new Dictionary<string, int>();

        private string m_strTargetFile = "CCTVAlarm.txt";
        private string m_strCurrentFile = "";
        private string m_strFolderPath = "";
        private DateTime m_dtCurrent;
        private long m_nPrevFileSize = 0;
        private UTF8Encoding m_encoding = new UTF8Encoding(true);

        public TrayManager()
        {
            CreateNotifyicon();

            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("Port");
            string strHost = System.Configuration.ConfigurationManager.AppSettings.Get("host");
            string strID = System.Configuration.ConfigurationManager.AppSettings.Get("id");
            string strPW = System.Configuration.ConfigurationManager.AppSettings.Get("pw");
            string strLoginData = string.Format("{0}:{1}:{2}:{3}", strHost, strPort, strID, strPW);
            m_strFolderPath = System.Configuration.ConfigurationManager.AppSettings.Get("folderPath");

            ReadFile();

            m_frmCCTV = new FormCCTV(strHost, strPort, strID, strPW, this, null);
            m_frmCCTV.StartPosition = FormStartPosition.Manual;
            // 화면에서 안보이는 곳으로...
            m_frmCCTV.Location = new Point(-10000, -10000);
            m_frmCCTV.ShowInTaskbar = false;
            m_frmCCTV.Show();
        }

        private void ReadFile()
        {
            StreamReader reader = new StreamReader("AlarmCCTVList.txt", Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                if (tokens.Count() != 3)
                    continue;

                int nCCTVID = 0;
                int nEventCode = 0;

                if (int.TryParse(tokens[0].Trim(), out nCCTVID) && int.TryParse(tokens[1].Trim(), out nEventCode))
                {
                    string strKey = string.Format("{0}_{1}", nEventCode, tokens[2].Trim());
                    m_dicEvents[strKey] = nCCTVID;
                }
            }
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuTestAlarm = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuTestAlarm,
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::DivisysCCTVInfo.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "열화상 감시";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuClose
            // 
            this.tsMenuTestAlarm.Name = "tsMenuTestAlarm";
            this.tsMenuTestAlarm.Size = new System.Drawing.Size(180, 22);
            this.tsMenuTestAlarm.Text = "테스트 알람";
            this.tsMenuTestAlarm.Click += new System.EventHandler(this.tsMenuTestAlarm_Click);
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

        private void tsMenuTestAlarm_Click(object sender, EventArgs e)
        {
            AddEvent(5, "4:1");
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }

        public void AddEvent(int nEventCode, string strData)
        {
            int nCCTVID;
            string strKey = string.Format("{0}_{1}", nEventCode, strData);

            if (m_dicEvents.TryGetValue(strKey, out nCCTVID))
            {
                DateTime dtNow = DateTime.Now;

                if (m_strCurrentFile.Length == 0)
                {
                    SetCurrentFile(dtNow);
                    m_nPrevFileSize = 0;
                }
                else if (IsSameDay(dtNow, m_dtCurrent) == false)
                {
                    SetCurrentFile(dtNow);
                    m_nPrevFileSize = 0;
                }

                string strFilePath = m_strFolderPath.EndsWith("\\") ? m_strFolderPath + m_strCurrentFile : m_strFolderPath + "\\" + m_strCurrentFile;

                //if (System.IO.File.Exists(strFilePath) == false)
                //    System.IO.File.Create(strFilePath);

                using (FileStream fs = File.Open(strFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);

                    string strAlarm = string.Format("{0} 0 1 {1}\n", GetDateTimeString(dtNow), nCCTVID);
                    byte[] bytes = m_encoding.GetBytes(strAlarm);

                    fs.Write(bytes, 0, bytes.Count());
                }

                System.Diagnostics.Trace.WriteLine("{0} CCTV Alarm");
            }
        }

        private void SetCurrentFile(DateTime dtNow)
        {
            int nIndex1 = m_strTargetFile.LastIndexOf('\\');
            int nIndex2 = m_strTargetFile.LastIndexOf('.');

            if (nIndex2 < 0 || nIndex2 < nIndex1)
                m_strCurrentFile = m_strTargetFile + GetDateTimeFileString(dtNow);
            else
            {
                string str1 = m_strTargetFile.Substring(0, nIndex2);
                string str2 = m_strTargetFile.Substring(nIndex2);

                m_strCurrentFile = str1 + GetDateTimeFileString(dtNow) + str2;
            }

            m_dtCurrent = dtNow;
        }

        private string GetDateTimeFileString(DateTime timeStamp)
        {
            return string.Format("_{0}{1:00}{2:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day);
        }

        private string GetDateTimeString(DateTime timeStamp)
        {
            return string.Format("{0}-{1:00}-{2:00}_{3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
        }

        private bool IsSameDay(DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
        }
    }
}
