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
    class Section
    {
        private static Font TEXT_FONT = new Font("맑은고딕", 12);
        private static SolidBrush TEXT_BRUSH = new SolidBrush(Color.Black);
        private static SolidBrush RECT_BRUSH = new SolidBrush(Color.White);
        private static Pen SECTION_PEN = new Pen(Color.Gray, 1);
        private static bool m_transparentFill = false;
        private static bool m_transparentLine = false;
        private static int m_nLineThick = 1;
        private static Color m_clrLine = Color.Gray;
        private static Color m_clrFill = Color.White;
        private static Color m_clrText = Color.Black;

        protected Form m_frmParent = null;
        protected int x = 0, y = 0;
        protected string m_strSectionName = "Test";
        protected int m_nWidth = 100, m_nHeight  = 30;
        protected bool m_isHidden = false;
        protected bool m_isSelected = false;
        protected EditBox m_editBox = new EditBox();
        
        protected EditBox.BoxPosition m_optChangeSize = EditBox.BoxPosition.NO_SELECT;
        protected Point m_ptChangeSizeOrigin = new Point();
        protected Point m_ptOriginStart = new Point();
        protected Size m_sizeOrigin = new Size();

        protected SectionTextBox m_textBox = null;
        protected bool m_isDoubleSelected = false;

        protected Section m_sectionParent = null;
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
            m_textBox.Text = "Test Text";
        }

        [DllImport("gdi32")]
        public static extern int RoundRect(int hdc, int x1, int y1, int x2, int y2, int x3, int y3);

        public void Draw(Graphics g)
        {
            if (m_isHidden) return;

            //RoundRect((int)g.GetHdc(), x, y, x + m_nWidth, y + m_nHeight, 20, 20);
            //g.ReleaseHdc();
            g.FillRectangle(RECT_BRUSH, x, y, m_nWidth, m_nHeight);
            g.DrawRectangle(SECTION_PEN, x, y, m_nWidth, m_nHeight);

            // 가운데 정렬
            /*StringFormat format = new StringFormat();
            format.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, m_strSectionName.Length) });
            Region[] r = g.MeasureCharacterRanges(m_strSectionName, TEXT_FONT, new Rectangle(x, y, m_nWidth, m_nHeight), format);
            RectangleF rect = r[0].GetBounds(g);

            g.DrawString(m_strSectionName, TEXT_FONT, TEXT_BRUSH, x + rect.Width, y + (m_nHeight - rect.Height) / 2);*/
            //g.DrawString(m_strSectionName, TEXT_FONT, TEXT_BRUSH, x, y);
            //g.DrawEllipse(m_pen, x, y, m_nWidth, m_nHeight);

            if (m_isSelected)
                m_editBox.Draw(g);

            float xMB = m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE);
            float yMB = m_editBox.GetCoord(EditBox.CoordType.Y_BOTTOM);

            float fDiff = y + m_nHeight - yMB;

            foreach (Section child in m_arrChildSection)
            {
                g.DrawLine(SECTION_PEN, xMB, yMB + fDiff, child.m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE), child.m_editBox.GetCoord(EditBox.CoordType.Y_TOP) + fDiff);
                child.Draw(g);
            }
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
                section.m_sectionParent = null;
                section.m_textBox.Hide();
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

        public Section GetParentSection()
        {
            return m_sectionParent;
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

            Rectangle rectOrigin = InvalidateRectArea;

            int nWidth = rectOrigin.Width;
            int nHeight = rectOrigin.Height;
            Rectangle rect = new Rectangle(rectOrigin.Left - nWidth, rectOrigin.Top - nHeight, nWidth * 3, nHeight * 3);

            m_frmParent.Invalidate(rect);
        }

        public Point Position
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
                }
            }
        }

        public string SectionName
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
}
