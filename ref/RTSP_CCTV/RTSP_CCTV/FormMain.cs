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
using System.Threading;

namespace RTSP_CCTV
{
    public partial class FormMain : Form, IConnectionManagerOwner
    {
        private string m_strRTSPConfig = "rtsp.ini";
        private string m_strFolderPath = ".\\";
        private bool m_systemInput = false;
        //private bool m_userInput = false;
        private Thread m_threadConnect = null;
        private ConnectionManager m_mgr = null;

        public FormMain()
        {
            InitializeComponent();

            m_mgr = new ConnectionManager(this);
            int nIndex = Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex >= 0)
                m_strFolderPath = Application.ExecutablePath.Substring(0, nIndex + 1);

            ReadConfig();
            panelNoConnect.Visible = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (textBoxFullURL.Text.Trim().Length == 0)
                return;

            //btnConnect.Enabled = false;

            Uri url = new Uri(textBoxFullURL.Text.Trim());
            streamPlayerControl1.StartPlay(url);

            //btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //m_userInput = true;

            //btnStop.Enabled = false;
            m_mgr.Close();
            streamPlayerControl1.Stop();
            //btnConnect.Enabled = true;

            //m_userInput = false;
        }

        private void MakeFullPath()
        {
            if (m_systemInput)
                return;

            string strIP = textBoxIP.Text.Trim();
            string strPort = textBoxPort.Text.Trim();
            string strID = textBoxID.Text.Trim();
            string strPW = textBoxPassword.Text.Trim();
            string strURL = textBoxURL.Text.Trim();

            if (strIP.Length > 0 || strPort.Length > 0)
            {
                string strPath = "rtsp://";

                if (strID.Length > 0)
                    strPath += strID + ":" + strPW + "@";

                strPath += strIP + ":" + strPort;

                if (strURL.Length > 0)
                {
                    if (strURL.StartsWith("/"))
                        strPath += strURL;
                    else
                        strPath += "/" + strURL;
                }

                textBoxFullURL.Text = strPath;
            }
        }

        private void textBoxIP_TextChanged(object sender, EventArgs e)
        {
            MakeFullPath();
        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            MakeFullPath();
        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            MakeFullPath();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            MakeFullPath();
        }

        private void textBoxURL_TextChanged(object sender, EventArgs e)
        {
            MakeFullPath();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteConfig();
        }

        private void ReadConfig()
        {
            string strPath = m_strFolderPath + m_strRTSPConfig;

            if (File.Exists(strPath) == false)
                return;

            m_systemInput = true;
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                int nIndex = strLine.IndexOf('#');

                if (nIndex >= 0)
                    strLine = strLine.Substring(0, nIndex);

                if (strLine.Length == 0)
                    continue;

                nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strTag = strLine.Substring(0, nIndex).Trim();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (strTag == "IP")
                    textBoxIP.Text = strValue;
                else if (strTag == "PORT")
                    textBoxPort.Text = strValue;
                else if (strTag == "ID")
                    textBoxID.Text = strValue;
                else if (strTag == "PW")
                    textBoxPassword.Text = strValue;
                else if (strTag == "URL")
                    textBoxURL.Text = strValue;
                else if (strTag == "FULL")
                    textBoxFullURL.Text = strValue;
            }

            m_systemInput = false;
            reader.Close();
        }

        private void WriteConfig()
        {
            StreamWriter writer = new StreamWriter(m_strFolderPath + m_strRTSPConfig, false, Encoding.UTF8);

            writer.WriteLine("IP : " + textBoxIP.Text.Trim());
            writer.WriteLine("PORT : " + textBoxPort.Text.Trim());
            writer.WriteLine("ID : " + textBoxID.Text.Trim());
            writer.WriteLine("PW : " + textBoxPassword.Text.Trim());
            writer.WriteLine("URL : " + textBoxURL.Text.Trim());
            writer.WriteLine("FULL : " + textBoxFullURL.Text.Trim());

            writer.Close();
        }

        private void streamPlayerControl1_StreamFailed(object sender, WebEye.Controls.WinForms.StreamPlayerControl.StreamFailedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("StreamFailed : " + e.Error);

            panelNoConnect.Invoke((MethodInvoker)delegate
            {
                panelNoConnect.Visible = true;
            });

            btnStop.Enabled = false;
            btnConnect.Enabled = true;
            labelVideoSize.Visible = false;

            m_mgr.OnFail();
        }

        private void streamPlayerControl1_StreamStarted(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("StreamStarted");

            panelNoConnect.Invoke((MethodInvoker)delegate
            {
                panelNoConnect.Visible = false;
            });

            string strSize = string.Format("해상도 : {0}, {1}", streamPlayerControl1.VideoSize.Width, streamPlayerControl1.VideoSize.Height);
            labelVideoSize.Text = strSize;
            labelVideoSize.Visible = true;

            btnStop.Enabled = true;
            btnConnect.Enabled = false;
        }

        private void streamPlayerControl1_StreamStopped(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("StreamStopped");

            btnStop.Enabled = false;
            btnConnect.Enabled = true;
            labelVideoSize.Visible = false;

            m_mgr.OnStop();
            // 예기치 못한 원인으로 인하여 접속이 끊어지면 다시 접속을 시도한다.
            /*if (m_userInput == false && m_threadConnect == null)
            {
                m_threadConnect = new Thread(new ThreadStart(ReconnectThread));
                m_threadConnect.Start();
            }*/
        }

        private void ReconnectThread()
        {
            while (streamPlayerControl1.IsPlaying == false)
            {
                btnConnect_Click(null, null);
                Thread.Sleep(1000);
            }

            m_threadConnect = null;
        }

        public bool IsConnected
        {
            get { return streamPlayerControl1.IsPlaying; }
        }

        public void Connect()
        {
            btnConnect_Click(null, null);
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            Bitmap bmp = streamPlayerControl1.GetCurrentFrame();

            if (bmp == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PNG File|*.png";
            dlg.Title = "저장할 파일명을 입력하세요";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bmp.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void streamPlayerControl1_Resize(object sender, EventArgs e)
        {
            panelNoConnect.Location = streamPlayerControl1.Location;
            panelNoConnect.Size = streamPlayerControl1.Size;
        }
    }

    public class ConnectionManager
    {
        private DateTime m_dtClose = new DateTime();
        // Close() 요청이 발생한 후 실제로 처리되기까지의 유예 시간(초)
        private double m_closeTime = 1.0;
        private Thread m_threadConnect = null;
        private IConnectionManagerOwner m_owner = null;

        public ConnectionManager(IConnectionManagerOwner owner)
        {
            m_owner = owner;
        }

        public void Close()
        {
            m_dtClose = DateTime.Now;
        }

        public void ReleaseThread()
        {
            m_threadConnect = null;
        }

        public void OnStop()
        {
            TimeSpan span = DateTime.Now - m_dtClose;

            // 정상적인 CCTV 종료
            if (span.TotalSeconds <= m_closeTime)
                return;
            else if (m_threadConnect == null)
            {
                m_threadConnect = new Thread(new ThreadStart(ReconnectThread));
                m_threadConnect.Start();
            }
        }

        public void OnFail()
        {
            TimeSpan span = DateTime.Now - m_dtClose;

            // 정상적인 CCTV 종료
            if (span.TotalSeconds <= m_closeTime)
                return;
            else if (m_threadConnect == null)
            {
                m_threadConnect = new Thread(new ThreadStart(ReconnectThread));
                m_threadConnect.Start();
            }
        }

        private void ReconnectThread()
        {
            if (m_owner != null)
            {
                while (m_owner.IsConnected == false && m_threadConnect != null)
                {
                    m_owner.Connect();
                    Thread.Sleep(1000);
                }
            }

            m_threadConnect = null;
        }
    }

    public interface IConnectionManagerOwner
    {
        bool IsConnected
        {
            get;
        }

        void Connect();
    }
}
