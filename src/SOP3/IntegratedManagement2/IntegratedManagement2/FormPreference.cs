using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace IntegratedManagement2
{
    public partial class FormPreference : Form
    {
        private FormMain m_MainForm = null;
        public FormPreference(FormMain form)
        {
            m_MainForm = form;
            InitializeComponent();

            SetDefaultData();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }

        private void ribbonButtonSetup_Click(object sender, EventArgs e)
        {
            // SAVE DB;
            SaveData();
            //this.DialogResult = DialogResult.OK;
            this.Visible = false;
        }

        private void SetDefaultData()
        {
            string szURL = RegUtil.ReadRegValue("IntegratedManager", "admin_id");
            if (szURL == "")
            {
                RegUtil.WriteRegValue("Server Connection Info", "webserver_url", "http://172.18.101.50:8080/SOP");
                RegUtil.WriteRegValue("Server Connection Info", "dxf_ftp_url", "ftp://172.18.101.50");
                RegUtil.WriteRegValue("Server Connection Info", "time_server", "time2.kriss.re.kr");

                string[] strValues = { "SOPSimulator", "SOPDiaster", "MissionList", "SDMS" };
                
                RegUtil.WriteRegValue("Monitor Info", strValues[0], "1");
                RegUtil.WriteRegValue("Monitor Info", strValues[1], "2");
                RegUtil.WriteRegValue("Monitor Info", strValues[2], "-1");
                RegUtil.WriteRegValue("Monitor Info", strValues[3], "3");

                RegUtil.WriteRegValue("IntegratedManager", "admin_id", "2119");

            }
        }

        public void InitDataLoad()
        {
            string szURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url" );
            string szFtp = RegUtil.ReadRegValue("Server Connection Info", "dxf_ftp_url");
            string szNtp = RegUtil.ReadRegValue("Server Connection Info", "time_server");

            textBox1.Text = szURL;
            textBox2.Text = szFtp;
            textBox3.Text = szNtp;

            string[] strValues = { "SOPSimulator", "SOPDiaster", "MissionList", "SDMS" };
            string [] strRetVal = new string[5];
            strRetVal[0] = RegUtil.ReadRegValue("Monitor Info", strValues[0]);
            strRetVal[1] = RegUtil.ReadRegValue("Monitor Info", strValues[1]);
            strRetVal[2] = RegUtil.ReadRegValue("Monitor Info", strValues[2]);
            strRetVal[3] = RegUtil.ReadRegValue("Monitor Info", strValues[3]);

            ComboBox[] cmbs = { null, comboBox1, comboBox2, comboBox3, comboBox4 };
            //comboBox1.SelectedIndex = 0;
            //comboBox2.SelectedIndex = 0;
            //comboBox3.SelectedIndex = 0;
            //comboBox4.SelectedIndex = 0;

            int nMonitor = -1;
            for( int i = 0 ; i < 4; i++)
            {
                if (int.TryParse(strRetVal[i], out nMonitor))
                {
                    if (nMonitor >= 0 && nMonitor <= 4)
                    {
                        ComboBox cmb = cmbs[nMonitor];
                        cmb.SelectedItem =strValues[i];
                    }
                }
            }
        }

        public void SaveData()
        {
            string szURL = textBox1.Text;
            string szFtp = textBox2.Text;
            string szNtp = textBox3.Text;

            RegUtil.WriteRegValue("Server Connection Info", "webserver_url", szURL);
            RegUtil.WriteRegValue("Server Connection Info", "dxf_ftp_url", szFtp);
            RegUtil.WriteRegValue("Server Connection Info", "time_server", szNtp);
            
            string[] strValues = { "SOPSimulator", "SOPDiaster", "MissionList", "SDMS" };
                        
            string value1 = (string)comboBox1.SelectedItem;
            string value2 = (string)comboBox2.SelectedItem;
            string value3 = (string)comboBox3.SelectedItem;
            string value4 = (string)comboBox4.SelectedItem;

            ArrayList arList = new ArrayList();
            if (!arList.Contains(value1))
                arList.Add(value1);
            if (!arList.Contains(value2))
                arList.Add(value2);
            if (!arList.Contains(value3))
                arList.Add(value3);
            if (!arList.Contains(value4))
                arList.Add(value4);

            if (value1 != "기본 설정" )
                RegUtil.WriteRegValue("Monitor Info", value1, "1");            

            if (value2 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value2, "2");            

            if (value3 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value3, "3");

            if (value4 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value4, "4");

            for (int i = 0; i < 4; i++)
            {                
                if (!arList.Contains(strValues[i]))
                {
                    RegUtil.WriteRegValue("Monitor Info", strValues[i], "-1");
                }
            }

            if( m_MainForm != null)
                m_MainForm.ReloadNetwork();
        }

        private void FormPreference_Load(object sender, EventArgs e)
        {
            ((RibbonButton)ribbonButton1).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
            ((RibbonButton)ribbonButton1).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;

            ((RibbonButton)ribbonButtonSetup).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
            ((RibbonButton)ribbonButtonSetup).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
            
        }

    }
}
