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

	public class OverlayFreeHands : OverlayElement
	{
		private ArrayList m_arPointList = new ArrayList();
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
			foreach (PointF pt in m_arPointList)
			{
				points[nIdx++] = pt;

			}
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

					Point pt = (Point)m_arPointList[0];
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
			return false;
		}


	}
	
}
