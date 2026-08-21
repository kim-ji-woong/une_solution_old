using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSMSensorServer
{
    public partial class FormMain : Form
    {     
#if! SERVICE
        private Color m_bAlarmColor = Color.Orange;

        private string m_szBuzStop = "Buz Stop";

        private bool bStart = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true)
                return;
            server = new PSMNetworkServer();

            DBUtility.WebDBManager dbMgr = PSMNetworkServer.Instance.DBManager;

            client = new NetworkClient(dbMgr, null, PSMNetworkServer.Instance.SiteID);

            sensor = new PSMSensorManager(client);

            // 새로 접속하니까 일단 모두 접속이 끊긴 것으로 초기화
            sensor.SaveAllSensorServerInfo(false);

            server.NetworkServerLoad();

#if !SIMULATION
            sensor.BeginServer(GasDetector_OnNotifyAlarm);
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
#endif

            bStart = true;
        }

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if( client != null)
            {
                if( nStatus == 1)
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, (nChannel + 1), true);
                }
                else
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, nStatus, true);
                }
                
                System.Diagnostics.Trace.WriteLine("Alarm : " + nComm + "," + nAlarmUnit + "," + fValue + "," + nChannel + "," + nStatus);
            }
            //MessageBox.Show("COMM : " + nComm + ", Alarm Unit:" + nAlarmUnit + ", Value : " + fValue + ", Alarm : " + (nChannel + 1) + ", Status : " + nStatus);
        }

        private void OnStopServer(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.ClientProvider.Close();
                client.ShutdownSensorThread = true;

            }

            if (sensor != null)
                sensor.StopServer();

            timer1.Stop();
            timer1.Enabled = false;

            bStart = false;
        }

        private void SetProgressBar(float value, Label lb, ProgressBar bar, PictureBox pbOn)
        {
            if (value >= -997.0f && value <= 100.0f)
            {
                lb.Text = value.ToString("F2");

                bar.Value = (int)value;

                pbOn.BackColor = Color.Blue;
                //pbOff.BackColor = this.BackColor;
            }
            else
            {
                bar.Value = 0;
                pbOn.BackColor = Color.Red;
                //pbOff.BackColor = Color.Red;
            }
        }

        private void SetPictureboxState(int nUnit, int nAlarm, PictureBox pb1, PictureBox pb2, PictureBox pb3)
        {
            int nStatus = sensor.Detector.GetStatus(nUnit, nAlarm, 2);
            if (nStatus == 1)
            {
                pb3.BackColor = m_bAlarmColor;
                pb2.BackColor = m_bAlarmColor;
                pb1.BackColor = m_bAlarmColor;
            }
            else
            {
                if (nStatus < 0)
                    pb3.BackColor = Color.Gray;
                else
                    pb3.BackColor = Color.Green;

                nStatus = sensor.Detector.GetStatus(nUnit, nAlarm, 1);
                if (nStatus == 1)
                {
                    pb2.BackColor = m_bAlarmColor;
                    pb1.BackColor = m_bAlarmColor;
                }
                else
                {
                    if (nStatus < 0)
                        pb2.BackColor = Color.Gray;
                    else
                        pb2.BackColor = Color.Green;

                    nStatus = sensor.Detector.GetStatus(nUnit, nAlarm, 0);
                    if (nStatus == 1)
                    {
                        pb1.BackColor = m_bAlarmColor;
                    }
                    else
                    {
                        if (nStatus < 0)
                            pb1.BackColor = Color.Gray;
                        else
                            pb1.BackColor = Color.Green;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            object obj = comboBox1.SelectedItem;
            if( obj == null)
            {
                return;
            }

            int nUnit = Convert.ToInt32(obj);
            if (nUnit < 0)
                return;


            // 알람 유닛0번은 전체 상태를 가져오므로 첫번째 유닛의 상태는 1번부터 시작
            float a = sensor.Detector.GetDensity(nUnit, 1);
            SetProgressBar(a, lbValue1, progressBar1, pbOn1);
            SetPictureboxState(nUnit, 1, pbAlarm1, pbAlarm2, pbAlarm3);

            float b = sensor.Detector.GetDensity(nUnit, 2);
            SetProgressBar(b, lbValue2, progressBar2, pbOn2);
            SetPictureboxState(nUnit, 2, pbAlarm4, pbAlarm5, pbAlarm6);

            float c = sensor.Detector.GetDensity(nUnit, 3);
            SetProgressBar(c, lbValue3, progressBar3, pbOn3);
            SetPictureboxState(nUnit, 3, pbAlarm7, pbAlarm8, pbAlarm9);

            float d = sensor.Detector.GetDensity(nUnit, 4);
            SetProgressBar(d, lbValue4, progressBar4, pbOn4);
            SetPictureboxState(nUnit, 4, pbAlarm10, pbAlarm11, pbAlarm12);

            float f = sensor.Detector.GetDensity(nUnit, 5);
            SetProgressBar(f, lbValue5, progressBar5, pbOn5);
            SetPictureboxState(nUnit, 5, pbAlarm13, pbAlarm14, pbAlarm15);

            float g = sensor.Detector.GetDensity(nUnit, 6);
            SetProgressBar(g, lbValue6, progressBar6, pbOn6);
            SetPictureboxState(nUnit, 6, pbAlarm16, pbAlarm17, pbAlarm18);

            float h = sensor.Detector.GetDensity(nUnit, 7);
            SetProgressBar(h, lbValue7, progressBar7, pbOn7);
            SetPictureboxState(nUnit, 7, pbAlarm19, pbAlarm20, pbAlarm21);

            float i = sensor.Detector.GetDensity(nUnit, 8);
            SetProgressBar(i, lbValue8, progressBar8, pbOn8);
            SetPictureboxState(nUnit, 8, pbAlarm22, pbAlarm23, pbAlarm24);

            float j = sensor.Detector.GetDensity(nUnit, 16);
            SetProgressBar(j, lbValue9, progressBar9, pbOn9);
            SetPictureboxState(nUnit, 16, pbAlarm25, pbAlarm26, pbAlarm27);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;

            if (server != null)
                server.NetworkServerClosing();

            if (client != null)
                client.ReleaseThread();

            if (sensor != null)
                sensor.StopServer();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_szBuzStop = "Buz Stop";
            button3.Text = m_szBuzStop;

            object obj = comboBox1.SelectedItem;
            if( obj != null)
            {
                string szText = obj.ToString();
                int nUnit = -1;
                if(int.TryParse(szText, out nUnit))
                {
                    sensor.Detector.SetControlRegister(nUnit, 5, 0, 1);
                }
                
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_szBuzStop == "Buz Stop")
            {
                m_szBuzStop = "Buz Cont";

                object obj = comboBox1.SelectedItem;
                if (obj != null)
                {
                    string szText = obj.ToString();
                    int nUnit = -1;
                    if (int.TryParse(szText, out nUnit))
                    {
                        sensor.Detector.SetControlRegister(nUnit, 5, 1, 1);
                    }

                }

                
            }
            else
            {
                m_szBuzStop = "Buz Stop";
                object obj = comboBox1.SelectedItem;
                if (obj != null)
                {
                    string szText = obj.ToString();
                    int nUnit = -1;
                    if (int.TryParse(szText, out nUnit))
                    {
                        sensor.Detector.SetControlRegister(nUnit, 5, 1, 0);
                    }

                }
               
            }
            button3.Text = m_szBuzStop;
        }


        private PSMNetworkServer server = null;
        private NetworkClient client = null;
        private PSMSensorManager sensor = null;

        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            #if VALUE_DEBUG
            sensor.Detector.SetStatusClear(1, 1);
#endif
        }

        private void button5_Click(object sender, EventArgs e)
        {
#if VALUE_DEBUG
            sensor.Detector.SetDensity(1, 1, 10.0f);
            client.SendSensorData(1, 1, 0, 1, true);
#endif
        }

        private void button7_Click(object sender, EventArgs e)
        {
#if VALUE_DEBUG
            sensor.Detector.SetDensity(1, 1, 0.0f);
            //client.SendSensorData(1, 1, 0, 0, true);
#endif
        }

#endif
    }
}
