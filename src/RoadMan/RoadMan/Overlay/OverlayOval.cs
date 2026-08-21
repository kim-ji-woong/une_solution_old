using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace UnE.Overlay
{
	public class OverlayOval : OverlayElement
	{
		private RectangleF mRect = new RectangleF();
		//private RectangleF m_UnderOffsetRect;
        private UnE.Geometry.EArc2D m_ellipse = new Geometry.EArc2D();

		private PointF m_pt1;
		public PointF Point1
		{
			get { return m_pt1; }
			set
			{
				m_pt1 = value;
				m_bPt1Set = true;
			}
		}
		private PointF m_pt2;
		public PointF Point2
		{
			get { return m_pt2; }
			set
			{
				m_pt2 = value;
				m_bPt2Set = true;
				float dx = m_DistGravity * 2.0f;

				float x = (float)Math.Min(m_pt1.X, m_pt2.X);
				float y = (float)Math.Min(m_pt1.Y, m_pt2.Y);

				float width = Math.Abs(m_pt1.X - m_pt2.X);
				float height = Math.Abs(m_pt1.Y - m_pt2.Y);

                UnE.Geometry.Vertex2D vTL = new Geometry.Vertex2D(x, y + height);
                UnE.Geometry.Vertex2D vBL = new Geometry.Vertex2D(x, y);
                UnE.Geometry.Vertex2D vBR = new Geometry.Vertex2D(x + width, y);

                m_ellipse.SetEArc(vTL, vBL, vBR, 0.0, UnE.Geometry.Math._2PI(), true);

				//m_OffsetRect = new RectangleF(x - m_DistGravity, y - m_DistGravity, width + dx, height + dx);


				mRect = new RectangleF(x, y, width, height);
				//m_OffsetRect.Offset(m_DistGravity, m_DistGravity);
				//m_UnderOffsetRect = new RectangleF(x + m_DistGravity, y + m_DistGravity, width - dx, height - dx);
			}
		}
		private bool m_bPt1Set = false;
		private bool m_bPt2Set = false;

		public override void DrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Color penColor = m_LineColor;
			if (m_bHighLight)
				penColor = m_HighLightColor;
			if (m_bPt1Set && m_bPt2Set)
			{
				//if (e.ClipRectangle.Contains(m_pt1) || e.ClipRectangle.Contains(m_pt2))
				{
					using (Pen pen = new Pen(penColor, m_Thick))
					{						
						g.DrawEllipse(pen, mRect.X, mRect.Y, mRect.Width, mRect.Height);
					}
				}
			}
		}

		public override void TempDrawElement(PaintEventArgs e)
		{
			DrawElement(e);
		}

		public override bool IsPicked(PointF pt)
		{
            return false;
			/*if (!m_OffsetRect.Contains(pt.X, pt.Y))
				return false;
			if (m_UnderOffsetRect.Contains(pt.X, pt.Y))
				return false;

			return true;*/
		}

        public override bool IsPicked(RectangleF rect)
        {
            UnE.Geometry.Vertex2D vRect = new Geometry.Vertex2D(rect.X, rect.Y);
            UnE.Geometry.Vertex2D vEllipseCenter = m_ellipse.GetCenter();
            UnE.Geometry.Line2D vCenterLine = new Geometry.Line2D(vRect, vEllipseCenter, Geometry.Line2D.LineType.SEGMENT);

            UnE.Geometry.Vertex2D result1, result2;

            // rect가 타원안에 포함되어 있다.
            if (m_ellipse.IntersectLine(vCenterLine, out result1, out result2) == 0)
                return true;

            UnE.Geometry.Vertex2D v1 = new Geometry.Vertex2D(rect.X, rect.Y);
            UnE.Geometry.Vertex2D v2 = new Geometry.Vertex2D(rect.X + rect.Width, rect.Y);
            UnE.Geometry.Vertex2D v3 = new Geometry.Vertex2D(rect.X + rect.Width, rect.Y + rect.Height);
            UnE.Geometry.Vertex2D v4 = new Geometry.Vertex2D(rect.X, rect.Y + rect.Height);

            UnE.Geometry.Line2D[] arrLines = new Geometry.Line2D[4];
            arrLines[0] = new Geometry.Line2D(v1, v2, Geometry.Line2D.LineType.SEGMENT);
            arrLines[1] = new Geometry.Line2D(v2, v3, Geometry.Line2D.LineType.SEGMENT);
            arrLines[2] = new Geometry.Line2D(v3, v4, Geometry.Line2D.LineType.SEGMENT);
            arrLines[3] = new Geometry.Line2D(v4, v1, Geometry.Line2D.LineType.SEGMENT);

            foreach (UnE.Geometry.Line2D line in arrLines)
            {
                if (m_ellipse.IntersectLine(line, out result1, out result2) > 0)
                    return true;
            }

            return false;
        }
	}
}
