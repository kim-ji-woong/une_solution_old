using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace HSMSServer
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;
        private DataManager m_dataMgr = null;
        private NetworkServer m_netServer = null;
        private XMLManager m_xmlMgr = null;
        private bool m_keepGoing = false;

        public HSMSServer.DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_dataMgr = new DataManager();
            m_netServer = new NetworkServer();
            m_xmlMgr = new XMLManager();

            RandomSetting();
        }

        private void SetLocInfo(LocInfo loc)
        {
            if (loc == null)
                return;

            textBoxDeviceID.Text = loc.DeviceID;
            textBoxX.Text = loc.X.ToString();
            textBoxY.Text = loc.Y.ToString();
            textBoxLatitude.Text = loc.Latitude.ToString();
            textBoxLongitude.Text = loc.Longitude.ToString();
            textBoxMethan.Text = loc.MethanGas.ToString();
            textBoxCO.Text = loc.CoGas.ToString();
        }

        public void AddLog(string strLog)
        {
            if (IsDisposed == true)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                if (textBoxLog.Text.Length == 0)
                    textBoxLog.Text = strLog;
                else
                    textBoxLog.Text += "\r\n" + strLog;

                textBoxLog.SelectionStart = textBoxLog.Text.Length;
                textBoxLog.ScrollToCaret();
            });
        }

        public void ClearLog()
        {
            this.Invoke((MethodInvoker)delegate
            {
                textBoxLog.Text = "";
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (textBoxDeviceID.Text.Length == 0)
            {
                MessageBox.Show("DeviceID 값을 입력하세요");
                return;
            }

            if (textBoxX.Text.Length == 0)
            {
                MessageBox.Show("X 값을 입력하세요");
                return;
            }

            if (textBoxY.Text.Length == 0)
            {
                MessageBox.Show("Y 값을 입력하세요");
                return;
            }

            if (textBoxLatitude.Text.Length == 0)
            {
                MessageBox.Show("위도 값을 입력하세요");
                return;
            }

            if (textBoxLongitude.Text.Length == 0)
            {
                MessageBox.Show("경도 값을 입력하세요");
                return;
            }

            if (textBoxMethan.Text.Length == 0)
            {
                MessageBox.Show("메탄가스 값을 입력하세요");
                return;
            }

            if (textBoxCO.Text.Length == 0)
            {
                MessageBox.Show("일산화탄소 값을 입력하세요");
                return;
            }

            double x, y, latitude, longitude, methanGas, coGas;

            if (!double.TryParse(textBoxX.Text, out x))
            {
                MessageBox.Show("X 값은 실수 형태이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxY.Text, out y))
            {
                MessageBox.Show("Y 값은 실수 형태이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxLatitude.Text, out latitude))
            {
                MessageBox.Show("위도 값은 실수 형태이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxLongitude.Text, out longitude))
            {
                MessageBox.Show("경도 값은 실수 형태이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxMethan.Text, out methanGas))
            {
                MessageBox.Show("메탄가스 값은 실수 형태이어야 합니다.");
                return;
            }

            if (!double.TryParse(textBoxCO.Text, out coGas))
            {
                MessageBox.Show("일산화탄소 값은 실수 형태이어야 합니다.");
                return;
            }

            LocInfo loc = new LocInfo();

            loc.DeviceID = textBoxDeviceID.Text;
            loc.X = x;
            loc.Y = y;
            loc.Latitude = latitude;
            loc.Longitude = longitude;
            loc.MethanGas = methanGas;
            loc.CoGas = coGas;

            m_netServer.SendLocInfo(loc);
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            RandomSetting();
        }

        private void RandomSetting()
        {
            DataManager.SensorType type = HSMSServer.DataManager.SensorType.WORKER;

            if (radioWorker.Checked)
                type = HSMSServer.DataManager.SensorType.WORKER;
            else if (radioVehicle.Checked)
                type = HSMSServer.DataManager.SensorType.VEHICLE;
            else if (radioEquip.Checked)
                type = HSMSServer.DataManager.SensorType.EQUIPMENT;
            else
                return;

            LocInfo loc = m_dataMgr.GetRandomLocInfo(type);
            SetLocInfo(loc);
        }

        private void btnSimulation_Click(object sender, EventArgs e)
        {
            if (btnSimulation.Text == "Simulation")
            {
                if (m_xmlMgr.ReadXML(m_xmlMgr.FilePath, m_dataMgr))
                {
                    m_keepGoing = true;
                    btnSimulation.Text = "Stop";

                    Thread t = new Thread(new ThreadStart(SimulationThread));
                    t.Start();
                }
            }
            else if (btnSimulation.Text == "Stop")
            {
                m_keepGoing = false;
            }
        }

        private void SimulationThread()
        {
            int nRunningTime = m_dataMgr.RunningTime;
            int nRepeatCount = m_dataMgr.RepeatCount;

            if (nRepeatCount <= 0)
            {
                while (m_keepGoing)
                {
                    RunSimulation();
                }
            }
            else
            {
                for (int i = 0; i < nRepeatCount; i++)
                {
                    RunSimulation();
                }
            }

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnSimulation.Text = "Simulation";
                });
            }
            catch (System.InvalidOperationException)
            {
            }
        }

        private void RunSimulation()
        {
            int nRunningTime = m_dataMgr.RunningTime;
            ArrayList arrEvents = m_dataMgr.Events;

            int nIndex = 0;
            int nEventCount = arrEvents.Count;

            for (int i = 0; i < nRunningTime && m_keepGoing; i++)
            {
                if (nIndex < nEventCount)
                {
                    EventData data = (EventData)arrEvents[nIndex];

                    if (data.EventTime == i)
                    {
                        foreach (SensorData sensor in data.Sensors)
                        {
                            LocInfo loc = new LocInfo();

                            loc.DeviceID = sensor.SensorID;
                            loc.X = sensor.X;
                            loc.Y = sensor.Y;

                            m_netServer.SendLocInfo(loc);
                        }

                        nIndex++;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}

namespace HSMS
{
    public class SensorWorker
    {
    }

    public class SensorVehicle
    {
    }
}
