using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Collections;
using System.Threading;
using TeamReader;

namespace ControlMonitoring
{
    public partial class ControlMonitor : ServiceBase
    {

        private ArrayList m_arrCompanyMember = new ArrayList();
        private ArrayList m_arrGenLevel = new ArrayList();
        private ArrayList m_arrGenUser = new ArrayList();
        private ArrayList m_arrCheckControl = new ArrayList();
        private ArrayList m_arrUserInfo = new ArrayList();

        private WebDBManager m_dbMgr = null;
		private TeamReader.TeamReader reader = null;
        private Thread m_Thread = null;

        private bool m_isStart = false;
        private int m_nPrevUserID = -1;

        public ControlMonitor()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_isStart = true;
			m_dbMgr = new WebDBManager();
			reader = new TeamReader.TeamReader(m_dbMgr);
			reader.StartDB();
			StartDB();
        }

        protected override void OnStop()
        {
			reader.StopDB();

            m_isStart = false;
		
            Thread.Sleep(1000);
            StopDB();			
        }
        
        public void WorkerThreadMethod()
        {
            
            m_nPrevUserID = m_dbMgr.TakeControl(m_nPrevUserID);

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
                    m_Thread.Abort();
                    m_Thread = null;
                }
            }
            catch (Exception)
            {
            }  
        }
    }
}


