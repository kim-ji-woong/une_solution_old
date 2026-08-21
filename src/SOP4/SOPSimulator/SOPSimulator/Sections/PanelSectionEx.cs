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

//libSection
using Sections;
//libSOP
using UnE.SOP;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Process;
using UnE.SOP.Workstate;
using SOPMonitoringSystem;

namespace Sections
{
    public partial class PanelSectionEx : PanelSection
    {
        private PointF[] m_arrDragDropDrawing = null;
        private Section.ComponentType m_sectionDragDropType = Section.ComponentType.NONE;

        // Panel 상단에 표시되는 팀 이름
        private string m_strTeamName = "";
        private string m_strStepName = "";
        private int m_nTeamID = 0;
        // 0(평일 비상조직), 1(휴일 및 야간 비상조직), 2(사용자 정의 조직), 3(외부기관)
        private int m_nTeamType = 0;
        //private int m_nActionStepID = -1;

        private Section m_sectionArrowLink = null;

        // Section별 ComponentID
        private Dictionary<Section, int> m_dicComponentID = new Dictionary<Section, int>();

        //////////////////////////////////////////////////////////////////////////
        private float m_fTranslateX = 0.0f, m_fTranslateY = 0.0f;
        //private float m_fPrevOriginX = 0.0f, m_fPrevOriginY = 0.0f;
            
        //private Point m_ptMClicked = new Point();

        //private float m_fScale = 1.0f;
        //private float m_fNewScale = 1.0f;
        //////////////////////////////////////////////////////////////////////////
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

        // Arrow의 텍스트를 입력받기 위한 임시 Control
        private ZBobb.AlphaBlendTextBox m_tempArrowTextBox = new ZBobb.AlphaBlendTextBox();

        
        // 0(그리지 않는다), 1(그린다), -1(그리지 않으며 객체를 저장한다)
        
        private ProcessButton m_mouseOverButton = null;
        private ConfirmButton m_mouseOverButton2 = null;

        private SOPMonitoringSystem.FormLegend m_frmLegend = null;

        /// <summary>
        /// 타이틀 바  
        /// </summary>
        private Label label = new Label();
        private int m_nTitleBarInitHeight = 0;
        private int m_nTitleBarPSMHeight = 0;

        /// <summary>
        /// 타이틀 바 내 타이틀 바
        /// </summary>
        private Label labelTitle = new Label();

        /// <summary>
        /// 타이틀 바 내 시작정보표시 바
        /// </summary>
        private Label labelInfo1 = new Label();
        private Label labelInfo2 = new Label();
        private Label labelPSMMaterial = new Label();

        /// <summary>
        /// 타이틀 바 내 ActionStepName
        /// </summary>
        private Label labelTitle2 = new Label();

        public PanelSectionEx()
        {
			this.DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			UpdateStyles();

            InitializeComponent();

            m_tempArrowTextBox.Parent = this;
            m_tempArrowTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            m_tempArrowTextBox.Font = Arrow.TextFont;
            m_tempArrowTextBox.TextAlign = HorizontalAlignment.Center;
            m_tempArrowTextBox.Hide();

            label.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            labelTitle.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            labelInfo1.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            labelInfo2.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            labelPSMMaterial.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);

            m_bShowSectionBtn = FormSOP.Instance.ShowSectionBtn;

            m_nTitleBarPSMHeight = m_nTitleBarInitHeight = label.Size.Height + 20;
        }

        private void PrintTitle()
        {
            label.Controls.Add(labelTitle);
            labelTitle.AutoSize = true;
            labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            labelTitle.ForeColor = System.Drawing.Color.Black;
            labelTitle.Size = new System.Drawing.Size(59, 15);
            labelTitle.Location = new System.Drawing.Point(3, Convert.ToInt32(label.Size.Height - label.Size.Height / 2 - 9));
            labelTitle.TabIndex = 1;
            labelTitle.Text = "SOP 흐름도";
            
            label.Controls.Add(labelInfo1);
            labelInfo1.AutoSize = true;
            labelInfo1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            labelInfo1.ForeColor = System.Drawing.Color.Black;
            labelInfo1.Size = new System.Drawing.Size(220, 30);
            labelInfo1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            int x = Convert.ToInt32(label.Size.Width - 220);
            labelInfo1.Location = new System.Drawing.Point(x, 7);
            labelInfo1.TabIndex = 2;
            labelInfo1.Text = "";

            label.Controls.Add(labelInfo2);
            labelInfo2.AutoSize = true;
            labelInfo2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            labelInfo2.ForeColor = System.Drawing.Color.Black;
            labelInfo2.Size = new System.Drawing.Size(220, 30);
            labelInfo2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            labelInfo2.Location = new System.Drawing.Point(x, labelInfo1.Location.Y + labelInfo2.Height);
            labelInfo2.TabIndex = 2;
            labelInfo2.Text = "";

            label.Controls.Add(labelPSMMaterial);
            labelPSMMaterial.AutoSize = true;
            labelPSMMaterial.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            labelPSMMaterial.ForeColor = System.Drawing.Color.Black;
            labelPSMMaterial.Size = new System.Drawing.Size(220, 30);
            labelPSMMaterial.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            labelPSMMaterial.Location = new System.Drawing.Point(x, labelInfo2.Location.Y + labelPSMMaterial.Height);
            labelPSMMaterial.TabIndex = 2;
            labelPSMMaterial.Text = "";
            labelPSMMaterial.Visible = false;

            m_nTitleBarPSMHeight = m_nTitleBarInitHeight + labelPSMMaterial.Size.Height;
        }

        public void SetInfoText(string info1, string info2)
        {
            if(info1 == null || info1 == "")
            {
                labelInfo1.Text = "";
            }
            else
            {
                labelInfo1.Text = "재난위치: " + info1;
            }

            if (info2 == null || info2 == "")
            {
                labelInfo2.Text = "";
            }
            else
            {
                labelInfo2.Text = "발생시각: " + info2;
            }

            label.Size = new Size(label.Size.Width, m_nTitleBarInitHeight);
            labelPSMMaterial.Visible = false;
        }

        public void SetInfoText(string info1, string info2, string strPSMMaterialName)
        {
            SetInfoText(info1, info2);

            labelPSMMaterial.Text = "오염물질: " + strPSMMaterialName;
            label.Size = new Size(label.Size.Width, m_nTitleBarPSMHeight);
            labelPSMMaterial.Visible = true;
        }

        public void AddPanelTitle(string strTitle)
        {
            if (label.Text == "")
            {
                label.Dock = DockStyle.Top;
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.BackColor = Color.LightSkyBlue;
                
                label.Font = new System.Drawing.Font("맑은고딕", 18F, System.Drawing.FontStyle.Bold);//, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                label.ForeColor = Color.Black;
                Size size = label.Size;
                label.Size = labelPSMMaterial.Visible ? new Size(size.Width, m_nTitleBarPSMHeight) : new Size(size.Width, m_nTitleBarInitHeight);
                Controls.Add(label);
            }
            label.Text = strTitle;

            PrintTitle();
        }

        public override string GetTitle()
        {
            return label.Text;
        }

        protected override void InitHandler()
        {
            base.InitHandler();

            this.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);
            this.SizeChanged += new System.EventHandler(OnSizeChanged);

            m_tempArrowTextBox.KeyDown += new KeyEventHandler(ArrowTextBox_KeyDown);
            m_tempArrowTextBox.Leave += new EventHandler(ArrowTextBox_Leave);
        }

        private void ArrowTextBox_Leave(object sender, EventArgs e)
        {
            string strText = m_tempArrowTextBox.Text;
            Arrow arrow = (Arrow)m_tempArrowTextBox.Tag;

            arrow.Select(false);
            arrow.Text = strText;
                
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

        void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                FormSOP.Instance.GetPageHome().OneTop(PageBackstageSOP.Player.SectionPanel);
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
            
        protected override void OnPaint(object sender, PaintEventArgs e)
        {
            if (m_arrDragDropDrawing != null)
                e.Graphics.DrawPolygon(Shape.BOUNDARY_PEN, m_arrDragDropDrawing);
                
            e.Graphics.ResetTransform();
            e.Graphics.TranslateTransform(m_fTranX, m_fTranY);
            e.Graphics.ScaleTransform(m_fCurScale, m_fCurScale);

            PrevPaint();

            base.OnPaint(sender, e);

            PostPaint(e.Graphics);
                
            if (sender != this)
            {
                try
                {
                    Bitmap bitmap = new Bitmap(label.Size.Width, label.Size.Height*2);
                    label.DrawToBitmap(bitmap, new Rectangle(0, 0, label.Size.Width, label.Size.Height*2));
                    e.Graphics.DrawImage((Image)bitmap, 0, 0);
                }
                catch (Exception)
                {

                }
            }
        }

        protected void PrevPaint()
        {
            m_nDrawingProcessOption = 0;
        }

        protected void PostPaint(Graphics g)
        {
            if (m_bShowSectionBtn == true)
            {
                m_nDrawingProcessOption = 1;

                foreach (ISectionPainter mgr in ProcessManagers)
                {
                    mgr.Draw(g);
                }
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

        // Matrix를 사용
        protected PointF ScreenToGlobal2(Point pt)
        {
            System.Drawing.Drawing2D.Matrix inverseMaxtrix = new System.Drawing.Drawing2D.Matrix(m_fCurScale, 0.0f, 0.0f, m_fCurScale, m_fTranX, m_fTranY);

            try
            {
                inverseMaxtrix.Invert();
            }
            catch (System.ArgumentException)
            {
                return new PointF(0.0f, 0.0f);
            }

            PointF ptResult = new PointF();

            ptResult.X = inverseMaxtrix.Elements[0] * pt.X + inverseMaxtrix.Elements[2] * pt.Y + inverseMaxtrix.Elements[4];
            ptResult.Y = inverseMaxtrix.Elements[1] * pt.X + inverseMaxtrix.Elements[3] * pt.Y + inverseMaxtrix.Elements[5];

            return ptResult;
        }

        public override Point GlobalToScreen(PointF pt)
        {
            int x = (int)(pt.X * m_fCurScale + m_fTranslateX);
            int y = (int)(pt.Y * m_fCurScale + m_fTranslateY);
            return new Point(x, y);
        }

        // Matrix를 사용
        public override Point GlobalToScreen2(PointF pt)
        {
            double m11 = m_fCurScale;
            double m12 = 0.0;
            double m21 = 0.0;
            double m22 = m_fCurScale;
            double dx = m_fTranX;
            double dy = m_fTranY;

            Point ptResult = new Point();

            ptResult.X = (int)(m11 * pt.X + m21 * pt.Y + dx);
            ptResult.Y = (int)(m12 * pt.X + m22 * pt.Y + dy);

            return ptResult;
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

            int nPointCount = arrDragDropOrigin.Count();

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

            for (int i = 0; i < nPointCount; i++)
            {
                m_arrDragDropDrawing[i].X = arrDragDropOrigin[i].X + x;
                m_arrDragDropDrawing[i].Y = arrDragDropOrigin[i].Y + y;
            }

            m_sectionDragDropType = type;
            Refresh();
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

        private bool EditingArrowText()
        {
            return m_tempArrowTextBox.Visible;
        }

        private void toolStripMenuDecision_Click(object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;        
            Section section = (Section)item.Tag;

            if (section != null)
            {
                SectionTabPage page = (SectionTabPage)(this.Parent);
                SectionState stateDecision = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
                if (section.GetComponentType() == Section.ComponentType.LINK)
                {
                    SectionData data = state.SelectSection.Data;
                    data.AggSection = section;
                }

                if (state != null)
                {                        
                    if (stateDecision != null)
                    {
                        stateDecision.Parent.DecisionNextSection = section; 
                        stateDecision.Skip();
                        stateDecision.Parent.DecisionNextSection = null;
                    }
                    if (section.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = section;
                    }

                    state.InputWait();
                    Refresh();
                }
            }                
        }
        private void toolStripMenuDecisionExec_Click(object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Section section = (Section)item.Tag;

            if (section != null)
            {
                ProcessButton btn = GetProcessButton(m_sectionSelected, section);

                if (btn != null)
                {
                    SectionTabPage page = (SectionTabPage)(this.Parent);
                    SectionState stateDecision = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);

                    if (stateDecision != null)
                    {
                        ComponentContents contents = FormSOP.Instance.GetPageHome().GetComponentContents(m_sectionSelected);

                        if (contents != null)
                        {
                            // 재실행하는 것이므로 일단 초기화시킨후 실행시킨다.
                            contents.RunDecision(btn, true);
                        }
                    }
                }
            }

            /*if (section != null)
            {
                SectionTabPage page = (SectionTabPage)(this.Parent);
                SectionState stateDecision = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);

                // SectionState state = stateDecision.Parent.FindState(section, true);
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
                if (section.GetComponentType() == Section.ComponentType.LINK)
                {
                    SectionData data = state.SelectSection.Data;
                    data.AggSection = section;
                }
                
                if (state != null)
                {
                    if (stateDecision != null)
                    {
                        stateDecision.Parent.DecisionNextSection = section;
                        stateDecision.Complete();
                        stateDecision.Parent.DecisionNextSection = null;
                    }

                    if (section.GetComponentType() == Section.ComponentType.TRANSMISSION)
                    {
                        TSectionState tstate = (TSectionState)state;
                        tstate.InProgress();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.INTERNAL)
                    {
                        ISectionState istate = (ISectionState)state;
                        istate.InProgress();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.EXTERNAL)
                    {
                        ESectionState estate = (ESectionState)state;
                        estate.InProgress();
                    }
                    else
                        state.InProgress();
                    Refresh();
                }
            }*/
        }

        private ProcessButton GetProcessButton(Section sectionMain, Section sectionTrg)
        {
            if (sectionMain == null || sectionTrg == null)
                return null;

            ProcessButton btn = null;
            int nLimit = 20;

            for (int i = 0; i < nLimit; i++)
            {
                ISectionPainter painter = sectionMain.GetSectionPainter(i);

                if (painter == null)
                    break;

                if (painter is ProcessButtonManager)
                {
                    ProcessButtonManager mgr = (ProcessButtonManager)painter;
                    List<DecisionProcessButton> buttons = new List<DecisionProcessButton>();

                    ProcessButton btnLeft = mgr.FindButton(Arrow.ArrowPosition.LEFT);

                    if (btnLeft != null && btnLeft.GetNextSection(sectionMain) == sectionTrg)
                    {
                        btn = btnLeft;
                        break;
                    }

                    ProcessButton btnRight = mgr.FindButton(Arrow.ArrowPosition.RIGHT);

                    if (btnRight != null && btnRight.GetNextSection(sectionMain) == sectionTrg)
                    {
                        btn = btnRight;
                        break;
                    }

                    ProcessButton btnBottom = mgr.FindButton(Arrow.ArrowPosition.BOTTOM);

                    if (btnBottom != null && btnBottom.GetNextSection(sectionMain) == sectionTrg)
                    {
                        btn = btnBottom;
                        break;
                    }

                    ProcessButton btnTop = mgr.FindButton(Arrow.ArrowPosition.TOP);

                    if (btnTop != null && btnTop.GetNextSection(sectionMain) == sectionTrg)
                    {
                        btn = btnTop;
                        break;
                    }

                    break;
                }
            }

            return btn;
        }

        private string MakeItemText(Section section, Arrow arrow)
        {
            string szName = "";

            if (arrow.Text != "")
            {
                szName = "<" +arrow.Text + "> (으)로 이동";
                return szName;
            }

            if (section.Title != "")
            {
                string szTitle = section.Title;
                string szShort = "";
                if (szTitle.Length > 64)
                    szShort = szTitle.Substring(0, 64);
                else
                    szShort = szTitle;

                int nIdx = szShort.IndexOf('\n');
                if (nIdx >= 0)
                {
                    if (nIdx != 0)
                    {
                        szName = "[" + szShort.Substring(0, nIdx - 1) + "]";
                        return szName;
                    }                        
                }
                else
                {
                    szName = "[" + szShort + "]";
                    return szName;
                } 
            }

            szName = "(" + section.Data.ID + ")";
            return szName;
        }

        private bool MakeTransSOPMenu(Section section)
        {
            ToolStripMenuItem drop = new ToolStripMenuItem();
            drop.Text = "SOP전환";
            popupContextMenuStrip.Items.Add(drop);
               
            SectionTabPage page = (SectionTabPage)Parent;
            SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
            if (state == null)
                return false;
                
            SectionDataTransSOP data = (SectionDataTransSOP)section.Data;

            VersionInfo ainfo = SOPMonitoringSystem.FormSOP.Instance.SOPManager.GetActionStepVersionInfo(data.LinkedActionStepID);
            ActionStepInfo info = SOPMonitoringSystem.FormSOP.Instance.SOPManager.GetActionStepInfo(data.LinkedActionStepID);
            if( ainfo != null)
            {
                string szText = "";
                if ( ainfo.IsNormal == false)
                {
                    szText += "미등록모드";
                }
                else
                {
                    szText += "등록모드";
                }
                if( ainfo.IsRegular == false)
                {
                    szText += "/야간,휴일";
                }
                else
                {
                    szText += "/평일";
                }
                
                string szName = "<" +szText + "/" + info.ActionStepName + "> (으)로 전환";
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Tag = section;
                item.Size = new System.Drawing.Size(182, 22);
                item.Text = szName;
                item.Click += new System.EventHandler(this.toolStringMenuTransSOP_Click);
                drop.DropDownItems.Add(item);
            }
            return true;
        }

        private bool MakeDecisionMenu(Section section)
        {                
               
            SectionTabPage page = (SectionTabPage)Parent;
                
            SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
            if (state == null)
                return false;

                
            ToolStripMenuItem drop = null;
            if (state.State == State.INPUT)
            {
                drop = new ToolStripMenuItem();
                drop.Text = "건너뛰기";
                popupContextMenuStrip.Items.Add(drop);                    
            }
              
            ToolStripMenuItem target = toolStripMenuDecisionExec;
            if (state.State == State.DONE || state.State == State.RUN)
            {
                target = toolStripMenuRestart;                   
            }
            toolStripMenuDecisionExec.DropDownItems.Clear();
            toolStripMenuRestart.DropDownItems.Clear();
            popupContextMenuStrip.Items.Add(target);

            state.Parent.Decision = section;

            foreach (Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink == section)
                {
                    Section endSection = arrow.EndLink;
                    if (endSection.GetComponentType() == Section.ComponentType.ANNOTATION)
                        continue;

                    string szName = MakeItemText(endSection, arrow);
                    if (szName != "")
                    {
                        if (state.State == State.INPUT)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();
                            item.Tag = endSection;
                            item.Size = new System.Drawing.Size(182, 22);
                            item.Text = szName;
                            item.Click += new System.EventHandler(this.toolStripMenuDecision_Click);
                            drop.DropDownItems.Add(item);
                        }

                        ToolStripMenuItem item2 = new ToolStripMenuItem();
                        item2.Tag = endSection;
                        item2.Size = new System.Drawing.Size(182, 22);
                        item2.Text = szName;
                        if (state.State == State.DONE || state.State == State.RUN)
                        {
                            item2.Click += new System.EventHandler(this.toolStripMenuDecisionExec_Click);
                        }
                        else
                        {
                            item2.Click += new System.EventHandler(this.toolStripMenuDecisionExec_Click);
                        }
                        target.DropDownItems.Add(item2);
                    }
                }
            }
            return true;
        }

        private bool ChangeContextMenu(Section section)
        {
            if (section.GetComponentType() == Section.ComponentType.LINK)
            {
                return false;
            }
            if (section.GetComponentType() == Section.ComponentType.TRANSSOP)
            {
                popupContextMenuStrip.Items.Clear();
                bool bResult = MakeTransSOPMenu(section);
                return bResult;
            }

            if (section.GetComponentType() == Section.ComponentType.DECISION)
            {
                popupContextMenuStrip.Items.Clear();                    
                bool bResult = MakeDecisionMenu(section);
                return bResult;
            }
            SectionTabPage page = (SectionTabPage)Parent;
            SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
                
            if (state == null)
            {
                popupContextMenuStrip.Items.Clear();
                return false;
            }
            toolStripMenuRestart.DropDownItems.Clear();
            toolStripMenuExcec.DropDownItems.Clear();
            popupContextMenuStrip.Items.Clear();

            if (state.State == State.INPUT)
                popupContextMenuStrip.Items.Add(toolStripMenuSkip);
            else if (state.State == State.RUN)
                popupContextMenuStrip.Items.Add(toolStripMenuCancel);

            /*if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)(section.Data);
                if (data.IsBegin == true)
                {
                    if (state.State == State.DONE)
                    {
                            
                        popupContextMenuStrip.Items.Add(toolStripMenuRestart);
                    }
                    else
                    {
                            
                        popupContextMenuStrip.Items.Add(toolStripMenuExcec);                           
                    }
                       
                }
                else
                {
                    if (state.State != State.DONE)
                    {
                        popupContextMenuStrip.Items.Add(toolStripMenuComplete);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
                             

            switch (state.State)
            {
                case State.NORMAL:
                case State.SKIP:
                    {
                            
                        popupContextMenuStrip.Items.Clear();
                        popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {                                
                            toolStripMenuExcec,
                            toolStripMenuComplete
                            });
                    }
                    break;
                case State.INPUT:
                    {
                        popupContextMenuStrip.Items.Clear();
                        popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {                               
                            toolStripMenuExcec,
                            toolStripMenuComplete,
                            toolStripMenuSkip
                            });
                    }
                    break;
                case State.RUN:
                    {
                        popupContextMenuStrip.Items.Clear();
                        popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {                               
                                toolStripMenuComplete,
                                toolStripMenuCancel
                                });
                    }
                    break;
                case State.DONE:
                    {
                        popupContextMenuStrip.Items.Clear();
                        popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {                               
                                toolStripMenuRestart                                    
                                });
                    }
                    break;
                default:
                    {
                        popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            toolStripMenuCancel});
                    }
                    break;
            }*/
            return true;
        }

        protected bool _SelectSection(float x, float y, bool informToListener = true)
        {
            Section section = SelectSection(x, y);
            if (section == null) return false;

            if (section == m_sectionSelected)
                return false;

            m_ptSelected = section.Position;

            if (m_sectionSelected != null)
                m_sectionSelected.Select(false);

            if (m_listener != null && informToListener)
                m_listener.OnSelectedSection(section);
            else
            {
                // OnSelectedSection의 호출을 막아서 Setion의 Status 변경은 막고 Section 선택을 통한 속성창만 보여지도록 한다.
                SOPMonitoringSystem.PageBackstageSOP pageHome = SOPMonitoringSystem.FormSOP.Instance.GetPageHome();
                pageHome.ClearSelection(pageHome.GetCurrentPanel());
                pageHome.ShowSectionProperty(section);
            }

            section.Select(true);
            m_sectionSelected = section;
            Refresh();

            return true;
        }

        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();

            if (m_listener != null)
                m_listener.SetCurrentPanel(this);
            //Arrow.ArrowPosition pos = Arrow.ArrowPosition.NONE;

            float x, y;
            ScreenToGlobal(e.X, e.Y, out x, out y);

            SectionButton button = SelectSectionButton(x, y);
            if (button != null)
            {
                m_bDownButton = true;
                m_SelectedBtn = button;
                return;
            }

            ProcessButton btn = null;
            ConfirmButton btn2 = null;
            if (EditingArrowText())
            {
                ArrowTextBox_Leave(null, null);
            }
            else if (ProcessButtonClick(e.Button, x, y, out btn, out btn2))
            {
                if(btn != null)
                    btn.OnClick();
                if (btn2 != null)
                    btn2.OnClick();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                _SelectSection(x, y, false);

                if (m_arrDragDropDrawing != null && m_sectionDragDropType != Section.ComponentType.NONE)
                {
                    SOPMonitoringSystem.FormSOP.Instance.GetPageHome().SetDragDropShape(null, Section.ComponentType.NONE);
                    m_arrDragDropDrawing = null;
                }
                else if (m_sectionSelected != null)
                {
                    if (SOPMonitoringSystem.FormSOP.Instance.HasControl)
                    {
                        if (ChangeContextMenu(m_sectionSelected))
                            popupContextMenuStrip.Show(this, e.X, e.Y);
                    }
                }
                else
                    PopupRClickMenu(e.X, e.Y);
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

                    Refresh();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_ptCurrent.X = e.X;
                m_ptCurrent.Y = e.Y;

                m_bTranslation = true;
                m_ptPrev.X = e.X;
                m_ptPrev.Y = e.Y;
            }
            else
            {
                base.OnMouseDown(sender, e);
            }

            if (m_sectionSelected != null)
                SOPMonitoringSystem.FormSOP.Instance.CurrentSection = m_sectionSelected;

            Invalidate();
        }

        private void PopupRClickMenu(int x, int y)
        {
            SectionTabPage page = (SectionTabPage)this.Parent;
            WorkFlow workFlow = WorkFlowManager.Instance.Get(m_nActionStepID, !page.VirtualMode);

            if (workFlow == null || workFlow.State == WorkFlowState.DONE)
            {
                contextMenuStripRClick.Show(this, x, y);
            }
        }

        protected bool ProcessButtonClick(MouseButtons mouseButton, float x, float y, out ProcessButton btn, out ConfirmButton btn2)
        {
            btn = null;
            btn2 = null;

            if (!SOPMonitoringSystem.FormSOP.Instance.HasControl)
                return false;
            
            if (mouseButton != System.Windows.Forms.MouseButtons.Left)
                return false;

            
            foreach (object obj in m_arrProcessManagers)
            {
                if (obj.GetType() == typeof(ProcessButtonManager) || obj.GetType() == typeof(ProcessRectButtonManager))
                {                    
                    ProcessButtonManager mgr = (ProcessButtonManager)obj;
                    btn = mgr.GetProcessButton(x, y);

                    if (btn != null)
                        return true;
                }
                if (obj.GetType() == typeof(ConfirmButtonManager))
                {
                    ConfirmButtonManager mgr = (ConfirmButtonManager)obj;
                    btn2 = mgr.GetProcessButton(x, y);

                    if (btn2 != null)
                        return true;
                } 
            }
            return false;
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            if (e.Button == MouseButtons.Middle)
            {
                if (m_bTranslation == true)
                {
                    m_bTranslation = false;
                }
            }
        }

        private void Translate(int prevX, int prevY, int x, int y)
        {
            m_ptOrigin.X += (x - prevX);
            m_ptOrigin.Y += (y - prevY);

            m_fTranX = m_ptOrigin.X;
            m_fTranY = m_ptOrigin.Y;
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
            else
            {
                CheckProcessButton(x, y);
                CheckConfirmButton(x, y);
            }
        }

        protected void CheckConfirmButton(float x, float y)
        {
            if (!SOPMonitoringSystem.FormSOP.Instance.HasControl)
                return;

            ConfirmButton btn = GetConfirmButton(x, y);

            if (btn == null)
            {
                if (m_mouseOverButton2 != null)
                {
                    if (m_mouseOverButton2.Status == ConfirmButton.ButtonStatus.WAIT_MOUSE_OVER)
                        m_mouseOverButton2.Status = ConfirmButton.ButtonStatus.WAIT;

                    m_mouseOverButton2 = null;
                    Refresh();
                }
            }
            else
            {
                if (btn != m_mouseOverButton2)
                {
                    bool refresh = false;

                    if (m_mouseOverButton != null)
                    {
                        if (m_mouseOverButton2.Status == ConfirmButton.ButtonStatus.WAIT_MOUSE_OVER)
                            m_mouseOverButton2.Status = ConfirmButton.ButtonStatus.WAIT;

                        m_mouseOverButton2 = null;
                        refresh = true;
                    }

                    //if (btn.Status == ConfirmButton.ButtonStatus.WAIT)
                    //{
                        if (btn.EnableClick())
                        {
                            btn.Status = ConfirmButton.ButtonStatus.WAIT_MOUSE_OVER;
                            m_mouseOverButton2 = btn;
                            refresh = true;
                        }
                    //}

                    if (refresh)
                        Refresh();
                }
            }
        }

        protected ConfirmButton GetConfirmButton(float x, float y)
        {
            foreach (object obj in m_arrProcessManagers)
            {
                if (obj.GetType() == typeof(ConfirmButtonManager))
                {
                    ConfirmButtonManager mgr = (ConfirmButtonManager)obj;
                    ConfirmButton btn = mgr.GetProcessButton(x, y);
                    if (btn != null)
                        return btn;
                }

            }
            return null;
        }

        protected void CheckProcessButton(float x, float y)
        {
            if (!SOPMonitoringSystem.FormSOP.Instance.HasControl)
                return;

            ProcessButton btn = GetProcessButton(x, y);

            if (btn == null)
            {
                if (m_mouseOverButton != null)
                {
                    if (m_mouseOverButton.Status == ProcessButton.ButtonStatus.WAIT_MOUSE_OVER)
                        m_mouseOverButton.Status = ProcessButton.ButtonStatus.WAIT;
                    
                    m_mouseOverButton = null;
                    Refresh();
                }
            }
            else
            {
                if (btn != m_mouseOverButton)
                {
                    bool refresh = false;

                    if (m_mouseOverButton != null)
                    {
                        if (m_mouseOverButton.Status == ProcessButton.ButtonStatus.WAIT_MOUSE_OVER)
                            m_mouseOverButton.Status = ProcessButton.ButtonStatus.WAIT;

                        m_mouseOverButton = null;
                        refresh = true;
                    }

                    if( btn.IsStartButton() == true)
                    {
                        if (btn.EnableClick())
                        {
                            btn.Status = ProcessButton.ButtonStatus.WAIT_MOUSE_OVER;
                            m_mouseOverButton = btn;
                            refresh = true;
                        }
                    }
                    else
                    {
                        if (btn.Status == ProcessButton.ButtonStatus.WAIT)
                        {
                            if (btn.EnableClick())
                            {
                                btn.Status = ProcessButton.ButtonStatus.WAIT_MOUSE_OVER;
                                m_mouseOverButton = btn;
                                refresh = true;
                            }
                        }
                    } 
                    if (refresh)
                        Refresh();
                }
            }
        }

        protected ProcessButton GetProcessButton(float x, float y)
        {
            foreach (object obj in m_arrProcessManagers)
            {
                if( obj.GetType() == typeof(ProcessButtonManager)
                    || obj.GetType() == typeof(ProcessRectButtonManager))
                {
                    ProcessButtonManager mgr = (ProcessButtonManager)obj;
                    ProcessButton btn = mgr.GetProcessButton(x, y);
                    if (btn != null)
                        return btn;
                }
                
            }

            return null;
        }

        /*public override int GetScrollPos(bool isHorz)
        {
            return isHorz ? -(int)m_fTranslateX : -(int)m_fTranslateY;
        }*/

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
                    return true;
                }

                ArrayList arrChilds = section.GetChildSections();

                if (arrChilds != null)
                {
                    if (CheckArrowLinkSection(x, y, arrChilds, nDepth + 1))
                        return true;
                }
            }

            return false;
        }

        /*protected override void OnTimer(object sender, EventArgs e)
        {
            if (m_tempArrow != null)
                return;

            base.OnTimer(sender, e);
                
        }*/

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
            else
                return null;

            section.MakeData(m_strStepName, m_strTeamName);

            /*FormMain.Instance.GetPageLevel().SetDragDropShape(null, Section.ComponentType.NONE);
            m_arrDragDropDrawing = null;*/

            Sections.Add(section);
            return section;
        }

        public void WheelMouse(int x, int y, int nDelta)
        {
            //float _x, _y;
            //ScreenToGlobal(x, y, out _x, out _y);

            if (nDelta > 0)
                ZoomIn(x, y);
            else
                ZoomOut(x, y);
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

                m_fPrevScale = m_fCurScale;

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

                Refresh();
            }  
        }
        
        private void Zoom(int x, int y, float fScale, bool refresh)
        {
            Point pt = new Point(x, y);
            //PointF pt1 = ScreenToGlobal(pt);
            PointF pt1 = ScreenToGlobal2(pt);

            m_fCurScale = fScale;

            //PointF pt2 = ScreenToGlobal(pt);
            PointF pt2 = ScreenToGlobal2(pt);

            float dx = (pt2.X - pt1.X) * m_fCurScale;
            float dy = (pt2.Y - pt1.Y) * m_fCurScale;

            m_ptOrigin.X += dx;
            m_ptOrigin.Y += dy;

            m_fTranX += dx;
            m_fTranY += dy;

            m_fPrevScale = m_fCurScale;

            if (refresh)
                Refresh();
        }

        protected void ZoomArea(PointF newCenter, float fAreaWidth, float fAreaHeight, float fRatio = 0.5f)
        {
            Point ptNewCenter = GlobalToScreen2(newCenter);

            Size szPanel = this.Size;
            Point ptPanelCenter = new Point(szPanel.Width / 2, szPanel.Height / 2);

            Translate(ptNewCenter.X, ptNewCenter.Y, ptPanelCenter.X, ptPanelCenter.Y);

            float fScale = 1.0f;

            if (fAreaWidth / szPanel.Width > fAreaHeight / szPanel.Height)
                fScale = szPanel.Width / fAreaWidth * fRatio;
            else
                fScale = szPanel.Height / fAreaHeight * fRatio;

            Zoom(ptPanelCenter.X, ptPanelCenter.Y, fScale, true);
        }
        
        public void ZoomSection(Sections.Section section)
        {
            PointF ptfSection = section.Position;
            SizeF szfSection = section.RectSize;

            PointF ptfSectionCenter = new PointF(ptfSection.X + szfSection.Width / 2, ptfSection.Y + szfSection.Height / 2);
            ZoomArea(ptfSectionCenter, szfSection.Width, szfSection.Height);
            /*PointF ptfSectionCenter = new PointF(ptfSection.X + szfSection.Width / 2, ptfSection.Y + szfSection.Height / 2);
            //Point ptSectionCenter = GlobalToScreen(ptfSectionCenter);
            Point ptSectionCenter = GlobalToScreen2(ptfSectionCenter);

            Size szPanel = this.Size;
            Point ptPanelCenter = new Point(szPanel.Width / 2, szPanel.Height / 2);

            Translate(ptSectionCenter.X, ptSectionCenter.Y, ptPanelCenter.X, ptPanelCenter.Y);

            float fScale = 1.0f;

            if (szfSection.Width / szPanel.Width > szfSection.Height / szPanel.Height)
                fScale = szPanel.Width / szfSection.Width / 2;
            else
                fScale = szPanel.Height / szfSection.Height / 2;

            Zoom(ptPanelCenter.X, ptPanelCenter.Y, fScale, true);*/
        }

        public void ZoomPanel()
        {
            bool isFirst = true;
            PointF ptTL = new PointF();
            PointF ptBR = new PointF();

            foreach (Section section in m_arrSection)
            {
                PointF ptfSection = section.Position;
                SizeF szfSection = section.RectSize;

                if (isFirst)
                {
                    isFirst = false;

                    ptTL = new PointF(ptfSection.X, ptfSection.Y);
                    ptBR = new PointF(ptfSection.X + szfSection.Width, ptfSection.Y + szfSection.Height);
                }
                else
                {
                    if (ptTL.X > ptfSection.X) ptTL.X = ptfSection.X;
                    if (ptTL.Y > ptfSection.Y) ptTL.Y = ptfSection.Y;
                    if (ptBR.X < ptfSection.X + szfSection.Width) ptBR.X = ptfSection.X + szfSection.Width;
                    if (ptBR.Y < ptfSection.Y + szfSection.Height) ptBR.Y = ptfSection.Y + szfSection.Height;
                }
            }

            if (isFirst)
                return;

            PointF ptfNewCenter = new PointF((ptTL.X + ptBR.X) / 2, (ptTL.Y + ptBR.Y) / 2);
            ZoomArea(ptfNewCenter, ptBR.X - ptTL.X, ptBR.Y - ptTL.Y, 0.8f);
        }

        public void FocusSection(Section section)
        {
            FocusSection(section, this.Size.Width / 2, this.Size.Height / 2);
        }

        public void FocusSection(Section section, int y)
        {
            FocusSection(section, this.Size.Width / 2, y);
        }

        public void FocusSection(Section section, int x, int y)
        {
            PointF ptfSection = section.Position;
            SizeF szfSection = section.RectSize;

            PointF ptfSectionCenter = new PointF(ptfSection.X + szfSection.Width / 2, ptfSection.Y + szfSection.Height / 2);

            Point ptNewCenter = GlobalToScreen2(ptfSectionCenter);

            Translate(ptNewCenter.X, ptNewCenter.Y, x, y);

            float fScale = 1.0f;

            Zoom(x, y, fScale, true);
        }

        public new void Delete()
        {
            if (m_sectionSelected != null)
            {
                RemoveSection(m_sectionSelected);
                m_sectionSelected = null;
                Refresh();
            }
        }

        private void toolStringMenuTransSOP_Click(object sender, EventArgs e)
        {
            Section section = (Section)(((ToolStripMenuItem)sender).Tag);
            SectionDataTransSOP data = (SectionDataTransSOP)section.Data;
            int nLinkID = data.LinkedActionStepID;
                
            SectionTabPage tabPage = (SectionTabPage)(Parent);
            SectionState state = WorkFlowManager.Instance.Find(section, !tabPage.VirtualMode);
            state.Complete();

            VersionInfo ainfo = SOPMonitoringSystem.FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nLinkID);
            ActionStepInfo info = SOPMonitoringSystem.FormSOP.Instance.SOPManager.GetActionStepInfo(nLinkID);

            SOPMonitoringSystem.FormSOP.Instance.ChangeSOP(ainfo, info, !tabPage.VirtualMode);

        }

        private void toolStripMenuDelete_Click(object sender, System.EventArgs e)
        {
            Delete();
        }
        private void toolStripMenuSkip_Click(object sender, System.EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;                  
                SectionState state = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);
                if (state != null)
                {
                    if (m_sectionSelected.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = m_sectionSelected;
                    } 
                    state.Skip();
                    Refresh();
                }
                   
            }                
        }

        private void toolStripMenuExce_Click(object sender, System.EventArgs e)
        {
            RunSection(m_sectionSelected);
        }

        public override bool RunSection(Section section, bool refresh = true)
        {
            if (section != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
                if (state != null)
                {
                    if (section.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = section;
                    }

                    if (section.GetComponentType() == Section.ComponentType.TRANSMISSION)
                    {
                        TSectionState tstate = (TSectionState)state;
                        tstate.InProgress();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.INTERNAL)
                    {
                        ISectionState istate = (ISectionState)state;
                        istate.InProgress();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.EXTERNAL)
                    {
                        ESectionState estate = (ESectionState)state;
                        estate.InProgress();
                    }
                    else
                        state.InProgress();

                    if (section.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)section.GetSectionPainter(0);
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, state);
                    }
                    
                    if (refresh)
                        Refresh();
                }
            }
            return true;
        }

        public void RunSection(Section section, int nMissionIndex, bool run = true)
        {
            if (section != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                if (state != null)
                {
                    int flag = 1 << nMissionIndex;

                    if (run)
                        state.CheckedRun = state.CheckedRun | flag;
                    else
                        state.CheckedRun = state.CheckedRun & (~flag);
                }
            }
        }

        private void toolStripMenuComplete_Click(object sender, System.EventArgs e)
        {
            CompleteSection(m_sectionSelected);
            
        }

        public override bool CompleteSection(Section section, int processDirection = (int)ProcessDirectionHistory.NONE, bool refresh = true, Sections.Section sectionNext = null)
        {
            if (section != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
                if (state != null)
                {
                    if (state.SelectSection.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = section;
                    }

                    int nProcessDirections = state.ProcessDirections | (int)processDirection;

                    /*if (section.GetComponentType() == Section.ComponentType.TRANSMISSION)
                    {
                        TSectionState tstate = (TSectionState)state;
                        tstate.Complete();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.INTERNAL)
                    {
                        ISectionState istate = (ISectionState)state;
                        istate.Complete();
                    }
                    else if (section.GetComponentType() == Section.ComponentType.EXTERNAL)
                    {
                        ESectionState estate = (ESectionState)state;
                        estate.Complete();
                    }
                    else */
                    if (section.GetComponentType() == Section.ComponentType.DECISION)
                    {
                        state.Parent.DecisionNextSection = sectionNext;
                        state.Complete(nProcessDirections);
                        state.Parent.DecisionNextSection = null;

                        // 동기화 문제로 인하여 Thread를 통해 1초후 실행되도록 한다.
                        SelectSection(sectionNext);
                    }
                    else
                    {
                        state.Complete(nProcessDirections);

                        if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
                        {
                            if (!((SectionDataEndPoint)section.Data).IsBegin)
                                FormSOP.Instance.EnableOptions(true);
                        }
                    }

                    if (refresh)
                        Refresh();

                    return true;
                }
            }

            return false;
        }

        private void SelectSection(Section section)
        {
            if (section != null && section.GetComponentType() == Section.ComponentType.ENDPOINT)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectSectionThread));
                t.Start(section);
            }
        }

        private void SelectSectionThread(object param)
        {
            System.Threading.Thread.Sleep(1000);
            Section section = (Section)param;

            this.Invoke((MethodInvoker)delegate
            {
                ComponentContents contents = FormSOP.Instance.GetPageHome().GetComponentContents(section);

                if (contents != null)
                    FormSOP.Instance.GetPageHome().SelectComponentContents(contents, true);
            });
        }

        private void toolStripMenuCancel_Click(object sender, System.EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;                   
                SectionState state = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);
                if (state != null)
                {
                    if (m_sectionSelected.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = m_sectionSelected;
                    }
                    state.Cancel();
                    Refresh();
                }
            }
        }

        private void toolStripMenuRestart_Click(object sender, System.EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                SectionTabPage page = (SectionTabPage)Parent;                    
                SectionState state = WorkFlowManager.Instance.Find(m_sectionSelected, !page.VirtualMode);
                if (state != null)
                {
                    if (m_sectionSelected.GetComponentType() == Section.ComponentType.LINK)
                    {
                        SectionData data = state.SelectSection.Data;
                        data.AggSection = m_sectionSelected;
                    }

                    if (m_sectionSelected.GetComponentType() == Section.ComponentType.TRANSMISSION)
                    {
                        TSectionState tstate = (TSectionState)state;
                        tstate.InProgress();
                    }
                    else if (m_sectionSelected.GetComponentType() == Section.ComponentType.INTERNAL)
                    {
                        ISectionState istate = (ISectionState)state;
                        istate.InProgress();
                    }
                    else if (m_sectionSelected.GetComponentType() == Section.ComponentType.EXTERNAL)
                    {
                        ESectionState estate = (ESectionState)state;
                        estate.InProgress();
                    }
                    else
                        state.InProgress();
                    Refresh();
                }
            }
        }
             

        public new void ClearSelection()
        {
            if (m_sectionSelected != null)
            {
                m_sectionSelected.Select(false);
                m_sectionSelected = null;
            }
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

        public void SetComponentID(Section section, int nComponentID)
        {
            m_dicComponentID[section] = nComponentID;
        }

        public override int GetComponentID(Section section)
        {
            if (m_dicComponentID.ContainsKey(section))
                return m_dicComponentID[section];

            return -1;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (m_frmLegend != null)
            {
                Point pt = m_frmLegend.Location;
                pt.Y = this.Size.Height - m_frmLegend.Size.Height;
                m_frmLegend.Location = pt;
            }
        }

        public new string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string StepName
        {
            get { return m_strStepName; }
            set { m_strStepName = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public new int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        // 0(평일 비상조직), 1(휴일 및 야간 비상조직), 2(사용자 정의 조직), 3(외부기관)
        public int TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        

        // 범례
        public SOPMonitoringSystem.FormLegend Legend
        {
            get { return m_frmLegend; }
            set { m_frmLegend = value; }
        }

        

        private void PanelSectionEx_SizeChanged(object sender, EventArgs e)
        {
               
        }

        private void PanelSectionEx_BackColorChanged(object sender, EventArgs e)
        {
               
        }

        private void toolStripMenuCloseSOP_Click(object sender, EventArgs e)
        {
            SectionTabPage page = (SectionTabPage)this.Parent;
            WorkFlow workFlow = WorkFlowManager.Instance.Get(m_nActionStepID, !page.VirtualMode);

            if (workFlow != null && workFlow.State != WorkFlowState.DONE)
                return;

            CloseTabPage(page, m_nActionStepID);
        }

        public static void CloseTabPage(SectionTabPage page, int nActionStepID)
        {
            SOPMonitoringSystem.PageBackstageSOP pageHome = SOPMonitoringSystem.FormSOP.Instance.GetPageHome();

            if (!SOPScenarioManager.Instance.RemoveTabPage(page))
            {
                TabPageManager.Instance.RemovePage(nActionStepID, !page.VirtualMode);

                pageHome.ClearComponentContents(page, nActionStepID, !page.VirtualMode);

                pageHome.RemoveTabPage(page);
                pageHome.PanelArray.Clear();
            }

            SOPScenarioManager.Instance.AfterRemoveTabPage();

            if (FormSOP.Instance.GetAllSenario().Count == 0)
            {
                FormSOP.Instance.EnableOptions(false);
            }
            else
            {
                // 시나리오가 제거되면 다른 시나리오가 화면에 표시되므로 관련 이벤트 함수를 호출시킨다.
                FormSOP.Instance.GetPageHome().RefreshPage();
            }
        }

        public void ClearLButtonClick()
        {
            this.m_clickedLButton = false;
        }

        private bool m_bShowSectionBtn = false;
        public void ShowSectionButton(bool bShow)
        {
            m_bShowSectionBtn = bShow;
        }
    }
}
