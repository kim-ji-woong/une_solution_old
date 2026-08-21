using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnE.SenarioMaker;

namespace Sections
{
    public partial class PanelSectionEx : PanelSection
    {
        private PointF[] m_arrDragDropDrawing = null;
        private Section.ComponentType m_sectionDragDropType = Section.ComponentType.NONE;

        private string m_szDisasterType = "";
        public string DisasterType
        {
            get { return m_szDisasterType; }
            set { m_szDisasterType = value; }
        }

        // Panel 상단에 표시되는 팀 이름
        private string m_strTeamName = "";
        //private string m_strStepName = "";
        private int m_nTeamID = 0;
        // 0(평일 비상조직), 1(휴일 및 야간 비상조직), 2(사용자 정의 조직), 3(외부기관)
        private int m_nTeamType = 0;

        private Section m_sectionArrowLink = null;
        protected Arrow m_tempArrow = null;
        // m_tempArrow의 방향
        // true이면 시작 Section에서 끝 Section 쪽을 향하고 있으며,
        // false이면 끝 Section에서 시작 Section 쪽을 향하고 있다.
        protected bool m_tempArrowDirection = true;

        private Arrow m_arrowSelected = null;


        //////////////////////////////////////////////////////////////////////////
        //private float m_fTranslateX = 0.0f, m_fTranslateY = 0.0f;
        //private float m_fPrevOriginX = 0.0f, m_fPrevOriginY = 0.0f;
            
        private Point m_ptMClicked = new Point();

		//private float m_fScale = 1.0f;
		//private float m_fNewScale = 1.0f;
		////////////////////////////////////////////////////////////////////////////
		//private Point m_ptScrCenter;
        private Point m_ptCurrent;
        private Point m_ptPrev;
        private PointF m_ptOrigin;

        private float m_fTranX;
        private float m_fTranY;

        private float m_fPrevScale = 1.0f;
        private float m_fCurScale = 1.0f;
        private bool m_bTranslation = false;
        //////////////////////////////////////////////////////////////////////////

        // Snap을 사용하기 위한 최소 거리(화면 좌표)
        private int m_nSnapPixels = 10;
        // Snap을 사용하기 위한 최소 거리(Global 좌표)
        private float m_fSnapDistance = -1.0f;

        // Arrow의 텍스트를 입력받기 위한 임시 Control
        //private ZBobb.AlphaBlendTextBox m_tempArrowTextBox = new ZBobb.AlphaBlendTextBox();
        private TextBox m_tempArrowTextBox = new TextBox();
        /// <summary>
        /// 타이틀 바  
        /// </summary>
        private Label label = new Label();

        // 링크될 대상 객체를 찾고자 하는 객체
        private static SectionLink m_linkRequest = null;

        private new int m_nActionStepID = -1;
        
        public PanelSectionEx()
        {
            this.DoubleBuffered = true; 
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            InitializeComponent();

            Editable = true;

            m_tempArrowTextBox.Parent = this;
            m_tempArrowTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            m_tempArrowTextBox.Font = Arrow.TextFont;
            m_tempArrowTextBox.TextAlign = HorizontalAlignment.Center;
            m_tempArrowTextBox.Hide();

            ArrowSnapOn = true;

            if (tmrKeyMove.Enabled == false)
            {
               // tmrLazyDraw.Enabled = true;
               // tmrLazyDraw.Start();
            }
        }

        public void AddPanelTitle(string strTitle)
        {
                
            label.Dock = DockStyle.Top;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = Color.Navy;
            label.Text = strTitle;
            label.Font = new System.Drawing.Font("맑은고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            label.ForeColor = Color.White;
            Size size = label.Size;
            label.Size = new Size(size.Width, size.Height + 20);

            Controls.Add(label);
        }

        protected override void InitHandler()
        {
            base.InitHandler();

            this.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            m_tempArrowTextBox.KeyDown += new KeyEventHandler(ArrowTextBox_KeyDown);
            m_tempArrowTextBox.Leave += new EventHandler(ArrowTextBox_Leave);
        }

        private void ArrowTextBox_Leave(object sender, EventArgs e)
        {          

            string strText = m_tempArrowTextBox.Text;
            Arrow arrow = (Arrow)m_tempArrowTextBox.Tag;

            UndoRedoManager.Instance.SaveSnapshot("화살표 내용 편집");

            arrow.Select(false);
            arrow.Text = strText;
            m_arrowSelected = null;

            //m_tempArrowTextBox.Text = "";
            m_tempArrowTextBox.Hide();
            Refresh();
        }

        private void ArrowTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ArrowTextBox_Leave(null, null);
            }
        }


        private static bool m_bEditArrowText = true;
        public static bool EditableArrowText
        {
            get { return m_bEditArrowText; }
            set { m_bEditArrowText = value; }
        }

        void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // fDistance : 마우스 오차를 고려하여 Arrow를 선택하기 위한 최소 거리                   
                float fDistance = Arrow.SELECT_DISTANCE / m_fCurScale;
                float x, y;
                ScreenToGlobal(e.X, e.Y, out x, out y);

                if (SelectArrow(x,y , fDistance) == true)
                {
                    PointF pt;

                    if (m_arrowSelected.FindArrowMiddlePoint(out pt))
                    {
                        if(m_bEditArrowText == true)
                        {
                            Point ptPos2 = GlobalToScreen(pt);
                            m_tempArrowTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            m_tempArrowTextBox.BackColor = this.BackColor;
                            m_tempArrowTextBox.Font = Arrow.TextFont;
                            m_tempArrowTextBox.ForeColor = Arrow.TextBrush.Color;
                            m_tempArrowTextBox.Text = m_arrowSelected.Text;
                            m_arrowSelected.Text = "";
                            m_tempArrowTextBox.Show();
                            // comment by skkim 2013-01-09 : 잘못된 변환위치 제거
                            //Point ptPos = GlobalToScreen(pt);
                            Point ptPos = new Point(ptPos2.X, ptPos2.Y);
                            ptPos.X -= m_tempArrowTextBox.Size.Width / 2;
                            ptPos.Y -= m_tempArrowTextBox.Size.Height / 2;

                            m_tempArrowTextBox.Tag = m_arrowSelected;
                            // add by skkim 2013-01-09 : 연산후 변환적용
                            //Point ptResult = GlobalToScreen(new PointF(ptPos.X, ptPos.Y));
                            m_tempArrowTextBox.Location = ptPos;//ptResult;
                            m_tempArrowTextBox.Focus();

                            Refresh();
                        }

                    }
                }

				Section section = SelectSection(x, y);
				if (section != null && section.GetComponentType() == Section.ComponentType.GROUP)
				{
					SectionGroup groupSection = (SectionGroup)section;
					groupSection.Collapse = !groupSection.Collapse;
					Refresh();
				}
            }
        }

        protected void RemoveArrow(Arrow arrow)
        {
            if (arrow.BeginLink != null)
                arrow.BeginLink.RemoveArrow(arrow);
            else if (arrow.EndLink != null)
                arrow.EndLink.RemoveArrow(arrow);
        }

        protected void RemoveSection(Section section)
        {
            Section sectionParent = section.GetParentSection();

            if (sectionParent == null)
            {
                section.RemoveAllArrow();
                section.RemoveAllChild();
                m_arrSection.Remove(section);
            }
            else
            {
                sectionParent.RemoveChild(section);
            }
        }

        Bitmap mBackImage = null;
        Bitmap mBackImage2 = null;
        SolidBrush mBackBrush = new SolidBrush(Color.White);
        private Pen bBoundPen = new Pen(Color.FromArgb(253, 10, 10), 1);


        
        protected override void OnPaint(object sender, PaintEventArgs e)        
        {
            if (mBackImage == null || mBackImage2 == null)
                return;
            mBackBrush.Color = this.BackColor;            
            mBackImage = mBackImage2;
            
            //Graphics g = Graphics.FromImage(mBackImage2);

            if (m_bNeedEraseBkg == true && m_bProcessDrawing == false)
            {
                Graphics g2 = Graphics.FromImage(mBackImage2);
                g2.FillRectangle(mBackBrush, 0, 0, Size.Width, Size.Height);
                m_bNeedEraseBkg = false;


            }
            tmrLazyDraw_Tick();

            Graphics g = e.Graphics;
           
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(mBackImage, new Point(0, 0));
                      

            
            if (m_fSnapDistance < 0.0f)
                CalcSnapDistance();

           

            g.TranslateTransform(m_fTranX, m_fTranY);
            g.ScaleTransform(m_fCurScale, m_fCurScale);


            int nCount = m_arrSection.Count;

            if( nCount > 0)
            {
                int nLength = nCount - 1;
                Section section = (Section)m_arrSection[nLength];
                section.Draw(g);
            }

            if (m_tempArrow != null)
                m_tempArrow.DrawTemp( e.Graphics, ScreenToGlobal(m_ptCurrentPos));

            g.ResetTransform();

            if (m_arrDragDropDrawing != null && m_bExitMouse == false)
                g.DrawPolygon(bBoundPen, m_arrDragDropDrawing);
            
            if( m_bDragSelectMode == true)
            {
                int mMinX  = Math.Min(m_ptDragStart.X, m_ptDragCurrent.X);
                int mMaxX  = Math.Max(m_ptDragStart.X, m_ptDragCurrent.X);
                
                int mMinY  = Math.Min(m_ptDragStart.Y, m_ptDragCurrent.Y);
                int mMaxY  = Math.Max(m_ptDragStart.Y, m_ptDragCurrent.Y);

                Rectangle rect = new Rectangle(mMinX, mMinY, mMaxX - mMinX, mMaxY - mMinY);
                g.DrawRectangle(bBoundPen, rect);
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            

            if (sender != this)
            {
                try
                {
                    Bitmap bitmap = new Bitmap(label.Size.Width, label.Size.Height);
                    label.DrawToBitmap(bitmap, new Rectangle(0, 0, label.Size.Width, label.Size.Height));
                    g.DrawImage((Image)bitmap, 0, 0);
                }
                catch (Exception)
                {
                }
            }
           
           

            
        }

        protected void OnLazyPaint(int nCount1, int nCount2 , bool bArrow)
        {
            mBackBrush.Color = this.BackColor;
            
            if (m_fSnapDistance < 0.0f)
                CalcSnapDistance();
            Graphics g = Graphics.FromImage(mBackImage2);
            g.TranslateTransform(m_fTranX, m_fTranY);
            g.ScaleTransform(m_fCurScale, m_fCurScale);

           
            if (bArrow == false)
            {
                Debug.WriteLine("" + nCount1 + ", " + nCount2);

                for (int i = nCount1; i < nCount2; i++)
                {
                    Section section = (Section)m_arrSection[i];
                    section.Draw(g);
                    
                }
            }

            // Section에 화살표가 가려지는 것을 막기 위하여 화살표를 나중에 따로 그린다.
            if (bArrow == true)
            foreach (Section section in m_arrSection)
            {
                section.DrawArrow(g);
            }

            g.ResetTransform();

        }

        private bool m_bNeedEraseBkg = false;
        private bool m_bProcessDrawing = false;
        private void tmrLazyDraw_Tick()
        {
            tmrKeyMove.Enabled = false;
            tmrKeyMove.Stop();

            if (m_bProcessDrawing == false)
            {
                m_bProcessDrawing = true;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                
                int nLength = m_arrSection.Count;
                if( nLength > 1)
                {
                    int nNum = nLength / 100;
                    int nLast = nLength % 100;

                    int nStart = 0;
                    int nEnd = 0;
                    for (int i = 0; i < nNum; i++)
                    {
                        nEnd = 100 * (i + 1) ;
                        nStart = 100 * (i);
                        OnLazyPaint(nStart, nEnd, false);
                        mBackImage = mBackImage2;
                    }

                    OnLazyPaint(nEnd , nEnd + nLast, false);
                    mBackImage = mBackImage2;
                }               

                OnLazyPaint(0,nLength, true);
                mBackImage = mBackImage2;
                               
                m_bProcessDrawing = false;
                sw.Stop();
                Debug.WriteLine("OnPaint: " + sw.ElapsedMilliseconds.ToString() + "ms");

            }

           

        }

        public override void ScreenToGlobal(int x, int y, out float gx, out float gy)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            gx = (x / m_fCurScale - dx);
            gy = (y / m_fCurScale - dy);
        }

        public override PointF ScreenToGlobal(Point pt)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            float gx = (pt.X / m_fCurScale - dx);
            float gy = (pt.Y / m_fCurScale - dy);

            return new PointF(gx, gy);
        }

        public override Point GlobalToScreen(PointF pt)
        {
            int x = (int)(pt.X * m_fCurScale + m_fTranX);
            int y = (int)(pt.Y * m_fCurScale + m_fTranY);
            return new Point(x , y);
        }

        public void MoveDrawingArray(PointF[] arrDragDropOrigin, Section.ComponentType type, float x, float y)
        {
            if (arrDragDropOrigin == null)
            {
                if (m_arrDragDropDrawing == null)
                    return;

                m_arrDragDropDrawing = null;
                Refresh();
                return;
            }

            int nPointCount = arrDragDropOrigin.Length;
            if (nPointCount == 0)
            {
                if (m_arrDragDropDrawing == null)
                    return;

                m_arrDragDropDrawing = null;
                Refresh();
                return;
            }

            if (m_arrDragDropDrawing == null || m_arrDragDropDrawing.Count() != nPointCount)
                m_arrDragDropDrawing = new PointF[nPointCount];


            ScaleDragDropDrawing(arrDragDropOrigin, type, x, y);

            m_sectionDragDropType = type;       
                

            Invalidate();
        }

        private void ScaleDragDropDrawing(PointF[] arrDragDropOrigin, Section.ComponentType type, float fx, float fy)
        {
            if (m_arrDragDropDrawing != null)
            {
                int nCount = arrDragDropOrigin.Length;
                for( int i = 0 ; i < nCount ; i++)
                {
                    m_arrDragDropDrawing[i].X = arrDragDropOrigin[i].X * m_fCurScale + fx;
                    m_arrDragDropDrawing[i].Y = arrDragDropOrigin[i].Y *  m_fCurScale + fy;
                }                         
            }
        }

        private bool DragNDropState(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && m_arrDragDropDrawing != null && m_sectionDragDropType != Section.ComponentType.NONE)
                return true;
            return false;
        }

        private bool TempArrowState(MouseEventArgs e, ref Arrow.ArrowPosition pos)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return false;

            if (m_sectionArrowPoint == null)
                return false;

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);

            pos = m_sectionArrowPoint.GetArrowStartPosition(x, y);
            return pos != Arrow.ArrowPosition.NONE;
        }

        // 화살표를 위치 이동할 시점인가?
        private bool MovingArrow(MouseEventArgs e)
        {
            if (m_arrowSelected == null)
                return false;

            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return false;

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);

            int nResult = m_arrowSelected.IsMovingPoint(x, y);
            if (nResult == 0)
                return false;

            if (nResult > 0)
                m_tempArrowDirection = true;
            else
                m_tempArrowDirection = false;

            return true;
        }

        private bool EditingArrowText()
        {
            return m_tempArrowTextBox.Visible;
        }

        // 리턴값 : 마우스 오차를 고려하여 Arrow를 선택하기 위한 최소 거리
        protected float GetArrowDistance()
        {
            return Arrow.SELECT_DISTANCE / m_fCurScale;
        }

        protected bool SelectSectionNArrow(float x, float y)
        {
            Section section = SelectSection(x, y);

            if (section == null)
            {
                // fDistance : 마우스 오차를 고려하여 Arrow를 선택하기 위한 최소 거리
                //float fDistance, fY;
                //ScreenToGlobal(Arrow.SELECT_DISTANCE, 0, out fDistance, out fY);
                float fDistance = GetArrowDistance();

                Arrow arrowSelected = m_arrowSelected;

                if (SelectArrow(x, y, fDistance))
                {
                    if (m_sectionSelected != null)
                        m_sectionSelected.Select(false);
                    m_sectionSelected = null;
                    return true;
                }
                else
                    m_arrowSelected = arrowSelected;

                return false;
            }

            if (section == m_sectionSelected)
                return false;

            section.MovingStartPosition = section.Position;

            if (m_sectionSelected != null)
                m_sectionSelected.Select(false);

            if (m_listener != null)
                m_listener.OnSelectedSection(section);

            section.Select(true);
            m_sectionSelected = section;
            Refresh();

            return true;
        }

        // nPos : 0이면 가운데, 0보다 작으면 제일 왼쪽, 0보다 크면 제일 오른쪽에 현재 Panel이 위치한다.
        private int GetPanelCount(out int nPos)
        {
            nPos = -1;
            TabPage tabPage = (TabPage)this.Parent;

            if (tabPage == null)
                return 1;

            int nPanelCount = 0, nCurrentIndex = -1;
            Type type = typeof(PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    if ((PanelSectionEx)ctrl == this)
                        nCurrentIndex = nPanelCount;
                    nPanelCount++;
                }
            }

            if (nCurrentIndex == nPanelCount - 1)
                nPos = 1;
            else if (nCurrentIndex > 0 && nCurrentIndex < nPanelCount - 1)
                nPos = 0;

            return nPanelCount;
        }

        private void PopupSortPanel(int x, int y)
        {
            int nPos;
            int nPanelCount = GetPanelCount(out nPos);

            int nCurrentIndex = GetTabPagePanelIndex(this, (TabPage)this.Parent);
            if (nCurrentIndex < 0)
                return;

            bool enableDelete = true;

            if (nPanelCount == 1)
            {
                panelContextMenuStrip.Items[0].Enabled = false;
                panelContextMenuStrip.Items[1].Enabled = false;
                panelContextMenuStrip.Items[2].Enabled = true;
                panelContextMenuStrip.Items[3].Enabled = true;
                panelContextMenuStrip.Items[4].Enabled = false;
            }
            else if (nPos < 0)
            {
                panelContextMenuStrip.Items[0].Enabled = false;
                panelContextMenuStrip.Items[1].Enabled = true;
                panelContextMenuStrip.Items[2].Enabled = true;
                panelContextMenuStrip.Items[3].Enabled = true;
                panelContextMenuStrip.Items[4].Enabled = enableDelete;
            }
            else if (nPos > 0)
            {
                panelContextMenuStrip.Items[0].Enabled = true;
                panelContextMenuStrip.Items[1].Enabled = false;
                panelContextMenuStrip.Items[2].Enabled = true;
                panelContextMenuStrip.Items[3].Enabled = true;
                panelContextMenuStrip.Items[4].Enabled = enableDelete;
            }
            else
            {
                panelContextMenuStrip.Items[0].Enabled = true;
                panelContextMenuStrip.Items[1].Enabled = true;
                panelContextMenuStrip.Items[2].Enabled = true;
                panelContextMenuStrip.Items[3].Enabled = true;
                panelContextMenuStrip.Items[4].Enabled = enableDelete;
            }

            panelContextMenuStrip.Show(this, x, y);
        }

        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();

            Arrow.ArrowPosition pos = Arrow.ArrowPosition.NONE;

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);
                
            if (EditingArrowText())
            {
                m_bModify = true;
                ArrowTextBox_Leave(null, null);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {

				if (SelectedSectionList.Count > 0)
				{
                    toolStripMenuDelete.Enabled = true;
                    toolStripMenuCut.Enabled = true;
                    toolStripMenuCopy.Enabled = true;

                    if (SectionClipboardEx.Instance.EditSectionCount == 0)
                    {
                         toolStripMenuPaste.Enabled = false;
                    }
                    else
                    {
                        toolStripMenuPaste.Enabled = true;                        
                    } 
                    popupContextMenuStrip.Show(this, e.X, e.Y);

				}
				else
				{
					SelectSectionNArrow(x, y);

					if (m_arrDragDropDrawing != null && m_sectionDragDropType != Section.ComponentType.NONE)
					{
                        FormContent form = (FormContent)(this.Parent.Parent.Parent);
                        form.SetDragDropShape(null, Section.ComponentType.NONE);
                        form.ClearSelectionComponent();
						m_arrDragDropDrawing = null;
					}
					else if (m_sectionSelected != null)
					{
						if (m_sectionSelected.GetComponentType() == Section.ComponentType.LINK)
							linkContextMenuStrip.Show(this, e.X, e.Y);
						else if (m_sectionSelected.GetComponentType() == Section.ComponentType.GROUP)
						{
							groupContextMenuStrip.Items[2].Enabled = false;
							groupContextMenuStrip.Items[3].Enabled = true;
							groupContextMenuStrip.Show(this, e.X, e.Y);
						}
						else
						{
							bool bGroupMember = m_sectionSelected.GroupMember;
                            if (bGroupMember != true)
                            {
                                if( m_sectionSelected.GetComponentType() == Section.ComponentType.PROCESS ||
                                    m_sectionSelected.GetComponentType() == Section.ComponentType.DECISION)
                                {

                                    bool bShowExpr = m_sectionSelected.Data.ShowExpression;
                                    if(bShowExpr == true)
                                    {
                                        toolStripMenuItemShowType.Text = "[내용]으로 표시";
                                    }
                                    else
                                    {
                                        toolStripMenuItemShowType.Text = "[수식]으로 표시";
                                    }
                                    editContextMenuStrip.Show(this, e.X, e.Y);
                                }
                                else if(m_sectionSelected.GetComponentType() == Section.ComponentType.ANNOTATION)
                                {
                                    editTextContextMenuStrip.Show(this, e.X, e.Y);
                                }
                                else if(m_sectionSelected.GetComponentType() == Section.ComponentType.ENDPOINT)
                                {
                                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)m_sectionSelected.Data;
                                    if (data != null)
                                    {
                                        if(data.IsBegin == true)
                                        {
                                            endTypeToolStripMenuItem.Text = "[종료] 타입으로 변경";
                                        }
                                        else
                                        {
                                            endTypeToolStripMenuItem.Text = "[시작] 타입으로 변경";
                                        }
                                        
                                        endPointContextMenuStrip.Show(this, e.X, e.Y);
                                    }
                                    
                                }
                                else
                                    popupContextMenuStrip.Show(this, e.X, e.Y);
                            }
                            else
                            {
                                
                            }
						}
					}
                    else if (m_arrowSelected != null)
                    {
                        arrowPopupContextMenuStrip.Tag = m_arrowSelected;
                        arrowPopupContextMenuStrip.Show(this, e.X, e.Y);
                    }
                    else
                    {
                        if (SelectedSectionList.Count > 0)
                        {
                            toolStripMenuDelete.Enabled = true;
                            toolStripMenuCut.Enabled = true;
                            toolStripMenuCopy.Enabled = true;
                            if (SectionClipboardEx.Instance.EditSectionCount == 0)
                            {
                                toolStripMenuPaste.Enabled = false;
                            }
                            else
                            {
                                toolStripMenuPaste.Enabled = true;
                            }
                        }
                        else
                        {
                            toolStripMenuDelete.Enabled = false;
                            toolStripMenuCut.Enabled = false;
                            toolStripMenuCopy.Enabled = false;
                            if (SectionClipboardEx.Instance.EditSectionCount == 0)
                            {
                                toolStripMenuPaste.Enabled = false;
                            }
                            else
                            {
                                toolStripMenuPaste.Enabled = true;
                            }
                        }
                        popupContextMenuStrip.Show(this, e.X, e.Y);
                    }
				}
                
            }
            else if (DragNDropState(e))
            {
                if (CreateSection(x, y, m_sectionDragDropType) != null)
                {
                    if (m_sectionSelected != null)
                    {
                        m_sectionSelected.Select(false);
                        m_sectionSelected = null;
                    }

                    if (m_arrowSelected != null)
                    {
                        m_arrowSelected.Select(false);
                        m_arrowSelected = null; 
                    }

                    m_bModify = true;
                    Refresh();
                    
                }
            }
            else if (TempArrowState(e, ref pos))
            {
                m_tempArrow = new Arrow();
                m_tempArrow.BeginLink = m_sectionArrowPoint;
                m_tempArrow.BeginPosition = pos;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_ptMClicked.X = e.X;
                m_ptMClicked.Y = e.Y;

                m_bTranslation = true;
                m_ptPrev.X = e.X;
                m_ptPrev.Y = e.Y;
				m_ptCurrent = e.Location;
            }
            else if (MovingArrow(e))
            {
                Section sectionBegin = m_arrowSelected.BeginLink;
                Section sectionEnd = m_arrowSelected.EndLink;

                if (sectionBegin != null && sectionEnd != null)
                {
                    m_tempArrow = new Arrow();

                    if (m_tempArrowDirection)
                    {
                        m_tempArrow.BeginLink = m_arrowSelected.BeginLink;
                        m_tempArrow.BeginPosition = m_arrowSelected.BeginPosition;
                    }
                    else
                    {
                        m_tempArrow.BeginLink = m_arrowSelected.EndLink;
                        m_tempArrow.BeginPosition = m_arrowSelected.EndPosition;
                    }

                    sectionBegin.RemoveArrow(m_arrowSelected);
                    m_bModify = true;
                    m_arrowSelected = null;
                }
            }
            else
            {
                base.OnMouseDown(sender, e);

                if (m_sectionSelected == null)
                {
                    // fDistance : 마우스 오차를 고려하여 Arrow를 선택하기 위한 최소 거리
                    //float fDistance, fY;
                    //ScreenToGlobal(Arrow.SELECT_DISTANCE, 0, out fDistance, out fY);
                    float fDistance = GetArrowDistance();

                    if(!SelectArrow(x, y, fDistance))
                    {
                        m_bDragSelectMode = true;
                        m_ptDragStart = new Point(e.X, e.Y);
                        m_ptDragCurrent = new Point(e.X, e.Y);
                    }
                }
                else
                {
                    CheckLinkRequest(m_sectionSelected);

                    if (m_arrowSelected != null)
                    {
                        m_arrowSelected.Select(false);
                        m_arrowSelected = null;
                    }
                }
            }

            Invalidate();
        }

        private void CheckLinkRequest(Section section)
        {
            if (section == null || m_linkRequest == null)
                return;

            // 같은 패널 내에서는 링크될 수 없다.
            if (section.GetParent() == m_linkRequest.GetParent())
                return;

            Section.ComponentType type = section.GetComponentType();

            if (type != Section.ComponentType.ANNOTATION && type != Section.ComponentType.LINK
                && type != Section.ComponentType.NONE)
            {
                SectionDataLink data = (SectionDataLink)m_linkRequest.Data;
                data.LinkedSection = section;
                m_linkRequest = null;
            }
        }

        private bool SelectArrow(float x, float y, float fSelectDistance, bool includeText = false)
        {
            foreach (Section section in m_arrSection)
            {
                Arrow arrow = section.SelectArrow(x, y, fSelectDistance, includeText);

                if (arrow != null)
                {
                    if (m_arrowSelected == arrow)
                        return true;

                    m_arrowSelected = arrow;
                    m_arrowSelected.Select(true);

                    if (m_listener!= null)
                        m_listener.OnSelectedArrow(m_arrowSelected);
                    Refresh();
                    return true;
                }
            }

            if (m_arrowSelected != null)
            {
                m_arrowSelected.Select(false);
                m_arrowSelected = null;
                Refresh();
            }

            return false;
        }

        private bool LinkSection(float x, float y)
        {
            if (m_tempArrow == null)
                return false;

            if (m_sectionArrowLink == null)
            {
                m_tempArrow = null;
                return false;
            }

            Arrow.ArrowPosition pos = m_sectionArrowLink.GetArrowStartPosition(x, y);

            if (pos == Arrow.ArrowPosition.NONE)
            {
                m_tempArrow = null;
                m_sectionArrowLink.HideArrowPoint();
                m_sectionArrowLink = null;
                Refresh();
                return false;
            }

            if (m_tempArrowDirection)
            {
                m_tempArrow.EndLink = m_sectionArrowLink;
                m_tempArrow.EndPosition = pos;
            }
            else
            {
                m_tempArrow.EndLink = m_tempArrow.BeginLink;
                m_tempArrow.EndPosition = m_tempArrow.BeginPosition;
                m_tempArrow.BeginLink = m_sectionArrowLink;
                m_tempArrow.BeginPosition = pos;

                m_tempArrowDirection = true;
            }

            Section sectionBegin = m_tempArrow.BeginLink;
            Section sectionEnd = m_tempArrow.EndLink;
            bool result = false;

            if (sectionBegin.IsAddableArrow(m_tempArrow))
            {
                if (sectionEnd.IsAddableArrow(m_tempArrow))
                {
                    UndoRedoManager.Instance.SaveSnapshot("화살표 연결");
                    result = true;
                    sectionBegin.AddArrow(m_tempArrow);
                    sectionEnd.AddArrow(m_tempArrow);
                    m_tempArrow.CalcArrowLine();       
                }                
            }

            m_tempArrow = null;
            m_sectionArrowLink.HideArrowPoint();
            m_sectionArrowLink = null;

            return result;
        }

        protected override void OnSectionMoving(Section section, PointF pt)
        {            
            UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");            
            base.OnSectionMoving(section, pt);
        }

        protected override void OnSectionSizeChanging(Section section, PointF pt)
        {
            UndoRedoManager.Instance.SaveSnapshot("컴포넌트 크기 변경");
            base.OnSectionSizeChanging(section, pt);
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (LinkSection(x, y))
                {
                    m_bModify = true;
                    Refresh();
                }
            }
            if (e.Button == MouseButtons.Middle)
            {
                if (m_bTranslation == true)
                {
                    m_bTranslation = false;
                }
            }

            if (m_bDragSelectMode == true)
            {
                m_bDragSelectMode = false;
                int mMinX = Math.Min(m_ptDragStart.X, m_ptDragCurrent.X);
                int mMaxX = Math.Max(m_ptDragStart.X, m_ptDragCurrent.X);

                int mMinY = Math.Min(m_ptDragStart.Y, m_ptDragCurrent.Y);
                int mMaxY = Math.Max(m_ptDragStart.Y, m_ptDragCurrent.Y);

                Rectangle rect = new Rectangle(mMinX, mMinY, mMaxX - mMinX, mMaxY - mMinY);

                if( SelectSectionRect(rect))
                {
                    if (m_listener != null)
                    {
                        ArrayList arList = (ArrayList)SelectedSectionList.Clone();
                        m_listener.OnSelectedSectionList(arList);
                    }
                }
                Refresh();
            }            
        }

        private void Translate(int prevX, int prevY, int x, int y)
        {
            m_ptOrigin.X += (x - prevX);
            m_ptOrigin.Y += (y - prevY);

            m_fTranX = m_ptOrigin.X;
            m_fTranY = m_ptOrigin.Y;

            m_bNeedEraseBkg = true;
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);
			
            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (m_bTranslation == true)
                {
					m_ptCurrent.X = e.X;
					m_ptCurrent.Y = e.Y;

                    Translate(m_ptPrev.X, m_ptPrev.Y, e.X, e.Y);

					m_ptPrev = m_ptCurrent;
                    Invalidate();
                }
            }
            else if (m_tempArrow != null)
            {
                bool arrowLinkSection = CheckArrowLinkSection(x, y);

                if (!arrowLinkSection && m_sectionArrowLink != null)
                {
                    m_sectionArrowLink.HideArrowPoint();
                    m_sectionArrowLink = null;
                }

                Refresh();
            }
            if (m_bDragSelectMode)
            {
                m_ptDragCurrent = new Point(e.X, e.Y);
                Refresh();
            }
        }

        private bool CheckArrowLinkSection(float x, float y, ArrayList arrSections = null, int nDepth = 1)
        {
            if (arrSections == null)
                arrSections = m_arrSection;

            // 실제 Section 크기보다 10만큼 더 큰 영역을 잡는다.
            int nExtraSize = 10;

            foreach (Section section in arrSections)
            {
                PointF pt = section.Position;
                SizeF size = section.RectSize;

                if (x >= pt.X - nExtraSize && x <= pt.X + size.Width + nExtraSize &&
                    y >= pt.Y - nExtraSize && y <= pt.Y + size.Height + nExtraSize)
                {
                    if (m_sectionArrowLink != null)
                    {
                        if (m_sectionArrowLink != section)
                            m_sectionArrowLink.HideArrowPoint();
                    }

                    m_sectionArrowLink = section;
                    m_sectionArrowLink.ShowArrowPoint(new PointF(x, y));
                    m_bModify = true;
                    return true;
                }

                ArrayList arrChilds = section.GetChildSections();

                if (arrChilds != null)
                {
                    if (CheckArrowLinkSection(x, y, arrChilds, nDepth + 1))
                    {
                        m_bModify = true;
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void OnTimer(object sender, EventArgs e)
        {
            if (m_tempArrow != null)
                return;

            base.OnTimer(sender, e);
                
        }

        private Section CreateSection(float x, float y, Section.ComponentType type)
        {
            Section section = null;

			if (type == Section.ComponentType.PROCESS)
				section = new SectionProcess(this, x, y);
			else if (type == Section.ComponentType.DECISION)
				section = new SectionDecision(this, x, y);
			else if (type == Section.ComponentType.ANNOTATION)
				section = new SectionAnnotation(this, x, y);
			else if (type == Section.ComponentType.ENDPOINT)
				section = new SectionEndPoint(this, x, y);
			else if (type == Section.ComponentType.LINK)
				section = new SectionLink(this, x, y);
			else if (type == Section.ComponentType.TRANSSOP)
				section = new SectionTransSOP(this, x, y);
			else if (type == Section.ComponentType.INTERNAL)
				section = new SectionInternal(this, x, y);
			else if (type == Section.ComponentType.EXTERNAL)
				section = new SectionExternal(this, x, y);
			else if (type == Section.ComponentType.TRANSMISSION)
				section = new SectionTransmission(this, x, y);
			else if (type == Section.ComponentType.GROUP)
				section = new SectionGroup(this, x, y);
			else				
                return null;

            section.MakeData(StepName, m_strTeamName);

            // Test Notify
            //section.Notify(true);

            UndoRedoManager.Instance.SaveSnapshot("새 컴포넌트 추가");

			Sections.Add(section);

            m_bModify = true;

            return section;
        }

        public void WheelMouse(int x, int y, int nDelta)
        {
            if (nDelta > 0)
                ZoomIn(x, y);
            else
                ZoomOut(x, y);			

	
			//if(m_ptCurrent.X == 0 && m_ptCurrent.Y == 0)
			//{
			//	m_ptCurrent.X = x;
			//	m_ptCurrent.Y = y;
			//}
			//m_ptPrev = m_ptCurrent;
			//m_ptCurrent.X = x;
			//m_ptCurrent.Y = y;

            m_bNeedEraseBkg = true;
        }

        private void ZoomIn(int x, int y)
        {
            if (m_fCurScale <= 10.0f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);

                m_fCurScale = m_fCurScale * 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;

                m_fTranX += dx;
                m_fTranY += dy;

				//m_ptCurrent.X += (int)(dx / m_fCurScale + 0.5f);
				//m_ptCurrent.Y += (int)(dy / m_fCurScale + 0.5f);

                m_fPrevScale = m_fCurScale;
                CalcSnapDistance();
                
                Refresh();
            }
        }


        private void ZoomOut(int x, int y)
        {
            if (m_fCurScale > 0.01f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);

                m_fCurScale = m_fCurScale / 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;
				
                m_fTranX += dx;
                m_fTranY += dy;

                m_fPrevScale = m_fCurScale;
                CalcSnapDistance();
                
                Refresh();
            }  
        }

        private void Zoom(int x, int y, float fScale, bool refresh)
        {
            Point pt = new Point(x, y);
            PointF pt1 = ScreenToGlobal(pt);

            m_fCurScale = fScale;

            PointF pt2 = ScreenToGlobal(pt);

            float dx = (pt2.X - pt1.X) * m_fCurScale;
            float dy = (pt2.Y - pt1.Y) * m_fCurScale;

            m_ptOrigin.X += dx;
            m_ptOrigin.Y += dy;

            m_fTranX += dx;
            m_fTranY += dy;

            m_fPrevScale = m_fCurScale;
            CalcSnapDistance();

            if (refresh)
                Refresh();
        }

        private void CalcSnapDistance()
        {
            Point ptOrigin = new Point(0, 0);
            Point ptSnap = new Point(m_nSnapPixels, 0);

            PointF pt1 = ScreenToGlobal(ptOrigin);
            PointF pt2 = ScreenToGlobal(ptSnap);

            UnE.Geometry.Vertex2F v1 = new UnE.Geometry.Vertex2F(pt1.X, pt1.Y);
            UnE.Geometry.Vertex2F v2 = new UnE.Geometry.Vertex2F(pt2.X, pt2.Y);
            m_fSnapDistance = v1.GetDistance(v2);
        }

		public void ClearData()
		{
			ArrayList arSections = (ArrayList)m_arrSection.Clone();

			foreach (Sections.Section section in arSections)
			{
				DeleteSection(section);
			}
			m_arrSection.Clear();
		}

        public override void Delete()
        {
			if (m_sectionSelected != null || SelectedSectionList.Count > 0)
            {
				SuspendRefresh();

                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 삭제");

				if (m_sectionSelected != null)
				{
					if (m_sectionSelected.GetComponentType() == Section.ComponentType.GROUP)
					{
						SectionGroup groupSection = (SectionGroup)m_sectionSelected;
						if (groupSection.Collapse == false)
						{
							groupSection.Collapse = true;
						}
					}
					DeleteSection(m_sectionSelected);
					m_sectionSelected = null;
				}			

				foreach (Section section in SelectedSectionList)
				{
					DeleteSection(section);
				}
				ClearSelectedSection();
				ResumeRefresh();
				Refresh();	          
            }
            else if (m_arrowSelected != null)
            {
				UndoRedoManager.Instance.SaveSnapshot("화살표 삭제");

                RemoveArrow(m_arrowSelected);
                m_arrowSelected = null;
                Refresh();
            }
        }

		// 그룹 Section은 다른 Group을 가질 수 있으므로 재귀적으로 삭제한다.
		protected void DeleteSection(Section section)
		{
			if (section != null)
			{
				// 지우고자 하는 대상 객체가 m_linkRequest이면 m_linkRequest를 초기화시킨다.
				if (section == m_linkRequest)
					m_linkRequest = null;

				section.Select(false);
				section.Hide();
				// 그룹인경우 포함된 컴포넌트도 함께 삭제 한다.
				if (section.GetComponentType() == Section.ComponentType.GROUP)
				{
					SectionDataGroup data = (SectionDataGroup)section.Data;
					ArrayList arGroupSections = (ArrayList)(data.GroupItems.Clone());
					foreach (Section comp in arGroupSections)
					{
						data.RemoveGroupMember(comp);
						comp.Select(false);
						comp.Hide();
						DeleteSection(comp);
					}
					data.GroupItems.Clear();
				}
				RemoveSection(section);
			}
		}

        private void toolStripMenuDelete_Click(object sender, System.EventArgs e)
        {
            Delete();
        }

        private void toolStripMenuSelection_Click(object sender, System.EventArgs e)
        {
            if (m_sectionSelected != null)
                m_linkRequest = (SectionLink)m_sectionSelected;
        }

        //public void AutoAlign()
        //{
        //    if (m_sectionSelected == null)
        //        return;
        //}

        //private void toolStripMenuAutoAlign_Click(object sender, System.EventArgs e)
        //{
        //    AutoAlign();
        //}

        public override void ClearSelection()
        {
            if (m_arrowSelected != null)
            {
                m_arrowSelected.Select(false);
                m_arrowSelected = null;
            }

            if (m_sectionSelected != null)
            {
                m_sectionSelected.Select(false);
                m_sectionSelected = null;
            }

            ClearSelectedSection();
			SelectedSectionList.Clear();
        }

        public void ResetComponentID(string strOldStepName, string strNewStepName)
        {
            string strBeginTag = strOldStepName + "_";
            string strNewBeginTag = strNewStepName + "_";

            foreach (Section section in m_arrSection)
            {
                string strComponentID = section.Data.ComponentID;

                if (strComponentID.StartsWith(strBeginTag))
                    section.Data.ComponentID = strComponentID.Replace(strBeginTag, strNewBeginTag);
            }
        }

        public new string TeamName
        {
            get { return m_strTeamName; }
            set 
            {
                m_strTeamName = value;
                base.TeamName = value;
            }
        }

        public string StepName
        {
            get
            {
                return Parent == null ? "" : Parent.Text;
            }
            set 
            {
                if(this.Parent != null)
                    Parent.Text = value;                
            }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        // 0(평일 비상조직), 1(휴일 및 야간 비상조직), 2(사용자 정의 조직), 3(외부기관)
        public int TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        private void PanelSectionEx_SizeChanged(object sender, EventArgs e)
        {
            mBackImage = new Bitmap(Size.Width, Size.Height);
            mBackImage2 = new Bitmap(Size.Width, Size.Height);
            m_bNeedEraseBkg = true;
        }

        private void PanelSectionEx_BackColorChanged(object sender, EventArgs e)
        {
            m_bNeedEraseBkg = true;   
        }                   

        public void ZoomSection(Sections.Section section)
        {
            PointF ptfSection = section.Position;
            SizeF szfSection = section.RectSize;

            PointF ptfSectionCenter = new PointF(ptfSection.X + szfSection.Width / 2, ptfSection.Y + szfSection.Height / 2);
            Point ptSectionCenter = GlobalToScreen(ptfSectionCenter);

            Size szPanel = this.Size;
            Point ptPanelCenter = new Point(szPanel.Width / 2, szPanel.Height / 2);

            Translate(ptSectionCenter.X, ptSectionCenter.Y, ptPanelCenter.X, ptPanelCenter.Y);

            float fScale = 1.0f;

            if (szfSection.Width / szPanel.Width > szfSection.Height / szPanel.Height)
                fScale = szPanel.Width / szfSection.Width / 2;
            else
                fScale = szPanel.Height / szfSection.Height / 2;
                
            Zoom(ptPanelCenter.X, ptPanelCenter.Y, fScale, true);
        }      

        // Return 값 : tabPage 내에 있는 Panel들의 TeamID, TeamType 리스트
        //             상위 4바이트(TeamID), 하위 4바이트(TeamType)
        public static ArrayList GetTabPageTeamList(TabPage tabPage)
        {
            if (tabPage == null)
                return null;

            ArrayList arrTeams = new ArrayList();
            Type type = typeof(PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    PanelSectionEx panel = (PanelSectionEx)ctrl;

                    long nTeamData = panel.TeamID;
                    nTeamData = nTeamData << 32;
					nTeamData = nTeamData | (uint)panel.TeamType;

                    arrTeams.Add(nTeamData);
                }
            }
            return arrTeams;
        }

		public static int GetLastTabPagePanelIndex(TabPage tabPage)
		{
			Type type = typeof(Sections.PanelSectionEx);
			int nIndex = -1;
			
			if (tabPage == null)
				return -1;

			foreach (Control ctrl in tabPage.Controls)
			{
				if (ctrl.GetType() == type)
				{
					nIndex++;
				}
			}
			return nIndex;
		}
        
        public override void Refresh()
        {
            m_bNeedEraseBkg = true;
            base.Refresh();
        }

        public new void Invalidate()
        {
            m_bNeedEraseBkg = true;
            base.Invalidate();
        }    

        private static int GetTabPagePanelIndex(PanelSectionEx panel, TabPage tabPage)
        {
            Type type = typeof(PanelSectionEx);
            int nIndex = 0;

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    if (ctrl == panel)
                        return nIndex;

                    nIndex++;
                }
            }
            return -1;
        }       

        public new int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }


        private void endPointContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "삭제")
            {
                Delete();
            }
            else if (e.ClickedItem.Text == "내용 편집")
            {
                if (m_sectionSelected != null)
                {
                    using (PopupNote form = new PopupNote(false))
                    {
                        form.Text = m_sectionSelected.Title;
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            if (m_sectionSelected.Title != form.Text)
                            {
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

                                m_sectionSelected.Title = form.Text;
                                if (m_listener != null)
                                {
                                    m_listener.OnSelectedSection(null);
                                    m_listener.OnSelectedSection(m_sectionSelected);
                                }
                            }
                        }
                    }
                }
            }
            else if(e.ClickedItem.Text == "[종료] 타입으로 변경")
            {
                if (m_sectionSelected != null)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)m_sectionSelected.Data;
                    if (data != null)
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 타입 변경");

                        data.IsBegin = false;

                        if (m_listener != null)
                        {
                            m_listener.OnSelectedSection(null);
                            m_listener.OnSelectedSection(m_sectionSelected);
                        }
                    }                    
                }
            }
            else if (e.ClickedItem.Text == "[시작] 타입으로 변경")
            {
                if (m_sectionSelected != null)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)m_sectionSelected.Data;
                    if (data != null)
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 타입 변경");
                        data.IsBegin = true;

                        if (m_listener != null)
                        {
                            m_listener.OnSelectedSection(null);
                            m_listener.OnSelectedSection(m_sectionSelected);
                        }
                    }
                }
            }
        }

        private void editTextContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "삭제")
            {
                Delete();
            }
            else if (e.ClickedItem.Text == "내용 편집")
            {
                if (m_sectionSelected != null)
                {
                    using (PopupNote form = new PopupNote(false))
                    {
                        form.Text = m_sectionSelected.Title;
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            if (m_sectionSelected.Title != form.Text)
                            {
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

                                m_sectionSelected.Title = form.Text;
                                if (m_listener != null)
                                {
                                    m_listener.OnSelectedSection(null);
                                    m_listener.OnSelectedSection(m_sectionSelected);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void editContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Text == "삭제")
            {
                Delete();
            }
            else if(e.ClickedItem.Text == "수식 편집")
            {
                if (m_sectionSelected != null)
                {
                    using (PopupNote form = new PopupNote(true))
                    {
                        form.Text = m_sectionSelected.Data.Expression;
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            if (m_sectionSelected.Data.Expression != form.Text)
                            {
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 수식 편집");

                                m_sectionSelected.Data.Expression = form.Text;
                                if (m_listener != null)
                                {
                                    m_listener.OnSelectedSection(null);
                                    m_listener.OnSelectedSection(m_sectionSelected);
                                }
                            }                            
                        }
                    }
                }                
            }
            else if(e.ClickedItem.Text == "내용 편집")
            {
                if (m_sectionSelected != null)
                {
                    using (PopupNote form = new PopupNote(false))
                    {
                        form.Text = m_sectionSelected.Title;
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            if (m_sectionSelected.Title != form.Text)
                            {
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

                                m_sectionSelected.Title = form.Text;
                                if (m_listener != null)
                                {
                                    m_listener.OnSelectedSection(null);
                                    m_listener.OnSelectedSection(m_sectionSelected);
                                }
                            }                            
                        }
                    }
                }
            }
            else if (e.ClickedItem == toolStripMenuItemShowType)
            {
                if (m_sectionSelected != null)
                {
                    if (toolStripMenuItemShowType.Text == "[내용]으로 표시")
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 표시 타입 변경");

                        m_sectionSelected.Data.ShowExpression = false;
                        if (m_PrefDisplayOption == "Component")
                        {
                            m_sectionSelected.Data.ResetShowExpression();
                        }
                        toolStripMenuItemShowType.Text = "[수식]으로 표시";

                        if(m_listener != null)
                        {
                            m_listener.OnSelectedSection(null);
                            m_listener.OnSelectedSection(m_sectionSelected);
                        }

                        
                    }
                    else
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 표시 타입 변경");

                        m_sectionSelected.Data.ShowExpression = true;
                        if (m_PrefDisplayOption == "Component")
                        {
                            m_sectionSelected.Data.ResetShowExpression();
                        }
                        toolStripMenuItemShowType.Text = "[내용]으로 표시";
                        if (m_listener != null)
                        {
                            m_listener.OnSelectedSection(null);
                            m_listener.OnSelectedSection(m_sectionSelected);
                        }
                    }
                }
            }
        }		

		private void groupContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Text == "그룹화")
			{
				Point screenPoint = Cursor.Position;
				Point pt = PointToClient(screenPoint);
				SuspendRefresh();
				int nSelectCount = SelectedSectionList.Count;
				if (nSelectCount > 0)
				{					
					SectionGroupCheker checker = new SectionGroupCheker(this);
					// Check Group
					if (checker.Check(SelectedSectionList))
					{
                        UndoRedoManager.Instance.SaveSnapshot("그룹 컴포넌트 생성");

						SectionGroup groupSection = (SectionGroup)CreateSection(pt.X, pt.Y, Section.ComponentType.GROUP);

						checker.MakeGroupArrow(groupSection);
						
						SectionDataGroup data = (SectionDataGroup)groupSection.Data;
						foreach (Section section in SelectedSectionList)
						{
							data.AddGroupMember(section);
						}
						groupSection.Editable = false;

                        m_bModify = true;
					}
					else
					{
						string szMsg = checker.GetLastErrorMessage();
						UnE.Utility.UMessageBox.Show(szMsg, "그룹생성오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						ResumeRefresh();
						return;
					}
				}
				ResumeRefresh();
				base.ClearSelectedSection();
				Refresh();
			}

			else if (e.ClickedItem.Text == "삭제")
			{
				Delete();		
			}
			else if (e.ClickedItem.Text == "그룹해제")
			{
				if (m_sectionSelected != null && m_sectionSelected.GetComponentType() == Section.ComponentType.GROUP)
				{
					SuspendRefresh();

					UndoRedoManager.Instance.SaveSnapshot("그룹 컴포넌트 해제");

					SectionGroup groupSection = (SectionGroup)m_sectionSelected;
					if (groupSection.Collapse == false)
					{						
						groupSection.Collapse = true;						
					}

					SectionDataGroup data = (SectionDataGroup)m_sectionSelected.Data;
					data.RemoveAllGroupMember();

					DeleteSection(m_sectionSelected);
					
					m_sectionSelected = null;
					ClearSelectedSection();
					ResumeRefresh();
					Refresh();
				}
			}
		}

		private void PanelSectionEx_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			// delete
			if (e.KeyValue == 46)
			{
				Delete();
			}
		}


        protected override void OnSectionSizeChanged(Section section, float x, float y)
        {
            base.OnSectionSizeChanged(section, x, y);
        }

        protected override void OnSectionMoved(Section section, PointF pt)
        {
            base.OnSectionMoved(section, pt);
            
            // 화살표 스냅 사용시
            if (ArrowSnapOn)
            {
                bool isHorz;
                float fDistance, fX = 0.0f, fY = 0.0f;

                foreach (Arrow arrow in section.Arrows)
                {
                    if (!arrow.CanBeStraight(out isHorz, out fDistance))
                        continue;

                    if (fDistance > m_fSnapDistance)
                        continue;

                    if (isHorz && fX == 0.0f)
                        fX = fDistance;
                    else if (!isHorz && fY == 0.0f)
                        fY = fDistance;
                }

                if (fX != 0.0f || fY != 0.0f)
                {
                    pt.X += fX;
                    pt.Y += fY;

                    base.OnSectionMoved(section, pt);

                    m_bModify = true;

                    Refresh();
                }
            }
        }

        
        private bool m_bExitMouse = false;
        private void PanelSectionEx_MouseEnter(object sender, EventArgs e)
        {
            if (m_bExitMouse == true)
            {
                m_bExitMouse = false;
                Refresh();
            }
        }

        private void PanelSectionEx_MouseLeave(object sender, EventArgs e)
        {
            if (m_bExitMouse == false)
            {
                m_bExitMouse = true;
                Refresh();
                m_arrDragDropDrawing = null;
            }
           
        }

        protected float xDim = 3.0f;
        internal float DimX
        {
            get { return xDim; }
            set { xDim = value; }
        }
        protected float yDim = 3.0f;
        internal float DimY
        {
            get { return yDim; }
            set { yDim = value; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                bool bChanged = false;
                if (keyData.Equals(Keys.Left))
                {
                    if (m_sectionSelected != null || SelectedSectionList.Count > 0)
                    {                       
                        if (m_sectionSelected != null)
                        {

                            PointF pt = new PointF(m_sectionSelected.Position.X - xDim, m_sectionSelected.Position.Y);
                            if (bChanged == false)
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");

                            base.OnSectionMoved(m_sectionSelected, pt);

                            bChanged = true;
                        }
                        foreach (Section section in SelectedSectionList)
                        {
                            if (m_sectionSelected != section)
                            {
                                PointF pt2 = GlobalToScreen(new Point(3, 3));
                                PointF pt = new PointF(section.Position.X - xDim, section.Position.Y);
                                if (bChanged == false)
                                    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                                base.OnSectionMoved(section, pt);
                                bChanged = true;
                            }
                        }
                        if(bChanged)
                            Refresh();
                    }                    
                    return true;
                }
                else if (keyData.Equals(Keys.Up))
                {
                    if (m_sectionSelected != null || SelectedSectionList.Count > 0)
                    {
                        if (m_sectionSelected != null)
                        {
                            PointF pt2 = GlobalToScreen(new Point(3, 3));
                            PointF pt = new PointF(m_sectionSelected.Position.X, m_sectionSelected.Position.Y - yDim);
                            if (bChanged == false)
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                            base.OnSectionMoved(m_sectionSelected, pt);
                            bChanged = true;
                        }
                        foreach (Section section in SelectedSectionList)
                        {
                            if (m_sectionSelected != section)
                            {
                                PointF pt2 = GlobalToScreen(new Point(3, 3));
                                PointF pt = new PointF(section.Position.X, section.Position.Y - yDim);
                                if (bChanged == false)
                                    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                                base.OnSectionMoved(section, pt);
                                bChanged = true;
                            }
                        }
                        if(bChanged)
                            Refresh();
                    }
                    return true;
                }
                else if (keyData.Equals(Keys.Down))
                {
                    if (m_sectionSelected != null || SelectedSectionList.Count > 0)
                    {
                        if (m_sectionSelected != null)
                        {
                            PointF pt2 = GlobalToScreen(new Point(3, 3));
                            PointF pt = new PointF(m_sectionSelected.Position.X, m_sectionSelected.Position.Y + yDim);
                            if (bChanged == false)
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                            base.OnSectionMoved(m_sectionSelected, pt);
                            bChanged = true;

                        }
                        foreach (Section section in SelectedSectionList)
                        {
                            if (m_sectionSelected != section)
                            {
                                PointF pt2 = GlobalToScreen(new Point(3, 3));
                                PointF pt = new PointF(section.Position.X, section.Position.Y + yDim);
                                if (bChanged == false)
                                    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                                base.OnSectionMoved(section, pt);
                                bChanged = true;
                            }
                        }
                        if(bChanged)
                            Refresh();
                    }    
                    return true;
                }
                else if (keyData.Equals(Keys.Right))
                {
                    if (m_sectionSelected != null || SelectedSectionList.Count > 0)
                    {
                        if (m_sectionSelected != null)
                        {
                            PointF pt2 = GlobalToScreen(new Point(3, 3));
                            PointF pt = new PointF(m_sectionSelected.Position.X + xDim, m_sectionSelected.Position.Y);
                            if (bChanged == false)
                                UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                            base.OnSectionMoved(m_sectionSelected, pt);
                            bChanged = true;
                        }
                        foreach (Section section in SelectedSectionList)
                        {
                            if (m_sectionSelected != section)
                            {
                                PointF pt2 = GlobalToScreen(new Point(3, 3));
                                PointF pt = new PointF(section.Position.X + xDim, section.Position.Y);
                                if (bChanged == false)
                                    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 위치 변경");
                                base.OnSectionMoved(section, pt);
                                bChanged = true;
                            }
                        }
                        if(bChanged)
                            Refresh();
                    }      
                    return true;
                } 
                else if (keyData.Equals(Keys.Control | Keys.C))
                {
                    SectionClipboardEx.Instance.Copy(this);
                    Refresh();
                    return true;
                }
                else if (keyData.Equals(Keys.Control | Keys.X))
                {
                    SectionClipboardEx.Instance.Cut(this);
                    Refresh();
                    return true;
                }
                else if (keyData.Equals(Keys.Control | Keys.V))
                {
                    SectionClipboardEx.Instance.Paste(this);
                    Refresh();
                    return true;
                } 
            }
            else
            {
                return true;
            }
            return false;
        }

        private void tmrKeyMove_Tick(object sender, EventArgs e)
        {

        }


        public void ChangeStepName(string szOrName, string szNewName)
        {
            foreach (Section section in m_arrSection)
            {

                if (section != null)
                {
                    Sections.SectionData data = section.Data;
                    if (data != null)
                    {
                        string szOrgID = data.ComponentID;
                        string szNewID = szOrgID.Replace(szOrName, szNewName);
                        data.ComponentID = szNewID;
                    }
                }
            }
        }


        private string m_PrefDisplayOption = "Component";
        public void SetDisplayText(string szDisplayOption)
        {
            foreach (Section section in m_arrSection)
            {


                if (section != null && 
                    (section.GetComponentType() == Section.ComponentType.PROCESS ||
                     section.GetComponentType() == Section.ComponentType.DECISION))
                {
                    Sections.SectionData data = section.Data;
                    if (data != null)
                    {
                        if (szDisplayOption == "Text")
                            data.ShowTempExpression = false;
                        else if (szDisplayOption == "Expr")
                            data.ShowTempExpression = true;
                        else
                            data.ResetShowExpression();
                     
   
                    }

                    m_PrefDisplayOption = szDisplayOption;
                }
            }
            Refresh();
        }
        

        private bool m_bCurrentSection = false;
        public bool CurrentSection
        {
            get { return m_bCurrentSection; }
            set { m_bCurrentSection = value; }
        }

        private void toolStripMenuCopy_Click(object sender, EventArgs e)
        {
            SectionClipboardEx.Instance.Copy(this);
        }
        private void toolStripMenuCut_Click(object sender, EventArgs e)
        {
            SectionClipboardEx.Instance.Cut(this);
        }
        private void toolStripMenuPaste_Click(object sender, EventArgs e)
        {
            
            SectionClipboardEx.Instance.Paste(this);
        }

        private void arrowPopupContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if( e.ClickedItem.Text == "[Yes] 화살표로 변경")
            {
                Arrow arrow = (Arrow)arrowPopupContextMenuStrip.Tag;
                if(arrow.Text != "Yes")
                {
                    UndoRedoManager.Instance.SaveSnapshot("화살표 타입 변경");
                    arrow.Text = "Yes";
                    Refresh();
                }               
            }
            else if (e.ClickedItem.Text == "[No] 화살표로 변경")
            {
                Arrow arrow = (Arrow)arrowPopupContextMenuStrip.Tag;
                if (arrow.Text != "No")
                {
                    UndoRedoManager.Instance.SaveSnapshot("화살표 타입 변경");
                    arrow.Text = "No";
                    Refresh();
                }  
            }
            else if (e.ClickedItem.Text == "일반 화살표로 변경")
            {
                Arrow arrow = (Arrow)arrowPopupContextMenuStrip.Tag;
                if (arrow.Text != "")
                {
                    UndoRedoManager.Instance.SaveSnapshot("화살표 타입 변경");
                    arrow.Text = "";
                    Refresh();
                }
            }
        } 

    }
}
