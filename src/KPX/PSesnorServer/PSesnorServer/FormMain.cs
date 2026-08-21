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
using System.IO;
using PSensorServer.Netowrk;

namespace PSensorServer
{
    public partial class FormMain : Form
    {     
#if! SERVICE
        private Color m_bAlarmColor = Color.Orange; 
        private bool bStart = false; 

#if DB_LOG
        private DBUtility.VariousData<DateTime> m_dbLogTime = new DBUtility.VariousData<DateTime>(new DateTime(2017, 6, 29, 0, 0, 0));
        private DateTime m_dtLogBegin = new DateTime(2017, 6, 29, 0, 0, 0);
#endif

        public FormMain()
        {
#if SIMULATOR 
#endif
            InitializeComponent();
            label2.Text = "접속안됨";
            label2.ForeColor = Color.Red; 

            KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AT, false);

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            this.Hide();
        } 

        private KPXLevelMeterManager tm = null;
        private KPXParagonManager pm = null;
        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true)
                return;

            DBUtility.WebDBManager dbMgr = KPXServerManager.Instance.DBManager;
            string szIP = KPXServerManager.Instance.LoggerIP;
            client = new JubixNetworkClient(dbMgr, szIP, KPXServerManager.Instance.SiteID);

            tm = new KPXLevelMeterManager();
            tm.BeginServer(GasDetector_OnNotifyAlarm);

            pm = new KPXParagonManager();
            pm.BeginServer(ParagonPipe_OnNotifyAlarm);

            KPXServerManager.Instance.BeginCommander();

#if !SIMULATION

            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
#endif

            bStart = true; 
        }

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {

        }

        void ParagonPipe_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {

        }

        private void OnStopServer(object sender, EventArgs e)
        {
            KPXServerManager.Instance.StopCommander();

            if (client != null)
            {
                client.ClientProvider.Close();
                client.ReleaseThread();
                client.ShutdownSensorThread = true;
                client = null;
            }

            if (tm != null)
                tm.StopServer();

            if (pm != null)
                pm.StopServer();

            timer1.Stop();
            timer1.Enabled = false;
             
            bStart = false;

            label2.Text = "접속안됨";
            label2.ForeColor = Color.Red;            
        }

        private ArrayList m_btnList = new ArrayList();
        ArrayList arPipe = new ArrayList();
        public void SetPipeButton()
        { 
            List<JubixNetwork.PipeSensor> pipeList = JubixNetwork.JubixSensorManager.Instance.SensorList;
            foreach (JubixNetwork.PipeSensor sensor in pipeList)
            {
                if (sensor.PipeName.IndexOf("100") > 0)
                {
                    arPipe.Add(sensor);
                }
            }
            foreach (JubixNetwork.PipeSensor sensor in pipeList)
            {
                if (sensor.PipeName.IndexOf("100") < 0)
                {
                    arPipe.Add(sensor);
                }
            }  

            int i = 0;
            foreach (JubixNetwork.PipeSensor sensor in arPipe)
            {
                Button btn = new Button();
                btn.Size = new System.Drawing.Size(120, 26);

                int m = i % 3;
                int n = i / 3;
                int x = 0; int y = 0;
                
                y = n * 30 + 30;
                x = m * 130 + 15;

                btn.Location = new System.Drawing.Point(x, y);
                btn.Text  = sensor.PipeName;
                btn.BackColor = Color.Orange;
                btn.Tag = sensor;
                btn.Click += PipeButtonClicked;
                groupBox1.Controls.Add(btn);
                m_btnList.Add(btn);
                i++;
            }          
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if( client != null && client.ClientProvider.IsConnected == true)
            {
                label2.Text = "접속중";
                label2.ForeColor = Color.Green;    
            }
            else
            {
                label2.Text = "접속안됨";
                label2.ForeColor = Color.Red;
            }

            if( m_btnList.Count == 0)
            {
                SetPipeButton();
            }
           

            int i = 0;
            foreach (JubixNetwork.PipeSensor sensor in arPipe)
            {
                Button btn = (Button)m_btnList[i];

                if (sensor.Working == true)
                {
                    btn.BackColor = Color.Green;
                }
                else
                {
                    btn.BackColor = Color.Orange;
                }                
                i++;
            }

            DateTime dtNow = DateTime.Now;

            if (dtNow.Hour == 1 && dtNow.Minute == 0 && dtNow.Second == 0)
                KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AT, false);
        }

        private void PipeButtonClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if(btn != null)
            {
                JubixNetwork.PipeSensor sensor = (JubixNetwork.PipeSensor)btn.Tag;
                if( sensor != null)
                {
                    if(btn.BackColor == Color.Orange)
                    {
                        if( sensor.Working == false)
                        {
                            sensor.BeginWork(-1);
                            sensor.UpdateRecentData();
                        }                        
                    }
                    else
                    {
                        if (sensor.Working == true)
                        {
                            sensor.DoneWork(-1);
                        }
                    }
                }
            }
        }
        bool isClose = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClose)
            {
                e.Cancel = true;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                timer1.Stop();
                timer1.Enabled = false; 

                OnStopServer(null, null);              
            }
            
        }
     
        private JubixNetworkClient client = null;     

        private void FormMain_Load(object sender, EventArgs e)
        {
            OnBeginServer(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        // 경광등 켜기
        private void button3_Click(object sender, EventArgs e)
        {
            KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, true);
        }

        // 경광등 끄기
        private void button4_Click(object sender, EventArgs e)
        {
            KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AR, true);
        }
        
        private void button7_Click(object sender, EventArgs e)
        {
            //libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1");
            //client.SendSMS("01052672290", "01043632290", "[KPX]테스트메시지");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            KPXServerManager.Instance.SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AT, false);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            this.notifyIcon1.Visible = false;
            this.Show();
            this.Activate();
        }

        private FormSimulator form = new FormSimulator();
        private void button2_Click_2(object sender, EventArgs e)
        {
            if( form.Visible == false)
            {
                form.ShowDialog();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool bCheck = checkBox1.Checked;

            KPXParagonManager.Instance.SimulationMode(bCheck);
            KPXLevelMeterManager.Instance.SimulationMode(bCheck);
            JubixNetwork.JubixSensorManager.Instance.SimulationMode(bCheck);
        }
   
#endif 
    }
}
