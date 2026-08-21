using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SOPMonitoringSystem
{
    public class EditBox
    {
        private static Pen LINE_PEN = new Pen(Color.Black, 1);
        private static SolidBrush BOX_BRUSH = new SolidBrush(Color.Aqua);
        private static float m_fRectSize = 6;
        private static Color m_clrFill = Color.Aqua;

        protected int x = 0, y = 0;
        protected int m_nWidth = 0, m_nHeight = 0;

        protected float m_xL, m_xM, m_xR, m_yT, m_yM, m_yB;

        public enum BoxPosition { NO_SELECT, TOP_LEFT, TOP_MIDDLE, TOP_RIGHT, MIDDLE_LEFT, MIDDLE_RIGHT, BOTTOM_LEFT, BOTTOM_MIDDLE, BOTTOM_RIGHT };
        public enum CoordType {X_LEFT, X_MIDDLE, X_RIGHT, Y_TOP, Y_MIDDLE, Y_BOTTOM};

        public EditBox()
        {
        }

        public EditBox(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            m_nWidth  = width;
            m_nHeight = height;
        }

        public float GetSmallRectSize()
        {
            return m_fRectSize;
        }

        public void Draw(Graphics g)
        {
            //g.DrawRectangle(LINE_PEN, x, y, m_nWidth, m_nHeight);
            DrawRect(g, m_xL, m_yT);
            DrawCircle(g, m_xM, m_yT);
            DrawRect(g, m_xR, m_yT);
            DrawCircle(g, m_xR, m_yM);
            DrawRect(g, m_xR, m_yB);
            DrawCircle(g, m_xM, m_yB);
            DrawRect(g, m_xL, m_yB);
            DrawCircle(g, m_xL, m_yM);
        }

        protected void SetDrawPoint()
        {
            float smallHalf = m_fRectSize / 2;
            float bigHalfW = m_nWidth / 2.0f;
            float bigHalfH = m_nHeight / 2.0f;

            m_xL = x - smallHalf;
            m_xM = x + bigHalfW - smallHalf;
            m_xR = x + m_nWidth - smallHalf;

            m_yT = y - smallHalf;
            m_yM = y + bigHalfH - smallHalf;
            m_yB = y + m_nHeight - smallHalf;
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

        public Point Position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;

                SetDrawPoint();
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
                m_nWidth  = value.Width;
                m_nHeight = value.Height;

                SetDrawPoint();
            }
        }

        public static void SetColor(bool isLine, Color clr)
        {
            if (isLine) LINE_PEN.Color = clr;
            else m_clrFill = clr;
        }

        public static Color GetColor(bool isLine)
        {
            return isLine ? LINE_PEN.Color : m_clrFill;
        }
    }
}
