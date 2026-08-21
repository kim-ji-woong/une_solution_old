using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UnE.Overlay
{
	//public delegate void OnSelectRect(RectangleF rect);
	//public delegate void OnSelectScreenRect(Rectangle rect);

	public class OverlaySelectScreenRectPainter
	{
		public event InvalidateControl InvalidateControl;

		public event OnSelectRect OnSelectRect;

		public event OnSelectScreenRect OnSelectScreenRect;

		private DXFViewer.DXFControl m_ctrlViewer = null;
		public OverlaySelectScreenRectPainter(DXFViewer.DXFControl control)
		{
			m_ctrlViewer = control;

			RectColor = Color.FromArgb(128, Color.Aqua);
		}



		private Pen mRectPen = null;

		private Color m_RectColor;
		public Color RectColor
		{
			get { return m_RectColor; }
			set 
			{
				m_RectColor = value; 
				if( mRectPen != null)
				{
					mRectPen.Dispose();
				}
				mRectPen = new Pen(value);
			}
		}

		private RectangleF m_SelectRect;
		public RectangleF SelectRect
		{
			get 
			{				
				return m_SelectRect;
			}
			
		}

		private PointF m_ptDown;
		private PointF m_ptCurrent;
		//private bool m_bDragMode = false;

		private Rectangle m_ScreenRect;
		private Point m_ptScDown;
		private Point m_ptScCurrent;
		

		public void Invalidate()
		{
			if(InvalidateControl!= null)
			{
				InvalidateControl();
			}
		}

		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left)
			{
				m_ptScDown = e.Location;
				m_ptScCurrent = e.Location;
				m_ptDown = ScreenToGlobal(e.Location);
				m_ptCurrent = ScreenToGlobal(e.Location);

				m_SelectRect.Location = m_ptDown;
				m_SelectRect.Width = 0.0f;
				m_SelectRect.Height = 0.0f;
				//m_bDragMode = true;

				m_ScreenRect.Location = e.Location;
				m_ScreenRect.Width = 0;
				m_ScreenRect.Height = 0;

				Invalidate();
			}			
		}
		
		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				m_ptCurrent = ScreenToGlobal(e.Location);


				float x = (float)Math.Min(m_ptDown.X, m_ptCurrent.X);
				float y = (float)Math.Min(m_ptDown.Y, m_ptCurrent.Y);

				m_SelectRect.Location = new PointF(x, y);
				m_SelectRect.Width = Math.Abs(m_ptDown.X - m_ptCurrent.X);
				m_SelectRect.Height = Math.Abs(m_ptDown.Y - m_ptCurrent.Y);



				m_ptScCurrent = e.Location;
				int dx = Math.Min(m_ptScDown.X, m_ptScCurrent.X);
				int dy = Math.Min(m_ptScDown.Y, m_ptScCurrent.Y);

				m_ScreenRect.Location = new Point(dx, dy);
				m_ScreenRect.Width = Math.Abs(m_ptScDown.X - m_ptScCurrent.X);
				m_ScreenRect.Height = Math.Abs(m_ptScDown.Y - m_ptScCurrent.Y);


				Invalidate();
			}
		}

		public void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)	
			{
				m_ptDown = ScreenToGlobal(e.Location);	
				m_ptCurrent = ScreenToGlobal(e.Location);

				m_ptScDown = e.Location;
				m_ptScCurrent = e.Location;

				//m_bDragMode = false;

				if (OnSelectRect != null)
				{
					OnSelectRect(m_SelectRect);
				}

				if (OnSelectScreenRect != null)
				{
					OnSelectScreenRect(m_ScreenRect);
				}

				Invalidate();
			}
		}

		public void OnMouseEnter(object sender, EventArgs e)
		{

		}

		public void OnMouseHover(object sender, EventArgs e)
		{

		}

		public void OnMouseLeave(object sender, EventArgs e)
		{

		}

		private PointF ScreenToGlobal(Point pt)
		{
			Geometry.Vertex2D vert = m_ctrlViewer.ScreenToGlobal(pt.X, pt.Y);
			return new PointF((float)vert.x, (float)vert.y);
		}


		public void DrawSelectRect(PaintEventArgs e)
		{
			//if(m_bDragMode == true)
			{
				Graphics g = e.Graphics;
				if (mRectPen != null)
				{
					if (m_SelectRect.Width < 0.01f || m_SelectRect.Height < 0.01f)
						return;
					g.DrawRectangle(mRectPen, m_SelectRect.X - 2, m_SelectRect.Y - 2, m_SelectRect.Width + 4, m_SelectRect.Height + 4);
				}
			}			
		}
		
		public void Clear()
		{
			m_SelectRect = new RectangleF();
		}
	}
}
