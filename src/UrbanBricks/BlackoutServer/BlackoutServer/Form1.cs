using BlackoutServer.Data;
using BlackoutServer.Network;
using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackoutServer
{
    public partial class Form1 : Form
    {
        private ProcessManager processManager = null;
        private Timer m_timer = null;

        public Form1()
        {
            InitializeComponent();

            processManager = new ProcessManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            if (DataManager.Sensors == null || DataManager.Sensors.Count == 0)
            {
                if(DataManager.Sensors == null)
                    Logger.Instance.Write("DataManager.Sensors is null");
                else if (DataManager.Sensors.Count == 0)
                    Logger.Instance.Write("DataManager.Sensors count 0");
                return;
            }

            foreach (Sensor sensor in DataManager.Sensors)
            {
                if (sensor.SensorName.Contains("호텔"))
                {
                    label1.Text = "호텔 : " + sensor.Data;
                }
                else if (sensor.SensorName.Contains("UTower"))
                {
                    label2.Text = "UTower : " + sensor.Data;
                }
                else if (sensor.SensorName.Contains("TTower"))
                {
                    label3.Text = "TTower : " + sensor.Data;
                }
                else if (sensor.SensorName.Contains("백화점"))
                {
                    label4.Text = "백화점 : " + sensor.Data;
                }
            }

            label5.Text = "Modbus Conneted : " + processManager.NetModbusManager.Provider.IsConnected;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processManager != null)
                processManager.Close();
        }
    }
}
