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
using System.Xml;

namespace UnE.Overlay
{
	public class OverlayLine : OverlayElement
	{
        private UnE.Geometry.Line2D m_line = new Geometry.Line2D(new Geometry.Vertex2D(), new Geometry.Vertex2D(), Geometry.Line2D.LineType.SEGMENT);

		private PointF m_pt1;
		public PointF Point1
		{
			get { return m_pt1; }
			set
			{
				m_pt1 = value;
				m_bPt1Set = true;
                m_line.SetVertex(new Geometry.Vertex2D(m_pt1.X, m_pt1.Y), true);
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
                m_line.SetVertex(new Geometry.Vertex2D(m_pt2.X, m_pt2.Y), false);
				/*float dx = m_DistGravity * 2.0f;

				float x = (float)Math.Min(m_pt1.X, m_pt2.X) - m_DistGravity;
				float y = (float)Math.Min(m_pt1.Y, m_pt2.Y) - m_DistGravity;

				float width = Math.Abs(m_pt1.X - m_pt2.X) + dx;
				float height = Math.Abs(m_pt1.Y - m_pt2.Y) + dx;


				m_OffsetRect = new RectangleF(x, y, width, height);*/
				//m_OffsetRect.Offset(m_DistGravity, m_DistGravity);

			}
		}
		private bool m_bPt1Set = false;
		private bool m_bPt2Set = false;

		private System.Drawing.Drawing2D.LineCap m_LineCap = System.Drawing.Drawing2D.LineCap.Round;
		public System.Drawing.Drawing2D.LineCap LineCap
		{
			get { return m_LineCap; }
			set { m_LineCap = value; }
		}

		public override void DrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Color penColor = m_LineColor;
			if (m_bHighLight)
				penColor = m_HighLightColor;
			if (m_bPt1Set && m_bPt2Set)
			{
				//if (e.ClipRectangle.Contains(m_pt1.X, m_pt1.Y) || e.ClipRectangle.Contains(m_pt2.X, m_pt2.Y))
				{
					using (Pen pen = new Pen(penColor, m_Thick))
					{
						pen.StartCap = m_LineCap;
						pen.EndCap = m_LineCap;
						g.DrawLine(pen, m_pt1, m_pt2);
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
			if (!m_OffsetRect.Contains(pt.X, pt.Y))
				return false;

			float tx = (float)(m_pt2.X - m_pt1.X);

			if (tx != 0)
			{
				float dx = (float)(pt.X - m_pt1.X) / (float)(m_pt2.X - m_pt1.X);
				float dy = (float)dx * (float)(m_pt2.Y - m_pt1.Y) + (float)m_pt1.Y;

				float dist = pt.Y - dy;
				if (dist < m_DistGravity && dist > -m_DistGravity)
					return true;
			}
			else
			{
				float minY = Math.Min(m_pt2.Y, m_pt1.Y);
				float maxY = Math.Max(m_pt2.Y, m_pt1.Y);

				if (pt.Y < maxY && pt.Y > minY)
					return true;
			}
			return false;
		}

		public override bool IsPicked(RectangleF rect)
		{
            UnE.Geometry.Vertex2D v1 = new Geometry.Vertex2D(rect.X, rect.Y);
            UnE.Geometry.Vertex2D v2 = new Geometry.Vertex2D(rect.X + rect.Width, rect.Y);
            UnE.Geometry.Vertex2D v3 = new Geometry.Vertex2D(rect.X + rect.Width, rect.Y + rect.Height);
            UnE.Geometry.Vertex2D v4 = new Geometry.Vertex2D(rect.X, rect.Y + rect.Height);

            UnE.Geometry.Vertex2D result1, result2;
            UnE.Geometry.Line2D.LineType resultType;
            
            if (m_line.IntersectLine(new Geometry.Line2D(v1, v2, Geometry.Line2D.LineType.SEGMENT), out result1, out result2, out resultType) > 0)
                return true;

            if (m_line.IntersectLine(new Geometry.Line2D(v2, v3, Geometry.Line2D.LineType.SEGMENT), out result1, out result2, out resultType) > 0)
                return true;

            if (m_line.IntersectLine(new Geometry.Line2D(v3, v4, Geometry.Line2D.LineType.SEGMENT), out result1, out result2, out resultType) > 0)
                return true;

            if (m_line.IntersectLine(new Geometry.Line2D(v4, v1, Geometry.Line2D.LineType.SEGMENT), out result1, out result2, out resultType) > 0)
                return true;

            return false;

			/*if (!m_OffsetRect.IntersectsWith(rect))
				return false;

			float tx = (float)(m_pt2.X - m_pt1.X);
			PointF pt = new PointF(rect.X, rect.Y + rect.Height);
			if (tx != 0)
			{
				float dx = (float)(pt.X - m_pt1.X) / (float)(m_pt2.X - m_pt1.X);
				float dy = (float)dx * (float)(m_pt2.Y - m_pt1.Y) + (float)m_pt1.Y;

				float dist = pt.Y - dy;
				if (dist < rect.Height && dist > -rect.Height)
					return true;
			}
			else
			{
				float minY = Math.Min(m_pt2.Y , m_pt1.Y);
				float maxY = Math.Max(m_pt2.Y , m_pt1.Y);

				if (pt.Y < maxY && pt.Y > minY)
					return true;
			}
			return false;*/
		}
	}	
}
