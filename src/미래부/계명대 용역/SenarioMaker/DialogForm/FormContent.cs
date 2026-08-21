using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace UnE.SenarioMaker
{

    internal partial class FormContent : Form, ISectionListener, ISectionPageContainer
    {
        internal enum ShowOption
        {
            Text = 0,
            Expression = 1,
            Component = 2
        }

        public String SenarioTitle
        {
            get 
            {
                return mSenarioTitle.Text; 
            }
            set
            {
                mSenarioTitle.Text = value;

                SenarioManager.Instance.VersionName = value;

                this.Text = value;

            }
        }

        private int m_nSenarioType = 1;
        public int SenarioType
        {
            get 
            {
                return m_nSenarioType; 
            }
            
            set 
            {                
                m_nSenarioType = value;
                if (m_CurrentPane != null)
                {
                    m_CurrentPane.DisasterType = SenarioManager.ToDisasterType(m_nSenarioType);
                }
            }
        }

        private string m_szSenarioPath = "";
        public string SenarioPath
        {
            get { return m_szSenarioPath; }
            set { m_szSenarioPath = value; }
        }

        public PanelSectionEx SectionPanel
        {
            get { return m_CurrentPane; }
        }

        private ShowOption m_ShowOption = ShowOption.Component;
        public ShowOption ContentOption
        {
            get { return m_ShowOption; }
            set
            {
                m_ShowOption = value;
                ChangeVisibleOption(m_ShowOption);
            }
        }

        
        private PointF[] m_arrDragDropOrigin = null;

        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;
        
        public FormContent()
        {
            InitializeComponent();

            mSecionTab.UseCloseButton = false;
            SetSectionColor();
          
            SenarioTitle = "새시나리오";
            this.MouseWheel += new MouseEventHandler(FormContent_MouseWheel);
        }

        public TabPage OnAddActionStep(ActionStep step)
        {
            return AddTabPage(step);
        }

        public TabPage OnDeleteActionStep(ActionStep step)
        {
            if (step == null || step.Panel == null)
                return null;

            TabPage page = (TabPage)step.Panel.Parent;
            if( page != null)
            {
                if (mSecionTab.TabPages.Contains(page))
                    mSecionTab.TabPages.Remove(page);
            }
            return page;            
        }

        public TabPage OnShowActionStep(ActionStep step)
        {
            return ShowActionStep(step);
        }

        public ArrayList AddTabPage(ArrayList arActionStepList)
        {
            ArrayList arResult = new ArrayList();

            foreach(ActionStep step in arActionStepList)
            {
                TabPage page = AddTabPage(step);
                
                arResult.Add(page);
            }
            return arResult;
        }
        
        private TabPage AddTabPage(ActionStep actionStep)
        {
            TabPage page = new TabPage();
            page.Text = actionStep.StepName;
           
            PanelSectionEx mSectionPanel = CreateSection(actionStep.TeamName);
            mSectionPanel.Dock = DockStyle.Fill;
            page.Controls.Add(mSectionPanel);
            page.Tag = mSectionPanel;
            mSecionTab.TabPages.Add(page);

            mSectionPanel.DisasterType = SenarioManager.ToDisasterType(m_nSenarioType);

            mSectionPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SectionPanel_MouseMove);
            mSectionPanel.SetListener(this);

            mSectionPanel.Tag = actionStep;

            actionStep.Panel = mSectionPanel;

            return page;
        }    

        private Sections.PanelSectionEx CreateSection(string szName)
        {
            Sections.PanelSectionEx mSectionPanel = new Sections.PanelSectionEx();
            mSectionPanel.ActionStepID = -1;         
            mSectionPanel.ArrowSnapOn = true;
            mSectionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            mSectionPanel.Collapse = true;
            mSectionPanel.DisasterType = "";
            mSectionPanel.DragSelectMode = false;
            mSectionPanel.Editable = true;
            mSectionPanel.IsModified = false;
            mSectionPanel.Name = "SectionPanel";
            mSectionPanel.TeamID = 0;
            mSectionPanel.TeamName = szName;
            mSectionPanel.TeamType = 0;
            return mSectionPanel;
        }

        public ArrayList GetAllTabPages()
        {
            ArrayList ar = new ArrayList();
            ar.AddRange(mSecionTab.TabPages);
            return ar;
        }

        public TabControl GetTabContorl()
        {
            return mSecionTab;
        }

        public void RefreshContent()
        {
            if(m_CurrentPane != null)
                m_CurrentPane.Refresh();
        }

        public bool InitSectionPanel()
        {
            if (CheckModify())
            {
                DialogResult result = UnE.Utility.UMessageBox.Show("변경된 사항이 있습니다. 현재 시나리오를 저장하시겠습니까?", "저장 확인", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if( result == DialogResult.Yes)
                {
                    if(FormMain.Instance.SaveSenarioFile())
                    {                        
                        return true;
                    }                    
                }
                else if( result == DialogResult.No)
                {                    
                    return true;
                }

                return false;
            } 
            return true;
        }

        public void ClearData()
        {
            m_arrDragDropOrigin = null;
            m_sectionDragDropType = Section.ComponentType.NONE;

            if (m_CurrentPane != null)
            {
                m_CurrentPane.ClearSelection();
                m_CurrentPane.ClearData();            

                ClearModify();
                m_CurrentPane.Refresh();
            }

            mSecionTab.TabPages.Clear();
        }

        public void ClearModify()
        {
            bool bModified = false;

            ArrayList arActionSteps = SenarioManager.Instance.ActionStepList;
            foreach (ActionStep step in arActionSteps)
            {
                if (step.Panel.IsModified == true)
                {
					step.Panel.IsModified = bModified;
                }
            }
        }
        
        public bool CheckModify()
        {
            bool bModified = false;

            ArrayList arActionSteps = SenarioManager.Instance.ActionStepList;
            foreach(ActionStep step in arActionSteps)
            {
                if(step.Panel.IsModified == true)
                {
                    bModified = true;
                }
            }
            return bModified;
        }
        
        private void SetSectionColor()
        {
            EditBox.SetColor(true, Color.White);
            EditBox.SetColor(false, Color.FromArgb(60, 56, 71));

            Arrow.NormalPen.Color = Color.White;
            Arrow.TempLinePen.Color = Color.WhiteSmoke;
            Arrow.TriangleBrush.Color = Color.White;



            Arrow.TextFont = Properties.Settings.Default.ArrowFont;
            Arrow.TextBrush.Color = Color.WhiteSmoke;
            Sections.Shape.UseImage = false;

            
            SizeManager.MinSize = new Size(100, 40);
            SectionDecision.DefaultSize = new Size(200, 85);

            PanelSectionEx.EditableArrowText = false;
            PathNotifier.PathColor = Color.Purple;
        }

        public void ClearSelectionComponent()
        {
            FormSelectComponet form = FormMain.Instance.ComponentForm;
            form.ClearSelection();
        }

        private void FormContent_Load(object sender, EventArgs e)
        {
        }

        private void FormContent_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormContent_Resize(object sender, EventArgs e)
        {
        }

        public void SetDragDropShape(PointF[] arrDragDrop, Sections.Section.ComponentType sectionType)
        {
            m_arrDragDropOrigin = arrDragDrop;
            m_sectionDragDropType = sectionType;

            string szType = "새 프로세스";
            switch (sectionType)
            {
                case Section.ComponentType.ANNOTATION:
                    szType = "새 주석";
                    break;
                case Section.ComponentType.DECISION:
                    szType = "새 비교/판단";
                    break;
                case Section.ComponentType.ENDPOINT:
                    szType = "새 시작/종료";
                    break;
                case Section.ComponentType.PROCESS:                   
                    break;
                default:
                    return;
            }
            FormMain.Instance.SetStatusText(szType + " 컴포넌트 추가");
        }

        private void SectionPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_CurrentPane == null)
                return;

            if (m_arrDragDropOrigin == null)
            {
                m_CurrentPane.MoveDrawingArray(null, Sections.Section.ComponentType.NONE, 0, 0);
                return;
            }

            Point ptPanel = m_CurrentPane.Location;
            Size sizePanel = m_CurrentPane.Size;

            if (m_CurrentPane.Visible == true)
            {
                if (e.X >= 0 && e.X <= sizePanel.Width && e.Y >= 0 && e.Y <= sizePanel.Height)
                    m_CurrentPane.MoveDrawingArray(m_arrDragDropOrigin, m_sectionDragDropType, e.X, e.Y);
                else
                    m_CurrentPane.MoveDrawingArray(null, m_sectionDragDropType, 0, 0);
            }
        }
        
        void FormContent_MouseWheel(object sender, MouseEventArgs e)
        {
            if (m_CurrentPane == null)
                return;

            mWheelEndCheckTimer.Stop();
            mWheelEndCheckTimer.Enabled = false;

            Point ptTabBegin = Location;
            Rectangle rect = DisplayRectangle;

            int nPanelX = e.X - (ptTabBegin.X + rect.X);
            int nPanelY = e.Y - (ptTabBegin.Y + rect.Y);


            Point ptPanel = m_CurrentPane.Location;
            Size sizePanel = m_CurrentPane.Size;

            if (nPanelX >= ptPanel.X && nPanelX <= ptPanel.X + sizePanel.Width &&
                nPanelY >= ptPanel.Y && nPanelY <= ptPanel.Y + sizePanel.Height)
            {
                if (m_CurrentPane.Visible == true)
                {
                    m_CurrentPane.WheelMouse(nPanelX - ptPanel.X, nPanelY - ptPanel.Y, e.Delta);
                }
                
            }

            mWheelEndCheckTimer.Interval = 500;
            mWheelEndCheckTimer.Enabled = true;
            mWheelEndCheckTimer.Start();            
        }

        private void mWheelEndCheckTimer_Tick(object sender, EventArgs e)
        {
            if (m_CurrentPane != null)
            {
                mWheelEndCheckTimer.Stop();
                mWheelEndCheckTimer.Enabled = false;

                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                Point ptClient = m_CurrentPane.PointToClient(new Point(x, y));
                MouseEventArgs ex = new MouseEventArgs(MouseButtons.None, 0, ptClient.X, ptClient.Y, 0);

                SectionPanel_MouseMove(m_CurrentPane, ex);
            } 
        }

        public void OnSelectedArrow(Sections.Arrow arSelected)
        {
            System.Diagnostics.Debug.WriteLine(arSelected);
        }

        public void OnSelectedSection(Sections.Section secSelected)
        {
            if (secSelected != null)
            {
                int i = 0;
                i++;
                
            }
            System.Diagnostics.Debug.WriteLine(secSelected);
            FormProperties form = FormMain.Instance.PropertiesForm;
            if (form != null)
                form.SetComponent(secSelected);

			if (secSelected != null)
			{
				Sections.PanelSection panel = secSelected.GetParent();
				if (panel != null)
					panel.Focus();
			}
			
        }
        
        private Sections.PanelSectionEx m_CurrentPane = null;
        public void SetCurrentPanel(Sections.PanelSection panel)
        {
            m_CurrentPane = (PanelSectionEx)panel;

            ActionStep actionStep = (ActionStep)panel.Tag;
            if (actionStep != null)
            {
                SenarioManager.Instance.SelectActionStep(actionStep);                
            }

			if (m_CurrentPane != null)
				m_CurrentPane.Focus();
        }

        public void OnSelectedSectionList(ArrayList arSections)
        {
            if (arSections != null && arSections.Count > 0)
            {
                FormMain.Instance.SetStatusText(string.Format("{0}개 컴포넌트 선택됨",arSections.Count));
            }
            else
            {
                FormMain.Instance.SetStatusText("컴포넌트 선택 취소");
            }
        }

        private void ChangeVisibleOption(ShowOption option)
        {
            if (m_CurrentPane == null)
                return;

			string szDisplayOption = "Component";
            if (option == ShowOption.Text)
            {
                //m_CurrentPane.SetDisplayText("Text");
				szDisplayOption = "Text";
            }
            else if (option == ShowOption.Expression)
            {
                //m_CurrentPane.SetDisplayText("Expr");
				szDisplayOption = "Expr";
            }
            else
            {
                //m_CurrentPane.SetDisplayText("Component");
				szDisplayOption = "Component";					
            }
			
			ArrayList arPanes = SenarioManager.Instance.ActionStepList;
			foreach(ActionStep step in arPanes)
			{
				if( step != null && step.Panel != null)
					step.Panel.SetDisplayText(szDisplayOption);
			}
        }

        private void mSecionTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            i++;
        }

        private void mSecionTab_OnTabPageDeleted(object sender, UnE.Controls.TabControlExEventArgs e)
        {

        }

        private void mSecionTab_OnTabPageDeleting(object sender, UnE.Controls.TabControlExEventArgs e)
        {

        }

        private void mSecionTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage page = mSecionTab.SelectedTab;
            if (page == null)
            {
                m_CurrentPane = null;
                return;
            }

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)page.Tag;
            m_CurrentPane = panel;

            ActionStep actionStep = (ActionStep)panel.Tag;
            if( actionStep != null)
            {
                SenarioManager.Instance.SelectActionStep(actionStep);
            }


			if (m_CurrentPane != null)
				m_CurrentPane.Focus();
            FormMain.Instance.SetStatusText("시라니오 : " + page.Text);
        }

        public TabPage ShowActionStep(ActionStep step)
        {
            if (step == null || step.Panel == null)
                return null;

            TabPage page = (TabPage)step.Panel.Parent;
            if (page == null)
                return null;

            if(!mSecionTab.TabPages.Contains(page))
                mSecionTab.TabPages.Add(page);

            m_CurrentPane = step.Panel;
            mSecionTab.SelectedTab = page;
            return page;
        }
        
    }
}
