using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace SDMS.PopupDialog
{
    public partial class FormRemoteControl : Form
    {
        private Dictionary<ImageButton, int> m_dicButtonIDs = new Dictionary<ImageButton, int>();
        private Dictionary<int, ImageButton> m_dicIDButtons = new Dictionary<int, ImageButton>();

        private Image m_Img3dTabDefault = global::SDMS.Properties.Resources.Remote_3D_Default;
        private Image m_Img3dTabClick = global::SDMS.Properties.Resources.Remote_3D_Click;
        private Image m_ImgAdminTabDefault = global::SDMS.Properties.Resources.Remote_Admin_Default;
        private Image m_ImgAdminTabClick = global::SDMS.Properties.Resources.Remote_Admin_Click;
        private Image m_ImgReportdTabDefault = global::SDMS.Properties.Resources.Remote_Report_Default;
        private Image m_ImgReportTabClick = global::SDMS.Properties.Resources.Remote_Report_Click;

        private Image m_ImgSoundOnDefault = global::SDMS.Properties.Resources.Remote_SoundOn_Default;
        private Image m_ImgSoundOnClick = global::SDMS.Properties.Resources.Remote_SoundOn_Click;
        private Image m_ImgSoundOffDefault = global::SDMS.Properties.Resources.Remote_SoundOff_Default;
        private Image m_ImgSoundOfftClick = global::SDMS.Properties.Resources.Remote_SoundOff_Click;

        private Image m_ImgReportFireDefulat = global::SDMS.Properties.Resources.DlgSelectCase_ReportFire_Default;
        private Image m_ImgReportFireClick = global::SDMS.Properties.Resources.DlgSelectCase_ReportFire_Click;

        private Image m_ImgReportPsmDefulat = global::SDMS.Properties.Resources.DlgSelectCase_ReportPSM_Default;
        private Image m_ImgReportPsmClick = global::SDMS.Properties.Resources.DlgSelectCase_ReportPSM_Click;

        private Image m_ImgReportSecurityDefulat = global::SDMS.Properties.Resources.DlgSelectCase_ReportSecurity_Default;
        private Image m_ImgReportSecurityClick = global::SDMS.Properties.Resources.DlgSelectCase_ReportSecurity_Click;

        private Image m_ImgMalfunctionFireDefault = global::SDMS.Properties.Resources.DlgSelectCase_Malfunction_Default;
        private Image m_ImgMalfunctionFireClick = global::SDMS.Properties.Resources.DlgSelectCase_Malfunction_Click;

        private Image m_ImgMalfunctionPsmDefault = global::SDMS.Properties.Resources.DlgSelectCase_PSMMalfunction_Default;
        private Image m_ImgMalfunctionPsmClick = global::SDMS.Properties.Resources.DlgSelectCase_PSMMalfunction_Click;

        public FormRemoteControl()
        {
            InitializeComponent();

            pn3D.Location = pnManage.Location = pnReport.Location = new Point(18, 67);
            this.Size = new Size(605, 821);
        }

        private void FormRemoteControl_Load(object sender, EventArgs e)
        {
            
        }

        private void SetButtonID()
        {
            SetButtonID(btn3DTab, ID.ID_TAB_3D, "3D");
            SetButtonID(btnAdminTab, ID.ID_TAB_MANAGE, "관리");
            SetButtonID(btnReportTab, ID.ID_TAB_REPORT, "리포트");

            SetButtonID(btnFullScreen, ID.ID_VIEW_HOME_MAIN, "전체 화면");
            SetButtonID(btnPick, ID.ID_VIEW_PICK, "선택");
            SetButtonID(btnPanning, ID.ID_VIEW_PAN, "화면 이동");
            SetButtonID(btnOrbit, ID.ID_VIEW_ORBIT, "화면 회전");
            SetButtonID(btnZoomIn, ID.ID_VIEW_ZOOMIN, "확대");
            SetButtonID(btnZoomOut, ID.ID_VIEW_ZOOMOUT, "축소");
            SetButtonID(btnMultiCCTV, ID.ID_VIEW_CCTV, "CCTV 크게 보기");
            SetButtonID(btnSimulator, ID.ID_VIEW_SIMULATOR, "센서 시뮬레이터 기동");
            SetButtonID(btnPSMStatus, ID.ID_VIEW_PSM, "유해 화학물질 리스트 보기");
            SetButtonID(btnScreenShot, ID.ID_VIEW_SCREENSHOT, "화면 캡쳐");

            SetButtonID(btnDetectAnalyze, ID.ID_BTN_DETECT_ANALYZE);
            SetButtonID(btnDetectHistory, ID.ID_BTN_DETECT);
            SetButtonID(btnProcessHistory, ID.ID_BTN_NOTOPERATION);
            SetButtonID(btnReactionHistory, ID.ID_BTN_ACTION);
            SetButtonID(btnSMSHistory, ID.ID_BTN_SMSREPORT);
            SetButtonID(btnDetectAnalyzePsm, ID.ID_BTN_DETECT_PSM_ANALYZE);
            SetButtonID(btnDetectHistoryPsm, ID.ID_BTN_DETECT_PSM);
            SetButtonID(btnProcessHistoryPsm, ID.ID_BTN_NOTOPERATION_PSM);
            SetButtonID(btnReactionHistoryPsm, ID.ID_BTN_ACTION_PSM);
            SetButtonID(btnSMSHistoryPsm, ID.ID_BTN_SMSREPORT_PSM);

            SetButtonID(btnSensorMgr, ID.ID_MANAGE_SENSOR);
            SetButtonID(btnShowList, ID.ID_SHOW_LIST_FACILITY);
            SetButtonID(btnManageManager, ID.ID_MANAGE_MANAGER);
            SetButtonID(btnManageSMS, ID.ID_MANAGE_MESSAGE);
            SetButtonID(btnManageBroadcast, ID.ID_MANAGE_BROADCAST);
            SetButtonID(btnManageDetect, ID.ID_MANAGE_DETECT);
        }

        public void SetTab()
        {
            SetButtonID();

            if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
                btn_Click(btn3DTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
            else if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                btn_Click(btnAdminTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
            else if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                btn_Click(btnReportTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));

            SensorDectect();
        }

        public void ChangeTab(UnE.View.Content.ContentOwnerTab tab)
        {
            switch (tab)
            {
                case UnE.View.Content.ContentOwnerTab.M3D_TAB: // ID.ID_TAB_3D:
                    pn3D.Visible = true;
                    pnManage.Visible = false;
                    pnReport.Visible = false;

                    btn3DTab.Image = m_Img3dTabClick;
                    btn3DTab.ImageNormal = m_Img3dTabClick;
                    btn3DTab.ImageMouseOver = m_Img3dTabClick;

                    btnAdminTab.Image = m_ImgAdminTabDefault;
                    btnAdminTab.ImageNormal = m_ImgAdminTabDefault;
                    btnAdminTab.ImageMouseOver = m_ImgAdminTabDefault;

                    btnReportTab.Image = m_ImgReportdTabDefault;
                    btnReportTab.ImageNormal = m_ImgReportdTabDefault;
                    btnReportTab.ImageMouseOver = m_ImgReportdTabDefault;
                    break;
                case UnE.View.Content.ContentOwnerTab.ADMIN_TAB: // ID.ID_TAB_MANAGE:
                    pn3D.Visible = false;
                    pnManage.Visible = true;
                    pnReport.Visible = false;

                    btn3DTab.Image = m_Img3dTabDefault;
                    btn3DTab.ImageNormal = m_Img3dTabDefault;
                    btn3DTab.ImageMouseOver = m_Img3dTabDefault;

                    btnAdminTab.Image = m_ImgAdminTabClick;
                    btnAdminTab.ImageNormal = m_ImgAdminTabClick;
                    btnAdminTab.ImageMouseOver = m_ImgAdminTabClick;

                    btnReportTab.Image = m_ImgReportdTabDefault;
                    btnReportTab.ImageNormal = m_ImgReportdTabDefault;
                    btnReportTab.ImageMouseOver = m_ImgReportdTabDefault;
                    break;
                case UnE.View.Content.ContentOwnerTab.REPORT_TAB:// ID.ID_TAB_REPORT:
                    pn3D.Visible = false;
                    pnManage.Visible = false;
                    pnReport.Visible = true;

                    btn3DTab.Image = m_Img3dTabDefault;
                    btn3DTab.ImageNormal = m_Img3dTabDefault;
                    btn3DTab.ImageMouseOver = m_Img3dTabDefault;

                    btnAdminTab.Image = m_ImgAdminTabDefault;
                    btnAdminTab.ImageNormal = m_ImgAdminTabDefault;
                    btnAdminTab.ImageMouseOver = m_ImgAdminTabDefault;

                    btnReportTab.Image = m_ImgReportTabClick;
                    btnReportTab.ImageNormal = m_ImgReportTabClick;
                    btnReportTab.ImageMouseOver = m_ImgReportTabClick;
                    break;
            }
        }

        private void SetButtonID(ImageButton btn, int nID, string strTooltipText = "")
        {
            if (btn == null)
                return;

            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            btn.Tag = nID;
            
            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            
            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn.Tag == null)
                return;

            int nBtnID = (int)btn.Tag;

            if (nBtnID == ID.ID_TAB_3D)
                ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
            else if (nBtnID == ID.ID_TAB_MANAGE)
                ChangeTab(UnE.View.Content.ContentOwnerTab.ADMIN_TAB);
            else if (nBtnID == ID.ID_TAB_REPORT)
                ChangeTab(UnE.View.Content.ContentOwnerTab.REPORT_TAB);

            FormMain.Instance.RemoteOperation(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private Point m_ptOrigin = new Point();

        private void FormRemoteControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            m_bLeftMouseDown = true;
            m_ptMove = Control.MousePosition;
            m_ptOrigin = this.Location;
        }

        private void FormRemoteControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void FormRemoteControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            m_bLeftMouseDown = false;
        }
        #endregion
        
        public void SetNextTab()
        {
            if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
            {
                btn_Click(btnAdminTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));                
            }
            else if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
            {
                btn_Click(btnReportTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
            }
            else if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.REPORT_TAB)
            {
                btn_Click(btn3DTab, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
            }
        }

        public void SensorDectect()
        {
            if (!FormMain.Instance.IsDisaster)
            {
                this.Size = new Size(605, 821);
                SetLocation();
                return;
            }

            this.Size = new Size(605, 999);
            SetLocation();

            libSensorProcess.ProcessIF process = FormMain.Instance.CurrentSensorDetectProcess;

            string strType = "[화재] ";
            libSensorProcess.ProcessType type = process.ProcessType;
            if (type == libSensorProcess.ProcessType.FireAlarm)
            {
                mBtnReportFire.ImageNormal = m_ImgReportFireDefulat;
                mBtnReportFire.ImageMouseOver = m_ImgReportFireClick;
                mBtnReportFire.ImageClicked = m_ImgReportFireClick;

                mBtnReportMalfunction.ImageNormal = m_ImgMalfunctionFireDefault;
                mBtnReportMalfunction.ImageMouseOver = m_ImgMalfunctionFireClick;
                mBtnReportMalfunction.ImageClicked = m_ImgMalfunctionFireClick;
            }
            else if (type == libSensorProcess.ProcessType.PSMAlarm)
            {
                strType = "[누출] ";
                mBtnReportFire.ImageNormal = m_ImgReportPsmDefulat;
                mBtnReportFire.ImageMouseOver = m_ImgReportPsmClick;
                mBtnReportFire.ImageClicked = m_ImgReportPsmClick;

                mBtnReportMalfunction.ImageNormal = m_ImgMalfunctionPsmDefault;
                mBtnReportMalfunction.ImageMouseOver = m_ImgMalfunctionPsmClick;
                mBtnReportMalfunction.ImageClicked = m_ImgMalfunctionPsmClick;
            }
            else if (type == libSensorProcess.ProcessType.SecurityAlarm)
            {
                strType = "[방범] ";
                mBtnReportFire.ImageNormal = m_ImgReportSecurityDefulat;
                mBtnReportFire.ImageMouseOver = m_ImgReportSecurityClick;
                mBtnReportFire.ImageClicked = m_ImgReportSecurityClick;

                mBtnReportMalfunction.ImageNormal = m_ImgMalfunctionPsmDefault;
                mBtnReportMalfunction.ImageMouseOver = m_ImgMalfunctionPsmClick;
                mBtnReportMalfunction.ImageClicked = m_ImgMalfunctionPsmClick;
            }

            mBtnReportFire.Refresh();
            mBtnReportMalfunction.Refresh();

            label1.Text =  strType + process.TargetZone.ZoneName;
        }

        private void mBtnViewCCTV_Click(object sender, EventArgs e)
        {
            DlgSelectCase.Instance.SetRemoteViewCCTV();
        }

        private void mBtnReportFire_Click(object sender, EventArgs e)
        {
            DlgSelectCase.Instance.SetRemoteReportFire();
        }

        private void mBtnReportMalfunction_Click(object sender, EventArgs e)
        {
            DlgSelectCase.Instance.SetRemoteMalfunction();
        }

        private void btnSound_Click(object sender, EventArgs e)
        {
            DlgSelectCase.Instance.SetRemoteSoundOnOff();
            if (DlgSelectCase.Instance.CurrentData.Sensor.SoundOn)
            {
                btnSound.ImageNormal = global::SDMS.Properties.Resources.Remote_SoundOn_Default;
                btnSound.ImageClicked = global::SDMS.Properties.Resources.Remote_SoundOn_Click;
                btnSound.ImageMouseOver = global::SDMS.Properties.Resources.Remote_SoundOn_Click;
            }
            else
            {
                btnSound.ImageNormal = global::SDMS.Properties.Resources.Remote_SoundOff_Default;
                btnSound.ImageClicked = global::SDMS.Properties.Resources.Remote_SoundOff_Click;
                btnSound.ImageMouseOver = global::SDMS.Properties.Resources.Remote_SoundOff_Click;
            }

            btnSound.Refresh();
        }

        private void SetLocation()
        {
            Screen[] sc = Screen.AllScreens;
        }
    }
}
