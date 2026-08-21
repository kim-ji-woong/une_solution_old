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
using Sections;
using DBUtility;
using UnE.SOP;
using UnE.SOP.History;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Log;
using UnE.SOP.Tree;

namespace SOPMonitoringSystem
{
    public partial class PageBackstageSOP : Form, IRibbonButtonOwner, Sections.ISectionListener, ITabPageSpecialWorker
    {
        public class SectionTabControlEx : SectionTabControl
        {
            protected override void WndProc(ref Message m)
            {
                // Hide tabs by trapping the TCM_ADJUSTRECT message
                if (m.Msg == 0x1328 && !DesignMode)
                {
                    // 탭 헤더를 안보이게 한다.
                    m.Result = (IntPtr)1;
                }
                else
                    base.WndProc(ref m);
            }
        }

        public class QuickSOPButton
        {
            private string m_textNormal = string.Empty;
            private string m_textEmergency = string.Empty;

            private string m_textActionStepNormal = string.Empty;
            private string m_textActionStepEmergency = string.Empty;

            private bool m_bEnable = false;
            private Button m_btnSOP = null;
            private RibbonButtonQuick m_rbtnSOP = null;
            private PictureBox m_pic = null;


            public string SOPNormal
            {
                get { return m_textNormal; }
                set
                { 
                    m_textNormal = value;

                    InitButtonStatus();

                }
            }

            public string SOPEmergency
            {
                get { return m_textEmergency; }
                set 
                { 
                    m_textEmergency = value;

                    InitButtonStatus();

                }
            }

            public string SOPActionStepNameNormal
            {
                get { return m_textActionStepNormal; }
                set { m_textActionStepNormal = value; }
            }

            public string SOPActionStepNameEmergency
            {
                get { return m_textActionStepEmergency; }
                set { m_textActionStepEmergency = value; }
            }

            public string SOPNormalPath
            {
                get
                {
                    return m_textNormal + (String.IsNullOrWhiteSpace(m_textActionStepNormal) ? "" : "/" + m_textActionStepNormal);
                }
            }

            public string SOPEmergencyPath
            {
                get
                {
                    return m_textEmergency + (String.IsNullOrWhiteSpace(m_textActionStepEmergency) ? "" : "/" + m_textActionStepEmergency);
                }
            }

            public Button SOPButton
            {
                get { return m_btnSOP; }
                set 
                {
                    m_btnSOP = value;

                    InitButtonStatus();
                }
            }

            public RibbonButtonQuick SOPRibbonButton
            {
                get { return m_rbtnSOP; }
                set
                {
                    m_rbtnSOP = value;

                    InitButtonStatus();
                }
            }
            public PictureBox SOPRibbonPicture
            {
                get { return m_pic; }
                set
                {
                    m_pic = value;

                    InitButtonStatus();
                }
            }

            public bool ButtonEnable
            {
                set
                {
                    m_bEnable = value;

                    InitButtonStatus();
                }
            }

            
            public QuickSOPButton()
            {
            }

            public QuickSOPButton(string strSOPNormal, string strSOPActionStepNameNormal, string strSOPEmergency, string strSOPActionStepNameEmergency)
            {
                m_textNormal = strSOPNormal;
                m_textEmergency = strSOPEmergency;

                m_textActionStepNormal = strSOPActionStepNameNormal;
                m_textActionStepEmergency = strSOPActionStepNameEmergency;
            }

            public QuickSOPButton Clone()
            {
                return new QuickSOPButton(m_textNormal, m_textActionStepNormal, m_textEmergency, m_textActionStepEmergency);
            }

            private void InitButtonStatus()
            {
                bool isDayLight = Popup.SOPLoader.IsDayLight(DateTime.Now);
                bool hasSOP = false;

                // 현재시간 기준 평일주간모드인지 여부에 따라 퀵버튼 상태가 바뀌도록 한다.
                if (isDayLight && String.IsNullOrWhiteSpace(m_textNormal) == false)
                    hasSOP = true;
                else if (!isDayLight && String.IsNullOrWhiteSpace(m_textEmergency) == false)
                    hasSOP = true;

                if (m_btnSOP != null)
                {
                    if (m_bEnable == false)
                    {
                        m_btnSOP.Enabled = false;
                    }
                    else
                    {
                        m_btnSOP.Enabled = hasSOP;
                        /*if (String.IsNullOrWhiteSpace(m_textNormal) == true &&
                            String.IsNullOrWhiteSpace(m_textEmergency) == true)
                            m_btnSOP.Enabled = false;
                        else
                            m_btnSOP.Enabled = true;*/
                    }
                }

                if (m_rbtnSOP != null)
                {
                    if (m_bEnable == false)
                    {
                        m_rbtnSOP.Enabled = false;
                    }
                    else
                    {
                        m_rbtnSOP.Enabled = hasSOP;
                        /*if (String.IsNullOrWhiteSpace(m_textNormal) == true &&
                            String.IsNullOrWhiteSpace(m_textEmergency) == true)
                            m_rbtnSOP.Enabled = false;
                        else
                            m_rbtnSOP.Enabled = true;*/
                    }
                }
            }

        }

        public class SpecialWork
        {
            public enum SpecialWorkType { NONE = 0, SAVE_USING_UserDefinedTeam };

            private object m_data = null;
            private SpecialWorkType m_workType = SpecialWorkType.NONE;

            public object Data
            {
                get { return m_data; }
                set { m_data = value; }
            }

            public SpecialWorkType WorkType
            {
                get { return m_workType; }
                set { m_workType = value; }
            }

            public SpecialWork()
            {
            }

            public SpecialWork(SpecialWorkType workType, object data)
            {
                m_workType = workType;
                m_data = data;
            }
        }

        public enum Player { SectionPanel = 0, SectionLog, ComponentContents, None };

        private Player m_currentOneTopPlayer = Player.None;

        private Dictionary<ComponentContents, ComponentContents> m_dicNeedRefreshContents = new Dictionary<ComponentContents, ComponentContents>();
        private bool m_noRefreshComponentContents = false;

        public Player CurrentOneTopPlayer
        {
            get { return m_currentOneTopPlayer; }
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }
        public void OnSelectedSectionList(ArrayList arSections)
        {

        }

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        // Quick Button의 SOP 링크
        private Dictionary<int, QuickSOPButton> m_dicQuickSOPs = new Dictionary<int, QuickSOPButton>();
        public Dictionary<int, QuickSOPButton> QuickSOPs { get { return m_dicQuickSOPs; } }

        private DockingBottomSOPLog m_dockSOPLog = null;
        private DockingRightPersonnel m_dockPersonnel = null;
        private DockingReceiveMessage m_dockMessage = null;

        // 현재 선택되어진 ComponentContents
        // Key : 양수일 경우 RealMode의 ActionStepID를 의미
        //       음수일 경우 VirtualMode의 ActionStepID를 의미
        private Dictionary<int, ComponentContents> m_dicSelectedComponentContents = new Dictionary<int, ComponentContents>();
        //private static Color SelectedComponentContentsColor = Color.Red;

        private static PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();
        public static PopupTranslucentForm TranslucentForm
        {
            get { return PageBackstageSOP.mTranslucentForm; }
        }

        //private static int m_nTranslucentCommandID = -1;

        //private SDMS.Form4CCTV m_frm4CCTV = null;

        private int m_nCurrentScenarioIndex = -1;

        private TabPageSOPTeamUserManager m_sopUsingTeamManager = new TabPageSOPTeamUserManager();

        public TabPageSOPTeamUserManager SOPTeamMemberManager
        {
            get { return m_sopUsingTeamManager; }
        }

        public void OnSelectMission(int nActionStepID, int nReal, int nComponentID, string strRowIndex)
        {
            if (WorkFlowManager.Instance.Get(nActionStepID, (nReal == 1 ? true : false)) != null)
            {
                WorkFlow work = WorkFlowManager.Instance.Get(nActionStepID, (nReal == 1 ? true : false));
                if (work.State == WorkFlowState.RUN)
                {
                    ComponentContents content = GetComponentContents(nActionStepID, (nReal == 1 ? true : false), nComponentID);
                    if (content == null)
                    {
                        m_bSelectedCurrentMission = false;
                        return;
                    }
                    else
                    {
                        m_bSelectedCurrentMission = true;
                        timerSelectMission.Stop();
                    }

                    content.SelectRow(strRowIndex);
                    FormSOP.Instance.GetPageHome().ClearSelectComponentContentsExclude(content);
                }
            }
            else
            {
                timerSelectMission.Stop();
            }
        }

        private bool m_bSelectedCurrentMission = true;

        private int m_nActionStepID = -1;
        private int m_nReal = -1;
        private int m_nComponentID = -1;
        private string m_strRowIndex = null;
        private bool m_isCallingSelf = false, m_systemCall = false; 
        public void OnCurrentSelectedMission(int nActionStepID, int nReal, int nComponentID, string strRowIndex)
        {
            m_nActionStepID = nActionStepID;
            m_nReal = nReal;
            m_nComponentID = nComponentID;
            m_strRowIndex = strRowIndex;

            OnBeginMissionSelection(false);
        }

        private void OnBeginMissionSelection(bool isCallingSelf)
        {
            m_isCallingSelf = isCallingSelf;

            m_bSelectedCurrentMission = false;
            timerSelectMission.Start();
        }

        public void OnApplyControlUserToMissionStatus(SectionTabPage page)
        {
            if (page != null)
            {
                SetMissionStatus(page.ActionStepID, !page.VirtualMode, m_currentSection);
                OnBeginMissionSelection(true);
            }
        }

        // ActionStep별 ComponentContents List
        // Key : 상위 4바이트(1이면 실제 모드, 0이면 훈련 모드), 하위 4바이트(ActionStep ID)
        private Dictionary<long, ArrayList> m_dicComponentContents = new Dictionary<long, ArrayList>();

        public ArrayList GetComponentContentsList(int nActionStepID, bool isRealMode)
        {
            // 로딩시 훈련모드이어도 실행시 실제 모드 일 수 있음
            // 한개의 ActionStep은 한개만 실행되므로 구분은 무의미함
            // 20150-09-16 skkim

            long nHi = isRealMode ? 1 : 0;
            long nLow = nActionStepID;
            long nKey = nActionStepID;// (nHi << 32) | nLow;

            if (m_dicComponentContents.ContainsKey(nKey))
                return m_dicComponentContents[nKey];

            return null;
        }

        public ComponentContents GetComponentContents(Section section)
        {
            if (section == null)
                return null;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

            if (panel == null)
                return null;

            SectionTabPage page = (SectionTabPage)panel.Parent;

            ArrayList arrContents = GetComponentContentsList(page.ActionStepID, !page.VirtualMode);
            if (arrContents == null)
                return null;

            foreach (ComponentContents contents in arrContents)
            {
                if (contents.Section == section)
                    return contents;
            }

            return null;
        }

        public ComponentContents GetComponentContents(int nActionStepID, bool isRealMode, int nComponentID)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
                return null;

            foreach (ComponentContents contents in arrContents)
            {
                if (contents.ComponentID== nComponentID)
                    return contents;

                //if (contents.ComponentHistoryID == nComponentHistoryID)
                //    return contents;
            }

            return null;
        }


        public void ClearSelectComponentContentsExclude(ComponentContents contents)
        {
            if (contents == null)
                return;

            // changed by mwkim 2015-10-07 컴포넌트의 아아디 값이 부여되지 않아도 해당 컴포넌트의 그리드를 선택하였으면 타 컴포넌트 그리드의 선택을 해재하도록 함.
            //if (contents.ComponentID == -1)
            //    return;
            
            foreach (KeyValuePair<long, ArrayList> pair in m_dicComponentContents)
            {
                ArrayList arrList = pair.Value;
                arrList.Contains(contents);
                {
                    foreach (ComponentContents c in arrList)
                    {
                        if (c != contents)
                        {
                            c.gridView.ClearSelection();
                        }
                    }
                }
            }
            return;
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

        public void AddComponentContents(SectionTabPage tabPage, int nActionStepID, bool isRealMode, ComponentContents contents)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
            {

                // 로딩시 훈련모드이어도 실행시 실제 모드 일 수 있음
                // 한개의 ActionStep은 한개만 실행되므로 구분은 무의미함
                // 20150-09-16 skkim

                long nHi = isRealMode ? 1 : 0;
                long nLow = nActionStepID;
                long nKey = nActionStepID;// (nHi << 32) | nLow;

                arrContents = new ArrayList();
                m_dicComponentContents[nKey] = arrContents;
            }

            if (arrContents.Count == 0)
            {
                arrContents.Add(contents);

                contents.EnableGrid(FormSOP.Instance.HasControl);
                tabPage.PanelComponentContents.Controls.Add(contents);
                tabPage.PanelComponentContents.Controls.SetChildIndex(contents, 0);
                contents.SendToBack();

                if (tabPage.PanelComponentContents.Controls.Count == 1)
                {
                    toolstripSetting("");
                }
            }
            else
            {
                InsertComponentContents(tabPage, nActionStepID, isRealMode, contents);
                return;
            }
        }

        public void InsertComponentContents(SectionTabPage tabPage, int nActionStepID, bool isRealMode, ComponentContents contents)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            if (arrContents == null)
            {

                // 로딩시 훈련모드이어도 실행시 실제 모드 일 수 있음
                // 한개의 ActionStep은 한개만 실행되므로 구분은 무의미함
                // 20150-09-16 skkim

                long nHi = isRealMode ? 1 : 0;
                long nLow = nActionStepID;
                long nKey = nActionStepID;// (nHi << 32) | nLow;

                arrContents = new ArrayList();
                m_dicComponentContents[nKey] = arrContents;
            }
            int nCount = 0;
            foreach (ComponentContents comp in arrContents)
            {
                if (comp.ExecTime > contents.ExecTime)
                {
                    break;
                }
                nCount++;
            }
            arrContents.Add(contents);

            contents.EnableGrid(FormSOP.Instance.HasControl);
            tabPage.PanelComponentContents.Controls.Add(contents);
            tabPage.PanelComponentContents.Controls.SetChildIndex(contents, nCount);

            if (tabPage.PanelComponentContents.Controls.Count == 1)
            {
                toolstripSetting("");
            }
        }

        public void ClearComponentContents(SectionTabPage tabPage, int nActionStepID, bool isRealMode)
        {

            // 로딩시 훈련모드이어도 실행시 실제 모드 일 수 있음
            // 한개의 ActionStep은 한개만 실행되므로 구분은 무의미함
            // 20150-09-16 skkim

            long nHi = isRealMode ? 1 : 0;
            long nLow = nActionStepID;
            long nKey = nActionStepID;// (nHi << 32) | nLow;

            m_dicComponentContents.Remove(nKey);
            //splitContainerMain.Panel2.Controls.Clear();
            tabPage.PanelComponentContents.Controls.Clear();

            //HideComponentContents();
        }

        public SOPMonitoringSystem.DockingReceiveMessage DockingMessage
        {
            get { return m_dockMessage; }
            set { m_dockMessage = value; }
        }

        private PointF[] m_arrDragDropOrigin = null;
        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;
        private Sections.PanelSectionEx m_currentPanel = null;

        private Sections.Section m_currentSection = null;
        public Sections.Section CurrentSection
        {
            set { m_currentSection = value; }
        }

        private Form[] m_arrDocking = new Form[8];

        private ArrayList m_arrPanel = new ArrayList();
        public System.Collections.ArrayList PanelArray
        {
            get { return m_arrPanel; }
            set { m_arrPanel = value; }
        }
        //private ArrayList m_arrTabPage = new ArrayList();

        private Color m_colorPanel1 = System.Drawing.Color.FromArgb(255, 192, 255);
        private Color m_colorPanel2 = System.Drawing.Color.FromArgb(192, 192, 255);

        //private int m_nTabPage = 1;
        private int m_nActiopnStepID = 0;
        private ArrayList m_arrTeams = new ArrayList();

        ArrayList m_arrSOPButtons = new ArrayList();

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

        public PageBackstageSOP()
        {
            InitializeComponent();
             
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
             
            FormSOP.SetDoubleBuffer(this.panelTop, true);
            FormSOP.SetDoubleBuffer(splitContainerMain.Panel2, true);
            FormSOP.SetDoubleBuffer(splitContainerMain.Panel1, true);

            

            SetSectionColor();

            this.MouseWheel += new MouseEventHandler(PageBackstageHome_MouseWheel);

            CreatePane();
        }

        private void SetSectionColor()
        {
            EditBox.SetColor(true, Color.White);
            EditBox.SetColor(false, Color.FromArgb(60, 56, 71));

            Arrow.NormalPen.Color = Color.Gray;
            Arrow.TempLinePen.Color = Color.LightGray;
            Arrow.TriangleBrush.Color = Color.Gray;

            Arrow.TextFont = new Font("맑은 고딕", 12, FontStyle.Regular);
            Arrow.TextBrush.Color = Color.Black;
            Sections.Shape.UseImage = false;


            SizeManager.MinSize = new Size(100, 40);
            SectionDecision.DefaultSize = new Size(200, 85);

            //PanelSectionEx.EditableArrowText = false;
            PathNotifier.PathColor = Color.Purple;
        }

        private void PageBackstageHome_Load(object sender, EventArgs e)
        {
            //GetDockSOPLog().SetPane(m_paneSOPLog);
            m_dockSOPLog.TopLevel = false;
            m_dockMessage.TopLevel = false;

            tabPage2.Controls.Add(m_dockSOPLog);
            tabPage3.Controls.Add(m_dockMessage);
            //splitContainerVertical.Panel2.Controls.Add(m_dockSOPLog);
            m_dockSOPLog.Dock = DockStyle.Fill;
            m_dockSOPLog.Visible = true;

            m_dockMessage.Dock = DockStyle.Fill;
            m_dockMessage.Visible = true;

            // Key : ButtonID
            // Value : SOP Full Path
            m_dicQuickSOPs = LoadBookMark();
            AddQuickButtons(m_dicQuickSOPs);
            InitButtons(m_dicQuickSOPs);
            NetworkManager.Instance.ReleaseConnection();

            InitScenarioPanel();

            labelScenarioName.Text = "";

            InitSplitSize();
            PageBackstageHome_Resize(null, null);

            if (tabControl.GetValidTabPageCount() > 0)
            {
                panel.Visible = true;
                tabControl.Visible = true;
            }

            mTranslucentForm.Location = PointToScreen(new Point(0, 0));
            mTranslucentForm.Size = this.Size;
        }

        private void InitSplitSize()
        {
            if (m_nInitSplitterDistance > 0)
            {
                splitContainerMain.SplitterDistance = m_nInitSplitterDistance;
            }

            //this.splitContainerMain.Size = this.Size;
            this.splitContainerMain.Size = new Size(this.Size.Width, this.Size.Height - splitContainerMain.Location.Y);

            int nTotalWidth = splitContainerMain.Panel1.ClientSize.Width + splitContainerMain.Panel2.ClientSize.Width;
            splitContainerMain.Panel1.ClientSize = new Size(splitContainerMain.SplitterDistance, splitContainerMain.Panel1.ClientSize.Height);
            splitContainerMain.Panel2.ClientSize = new Size(nTotalWidth - splitContainerMain.Panel1.ClientSize.Width, splitContainerMain.Panel2.ClientSize.Height);
        }

        private void InitScenarioPanel()
        {
            Size sizeImage = global::SOPMonitoringSystem.Properties.Resources.Scenario_Background.Size;
            //panelScenarioName.Size = new Size(sizeImage.Width, panelScenarioName.Height);
            labelScenarioName.MaximumSize = new Size(sizeImage.Width - labelScenarioName.Location.X * 2, 0);
        }

        // Key : ButtonID
        // Value : SOP Full Path        
        private void AddQuickButtons(Dictionary<int, QuickSOPButton> dicQuickSOPs)
        { 
            //if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
            {
                Bitmap bitmap1 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Quake, new Size(160, 170));
                Bitmap bitmap2 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Quake_Click, new Size(160, 170));
                Bitmap bitmap3 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Quake_Over, new Size(210, 220));

                AddQuickButton(bitmap1, bitmap2, bitmap3, "지진", ID.ID_SOP_EARTHQUAKE, dicQuickSOPs, BtnImgStartPosition.RIGHT);

                Bitmap bitmap4 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Security, new Size(160, 170));
                Bitmap bitmap5 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Security_Click, new Size(160, 170));
                Bitmap bitmap6 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Security_Over, new Size(210, 220));

                AddQuickButton(bitmap4, bitmap5, bitmap6, "방범", ID.ID_SOP_SECURITY, dicQuickSOPs, BtnImgStartPosition.MIDDLE);

                Bitmap bitmap7 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Fire, new Size(160, 170));
                Bitmap bitmap8 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Fire_Click, new Size(160, 170));
                Bitmap bitmap9 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Fire_Over, new Size(210, 220));

                AddQuickButton(bitmap7, bitmap8, bitmap9, "화재", ID.ID_SOP_FIRE, dicQuickSOPs, BtnImgStartPosition.LEFT);

                Bitmap bitmap10 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_FineDust, new Size(160, 170));
                Bitmap bitmap11 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_FineDust_Click, new Size(160, 170));
                Bitmap bitmap12 = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_FineDust_Over, new Size(210, 220));

                AddQuickButton(bitmap10, bitmap11, bitmap12, "미세먼지", ID.ID_SOP_HEAVY_SNOW, dicQuickSOPs, BtnImgStartPosition.CENTER);
            }
            /*else
            {
                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.화재_03,
                    global::SOPMonitoringSystem.Properties.Resources.화재_normal,
                    global::SOPMonitoringSystem.Properties.Resources.화재_over,
                    "화재", ID.ID_SOP_FIRE, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.지진_03,
                    global::SOPMonitoringSystem.Properties.Resources.지진_normal,
                    global::SOPMonitoringSystem.Properties.Resources.지진_over,
                    "지진", ID.ID_SOP_EARTHQUAKE, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.태풍_03,
                    global::SOPMonitoringSystem.Properties.Resources.태풍_normal,
                    global::SOPMonitoringSystem.Properties.Resources.태풍_over,
                    "태풍", ID.ID_SOP_TYPHOON, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.침수_03,
                    global::SOPMonitoringSystem.Properties.Resources.침수_normal,
                    global::SOPMonitoringSystem.Properties.Resources.침수_over,
                    "침수", ID.ID_SOP_SUBMERGENCE, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.일반재해_03,
                    global::SOPMonitoringSystem.Properties.Resources.일반재해_normal,
                    global::SOPMonitoringSystem.Properties.Resources.일반재해_over,
                    "방범", ID.ID_SOP_GENERAL_DISASTER, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.폭설_03,
                    global::SOPMonitoringSystem.Properties.Resources.폭설_normal,
                    global::SOPMonitoringSystem.Properties.Resources.폭설_over,
                    "폭설", ID.ID_SOP_HEAVY_SNOW, dicQuickSOPs);
                  
                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.테러_03,
                    global::SOPMonitoringSystem.Properties.Resources.테러_normal,
                    global::SOPMonitoringSystem.Properties.Resources.테러_over,
                    "테러", ID.ID_SOP_TERROR, dicQuickSOPs);

                AddQuickButton(global::SOPMonitoringSystem.Properties.Resources.오염_03,
                    global::SOPMonitoringSystem.Properties.Resources.오염_normal,
                    global::SOPMonitoringSystem.Properties.Resources.오염_over,
                    "오염", ID.ID_SOP_POLLUTION, dicQuickSOPs);
            }*/
        } 
        // Key : ButtonID
        // Value : SOP Full Path
        private void AddQuickButton(Image cImg, Image norBack, Image ovBack, string strButtonName, int nButtonID, Dictionary<int, QuickSOPButton> dicQuickSOPs, BtnImgStartPosition position = BtnImgStartPosition.NONE)
        {
            RibbonButtonQuick btn = null;
            //if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
            {
                int nButtonWidth = 250;
                int nButtonHeight = 250;

                btn = new RibbonButtonQuick(nButtonWidth, position);
                btn.BackColor = Color.Transparent;
                btn.NormalImage = cImg;
                //btn.BackgroundImage = norBack; 
                btn.ClickedImage = norBack;
                btn.MouseOverImage = ovBack;
                btn.MouseOverImage.Tag = "OVER";
                btn.CustomImageRect = new Rectangle(0, 0, nButtonWidth, nButtonHeight);
                btn.UseCustomImageRect = false;  
                btn.Name = strButtonName;
                btn.Owner = this.panelBackImage;  
                btn.BackColor = Color.Transparent; 
            }
            /*else
            {
                int nButtonWidth = 300;

                btn = new RibbonButtonQuick(nButtonWidth);
                btn.NormalImage = cImg;
                btn.BackgroundImage = norBack;                
                btn.MouseOverBkgndImage = ovBack;
                btn.UseCustomImageRect = false; 
                btn.Name = strButtonName;
                btn.Owner = this.panelBackImage;
                btn.Size = new Size(nButtonWidth, nButtonWidth);// + 20);
                btn.Text = strButtonName;
                btn.Font = new System.Drawing.Font("맑은 고딕", 20.0f);
                btn.UseVisualStyleBackColor = true; 
            }*/

            if (dicQuickSOPs.ContainsKey(nButtonID))
            {
                dicQuickSOPs[nButtonID].SOPRibbonButton = btn;
                btn.Tag = dicQuickSOPs[nButtonID];
            }
            else
            {
                btn.Enabled = false;
            }
            panelBackImage.AddQuickButton(btn);
        }

        // Key : ButtonID
        // Value : SOP Full Path
        private void InitButtons(Dictionary<int, QuickSOPButton> dicQuickSOPs)
        {
            Image imgMouseOverBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonGrayChecked_bkgnd;

            // PanelTop SOP Quick Buttons
            InitRibbonButton(btnFire, ID.ID_SOP_FIRE, global::SOPMonitoringSystem.Properties.Resources.Fire_Normal, global::SOPMonitoringSystem.Properties.Resources.Fire_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnEarthquake, ID.ID_SOP_EARTHQUAKE, global::SOPMonitoringSystem.Properties.Resources.Earthquake_Normal, global::SOPMonitoringSystem.Properties.Resources.Earthquake_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnTyphoon, ID.ID_SOP_TYPHOON, global::SOPMonitoringSystem.Properties.Resources.Typhoon_Normal, global::SOPMonitoringSystem.Properties.Resources.Typhoon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnSubmergence, ID.ID_SOP_SUBMERGENCE, global::SOPMonitoringSystem.Properties.Resources.Submergence_Normal, global::SOPMonitoringSystem.Properties.Resources.Submergence_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnSecurity, ID.ID_SOP_SECURITY, global::SOPMonitoringSystem.Properties.Resources.security_Normal, global::SOPMonitoringSystem.Properties.Resources.security_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnHeavySnow, ID.ID_SOP_HEAVY_SNOW, global::SOPMonitoringSystem.Properties.Resources.HeavySnow_Normal, global::SOPMonitoringSystem.Properties.Resources.HeavySnow_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnTerror, ID.ID_SOP_TERROR, global::SOPMonitoringSystem.Properties.Resources.Terror_Normal, global::SOPMonitoringSystem.Properties.Resources.Terror_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);
            InitRibbonButton(btnPollution, ID.ID_SOP_POLLUTION, global::SOPMonitoringSystem.Properties.Resources.Pollution_Normal, global::SOPMonitoringSystem.Properties.Resources.Pollution_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, dicQuickSOPs, m_arrSOPButtons);

            ArrangeRibbonButtons(m_arrSOPButtons);

            // 불러오기 버튼
            InitRibbonButton(btnOpenSOP, ID.ID_OPEN_SOP, null, null, null, imgMouseOverBkgnd, imgCheckedBkgnd, null, null, null);
            btnOpenSOP.Text = "시나리오 불러오기 ";
            btnOpenSOP.Size = new Size(141, 30);
            btnOpenSOP.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenSOP.TextAlign = ContentAlignment.MiddleRight;

            this.btnFire.Click += btnQuipButton_Click;
            this.btnEarthquake.Click += btnQuipButton_Click;
            this.btnTyphoon.Click += btnQuipButton_Click;
            this.btnSubmergence.Click += btnQuipButton_Click;
            this.btnSecurity.Click += btnQuipButton_Click;
            this.btnHeavySnow.Click += btnQuipButton_Click;
            this.btnTerror.Click += btnQuipButton_Click;
            this.btnPollution.Click += btnQuipButton_Click;

            this.btnOpenSOP.Click += btnOpenSOP_Click;

            //if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
            {
                btnTyphoon.Visible = btnHeavySnow.Visible = btnTerror.Visible = btnPollution.Visible = btnSubmergence.Visible = false;
                btnSecurity.Location = btnTyphoon.Location;
            }
        }

        private void btnOpenSOP_Click(object sender, EventArgs e)
        {
            OpenSOP();
        }

        private void btnQuipButton_Click(object sender, EventArgs e)
        {
            LoadQuickSOP((sender as Button));
        }

        private void ArrangeRibbonButtons(ArrayList arrButtons)
        {
            int nButtonCount = arrButtons.Count;
            if (nButtonCount < 2)
                return;

            //Size sizeButton = new Size(30, 30);
            Size sizeButton = new Size(64, 30);

            Button btnPrev = (Button)arrButtons[0];
            btnPrev.Size = sizeButton;// btnPrev.NormalImage.Size;

            btnPrev.Location = new Point(btnPrev.Location.X, btnPrev.Location.Y + 1);

            for (int i = 1; i < nButtonCount; i++)
            {
                Button btnNext = (Button)arrButtons[i];
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
        private void InitRibbonButton(Button btn, int nID, Image imgNormal, Image imgChecked, Image imgDisabled, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, Image imgDisabledBkgnd, Dictionary<int, QuickSOPButton> dicQuickSOPs, ArrayList arrButtons)
        {
            btn.TextAlign = ContentAlignment.MiddleCenter;

            SetButtonID(btn, nID, btn.Text);

            if (btn is RibbonButton)
            {
                RibbonButton rbtn = btn as RibbonButton;
                rbtn.NormalImage = imgNormal;
                rbtn.CheckedImage = imgChecked;
                rbtn.DisabledImage = imgDisabled;

                rbtn.MouseOverBkgndImage = imgMouseOverBkgnd;
                rbtn.CheckedBkgndImage = imgCheckedBkgnd;
                rbtn.DisabledBkgndImage = imgDisabledBkgnd;
                rbtn.Owner = this;

                rbtn.Text = "";
            }

            if (dicQuickSOPs != null)
            {
                if (!dicQuickSOPs.ContainsKey(nID))
                {
                    btn.Enabled = false;
                }
                else
                {
                    dicQuickSOPs[nID].SOPButton = btn;

                    btn.Tag = dicQuickSOPs[nID];
                    btn.ForeColor = System.Drawing.Color.Black;
                }
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

        private void LoadBookMarkButton(Dictionary<int, QuickSOPButton> dicQuickSOPs, int nID, WebDBManager dbMgr)
        {
            QuickSOPButton btn = LoadBookMark(nID, dbMgr);

            if (btn != null)
                dicQuickSOPs[nID] = btn;
        }

        // Key : ButtonID
        // Value : SOP Full Path
        Dictionary<int, QuickSOPButton> LoadBookMark()
        {
            Dictionary<int, QuickSOPButton> dicQuickSOPs = new Dictionary<int, QuickSOPButton>();
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_FIRE, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_POLLUTION, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_TYPHOON, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_TERROR, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_HEAVY_SNOW, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_GENERAL_DISASTER, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_SUBMERGENCE, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_EARTHQUAKE, dbMgr);
            LoadBookMarkButton(dicQuickSOPs, ID.ID_SOP_SECURITY, dbMgr);
            /*dicQuickSOPs[ID.ID_SOP_FIRE] = LoadBookMark(ID.ID_SOP_FIRE, dbMgr);
            dicQuickSOPs[ID.ID_SOP_POLLUTION] = LoadBookMark(ID.ID_SOP_POLLUTION, dbMgr);
            dicQuickSOPs[ID.ID_SOP_TYPHOON] = LoadBookMark(ID.ID_SOP_TYPHOON, dbMgr);
            dicQuickSOPs[ID.ID_SOP_TERROR] = LoadBookMark(ID.ID_SOP_TERROR, dbMgr);
            dicQuickSOPs[ID.ID_SOP_HEAVY_SNOW] = LoadBookMark(ID.ID_SOP_HEAVY_SNOW, dbMgr);
            dicQuickSOPs[ID.ID_SOP_GENERAL_DISASTER] = LoadBookMark(ID.ID_SOP_GENERAL_DISASTER, dbMgr);
            dicQuickSOPs[ID.ID_SOP_SUBMERGENCE] = LoadBookMark(ID.ID_SOP_SUBMERGENCE, dbMgr);
            dicQuickSOPs[ID.ID_SOP_EARTHQUAKE] = LoadBookMark(ID.ID_SOP_EARTHQUAKE, dbMgr);*/

            //string strSOPFire = LoadBookMark("화재", "SOP BookMark", dbMgr);
            //string strSOPPollution = LoadBookMark("오염", "SOP BookMark", dbMgr);
            //string strSOPTyphoon = LoadBookMark("태풍", "SOP BookMark", dbMgr);
            //string strSOPTerror = LoadBookMark("테러", "SOP BookMark", dbMgr);
            //string strSOPHeavySnow = LoadBookMark("폭설", "SOP BookMark", dbMgr);
            //string strSOPGeneral = LoadBookMark("일반재해", "SOP BookMark", dbMgr);
            //string strSOPSubmergence = LoadBookMark("침수", "SOP BookMark", dbMgr);
            //string strSOPEarthquake = LoadBookMark("지진", "SOP BookMark", dbMgr);

            //if (strSOPFire.Length > 0)
            //{
            //    if (strSOPFire.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_FIRE] = new QuickSOPButton(strSOPFire.Replace(strNormal, ""));
            //    else if (strSOPFire.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_FIRE] = new QuickSOPButton(strSOPFire.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_FIRE] = new QuickSOPButton();
            //}

            //if (strSOPPollution.Length > 0)
            //{
            //    if (strSOPPollution.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_POLLUTION] = new QuickSOPButton(strSOPPollution.Replace(strNormal, ""));
            //    else if (strSOPPollution.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_POLLUTION] = new QuickSOPButton(strSOPPollution.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_POLLUTION] = new QuickSOPButton();
            //}

            //if (strSOPTyphoon.Length > 0)
            //{
            //    if (strSOPTyphoon.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_TYPHOON] = new QuickSOPButton(strSOPTyphoon.Replace(strNormal, ""));
            //    else if (strSOPTyphoon.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_TYPHOON] = new QuickSOPButton(strSOPTyphoon.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_TYPHOON] = new QuickSOPButton();
            //}

            //if (strSOPTerror.Length > 0)
            //{
            //    if (strSOPTerror.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_TERROR] = new QuickSOPButton(strSOPTerror.Replace(strNormal, ""));
            //    else if (strSOPTerror.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_TERROR] = new QuickSOPButton(strSOPTerror.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_TERROR] = new QuickSOPButton();
            //}

            //if (strSOPHeavySnow.Length > 0)
            //{
            //    if (strSOPHeavySnow.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_HEAVY_SNOW] = new QuickSOPButton(strSOPHeavySnow.Replace(strNormal, ""));
            //    else if (strSOPHeavySnow.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_HEAVY_SNOW] = new QuickSOPButton(strSOPHeavySnow.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_HEAVY_SNOW] = new QuickSOPButton();
            //}

            //if (strSOPGeneral.Length > 0)
            //{
            //    if (strSOPGeneral.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_GENERAL_DISASTER] = new QuickSOPButton(strSOPGeneral.Replace(strNormal, ""));
            //    else if (strSOPGeneral.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_GENERAL_DISASTER] = new QuickSOPButton(strSOPGeneral.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_GENERAL_DISASTER] = new QuickSOPButton();
            //}

            //if (strSOPSubmergence.Length > 0)
            //{
            //    if (strSOPSubmergence.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_SUBMERGENCE] = new QuickSOPButton(strSOPSubmergence.Replace(strNormal, ""));
            //    else if (strSOPSubmergence.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_SUBMERGENCE] = new QuickSOPButton(strSOPSubmergence.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_SUBMERGENCE] = new QuickSOPButton();
            //}

            //if (strSOPEarthquake.Length > 0)
            //{
            //    if (strSOPEarthquake.IndexOf(strNormal) == 0)
            //        dicQuickSOPs[ID.ID_SOP_EARTHQUAKE] = new QuickSOPButton(strSOPEarthquake.Replace(strNormal, ""));
            //    else if (strSOPEarthquake.IndexOf(strEmergency) == 0)
            //        dicQuickSOPs[ID.ID_SOP_EARTHQUAKE] = new QuickSOPButton(strSOPEarthquake.Replace(strEmergency, ""), false);
            //}
            //else
            //{
            //    dicQuickSOPs[ID.ID_SOP_EARTHQUAKE] = new QuickSOPButton();
            //}

            return dicQuickSOPs;
        }

        private QuickSOPButton LoadBookMark(int nButtonID, WebDBManager dbMgr)
        {
            QuickSOPButton data = new QuickSOPButton();

            string strSQL = String.Format("SELECT IsNormal, DisasterName, ActionStepName FROM OptionQuickButton WHERE ButtonID = {0} AND SiteID = {1}", nButtonID, UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return data;

            if (arrResult.Count == 0)
                return data;

            for (int nIndex = 0; nIndex < arrResult.Count; nIndex += 3)
            {
                bool isNormal = Convert.ToBoolean(WebDBManager.GetIntField(arrResult[nIndex].ToString(), 0));
                string strDisaterName = WebDBManager.GetStringField(arrResult[nIndex + 1]).Replace("null", "");
                string strActionStepName = WebDBManager.GetStringField(arrResult[nIndex + 2]).Replace("null", "");

                if (strDisaterName.Length != 0)
                {
                    if (isNormal)
                    {
                        data.SOPNormal = strDisaterName;
                        data.SOPActionStepNameNormal = strActionStepName;
                    }
                    else
                    {
                        data.SOPEmergency = strDisaterName;
                        data.SOPActionStepNameEmergency = strActionStepName;
                    }
                }

            }

            return data;
            //return dbMgr.LoadIni(strTagName, strSectionName);
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
            m_arrDocking[6] = new DockingBottomSOPLog();
            m_dockSOPLog = (DockingBottomSOPLog)m_arrDocking[6];

            m_arrDocking[5] = new DockingRightPersonnel();
            m_dockPersonnel = (DockingRightPersonnel)m_arrDocking[5];

            m_arrDocking[7] = new DockingReceiveMessage();
            m_dockMessage = (DockingReceiveMessage)m_arrDocking[7];
        }


        private void PageBackstageHome_Resize(object sender, EventArgs e)
        {
            panelBackImage.Dock = DockStyle.Fill;

            // panelTop
            //panelTop.Size = new Size(splitContainerMain.Panel1.Width, panelTop.Height);
            //Point ptPanelScenarioName = new Point(panelTop.Size.Width - panelScenarioName.Size.Width - 5, panelScenarioName.Location.Y);
            //Point ptOpenSOP = new Point(ptPanelScenarioName.X - btnOpenSOP.Size.Width, btnOpenSOP.Location.Y);
            Point ptOpenSOP = btnOpenSOP.Location;
            Point ptPanelScenarioName = new Point(ptOpenSOP.X + btnOpenSOP.Size.Width + 10 + lblSOPHeader.Size.Width + 26, panelScenarioName.Location.Y);

            btnOpenSOP.Location = ptOpenSOP;
            panelScenarioName.Location = ptPanelScenarioName;

            /*int nSOPButtonCount = m_arrSOPButtons.Count;

            if (nSOPButtonCount == 0)
            {
                btnOpenSOP.Location = ptOpenSOP;
                panelScenarioName.Location = ptPanelScenarioName;
                return;
            }

            Button btnLast = (Button)m_arrSOPButtons[nSOPButtonCount - 1];
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
            }*/
            /////////////////////////////////////////////////////////////////////

            // splitContainerVertical
            //splitContainerVertical.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            splitContainerVertical.Size = new Size(splitContainerMain.Panel1.ClientSize.Width, splitContainerMain.Panel1.ClientSize.Height - splitContainerVertical.Location.Y);
            /////////////////////////////////////////////////////////////////////

            if (tabControl.Visible == true)
            {
                tabControl.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                tabControl.Size = new Size(splitContainerMain.Panel1.ClientSize.Width, splitContainerMain.Panel1.ClientSize.Height - tabControl.Location.Y);
            }
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
                    case ID.ID_SOP_SECURITY:
                        if (btn.IsChecked)
                            return;

                        LoadQuickSOP(btn);
                        break;
                }
            }
        }

        public void OpenSOP()
        {
            // SOP 선택창을 열기전에 혹시 버전이 바뀌지 않았는지 검사한다.
            if (!CheckSOPVersion())
                return;

            PopupSOPList sopList = new PopupSOPList();
            sopList.ShowDialog(this);
        }

        // SOP 버전이 바뀌지 않았는지 검사한다.
        public bool CheckSOPVersion()
        {
            DBUtility.VariousData<DateTime> dtSOP = null, dtMember = null;
            FormSOP.Instance.ReadLastAccessedTime(ref dtSOP, ref dtMember);

            if (FormSOP.Instance.LastAccessedSOPTime == null)
            {
                if (dtSOP == null)
                    return true;
                else
                    FormSOP.Instance.LastAccessedSOPTime = dtSOP;
            }
            else
            {
                if (dtSOP == null)
                    return true;
                else if (FormSOP.Instance.LastAccessedSOPTime.Data < dtSOP.Data)
                    FormSOP.Instance.LastAccessedSOPTime = dtSOP;
                else
                    return true;
            }

            if (!FormSOP.Instance.SOPManager.Load(FormSOP.Instance.IsRegular, FormSOP.Instance.IsNormal, true))
                return false;

            return FormSOP.Instance.LoadSOP();
        }

        public string GetQuickSOPFullPath(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
            {
                Button btn = m_dicIDButtons[nID];
                return ((QuickSOPButton)btn.Tag).SOPNormal;
            }

            return null;
        }

        public void LoadQuickSOP(Button btnSOP)
        {
            if (FormSOP.Instance.HasControl == false)
                return;

            QuickSOPButton btnOption = (QuickSOPButton)btnSOP.Tag;
            if (btnOpenSOP == null)
                return;

            bool isDayLight =false;
            string strDisasterFullPath = string.Empty;

            if (Popup.SOPLoader.IsNormal(DateTime.Now))
            {
                if (String.IsNullOrWhiteSpace(btnOption.SOPNormalPath) &&
                    String.IsNullOrWhiteSpace(btnOption.SOPEmergencyPath) == false)
                {
                    strDisasterFullPath = btnOption.SOPEmergencyPath;
                    isDayLight = false;
                }
                else
                {
                    strDisasterFullPath = btnOption.SOPNormalPath;
                    isDayLight = true;
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(btnOption.SOPNormalPath) == false &&
                    String.IsNullOrWhiteSpace(btnOption.SOPEmergencyPath))
                {
                    strDisasterFullPath = btnOption.SOPNormalPath;
                    isDayLight = true;
                }
                else
                {
                    strDisasterFullPath = btnOption.SOPEmergencyPath;
                    isDayLight = false;
                }
            }

            // SOP를 열기전에 혹시 버전이 바뀌지 않았는지 검사한다.
            if (!CheckSOPVersion())
                return;

            FormSOP.Instance.ChangeMode(FormSOP.Instance.IsReal, true, isDayLight);

            if (String.IsNullOrWhiteSpace(strDisasterFullPath))
                return;

            ReplaceDisasterPath(ref strDisasterFullPath);
            TreeNode node = FindDisasterNode(strDisasterFullPath, isDayLight);

            if (node != null)
            {
                CheckSOPButton(btnSOP);
                BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                if (tree != null)
                {

                   
                    tree.SelectNode(node);
                }
            }
        }

        private int m_nInitSplitterDistance = -1;

        public void HideComponentContents()
        {
            if (m_nInitSplitterDistance < 0)
                m_nInitSplitterDistance = this.splitContainerMain.SplitterDistance;

            this.splitContainerMain.SplitterDistance = this.splitContainerMain.Width;
        }

        public void ShowComponentContents()
        {
            //if (m_nInitSplitterDistance == -1)
            {
                m_nInitSplitterDistance = FormSOP.Instance.Size.Width / 2 - 2;

            }
            if (m_nInitSplitterDistance > 0)
                this.splitContainerMain.SplitterDistance = m_nInitSplitterDistance;
        }

        private void ReplaceDisasterPath(ref string strPath)
        {
            int nIndex = strPath.IndexOf('/');

            if (nIndex < 0)
                return;

            int nIndex2 = strPath.IndexOf('/', nIndex + 1);

            if (nIndex2 < 0)
                return;

            int nIndex3 = strPath.IndexOf('/', nIndex2 + 1);

            if (nIndex3 < 0)
            {
                strPath = strPath.Substring(0, nIndex) + ((char)0x06) + strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + ((char)0x06) + strPath.Substring(nIndex2 + 1);
            }
            else
            {
                strPath = strPath.Substring(0, nIndex) + ((char)0x06) + strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + ((char)0x06) + strPath.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1) + ((char)0x06) + strPath.Substring(nIndex3 + 1);
            }

        }

        //////////////////////////////////////////////////////////////////////////

        public DockingBottomSOPLog GetDockSOPLog()
        {
            return m_dockSOPLog;
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

        /*private ArrayList GetUsingUserDefineTeamList(int nActionStepID)
        {
            ArrayList arList = new ArrayList();

            ArrayList arIDs = FormSOP.Instance.SOPManager.GetUsingUserDefineTeams(nActionStepID);
            foreach(int nTeamID in arIDs)
            {
                Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeamMember(nTeamID);
                if( team != null)
                {
                    Data_ExternalTeam newTeam = new Data_ExternalTeam(team.ID, team.TeamName, team.PhoneNumber, team.FaxNumber);
                    arList.Add(newTeam);
                }
            }
            return arList;
        }*/

        public TabPage AddTabPage(Data_ActionStep data, bool bReal)
        {
            int ActionStepID = data.ID;
            SectionTabPage tabPage = (SectionTabPage)TabPageManager.Instance.GetPage(ActionStepID, bReal);
            if (tabPage == null)
            {
                tabPage = new SectionTabPage(tabControl);
                tabPage.Location = new System.Drawing.Point(4, 22);
                tabPage.Name = string.Format("TabPage_{0}", tabPage.Handle);
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = tabControl.Size;
                tabPage.Text = data.StepName;
                tabPage.ToolTipText = data.StepName;
                tabPage.ActionStepID = data.ID;
                tabPage.CreateNew = true;               
                //tabPage.AddExternalTeams(GetUsingUserDefineTeamList(data.ID));
                m_sopUsingTeamManager.SetPageSOPTeams(tabPage, data.ID);

                ((Control)tabPage).Enabled = true;

                tabPage.PanelComponentContents.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.ComponentContentsPanel_ControlAdded);
                tabPage.PanelComponentContents.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.ComponentContentsPanel_ControlRemoved);
                tabPage.PanelComponentContents.MouseDoubleClick += new MouseEventHandler(OnComponentContentsDoubleClick);

                PreviewComponentContainer container = new PreviewComponentContainer(tabPage);
                tabPage.PanelPreviewComponentContents = container;

            }
            else
            {
                tabPage.CreateNew = false;
            }

            tabPage.Tag = data;
            tabPage.VirtualMode = !bReal;
            tabPage.UseWaterMark = ProxySOP.Instance.UseWaterMark;

            ++m_nActiopnStepID;
            
            tabControl.AddTabPage(tabPage);
            tabControl.SelectedTab = tabPage;

            //m_arrTabPage.Add(tabPage);

            tabPage.ReSizePanel();

            return tabPage;
        }

        //////////////////////////////////////////////////////////////////////////
        // DB Loading을 통한 Tab Page 생성
        public TabPage AddTabPage(Data_ActionStep data)
        {
            bool bReal = FormSOP.Instance.IsReal;
            if (tabControl.Visible == false)
            {
                this.tabControl.Visible = true;
                this.panel.Visible = true;
            }
            return AddTabPage(data, bReal);
        }

        public FormLegend frmLegend;
        public void changecolor(int num, Color color)
        {
            if (frmLegend != null)
            {
                frmLegend.ChangeBackColor(num, color);
            }
        }
        public void changeLocation(int height)
        {
            try
            {
                if (frmLegend != null)
                    frmLegend.Location = new Point(0, height - frmLegend.Height);
            }
            catch(Exception)
            {
            }
		}

        char szDeli = (char)0x06;
        public void SetScenarioName(int nActionStepID)
        {
            ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
            if (tree == null)
                return;

            TreeNode node = tree.FindActionStepNode(nActionStepID);
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

                ((SectionTabPage)tabPage).Controls.Add(panel);
                pt.X += sz.Width;

                m_arrPanel.Add(panel);
                arrPanels.Add(panel);

                if (i == 0)
                {
                    frmLegend = new FormLegend();
                    frmLegend.Location = new Point(0, 100);
                    frmLegend.Dock = DockStyle.None;
                    frmLegend.TopLevel = false;
                    frmLegend.Parent = this;
                    panel.Controls.Add(frmLegend);
                    panel.Legend = frmLegend;

                    if (FormSOP.Instance.ShowLegend == true)
                        frmLegend.Show();
                    else
                        frmLegend.Visible = false;

                    frmLegend.ChangeBackColor(0, Color.FromArgb(FormSOP.Instance.GetPageOption().getColor(0)));
                    frmLegend.ChangeBackColor(1, Color.FromArgb(FormSOP.Instance.GetPageOption().getColor(1)));
                    frmLegend.ChangeBackColor(2, Color.FromArgb(FormSOP.Instance.GetPageOption().getColor(2)));
                    frmLegend.ChangeBackColor(3, Color.FromArgb(FormSOP.Instance.GetPageOption().getColor(3)));
                    frmLegend.ChangeBackColor(4, Color.White);
                }
            }

            return arrPanels;
        }

        public void SelectTab(TabPage tabPage)
        {
            if (tabControl.SelectedTab == tabPage)
                return;

            tabControl.SelectedTab = tabPage;
            if (tabControl.Visible == false)
            {
                tabControl.Visible = true;
                tabControl.ResizeTabContorl();
            }

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
            tabControl.RemoveTabPage((SectionTabPage)tabPage); 
            if (tabControl.GetValidTabPageCount() == 0)
                EmptySOP();
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
            if (tabControl.IsHandleCreated == false)
                return null;

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
            Type type = typeof(SectionTabPage);

            foreach (Control ctrl in ctrlList)
            {
                if (ctrl.GetType() == type)
                {
                    SectionTabPage tabPage = (SectionTabPage)ctrl;
                    if (tabPage.ActionStepID == 0)
                        continue;
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
            SectionTabPage page = (SectionTabPage)pane.Parent;
            if (page != null)
            {
                page.ReSizePanel();
            }
        }

        public void PanelResize()
        {
            SectionTabPage tabPage1 = (SectionTabPage)tabControl.SelectedTab;
            if (tabPage1 != null)
                tabPage1.ReSizePanel();
        }

        public SplitContainer panel
        {
            get { return splitContainerMain; }
        }

        public SectionTabControl TabControls
        {
            get { return tabControl; }
        }

        public ArrayList GetTabPage()
        {
            ArrayList arResult = new ArrayList();

            foreach(SectionTabPage page in tabControl.TabPages)
            {
                if( page.ActionStepID > 0)
                {
                    arResult.Add(page);
                }
            }
            return arResult;
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
            if (nActionStepID < 0)
                return;

            ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
            if (tree == null)
                return;

            TreeNode node = tree.FindActionStepNode(nActionStepID);
            if (node != null)
            {
                string strFullPath = node.FullPath;
                GetDockSOPLog().ShowActionStepLog(nActionStepID, isRealMode, strFullPath.Replace('\\', szDeli), updateComponentContents);
            }
        }

        private void ShowActionStepLog(TabPage tabPage)
        {
            SectionTabPage page = (SectionTabPage)tabPage;
            int nActionStepID = FormSOP.Instance.GetTabActionStepID(page);
            ShowActionStepLog(nActionStepID, !page.VirtualMode, false);
        }

        public void ColorChangedPanel()
        {
            foreach (TabPage page in tabControl.TabPages)
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
            if (section == null)
                return false;

            SectionTabPage tabPage = (SectionTabPage)section.GetParent().Parent;
            return IsWorkingMode(tabPage.ActionStepID, !tabPage.VirtualMode);
        }

        public void OnSelectedSection(Sections.Section section)
        {
            if (section == null)
            {
                if (m_currentPanel != null)
                {
                    SectionTabPage tabPage = (SectionTabPage)m_currentPanel.Parent;
                    ShowActionStepLog(m_currentPanel.ActionStepID, !tabPage.VirtualMode, false);
                }
                return;
            }

            if (FormSOP.Instance.HasControl && IsWorkingMode(section))
                ChangeSelectedSectionState(section);

            ShowSectionProperty(section);            
            if (section != null)
            {
                if (m_currentPanel != null)
                {
                    SectionTabPage tabPage = (SectionTabPage)m_currentPanel.Parent;
                    PreviewComponentContainer pane = (PreviewComponentContainer)tabPage.PanelPreviewComponentContents;
                    pane.SelectSection(section);
                }
                return;
            }
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
            FormSOP.Instance.CurrentSection = section;

            if (FormSOP.Instance.CurrentWork == null)
                return;

            SectionState state = FormSOP.Instance.CurrentWork.FindState(section);
            if (state == null) return;

            /*if (state.State == State.RUN)
            {
                int i = 0;
                i++;
                //MessageBox.Show("run~!!!");
            }
            else*/
            {
                if (type == Sections.Section.ComponentType.TRANSMISSION)
                {
                    TSectionState tstate = (TSectionState)state;

                    if (tstate.Section.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)tstate.Section.GetSectionPainter(0);
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, tstate);
                    }

                    tstate.InProgress();
                }
                else if (type == Sections.Section.ComponentType.INTERNAL)
                {
                    ISectionState istate = (ISectionState)state;

                    if (istate.Section.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)istate.Section.GetSectionPainter(0);
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, istate);
                    }

                    istate.InProgress();
                }
                else if (type == Sections.Section.ComponentType.EXTERNAL)
                {
                    ESectionState estate = (ESectionState)state;

                    if (estate.Section.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)estate.Section.GetSectionPainter(0);
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, estate);
                    }

                    estate.InProgress();
                }
                // 시작 Section의 정보는 바뀌지 않는다.
                else if (!IsBeginSection(section))
                {
                    if (state.Section.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)state.Section.GetSectionPainter(0);
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
            SelectedSection();
        }

        public void SetCurrentPanel(Sections.PanelSection panel)
        {
            m_currentPanel = (Sections.PanelSectionEx)panel;
        }

        public void DeleteOptionChanged(object sender, DeleteOptionChangeEventArgs e)
        {
            SOPScenarioManager.Instance.DeleteOptionChanged(sender, e);
        }

        public void ChangeWaterMark(bool bUse)
        {
            if (tabControl.IsHandleCreated)
            {
                TabPage page = tabControl.SelectedTab;
                if (page != null)
                {
                    SectionTabPage tabpage = (SectionTabPage)page;
                    tabpage.UseWaterMark = bUse;
                    tabpage.Refresh();
                }
            }

        }

        public bool IsChangeCurrentTab()
        {
            TabPage page = tabControl.SelectedTab;
            if (page == null)
            {
                return true;
            }

            SectionTabPage tabPage = (SectionTabPage)page;
            if (tabPage.VirtualMode == !FormSOP.Instance.IsReal)
            {
                return false;
            }
            return true;
        }

        public void SetBackgroundImage(bool isVisible)
        {
            if (!isVisible)
            {
                //if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
                {
                    Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Quick_Main, new Size(650, 550));                    
                    panelBackImage.BackgroundImage = (Image)bitmap;
                    panelBackImage.BackgroundImageLayout = ImageLayout.Center;
                    panelBackImage.backImgWidth = 650;
                    panelBackImage.backImgHeight = 550;
                } 
                /*else
                {
                    Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.Background_Symbol);
                    panelBackImage.BackgroundImage = bitmap;
                    panelBackImage.BackgroundImageLayout = ImageLayout.Center;
                    panelBackImage.BackColor = Color.FromArgb(52, 73, 94); 
                }*/

                this.m_dockSOPLog.StopTimer();
                timerBackgroundImage.Start();
            }
            else
            {
                Bitmap bitmap = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.BackgroundNon);
                panelBackImage.BackgroundImage = bitmap;
                panelBackImage.BackgroundImageLayout = ImageLayout.None;

                splitContainerMain.Visible = true;
                this.m_dockSOPLog.StartTimer();
                timerBackgroundImage.Stop();
            }
        } 

        public void GetComponentContents(int nActionStepID, int nComponentHistoryID, Sections.Section.ComponentType componentType, DateTime time, string strComponentType, string strTask, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow logRow)
        {
            if (section == null)
                return;

            ArrayList arrAllSections = SOPScenarioManager.Instance.GetAllPanelSections(m_arrPanel);

            int nSectionCount = arrAllSections.Count;
            int nSelectedActionID = -1;

            SOPScenario sopCurrent = SOPScenarioManager.Instance.CurrentScenario;
            if (sopCurrent == null)
                return;

            nSelectedActionID = sopCurrent.ActionStepID;

            if (FormSOP.Instance.CurrentWork == null)
                return;

            SectionState state = FormSOP.Instance.CurrentWork.FindState(section);
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
            int nComponentID = panel.GetComponentID(section);

            if (state == null)
                return;

            SectionTabPage page = (SectionTabPage)panel.Parent;

            #region 실행시 새로 만드는 대신 로딩시 만들어진 것을 불러와 사용하는 옵션
            ComponentContents frmContents = GetComponentContents(/*nActionStepID, !page.VirtualMode, */section);

            if (frmContents == null)
                return;

            frmContents.ComponentID = nComponentID;
            frmContents.ComponentHistoryID = nComponentHistoryID;
            #endregion

            if (nSelectedActionID == nActionStepID)
            {
              

                if (state.State == State.RUN)
                {
                    UpdateComponentContents(frmContents, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, state.CheckedRun, state.CheckedComplete, logRow);
                    //CreateComponentContents(nComponentID, nComponentHistoryID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, logRow);
                }
                else if (state.State == State.DONE)
                {
                    frmContents.State = state.State;
                    //bool isFlag = false;
                    //ComponentContents frmContents = GetComponentContents(nActionStepID, !page.VirtualMode, nComponentHistoryID);

                    if (frmContents != null)
                    {
                        if (nComponentID == frmContents.ComponentID)
                        {
                            string strOldTitle = frmContents.GetTitle();
                            string[] strTemp = strOldTitle.Split('/');
                            if (strTemp[strTemp.Length - 1] != "실행완료")
                            {
                                if (strTemp.Length > 1)
                                {
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

                                   

                                    frmContents.UpdateContents(nCheckNotify1, nCheckNotify2, state.CheckedRun, state.CheckedComplete);
                                    //isFlag = true;
                                }
                            }
                        }
                        frmContents.BackColor = Color.Black;
                        frmContents.RemoveMargin();
                    }
                    //if (isFlag == false/* || (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)*/)
                    //{
                    //    CreateComponentContents(nComponentID, nComponentHistoryID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, logRow);
                    //}
                }

                // 현재 ComponentContents의 내용을 임무현황판(FormMissionStatus)에 전달한다.
                SetMissionStatus(page.ActionStepID, !page.VirtualMode, section);

            }

        }

        public static void MakeComponentContentsData(ComponentContents frmContents, string strTask, DateTime time, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2)
        {
            frmContents.SetTitle(strTask, time, strStatus);
            frmContents.AddGridData(section, strStatus, nCheckNotify1, nCheckNotify2);
            frmContents.State = sectionState;

        }

        private ComponentContents MakeComponentContents(int nActionStepID, bool isRealMode, int nComponentHistoryID, int nComponentID, string strTask, DateTime time, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow row, bool showNSelect)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);
            int nContentsCount = arrContents == null ? 0 : arrContents.Count;

            ComponentContents frmContents = new ComponentContents();

            frmContents.Location = new Point(0, 0);
            frmContents.Anchor = ((System.Windows.Forms.AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right));
            frmContents.TopLevel = false;
            frmContents.Parent = this;
            frmContents.ComponentID = nComponentID;
            frmContents.ComponentHistoryID = nComponentHistoryID;
            frmContents.LogGridRow = row;
            frmContents.Dock = DockStyle.Top;
            //frmContents.BringToFront();
            //splitContainerMain.Panel2.Controls.Add(frmContents);
            bool scrollVisible = splitContainerMain.Panel2.VerticalScroll.Visible;

            if (scrollVisible)
                frmContents.Size = new Size(splitContainerMain.Panel2.Width - 18, frmContents.Height);
            else
                frmContents.Size = new Size(splitContainerMain.Panel2.Width, frmContents.Height);

            string strTitle = strTask;

            if (section.Data.SectionNumber > 0)
                strTitle = section.Data.SectionNumber.ToString() + ". " + strTask;

            MakeComponentContentsData(frmContents, strTitle, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2);

            PanelSection panel = section.GetParent();
            SectionTabPage tabPage = (SectionTabPage)panel.Parent;
            AddComponentContents(tabPage, nActionStepID, isRealMode, frmContents);

            if (sectionState == State.DONE)
            {
                frmContents.GetPanel().BackColor = Color.DimGray;
                frmContents.EnableGrid(false);
            }

            frmContents.State = sectionState;

            if (showNSelect)
            {
                frmContents.Show();
                frmContents.Select();
            }

            return frmContents;
        }

        private void UpdateComponentContents(ComponentContents frmContents, string strTask, DateTime time, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2, int nCheckedRun, int nCheckedComplete, bool noDBWrite)
        {
            if (sectionState == State.DONE)
            {
                frmContents.GetPanel().BackColor = Color.DimGray;

                frmContents.EnableGrid(false);
            }

            frmContents.SetTitle(strTask, time, strStatus);
            frmContents.ChangeTitle();
            frmContents.State = sectionState;

            if (sectionState != State.DONE || noDBWrite)
            {
                // 실행완료 상태일 경우 nCheckNotify1, 2가 초기화되는 현상이 발생함
                // 완료상태는 직전 상태와 CheckNotify가 동일하므로 굳이 바꿀 필요가 없음
                //frmContents.UpdateContents(nCheckNotify1, nCheckNotify2, nCheckedRun, nCheckedComplete);
            }

            frmContents.State = sectionState;
            frmContents.Show();
            //frmContents.Select();
            SelectComponentContents(frmContents);

            if (sectionState == State.DONE)
            {
                SectionState state = FormSOP.Instance.CurrentWork.FindState(section);

                if (state != null)
                    BackToOriginState(state);
            }
        }

        public void SelectComponentContents(int nActionStepID, bool isRealMode, Sections.Section section)
        {
            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents == null)
                return;

            SectionTabPage page = GetTabPage(nActionStepID, isRealMode);

            if (page == null)
                return;

            if (page.ActionStepID == nActionStepID && page.VirtualMode == !isRealMode)
            {
                ComponentContents contents = null;

                if (section != null)
                {
                    foreach (ComponentContents _contents in arrContents)
                    {
                        if (_contents.Section == section)
                        {
                            contents = _contents;
                            break;
                        }
                    }
                }


                if (contents!= null)

                {

                    this.Invoke((MethodInvoker)delegate
                    {
                        SelectComponentContents(page, contents);
                    });
                }
                else
                {
                    int i = 0;
                    i++;
                }


            }
        }

        private void SelectComponentContents(SectionTabPage page, ComponentContents frmContents)
        {
            int nKey = page.VirtualMode ? -page.ActionStepID : page.ActionStepID;
            ComponentContents currentContents = null;

            if (m_dicSelectedComponentContents.TryGetValue(nKey, out currentContents))
            {
                if (currentContents == frmContents)
                {
                    Color colCurrent = WorkFlowManager.Instance.CurrentColor;

                    if (frmContents.Section != null && frmContents.Section.GetColor(Section.ColorTarget.FILL) == colCurrent &&
                        frmContents.BackColor == colCurrent && frmContents.GetTitleColor() == colCurrent)
                        return;
                }
            }

            m_dicSelectedComponentContents[nKey] = frmContents;

            Panel panel = page.PanelComponentContents;

            if (frmContents != null)
            {
                frmContents.Select();

                int nPanelSize = panel.Size.Height;
                int nContentsSize = frmContents.Size.Height;
                int nContentsPosition = frmContents.Location.Y;

                // 바닥으로부터의 최소 유격거리
                int nSpace = 200;
                int nDiff = nPanelSize - (nContentsPosition + nContentsSize);

                if (nDiff < nSpace)
                {
                    using (Control c = new Control() { Parent = panel, Height = 1, Top = nPanelSize + (nSpace - nDiff) })
                    {
                        panel.ScrollControlIntoView(c);
                    }
                }
            }

            WorkFlow workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

            if (workFlow == null)
                return;

            // frmContents를 제외하고 모두 선택해제 시킨다.
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is ComponentContents)
                {
                    ComponentContents contents = (ComponentContents)ctrl;

                    if (contents.Section == null)
                        continue;

                    SectionState sectionState = workFlow.FindState(contents.Section);

                    if (sectionState == null)
                        continue;

                    if (contents != frmContents)
                    {
                        // 선택되지 않은 ComponentContents의 색상은 상태값에 따라 바꾼다.
                        sectionState.SetColor(contents.Section, WorkFlowManager.GetStateColor(sectionState.State), contents.Section.Shape.Status);
                        //sectionState.SetColor(contents.Section, WorkFlowManager.GetStateColor(contents.State), contents.Section.Shape.Status);

                        contents.SetStateColor();
                        contents.ClearSelection();
                        contents.BackColor = Color.Black;
                        contents.RemoveMargin();
                        contents.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
                    }
                    else
                    {
                        Color colCurrent = WorkFlowManager.Instance.CurrentColor;

                        contents.SetStateColor();
                        // 선택된 ComponentContents의 색상은 빨간색으로 바꾼다.
                        sectionState.SetColor(contents.Section, colCurrent, contents.Section.Shape.Status);

                        contents.SetTitleColor(colCurrent);
                        contents.BackColor = colCurrent;// Color.FromArgb(45, 145, 201);
                        contents.AddMargin(7);
                        contents.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);

                        // 제어권이 있을때에만 SelectedSection을 DB에 저장한다.
                        if (FormSOP.Instance.HasControl)
                            HistoryManager.Instance.AddActionStepHistory(page.ActionStepID, !page.VirtualMode, workFlow.State, contents.Section, workFlow.BeginEndEventSendSMS);
                    }
                }
            }

            foreach (Control control in page.Controls)
            {
                if (control is PanelSectionEx)
                {
                    control.Refresh();
                    break;
                }
            }
        }

        public void SelectComponentContents(ComponentContents frmContents, bool callFromInvoke = false)
        {
            if (frmContents == null || frmContents.Section == null)
                return;

            if (callFromInvoke)
                SelectComponentContents((SectionTabPage)frmContents.Section.GetParent().Parent, frmContents);
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    SelectComponentContents((SectionTabPage)frmContents.Section.GetParent().Parent, frmContents);
                });
            }

            /*FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                if (frmContents != null)
                    frmContents.Select();

                Panel panel = (Panel)frmContents.Parent;

                int nPanelSize = panel.Size.Height;
                int nContentsSize = frmContents.Size.Height;
                int nContentsPosition = frmContents.Location.Y;

                // 바닥으로부터의 최소 유격거리
                int nSpace = 200;
                int nDiff = nPanelSize - (nContentsPosition + nContentsSize);

                if (nDiff < nSpace)
                {
                    using (Control c = new Control() { Parent = panel, Height = 1, Top = nPanelSize + (nSpace - nDiff) })
                    {
                        panel.ScrollControlIntoView(c);
                    }
                }

                SectionTabPage page = (SectionTabPage)frmContents.Section.GetParent().Parent;
                WorkFlow workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

                if (workFlow == null)
                    return;

                // frmContents를 제외하고 모두 선택해제 시킨다.
                foreach (Control ctrl in panel.Controls)
                {
                    if (ctrl is ComponentContents)
                    {
                        ComponentContents contents = (ComponentContents)ctrl;

                        if (contents.Section == null)
                            continue;

                        SectionState sectionState = workFlow.FindState(contents.Section);

                        if (sectionState == null)
                            continue;

                        if (contents != frmContents)
                        {
                            // 선택되지 않은 ComponentContents의 색상은 상태값에 따라 바꾼다.
                            sectionState.SetColor(contents.Section, WorkFlowManager.GetStateColor(contents.State), contents.Section.Shape.Status);

                            contents.SetStateColor();
                            contents.ClearSelection();
                            contents.BackColor = Color.Black;
                            contents.RemoveMargin();
                            contents.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
                        }
                        else
                        {
                            contents.SetStateColor();
                            // 선택된 ComponentContents의 색상은 빨간색으로 바꾼다.
                            sectionState.SetColor(contents.Section, SelectedComponentContentsColor, contents.Section.Shape.Status);

                            contents.SetTitleColor(SelectedComponentContentsColor);
                            contents.BackColor = SelectedComponentContentsColor;// Color.FromArgb(45, 145, 201);
                            contents.AddMargin(7);
                            contents.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);

                            // 제어권이 있을때에만 SelectedSection을 DB에 저장한다.
                            if (FormSOP.Instance.HasControl)
                                HistoryManager.Instance.AddActionStepHistory(page.ActionStepID, !page.VirtualMode, workFlow.State, contents.Section);
                        }
                    }
                }
            });*/
        }

        public ComponentContents GetCurrentSelectedComponentContents()
        {
            PanelSectionEx panel = GetCurrentPanel();

            if (panel == null)
                return null;

            SectionTabPage page = (SectionTabPage)panel.Parent;
            WorkFlow workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

            if (workFlow == null)
                return null;

            // frmContents를 제외하고 모두 선택해제 시킨다.
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is ComponentContents)
                {
                    ComponentContents contents = (ComponentContents)ctrl;

                    if (contents.Section == null)
                        continue;

                    if (contents.Section.GetColor(Section.ColorTarget.FILL) == WorkFlowManager.Instance.CurrentColor)
                        return contents;
                }
            }

            return null;
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
            SectionState state = FormSOP.Instance.CurrentWork.FindState(section);

            if (state != null)
                BackToOriginState(state);

            PanelSection panel = section.GetParent();
            SectionTabPage tabPage = (SectionTabPage)panel.Parent;

            // 같은 Section의 직전 Log가 완료 상태로 끝나지 않았다면 제거한다.
            RemovePrevComponent(tabPage, nActionStepID, isRealMode, nComponentID);
        }

        private void RemovePrevComponent(SectionTabPage tabPage, int nActionStepID, bool isRealMode, int nComponentID)
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
                        foreach (Control ctrl in tabPage.PanelComponentContents.Controls)
                        //foreach (Control ctrl in splitContainerMain.Panel2.Controls)
                        {
                            if (ctrl == contents)
                            {
                                arrContents.RemoveAt(i);
                                tabPage.PanelComponentContents.Controls.Remove(ctrl);
                                //splitContainerMain.Panel2.Controls.Remove(ctrl);
                                //ReLocation();
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            //if (tabPage.PanelComponentContents.Controls.Count == 0)
            //    HideComponentContents();
        }

        private void SetMissionStatus(int nActionStepID, bool isRealMode, Section currSection)
        {
            FormMissionStatus frmMissionStatus = FormSOP.Instance.FrmMain3;
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

            WorkFlow workCurrent = FormSOP.Instance.CurrentWork;
            if (workCurrent != null)
            {
                frmMissionStatus.Title = workCurrent.SOPName;
            }
            else
            {
                frmMissionStatus.Title = "";
            }


            if (contentsCurrent.LogGridRow != null)
            {
                Sections.Section sectionCurrent = contentsCurrent.LogGridRow.Section;

                if (workCurrent != null && sectionCurrent != null)
                {
                    SectionState stateCurrent = WorkFlowManager.Instance.Find(sectionCurrent, isRealMode);

                    if (stateCurrent != null)
                    {
                        ArrayList arrNextStates = workCurrent.FindNext(stateCurrent);

                        if (arrNextStates != null && arrNextStates.Count > 0)
                        {
                            SectionState stateNext = (SectionState)arrNextStates[0];
                            if (stateNext != null)
                                sectionNext = stateNext.Section;
                        }
                    }
                }
            }

            if (nContentsCount >= 2)
            {
                ComponentContents component = null;
                int nCurrentIndex = -1;

                if (workCurrent != null)
                {
                    bool hasRunComponent = false;

                    ComponentContents _item = null;
                    DateTime dtLatest = new DateTime();

                    foreach (ComponentContents contents in arrContents)
                    {
                        SectionState state = contents.GetSectionState();

                        if (state != null && state.State == State.DONE && contents.Section.GetComponentType() != Section.ComponentType.ENDPOINT && state.Time != null)
                        {
                            if (_item == null)
                            {
                                _item = contents;
                                dtLatest = state.Time.Data;
                            }
                            else if (dtLatest < state.Time.Data)
                            {
                                _item = contents;
                                dtLatest = state.Time.Data;
                            }
                        }
                    }

                    //// 가장 최근에 완료된 컴포넌트 찾음.
                    //foreach (ComponentContents item in from items in arrContents.AsParallel().Cast<ComponentContents>()
                    //                                   where (items.GetSectionState() != null && items.GetSectionState().State == State.DONE)
                    //                                   && items.Section.GetComponentType() != Section.ComponentType.ENDPOINT &&
                    //                                   items.GetSectionState().Time != null
                    //                                   orderby items.GetSectionState().Time.Data descending
                    //                                   select items)

                    if( _item != null)
                    {
                        frmMissionStatus.SetContents(_item, FormMissionStatus.ItemType.PREV_ITEM);                        
                    }

                    // 가장 최근에 완료된 컴포넌트가 없으면 그리드를 초기화 해줌.
                    if (_item == null)
                    {
                        frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.PREV_ITEM);
                    }

                    _item = null;
                    dtLatest = new DateTime();

                    foreach (ComponentContents contents in arrContents)
                    {
                        SectionState state = contents.GetSectionState();

                        if (state != null && state.State == State.RUN && 
                            contents.Section.GetComponentType() != Section.ComponentType.ENDPOINT &&
                            state.Time != null)
                        {
                            if (_item == null)
                            {
                                _item = contents;
                                dtLatest = state.Time.Data;
                            }
                            else if (dtLatest < state.Time.Data)
                            {
                                _item = contents;
                                dtLatest = state.Time.Data;
                            }
                        }
                    }

                    if (_item != null)
                    {
                        hasRunComponent = true;

                        component = _item;
                        frmMissionStatus.SetContents(_item, FormMissionStatus.ItemType.CURRENT_ITEM);
                    }

                    // 가장 최근에 실행중인 컴포넌트 찾음.
                    /*foreach (ComponentContents item in from items in arrContents.AsParallel().Cast<ComponentContents>()
                                                       where (items.GetSectionState() != null && items.GetSectionState().State == State.RUN) &&
                                                       items.Section.GetComponentType() != Section.ComponentType.ENDPOINT
                                                       orderby items.GetSectionState().Time.Data descending
                                                       select items)
                    {
                        hasRunComponent = true;

                        component = item;
                        frmMissionStatus.SetContents(item, FormMissionStatus.ItemType.CURRENT_ITEM);
                        break;
                    }*/

                    if (hasRunComponent == false)
                    {
                        foreach (ComponentContents item in arrContents)
                        {
                            SectionState state = item.GetSectionState();

                            if (state != null && state.State == State.INPUT)
                            {
                                if (state.Section.Data.ID == currSection.Data.ID)
                                {
                                    component = item;
                                    frmMissionStatus.SetContents(item, FormMissionStatus.ItemType.CURRENT_ITEM);
                                    break;
                                }
                            }
                        }
                        /*foreach (ComponentContents item in from items in arrContents.AsParallel().Cast<ComponentContents>()
                                                           where (items.GetSectionState() != null &&items.GetSectionState().State == State.INPUT) &&
                                                           items.Section.Data.ID == currSection.Data.ID
                                                           select items)
                        {
                            component = item;
                            frmMissionStatus.SetContents(item, FormMissionStatus.ItemType.CURRENT_ITEM);
                            break;
                        }*/
                    }

                }

                for (int nIndex = nContentsCount - 1; nIndex > -1; nIndex--)
                {
                    if (object.Equals(arrContents[nIndex], component))
                    {
                        nCurrentIndex = nIndex;
                        break;
                    }
                }

                if (nCurrentIndex > 0)
                {
                    frmMissionStatus.SetContents((arrContents[nCurrentIndex - 1] as ComponentContents), FormMissionStatus.ItemType.NEXT_ITEM);
                }
                else
                {
                    frmMissionStatus.SetSectionContents(null, FormMissionStatus.ItemType.NEXT_ITEM);
                }

                //frmMissionStatus.SetContents((ComponentContents)arrContents[nContentsCount - 1], FormMissionStatus.ItemType.CURRENT_ITEM);
                //frmMissionStatus.SetContents((ComponentContents)arrContents[nContentsCount - 2], FormMissionStatus.ItemType.PREV_ITEM);
                //frmMissionStatus.SetSectionContents(sectionNext, FormMissionStatus.ItemType.NEXT_ITEM);
            }
            else
            {
                frmMissionStatus.SetContents(null, FormMissionStatus.ItemType.PREV_ITEM);
                frmMissionStatus.SetContents((ComponentContents)arrContents[0], FormMissionStatus.ItemType.CURRENT_ITEM);
                frmMissionStatus.SetSectionContents(sectionNext, FormMissionStatus.ItemType.NEXT_ITEM);
            }
        }

        private ComponentContents CreateComponentContents(Sections.Section section, bool showNSelect)
        {
            PanelSectionEx panel = (PanelSectionEx)section.GetParent();
            SectionTabPage page = (SectionTabPage)panel.Parent;

            string strTitle = section.Title;
            return MakeComponentContents(page.ActionStepID, !page.VirtualMode, -1, -1, strTitle, DateTime.Now, "", section, State.NORMAL, 0, 0, null, showNSelect);
        }

        /*private void CreateComponentContents(int nComponentID, int nComponentHistoryID, string strTask, DateTime time, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2, DataLogGridViewRow row)
        {
            PanelSectionEx panel = (PanelSectionEx)section.GetParent();
            SectionTabPage page = (SectionTabPage)panel.Parent;

            string strTitle = strTask;

            if (section.Data.SectionNumber > 0)
                strTitle = section.Data.SectionNumber.ToString() + ". " + strTask;

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

                // 직전 실행된 Component HistoryID와 같다면 내용을 업데이트 한다.
                if (lastContents.ComponentHistoryID == nComponentHistoryID)
                {
                    UpdateComponentContents(lastContents, strTitle, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                    ClearSelectComponentContentsExclude(lastContents);
                }
                // 직전 State와 같다면 새로운 Action이 행해졌다.
                else if (lastContents.State == sectionState || lastContents.State == State.DONE)
                {
                    ComponentContents contents = MakeComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID, nComponentID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row);

                    ClearSelectComponentContentsExclude(contents);

                    if (sectionState == State.DONE)
                        ProcessDoneComponents(page.ActionStepID, !page.VirtualMode, section, nComponentID);
                }
                else
                {
                    UpdateComponentContents(lastContents, strTitle, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                    ClearSelectComponentContentsExclude(lastContents);
                }
            }
            else
            {
                // 해당 CompoenntHistory의 CompoenentContent를 가져온다.
                ComponentContents contents = GetComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID);

                if (contents == null)
                {
                    contents = MakeComponentContents(page.ActionStepID, !page.VirtualMode, nComponentHistoryID, nComponentID, strTask, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row);

                    if (sectionState == State.DONE)
                        ProcessDoneComponents(page.ActionStepID, !page.VirtualMode, section, nComponentID);
                }
                else
                {
                    UpdateComponentContents(contents, strTitle, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, row.NoDBWrite);
                }
                // 다른 ComponentContent의 선택을 해제
                ClearSelectComponentContentsExclude(contents);

            }

            // 현재 ComponentContents의 내용을 임무현황판(FormMissionStatus)에 전달한다.
            SetMissionStatus(page.ActionStepID, !page.VirtualMode);
        }*/

        private void UpdateComponentContents(ComponentContents frmContents, string strTask, DateTime time, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2, int nCheckedRun, int nCheckedComplete, DataLogGridViewRow logRow)
        {
            string strTitle = strTask;

            if (section.Data.SectionNumber > 0)
                strTitle = section.Data.SectionNumber.ToString() + ". " + strTask;

            UpdateComponentContents(frmContents, strTitle, time, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2, nCheckedRun, nCheckedComplete, logRow.NoDBWrite);
        }

        public void AddScenario(SOPScenario sopSC)
        {
            string strTrgDisasterPath = GetDisasterPath(sopSC);
            int nItemCount = cmbScenario.Items.Count;

            for (int i = 0; i < nItemCount; i++)
            {
                SOPScenario scenario = (SOPScenario)cmbScenario.Items[i];

                if (scenario.ActionStepID == sopSC.ActionStepID)
                {
                    if (scenario.RealMode == sopSC.RealMode)
                    {
                        cmbScenario.Items.RemoveAt(i);
                        i--;
                        nItemCount--;
                    }
                    else if (!IsWorkingMode(scenario.ActionStepID, scenario.RealMode))
                    {
                        cmbScenario.Items.RemoveAt(i);
                        i--;
                        nItemCount--;
                    }
                }
            }

            for (int i = 0; i < nItemCount; i++)
            {
                SOPScenario scenario = (SOPScenario)cmbScenario.Items[i];

                if (scenario.ActionStepID != sopSC.ActionStepID)
                {
                    string strDisasterPath = GetDisasterPath(scenario);

                    if (strDisasterPath == strTrgDisasterPath)
                    {
                        // 같은 Disaster 이름을 가진 SOP가 이미 실행중이면 SOP 이름에 단계명을 붙인다.
                        scenario.DisplayActionStepName = true;
                        sopSC.DisplayActionStepName = true;

                        // ComboBox의 Item Text가 Update되지 않아서 삭제후 다시 삽입한다.
                        cmbScenario.Items.RemoveAt(i);
                        cmbScenario.Items.Insert(i, scenario);
                        break;
                    }
                }
            }

            //if (!cmbScenario.Items.Contains(sopSC))
            {
                int idx = cmbScenario.Items.Add(sopSC);
                cmbScenario.SelectedIndex = idx;
            }
        }

        private string GetDisasterPath(SOPScenario scenario)
        {
            char ch = (char)6;
            int nIndex1 = scenario.ActionStepFullPath.LastIndexOf(ch);

            if (nIndex1 >= 0)
                return scenario.ActionStepFullPath.Substring(0, nIndex1).Replace(ch, '/');
            else
            {
                int nIndex2 = scenario.ActionStepFullPath.LastIndexOf('/');

                if (nIndex2 >= 0)
                    return scenario.ActionStepFullPath.Substring(0, nIndex2);
                else
                {
                    int nIndex3 = scenario.ActionStepFullPath.LastIndexOf('\\');

                    if (nIndex3 >= 0)
                        return scenario.ActionStepFullPath.Substring(0, nIndex3).Replace('\\', '/');
                }
            }

            return scenario.ActionStepFullPath;
        }

        public void RemoveScenario(SOPScenario sopSC)
        {
            int nIndex = cmbScenario.Items.IndexOf(sopSC);

            //if (cmbScenario.Items.Contains(sopSC))
            if (nIndex >= 0)
            {
                cmbScenario.Items.Remove(sopSC);

                int nSameCount = 0, nOneIndex = -1;
                SOPScenario oneScenario = null;
                string strTrgDisasterPath = GetDisasterPath(sopSC);

                for (int i = 0; i < cmbScenario.Items.Count; i++)
                {
                    SOPScenario scenario = (SOPScenario)cmbScenario.Items[i];
                    string strDisasterPath = GetDisasterPath(scenario);

                    if (strDisasterPath == strTrgDisasterPath)
                    {
                        nOneIndex = i;
                        oneScenario = scenario;
                        nSameCount++;
                    }
                }

                // 같은 Disaster 이름을 공유하는 시나리오가 없으면 단계명은 표시하지 않는다.
                if (nSameCount == 1)
                {
                    oneScenario.DisplayActionStepName = false;

                    // ComboBox의 Item Text가 Update되지 않아서 삭제후 다시 삽입한다.
                    cmbScenario.Items.RemoveAt(nOneIndex);
                    cmbScenario.Items.Insert(nOneIndex, oneScenario);
                }

                if (m_nCurrentScenarioIndex == nIndex)
                    m_nCurrentScenarioIndex = -1;
            }
        }

        public void ClearScenario()
        {
            m_nCurrentScenarioIndex  = -1;
            cmbScenario.Items.Clear();
        }

        private void EmptySOP()
        {
            FormSOP.Instance.EmptySOP();
        }

        public void SelectScenario(SOPScenario sopSC)
        {
            if (cmbScenario.Items.Contains(sopSC))
            {
                if (cmbScenario.SelectedItem == sopSC)
                {
                    SectionTabPage tabPage = GetTabPage(sopSC.ActionStepID, sopSC.RealMode);

                    if (tabPage != null)
                    {
                        SetComponentContentsPanel(tabPage.PanelComponentContents);
                        ShowComponentContents();
                    }
                }
                else
                    cmbScenario.SelectedItem = sopSC;
            }
        }

        public void ClearProcess()
        {
            foreach (object obj in splitContainerMain.Panel2.Controls)
            {
                //if (obj.GetType() == typeof(ComponentContents))
                if (obj.GetType() == typeof(Panel))
                {
                    Panel panel = (Panel)obj;

                    foreach (ComponentContents frmContents in panel.Controls)
                    {
                        //ComponentContents frmContents = (ComponentContents)obj;

                        foreach (KeyValuePair<long, ArrayList> pair in m_dicComponentContents)
                        {
                            ArrayList arrContents = pair.Value;
                            if (arrContents.Count == 0)
                                continue;

                            if (frmContents == (ComponentContents)arrContents[0])
                            {
                                arrContents.Clear();
                                panel.Controls.Clear();
                                //splitContainerMain.Panel2.Controls.Clear();
                                return;
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void splitContainerMain_Panel2_Resize(object sender, EventArgs e)
        {
            foreach (object obj in splitContainerMain.Panel2.Controls)
            {
                if (obj.GetType() == typeof(Panel))
                {
                    Panel panel = (Panel)obj;

                    foreach (Control ctrl in panel.Controls)
                    //foreach (ComponentContents frmContents in splitContainerMain.Panel2.Controls)
                    {
                        if (ctrl is ComponentContents)
                        {
                            ComponentContents frmContents = (ComponentContents)ctrl;

                            if (panel.Controls.Count > 5)
                                //if (splitContainerMain.Panel2.Controls.Count > 5)
                                frmContents.Size = new Size(splitContainerMain.Panel2.Width - 18, frmContents.Height);
                            else
                                frmContents.Size = new Size(splitContainerMain.Panel2.Width, frmContents.Height);
                        }
                    }
                }
            }

            PageBackstageHome_Resize(null, null);
        }

        public Sections.PanelSectionEx GetCurrentPanel()
        {
            return m_currentPanel;
        }

        private void EnableQuickButton(Button btn, bool enabled)
        {
            RibbonButton rbtn = null;

            QuickSOPButton btnSOP = null;

            if (btn.Tag != null && btn.Tag is QuickSOPButton)
            {
                btnSOP = btn.Tag as QuickSOPButton;

                foreach (Control ctrl in panelBackImage.Controls)
                {
                    if (ctrl is RibbonButtonQuick)
                    {
                        if (ctrl.Tag != null && (QuickSOPButton)ctrl.Tag == btnSOP)
                        {
                            rbtn = (RibbonButtonQuick)ctrl;
                            break;
                        }
                    }
                }
            }

            if (btn.Tag != null)
            {
                btnSOP.ButtonEnable = enabled;
            }

        }

        public void OnEnabled(bool isFlag)
        {
            EnableQuickButton(btnFire, isFlag);
            EnableQuickButton(btnEarthquake, isFlag);
            EnableQuickButton(btnTyphoon, isFlag);
            EnableQuickButton(btnSubmergence, isFlag);
            EnableQuickButton(btnSecurity, isFlag);
            EnableQuickButton(btnHeavySnow, isFlag);
            EnableQuickButton(btnTerror, isFlag);
            EnableQuickButton(btnPollution, isFlag);
            /*btnFire.Enabled = btnEarthquake.Enabled = btnTyphoon.Enabled = btnSubmergence.Enabled = btnGeneralDisaster.Enabled = isFlag;
            btnHeavySnow.Enabled = btnTerror.Enabled = btnPollution.Enabled = */btnOpenSOP.Enabled = cmbScenario.Enabled = isFlag;

            foreach (SectionTabPage page in this.TabControls.TabPages)
            {
                if (page.PanelComponentContents == null)
                    continue;

                foreach (Control ctrl in page.PanelComponentContents.Controls)
                {
                    if (ctrl is ComponentContents)
                    {
                        ComponentContents contents = (ComponentContents)ctrl;
                        contents.EnableGrid(isFlag);
                    }
                }
            }
            //foreach (object obj in splitContainerMain.Panel2.Controls)
            //{
            //    if (obj.GetType() == typeof(Panel))
            //    {
            //        Panel panel = (Panel)obj;

            //        foreach (ComponentContents contents in panel.Controls)
            //        {
            //            contents.EnableGrid(isFlag);
            //        }
            //        break;
            //        /*foreach (object obj in splitContainerMain.Panel2.Controls)
            //        {
            //            if (obj.GetType() == typeof(ComponentContents))
            //            {
            //                ComponentContents contents = (ComponentContents)obj;
            //                contents.EnableGrid(isFlag);
            //            }
            //        }*/
            //    }
            //}

            //splitContainerMain.Enabled = isFlag;
        }


        private TreeNode FindDisasterNode(string strDisasterFullPath, bool isDayLight)
        {
            int nIndex1 = strDisasterFullPath.IndexOf((char)0x06);
            if (nIndex1 < 0)
                return null;

            int nIndex2 = strDisasterFullPath.IndexOf((char)0x06, nIndex1 + 1);
            if (nIndex2 < 0)
                return null;

            int nIndex3 = strDisasterFullPath.IndexOf((char)0x06, nIndex2 + 1);

            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            // Tree의 현재 상태가 검색하는 SOP와 (평일/휴일) 옵션이 다를 경우 새로 로딩한다.
            if (tree.IsNormal != isDayLight)
            {
                if (!tree.Load(FormSOP.Instance.SOPManager, true, isDayLight))
                    return null;
            }

            TreeNode node = tree.FindNode(strDisasterFullPath.Substring(0, nIndex1));
            if (node == null)
                return null;

            node = tree.FindNode(strDisasterFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1), node.Nodes);
            if (node == null)
                return null;

            if (nIndex3 > -1)
            {
                node = tree.FindNode(strDisasterFullPath.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1), node.Nodes);

                if (node == null)
                    return null;

                node = tree.FindNode(strDisasterFullPath.Substring(nIndex3 + 1), node.Nodes);
            }
            else
            {
                node = tree.FindNode(strDisasterFullPath.Substring(nIndex2 + 1), node.Nodes);
            }

            return node;
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
                    SectionState state = workFlow.FindState(section);
                    if (state == null)
                        continue;

                    if (state.State == State.INPUT)
                    {
                        panel.FocusSection(section, 210);
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
                            panel.FocusSection(section, 100);
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

            SectionTabPage currentTabPage = (SectionTabPage)tabControl.SelectedTab;
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
            foreach (Button btn in m_arrSOPButtons)
            {
                if (btn.Tag == null)
                    continue;

                string strPath = ((QuickSOPButton)btn.Tag).SOPNormal;

                if (btn is RibbonButton)
                {
                    if (strPath == strDisasterFullPath)
                        (btn as RibbonButton).IsChecked = true;
                    else
                        (btn as RibbonButton).IsChecked = false;
                }
            }
        }

        private void CheckSOPButton(Button btnChecked)
        {
            foreach (Button btn in m_arrSOPButtons)
            {
                if (btn is RibbonButton)
                {
                    if (btn == btnChecked)
                        (btn as RibbonButton).IsChecked = true;
                    else
                        (btn as RibbonButton).IsChecked = false;
                }
            }
        }


        private void tabPage3_Click(object sender, EventArgs e)
        {

        }


        //int nPrevIdx = -1;
        private void cmbScenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = cmbScenario.SelectedIndex;
            if (nIdx == -1)
                return;

            if (m_systemCall)
                return;

            bool bNeedChangeSOP = false;
            TabControl tab = FormSOP.Instance.GetPageHome().TabControls;
            if (tab != null)
            {
                SOPScenario sc = (SOPScenario)cmbScenario.SelectedItem;
                int nActionStepID = sc.ActionStepID;
                bool bReal = sc.RealMode;

                SectionTabPage page = (SectionTabPage)tab.SelectedTab;
                if (page != null)
                {
                    if (page.ActionStepID != nActionStepID)
                    {
                        bNeedChangeSOP = true;
                    }
                    if (page.VirtualMode == bReal)
                    {
                        bNeedChangeSOP = true;
                    }
                }
            }

            if (m_nCurrentScenarioIndex == nIdx && bNeedChangeSOP == false)
                return;

            if (nIdx != -1)
            {
                SOPScenario sc = (SOPScenario)cmbScenario.SelectedItem;
                if (SOPScenarioManager.Instance.CurrentScenario != sc || bNeedChangeSOP == true)
                {
                    FormSOP.Instance.SelectedScenario(sc.ActionStepID, sc.RealMode);
                }

                SectionTabPage tabPage = GetTabPage(sc.ActionStepID, sc.RealMode);

                if (tabPage != null && tabPage.PanelComponentContents.Controls.Count == 0)
                {
                    GetDockSOPLog().UpdateComponentContents(sc.ActionStepID, sc.RealMode);
                }
                m_nCurrentScenarioIndex = nIdx;

                if (tabPage != null)
                {
                    if (tabControl.SelectedTab != tabPage)
                    {
                        tabControl.SelectedTab = tabPage;
                    }
                    /*SetComponentContentsPanel(tabPage.PanelComponentContents);

                    //if (tabPage.PanelComponentContents.Controls.Count > 0)
                    ShowComponentContents();*/

                    m_sopUsingTeamManager.UpdateUsingTeams(tabPage);
                    //UpdateUsingUserDefinedTeam(tabPage);


                    //  ((SOPMonitoringSystem.ComponentContents)(tabPage.PanelComponentContents.Controls[1])).Section.Editable

                    // SOP제어권자 이름 적용
                    OnChangeControlUser();
                    // 임무현황판 갱신
                    SetMissionStatus(tabPage.ActionStepID, !tabPage.VirtualMode, m_currentSection);
                }

                if (sc != null && WorkFlowManager.Instance.Get(sc.ActionStepID, sc.RealMode) != null)
                {
                    // 실행중인 SOP로 전환되었으므로 Option false로 전환한다.
                    FormSOP.Instance.EnableOptions(false);
                }

                // 제어권한에 따른 컨트롤 활성화
                OnEnabled(FormSOP.Instance.HasControl);
                // 실행자 컬럼 가시/비가시
                OnChangeVisiblityToPerformer(FormSOP.Instance.VisiblityToPerformer);
                // 이전에 선택중이던 미션항목 선택
                OnBeginMissionSelection(true);
            }
        }

        public SectionTabPage GetTabPage(int nActionStepID, bool isRealMode)
        {
            foreach (SectionTabPage tabPage in tabControl.TabPages)
            {
                if (((Control)tabPage).Enabled == false)
                    continue;
                if (tabPage.ActionStepID == nActionStepID && tabPage.VirtualMode == !isRealMode)
                    return tabPage;
            }

            return null;
        }

        public SectionTabPage GetTabPage(int nActionStepHistoryID)
        {
            foreach (SectionTabPage tabPage in tabControl.TabPages)
            {
                if (tabPage.ActionStepHistoryID == nActionStepHistoryID)
                    return tabPage;
            }

            return null;
        }

        private void SetComponentContentsPanel(Panel panel)
        {
            Panel oldPanel = GetComponentContentsPanel();

            if (oldPanel != null)
            {
                splitContainerMain.Panel2.Controls.Remove(oldPanel);
            }

            if (panel != null)
            {
                labelComponentContentsTitle.Visible = true;

                splitContainerMain.Panel2.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                panel.AutoScroll = true;
                panel.Show();
                panel.BringToFront();
            }
        }

        void panel_ControlAdded(object sender, ControlEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Panel GetComponentContentsPanel()
        {
            foreach (object obj in splitContainerMain.Panel2.Controls)
            {
                if (obj.GetType() == typeof(Panel))
                    return (Panel)obj;
            }

            return null;
        }

        private void ComponentContentsPanel_ControlAdded(object sender, ControlEventArgs e)
        //private void splitContainerMain_Panel2_ControlAdded(object sender, ControlEventArgs e)
        {
            // changed by mwkim 2015-11-23 FlowChart만 화면보기 모드일 때, Component창이 안나오도록 함.
            if (this.splitContainerMain.SplitterDistance != this.splitContainerMain.Width - this.splitContainerMain.SplitterRectangle.Width)
                ShowComponentContents();
        }

        private void ComponentContentsPanel_ControlRemoved(object sender, ControlEventArgs e)
        //private void splitContainerMain_Panel2_ControlRemoved(object sender, ControlEventArgs e)
        {
            //Panel panel = (Panel)sender;


            // 2015-06-25 영흥요청으로 항상 보이도록 수정.skkim
            //if (panel.Controls.Count == 0)
            //    HideComponentContents();
        }

        private void PageBackstageSOP_Shown(object sender, EventArgs e)
        {
            if (tabControl.GetValidTabPageCount() == 0)
                EmptySOP();
            else
            {
                // 초기 로딩시 Tab 개수가 0보다 크다면 실행중인 SOP가 존재한다는 증거가 된다.
                FormSOP.Instance.EnableOptions(false);
            }
        }

        /// <summary>
        /// 해당 시나리오가 아닌 다른 시나리오를 기본으로 선택하는 함수
        /// </summary>
        public void RefreshPage()
        {
            SOPScenario current = SOPScenarioManager.Instance.CurrentScenario;

            for (int i = 0; i < cmbScenario.Items.Count; i++)
            {
                SOPScenario sc = (SOPScenario)cmbScenario.Items[i];
                if (current == null || (current.ActionStepID != sc.ActionStepID || sc.RealMode != current.RealMode))
                {
                    m_nCurrentScenarioIndex = -1;

                    if (cmbScenario.SelectedIndex == i)
                        cmbScenario_SelectedIndexChanged(null, null);
                    else
                        cmbScenario.SelectedIndex = i;
                    break;
                }
            }
        }

        private void timerBackGroundImage_Tick(object sender, EventArgs e)
        {
            SOPScenarioManager.Instance.LoadHistory(FormSOP.Instance.DBManager, FormSOP.Instance.SOPManager);

            if (tabControl.GetValidTabPageCount() > 0)
            {
                FormSOP.Instance.SelectViewTab();

                int nSplitDistance = this.splitContainerMain.SplitterDistance;
                tabControl.Visible = true;
                panel.Visible = true;
                this.splitContainerMain.SplitterDistance = nSplitDistance;

                SetBackgroundImage(true);
            }

        }

        private void timerSelectMission_Tick(object sender, EventArgs e)
        {
            if (FormSOP.Instance.HasControl == false || m_isCallingSelf == true)
            {
                if (m_bSelectedCurrentMission == false)
                {
                    m_isCallingSelf = false;

                    OnSelectMission(m_nActionStepID, m_nReal, m_nComponentID, m_strRowIndex);
                    //System.Diagnostics.Trace.WriteLine(String.Format("{0}, {1}, {2}, {3}", m_nActionStepID, m_nReal, m_nComponentID, m_strRowIndex));
                }
            }
        }
        

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;
            if (page != null)
            {
                BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                TreeNode node = tree.FindActionStepNode(page.ActionStepID);

                if (node != null)
                {
                    tree.SelectNode(node, true);
                }

                SetComponentContentsPanel(page.PanelComponentContents);

                SetCurrentScenario(page.ActionStepID);
                FormSOP.Instance.EnabledRunGroup();
                /*PreviewComponentContainer container = (PreviewComponentContainer)page.PanelPreviewComponentContents;
                if(container != null)
                {
                    FormPreviewContainer form = container.GetParent();
                    if( form != null)
                    {
                        form.BringToFront();
                    }
                }*/
            }
        }

        private bool SetCurrentScenario(int nActionStepID)
        {
            for (int i = 0; i < cmbScenario.Items.Count; i++)
            {
                SOPScenario scenario = (SOPScenario)cmbScenario.Items[i];

                if (scenario.ActionStepID == nActionStepID)
                {
                    m_systemCall = true;
                    cmbScenario.SelectedIndex = i;
                    m_systemCall = false;
                    return true;
                }
            }

            return false;
        }

        public void CreateComponentContentsSet(SectionTabPage page)
        {
            splitContainerMain.Size = new Size(splitContainerMain.Width, splitContainerMain.Parent.Size.Height - splitContainerMain.Location.Y);
            ClearComponentContents(page, page.ActionStepID, !page.VirtualMode);

            ArrayList arrList = SOPScenarioManager.Instance.GetAllPanels(page.ActionStepID);

            if (arrList == null)
                return;

            ArrayList arrSections = SOPScenarioManager.Instance.GetAllPanelSections(arrList);

            if (arrSections == null)
                return;

            arrSections.Sort();
            arrSections.Reverse();

            // Valid phone number db loading
            //ControlTeamEditor.VaildMemberPhoneNumber.LoadDB();

            // ComponentContents 생성하는 동안 깜빡이는 현상을 없애기 위하여 생성이 끝날때까지 Hide() 시킴
            page.PanelComponentContents.Hide();

            // 개별 ComponentContents 생성시마다 교대근무자 정보를 새로 읽을 필요가 없으니 처음에 한번만 로딩하도록 한다.
            FormSOP.Instance.SOPManager.LockControlRoomMembers = false;
            FormSOP.Instance.SOPManager.LoadControlRoomMembers();
            FormSOP.Instance.SOPManager.LockControlRoomMembers = true;

            ArrayList arrContents = new ArrayList();
            
            foreach (Section section in arrSections)
            {
                Section.ComponentType type = section.GetComponentType();

                if (type == Section.ComponentType.ANNOTATION ||
                    type == Section.ComponentType.GROUP ||
                    type == Section.ComponentType.LINK ||
                    type == Section.ComponentType.NONE)
                    continue;

                ComponentContents contents = CreateComponentContents(section, false);

                if (contents != null)
                {
                    if (contents.Section == null)
                        contents.Section = section;

                    contents.Ready();
                    arrContents.Add(contents);
                }
            }

            ComponentContents lastContents = null;

            foreach (ComponentContents contents in arrContents)
            {
                contents.Show();
                lastContents = contents;
            }

            if (lastContents != null)
            {
                lastContents.Select();
            }

            FormSOP.Instance.SOPManager.LockControlRoomMembers = false;

            SetComponentContentsPanel(page.PanelComponentContents);
            page.PanelComponentContents.Show();

            //ControlTeamEditor.VaildMemberPhoneNumber.ReleaseDB();
        }

        public void CreatePreviewComponentContents(SectionTabPage page)
        {
            PreviewComponentContainer container = (PreviewComponentContainer)page.PanelPreviewComponentContents;
            if (container != null)
            {
                labelComponentContentsTitle.Visible = false;

                container.InitSectionCentent();
                FormPreviewContainer a = new FormPreviewContainer();
                a.TopLevel = false;
                a.AddPreviewContainer(container);
                a.FormBorderStyle = FormBorderStyle.None;
                a.Dock = DockStyle.Fill;
                a.Visible = true;
                splitContainerMain.Panel2.Controls.Add(a);
                a.BringToFront();
            }                   
        }
    
        public void OneTop(Player player)
        {
            if (m_currentOneTopPlayer == player)
            {
                FourBack();
                ResizeComponentContents();
                return;
            }

            if (player == Player.SectionPanel)
            {
                splitContainerVertical.Panel1Collapsed = false;
                splitContainerVertical.Panel2Collapsed = true;
                HideComponentContents();
                /*splitContainerMain.Panel2Collapsed = true;
                //splitContainerVertical.Panel1.Controls.Remove(tabControl);
                //this.Controls.Add(tabControl);
                splitContainerMain.SplitterDistance = 0;*/
            }
            else if (player == Player.SectionLog)
            {
                splitContainerVertical.Panel2Collapsed = false;
                splitContainerVertical.Panel1Collapsed = true;
                HideComponentContents();
            }
            else if (player == Player.ComponentContents)
            {
                splitContainerMain.Panel1Collapsed = true;
                ShowComponentContents();
                ResizeComponentContents();
            }

            //splitContainerMain.Visible = false;
            m_currentOneTopPlayer = player;
        }

        private void ResizeComponentContents()
        {
            if (tabControl.SelectedTab == null)
                return;

            SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;

            foreach (Control ctrl in page.PanelComponentContents.Controls)
            {
                if (ctrl is ComponentContents)
                {
                    ComponentContents contents = (ComponentContents)ctrl;
                    contents.ResizeGrid();
                }
            }
        }

        public void FourBack()
        {
            splitContainerMain.Panel1Collapsed = false;
            splitContainerVertical.Panel1Collapsed = false;
            splitContainerVertical.Panel2Collapsed = false;
            ShowComponentContents();

            m_currentOneTopPlayer = Player.None;
        }

        private void OnComponentContentsDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                OneTop(Player.ComponentContents);
        }

        public void StartComponentContents(int nActionStepID, bool isRealMode, WorkflowOption option, bool isAutoStart = false)
        //public void StartComponentContents(int nActionStepID, bool isRealMode, VariousData<DateTime> dtDetect, string strPosition, string strBroadcastPositionName, bool isAutoStart = false, string strPSMMaterialName = null, VariousData<int> psmDistance = null, string strAmountSnowfall = null)
        {
            // Section에 표시된 특수문자들을 Parsing한다.
            ChangeSectionTitle(nActionStepID, isRealMode, option);

            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents != null)
            {
                SectionTabPage page = GetTabPage(nActionStepID, isRealMode);

                // ComponentContents 생성하는 동안 깜빡이는 현상을 없애기 위하여 생성이 끝날때까지 Hide() 시킴
                if (page != null)
                    page.PanelComponentContents.Hide();

                foreach (ComponentContents contents in arrContents)
                {
                    contents.Start(option, isRealMode);

                    if (isAutoStart == false)
                        contents.InitGrid();
                }

                // ComponentContents 생성하는 동안 깜빡이는 현상을 없애기 위하여 생성이 끝날때까지 Hide() 시킴
                if (page != null)
                    page.PanelComponentContents.Show();
            }

        }

        // Section에 표시된 특수문자들을 Parsing한다.
        private void ChangeSectionTitle(int nActionStepID, bool isRealMode, WorkflowOption option)
        //private void ChangeSectionTitle(int nActionStepID, bool isRealMode, VariousData<DateTime> dtDetect, string strPosition, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall)
        {
            SectionTabPage tabPage = GetTabPage(nActionStepID, isRealMode);

            if (tabPage == null)
                return;

            DateTime time = DateTime.Now;
            string strPosition = "", strPSMMaterialName = "", strAmountSnowfall = "";
            VariousData<int> psmDistance = null;

            if (option != null)
            {
                if (option.DetectTime != null)
                    time = option.DetectTime.Data;

                strPosition = option.PositionName;

                if (option is WorkflowOptionPSM)
                {
                    WorkflowOptionPSM optionPSM = (WorkflowOptionPSM)option;

                    if (optionPSM.PSMMaterial != null)
                        strPSMMaterialName = optionPSM.PSMMaterial.MaterialName;

                    psmDistance = new VariousData<int>(optionPSM.PSMDistance);
                }
                else if (option is WorkflowOptionSnowFall)
                {
                    WorkflowOptionSnowFall optionSnow = (WorkflowOptionSnowFall)option;
                    strAmountSnowfall = optionSnow.AmountSnowFall.ToString();
                }
            }

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter("", time, strPosition);

            if (strPSMMaterialName != null && strPSMMaterialName.Length > 0)
            {
                param.PSMMaterialType = strPSMMaterialName;
                param.PSMDistance = psmDistance.Data;
            }

            if (strAmountSnowfall != null && strAmountSnowfall.Length > 0)
                param.AmountSnowfall = strAmountSnowfall;

            List<PanelSection> panels = tabPage.GetPanelSections();

            foreach (PanelSection panel in panels)
            {
                foreach (Section section in panel.Sections)
                {
                    param.Message = section.Title;
                    section.Title = UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
                }
            }
        }

        public void PostCreateProcess(UnE.SOP.Process.ProcessSectionIF process, SectionState state)
        {
            ComponentContents contents = GetComponentContents(state.Section);

            if (contents == null)
                return;

            contents.Process = process;
        }

        public void OnCloseWorkFlow(int nActionStepID, bool isRealMode, WorkFlowState state)
        {
            SelectComponentContents(nActionStepID, isRealMode, null);

            int nKey = isRealMode ? nActionStepID : -nActionStepID;
            m_dicSelectedComponentContents.Remove(nKey);

            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents != null)
            {
                foreach (ComponentContents contents in arrContents)
                {
                    contents.Complete();
                }
            }

            SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;
            if( page != null)
            {
                List<PanelSection> panels = page.GetPanelSections();
                foreach(PanelSectionEx pane in panels)
                {
                    pane.ShowAllSectionButtons();
                    pane.SetInfoText("", "");
                }
            }

            // 종료된 SOP의 ActionStepHistoryID 정보를 삭제한다.
            if (state == WorkFlowState.STOP || state == WorkFlowState.DONE)
            {
                FormSOP.Instance.SOPManager.RemoveActionStepHistoryID(nActionStepID, isRealMode);
            }
        }

        public void SetSectionDetailDatas(Dictionary<int, List<HistorySectionData.DetailData>> detailDatas, Section section, int nComponentHistory)
        {
            ComponentContents contents = GetComponentContents(section);

            if (contents != null)
                contents.SetDetailDatas(detailDatas, nComponentHistory, false);
        }

        public void OnChangeVisiblityToPerformer(bool isVisible)
        {
            if (FormSOP.Instance.CurrentWork == null)
                return;

            int nActionStepID = FormSOP.Instance.CurrentWork.ActionStepID;
            bool isRealMode = WorkFlowManager.Instance.RealWorkFlowList.ContainsKey(nActionStepID);

            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents != null)
            {
                foreach (ComponentContents contents in arrContents)
                {
                    contents.ChangeVisiblityToPerformer(isVisible);
                }
            }
        }

        public void OnChangeControlUser()
        {
            if (FormSOP.Instance.CurrentWork == null)
                return;

            int nActionStepID = FormSOP.Instance.CurrentWork.ActionStepID;
            bool isRealMode = WorkFlowManager.Instance.RealWorkFlowList.ContainsKey(nActionStepID);

            ArrayList arrContents = GetComponentContentsList(nActionStepID, isRealMode);

            if (arrContents != null)
            {
                foreach (ComponentContents contents in arrContents)
                {
                    contents.ChangeCommanderName();
                }
            }

            if (tabControl.SelectedTab == null)
                return;
            else if ((SectionTabPage)tabControl.SelectedTab == null)
                return;

            SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;
            OnApplyControlUserToMissionStatus(page);
        }

        private void btnEditExternalMembers_Click(object sender, EventArgs e)
        {
            if (m_sopUsingTeamManager.ShowDialogEditExternalMembers(tabControl))
            {
                SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;
                SetMissionStatus(page.ActionStepID, !page.VirtualMode, m_currentSection);
                OnBeginMissionSelection(true);
            }
        }

        /*public void UpdateUsingUserDefinedTeam(SectionTabPage page)
        {       
            if (page == null)
            {
                return;
            }
            // 진행중인 SOP인경우 
            int nHistoryID = page.ActionStepHistoryID;
            if (nHistoryID > 0)
            {
                // 해당 HistoryID로 DB에 저장된 UsingTeam정보를 가져온다.
                ArrayList arUTeams = FormSOP.Instance.SOPManager.GetUsingUserDefineTeamsByHistoryID(nHistoryID);
                if (arUTeams != null && arUTeams.Count > 0)
                    page.AddUserDefinedTeams(arUTeams);
            } 
        }*/

        /*public void SaveUsingUserDefinedTeam(SectionTabPage page)
        {
            if (page == null)
            {
                return;
            }

            int nHistoryID = page.ActionStepHistoryID;
            if( nHistoryID > 0)
            {
                List<Data_UserDefinedTeam> arTeams = page.GetUsingUserDefineTeams();
                FormSOP.Instance.SOPManager.SaveUsingUserDefinedTeam(nHistoryID, arTeams);
            }
        }*/
        
        public void ShowSectionBtn(bool bShow)
        {
            TabControl.TabPageCollection tabs = TabControls.TabPages;
            if( tabs != null && tabs.Count > 0)
            {
                foreach(SectionTabPage page in tabs)
                {
                    List<PanelSection> panels = page.GetPanelSections();
                    foreach(PanelSection pane in panels)
                    {
                        PanelSectionEx panel = (PanelSectionEx)pane;
                        panel.ShowSectionButton(bShow);
                    }
                }
            }
        }

        public void EnableButton(bool bEnable)
        {
            btnEditExternalMembers.Enabled = bEnable;
        }

        public void Work(object arg)
        {
            if (arg == null)
                return;

            if (arg is SpecialWork)
            {
                SpecialWork work = (SpecialWork)arg;

                if (work.WorkType == SpecialWork.SpecialWorkType.SAVE_USING_UserDefinedTeam)
                {
                    if (work.Data != null && work.Data is SectionTabPage)
                    {
                        SectionTabPage page = (SectionTabPage)work.Data;
                        m_sopUsingTeamManager.SaveUsingTeams(page);
                       // SaveUsingUserDefinedTeam(page);
                    }
                }
            }
        }

        public void ShowTranslucentForm(Form targetForm, int nCommandID)
        {
            ShowTranslucentForm(targetForm, 0, 0, this.Size.Width, this.Size.Height, nCommandID);
        }

        public void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
                return;

            //FormSOP.Instance.SetDisableToolBar();

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, width, height, this);
            mTranslucentForm.Parent = this;
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(this);
        }

        public void CloseTranslucentForm()
        {
            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                return;

            mTranslucentForm.CloseExternal();
        }

        public void ShowCCTVToolStripMenuItem(bool isVisible)
        {
            panelBackImage.ShowCCTVToolStripMenuItem(isVisible);
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.ShowSendSMS();
        }

        private void btnBroadRunner_Click(object sender, EventArgs e)
        {
            /*string strWorkingDirectory = ".\\", strFileName = "TTSServerDotNetCmd.exe";

            if (System.IO.File.Exists(strWorkingDirectory + strFileName))
            {
                System.Diagnostics.Trace.WriteLine("true");
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strWorkingDirectory;
                startInfo.ErrorDialog = true;

                System.Diagnostics.Process process;

                try
                {
                    process = System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }*/
            FormSOP.Instance.ShowTestBroadcast();
        }

        public void RefreshComponentContents(ComponentContents contents)
        {
            if (m_noRefreshComponentContents)
                m_dicNeedRefreshContents[contents] = contents;
            else
            {
                contents.Refresh();
            }
        }

        public void BeginHistory()
        {
            m_noRefreshComponentContents = true;
        }

        public void EndHistory()
        {
            m_noRefreshComponentContents = false;
            
            try
            {
                if (FormSOP.Instance.IsDisposed == false)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        foreach (KeyValuePair<ComponentContents, ComponentContents> pair in m_dicNeedRefreshContents)
                        {
                            pair.Key.Refresh();
                        }
                    });

                }   
            }catch(Exception)
            { }
                     
            m_dicNeedRefreshContents.Clear();
        }
	}

    public class TabPageSOPTeamUserManager
    {
        private ArrayList GetUsingSOPTeamList(int nActionStepID)
        {
            ArrayList arrTeams = new ArrayList();

            List<long> teamDatas = FormSOP.Instance.SOPManager.GetSOPTeamIDs(nActionStepID, true);

            foreach (long data in teamDatas)
            {
                int nTeamType = (int)(data >> 32);
                int nTeamID = (int)(data & 0xffffffff);

                if (nTeamType == 0)
                {
                    Data_NormalTeam team = FormSOP.Instance.SOPManager.GetTemporaryNormalTeam(nTeamID);

                    if (team != null)
                        arrTeams.Add(team.Clone());
                }
                else if (nTeamType == 1)
                {
                    Data_EmergencyTeam team = FormSOP.Instance.SOPManager.GetTemporaryEmergencyTeam(nTeamID);

                    if (team != null)
                        arrTeams.Add(team.Clone());
                }
                else if (nTeamType == 2)
                {
                    Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetExternalTeam(nTeamID);

                    if (team != null)
                        arrTeams.Add(new Data_ExternalTeam(team.ID, team.TeamName, team.PhoneNumber, team.FaxNumber));
                }
                else if (nTeamType == 3)
                {
                    Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeamMember(nTeamID);

                    if (team != null)
                        arrTeams.Add(new Data_UserDefinedTeam(team.ID, team.TeamName, team.PhoneNumber, team.FaxNumber));
                }
                else if (nTeamType == 4)
                {
                    Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(nTeamID);

                    if (team != null)
                        arrTeams.Add(team.Clone());
                }
                else if (nTeamType == 10)
                {
                    Data_ControlRoom team = FormSOP.Instance.SOPManager.GetControlRoom(nTeamID);

                    if (team != null)
                        arrTeams.Add(team.Clone());
                }
            }

            return arrTeams;
        }

        public void SetPageSOPTeams(SectionTabPage page, int nActionStepID)
        {
            ArrayList arrTeams = GetUsingSOPTeamList(nActionStepID);

            if (arrTeams == null)
                return;

            foreach (object team in arrTeams)
            {
                if (team is Data_UserDefinedTeam)
                {
                    page.AddUserDefinedTeam((Data_UserDefinedTeam)team);
                }
                else if (team is Data_ExternalTeam)
                {
                    page.AddExternalTeam((Data_ExternalTeam)team);
                }
                else if (team is Data_NormalTeam)
                {
                    page.AddTemporaryNormalTeam((Data_NormalTeam)team);
                }
                else if (team is Data_EmergencyTeam)
                {
                    page.AddTemporaryEmergencyTeam((Data_EmergencyTeam)team);
                }
                else if (team is Data_RegularTeam)
                {
                    page.AddRegularTeam((Data_RegularTeam)team);
                }
                else if (team is Data_ControlRoom)
                {
                    page.AddControlRoom((Data_ControlRoom)team);
                }
            }
        }

        public void UpdateUsingTeams(SectionTabPage page)
        {
            if (page == null)
                return;

            // 진행중인 SOP인경우 
            int nHistoryID = page.ActionStepHistoryID;
            if (nHistoryID > 0)
            {
                // 해당 HistoryID로 DB에 저장된 UsingTeam정보를 가져온다.
                ArrayList arrTeams = FormSOP.Instance.SOPManager.RoleMemberManager.GetUsingTeamsByHistoryID(nHistoryID);

                if (arrTeams != null)
                {
                    foreach (object team in arrTeams)
                    {
                        if (team is Data_NormalTeam)
                            page.AddTemporaryNormalTeam((Data_NormalTeam)team);
                        else if (team is Data_EmergencyTeam)
                            page.AddTemporaryEmergencyTeam((Data_EmergencyTeam)team);
                        else if (team is Data_ExternalTeam)
                            page.AddExternalTeam((Data_ExternalTeam)team);
                        else if (team is Data_RegularTeam)
                            page.AddRegularTeam((Data_RegularTeam)team);
                        else if (team is Data_UserDefinedTeam)
                            page.AddUserDefinedTeam((Data_UserDefinedTeam)team); 
                    }
                }
            }
        }

        public void SaveUsingTeams(SectionTabPage page)
        {
            if (page == null)
                return;

            int nHistoryID = page.ActionStepHistoryID;

            if (nHistoryID > 0)
            {
                List<Data_UserDefinedTeam> userDefinedTeams = page.GetUsingUserDefineTeams();
                List<Data_ExternalTeam> externalTeams = page.GetUsingExternalTeams();
                List<Data_RegularTeam> regularTeams = page.GetUsingRegularTeams();
                List<Data_NormalTeam> normalTeams = page.GetUsingTemporaryNormalTeams();
                List<Data_EmergencyTeam> emergencyTeams = page.GetUsingTemporaryEmergencyTeams(); 

                CheckTeams(userDefinedTeams, externalTeams, regularTeams, normalTeams, emergencyTeams);
                FormSOP.Instance.SOPManager.RoleMemberManager.SaveUsingTeams(nHistoryID, userDefinedTeams, externalTeams, regularTeams, normalTeams, emergencyTeams);
            }
        }

        private void CheckTeams(List<Data_UserDefinedTeam> userDefinedTeams, List<Data_ExternalTeam> externalTeams, List<Data_RegularTeam> regularTeams, List<Data_NormalTeam> normalTeams, List<Data_EmergencyTeam> emergencyTeams)
        {
            if (userDefinedTeams != null)
            {
                foreach (Data_UserDefinedTeam team in userDefinedTeams)
                {
                    if (team.Tag == null)
                        FormEditExteranlTeam.MakeRoleMember(team);
                }
            }

            if (externalTeams != null)
            {
                foreach (Data_ExternalTeam team in externalTeams)
                {
                    if (team.Tag == null)
                        FormEditExteranlTeam.MakeRoleMember(team);
                }
            }

            if (regularTeams != null)
            {
                foreach (Data_RegularTeam team in regularTeams)
                {
                    if (team.Tag == null)
                        FormEditExteranlTeam.MakeRoleMember(team);
                }
            }

            if (normalTeams != null)
            {
                foreach (Data_NormalTeam team in normalTeams)
                {
                    if (team.Tag == null)
                        FormEditExteranlTeam.MakeRoleMember(team);
                }
            }

            if (emergencyTeams != null)
            {
                foreach (Data_EmergencyTeam team in emergencyTeams)
                {
                    if (team.Tag == null)
                        FormEditExteranlTeam.MakeRoleMember(team);
                }
            } 
        }

        public bool ShowDialogEditExternalMembers(SectionTabControl tabControl)
        {
            SectionTabPage page = (SectionTabPage)tabControl.SelectedTab;
            if (page == null)
            {
                return false;
            }

            UpdateUsingTeams(page);
            //UpdateUsingUserDefinedTeam(page);     

            //List<Data_UserDefinedTeam> arTeams = page.GetUsingUserDefineTeams();
            FormEditExteranlTeam teamEditor = new FormEditExteranlTeam();
            teamEditor.SetUsingTeam(page);

            //teamEditor.UsingTeams = arTeams;
            if (teamEditor.ShowDialog() == DialogResult.OK)
            {
                ArrayList usingTeams = teamEditor.UsingTeams;

                foreach (object team in usingTeams)
                {
                    if (team is Data_UserDefinedTeam)
                        page.AddUserDefinedTeam((Data_UserDefinedTeam)team);
                    else if (team is Data_ExternalTeam)
                        page.AddExternalTeam((Data_ExternalTeam)team);
                    else if (team is Data_RegularTeam)
                        page.AddRegularTeam((Data_RegularTeam)team);
                    else if (team is Data_NormalTeam)
                        page.AddTemporaryNormalTeam((Data_NormalTeam)team);
                    else if (team is Data_EmergencyTeam)
                        page.AddTemporaryEmergencyTeam((Data_EmergencyTeam)team); 
                }

                SaveUsingTeams(page);
                /*List<Data_UserDefinedTeam> arResult = teamEditor.UsingTeams;
                ArrayList ar = new ArrayList();
                ar.AddRange(arResult);
                page.AddUserDefinedTeams(ar);
                
                SaveUsingUserDefinedTeam(page);*/
                ResetUserDefinedTeamNames(page);
                FormSOP.Instance.NetworkManager.ClientProvier.SendResetUserDefinedTeamNames(page.ActionStepHistoryID);

                return true;
            }

            return false;
        }

        public void ResetUserDefinedTeamNames(int nActionStepHistoryID)
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                SectionTabPage page = FormSOP.Instance.GetPageHome().GetTabPage(nActionStepHistoryID);

                if (page == null)
                    return;

                UpdateUsingTeams(page);
                ResetUserDefinedTeamNames(page);

                FormSOP.Instance.GetPageHome().OnApplyControlUserToMissionStatus(page);
            });
        }

        private void ResetUserDefinedTeamNames(SectionTabPage page)
        {
            foreach (object obj in page.PanelComponentContents.Controls)
            {
                if (obj is ComponentContents)
                {
                    ComponentContents contents = (ComponentContents)obj;
                    contents.ResetUserDefinedTeamNames(page);
                }
            }
        }
    }
}
