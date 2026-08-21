using System;
using System.Threading;
using System.Windows.Forms;

namespace AlarmWatcher
{
    public partial class FormMain : Form
    {
        private SensorManager m_sensorManager = new SensorManager();
        private bool m_closeThread = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(TestThread));
            t.Start();
        }

        private void TestThread()
        {
            while (m_closeThread == false)
            {
                m_sensorManager.ReadSensorData();
                System.Threading.Thread.Sleep(10);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closeThread = true;
        }
    }
}
