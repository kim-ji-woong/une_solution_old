using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace Sections
{
    public delegate void SectionButtonClicked(Sections.SectionButton btn, int x, int y);    

    public abstract class SectionButton
    {
        protected static Font TEXT_FONT = new Font("나눔바른고딕", 11, FontStyle.Bold);
        private static SolidBrush TEXT_BRUSH = new SolidBrush(Color.Black);

        protected PositionManager m_posMgr = null;
        protected SizeManager m_sizeMgr = null;
        protected Shape m_shape = null;

        protected PanelSection m_ctrlParent = null;
        protected string m_strSectionName = "";
        protected bool m_isHidden = false;
        protected bool m_isSelected = false;
        protected EditBox m_editBox = null;

        protected string m_strText = "";

        protected Section m_sectionParent = null;
        protected ArrayList m_arrChildSection = new ArrayList();

        protected StringFormat m_textFormat = new StringFormat();
        protected StringFormat m_textRightFormat = new StringFormat();

        protected static Pen BOUNDARY_PEN = new Pen(Color.FromArgb(185, 255, 185), 1);
        protected static Brush m_brushText = new SolidBrush(Color.FromArgb(58, 58, 58));

        protected bool m_isMouseOver = false;
        protected PointF m_ptMouseCursor = new PointF();

        // 자동 Scroll을 위한 버튼
        // 화면에 표시하지 않으며, 실제 Section 영역보다 조금더 오른쪽 아래에 위치하도록 하여
        // Section 위치에 따라 화면이 스크롤 되도록 한다.
        protected Button m_btnScroll = new Button();

        protected SectionData m_data = null;
            
        protected ArrayList m_arrArrows = new ArrayList();

        protected int m_nCompleteCount = 0;


        public enum ColorTarget { LINE, FILL, TEXT };
        public enum ComponentType { PROCESS = 0, DECISION, ANNOTATION, ENDPOINT, LINK, TRANSSOP, INTERNAL, EXTERNAL, TRANSMISSION, GROUP, NONE }

        protected static Size m_MinSize = new Size(80, 50);
        public static Size MinSize
        {
            get { return m_MinSize; }
            set
            {
                if (value == null)
                    return;
                m_MinSize = value;
                Shape.MinSize = m_MinSize;
                EditBox.MinSize = m_MinSize;
            }
        }
               
        public virtual string Title
        {
            get 
            {                
                return m_strText; 
            }
            set 
            {
                m_strText = value; 
            }
        }

		protected bool m_bGroupMember = false;
		public bool GroupMember
		{
			get { return m_bGroupMember; }
			set { m_bGroupMember = value; }
		}
		protected Section m_GroupSection = null;
		public Sections.Section GroupSection
		{
			get { return m_GroupSection; }
			set { m_GroupSection = value; }
		}

        public bool Hidden
        {
            get { return m_isHidden; }
        }

        public SectionButton()
        {
            m_editBox = new EditBox(mSection);
        }

        private Section mSection = null;
        public Section Section
        {
            get { return mSection; }
            set { mSection = value; }
        }

        public SectionButton(PanelSection ctrlParent)
        {
            mSection = new SectionEndPoint(ctrlParent);

            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(mSection);

            m_shape = new Shape(mSection);
            m_posMgr = new PositionManager(mSection, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            //InitControl();
        }

        public SectionButton(PanelSection ctrlParent, float x, float y)
        {
            mSection = new SectionEndPoint(ctrlParent);

            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(mSection);

            m_shape = new Shape(mSection);
            m_posMgr = new PositionManager(mSection, m_shape, m_btnScroll, m_editBox, x, y);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

			MovingStartPosition = new PointF(x, y);
            //InitControl();
        }

        public SectionButton(PanelSection ctrlParent, float x, float y, ArrayList arrBoundary)
        {
            mSection = new SectionEndPoint(ctrlParent);
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(mSection);

            m_shape = new Shape(mSection);
            m_posMgr = new PositionManager(mSection, m_shape, m_btnScroll, m_editBox, x, y);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

			MovingStartPosition = new PointF(x, y);

            SetBoundary(arrBoundary);
            //InitControl();
        }

        public abstract ComponentType GetComponentType();
        public abstract void MakeData(string strStepName, string strTeamName);
        public abstract Section Clone(PanelSection ctrlParent);

        public void SetBoundary(ArrayList arrBoundary)
        {
            PointF ptCurrent = m_posMgr.Position;
            float fHScrollPos = 0, fVScrollPos = 0;

            m_shape.SetBoundary(arrBoundary, ptCurrent.X, ptCurrent.Y);

            InitControl(ptCurrent, fHScrollPos, fVScrollPos);
        }

        private void InitControl(PointF ptCurrent, float fHScrollPos, float fVScrollPos)
        {
            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            if (fWidth <= 0 || fHeight <= 0)
                return;

            m_editBox.Position = new PointF(ptCurrent.X, ptCurrent.Y);
            m_editBox.RectSize = new SizeF(fWidth, fHeight);

            if (m_ctrlParent != null)
            {
                // 화면 Scroll을 위한 버튼으로 화면에 보이지 않는다.
                m_ctrlParent.Controls.Add(m_btnScroll);

                m_btnScroll.Location = new System.Drawing.Point((int)(ptCurrent.X - fHScrollPos + GetScrollButtonArea(true)), (int)(ptCurrent.Y - fVScrollPos + GetScrollButtonArea(false)));
                m_btnScroll.Name = "btnSectionScroll";
                m_btnScroll.Size = new System.Drawing.Size(0, 0);
                m_btnScroll.TabIndex = 0;
                m_btnScroll.Text = "";
                m_btnScroll.UseVisualStyleBackColor = true;
            }
        }

        public virtual void AdjustStringFormat()
        {
            m_textFormat = GetStringFormat();
            m_textRightFormat = GetRightStringFormat();
        }

        public virtual StringFormat GetStringFormat()
        {
            StringFormat format = new StringFormat();

            // Set the LineAlignment and Alignment properties for 
            // both StringFormat objects to different values.

            
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            if (m_data != null)
            {
                if (m_data.TextHorizontalAlign == SectionData.TextHAlign.RIGHT)
                {
                    if( format.FormatFlags == StringFormatFlags.DirectionRightToLeft)
                        format.Alignment = StringAlignment.Near;
                    else
                        format.Alignment = StringAlignment.Far;
                }
                else if (m_data.TextHorizontalAlign == SectionData.TextHAlign.MIDDLE)
                {
                    format.Alignment = StringAlignment.Center;
                }
                else if (m_data.TextHorizontalAlign == SectionData.TextHAlign.LEFT)
                {
                    if (format.FormatFlags == StringFormatFlags.DirectionRightToLeft)
                        format.Alignment = StringAlignment.Far;
                    else
                        format.Alignment = StringAlignment.Near;
                }

                if (m_data.TextVerticalAlign == SectionData.TextVAlign.UP)
                {
                    format.LineAlignment = StringAlignment.Near;                   
                }
                else if (m_data.TextVerticalAlign == SectionData.TextVAlign.MIDDLE)
                {
                    format.LineAlignment = StringAlignment.Center;
                }
                else if (m_data.TextVerticalAlign == SectionData.TextVAlign.BOTTOM)
                {                    
                    format.LineAlignment = StringAlignment.Far;
                }
                
            }

            return format;
        }

        public virtual StringFormat GetRightStringFormat()
        {
            StringFormat format = new StringFormat();

            // Set the LineAlignment and Alignment properties for 
            // both StringFormat objects to different values.
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Far;


            return format;
        }

        public void Transform()
        {
            m_shape.Transform();

            foreach (Section child in m_arrChildSection)
            {
                child.Transform();
            }
        }
            
        public virtual void DrawText(Graphics g, PointF ptCurrent)
        {
            float xMB = m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE);
            float yMB = m_editBox.GetCoord(EditBox.CoordType.Y_BOTTOM);

            float fFontHeight = TEXT_FONT.Size;

            float fWidth = m_shape.GetSize(true) - 6;
            float fHeight = m_shape.GetSize(false) - 6;
            float x = ptCurrent.X + 3;
            float y = ptCurrent.Y + 6;

            string szDisplayText = m_strText;
            if (m_data != null)
            {
                if (m_data.ShowTempExpression == true)
                    szDisplayText = m_data.Expression;
            }

            if (m_ctrlParent.VisibleSectionNumber && m_data.SectionNumber > 0)
                szDisplayText = m_data.SectionNumber.ToString() + ". " + szDisplayText;

            g.DrawString(szDisplayText, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);
            //g.DrawString(m_strText, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);
        }

        protected virtual void DrawCompleteCount(Graphics g)
        {
            if (m_nCompleteCount <= 0)
                return;

            string strCount = string.Format("({0})", m_nCompleteCount);
            SizeF sizeCount = g.MeasureString(strCount, TEXT_FONT);

            float x = m_editBox.GetCoord(EditBox.CoordType.X_RIGHT) - sizeCount.Width;
            float y = m_editBox.GetCoord(EditBox.CoordType.Y_TOP) - sizeCount.Height;

            g.DrawString(strCount, TEXT_FONT, Brushes.Black, new RectangleF(x, y, sizeCount.Width + 2, sizeCount.Height + 2), m_textRightFormat);
        }

        public void DrawArrow(Graphics g)
        {
            foreach (Arrow arrow in m_arrArrows)
            {
                // 화살표가 두 번 그려지는 것을 막기 위하여 시작 링크일때만 그리도록 한다.
                if (arrow.BeginLink == mSection)
                    arrow.Draw(g);
            }
        }

        public virtual void Draw(Graphics g)
        {
            if (m_isHidden || m_ctrlParent == null) return;

            if (m_posMgr == null)
                return;
            Point[] bound = m_shape.ClipBoundRect;
            int mx1 = Math.Abs(bound[0].X - bound[1].X);
            int my1 = Math.Abs(bound[0].Y - bound[1].Y);
               
            PointF pLoc = new PointF(bound[0].X, bound[0].Y);
            Size pSize = new Size(mx1, my1);
            RectangleF rectComp = new RectangleF(pLoc, pSize);
            if (g.ClipBounds.Contains(rectComp) || g.ClipBounds.IntersectsWith(rectComp))
            {
                PointF ptCurrent = m_posMgr.Position;

                if (m_shape == null || !m_shape.Draw(g, ptCurrent.X, ptCurrent.Y))
                    return;


                if (m_isSelected)
                    m_editBox.Draw(g);

                if (m_isMouseOver && Editable == true)
                    m_editBox.DrawArrowPoint(g, m_ptMouseCursor);

                DrawText(g, ptCurrent);
                DrawCompleteCount(g);

                foreach (ISectionPainter listener in m_arSectionPainter)
                {
                    listener.Draw(g);
                }
                //if (m_additionalPainter != null)
               //     m_additionalPainter.Draw(g);
            }

            //DrawArrow(g);
        }

        public Arrow.ArrowPosition GetArrowStartPosition(float x, float y)
        {
            if (!m_isMouseOver)
                return Arrow.ArrowPosition.NONE;


            EditBox.BoxPosition pos = m_editBox.GetArrowPosition(new PointF(x, y));

            if (pos == EditBox.BoxPosition.TOP_MIDDLE)
                return Arrow.ArrowPosition.TOP;
            else if (pos == EditBox.BoxPosition.MIDDLE_LEFT)
                return Arrow.ArrowPosition.LEFT;
            else if (pos == EditBox.BoxPosition.MIDDLE_RIGHT)
                return Arrow.ArrowPosition.RIGHT;
            else if (pos == EditBox.BoxPosition.BOTTOM_MIDDLE)
                return Arrow.ArrowPosition.BOTTOM;

            return Arrow.ArrowPosition.NONE;
        }

        public EditBox GetEditBox()
        {
            return m_editBox;
        }

        public void AddChild(Section section)
        {
 
        }

        public void RemoveChild(Section section, bool arrRemove = true)
        {
            if (m_arrChildSection.Contains(section))
            {
                section.RemoveAllArrow();
                section.RemoveAllChild();

                if (arrRemove)
                    m_arrChildSection.Remove(section);
              
            }
        }

        public void RemoveAllChild()
        {
            foreach (Section child in m_arrChildSection)
            {
                RemoveChild(child, false);
            }

            m_arrChildSection.Clear();
        }

        public void Show()
        {
            m_isHidden = false;
        }

        public void Hide()
        {
            m_isHidden = true;
        }

        // x, y : 화면 Scroll을 고려하지 않은 화면 좌표
        public virtual SectionButton Select(float x, float y)
        {

			// 보이지 않는 상태에서는 Select가 되지 않도록 변경
			if (m_isHidden == true)
				return null;

            if (m_shape.Select(x, y))
            {
                //System.Diagnostics.Trace.WriteLine("SelectedSection : " + Position.X.ToString() + ", " + Position.Y.ToString());
                return this;
            }

            return null;
        }

        public virtual Section Select(Rectangle rectF)
        {
            // 보이지 않는 상태에서는 Select가 되지 않도록 변경
            if (m_isHidden == true)
                return null;

            if (m_shape.Select(rectF))
            {
                return mSection;
            }

            foreach (Section child in m_arrChildSection)
            {
                Section selected = child.Select(rectF);
                if (selected != null)
                {                   
                    return selected;
                }
            }
            return null;
        }

        // x, y : 화면 Scroll을 고려하지 않은 화면 좌표
        // includeText : Text 영역까지 포함하여 영역 선택할 것인가?
        public virtual Arrow SelectArrow(float x, float y, float fSelectDistance, bool includeText)
        {
            foreach (Arrow arrow in m_arrArrows)
            {
                if (arrow.Select(x, y, fSelectDistance, includeText))
                    return arrow;
            }

            foreach (Section child in m_arrChildSection)
            {
                Arrow arrow = child.SelectArrow(x, y, fSelectDistance, includeText);
                if (arrow != null) return arrow;
            }

            return null;
        }

        public void Select(bool isSelected)
        {
            m_isSelected = isSelected;
        }

        protected void SelectAll(bool isSelected, Section exceptSection)
        {
        }

        public PanelSection GetParent()
        {
            return m_ctrlParent;
        }

        public Section GetParentSection()
        {
            return m_sectionParent;
        }

        public ArrayList GetChildSections()
        {
            return m_arrChildSection;
        }

        /*public TextBox GetTextBox()
        {
            return m_textBox;
        }*/

        public bool CheckMouse(float x, float y)
        {
            return m_sizeMgr.CheckMouse(x, y, m_isSelected, m_ctrlParent);
        }

        public EditBox.BoxPosition GetChangeSizeOption()
        {
            return m_sizeMgr.GetChangeSizeOption();
        }

        public void SetChangeSizeOriginPoint(float x, float y)
        {
            m_sizeMgr.SetChangeSizeOriginPoint(x, y, m_posMgr.Position, m_shape.GetSize(true), m_shape.GetSize(false));
        }

        public void ChangeSize(float x, float y)
        {
            if (!m_isSelected) return;

            if (m_sizeMgr.ChangeSize(x, y, m_posMgr))
            {
                CalcArrowPositions();
                m_ctrlParent.Refresh();
            }
        }

        public void ShowArrowPoint(PointF ptMouseCursor)
        {
            if (!Editable)
                return;

            m_isMouseOver = true;
            m_ptMouseCursor = ptMouseCursor;
        }

        public void HideArrowPoint()
        {
            m_isMouseOver = false;
        }

        public float GetScrollButtonArea(bool isHorz)
        {
            return isHorz ? m_shape.GetSize(true) + 10 : m_shape.GetSize(false) + 10;
        }


        public virtual PointF Position
        {
            get
            {
                return m_posMgr.Position;
            }
            set
            {
                m_posMgr.Position = value;
            }
        }

        protected PointF mMovingStartPos;
        public PointF MovingStartPosition
        {
            get
            {
                return mMovingStartPos;
            }
            set
            {
                mMovingStartPos = value;
            }
        }


		protected PointF m_ptCollapse;
		public virtual PointF CollapsePosition
		{
			get
			{
				return m_ptCollapse;
			}
			set
			{
				m_ptCollapse = value;
			}
		}

		public virtual string SectionName
        {
            get
            {
                return m_strSectionName;
            }
            set
            {
                m_strSectionName = value;
            }
        }

		public virtual RectangleF InvalidateRectArea
        {
            get
            {
                PointF ptCurrent = m_posMgr.Position;

                float fSmallRectSize = m_editBox.GetSmallRectSize();
                return new RectangleF(ptCurrent.X - fSmallRectSize , ptCurrent.Y - fSmallRectSize, m_shape.GetSize(true) + fSmallRectSize * 2, m_shape.GetSize(false) + fSmallRectSize * 2);
            }
        }

		public virtual SizeF RectSize
        {
            get
            {
                return m_sizeMgr.RectSize;
            }
            set
            {
                m_sizeMgr.RectSize = value;
            }
        }

		public virtual void SetColor(ColorTarget trg, Color clr)
        {
            switch (trg)
            {
                case ColorTarget.LINE:
                    Shape.SetLineColor(clr);
                    break;

                case ColorTarget.FILL:
                    m_shape.SetFillColor(clr);
                    break;

                case ColorTarget.TEXT:
                    TEXT_BRUSH.Color = clr;
                    break;
            }
        }

		public virtual Color GetColor(ColorTarget trg)
        {
            if (trg == ColorTarget.LINE)
                return Shape.GetLineColor();
            else if (trg == ColorTarget.FILL)
                return Shape.GetFillColor();
            return TEXT_BRUSH.Color;
        }

        public static void SetLineThick(int nLineThick)
        {
            Shape.SetLineThick(nLineThick);
        }

        public static int GetLineThick()
        {
            return Shape.GetLineThick();
        }

        public void SetTransparency(bool isLine, bool transparency)
        {
            m_shape.SetTransparency(isLine, transparency);
        }

        public bool GetTransparency(bool isLine)
        {
            return m_shape.GetTransparency(isLine);
        }

        public static void SetFont(Font font)
        {
            TEXT_FONT = font;
        }

        public static Font GetFont()
        {
            return TEXT_FONT;
        }

        public bool NeedMouseOverRefresh(PointF ptMouseCursor)
        {
            if (!m_isMouseOver)
                return false;

            if (ptMouseCursor == m_ptMouseCursor)
                return false;

            return m_editBox.NeedMouseOverRefresh(m_ptMouseCursor, ptMouseCursor);
        }

        public bool InArrowArea(PointF ptMouseCursor)
        {
            return m_editBox.InArrowArea(ptMouseCursor);
        }

        public PointF GetArrowPoint(Arrow.ArrowPosition pos, out bool isSuccess)
        {
            isSuccess = true;

            PointF pt = m_posMgr.Position;
            SizeF sizeSection = m_sizeMgr.RectSize;

            float thick = Shape.OutLineThick / 2.0f;
            if (pos == Arrow.ArrowPosition.LEFT)
            {
                return new PointF(pt.X - thick, pt.Y + sizeSection.Height / 2);
            }
            else if (pos == Arrow.ArrowPosition.TOP)
            {
                return new PointF(pt.X + sizeSection.Width / 2, pt.Y - thick);
            }
            else if (pos == Arrow.ArrowPosition.RIGHT)
            {
                return new PointF(pt.X + sizeSection.Width + thick, pt.Y + sizeSection.Height / 2);
            }
            else if (pos == Arrow.ArrowPosition.BOTTOM)
            {
                return new PointF(pt.X + sizeSection.Width / 2, pt.Y + sizeSection.Height + thick);
            }

            isSuccess = false;
            return new PointF(0, 0);
        }

        public virtual bool IsAddableArrow(Arrow arrow)
        {
            Section sectionBegin = arrow.BeginLink;
            if (sectionBegin == null)
                return false;

            Section sectionEnd = arrow.EndLink;
            if (sectionEnd == null)
                return false;

            foreach (Arrow _arrow in m_arrArrows)
            {
                if (_arrow.BeginLink == sectionBegin && _arrow.EndLink == sectionEnd)
                    return false;
            }            
            return true;
        }

        public virtual bool AddArrow(Arrow arrow)
        {
            Section sectionBegin = arrow.BeginLink;
            if (sectionBegin == null)
                return false;

            Section sectionEnd = arrow.EndLink;
            if (sectionEnd == null)
                return false;

            foreach (Arrow _arrow in m_arrArrows)
            {
                if (_arrow.BeginLink == sectionBegin && _arrow.EndLink == sectionEnd)
                    return false;
            }

            m_arrArrows.Add(arrow);
            return true;
        }
        public void Notify(bool bNoti)
        {
            m_shape.SetNotify(bNoti);
        }
        public bool RemoveArrow(Arrow arrow)
        {
            Section sectionBegin = arrow.BeginLink;
            if (sectionBegin == null)
                return false;

            Section sectionEnd = arrow.EndLink;
            if (sectionEnd == null)
                return false;

            Section other = null;

            if (m_arrArrows.Contains(arrow))
                m_arrArrows.Remove(arrow);

            return true;
        }

        public void RemoveAllArrow()
        {
            int nArrowCount = m_arrArrows.Count;

            for (int i=nArrowCount-1;i>=0;i--)
            {
                Arrow arrow = (Arrow)m_arrArrows[i];

                Section sectionBegin = arrow.BeginLink;
                Section sectionEnd = arrow.EndLink;



                arrow.BeginLink = arrow.EndLink = null;
            }

            m_arrArrows.Clear();
        }

        public void CalcArrowPositions()
        {
            foreach (Arrow arrow in m_arrArrows)
            {
                arrow.CalcArrowLine();
            }
        }


        public event SectionButtonClicked ButtonClicked;
        public void PerformClick(int x, int y)
        {
            ButtonClicked(this, x, y);
        }

        public SectionData Data
        {
            get { return m_data; }
            set
            { 
                m_data = value;
                if( m_data != null)
                {

                    AdjustStringFormat();
                }                
            }
        }

        public ArrayList Arrows
        {
            get { return m_arrArrows; }
            set { m_arrArrows = value; }
        }

		// 이 Section에서 화살표가 시작될 수 있는가?
        public virtual bool ArrowBegin
        {
            get { return true; }
        }

        public int CompleteCount
        {
            get { return m_nCompleteCount; }
            set { m_nCompleteCount = value; }
        }

        public virtual bool Editable
        {
            get { return ((PanelSection)m_ctrlParent).Editable && m_sizeMgr.Editable; }
            set { m_sizeMgr.Editable = value; }
        }

		public virtual bool Movable
		{
			get { return m_posMgr.Editable; }
			set { m_posMgr.Editable = value; }
		}

        protected ArrayList m_arSectionPainter = new ArrayList();
        public void AddSectionPainter(ISectionPainter painter)
        {
            if(!m_arSectionPainter.Contains(painter))
            {
                m_arSectionPainter.Add(painter);
            }
        }

        public ISectionPainter GetSectionPainter(int nIdx)
        {
            if (nIdx < 0 || nIdx >= m_arSectionPainter.Count)
                return null;
            return (ISectionPainter)(m_arSectionPainter[nIdx]);
        }

        public Sections.Shape Shape
        {
            get { return m_shape; }
            set { m_shape = value; }
        }
    }
}
