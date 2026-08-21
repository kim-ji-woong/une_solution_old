using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPManager
{
	public partial class FormPageSOP : Form, Sections.ISectionListener
	{
		public FormPageSOP()
		{
			InitializeComponent();

			

			TopLevel = false;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			BackColor = Color.FromArgb(227, 226, 226);

			CreateShortcutBar();
            InitPropertiesLevel();
            InitPropertiesProcess();
            InitPropertiesDicision();
            InitPropertiesAnnotation();
            InitPropertiesEndPoint();
            InitPropertiesLink();
            InitPropertiesTransSOP();
            InitPropertiesInternal();
            InitPropertiesExternal();
            InitPropertiesTransmission();

			InitPropertiesGroup();

            this.MouseWheel += new MouseEventHandler(PageBackstageLevel_MouseWheel);

            
		}
	

        private BarComponent m_barComponent = null;
        private BarLevelTree m_barLevelTree = null;
        private BarPage m_barPage = null;

        private PropertiesLevel m_propertiesLevel = new PropertiesLevel();

        private PropertiesProcess m_propertiesProcess = new PropertiesProcess();
        private PropertiesDecision m_propertiesDecision = new PropertiesDecision();
        private PropertiesAnnotation m_propertiesAnnotation = new PropertiesAnnotation();
        private PropertiesEndPoint m_propertiesEndPoint = new PropertiesEndPoint();
        private PropertiesLink m_propertiesLink = new PropertiesLink();
        private PropertiesTransSOP m_propertiesTransSOP = new PropertiesTransSOP();
		private PropertiesInternal m_propertiesInternal = new PropertiesInternal();
        private PropertiesExternal m_propertiesExternal = new PropertiesExternal();
        private PropertiesTransmission m_propertiesTransmission = new PropertiesTransmission();
		private PropertiesGroup m_propertiesGroup = new PropertiesGroup();

        private PointF[] m_arrDragDropOrigin = null;
        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;

        private ArrayList m_arrPanel = new ArrayList();
        private ArrayList m_arrTabPage = new ArrayList();
//         private ArrayList m_arrComponent = new ArrayList();

        private int m_nTabPage = 1;
        private int m_nActiopnStepID = 0;

        private Sections.PanelSectionEx m_currentPanel = null;

        private Color m_colorPanel1 = Color.FromArgb(234, 236, 236);//Color.FromArgb((int)UInt32.Parse("4292993535"));
        private Color m_colorPanel2 = Color.FromArgb(255, 255, 255);//Color.FromArgb((int)UInt32.Parse("4292673535"));

        private string m_strOldTabPageText;
        public string OldTabPageText
        {
            get { return m_strOldTabPageText; }
            set{m_strOldTabPageText = value;}
        }
		       
        void PageBackstageLevel_MouseWheel(object sender, MouseEventArgs e)
        {
            TabPage pageCurrent = this.TabControls.SelectedTab;
            if (pageCurrent == null)
                return;

			Point ptTabBegin = panelMain.Location;
			Rectangle rect = panelMain.DisplayRectangle;

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

        public void SetCurrentPanel(Sections.PanelSection panel)
        {
            m_currentPanel = (Sections.PanelSectionEx)panel;
        }

        private void CreateShortcutBar()
        {

            m_barComponent = new BarComponent();
            m_barLevelTree = new BarLevelTree();
            m_barPage = new BarPage();

			m_barComponent.TopLevel = false;
			m_barComponent.Dock = DockStyle.Fill;
			m_barComponent.Visible = true;
			panelComponent.Controls.Add(m_barComponent);
			

			m_barLevelTree.TopLevel = false;
			m_barLevelTree.Dock = DockStyle.Fill;
			m_barLevelTree.Visible = true;
			panelTree.Controls.Add(m_barLevelTree);

			m_barPage.TopLevel = false;
			m_barPage.Dock = DockStyle.Fill;
			m_barPage.Visible = true;
			panelPage.Controls.Add(m_barPage);



            //ShortcutBarItem ItemComponent = axShortcutBar.AddItem(ID.ID_SHORTCUT_COMPONENT, "컴포넌트", m_barComponent.Handle.ToInt32());
           // ShortcutBarItem Item = axShortcutBar.AddItem(ID.ID_SHORTCUT_LEVELTREE, "단계 Tree", m_barLevelTree.Handle.ToInt32());
           // Item = axShortcutBar.AddItem(ID.ID_SHORTCUT_PAGE, "페이지", m_barPage.Handle.ToInt32());

           // axShortcutBar.Selected = ItemComponent;
          //  axShortcutBar.ExpandedLinesCount = 3;

           


			if (TabControls.TabCount > 0)
			{
				TabPage tabPage1 = TabControls.SelectedTab;
				
				if (tabPage1 != null)
				{
					m_nActiopnStepID = LastActionStepID();
					++m_nActiopnStepID;
					m_arrTabPage.Add(tabPage1);

					m_propertiesLevel.GetLevelProperties(tabPage1);

					Data_ActionStep data = new Data_ActionStep();
					data.StepName = tabPage1.Text;
					//data.ParentStepID = (int)tabPage1.Tag;
					data.ParentStepID = -1;
					m_propertiesLevel.LevelProperties.Add(data);
				} 
			}
			
        }

        private void InitPropertiesLevel()
        {
            m_propertiesLevel.Location = new Point(0, 0);
            m_propertiesLevel.Dock = DockStyle.Fill;
            m_propertiesLevel.TopLevel = false;
            m_propertiesLevel.Parent = this;
			panelLevel.Controls.Add(m_propertiesLevel);
            m_propertiesLevel.Show();
        }

        private void InitPropertiesProcess()
        {
            m_propertiesProcess.Location = new Point(0, 0);
            m_propertiesProcess.Dock = DockStyle.Fill;
            m_propertiesProcess.TopLevel = false;
            m_propertiesProcess.Parent = this;
			panelComProperties.Controls.Add(m_propertiesProcess);
            m_propertiesProcess.Show();
        }

        private void InitPropertiesDicision()
        {
            m_propertiesDecision.Location = new Point(0, 0);
            m_propertiesDecision.Dock = DockStyle.Fill;
            m_propertiesDecision.TopLevel = false;
            m_propertiesDecision.Parent = this;
			panelComProperties.Controls.Add(m_propertiesDecision);
            m_propertiesDecision.Show();
        }

        private void InitPropertiesAnnotation()
        {
            m_propertiesAnnotation.Location = new Point(0, 0);
            m_propertiesAnnotation.Dock = DockStyle.Fill;
            m_propertiesAnnotation.TopLevel = false;
            m_propertiesAnnotation.Parent = this;
			panelComProperties.Controls.Add(m_propertiesAnnotation);
            m_propertiesAnnotation.Show();
        }

        private void InitPropertiesEndPoint()
        {
            m_propertiesEndPoint.Location = new Point(0, 0);
            m_propertiesEndPoint.Dock = DockStyle.Fill;
            m_propertiesEndPoint.TopLevel = false;
            m_propertiesEndPoint.Parent = this;
			panelComProperties.Controls.Add(m_propertiesEndPoint);
            m_propertiesEndPoint.Show();
        }

        private void InitPropertiesLink()
        {
            m_propertiesLink.Location = new Point(0, 0);
            m_propertiesLink.Dock = DockStyle.Fill;
            m_propertiesLink.TopLevel = false;
            m_propertiesLink.Parent = this;
			panelComProperties.Controls.Add(m_propertiesLink);
            m_propertiesLink.Show();
        }

        private void InitPropertiesTransSOP()
        {
            m_propertiesTransSOP.Location = new Point(0, 0);
            m_propertiesTransSOP.Dock = DockStyle.Fill;
            m_propertiesTransSOP.TopLevel = false;
            m_propertiesTransSOP.Parent = this;
			panelComProperties.Controls.Add(m_propertiesTransSOP);
            m_propertiesTransSOP.Show();
        }

        private void InitPropertiesInternal()
        {
            m_propertiesInternal.Location = new Point(0, 0);
            m_propertiesInternal.Dock = DockStyle.Fill;
            m_propertiesInternal.TopLevel = false;
            m_propertiesInternal.Parent = this;
			panelComProperties.Controls.Add(m_propertiesInternal);
            m_propertiesInternal.Show();
        }

        private void InitPropertiesExternal()
        {
            m_propertiesExternal.Location = new Point(0, 0);
            m_propertiesExternal.Dock = DockStyle.Fill;
            m_propertiesExternal.TopLevel = false;
            m_propertiesExternal.Parent = this;
			panelComProperties.Controls.Add(m_propertiesExternal);
            m_propertiesExternal.Show();
        }

        private void InitPropertiesTransmission()
        {
            m_propertiesTransmission.Location = new Point(0, 0);
            m_propertiesTransmission.Dock = DockStyle.Fill;
            m_propertiesTransmission.TopLevel = false;
            m_propertiesTransmission.Parent = this;
			panelComProperties.Controls.Add(m_propertiesTransmission);
            m_propertiesTransmission.Show();
        }

		private void InitPropertiesGroup()
		{
			m_propertiesGroup.Location = new Point(0, 0);
			m_propertiesGroup.Dock = DockStyle.Fill;
			m_propertiesGroup.TopLevel = false;
			m_propertiesGroup.Parent = this;
			panelComProperties.Controls.Add(m_propertiesGroup);
			m_propertiesGroup.Show();
		}

        public PropertiesLevel GetPropertiesLevel()
        {
            return m_propertiesLevel;
        }

        public PropertiesProcess GetPropertiesProcess()
        {
            return m_propertiesProcess;
        }

        public PropertiesDecision GetPropertiesDecision()
        {
            return m_propertiesDecision;
        }
        
        public PropertiesAnnotation GetPropertiesAnnotation()
        {
            return m_propertiesAnnotation;
        }

        public PropertiesEndPoint GetPropertiesEndPoint()
        {
            return m_propertiesEndPoint;
        }

        public PropertiesLink GetPropertiesLink()
        {
            return m_propertiesLink;
        }
        
        public PropertiesTransSOP  GetPropertiesTransSOP()
        {
            return m_propertiesTransSOP;
        }

		public PropertiesInternal GetPropertiesInternal()
        {
            return m_propertiesInternal;
        }

        public PropertiesExternal GetPropertiesExternal()
        {
            return m_propertiesExternal;
        }

        public PropertiesTransmission GetPropertiesTransmission()
        {
            return m_propertiesTransmission;
        }

		public PropertiesGroup GetPropertiesGrouup()
		{
			return m_propertiesGroup;
		}

        public  BarComponent GetBarComponent()
        {
            return m_barComponent;
        }

        public BarLevelTree GetBarLevelTree()
        {
            return m_barLevelTree;
        }
        
        public BarPage GetBarPage()
        {
            return m_barPage;
        }

        public void SetDragDropShape(PointF[] arrDragDrop, Sections.Section.ComponentType sectionType)
        {
            m_arrDragDropOrigin = arrDragDrop;
            m_sectionDragDropType = sectionType;
        }

        public void  ShowProperties(int nIndex)
        {
            m_propertiesProcess.Hide();
            m_propertiesDecision.Hide();
            m_propertiesAnnotation.Hide();
            m_propertiesEndPoint.Hide();
            m_propertiesLink.Hide();
            m_propertiesTransSOP.Hide();
            m_propertiesInternal.Hide();
            m_propertiesExternal.Hide();
            m_propertiesTransmission.Hide();

            switch(nIndex)
            {
                case 1:
                    m_propertiesProcess.Show();
                    break;
                case 2:
                    m_propertiesDecision.Show();
                    break;
                case 3:
                    m_propertiesAnnotation.Show();
                    break;
                case 4:
                    m_propertiesEndPoint.Show();
                    break;
                case 5:
                    m_propertiesLink.Show();
                    break;
                case 6:
                    m_propertiesTransSOP.Show();
                    break;
                case 7:
                    m_propertiesInternal.Show();
                    break;
                case 8:
                    m_propertiesExternal.Show();
                    break;
                case 9:
                    m_propertiesTransmission.Show();
                    break;
				case 10:
					m_propertiesGroup.Show();
					break;
            }
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

                //Point ptPanel = panel.Location;
                Point ptPanel = new Point(0, 0);
                Size sizePanel = panel.Size;

                if (e.X >= ptPanel.X && e.X <= ptPanel.X + sizePanel.Width && e.Y >= ptPanel.Y && e.Y <= ptPanel.Y + sizePanel.Height)
                    panel.MoveDrawingArray(m_arrDragDropOrigin, m_sectionDragDropType, e.X - ptPanel.X, e.Y - ptPanel.Y);
                else
                    panel.MoveDrawingArray(null, m_sectionDragDropType, 0, 0);
            }
        }

        private void tabControl_ClientSizeChanged(object sender, EventArgs e)
        {
            int nCount = GetBarPage().CheckCount;
            if (nCount == 0) return;

            TabPage tabPage1 = tabControl.SelectedTab;
            Size sz = tabPage1.Size;
            sz.Width = tabPage1.Width / nCount; // tabPage1.Controls.Count;
            sz.Height = tabPage1.Size.Height;

            foreach (TabPage tabPage in m_arrTabPage)
            {
                tabPage.Size = tabPage1.Size;
                Point pt = new Point(0, 0);

                foreach (Sections.PanelSectionEx panel in tabPage.Controls)
                {
                    panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                    panel.Location = new System.Drawing.Point(pt.X, 0);
                    pt.X += sz.Width;
                }
            }
        }

        // DB Loading을 통한 Tab Page 생성
        public TabPage AddTabPage(Data_ActionStep data)
        {
            TabPage tabPage = new TabPage();

            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = string.Format("TabPage_{0}", tabPage.Handle);
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(706, 604);
            tabPage.Text = data.StepName;
            //tabPage.Tag = ++m_nActiopnStepID;
            ++m_nActiopnStepID;

            tabControl.Controls.Add(tabPage);
            tabControl.SelectedTab = tabPage;
            
            m_arrTabPage.Add(tabPage);
            m_propertiesLevel.GetLevelProperties(tabPage);

            //m_barLevelTree.AddTreeNode();

            m_propertiesLevel.LevelProperties.Add(data);
            return tabPage;
        }

        // 사용자가 수동으로 단계추가 버튼을 눌러서 Tab 추가
        public void AddTabPage(string szValue = "")
        {
			if (szValue == "")
				szValue = "대응";
			int nTabPage = 0;
            foreach (TabPage page in tabControl.TabPages)
            {
                // 맨앞 두글자만 비교
				if (page.Text.Substring(0, 2) == szValue)
				{
					if (page.Text == szValue || nTabPage == 0)
					{
						nTabPage = 1;
					}
					else
					{
						string strNum = System.Text.RegularExpressions.Regex.Replace(page.Text, @"\D", "");
						if (strNum != "")
						{
							nTabPage = int.Parse(strNum) + 1;
						}
					}
				}
            }
            TabPage tabPage = new TabPage();
            tabPage.Location = new System.Drawing.Point(4, 22);
			if (nTabPage <= 0)
				tabPage.Name = szValue;
			else
				tabPage.Name = szValue + nTabPage.ToString();
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(706, 604);
			if (nTabPage <= 0)
				tabPage.Text = szValue;
			else
				tabPage.Text = szValue + nTabPage.ToString();
            //tabPage.Tag = ++m_nActiopnStepID;
            ++m_nActiopnStepID;

            
            tabControl.Controls.Add(tabPage);
            tabControl.SelectedTab = tabPage;

            m_arrTabPage.Add(tabPage);
            m_barLevelTree.AddTreeNode();
           
          

            Data_ActionStep data = new Data_ActionStep();
            data.StepName = tabPage.Text;
            //data.ParentStepID = (int)tabPage.Tag;
            data.ParentStepID = -1;

            m_propertiesLevel.LevelProperties.Add(data);
            m_propertiesLevel.GetLevelProperties(tabPage);
        }

        //ArrayList m_arrTeams = null;

        // DB Loading을 통한 Panel 생성
        // Return 값 : 새로 생성된 Panel 리스트
        public ArrayList AddPane(ArrayList arrTeams, TabPage tabPage = null , bool bAddOnly = false, int nIdx = -1)
        {
            //m_arrTeams = arrTeams;
            int nTeamCount = arrTeams.Count;
            if (nTeamCount == 0)
                return null;

            if (tabPage == null)
                tabPage = tabControl.SelectedTab;

            Size sz = new Size();
            sz.Width = tabPage.Size.Width / nTeamCount;
            sz.Height = tabPage.Height;

			//if (bAddOnly == false)
			//	m_arUsingTeam.Clear();

            Point pt = new Point(0, 0);
            ArrayList arrPanels = new ArrayList();

            for (int i=0;i<nTeamCount;i++)
            {
                SOPManager.StepMemberData data = (SOPManager.StepMemberData)arrTeams[i];
                
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
                //panel.StepName = tabControl.SelectedTab.Text;
                panel.TeamName = data.TeamName;
                panel.TeamID = data.TeamID;
                panel.TeamType = data.TeamType;
                panel.SetListener(this);
                panel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel1_MouseMove);

#if DEBUG
				string szTEmp  = string.Format("{0}_{1}_{2}", data.TeamName, data.TeamID,data.TeamType );
				panel.AddPanelTitle(szTEmp);
#else
				panel.AddPanelTitle(data.TeamName);
#endif


				tabPage.Controls.Add(panel); 
                pt.X += sz.Width;

                m_arrPanel.Add(panel);
                arrPanels.Add(panel);

				//AddUsingTeam(panel.TeamID, panel.TeamName, panel.TeamType, nIdx);
            }

            return arrPanels;
        }

		public void AddUsingTeam(ArrayList arTeamMemeber)
		{
			 int nTeamCount = arTeamMemeber.Count;

			 int nIdx = m_arUsingTeam.Count;
			 for (int i = 0; i < nTeamCount; i++)
			 {
				 StepMemberData data = (StepMemberData)arTeamMemeber[i];
				 AddUsingTeam(data.TeamID, data.TeamName, data.TeamType, nIdx++);
			 }
		}

		private void RemoveUsingTeam(int nTeamID, string szTeamName, int nTeamType)
		{
			bool bFindData = false;
			StepMemberData findData = new StepMemberData();
			foreach (StepMemberData team in m_arUsingTeam)
			{
				if (team.TeamID == nTeamID && team.TeamName == szTeamName && team.TeamType == nTeamType)
				{
					findData = team;
					bFindData = true;
					break;
				}
			}
			if (bFindData == true)
				m_arUsingTeam.Remove(findData);
		}

		private void AddUsingTeam(int nTeamID, string szTeamName, int nTeamType, int nIdx = -1)
		{
			StepMemberData teamData = new StepMemberData();
			teamData.TeamID = nTeamID;
			teamData.TeamType = nTeamType;
			teamData.TeamName = szTeamName;

			if (nIdx == -1)
			{
				m_arUsingTeam.Add(teamData);
			}
			else
			{
				m_arUsingTeam.Insert(nIdx, teamData);
			}
		}

        // 처음 SOP생성시 선택된 팀으로 Panel 생성
        public void AddPane()
        {
            ArrayList arrSelectedList = FormMain.Instance.GetPageDisaster().SelectedTeamList;
			if( arrSelectedList == null || arrSelectedList.Count == 0)
				return;

            TabPage tabPage = tabControl.SelectedTab;
            Size sz = new Size();
            sz.Width = tabPage.Size.Width / arrSelectedList.Count;
            sz.Height = tabPage.Height;

            Point pt = new Point(0, 0);

			m_arUsingTeam.Clear();

            ArrayList arrPanel = new ArrayList();
            for (int i = 0; i < arrSelectedList.Count; i++)
            {
				DataGridViewRow row = (DataGridViewRow)arrSelectedList[i];
				string strTeamName = row.Cells[1].Value.ToString();
                //string strTeamName = FormMain.Instance.ParseCaption(btn.Caption);

                Sections.PanelSectionEx panel = new Sections.PanelSectionEx();
                panel.TeamName = strTeamName;
                if (i % 2 == 0)
                    panel.BackColor = m_colorPanel1;
                if (i % 2 == 1)
                    panel.BackColor = m_colorPanel2;

                panel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom));
                // Panel에 Scroll이 생기면 Zoom In/Out의 계산이 복잡해지며, Mouse Wheel 이벤트가 동작하지 않는다.
                // 따라서, AutoScroll 옵션은 반드시 false로 둔다.
                panel.AutoScroll = false;
                panel.Dock = System.Windows.Forms.DockStyle.None;
                panel.Location = new System.Drawing.Point(pt.X, 0);
                panel.Name = string.Format("panel{0}", i + 1);
                panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                panel.BorderStyle = BorderStyle.FixedSingle;
                //panel.StepName = tabControl.SelectedTab.Text;
                panel.TeamName = strTeamName;

				panel.TeamID = (int)(row.Cells[0].Tag);

                int nTeamType = 0;
				//0(평일 비상 조직, TemporaryNormalTeam), 1(휴일 비상 조직, TemporaryEmergencyTeam), 2(외부 기관, ExternalTeam), 3(사용자 정의 조직, UserDefinedTeam), 4(정규 조직, RegularTeam)
				object obj = row.Tag;
				if (obj.GetType() == typeof(Data_NormalTeam))
				{
					nTeamType = 0;
				}
				else if (obj.GetType() == typeof(Data_EmergencyTeam))
				{
					nTeamType = 1;
				}
				else if (obj.GetType() == typeof(Data_ExternalTeam))
				{
					nTeamType = 2;
				}
				else if (obj.GetType() == typeof(Data_UserDefinedTeam))
				{
					nTeamType = 3;
				}
				else if (obj.GetType() == typeof(Data_RegularTeam))
				{
					nTeamType = 4;
				}

                panel.TeamType = nTeamType;
                panel.SetListener(this);
                panel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel1_MouseMove);

                panel.AddPanelTitle(strTeamName);

                tabPage.Controls.Add(panel);

                pt.X += sz.Width;

                m_arrPanel.Add(panel);

				AddUsingTeam(panel.TeamID, panel.TeamName, panel.TeamType);
				
            }
        }

		private ArrayList m_arUsingTeam = new ArrayList();
		public System.Collections.ArrayList UsingTeam
		{
			get { return m_arUsingTeam; }
		}

        // panelExcept를 제외한 모든 Panel의 선택을 해제한다.
        private void ClearSelection(Sections.PanelSectionEx panelExcept)
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

		public void OnClearComponentProperties()
		{
			ShowProperties(1);
			m_propertiesProcess.GetSectionData(null);			
		}

        public void OnSelectedSectionList(ArrayList arSections)
        { 
        }

        public void OnSelectedSection(Sections.Section section)
        {
            ClearSelection(m_currentPanel);
            if (section == null)
                return;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS) //프로세스
            {
                ShowProperties(1);
                m_propertiesProcess.GetSectionData((Sections.SectionProcess)section);
            }
            else if (type == Sections.Section.ComponentType.DECISION) // 판단
            {
                ShowProperties(2);
                m_propertiesDecision.GetSectionData((Sections.SectionDecision)section);
            }
            else if (type == Sections.Section.ComponentType.ANNOTATION) // 설명
            {
                ShowProperties(3);
                m_propertiesAnnotation.GetSectionData((Sections.SectionAnnotation)section);
            }
            else if (type == Sections.Section.ComponentType.ENDPOINT) // 시작/끝
            {
                ShowProperties(4);
                m_propertiesEndPoint.GetSectionData((Sections.SectionEndPoint)section);
            }
            else if (type == Sections.Section.ComponentType.LINK) // 링크
            {
                ShowProperties(5);
                m_propertiesLink.GetSectionData((Sections.SectionLink)section);
            }
            else if (type == Sections.Section.ComponentType.TRANSSOP) // 다른 SOP로 전환
            {
                ShowProperties(6);
                m_propertiesTransSOP.GetSectionData((Sections.SectionTransSOP)section);
            }
            else if (type == Sections.Section.ComponentType.INTERNAL) // 내부 상황전파
            {
                ShowProperties(7);
                m_propertiesInternal.GetSectionData((Sections.SectionInternal)section);
            }
            else if (type == Sections.Section.ComponentType.EXTERNAL) // 외부 상황전파
            {
                ShowProperties(8);
                m_propertiesExternal.GetSectionData((Sections.SectionExternal)section);
            }
            else if (type == Sections.Section.ComponentType.TRANSMISSION)   // 통합 상황전파
            {
                ShowProperties(9);
                m_propertiesTransmission.SetSection((Sections.SectionTransmission)section);
            }
			else if (type == Sections.Section.ComponentType.GROUP) // 그룹
			{
				ShowProperties(10);
				m_propertiesGroup.GetSectionData((Sections.SectionGroup)section);
			}
			else// if (type == Sections.Section.ComponentType.NONE)
				return;
        }

        public void OnSelectedArrow(Sections.Arrow arrow)
        {
            ClearSelection(m_currentPanel);
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
			if (tabPage == null)
				return "";
            return tabPage.Text;
        }

        public void ShowPanel()
        {
            ArrayList arrCheckTeam = GetBarPage().CheckTeamList;

            foreach (TabPage tabPage in m_arrTabPage)
            {
                foreach (Sections.PanelSectionEx panel in m_arrPanel)
                {
                    foreach (CheckTeam checkTeam in arrCheckTeam)
                    {
						 if (panel.TeamName == checkTeam.TeamName 
							&& panel.TeamID == checkTeam.TeamID 
							&& panel.TeamType == checkTeam.TeamType)
						 {                       
                            if (checkTeam.Check)
                            {
                                panel.Show();
                                break;
                            }
                            else
                            {
                                panel.Hide();
                                break;
                            }
                        }
                    }
                }
				tabPage.Refresh();
            }
            PanelResize();
        }

        public void PanelResize()
        {
            int nCount = GetBarPage().CheckCount;
            if (nCount == 0)
				return;

			if (tabControl.TabPages.Count == 0)
				return;

			ArrayList arrCheckTeam = GetBarPage().CheckTeamList;

            TabPage tabPage1 = tabControl.SelectedTab;
            Size sz = tabPage1.Size;
            sz.Width = tabPage1.Width / nCount;
            sz.Height = tabPage1.Size.Height;

            foreach (TabPage tabPage in m_arrTabPage)
            {
                tabPage.Size = tabPage1.Size;
                Point pt = new Point(0, 0);

                int nPanelCount = 0;

                foreach (Sections.PanelSectionEx panel in tabPage.Controls)
                {
                    panel.BackColor = nPanelCount++ % 2 == 0 ? m_colorPanel1 : m_colorPanel2;

                    foreach (CheckTeam checkTeam in arrCheckTeam)
                    {
                        if (panel.TeamName == checkTeam.TeamName 
							&& panel.TeamID == checkTeam.TeamID 
							&& panel.TeamType == checkTeam.TeamType
							&& checkTeam.Check == true)
                        {
                            panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                            panel.Location = new System.Drawing.Point(pt.X, 0);
                            pt.X += sz.Width;
                        }
                    }
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.Controls.Count == 0)
				return;
            
            TreeNode node = m_barLevelTree.FindNode(tabControl.SelectedTab.Text);
            if (node == null)
                return;

            m_barLevelTree.SelectNode(node);

            m_propertiesLevel.SetSelectedTabName(tabControl.SelectedTab.Text);
            m_propertiesLevel.ClearProperties();
            m_propertiesLevel.AddTitle(node.FullPath.Replace('\\', '/'));
            m_propertiesLevel.GetLevelProperties(tabControl.SelectedTab);

            m_propertiesLevel.AddParentName();
        }

        public void RemoveAll()
        {
            m_barPage.ClearGrid();
            m_barLevelTree.ClearTree();

            m_propertiesLevel.LevelProperties.Clear();
            m_propertiesLevel.ClearProperties();
            m_arrTabPage.Clear();
            //m_arrTeams = null;

            tabControl.Controls.Clear();
            m_nTabPage = 0;

			m_arUsingTeam.Clear();
            //AddTabPage();
        }

        public void RemoveTabPage(TabPage tabPage = null, bool needConfirm = true)
        {
            if (tabControl.Controls.Count == 0)
            {
				return;
               // MessageBox.Show("마지막 탭은 삭제 할 수 없습니다.");
                //return;
            }

            bool remove = !needConfirm;

            if (!remove)
                remove = MessageBox.Show("선택한 단계를 삭제 하시겠습니까?", "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK;

            if (remove)
            {
                if (tabPage == null)
                {
                    tabPage = tabControl.SelectedTab;
                    if (tabPage == null)
                        return;
                }
				
				UndoRedoManager.Instance.SaveSnapshot();
                
				tabControl.Controls.Remove(tabPage);
				m_arrTabPage.Remove(tabPage);
				m_barLevelTree.RemoveTreeNode(tabPage.Text);
            }
        }

        public void Delete()
        {
            if (m_currentPanel != null)
                m_currentPanel.Delete();

            // 삭제 버튼이 눌려지면 임시 Section 객체 해제
            SetDragDropShape(null, Sections.Section.ComponentType.NONE);
        }

        public ArrayList AllComponentList()
        {
            ArrayList arrComponent = new ArrayList();
            TabPage tabPage = tabControl.SelectedTab;
            foreach (Sections.PanelSectionEx panel in tabPage.Controls)
            {
                int n = panel.Sections.Count;
                if(this.m_currentPanel != panel)
                {
                    foreach (Sections.Section section in panel.Sections)
                    {
                        string strComponentID = "";
                        Sections.Section.ComponentType type = section.GetComponentType();

                        if (type == Sections.Section.ComponentType.PROCESS)
                        {
                            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
                            strComponentID = data.ComponentID;
                        }
                        else if (type == Sections.Section.ComponentType.DECISION)
                        {
                            Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;
                            strComponentID = data.ComponentID;
                        }
                        //else if (type == Sections.Section.ComponentType.ANNOTATION)
                        //{
                        //    Sections.SectionDataAnnotation data = (Sections.SectionDataAnnotation)section.Data;
                        //    strComponentID = data.ComponentID;
                        //}
                        else if (type == Sections.Section.ComponentType.ENDPOINT)
                        {
                            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
                            strComponentID = data.ComponentID;
                        }
                        //else if (type == Sections.Section.ComponentType.LINK)
                        //{
                        //}
                        else if (type == Sections.Section.ComponentType.TRANSSOP)
                        {
                            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;
                            strComponentID = data.ComponentID;
                        }
                        else if (type == Sections.Section.ComponentType.INTERNAL)
                        {
                            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
                            strComponentID = data.ComponentID;
                        }
                        else if (type == Sections.Section.ComponentType.EXTERNAL)
                        {
                            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
                            strComponentID = data.ComponentID;
                        }

                        if (strComponentID != "")
                            arrComponent.Add(section);
                    }
                }
            }
            return arrComponent;
        }

        // page(단계)에 해당하는 ActionStepOption 객체를 리턴한다.
        // 만일, page 객체를 인식할 수 없으면 null을 리턴한다.
        public Data_ActionStep GetActionStepOption(System.Windows.Forms.TabPage page)
        {
            Data_ActionStep data = m_propertiesLevel.SavePropertiesLevel(page);

            return data;
        }

        public TabControl TabControls
        {
            get { return tabControl; }
        }

        public bool numericCheck(string strValue)
        {
            try
            {
                int ll = Convert.ToInt32(strValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ArrayList GetTabPages()
        {
            return new ArrayList(tabControl.Controls);
        }

        public ArrayList GetTabPage()
        {
            return m_arrTabPage;
        }

        public int LastActionStepID()
        {
            int nID = 0;
            foreach (Data_ActionStep data in FormMain.Instance.ActionStep)
            {
                if (nID < data.ID)
                    nID = data.ID;
            }

            return nID;
        }

        public TabPage GetCurrentTabPage()
        {
			if (tabControl.TabPages.Count == 0)
				return null;
            return tabControl.SelectedTab;
        }

        // nControlIndex : Panel 내에서 삭제하고자하는 컨트롤의 Index
        public void DeletePanel(int nControlIndex)
        {
            Type type = typeof(TabPage);			
			TabPage tabPage2 = tabControl.SelectedTab;
			if (tabPage2 == null)
				return;

			// 사용중인 팀 목록에서 제거한다.
			Sections.PanelSectionEx panel = (Sections.PanelSectionEx)tabPage2.Controls[nControlIndex];
			if (panel != null)
			{
				RemoveUsingTeam(panel.TeamID, panel.TeamName, panel.TeamType);
			}

			// 모든 탭페이지에서 해당 팀의 패널을 제거한다.
            foreach (Control ctrl in tabControl.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    TabPage tabPage = (TabPage)ctrl;
                    tabPage.Controls.RemoveAt(nControlIndex);
                }
            }			
        }

		// 마지막 패널을 삭제
		public void DeletePanelLast()
		{
			Type type = typeof(Sections.PanelSectionEx);
			int nIndex = -1;

			TabPage tabPage = tabControl.SelectedTab;
			if (tabPage == null)
				return;

			foreach (Control ctrl in tabPage.Controls)
			{
				if (ctrl.GetType() == type)
				{
					nIndex++;
				}
			}			

			if (nIndex != -1)
			{
				Sections.PanelSectionEx panel = (Sections.PanelSectionEx)tabPage.Controls[nIndex];
				string strMsg = string.Format("{0} 패널을 삭제하려고 합니다.\r\n이 작업은 현재 열려있는 모든 탭들에 영향을 주게 됩니다.\r\n계속하시겠습니까?",
	   panel.TeamName);

				if (MessageBox.Show(strMsg, "알림", MessageBoxButtons.YesNo)
					== DialogResult.Yes)
				{
					Type type2 = typeof(TabPage);

					UndoRedoManager.Instance.SaveSnapshot();

					// 사용중인 팀 목록에서 해당 팀을 제거한다.
					if (panel != null)
					{
						int teamID = panel.TeamID;
						int teamType = panel.TeamType;
						string teamName = panel.TeamName;
						RemoveUsingTeam(teamID, teamName, teamType);	
					}					

					// 모든 탭페이지에 포함된 해당 팀의 패널을 삭제한다.
					foreach (Control ctrl in tabControl.Controls)
					{
						if (ctrl.GetType() == type2)
						{
							TabPage tabPage2 = (TabPage)ctrl;
							Sections.PanelSectionEx panel2 = (Sections.PanelSectionEx)tabPage2.Controls[nIndex];
							if (panel2 != null)
							{
								tabPage2.Controls.Remove(panel2);
								m_arrPanel.Remove(panel2);
							}
						}
					}
									
				}							
			}
			GetBarPage().SetDataGrid();
			PanelResize();
		}

        private Sections.PanelSectionEx GetPanel(int nTeamID, int nTeamType, TabPage tabPage)
        {
            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;

                    if (panel.TeamID == nTeamID && panel.TeamType == nTeamType)
                        return panel;
                }
            }

            return null;
        }

        // 패널의 순서 바꾸기
        // nTeamID, nTeamType에 해당하는 Panel을 nControlIndex의 위치로 바꾸어준다.
        public void ReorderPanel(int nTeamID, int nTeamType, int nControlIndex)
        {
            Type type = typeof(TabPage);

            foreach (Control ctrl in tabControl.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    TabPage tabPage = (TabPage)ctrl;
                    Sections.PanelSectionEx panel = GetPanel(nTeamID, nTeamType, tabPage);
                    if (panel == null)
                        continue;

                    tabPage.Controls.SetChildIndex(panel, nControlIndex);
                }
            }
        }

        // 새로운 Panel을 nControlIndex의 위치에 만든다.
        public void AddPanel(StepMemberData data, int nControlIndex)
        {
			// 사용중인 팀에 추가한다.
            ArrayList arrTeams = new ArrayList();
            arrTeams.Add(data);
			AddUsingTeam(arrTeams);
           
			Type type = typeof(TabPage);
			
            foreach (Control ctrl in tabControl.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    TabPage tabPage = (TabPage)ctrl;

					ArrayList arrPanels = FormMain.Instance.GetPageLevel().AddPane(arrTeams, tabPage, true, nControlIndex);
					
                    if (arrPanels == null || arrPanels.Count == 0)
                        return;

                    Sections.PanelSectionEx newPanel = (Sections.PanelSectionEx)arrPanels[0];
                    tabPage.Controls.SetChildIndex(newPanel, nControlIndex);

                }
            }
		}

		private void FormPageSOP_Load(object sender, EventArgs e)
		{
			PanelResize();
		}


		# region Splitter 이벤트
		private void FormPageSOP_SizeChanged(object sender, EventArgs e)
		{
			int nWidth = this.Size.Width;
			int nHeight  = this.Size.Height;
			if (nWidth > 257)
			{				
				splitter3.MinExtra = nWidth - 546;
				splitter3.MinSize = 250;
				splitter1.MinExtra = nWidth - 546;
				splitter1.MinSize = m_nMinSplitPos;
			}			
		}

		private void splitter3_MouseUp(object sender, MouseEventArgs e)
		{
			if (splitter3.SplitPosition < 250)
			{
				splitter3.SplitPosition = 250;
			}
		}
		
		private void splitter3_SplitterMoved(object sender, SplitterEventArgs e)
		{

			if (splitter3.SplitPosition < 250)
			{
				splitter3.SplitPosition = 250;
			}
		}

		private void splitter3_DoubleClick(object sender, EventArgs e)
		{
		}

		private void splitter3_SplitterMoving(object sender, SplitterEventArgs e)
		{
			if (splitter3.SplitPosition < 250)
			{
				splitter3.SplitPosition = 250;
			}
		}


		private int m_nMaxSplitPos = 290;
		private int m_nMinSplitPos = 40;
		private void splitter1_MouseUp(object sender, MouseEventArgs e)
		{
			if (splitter1.SplitPosition > m_nMaxSplitPos)
			{
				splitter1.SplitPosition = m_nMaxSplitPos;
			}
		}

		private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (splitter1.SplitPosition > m_nMinSplitPos)
			{
				button1.Location = button2.Location;
				button1.Visible = true;
				button2.Visible = false;
			}
			else
			{
				button2.Location = button1.Location;
				button2.Visible = true;
				button1.Visible = false;			
			}
			PanelResize();
		}

		private void splitter1_SplitterMoving(object sender, SplitterEventArgs e)
		{
			if (splitter1.SplitPosition > m_nMaxSplitPos)
			{
				splitter1.SplitPosition = m_nMaxSplitPos;
			}
		}

		private void splitter1_DoubleClick(object sender, EventArgs e)
		{
			if (splitter1.SplitPosition != 0)
				splitter1.SplitPosition = 0;
			else
				splitter1.SplitPosition = m_nMaxSplitPos;
		}

		private void splitter1_Move(object sender, EventArgs e)
		{
			if (splitter1.SplitPosition > m_nMaxSplitPos)
			{
				splitter1.SplitPosition = m_nMaxSplitPos;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			splitter1.SplitPosition = m_nMinSplitPos;
			button2.Location = button1.Location;
			button2.Visible = true;
			button1.Visible = false;
			PanelResize();	
		}

		private void button2_Click(object sender, EventArgs e)
		{
			splitter1.SplitPosition = m_nMaxSplitPos;
			//button2.Location = button1.Location;
			button1.Visible = true;
			button2.Visible = false;
			PanelResize();
		}
		#endregion

        public void LevelTabSelected()
        {
			if (tabControl.Controls.Count > 0 && tabControl.SelectedTab != null)
			{
				TreeNode node = m_barLevelTree.FindNode(tabControl.SelectedTab.Text);
				if (node != null)
				{
					m_barLevelTree.SelectNode(node);

					m_propertiesLevel.SetSelectedTabName(tabControl.SelectedTab.Text);
					m_propertiesLevel.ClearProperties();
					m_propertiesLevel.AddTitle(node.FullPath.Replace('\\', '/'));
					m_propertiesLevel.GetLevelProperties(tabControl.SelectedTab);

					m_propertiesLevel.AddParentName();
				}
			}
			else
			{
				m_propertiesLevel.SetSelectedTabName("");
				m_propertiesLevel.ClearProperties();				
			}          
        }

        private void tabControl_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LevelTabSelected();
        }

		// 이거누가했음?? 
		//private void tabControl_ClientSizeChanged_1(object sender, EventArgs e)
		//{
		//    int nCount = GetBarPage().CheckCount;
		//    if (nCount == 0) return;

		//    TabPage tabPage1 = tabControl.SelectedTab;
		//    Size sz = tabPage1.Size;
		//    sz.Width = tabPage1.Width / nCount; // tabPage1.Controls.Count;
		//    sz.Height = tabPage1.Size.Height;

		//    foreach (TabPage tabPage in m_arrTabPage)
		//    {
		//        tabPage.Size = tabPage1.Size;
		//        Point pt = new Point(0, 0);

		//        foreach (Sections.PanelSectionEx panel in tabPage.Controls)
		//        {
		//            panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
		//            panel.Location = new System.Drawing.Point(pt.X, 0);
		//            pt.X += sz.Width;
		//        }
		//    }
		//}
	}

    public class LevelOption
    {
        private string m_strLevelName;
        private string m_strTerm;
        private string m_strNumber;
        private string m_strProcessTime;
        private string m_strParent;

        public string LevelName
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }
        public string Term
        {
            get { return m_strTerm; }
            set { m_strTerm = value; }
        }
        public string Number
        {
            get { return m_strNumber; }
            set { m_strNumber = value; }
        }
        public string ProcessTime
        {
            get { return m_strProcessTime; }
            set { m_strProcessTime = value; }
        }
        public string Parent
        {
            get { return m_strParent; }
            set { m_strParent = value; }
        }
    }

	public class TabControlEx : TabControl
	{
		public TabControlEx()
		{
			SetStyle(ControlStyles.DoubleBuffer |
			  ControlStyles.AllPaintingInWmPaint,
			  true);
		}
	}
    

    public class ActionStepOption
    {
        // PeriodType : 기간 Type : 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
        //                                      11(고정 년도 사용 + 날짜 옵션), 12(고정 년도 사용 + 시간 옵션), 13(고정 년도 사용 + 날짜 옵션 + 시간 옵션)
        // WeekDayOption : 요일 옵션(bit 연산), bit : 1(일요일), 2(월요일), 4(화요일), 8(수요일), 16(목요일), 32(금요일), 64(토요일)
        // Iteration : 반복 회수
        // IterationType : 반복 회수 옵션 : 0(전체 기간중 몇회), 1(년중 몇회), 2(월중 몇회), 3(주중 몇회), 4(하루중 몇회), 5(시간당 몇회)
        // ProcessTimeType : 처리시간 옵션, 0(개월), 1(주), 2(일), 3(시간), 4(분)
        private int m_nPeriodType = 0;
        private DateTime m_dtBegin = new DateTime();
        private DateTime m_dtEnd = new DateTime();
        private int m_nWeekdayOption = 127;
        private int m_nIterationType = 0;
        private int m_nIteration = 1;
        private int m_nProcessTimeType = 4;
        private int m_nProcessTime = 1;

        public ActionStepOption()
        {
        }

        public ActionStepOption(int nPeriodType, DateTime dtBegin, DateTime dtEnd, int nWeekdayOption, int nIterationType, int nIteration, int nProcessTimeType, int nProcessTime)
        {
            m_nPeriodType = nPeriodType;
            m_dtBegin = dtBegin;
            m_dtEnd = dtEnd;
            m_nWeekdayOption = nWeekdayOption;
            m_nIterationType = nIterationType;
            m_nIteration = nIteration;
            m_nProcessTimeType = nProcessTimeType;
            m_nProcessTime = nProcessTime;
        }

        public int PeriodType
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public int WeekdayOption
        {
            get { return m_nWeekdayOption; }
            set { m_nWeekdayOption = value; }
        }

        public int IterationType
        {
            get { return m_nIterationType; }
            set { m_nIterationType = value; }
        }

        public int Iteration
        {
            get { return m_nIteration; }
            set { m_nIteration = value; }
        }

        public int ProcessTimeType
        {
            get { return m_nProcessTimeType; }
            set { m_nProcessTimeType = value; }
        }

        public int ProcessTime
        {
            get { return m_nProcessTime; }
            set { m_nProcessTime = value; }
        }
    }

}
