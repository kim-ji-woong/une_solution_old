using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TeamReader
{
    public partial class Tray : Form
    {
        private Thread m_Thread = null;
        private bool m_isWorkingThread = false;
        private OwnDataManager m_ownManager = null;
        private CustomerDataManager m_customerManager = null;

        public Tray()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Visible = true;

            m_ownManager = new OwnDataManager();
            m_customerManager = new CustomerDataManager();

            this.StartDB();
        }

        private void tsMenuExit_Click(object sender, EventArgs e)
        {
            m_isWorkingThread = false;
            notifyIcon1.Visible = false;
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
                    m_isWorkingThread = false;
                    m_Thread.Join(1000);
                    m_Thread.Abort();

                    m_Thread = null;
                }
            }
            catch (Exception ex)
            {
            }

            notifyIcon1.Visible = false;
            Application.ExitThread();
            Environment.Exit(0);
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
                
                // 한시간에 한번씩 동작
                Thread.Sleep(1000 * 3600);
            }
        }

        private void Tray_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
