using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLWebServiceManager;

namespace SampleProject
{
    public partial class uProgressForm : Form
    {
        public enum Type { Upload = 0, Download }

        private Type m_UpDown = Type.Upload;//is Uploading. Downloading is 1
        ProgressInfo m_progressInfo = null;

        private System.Timers.Timer m_timer = null;

        private DateTime m_dtStartTime;
        public DateTime StartTime
        {
            get { return m_dtStartTime; }
        }

        private static uProgressForm m_instance = null;
        public static uProgressForm Instance
        {
            get { return m_instance; }
        }

        public uProgressForm(Type type, ProgressInfo progressInfo)
        {
            InitializeComponent();
            m_instance = this;

            m_UpDown = type;
            m_progressInfo = progressInfo;

            m_dtStartTime = DateTime.Now;
            lbStartTime.Text = m_dtStartTime.ToString();
        }

        public void SetProgress(string sMessage, int nPercent)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblMessage.Text = sMessage;
                if (m_UpDown == 0)
                    progressBar1.Value = nPercent;
            });
        }

        private void uProgressForm_Load(object sender, EventArgs e)
        {
            if (m_UpDown == Type.Upload)
            {
                lblMessage.Text = "건물데이터 Uploading...";
                progressBar1.Style = ProgressBarStyle.Continuous;
            }
            else
            {
                lblMessage.Text = "건물데이터 Downloading...";
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1000;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            m_timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_progressInfo == null)
                return;

            Console.WriteLine("uProgressForm: " + m_progressInfo.Message + ", " + m_progressInfo.Percent + "%");

            uProgressForm.Instance.SetProgress(m_progressInfo.Message, m_progressInfo.Percent);
        }

        private void uProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain.Instance.CancleWorker();
            m_timer.Stop();
        }
    }
}
