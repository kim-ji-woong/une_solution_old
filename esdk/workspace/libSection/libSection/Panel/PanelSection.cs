using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sections
{
    public partial class PanelSection : Panel
    {
        protected ArrayList m_arrSection = new ArrayList();

        protected Section m_sectionSelected = null;
        public Section SelectedSection
        {
            get { return m_sectionSelected; }
        }

        protected bool m_clickedLButton = false;
        protected Point m_ptClicked = new Point();
        protected PointF m_ptSelected = new PointF();

        private Button m_btnScroll = new Button();
        private int m_nScrollInit = -100;

        // 마우스 포인터가 움직였는가를 확인하기 위한 변수들
        protected Point m_ptPrevPos = new Point(-1, -1);
        protected Point m_ptCurrentPos = new Point();
        protected Point m_ptLastChecked = new Point(-2, -2);
        protected int m_nNoMoveCount = 0;

        protected Section m_sectionArrowPoint = null;
        protected ISectionListener m_listener = null;

        protected bool m_isEditable = false;

        // 자동 정렬시 사용되는 Section 간의 간격
        protected int m_nAutoPositioningSpaceX = 100;
        protected int m_nAutoPositioningSpaceY = 100;

        protected int m_nDrawingProcessOption = 0;
        // 0(그리지 않는다), 1(그린다), -1(그리지 않으며 객체를 저장한다)
        public int DrawingProcessOption
        {
            get { return m_nDrawingProcessOption; }
        }

        protected ArrayList m_arrProcessManagers = new ArrayList();
        public ArrayList ProcessManagers
        {
            get { return m_arrProcessManagers; }
        }

        protected bool m_arrowSnapOn = false;
        public bool ArrowSnapOn
        {
            get { return m_arrowSnapOn; }
            set { m_arrowSnapOn = value; }
        }

        protected int m_nActionStepID = -1;
        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        protected int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        protected Point m_ptDragStart;
        protected Point m_ptDragCurrent;
        protected bool m_bDragSelectMode = false;
        public bool DragSelectMode
        {
            get { return m_bDragSelectMode; }
            set { m_bDragSelectMode = value; }
        }

        protected string m_szTeamName = "";
        public string TeamName
        {
            get { return m_szTeamName; }
            set
            {
                m_szTeamName = value;
            }
        }

        protected bool m_visibleSectionNumber = false;
        public bool VisibleSectionNumber
        {
            get { return m_visibleSectionNumber; }
            set { m_visibleSectionNumber = value; }
        }

        protected ArrayList m_arrButtons = new ArrayList();
        public ArrayList Buttons
        {
            get { return m_arrButtons; }
        }


        public PanelSection()
        {
            InitializeComponent();

            InitHandler();

            // Panel의 Scroll 위치를 얻어오기 위한 Button
            InitScrollButton();

            timer1.Start();
        }

        public void SetListener(ISectionListener listener)
        {
            m_listener = listener;
        }

        protected virtual void InitHandler()
        {
            this.DoubleBuffered = true;

            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.Paint += new PaintEventHandler(OnPaint);
        }

        // Panel의 Scroll 위치를 얻어오기 위한 Button
        private void InitScrollButton()
        {
            this.Controls.Add(m_btnScroll);

            m_btnScroll.Location = new System.Drawing.Point(m_nScrollInit, 0);
            m_btnScroll.Name = "btnScroll";
            m_btnScroll.Size = new System.Drawing.Size(28, 23);
            m_btnScroll.TabIndex = 0;
            m_btnScroll.Text = "NotUse";
            m_btnScroll.UseVisualStyleBackColor = true;
        }



        protected virtual SectionButton SelectSectionButton(float x, float y)
        {
            ArrayList arSection = (ArrayList)m_arrButtons.Clone();
            arSection.Reverse();
            foreach (SectionButton section in arSection)
            {
                SectionButton secsionSelected = section.Select(x, y);

                if (secsionSelected != null)
                    return secsionSelected;
            }

            return null;
        }

        protected virtual Section SelectSection(float x, float y)
        {
            ArrayList arSection = (ArrayList)m_arrSection.Clone();
            arSection.Reverse();
            foreach (Section section in arSection)
            {
                Section secsionSelected = section.Select(x, y);

                if (secsionSelected != null)
                    return secsionSelected;
            }

            return null;
        }

        protected virtual bool SelectSectionRect(Rectangle rect)
        {
            PointF pt = ScreenToGlobal(new Point(rect.X, rect.Y));
            PointF pt2 = ScreenToGlobal(new Point(rect.X + rect.Width, rect.Y + rect.Height));

            float width = Math.Abs(pt2.X - pt.X);
            float height = Math.Abs(pt2.Y - pt.Y);

            if (width < 20.0f && height < 20.0f)
                return false;

            Rectangle rectf = new Rectangle((int)pt.X, (int)pt.Y, (int)width, (int)height);      

            if(m_sectionSelected != null)
            {
                if (!m_arSelectedSectionList.Contains(m_sectionSelected))
				    m_arSelectedSectionList.Add(m_sectionSelected);
                m_sectionSelected = null;
            }

            ArrayList arSection = (ArrayList)m_arrSection.Clone();
            arSection.Reverse();
            foreach (Section section in arSection)
            {
                Section selectedSection = section.Select(rectf);
                if (selectedSection != null)
			    {
				    if (!m_arSelectedSectionList.Contains(selectedSection))
				    {
                        selectedSection.MovingStartPosition = selectedSection.Position;
					    selectedSection.Select(true);
					    m_arSelectedSectionList.Add(selectedSection);
				    }			        
                }            
            }	

            if(m_arSelectedSectionList.Count == 1)
            {
                Section section = (Section)m_arSelectedSectionList[0];
                if (m_listener != null)
                    m_listener.OnSelectedSection(section);
            }
            return true;
        }

        public virtual bool RunSection(Section section, bool bRun)
        {
            return false;
        }

        public virtual int GetComponentID(Section section)
        {
            return -1;
        }

        public virtual string GetTitle()
        {
            return "";
        }
        
        public bool SelectSection(Section section)
        {
            foreach (Section _section in m_arrSection)
            {
                if (_section == section)
                {
                    section.Select(true);

                    if (m_listener != null)
                        m_listener.OnSelectedSection(section);
                    m_sectionSelected = section;
                    return true;
                }
            }

            return false;
        }

        // 화면 좌표를 Global 좌표로 변환
        public virtual void ScreenToGlobal(int xIn, int yIn, out float xOut, out float yOut)
        {
            throw new NotImplementedException();
        }

        // 화면 좌표를 Global 좌표로 변환
        public virtual PointF ScreenToGlobal(Point pt)
        {
            throw new NotImplementedException();
        }

        public virtual Point GlobalToScreen(PointF pt)
        {
            throw new NotImplementedException();
        }

        // Matrix를 사용
        public virtual Point GlobalToScreen2(PointF pt)
        {
            throw new NotImplementedException();
        }

        private bool m_bSectionSizeChangeMode = false;
        protected bool SectionSizeChangeMode
        {
            get { return m_bSectionSizeChangeMode; }
        }

        private bool m_bSectionMoveMode = false;
        public bool SectionMoveMode
        {
            get { return m_bSectionMoveMode; }
        }

        protected virtual void OnSectionBeginMove()
        {

        }

        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            m_ptCurrentPos.X = e.X;
            m_ptCurrentPos.Y = e.Y;

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);


            if (m_clickedLButton == true)
            {
                DateTime dtNow = DateTime.Now;
                dtNow = dtNow.AddMilliseconds(-130);
                if (dtNow < m_lastLClicked)
                    return;
                if (m_sectionSelected != null)
                {
                    m_sectionSelected.HideArrowPoint();

                    if (m_isEditable && m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)                    
                    {
                        if (m_bSectionSizeChangeMode == false)
                        {
                            OnSectionSizeChanging(m_sectionSelected,  new PointF(x, y));
                        }

                        m_bSectionSizeChangeMode = true;
                        OnSectionSizeChanged(m_sectionSelected, x, y);
                    }
                    else
                    {
                        if (m_bSectionMoveMode == false)
                        {
                            OnSectionBeginMove();
                        }
                        m_bSectionMoveMode = true;

                        PointF ptSelected = m_sectionSelected.MovingStartPosition;
                        PointF ptClicked = ScreenToGlobal(m_ptClicked);
                        float xMove = x - ptClicked.X;
                        float yMove = y - ptClicked.Y;

                        OnSectionMoving(m_sectionSelected, new PointF(ptSelected.X + xMove, ptSelected.Y + yMove));
                    
                        foreach(Section section in m_arSelectedSectionList)
                        {
                            if( section != m_sectionSelected)
                            {
                                PointF ptLocation = section.Position;
                              
                                PointF ptOrgLoc = section.MovingStartPosition;
                                OnSectionMoving(section, new PointF(ptOrgLoc.X + xMove, ptOrgLoc.Y + yMove));                        
                            }                         

                        }
                        Refresh();
                    }
                }
            }
            else
            {
                if (m_sectionSelected != null)
                {
                    m_sectionSelected.CheckMouse(x, y);
                }
                else
                    this.Cursor = Cursors.Arrow;
            }
        }

        protected virtual void OnSectionMoving(Section section, PointF pt)
        {
            section.Position = pt;
            m_bModify = true;
        }

        protected virtual void OnSectionSizeChanging(Section section, PointF pt)
        {
        }

        protected virtual void OnSectionMoved(Section section, PointF pt)
        {
            section.Position = pt;
            m_bModify = true;
        }

		public virtual void SectionMove(Section section, PointF pt)
		{
			OnSectionMoved(section, pt);
		}

        protected virtual void OnSectionSizeChanged(Section section, float x, float y)
        {
            section.ChangeSize(x, y);
            
            m_bModify = true;
        }

		protected ArrayList m_arSelectedSectionList = new ArrayList();
		public System.Collections.ArrayList SelectedSectionList
		{
			get { return m_arSelectedSectionList; }
		}

		protected void ClearSelectedSection()
		{
			foreach (Section section in m_arSelectedSectionList)
			{
				section.Select(false);
			}
			m_arSelectedSectionList.Clear();

            if (m_listener != null)
                m_listener.OnSelectedSection(null);
            
		}

        protected bool m_bDownButton = false;
        protected SectionButton m_SelectedBtn = null;

        private DateTime m_lastLClicked;
        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (m_listener != null)
                m_listener.SetCurrentPanel(this);

			if (e.Button == MouseButtons.Left && (Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (m_sectionSelected != null)
				{
                    if(!m_arSelectedSectionList.Contains(m_sectionSelected))
                    {
                        m_arSelectedSectionList.Add(m_sectionSelected);
                    }					
					m_sectionSelected = null;
				}

				float x, y;
				ScreenToGlobal(e.X, e.Y, out x, out y);

                SectionButton button = SelectSectionButton(x, y);
                if( button != null)
                {
                    m_bDownButton = true;
                    m_SelectedBtn = button;
                    return;
                }

				Section selectedSection = SelectSection(x, y);
				
				if (selectedSection != null)
				{
					if (!m_arSelectedSectionList.Contains(selectedSection))
					{
						selectedSection.MovingStartPosition = selectedSection.Position;
						selectedSection.Select(true);

						m_arSelectedSectionList.Add(selectedSection);

					}
					else
					{
						selectedSection.Select(false);
						m_arSelectedSectionList.Remove(selectedSection);
					}

                    if (m_listener != null)
                    {
                        ArrayList arList = (ArrayList)m_arSelectedSectionList.Clone();
                        m_listener.OnSelectedSectionList(arList);
                    }
					
					Refresh();
					return;
				}

                if (m_arSelectedSectionList.Count > 0)
                    return;
			}

            bool bClearSelect = true;
            if (m_arSelectedSectionList.Count > 0)
            {
                
                float x, y;
                ScreenToGlobal(e.X, e.Y, out x, out y);
                
                Section selectedSection = SelectSection(x, y);				
                if(selectedSection != null)
                {
                    if(m_arSelectedSectionList.Contains(selectedSection))
                    {
                        bClearSelect = false;
                    }
                }
            }

            if (bClearSelect == true)
                ClearSelectedSection();

            if (e.Button == MouseButtons.Left)
            {
                m_clickedLButton = true;
                m_lastLClicked = DateTime.Now;

                float x, y;
                ScreenToGlobal(e.X, e.Y, out x, out y);

                m_ptClicked.X = e.X;
                m_ptClicked.Y = e.Y;

                if (m_isEditable && m_sectionSelected != null && m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                {
                    m_sectionSelected.SetChangeSizeOriginPoint(x, y);
                }
                else
                {
                    Section section = SelectSection(x, y);

                    if (section != null)
                    {
                        m_ptSelected = section.Position;
                        section.MovingStartPosition = section.Position;

                        if (m_listener != null)
                            m_listener.OnSelectedSection(section);

                        if (m_sectionSelected == section)
                        { 
                            return;
                        }
                        else
                        {
                            if (m_sectionSelected != null)
                                m_sectionSelected.Select(false);

                            section.Select(true);
                            m_sectionSelected = section;
                            Refresh();
                        }
                    }
                    else
                    {
                        if (m_listener != null)
                            m_listener.OnSelectedSection(null);

                        if (m_sectionSelected != null)
                        {
                            m_sectionSelected.Select(false);
                            m_sectionSelected = null;
                            Refresh();
                        }
                    }
                }               
            }
        }

        protected virtual void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if( m_bDownButton == true)
                {
                    m_SelectedBtn.PerformClick(e.X, e.Y);
                    m_bDownButton = false;
                    m_SelectedBtn = null;
                }
       
                if (m_bSectionMoveMode == true)
                {
                    if (m_sectionSelected != null)
                    {
                        m_sectionSelected.MovingStartPosition = m_sectionSelected.Position;
                        OnSectionMoved(m_sectionSelected, m_sectionSelected.Position);
                    }
                    foreach (Section section in m_arSelectedSectionList)
                    {
                        if (section != m_sectionSelected)
                        {
                            section.MovingStartPosition = section.Position;
                            OnSectionMoved(section, section.Position);
                        }
                    }
                }

                m_bSectionSizeChangeMode = false;
                m_bSectionMoveMode = false;
                m_clickedLButton = false;

                //if (m_sectionSelected!= null)
                //{
                //    m_sectionSelected.MovingStartPosition = m_sectionSelected.Position;
                //}                
                //foreach (Section section in m_arSelectedSectionList)
                //{
                //    if (section != m_sectionSelected)
                //    {
                //        section.MovingStartPosition = section.Position;
                //    }
                //}
            }
        }

        public void ShowAllSectionButtons()
        {
            foreach(SectionButton btn in Buttons)
            {
                btn.Show();
                btn.Notify(true);
            }
        }

        public void HideBeginSectionButton()
        {
            foreach (SectionButton btn in Buttons)
            {
                if(btn.GetType() == typeof(ButtonEndPoint))
                {
                    SectionDataEndPoint data = (SectionDataEndPoint)btn.Data;
                    if( data != null)
                    {
                        if(data.IsBegin == true)
                        {
                            btn.Notify(false);
                            btn.Hide();
                        }
                    }
                }                
            }
        }

        public void HideAllSectionButtons()
        {
            foreach (SectionButton btn in Buttons)
            {              
                btn.Notify(false);
                btn.Hide();
            }
        }

        
        protected virtual void OnPaint(object sender, PaintEventArgs e)
        {         
            foreach (Section section in m_arrSection)
            {
                section.Draw(e.Graphics);
            }                

            // Section에 화살표가 가려지는 것을 막기 위하여 화살표를 나중에 따로 그린다.
            foreach (Section section in m_arrSection)
            {
                section.DrawArrow(e.Graphics);
            }

            foreach (SectionButton section in m_arrButtons)
            {
                section.Draw(e.Graphics);
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            foreach (Section section in m_arrSection)
            {
                section.Transform();
            }
            Refresh();
        }

        protected virtual Section CheckMouseOverSection(ref bool needRefresh)
        {
            Section sectionMouseOver = null;

            if (m_ptPrevPos == m_ptCurrentPos)
                m_nNoMoveCount++;
            else
            {
                if (m_sectionArrowPoint == null)
                    m_nNoMoveCount = 0;
                else
                {
                    if (m_sectionArrowPoint.InArrowArea(ScreenToGlobal(m_ptCurrentPos)))
                        sectionMouseOver = m_sectionArrowPoint;

                    if (sectionMouseOver == null)
                    {
                        m_nNoMoveCount = 0;
                        m_sectionArrowPoint.HideArrowPoint();
                        m_sectionArrowPoint = null;
                        needRefresh = true;
                    }
                    else
                        m_nNoMoveCount++;
                }
            }
            return sectionMouseOver;
        }

        protected virtual void OnTimer(object sender, EventArgs e)
        {
            if (m_isEditable)
            {
                // Mouse가 정지 상태로 그 위에 머물러 있는 Section이 있는지 검사한다.
                bool needRefresh = false;
                Section sectionMouseOver = CheckMouseOverSection(ref needRefresh);

                if (m_nNoMoveCount >= 1)
                {
                    if (m_ptCurrentPos != m_ptLastChecked)
                    {
                        PointF ptCurrentPos = ScreenToGlobal(m_ptCurrentPos);

                        if (sectionMouseOver == null)
                            sectionMouseOver = SelectSection(ptCurrentPos.X, ptCurrentPos.Y);

                        if (sectionMouseOver != null)
                        {
                            needRefresh = sectionMouseOver.NeedMouseOverRefresh(ptCurrentPos);
                            sectionMouseOver.ShowArrowPoint(ptCurrentPos);
                        }

                        if (m_sectionArrowPoint != null && m_sectionArrowPoint != sectionMouseOver)
                            m_sectionArrowPoint.HideArrowPoint();

                        if (m_sectionArrowPoint != sectionMouseOver)
                        {
                            m_sectionArrowPoint = sectionMouseOver;
                            needRefresh = true;
                        }
                        else if (sectionMouseOver != null)
                        {
                        }
                    }
                    m_ptLastChecked = m_ptCurrentPos;                    
                }
                else
                {
                    if (m_sectionArrowPoint != null)
                    {
                        m_sectionArrowPoint.HideArrowPoint();
                        m_sectionArrowPoint = null;
                        needRefresh = true;
                    }
                }

                if (needRefresh)
                    Refresh();

                m_ptPrevPos = m_ptCurrentPos;
            }
        }

        public virtual bool CompleteSection(Section section, int nDir = 0, bool refresh = true, Section sectionNext = null)
        {
            return false;
        }

        public ArrayList Sections
        {
            get { return m_arrSection; }
        }

        public bool Editable
        {
            get { return m_isEditable; }
            set { m_isEditable = value; }
        }

		private CollapseEventArgs m_prvEventArg = null;
		protected bool m_bCollapse = true;
		public bool Collapse
		{
			get { return m_bCollapse; }
			set { m_bCollapse = value; }
		}

		public virtual void OnCollapseChanged(CollapseEventArgs e)
		{
			m_bCollapse = e.Collapse;

			float dWidth = (e.Width) / 2.0f;
			float dHeight = (e.Height) / 2.0f;
			float dx = dWidth;
			float dy = dHeight;
			if (m_bCollapse == true)
			{
				dx *= -1.0f;
				dy *= -1.0f;
			}

			AutoPositioning();
			
			Refresh();
		}

		public void CollapseAllGroup()
		{
			foreach (Section section in m_arrSection)
			{
				if (section.GetComponentType() == Section.ComponentType.GROUP)
				{
					SectionGroup group = (SectionGroup)section;
					if (group.Collapse == false)
					{
						group.Collapse = true;
					}
				}
			}
		}


		protected bool m_bUseRefresh = true;
		public override void Refresh()
		{
			if (m_bUseRefresh == true)
				base.Refresh();
		}

        public void Invalidate()
        {
            Refresh();
        }

		public virtual void SuspendRefresh()
		{
			m_bUseRefresh = false;
		}
		public virtual void ResumeRefresh()
		{
			m_bUseRefresh = true;
		}

        protected bool m_bModify = false;
        public bool IsModified
        {
            get { return m_bModify; }
            set { m_bModify = value; }
        }

        public virtual void Delete()
        {            
        }

        public virtual void ClearSelection()
        {
        }


        #region 자동 정렬
        public void AutoPositioning()
        {
            ArrayList arrSections = new ArrayList();
            ArrayList arrSectionsCompleted = new ArrayList();

            foreach (Section section in m_arrSection)
            {
                if (!section.Hidden)
                {
                    Section.ComponentType type = section.GetComponentType();

                    if (type != Section.ComponentType.GROUP)
                    {
                        arrSections.Add(section);
                    }
                    else
                    {
                        SectionGroup group = (SectionGroup)section;

                        if (group.Collapse)
                            arrSections.Add(section);
                    }
                }
            }

            if (arrSections.Count == 0)
                return;

            PointF ptTR = new PointF();
            bool isFirst = true;

            while (arrSections.Count > 0)
            {
                Section sectionFirst = GetFirstSection(arrSections);

                if (isFirst)
                {
                    isFirst = false;
                    ptTR.X = sectionFirst.Position.X + sectionFirst.RectSize.Width;
                    ptTR.Y = sectionFirst.Position.Y;
                }
                else
                {
                    sectionFirst.Position = new PointF(ptTR.X + m_nAutoPositioningSpaceX, ptTR.Y);
                }

                if (sectionFirst == null)
                {
                    sectionFirst = (Section)arrSections[0];
                    arrSections.RemoveAt(0);
                }
                else
                    arrSections.Remove(sectionFirst);

                AutoPositioning(sectionFirst, arrSections, arrSectionsCompleted, ref ptTR);
            }

			foreach (Section section in m_arrSection)
			{
				if (section.GetComponentType() == Section.ComponentType.GROUP)
				{
					SectionGroup group = (SectionGroup)section;

					if (!group.Collapse)
						group.UpdateGroupBound();
				}
			}

            Refresh();
        }

        private Section GetFirstSection(ArrayList arrSections)
        {
            foreach (Section section in arrSections)
            {
                if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
                {
                    SectionDataEndPoint data = (SectionDataEndPoint)section.Data;

                    if (data.IsBegin)
                        return section;
                }
            }

            if (arrSections.Count == 0)
                return null;

            return (Section)arrSections[0];
        }

        private void AutoPositioning(Section section, ArrayList arrSections, ArrayList arrSectionsCompleted, ref PointF ptTR)
        {
            float fCenterX = section.Position.X + section.RectSize.Width / 2;
            float fCenterY = section.Position.Y + section.RectSize.Height / 2;

            arrSectionsCompleted.Add(section);

            foreach (Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink == section)
                {
                    if (arrow.EndLink == null || arrow.EndLink.Hidden)
                        continue;

                    if (!arrSections.Contains(arrow.EndLink))
                        continue;

                    PointF ptSection = new PointF();

                    if (!CalcSectionPoint(arrow, fCenterX, fCenterY, section, ref ptSection))
                        continue;

                    // 영역 중복검사를 실시하여 중복되지 않을때까지 nSpaceX만큼 오른쪽으로 옮긴다.
                    while (!CheckDuplicateSectionArea(arrSectionsCompleted, ptSection, arrow.EndLink.RectSize))
                    {
                        ptSection.X += m_nAutoPositioningSpaceX;
                    }
                    
                    bool movable = arrow.EndLink.Movable;
                    arrow.EndLink.Movable = true;
                    arrow.EndLink.Position = ptSection;
                    arrow.EndLink.Movable = movable;

                    /*if (arrow.EndLink.GroupMember)
                    {
                        SectionGroup group = (SectionGroup)arrow.EndLink.GroupSection;
                        group.UpdateGroupBound();
                    }		*/


                    float x = ptSection.X + arrow.EndLink.RectSize.Width;
                    float y = ptSection.Y;

                    if (ptTR.X < x)
                        ptTR.X = x;

                    if (ptTR.Y > y)
                        ptTR.Y = y;

                    //if (arrSections.Contains(arrow.EndLink))
                    {
                        arrSections.Remove(arrow.EndLink);
                        AutoPositioning(arrow.EndLink, arrSections, arrSectionsCompleted, ref ptTR);
                    }
                }
            }

            float _x = section.Position.X + section.RectSize.Width;
            float _y = section.Position.Y;

            if (ptTR.X < _x)
                ptTR.X = _x;

            if (ptTR.Y > _y)
                ptTR.Y = _y;
        }

        private bool CalcSectionPoint(Arrow arrow, float fCenterX, float fCenterY, Section section, ref PointF ptResult)
        {
            if (arrow.BeginPosition == Arrow.ArrowPosition.TOP)
            {
                if (arrow.EndPosition == Arrow.ArrowPosition.TOP)
                {
                    // section의 오른쪽에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = section.Position.Y;    
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.RIGHT)
                {
                    // section의 왼쪽 상단에 위치
                    ptResult.X = fCenterX - section.RectSize.Width / 2 - m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.BOTTOM)
                {
                    // section의 위쪽에 위치
                    ptResult.X = fCenterX - arrow.EndLink.RectSize.Width / 2;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.LEFT)
                {
                    // section의 오른쪽 상단에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else
                    return false;
            }
            else if (arrow.BeginPosition == Arrow.ArrowPosition.RIGHT)
            {
                if (arrow.EndPosition == Arrow.ArrowPosition.TOP)
                {
                    // section의 오른쪽 하단에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY + section.RectSize.Height / 2 + m_nAutoPositioningSpaceY;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.RIGHT)
                {
                    // section의 위쪽에 위치
                    ptResult.X = fCenterX - arrow.EndLink.RectSize.Width / 2;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.BOTTOM)
                {
                    // section의 오른쪽 상단에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.LEFT)
                {
                    // section의 오른쪽에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = section.Position.Y;
                }
                else
                    return false;
            }
            else if (arrow.BeginPosition == Arrow.ArrowPosition.BOTTOM)
            {
                if (arrow.EndPosition == Arrow.ArrowPosition.TOP)
                {
                    // section의 아래쪽에 위치
                    ptResult.X = fCenterX - arrow.EndLink.RectSize.Width / 2;
                    ptResult.Y = fCenterY + section.RectSize.Height / 2 + m_nAutoPositioningSpaceY;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.RIGHT)
                {
                    // section의 왼쪽 하단에 위치
                    ptResult.X = fCenterX - section.RectSize.Width / 2 - m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY + section.RectSize.Height / 2 + m_nAutoPositioningSpaceY;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.BOTTOM)
                {
                    // section의 오른쪽에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = section.Position.Y;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.LEFT)
                {
                    // section의 오른쪽 하단에 위치
                    ptResult.X = fCenterX + section.RectSize.Width / 2 + m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY + section.RectSize.Height / 2 + m_nAutoPositioningSpaceY;
                }
                else
                    return false;
            }
            else if (arrow.BeginPosition == Arrow.ArrowPosition.LEFT)
            {
                if (arrow.EndPosition == Arrow.ArrowPosition.TOP)
                {
                    // section의 왼쪽 하단에 위치
                    ptResult.X = fCenterX - section.RectSize.Width / 2 - m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY + section.RectSize.Height / 2 + m_nAutoPositioningSpaceY;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.RIGHT)
                {
                    // section의 왼쪽에 위치
                    ptResult.X = fCenterX - section.RectSize.Width / 2 - m_nAutoPositioningSpaceX;
                    ptResult.Y = section.Position.Y;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.BOTTOM)
                {
                    // section의 왼쪽 상단에 위치
                    ptResult.X = fCenterX - section.RectSize.Width / 2 - m_nAutoPositioningSpaceX;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else if (arrow.EndPosition == Arrow.ArrowPosition.LEFT)
                {
                    // section의 위쪽에 위치
                    ptResult.X = fCenterX - arrow.EndLink.RectSize.Width / 2;
                    ptResult.Y = fCenterY - section.RectSize.Height / 2 - m_nAutoPositioningSpaceY - arrow.EndLink.RectSize.Height;
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        // Return 값 : 영역이 중복되면 false
        //             중복되지 않으면 true를 리턴한다.
        private bool CheckDuplicateSectionArea(ArrayList arrSections, PointF ptSection, SizeF sizeSection)
        {
            RectangleF rectSrc = new RectangleF(ptSection, sizeSection);

            foreach (Section section in arrSections)
            {
                RectangleF rectTrg = new RectangleF(section.Position, section.RectSize);

                if (rectSrc.IntersectsWith(rectTrg))
                    return false;
            }

            return true;
        }
        #endregion
    }

	public class CollapseEventArgs
	{
		protected Section m_Target = null;
		public Sections.Section Target
		{
			get { return m_Target; }
			set { m_Target = value; }
		}

		protected float m_fWidth = 0.0f;
		public float Width
		{
			get { return m_fWidth; }
			set { m_fWidth = value; }
		}

		protected float m_fHeight = 0.0f;
		public float Height
		{
			get { return m_fHeight; }
			set { m_fHeight = value; }
		}

		protected PointF m_ptCenter;
		public System.Drawing.PointF Center
		{
			get { return m_ptCenter; }
			set { m_ptCenter = value; }
		}

		protected bool m_bCollapse = false;
		public bool Collapse
		{
			get { return m_bCollapse; }
			set { m_bCollapse = value; }
		}
	}
}
