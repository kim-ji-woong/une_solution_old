using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FireSignalSender
{
    public partial class MainForm : Form
    {

        private bool m_bExitProgram = false;
        private NetworkServer mServer = null;

        public MainForm()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            this.ContextMenuStrip = null;

            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
                        
            DateTime dt = DataManager.Instance.LastSignalReadTime;
            label3.Text = Utility.MakeDateTimeString(dt);

            string szPath = DataManager.Instance.PathDB;
            if (szPath == null || szPath == "")
            {
                Application.Exit();
                return;
            }
            
            mServer = new NetworkServer();
            mServer.FormDelegate = this;
            mServer.NetworkServerLoad();

            this.Controls.Add(mServer.DataGridView1);

            mServer.DataGridView1.Location = this.panel1.Location;
            mServer.DataGridView1.Size = this.panel1.Size;
            mServer.DataGridView1.TabIndex = 3;
            mServer.DataGridView1.BringToFront();


            Utility.SetDoubleBuffer(mServer.DataGridView1, true);
            Utility.SetDoubleBuffer(dataGridView1, true);

            timer1.Interval = 2000;
            timer1.Start();
        }
        
        
        private bool SendSignalList()
        {
            bool bResult = false;
            List<FireSignalInfo> arSignals = new List<FireSignalInfo>();

            lock (signalInfoBindingSource)
            {
                foreach (FireSignalInfo info in signalInfoBindingSource)
                {
                    arSignals.Add(info);
                }
            }

            if (arSignals.Count == 0)
                return true;

            arSignals.Sort(FireSignalInfo.CompareSignal);
            //arSignals.Reverse();
            foreach (FireSignalInfo info in arSignals)
            {
                ArrayList ar = new ArrayList();
                ar.Add(Utility.MakeDateTimeString(info.Time));
                ar.Add(info.ReciverNo);
                ar.Add(info.IsOff);
                ar.Add(info.Code);
                ar.Add(info.Circuit);

                bResult = NetworkServer.Instance.ServiceProvider.SendSensorInfo(ar);
            }
            return bResult;
        }

        private void MakeSignalList()
        {
            lock (signalInfoBindingSource)
            {
                signalInfoBindingSource.Clear();


                if (DataManager.Instance.ReadFireSignalList())
                {
                    DateTime dt = DataManager.Instance.LastSignalReadTime;
                    label3.Text = Utility.MakeDateTimeString(dt);

                    dataGridView1.Rows.Clear();

                    List<FireSignalInfo> arSignals = DataManager.Instance.GetSignalList();
                    arSignals.Sort(FireSignalInfo.CompareSignal);
                    foreach (FireSignalInfo info in arSignals)
                    {
                        signalInfoBindingSource.Add(info);
                    }
                    signalInfoBindingSource.Sort = "Time DESC";
                }
            }  
        }
                
        private void button1_Click(object sender, EventArgs e)
        {
            MakeSignalList();                      
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();

            if (SendSignalList())
            {
                MakeSignalList();
            }


            timer1.Enabled = true;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Iconize();       
        }    

        private void Iconize()
        {
            //this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();            
            this.notifyIcon1.Visible = true;
        }

        private void Normalize()
        {
            this.ShowInTaskbar = true;
            this.Show();           
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;

            this.BringToFront();
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Iconize();
            }
            else
            {
                Normalize();
            }
        }
        
        private void 종료하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("이 프로그램을 종료하시면 SOP시스템으로 신호를 전송할 수 없습니다. \n그래도 종료하시겠습니까?", "종료알림", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                // Save info
                m_bExitProgram = true;
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bExitProgram == false)
            {
                e.Cancel = true;
                Iconize();
                return;
            }

            this.notifyIcon1.Visible = false;
            
            if( mServer != null)
            {
                mServer.NetworkServerClosing();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Iconize();
            }            
        }

        
    }    
}
