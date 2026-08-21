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
	public class OverlayText : OverlayElement
	{
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
				
			}
		}


		private Size mTextSize;
		private string m_szText = "테스트";
		public string Text
		{
			get { return m_szText; }
			set
			{
				m_szText = value;

				float fSize = Math.Abs(m_pt1.Y - m_pt2.Y);
				if (fSize <= 0.01f)
					return;

				m_Font = new Font("맑은 고딕", fSize);
				
				float dx = m_DistGravity * 2.0f;

				mTextSize = TextRenderer.MeasureText(m_szText, m_Font);
				float x = (float)Math.Min(m_pt1.X, m_pt2.X);
				float y = (float)Math.Min(m_pt1.Y, m_pt2.Y);

				float width = mTextSize.Width;
				float height = Math.Abs(m_pt1.Y - m_pt2.Y);

				m_OffsetRect = new RectangleF(x - m_DistGravity, y - m_DistGravity, width + dx, height + dx);
				
			}
		}

		private Font m_Font = null;
		private bool m_bPt1Set = false;
		private bool m_bPt2Set = false;

		private float m_fFontSize = 10.0f;

		public override void DrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			SetScale(g);
			Color penColor = m_LineColor;
			if (m_bHighLight)
				penColor = m_HighLightColor;
			if (m_bPt1Set && m_bPt2Set)
			{
				using (Brush pen = new SolidBrush(penColor))
				{
					g.DrawString(m_szText, m_Font, pen, BasePoint);
				}				
			}
			UnsetScale(g);
		}


		public override void TempDrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Color penColor = Color.Gray;
			
			if (m_bPt1Set)
			{
				using (Pen pen = new Pen(penColor))
				{
					pen.Width = 2.0F;
					pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
					pen.DashStyle = DashStyle.Dash;

					float width = Math.Abs(m_pt1.X - m_pt2.X) + 4;
					float height = Math.Abs(m_pt1.Y - m_pt2.Y) + 4;

					g.DrawRectangle(pen, m_pt1.X - 2, m_pt1.Y - 2, width, height);

				}				
			}
		}

		public override bool IsPicked(PointF pt)
		{
			if (m_OffsetRect.Contains(pt.X, pt.Y))
				return true;			
			return false;
		}
	}
}
