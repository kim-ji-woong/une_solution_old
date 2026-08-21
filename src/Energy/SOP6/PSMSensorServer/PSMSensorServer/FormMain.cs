using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using DBUtility2;

namespace PSMSensorServer
{
    public partial class FormMain : Form
    {     
#if! SERVICE
        private Color m_bAlarmColor = Color.Orange;

        private string m_szBuzStop = "Buz Stop";

        private bool bStart = false;
        LocalDBManager m_dbMgrJubix = null;
        private Utility m_ini = new Utility();

        private PSMNetworkServer server = null;
        private NetworkWebClient client = null;
        private PSMSensorManager sensor = null;
       

        public FormMain()
        {
            InitializeComponent();

            //dbMgr.DatabaseHost = "192.168.0.156";
            //dbMgr.WebServerURL = "http://127.0.0.1:8080/SOP";
           // dbMgr.DatabaseType = WebDBManager.DBType.mysql;
           // dbMgr.DatabaseName = "etadams";

            string strSection = "Jubix Connection Info";
            string strServerIP = m_ini.getinivalue(strSection, "server_ip");
            string strServerPort = m_ini.getinivalue(strSection, "server_port");
            string strServerDB = m_ini.getinivalue(strSection, "server_db");

            m_dbMgrJubix = new LocalDBManager(strServerIP, strServerDB, "mysql", 3);
            //dbMgr.DatabaseHost = strServerIP;
            //dbMgr.WebServerURL = "http://127.0.0.1:8080/JUBIX";
            //dbMgr.DatabaseType = WebDBManager.DBType.mysql;
            //dbMgr.DatabaseName = strServerDB;
            //dbMgr.DatabasePort = strServerPort;
        }

        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true)
                return;
            server = new PSMNetworkServer();

            WebDBManager dbMgr = PSMNetworkServer.Instance.DBManager;

            client = new NetworkWebClient(dbMgr);
            sensor = new PSMSensorManager(client);

           

            string strSection = "Server Connection Info";
            string saveData = m_ini.getinivalue(strSection, "save_data");
            if(saveData == null || saveData == "")
            {
                saveData = "1";
            }
            if (saveData == "1")
                sensor.SavePSMData = true;
            else
                sensor.SavePSMData = false;

            // 새로 접속하니까 일단 모두 접속이 끊긴 것으로 초기화
            sensor.SaveAllSensorServerInfo(false);

            server.NetworkServerLoad();

            sensor.BeginServer(GasDetector_OnNotifyAlarm, FireSensorDetector_OnNotifyAlarm);
            
#if !SIMULATION            
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
#endif

            bStart = true;
        }
        
        void FireSensorDetector_OnNotifyAlarm(int sensorType, int sensorTagID, int sensorZoneID) 
        {
            if (client != null)
            {
                client.SendFireSensorData(sensorType, sensorTagID, sensorZoneID);
            }
        }

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus, int windDirection = -1, int windSpeed = -1)
        {
            if( client != null)
            {
                if( nStatus == 1)
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, (nChannel + 1), true, windDirection, windSpeed);
                }
                else
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, nStatus, true, windDirection, windSpeed);
                }
                
                System.Diagnostics.Trace.WriteLine("Alarm : " + nComm + "," + nAlarmUnit + "," + fValue + "," + nChannel + "," + nStatus);
            }
            //MessageBox.Show("COMM : " + nComm + ", Alarm Unit:" + nAlarmUnit + ", Value : " + fValue + ", Alarm : " + (nChannel + 1) + ", Status : " + nStatus);
        }

        private void OnStopServer(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
                client.ShutdownSensorThread = true;
            }

            if (server != null)
                server.NetworkServerClosing();

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

            int nOffset = 0;
            int nUnit = 1;
            string szUnit = Convert.ToString(obj);
            if (szUnit == "훈련")
            {
                nOffset = 15;
                nUnit = 1;
            }

            // 알람 유닛0번은 전체 상태를 가져오므로 첫번째 유닛의 상태는 1번부터 시작
            float a = sensor.Detector.GetDensity(nUnit, 1 + nOffset);
            SetProgressBar(a, lbValue1, progressBar1, pbOn1);
            SetPictureboxState(nUnit, 1 + nOffset, pbAlarm1, pbAlarm2, pbAlarm3);

            float b = sensor.Detector.GetDensity(nUnit, 2 + nOffset);
            SetProgressBar(b, lbValue2, progressBar2, pbOn2);
            SetPictureboxState(nUnit, 2 + nOffset, pbAlarm4, pbAlarm5, pbAlarm6);

            float c = sensor.Detector.GetDensity(nUnit, 3 + nOffset);
            SetProgressBar(c, lbValue3, progressBar3, pbOn3);
            SetPictureboxState(nUnit, 3 + nOffset, pbAlarm7, pbAlarm8, pbAlarm9);

            float d = sensor.Detector.GetDensity(nUnit, 4 + nOffset);
            SetProgressBar(d, lbValue4, progressBar4, pbOn4);
            SetPictureboxState(nUnit, 4 + nOffset, pbAlarm10, pbAlarm11, pbAlarm12);

            float f = sensor.Detector.GetDensity(nUnit, 5 + nOffset);
            SetProgressBar(f, lbValue5, progressBar5, pbOn5);
            SetPictureboxState(nUnit, 5 + nOffset, pbAlarm13, pbAlarm14, pbAlarm15);

            float g = sensor.Detector.GetDensity(nUnit, 6 + nOffset);
            SetProgressBar(g, lbValue6, progressBar6, pbOn6);
            SetPictureboxState(nUnit, 6 + nOffset, pbAlarm16, pbAlarm17, pbAlarm18);

            float h = sensor.Detector.GetDensity(nUnit, 7+nOffset);
            SetProgressBar(h, lbValue7, progressBar7, pbOn7);
            SetPictureboxState(nUnit, 7 + nOffset, pbAlarm19, pbAlarm20, pbAlarm21);

            float i = sensor.Detector.GetDensity(nUnit, 8 + nOffset);
            SetProgressBar(i, lbValue8, progressBar8, pbOn8);
            SetPictureboxState(nUnit, 8 + nOffset, pbAlarm22, pbAlarm23, pbAlarm24);

            float j = sensor.Detector.GetDensity(nUnit, 9 + nOffset);
            SetProgressBar(j, lbValue9, progressBar9, pbOn9);
            SetPictureboxState(nUnit, 9 + nOffset, pbAlarm25, pbAlarm26, pbAlarm27);

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
             if( sensor != null)
             {

                 sensor.TestReset();
             }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }


       
        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormWeather w = new FormWeather(m_dbMgrJubix);
            w.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            FormAlarm a = new FormAlarm(m_dbMgrJubix);
            a.ShowDialog();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (sensor != null)
            {

                sensor.RequestTestAlarm(1);
            }
        }

#endif
    }
}
