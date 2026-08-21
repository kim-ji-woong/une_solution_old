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

            
        }

        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true)
                return;

            this.Controls.Remove(dataGridView1);
            server = new NetworkServer(dataGridView1);

            dataGridView1 = server.DataGridView1;
            
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(10, 74);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 23;
            dataGridView1.Size = new System.Drawing.Size(413, 194);
            Controls.Add(dataGridView1);

            server.FormDelegate = this;           
            


            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
                               
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

            if (server != null)
            {
                server.NetworkServerClosing();
                server = null;
            }


            timer1.Stop();
            timer1.Enabled = false;

            bStart = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;

            OnStopServer(null, null);
   
        }

        private NetworkServer server = null;
  
        private void FormMain_Load(object sender, EventArgs e)
        {
            InitGrid();

            OnBeginServer(null, null);
        }

        private void InitGrid()
        {
            colIndex.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colIP.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIP.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 작업시작
            try
            {
                int nCh = Convert.ToInt32(textBox1.Text);
                //int v = Convert.ToInt32(textBox2.Text);

                NetworkServer.Instance.ServiceProvider.SendWorkStart(nCh);
            }
            catch (Exception)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 알람 생성
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int nCh = Convert.ToInt32(textBox1.Text);
                int v = Convert.ToInt32(textBox2.Text);

                NetworkServer.Instance.ServiceProvider.SetSimValue(nCh, (float)v);
            }
            catch(Exception)
            {

            }
            
        }

   
#endif
    }
}
