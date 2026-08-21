using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using XtremeDockingPane;

namespace TeamManagementSystem
{
    public partial class FormMain : Form
    {
        private FormLeftTeamState m_frmTeamState = null;
        private FormLeftTeamTree m_frmTeamTree = null;
        private FormRightTeamProperties m_frmTeamProperties = null;
        private FormRightPerssonnel m_frmPerssonnel = null;
        private FormTeamVersion m_frmVersion = new FormTeamVersion();

        private Form[] arrDocking = new Form[8];

        private WebDBManager m_dbMgr = null;

        // DB를 저장할 ArrayList
        private ArrayList m_arrTeamVersion = new ArrayList();
        private ArrayList m_arrMember = new ArrayList();
        private ArrayList m_arrRegular = new ArrayList();
        private ArrayList m_arrOrgani = new ArrayList();
        private ArrayList m_arrNormal = new ArrayList();
        private ArrayList m_arrEmergency = new ArrayList();


        private DataGridView m_dataGrid = new DataGridView();

        private ArrayList m_arrSections = new ArrayList();
        private ArrayList m_arrESections = new ArrayList();
        private ArrayList m_arrNSections = new ArrayList();

        private ArrayList m_arrCurrentView = new ArrayList();
        private ArrayList m_arrIndex = new ArrayList();

        private int m_nTeamVersionID = -1;
        private string m_strTeamVersion;
        private string m_strTeamName;
        private Point m_gridPosition;
        private bool m_isEditMode = false;
        private int m_nTeamMode = 0;
        private int m_nSectionIndex = 1;
        private bool m_isNewProject = false;
        private bool m_isOpen = false;
        private int m_nWeekday = 1;
        private bool m_isWeekday = true;
        private bool m_isWeekend = false;

        private int m_nNormalCount = 0;
        private int m_nEmergencyCount = 0;
        private int m_nLoginID = 0;

        private SectionGrid m_sectionGrid = null;
        // <RegularTeamID, SectionGrid>
        private Dictionary<int, SectionGrid> m_dicSections = new Dictionary<int, SectionGrid>();
        private Dictionary<int, SectionGrid> m_dicESections = new Dictionary<int, SectionGrid>();
        private Dictionary<int, SectionGrid> m_dicNSections = new Dictionary<int, SectionGrid>();


        // <RegularTeamID, ...>
        private Dictionary<int, Data_OrganizationHistory> m_dicMember = new Dictionary<int, Data_OrganizationHistory>();
        private Dictionary<int, Data_NormalHistory> m_dicNormal = new Dictionary<int, Data_NormalHistory>();
        private Dictionary<int, Data_EmergencyHistory> m_dicEmergency = new Dictionary<int, Data_EmergencyHistory>();
        //private ArrayList m_arrRSelectedSections = new ArrayList();
        //private ArrayList m_arrNSelectedSections = new ArrayList();
        //private ArrayList m_arrESelectedSections = new ArrayList();

        private int m_nScrollInitPos1 = 0;
        private int m_nScrollInitPos2 = 0;
        private int m_nScrollInitPos3 = 0;

        // History Table과 원래 Data Table 간의 ID 값 차이를 계산하기 위한 임시 변수
        private int m_nTemporaryNormalTeamHistoryID = 0;
        private int m_nTemporaryEmergencyTeamHistoryID = 0;

        private int m_nID = 0;

        public int LoginID
        {
            get { return m_nLoginID; }
            set { m_nLoginID = value; }
        }

        public int NormalCount
        {
            get { return m_nNormalCount; }
            set { m_nNormalCount = value; }
        }

        public int EmergencyCount
        {
            get { return m_nEmergencyCount; }
            set { m_nEmergencyCount = value; }
        }

        public ArrayList RemoveSectionIndex
        {
            get{return m_arrIndex;}
            set{m_arrIndex = value;}
        }

        public ArrayList CurrentView
        {
            get { return m_arrCurrentView; }
            set { m_arrCurrentView = value; }
        }

        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }

        public int SectionIndex
        {
            get { return m_nSectionIndex; }
            set { m_nSectionIndex = value; }
        }

        public bool Weekday
        {
            get { return m_isWeekday; }
            set { m_isWeekday = value; }
        }

        public bool Weekend
        {
            get { return m_isWeekend; }
            set { m_isWeekend = value; }
        }

        public int TeamMode
        {
            get { return m_nTeamMode; }
            set { m_nTeamMode = value; }
        }

        public bool EditMode
        {
            get { return m_isEditMode; }
            set { m_isEditMode = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        //private float m_fRScale = 1.0f;

        public string VersionName
        {
            get { return m_strTeamVersion; }
            set { m_strTeamVersion = value; }
        }

        public int VersionID
        {
            get { return m_nTeamVersionID; }
            set { m_nTeamVersionID = value; }
        }

        public ArrayList TeamVersion
        {
            get { return m_arrTeamVersion; }
            set { m_arrTeamVersion = value; }
        }

        public ArrayList CompanyMember
        {
            get { return m_arrMember; }
            set { m_arrMember = value; }
        }

        public ArrayList RegularTeam
        {
            get { return m_arrRegular; }
            set { m_arrRegular = value; }
        }

        public ArrayList Organigation
        {
            get { return m_arrOrgani; }
            set { m_arrOrgani = value; }
        }

        public ArrayList NormalTeam
        {
            get { return m_arrNormal; }
            set { m_arrNormal = value; }
        }

        public ArrayList EmergencyTeam
        {
            get { return m_arrEmergency; }
            set { m_arrEmergency = value; }
        }

        public Dictionary<int, Data_OrganizationHistory> DictionaryMember
        {
            get { return m_dicMember; }
            set { m_dicMember = value; }
        }

        public Dictionary<int, Data_NormalHistory> DictionaryNormal
        {
            get { return m_dicNormal; }
            set { m_dicNormal = value; }
        }

        public Dictionary<int, Data_EmergencyHistory> DictionaryEmergency
        {
            get { return m_dicEmergency; }
            set { m_dicEmergency = value; }
        }

        public FormMain()
        {
            InitializeComponent();
          
            m_dbMgr = new WebDBManager(this);
            m_frmVersion.SetMain(this);
            toolStrip1.Visible = false;
        }

        public void FormMain_Load(object sender, EventArgs e)
        {
            m_nScrollInitPos1 = btnScroll.Left;
            m_nScrollInitPos2 = btnScroll2.Left;
            m_nScrollInitPos3 = btnScroll3.Left;

            tsbtnRegular_Click(sender, e);
        }

        // nPanelType : 상시조직(0), 평일 비상조직(1), 휴일 비상조직(2)
        public int GetScrollPos(int nPanelType)
        {
            Button scroll = null;
            int nInitPos = 0;

            if (nPanelType == 0)
            {
                nInitPos = m_nScrollInitPos1;
                scroll = btnScroll;
            }
            else if (nPanelType == 1)
            {
                nInitPos = m_nScrollInitPos2;
                scroll = btnScroll2;
            }
            else if (nPanelType == 2)
            {
                nInitPos = m_nScrollInitPos3;
                scroll = btnScroll3;
            }
            else
                return 0;

            return nInitPos - scroll.Left;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            panelOrganizational.SetBounds(left, top, right - left, bottom - top);
        }

        private void axDockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            int nIndex = e.item.Id;

            if (nIndex == 0)
                e.item.Handle = arrDocking[0].Handle.ToInt32();
            else if (nIndex == 1)
                e.item.Handle = arrDocking[1].Handle.ToInt32();
            else if (nIndex == 2)
                e.item.Handle = arrDocking[2].Handle.ToInt32();
            else if (nIndex == 3)
                e.item.Handle = arrDocking[3].Handle.ToInt32();
            else if (nIndex == 4)
                e.item.Handle = arrDocking[4].Handle.ToInt32();
        }

        private void axDockingPane_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            panelOrganizational.SetBounds(left, top, right - left, bottom - top);
        }

        public void CreatePane()
        {
            axDockingPane.AttachToWindow(this.Handle.ToInt32());

            //axDockingPane.ScaleMode = XtremeDockingPane.XTPScaleMode.xtpScalePixel;

            // Left
            Pane paneTeamTree = axDockingPane.CreatePane(1, 250, 130, DockingDirection.DockLeftOf, null);
            paneTeamTree.Title = "조직 체계";
            paneTeamTree.Options = PaneOptions.PaneNoCloseable;

            Pane paneTeamState = axDockingPane.CreatePane(0, 250, 70, DockingDirection.DockTopOf, paneTeamTree);
            paneTeamState.Title = "조직 구성 현황";
            paneTeamState.Options = PaneOptions.PaneNoCloseable;

            //Right
            Pane panePersonnel = axDockingPane.CreatePane(3, 250, 50, DockingDirection.DockRightOf, null);
            panePersonnel.Title = "개인 속성";
            panePersonnel.Options = PaneOptions.PaneNoCloseable;

            Pane paneTeam = axDockingPane.CreatePane(2, 250, 130, DockingDirection.DockTopOf, panePersonnel);
            paneTeam.Title = "팀 속성";
            paneTeam.Options = PaneOptions.PaneNoCloseable;

            arrDocking[0] = new FormLeftTeamState(this);
            m_frmTeamState = (FormLeftTeamState)arrDocking[0];

            arrDocking[1] = new FormLeftTeamTree(this);
            m_frmTeamTree = (FormLeftTeamTree)arrDocking[1];

            arrDocking[2] = new FormRightTeamProperties(this);
            m_frmTeamProperties = (FormRightTeamProperties)arrDocking[2];

            arrDocking[3] = new FormRightPerssonnel(this);
            m_frmPerssonnel = (FormRightPerssonnel)arrDocking[3];

        }
        
        public void CreateMainView(Form parent)
        {
            this.MdiParent = parent;
            this.WindowState = FormWindowState.Maximized;
            this.Show();
            this.Text = "";

            this.CreatePane();
        }
        
        public FormLeftTeamState GetTeamState()
        {
            return m_frmTeamState;
        }

        public FormRightTeamProperties GetTeamProperties()
        {
            return m_frmTeamProperties;
        }

        public FormRightPerssonnel GetPerssonnel()
        {
            return m_frmPerssonnel;
        }

        public void NewFile()
        {
            
            //m_arrSections.Clear();
            //m_dicSections.Clear();
            //splitContainer.Panel1.Controls.Clear();
            //splitContainer.Panel1.Refresh();
            
                        
            m_arrESections.Clear();
            m_arrNSections.Clear();

            m_dicNSections.Clear();
            m_dicESections.Clear();

            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel1.Refresh();
            splitContainer1.Panel2.Refresh();

            m_frmTeamState.RemoveData();
            //m_frmTeamTree.RemoveData();
            m_frmTeamProperties.RemoveData();
            m_frmPerssonnel.RemoveData();

            m_isNewProject = true;
        }

        public void GetVersion(bool isBegin, Form parent)
        {
            if (isBegin)
            {
                if (m_frmVersion.ShowDialog() == DialogResult.OK)
                {
                    NewFile();
                    m_isOpen = true;
                    GetTeamState().InitData();
                    m_frmTeamTree.Init();
                    ReadOnly(!EditMode);
                }
            }
            else
            {
                if (m_frmVersion.ShowDialog() == DialogResult.OK)
                {
                    CreateMainView(parent);
                }
                else
                {
                    Application.Exit();
                }
            }

            m_isNewProject = false;
        }

        // 상시조직도
        public void Organization_Regular()
        {
            tsbtnRegular.Checked = true;
            tsbtn1.Checked = tsbtnEmergency1.Checked = tsbtnEmergency.Checked = tsbtnBoth.Checked = !tsbtnRegular.Checked;

            splitContainer.Panel2Collapsed = true;

            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(0);

            m_arrCurrentView.Clear();
            m_arrCurrentView.Add(m_nTeamMode);
        }

        // 상시조직도 & 비상조직도
        public void Organization_Both()
        {
            tsbtnBoth.Checked = true;
            tsbtn1.Checked = tsbtnEmergency1.Checked = tsbtnRegular.Checked = tsbtnEmergency.Checked = !tsbtnBoth.Checked;

            splitContainer.Panel2Collapsed = false;
            splitContainer.Panel1Collapsed = false;

            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(2);

            m_arrCurrentView.Clear();
            m_arrCurrentView.Add(0);
            m_arrCurrentView.Add(m_nWeekday);
        }

        // 평일비상조직도
        public void Check_Weekday()
        {
            tsbtnEmergency.Checked = true;
            tsbtn1.Checked = tsbtnEmergency1.Checked = tsbtnBoth.Checked = tsbtnRegular.Checked = !tsbtnEmergency.Checked;

            splitContainer.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = true;

            AllSectionClearSelect(1);
            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(1);

            m_nWeekday = 1;
            m_arrCurrentView.Clear();
            m_arrCurrentView.Add(m_nWeekday);
        }

        // 주말비상조직도
        public void Check_Weekend()
        {
            tsbtnEmergency1.Checked = true;
            tsbtn1.Checked = tsbtnBoth.Checked = tsbtnEmergency.Checked = tsbtnRegular.Checked = !tsbtnEmergency1.Checked;

            splitContainer.Panel1Collapsed = true;
            splitContainer1.Panel1Collapsed = true;
            AllSectionClearSelect(2);
            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(1);

            m_nWeekday = 2;
            m_arrCurrentView.Clear();
            m_arrCurrentView.Add(m_nWeekday);
        }

        private void tsbtnRegular_Click(object sender, EventArgs e)
        {
            Organization_Regular();
        }

        private void tsbtnEmergency_Click(object sender, EventArgs e)
        {
            tsbtnEmergency.Checked = true;
            tsbtn1.Checked = tsbtnEmergency1.Checked = tsbtnBoth.Checked = tsbtnRegular.Checked = !tsbtnEmergency.Checked;

            splitContainer.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = true;

            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(1);
        }

        private void tsbtnEmergency1_Click(object sender, EventArgs e)
        {
            tsbtnEmergency1.Checked = true;
            tsbtn1.Checked = tsbtnBoth.Checked = tsbtnEmergency.Checked = tsbtnRegular.Checked = !tsbtnEmergency1.Checked;

            splitContainer.Panel1Collapsed = true;
            splitContainer1.Panel1Collapsed = true;

            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(1);
        }

        private void tsbtnBoth_Click(object sender, EventArgs e)
        {
            Organization_Both();
        }

        private void tsbtn1_Click(object sender, EventArgs e)
        {
            tsbtn1.Checked = true;
            tsbtnRegular.Checked = tsbtnEmergency1.Checked = tsbtnEmergency.Checked = tsbtnBoth.Checked = !tsbtn1.Checked;

            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel1Collapsed = false;

            if (m_frmTeamTree != null)
                m_frmTeamTree.treeViewResize(2);
        }

        public void ShowContextMenu(DataGridView datagrid, int x, int y)
        {
            if(EditMode)
            {
                contextMenuNormal.Show(datagrid, x, y);
                m_dataGrid = datagrid;
            }
        }

        private void tsbtnTest_Click(object sender, EventArgs e)
        {
//             //m_arrSections = new ArrayList();
//             m_sg = new SectionGrid(this, splitContainer.Panel1);
// 
//             //m_sg.AddRowData("aaa");
//             //m_sg.AddDataSource();
// 
//             m_arrSections.Add(m_sg);
//             //AddSection(m_arrSections);
// 
//             AutoAlign();
        }

        private void tsMenuAdd_Click(object sender, EventArgs e)
        {
            if (EditMode)
                AddSection(TeamMode);
            SectionIndex++;
        }

        private void tsMenuTeamDel_Click(object sender, EventArgs e)
        {
            if (EditMode)
            {
                DeleteSection(TeamMode > 0 ? TeamMode : m_nWeekday);
            }
        }

        private void tsMenuMemberAdd_Click(object sender, EventArgs e)
        {

        }

        private void tsMenuMemberDel_Click(object sender, EventArgs e)
        {

        }
        
        public void AddRowData(int nType)
        {
            if (m_isOpen && nType == 0) return;
            if(nType == 0)
            {
                int nPrevTeamID = -1;
                SectionGrid prevSection = null;

                foreach (Data_OrganizationHistory data in m_arrOrgani)
                {
                    if (nPrevTeamID == data.RegularTeamID)
                    {
                        prevSection.AddRowData(data.MemberName, "", data.CompanyMemberID, false);
                    }
                    else
                    {
                        // 알수 없는 팀 ID 보유
                        if (!m_dicSections.ContainsKey(data.RegularTeamID))
                            continue;

                        SectionGrid section = m_dicSections[data.RegularTeamID];
                        // AddRow
                        section.AddRowData(data.MemberName, "", data.CompanyMemberID, false);

                        nPrevTeamID = data.RegularTeamID;
                        prevSection = section;
                    }
                }
            }
            else if(nType == 1)
            {
                foreach (Data_NormalHistory data in m_arrNormal)
                {
                    int nDataID = data.ID - this.TempNormalHistoryID;
                    if (nDataID <= 0) continue;

                    if (!m_dicNSections.ContainsKey(nDataID))
                        continue;

                    SectionGrid section = m_dicNSections[nDataID];
                    
                    string[] str = data.RegularTeamLink.Split(',');
                    int nID;
                    for (int i = 0; i < str.Count(); i++)
                    {
                        if (str[i] == "null")
                            nID = 0;
                        else
                        {
                            try
                            {
                                nID = int.Parse(str[i]);
                            }
                            catch (Exception)
                            {
                                nID = 0;
                            }
                        }

                        if (nID == 0)
                            continue;
                        else if (nID < 0) //해당 팀만 포함
                        {
                            foreach (Data_RegularTeam regular in m_arrRegular)
                            {
                                if (regular.ID == (nID * -1))
                                {
                                    //data.RegularTeamLink;
                                    section.AddRowData(regular.TeamName, "", regular.ID, false);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Data_RegularTeam regular in m_arrRegular)
                            {
                                if (regular.ID == nID)
                                {
                                    section.AddRowData(regular.TeamName, "", regular.ID, true);
                                    break;
                                }
                            }
                            
                        }
                    }
                }
            }
            else
            {
                foreach (Data_EmergencyHistory data in m_arrEmergency)
                {
                    int nDataID = data.ID - this.TempEmergencyHistoryID;
                    if (nDataID < 0) continue;

                    if (!m_dicESections.ContainsKey(nDataID))
                        continue;

                    SectionGrid section = m_dicESections[nDataID];

                    string[] str = data.RegularTeamLink.Split(',');
                    int nID;
                    for (int i = 0; i < str.Count(); i++)
                    {
                        if (str[i] == "null")
                            nID = 0;
                        else
                            nID = int.Parse(str[i]);

                        if (nID == 0)
                            continue;
                        else if (nID < 0) //해당 팀만 포함
                        {
                            foreach (Data_RegularTeam regular in m_arrRegular)
                            {
                                if (regular.ID == (nID * -1))
                                {
                                    section.AddRowData(regular.TeamName, "", regular.ID, false);
                                    break;
                                }
                            }
                        }
                        else //해당 팀의 하위 조직을 포함
                        {
                            foreach (Data_RegularTeam regular in m_arrRegular)
                            {
                                if (regular.ID == nID)
                                {
                                    section.AddRowData(regular.TeamName, "", regular.ID, true);
                                    break;
                                }
                            }
                            
                        }
                    }
                }
            }
            
        }

        // nType : RegularTeam(0), 평일비상조직(1), 휴일비상조직(2)
        public ArrayList GetSections(int nType = 0)
        {
            if (nType == 0)
                return m_arrSections;
            else if (nType == 1)
                return m_arrNSections;
            //else if (nType == 2)
            return m_arrESections;
        }
        
        public void AddSection(int nType, bool refresh = false)
        {
            //if (nType == 0 && EditMode == true) return;
            //int nSectionCount = 0;
            Dictionary<int, SectionGrid> dicSections = null;
            SectionGrid section = null;
            Control ctrl = null;
            SectionGrid newSection = null;

            switch (nType)
            {
                //case 0:
                //    ctrl = splitContainer.Panel1;
                //    dicSections = m_dicSections;
                //    break;
                case 1:
                    ctrl = splitContainer1.Panel1;
                    dicSections = m_dicNSections;
                    break;
                case 2:
                    ctrl = splitContainer1.Panel2;
                    dicSections = m_dicESections;
                    break;
            }
            
            if(Weekday)
            {
                ctrl = splitContainer1.Panel1;
                dicSections = m_dicNSections;
                //nSectionCount = NormalCount;
            }
            if (Weekend)
            {
                ctrl = splitContainer1.Panel2;
                dicSections = m_dicESections;
                //nSectionCount = EmergencyCount;
            }

            if(m_isNewProject)
            {
                if (dicSections.Count == 0)
                {
                    //ReadOnly(true);
                    AddSectionParent(ctrl, nType, m_nSectionIndex, "");
                    section = m_sectionGrid;
                    //ReadOnly(false);
                }
                else
                {
                    ArrayList arrSections = null;
                    arrSections = SectionGrid.GetSelectedSections(nType);

                    for (int i = arrSections.Count - 1; i >= 0; i--)
                    {
                        section = (SectionGrid)arrSections[i];
                        break;
                    }
                    //ReadOnly(true);
                    newSection = AddSectionChild(ctrl, dicSections, section, m_nSectionIndex, "");
                    //ReadOnly(false);
                }
            }
            else
            {
                
                ArrayList arrSections = null;
                if(tsbtnBoth.Checked && Weekday == true)
                {
                    nType = 1;
                }
                else if (tsbtnBoth.Checked && Weekend == true)
                {
                    nType = 2;
                }

                arrSections = SectionGrid.GetSelectedSections(nType);

                if (arrSections.Count == 0)
                {
                    //if (dicSections.Count == 0)
                    {
                        int newTeamID = GetMaxTeamID(nType) + 1;
                        //AddSectionParent(ctrl, nType, m_nSectionIndex, "");
                        AddSectionParent(ctrl, nType, newTeamID, "");
                        section = m_sectionGrid;

                        if (EditMode && section != null)
                            section.ReadOnly = false;
                    }
                    /*else
                        return;*/
                }
                else
                {
                    for (int i = arrSections.Count - 1; i >= 0; i--)
                    {
                        section = (SectionGrid)arrSections[i];
                        break;
                    }

                    int newTeamID = GetMaxTeamID(nType) + 1;
                    //nSectionCount++;
                    //ReadOnly(true);

                    SectionGrid.SetAutoRefresh(nType, false);
                    newSection = AddSectionChild(ctrl, dicSections, section, newTeamID, "");
                    SectionGrid.SetAutoRefresh(nType, true);
                    //ReadOnly(false);
                    
                }
            }

            if (EditMode)
            {
                //ReadOnly(!EditMode);
                if (newSection != null)
                {
                    AddTree(newSection);
                    newSection.ReadOnly = false;
                }

                AutoAlign(true, GetSections(nType));
            }

//             if(refresh)
//                 AutoAlign();

        }

        // 새로 생성된 section을 Tree에도 추가시킨다.
        private void AddTree(SectionGrid section)
        {
            //SectionGrid sectionParent = section.GetParentSection();

            //if (sectionParent == null)
            //    m_frmTeamTree.
        }

        private int GetMaxID(string strTableName)
        {
            string strSQL = "select max(id) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            int nMaxID = m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            return nMaxID;
        }

        private int GetMaxTeamID(int nType)
        {
            Dictionary<int, SectionGrid> dicSections = null;

            if (nType == 0)
                dicSections = m_dicSections;
            else if (nType == 1)
                dicSections = m_dicNSections;
            else if (nType == 2)
                dicSections = m_dicESections;
            else
                return 0;

            bool isFirst = true;
            int max = 0;

            foreach (KeyValuePair<int, SectionGrid> pair in dicSections)
            {
                if (isFirst)
                {
                    isFirst = false;
                    max = pair.Key;
                }
                else
                {
                    if (max < pair.Key)
                        max = pair.Key;
                }
            }

            return max;
        }

        public void AddSection(int nType, int nRegularTeamID, int nParentID, string strTeamName)
        {
            if(m_isOpen && nType == 0) return;
            Dictionary<int, SectionGrid> dicSections = null;
            Control ctrl = null;
            switch (nType)
            {
                case 0:
                    ctrl = splitContainer.Panel1;
                    dicSections = m_dicSections;
                    break;
                case 1:
                    ctrl = splitContainer1.Panel1;
                    dicSections = m_dicNSections;
                    break;
                case 2:
                    ctrl = splitContainer1.Panel2;
                    dicSections = m_dicESections;
                    break;
            }

            if (nParentID == 0)
            {
                AddSectionParent(ctrl, nType, nRegularTeamID, strTeamName);
            }
            else
            {
                if (!dicSections.ContainsKey(nParentID))
                    return;

                SectionGrid section = dicSections[nParentID];

                AddSectionChild(ctrl, dicSections, section, nRegularTeamID, strTeamName);
            }
        }

        private void AddSectionParent(Control ctrl, int nType, int nTeamID, string strTeamName, bool refresh = false)
        {
            SectionGrid section = null;

            if (nType == 0)
            {
                section = new SectionGrid(this, ctrl, 2, nType);
                section.Tag = nTeamID;
                section.EditTextBoxData(strTeamName);

                m_arrSections.Add(section);
                m_dicSections[nTeamID] = section;
            }
            else if (nType == 1)
            {
                section = new SectionGrid(this, ctrl, 2, nType);
                section.Tag = nTeamID;
                section.EditTextBoxData(strTeamName);

                m_arrNSections.Add(section);
                m_dicNSections[nTeamID] = section;
            }
            else
            {
                section = new SectionGrid(this, ctrl, 2, nType);
                section.Tag = nTeamID;
                section.EditTextBoxData(strTeamName);

                m_arrESections.Add(section);
                m_dicESections[nTeamID] = section;
            }

            m_sectionGrid = section;
//             if (refresh)
//                 AutoAlign();
        }

        /*private SectionGrid FindSection(int nTag, SectionGrid parentSection = null)
        {
            ArrayList arrSections = parentSection == null ? m_arrSections : parentSection.GetChildSections();

            foreach (SectionGrid section in arrSections)
            {
                if ((int)section.Tag == nTag)
                    return section;

                SectionGrid result = FindSection(nTag, section);
                if (result != null)
                    return result;
            }

            return null;
        }*/

        private SectionGrid AddSectionChild(Control ctrl, Dictionary<int, SectionGrid> dicSections, SectionGrid section, int nTeamID, string strTeamName, bool refresh = false)
        {
            SectionGrid newSection = null;

            if (!dicSections.ContainsKey(nTeamID))
            {
                int nSectionType = -1;
                Dictionary<int, SectionGrid> _dicSections = null;

                if (ctrl == splitContainer.Panel1)
                {
                    nSectionType = 0;
                    _dicSections = m_dicSections;
                }
                else if (ctrl == splitContainer1.Panel1)
                {
                    nSectionType = 1;
                    _dicSections = m_dicNSections;
                }
                else
                {
                    nSectionType = 2;
                    _dicSections = m_dicESections;
                }

                newSection = new SectionGrid(this, ctrl, 2, nSectionType);
                newSection.Tag = nTeamID;

                Point pt = m_dataGrid.Location;
                Size sz = m_dataGrid.Size;
                m_gridPosition = newSection.Position = new Point(pt.X, pt.Y + sz.Height + 10);

                newSection.EditTextBoxData(strTeamName);
                //newSection.AddRowData(strMemberName, strPhone);
                section.AddChild(newSection);

                _dicSections[nTeamID] = newSection;
            }
            else
                return newSection;
                        
            if (refresh)
                AutoAlign();

            return newSection;
        }

        public void DeleteSection(int nType)
        {
            ArrayList arrSections = null;
            
            if (tsbtnBoth.Checked && Weekday)
                arrSections = SectionGrid.GetSelectedSections(1);
            else if (tsbtnBoth.Checked && Weekend)
                arrSections = SectionGrid.GetSelectedSections(2);
            else
            {
                if (nType == 0) return;

                arrSections = SectionGrid.GetSelectedSections(nType);
            }

            if (arrSections.Count == 0) return;

            if (MessageBox.Show("조직을 삭제하시겠습니까?", "삭제", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                for (int i = arrSections.Count - 1; i >= 0; i--)
                {
                    SectionGrid section = (SectionGrid)arrSections[i];
                    SectionGrid sectionParent = (SectionGrid)section.GetParentSection();

                    if (sectionParent != null)
                    {
                        sectionParent.RemoveChild(section);

                        if (nType == 0)
                        {
                            for (int j = 0; j < RemoveSectionIndex.Count; j++ )
                            {
                                SectionGrid data = (SectionGrid)RemoveSectionIndex[j];
                                m_dicSections.Remove(data.Tag);

                                m_arrRegular.RemoveAt(j);
                            }
                        }
                        else if (nType == 1)
                        {
                            for (int j = 0; j < RemoveSectionIndex.Count; j++ )
                            {
                                SectionGrid data = (SectionGrid)RemoveSectionIndex[j];
                                m_dicNSections.Remove(data.Tag);

                                m_arrNormal.RemoveAt(j);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < RemoveSectionIndex.Count; j++ )
                            {
                                SectionGrid data = (SectionGrid)RemoveSectionIndex[j];
                                m_dicESections.Remove(data.Tag);

                                m_arrEmergency.RemoveAt(j);
                            }                            
                        }
                    }
                    else
                    {
                        section.RemoveAllChild();

                        if (arrSections != null)
                        {
                            if (arrSections.Contains(section))
                                arrSections.Remove(section);
                        }

                        if (nType == 0)
                        {
                            m_arrSections.Clear();
                            m_dicSections.Clear();
                            splitContainer.Panel1.Controls.Clear();
                            splitContainer.Panel1.Refresh();
                        }
                        else if (nType == 1)
                        {
                            //m_arrNSections.Clear();
                            m_arrNSections.Remove(section);
                            //m_dicNSections.Clear();
                            RemoveSectionDictionary(section, m_dicNSections);
                            //splitContainer1.Panel1.Controls.Clear();
                            section.Remove();
                            splitContainer1.Panel1.Refresh();
                        }
                        else
                        {
                            //m_arrESections.Clear();
                            m_arrESections.Remove(section);
                            //m_dicESections.Clear();
                            RemoveSectionDictionary(section, m_dicESections);
                            //splitContainer1.Panel2.Controls.Clear();
                            section.Remove();
                            splitContainer1.Panel2.Refresh();
                        }
                    }
                }
                //AutoAlign();

                if (EditMode)
                    ReadOnly(!EditMode);
            }
        }

        private void RemoveSectionDictionary(SectionGrid section, Dictionary<int, SectionGrid> dicSections)
        {
            ArrayList arrChildSections = section.GetChildSections();

            if (arrChildSections != null)
            {
                foreach (SectionGrid sectionChild in arrChildSections)
                {
                    RemoveSectionDictionary(sectionChild, dicSections);
                }
            }

            dicSections.Remove(section.Tag);
        }

        private void DeleteSection(SectionGrid section)
        {
             SectionGrid sectionParent = (SectionGrid)section.GetParentSection();
 
             if (sectionParent != null)
             {
                 sectionParent.RemoveChild(section);
             }
             else
             {
                 section.RemoveAllChild();

                 ArrayList arrSections = GetCurrentSections();
                 if (arrSections != null)
                 {
                     if (arrSections.Contains(section))
                         arrSections.Remove(section);
                 }
             }

             Invalidate();
        }

        private void CalcVerticalPos(int nVertSpace, int nCurrentPos, ArrayList arrSections = null)
        {
            if (arrSections == null)
                arrSections = m_arrSections;

            int nNextPos = 0;

            foreach (SectionGrid section in arrSections)
            {
                section.Position = new Point(section.Position.X, nCurrentPos);
                int nPos = nCurrentPos + section.Size.Height + nVertSpace;

                if (nPos > nNextPos)
                    nNextPos = nPos;
            }

            foreach (SectionGrid section in arrSections)
            {
                ArrayList arrChilds = section.GetChildSections();
                if (arrChilds != null && arrChilds.Count > 0)
                    CalcVerticalPos(nVertSpace, nNextPos, arrChilds);
            }
        }

        // 화면 스크롤을 고려하지 않은 상태에서 수평 위치 계산
        private int CalcHorizontalPos(int nHorzSpace, int nCurrentPos, ArrayList arrSections = null)
        {
            if (arrSections == null)
                arrSections = m_arrSections;

            foreach (SectionGrid section in arrSections)
            {
                section.ChildBegin = nCurrentPos;
                section.ChildEnd = nCurrentPos + section.Size.Width;

                ArrayList arrChilds = section.GetChildSections();

                if (arrChilds != null && arrChilds.Count > 0)
                    nCurrentPos = CalcHorizontalPos(nHorzSpace, nCurrentPos, arrChilds);
                else
                    nCurrentPos = section.ChildEnd + nHorzSpace;
            }

            return nCurrentPos;
        }

        // 화면 스크롤을 고려하여 수평 위치를 재계산
        private void CalcHorizontalPos2(ArrayList arrSections, int nScrollPos = -1)
        {
            if (arrSections == null)
                arrSections = m_arrSections;

            if (nScrollPos < 0)
            {
                if (arrSections.Count == 0)
                    return;

                SectionGrid section = (SectionGrid)arrSections[0];
                nScrollPos = GetScrollPos(section.SectionType);

                // 스크롤이 일어나지 않았으면 재계산할 필요 없음
                if (nScrollPos == 0)
                    return;
            }

            foreach (SectionGrid section in arrSections)
            {
                Point pt = section.Position;
                section.Position = new Point(pt.X - nScrollPos, pt.Y);

                CalcHorizontalPos2(section.GetChildSections(), nScrollPos);
            }
        }

        // nSectionType : RegularTeam(0), 평일비상조직(1), 휴일비상조직(2)
        public void SetAutoAlignRefresh(int nSectionType, bool refresh)
        {
            SectionGrid.SetAutoRefresh(nSectionType, refresh);
        }

        public void AutoAlign(bool refresh = true, ArrayList arrSections = null)
        {
            // Tree 깊이별 Section 위치
            CalcVerticalPos(SectionGrid.VertSpace, 15, arrSections);
            CalcHorizontalPos(SectionGrid.HorzSpace, 15, arrSections);
            CalcHorizontalPos2(arrSections);

            if (refresh)
                Refresh();
        }

        public void AllSectionClearSelect(int nSectionType)
        {
            ArrayList arrSections = null;

            if (nSectionType == 0)
                arrSections = m_arrSections;
            else if (nSectionType == 1)
                arrSections = m_arrNSections;
            else if (nSectionType == 2)
                arrSections = m_arrESections;

            foreach (SectionGrid section in arrSections)
            {
                section.Select(false, true);
            }
        }

        public bool SelectSection(int nType, int x, int y, string strTeamName, int nSectionID)
        {
//             AllSectionClearSelect(nType);
//             Refresh();
//             return false;
            ArrayList arrSections = null;
            if(nType == 0)
            {
                arrSections = m_arrSections;
            }
            else if (nType == 1)
            {
                arrSections = m_arrNSections;
            }
            else
            {
                arrSections = m_arrESections;
            }
            
            int nSectionCount = arrSections.Count;

            for (int i = nSectionCount - 1; i >= 0; i--)
            {
                SectionGrid section = (SectionGrid)arrSections[i];
                SectionGrid selectedSection = section.Select(x, y);
                if (selectedSection != null)
                {
                    selectedSection.AddRowData(strTeamName, "", nSectionID, false);
                    Refresh();
                    return true;
                }
            }
// 
//             if (nSectionCount > 0)
//                 Refresh();

            return false;
        }
        
        private ArrayList GetCurrentSections()
        {
            return null;
        }
        
        public void ReadOnly(bool isValue)
        {
            //SetAutoAlignRefresh(0, false);
            SetAutoAlignRefresh(1, false);
            SetAutoAlignRefresh(2, false);

            //foreach (SectionGrid section in m_arrSections)
            //{
            //    if (section.ReadOnly == isValue)
            //    {
            //        SetAutoAlignRefresh(0, true);
            //        SetAutoAlignRefresh(1, true);
            //        SetAutoAlignRefresh(2, true);
            //        return;
            //    }

            //    section.ReadOnly = isValue;
            //}

            foreach (SectionGrid section in m_arrNSections)
            {
                section.ReadOnly = isValue;
            }

            foreach (SectionGrid section in m_arrESections)
            {
                section.ReadOnly = isValue;
            }

            //SetAutoAlignRefresh(0, true);
            SetAutoAlignRefresh(1, true);
            SetAutoAlignRefresh(2, true);

            //AutoAlign(true, m_arrSections);
            AutoAlign(true, m_arrNSections);
            AutoAlign(true, m_arrESections);

        }

        private void splitContainer_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AllSectionClearSelect(0);
                Refresh();
            }
        }

        private void splitContainer_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (m_arrSections == null) return;
            foreach (SectionGrid section in m_arrSections)
            {
                section.DrawSection(e.Graphics);
            }
        }

        private void splitContainer_Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            splitContainer.Panel1.Refresh();
        }
        
        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AllSectionClearSelect(1);
                Refresh();
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (m_arrNSections == null) return;
            foreach (SectionGrid section in m_arrNSections)
            {
                section.DrawSection(e.Graphics);
            }
        }

        private void splitContainer1_Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            splitContainer1.Panel1.Refresh();
        }

        private void splitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AllSectionClearSelect(2);
                Refresh();
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (m_arrESections == null) return;
            foreach (SectionGrid section in m_arrESections)
            {
                section.DrawSection(e.Graphics);
            }
        }

        private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e)
        {
            splitContainer1.Panel2.Refresh();
        }

        public SplitterPanel GetVeiw1()
        {
            return splitContainer.Panel1;
        }

        public SplitterPanel GetVeiw2()
        {
            return splitContainer.Panel1;
        }

        public SplitterPanel GetVeiw3()
        {
            return splitContainer.Panel1;
        }

        public void FoucsPanel(int nIndex)
        {
            if(nIndex == 0)
                splitContainer.Panel1.Focus();
            else if(nIndex == 1)
                splitContainer1.Panel1.Focus();
            else if (nIndex == 2)
                splitContainer1.Panel2.Focus();
        }

        public ArrayList FindTeam(int nTeamMode, string strTeamName)
        {
            ArrayList arrTeamInfo = new ArrayList();

            if (nTeamMode == 1)
            {
                foreach(Data_NormalHistory data in NormalTeam)
                {
                    if(data.TeamName == strTeamName)
                    {
                        arrTeamInfo.Add(data);
                        return arrTeamInfo;
                    }
                }
            }
            else if (nTeamMode == 2)
            {
                foreach (Data_EmergencyHistory data in EmergencyTeam)
                {
                    if (data.TeamName == strTeamName)
                    {
                        arrTeamInfo.Add(data);
                        return arrTeamInfo;
                    }
                }
            }
            return null;
        }

        public string FindTeamLeader(int nRegularTeamID)
        {
            ArrayList arr = new ArrayList();
            string strTeam = "";

            foreach (KeyValuePair<int, Data_OrganizationHistory> pair in m_dicMember)
            {
                if (pair.Value.RegularTeamID == nRegularTeamID)
                {
                    if (pair.Value.PositionID >= 2 && pair.Value.PositionID <= 7)
                    {
                        strTeam = pair.Value.MemberName;
                        arr.Add(pair.Value);

                        return strTeam;
                    }
                }
            }

            return null;
        }

        private ArrayList FindDicValue(Dictionary<int, Data_OrganizationHistory> dic, int nRegularTeamID)
        {
            ArrayList arr = new ArrayList();

            foreach (KeyValuePair<int, Data_OrganizationHistory> pair in dic)
            {
                if (pair.Value.RegularTeamID == nRegularTeamID)
                {
                    arr.Add(pair.Value);
                }
            }

            return arr;
        }

        // nID에 해당하는 Member를 검색하여 리턴한다.
        public Data_OrganizationHistory FindMember(int nID)
        {
             if (TeamMode != 0)
             {
                 ArrayList arr = FindDicValue(m_dicMember, nID);
                 nID = 0;
                 int nPositionID = 0;
                 foreach (Data_OrganizationHistory data in arr)
                 {
                     if (data.PositionID >= 2 && data.PositionID <= 7)
                     {
                         if (nPositionID < data.PositionID)
                         {
                             nPositionID = data.PositionID;
                             nID = data.CompanyMemberID;
                         }
                     }
                 }
             }

            if (m_dicMember.ContainsKey(nID))
                return m_dicMember[nID];

            return null;
        }

        public Data_NormalHistory FindNormal(int nID)
        {
            if (m_dicMember.ContainsKey(nID))
                return m_dicNormal[nID];

            return null;
        }

        public Data_EmergencyHistory FindEmergency(int nID)
        {
            if (m_dicMember.ContainsKey(nID))
                return m_dicEmergency[nID];

            return null;
        }

        // strTeamName에 해당하는 Section을 모두 검색하여 ArrayList에 담아 리턴한다.
        public ArrayList FindSection(string strTeamName, int nSectionType)
        {
            ArrayList arrSections = new ArrayList();
            Dictionary<int, SectionGrid> dicSections = null;

            if (nSectionType == 0)
                dicSections = m_dicSections;
            else if (nSectionType == 1)
                dicSections = m_dicNSections;
            else if (nSectionType == 2)
                dicSections = m_dicESections;
            else
                return arrSections;

            foreach (KeyValuePair<int, SectionGrid> pair in dicSections)
            {
                if (pair.Value.GetTitle() == strTeamName)
                {
                    arrSections.Add(pair.Value);
                }
            }

            return arrSections;
        }

        // nTeamID에 해당하는 Section을 리턴한다.
        public SectionGrid FindSection(int nTeamID, int nSectionType)
        {
            Dictionary<int, SectionGrid> dicSections = null;

            if (nSectionType == 0)
                dicSections = m_dicSections;
            else if (nSectionType == 1)
                dicSections = m_dicNSections;
            else if (nSectionType == 2)
                dicSections = m_dicESections;
            else
                return null;

            if (!dicSections.ContainsKey(nTeamID))
                return null;

            return dicSections[nTeamID];
        }

        public void Save_OrganizationChart(int nVersionID, string strVersionName, string strDescription)
        {
            SaveVersion(nVersionID, strVersionName, strDescription);

            SaveTemporaryNormal(nVersionID);
            SaveTemporaryEmergency(nVersionID);
        }

        public void Save_OrganizationChart()
        {
            int r = m_dicSections.Count;
            int e = m_dicESections.Count;
            int n = m_dicNSections.Count;

            int nVersionID = SaveVersion();
            
            SaveTemporaryNormal(nVersionID);
            SaveTemporaryEmergency(nVersionID);
        }

        public void SaveVersion(int nVersionID, string strVersionName, string strDescription)
        {
            DateTime dt = DateTime.Now;
            string strCreateTime = dt.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", dt.Hour, dt.Minute, dt.Second);

            string strSQL = string.Format("INSERT INTO TeamVersion(ID, VersionName, GenUserID, CreateTime, Description) VALUES ({0}, '{1}', {2}, '{3}', '{4}')", nVersionID, strVersionName, LoginID, strCreateTime, strDescription);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        public int SaveVersion()
        {
            int nID = 0;
            //double nOld = 0;
            DateTime dt = DateTime.Now;
            /*foreach (Data_TeamVersion data in TeamVersion)
            {
                nID = data.VersionID;
                string strTemp = System.Text.RegularExpressions.Regex.Replace(data.VersionName, @"\D", "");
                nOld = double.Parse(strTemp);

                break;
            }*/

            Data_TeamVersion data = null;

            if (TeamVersion.Count > 0)
            {
                 data = (Data_TeamVersion)TeamVersion[0];
                 nID = data.VersionID;
            }

            //nOld++;
            //double nNew = nOld / 10;
            //string strNewVersion = "v" + nNew.ToString();
            string strNewVersion = data == null ? "v1.0" : NewVersionName(data.VersionName);
            string strDescription = "";
            string strCreateTime = dt.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", dt.Hour, dt.Minute, dt.Second);
            nID++;
            string strSQL = string.Format("INSERT INTO TeamVersion(ID, VersionName, GenUserID, CreateTime, Description) VALUES ({0}, '{1}', {2}, '{3}', '{4}')", nID, strNewVersion, LoginID, strCreateTime, strDescription);
            m_dbMgr.GetResultData(strSQL, 1);

            return nID;
        }

        public static string NewVersionName(string strLastVersionName)
        {
            double num = 0.0;
            bool isDot = false;
            int nCount = 0, nCount2 = 0;

            string strHeader = "";
            int nLen = strLastVersionName.Length;

            for (int i = nLen - 1; i >= 0; i--)
            {
                char ch = strLastVersionName[i];

                if (char.IsDigit(ch))
                {
                    num += Math.Pow(10.0, nCount) * int.Parse(ch.ToString());
                    nCount++;
                }
                else if (ch == '.')
                {
                    if (isDot)
                    {
                        strHeader = strLastVersionName.Substring(0, i + 1);
                        break;
                    }
                    else
                        isDot = true;

                    if (nCount == 0)
                    {
                        strHeader = strLastVersionName.Substring(0, i + 1);
                        break;
                    }

                    num = num / Math.Pow(10.0, nCount);

                    // 소수점 아래 자리수
                    nCount2 = nCount;
                    nCount = 0;
                }
                else
                {
                    strHeader = strLastVersionName.Substring(0, i + 1);
                    break;
                }
            }

            if (nCount == 0 && nCount2 == 0)
                return "V1.0";

            if (nCount2 == 0)
                return string.Format("{0}{1}", strHeader, num + 1);

            string strFormat = "{0}{1:F" + nCount2.ToString() + "}";
            return string.Format(strFormat, strHeader, num + 1.0 / Math.Pow(10.0, nCount2));
        }

        // Tree 형태로 담겨있는 arrSections의 Section들을 arrTempSections에 일렬로 집어넣는다.
        private void ArrangeSections(ArrayList arrSections, ref ArrayList arrTempSections)
        {
            if (arrSections == null)
                return;

            foreach (SectionGrid section in arrSections)
            {
                arrTempSections.Add(section);
                ArrangeSections(section.GetChildSections(), ref arrTempSections);
            }
        }

        private int GetMaxHistoryID(int nType)
        {
            string strTableName = "";
            
            if (nType == 1)
                strTableName = "TemporaryNormalTeamHistory";
            else if (nType == 2)
                strTableName = "TemporaryEmergencyTeamHistory";
            else
                return 0;

            string strSQL = "select max(id) from " + strTableName;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult.Count == 0)
                return 0;

            int nMaxID = m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            return nMaxID;
        }

        public void SaveTemporaryNormal(int nTeamVersionID)
        {
            SaveTemporaryTeams("TemporaryNormalTeam", m_arrNSections, nTeamVersionID);
        }

        //public void SaveTemporaryNormal(int nTeamVersionID)
        //{
        //    int nMaxNormalID = GetMaxHistoryID(1);
        //    int nHistoryID = nMaxNormalID;
        //    int nID = 0;

        //    RemoveTemporaryNormal();

        //    // Tree 형태로 담겨있는 arrSections의 Section들을 arrTempSections에 일렬로 집어넣는다.
        //    ArrayList arrTempSections = new ArrayList();
        //    ArrangeSections(m_arrNSections, ref arrTempSections);

        //    // Section별 History ID를 기록
        //    Dictionary<SectionGrid, int> dicSectionHistory = new Dictionary<SectionGrid, int>();

        //    //for (int i = 1; i < m_dicNSections.Count + 1; i++)
        //    //foreach (KeyValuePair<int, SectionGrid> pair in m_dicNSections)
        //    foreach (SectionGrid sectionCurrent in arrTempSections)
        //    {
        //        string strGroupName = "NULL", strDescription = "NULL", strRegularTeamLink = "";

        //        //SectionGrid sectionCurrent = pair.Value;

        //        //////////////////////////////////////////////////////////////////////////
        //        // NormalTeam
        //        nID++; //ID
        //        nHistoryID++; // TemporaryNormalTeamID
        //        //string strTeamName = m_dicNSections[i].GetTitle(); //TeamName
        //        //SectionGrid section = m_dicNSections[i].GetSectionParent();
        //        string strTeamName = sectionCurrent.GetTitle(); //TeamName
        //        SectionGrid section = sectionCurrent.GetSectionParent();
        //        int nParentTeamID = -1, nParentTeamID2 = -1;
        //        if (section != null)
        //        {
        //            //nParentTeamID = (int)section.Tag; //ParentTeamID
        //            //nParentTeamID2 = (int)section.Tag + nMaxNormalID;
        //            if (!dicSectionHistory.ContainsKey(section))
        //                return;

        //            nParentTeamID = dicSectionHistory[section] - nMaxNormalID;
        //            nParentTeamID2 = nParentTeamID + nMaxNormalID;
        //        }
        //        //DataGridView dataGrid = m_dicNSections[i].GetDataGrid();
        //        DataGridView dataGrid = sectionCurrent.GetDataGrid();

        //        //if (dataGrid.RowCount - 1 > 0)
        //        //    strRegularTeamLink = "'";

        //        int nRow = 0, nRegularTeam = 0;
        //        foreach (DataGridViewRow row in dataGrid.Rows)
        //        {
        //            if (EditMode && nRow == dataGrid.RowCount - 1)
        //                break;

        //            string strCellValue = row.Cells[0].Value.ToString();
        //            if (strCellValue.Length == 0)
        //                continue;

        //            if ((bool)row.Cells[1].Value == true)
        //                nRegularTeam = (int)row.Cells[0].Tag;
        //            else
        //                nRegularTeam = (int)row.Cells[0].Tag * -1;

        //            strRegularTeamLink += nRegularTeam.ToString();

        //            nRow++;
        //            if ((EditMode && nRow != dataGrid.RowCount - 1) || (!EditMode && nRow != dataGrid.RowCount))
        //                strRegularTeamLink += ", ";

        //        }

        //        if (strRegularTeamLink.Length > 0)
        //            strRegularTeamLink = "'" + strRegularTeamLink + "'";
        //        else
        //            strRegularTeamLink = "NULL";

        //        dicSectionHistory[sectionCurrent] = nHistoryID;

        //        //////////////////////////////////////////////////////////////////////////
        //        // NormalTeamHistory
        //        string strSQLHistory = string.Format("INSERT INTO TemporaryNormalTeamHistory(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, TeamVersionID) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7})", nHistoryID, strTeamName, nParentTeamID2 >= 0 ? nParentTeamID2.ToString() : "NULL", strGroupName, "NULL", strDescription, strRegularTeamLink, nTeamVersionID);
        //        m_dbMgr.GetResultData(strSQLHistory, 1);

        //        string strSQL = string.Format("INSERT INTO TemporaryNormalTeam(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})", nID, strTeamName, nParentTeamID >= 0 ? nParentTeamID.ToString() : "NULL", strGroupName, "NULL", strDescription, strRegularTeamLink);
        //        m_dbMgr.GetResultData(strSQL, 1);    
        //    }
        //    SaveTeamLevel(nID, true);
        //}

        private NETeam FindTeam(int nID, Dictionary<NETeam, int> dicTeams, NETeam teamExcept)
        {
            foreach (KeyValuePair<NETeam, int> pair in dicTeams)
            {
                if (pair.Key == teamExcept)
                    continue;

                if (pair.Key.ID == nID)
                    return pair.Key;
            }

            return null;
        }

        private ArrayList LoadTeams(string strTableName)
        {
            string strSQL = "select id, TeamName, ParentTeamID from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            // Team별 부모팀의 ID
            Dictionary<NETeam, int> dicTeams = new Dictionary<NETeam, int>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                NETeam team = new NETeam();
                team.ID = nID;
                team.TeamName = strTeamName;

                dicTeams[team] = nParentTeamID;
            }

            // 최상위 팀들만 담는다.
            ArrayList arrTeams = new ArrayList();

            foreach (KeyValuePair<NETeam, int> pair in dicTeams)
            {
                NETeam teamParent = pair.Value < 0 ? null : FindTeam(pair.Value, dicTeams, pair.Key);

                if (teamParent == null)
                    arrTeams.Add(pair.Key);
                else
                {
                    teamParent.ChildTeams.Add(pair.Key);
                    pair.Key.ParentTeam = teamParent;
                }
            }

            return arrTeams;
        }

        private NETeam FindTeam(SectionGrid section, ArrayList arrDBTeams, Dictionary<SectionGrid, int> dicUpdateIDs, Dictionary<SectionGrid, int> dicInsertIDs, ref int nMaxID, bool useDBTeams = true)
        {
            SectionGrid sectionParent = section.GetParentSection();
            NETeam teamResult = null;

            if (sectionParent == null)
            {
                foreach (NETeam team in arrDBTeams)
                {
                    if (team.TeamName == section.GetTitle())
                    {
                        dicUpdateIDs[section] = team.ID;
                        teamResult = team;
                        break;
                    }
                }
            }
            else
            {
                if (dicUpdateIDs.ContainsKey(sectionParent))
                {
                    int nParentTeamID = dicUpdateIDs[sectionParent];
                    NETeam team = useDBTeams ? FindTeam(nParentTeamID, arrDBTeams) : null;

                    if (team != null)
                    {
                        foreach (NETeam _team in team.ChildTeams)
                        {
                            if (_team.TeamName == section.GetTitle())
                            {
                                dicUpdateIDs[section] = _team.ID;
                                teamResult = _team;
                                break;
                            }
                        }
                    }
                }
            }

            if (teamResult == null)
                dicInsertIDs[section] = ++nMaxID;

            ArrayList arrChildSections = section.GetChildSections();
            useDBTeams = teamResult != null;

            foreach (SectionGrid sectionChild in arrChildSections)
            {
                FindTeam(sectionChild, arrDBTeams, dicUpdateIDs, dicInsertIDs, ref nMaxID, useDBTeams);
            }

            return teamResult;
        }

        private NETeam FindTeam(int nID, ArrayList arrTeams)
        {
            foreach (NETeam team in arrTeams)
            {
                if (nID == team.ID)
                    return team;

                NETeam _team = FindTeam(nID, team.ChildTeams);
                if (_team != null)
                    return _team;
            }

            return null;
        }

        private string GetSpecialTeamListString(ArrayList arrDBTeams, string strTableName, out string strInsertSQL, ref int nMaxID)
        {
            strInsertSQL = "";

            string strSpecialTeamIDs = "";
            bool[] arrLevel = new bool[10] { false, false, false, false, false, false, false, false, false, false };
            bool entireMember = false;

            foreach (NETeam team in arrDBTeams)
            {
                if (team.TeamName == "1급")
                    arrLevel[0] = true;
                else if (team.TeamName == "2급")
                    arrLevel[1] = true;
                else if (team.TeamName == "3급")
                    arrLevel[2] = true;
                else if (team.TeamName == "4급")
                    arrLevel[3] = true;
                else if (team.TeamName == "5급")
                    arrLevel[4] = true;
                else if (team.TeamName == "6급")
                    arrLevel[5] = true;
                else if (team.TeamName == "7급")
                    arrLevel[6] = true;
                else if (team.TeamName == "8급")
                    arrLevel[7] = true;
                else if (team.TeamName == "9급")
                    arrLevel[8] = true;
                else if (team.TeamName == "전직원")
                    entireMember = true;
                else
                    continue;

                if (strSpecialTeamIDs.Length == 0)
                    strSpecialTeamIDs = "(" + team.ID.ToString();
                else
                    strSpecialTeamIDs += ", " + team.ID.ToString();
            }

            if (strSpecialTeamIDs.Length > 0)
                strSpecialTeamIDs += ")";

            for (int i = 1; i < 10; i++)
            {
                if (!arrLevel[i])
                {
                    strInsertSQL += string.Format("Insert into {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) values ({1}, '{2}급', NULL, NULL, {2}, NULL, NULL);",
                        strTableName, ++nMaxID, i);
                }
            }

            if (!entireMember && m_arrSections.Count > 0)
            {
                SectionGrid sectionRoot = (SectionGrid)m_arrSections[0];
                int nRegularTeamID = (int)sectionRoot.Tag;

                strInsertSQL += string.Format("Insert into {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) values ({1}, '전직원', NULL, NULL, NULL, NULL, '{2}');",
                    strTableName, ++nMaxID, nRegularTeamID);
            }

            return strSpecialTeamIDs;
        }

        // dicTeamIDs : Section별 비상조직 ID
        private void SaveTeamList(string strTableName, Dictionary<SectionGrid, int> dicTeamIDs, Dictionary<SectionGrid, int> dicTeam2IDs, int nMaxHistoryID, int nTeamVersionID)
        {
            foreach (KeyValuePair<SectionGrid, int> pair in dicTeamIDs)
            {
                SectionGrid section = pair.Key;
                SectionGrid sectionParent = section.GetParentSection();
                int nParentTeamID = -1;

                int nID = pair.Value;
                string strTeamName = section.GetTitle();

                if (sectionParent != null)
                {
                    if (dicTeamIDs.ContainsKey(sectionParent))
                        nParentTeamID = dicTeamIDs[sectionParent];
                    else if (dicTeam2IDs.ContainsKey(sectionParent))
                        nParentTeamID = dicTeam2IDs[sectionParent];
                }

                DataGridView dataGrid = section.GetDataGrid();

                int nRow = 0, nRegularTeam = 0;
                string strRegularTeamLink = "";

                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    if (EditMode && nRow == dataGrid.RowCount - 1)
                        break;

                    string strCellValue = row.Cells[0].Value.ToString();
                    if (strCellValue.Length == 0)
                        continue;

                    if ((bool)row.Cells[1].Value == true)
                        nRegularTeam = (int)row.Cells[0].Tag;
                    else
                        nRegularTeam = (int)row.Cells[0].Tag * -1;

                    if (strRegularTeamLink.Length == 0)
                        strRegularTeamLink = nRegularTeam.ToString();
                    else
                        strRegularTeamLink += ", " + nRegularTeam.ToString();

                    nRow++;
                }

                if (strRegularTeamLink.Length > 0)
                    strRegularTeamLink = "'" + strRegularTeamLink + "'";
                else
                    strRegularTeamLink = "NULL";

                //////////////////////////////////////////////////////////////////////////
                // EmergencyTeamHistory
                string strSQLHistory = string.Format("INSERT INTO {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, TeamVersionID) VALUES ({1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8})",
                    strTableName + "History", nID + nMaxHistoryID, strTeamName, nParentTeamID >= 0 ? (nParentTeamID + nMaxHistoryID).ToString() : "NULL", "NULL", "NULL", "NULL", strRegularTeamLink, nTeamVersionID);
                m_dbMgr.GetResultData(strSQLHistory, 0);

                string strSQL = string.Format("insert into {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) values ({1}, '{2}', {3}, NULL, NULL, NULL, {4})",
                    strTableName, nID, strTeamName, nParentTeamID < 0 ? "NULL" : nParentTeamID.ToString(), strRegularTeamLink);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }
        }

        private void SaveTemporaryTeams(string strTableName, ArrayList arrSections, int nTeamVersionID)
        {
            int nMaxID = GetMaxID(strTableName);

            // DB에 현재 담겨있는 조직 정보
            ArrayList arrDBTeams = LoadTeams(strTableName);
            if (arrDBTeams == null)
                return;

            // Section별 비상조직 ID
            Dictionary<SectionGrid, int> dicUpdateIDs = new Dictionary<SectionGrid, int>();
            Dictionary<SectionGrid, int> dicInsertIDs = new Dictionary<SectionGrid, int>();

            foreach (SectionGrid section in arrSections)
            {
                FindTeam(section, arrDBTeams, dicUpdateIDs, dicInsertIDs, ref nMaxID);
            }

            // 기존 조직 Data 삭제
            string strSpecialInsertSQL;
            string strSpecialTeamIDs = GetSpecialTeamListString(arrDBTeams, strTableName, out strSpecialInsertSQL, ref nMaxID);
            string strSQL = "Delete from " + strTableName;

            if (strSpecialTeamIDs.Length > 0)
                strSQL += " where id not in " + strSpecialTeamIDs;

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;
            ///////////////////////////////////////////////////////////////

            int nMaxHistoryID = GetMaxID(strTableName + "History");

            // 편집된 조직 Data 저장
            SaveTeamList(strTableName, dicUpdateIDs, dicInsertIDs, nMaxHistoryID, nTeamVersionID);
            SaveTeamList(strTableName, dicInsertIDs, dicUpdateIDs, nMaxHistoryID, nTeamVersionID);
            ///////////////////////////////////////////////////////////////

            // 1급 ~ 9급, 전직원 가운데 DB에 아직 입력되지 않은것이 있으면 추가해준다.
            if (strSpecialInsertSQL.Length == 0)
            {
                if (m_dbMgr.GetResultData(strSpecialInsertSQL, 0) == null)
                    return;
            }
            ///////////////////////////////////////////////////////////////
        }
        
        public void SaveTemporaryEmergency(int nTeamVersionID)
        {
            SaveTemporaryTeams("TemporaryEmergencyTeam", m_arrESections, nTeamVersionID);
        }

        //public void SaveTemporaryEmergency(int nTeamVersionID)
        //{
        //    int nMaxEmergencyID = GetMaxHistoryID(2);
        //    int nHistoryID = nMaxEmergencyID;
        //    int nID = 0;
        //    RemoveTemporaryEmergency();

        //    // Tree 형태로 담겨있는 arrSections의 Section들을 arrTempSections에 일렬로 집어넣는다.
        //    ArrayList arrTempSections = new ArrayList();
        //    ArrangeSections(m_arrESections, ref arrTempSections);

        //    // Section별 History ID를 기록
        //    Dictionary<SectionGrid, int> dicSectionHistory = new Dictionary<SectionGrid, int>();
            
        //    //for (int i = 1; i < m_dicESections.Count + 1; i++)
        //    //foreach (KeyValuePair<int, SectionGrid> pair in m_dicESections)
        //    foreach (SectionGrid sectionCurrent in arrTempSections)
        //    {
        //        string strGroupName = "NULL", strDescription = "NULL", strRegularTeamLink = "";

        //        //SectionGrid sectionCurrent = pair.Value;

        //        //////////////////////////////////////////////////////////////////////////
        //        // EmergencyTeam
        //        //int nID = (int)m_dicESections[i].Tag; //ID
        //        //nHistoryID++; // TemporaryEmergencyTeamID
        //        //string strTeamName = m_dicESections[i].GetTitle(); //TeamName
        //        //SectionGrid section = m_dicESections[i].GetSectionParent();
        //        //int nID = _sec.Tag; //ID
        //        nID++;
        //        nHistoryID++; // TemporaryEmergencyTeamID
        //        string strTeamName = sectionCurrent.GetTitle(); //TeamName
        //        SectionGrid section = sectionCurrent.GetSectionParent();
        //        int nParentTeamID = -1, nParentTeamID2 = -1;
        //        if (section != null)
        //        {
        //            //nParentTeamID = (int)section.Tag; //ParentTeamID
        //            //nParentTeamID2 = (int)section.Tag + nMaxEmergencyID;
        //            if (!dicSectionHistory.ContainsKey(section))
        //                return;

        //            nParentTeamID = dicSectionHistory[section] - nMaxEmergencyID;
        //            nParentTeamID2 = nParentTeamID + nMaxEmergencyID;
        //        }
        //        DataGridView dataGrid = sectionCurrent.GetDataGrid();
        //        /*if (dataGrid.RowCount - 1 > 0)
        //            strRegularTeamLink = "'";*/

        //        int nRow = 0, nRegularTeam = 0;
        //        foreach (DataGridViewRow row in dataGrid.Rows)
        //        {
        //            if (EditMode && nRow == dataGrid.RowCount - 1)
        //                break;

        //            string strCellValue = row.Cells[0].Value.ToString();
        //            if (strCellValue.Length == 0)
        //                continue;

        //            if ((bool)row.Cells[1].Value == true)
        //                nRegularTeam = (int)row.Cells[0].Tag;
        //            else
        //                nRegularTeam = (int)row.Cells[0].Tag * -1;

        //            strRegularTeamLink += nRegularTeam.ToString();

        //            nRow++;
        //            if ((EditMode && nRow != dataGrid.RowCount - 1) || (!EditMode && nRow != dataGrid.RowCount))
        //                strRegularTeamLink += ", ";

        //        }

        //        if (strRegularTeamLink.Length > 0)
        //            strRegularTeamLink = "'" + strRegularTeamLink + "'";
        //        else
        //            strRegularTeamLink = "NULL";

        //        dicSectionHistory[sectionCurrent] = nHistoryID;

        //        //////////////////////////////////////////////////////////////////////////
        //        // EmergencyTeamHistory
        //        string strSQLHistory = string.Format("INSERT INTO TemporaryEmergencyTeamHistory(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, TeamVersionID) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7})", nHistoryID, strTeamName, nParentTeamID2 >= 0 ? nParentTeamID2.ToString() : "NULL", strGroupName, "NULL", strDescription, strRegularTeamLink, nTeamVersionID);
        //        m_dbMgr.GetResultData(strSQLHistory, 1);

        //        string strSQL = string.Format("INSERT INTO TemporaryEmergencyTeam(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})", nID, strTeamName, nParentTeamID >= 0 ? nParentTeamID.ToString() : "NULL", strGroupName, "NULL", strDescription, strRegularTeamLink);
        //        m_dbMgr.GetResultData(strSQL, 1);

        //    }
        //    SaveTeamLevel(nID, false);
        //}

        private void SaveTeamLevel(int nID, bool isCheck)
        {
            string strSQL = "";
            for (int i=1; i>10; i++)
            {
                nID++;
                string strTeamName = i.ToString() + "급";
                if(isCheck)
                    strSQL = string.Format("INSERT INTO TemporaryNormalTeam(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})", nID, strTeamName, "NULL", i, "NULL", "NULL");
                else
                    strSQL = string.Format("INSERT INTO TemporaryEmergencyTeam(ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})", nID, strTeamName, "NULL", i, "NULL", "NULL");
                m_dbMgr.GetResultData(strSQL, 1);
            }
        }

        private void RemoveTemporaryNormal()
        {
            string strSQL = string.Format("DELETE FROM TemporaryNormalTeam");
            m_dbMgr.GetResultData(strSQL, 1);
        }

        private void RemoveTemporaryEmergency()
        {
            string strSQL = string.Format("DELETE FROM TemporaryEmergencyTeam" );
            m_dbMgr.GetResultData(strSQL, 1);
        }

        public int TempNormalHistoryID
        {
            get { return m_nTemporaryNormalTeamHistoryID; }
            set { m_nTemporaryNormalTeamHistoryID = value; }
        }

        public int TempEmergencyHistoryID
        {
            get { return m_nTemporaryEmergencyTeamHistoryID; }
            set { m_nTemporaryEmergencyTeamHistoryID = value; }
        }
    }
}
