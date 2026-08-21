using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace IntegratedManagement4
{
    public partial class FormPreference : Form
    {
        private FormMain m_MainForm = null;

        private int m_nSiteID = 1;

        private double m_WindowRateWidth= 1d;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1d;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public FormPreference(FormMain form)
        {
            m_nSiteID = FormMain.Instance.SiteID;

            m_MainForm = form;
            InitializeComponent();

            SetDefaultData();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void ribbonButtonSetup_Click(object sender, EventArgs e)
        {
            // SAVE DB;
            SaveData();
            
            // 바뀐 서버 정보로 SiteName을 설정한다.
            string szSiteName = GetSiteName(m_nSiteID);
            if (szSiteName != "")
            {
                lbSiteName.Text = szSiteName;
            }
            else
            {
                lbSiteName.Text = "대상 본부 지정";
            } 

            this.Visible = false;
        }

        private string GetSiteName(int nSiteID)
        {
            string szText = "SELECT SiteName FROM Site WHERE ID = {0}";

            string szSQL = string.Format(szText, nSiteID);

            ArrayList arResult = m_MainForm.DBManager.GetResultData(szSQL);
            if (arResult == null || arResult.Count == 0)
            {
                return "";
            }

            return arResult[0].ToString();
        }

        private void SetDefaultData()
        {
            string szURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if (szURL == "")
            {
                WebDBManager dbMgr = FormMain.Instance.DBManager;

                string strWebServerURL, strDxfFtpURL, strTimeServer, strAdminID;
                SetDefaultData(m_nSiteID, out strWebServerURL, out strDxfFtpURL, out strTimeServer, out strAdminID);
                /*string strWebServerURL = dbMgr.LoadIni("webserver_url", "Server Connection Info");
                string strDxfFtpURL = dbMgr.LoadIni("dxf_ftp_url", "Server Connection Info");
                string strTimeServer = dbMgr.LoadIni("time_server", "Server Connection Info");
                string strAdminID = dbMgr.LoadIni("admin_id", "IntegratedManager");*/

                RegUtil.WriteRegValue("Server Connection Info", "webserver_url", strWebServerURL, m_nSiteID);
                RegUtil.WriteRegValue("Server Connection Info", "dxf_ftp_url", strDxfFtpURL, m_nSiteID);
                RegUtil.WriteRegValue("Server Connection Info", "time_server", strTimeServer, m_nSiteID);

                string[] strValues = { "SOPSimulator", "SDMS", "MissionList", "CCTV" };

                RegUtil.WriteRegValue("Monitor Info", strValues[0], "2", m_nSiteID);
                RegUtil.WriteRegValue("Monitor Info", strValues[1], "1", m_nSiteID);
                RegUtil.WriteRegValue("Monitor Info", strValues[2], "-1", m_nSiteID);
                RegUtil.WriteRegValue("Monitor Info", strValues[3], "-1", m_nSiteID);

                RegUtil.WriteRegValue("IntegratedManager", "admin_id", strAdminID, m_nSiteID);
                RegUtil.WriteRegValue("IntegratedManager", "cctv_mode", "1", m_nSiteID);

                lbSiteName.Text = "대상 본부 지정";
            }
            else
            {
                string szSiteName = GetSiteName(m_nSiteID);
                if( szSiteName != "")
                {
                    lbSiteName.Text = szSiteName;
                }
                else
                {
                    lbSiteName.Text = "대상 본부 지정";
                }               
            }
        }

        private void SetDefaultData(int nSiteID, out string strWebServerURL, out string strDxfFtpURL, out string strTimeServer, out string strAdminID)
        {
            // 삼천포
            if (nSiteID == 1)
            {
                strWebServerURL = "http://172.18.101.50:8080/SOP";
                strDxfFtpURL = "ftp://172.18.101.50";
                strTimeServer = "time2.kriss.re.kr";
                strAdminID = "2119";
            }
            // 영흥
            else if (nSiteID == 2)
            {
                strWebServerURL = "http://172.20.127.150:8080/SOP";
                strDxfFtpURL = "ftp://172.20.127.150";
                strTimeServer = "time2.kriss.re.kr";
                strAdminID = "3119";
            }
            else
            {
                strWebServerURL = "";
                strDxfFtpURL = "";
                strTimeServer = "time2.kriss.re.kr";
                strAdminID = "";
            }
        }

        public void WinRateChanged()
        {
            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in groupBox2.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in groupBox1.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            FormMain.Instance.UpdateWindowRate(ribbonButtonSetup, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton1, WindowRateWidth, WindowRateHeight);
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            FormMain.Instance.UpdateWindowRate(pctl, WindowRateWidth, WindowRateHeight);
        }

        private string m_strURL = "";
        private string m_strNTP = "";

        public void InitDataLoad()
        {
            //string cctvMode = RegUtil.ReadRegValue("IntegratedManager", "cctv_mode", m_nSiteID);
            //if (cctvMode == null || cctvMode == "")
            string cctvMode = "1";

            if (cctvMode == "1")
                ckbSituationMode.Checked = true;
            else
                ckbSituationMode.Checked = false;

            string szURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            string szFtp = RegUtil.ReadRegValue("Server Connection Info", "dxf_ftp_url", m_nSiteID);
            string szNtp = RegUtil.ReadRegValue("Server Connection Info", "time_server", m_nSiteID);

            m_strURL = textBox1.Text = szURL;
            textBoxDrawingFTP.Text = szFtp;
            m_strNTP = textBoxNTPServer.Text = szNtp;

            string[] strValues = { "SOPSimulator", "SDMS", "MissionList", "CCTV" };
            string [] strRetVal = new string[5];
            strRetVal[0] = RegUtil.ReadRegValue("Monitor Info", strValues[0], m_nSiteID);
            strRetVal[1] = RegUtil.ReadRegValue("Monitor Info", strValues[1], m_nSiteID);
            strRetVal[2] = RegUtil.ReadRegValue("Monitor Info", strValues[2], m_nSiteID);
            strRetVal[3] = RegUtil.ReadRegValue("Monitor Info", strValues[3], m_nSiteID);

            ComboBox[] cmbs = { null, comboBox1, comboBox2, comboBox3, comboBox4 };

            for (int i = 1; i < cmbs.Length; i++)
            {
                ComboBox cmb = cmbs[i];
                cmb.SelectedIndex = 0;
            }

            int nMonitor = -1;
            for( int i = 0 ; i < 4; i++)
            {
                if (int.TryParse(strRetVal[i], out nMonitor))
                {
                    if (nMonitor >= 0 && nMonitor <= 4)
                    {
                        ComboBox cmb = cmbs[nMonitor];
                        cmb.SelectedItem = strValues[i];
                    }
                }
            }
        }

        public void SaveData()
        {
            bool bReload = false;

            if (textBox1.Text != m_strURL || textBoxNTPServer.Text != m_strNTP)
            {
                string szURL = textBox1.Text;
                string szFtp = textBoxDrawingFTP.Text;
                string szNtp = textBoxNTPServer.Text;

                //if(ckbSituationMode.Checked == true)
                RegUtil.WriteRegValue("IntegratedManager", "cctv_mode", "1", m_nSiteID);
                //else
                //    RegUtil.WriteRegValue("IntegratedManager", "cctv_mode", "0", m_nSiteID);

                RegUtil.WriteRegValue("Server Connection Info", "webserver_url", szURL, m_nSiteID);
                RegUtil.WriteRegValue("Server Connection Info", "dxf_ftp_url", szFtp, m_nSiteID);
                RegUtil.WriteRegValue("Server Connection Info", "time_server", szNtp, m_nSiteID);

                bReload = true;
            }

            string[] strValues = { "SOPSimulator", "SDMS", "MissionList", "CCTV" };
                        
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

            if (value1 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value1, "1", m_nSiteID);            

            if (value2 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value2, "2", m_nSiteID);            

            if (value3 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value3, "3", m_nSiteID);

            if (value4 != "기본 설정")
                RegUtil.WriteRegValue("Monitor Info", value4, "4", m_nSiteID);

            for (int i = 0; i < 4; i++)
            {                
                if (!arList.Contains(strValues[i]))
                {
                    RegUtil.WriteRegValue("Monitor Info", strValues[i], "-1", m_nSiteID);
                }
            }

            if (bReload && m_MainForm != null)
                m_MainForm.ReloadNetwork();
        }

        private void FormPreference_Load(object sender, EventArgs e)
        {
            // 보안상의 이유로 도면의 FTP 경로는 볼수 없도록 한다.
            labelNTPServer.Location = labelDrawingFTP.Location;
            textBoxNTPServer.Location = textBoxDrawingFTP.Location;

            m_MonitorSettingForm = new FormMonitorSettings();
            m_MonitorSettingForm.TopLevel = false;
            m_MonitorSettingForm.StartPosition = FormStartPosition.Manual;
            m_MonitorSettingForm.Parent = this;
            this.Controls.Add(m_MonitorSettingForm);
        }

        private void btnShowCreator_Click(object sender, EventArgs e)
        {
            FormCreator frm = new FormCreator();
            frm.ShowDialog();
        } 
        
        private bool m_bSituationMode = false;
        public bool SituationMode
        {
            get { return m_bSituationMode; }
            set { m_bSituationMode = value; }
        }

        private void ckbSituationMode_CheckedChanged(object sender, EventArgs e)
        {
            if(ckbSituationMode.Checked == true)
            {
                m_bSituationMode = true;
            }
            else
            {
                m_bSituationMode = false;
            }
        }

        private FormMonitorSettings m_MonitorSettingForm = null;
        private void rbMonitorSet_Click(object sender, EventArgs e)
        {
            m_MonitorSettingForm.Location = new Point(0, 0);
            m_MonitorSettingForm.BringToFront();

            //btnMin.BringToFront();
            //btnClose.BringToFront();

            //labelCopyright.BringToFront();
            //labelCurrVersion.BringToFront();

            m_MonitorSettingForm.InitDataLoad();
            m_MonitorSettingForm.Show();
        }

        private object m_strLastCmbSelectedItem = "";
        private void comboBox_Enter(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            m_strLastCmbSelectedItem = cmb.SelectedItem;
        }

        private void comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox[] cmbs = { comboBox1, comboBox2, comboBox3, comboBox4 };

            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem.ToString() != "기본 설정")
            {
                for (int i = 0; i < cmbs.Length; i++)
                {
                    if (cmbs[i] == cmb)
                        continue;

                    if (cmbs[i].SelectedItem == cmb.SelectedItem)
                    {
                        UnE.Utility.UMessageBoxRibbon.Show("[" + cmb.SelectedItem.ToString() + "] 은(는) 이미 설정되어 있습니다.", "오류");
                        cmb.SelectedItem = m_strLastCmbSelectedItem;
                        return;
                    }
                }
            }
        }
    }
}
