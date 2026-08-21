using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using System.Collections;
using XtremeDockingPane;
using Sections;

namespace SOPMonitoringSystem
{
    public partial class PageBackstageHome : Form, IRibbonButtonOwner, Sections.ISectionListener
    {
        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        private DockingLeftScenario m_dockScenario = null;
        private DockingLeftPropertiesLevel m_dockPropertiesLevel = null;
        private DockingLeftProperties m_dockProperties = null;
        private DockingBottomSOPLog m_dockSOPLog = null;
        private DockingRightProgress m_dockProgress = null;
        private DockingRightPersonnel m_dockPersonnel = null;
        private DockingReceiveMessage m_dockMessage = null;

		public void OnSelectMission(int nActionStepID, int nReal, int nComponentHistory, int nRowId)
		{
			if( WorkFlowManager.Instance.Get(nActionStepID, (nReal == 1 ? true : false)) != null)
			{
				WorkFlow work = WorkFlowManager.Instance.Get(nActionStepID, (nReal == 1 ? true : false));
				if( work.State == WorkFlowState.RUN)
				{
					ComponentContents content = GetComponentContents(nActionStepID, (nReal == 1 ? true : false), nComponentHistory);
					if (content == null)
						return;
					content.SelectRow(nRowId);
				}
				
			}			
		}
		
		
		// ActionStep별 ComponentContents List
        // Key : 상위 4바이트(1이면 실제 모드, 0이면 훈련 모드), 하위 4바이트(ActionStep ID)
        private Dictionary<long, ArrayList> m_dicComponentContents = new Dictionary<long, ArrayList>();

        public ArrayList GetComponentContentsList(int nActionStepID, bool isRealMode)
        {
            long nHi = isRealMode ? 1 : 0;
            long nLow = nActionStepID;
            long nKey = (nHi << 32) | nLow;

            if (m_dicComponentContents.ContainsKey(nKey))
                return m_dicComponentContents[nKey];

            return null;
        }

        public ComponentContents GetComponentContents(int nActionStepID, bool isRealMode, int nComponentHistoryID)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
                return null;

            foreach (ComponentContents contents in arrContents)
            {
                if (contents.ComponentHistoryID == nComponentHistoryID)
                    return contents;
            }

            return null;
        }

        public ComponentContents GetLastComponentContents(int nActionStepID, bool isRealMode)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
                return null;

            int nContentsCount = arrContents.Count;
            if (nContentsCount == 0)
                return null;

            return (ComponentContents)arrContents[nContentsCount - 1];
        }

        public void AddComponentContents(int nActionStepID, bool isRealMode, ComponentContents contents)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents == null)
            {
                long nHi = isRealMode ? 1 : 0;
                long nLow = nActionStepID;
                long nKey = (nHi << 32) | nLow;

                arrContents = new ArrayList();
                m_dicComponentContents[nKey] = arrContents;
            }

            arrContents.Add(contents);
            splitContainerMain.Panel2.Controls.Add(contents);
        }

        public void ClearComponentContents(int nActionStepID, bool isRealMode)
        {
            long nHi = isRealMode ? 1 : 0;
            long nLow = nActionStepID;
            long nKey = (nHi << 32) | nLow;

            m_dicComponentContents.Remove(nKey);
            splitContainerMain.Panel2.Controls.Clear();
        }

        private Pane m_paneProperties = null;

        public SOPMonitoringSystem.DockingLeftScenario DockScenario
        {
            get { return m_dockScenario; }
            set { m_dockScenario = value; }
        }
        public SOPMonitoringSystem.DockingLeftPropertiesLevel DockPropertiesLevel
        {
            get { return m_dockPropertiesLevel; }
            set { m_dockPropertiesLevel = value; }
        }
        public SOPMonitoringSystem.DockingLeftProperties DockProperties
        {
            get { return m_dockProperties; }
            set { m_dockProperties = value; }
        }
        public SOPMonitoringSystem.DockingRightProgress DockProgress
        {
            get { return m_dockProgress; }
            set { m_dockProgress = value; }
        }
        public SOPMonitoringSystem.DockingRightPersonnel DockPersonnel
        {
            get { return m_dockPersonnel; }
            set { m_dockPersonnel = value; }
        }
        public SOPMonitoringSystem.DockingReceiveMessage DockingMessage
        {
            get { return m_dockMessage; }
            set { m_dockMessage = value; }
        }
        public XtremeDockingPane.Pane PaneProperties
        {
            get { return m_paneProperties; }
            set { m_paneProperties = value; }
        }

        private PointF[] m_arrDragDropOrigin = null;
        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;
        private Sections.PanelSectionEx m_currentPanel = null;

        private Sections.Section m_currentSection = null;
        public Sections.Section CurrentSection
        {
            set { m_currentSection = value; }
        }

        private Form[] m_arrDocking = new Form[6];

        private ArrayList m_arrPanel = new ArrayList();
        public System.Collections.ArrayList PanelArray
        {
            get { return m_arrPanel; }
            set { m_arrPanel = value; }
        }
        private ArrayList m_arrTabPage = new ArrayList();

        private Color m_colorPanel1 = System.Drawing.Color.FromArgb(255, 192, 255);
        private Color m_colorPanel2 = System.Drawing.Color.FromArgb(192, 192, 255);

        //private int m_nTabPage = 1;
        private int m_nActiopnStepID = 0;        
        private ArrayList m_arrTeams = new ArrayList();
        //private ArrayList m_arrSectionLog = new ArrayList();

        //Pane m_paneSOPLog = null;

        ArrayList m_arrSOPButtons = new ArrayList();

        /*private int m_nContents = 0;
        public int Contents
        {
            get { return m_nContents; }
            set { m_nContents = value; }
        }*/
        public ArrayList TeamList
        {
            get { return m_arrTeams; }
       }

        public ArrayList PanelinTabPage
        {
            get { return m_arrPanel; }
        }

        public Color ColorPanel1
        {
            set { m_colorPanel1 = value; }
        }

        public Color ColorPanel2
        {
            set { m_colorPanel2 = value; }
        }

        public PageBackstageHome()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(PageBackstageHome_MouseWheel);

            CreatePane();

            m_arrTabPage.Add(tabPage1);
        }

        private void PageBackstageHome_Load(object sender, EventArgs e)
        {
            //GetDockSOPLog().SetPane(m_paneSOPLog);
            m_dockSOPLog.TopLevel = false;
            splitContainerVertical.Panel2.Controls.Add(m_dockSOPLog);
            m_dockSOPLog.Dock = DockStyle.Fill;
            m_dockSOPLog.Visible = true;

            // Key : ButtonID
            // Value : SOP Full Path
            Dictionary<int, string> dicQuickSOPs = LoadBookMark();
            AddQuickButtons(dicQuickSOPs);
            InitButtons(dicQuickSOPs);

            InitScenarioPanel();

            labelScenarioName.Text = "";
            PageBackstageHome_Resize(null, null);
        }

        private void InitScenarioPanel()
        {
            Size sizeImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Background.Size;
            panelScenarioName.Size = new Size(sizeImage.Width, panelScenarioName.Height);
            labelScenarioName.MaximumSize = new Size(panelScenarioName.Size.Width - labelScenarioName.Location.X * 2, 0);
        }

        // Key : ButtonID
        // Value : SOP Full Path
        private void AddQuickButtons(Dictionary<int, string> dicQuickSOPs)
        {
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Fire, "화재", ID.ID_SOP_FIRE, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Earthquake, "지진", ID.ID_SOP_EARTHQUAKE, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Typhoon, "태풍", ID.ID_SOP_TYPHOON, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Submergence, "침수", ID.ID_SOP_SUBMERGENCE, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.General_Disaster, "일반재해", ID.ID_SOP_GENERAL_DISASTER, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.HeavySnow, "폭설", ID.ID_SOP_HEAVY_SNOW, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Terror, "테러", ID.ID_SOP_TERROR, dicQuickSOPs);
            AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.Pollution, "오염", ID.ID_SOP_POLLUTION, dicQuickSOPs);
        }

        // Key : ButtonID
        // Value : SOP Full Path
        private void AddQuickButton(Image img, string strButtonName, int nButtonID, Dictionary<int, string> dicQuickSOPs)
        {
            RibbonButtonQuick btn = new RibbonButtonQuick(img.Size.Width);

            btn.NormalImage = img;
            btn.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.RibbonCircleMouseOver_bkgnd;
            btn.Name = strButtonName;
            btn.Owner = this.panelBackImage;
            btn.Size = new Size(img.Size.Width, img.Size.Height);// + 20);
            btn.Text = strButtonName;
            btn.UseVisualStyleBackColor = true;

            if (dicQuickSOPs.ContainsKey(nButtonID))
                btn.Tag = dicQuickSOPs[nButtonID];
            else
                btn.Enabled = false;

            panelBackImage.AddQuickButton(btn);
        }

        // Key : ButtonID
        // Value : SOP Full Path
        private void InitButtons(Dictionary<int, string> dicQuickSOPs)
        {
            Image imgMouseOverBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonGrayChecked_bkgnd;

            // PanelTop SOP Quick Buttons
            InitRibbonButton(btnFire, ID.ID_SOP_FIRE, global::SOPMonitoringSystem.Properties.Resources.Fire_Normal, global::SOPMonitoringSystem.Properties.Resources.Fire_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnEarthquake, ID.ID_SOP_EARTHQUAKE, global::SOPMonitoringSystem.Properties.Resources.Earthquake_Normal, global::SOPMonitoringSystem.Properties.Resources.Earthquake_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnTyphoon, ID.ID_SOP_TYPHOON, global::SOPMonitoringSystem.Properties.Resources.Typhoon_Normal, global::SOPMonitoringSystem.Properties.Resources.Typhoon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnSubmergence, ID.ID_SOP_SUBMERGENCE, global::SOPMonitoringSystem.Properties.Resources.Submergence_Normal, global::SOPMonitoringSystem.Properties.Resources.Submergence_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnGeneralDisaster, ID.ID_SOP_GENERAL_DISASTER, global::SOPMonitoringSystem.Properties.Resources.General_Disaster_Normal, global::SOPMonitoringSystem.Properties.Resources.General_Disaster_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnHeavySnow, ID.ID_SOP_HEAVY_SNOW, global::SOPMonitoringSystem.Properties.Resources.HeavySnow_Normal, global::SOPMonitoringSystem.Properties.Resources.HeavySnow_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnTerror, ID.ID_SOP_TERROR, global::SOPMonitoringSystem.Properties.Resources.Terror_Normal, global::SOPMonitoringSystem.Properties.Resources.Terror_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnPollution, ID.ID_SOP_POLLUTION, global::SOPMonitoringSystem.Properties.Resources.Pollution_Normal, global::SOPMonitoringSystem.Properties.Resources.Pollution_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);

            ArrangeRibbonButtons(m_arrSOPButtons);

            // 불러오기 버튼
            InitRibbonButton(btnOpenSOP, ID.ID_OPEN_SOP, global::SOPMonitoringSystem.Properties.Resources.Open_SOP_Normal, global::SOPMonitoringSystem.Properties.Resources.Open_SOP_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, null, null);
            btnOpenSOP.Size = new Size(30, 30);
        }

        private void ArrangeRibbonButtons(ArrayList arrButtons)
        {
            int nButtonCount = arrButtons.Count;
            if (nButtonCount < 2)
                return;

            Size sizeButton = new Size(30, 30);

            RibbonButton btnPrev = (RibbonButton)arrButtons[0];
            btnPrev.Size = sizeButton;// btnPrev.NormalImage.Size;

            for (int i = 1; i < nButtonCount; i++)
            {
                RibbonButton btnNext = (RibbonButton)arrButtons[i];
                ArrangeRibbonButton(btnPrev, btnNext);

                btnPrev = btnNext;
                btnPrev.Size = sizeButton;// btnPrev.NormalImage.Size;
            }
        }

        private void ArrangeRibbonButton(Control ctrlPrev, Control ctrlNext)
        {
            ctrlNext.Location = new Point(ctrlPrev.Location.X + ctrlPrev.Size.Width, ctrlPrev.Location.Y);
        }

        // Key : ButtonID
        // Value : SOP Full Path
        private void InitRibbonButton(RibbonButton btn, int nID, Image imgNormal, Image imgChecked, Image imgDisabled, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, Image imgDisabledBkgnd, Dictionary<int, string> dicQuickSOPs, ArrayList arrButtons)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.DisabledImage = imgDisabled;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.DisabledBkgndImage = imgDisabledBkgnd;
            btn.Owner = this;

            SetButtonID(btn, nID, btn.Text);

            btn.Text = "";

            if (dicQuickSOPs != null)
            {
                if (!dicQuickSOPs.ContainsKey(nID))
                    btn.Enabled = false;
                else
                    btn.Tag = dicQuickSOPs[nID];
            }

            if (arrButtons != null)
                arrButtons.Add(btn);
        }

        private void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        // Key : ButtonID
        // Value : SOP Full Path
        Dictionary<int, string> LoadBookMark()
        {
            Dictionary<int, string> dicQuickSOPs = new Dictionary<int,string>();
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSOPFire = LoadBookMark("화재", "SOP BookMark", dbMgr);
            string strSOPPollution = LoadBookMark("오염", "SOP BookMark", dbMgr);
            string strSOPTyphoon = LoadBookMark("태풍", "SOP BookMark", dbMgr);

            if (strSOPFire.Length > 0)
                dicQuickSOPs[ID.ID_SOP_FIRE] = strSOPFire;

            if (strSOPPollution.Length > 0)
                dicQuickSOPs[ID.ID_SOP_POLLUTION] = strSOPPollution;

            if (strSOPTyphoon.Length > 0)
                dicQuickSOPs[ID.ID_SOP_TYPHOON] = strSOPTyphoon;

            return dicQuickSOPs;
        }

        private string LoadBookMark(string strTagName, string strSectionName, WebDBManager dbMgr)
        {
            return dbMgr.LoadIni(strTagName, strSectionName);
        }

        void PageBackstageHome_MouseWheel(object sender, MouseEventArgs e)
        {
            TabPage pageCurrent = this.TabControls.SelectedTab;
            if (pageCurrent == null)
                return;

            Point ptTabBegin = this.tabControl.Parent.Location;
            Rectangle rect = this.tabControl.DisplayRectangle;

            int nPanelX = e.X - (ptTabBegin.X + rect.X);
            int nPanelY = e.Y - (ptTabBegin.Y + rect.Y);

            //foreach (Sections.PanelSectionEx panel in m_arrPanel)
            foreach (Control control in pageCurrent.Controls)
            {
                if (control.GetType() != typeof(Sections.PanelSectionEx))
                    continue;

                if (!control.Visible)
                    continue;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;

                Point ptPanel = panel.Location;
                Size sizePanel = panel.Size;

                if (nPanelX >= ptPanel.X && nPanelX <= ptPanel.X + sizePanel.Width &&
                    nPanelY >= ptPanel.Y && nPanelY <= ptPanel.Y + sizePanel.Height)
                {
                    panel.WheelMouse(nPanelX - ptPanel.X, nPanelY - ptPanel.Y, e.Delta);
                    break;
                }
            }
        }

        public void CreatePane()
        {
            // Bottom
            /*Pane paneSOPLog = axDockingPane.CreatePane(0, 300, 170, DockingDirection.DockBottomOf, null);
            paneSOPLog.Title = "SOP Log";
            paneSOPLog.Options = PaneOptions.PaneNoCloseable;

            m_paneSOPLog = paneSOPLog;*/

            //// Left
            //m_paneProperties = axDockingPane.CreatePane(2, 280, 170, DockingDirection.DockLeftOf, null);
            //m_paneProperties.Title = "컴포넌트 속성";
            //m_paneProperties.Options = PaneOptions.PaneNoCloseable;

            //Pane panePropertiesLevel = axDockingPane.CreatePane(1, 280, 170, DockingDirection.DockTopOf, m_paneProperties);
            //panePropertiesLevel.Title = "위기관리 활동단계 속성";
            //panePropertiesLevel.Options = PaneOptions.PaneNoCloseable;
            //m_paneProperties.AttachTo(panePropertiesLevel);
            //panePropertiesLevel.Select();

            //Pane paneScenario = axDockingPane.CreatePane(0, 280, 270, DockingDirection.DockTopOf, panePropertiesLevel);
            //paneScenario.Title = "운용 중 시나리오";
            //paneScenario.Options = PaneOptions.PaneNoCloseable;

            ////Right
            //Pane panePersonnel = axDockingPane.CreatePane(5, 290, 350, DockingDirection.DockRightOf, null);
            //panePersonnel.Title = "SOP 요원 현황";
            //panePersonnel.Options = PaneOptions.PaneNoCloseable;

            //Pane paneProgress = axDockingPane.CreatePane(4, 290, 200, DockingDirection.DockTopOf, panePersonnel);
            //paneProgress.Title = "SOP 진행 현황";
            //paneProgress.Options = PaneOptions.PaneNoCloseable;

            //m_arrDocking[0] = new DockingLeftScenario();
            //m_dockScenario = (DockingLeftScenario)m_arrDocking[0];

            //m_arrDocking[1] = new DockingLeftPropertiesLevel();
            //m_dockPropertiesLevel = (DockingLeftPropertiesLevel)m_arrDocking[1];

            //m_arrDocking[2] = new DockingLeftProperties();
            //m_dockProperties = (DockingLeftProperties)m_arrDocking[2];

            m_arrDocking[0] = new DockingBottomSOPLog();
            m_dockSOPLog = (DockingBottomSOPLog)m_arrDocking[0];

            //m_arrDocking[4] = new DockingRightProgress();
            //m_dockProgress = (DockingRightProgress)m_arrDocking[4];

            //m_arrDocking[5] = new DockingRightPersonnel();
            //m_dockPersonnel = (DockingRightPersonnel)m_arrDocking[5];
        }

        private void PageBackstageHome_Resize(object sender, EventArgs e)
        {
            panelBackImage.Dock = DockStyle.Fill;

            // panelTop
            panelTop.Size = new Size(splitContainerMain.Panel1.Width, panelTop.Height);
            Point ptPanelScenarioName = new Point(panelTop.Size.Width - panelScenarioName.Size.Width - 5, panelScenarioName.Location.Y);
            Point ptOpenSOP = new Point(ptPanelScenarioName.X - btnOpenSOP.Size.Width, btnOpenSOP.Location.Y);

            int nSOPButtonCount = m_arrSOPButtons.Count;

            if (nSOPButtonCount == 0)
            {
                btnOpenSOP.Location = ptOpenSOP;
                panelScenarioName.Location = ptPanelScenarioName;
                return;
            }

            RibbonButton btnLast = (RibbonButton)m_arrSOPButtons[nSOPButtonCount - 1];
            int nLimitPos = btnLast.Location.X + btnLast.Size.Width;

            if (ptOpenSOP.X < nLimitPos)
            {
                btnOpenSOP.Location = new Point(nLimitPos, ptOpenSOP.Y);
                panelScenarioName.Location = new Point(btnOpenSOP.Location.X + btnOpenSOP.Size.Width, panelScenarioName.Location.Y);
            }
            else
            {
                btnOpenSOP.Location = ptOpenSOP;
                panelScenarioName.Location = ptPanelScenarioName;
            }
            /////////////////////////////////////////////////////////////////////

            // splitContainerVertical
            splitContainerVertical.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            splitContainerVertical.Size = new Size(splitContainerMain.Panel1.ClientSize.Width, splitContainerMain.Panel1.ClientSize.Height - splitContainerVertical.Location.Y);
            /////////////////////////////////////////////////////////////////////

            //tabControl.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            //tabControl.Size = new Size(splitContainerMain.Panel1.ClientSize.Width, splitContainerMain.Panel1.ClientSize.Height - tabControl.Location.Y);
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;

            if (m_dicButtonIDs.ContainsKey(btn))
            {
                int nID = m_dicButtonIDs[btn];

                switch (nID)
                {
                    case ID.ID_OPEN_SOP:
                        OpenSOP();
                        break;

                    case ID.ID_SOP_FIRE:
                    case ID.ID_SOP_EARTHQUAKE:
                    case ID.ID_SOP_GENERAL_DISASTER:
                    case ID.ID_SOP_HEAVY_SNOW:
                    case ID.ID_SOP_POLLUTION:
                    case ID.ID_SOP_SUBMERGENCE:
                    case ID.ID_SOP_TERROR:
                    case ID.ID_SOP_TYPHOON:
                        if (btn.IsChecked)
                            return;

                        LoadQuickSOP(btn);
                        break;
                }
            }
        }

        public void OpenSOP()
        {
            PopupSOPList sopList = new PopupSOPList();
            sopList.ShowDialog();
        }

        public string GetQuickSOPFullPath(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
            {
                Button btn = m_dicIDButtons[nID];
                return (string)btn.Tag;
            }

            return null;
        }

        public void LoadQuickSOP(RibbonButton btnSOP)
        {
			if (FormMain.Instance.HasControl == false)
				return;

            string strDisasterFullPath = (string)btnSOP.Tag;

            if (strDisasterFullPath == null)
                return;

            ReplaceDisasterPath(ref strDisasterFullPath);
            TreeNode node = FindDisasterNode(strDisasterFullPath);

            if (node != null)
            {
                CheckSOPButton(btnSOP);

                GetDockScenario().GetBarLevelTree().SelectNode(node);
                //GetDockScenario().GetBarLevelTree().LoadSOP(node); // node에 해당하는 sop

                //foreach (SectionTabPage page in FormMain.Instance.GetPageHome().TabControls.Controls) // 범례 위치 설정
                //{
                //    FormMain.Instance.GetPageHome().changeLocation(page.Height);
                //}
            }
        }

        private void ReplaceDisasterPath(ref string strPath)
        {
            int nIndex = strPath.IndexOf('/');

            if (nIndex < 0)
                return;

            int nIndex2 = strPath.IndexOf('/', nIndex + 1);

            if (nIndex2 < 0)
                return;

            strPath = strPath.Substring(0, nIndex) + ((char)0x06) + strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + ((char)0x06) + strPath.Substring(nIndex2 + 1);
        }

        //////////////////////////////////////////////////////////////////////////
        public DockingLeftScenario GetDockScenario()
        {
            return m_dockScenario;
        }

        public DockingLeftPropertiesLevel GetDockPropertiesLevel()
        {
            return m_dockPropertiesLevel;
        }

        public DockingLeftProperties GetDockProperties()
        {
            return m_dockProperties;
        }

        public DockingBottomSOPLog GetDockSOPLog()
        {
            return m_dockSOPLog;
        }

        public DockingRightProgress GetDockProgress()
        {
            return m_dockProgress;
        }

        public DockingRightPersonnel GetDockPersonnel()
        {
            return m_dockPersonnel;
        }

        public void SetDragDropShape(PointF[] arrDragDrop, Sections.Section.ComponentType sectionType)
        {
            m_arrDragDropOrigin = arrDragDrop;
            m_sectionDragDropType = sectionType;
        }

        public TabPage AddTabPage(Data_ActionStep data, bool bReal)
        {
            int ActionStepID = data.ID;
            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)Sections.TabPageManager.Instance.GetPage(ActionStepID, bReal);
            if (tabPage == null)
            {
                tabPage = new Sections.SectionTabPage();
                tabPage.Location = new System.Drawing.Point(4, 22);
                tabPage.Name = string.Format("TabPage_{0}", tabPage.Handle);
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = new System.Drawing.Size(706, 604);
                tabPage.Text = data.StepName;
                tabPage.ActionStepID = data.ID;
                tabPage.CreateNew = true;
            }
            else
            {
                tabPage.CreateNew = false;
            }
            tabPage.VirtualMode = !bReal;

            ++m_nActiopnStepID;

            tabControl.Controls.Add(tabPage);
            //m_ignoreTabChanged = true;
            tabControl.SelectedTab = tabPage;

            m_arrTabPage.Add(tabPage);
            m_dockPropertiesLevel.GetLevelProperties(tabPage);
            //m_dockScenario.GetBarLevelTree().AddTreeNode();
            m_dockPropertiesLevel.LevelProperties.Add(data);
            tabPage.ReSizePanel();
            return tabPage;
        }
        //////////////////////////////////////////////////////////////////////////
        // DB Loading을 통한 Tab Page 생성
        public TabPage AddTabPage(Data_ActionStep data)
        {
            bool bReal = FormMain.Instance.IsReal;
            return AddTabPage(data, bReal);
        }


        //ArrayList m_arrTeams = null;
        public FormLegend frmLegend;

        public void changecolor(int num, Color color)
        {
            frmLegend.ChangeBackColor(num, color);
        }
        public void changeLocation(int height)
        {
            try
            {
                if (frmLegend != null)
                    frmLegend.Location = new Point(0, height - frmLegend.Height);
            }
            catch
            {

            }
        }
		char szDeli = (char)0x06;
        public void SetScenarioName(int nActionStepID)
        {
            TreeNode node = DockScenario.GetBarLevelTree().FindActionStepNode(nActionStepID);

            if (node == null)
                return;

            while (node.Level > 2)
            {
                node = node.Parent;
            }

            if (node.Level == 2)
            {
                string strDisasterFullPath = node.FullPath.Replace('\\', szDeli);
                CheckSOPButton(strDisasterFullPath);
                labelScenarioName.Text = node.Text;
            }
            else
                labelScenarioName.Text = "";

            // Text 가운데 정렬(세로)
            labelScenarioName.Location = new Point(labelScenarioName.Location.X, (panelScenarioName.Size.Height - labelScenarioName.Size.Height) / 2);
        }

        // DB Loading을 통한 Panel 생성
        // Return 값 : 새로 생성된 Panel 리스트
        public ArrayList AddPane(ArrayList arrTeams, int nActionStepID, TabPage tabPage = null)
        {
            //m_arrTeams = arrTeams;
            m_arrPanel.Clear();
            int nTeamCount = arrTeams.Count;
            if (nTeamCount == 0)
                return null;

            if (tabPage == null)
                tabPage = tabControl.SelectedTab;

            SetScenarioName(nActionStepID);

            Size sz = new Size();
            sz.Width = tabPage.Size.Width / nTeamCount;
            sz.Height = tabPage.Height;

            Point pt = new Point(0, 0);
            ArrayList arrPanels = new ArrayList();

            for (int i = 0; i < nTeamCount; i++)
            {
                StepMemberData data = (StepMemberData)arrTeams[i];

                Sections.PanelSectionEx panel = new Sections.PanelSectionEx();
                panel.TeamName = data.TeamName;
                if (i % 2 == 0)
                    panel.BackColor = m_colorPanel1;
                if (i % 2 == 1)
                    panel.BackColor = m_colorPanel2;

                panel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom));
                panel.AutoScroll = false;
                panel.Dock = System.Windows.Forms.DockStyle.None;
                panel.Location = new System.Drawing.Point(pt.X, 0);
                panel.Name = string.Format("panel{0}", i + 1);
                panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.StepName = tabControl.SelectedTab.Text;
                panel.TeamName = data.TeamName;
                panel.TeamID = data.TeamID;
                panel.TeamType = data.TeamType;
                panel.SetListener(this);
                panel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel1_MouseMove);
                panel.AddPanelTitle(data.TeamName);
                panel.ActionStepID = nActionStepID;

                ((Sections.SectionTabPage)tabPage).Controls.Add(panel);
                pt.X += sz.Width;

                m_arrPanel.Add(panel);
                arrPanels.Add(panel);

                if (i == 0)
                {
                    frmLegend = new FormLegend();

                    //MessageBox.Show(panel.Height.ToString() + "      " + panel.Width.ToString());

                    frmLegend.Location = new Point(0, 100);
                    frmLegend.Dock = DockStyle.None;
                    frmLegend.TopLevel = false;
                    frmLegend.Parent = this;
                    panel.Controls.Add(frmLegend);
                    panel.Legend = frmLegend;

					if (FormMain.Instance.ShowLegend == true)
						frmLegend.Show();
					else
						frmLegend.Visible = false;

                    frmLegend.ChangeBackColor(0, Color.FromArgb(FormMain.Instance.GetPageOption().getColor(0)));
                    frmLegend.ChangeBackColor(1, Color.FromArgb(FormMain.Instance.GetPageOption().getColor(1)));
                    frmLegend.ChangeBackColor(2, Color.FromArgb(FormMain.Instance.GetPageOption().getColor(2)));
                    frmLegend.ChangeBackColor(3, Color.FromArgb(FormMain.Instance.GetPageOption().getColor(3)));
                    frmLegend.ChangeBackColor(4, Color.White);
                }
            }

            return arrPanels;
        }

        public void SelectTab(TabPage tabPage)
        {
            //m_ignoreTabChanged = true;
            tabControl.SelectedTab = tabPage;
            if (tabControl.Visible == false)
                tabControl.Visible = true;
            m_dockPropertiesLevel.GetLevelProperties(tabPage);
            m_dockScenario.GetBarPage().SetDataGrid((Sections.SectionTabPage)tabPage);
            ShowActionStepLog(tabPage);
        }

        public void RemoveTabPage(TabPage tabPage)
        {
            if (tabPage == null)
            {
                tabPage = tabControl.SelectedTab;
                if (tabPage == null)
                    return;
            }

            //foreach (Sections.PanelSectionEx panel in tabPage.Controls)
            //{
            //    m_arrPanel.Remove(panel);
            // }

            //m_arrTabPage.Remove(tabPage);
            tabControl.Controls.Remove(tabPage);
        }

        // panelExcept를 제외한 모든 Panel의 선택을 해제한다.
        public void ClearSelection(Sections.PanelSectionEx panelExcept)
        {
            TabPage page = tabControl.SelectedTab;
            if (page == null)
                return;

            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control control in page.Controls)
            {
                if (control.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;
                    if (panel == panelExcept)
                        continue;

                    panel.ClearSelection();
                    panel.Refresh();
                }
            }
        }

        private void AddPanelTitle(string strTitle, Sections.PanelSectionEx panel)
        {
            Label label = new Label();
            label.Dock = DockStyle.Top;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = Color.Gold;
            label.Text = strTitle;

            panel.Controls.Add(label);
        }

        public string GetTabPageName()
        {
            TabPage tabPage = tabControl.SelectedTab;
            return tabPage.Text;
        }

        // 현재 TabPage 상에 나타난 Panel들
        public ArrayList GetPanels()
        {
            if (tabControl.SelectedTab == null)
                return null;

            Type type = typeof(Sections.PanelSectionEx);
            ArrayList arrPanels = new ArrayList();

            foreach (Control ctrl in tabControl.SelectedTab.Controls)
            {
                if (ctrl.GetType() == type)
                    arrPanels.Add(ctrl);
            }

            return arrPanels;
        }

		private Dictionary<PanelSectionEx, bool> m_arTempVisiblePanel = new Dictionary<PanelSectionEx, bool>();
		private bool m_bDiasblePanels = false;
		public void ResotreShowPanels()
		{
			m_bDiasblePanels = false;
			ArrayList arPanels = GetPanels();
			foreach (Sections.PanelSectionEx pane in arPanels)
			{
				if (pane != null && pane.IsHandleCreated && !pane.IsDisposed && pane.Visible == true)
				{
					if (m_arTempVisiblePanel.ContainsKey(pane))
					{
						bool bEnabled = m_arTempVisiblePanel[pane];
						pane.Enabled = bEnabled;
					}
					else
						pane.Enabled = true;					
				}
			}
			m_arTempVisiblePanel.Clear();
		}

		public void DiableShowPanels()
		{
			if (m_bDiasblePanels == true)
				return;

			m_bDiasblePanels = true;
			m_arTempVisiblePanel.Clear();

			ArrayList arPanels = GetPanels();
			foreach (Sections.PanelSectionEx pane in arPanels)
			{
				if (pane != null && pane.IsHandleCreated && !pane.IsDisposed && pane.Visible == true)
				{
					if (!m_arTempVisiblePanel.ContainsKey(pane))
					{
						m_arTempVisiblePanel[pane] = pane.Enabled;
					}
					
					pane.Enabled = false;
				}
			}			
		}


        public ArrayList GetAllPanels(int nActionStepID)
        {
            Control.ControlCollection ctrlList = tabControl.Controls;
            Type type = typeof(Sections.SectionTabPage);

            foreach (Control ctrl in ctrlList)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.SectionTabPage tabPage = (Sections.SectionTabPage)ctrl;

                    if (tabPage.ActionStepID == nActionStepID)
                    {
                        ArrayList arrPanels = new ArrayList();
                        Type type2 = typeof(Sections.PanelSectionEx);

                        foreach (Control control in tabPage.Controls)
                        {
                            if (control.GetType() == type2)
                                arrPanels.Add(control);
                        }

                        return arrPanels;
                    }
                }
            }

            return null;
        }

        public void ShowPanel(Sections.PanelSection pane)
        {
            Sections.SectionTabPage page = (Sections.SectionTabPage)pane.Parent;
            if (page != null)
            {
                page.ReSizePanel();
            }
        }

        public void PanelResize()
        {
            Sections.SectionTabPage tabPage1 = (Sections.SectionTabPage)tabControl.SelectedTab;
            if (tabPage1 != null)
                tabPage1.ReSizePanel();
        }

        public SplitContainer panel
        {
            get { return splitContainerMain; }
        }

        public TabControl TabControls
        {
            get { return tabControl; }
        }

        public ArrayList GetTabPage()
        {
            return m_arrTabPage;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (Sections.PanelSectionEx panel in m_arrPanel)
            {
                if (panel != sender)
                    continue;

                if (m_arrDragDropOrigin == null)
                {
                    panel.MoveDrawingArray(null, Sections.Section.ComponentType.NONE, 0, 0);
                    return;
                }

                Point ptPanel = new Point(0, 0);
                Size sizePanel = panel.Size;

                if (e.X >= ptPanel.X && e.X <= ptPanel.X + sizePanel.Width && e.Y >= ptPanel.Y && e.Y <= ptPanel.Y + sizePanel.Height)
                    panel.MoveDrawingArray(m_arrDragDropOrigin, m_sectionDragDropType, e.X - ptPanel.X, e.Y - ptPanel.Y);
                else
                    panel.MoveDrawingArray(null, m_sectionDragDropType, 0, 0);
            }
        }

        private TreeNode GetTabDisasterNode(TabPage tabPage, BarLevelTree tree)
        {
            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                    if (panel.ActionStepID < 0)
                        return null;

                    TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
                    if (node == null) return null;

                    while (node.Level > 2)
                    {
                        node = node.Parent;
                    }

                    return node;
                }
            }

            return null;
        }

        private void ShowActionStepLog(int nActionStepID, bool isRealMode, bool updateComponentContents = true)
        {
            if (nActionStepID < 0) return;

            TreeNode node = GetDockScenario().GetBarLevelTree().FindActionStepNode(nActionStepID);

            if (node != null)
            {
                string strFullPath = node.FullPath;
                GetDockSOPLog().ShowActionStepLog(nActionStepID, isRealMode, strFullPath.Replace('\\', szDeli), updateComponentContents);
            }
        }

        private void ShowActionStepLog(TabPage tabPage)
        {
            Sections.SectionTabPage page = (Sections.SectionTabPage)tabPage;
            int nActionStepID = FormMain.Instance.GetTabActionStepID(page);
            ShowActionStepLog(nActionStepID, !page.VirtualMode);
        }

        public void ColorChangedPanel()
        {
            foreach (TabPage page in m_arrTabPage)
            {
                int i = 0;
                foreach (Control contorl in page.Controls)
                {
                    if (contorl.GetType() == typeof(Sections.PanelSectionEx))
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)contorl;
                        if (i % 2 == 0)
                            panel.BackColor = m_colorPanel1;
                        if (i % 2 == 1)
                            panel.BackColor = m_colorPanel2;
                        i++;
                    }
                }
            }
        }

        //현재 탭에 보여지는 섹션 중 전체 프로세스의 팀 리스트를 가져온다 
        public void GetTeamList()
        {
            m_arrTeams.Clear();

            TabPage tabPage = tabControl.SelectedTab;
            if (tabPage == null)
                return;

            foreach (Sections.PanelSectionEx panel in tabPage.Controls)
            {
                foreach (Sections.Section section in panel.Sections)
                {
                    Sections.Section.ComponentType type = section.GetComponentType();

                    if (type == Sections.Section.ComponentType.PROCESS) //프로세스
                    {
                        Sections.SectionProcess psection = (Sections.SectionProcess)section;
                        string str = psection.TextDown;
                        Sections.SectionDataProcess data = (Sections.SectionDataProcess)psection.Data;
                        ArrayList arrTeam = data.TeamList;
                        //teamtype : 0 Normal 1 Emergency
                        foreach (Sections.SOPTeam team in arrTeam)
                        {
                            m_arrTeams.Add(team);
                        }
                    }
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////
        private void SelectedSection()
        {
            //m_paneProperties.Select();
        }

        public void OnSelectedArrow(Sections.Arrow arrow)
        {
            ClearSelection(m_currentPanel);
        }

        public static bool IsWorkingMode(int nActionStepID, bool isRealMode)
        {
            WorkFlow work = WorkFlowManager.Instance.Get(nActionStepID, isRealMode);

            if (work == null)
                return false;

            return work.State == WorkFlowState.RUN;
        }

        public bool IsWorkingMode(Sections.Section section)
        {
            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)section.GetParent().Parent;
            return IsWorkingMode(tabPage.ActionStepID, !tabPage.VirtualMode);
        }


        public void OnSelectedSectionList(ArrayList arSections)
        {
            if (arSections == null)
                System.Diagnostics.Trace.WriteLine("OnSelectedSectionList : null");
            else
                System.Diagnostics.Trace.WriteLine("OnSelectedSectionList : " + arSections.Count);
        }

        public void OnSelectedSection(Sections.Section section)
        {
            if (section == null)
            {
                if (m_currentPanel != null)
                {
                    Sections.SectionTabPage tabPage = (Sections.SectionTabPage)m_currentPanel.Parent;
                    ShowActionStepLog(m_currentPanel.ActionStepID, !tabPage.VirtualMode, false);
                }
                return;
            }

            if (FormMain.Instance.HasControl && IsWorkingMode(section))
                ChangeSelectedSectionState(section);

            ShowSectionProperty(section);
        }

        private bool IsBeginSection(Sections.Section section)
        {
            if (section.GetComponentType() != Sections.Section.ComponentType.ENDPOINT)
                return false;

            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
            return data.IsBegin;
        }

        // 선택된 Section의 상태 정보를 바꾸어준다.
        private void ChangeSelectedSectionState(Sections.Section section)
        {
            Sections.Section.ComponentType type = section.GetComponentType();
            FormMain.Instance.CurrentSection = section;

            if (FormMain.Instance.CurrentWork == null)
                return;

            Sections.SectionState state = FormMain.Instance.CurrentWork.FindState(section);
            if (state == null) return;

            if (state.State == Sections.State.RUN)
            {
				int i = 0;
				i++;
                //MessageBox.Show("run~!!!");
            }
            else
            {
                if (type == Sections.Section.ComponentType.TRANSMISSION)
                {
                    Sections.TSectionState tstate = (Sections.TSectionState)state;

                    if (tstate.Section.AdditionalPainter != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)tstate.Section.AdditionalPainter;
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, tstate);
                    }

                    tstate.InProgress();
                }
                else if (type == Sections.Section.ComponentType.INTERNAL)
                {
                    Sections.ISectionState istate = (Sections.ISectionState)state;

                    if (istate.Section.AdditionalPainter != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)istate.Section.AdditionalPainter;
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, istate);
                    }

                    istate.InProgress();
                }
                else if (type == Sections.Section.ComponentType.EXTERNAL)
                {
                    Sections.ESectionState estate = (Sections.ESectionState)state;

                    if (estate.Section.AdditionalPainter != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)estate.Section.AdditionalPainter;
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, estate);
                    }

                    estate.InProgress();
                }
                // 시작 Section의 정보는 바뀌지 않는다.
                else if (!IsBeginSection(section))
                {
                    if (state.Section.AdditionalPainter != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)state.Section.AdditionalPainter;
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, state);
                    }

                    state.InProgress();
                }

                // LButton Clicked 상태를 해제시켜준다.
                PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                
				panel.ClearLButtonClick();

                Refresh();
            }
        }

        public void ShowSectionProperty(Sections.Section section)
        {
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
            int nComponentID = panel.GetComponentID(section);
            Sections.Section.ComponentType type = section.GetComponentType();

            if (nComponentID > 0)
            {
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;
                GetDockSOPLog().ShowComponentLog(nComponentID, !tabPage.VirtualMode, type, section);
            }

            if (type == Sections.Section.ComponentType.PROCESS) //프로세스
            {
                m_dockProperties.ShowProperties(1);
                m_dockProperties.GetPropertiesProcess().GetSectionData((Sections.SectionProcess)section);
            }
            else if (type == Sections.Section.ComponentType.DECISION) // 판단
            {
                m_dockProperties.ShowProperties(2);
                m_dockProperties.GetPropertiesDecision().GetSectionData((Sections.SectionDecision)section);
            }
            else if (type == Sections.Section.ComponentType.ANNOTATION) // 설명
            {
                m_dockProperties.ShowProperties(3);
                m_dockProperties.GetPropertiesAnnotation().GetSectionData((Sections.SectionAnnotation)section);
            }
            else if (type == Sections.Section.ComponentType.ENDPOINT) // 시작/끝
            {
                m_dockProperties.ShowProperties(4);
                m_dockProperties.GetPropertiesEndPoint().GetSectionData((Sections.SectionEndPoint)section);
            }
            else if (type == Sections.Section.ComponentType.LINK) // 링크
            {
                m_dockProperties.ShowProperties(5);
                m_dockProperties.GetPropertiesLink().GetSectionData((Sections.SectionLink)section);
            }
            else if (type == Sections.Section.ComponentType.TRANSSOP) // 다른 SOP로 전환
            {
                m_dockProperties.ShowProperties(6);
                m_dockProperties.GetPropertiesTransSOP().GetSectionData((Sections.SectionTransSOP)section);
            }
            else if (type == Sections.Section.ComponentType.INTERNAL) // 내부 상황전파
            {
                m_dockProperties.ShowProperties(7);
                m_dockProperties.GetPropertiesInternal().GetSectionData((Sections.SectionInternal)section);
            }
            else if (type == Sections.Section.ComponentType.EXTERNAL) // 외부 상황전파
            {
                m_dockProperties.ShowProperties(8);
                m_dockProperties.GetPropertiesExternal().GetSectionData((Sections.SectionExternal)section);
            }
            else if (type == Sections.Section.ComponentType.TRANSMISSION)   // 통합 상황전파
            {
                m_dockProperties.ShowProperties(9);
                m_dockProperties.GetPropertiesTransmission().SetSection((Sections.SectionTransmission)section);
            }
            else if (type == Sections.Section.ComponentType.NONE) //
            {
            }

            SelectedSection();
        }

        public void SetCurrentPanel(Sections.PanelSection panel)
        {
            m_currentPanel = (Sections.PanelSectionEx)panel;
        }

        public void DeleteOptionChanged(object sender, DeleteOptionChangeEventArgs e)
        {
            m_dockScenario.DeleteOptionChanged(sender, e);
        }

        public void ChangeWaterMark(bool bUse)
        {
            TabPage page = tabControl.SelectedTab;
            if (page != null)
            {
                Sections.SectionTabPage tabpage = (Sections.SectionTabPage)page;
                tabpage.UseWaterMark = bUse;
                tabpage.Refresh();
            }
        }

        public bool IsChangeCurrentTab()
        {
            TabPage page = tabControl.SelectedTab;
            if (page == null)
            {
                return true;
            }

            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)page;
            if (tabPage.VirtualMode == !FormMain.Instance.IsReal)
            {
                return false;
            }
            return true;
        }

        public void SetBackgroundImage(bool isVisible)
        {
            if (!isVisible)
            {
                Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Background_Symbol);
                panelBackImage.BackgroundImage = bitmap;
                //panelBackImage.BackgroundImageLayout = ImageLayout.Stretch;
                panelBackImage.BackgroundImageLayout = ImageLayout.Center;
                panelBackImage.BackColor = Color.FromArgb(52, 73, 94);
            }
            else
            {
                Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.BackgroundNon);
                panelBackImage.BackgroundImage = bitmap;
                panelBackImage.BackgroundImageLayout = ImageLayout.None;

                splitContainerMain.Visible = true;
            }

        }

        public void GetComponentContents(int nActionStepID, int nComponentHistoryID, Sections.Section.ComponentType componentType, DateTime time, string strComponentType, string strTask, string strStatus, Sections.Section section, Sections.State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow logRow)
        {
            if (m_dockScenario == null || section == null) return;
            ArrayList arrAllSections = GetDockScenario().GetAllPanelSections(m_arrPanel);

            int nSectionCount = arrAllSections.Count;

            int nSelectedActionID = -1;
            DataGridView dataGrid = GetDockScenario().GetGridView();
            foreach (DataGridViewRow row in dataGrid.SelectedRows)
            {
                nSelectedActionID = (int)row.Cells[3].Tag;
            }

            if (FormMain.Instance.CurrentWork == null)
                return;

            Sections.SectionState state = FormMain.Instance.CurrentWork.FindState(section);
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
            int nComponentID = panel.GetComponentID(section);

            if (state == null)
                return;

            if (nSelectedActionID == nActionStepID)
            {
                //Sections.Section section = (Sections.Section)arrAllSections[i];
                if (state.State == Sections.State.RUN)
                {
                    CreateComponentContents(nComponentID, nComponentHistoryID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, logRow);
                }
                else if (state.State == Sections.State.DONE)
                {
                    bool isFlag = false;
                    SectionTabPage page = (SectionTabPage)panel.Parent;
                    ComponentContents frmContents = GetComponentContents(nActionStepID, !page.VirtualMode, nComponentHistoryID);

                    //foreach (ComponentContents frmContents in m_arrSectionLog)
                    if (frmContents != null)
                    {
                        if (nComponentID == frmContents.ComponentID)
                        {
                            string strOldTitle = frmContents.GetTitle();
                            string[] strTemp = strOldTitle.Split('/');
                            if (strTemp[strTemp.Length - 1] != "실행완료")
                            {
                                if (strTemp.Length < 2)
                                    return;

                                string strLast = strTemp[strTemp.Length - 1];

                                if (strLast == "-")
                                {
                                    strLast = "***";
                                    strOldTitle = strOldTitle.Replace("/-", "/" + strLast);
                                    strTemp[strTemp.Length - 1] = strLast;
                                }

                                if (strStatus.StartsWith("'"))
                                    strStatus = strStatus.Remove(0, 1);
                                if (strStatus.EndsWith("'"))
                                    strStatus = strStatus.Remove(strStatus.Length - 1);

                                string strValue = strOldTitle.Replace(strTemp[strTemp.Length - 2], time.ToString());
                                strValue = strValue.Replace(strTemp[strTemp.Length - 1], strStatus);
                                string strNewTitle = "";
                                if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
                                {
                                    if (strValue.EndsWith("'"))
                                        strValue.Remove(strValue.Length - 1);

                                    if (strTemp[strTemp.Length - 1] == "-")
                                    {
                                        string[] str = strValue.Split('/');
                                        string str1 = str[str.Length - 1];
                                        int nIndex = strValue.LastIndexOf('-');

                                        if (nIndex >= 0)
                                            strNewTitle = strValue.Substring(0, nIndex) + /*"실행 완료 " +*/ strStatus;
                                    }
                                    else if (strTemp[strTemp.Length - 1] != "실행완료")
                                    {
                                        strNewTitle = strValue;
                                    }
                                }
                                else
                                {
                                    strNewTitle = strValue.Replace("실행중", "실행 완료");
                                }

                                //frmContents.gridView.Enabled = false;
                                frmContents.EnableGrid(false);
                                frmContents.ChangeTitle(strNewTitle);
                                frmContents.GetPanel().BackColor = Color.DimGray;
                                frmContents.UpdateContents(nCheckNotify1, nCheckNotify2);
                                isFlag = true;
                            }
                        }
                    }
                    if (isFlag == false/* || (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)*/)
                    {
                        CreateComponentContents(nComponentID, nComponentHistoryID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, logRow);
                    }
                }
            }
        }

        public static void MakeComponentContentsData(ComponentContents frmContents, string strTask, DateTime time, string strStatus, Sections.Section section, Sections.State sectionState, int nCheckNotify1, int nCheckNotify2)
        {
            frmContents.SetTitle(strTask, time, strStatus);
            frmContents.AddGridData(section, strStatus, nCheckNotify1, nCheckNotify2);
            frmContents.State = sectionState;

        }

        private ComponentContents MakeComponentContents(int nActionStepID, bool isRealMode, int nComponentHistoryID, int nComponentID, string strTask, DateTime time, string strStatus, Sections.Section section, Sections.State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow row)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            int nContentsCount = arrContents == null ? 0 : arrContents.Count;

            ComponentContents frmContents = new ComponentContents();

            frmContents.Location = new Point(0, frmContents.Height * nContentsCount);
            //m_nContents++;
            //frmContents.Location = new Point(0, frmContents.GetPanel().Height * i);
            //frmContents.Dock = DockStyle.Fill;
            frmContents.Anchor = ((System.Windows.Forms.AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right));
            frmContents.TopLevel = false;
            frmContents.Parent = this;
            frmContents.ComponentID = nComponentID;
            frmContents.ComponentHistoryID = nComponentHistoryID;
            frmContents.LogGridRow = row;

            bool scrollVisible = splitContainerMain.Panel2.VerticalScroll.Visible;

            if (scrollVisible)
                frmContents.Size = new Size(splitContainerMain.Panel2.Width - 18, frmContents.Height);
            else
                frmContents.Size = new Size(splitContainerMain.Panel2.Width, frmContents.Height);

            if (sectionState == State.DONE)
            //if (strStatus == "실행 완료")
            {
                frmContents.GetPanel().BackColor = Color.DimGray;
                //frmContents.gridView.Enabled = false;
                frmContents.EnableGrid(false);
            }

            splitContainerMain.Panel2.Controls.Add(frmContents);

            MakeComponentContentsData(frmContents, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2);
            /*frmContents.SetTitle(strTask, time, strStatus);
            frmContents.AddGridData(section, strStatus, nCheckNotify1, nCheckNotify2);
            frmContents.State = sectionState;*/
            frmContents.Show();
            frmContents.Select();
            return frmContents;
        }

        private void UpdateComponentContents(ComponentContents frmContents, string strTask, DateTime time, string strStatus, Sections.Section section, Sections.State sectionState, int nCheckNotify1, int nCheckNotify2, bool noDBWrite)
        {
            if (sectionState == State.DONE)
            //if (strStatus == "실행 완료")
            {
                frmContents.GetPanel().BackColor = Color.DimGray;
                //frmContents.gridView.Enabled = false;
                frmContents.EnableGrid(false);
            }

            frmContents.SetTitle(strTask, time, strStatus);

            if (sectionState != State.DONE || noDBWrite)
            {
                // 실행완료 상태일 경우 nCheckNotify1, 2가 초기화되는 현상이 발생함
                // 완료상태는 직전 상태와 CheckNotify가 동일하므로 굳이 바꿀 필요가 없음
                frmContents.UpdateContents(nCheckNotify1, nCheckNotify2);
            }

            frmContents.State = sectionState;
            frmContents.Show();
            frmContents.Select();

            if (sectionState == State.DONE)
            {
                Sections.SectionState state = FormMain.Instance.CurrentWork.FindState(section);

                if (state != null)
                    BackToOriginState(state);
            }
        }

        // state의 CheckNotify1, CheckNotify2를 원래 상태대로 돌려놓는다.
        private void BackToOriginState(SectionState state)
        {
            Sections.Section.ComponentType type = state.Section.GetComponentType();

            int nCheckedNotify1 = 0, nCheckedNotify2 = 0;

            if (type == Section.ComponentType.PROCESS)
                WorkFlow.GetProcessCheckedNotify((SectionProcess)state.Section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Section.ComponentType.INTERNAL)
                WorkFlow.GetInternalCheckedNotify((SectionInternal)state.Section, out nCheckedNotify1);
            else if (type == Section.ComponentType.EXTERNAL)
                WorkFlow.GetExternalCheckedNotify((SectionExternal)state.Section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Section.ComponentType.TRANSMISSION)
                WorkFlow.GetTransmissionCheckedNotify((SectionTransmission)state.Section, out nCheckedNotify1, out nCheckedNotify2);

            state.CheckNotify1 = nCheckedNotify1;
            state.CheckNotify2 = nCheckedNotify2;
        }

        private void ProcessDoneComponents(int nActionStepID, bool isRealMode, Section section, int nComponentID)
        {
            Sections.SectionState state = FormMain.Instance.CurrentWork.FindState(section);

            if (state != null)
                BackToOriginState(state);

            // 같은 Section의 직전 Log가 완료 상태로 끝나지 않았다면 제거한다.
            RemovePrevComponent(nActionStepID, isRealMode, nComponentID);
        }

        private void RemovePrevComponent(int nActionStepID, bool isRealMode, int nComponentID)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
                return;

            // 마지막 Contents는 자신의 완료 로그이므로 건너뛴다.
            int nContentsCount = arrContents.Count - 1;

            for (int i = nContentsCount - 1; i >= 0; i--)
            {
                ComponentContents contents = (ComponentContents)arrContents[i];

                if (contents.ComponentID == nComponentID)
                {
                    if (contents.State != State.DONE)
                    {
                        foreach (Control ctrl in splitContainerMain.Panel2.Controls)
                        {
                            if (ctrl == contents)
                            {
                                arrContents.RemoveAt(i);
                                splitContainerMain.Panel2.Controls.Remove(ctrl);
                                ReLocation();
                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void SetMissionStatus(int nActionStepID, bool isRealMode)
        {
            FormMissionStatus frmMissionStatus = FormMain.Instance.FrmMain3;

            if (frmMissionStatus == null)
                return;

            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents == null || arrContents.Count == 0)
            {
                frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.PREV_ITEM);
                frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.CURRENT_ITEM);
                frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.NEXT_ITEM);
                return;
            }

            int nContentsCount = arrContents.Count;

            ComponentContents contentsCurrent = (ComponentContents)arrContents[nContentsCount - 1];
            Sections.Section sectionNext = null;

            if (contentsCurrent.LogGridRow != null)
            {
                Sections.Section sectionCurrent = contentsCurrent.LogGridRow.Section;
                Sections.WorkFlow workCurrent = FormMain.Instance.CurrentWork;

                if (workCurrent != null && sectionCurrent != null)
                {
                    Sections.SectionState stateCurrent = Sections.WorkFlowManager.Instance.Find(sectionCurrent, isRealMode);

                    if (stateCurrent != null)
                    {
                        ArrayList arrNextStates = workCurrent.FindNext(stateCurrent);

                        if (arrNextStates != null && arrNextStates.Count > 0)
                        {
                            Sections.SectionState stateNext = (Sections.SectionState)arrNextStates[0];
                            if (stateNext != null)
                                sectionNext = stateNext.Section;
                        }
                    }
                }
            }

            if (nContentsCount >= 2)
            {
                frmMissionStatus.SetContents((ComponentContents)arrContents[nContentsCount - 1], FormMissionStatus.ItemType.CURRENT_ITEM);
                frmMissionStatus.SetContents((ComponentContents)arrContents[nContentsCount - 2], FormMissionStatus.ItemType.PREV_ITEM);
                frmMissionStatus.SetSectionContents(sectionNext, FormMissionStatus.ItemType.NEXT_ITEM);
            }
            else
            {
                frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.PREV_ITEM);
                frmMissionStatus.SetContents((ComponentContents)arrContents[0], FormMissionStatus.ItemType.CURRENT_ITEM);
                frmMissionStatus.SetSectionContents(sectionNext, FormMissionStatus.ItemType.NEXT_ITEM);
            }
        }

        private void CreateComponentContents(int nComponentID, int nComponentHistoryID, string strTask, DateTime time, string strStatus, Sections.Section section, Sections.State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow row)
        {
            PanelSectionEx panel = (PanelSectionEx)section.GetParent();
            SectionTabPage page = (SectionTabPage)panel.Parent;

            ComponentContents lastContents = GetLastComponentContents(page.ActionStepID, !page.VirtualMode);

            // 바로 직전에 같은 Component에 대한 로그가 기록되어 있는가?
            if (lastContents != null && lastContents.ComponentID == nComponentID)
            {
                int nIndex = lastContents.Title.LastIndexOf('/');
                if (nIndex >= 0)
                {
                    string str = lastContents.Title.Substring(nIndex + 1);

                    if (str == "실행중")
                        lastContents.State = State.RUN;
                }

                if (lastContents.ComponentHistoryID == nComponentHistoryID)
                {
                    UpdateComponentContents(lastContents, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                }
                // 직전 State와 같다면 새로운 Action이 행해졌다.
                else if (lastContents.State == sectionState || lastContents.State == State.DONE)
                {
                    ComponentContents contents = MakeComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID, nComponentID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row);
                    AddComponentContents(page.ActionStepID, !page.VirtualMode, contents);

                    if (sectionState == State.DONE)
                        ProcessDoneComponents(page.ActionStepID, !page.VirtualMode, section, nComponentID);
                }
                else
                {
                    UpdateComponentContents(lastContents, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                }
            }
            else
            {
                ComponentContents contents = GetComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID);

                if (contents == null)
                {
                    contents = MakeComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID, nComponentID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row);
                    AddComponentContents(page.ActionStepID, !page.VirtualMode, contents);

                    if (sectionState == State.DONE)
                        ProcessDoneComponents(page.ActionStepID, !page.VirtualMode, section, nComponentID);
                }
                else
                {
                    UpdateComponentContents(contents, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                }
            }

            // 현재 ComponentContents의 내용을 임무현황판(FormMissionStatus)에 전달한다.
            SetMissionStatus(page.ActionStepID, !page.VirtualMode);

        }

        public void ClearProcess()
        {
            foreach (object obj in splitContainerMain.Panel2.Controls)
            {
                if (obj.GetType() == typeof(ComponentContents))
                {
                    ComponentContents frmContents = (ComponentContents)obj;

                    foreach (KeyValuePair<long, ArrayList> pair in m_dicComponentContents)
                    {
                        ArrayList arrContents = pair.Value;
                        if (arrContents.Count == 0)
                            continue;

                        if (frmContents == (ComponentContents)arrContents[0])
                        {
                            arrContents.Clear();
                            splitContainerMain.Panel2.Controls.Clear();
                            return;
                        }
                    }

                    break;
                }
            }
        }

        public void ReLocation()
        {
            int nCount = splitContainerMain.Panel2.Controls.Count;

            int nHeight = 0;
            ComponentContents OldfrmContents = null;
            
            // 먼저 만들어진 Contents가 위로 가도록 구성
            /*foreach (ComponentContents frmContents in splitContainerMain.Panel2.Controls)
            {
                if (OldfrmContents == null)
                {
                    nHeight = frmContents.Location.Y;
                    frmContents.Location = new Point(0, nHeight);
                }
                else
                    frmContents.Location = new Point(0, nHeight);

                OldfrmContents = frmContents;
                nHeight += OldfrmContents.Height;
            }*/

            // 나중에 만들어진 Contents가 위로 가도록 구성
            for (int i=nCount-1;i>=0;i--)
            {
                ComponentContents frmContents = (ComponentContents)splitContainerMain.Panel2.Controls[i];

                frmContents.Location = new Point(0, nHeight);

                OldfrmContents = frmContents;
                nHeight += OldfrmContents.Height;
            }
        }

        private void splitContainerMain_Panel2_Resize(object sender, EventArgs e)
        {
            foreach (ComponentContents frmContents in splitContainerMain.Panel2.Controls)
            {
                if (splitContainerMain.Panel2.Controls.Count > 5)
                    frmContents.Size = new Size(splitContainerMain.Panel2.Width - 18, frmContents.Height);
                else
                    frmContents.Size = new Size(splitContainerMain.Panel2.Width, frmContents.Height);
            }

            PageBackstageHome_Resize(null, null);
        }

        public Sections.PanelSectionEx GetCurrentPanel()
        {
            return m_currentPanel;
        }

        public void OnEnabled(bool isFlag)
        {
            foreach (object obj in splitContainerMain.Panel2.Controls)
            {
                if (obj.GetType() == typeof(ComponentContents))
                {
                    ComponentContents contents = (ComponentContents)obj;
                    contents.EnableGrid(isFlag);
                }
            }
            splitContainerMain.Enabled = isFlag;
        }

        //////////////////////////////////// 즐겨찾기 - 자연재해 버튼
        /*private void toolStripBtn_A_1_Click(object sender, EventArgs e)
        {
            TreeNode node = GetDockScenario().GetBarLevelTree().FindActionStepNode(12);
            GetDockScenario().GetBarLevelTree().SelectSop(node);

            //MessageBox.Show(node.Text);

            //GetDockScenario().GetBarLevelTree().LoadSOP(node);
        }*/

        private TreeNode FindDisasterNode(string strDisasterFullPath)
        {
			int nIndex1 = strDisasterFullPath.IndexOf((char)0x06);
            if (nIndex1 < 0) return null;

			int nIndex2 = strDisasterFullPath.IndexOf((char)0x06, nIndex1 + 1);
            if (nIndex2 < 0) return null;

            BarLevelTree tree = GetDockScenario().GetBarLevelTree();

            TreeNode node = tree.FindNode(strDisasterFullPath.Substring(0, nIndex1));
            if (node == null) return null;

            node = tree.FindNode(strDisasterFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1), node.Nodes);
            if (node == null) return null;

            return tree.FindNode(strDisasterFullPath.Substring(nIndex2 + 1), node.Nodes);
        }

        // 실행 대기중인 Section을 찾아 Focus를 준다.
        private void FocusSection(WorkFlow workFlow)
        {
            ArrayList arrPanels = GetPanels();
            if (arrPanels == null)
                return;

            foreach (Sections.PanelSectionEx panel in arrPanels)
            {
                foreach (Sections.Section section in panel.Sections)
                {
                    Sections.SectionState state = workFlow.FindState(section);
                    if (state == null)
                        continue;

                    if (state.State == State.INPUT)
                    {
                        panel.FocusSection(section);
                        return;
                    }
                }
            }
        }

        // BeginSection을 찾아 Focus를 준다.
        private void FocusSection()
        {
            ArrayList arrPanels = GetPanels();
            if (arrPanels == null)
                return;

            foreach (Sections.PanelSectionEx panel in arrPanels)
            {
                foreach (Sections.Section section in panel.Sections)
                {
                    if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
                    {
                        Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

                        if (data.IsBegin)
                        {
                            panel.FocusSection(section);
                            return;
                        }
                    }
                }
            }
        }

        public void toolstripSetting(string strDisasterFullPath)
        {
            if (tabControl.SelectedTab == null)
                return;

            Sections.SectionTabPage currentTabPage = (Sections.SectionTabPage)tabControl.SelectedTab;
            WorkFlow workFlow = WorkFlowManager.Instance.Get(currentTabPage.ActionStepID, !currentTabPage.VirtualMode);

            if (workFlow != null)
            {
                // 이미 실행중인 SOP
                FocusSection(workFlow);
            }
            else
            {
                // 아직 실행되지 않은 SOP
                FocusSection();
            }            
        }

        private void CheckSOPButton(string strDisasterFullPath)
        {
            foreach (RibbonButton btn in m_arrSOPButtons)
            {
                if (btn.Tag == null)
                    continue;

                string strPath = (string)btn.Tag;

                if (strPath == strDisasterFullPath)
                    btn.IsChecked = true;
                else
                    btn.IsChecked = false;
            }
        }

        private void CheckSOPButton(RibbonButton btnChecked)
        {
            foreach (RibbonButton btn in m_arrSOPButtons)
            {
                if (btn == btnChecked)
                    btn.IsChecked = true;
                else
                    btn.IsChecked = false;
            }
        }
    }
}
