using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sections;

namespace SectionStyle.Fancy
{
    class StyleEndPoint : Style
    {
        private const int CIRCLE_SLICE = 100;        
        private PointF m_ptPrev = new PointF(0, 0);

        public StyleEndPoint(SectionEndPoint section, ShapeEndPoint shape)
        {
            m_section = section;
            m_shape = shape;
            m_arrDrawing = new PointF[CIRCLE_SLICE + 2];
        }

        protected override void Resize(float x, float y, float fWidth, float fHeight)
        {
            if (m_shape == null)
                return;

            m_ptPrev.X = x;
            m_ptPrev.Y = y;

            double dRadius = fHeight / 2.0;
            double centerX = x + dRadius, centerX2 = x + fWidth - dRadius;
            double centerY = y + dRadius;
            double dStartAngle = 0.0;
            double dStartAngle2 = System.Math.PI;
            double dArcAngle = System.Math.PI;
            double dAddAngle = 0.0;
            float fArcWidth = (float)dRadius;

            // 전체 너비가 지름의 3배보다 작을 경우
            if (dRadius * 4 > fWidth)
            {
                double dAddRadius = dRadius * 4 - fWidth;
                centerX += dAddRadius;
                centerX2 -= dAddRadius;

                dRadius = centerX - x;

                dAddAngle = System.Math.Acos((fHeight / 2) / dRadius);
                dStartAngle += dAddAngle;
                dStartAngle2 += dAddAngle;
                dArcAngle -= dAddAngle * 2;

                fArcWidth = (float)(dRadius - dRadius * System.Math.Sin(dAddAngle));
            }

            float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;

            int nSliceCount = CIRCLE_SLICE / 2;
            double delta = dArcAngle / nSliceCount;

            for (int i = 0; i <= nSliceCount; i++)
            {
                double dAngle = delta * i + dStartAngle;
                double _x = centerX - dRadius * System.Math.Sin(dAngle);
                double _y = centerY + dRadius * System.Math.Cos(dAngle);

                m_arrDrawing[i] = new PointF((float)_x, (float)_y);

                if (i == 0)
                {
                    _left = m_arrDrawing[0].X;
                    _top = m_arrDrawing[0].Y;
                    _right = m_arrDrawing[0].X;
                    _bottom = m_arrDrawing[0].Y;
                }
                else
                {
                    if (_left > m_arrDrawing[i].X) _left = m_arrDrawing[i].X;
                    if (_right < m_arrDrawing[i].X) _right = m_arrDrawing[i].X;
                    if (_top > m_arrDrawing[i].Y) _top = m_arrDrawing[i].Y;
                    if (_bottom < m_arrDrawing[i].Y) _bottom = m_arrDrawing[i].Y;
                }
            }

            // CIRCLE_SLICE가 홀수일수 있으므로 2로 나누지 않고 nSliceCount를 빼준다.
            int nSliceCount2 = CIRCLE_SLICE - nSliceCount;
            delta = dArcAngle / nSliceCount2;

            for (int i = 0; i <= nSliceCount2; i++)
            {
                double dAngle = delta * i + dStartAngle2;
                double _x = centerX2 - dRadius * System.Math.Sin(dAngle);
                double _y = centerY + dRadius * System.Math.Cos(dAngle);

                int nIndex = i + nSliceCount + 1;
                m_arrDrawing[nIndex] = new PointF((float)_x, (float)_y);

                if (_left > m_arrDrawing[nIndex].X) _left = m_arrDrawing[nIndex].X;
                if (_right < m_arrDrawing[nIndex].X) _right = m_arrDrawing[nIndex].X;
                if (_top > m_arrDrawing[nIndex].Y) _top = m_arrDrawing[nIndex].Y;
                if (_bottom < m_arrDrawing[nIndex].Y) _bottom = m_arrDrawing[nIndex].Y;
            }

            MakePath();
        }

        protected override void Move(float x, float y)
        {
            float fMoveX = x - m_ptPrev.X;
            float fMoveY = y - m_ptPrev.Y;

            float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;
            int nPointCount = m_arrDrawing.Count();

            for (int i = 0; i < nPointCount; i++)
            {
                m_arrDrawing[i].X += fMoveX;
                m_arrDrawing[i].Y += fMoveY;

                if (i == 0)
                {
                    _left = m_arrDrawing[0].X;
                    _top = m_arrDrawing[0].Y;
                    _right = m_arrDrawing[0].X;
                    _bottom = m_arrDrawing[0].Y;
                }
                else
                {
                    if (_left > m_arrDrawing[i].X) _left = m_arrDrawing[i].X;
                    if (_right < m_arrDrawing[i].X) _right = m_arrDrawing[i].X;
                    if (_top > m_arrDrawing[i].Y) _top = m_arrDrawing[i].Y;
                    if (_bottom < m_arrDrawing[i].Y) _bottom = m_arrDrawing[i].Y;
                }

                m_ptPrev.X = x;
                m_ptPrev.Y = y;
            }

            MakePath();
        }

        private void MakePath()
        {
            m_path.Reset();
            m_path.StartFigure();

            for (int i = 1; i < m_arrDrawing.Length; i++)
            {
                m_path.AddLine(
                    m_arrDrawing[i - 1].X, m_arrDrawing[i - 1].Y,
                    m_arrDrawing[i].X, m_arrDrawing[i].Y
                    );
            }

            m_path.CloseFigure();
        }
    }
}
