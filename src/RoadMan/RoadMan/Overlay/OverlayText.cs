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
	
				//if( m_pt)
				
			}
		}

		private Font m_Font = null;
		private bool m_bPt1Set = false;
		private bool m_bPt2Set = false;

		private float m_fFontSize = 10.0f;
		private Size mTextSize;

		
		private string m_szFontName = "맑은 고딕";
		public string FontName
		{
			get { return m_szFontName; }
			set { m_szFontName = value; }
		}


		private FontStyle mFontStyle;

		private bool m_bFontBold = false;
		public bool FontStyleBold
		{
			get { return m_bFontBold; }
			set { m_bFontBold = value; }
		}
		private bool m_bFontUnderLine = false;
		public bool FontStyleUnderLine
		{
			get { return m_bFontUnderLine; }
			set { m_bFontUnderLine = value; }
		}

		private bool m_bFontItalic = false;
		public bool FontStyleItalic
		{
			get { return m_bFontItalic; }
			set { m_bFontItalic = value; }
		}

		private float m_FontHeight = 10.0f;
		public float FontHeight
		{
			get { return m_FontHeight; }
			set { m_FontHeight = value; }
		}
		
		private string m_szText = "";
		public string Text
		{
			get { return m_szText; }
			set
			{
				m_szText = value;

				float fSize = m_FontHeight;
				if (fSize <= 0.01f)
					return;

				mFontStyle = FontStyle.Regular;
				if (m_bFontBold)
				{
					mFontStyle = FontStyle.Bold;
				}
				if (m_bFontItalic)
				{
					mFontStyle |= FontStyle.Italic;
				}
				if (m_bFontUnderLine)
				{
					mFontStyle |= FontStyle.Underline;
				}
				m_Font = new Font(m_szFontName, fSize, mFontStyle);
				
				float dx = m_DistGravity * 2.0f;

				mTextSize = TextRenderer.MeasureText(m_szText, m_Font);
				float x = (float)Math.Min(m_pt1.X, m_pt2.X);
				float y = (float)Math.Min(m_pt1.Y, m_pt2.Y);

				float width = mTextSize.Width;
				float height = mTextSize.Height;

				m_OffsetRect = new RectangleF(x - m_DistGravity, y - m_DistGravity, width + dx, height + dx);
				
			}
		}

		public override void DrawElement(PaintEventArgs e)
		{
			if (m_fFontSize < 0.05f)
				return;

			Graphics g = e.Graphics;
			//SetScale(g);
			Color penColor = m_LineColor;
			if (m_bHighLight)
				penColor = m_HighLightColor;
			if (m_bPt1Set && m_bPt2Set)
			{
				Matrix m = g.Transform.Clone();
				using (Brush pen = new SolidBrush(penColor))
				{
					g.ScaleTransform(1.0f, -1.0f);
					g.DrawString(m_szText, m_Font, pen, new PointF(BasePoint.X, -BasePoint.Y));
					
				}				
				g.Transform = m;
			}
			//UnsetScale(g);
		}


		public override void TempDrawElement(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Color penColor = Color.Gray;
			if (m_bPt1Set && m_bPt2Set)
			{
				using (Pen pen = new Pen(penColor))
				{
					pen.Width = 2.0F;
					pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
					pen.DashStyle = DashStyle.Dash;

					float x = (float)Math.Min(m_pt1.X, m_pt2.X);
					float y = (float)Math.Min(m_pt1.Y, m_pt2.Y);
					float width = Math.Abs(m_pt1.X - m_pt2.X) + 4;
					float height = Math.Abs(m_pt1.Y - m_pt2.Y) + 4;

					if (width < 0.05f || height < 0.05f)
						return;					

					
					//e.Graphics.DrawRectangle(pen, rectDraw);					
					g.DrawRectangle(pen, x- 2, y - 2, width, height);
					
				}				
			}
		}

		public override bool IsPicked(PointF pt)
		{
			if (m_OffsetRect.Contains(pt.X, pt.Y))
				return true;			
			return false;
		}

        public override bool IsPicked(RectangleF rect)
        {
            if (m_OffsetRect.Contains(rect))
                return true;

            if (m_OffsetRect.IntersectsWith(rect))
                return true;

            return false;
        }
	}
}
