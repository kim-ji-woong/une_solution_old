using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Sections
{
    public class Arrow
    {
        protected Section m_sectionBegin = null;
        protected Section m_sectionEnd = null;
        protected ArrowPosition m_posBegin = ArrowPosition.NONE;
        protected ArrowPosition m_posEnd = ArrowPosition.NONE;

        protected PointF[] m_arrPoint = null;
        protected PointF[] m_arrTriangle = new PointF[3];

        protected string m_strText = "";

        protected bool m_isSelected = false;
        protected RectangleF m_rectText = new RectangleF();

        protected Color m_tempLine = Color.FromArgb(76, 75, 76);//Color.Black;
        protected Color m_fillColor = Color.FromArgb(76, 75, 76);//Color.Black;
        protected Color m_lineColor = Color.FromArgb(76, 75, 76);//Color.Black;
        protected int m_nLineThick = 5;
        protected System.Drawing.Drawing2D.DashStyle m_lineStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        public enum ArrowPosition { TOP, RIGHT, BOTTOM, LEFT, NONE };

        private static Pen TEMP_LINE_PEN = MakePen(Color.Black, System.Drawing.Drawing2D.DashStyle.Dash);
        public static Pen TempLinePen
        {
            get { return Arrow.TEMP_LINE_PEN; }
            set 
            {
                Arrow.TEMP_LINE_PEN = value; 
            }
        }

        private static Pen NORMAL_PEN = MakePen(Color.Black, System.Drawing.Drawing2D.DashStyle.Solid);
        public static Pen NormalPen
        {
            get { return Arrow.NORMAL_PEN; }
            set { Arrow.NORMAL_PEN = value; }
        }
        private static SolidBrush TRIANGLE_BRUSH = new SolidBrush(Color.Black);
        public static SolidBrush TriangleBrush
        {
            get { return Arrow.TRIANGLE_BRUSH; }
            set { Arrow.TRIANGLE_BRUSH = value; }
        }
        private static Pen BLUE_RECT_PEN = MakePen(Color.Aqua, System.Drawing.Drawing2D.DashStyle.Solid);
        public static Pen BlueRectPen
        {
            get { return Arrow.BLUE_RECT_PEN; }
            set { Arrow.BLUE_RECT_PEN = value; }
        }
        private static Pen RED_RECT_PEN = MakePen(Color.Red, System.Drawing.Drawing2D.DashStyle.Solid);
        public static Pen RedRectPen
        {
            get { return Arrow.RED_RECT_PEN; }
            set { Arrow.RED_RECT_PEN = value; }
        }
        private static SolidBrush RED_BRUSH = new SolidBrush(Color.Red);
        public static SolidBrush RedBrush
        {
            get { return Arrow.RED_BRUSH; }
            set { Arrow.RED_BRUSH = value; }
        }

        protected static SolidBrush TEXT_BRUSH = new SolidBrush(Color.Black);
        public static SolidBrush TextBrush
        {
            get { return Arrow.TEXT_BRUSH; }
            set { Arrow.TEXT_BRUSH = value; }
        }

        protected static Font TEXT_FONT = new Font("나눔바른고딕", 12);
        public static Font TextFont
        {
            get { return Arrow.TEXT_FONT; }
            set { Arrow.TEXT_FONT = value; }
        }

        protected static StringFormat m_textFormat = GetStringFormat();

        // 화살표 꺽임선의 최소 길이
        protected static float MIN_DISTANCE = 30;
        // 화살표 삼각형의 가로와 세로 높이
        protected static float TRIANGLE_SIZE = 20;
        // 선택 표시를 위한 사각형 너비와 높이
        protected static float SMALL_RECT_SIZE = 8;
            
        // Mouse Click을 통한 선택을 위한 최소 거리(화면 좌표 기준)
        private static int _SELECT_DISTANCE = 5;

		private bool m_bVisible = true;
		public bool Visible
		{
			get { return m_bVisible; }
			set { m_bVisible = value; }
		}
        protected static Pen MakePen(Color color, System.Drawing.Drawing2D.DashStyle style)
        {
            Pen pen = new Pen(color, 1);
            pen.DashStyle = style;
            return pen;
        }

        public static void CopyPoints(Arrow arrowTrg, Arrow arrowSrc)
        {
            int nPointCount = arrowSrc.m_arrPoint.Count();
            arrowTrg.m_arrPoint = new PointF[nPointCount];

            for (int i = 0; i < nPointCount; i++)
            {
                arrowTrg.m_arrPoint[i].X = arrowSrc.m_arrPoint[i].X;
                arrowTrg.m_arrPoint[i].Y = arrowSrc.m_arrPoint[i].Y;
            }

            for (int i = 0; i < 3; i++)
            {
                arrowTrg.m_arrTriangle[i].X = arrowSrc.m_arrTriangle[i].X;
                arrowTrg.m_arrTriangle[i].Y = arrowSrc.m_arrTriangle[i].Y;
            }
        }

		public static void CopyPoints(Arrow arrowTrg, Arrow arrowSrc, float dx, float dy)
		{
			int nPointCount = arrowSrc.m_arrPoint.Count();
			arrowTrg.m_arrPoint = new PointF[nPointCount];

			for (int i = 0; i < nPointCount; i++)
			{
				arrowTrg.m_arrPoint[i].X = arrowSrc.m_arrPoint[i].X + dx;
				arrowTrg.m_arrPoint[i].Y = arrowSrc.m_arrPoint[i].Y + dy;
			}

			for (int i = 0; i < 3; i++)
			{
				arrowTrg.m_arrTriangle[i].X = arrowSrc.m_arrTriangle[i].X + dx;
				arrowTrg.m_arrTriangle[i].Y = arrowSrc.m_arrTriangle[i].Y + dy;
			}
		}

        public static bool IntToArrowPosition(int nPos, out ArrowPosition result)
        {
            switch (nPos)
            {
                case 0:
                    result = ArrowPosition.TOP;
                    return true;

                case 1:
                    result = ArrowPosition.RIGHT;
                    return true;

                case 2:
                    result = ArrowPosition.BOTTOM;
                    return true;

                case 3:
                    result = ArrowPosition.LEFT;
                    return true;

                case 4:
                    result = ArrowPosition.NONE;
                    return true;

                default:
                    result = ArrowPosition.NONE;
                    break;
            }

            return false;
        }

        public Arrow()
        {
             m_tempLine = TEMP_LINE_PEN.Color;
             m_fillColor = TRIANGLE_BRUSH.Color;
             m_lineColor = NORMAL_PEN.Color;
        }

        public virtual void Draw(Graphics g)
        {
            if (m_arrPoint == null)
                return;

			if (m_bVisible == false)
				return;

            int nPointCount = m_arrPoint.Count();
            if (nPointCount < 2) return;

            if (m_sectionBegin == null || m_sectionEnd == null)
                return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;  

            if (m_sectionBegin.GetComponentType() == Section.ComponentType.ANNOTATION ||
                m_sectionEnd.GetComponentType() == Section.ComponentType.ANNOTATION)
            {
                TEMP_LINE_PEN.Color = m_lineColor;
                TEMP_LINE_PEN.Width = m_nLineThick;

                // 설명 Section은 화살표가 아닌 점선으로만 표시
                g.DrawLines(TEMP_LINE_PEN, m_arrPoint);
            }
            else
            {
                NORMAL_PEN.Color = m_lineColor;
                NORMAL_PEN.Width = m_nLineThick;
                NORMAL_PEN.DashStyle = m_lineStyle;
                TRIANGLE_BRUSH.Color = m_fillColor;

                g.DrawLines(NORMAL_PEN, m_arrPoint);
                g.FillPolygon(TRIANGLE_BRUSH, m_arrTriangle);
            }

            if (m_isSelected)
                DrawSelection(g);

            if (m_strText.Length > 0)
                DrawText(g);
        }

        public bool FindArrowMiddlePoint(out PointF pt)
        {
            pt = new PointF();

            ArrayList arrLength = new ArrayList();
            int nPointCount = m_arrPoint.Count();
            double dLen = 0.0;

            for (int i = 1; i < nPointCount; i++)
            {
                PointF pt1 = m_arrPoint[i - 1];
                PointF pt2 = m_arrPoint[i];

                double distance = Geometry.GetDistance(pt1, pt2);
                arrLength.Add(distance);
                dLen += distance;
            }

            double dTargetLength = dLen / 2;
            dLen = 0.0;

            for (int i = 1; i < nPointCount; i++)
            {
                double distance = (double)arrLength[i - 1];
                dLen += distance;

                if (dLen >= dTargetLength)
                {
                    double len = dLen - dTargetLength;

                    PointF pt1 = m_arrPoint[i - 1];
                    PointF pt2 = m_arrPoint[i];
                    pt = Geometry.GetLinearPoint(pt2, pt1, len);

                    return true;
                }
            }

            return false;
        }

        protected virtual void DrawText(Graphics g)
        {
            if (m_sectionBegin == null)
                return;
			if (m_bVisible == false)
				return;

            PointF pt;
            if (!FindArrowMiddlePoint(out pt))
                return;

            PanelSection panel = (PanelSection)m_sectionBegin.GetParent();
            SolidBrush rectBrush = new SolidBrush(panel.BackColor);

            float fWidth = g.MeasureString(m_strText, TEXT_FONT).Width;
            float fHeight = TEXT_FONT.Height;

            m_rectText.X = pt.X - fWidth / 2;
            m_rectText.Y = pt.Y - fHeight / 2;
            m_rectText.Width = fWidth;
            m_rectText.Height = fHeight;
            //RectangleF rect = new RectangleF(pt.X - fWidth / 2, pt.Y - fHeight / 2, fWidth, fHeight);
            g.FillRectangle(rectBrush, m_rectText);
            g.DrawString(m_strText, TEXT_FONT, TEXT_BRUSH, m_rectText, m_textFormat);
        }

        protected void DrawSelection(Graphics g)
        {
			if (m_bVisible == false)
				return;

            int nPointCount = m_arrPoint.Count();
            if (nPointCount < 2) return;

            //g.DrawRectangle(RED_RECT_PEN, m_arrPoint[0].X - SMALL_RECT_SIZE / 2, m_arrPoint[0].Y - SMALL_RECT_SIZE / 2, SMALL_RECT_SIZE, SMALL_RECT_SIZE);
            //g.DrawRectangle(RED_RECT_PEN, m_arrPoint[nPointCount - 1].X - SMALL_RECT_SIZE / 2, m_arrPoint[nPointCount - 1].Y - SMALL_RECT_SIZE / 2, SMALL_RECT_SIZE, SMALL_RECT_SIZE);
            g.FillRectangle(RED_BRUSH, m_arrPoint[0].X - SMALL_RECT_SIZE / 2, m_arrPoint[0].Y - SMALL_RECT_SIZE / 2, SMALL_RECT_SIZE, SMALL_RECT_SIZE);
            g.FillRectangle(RED_BRUSH, m_arrPoint[nPointCount - 1].X - SMALL_RECT_SIZE / 2, m_arrPoint[nPointCount - 1].Y - SMALL_RECT_SIZE / 2, SMALL_RECT_SIZE, SMALL_RECT_SIZE);

            for (int i = 1; i < nPointCount - 1; i++)
            {
                g.DrawRectangle(BLUE_RECT_PEN, m_arrPoint[i].X - SMALL_RECT_SIZE / 2, m_arrPoint[i].Y - SMALL_RECT_SIZE / 2, SMALL_RECT_SIZE, SMALL_RECT_SIZE);
            }
        }

        // 화살표 이동 좌표인가?
        // Return 값 : 1(화살표 끝점), -1(화살표 시작점), 0(그 외의 좌표), 2(중간이동점)
        public int IsMovingPoint(float x, float y)
        {
#if KNOT
            nSelectKnotPoint = 0;
#endif

            int nPointCount = m_arrPoint.Count();
            if (nPointCount < 2)
                return 0;

            if (x >= m_arrPoint[0].X - SMALL_RECT_SIZE / 2 && x <= m_arrPoint[0].X + SMALL_RECT_SIZE / 2 &&
                y >= m_arrPoint[0].Y - SMALL_RECT_SIZE / 2 && y <= m_arrPoint[0].Y + SMALL_RECT_SIZE / 2)
                return -1;

            if (x >= m_arrPoint[nPointCount - 1].X - SMALL_RECT_SIZE / 2 && x <= m_arrPoint[nPointCount - 1].X + SMALL_RECT_SIZE / 2 &&
                y >= m_arrPoint[nPointCount - 1].Y - SMALL_RECT_SIZE / 2 && y <= m_arrPoint[nPointCount - 1].Y + SMALL_RECT_SIZE / 2)
                return 1;

#if KNOT
            if( nPointCount > 3)
            {
                for (int i = 1; i < nPointCount - 1; i++)
                {
                    nSelectKnotPoint = i;
                    if (x >= m_arrPoint[i].X - SMALL_RECT_SIZE / 2 && x <= m_arrPoint[i].X + SMALL_RECT_SIZE / 2 &&
                   y >= m_arrPoint[i].Y - SMALL_RECT_SIZE / 2 && y <= m_arrPoint[i].Y + SMALL_RECT_SIZE / 2)
                        return 2;
                }
            }
#endif

            return 0;
        }
#if KNOT
        private int nSelectKnotPoint = 0;
        public int SelectKnotPoint
        {
            get { return nSelectKnotPoint; }
            set { nSelectKnotPoint = value; }
        }

        public int KnotCount
        {
            get { return m_arrPoint.Length; }
        }

        private bool SameFloat(float x, float b, float eps)
        {
            if (Math.Abs(x - b) < eps)
                return true;
            return false;
        }
        public void MoveKnotPoint(int nIdx, float x, float y)
        {
            if (nIdx < 0)// || (m_arrPoint.Length - 1) <= nIdx)
                return;

            bool moveX = false;
            bool moveY = false;
            if( nIdx - 1 == 0)
            {
               if(!SameFloat(m_arrPoint[0].X, m_arrPoint[nIdx].X, 0.01f))
               {
                   m_arrPoint[nIdx].X = x;
                   m_arrPoint[nIdx + 1].X = x;
               }

               if (!SameFloat(m_arrPoint[0].Y, m_arrPoint[nIdx].Y, 0.01f))
               {
                   m_arrPoint[nIdx].Y = y;
                   m_arrPoint[nIdx + 1].Y = y;
               }
            }

            if (nIdx == m_arrPoint.Length - 2)
            {
                if (!SameFloat(m_arrPoint[m_arrPoint.Length-1].X, m_arrPoint[nIdx].X, 0.01f))
                {
                    m_arrPoint[nIdx].X = x;
                    m_arrPoint[nIdx - 1].X = x;
                }

                if (!SameFloat(m_arrPoint[m_arrPoint.Length - 1].Y, m_arrPoint[nIdx].Y, 0.01f))
                {
                    m_arrPoint[nIdx].Y = y;
                    m_arrPoint[nIdx - 1].Y = y;
                }
            }

        }
        public PointF GetKnotPoint(int nIdx)
        {
            if (nIdx < 0 || m_arrPoint.Length <= nIdx)
                return new PointF();

            return m_arrPoint[nIdx];
        }
#endif

        private void SetTriangleArray(int nPointCount)
        {
            PointF pt1 = m_arrPoint[nPointCount - 2];
            PointF pt2 = m_arrPoint[nPointCount - 1];

            if (System.Math.Abs(pt1.X - pt2.X) < System.Math.Abs(pt1.Y - pt2.Y))    // 세로방향 화살표
            {
                m_arrTriangle[0].X = pt1.X - TRIANGLE_SIZE / 2;

                if (pt2.Y > pt1.Y)  // 위에서 아래로 향하는 화살표
                {
                    m_arrTriangle[0].Y = pt2.Y - TRIANGLE_SIZE;
                    m_arrTriangle[1].X = pt1.X;
                    m_arrTriangle[1].Y = pt2.Y;
                    m_arrTriangle[2].X = pt1.X + TRIANGLE_SIZE / 2;
                    m_arrTriangle[2].Y = m_arrTriangle[0].Y;

                    m_arrPoint[nPointCount - 1] = new PointF((m_arrTriangle[0].X + m_arrTriangle[2].X) / 2, (m_arrTriangle[0].Y + m_arrTriangle[2].Y) / 2);
                }
                else                // 아래에서 위로 향하는 화살표
                {
                    m_arrTriangle[0].Y = pt2.Y + TRIANGLE_SIZE;
                    m_arrTriangle[1].X = pt1.X + TRIANGLE_SIZE / 2;
                    m_arrTriangle[1].Y = m_arrTriangle[0].Y;
                    m_arrTriangle[2].X = pt1.X;
                    m_arrTriangle[2].Y = pt2.Y;

                    m_arrPoint[nPointCount - 1] = new PointF((m_arrTriangle[0].X + m_arrTriangle[1].X) / 2, (m_arrTriangle[0].Y + m_arrTriangle[1].Y) / 2);
                }
            }
            else    // 가로방향 화살표
            {
                m_arrTriangle[0].Y = pt1.Y - TRIANGLE_SIZE / 2;

                if (pt1.X < pt2.X)  // 왼쪽에서 오른쪽으로 향하는 화살표
                {
                    m_arrTriangle[0].X = pt2.X - TRIANGLE_SIZE;
                    m_arrTriangle[1].X = m_arrTriangle[0].X;
                    m_arrTriangle[1].Y = pt1.Y + TRIANGLE_SIZE / 2;
                    m_arrTriangle[2].X = pt2.X;
                    m_arrTriangle[2].Y = pt1.Y;

                    m_arrPoint[nPointCount - 1] = new PointF((m_arrTriangle[0].X + m_arrTriangle[1].X) / 2, (m_arrTriangle[0].Y + m_arrTriangle[1].Y) / 2);
                }
                else                // 오른쪽에서 왼쪽으로 향하는 화살표
                {
                    m_arrTriangle[0].X = pt2.X + TRIANGLE_SIZE;
                    m_arrTriangle[1].X = pt2.X;
                    m_arrTriangle[1].Y = pt1.Y;
                    m_arrTriangle[2].X = m_arrTriangle[0].X;
                    m_arrTriangle[2].Y = pt1.Y + TRIANGLE_SIZE / 2;

                    m_arrPoint[nPointCount - 1] = new PointF((m_arrTriangle[0].X + m_arrTriangle[2].X) / 2, (m_arrTriangle[0].Y + m_arrTriangle[2].Y) / 2);
                }
            }
        }

        // 시작 Section만 연결되어 있는 상황에서 끝 Section을 찾기 위하여 임시 화살표를 그린다.
        public virtual void DrawTemp(Graphics g, PointF ptEnd)
        {
            if (m_posBegin == ArrowPosition.NONE || m_sectionBegin == null)
                return;

            bool result;
            PointF ptBegin = m_sectionBegin.GetArrowPoint(m_posBegin, out result);
            if (result == false)
                return;


            TEMP_LINE_PEN.Color = m_tempLine;
            TEMP_LINE_PEN.Width = m_nLineThick;

            g.DrawLine(TEMP_LINE_PEN, ptBegin, ptEnd);
        }

        public void CalcArrowLine()
        {
            if (m_sectionBegin == null || m_posBegin == ArrowPosition.NONE)
                return;
            if (m_sectionEnd == null || m_posEnd == ArrowPosition.NONE)
                return;

            bool result;
            PointF ptBegin = m_sectionBegin.GetArrowPoint(m_posBegin, out result);
            if (result == false) return;

            PointF ptEnd = m_sectionEnd.GetArrowPoint(m_posEnd, out result);
            if (result == false) return;

            if (System.Math.Abs(ptBegin.X - ptEnd.X) < 0.0001 || System.Math.Abs(ptBegin.Y - ptEnd.Y) < 0.0001)
            {
                if (CalcStraightArrowLine(ptBegin, ptEnd))
                {
                    SetTriangleArray(m_arrPoint.Count());
                    return;
                }
            }
                
            if (ptBegin.X < ptEnd.X)
            {
                if (ptBegin.Y < ptEnd.Y)
                    CalcNWArrowLine(ptBegin, ptEnd, m_sectionBegin, m_sectionEnd, m_posBegin, m_posEnd);  // 좌측 상단에서 우측 하단 방향으로 화살표
                else
                    CalcSWArrowLine(ptBegin, ptEnd, m_sectionBegin, m_sectionEnd, m_posBegin, m_posEnd);  // 좌측 하단에서 우측 상단 방향으로 화살표
            }
            else
            {
                if (ptBegin.Y < ptEnd.Y)
                    CalcNEArrowLine(ptBegin, ptEnd, m_sectionBegin, m_sectionEnd, m_posBegin, m_posEnd);  // 우측 상단에서 좌측 하단 방향으로 화살표
                else
                    CalcSEArrowLine(ptBegin, ptEnd, m_sectionBegin, m_sectionEnd, m_posBegin, m_posEnd);  // 우측 하단에서 좌측 상단 방향으로 화살표
            }

            SetTriangleArray(m_arrPoint.Count());
        }

        private void ResetPoint(int nArrayCount)
        {
            if (m_arrPoint != null)
            {
                if (m_arrPoint.Count() != nArrayCount)
                    m_arrPoint = new PointF[nArrayCount];
            }
            else
            {
                m_arrPoint = new PointF[nArrayCount];
            }
        }

        private bool CalcStraightArrowLine(PointF ptBegin, PointF ptEnd)
        {
            bool processed = false;

            if (System.Math.Abs(ptBegin.X - ptEnd.X) < 0.0001)   // 수직 방향 화살표
            {
                if (m_posBegin == ArrowPosition.TOP && m_posEnd == ArrowPosition.BOTTOM)
                    processed = true;
                else if (m_posBegin == ArrowPosition.BOTTOM && m_posEnd == ArrowPosition.TOP)
                    processed = true;
            }
            else                        // 수평 방향 화살표
            {
                if (m_posBegin == ArrowPosition.RIGHT && m_posEnd == ArrowPosition.LEFT)
                    processed = true;
                else if (m_posBegin == ArrowPosition.LEFT && m_posEnd == ArrowPosition.RIGHT)
                    processed = true;
            }

            if (!processed)
                return false;

            ResetPoint(2);

            m_arrPoint[0] = ptBegin;
            m_arrPoint[1] = ptEnd;
            return true;
        }

        // 좌측 상단에서 우측 하단 방향으로 화살표
        private void CalcNWArrowLine(PointF ptBegin, PointF ptEnd, Section sectionBegin, Section sectionEnd, ArrowPosition posBegin, ArrowPosition posEnd)
        {
            SizeF sizeBeginSection = sectionBegin.RectSize;

            if (posBegin == ArrowPosition.TOP)
            {
                float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;

                if (posEnd == ArrowPosition.TOP)
                {
                    if (ptEnd.X >= fBeginRight + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X, m_arrPoint[1].Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndRight = ptEnd.X + sizeEndSection.Width / 2;

                    float x = fEndRight >= fBeginRight ? fEndRight + MIN_DISTANCE : fBeginRight + MIN_DISTANCE;

                    ResetPoint(5);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                    m_arrPoint[2] = new PointF(x, m_arrPoint[1].Y);
                    m_arrPoint[3] = new PointF(x, ptEnd.Y);
                    m_arrPoint[4] = ptEnd;
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;

                    float x = fEndLeft >= fBeginRight + MIN_DISTANCE ? fBeginRight + MIN_DISTANCE : ptEnd.X + sizeBeginSection.Width / 2 + MIN_DISTANCE;

                    ResetPoint(6);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                    m_arrPoint[2] = new PointF(x, m_arrPoint[1].Y);
                    m_arrPoint[3] = new PointF(x, ptEnd.Y + MIN_DISTANCE);
                    m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                    m_arrPoint[5] = ptEnd;
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                    float fBeginBottom = ptBegin.Y + sizeBeginSection.Height;

                    bool isLeft = fEndLeft <= fBeginRight && ptEnd.Y < fBeginBottom + MIN_DISTANCE * 2;

                    if (isLeft)
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptBegin.X - sizeBeginSection.Width / 2 - MIN_DISTANCE, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        if (fEndLeft < fBeginRight + MIN_DISTANCE * 2)
                        {
                            ResetPoint(7);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                            m_arrPoint[2] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[1].Y);
                            m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y - MIN_DISTANCE);
                            m_arrPoint[4] = new PointF(ptEnd.X - MIN_DISTANCE, m_arrPoint[3].Y);
                            m_arrPoint[5] = new PointF(m_arrPoint[4].X, ptEnd.Y);
                            m_arrPoint[6] = ptEnd;
                        }
                        else
                        {
                            ResetPoint(5);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y - MIN_DISTANCE);
                            m_arrPoint[2] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[1].Y);
                            m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                            m_arrPoint[4] = ptEnd;
                        }
                    }
                }
            }
            else if (posBegin == ArrowPosition.RIGHT)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    if (ptEnd.Y < ptBegin.Y + MIN_DISTANCE)
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;

                        float fEndLeft = ptEnd.X - sectionEnd.RectSize.Width / 2;
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (ptBegin.X + fEndLeft) / 2;

                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else if (ptEnd.X < ptBegin.X + MIN_DISTANCE)
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X + MIN_DISTANCE, ptBegin.Y);

                        float fBeginBottom = ptBegin.Y + sizeBeginSection.Height;
                        float y = ptEnd.Y >= fBeginBottom + MIN_DISTANCE * 2 ? ptEnd.Y - MIN_DISTANCE : (fBeginBottom + ptEnd.Y) / 2;

                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X, y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(3);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X, ptBegin.Y);
                        m_arrPoint[2] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndTop = ptEnd.Y - MIN_DISTANCE / 2;

                    if (fEndTop < ptBegin.Y + MIN_DISTANCE)
                    {
                        float fEndLeft = ptEnd.X - sizeEndSection.Width;
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (fEndLeft + ptBegin.X) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, fEndTop + MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width;

                    if (fEndLeft >= ptBegin.X + MIN_DISTANCE)
                    {
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (fEndLeft + ptBegin.X) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y + MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        float fEndTop = ptEnd.Y - sizeEndSection.Height;
                        float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;

                        float y = fEndTop >= fBeginBottom + MIN_DISTANCE * 2 ? fEndTop - MIN_DISTANCE : (fEndTop + fBeginBottom) / 2;

                        ResetPoint(7);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(fEndLeft - MIN_DISTANCE, y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y + MIN_DISTANCE);
                        m_arrPoint[5] = new PointF(ptEnd.X, m_arrPoint[4].Y);
                        m_arrPoint[6] = ptEnd;
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width;

                    if (fEndLeft >= ptBegin.X + MIN_DISTANCE)
                    {
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (fEndLeft + ptBegin.X) / 2;

                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float fEndTop = ptEnd.Y - sizeEndSection.Height / 2;
                        float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;

                        float y = fEndTop >= fBeginBottom + MIN_DISTANCE * 2 ? fEndTop - MIN_DISTANCE : (fBeginBottom + fEndTop) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X - MIN_DISTANCE, y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
            }
            else if (posBegin == ArrowPosition.BOTTOM)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float y = ptEnd.Y >= ptBegin.Y + MIN_DISTANCE * 2 ? ptEnd.Y - MIN_DISTANCE : (ptEnd.Y + ptBegin.Y) / 2;

                    ResetPoint(4);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, y);
                    m_arrPoint[2] = new PointF(ptEnd.X, y);
                    m_arrPoint[3] = ptEnd;
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndTop = ptEnd.Y - sizeEndSection.Height / 2;

                    if (fEndTop >= ptBegin.Y + MIN_DISTANCE)
                    {
                        float y = fEndTop >= ptBegin.Y + MIN_DISTANCE * 2 ? fEndTop - MIN_DISTANCE : (fEndTop + ptBegin.Y) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, y);
                        m_arrPoint[2] = new PointF(ptEnd.X + MIN_DISTANCE, y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y + sizeEndSection.Height / 2 + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                        m_arrPoint[4] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;

                    if (fEndLeft >= ptBegin.X + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X, m_arrPoint[1].Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float fEndTop = ptEnd.Y - sizeEndSection.Height;
                        float y = fEndTop >= ptBegin.Y + MIN_DISTANCE * 2 ? fEndTop - MIN_DISTANCE : (fEndTop + ptBegin.Y) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, y);
                        m_arrPoint[2] = new PointF(fEndLeft - MIN_DISTANCE, y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                        m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    ResetPoint(3);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y);
                    m_arrPoint[2] = ptEnd;
                }
            }
            else// if (posBegin == ArrowPosition.LEFT)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width;
                    float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;

                    if (ptEnd.X >= fBeginRight + MIN_DISTANCE && ptEnd.Y < fBeginBottom + MIN_DISTANCE)
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptBegin.Y - sizeBeginSection.Height / 2 - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(5);

                        float y = ptEnd.Y >= fBeginBottom + MIN_DISTANCE * 2 ? ptEnd.Y - MIN_DISTANCE : (ptEnd.Y + fBeginBottom) / 2;

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X, y);
                        m_arrPoint[4] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    SizeF sizeEndSection = sectionEnd.RectSize;
                    float fEndTop = ptEnd.Y - sizeEndSection.Height / 2;
                    float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;

                    if (fEndTop >= fBeginBottom + MIN_DISTANCE)
                    {
                        float y = fEndTop >= fBeginBottom + MIN_DISTANCE * 2 ? fEndTop - MIN_DISTANCE : (fEndTop + fBeginBottom) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        float fBeginRight = ptBegin.X + sizeBeginSection.Width;

                        if (ptEnd.X >= fBeginRight + MIN_DISTANCE)
                        {
                            ResetPoint(6);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                            m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptBegin.Y + sizeBeginSection.Height / 2 + MIN_DISTANCE);
                            m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                            m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                            m_arrPoint[5] = ptEnd;
                        }
                        else
                        {
                            ResetPoint(6);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                            m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y + sizeEndSection.Height / 2 + MIN_DISTANCE);
                            m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                            m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                            m_arrPoint[5] = ptEnd;
                        }
                    }
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;
                    float y = ptEnd.Y > fBeginBottom ? ptEnd.Y + MIN_DISTANCE : fBeginBottom + MIN_DISTANCE;

                    ResetPoint(5);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                    m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                    m_arrPoint[3] = new PointF(ptEnd.X, y);
                    m_arrPoint[4] = ptEnd;
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    float fBeginBottom = ptBegin.Y + sizeBeginSection.Height / 2;
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width;

                    if (ptEnd.X >= fBeginRight + MIN_DISTANCE)
                    {
                        float x = ptEnd.X >= fBeginRight + MIN_DISTANCE * 2 ? ptEnd.X - MIN_DISTANCE : (fBeginRight + ptEnd.X) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, fBeginBottom + MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(x, m_arrPoint[2].Y);
                        m_arrPoint[4] = new PointF(x, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                }
            }
        }

        // 좌측 하단에서 우측 상단 방향으로 화살표
        private void CalcSWArrowLine(PointF ptBegin, PointF ptEnd, Section sectionBegin, Section sectionEnd, ArrowPosition posBegin, ArrowPosition posEnd)
        {
            SizeF sizeBeginSection = sectionBegin.RectSize;
            SizeF sizeEndSection = sectionEnd.RectSize;

            if (posBegin == ArrowPosition.TOP)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;

                    if (fEndLeft >= ptBegin.X + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X, m_arrPoint[1].Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float fEndBottom = ptEnd.Y + sizeEndSection.Height;
                        float y = ptBegin.Y >= fEndBottom + MIN_DISTANCE * 2 ? ptBegin.Y - MIN_DISTANCE : (ptBegin.Y + fEndBottom) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, y);
                        m_arrPoint[2] = new PointF(fEndLeft - MIN_DISTANCE, y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    ResetPoint(3);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y);
                    m_arrPoint[2] = ptEnd;
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;

                    if (ptBegin.Y < ptEnd.Y + MIN_DISTANCE && fEndLeft >= fBeginRight + MIN_DISTANCE)
                    {
                        float x = fEndLeft >= fBeginRight + MIN_DISTANCE * 2 ? fBeginRight + MIN_DISTANCE : (fEndLeft + fBeginRight) / 2;
                        float y = (ptBegin.Y + ptEnd.Y) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, y);
                        m_arrPoint[2] = new PointF(x, y);
                        m_arrPoint[3] = new PointF(x, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        float y = ptBegin.Y >= ptEnd.Y + MIN_DISTANCE * 2 ? ptEnd.Y + MIN_DISTANCE : (ptBegin.Y + ptEnd.Y) / 2;

                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, y);
                        m_arrPoint[2] = new PointF(ptEnd.X, y);
                        m_arrPoint[3] = ptEnd;
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    ResetPoint(3);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, ptEnd.Y);
                    m_arrPoint[2] = ptEnd;
                }
            }
            else if (posBegin == ArrowPosition.RIGHT)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;

                    if (fEndLeft >= ptBegin.X + MIN_DISTANCE)
                    {
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (ptBegin.X + fEndLeft) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X + sizeEndSection.Width / 2 + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    float fEndBottom = ptEnd.Y - sizeEndSection.Height / 2;

                    if (ptBegin.Y >= fEndBottom + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float fEndLeft = ptEnd.X - sizeEndSection.Width;
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (ptBegin.X + fEndLeft) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, fEndBottom + MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    if (ptEnd.X < ptBegin.X + MIN_DISTANCE)
                    {
                        float fBeginTop = ptBegin.Y - sizeBeginSection.Height / 2;
                        float y = fBeginTop >= ptEnd.Y + MIN_DISTANCE * 2 ? ptEnd.Y + MIN_DISTANCE : (fBeginTop + ptEnd.Y) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X, y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else if (ptBegin.Y < ptEnd.Y + MIN_DISTANCE)
                    {
                        float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                        float x = fEndLeft >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (ptBegin.X + fEndLeft) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, ptEnd.Y + MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(3);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X, ptBegin.Y);
                        m_arrPoint[2] = ptEnd;
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    if (ptEnd.X < ptBegin.X + MIN_DISTANCE)
                    {
                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptEnd.X + sizeEndSection.Width + MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptEnd.Y - sizeEndSection.Height / 2 - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(ptEnd.X - MIN_DISTANCE, m_arrPoint[2].Y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(4);

                        float x = ptEnd.X >= ptBegin.X + MIN_DISTANCE * 2 ? ptBegin.X + MIN_DISTANCE : (ptBegin.X + ptEnd.X) / 2;

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                }
            }
            else if (posBegin == ArrowPosition.BOTTOM)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;

                    if (fEndLeft >= fBeginRight + MIN_DISTANCE)
                    {
                        float x = fEndLeft >= fBeginRight + MIN_DISTANCE * 2 ? fBeginRight + MIN_DISTANCE : (fEndLeft + fBeginRight) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(x, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(x, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[4] = new PointF(ptEnd.X, m_arrPoint[3].Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X + sizeEndSection.Width / 2 + MIN_DISTANCE, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[4] = new PointF(ptEnd.X, ptEnd.Y - MIN_DISTANCE);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;
                    float fEndBottom = ptEnd.Y + sizeEndSection.Height / 2;
                    float y = fEndBottom > ptBegin.Y ? fEndBottom + MIN_DISTANCE : ptBegin.Y + MIN_DISTANCE;
                    float x = ptEnd.X > fBeginRight ? ptEnd.X + MIN_DISTANCE : fBeginRight + MIN_DISTANCE;

                    ResetPoint(5);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(ptBegin.X, y);
                    m_arrPoint[2] = new PointF(x, y);
                    m_arrPoint[3] = new PointF(x, ptEnd.Y);
                    m_arrPoint[4] = ptEnd;
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;
                    float fBeginTop = ptBegin.Y - sizeBeginSection.Height;

                    if (ptEnd.X >= fBeginRight + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(ptEnd.X, m_arrPoint[1].Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float y = fBeginTop >= ptEnd.Y + MIN_DISTANCE * 2 ? ptEnd.Y + MIN_DISTANCE : (ptEnd.Y + fBeginTop) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(m_arrPoint[2].X, y);
                        m_arrPoint[4] = new PointF(ptEnd.X, y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    float fBeginRight = ptBegin.X + sizeBeginSection.Width / 2;
                    float fBeginTop = ptBegin.Y - sizeBeginSection.Height;

                    if (ptEnd.X >= fBeginRight + MIN_DISTANCE)
                    {
                        float x = ptEnd.X >= fBeginRight + MIN_DISTANCE * 2 ? ptEnd.X - MIN_DISTANCE : (ptEnd.X + fBeginRight) / 2;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                        m_arrPoint[2] = new PointF(x, m_arrPoint[1].Y);
                        m_arrPoint[3] = new PointF(x, ptEnd.Y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        float fEndBottom = ptEnd.Y + sizeEndSection.Height / 2;

                        if (fBeginTop >= fEndBottom + MIN_DISTANCE)
                        {
                            float y = fBeginTop >= fEndBottom + MIN_DISTANCE * 2 ? fEndBottom + MIN_DISTANCE : (fBeginTop + fEndBottom) / 2;

                            ResetPoint(7);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                            m_arrPoint[2] = new PointF(fBeginRight + MIN_DISTANCE, m_arrPoint[1].Y);
                            m_arrPoint[3] = new PointF(m_arrPoint[2].X, y);
                            m_arrPoint[4] = new PointF(ptEnd.X - MIN_DISTANCE, y);
                            m_arrPoint[5] = new PointF(m_arrPoint[4].X, ptEnd.Y);
                            m_arrPoint[6] = ptEnd;
                        }
                        else
                        {
                            ResetPoint(5);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X, ptBegin.Y + MIN_DISTANCE);
                            m_arrPoint[2] = new PointF(ptBegin.X - sizeBeginSection.Width / 2 - MIN_DISTANCE, m_arrPoint[1].Y);
                            m_arrPoint[3] = new PointF(m_arrPoint[2].X, ptEnd.Y);
                            m_arrPoint[4] = ptEnd;
                        }
                    }
                }
            }
            else// if (posBegin == ArrowPosition.LEFT)
            {
                if (posEnd == ArrowPosition.TOP)
                {
                    float fBeginTop = ptBegin.Y - sizeBeginSection.Height / 2;
                    float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                    float x = fEndLeft > ptBegin.X ? ptBegin.X - MIN_DISTANCE : fEndLeft - MIN_DISTANCE;
                    float y = fBeginTop > ptEnd.Y ? fBeginTop - MIN_DISTANCE : ptEnd.Y - MIN_DISTANCE;

                    ResetPoint(5);

                    m_arrPoint[0] = ptBegin;
                    m_arrPoint[1] = new PointF(x, ptBegin.Y);
                    m_arrPoint[2] = new PointF(x, y);
                    m_arrPoint[3] = new PointF(ptEnd.X, y);
                    m_arrPoint[4] = ptEnd;
                }
                else if (posEnd == ArrowPosition.RIGHT)
                {
                    float fBeginTop = ptBegin.Y - sizeBeginSection.Height / 2;
                    float fEndBottom = ptEnd.Y + sizeEndSection.Height / 2;

                    if (fBeginTop >= fEndBottom + MIN_DISTANCE)
                    {
                        float y = fBeginTop >= fEndBottom + MIN_DISTANCE * 2 ? fEndBottom + MIN_DISTANCE : (fBeginTop + fEndBottom) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, y);
                        m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, y);
                        m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                    else
                    {
                        float fBeginRight = ptBegin.X + sizeBeginSection.Width;

                        if (ptEnd.X > fBeginRight)
                        {
                            ResetPoint(6);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                            m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptBegin.Y + sizeBeginSection.Height / 2 + MIN_DISTANCE);
                            m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                            m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                            m_arrPoint[5] = ptEnd;
                        }
                        else
                        {
                            ResetPoint(6);
                                
                            float fEndLeft = ptEnd.X - sizeEndSection.Width;
                            float x = fEndLeft > ptBegin.X ? ptBegin.X - MIN_DISTANCE : fEndLeft - MIN_DISTANCE;

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(x, ptBegin.Y);
                            m_arrPoint[2] = new PointF(x, ptEnd.Y - sizeEndSection.Height / 2 - MIN_DISTANCE);
                            m_arrPoint[3] = new PointF(ptEnd.X + MIN_DISTANCE, m_arrPoint[2].Y);
                            m_arrPoint[4] = new PointF(m_arrPoint[3].X, ptEnd.Y);
                            m_arrPoint[5] = ptEnd;
                        }
                    }
                }
                else if (posEnd == ArrowPosition.BOTTOM)
                {
                    float fBeginTop = ptBegin.Y - sizeEndSection.Height / 2;

                    if (fBeginTop >= ptEnd.Y + MIN_DISTANCE)
                    {
                        float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                        float y = fBeginTop >= ptEnd.Y + MIN_DISTANCE * 2 ? ptEnd.Y + MIN_DISTANCE : (fBeginTop + ptEnd.Y) / 2;
                        float x = fEndLeft > ptBegin.X ? ptBegin.X - MIN_DISTANCE : fEndLeft - MIN_DISTANCE;

                        ResetPoint(5);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, y);
                        m_arrPoint[3] = new PointF(ptEnd.X, y);
                        m_arrPoint[4] = ptEnd;
                    }
                    else
                    {
                        float fBeginRight = ptBegin.X + sizeBeginSection.Width;

                        if (ptEnd.X > fBeginRight)
                        {
                            ResetPoint(5);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                            m_arrPoint[2] = new PointF(m_arrPoint[1].X, ptBegin.Y + sizeBeginSection.Height / 2 + MIN_DISTANCE);
                            m_arrPoint[3] = new PointF(ptEnd.X, m_arrPoint[2].Y);
                            m_arrPoint[4] = ptEnd;
                        }
                        else
                        {
                            float fEndLeft = ptEnd.X - sizeEndSection.Width / 2;
                            float x = fEndLeft > ptBegin.X ? ptBegin.X - MIN_DISTANCE : fEndLeft - MIN_DISTANCE;
                            float y = (fBeginTop + ptEnd.Y) / 2;

                            ResetPoint(5);

                            m_arrPoint[0] = ptBegin;
                            m_arrPoint[1] = new PointF(x, ptBegin.Y);
                            m_arrPoint[2] = new PointF(x, y);
                            m_arrPoint[3] = new PointF(ptEnd.X, y);
                            m_arrPoint[4] = ptEnd;
                        }
                    }
                }
                else// if (posEnd == ArrowPosition.LEFT)
                {
                    float fBeginTop = ptBegin.Y - sizeBeginSection.Height / 2;

                    if (fBeginTop >= ptEnd.Y + MIN_DISTANCE)
                    {
                        ResetPoint(4);

                        float x = ptEnd.X > ptBegin.X ? ptBegin.X - MIN_DISTANCE : ptEnd.X - MIN_DISTANCE;

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(x, ptBegin.Y);
                        m_arrPoint[2] = new PointF(x, ptEnd.Y);
                        m_arrPoint[3] = ptEnd;
                    }
                    else
                    {
                        float fBeginRight = ptBegin.X + sizeBeginSection.Width;
                        float x = ptEnd.X >= fBeginRight + MIN_DISTANCE * 2 ? ptEnd.X - MIN_DISTANCE : (ptEnd.X + fBeginRight) / 2;

                        ResetPoint(6);

                        m_arrPoint[0] = ptBegin;
                        m_arrPoint[1] = new PointF(ptBegin.X - MIN_DISTANCE, ptBegin.Y);
                        m_arrPoint[2] = new PointF(m_arrPoint[1].X, fBeginTop - MIN_DISTANCE);
                        m_arrPoint[3] = new PointF(x, m_arrPoint[2].Y);
                        m_arrPoint[4] = new PointF(x, ptEnd.Y);
                        m_arrPoint[5] = ptEnd;
                    }
                }
            }
        }

        // 우측 상단에서 좌측 하단 방향으로 화살표
        private void CalcNEArrowLine(PointF ptBegin, PointF ptEnd, Section sectionBegin, Section sectionEnd, ArrowPosition posBegin, ArrowPosition posEnd)
        {
            // NE의 반대인 SW를 사용한다.
            CalcSWArrowLine(ptEnd, ptBegin, sectionEnd, sectionBegin, posEnd, posBegin);

            // Begin과 End를 바꾸어서 계산하였으므로, m_arrPoint의 순서를 뒤집어준다.
            int nPointCount = m_arrPoint.Count();
            int nHalfCount = nPointCount / 2;

            for (int i = 0; i < nHalfCount; i++)
            {
                PointF pt1 = m_arrPoint[i];
                PointF pt2 = m_arrPoint[nPointCount - 1 - i];
                PointF ptTemp = pt1;

                m_arrPoint[i] = pt2;
                m_arrPoint[nPointCount - 1 - i] = ptTemp;
            }
        }

        // 우측 하단에서 좌측 상단 방향으로 화살표
        private void CalcSEArrowLine(PointF ptBegin, PointF ptEnd, Section sectionBegin, Section sectionEnd, ArrowPosition posBegin, ArrowPosition posEnd)
        {
            // SE의 반대인 NW를 사용한다.
            CalcNWArrowLine(ptEnd, ptBegin, sectionEnd, sectionBegin, posEnd, posBegin);

            // Begin과 End를 바꾸어서 계산하였으므로, m_arrPoint의 순서를 뒤집어준다.
            int nPointCount = m_arrPoint.Count();
            int nHalfCount = nPointCount / 2;

            for (int i = 0; i < nHalfCount; i++)
            {
                PointF pt1 = m_arrPoint[i];
                PointF pt2 = m_arrPoint[nPointCount - 1 - i];
                PointF ptTemp = pt1;

                m_arrPoint[i] = pt2;
                m_arrPoint[nPointCount - 1 - i] = ptTemp;
            }
        }

        public bool Select(float x, float y, float fSelectDistance, bool includeText)
        {
            if (includeText && m_strText.Length > 0)
            {
                if (x >= m_rectText.X && x <= m_rectText.X + m_rectText.Width &&
                    y >= m_rectText.Y && y <= m_rectText.Y + m_rectText.Height)
                    return true;
            }

            PointF pt = new PointF(x, y);
            int nPointCount = m_arrPoint.Count();

            for (int i = 1; i < nPointCount; i++)
            {
                PointF pt1 = m_arrPoint[i - 1];
                PointF pt2 = m_arrPoint[i];

                PointF pt3 = Geometry.GetNearestPoint(pt, pt1, pt2);
                double dLen = Geometry.GetDistance(pt, pt3);

                if (dLen <= fSelectDistance)
                    return true;
            }

            return false;
        }

        public void Select(bool isSelected)
        {
            m_isSelected = isSelected;
        }

        private static StringFormat GetStringFormat()
        {
            StringFormat format = new StringFormat();

            // Set the LineAlignment and Alignment properties for 
            // both StringFormat objects to different values.
            format.LineAlignment = StringAlignment.Near;
            format.Alignment = StringAlignment.Center;

            return format;
        }

        // 좌표 조절을 하면 일자로 나타낼 수 있는 화살표인가?
        // isHorz : true이면 X축으로 nSnapPixel만큼 움직여야 함
        //          false이면 Y축으로 nSnapPixel만큼 움직여야 함
        // fDistance : 얼만큼 이동하면 일자로 바뀔 수 있는가?
        public bool CanBeStraight(out bool isHorz, out float fDistance)
        {
            isHorz = true;
            fDistance = -1;

            if (m_sectionBegin == null ||
                m_sectionEnd == null)
                return false;

            if (m_posBegin == ArrowPosition.BOTTOM && m_posEnd == ArrowPosition.TOP)
            {
                isHorz = true;
                if (!GetSnapDistance(m_posBegin, m_posEnd, isHorz, out fDistance))
                    return false;

                return true;
            }
            else if (m_posBegin == ArrowPosition.RIGHT && m_posEnd == ArrowPosition.LEFT)
            {
                isHorz = false;
                if (!GetSnapDistance(m_posBegin, m_posEnd, isHorz, out fDistance))
                    return false;

                return true;
            }
            else if (m_posBegin == ArrowPosition.TOP && m_posEnd == ArrowPosition.BOTTOM)
            {
                isHorz = true;
                if (!GetSnapDistance(m_posBegin, m_posEnd, isHorz, out fDistance))
                    return false;

                return true;
            }
            else if (m_posBegin == ArrowPosition.LEFT && m_posEnd == ArrowPosition.RIGHT)
            {
                isHorz = false;
                if (!GetSnapDistance(m_posBegin, m_posEnd, isHorz, out fDistance))
                    return false;

                return true;
            }

            return false;
        }

        // isHorz이 true일 경우 pos1과 pos2간에 X축으로 떨어진 거리를 리턴한다.
        // isHorz이 false일 경우 pos1과 pos2간에 Y축으로 떨어진 거리를 리턴한다.
        private bool GetSnapDistance(ArrowPosition pos1, ArrowPosition pos2, bool isHorz, out float fDistance)
        {
            fDistance = 0.0f;

            bool isSuccess;
            PointF pt1 = m_sectionBegin.GetArrowPoint(pos1, out isSuccess);

            if (!isSuccess)
                return false;

            PointF pt2 = m_sectionEnd.GetArrowPoint(pos2, out isSuccess);

            if (!isSuccess)
                return false;

            if (isHorz)
                fDistance = pt2.X > pt1.X ? pt2.X - pt1.X : pt1.X - pt2.X;
            else
                fDistance = pt2.Y > pt1.Y ? pt2.Y - pt1.Y : pt1.Y - pt2.Y;

            return true;
        }

        public Section BeginLink
        {
            get { return m_sectionBegin; }
            set { m_sectionBegin = value; }
        }

        public Section EndLink
        {
            get { return m_sectionEnd; }
            set { m_sectionEnd = value; }
        }

        public ArrowPosition BeginPosition
        {
            get { return m_posBegin; }
            set { m_posBegin = value; }
        }

        public ArrowPosition EndPosition
        {
            get { return m_posEnd; }
            set { m_posEnd = value; }
        }

        public static int SELECT_DISTANCE
        {
            get { return _SELECT_DISTANCE; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public Color LineColor
        {
            get { return m_lineColor; }
            set { m_lineColor = value; }
        }

        public Color FillColor
        {
            get { return m_fillColor; }
            set { m_fillColor = value; }
        }

        public int LineThick
        {
            get { return m_nLineThick; }
            set { m_nLineThick = value; }
        }

        public System.Drawing.Drawing2D.DashStyle LineStyle
        {
            get { return m_lineStyle; }
            set { m_lineStyle = value; }
        }
    }
}
