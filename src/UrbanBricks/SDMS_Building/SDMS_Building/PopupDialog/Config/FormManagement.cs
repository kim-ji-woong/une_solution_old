using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace SDMS_Building.PopupDialog.Config
{
    public enum Page { None = -1, Manager = 0, SensorList, DetectPolicy, Earthquake, BroadcastConfig, SMSConfig }
    public partial class FormManagement : Form
    {
        private Page m_curPage = Page.Manager;

        private Panel m_pnManager = null;
        private Panel m_pnSensorList = null;
        private Panel m_pnDetectPolicy = null;
        private Panel m_pnEarthquake = null;
        private Panel m_pnBroadcastConfig = null;
        private Panel m_pnSMSConfig = null;
        
        private FormManager m_frmManager = null;
        private FormSensorList m_frmSensorList = null;
        private FormDetectPolicy m_frmDetectPolicy = null;
        private FormEarthquake m_frmEarthquake = null;
        private FormBroadcastConfig m_frmBroadcastConfig = null;
        private FormSMSConfig m_frmSMSConfig = null;

        private Font m_fontBold = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

        public FormManagement()
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 35, 35));

            this.Location = new Point(500, 500);

            btnManagerTab.Font = m_fontBold;
            btnSensorListTab.Font = m_fontBold;
            btnDetectPolicyTab.Font = m_fontBold;
            btnEarthquakeTab.Font = m_fontBold;
            btnBroadcast.Font = m_fontBold;
            btnSMS.Font = m_fontBold;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
            {
                SetButtonSize(189, btnManagerTab, btnSensorListTab, btnDetectPolicyTab, btnEarthquakeTab, btnSMS);
                btnBroadcast.Visible = false;

                btnManagerTab.Location = new Point(21, 100);
                btnSensorListTab.Location = new Point(213, 100);
                btnDetectPolicyTab.Location = new Point(405, 100);
                btnEarthquakeTab.Location = new Point(597, 100);
                btnSMS.Location = new Point(789, 100);
            }
        }

        private void SetButtonSize(int width, params RibbonButton[] btns)
        {
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Size = new Size(width, 50);
                btns[i].CustomImageRect = new Rectangle(0, 0, width, 50);
                btns[i].InitButtonWidth = width;
                btns[i].TextLocation = new Point(0, 15);
                btns[i].TextPos = RibbonButton.TextPosition.BOTTOM;
                btns[i].UseTextLocation = true;
                btns[i].TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private void FormManagement_Load(object sender, EventArgs e)
        {
            m_pnManager = new Panel();
            m_pnManager.Location = new Point(20, 149);
            m_pnManager.Size = new Size(960, 500);
            m_pnManager.Parent = this;

            m_frmManager = new FormManager();
            m_frmManager.TopLevel = false;            
            m_frmManager.Parent = m_pnManager;
            m_frmManager.Dock = DockStyle.Fill;
            m_frmManager.Show();

            m_pnSensorList = new Panel();
            m_pnSensorList.Location = new Point(20, 149);
            m_pnSensorList.Size = new Size(960, 500);
            m_pnSensorList.Parent = this;
            
            m_pnDetectPolicy = new Panel();
            m_pnDetectPolicy.Location = new Point(20, 149);
            m_pnDetectPolicy.Size = new Size(960, 500);
            m_pnDetectPolicy.Parent = this;

            m_pnEarthquake = new Panel();
            m_pnEarthquake.Location = new Point(20, 149);
            m_pnEarthquake.Size = new Size(960, 500);
            m_pnEarthquake.Parent = this;

            m_pnSMSConfig = new Panel();
            m_pnSMSConfig.Location = new Point(20, 149);
            m_pnSMSConfig.Size = new Size(960, 500);
            m_pnSMSConfig.Parent = this;

            if (UnE.SOP.ProxySOP.Instance.SiteID != 201)
            {
                m_pnBroadcastConfig = new Panel();
                m_pnBroadcastConfig.Location = new Point(20, 149);
                m_pnBroadcastConfig.Size = new Size(960, 500);
                m_pnBroadcastConfig.Parent = this;
            }
            
            m_pnSensorList.Visible = false;
            m_pnDetectPolicy.Visible = false;
            m_pnEarthquake.Visible = false;
            if (UnE.SOP.ProxySOP.Instance.SiteID != 201)
                m_pnBroadcastConfig.Visible = false;
            m_pnSMSConfig.Visible = false;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 취소 버튼 시에 매니져 다시 불러오기
            FormMain.Instance.DataManager.LoadFacilityManager();

            if (m_frmManager != null)
                m_frmManager.Close();

            if (m_frmSensorList != null)
                m_frmSensorList.Close();

            if (m_frmDetectPolicy != null)
                m_frmDetectPolicy.Close();

            if (m_frmEarthquake != null)
                m_frmEarthquake.Close();

            if (m_frmBroadcastConfig != null)
                m_frmBroadcastConfig.Close();

            if (m_frmSMSConfig != null)
                m_frmSMSConfig.Close();

            this.DialogResult = DialogResult.No;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (FormMain.Instance.UserType == LoginUserType.General) // 관리자가 아니면 저장할 수 없음
            {
                FormMessageBox msg = new FormMessageBox("관리자 계정만 변경할 수 있습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                return;
            }

            if (m_frmDetectPolicy != null)
                m_frmDetectPolicy.Save();       
            if (m_frmEarthquake != null)
                m_frmEarthquake.Save();
            if (m_frmManager != null)
                m_frmManager.Save();

            FormMain.Instance.NetworkWebManager.SendChangeFacilityManager();
            if (m_frmBroadcastConfig != null)
                m_frmBroadcastConfig.Save();
            if (m_frmSMSConfig != null)
                m_frmSMSConfig.Save();            

            if (m_frmManager != null)
                m_frmManager.Close();
            if (m_frmSensorList != null)
                m_frmSensorList.Close();
            if (m_frmDetectPolicy != null)
                m_frmDetectPolicy.Close();
            if (m_frmEarthquake != null)
                m_frmEarthquake.Close();
            if (m_frmBroadcastConfig != null)
                m_frmBroadcastConfig.Close();
            if (m_frmSMSConfig != null)
                m_frmSMSConfig.Close();

            this.DialogResult = DialogResult.Yes;
        }
        
        private void btnTab_Click(object sender, EventArgs e)
        {
            Page clickedPage = Page.None;

            RibbonButton btn = sender as RibbonButton;
            if (btn == btnManagerTab)
                clickedPage = Page.Manager;
            else if (btn == btnSensorListTab)
                clickedPage = Page.SensorList;
            else if (btn == btnDetectPolicyTab)
                clickedPage = Page.DetectPolicy;
            else if (btn == btnEarthquakeTab)
                clickedPage = Page.Earthquake;
            else if (btn == btnBroadcast)
                clickedPage = Page.BroadcastConfig;
            else if (btn == btnSMS)
                clickedPage = Page.SMSConfig;

            if (m_curPage == clickedPage) // 같은 페이지를 또 눌렀을 때
                return;
            
            if (m_curPage == Page.Manager)
            {
                btnManagerTab.IsChecked = false;
                btnManagerTab.Refresh();
                m_pnManager.Visible = false;
            }
            else if (m_curPage == Page.SensorList)
            {
                btnSensorListTab.IsChecked = false;
                btnSensorListTab.Refresh();
                m_pnSensorList.Visible = false;
            }
            else if (m_curPage == Page.DetectPolicy)
            {
                btnDetectPolicyTab.IsChecked = false;
                btnDetectPolicyTab.Refresh();
                m_pnDetectPolicy.Visible = false;
            }
            else if (m_curPage == Page.Earthquake)
            {
                btnEarthquakeTab.IsChecked = false;
                btnEarthquakeTab.Refresh();
                m_pnEarthquake.Visible = false;
            }
            else if (m_curPage == Page.BroadcastConfig)
            {
                btnBroadcast.IsChecked = false;
                btnBroadcast.Refresh();
                m_pnBroadcastConfig.Visible = false;
            }
            else if (m_curPage == Page.SMSConfig)
            {
                btnSMS.IsChecked = false;
                btnSMS.Refresh();
                m_pnSMSConfig.Visible = false;
            }
            
            m_curPage = clickedPage;

            btn.IsChecked = true;

            ShowPage();
        }

        private void ShowPage()
        {
            if (m_curPage == Page.Manager)
            {
                m_pnManager.Visible = true;
            }
            else if (m_curPage == Page.SensorList)
            {
                if (m_frmSensorList == null)
                {
                    m_frmSensorList = new FormSensorList();
                    m_frmSensorList.TopLevel = false;
                    m_frmSensorList.Parent = m_pnSensorList;
                    m_frmSensorList.Dock = DockStyle.Fill;
                    m_frmSensorList.Show();
                }

                m_pnSensorList.Visible = true;
            }
            else if (m_curPage == Page.DetectPolicy)
            {
                if (m_frmDetectPolicy == null)
                {
                    m_frmDetectPolicy = new FormDetectPolicy();
                    m_frmDetectPolicy.TopLevel = false;
                    m_frmDetectPolicy.Parent = m_pnDetectPolicy;
                    m_frmDetectPolicy.Dock = DockStyle.Fill;
                    m_frmDetectPolicy.Show();
                }

                m_pnDetectPolicy.Visible = true;
            }
            else if (m_curPage == Page.Earthquake)
            {
                if (m_frmEarthquake == null)
                {
                    m_frmEarthquake = new FormEarthquake();
                    m_frmEarthquake.TopLevel = false;
                    m_frmEarthquake.Parent = m_pnEarthquake;
                    m_frmEarthquake.Dock = DockStyle.Fill;
                    m_frmEarthquake.Show();
                }

                m_pnEarthquake.Visible = true;
            }
            else if (m_curPage == Page.BroadcastConfig)
            {
                if (m_frmBroadcastConfig == null)
                {
                    m_frmBroadcastConfig = new FormBroadcastConfig();
                    m_frmBroadcastConfig.TopLevel = false;
                    m_frmBroadcastConfig.Parent = m_pnBroadcastConfig;
                    m_frmBroadcastConfig.Dock = DockStyle.Fill;
                    m_frmBroadcastConfig.Show();
                }

                m_pnBroadcastConfig.Visible = true;
            }
            else if (m_curPage == Page.SMSConfig)
            {
                if (m_frmSMSConfig == null)
                {
                    m_frmSMSConfig = new FormSMSConfig();
                    m_frmSMSConfig.TopLevel = false;
                    m_frmSMSConfig.Parent = m_pnSMSConfig;
                    m_frmSMSConfig.Dock = DockStyle.Fill;
                    m_frmSMSConfig.Show();
                }

                m_pnSMSConfig.Visible = true;
            }
        }
    }
}
