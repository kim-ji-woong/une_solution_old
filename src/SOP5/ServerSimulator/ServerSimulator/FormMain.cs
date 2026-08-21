using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ServerSimulator
{
    public partial class FormMain : Form, ILogManager
    {
        private NetworkServer m_netServer = new NetworkServer();
        private string m_strOptFile = "config.opt";

        private DateTime m_dtPrev = new DateTime();
        // Key : Time String
        private Dictionary<string, List<byte[]>> m_dicTimeLogs = new Dictionary<string, List<byte[]>>();
        private TimePattern m_pattern = new TimePattern();

        public FormMain()
        {
            InitializeComponent();
            //m_netServer.NetworkServerLoad();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadOptions();
        }

        private void ReadOptions()
        {
            if (File.Exists(m_strOptFile) == false)
                return;

            StreamReader reader = new StreamReader(m_strOptFile, Encoding.Default, true);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strName = strLine.Substring(0, nIndex).Trim();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (string.Compare(strName, "Remember", true) == 0)
                {
                    if (strValue == "1")
                        checkBoxRememberFilePath.Checked = true;
                    else if (strValue == "0")
                        checkBoxRememberFilePath.Checked = false;
                }
                else if (string.Compare(strName, "LogFilePath", true) == 0)
                    textBoxFilePath.Text = strValue;
                else if (string.Compare(strName, "StartTime", true) == 0)
                    textBoxStartTime.Text = strValue;
                else if (string.Compare(strName, "LogTag", true) == 0)
                    textBoxLogTag.Text = strValue;
                else if (string.Compare(strName, "Port", true) == 0)
                    textBoxPort.Text = strValue;
            }

            reader.Close();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Log Files|*.log|All Files|*.*";
            dlg.Title = "읽어들일 로그파일을 선택하세요.";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = dlg.FileName;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            m_netServer.NetworkServerClosing();
            SaveOptions();
        }

        private void SaveOptions()
        {
            StreamWriter writer = new StreamWriter(m_strOptFile, false, Encoding.Default);

            if (checkBoxRememberFilePath.Checked)
            {
                writer.WriteLine("Remember : 1");
                writer.WriteLine("LogFilePath : " + textBoxFilePath.Text.Trim());
                writer.WriteLine("StartTime : " + textBoxStartTime.Text.Trim());
                writer.WriteLine("LogTag : " + textBoxLogTag.Text.Trim());
                writer.WriteLine("Port : " + textBoxPort.Text.Trim());
            }
            else
            {
                writer.WriteLine("Remember : 0");
            }

            writer.Close();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            string strFilePath = textBoxFilePath.Text.Trim();

            if (strFilePath.Length == 0)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("Log File의 경로를 입력하세요.");
                return;
            }

            if (File.Exists(strFilePath) == false)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("존재하지 않는 파일입니다.\r\n파일 경로를 다시 확인하세요.\r\n" + strFilePath);
                return;
            }

            string strStartTime = textBoxStartTime.Text.Trim();

            if (strStartTime.Length == 0)
            {
                textBoxStartTime.Focus();
                MessageBox.Show("시작 시간을 입력해 주세요.");
                return;
            }

            string strTag = textBoxLogTag.Text.Trim();

            if (strTag.Length == 0)
            {
                textBoxLogTag.Focus();
                MessageBox.Show("로그 Tag를 입력해 주세요.");
                return;
            }

            string strPort = textBoxPort.Text.Trim();

            if (strPort.Length == 0)
            {
                textBoxPort.Focus();
                MessageBox.Show("통신 Port를 입력해 주세요.");
                return;
            }

            int nPort = 0;

            if (int.TryParse(strPort, out nPort) == false || nPort <= 0 || nPort >= 65535)
            {
                textBoxPort.Focus();
                MessageBox.Show("통신 포트는 1 ~ 65535 사이의 정수값이어야만 합니다.");
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            btnStartServer.Enabled = false;
            MakeLog(strFilePath, strStartTime, strTag);

            this.Cursor = Cursors.Arrow;
            timer1.Start();

            m_netServer.NetworkServerLoad(nPort, "C:/temp/SensorSimulator.log");
        }

        private void MakeLog(string strFilePath, string strStartTime, string strTag)
        {
            m_dicTimeLogs.Clear();
            m_pattern.ReadPattern(strStartTime);

            StreamReader reader = new StreamReader(strFilePath, Encoding.Default, true);
            int nCodePage = reader.CurrentEncoding.CodePage;
            reader.Close();

            reader = new StreamReader(strFilePath, Encoding.GetEncoding(nCodePage));

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (strLine.Contains(strTag) == false)
                    continue;

                /*string strTime = GetLogTime(strLine);

                if (strTime == null)
                    continue;*/

                m_pattern.ReadLogBytes(strLine, strStartTime, m_dicTimeLogs);
            }

            reader.Close();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Year == m_dtPrev.Year && dtNow.Month == m_dtPrev.Month && dtNow.Day == m_dtPrev.Day &&
                dtNow.Hour == m_dtPrev.Hour && dtNow.Minute == m_dtPrev.Minute && dtNow.Second == m_dtPrev.Second)
                return;

            m_dtPrev = dtNow;
            m_netServer.SendLog(this);
        }

        public List<byte[]> GetLogBytes(DateTime dtBegin)
        {
            TimeSpan span = DateTime.Now - dtBegin;
            DateTime time = m_pattern.BeginTime.AddSeconds(span.TotalSeconds);
            string strTime = m_pattern.ToTimeString(time);

            List<byte[]> byteList = null;

            if (m_dicTimeLogs.TryGetValue(strTime, out byteList))
            {
                return byteList;
            }

            return null;
        }
    }

    public interface ILogManager
    {
        List<byte[]> GetLogBytes(DateTime dtBegin);
    }
}
