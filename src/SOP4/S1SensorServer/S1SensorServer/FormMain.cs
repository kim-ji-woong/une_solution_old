using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S1SensorServer
{
    public partial class FormMain : Form
    {     
#if! SERVICE
        private Color m_bAlarmColor = Color.Orange;

        private bool bStart = false;

        public FormMain()
        {
            InitializeComponent();
            label2.Text = "접속안됨";
            label2.ForeColor = Color.Red;
                
        }

        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true)
                return;
            server = new S1NetworkServer();

            DBUtility.WebDBManager dbMgr = S1NetworkServer.Instance.DBManager;

            client = new NetworkClient(dbMgr, null, S1NetworkServer.Instance.SiteID);

        
            server.NetworkServerLoad();

            //label2.Text = "접속중";
            //label2.ForeColor = Color.Green;    

#if !SIMULATION
           
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
#endif

            bStart = true;
        }

        private void OnStopServer(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.ClientProvider.Close();
                client.ShutdownSensorThread = true;
                client = null;
            }

            if (server != null)
            {
                server.NetworkServerClosing();
                server = null;
            }


            timer1.Stop();
            timer1.Enabled = false;

            bStart = false;


            label2.Text = "접속안됨";
            label2.ForeColor = Color.Red;
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
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;

            OnStopServer(null, null);
   
        }

        private S1NetworkServer server = null;
        private NetworkClient client = null;
     

        private void FormMain_Load(object sender, EventArgs e)
        {
            OnBeginServer(null, null);
        }
   
#endif
    }
}
