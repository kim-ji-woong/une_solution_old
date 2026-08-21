using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HSMSServer2
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private NetworkServer server = null;
        private NetworkClient client = null;

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

          
          
            server = new NetworkServer(dataGridView1);
            server.FormDelegate = this;

            client = NetworkClient.Instance;

            ModelManager model = ModelManager.Instance;

            if (model == null)
            {
                this.Close();
                Application.Exit();
            }
          
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            server.NetworkServerLoad();
            server.StartServer(server.PortNo);
            textBoxPort.Text = server.PortNo.ToString();
        }

        private void btnChangePort_Click(object sender, EventArgs e)
        {
            if (textBoxPort.Text.Length == 0)
            {
                MessageBox.Show("Server Port를 입력하세요.");
                return;
            }

            int nPortNo;

            if (!int.TryParse(textBoxPort.Text, out nPortNo))
            {
                MessageBox.Show("Server Port는 0보다 큰 정수값이어야 합니다.");
                return;
            }

            if (nPortNo <= 0)
            {
                MessageBox.Show("Server Port는 0보다 큰 정수값이어야 합니다.");
                return;
            }

            if (NetworkServer.Instance.PortNo != nPortNo)
            {
                NetworkServer.Instance.NetworkServerClosing();
                NetworkServer.Instance.WritePortToDB(nPortNo);
                NetworkServer.Instance.StartServer(nPortNo);
            }
        }

        public void SetSensorServer(string strServerURL, bool isConnected)
        {
            string strText = "";

            if (isConnected)
                strText = "센서 서버(" + strServerURL + ")와 연결중";
            else
                strText = "센서 서버(" + strServerURL + ")와 연결되지 않음";

            try
            {
                if (this.IsDisposed == false)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        if (strText != labelSensorServer.Text)
                        {
                            labelSensorServer.Text = strText;
                            labelSensorServer.Refresh();
                        }
                    });
                }
            }
            catch (System.Exception)
            {
            	
            }
            
            
        }
    }
}
