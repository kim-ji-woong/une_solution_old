using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Sections;
using System.Runtime.InteropServices;

namespace SOPManager
{
	public partial class FormPageSOP : Form, Sections.ISectionListener
	{
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02; 
        private const int MOUSEEVENTF_LEFTUP = 0x04; 
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08; 
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void AutoClickPanel()
        {      
            //Call the imported function with the cursor's current position 
            int X = tabControl.Location.X + 10;
            int Y = tabControl.Location.Y + tabControl.ItemSize.Height + 10; ;
            mouse_event((MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP), X, Y, 0, 0);
        }

        private int m_nMaxSplitPos = 380; 
        private int m_nCurSplitPos = 380;
        private int m_nMinSplitPos = 40;

		public FormPageSOP()
		{
			InitializeComponent();

			SetSectionColor();

			TopLevel = false;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			BackColor = Color.FromArgb(227, 226, 226);

			CreateShortcutBar();
            InitPropertiesLevel();
            InitPropertiesSection();

            this.MouseWheel += new MouseEventHandler(PageBackstageLevel_MouseWheel);
            
			FormToolBox toolBox = new FormToolBox();
            FormMain.Instance.event_WinRateChanged += toolBox.event_WinRateChanged;
			toolBox.TopLevel = false;
            toolBox.Dock = DockStyle.Fill;

            //FormMain.Instance.ChangeResolutionControlSize(panelLeft);
            //FormMain.Instance.ChangeResolutionControlSize(panelRight);
            
			paneToolBox.Controls.Add(toolBox);
			toolBox.Show();

            m_nMaxSplitPos = panelLeft.Size.Width;
            m_nCurSplitPos = panelLeft.Size.Width;

            //Resize Event 발생 후 Window 해상도 변경 시 호출 되는 이벤트
            FormMain.Instance.event_WinRateChanged += Instance_event_WinRateChanged;
		}

        void Instance_event_WinRateChanged()
        {            
            tabControl.Location = new Point((int)((float)tabControl.Location.X * FormMain.Instance.WindowWidthRate), tabControl.Location.Y);
            tabControl.Font = new System.Drawing.Font(tabControl.Font.FontFamily, (float)(tabControl.Font.Size * FormMain.Instance.WindowWidthRate), tabControl.Font.Style);
            tabControl.ItemSize = new Size((int)(tabControl.ItemSize.Width * FormMain.Instance.WindowWidthRate), (int)(tabControl.ItemSize.Height * FormMain.Instance.WindowHeightRate));

            //오른쪽 Layout
            panelRight.Size = new System.Drawing.Size((int)(panelRight.Size.Width * FormMain.Instance.WindowWidthRate), (int)(panelRight.Size.Height));

            m_nMaxSplitPos = (int)(m_nMaxSplitPos * FormMain.Instance.WindowWidthRate);            
            //splitter1.SplitPosition = (int)(splitter1.SplitPosition * FormMain.Instance.WindowWidthRate);
            //m_nCurSplitPos = splitter1.SplitPosition;

            panelLeft.Size = new System.Drawing.Size((int)(panelLeft.Size.Width * FormMain.Instance.WindowWidthRate), panelLeft.Size.Height);
            m_nCurSplitPos = panelLeft.Size.Width;
        }
        
		private void SetSectionColor()
		{
			EditBox.SetColor(true, Color.White);
			EditBox.SetColor(false, Color.FromArgb(60, 56, 71));

			Arrow.NormalPen.Color = Color.Gray;
			Arrow.TempLinePen.Color = Color.LightGray;
            Arrow.TriangleBrush.Color = Color.Gray;
			Arrow.TextFont = new Font(Program.prgFont, 12, FontStyle.Regular);
			Arrow.TextBrush.Color = Color.Black;
			Sections.Shape.UseImage = false;
            
			SizeManager.MinSize = new Size(100, 40);            
			SectionDecision.DefaultSize = new Size(200, 85);

			PanelSectionEx.EditableArrowText = false;
			PathNotifier.PathColor = Color.Purple;
		}			

        private BarComponent m_barComponent = null;
        private BarLevelTree m_barLevelTree = null;
        private BarPage m_barPage = null;
        private FormPanel.BarConfig m_barConfig = null;

        private FormProperties formProperties = new FormProperties();
        internal FormProperties FormProperties
        {
            get { return formProperties; }
        }

		private FormLevelProperties m_propertiesLevel = new FormLevelProperties();

        private PointF[] m_arrDragDropOrigin = null;
        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;

        private ArrayList m_arrPanel = new ArrayList();
        private ArrayList m_arrTabPage = new ArrayList();

		private int m_nActiopnStepID = 0;
        private Sections.PanelSectionEx m_currentPanel = null;

        private Color m_colorPanel1 = Color.FromArgb(234, 236, 236);
        private Color m_colorPanel2 = Color.FromArgb(207, 240, 196);

        private string m_strOldTabPageText;
        public string OldTabPageText
        {
            get { return m_strOldTabPageText; }
            set{m_strOldTabPageText = value;}
        }
		
		public void ClearModify()
		{
			bool bModified = false;

			foreach (ActionStepTabPage tabPage in m_arrTabPage)
			{
				foreach (Control control in tabPage.Controls)
				{
					if (control.GetType() != typeof(Sections.PanelSectionEx))
						continue;

					Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;
					if (panel.IsModified == true)
					{
						panel.IsModified = bModified;
					}
				}
			}
		}

		public bool InitSectionPanel()
		{
			if (CheckModify())
			{
				FormSaveOption form = new FormSaveOption();
                UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
				frame.StartPosition = FormStartPosition.CenterScreen;
				DialogResult result = frame.ShowDialog(this);
				if (result == DialogResult.Yes)
				{
					bool bSaveDB = form.SaveDB;
					if( bSaveDB == true)
					{
						// DB Save
						if( FormMain.Instance.SaveToDB())
						{
							return true;
						}
					}
					else
					{
						// Save XML
						if (FormMain.Instance.SaveSOPXML())
						{
							return true;
						}
					}
				}
				else if (result == DialogResult.No)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public bool CheckModify()
		{
			bool bModified = false;

			foreach (ActionStepTabPage tabPage in m_arrTabPage)
			{
				foreach (Control control in tabPage.Controls)
				{
					if (control.GetType() != typeof(Sections.PanelSectionEx))
						continue;

					Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;
					if (panel.IsModified == true)
					{
						bModified = true;
					}
				}
			}
			return bModified;
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
					panel.CurrentPanel = true;
                    panel.WheelMouse(nPanelX - ptPanel.X, nPanelY - ptPanel.Y, e.Delta);
					panel.Refresh();
                }
				else
				{
					panel.CurrentPanel = false;
					panel.Refresh();
				}
            }
        }

        public void SetCurrentPanel(Sections.PanelSection panel)
        {
            m_currentPanel = (Sections.PanelSectionEx)panel;
			if (panel == null)
				return;

			m_currentPanel.CurrentPanel = true;
			m_currentPanel.Refresh();

			TabPage page = tabControl.SelectedTab;
			if (page == null)
				return;

			Type type = typeof(Sections.PanelSectionEx);

			foreach (Control control in page.Controls)
			{
				if (control.GetType() == type)
				{
					Sections.PanelSectionEx panel2 = (Sections.PanelSectionEx)control;
					if (panel2 != m_currentPanel)
					{
						panel2.CurrentPanel = false;
						panel2.Refresh();
					}					
				}
			}
        }

        private void CreateShortcutBar()
        {
            m_barComponent = new BarComponent();
            m_barLevelTree = new BarLevelTree();
            m_barPage = new BarPage();
            m_barConfig = new FormPanel.BarConfig();
            
            FormMain.Instance.event_WinRateChanged += m_barComponent.event_WinRateChanged;
            FormMain.Instance.event_WinRateChanged += m_barLevelTree.event_WinRateChanged;
            FormMain.Instance.event_WinRateChanged += m_barPage.event_WinRateChanged;
            FormMain.Instance.event_WinRateChanged += m_barConfig.event_WinRateChanged;

			m_barComponent.TopLevel = false;
			m_barComponent.Dock = DockStyle.Fill;
			m_barComponent.Visible = true;
			panelComponent.Controls.Add(m_barComponent);
            
			m_barLevelTree.TopLevel = false;
			m_barLevelTree.Dock = DockStyle.Fill;
			m_barLevelTree.Visible = true;
			panelTree.Controls.Add(m_barLevelTree);

            //m_barPage.TopLevel = false;
            //m_barPage.Dock = DockStyle.Fill;
            //m_barPage.Visible = false;
            //panelPage.Controls.Add(m_barPage);

            m_barConfig.TopLevel = false;
            m_barConfig.Dock = DockStyle.Fill;
            m_barConfig.Visible = true;
            panelPage.Controls.Add(m_barConfig);
			
			if (TabControls.TabCount > 0)
			{
				ActionStepTabPage tabPage1 = (ActionStepTabPage)TabControls.SelectedTab;
				
				if (tabPage1 != null)
				{
					m_nActiopnStepID = LastActionStepID();
					++m_nActiopnStepID;
					m_arrTabPage.Add(tabPage1);

					m_propertiesLevel.SetActionStep(tabPage1.Data);
				} 
			}			
        }

        private void InitPropertiesLevel()
        {
            FormMain.Instance.event_WinRateChanged += m_propertiesLevel.event_WinRateChanged;

            m_propertiesLevel.Location = new Point(0, 0);
            m_propertiesLevel.Dock = DockStyle.Fill;
            m_propertiesLevel.TopLevel = false;
            m_propertiesLevel.Parent = this;
			panelLevel.Controls.Add(m_propertiesLevel);
            m_propertiesLevel.Show();
        }

        private void InitPropertiesSection()
        {
            FormMain.Instance.event_WinRateChanged += formProperties.event_WinRateChanged;

			formProperties.Location = new Point(0, 0);
			formProperties.Dock = DockStyle.Fill;
			formProperties.TopLevel = false;
			formProperties.Parent = this;
			panelComProperties.Controls.Add(formProperties);
			formProperties.Show();
        }
		
		public FormLevelProperties GetPropertiesLevel()
        {
			return m_propertiesLevel;
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

        public FormPanel.BarConfig  GetBarConfig()
        {
            return m_barConfig;
        }

        public void SetDragDropShape(PointF[] arrDragDrop, Sections.Section.ComponentType sectionType)
        {
            m_arrDragDropOrigin = arrDragDrop;
            m_sectionDragDropType = sectionType;

			if( arrDragDrop == null)
			{
				m_barComponent.ClearSelection();
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
            if (m_barPage == null)
                return;

            int nCount = GetBarPage().CheckCount;
            if (nCount == 0)
                return;

            TabPage tabPage1 = tabControl.SelectedTab;
            Size sz = tabPage1.Size;
            sz.Width = (tabPage1.Width) / nCount; // tabPage1.Controls.Count;
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
			ActionStepTabPage tabPage = new ActionStepTabPage();

            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = string.Format("TabPage_{0}", tabPage.Handle);
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(706, 604);
            tabPage.Text = data.StepName;

			tabPage.ToolTipText = tabPage.Text;
            ++m_nActiopnStepID;

            tabControl.Controls.Add(tabPage);
            tabControl.SelectedTab = tabPage;			

            m_arrTabPage.Add(tabPage);

			tabPage.Data = (Data_ActionStep)data.Clone();
			m_propertiesLevel.SetActionStep(tabPage.Data);

            return tabPage;
        }

        // 사용자가 수동으로 단계추가 버튼을 눌러서 Tab 추가
        public void AddTabPage(string szValue = "")
        {
            if (szValue == "")
            {
                szValue = Data_ActionStep.StandardActionStepNames[2];
            }

            int nIndex = GetActionStepIndex(szValue);
            
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
            
            string szTempName = szValue + nTabPage.ToString();
            TreeNode node = m_barLevelTree.FindNode(szTempName, null);
            int nCount = 0;
            while( node != null)
            {
                nTabPage++;
                szTempName = szValue + nTabPage.ToString();
                node = m_barLevelTree.FindNode(szTempName, null);
                if( nCount == 100)
                {
                    break;
                }
                nCount++;
            }
            
			ActionStepTabPage tabPage = new ActionStepTabPage();
			
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

            tabPage.ToolTipText = tabPage.Text;
            ++m_nActiopnStepID;

            InsertTab(tabPage, nIndex);
            //tabControl.Controls.Add(tabPage);
            tabControl.SelectedTab = tabPage;

            m_arrTabPage.Insert(nIndex, tabPage);
            //m_arrTabPage.Add(tabPage);
            m_barLevelTree.AddTreeNode();
			
            Data_ActionStep data = new Data_ActionStep();
            data.StepName = tabPage.Text;
            data.ParentStepID = -1;

			tabPage.Data = data;
			m_propertiesLevel.SetActionStep(tabPage.Data);
        }

        private void InsertTab(ActionStepTabPage tabPage, int nIndex)
        {
            List<Control> controls = new List<Control>();
            int nCount = tabControl.Controls.Count;

            for (int i=0;i<nCount;i++)
            {
                Control ctrl = tabControl.Controls[i];

                if (i >= nIndex)
                    controls.Add(ctrl);
            }

            foreach (Control ctrl in controls)
            {
                tabControl.Controls.Remove(ctrl);
            }

            tabControl.Controls.Add(tabPage);

            for (int i=0;i<controls.Count;i++)
            {
                Control ctrl = controls[i];
                tabControl.Controls.Add(ctrl);
            }
        }

        private int GetActionStepIndex(string strNewStepName)
        {
            List<string> actionStepNames = new List<string>();

            foreach (ActionStepTabPage tabPage in m_arrTabPage)
            {
                actionStepNames.Add(tabPage.Name);
            }

            return Data_ActionStep.GetActionStepIndex(strNewStepName, actionStepNames);
        }

        // DB Loading을 통한 Panel 생성
        // Return 값 : 새로 생성된 Panel 리스트
        public ArrayList AddPane(ArrayList arrTeams, TabPage tabPage = null , bool bAddOnly = false, int nIdx = -1)
        {
            int nTeamCount = arrTeams.Count;
            if (nTeamCount == 0)
                return null;

			tabControl.Margin = new Padding(0, 0, 0, 0);
			
            if (tabPage == null)
                tabPage = tabControl.SelectedTab;
			tabPage.Margin = new Padding(0, 0, 0, 0);
            
            Size sz = new Size();
			sz.Width = (tabPage.Size.Width) / nTeamCount;
            sz.Height = tabPage.Height;
            
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
                panel.TeamName = data.TeamName;
                panel.TeamID = data.TeamID;
                panel.TeamType = data.TeamType;
                panel.SetListener(this);
				panel.Margin = new Padding(0, 0, 0, 0);
                panel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel1_MouseMove);

//#if DEBUG
//                // 디버그일때 팀 아이디까지 출력하도록
//                string szTEmp  = string.Format("{0}_{1}_{2}", data.TeamName, data.TeamID,data.TeamType );
//                panel.AddPanelTitle(szTEmp);
//#else
                panel.AddPanelTitle("시나리오 흐름도");
				//panel.AddPanelTitle(data.TeamName);
//#endif
                panel.SetTitleForeColor(Color.Gray);
				panel.SetTitleBackColor(Color.WhiteSmoke);

				tabPage.Controls.Add(panel); 
                pt.X += sz.Width;

                m_arrPanel.Add(panel);
                arrPanels.Add(panel);
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

        private void RemoveUsingTeam(int nTeamID, string szTeamName, Sections.SOPTeam.SOPTeamType nTeamType)
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

        private void AddUsingTeam(int nTeamID, string szTeamName, Sections.SOPTeam.SOPTeamType nTeamType, int nIdx = -1)
		{

            foreach(StepMemberData data in m_arUsingTeam)
            {
                if(data.TeamID == nTeamID)
                {
                    if(data.TeamType == nTeamType)
                    {
                        return;
                    }
                }
            }


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
		public void AddPane(List<SelectTeamInfo> arSelectedTeamList)
        {
			if( arSelectedTeamList == null || arSelectedTeamList.Count == 0)
				return;

            TabPage tabPage = tabControl.SelectedTab;
            Size sz = new Size();
			sz.Width = (tabPage.Size.Width) / arSelectedTeamList.Count;
            sz.Height = tabPage.Height;

            Point pt = new Point(0, 0);

			m_arUsingTeam.Clear();

            ArrayList arrPanel = new ArrayList();
			for (int i = 0; i < arSelectedTeamList.Count; i++)
            {				
				SelectTeamInfo teamInfo = arSelectedTeamList[i];				
				string strTeamName = teamInfo.TeamName;
			
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
               
				panel.TeamID = teamInfo.TeamID;
                panel.TeamType = teamInfo.TeamType;

                panel.SetListener(this);
                panel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel1_MouseMove);

                //panel.AddPanelTitle("SOP 흐름도");
				//panel.SetTitleForeColor(Color.Gray);
				//panel.SetTitleBackColor(Color.WhiteSmoke);

                tabPage.Controls.Add(panel);

                pt.X += sz.Width;
                m_arrPanel.Add(panel);

				AddUsingTeam(teamInfo.TeamID, teamInfo.TeamName, teamInfo.TeamType);				
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
			if (formProperties != null)
				formProperties.SetComponent(null);	
		}

		private ArrayList m_arSelectedSections = null;
		private Sections.Section m_SelectedSection = null; 
        
		public bool IsSelectedSection()
		{
			if(m_arSelectedSections != null && m_arSelectedSections.Count > 0)
			{
				return true;
			}

			if (m_SelectedSection != null)
				return true;
			return false;
		}

        public void OnSelectedSectionList(ArrayList arSections)
        {
			m_arSelectedSections = arSections;
        }

        public void OnSelectedSection(Sections.Section section)
        {
			if (formProperties != null)
				formProperties.SetComponent(section);

			m_SelectedSection = section;
		}

        public void OnSelectedArrow(Sections.Arrow arrow)
        {
            ClearSelection(m_currentPanel);
        }


		private bool m_bShowRight = false;
		public bool IsShowRightPane
		{
			get { return m_bShowRight; }			
		}
		public void ShowRightPane(bool bShow)
		{
			panelRight.Visible = bShow;
			m_bShowRight = bShow;
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

							panel.MoveCenter(panel.Width, sz.Width);

                            panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                            panel.Location = new System.Drawing.Point(pt.X, 0);
                            pt.X += sz.Width;

							panel.Invalidate();
                        }
                    }
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
			m_propertiesLevel.SetActionStep(null);
            if (tabControl.Controls.Count == 0)
				return;            
            TreeNode node = m_barLevelTree.FindNode(tabControl.SelectedTab.Text);
            if (node == null)
                return;

            m_barLevelTree.SelectNode(node);

			ActionStepTabPage tabPage = (ActionStepTabPage)tabControl.SelectedTab;
			if( tabPage != null)
			{
				m_propertiesLevel.SetTitleText(node.FullPath.Replace('\\', '/'));
				m_propertiesLevel.SetActionStep(tabPage.Data);
			}            
        }

        public void RemoveAll()
        {
            m_barPage.ClearGrid();
            m_barLevelTree.ClearTree();
            
            m_propertiesLevel.ClearSelection();
            m_arrTabPage.Clear();

            tabControl.Controls.Clear();           

			m_arUsingTeam.Clear();
        }

        public void RemoveTabPage(ActionStepTabPage tabPage = null, bool needConfirm = true)
        {
            if (tabControl.Controls.Count == 0)
            {
				return;
			}

            bool remove = !needConfirm;

			if (tabPage == null)
			{
				tabPage = (ActionStepTabPage)tabControl.SelectedTab;
				if (tabPage == null)
					return;
			}

			if (!remove)
			{
				string szName = string.Format("선택한 [{0}] 단계를 삭제 하시겠습니까?", tabPage.Text);
				remove = UnE.Utility.UMessageBoxRibbon.Show(szName, "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK;
			}

            if (remove)
            {  
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

                        else if (type == Sections.Section.ComponentType.ENDPOINT)
                        {
                            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
                            strComponentID = data.ComponentID;
                        }

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
			ActionStepTabPage aPage = (ActionStepTabPage)page;
            return aPage.Data;
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

		public ActionStepTabPage GetTabPage(string szTabName)
		{
			foreach(ActionStepTabPage page in m_arrTabPage)
			{
				if (page.Text == szTabName)
					return page;
			}
			return null;
		}

        public ActionStepTabPage GetCurrentTabPage()
        {
			if (tabControl.TabPages.Count == 0)
				return null;
            return (ActionStepTabPage)tabControl.SelectedTab;
        }

		public Sections.PanelSectionEx GetCurrentPanel()
		{
			return m_currentPanel;
		}

        // nControlIndex : Panel 내에서 삭제하고자하는 컨트롤의 Index
        public void DeletePanel(int nControlIndex)
        {
            Type type = typeof(ActionStepTabPage);			
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
					ActionStepTabPage tabPage = (ActionStepTabPage)ctrl;
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
				string strMsg = string.Format("[{0}] 패널을 삭제하려고 합니다.\r\n이 작업은 현재 열려있는 모든 탭들에 영향을 주게 됩니다.\r\n계속하시겠습니까?",
	   panel.TeamName);

				if (UnE.Utility.UMessageBoxRibbon.Show(strMsg, "알림", MessageBoxButtons.YesNo)
					== DialogResult.Yes)
				{
					Type type2 = typeof(ActionStepTabPage);

					UndoRedoManager.Instance.SaveSnapshot("패널 삭제");

					// 사용중인 팀 목록에서 해당 팀을 제거한다.
					if (panel != null)
					{
						int teamID = panel.TeamID;
                        Sections.SOPTeam.SOPTeamType teamType = panel.TeamType;
						string teamName = panel.TeamName;
						RemoveUsingTeam(teamID, teamName, teamType);	
					}					

					// 모든 탭페이지에 포함된 해당 팀의 패널을 삭제한다.
					foreach (Control ctrl in tabControl.Controls)
					{
						if (ctrl.GetType() == type2)
						{
							ActionStepTabPage tabPage2 = (ActionStepTabPage)ctrl;
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

        private Sections.PanelSectionEx GetPanel(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType, TabPage tabPage)
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
        public void ReorderPanel(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType, int nControlIndex)
        {
			Type type = typeof(ActionStepTabPage);

            foreach (Control ctrl in tabControl.Controls)
            {
                if (ctrl.GetType() == type)
                {
					ActionStepTabPage tabPage = (ActionStepTabPage)ctrl;
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

			Type type = typeof(ActionStepTabPage);
			
            foreach (Control ctrl in tabControl.Controls)
            {
                if (ctrl.GetType() == type)
                {
					ActionStepTabPage tabPage = (ActionStepTabPage)ctrl;

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
				//splitter3.MinExtra = nWidth - 546;
				//splitter3.MinSize = 250;
				//splitter1.MinExtra = nWidth - 546;
				//splitter1.MinSize = m_nMinSplitPos;
			}
		}

		private void splitter3_MouseUp(object sender, MouseEventArgs e)
		{
			//if (splitter3.SplitPosition < 250)
			//{
			//	splitter3.SplitPosition = 250;
			//}
		}
		
		private void splitter3_SplitterMoved(object sender, SplitterEventArgs e)
		{

			//if (splitter3.SplitPosition < 250)
			//{
			//	splitter3.SplitPosition = 250;
			//}
		}

		private void splitter3_DoubleClick(object sender, EventArgs e)
		{
		}

		private void splitter3_SplitterMoving(object sender, SplitterEventArgs e)
		{
			//if (splitter3.SplitPosition < 250)
			//{
			//	splitter3.SplitPosition = 250;
			//}
		}



		private void splitter1_MouseUp(object sender, MouseEventArgs e)
		{
			if (splitter1.SplitPosition > m_nMaxSplitPos)
			{
				//splitter1.SplitPosition = m_nMaxSplitPos;
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
				m_nCurSplitPos = splitter1.SplitPosition;
			}
		}

		private void splitter1_DoubleClick(object sender, EventArgs e)
		{
			if (splitter1.SplitPosition != 0)
				splitter1.SplitPosition = 0;
			else
				splitter1.SplitPosition = m_nCurSplitPos;
		}

		private void splitter1_Move(object sender, EventArgs e)
		{
            if (splitter1.SplitPosition > m_nMaxSplitPos)
			{
				m_nCurSplitPos = splitter1.SplitPosition;
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
			splitter1.SplitPosition = m_nCurSplitPos;

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

					ActionStepTabPage page = (ActionStepTabPage)tabControl.SelectedTab;	
					m_propertiesLevel.SetTitleText(node.FullPath.Replace('\\', '/'));
					m_propertiesLevel.SetActionStep(page.Data);

				}
			}
			else
			{
				m_propertiesLevel.SetTitleText("SOP단계 속성");
				m_propertiesLevel.SetActionStep(null);				
				m_propertiesLevel.ClearSelection();				
			}          
        }

        private void tabControl_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LevelTabSelected();
        }
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
