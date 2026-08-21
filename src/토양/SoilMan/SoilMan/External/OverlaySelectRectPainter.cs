using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SoilMan.Overlay
{
	public delegate void OnSelectRect(RectangleF rect);
	public delegate void OnSelectScreenRect(Rectangle rect);
    public delegate void InvalidateControl();

	public class OverlaySelectRectPainter
	{
		public event InvalidateControl InvalidateControl;

		public event OnSelectRect OnSelectRect;

		public event OnSelectScreenRect OnSelectScreenRect;

		private DXFViewer.DXFControl m_ctrlViewer = null;
		public OverlaySelectRectPainter(DXFViewer.DXFControl control, Color rectColor)
		{
			m_ctrlViewer = control;

            RectColor = rectColor;
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
                mRectPen.Width = 0;
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
		private bool m_bDragMode = false;

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
				m_bDragMode = true;

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
                
				Invalidate();
			}
		}

		public void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)	
			{	
				m_ptCurrent = ScreenToGlobal(e.Location);

				m_ptScDown = e.Location;
				m_ptScCurrent = e.Location;

				m_bDragMode = false;

				if (OnSelectRect != null)
				{
					OnSelectRect(m_SelectRect);
				}

				if (OnSelectScreenRect != null)
				{
					OnSelectScreenRect(m_ScreenRect);
				}

                float left = m_ptDown.X, right = m_ptDown.X;
                float top = m_ptDown.Y, bottom = m_ptDown.Y;

                if (left > m_ptCurrent.X)
                    left = m_ptCurrent.X;
                if (right < m_ptCurrent.X)
                    right = m_ptCurrent.X;
                if (top < m_ptCurrent.Y)
                    top = m_ptCurrent.Y;
                if (bottom > m_ptCurrent.Y)
                    bottom = m_ptCurrent.Y;

                UnE.Geometry.Vertex2D vMoved = FormMain.Instance.DxfControl.MovedVertex;
                FormMain.Instance.OnSelectArea(left - (float)vMoved.x, top - (float)vMoved.y, right - (float)vMoved.x, bottom - (float)vMoved.y);

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
			UnE.Geometry.Vertex2D vert = m_ctrlViewer.ScreenToGlobal(pt.X, pt.Y);
			return new PointF((float)vert.x, (float)vert.y);
		}


		public void DrawSelectRect(PaintEventArgs e)
		{
			if(m_bDragMode == true)
			{
				Graphics g = e.Graphics;
                
				if (mRectPen != null)
				{
					if (m_SelectRect.Width < 0.01f || m_SelectRect.Height < 0.01f)
						return;
					g.DrawRectangle(mRectPen, m_SelectRect.X, m_SelectRect.Y, m_SelectRect.Width, m_SelectRect.Height);
				}
			}			
		}
	}
}
