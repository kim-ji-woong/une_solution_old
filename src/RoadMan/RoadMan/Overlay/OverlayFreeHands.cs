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
using System.Xml.Serialization;

namespace UnE.Overlay
{

	public class OverlayFreeHands : OverlayElement
	{
		[XmlArray("PointArray")]
		[XmlArrayItem("PointF")]
		public List<PointF> m_arPointList = new List<PointF>();
		public void AddPoint(PointF pt)
		{
			if (m_bClosed == true)
				return;
			m_arPointList.Add(pt);
		}

		private bool m_bClosed = false;
		public void Close()
		{
			m_bClosed = true;

			points = new PointF[m_arPointList.Count];
			int nIdx = 0;

			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;

			foreach (PointF pt in m_arPointList)
			{
				points[nIdx++] = pt;



				if (minX > (float)pt.X)
				{
					minX = (float)pt.X;
				}

				if (maxX < (float)pt.X)
				{
					maxX = (float)pt.X;
				}

				if (minY > (float)pt.Y)
				{
					minY = (float)pt.Y;
				}

				if (maxY < (float)pt.Y)
				{
					maxY = (float)pt.Y;
				}
			}


			float dx = m_DistGravity * 2.0f;
			float width = maxX - minX + dx;
			float height = maxY - minY + dx;
			m_OffsetRect = new RectangleF(minX, minY, width, height);
		}

		private PointF[] points = null;

		public override void DrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (m_bClosed == false)
				return;

			Color brColor = m_LineColor;
			if (m_bHighLight)
				brColor = m_HighLightColor;

			using (Brush br = new SolidBrush(brColor))
			{
				GraphicsPath path = new GraphicsPath();
				if (m_arPointList.Count <= 1)
				{
					if (m_arPointList.Count == 0)
						return;

					PointF pt = (PointF)m_arPointList[0];
					g.FillEllipse(br, pt.X, pt.Y, m_Thick, m_Thick);
				}
				else
				{
					path.AddLines(points);
				}
				using (Pen pen = new Pen(brColor, m_Thick))
				{
					g.DrawPath(pen, path);
				}
			}
		}

		public override void TempDrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			using (Brush br = new SolidBrush(m_LineColor))
			{
				GraphicsPath path = new GraphicsPath();
				if (m_arPointList.Count <= 1)
				{
					if (m_arPointList.Count == 0)
						return;

					PointF pt = (PointF)m_arPointList[0];
					g.FillEllipse(br, pt.X, pt.Y, m_Thick, m_Thick);
				}
				else
				{
					PointF[] points = new PointF[m_arPointList.Count];
					int nIdx = 0;
					foreach (PointF pt in m_arPointList)
					{
						points[nIdx++] = pt;

					}
					path.AddLines(points);
				}
				using (Pen pen = new Pen(m_LineColor, m_Thick))
				{
					g.DrawPath(pen, path);
				}
			}
		}

		public override bool IsPicked(PointF pt)
		{

			if (!m_OffsetRect.Contains(pt.X, pt.Y))
				return false;

			if( points.Length == 1)
				return true;

			for(int i = 1 ; i < points.Length ; i++)
			{
				PointF pt1 = points[i - 1];
				PointF pt2 = points[i];
				float tx = (float)(pt2.X - pt1.X);

				if (tx != 0)
				{
					float dx = (pt.X - pt1.X) / (pt2.X - pt1.X);
					float dy = dx * (float)(pt2.Y - pt1.Y) + (float)pt1.Y;

					float dist = pt.Y - dy;
					if (dist < m_DistGravity && dist > -m_DistGravity)
						return true;
				}
				else
				{
					if (pt.Y < m_DistGravity && pt.Y > -m_DistGravity)
						return true;
				}
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

            int nPointCount = m_arPointList.Count;
            if (nPointCount < 2)
                return false;

            UnE.Geometry.Line2D[] arrLines = new Geometry.Line2D[4];
            arrLines[0] = new Geometry.Line2D(v1, v2, Geometry.Line2D.LineType.SEGMENT);
            arrLines[1] = new Geometry.Line2D(v2, v3, Geometry.Line2D.LineType.SEGMENT);
            arrLines[2] = new Geometry.Line2D(v3, v4, Geometry.Line2D.LineType.SEGMENT);
            arrLines[3] = new Geometry.Line2D(v4, v1, Geometry.Line2D.LineType.SEGMENT);

            PointF ptPrev = m_arPointList[0];
            UnE.Geometry.Vertex2D vPrev = new Geometry.Vertex2D(ptPrev.X, ptPrev.Y);

            for (int i = 1; i < nPointCount;i++)
            {
                PointF ptCurrent = m_arPointList[i];
                UnE.Geometry.Vertex2D vCurrent = new Geometry.Vertex2D(ptCurrent.X, ptCurrent.Y);
                UnE.Geometry.Line2D line = new Geometry.Line2D(vPrev, vCurrent, Geometry.Line2D.LineType.SEGMENT);

                foreach (UnE.Geometry.Line2D line2 in arrLines)
                {
                    if (line.IntersectLine(line2, out result1, out result2, out resultType) > 0)
                        return true;
                }

                vPrev = vCurrent;
            }

            return false;
        }

        public override void OnPostXMLRead()
        {
            Close();
        }
	}
	
}
