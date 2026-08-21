using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using ControlMonitoring;

namespace TeamReader
{
    public class TeamReader
    {
		private WebDBManager m_dbMgr = null;
		
        private Thread m_Thread = null;
        private bool m_isWorkingThread = false;
        private OwnDataManager m_ownManager = null;
        private CustomerDataManager m_customerManager = null;

		public TeamReader(WebDBManager db)
        {
			m_dbMgr = db;
            Init();
        }

        private void Init()
        {
			m_ownManager = new OwnDataManager(m_dbMgr);
			try
			{
				m_customerManager = new CustomerDataManager();
			}
			catch (System.Exception)
			{
				m_customerManager = null;
			}
            

            this.StartDB();
        }

		public void StartDB()
        {
            m_Thread = new Thread(new ThreadStart(WorkerThreadMethod));
            m_Thread.IsBackground = false;
            m_Thread.Start();
        }

        public void StopDB()
        {
            try
            {
                if (m_Thread.IsAlive)
                {
                    m_isWorkingThread = false;
                    m_Thread.Join(1000);
                    m_Thread.Abort();

                    m_Thread = null;
                }
            }
            catch (Exception)
            {
				m_isWorkingThread = false;
            }          
        }

        public void WorkerThreadMethod()
        {
            m_isWorkingThread = true;
            
            while (m_isWorkingThread)
            {
                if (m_ownManager.Load())
                {
                    if (m_customerManager.Load())
                    {
                        m_ownManager.UpdateData(m_customerManager);
                    }
                }
                
                // 6시간에 한번씩 동작
                Thread.Sleep(1000 * 3600 * 6);
            }
        }
    }
}
