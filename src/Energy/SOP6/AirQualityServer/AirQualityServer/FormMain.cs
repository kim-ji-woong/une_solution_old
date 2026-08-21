using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Threading;
using System.Collections;

namespace AirQualityServer
{
    public partial class FormMain : Form, IManagerOwner
    {
        private SensorManager m_sensorMgr = null;

        public FormMain()
        {
            InitializeComponent();

            m_sensorMgr = new SensorManager(this, textBoxO2, textBoxCO2, textBoxCO, textBoxCH4, textBoxTemp, textBoxHumi, labelConnectionStatus);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_sensorMgr.RunThread();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_sensorMgr.CloseThread();
        }

        public void SetDBConnectionState(bool isConnected, string strIP)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (isConnected)
                {
                    labelConnectionStatus.Text = "DB 접속 성공";
                    labelConnectionStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelConnectionStatus.Text = "DB 접속 실패(" + strIP + ")";
                    labelConnectionStatus.ForeColor = Color.Red;
                }
            });
        }

        public void UpdateSensorData(Sensor sensor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                TextBox textBox = (TextBox)sensor.Tag;
                textBox.Text = string.Format("{0:F0}", sensor.Value);
            });
        }

        public void SetUnconnectedSensor(Sensor sensor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                TextBox textBox = (TextBox)sensor.Tag;
                textBox.Text = "연결안됨";
            });
        }
    }
}
