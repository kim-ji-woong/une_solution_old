using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace DdMonitorSample
{
    public partial class Form1 : Form
    {
        // 1: Global Lock , 2: Local Lock
        int nTest = 1;
        // Local lock Object
        private object m_LockObject = new object();

        // Form Level Lock
        private Form1 form = null;

        private Thread mThread1 = null;
        private Thread mThread2 = null;
        private bool m_bExitThread = false;

        public Form1()
        {
            InitializeComponent();
            form = this;
        }

        private int m_nSleepTime = 1000;
        public void AddThreadFunc1(object param)
        {
            
            int a = 0;
            while(!m_bExitThread)
            {

                DdMonitor.Enter(param);                 
                {
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                    form.Add(ref a);
                    Thread.Sleep(m_nSleepTime);
                }
                DdMonitor.Exit(param);
                
                using (DdMonitor.Lock(form))
                {
                    form.SetText(a);
                    Thread.Sleep(m_nSleepTime);
                }
            }          
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            nTest = 1;

            if (nTest == 1)
            {
                // Global Lock Test
                mThread1 = new Thread(AddThreadFunc1);
                mThread1.Name = "Thread1";
                mThread1.Start(form);
                mThread2 = new Thread(AddThreadFunc1);
                mThread2.Name = "Thread2";
                mThread2.Start(form);
            }
            else
            {
                mThread1 = new Thread(AddThreadFunc1);
                mThread1.Name = "Thread1";
                mThread1.Start(m_LockObject);
                mThread2 = new Thread(AddThreadFunc1);
                mThread2.Name = "Thread2";
                mThread2.Start(m_LockObject);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bExitThread = true;
            if (mThread1 != null)
                mThread1.Join();

            if (mThread2 != null)
                mThread2.Join();
        }

        public void SetText(int a)
        {            
            form.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = a.ToString();
            });
            string szName = Thread.CurrentThread.Name;
            Debug.WriteLine(szName + " SET : " + a.ToString());
        }

        public void Add(ref int a)
        {
            a = a + 1;
            string szName = Thread.CurrentThread.Name;
            Debug.WriteLine(szName + " ADD : " + a.ToString());
        }
        
    }

   
}
