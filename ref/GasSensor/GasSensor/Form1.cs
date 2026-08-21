using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialServer
{
    public partial class Form1 : Form
    {
        private GasDetector.DetectorManager dm = new GasDetector.DetectorManager();
        
        private Color m_bAlarmColor = Color.Orange;
        
        private string m_szBuzStop = "Buz Stop";

        public Form1()
        {
            InitializeComponent();            
        }
                        
        private void OnBeginServer(object sender, EventArgs e)
        {
            dm.SetNotify(1, 0, 0, false);
            dm.SetNotify(1, 0, 1, false);
            dm.SetNotify(1, 0, 2, false);

            dm.OnNotifyAlarm += GasDetector_OnNotifyAlarm;
            dm.Start();

            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
        }

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            MessageBox.Show("COMM : " + nComm + ", Alarm Unit:" + nAlarmUnit + ", Value : " + fValue+ ", Alarm : " + (nChannel+1) + ", Status : " + nStatus );
        }

        private void OnStopServer(object sender, EventArgs e)
        {
            dm.OnNotifyAlarm -= GasDetector_OnNotifyAlarm;
            dm.End();

            timer1.Stop();
            timer1.Enabled = false;
        }

        private void SetProgressBar(float value, Label lb, ProgressBar bar, PictureBox pbOn)
        {
            if (value >= 0.0f && value <= 100.0f)
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
            int nStatus = dm.GetStatus(nUnit, nAlarm, 2);
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

                nStatus = dm.GetStatus(nUnit, nAlarm, 1);
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

                    nStatus = dm.GetStatus(nUnit, nAlarm, 0);
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
            int nUnit = comboBox1.SelectedIndex;
            if (nUnit < 0)
                return;
            nUnit += 1;

            // 알람 유닛0번은 전체 상태를 가져오므로 첫번째 유닛의 상태는 1번부터 시작
            float a = dm.GetDensity(nUnit, 1);
            SetProgressBar(a, lbValue1, progressBar1, pbOn1);            
            SetPictureboxState(nUnit, 1, pbAlarm1, pbAlarm2, pbAlarm3);
                        
            float b = dm.GetDensity(nUnit, 2);
            SetProgressBar(b, lbValue2, progressBar2, pbOn2);
            SetPictureboxState(nUnit, 2, pbAlarm4, pbAlarm5, pbAlarm6);

            float c = dm.GetDensity(nUnit, 3);
            SetProgressBar(c, lbValue3, progressBar3, pbOn3);
            SetPictureboxState(nUnit, 3, pbAlarm7, pbAlarm8, pbAlarm9);

            float d = dm.GetDensity(nUnit, 4);
            SetProgressBar(d, lbValue4, progressBar4, pbOn4);
            SetPictureboxState(nUnit, 4, pbAlarm10, pbAlarm11, pbAlarm12);

            float f = dm.GetDensity(nUnit, 5);
            SetProgressBar(f, lbValue5, progressBar5, pbOn5);
            SetPictureboxState(nUnit, 5, pbAlarm13, pbAlarm14, pbAlarm15);
          
        }

        //private byte nRegister = 0;
        //private byte nValue = 1;
        //private byte nFunction = 3;
        //private byte nModbusAddress = 0;
        //// modbus 국번 1byte, 기능 1byte, 데이터 n byte, crc 2byte(hi, low)
        //// start , end 패킷마다 최소 3.5 char 이상
        //private void btnSendMsg(object sender, EventArgs e)
        //{
        //    string szRegister = txtBaseAddress.Text;
        //    if (!byte.TryParse(szRegister, out nValue))
        //    {
        //        return;
        //    }

        //    string szValue = txtHmiAddress.Text;
        //    if (!byte.TryParse(szValue, out nRegister))
        //    {
        //        return;
        //    }

        //    string szFunc = txtFunction.Text;
        //    if (!byte.TryParse(szFunc, out nFunction))
        //    {
        //        return;
        //    }

        //    string szMode = txtModbusAddress.Text;
        //    if (!byte.TryParse(szMode, out nModbusAddress))
        //    {
        //        return;
        //    }

        //    dm.SetControlRegister(nModbusAddress, nFunction, nRegister, nValue);
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;

            dm.End();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_szBuzStop = "Buz Stop";
            button3.Text = m_szBuzStop;

            dm.SetControlRegister(1, 5, 0, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_szBuzStop == "Buz Stop")
            {
                m_szBuzStop = "Buz Cont";
                dm.SetControlRegister(1, 5, 1, 1);
            }
            else
            {
                m_szBuzStop = "Buz Stop";
                dm.SetControlRegister(1, 5, 1, 0);
            }
            button3.Text = m_szBuzStop;             
        }
    }
}
