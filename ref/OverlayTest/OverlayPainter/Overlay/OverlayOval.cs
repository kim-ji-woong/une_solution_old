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
		private RectangleF m_UnderOffsetRect;

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


				m_OffsetRect = new RectangleF(x - m_DistGravity, y - m_DistGravity, width + dx, height + dx);


				mRect = new RectangleF(x, y, width, height);
				//m_OffsetRect.Offset(m_DistGravity, m_DistGravity);
				m_UnderOffsetRect = new RectangleF(x + m_DistGravity, y + m_DistGravity, width - dx, height - dx);
			}
		}
		private bool m_bPt1Set = false;
		private bool m_bPt2Set = false;

		//private System.Drawing.Drawing2D.LineCap m_LineCap = System.Drawing.Drawing2D.LineCap.Round;
		//public System.Drawing.Drawing2D.LineCap LineCap
		//{
		//	get { return m_LineCap; }
		//	set { m_LineCap = value; }
		//}

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
						//pen.StartCap = m_LineCap;
						//pen.EndCap = m_LineCap;
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
			if (!m_OffsetRect.Contains(pt.X, pt.Y))
				return false;
			if (m_UnderOffsetRect.Contains(pt.X, pt.Y))
				return false;

			return true;

			//float tx = (float)(m_pt2.X - m_pt1.X);

			//if (tx != 0)
			//{
			//	float dx = (float)(pt.X - m_pt1.X) / (float)(m_pt2.X - m_pt1.X);
			//	float dy = (float)dx * (float)(m_pt2.Y - m_pt1.Y) + (float)m_pt1.Y;

			//	float dist = pt.Y - dy;
			//	if (dist < m_DistGravity && dist > -m_DistGravity)
			//		return true;
			//}
			//else
			//{
			//	if (pt.Y < m_DistGravity && pt.Y > -m_DistGravity)
			//		return true;
			//}
			//return false;
		}
	}
}
