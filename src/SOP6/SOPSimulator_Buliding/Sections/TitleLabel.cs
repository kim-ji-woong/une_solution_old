using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Sections
{
    public partial class TitleLabel : Label
    {
        private const int CIRCLE_SLICE = 100;

        private Color m_fillStartColor = Color.FromArgb(218, 36, 68);
        private Color m_fillEndColor = Color.FromArgb(233, 118, 70);
        private GraphicsPath m_path = null;
        private PointF[] m_arrDrawing = null;
        private SolidBrush m_textBrush = new SolidBrush(Color.White);
        private RectangleF m_rectTitle = new RectangleF();
        private RectangleF m_rectTimeNPos = new RectangleF();
        private RectangleF m_rectSubText1 = new RectangleF();
        private RectangleF m_rectSubText2 = new RectangleF();
        private RectangleF m_rectSubText3 = new RectangleF();
        private StringFormat m_textFormat = new StringFormat();
        private StringFormat m_textFormat2 = new StringFormat();
        private StringFormat m_textFormat3 = new StringFormat();
        private string m_strPosition = "";
        private string m_strTime = "";
        private string m_strSubTextTop = "", m_strSubTextMiddle = "", m_strSubTextBottom = "";
        private Font m_fontTimeNPos = new System.Drawing.Font("나눔바른고딕", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Font m_fontSubText = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }

        public TitleLabel()
        {
            InitializeComponent();
            m_arrDrawing = new PointF[CIRCLE_SLICE + 2];

            m_textFormat.LineAlignment = StringAlignment.Center;
            m_textFormat.Alignment = StringAlignment.Center;
            m_textFormat2.LineAlignment = StringAlignment.Near;
            m_textFormat2.Alignment = StringAlignment.Center;
            m_textFormat3.LineAlignment = StringAlignment.Center;
            m_textFormat3.Alignment = StringAlignment.Far;
        }

        private void TitleLabel_Paint(object sender, PaintEventArgs e)
        {
            if (m_path == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Point pt1 = new Point(0, this.Size.Height / 2);
            Point pt2 = new Point(this.Size.Width, pt1.Y);

            LinearGradientBrush brush = new LinearGradientBrush(pt1, pt2, m_fillStartColor, m_fillEndColor);
            e.Graphics.FillPath(brush, m_path);
            brush.Dispose();

            if (this.Text.Length > 0)
            {
                //e.Graphics.DrawString("지하 주차장 화재 발생", this.Font, m_textBrush, m_rectTitle, m_textFormat);
                e.Graphics.DrawString(this.Text, this.Font, m_textBrush, m_rectTitle, m_textFormat);
            }

            string strTimeNPos = GetTimeNPos();

            if (strTimeNPos.Length > 0)
                e.Graphics.DrawString(strTimeNPos, m_fontTimeNPos, m_textBrush, m_rectTimeNPos, m_textFormat2);

            e.Graphics.DrawString(m_strSubTextTop, m_fontSubText, m_textBrush, m_rectSubText1, m_textFormat3);
            e.Graphics.DrawString(m_strSubTextMiddle, m_fontSubText, m_textBrush, m_rectSubText2, m_textFormat3);
            e.Graphics.DrawString(m_strSubTextBottom, m_fontSubText, m_textBrush, m_rectSubText3, m_textFormat3);
        }

        private string GetTimeNPos()
        {
            string str = "";

            if (m_strTime.Length > 0)
            {
                str = m_strTime;
            }

            if (m_strPosition.Length > 0)
            {
                if (str.Length > 0)
                    str += " " + m_strPosition;
                else
                    str = m_strPosition;
            }

            return str;
        }

        private void TitleLabel_Resize(object sender, EventArgs e)
        {
            if (this.Size.Width == 0 || this.Size.Height == 0)
            {
                m_path = null;
                return;
            }
            else if (m_path == null)
                m_path = new GraphicsPath();

            OnResize(0, 0, this.Size.Width, this.Size.Height);
        }

        private void OnResize(float x, float y, float fWidth, float fHeight)
        {
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

            float fRadius = (float)dRadius;
            float fAdd1 = 5.0f, fAdd2 = 10.0f;
            m_rectTitle = new RectangleF(fRadius, fAdd1, fWidth - fRadius * 2, fHeight / 2 + fAdd1);
            m_rectTimeNPos = new RectangleF(fRadius, fHeight / 2 + fAdd2, fWidth - fRadius * 2, fHeight + fAdd2);

            float subTextWidth = (fWidth - fRadius * 2) * 0.2f;
            float subTextBeginX = fWidth - fRadius - subTextWidth;
            float subTextSpace = 10;

            float y1 = (fHeight - (m_fontSubText.Height * 3 + subTextSpace * 2)) / 2;
            float y2 = y1 + m_fontSubText.Height + subTextSpace;
            float y3 = y2 + m_fontSubText.Height + subTextSpace;

            m_rectSubText1 = new RectangleF(subTextBeginX, y1, subTextWidth, m_fontSubText.Height);
            m_rectSubText2 = new RectangleF(subTextBeginX, y2, subTextWidth, m_fontSubText.Height);
            m_rectSubText3 = new RectangleF(subTextBeginX, y3, subTextWidth, m_fontSubText.Height);

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

        public void SetSubText(string strTop, string strMiddle, string strBottom)
        {
            m_strSubTextTop = strTop;
            m_strSubTextMiddle = strMiddle;
            m_strSubTextBottom = strBottom;
        }

        public void GetSubText(ref string strTop, ref string strMiddle, ref string strBottom)
        {
            strTop = m_strSubTextTop;
            strMiddle = m_strSubTextMiddle;
            strBottom = m_strSubTextBottom;
        }
    }
}
