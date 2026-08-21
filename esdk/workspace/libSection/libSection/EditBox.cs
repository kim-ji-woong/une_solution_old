using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sections
{
    public class EditBox
    {
        private static Pen LINE_PEN = new Pen(Color.Black, 1);
        private static SolidBrush BOX_BRUSH = new SolidBrush(Color.Aqua);
        private static float m_fRectSize = 6;
        private static Color m_clrFill = Color.Aqua;
        public static Color FillColor
        {
            get { return EditBox.m_clrFill; }
            set 
            {
                EditBox.m_clrFill = value;
                
                SolidBrush temp1 = new SolidBrush(value);
                SolidBrush temp2 = BOX_BRUSH;
                BOX_BRUSH = temp1;
                if (temp2!= null)
                    temp2.Dispose();
                
            }
        }
        private static float m_fTriangleSize = 15;

        private static SolidBrush LIGHT_TRI_BRUSH = new SolidBrush(Color.FromArgb(185, 223, 255));
        private static SolidBrush DARK_TRI_BRUSH = new SolidBrush(Color.FromArgb(0, 162, 232));

        protected float x = 0, y = 0;
        protected float m_fWidth = 0, m_fHeight = 0;

        protected float m_xL, m_xM, m_xR, m_yT, m_yM, m_yB;

        protected PointF[] m_arrTriangle = new PointF[3];     // Drawing 용
        protected PointF[] m_arrCheckTriangle = new PointF[3];// 계산용

        private Section m_sectionParent = null;

        protected static Size m_MinSize = new Size(100, 50);
        public static Size MinSize
        {
            get { return m_MinSize; }
            set
            {
                if (value == null)
                    return;
                m_MinSize = value;               
            }
        }

        public enum BoxPosition { NO_SELECT, TOP_LEFT, TOP_MIDDLE, TOP_RIGHT, MIDDLE_LEFT, MIDDLE_RIGHT, BOTTOM_LEFT, BOTTOM_MIDDLE, BOTTOM_RIGHT };
        public enum CoordType { X_LEFT, X_MIDDLE, X_RIGHT, Y_TOP, Y_MIDDLE, Y_BOTTOM };

        public EditBox(Section sectionParent)
        {
            m_sectionParent = sectionParent;
        }

        public EditBox(float x, float y, float width, float height, Section sectionParent)
        {
            this.x = x;
            this.y = y;
            m_fWidth = width;
            m_fHeight = height;
            m_sectionParent = sectionParent;
        }

        public float GetSmallRectSize()
        {
            return m_fRectSize;
        }

        public void Draw(Graphics g)
        {
            //g.DrawRectangle(LINE_PEN, x, y, m_nWidth, m_nHeight);
            DrawRect(g, m_xL , m_yT);
            DrawCircle(g, m_xM, m_yT );
            DrawRect(g, m_xR , m_yT );
            DrawCircle(g, m_xR , m_yM );
            DrawRect(g, m_xR, m_yB );
            DrawCircle(g, m_xM, m_yB);
            DrawRect(g, m_xL, m_yB);
            DrawCircle(g, m_xL, m_yM );
        }

        public void DrawArrowPoint(Graphics g, PointF ptMouseCursor)
        {
            float xL = x , xM = x + m_fWidth / 2 , xR = x + m_fWidth;
            float yT = y , yM = y + m_fHeight / 2 , yB = y + m_fHeight ;

            DrawTriangle(g, xM, yT, BoxPosition.TOP_MIDDLE, ptMouseCursor);
            DrawTriangle(g, xL, yM, BoxPosition.MIDDLE_LEFT, ptMouseCursor);
            DrawTriangle(g, xR, yM, BoxPosition.MIDDLE_RIGHT, ptMouseCursor);
            DrawTriangle(g, xM, yB, BoxPosition.BOTTOM_MIDDLE, ptMouseCursor);
        }

        public BoxPosition GetArrowPosition(PointF ptMouseCursor)
        {
            if (m_sectionParent.Editable)
            {
                float xL = x, xM = x + m_fWidth / 2 , xR = x + m_fWidth;
                float yT = y, yM = y + m_fHeight / 2, yB = y + m_fHeight;

                float left = 0, top = 0, right = 0, bottom = 0;

                if (GetArrowTriangle(xM, yT, BoxPosition.TOP_MIDDLE, ref m_arrCheckTriangle, ref left, ref top, ref right, ref bottom))
                {
                    if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                        return BoxPosition.TOP_MIDDLE;
                }
                else
                    return BoxPosition.NO_SELECT;

                if (GetArrowTriangle(xL, yM, BoxPosition.MIDDLE_LEFT, ref m_arrCheckTriangle, ref left, ref top, ref right, ref bottom))
                {
                    if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                        return BoxPosition.MIDDLE_LEFT;
                }
                else
                    return BoxPosition.NO_SELECT;

                if (GetArrowTriangle(xR, yM, BoxPosition.MIDDLE_RIGHT, ref m_arrCheckTriangle, ref left, ref top, ref right, ref bottom))
                {
                    if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                        return BoxPosition.MIDDLE_RIGHT;
                }
                else
                    return BoxPosition.NO_SELECT;

                if (GetArrowTriangle(xM, yB, BoxPosition.BOTTOM_MIDDLE, ref m_arrCheckTriangle, ref left, ref top, ref right, ref bottom))
                {
                    if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                        return BoxPosition.BOTTOM_MIDDLE;
                }
                else
                    return BoxPosition.NO_SELECT;
            }

            return BoxPosition.NO_SELECT;
        }

        protected bool GetArrowTriangle(float x, float y, BoxPosition pos, ref PointF[] arrTriangle, ref float left, ref float top, ref float right, ref float bottom)
        {
            if (pos == BoxPosition.TOP_MIDDLE)
            {
                y -= 3;

                arrTriangle[0].X = x - m_fTriangleSize / 2;
                arrTriangle[0].Y = y;
                arrTriangle[1].X = arrTriangle[0].X + m_fTriangleSize;
                arrTriangle[1].Y = y;
                arrTriangle[2].X = x;
                arrTriangle[2].Y = y - m_fTriangleSize;

                left = arrTriangle[0].X;
                top = arrTriangle[2].Y;
                right = arrTriangle[1].X;
                bottom = y;
            }
            else if (pos == BoxPosition.MIDDLE_LEFT)
            {
                x -= 3;

                arrTriangle[0].X = x;
                arrTriangle[0].Y = y + m_fTriangleSize / 2;
                arrTriangle[1].X = x;
                arrTriangle[1].Y = arrTriangle[0].Y - m_fTriangleSize;
                arrTriangle[2].X = x - m_fTriangleSize;
                arrTriangle[2].Y = y;

                left = arrTriangle[2].X;
                top = arrTriangle[1].Y;
                right = x;
                bottom = arrTriangle[0].Y;
            }
            else if (pos == BoxPosition.MIDDLE_RIGHT)
            {
                x += 3;

                arrTriangle[0].X = x;
                arrTriangle[0].Y = y + m_fTriangleSize / 2;
                arrTriangle[1].X = x;
                arrTriangle[1].Y = arrTriangle[0].Y - m_fTriangleSize;
                arrTriangle[2].X = x + m_fTriangleSize;
                arrTriangle[2].Y = y;

                left = x;
                top = arrTriangle[1].Y;
                right = arrTriangle[2].X;
                bottom = arrTriangle[0].Y;
            }
            else if (pos == BoxPosition.BOTTOM_MIDDLE)
            {
                y += 3;

                arrTriangle[0].X = x + m_fTriangleSize / 2;
                arrTriangle[0].Y = y;
                arrTriangle[1].X = arrTriangle[0].X - m_fTriangleSize;
                arrTriangle[1].Y = y;
                arrTriangle[2].X = x;
                arrTriangle[2].Y = y + m_fTriangleSize;

                left = arrTriangle[1].X;
                top = y;
                right = arrTriangle[0].X;
                bottom = arrTriangle[2].Y;
            }
            else
                return false;

            return true;
        }

        protected void DrawTriangle(Graphics g, float x, float y, BoxPosition pos, PointF ptMouseCursor)
        {
            float left = 0, top = 0, right = 0, bottom = 0;

            if (!GetArrowTriangle(x, y, pos, ref m_arrTriangle, ref left, ref top, ref right, ref bottom))
                return;

            bool mouseOverArrow = false;

            if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                mouseOverArrow = true;

            if (mouseOverArrow)
                g.FillPolygon(DARK_TRI_BRUSH, m_arrTriangle);
            else
                g.FillPolygon(LIGHT_TRI_BRUSH, m_arrTriangle);
        }

        protected void SetDrawPoint()
        {
            float smallHalf = m_fRectSize / 2;
            float bigHalfW = m_fWidth / 2.0f;
            float bigHalfH = m_fHeight / 2.0f;

            m_xL = x - smallHalf;
            m_xM = x + bigHalfW - smallHalf;
            m_xR = x + m_fWidth - smallHalf;

            m_yT = y - smallHalf;
            m_yM = y + bigHalfH - smallHalf;
            m_yB = y + m_fHeight - smallHalf;
        }

        public float GetCoord(CoordType type)
        {
            if (type == CoordType.X_LEFT)
                return m_xL;
            else if (type == CoordType.X_MIDDLE)
                return m_xM;
            else if (type == CoordType.X_RIGHT)
                return m_xR;
            else if (type == CoordType.Y_TOP)
                return m_yT;
            else if (type == CoordType.Y_MIDDLE)
                return m_yM;
            //else if (type == CoordType.Y_BOTTOM)
            return m_yB;
        }

        public BoxPosition CheckMouse(float x, float y)
        {
            if (IsInside(x, y, m_xL, m_yT))
                return BoxPosition.TOP_LEFT;
            else if (IsInside(x, y, m_xM, m_yT))
                return BoxPosition.TOP_MIDDLE;
            else if (IsInside(x, y, m_xR, m_yT))
                return BoxPosition.TOP_RIGHT;
            else if (IsInside(x, y, m_xL, m_yM))
                return BoxPosition.MIDDLE_LEFT;
            else if (IsInside(x, y, m_xR, m_yM))
                return BoxPosition.MIDDLE_RIGHT;
            else if (IsInside(x, y, m_xL, m_yB))
                return BoxPosition.BOTTOM_LEFT;
            else if (IsInside(x, y, m_xM, m_yB))
                return BoxPosition.BOTTOM_MIDDLE;
            else if (IsInside(x, y, m_xR, m_yB))
                return BoxPosition.BOTTOM_RIGHT;

            return BoxPosition.NO_SELECT;
        }

        protected bool IsInside(float x, float y, float tlX, float tlY)
        {
            if (x < tlX || x > tlX + m_fRectSize) return false;
            if (y < tlY || y > tlY + m_fRectSize) return false;
            return true;
        }

        protected void DrawRect(Graphics g, float x, float y)
        {
            g.FillRectangle(BOX_BRUSH, x, y, m_fRectSize, m_fRectSize);
            g.DrawRectangle(LINE_PEN, x, y, m_fRectSize, m_fRectSize);
        }

        protected void DrawCircle(Graphics g, float x, float y)
        {
            g.FillEllipse(BOX_BRUSH, x, y, m_fRectSize, m_fRectSize);
            g.DrawArc(LINE_PEN, x, y, m_fRectSize, m_fRectSize, 0.0f, 360.0f);
        }

        public bool NeedMouseOverRefresh(PointF ptPrevMouseCursor, PointF ptCurrentMouseCursor)
        {
            BoxPosition pos1 = GetArrowPosition(ptPrevMouseCursor);
            BoxPosition pos2 = GetArrowPosition(ptCurrentMouseCursor);
            return pos1 != pos2;
        }

        public bool InArrowArea(PointF ptMouseCursor)
        {
            float left = x - 3 - m_fTriangleSize, right = x + m_fWidth + 3 + m_fTriangleSize;
            float top = y - 3 - m_fTriangleSize, bottom = y + m_fHeight + 3 + m_fTriangleSize;

            if (ptMouseCursor.X >= left && ptMouseCursor.X <= right && ptMouseCursor.Y >= top && ptMouseCursor.Y <= bottom)
                return true;

            return false;
        }

        public PointF Position
        {
            get
            {
                return new PointF(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;

                SetDrawPoint();
            }
        }

        public SizeF RectSize
        {
            get
            {
                return new SizeF(m_fWidth, m_fHeight);
            }
            set
            {

                if (value.Width < (float)m_MinSize.Width)
                    m_fWidth = (float)m_MinSize.Width;
                else
                    m_fWidth = value.Width;

                if (value.Height < (float)m_MinSize.Height)
                    m_fHeight = (float)m_MinSize.Height;
                else
                    m_fHeight = value.Height;

                SetDrawPoint();
            }
        }

        public static void SetColor(bool isLine, Color clr)
        {
            if (isLine) LINE_PEN.Color = clr;
            else FillColor = clr;
        }

        public static Color GetColor(bool isLine)
        {
            return isLine ? LINE_PEN.Color : m_clrFill;
        }
    }
}
