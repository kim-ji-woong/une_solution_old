using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace ControlMonitoring
{
    public partial class ControlMonitoring : Form
    {
        private ArrayList m_arrCompanyMember = new ArrayList();
        private ArrayList m_arrGenLevel = new ArrayList();
        private ArrayList m_arrGenUser = new ArrayList();
        private ArrayList m_arrCheckControl = new ArrayList();
        private ArrayList m_arrUserInfo = new ArrayList();

        //private DBManager m_dbMgr = null;
        private WebDBManager m_dbMgr = null;
        private Thread m_Thread = null;

        private bool m_isStart = false;
        private int m_nPrevUserID = -1;

        public ControlMonitoring()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Visible = true;

            //m_dbMgr = new DBManager(this);
            m_dbMgr = new WebDBManager();
            //m_dbMgr.Load_UserInfo(ref m_arrUserInfo);
            m_nPrevUserID = m_dbMgr.TakeControl(m_nPrevUserID);

            m_isStart = true;
            this.StartDB();

            //m_dbMgr.Load_CheckControl(ref m_arrCheckControl);
            //m_dbMgr.Load_CompanyMember(ref m_arrCompanyMember);
            //m_dbMgr.Load_SOPGenLevel(ref m_arrGenLevel);
            //m_dbMgr.Load_SOPGenUser(ref m_arrGenUser);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Activate();

            this.Visible = true;
            this.ShowInTaskbar = true; // 현재 프로그램을 테스크 바에 표시하게 한다.
            this.WindowState = FormWindowState.Normal; // 폼을 윈도 상태를 normal
        }

        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            m_isStart = false;
            notifyIcon.Visible = false;
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void StartDB()
        {
            m_Thread = new Thread(new ThreadStart(WorkerThreadMethod));
            m_Thread.IsBackground = false;
            m_Thread.Start();
            Thread.Sleep(300);
        }

        public void StopDB()
        {
            try
            {
                if (m_Thread.IsAlive)
                {
                    m_isStart = false;
                    m_Thread.Join(1000);
                    m_Thread.Abort();

                    m_Thread = null;
                }
            }
            catch (Exception ex)
            {
            }
            notifyIcon.Visible = false;
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void WorkerThreadMethod()
        {
            while (m_isStart)
            {
                Thread.Sleep(1000);

                int nControllerID = m_dbMgr.Load_Controller(); //현재 로그인한 사용자 중 제어자가 있는지 확인
                if (nControllerID < 0) //제어자가 없는 경우
                {
                    m_nPrevUserID = m_dbMgr.TakeControl(m_nPrevUserID);
                    continue;
                }

                m_nPrevUserID = nControllerID;
            }
        }

        private void ControlMonitoring_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
