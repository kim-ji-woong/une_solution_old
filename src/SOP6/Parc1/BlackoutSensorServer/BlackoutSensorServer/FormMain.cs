using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlackoutSensorServer.Network;
using BlackoutSensorServer.RabbitMQ;
using DBUtility2;

namespace BlackoutSensorServer
{
    public partial class FormMain : Form
    {
        private NetworkWebManager m_netMgr = null;
        private RabbitMQService m_rabbitmqService = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();

            m_instance = this;
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_netMgr = new NetworkWebManager();

            string strRabbitmqIP = System.Configuration.ConfigurationManager.AppSettings["rabbitmqServerIP"].ToString().Trim();
            m_rabbitmqService = new RabbitMQService(strRabbitmqIP, m_netMgr);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.Close();
            m_rabbitmqService.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strState = (radioButton1.Checked) ? radioButton1.Text : radioButton2.Text;
            string strName = comboBox1.SelectedItem.ToString();
            int index = strName.IndexOf('(');
            strName = strName.Substring(0, index - 1);

            m_netMgr.OnBlackoutSignal(strState, strName);
        }

        public void CurrentState(string str)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblState.Text = str;
            });            
        }
    }
}
