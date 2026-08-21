using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.IO;
using System.Runtime.InteropServices;
using libTTS;

namespace BroadcastServer
{
    public partial class FormMain : Form
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

        public FormMain()
        {
            InitializeComponent();
            ReadFile();

            m_ttsMgr = TTSFactory.MakeInstance();
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

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            timer1.Start();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            this.Hide();
            trayIcon.Visible = true;
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

                if (frm.ShowDialog(this) == DialogResult.OK)
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
            this.Close();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show();
        }

        private void OnTimer(object sender, EventArgs e)
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

        private void tsMenuAdd_Click(object sender, EventArgs e)
        {
            AddSpeech(true);
        }

        private void tsMenuAddNoSiren_Click(object sender, EventArgs e)
        {
            AddSpeech(false);
        }

        private void AddSpeech(bool siren)
        {
            WebDBManager dbMgr = m_dbMgr;

            if (dbMgr == null)
                return;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) values ";
            strSQL += string.Format("('유엔이에서 알려드립니다.\r\n현재 아무일도 없습니다.', {0}, {1}, 1, '{2}', 1)",
                siren ? 1 : 0,
                (int)BroadcastMessage.MesageOption.PLAY,
                strTime);

            dbMgr.GetResultData(strSQL);
        }

        private void tsMenuStop_Click(object sender, EventArgs e)
        {
            WebDBManager dbMgr = m_dbMgr;

            if (dbMgr == null)
                return;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) values ";
            strSQL += string.Format("('', 0, {0}, 1, '{1}', 1)",
                (int)BroadcastMessage.MesageOption.STOP,
                strTime);

            dbMgr.GetResultData(strSQL);
        }

        private void tsMenuPause_Click(object sender, EventArgs e)
        {
            WebDBManager dbMgr = m_dbMgr;

            if (dbMgr == null)
                return;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) values ";
            strSQL += string.Format("('', 0, {0}, 1, '{1}', 1)",
                (int)BroadcastMessage.MesageOption.PAUSE,
                strTime);

            dbMgr.GetResultData(strSQL);
        }

        private void tsMenuResume_Click(object sender, EventArgs e)
        {
            WebDBManager dbMgr = m_dbMgr;

            if (dbMgr == null)
                return;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) values ";
            strSQL += string.Format("('', 0, {0}, 1, '{1}', 1)",
                (int)BroadcastMessage.MesageOption.RESUME,
                strTime);

            dbMgr.GetResultData(strSQL);
        }
    }
}
