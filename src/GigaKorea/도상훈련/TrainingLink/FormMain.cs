using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainingLink.Data;

namespace TrainingLink
{
    public partial class FormMain : Form
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        private MessageManager m_messageMgr = null;

        private void ProcessCall(string processName)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == processName)
                {
                    Console.WriteLine(SetForegroundWindow(process.MainWindowHandle).ToString());
                    ShowWindow(process.MainWindowHandle, 9);
                }
            }
        }

        public FormMain()
        {
            // main site id 읽기
            if (ReadConfig("MainSiteID", out m_nSiteID) == false)
                m_nSiteID = 300;
            
            m_dbMgr = new WebDBManager(m_nSiteID);
            m_messageMgr = new MessageManager(this);

            InitializeComponent();

            LoadMessage();
            m_messageMgr.StartThread();

            
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        public int LoadSiteID()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadMessage()
        {
            Dictionary<int, MessageData> dicMessage = new Dictionary<int, MessageData>();
            //dicMessage = m_messageMgr.LoadMessage(); 
            dicMessage = m_messageMgr.LoadSearchMessage(); 

            if (dicMessage == null)
                return;

            foreach (KeyValuePair<int, MessageData> pair in dicMessage)
            {
                MessageData message = pair.Value;
                ShowMessage(message);
            }
        }

        private void ShowMessage(MessageData message)
        {
            if (message == null)
                return;

            gridMessage.Rows.Insert(0, message.Sender, message.Receiver, message.CreateTime.ToString("HH:mm:ss"), message.Message);
            int nCount = gridMessage.Rows.Count;

            lbCount.Text = "Message (" + (nCount) + ")";
        }

        public void ShowThreadMessage(MessageData message)
        {
            if (message == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                gridMessage.Rows.Insert(0, message.Sender, message.Receiver, message.CreateTime.ToString("HH:mm:ss"), message.Message);
                int nCount = gridMessage.Rows.Count;

                lbCount.Text = "Message (" + (nCount) + ")";

                // 메시지 띄울 때 창 앞으로 호출하기
                ProcessCall("TrainingLink");
                
                if (this.Opacity != 100)
                    this.Opacity = 100;
            });
        }

        public void ClearThreadMessage()
        {
            this.Invoke((MethodInvoker)delegate
            {
                gridMessage.Rows.Clear();
                int nCount = gridMessage.Rows.Count;

                lbCount.Text = "Message (" + (nCount) + ")";
            });
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_messageMgr.Shutdown();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gridMessage.Rows.Clear();
            lbCount.Text = "Message (0)";

            m_messageMgr.Search = txtSearch.Text;

            Dictionary<int, MessageData> dicMessage = new Dictionary<int, MessageData>();
            dicMessage = m_messageMgr.LoadSearchMessage();

            if (dicMessage == null)
                return;

            foreach (KeyValuePair<int, MessageData> pair in dicMessage)
            {
                MessageData message = pair.Value;
                ShowMessage(message);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(null, null);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            
        }
    }
}
