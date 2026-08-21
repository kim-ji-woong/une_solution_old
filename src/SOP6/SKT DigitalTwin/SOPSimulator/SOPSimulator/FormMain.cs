using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using UnE.SOP.Sections;
using UnE.GUI;
using UnE.Controls;
using SOPMonitoringSystem;

namespace SOPSimulator
{
    public class FormMain : SOPMonitoringSystem.FormSOP
    {
        private Color m_themeColor = Color.FromArgb(20, 26, 47);
        private Dictionary<SectionTabPage, bool> m_dicTabPageEvents = new Dictionary<SectionTabPage, bool>();
        private int m_nPanelTopHeight = 20;
        private RibbonButton m_rbtnRealMode = null;
        private bool m_changeQuickButtons = false;
        private int m_nPicUserPos = -1;
        private int m_nLabelNamePos = -1;
        private Control m_picUser = null, m_labelName = null, m_btnConfig = null;
        private string m_strUserName = "";

        private bool m_systemInput = false;

        private ContextMenuStrip m_contextMenuStripLogin = new ContextMenuStrip();
        private ToolStripMenuItem m_toolStripMenuLogout = new ToolStripMenuItem();

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor, bool bSituationRoomMode)
            : base(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nTargetMonitor, bSituationRoomMode)
        {
            m_strUserName = strSOPGenUserRealName;
            this.Load += new System.EventHandler(this.FormMain_Load);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 배경 이미지 변경
            SetBackgroundImage();

            // 로고 변경
            SetLogo();

            HidePanelLeft();
            Sections.PanelSectionEx.BackgroundBrush = new SolidBrush(m_themeColor);
            GetPageHome().tabControl.ControlAdded += TabPage_Added;
            GetPageHome().tabControl.ControlRemoved += TabPage_Removed;

            SetLogoutMenu();
        }

        private void SetLogoutMenu()
        {
            m_contextMenuStripLogin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            m_toolStripMenuLogout});
            m_contextMenuStripLogin.Name = "m_contextMenuStripLogin";
            m_contextMenuStripLogin.Size = new System.Drawing.Size(126, 26);

            m_toolStripMenuLogout.Name = "m_toolStripMenuLogout";
            m_toolStripMenuLogout.Size = new System.Drawing.Size(125, 22);
            m_toolStripMenuLogout.Text = "로그아웃";
            m_toolStripMenuLogout.Click += new System.EventHandler(OnLogout);
        }

        private void OnLogout(object sender, EventArgs e)
        {
            DBUtility2.RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "0", FormSOP.Instance.DBManager.SiteID);
            this.Close();

        }

        private void SetLogo()
        {
            string strBulletin = "btnBulletin";
            string strSOPManager = "btnSOPManager2";
            string strTeamEditor = "btnTeamEditor2";
            string strConfig = "rbtnConfig";
            string strPicUser = "picUser";
            string strUserName = "labelUserName";
            string strStartSOP = "rbtnStartSOP";
            string strCancelSOP = "rbtnCancelSOP";

            Dictionary<string, Control> dicControls = new Dictionary<string, Control>();
            Dictionary<Control, RibbonButton> dicControlButtons = new Dictionary<Control, RibbonButton>();

            RibbonButton btnLoadSOP = null;
            Panel panelSOPMode = null;
            int moveLeft = 120;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Name == "panelTop")
                {
                    int nParentHeight = ctrl.Height;
                    m_nPanelTopHeight = nParentHeight;
                    ctrl.BackColor = m_themeColor;

                    foreach (Control child in ctrl.Controls)
                    {
                        if (child.Name == "pictureBoxLogo")
                        {
                            Bitmap bmp = (Bitmap)global::SOPSimulator.Properties.Resources.Logo;

                            PictureBox picLogo = (PictureBox)child;
                            picLogo.BackgroundImage = bmp;
                            picLogo.Size = new Size(bmp.Width, bmp.Height);
                            picLogo.Location = new Point(picLogo.Location.X, (nParentHeight - picLogo.Size.Height) / 2);
                        }
                        else if (child.Name == "rbtnLoadSOP")
                        {
                            btnLoadSOP = (RibbonButton)child;

                            Point oldLocation = btnLoadSOP.Location;
                            Size oldSize = btnLoadSOP.Size;

                            btnLoadSOP.ClickedImage = global::SOPSimulator.Properties.Resources.Click_callin;
                            btnLoadSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, btnLoadSOP.ClickedImage.Width, btnLoadSOP.ClickedImage.Height);
                            btnLoadSOP.InitButtonWidth = btnLoadSOP.ClickedImage.Width;
                            btnLoadSOP.MouseOverImage = global::SOPSimulator.Properties.Resources.Hover_callin;
                            btnLoadSOP.NormalImage = global::SOPSimulator.Properties.Resources.Normal_callin;
                            btnLoadSOP.Text = "";
                            btnLoadSOP.Size = new Size(btnLoadSOP.ClickedImage.Width, btnLoadSOP.ClickedImage.Height);

                            int moveY = (oldSize.Height - btnLoadSOP.Size.Height) / 2;
                            btnLoadSOP.Location = new Point(oldLocation.X - moveLeft, oldLocation.Y + moveY);
                        }
                        else if (child.Name == strBulletin)
                        {
                            RibbonButton btn = new RibbonButton();
                            dicControls[child.Name] = child;
                            dicControlButtons[child] = btn;

                            btn.ClickedImage = global::SOPSimulator.Properties.Resources.Click_board;
                            btn.MouseOverImage = global::SOPSimulator.Properties.Resources.Hover_board;
                            btn.NormalImage = global::SOPSimulator.Properties.Resources.Normal_board;

                            btn.Click += btnBulletin_Click;
                        }
                        else if (child.Name == strSOPManager)
                        {
                            RibbonButton btn = new RibbonButton();
                            dicControls[child.Name] = child;
                            dicControlButtons[child] = btn;

                            btn.ClickedImage = global::SOPSimulator.Properties.Resources.Click_sop;
                            btn.MouseOverImage = global::SOPSimulator.Properties.Resources.Hover_sop;
                            btn.NormalImage = global::SOPSimulator.Properties.Resources.Normal_sop;

                            btn.Click += btnSOPManager_Click;
                        }
                        else if (child.Name == strTeamEditor)
                        {
                            RibbonButton btn = new RibbonButton();
                            dicControls[child.Name] = child;
                            dicControlButtons[child] = btn;

                            btn.ClickedImage = global::SOPSimulator.Properties.Resources.Click_manage;
                            btn.MouseOverImage = global::SOPSimulator.Properties.Resources.Hover_manage;
                            btn.NormalImage = global::SOPSimulator.Properties.Resources.Normal_manage;

                            btn.Click += btnTeamEditor_Click;
                        }
                        else if (child.Name == strConfig)
                        {
                            RibbonButton btn = new RibbonButton();
                            dicControls[child.Name] = child;
                            dicControlButtons[child] = btn;

                            btn.ClickedImage = global::SOPSimulator.Properties.Resources.Click_setup;
                            btn.MouseOverImage = global::SOPSimulator.Properties.Resources.Hover_setup;
                            btn.NormalImage = global::SOPSimulator.Properties.Resources.Normal_setup;

                            btn.Click += btnConfig_Click;
                        }
                        else if (child.Name == strPicUser)
                        {
                            dicControls[child.Name] = child;
                        }
                        else if (child.Name == strUserName)
                        {
                            dicControls[child.Name] = child;
                            child.Text = m_strUserName;
                        }
                        else if (child.Name == strStartSOP)
                        {
                            dicControls[child.Name] = child;
                        }
                        else if (child.Name == strCancelSOP)
                        {
                            dicControls[child.Name] = child;
                        }
                        else if (child.Name == "panelSOPMode")
                        {
                            SetPanelSOPMode(child);
                            panelSOPMode = (Panel)child;
                        }
                        else if (child.Name == "rbtnControlStatus" || child.Name == "rbtnControlAction")
                        {
                            child.Location = new Point(child.Location.X, 10000);
                        }
                    }
                }
                else if (ctrl.Name == "panelMain")
                {
                    Panel panel = (Panel)ctrl;
                    panel.BackgroundImage = global::SOPSimulator.Properties.Resources.skt_bg;
                    panel.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }

            Control control;

            if (dicControls.TryGetValue(strConfig, out control) == false)
                return;

            int nConfigRightPos = control.Location.X + control.Size.Width;
            int nConfigLeftPos = control.Location.X;
            RibbonButton btnConfig = ControlToRibbonButton(strConfig, dicControls, dicControlButtons, nConfigRightPos);

            if (btnConfig == null)
                return;

            control.Location = btnConfig.Location;

            Control picUser, labelName;

            if (dicControls.TryGetValue(strPicUser, out picUser) == false || dicControls.TryGetValue(strUserName, out labelName) == false)
                return;

            int nPicUserLeftPos = picUser.Location.X;
            int nLabelNameLeftPos = labelName.Location.X;

            picUser.Location = new Point(nPicUserLeftPos - nConfigLeftPos + btnConfig.Location.X, picUser.Location.Y);
            labelName.Location = new Point(nPicUserLeftPos - nLabelNameLeftPos + picUser.Location.X, labelName.Location.Y);

            if (dicControls.TryGetValue(strBulletin, out control) == false)
                return;

            int nBulletinRightPos = control.Location.X + control.Size.Width;
            int nBulletinLeftPos = control.Location.X;

            if (dicControls.TryGetValue(strSOPManager, out control) == false)
                return;

            int nSOPManagerRightPos = control.Location.X + control.Size.Width;
            int nSOPManagerLeftPos = control.Location.X;

            if (dicControls.TryGetValue(strTeamEditor, out control) == false)
                return;

            int nTeamEditorRightPos = control.Location.X + control.Size.Width;

            RibbonButton btnBulletin = ControlToRibbonButton(strBulletin, dicControls, dicControlButtons, picUser.Location.X + (nBulletinRightPos - nPicUserLeftPos));
            RibbonButton btnSOPManager = null, btnTeamEditor = null;

            if (btnBulletin != null)
            {
                btnSOPManager = ControlToRibbonButton(strSOPManager, dicControls, dicControlButtons, nSOPManagerRightPos - nBulletinLeftPos + btnBulletin.Location.X);

                if (btnSOPManager != null)
                {
                    btnTeamEditor = ControlToRibbonButton(strTeamEditor, dicControls, dicControlButtons, nTeamEditorRightPos - nSOPManagerLeftPos + btnSOPManager.Location.X);
                }
            }

            Control ctrlStartSOP, ctrlCancelSOP;

            if (dicControls.TryGetValue(strStartSOP, out ctrlStartSOP) && dicControls.TryGetValue(strCancelSOP, out ctrlCancelSOP))
            {
                int nGap = ctrlCancelSOP.Location.X - (ctrlStartSOP.Location.X + ctrlStartSOP.Size.Width);
                RibbonButton btnStartSOP = ChangeLocationNImage(ctrlStartSOP, ctrlStartSOP.Location.X, global::SOPSimulator.Properties.Resources.Click_start, global::SOPSimulator.Properties.Resources.Hover_start, global::SOPSimulator.Properties.Resources.Normal_start);

                if (btnLoadSOP != null)
                {
                    btnStartSOP.Location = new Point(btnLoadSOP.Location.X + btnLoadSOP.Size.Width + nGap, btnStartSOP.Location.Y);
                }

                ChangeLocationNImage(ctrlCancelSOP, btnStartSOP.Location.X + btnStartSOP.Size.Width + nGap, global::SOPSimulator.Properties.Resources.Click_end, global::SOPSimulator.Properties.Resources.Hover_end, global::SOPSimulator.Properties.Resources.Normal_end);

                if (panelSOPMode != null)
                {
                    panelSOPMode.Location = new Point(ctrlCancelSOP.Location.X + ctrlCancelSOP.Size.Width + nGap, panelSOPMode.Location.Y);
                }
            }

            int nMoveX = btnSOPManager.Location.X - btnTeamEditor.Location.X;

            btnConfig.Location = btnBulletin.Location;
            btnBulletin.Location = btnSOPManager.Location;
            btnSOPManager.Location = btnTeamEditor.Location;

            btnTeamEditor.Location = new Point(btnTeamEditor.Location.X - nMoveX, btnTeamEditor.Location.Y);
            ChangePictureBoxImage((PictureBox)picUser, global::SOPSimulator.Properties.Resources.Login);

            picUser.Location = new Point(picUser.Location.X + moveLeft, picUser.Location.Y + 50);
            labelName.Location = new Point(labelName.Location.X + moveLeft, labelName.Location.Y);
            btnConfig.Location = new Point(btnConfig.Location.X + moveLeft, btnConfig.Location.Y);
            btnBulletin.Location = new Point(btnBulletin.Location.X + moveLeft, btnBulletin.Location.Y);
            btnSOPManager.Location = new Point(btnSOPManager.Location.X + moveLeft, btnSOPManager.Location.Y);
            btnTeamEditor.Location = new Point(btnTeamEditor.Location.X + moveLeft, btnTeamEditor.Location.Y);

            int nGapUserName = labelName.Location.X - picUser.Location.X;
            int nGapUser = btnConfig.Location.X - (btnBulletin.Location.X + btnBulletin.Size.Width);
            m_nPicUserPos = nGapUser + btnConfig.Size.Width;
            m_nLabelNamePos = m_nPicUserPos + picUser.Size.Width + nGapUserName;

            //m_nPicUserPos = picUser.Location.X - btnConfig.Location.X;
            //m_nLabelNamePos = labelName.Location.X - btnConfig.Location.X;

            m_picUser = picUser;
            m_labelName = labelName;
            m_btnConfig = btnConfig;

            if (m_picUser != null)
            {
                m_picUser.Click += picUser_Click;
            }
        }

        private void picUser_Click(object sender, EventArgs e)
        {
            m_contextMenuStripLogin.Show(this, m_picUser.Location.X + m_picUser.Size.Width / 2, m_picUser.Location.Y + m_picUser.Size.Height);
        }

        private void SetPanelSOPMode(Control panelSOPMode)
        {
            panelSOPMode.Location = new Point(panelSOPMode.Location.X + 100, panelSOPMode.Location.Y);
            panelSOPMode.Size = new Size(panelSOPMode.Size.Width + 100, panelSOPMode.Size.Height);
            RibbonButton btnMode = null;

            foreach (Control ctrl in panelSOPMode.Controls)
            {
                if (ctrl.Name == "rbtnCheckRealMode")
                {
                    RibbonButton btn = (RibbonButton)ctrl;
                    btnMode = btn;

                    btn.CheckedImage = global::SOPSimulator.Properties.Resources.RealMode_Checked;
                    btn.NormalImage = global::SOPSimulator.Properties.Resources.RealMode_Unchecked;
                    btn.MouseOverImage = null;
                    btn.MouseOverBkgndImage = null;
                    btn.CheckedMouseOver = null;
                    btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, btn.CheckedImage.Width, btn.CheckedImage.Height);
                    btn.InitButtonWidth = btn.CheckedImage.Width;
                    btn.Text = "";
                    btn.Size = new Size(btn.CheckedImage.Width, btn.CheckedImage.Height);
                    btn.Location = new Point(btn.Location.X, (panelSOPMode.Size.Height - btn.Size.Height * 2) / 3);

                    btnMode_Click(btn, null);
                    btn.Click += btnMode_Click;
                }
                else if (ctrl.Name == "rbtnCheckVirtualMode")
                {
                    int moveLeft = 0;
                    RibbonButton btn = (RibbonButton)ctrl;

                    btn.CheckedImage = global::SOPSimulator.Properties.Resources.VirtualMode_Checked;
                    btn.NormalImage = global::SOPSimulator.Properties.Resources.VirtualMode_Unchecked;
                    btn.MouseOverImage = null;
                    btn.MouseOverBkgndImage = null;
                    btn.CheckedMouseOver = null;
                    btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, btn.CheckedImage.Width, btn.CheckedImage.Height);
                    btn.InitButtonWidth = btn.CheckedImage.Width;
                    btn.Text = "";
                    btn.Size = new Size(btn.CheckedImage.Width, btn.CheckedImage.Height);
                    btn.Location = new Point(btn.Location.X - moveLeft, panelSOPMode.Size.Height - btn.Size.Height - (panelSOPMode.Size.Height - btn.Size.Height * 2) / 3);

                    btnMode_Click(btn, null);
                    btn.Click += btnMode_Click;

                    panelSOPMode.Size = new Size(panelSOPMode.Size.Width - moveLeft, panelSOPMode.Size.Height);
                }
                else if (ctrl.Name == "labelRealMode" || ctrl.Name == "labelVirtualMode")
                {
                    ctrl.Location = new Point(ctrl.Location.X, -10000);
                }
                else if (ctrl.Name == "rbtnRealMode")
                {
                    RibbonButton btn = (RibbonButton)ctrl;
                    m_rbtnRealMode = btn;

                    btn.ClickedImage = null;
                    btn.MouseOverImage = null;
                    btn.CheckedImage = null;
                    btn.DisabledImage = btn.NormalImage = global::SOPSimulator.Properties.Resources.VirtualMode;
                    btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, btn.NormalImage.Width, btn.NormalImage.Height);
                    btn.InitButtonWidth = btn.NormalImage.Width;
                    btn.Text = "";
                    btn.TextLocation = new Point(10000, 10000);
                    btn.Size = new Size(btn.NormalImage.Width, btn.NormalImage.Height);

                    if (btnMode != null)
                        btn.Location = new Point(btnMode.Location.X + btnMode.Size.Width + 20, (panelSOPMode.Size.Height - btn.Size.Height) / 2);
                    else
                        btn.Location = new Point(btn.Location.X, (panelSOPMode.Size.Height - btn.Size.Height) / 2);
                }
            }
        }

        private void btnMode_Click(object sender, EventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;

            if (btn.IsChecked)
                btn.MouseOverImage = btn.CheckedImage;
            else
                btn.MouseOverImage = btn.NormalImage;
        }

        private void ChangePictureBoxImage(PictureBox pic, Bitmap bmp)
        {
            Size oldSize = pic.Size;
            int nRightPos = pic.Location.X + oldSize.Width;

            pic.Size = bmp.Size;
            pic.Image = bmp;
            pic.Location = new Point(nRightPos - pic.Size.Width, pic.Location.Y - (pic.Size.Height - oldSize.Height) / 2);
        }

        private RibbonButton ChangeLocationNImage(Control ctrl, int nPos, Bitmap bmpClick, Bitmap bmpMouseOver, Bitmap bmpNormal)
        {
            RibbonButton btn = (RibbonButton)ctrl;

            Point oldLocation = btn.Location;
            Size oldSize = btn.Size;

            btn.ClickedImage = bmpClick;
            btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, btn.ClickedImage.Width, btn.ClickedImage.Height);
            btn.InitButtonWidth = btn.ClickedImage.Width;
            btn.MouseOverImage = bmpMouseOver;
            btn.NormalImage = bmpNormal;
            btn.Text = "";
            btn.Size = new Size(btn.ClickedImage.Width, btn.ClickedImage.Height);

            int moveY = (oldSize.Height - btn.Size.Height) / 2;
            btn.Location = new Point(nPos, oldLocation.Y + moveY);
            return btn;
        }

        private void btnBulletin_Click(object sender, EventArgs e)
        {
            ToggleSOPBulletin();
        }

        private void btnSOPManager_Click(object sender, EventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            ExecuteManager exeMgr = new ExecuteManager(this);
            exeMgr.Run((ExecuteManager.APP_TYPE)btn.Tag);
        }

        private void btnTeamEditor_Click(object sender, EventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            ExecuteManager exeMgr = new ExecuteManager(this);
            exeMgr.Run((ExecuteManager.APP_TYPE)btn.Tag);
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            SelectOptionTab();
        }

        private void btnStartSOP_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void btnCancelSOP_Click(object sender, EventArgs e)
        {
            if (HasCurrentSOPControl())
                StopWorkflow(DateTime.Now);
        }

        private RibbonButton ControlToRibbonButton(string strControlName, Dictionary<string, Control> dicControls, Dictionary<Control, RibbonButton> dicControlButtons, int nRightPos)
        {
            Control ctrl;

            if (dicControls.TryGetValue(strControlName, out ctrl) == false)
                return null;

            RibbonButton btn;

            if (dicControlButtons.TryGetValue(ctrl, out btn) == false)
                return null;

            ControlToRibbonButton(ctrl, btn, nRightPos);
            return btn;
        }

        private void ControlToRibbonButton(Control ctrl, RibbonButton btn, int nRightPos)
        {
            Control.ControlCollection controls = ctrl.Parent.Controls;
            int nIndex = controls.IndexOf(ctrl);

            if (nIndex < 0)
                return;

            Point oldLocation = ctrl.Location;
            Size oldSize = ctrl.Size;

            //controls.RemoveAt(nIndex);
            controls.Add(btn);
            controls.SetChildIndex(btn, nIndex);

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CheckButton = false;
            btn.CheckedBkgndImage = null;
            btn.CheckedImage = null;
            btn.CheckedMouseOver = null;
            btn.ClickedBackgroundImage = null;
            btn.DisabledBkgndImage = null;
            btn.DisabledImage = null;
            btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            btn.ForeColorChecked = System.Drawing.Color.White;
            btn.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            btn.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            btn.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            btn.ForeColorsByTypeUse = true;
            btn.ID = -1;
            btn.IsChecked = false;
            btn.MouseOverBkgndImage = null;
            btn.Name = "rbtn" + ctrl.Name;
            btn.Owner = null;
            btn.TabIndex = 9;
            btn.Text = "";
            btn.TextLocation = new System.Drawing.Point(0, 15);
            btn.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            btn.ToolTipText = "";
            btn.UseCustomImageRect = true;
            btn.UseTextLocation = true;
            btn.UseVisualStyleBackColor = false;

            //btn.ClickedImage = bmpClicked;
            btn.CustomImageRect = new System.Drawing.Rectangle(0, 0, btn.ClickedImage.Width, btn.ClickedImage.Height);
            btn.InitButtonWidth = btn.ClickedImage.Width;
            //btn.MouseOverImage = bmpMouseOver;
            //btn.NormalImage = bmpNormal;
            btn.Text = "";
            btn.Size = new Size(btn.ClickedImage.Width, btn.ClickedImage.Height);

            int moveY = (oldSize.Height - btn.Size.Height) / 2;

            // ctrl의 오른쪽 끝에 맞춘다.
            btn.Location = new Point(nRightPos - btn.Size.Width, oldLocation.Y + moveY);
            
            btn.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            btn.Tag = ctrl.Tag;
            ctrl.Visible = false;
        }

        private void HidePanelLeft()
        {
            foreach (Control control in GetPageHome().Controls)
            {
                if (control.Name == "panelBackImage")
                {
                    foreach (Control child in control.Controls)
                    {
                        if (child.Name == "panelLeft")
                        {
                            child.Location = new Point(child.Location.X - child.Size.Width, child.Location.Y);
                        }
                        else if (child.Name == "splitContainerMain")
                        {
                            child.Size = new Size(child.Size.Width + child.Location.X, child.Size.Height);
                            child.Location = new Point(0, child.Location.Y);
                        }
                        else if (child.Name == "panelScenarioTab")
                        {
                            child.Location = new Point(0, child.Location.Y);
                        }
                    }

                    break;
                }
            }
        }

        private void TabPage_Added(object sender, ControlEventArgs e)
        {
            if (e.Control != null && e.Control is SectionTabPage)
            {
                SectionTabPage page = (SectionTabPage)e.Control;

                if (m_dicTabPageEvents.ContainsKey(page))
                    return;

                m_dicTabPageEvents[page] = true;
                page.PanelComponentContents.ControlAdded += PanelComponentContents_ControlAdded;
            }
        }

        private void TabPage_Removed(object sender, ControlEventArgs e)
        {
            if (e.Control != null && e.Control is SectionTabPage)
                m_dicTabPageEvents.Remove((SectionTabPage)e.Control);
        }

        private void PanelComponentContents_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control != null && e.Control is UnE.SOP.Sections.ISectionContents)
            {
                foreach (Control control in e.Control.Controls)
                {
                    if (control.Name == "panelBody")
                    {
                        foreach (Control child in control.Controls)
                        {
                            if (child.Name == "PanelMission")
                            {
                                SectionContents.Fancy.PanelMission panelMission = (SectionContents.Fancy.PanelMission)child;
                                panelMission.MissionColor = Color.White;
                                child.BackColor = m_themeColor;
                            }
                            else if (child.Name == "PanelInternal")
                            {
                                SectionContents.Fancy.PanelInternal panelInternal = (SectionContents.Fancy.PanelInternal)child;
                                panelInternal.TitleColor = Color.White;
                                child.BackColor = m_themeColor;
                            }
                        }

                        break;
                    }
                }
            }
        }

        private Image m_imgBackground = null;

        private void SetBackgroundImage()
        {
            SOPMonitoringSystem.PageBackstageSOP pageHome = this.GetPageHome();
            Bitmap bmpBackground = new Bitmap(global::SOPSimulator.Properties.Resources.skt5g_bg_intro);
            m_imgBackground = bmpBackground;

            pageHome.SetBackgroundImage(bmpBackground);

            foreach (Control ctrl in pageHome.Controls)
            {
                if (ctrl.Name == "panelBackImage")
                {
                    SOPMonitoringSystem.PanelSOP panel = (SOPMonitoringSystem.PanelSOP)ctrl;
                    //panel.BackgroundImage = bmpBackground;
                    panel.BackgroundImage = global::SOPSimulator.Properties.Resources.skt_bg;
                    panel.BackgroundImageLayout = ImageLayout.Stretch;
                    pageHome.SetBackgroundImage(global::SOPSimulator.Properties.Resources.skt_bg);
                    //panel.Paint += Panel_Paint;
                    panel.BackgroundImageLayoutChanged += panelBackgroundImageLayoutChanged;
                    break;
                }
            }

            //pageHome.BackgroundImage = global::SOPSimulator.Properties.Resources.sk_bg;
            //pageHome.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void panelBackgroundImageLayoutChanged(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;

            if (panel.BackgroundImageLayout != ImageLayout.Stretch)
            {
                m_systemInput = true;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
                m_systemInput = false;
            }
        }

        private void ChangeQuickButtonImages()
        {
            int ID_SOP_FIRE = 10000;
            int ID_SOP_EARTHQUAKE = 10001;
            int ID_SOP_TYPHOON = 10002;
            int ID_SOP_SUBMERGENCE = 10003;
            int ID_SOP_GENERAL_DISASTER = 10004;
            int ID_SOP_TERROR = 10006;
            int ID_SOP_POLLUTION = 10007;
            int ID_SOP_POWEROFF = 10010;

            SOPMonitoringSystem.PageBackstageSOP pageHome = this.GetPageHome();

            if (pageHome.QuickSOPs.Count >= 8)
            {
                ChangeQuickButtonImage(ID_SOP_FIRE, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_fire, global::SOPSimulator.Properties.Resources.Hover_fire, global::SOPSimulator.Properties.Resources.Click_fire);
                ChangeQuickButtonImage(ID_SOP_EARTHQUAKE, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_quake, global::SOPSimulator.Properties.Resources.Hover_quake, global::SOPSimulator.Properties.Resources.Click_quake);
                ChangeQuickButtonImage(ID_SOP_TYPHOON, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_wind, global::SOPSimulator.Properties.Resources.Hover_wind, global::SOPSimulator.Properties.Resources.Click_wind);
                ChangeQuickButtonImage(ID_SOP_SUBMERGENCE, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_flooding, global::SOPSimulator.Properties.Resources.Hover_flooding, global::SOPSimulator.Properties.Resources.Click_flooding);
                ChangeQuickButtonImage(ID_SOP_GENERAL_DISASTER, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_etc, global::SOPSimulator.Properties.Resources.Hover_etc, global::SOPSimulator.Properties.Resources.Click_etc);
                ChangeQuickButtonImage(ID_SOP_TERROR, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_terror, global::SOPSimulator.Properties.Resources.Hover_terror, global::SOPSimulator.Properties.Resources.Click_terror);
                ChangeQuickButtonImage(ID_SOP_POLLUTION, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_gas, global::SOPSimulator.Properties.Resources.Hover_gas, global::SOPSimulator.Properties.Resources.Click_gas);
                ChangeQuickButtonImage(ID_SOP_POWEROFF, pageHome.QuickSOPs, global::SOPSimulator.Properties.Resources.Normal_blackout, global::SOPSimulator.Properties.Resources.Hover_blackout, global::SOPSimulator.Properties.Resources.Click_blackout);

                m_changeQuickButtons = true;
            }
        }

        private void ChangeQuickButtonImage(int key, Dictionary<int, PageBackstageSOP.QuickSOPButton> dicQuickButtons, Bitmap bmpNormal, Bitmap bmpMouseOver, Bitmap bmpClicked)
        {
            PageBackstageSOP.QuickSOPButton btn;

            if (dicQuickButtons.TryGetValue(key, out btn))
            {
                if (btn.SOPRibbonButton == null)
                    return;

                btn.SOPRibbonButton.NormalImage = bmpNormal;
                btn.SOPRibbonButton.MouseOverImage = bmpMouseOver;
                btn.SOPRibbonButton.ClickedImage = bmpClicked;
                btn.SOPRibbonButton.Refresh();
            }
        }

        /*private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (m_imgBackground != null)
            {
                int x = (this.Size.Width - m_imgBackground.Size.Width) / 2;
                int y = (this.Size.Height - m_nPanelTopHeight - m_imgBackground.Size.Height) / 2;

                e.Graphics.DrawImage(m_imgBackground, x, y);
            }
        }*/

        protected override void OnTimer()
        {
            if (m_rbtnRealMode == null)
                return;

            if (m_changeQuickButtons ==false)
            {
                ChangeQuickButtonImages();
            }

            if (m_rbtnRealMode.Text == "실제모드")
            {
                if (m_rbtnRealMode.IsChecked == false)
                {
                    m_rbtnRealMode.DisabledImage = m_rbtnRealMode.NormalImage = global::SOPSimulator.Properties.Resources.RealMode;
                    m_rbtnRealMode.Refresh();
                }
            }
            else if (m_rbtnRealMode.Text == "훈련모드")
            {
                if (m_rbtnRealMode.IsChecked == true)
                {
                    m_rbtnRealMode.DisabledImage = m_rbtnRealMode.NormalImage = global::SOPSimulator.Properties.Resources.VirtualMode;
                    m_rbtnRealMode.Refresh();
                }
            }

            base.OnTimer();
        }

        protected override void InitPanels()
        {
            base.InitPanels();

            if (m_picUser != null && m_labelName != null)
            {
                m_picUser.Location = new Point(m_btnConfig.Location.X + m_nPicUserPos, m_picUser.Location.Y);
                m_labelName.Location = new Point(m_btnConfig.Location.X + m_nLabelNamePos, m_labelName.Location.Y);
            }
        }
    }
}
