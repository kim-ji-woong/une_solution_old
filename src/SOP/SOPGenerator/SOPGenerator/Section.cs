using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace SOPGen
{
    public class Section
    {
        protected static Font TEXT_FONT = new Font("맑은고딕", 12);
        protected static SolidBrush TEXT_BRUSH = new SolidBrush(Color.Black);
        protected static Pen SECTION_PEN = new Pen(Color.Gray, 1);
        protected static bool m_transparentFill = false;
        protected static bool m_transparentLine = false;
        protected static int m_nLineThick = 1;
        protected static Color m_clrLine = Color.Gray;
        protected static Color m_clrFill = Color.White;
        protected static Color m_clrText = Color.Black;

        protected Form m_frmParent = null;
        protected int x = 0, y = 0;
        //protected string m_strSectionName = "Test";
        protected int m_nWidth = 100, m_nHeight  = 30;
        protected bool m_isHidden = false;
        protected bool m_isSelected = false;
        protected EditBox m_editBox = new EditBox();

        int m_nDiffEditX = 0, m_nDiffEditY = 0;
        int m_nDiffTextX = 0, m_nDiffTextY = 0;
        int m_nInterpolationX = 0, m_nInterpolationY = 0;
        
        protected EditBox.BoxPosition m_optChangeSize = EditBox.BoxPosition.NO_SELECT;
        protected Point m_ptChangeSizeOrigin = new Point();
        protected Point m_ptOriginStart = new Point();
        protected Size m_sizeOrigin = new Size();

        protected SectionTextBox m_textBox = null;
        protected bool m_isDoubleSelected = false;

        protected Section m_sectionParent = null;
        protected Section m_sectionPrev = null;
        protected Section m_sectionNext = null;

        protected ArrayList m_arrChildSection = new ArrayList();

        public enum ColorTarget {LINE, FILL, TEXT};
        

        public Section(Form frmParent)
        {
            m_frmParent = frmParent;

            InitControl();
        }

        public Section(Form frmParent, int x, int y)
        {
            m_frmParent = frmParent;
            
            this.x = x;
            this.y = y;

            InitControl();
        }

        public Section(Form frmParent, int x, int y, int width, int height)
        {
            m_frmParent = frmParent;
            
            this.x    = x;
            this.y    = y;
            m_nWidth  = width;
            m_nHeight = height;

            InitControl();
        }

        private void InitControl()
        {
            m_editBox.Position = new Point(x, y);
            m_editBox.RectSize = new Size(m_nWidth, m_nHeight);

            m_textBox = new SectionTextBox(this);

            m_textBox.Left = x + 3;
            m_textBox.Top = y + 3;
            m_textBox.Size = new Size(m_nWidth - 6, m_nHeight - 6);

            m_textBox.Parent = m_frmParent;
            m_textBox.BorderStyle = BorderStyle.None;
            //m_textBox.Multiline = true;
            m_textBox.TextAlign = HorizontalAlignment.Center;
            m_textBox.Enabled = false;
            m_textBox.BackColor = m_clrFill;
            m_textBox.Text = "";

            m_nDiffEditX = m_editBox.Position.X - x;
            m_nDiffEditY = m_editBox.Position.Y - y;
            m_nDiffTextX = m_textBox.Left - x;
            m_nDiffTextY = m_textBox.Top - y;
        }

        // m_textBox의 text가 strText로 변경되었음
        public virtual void OnTextChanged(string strText)
        {
        }

        public void SetInterpolation(int nInterpolationX, int nInterpolationY)
        {
            m_nInterpolationX = nInterpolationX;
            m_nInterpolationY = nInterpolationY;
        }

        // 두 section간의 연결을 표시한다.
        protected void DrawSectionLink(Section section, Graphics g, bool directLink, ref Vertex2D vCenter)
        {
            if (directLink)
            {
                int cx1 = x + m_nWidth / 2;
                int cy1 = y + m_nHeight / 2;

                int cx2 = section.x + section.m_nWidth / 2;
                int cy2 = section.y + section.m_nHeight / 2;

                Vertex2D vCenter1 = new Vertex2D(cx1, cy1);
                Vertex2D vCenter2 = new Vertex2D(cx2, cy2);

                Vertex2D vTL1 = new Vertex2D(x, y);
                Vertex2D vBL1 = new Vertex2D(x, y + m_nHeight);
                Vertex2D vBR1 = new Vertex2D(x + m_nWidth, y + m_nHeight);
                Vertex2D vTR1 = new Vertex2D(x + m_nWidth, y);

                Vertex2D vTL2 = new Vertex2D(section.x, section.y);
                Vertex2D vBL2 = new Vertex2D(section.x, section.y + section.m_nHeight);
                Vertex2D vBR2 = new Vertex2D(section.x + section.m_nWidth, section.y + section.m_nHeight);
                Vertex2D vTR2 = new Vertex2D(section.x + section.m_nWidth, section.y);

                Line lineT1 = new Line(vTL1, vTR1);
                Line lineL1 = new Line(vTL1, vBL1);
                Line lineR1 = new Line(vBL1, vBR1);
                Line lineB1 = new Line(vTR1, vBR1);
                Line[] arrLine1 = new Line[4] { lineT1, lineL1, lineR1, lineB1 };

                Line lineT2 = new Line(vTL2, vTR2);
                Line lineL2 = new Line(vTL2, vBL2);
                Line lineR2 = new Line(vBL2, vBR2);
                Line lineB2 = new Line(vTR2, vBR2);
                Line[] arrLine2 = new Line[4] { lineT2, lineL2, lineR2, lineB2 };

                Line line = new Line(vCenter1, vCenter2);

                Vertex2D v1, v2;
                
                for (int i = 0; i < 4; i++)
                {
                    int nVertexCount = line.Intersect(arrLine1[i], out v1, out v2);

                    if (nVertexCount >= 1)
                    {
                        vCenter1 = v1;
                        break;
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    int nVertexCount = line.Intersect(arrLine2[i], out v1, out v2);

                    if (nVertexCount >= 1)
                    {
                        vCenter2 = v1;
                        break;
                    }
                }

                g.DrawLine(SECTION_PEN, (int)vCenter1.x, (int)vCenter1.y, (int)vCenter2.x, (int)vCenter2.y);

                vCenter.x = (vCenter1.x + vCenter2.x) / 2;
                vCenter.y = (vCenter1.y + vCenter2.y) / 2;
            }
            else
            {
                int cx2 = section.x + section.m_nWidth / 2;
                int cy2 = section.y + section.m_nHeight / 2;

                g.DrawLine(SECTION_PEN, (int)vCenter.x, (int)vCenter.y, (int)vCenter.x, cy2);
                g.DrawLine(SECTION_PEN, (int)vCenter.x, cy2, section.x, cy2);
            }
        }

        public int GetDiffText(bool isX)
        {
            return isX ? m_nDiffTextX : m_nDiffTextY;
        }

        public int GetDiffEdit(bool isX)
        {
            return isX ? m_nDiffEditX : m_nDiffEditY;
        }

        [DllImport("gdi32")]
        public static extern int RoundRect(int hdc, int x1, int y1, int x2, int y2, int x3, int y3);

        public void Draw(Graphics g)
        {
            if (m_isHidden) return;

            m_textBox.Left += m_nInterpolationX;
            m_textBox.Top += m_nInterpolationY;
            m_nInterpolationX = m_nInterpolationY = 0;

            x = m_textBox.Left - m_nDiffTextX;
            y = m_textBox.Top - m_nDiffTextY;
            m_editBox.Position = new Point(x + m_nDiffEditX, y + m_nDiffEditY);
            
            RoundRect((int)g.GetHdc(), x, y, x + m_nWidth, y + m_nHeight, 20, 20);
            g.ReleaseHdc();

            // 가운데 정렬
            /*StringFormat format = new StringFormat();
            format.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, m_strSectionName.Length) });
            Region[] r = g.MeasureCharacterRanges(m_strSectionName, TEXT_FONT, new Rectangle(x, y, m_nWidth, m_nHeight), format);
            RectangleF rect = r[0].GetBounds(g);

            g.DrawString(m_strSectionName, TEXT_FONT, TEXT_BRUSH, x + rect.Width, y + (m_nHeight - rect.Height) / 2);*/
            //g.DrawString(m_strSectionName, TEXT_FONT, TEXT_BRUSH, x, y);
            //g.DrawEllipse(m_pen, x, y, m_nWidth, m_nHeight);

            int textBoxX = m_textBox.Left;
            int textBoxY = m_textBox.Top = y + 3;

            if (m_isSelected)
                m_editBox.Draw(g);

            Vertex2D vCenter = new Vertex2D();
            int nChildCount = m_arrChildSection.Count;

            for (int i=0;i<nChildCount;i++)
            //foreach (Section child in m_arrChildSection)
            {
                Section child = (Section)m_arrChildSection[i];
                DrawSectionLink(child, g, i == 0, ref vCenter);
                child.Draw(g);
            }

            if (m_sectionNext != null)
                DrawSectionLink(m_sectionNext, g, true, ref vCenter);

            //float xMB = m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE);
            //float yMB = m_editBox.GetCoord(EditBox.CoordType.Y_BOTTOM);

            //foreach (Section child in m_arrChildSection)
            //{
            //    g.DrawLine(SECTION_PEN, xMB, yMB, child.m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE), child.m_editBox.GetCoord(EditBox.CoordType.Y_TOP));
            //    child.Draw(g);
            //}
        }

        public EditBox GetEditBox()
        {
            return m_editBox;
        }

        public void AddChild(Section section)
        {
            if (!m_arrChildSection.Contains(section))
            {
                m_arrChildSection.Add(section);
                section.m_sectionParent = this;
            }
        }

        public void RemoveChild(Section section, bool arrRemove = true)
        {
            if (m_arrChildSection.Contains(section))
            {
                section.RemoveAllChild();

                if (arrRemove)
                    m_arrChildSection.Remove(section);
                section.Hide();

                section.SetPrev(null);
                section.SetNext(null);
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

        public void SetPrev(Section section)
        {
            if (m_sectionPrev == section)
                return;

            //Section sectionPrev = null;
            //Section sectionNext = null;

            if (section != null)
            {
                if (section.m_sectionPrev != null)
                    section.m_sectionPrev.m_sectionNext = section.m_sectionNext;
                if (section.m_sectionNext != null)
                    section.m_sectionNext.m_sectionPrev = section.m_sectionPrev;

                section.m_sectionPrev = m_sectionPrev;
            }

            if (m_sectionPrev == null)
            {
                m_sectionPrev = section;
            }
            else
            {
                m_sectionPrev.m_sectionNext = section;
                m_sectionPrev = section;
            }

            if (section != null)
            {
                section.m_sectionNext = this;
            }
        }

        public void SetNext(Section section)
        {
            if (m_sectionNext == section)
                return;

            if (section != null)
            {
                if (section.m_sectionPrev != null)
                    section.m_sectionPrev.m_sectionNext = section.m_sectionNext;
                if (section.m_sectionNext != null)
                    section.m_sectionNext.m_sectionPrev = section.m_sectionPrev;

                section.m_sectionNext = m_sectionNext;
            }

            if (m_sectionNext == null)
            {
                m_sectionNext = section;
            }
            else
            {
                m_sectionNext.m_sectionPrev = section;
                m_sectionNext = section;
            }

            if (section != null)
            {
                section.m_sectionPrev = this;
            }
        }

        public Section GetNext()
        {
            return m_sectionNext;
        }

        public Section GetPrev()
        {
            return m_sectionPrev;
        }

        public virtual void Show()
        {
            m_textBox.Show();
            m_isHidden = false;
        }

        public virtual void Hide()
        {
            m_textBox.Hide();
            m_isHidden = true;
        }

        public Section GetLastChild()
        {
            int nCount = m_arrChildSection.Count;
            return nCount == 0 ? null : (Section)m_arrChildSection[nCount - 1];
        }

        public ArrayList GetChildSections()
        {
            return m_arrChildSection;
        }

        public Section Select(int x, int y)
        {
            if (x >= this.x && x <= this.x + m_nWidth && y >= this.y && y <= this.y + m_nHeight)
                return this;

            foreach (Section child in m_arrChildSection)
            {
                Section selected = child.Select(x, y);
                if (selected != null)
                    return selected;
            }

            return null;
            /*if (x < this.x || x > this.x + m_nWidth) return false;
            if (y < this.y || y > this.y + m_nHeight) return false;
            return true;*/
        }

        public void Select(bool isSelected, ArrayList arrSection)
        {
            m_isSelected = isSelected;

            if (!m_isSelected)
            {
                m_isDoubleSelected = false;
                m_textBox.Enabled = false;
            }

            // 다중 선택이 안되도록 한다.
            if (isSelected && arrSection != null)
            {
                foreach (Section child in arrSection)
                {
                    child.SelectAll(false, this);
                }
            }
        }

        protected void SelectAll(bool isSelected, Section exceptSection)
        {
            if (this != exceptSection)
                Select(isSelected, null);

            foreach (Section child in m_arrChildSection)
            {
                child.SelectAll(isSelected, exceptSection);
            }
        }

        public void DoubleSelect(bool isSelected)
        {
            if (isSelected)
                m_isDoubleSelected = isSelected;
            else
            {
                if (m_isDoubleSelected)
                {
                    m_textBox.Enabled = true;
                    m_textBox.Focus();
                }

                m_isDoubleSelected = false;
            }
        }

        public Form GetParent()
        {
            return m_frmParent;
        }

        public void Edit()
        {
            DoubleSelect(true);
            DoubleSelect(false);
        }

        public Section GetParentSection()
        {
            return m_sectionParent;
        }

        public virtual void SetText(string text)
        {
            m_textBox.Text = text;
        }

        public TextBox GetTextBox()
        {
            return m_textBox;
        }

        public bool CheckMouse(int x, int y)
        {
            if (m_isSelected)
            {
                m_optChangeSize = m_editBox.CheckMouse((float)x, (float)y);

                if (m_optChangeSize == EditBox.BoxPosition.NO_SELECT)
                {
                    m_frmParent.Cursor = Cursors.Arrow;
                    return false;
                }

                switch (m_optChangeSize)
                {
                    case EditBox.BoxPosition.TOP_LEFT:
                    case EditBox.BoxPosition.BOTTOM_RIGHT:
                        m_frmParent.Cursor = Cursors.SizeNWSE;
                        break;

                    case EditBox.BoxPosition.TOP_RIGHT:
                    case EditBox.BoxPosition.BOTTOM_LEFT:
                        m_frmParent.Cursor = Cursors.SizeNESW;
                        break;

                    case EditBox.BoxPosition.TOP_MIDDLE:
                    case EditBox.BoxPosition.BOTTOM_MIDDLE:
                        m_frmParent.Cursor = Cursors.SizeNS;
                        break;

                    case EditBox.BoxPosition.MIDDLE_LEFT:
                    case EditBox.BoxPosition.MIDDLE_RIGHT:
                        m_frmParent.Cursor = Cursors.SizeWE;
                        break;
                }

                return true;
            }

            m_optChangeSize = EditBox.BoxPosition.NO_SELECT;
            m_frmParent.Cursor = Cursors.Arrow;
            return false;
        }

        public EditBox.BoxPosition GetChangeSizeOption()
        {
            return m_optChangeSize;
        }

        public void SetChangeSizeOriginPoint(int x, int y)
        {
            m_ptChangeSizeOrigin.X = x;
            m_ptChangeSizeOrigin.Y = y;

            m_ptOriginStart.X = this.x;
            m_ptOriginStart.Y = this.y;

            m_sizeOrigin.Width  = m_nWidth;
            m_sizeOrigin.Height = m_nHeight;
        }

        public void ChangeSize(int x, int y)
        {
            if (!m_isSelected) return;
            if (m_optChangeSize == EditBox.BoxPosition.NO_SELECT) return;

            m_isDoubleSelected = false;

            int xMove = x - m_ptChangeSizeOrigin.X;
            int yMove = y - m_ptChangeSizeOrigin.Y;

            switch (m_optChangeSize)
            {
                case EditBox.BoxPosition.TOP_LEFT:
                    Position = new Point(m_ptOriginStart.X + xMove, m_ptOriginStart.Y + yMove);
                    RectSize = new Size(m_sizeOrigin.Width - xMove, m_sizeOrigin.Height - yMove);
                    break;

                case EditBox.BoxPosition.BOTTOM_RIGHT:
                    RectSize = new Size(m_sizeOrigin.Width + xMove, m_sizeOrigin.Height + yMove);
                    break;

                case EditBox.BoxPosition.TOP_RIGHT:
                    Position = new Point(m_ptOriginStart.X, m_ptOriginStart.Y + yMove);
                    RectSize = new Size(m_sizeOrigin.Width + xMove, m_sizeOrigin.Height - yMove);
                    break;

                case EditBox.BoxPosition.BOTTOM_LEFT:
                    Position = new Point(m_ptOriginStart.X + xMove, m_ptOriginStart.Y);
                    RectSize = new Size(m_sizeOrigin.Width - xMove, m_sizeOrigin.Height + yMove);
                    break;

                case EditBox.BoxPosition.TOP_MIDDLE:
                    Position = new Point(m_ptOriginStart.X, m_ptOriginStart.Y + yMove);
                    RectSize = new Size(m_sizeOrigin.Width, m_sizeOrigin.Height - yMove);
                    break;

                case EditBox.BoxPosition.BOTTOM_MIDDLE:
                    Position = new Point(m_ptOriginStart.X, m_ptOriginStart.Y);
                    RectSize = new Size(m_sizeOrigin.Width, m_sizeOrigin.Height + yMove);
                    break;

                case EditBox.BoxPosition.MIDDLE_LEFT:
                    Position = new Point(m_ptOriginStart.X + xMove, m_ptOriginStart.Y);
                    RectSize = new Size(m_sizeOrigin.Width - xMove, m_sizeOrigin.Height);
                    break;

                case EditBox.BoxPosition.MIDDLE_RIGHT:
                    RectSize = new Size(m_sizeOrigin.Width + xMove, m_sizeOrigin.Height);
                    break;
            }

            m_nDiffEditX = m_editBox.Position.X - this.x;
            m_nDiffEditY = m_editBox.Position.Y - this.y;
            m_nDiffTextX = m_textBox.Left - this.x;
            m_nDiffTextY = m_textBox.Top - this.y;

            //Rectangle rectOrigin = InvalidateRectArea;

            //int nWidth = rectOrigin.Width;
            //int nHeight = rectOrigin.Height;
            //Rectangle rect = new Rectangle(rectOrigin.Left - nWidth, rectOrigin.Top - nHeight, nWidth * 3, nHeight * 3);

            m_frmParent.Refresh();
            //m_frmParent.Invalidate(rect, true);
            //m_frmParent.Update();
        }

        public virtual Point Position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                if (x != value.X || y != value.Y)
                {
                    x = value.X;
                    y = value.Y;

                    m_editBox.Position = value;

                    m_textBox.Left = x + 3;
                    m_textBox.Top = y + 3;

                    m_isDoubleSelected = false;

                    m_nDiffEditX = 0;
                    m_nDiffEditY = 0;
                    m_nDiffTextX = 3;
                    m_nDiffTextY = 3;
                }
            }
        }

        public string SectionName
        {
            get
            {
                //return m_strSectionName;
                return m_textBox.Text;
            }
            set
            {
                //m_strSectionName = value;
                m_textBox.Text = value;
            }
        }

        public Rectangle InvalidateRectArea
        {
            get
            {
                int nSmallRectSize = (int)m_editBox.GetSmallRectSize();
                return new Rectangle(x - nSmallRectSize, y - nSmallRectSize, m_nWidth + nSmallRectSize * 2, m_nHeight + nSmallRectSize * 2);
            }
        }

        public Size RectSize
        {
            get
            {
                return new Size(m_nWidth, m_nHeight);
            }
            set
            {
                if (m_nWidth != value.Width || m_nHeight != value.Height)
                {
                    m_nWidth = value.Width;
                    m_nHeight = value.Height;

                    m_editBox.RectSize = value;

                    m_textBox.Size = new Size(m_nWidth - 6, m_nHeight - 6);

                    m_nDiffEditX = 0;
                    m_nDiffEditY = 0;
                    m_nDiffTextX = m_textBox.Left - x;
                    m_nDiffTextY = m_textBox.Top - y;

                    m_isDoubleSelected = false;
                }
            }
        }

        public static void SetColor(ColorTarget trg, Color clr)
        {
            switch (trg)
            {
                case ColorTarget.LINE:
                    m_clrLine = clr;
                    SECTION_PEN.Color = clr;
                    break;

                case ColorTarget.FILL:
                    m_clrFill = clr;
                    break;

                case ColorTarget.TEXT:
                    m_clrText = clr;
                    TEXT_BRUSH.Color = clr;
                    break;
            }
        }

        public static Color GetColor(ColorTarget trg)
        {
            if (trg == ColorTarget.LINE)
                return m_clrLine;
            else if (trg == ColorTarget.FILL)
                return m_clrFill;
            //else if (trg == ColorTarget.TEXT)
                return m_clrText;
        }

        public static void SetLineThick(int nLineThick)
        {
            m_nLineThick = nLineThick;
            SECTION_PEN.Width = nLineThick;
        }

        public static int GetLineThick()
        {
            return m_nLineThick;
        }

        public static void SetTransparency(bool isLine, bool transparency)
        {
            if (isLine)
                m_transparentLine = transparency;
            else
                m_transparentFill = transparency;
        }

        public static bool GetTransparency(bool isLine)
        {
            return isLine ? m_transparentLine : m_transparentFill;
        }

        public static void SetFont(Font font)
        {
            TEXT_FONT = font;
        }

        public static Font GetFont()
        {
            return TEXT_FONT;
        }
    }

    public class Vertex2D
    {
        public double x, y;

        public Vertex2D()
        {
            x = y = 0.0;
        }

        public Vertex2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Line
    {
        protected Vertex2D m_vBegin = null;
        protected Vertex2D m_vEnd = null;

        public Line(Vertex2D vBegin, Vertex2D vEnd)
        {
            m_vBegin = vBegin;
            m_vEnd = vEnd;
        }

        // 두 직선이 만나지 않으면 0을 리턴한다.
        // 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
        // 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
        // 두 직선은 길이가 한정된 선분이다.
        //public static int Intersect(Vertex2D vLine1Begin, Vertex2D vLine1End, Vertex2D vLine2Begin, Vertex2D vLine2End, out Vertex2D rVertex1, out Vertex2D rVertex2)
        public int Intersect(Line lineTrg, out Vertex2D rVertex1, out Vertex2D rVertex2)
        {
            const double HALF_TOLERANCE = 0.0001;
            rVertex1 = new Vertex2D();
            rVertex2 = new Vertex2D();

            Vertex2D vLine1Begin = this.m_vBegin;
            Vertex2D vLine1End = this.m_vEnd;
            Vertex2D vLine2Begin = lineTrg.m_vBegin;
            Vertex2D vLine2End = lineTrg.m_vEnd;

            // vLine1Begin과 vLine1End을 잇는 직선 y = (a1)x + b1
	        // vLine2Begin와 vLine2End를 잇는 직선 y = (a2)x + b2
	        // x = constant 형태의 직선일 경우
	        // 첫번째 직선의 x값 : c1
	        // 두번째 직선의 x값 : c2
	        double[] a = new double[2];
            double[] b = new double[2];
            double[] c = new double[2] {0.0, 0.0};
	        int i, nIndex1, nIndex2;
	        bool[] bXEq = new bool[2] {false, false};	// x = const 형태의 방정식인가?
	        double x, y;
	        Vertex2D[] vArr = new Vertex2D[4] {vLine1Begin, vLine1End, vLine2Begin, vLine2End};

	        for (i=0;i<2;i++)
	        {
		        nIndex1 = i * 2;
		        nIndex2 = nIndex1 + 1;

		        if (Math.Abs(vArr[nIndex1].x - vArr[nIndex2].x) <= HALF_TOLERANCE)
		        {
			        a[i] = b[i] = 0.0;
			        c[i] = vArr[nIndex1].x;
			        bXEq[i] = true;
		        }
		        else if (Math.Abs(vArr[nIndex1].y - vArr[nIndex2].y) <= HALF_TOLERANCE)
		        {
			        a[i] = 0.0;
			        b[i] = vArr[nIndex1].y;
		        }
		        else
		        {
			        a[i] = (vArr[nIndex2].y - vArr[nIndex1].y) / (vArr[nIndex2].x - vArr[nIndex1].x);
			        b[i] = vArr[nIndex2].y - (vArr[nIndex2].y - vArr[nIndex1].y) * vArr[nIndex2].x / (vArr[nIndex2].x - vArr[nIndex1].x);
		        }
	        }

	        if (bXEq[0] && bXEq[1])
	        {
		        if (Math.Abs(c[0] - c[1]) > HALF_TOLERANCE) return 0;

		        double dBig1 = vLine1Begin.y, dSmall1 = vLine1End.y;
		        double dBig2 = vLine2Begin.y, dSmall2 = vLine2End.y;

		        if (dBig1 < vLine1End.y) 
		        {
			        dBig1   = vLine1End.y;
			        dSmall1 = vLine1Begin.y;
		        }
		        if (dBig2 < vLine2End.y) 
		        {
			        dBig2   = vLine2End.y;
			        dSmall2 = vLine2Begin.y;
		        }

		        if ((dBig1 < dSmall2 && Math.Abs(dBig1 - dSmall2) > HALF_TOLERANCE) || (dBig2 < dSmall1 && Math.Abs(dBig2 - dSmall1) > HALF_TOLERANCE)) return 0;
		        else if (Math.Abs(dBig1 - dSmall2) <= HALF_TOLERANCE)
		        {
			        rVertex1.x = c[0];
			        rVertex1.y = dBig1;
			        return 1;
		        }
		        else if (Math.Abs(dBig2 - dSmall1) <= HALF_TOLERANCE)
		        {
			        rVertex1.x = c[0];
			        rVertex1.y = dBig2;
			        return 1;
		        }
		        else if (dBig1 > dSmall2)
		        {
			        if (dBig1 <= dBig2) rVertex1.y = dBig1;
			        else rVertex1.y = dBig2;
			        if (dSmall1 < dSmall2) rVertex2.y = dSmall2;
			        else rVertex2.y = dSmall1;

			        rVertex1.x = rVertex2.x = c[0];
			        return -1;
		        }
		        else //if (dBig2 > dSmall1)
		        {
			        if (dBig2 <= dBig1) rVertex1.y = dBig2;
			        else rVertex1.y = dBig1;
			        if (dSmall2 < dSmall1) rVertex2.y = dSmall1;
			        else rVertex2.y = dSmall2;

			        rVertex1.x = rVertex2.x = c[0];
			        return -1;
		        }
	        }
	        else if (bXEq[0])
	        {
		        x = c[0];
		        y = a[1] * x + b[1];
	        }
	        else if (bXEq[1])
	        {
		        x = c[1];
		        y = a[0] * x + b[0];
	        }
	        else
	        {
		        if (Math.Abs(a[0] - a[1]) <= HALF_TOLERANCE)
		        {
			        if (Math.Abs(b[0] - b[1]) > HALF_TOLERANCE) return 0;

			        double dBig1 = vLine1Begin.x, dSmall1 = vLine1End.x;
			        double dBig2 = vLine2Begin.x, dSmall2 = vLine2End.x;

			        if (dBig1 < vLine1End.x) 
			        {
				        dBig1   = vLine1End.x;
				        dSmall1 = vLine1Begin.x;
			        }
			        if (dBig2 < vLine2End.x) 
			        {
				        dBig2   = vLine2End.x;
				        dSmall2 = vLine2Begin.x;
			        }

			        if ((dBig1 < dSmall2 && Math.Abs(dBig1 - dSmall2) > HALF_TOLERANCE) || (dBig2 < dSmall1 && Math.Abs(dBig2 - dSmall1) > HALF_TOLERANCE)) return 0;
			        else if (Math.Abs(dBig1 - dSmall2) <= HALF_TOLERANCE)
			        {
				        rVertex1.x = dBig1;
				        rVertex1.y = a[0] * dBig1 + b[0];
				        return 1;
			        }
			        else if (Math.Abs(dBig2 - dSmall1) <= HALF_TOLERANCE)
			        {
				        rVertex1.x = dBig2;
				        rVertex1.y = a[0] * dBig2 + b[0];
				        return 1;
			        }
			        else if (dBig1 > dSmall2)
			        {
				        if (dBig1 <= dBig2) rVertex1.x = dBig1;
				        else rVertex1.x = dBig2;
				        if (dSmall1 < dSmall2) rVertex2.x = dSmall2;
				        else rVertex2.x = dSmall1;

				        rVertex1.y = a[0] * rVertex1.x + b[0];
				        rVertex2.y = a[0] * rVertex2.x + b[0];
				        return -1;
			        }
			        else //if (dBig2 > dSmall1)
			        {
				        if (dBig2 <= dBig1) rVertex1.x = dBig2;
				        else rVertex1.x = dBig1;
				        if (dSmall2 < dSmall1) rVertex2.x = dSmall1;
				        else rVertex2.x = dSmall2;

				        rVertex1.y = a[0] * rVertex1.x + b[0];
				        rVertex2.y = a[0] * rVertex2.x + b[0];
				        return -1;
			        }
		        }
		        else
		        {
			        x = (b[1] - b[0]) / (a[0] - a[1]);
			        y = a[0] * x + b[0];
		        }
	        }

	        if (vLine1Begin.x > vLine1End.x)
	        {
		        if (vLine1End.x > x && Math.Abs(vLine1End.x - x) > HALF_TOLERANCE) return 0;
		        if (x > vLine1Begin.x && Math.Abs(x - vLine1Begin.x) > HALF_TOLERANCE) return 0;
	        }
	        else
	        {
		        if (vLine1Begin.x > x && Math.Abs(vLine1Begin.x - x) > HALF_TOLERANCE) return 0;
		        if (x > vLine1End.x && Math.Abs(x - vLine1End.x) > HALF_TOLERANCE) return 0;
	        }
	        if (vLine1Begin.y > vLine1End.y)
	        {
		        if (vLine1End.y > y && Math.Abs(vLine1End.y - y) > HALF_TOLERANCE) return 0;
		        if (y > vLine1Begin.y && Math.Abs(y - vLine1Begin.y) > HALF_TOLERANCE) return 0;
	        }
	        else
	        {
		        if (vLine1Begin.y > y && Math.Abs(vLine1Begin.y - y) > HALF_TOLERANCE) return 0;
		        if (y > vLine1End.y && Math.Abs(y - vLine1End.y) > HALF_TOLERANCE) return 0;
	        }

	        if (vLine2Begin.x > vLine2End.x)
	        {
		        if (vLine2End.x > x && Math.Abs(vLine2End.x - x) > HALF_TOLERANCE) return 0;
		        if (x > vLine2Begin.x && Math.Abs(x - vLine2Begin.x) > HALF_TOLERANCE) return 0;
	        }
	        else
	        {
		        if (vLine2Begin.x > x && Math.Abs(vLine2Begin.x - x) > HALF_TOLERANCE) return 0;
		        if (x > vLine2End.x && Math.Abs(x - vLine2End.x) > HALF_TOLERANCE) return 0;
	        }
	        if (vLine2Begin.y > vLine2End.y)
	        {
		        if (vLine2End.y > y && Math.Abs(vLine2End.y - y) > HALF_TOLERANCE) return 0;
		        if (y > vLine2Begin.y && Math.Abs(y - vLine2Begin.y) > HALF_TOLERANCE) return 0;
	        }
	        else
	        {
		        if (vLine2Begin.y > y && Math.Abs(vLine2Begin.y - y) > HALF_TOLERANCE) return 0;
		        if (y > vLine2End.y && Math.Abs(y - vLine2End.y) > HALF_TOLERANCE) return 0;
	        }

	        rVertex1.x = x;
	        rVertex1.y = y;
	        return 1;
        }
    }
}
