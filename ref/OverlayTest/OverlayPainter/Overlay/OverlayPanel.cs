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
	public class OverlayPanel : Panel
	{

		public OverlayPanel()
			: base()
		{
			this.DoubleBuffered = true;
			InitializeComponent();

			mTextBox.Visible = false;
			this.Controls.Add(mTextBox);
		}

		private Timer timer1;		

		private float m_LineThick = 1.0f;
		public float LineThick
		{
			get { return m_LineThick; }
			set { m_LineThick = value; }
		}

		private int m_DrawMode = 1;
		public int DrawMode
		{
			get { return m_DrawMode; }
			set { m_DrawMode = value; }
		}

		private OverlayFreeHands m_tempDraw = null;
		private OverlayLine m_tempLine = null;
		private OverlayRect m_tempRect = null;
		private OverlayOval m_tempOval = null;
		private OverlayText m_tempText = null;

		private ArrayList m_arEntityList = new ArrayList();

		private Point m_ptPrev;
		private Point m_ptDown;
		private Point m_ptCurrent;


		private OverlayElement m_SelectObject = null;
		public OverlayElement SelectObject
		{
			get { return m_SelectObject; }
			set { m_SelectObject = value; }
		}

		private bool m_bDragMode = false;

		
		private Color m_LineColor = Color.Red;
		
		private IContainer components;
		private TextBox mTextBox;
	
		public Color LineColor
		{
			get { return m_LineColor; }
			set { m_LineColor = value; }
		}

		private Color m_TextColor = Color.Black;
		public Color TextColor
		{
			get { return m_TextColor; }
			set { m_TextColor = value; }
		}

		public void Clear()
		{
			CancelTextInput();
			m_arEntityList.Clear();

			m_tempDraw = null;
			m_tempLine = null;
			m_tempRect = null;
			m_tempOval = null;
			m_tempText = null;

		}

		public void Remove(OverlayElement element)
		{
 			if( element != null)
			{
				m_arEntityList.Remove(element);
			}
		}	

		private PointF ScreenToGlobal(Point pt)
		{
			Matrix mTemp = mTransform.Clone();
			mTemp.Invert();

			PointF ff = new PointF(pt.X, pt.Y);

			PointF[] myArray =
            {
                ff
            };
			mTemp.TransformPoints(myArray);

			float x = myArray[0].X;
			float y = myArray[0].Y;

			return new PointF(x, y);
		}


		private Matrix mTransform = new Matrix();
		private bool m_bTranslateMode = false;
		private float m_Scale = 1.0f;

		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			CancelTextInput();

			if (e.Button == MouseButtons.Left)
			{
				// PickMode
				if (m_DrawMode == 6)
				{				
					OverlayElement element = Pick(e.Location);
					if (element != null)
					{
						element.HighLight();

					}
					m_SelectObject = element;
					Invalidate();
					return;
				}

				m_ptDown = e.Location;
				m_ptPrev = e.Location;
				m_bDragMode = true;

				PointF m_fPtDown = ScreenToGlobal(m_ptDown);


				if (m_DrawMode == 1)
				{
					m_tempLine = new OverlayLine();
					m_tempLine.LineThick = m_LineThick;
					m_tempLine.LineColor = m_LineColor;
					m_tempLine.Point1 = m_fPtDown;
					m_tempLine.BasePoint = m_fPtDown;
				}

				else if (m_DrawMode == 2)
				{
					m_tempDraw = new OverlayFreeHands();
					m_tempDraw.LineThick = m_LineThick;
					m_tempDraw.LineColor = m_LineColor;
					m_tempDraw.AddPoint(m_fPtDown);
					m_tempDraw.BasePoint = m_fPtDown;
				}
				else if (m_DrawMode == 3)
				{
					m_tempRect = new OverlayRect();
					m_tempRect.LineThick = m_LineThick;
					m_tempRect.LineColor = m_LineColor;
					m_tempRect.Point1 = m_fPtDown;
					m_tempRect.BasePoint = m_fPtDown;
				}
				else if (m_DrawMode == 4)
				{
					m_tempOval = new OverlayOval();
					m_tempOval.LineThick = m_LineThick;
					m_tempOval.LineColor = m_LineColor;
					m_tempOval.Point1 = m_fPtDown;
					m_tempOval.BasePoint = m_fPtDown;					
				}
				else if (m_DrawMode == 5)
				{
					m_tempText = new OverlayText();
					m_tempText.LineThick = m_LineThick;
					m_tempText.LineColor = m_TextColor;
					m_tempText.Point1 = m_fPtDown;
					m_tempText.BasePoint = m_fPtDown;
					m_tempText.ObjectScale = m_Scale;

					
					//Invalidate();
				}
			}
			if (e.Button == MouseButtons.Middle)
			{
				m_bDragMode = true;
				m_ptPrev = e.Location;
				m_ptDown = e.Location;
				m_bTranslateMode = true;
			}	
		}

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			CancelTextInput();

			//Point pt = e.Location;
			PointF pt = ScreenToGlobal(e.Location);	
			if (e.Delta > 0)
			{				
				mTransform.Translate(pt.X, pt.Y);
				mTransform.Scale(0.9f, 0.9f);
				mTransform.Translate(-pt.X, -pt.Y);
				m_Scale *= 0.75f;				
			}
			else
			{
				double d = 100.0 / 75.0;				
				mTransform.Translate(pt.X, pt.Y);
				mTransform.Scale(1.1f, 1.1f);
				mTransform.Translate(-pt.X, -pt.Y);

				m_Scale *= (float)d;
				
			}

			
		
			Invalidate();
		}

		public OverlayElement Pick(Point pt)
		{
			OverlayElement target = null;

			PointF fPt = ScreenToGlobal(pt);

			foreach (OverlayElement element in m_arEntityList)
			{
				if (element.IsPicked(fPt))
				{
					target = element;
				}
				element.Reset();
			}
			return target;
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

		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (m_bDragMode == false)
				return;
			
			

			if (m_bTranslateMode == true)
			{	
				PointF prevPt = ScreenToGlobal(m_ptPrev);
				PointF fpt = ScreenToGlobal(e.Location);	
				float dx = prevPt.X - fpt.X;
				float dy = prevPt.Y - fpt.Y;		

				mTransform.Translate(-dx, -dy);
				Invalidate();
				m_ptPrev = e.Location;
			}

			if (e.Button == MouseButtons.Left)
			{
				m_ptCurrent = e.Location;
				PointF fpt = ScreenToGlobal(m_ptCurrent);

				if (m_DrawMode == 1)
				{
					m_tempLine.Point2 = fpt;
					Invalidate();
				}
				else if (m_DrawMode == 2)
				{
					m_tempDraw.AddPoint(fpt);
					Invalidate();
				}
				else if (m_DrawMode == 3)
				{
					m_tempRect.Point2 = fpt;
					Invalidate();
				}
				else if (m_DrawMode == 4)
				{
					m_tempOval.Point2 = fpt;
					Invalidate();
				}
				else if( m_DrawMode == 5)
				{
					m_tempText.Point2 = fpt;
					Invalidate();
				}
			}
			

		}

		public void OnMouseUp(object sender, MouseEventArgs e)
		{
		

			if (m_DrawMode == 1)
			{
				if (m_tempLine != null)
				{
					m_arEntityList.Add(m_tempLine);
					m_tempLine = null;
				}
			}
			else if (m_DrawMode == 2)
			{
				if (m_tempDraw != null)
				{
					m_tempDraw.Close();
					m_arEntityList.Add(m_tempDraw);
					m_tempDraw = null;
				}
			}
			else if (m_DrawMode == 3)
			{
				if (m_tempRect != null)
				{
					
					m_arEntityList.Add(m_tempRect);
					m_tempRect = null;
				}
			}

			else if( m_DrawMode == 4)
			{
				if( m_tempOval != null)
				{
					m_arEntityList.Add(m_tempOval);
					m_tempOval = null;
				}
			}
			else if (m_DrawMode == 5)
			{
				if (m_tempText != null)	
				{				

					int width = Math.Abs(e.Location.X - m_ptDown.X);
					int height = Math.Abs(e.Location.Y - m_ptDown.Y);
					
					mTextBox.SetBounds(m_ptDown.X, m_ptDown.Y, width, height);
					mTextBox.Location = new Point(m_ptDown.X + 1, m_ptDown.Y+1);
					mTextBox.Text = "";
					mTextBox.Visible = true;
					mTextBox.BackColor = this.BackColor;
					mTextBox.ForeColor = m_TextColor;
					mTextBox.Focus();
				}
			}

			m_bDragMode = false;
			m_ptPrev = e.Location;
			m_bTranslateMode = false;
		}

		public void CancelTextInput()
		{			
			mTextBox.Text = "";
			mTextBox.Visible = false;
			m_tempText = null;
		}

		public void OnPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			try
			{
				e.Graphics.Transform = mTransform;

				foreach (OverlayElement element in m_arEntityList)
				{
					element.DrawElement(e);
				}


				if (m_DrawMode == 1)
				{
					if (m_tempLine != null)
						m_tempLine.TempDrawElement(e);
				}
				else if (m_DrawMode == 2)
				{
					if (m_tempDraw != null)
						m_tempDraw.TempDrawElement(e);
				}
				else if (m_DrawMode == 3)
				{
					if (m_tempRect != null)
						m_tempRect.TempDrawElement(e);
				}
				else if (m_DrawMode == 4)
				{
					if (m_tempOval != null)
					{
						m_tempOval.TempDrawElement(e);
					}
				}
				else if (m_DrawMode == 5)
				{
					if (m_tempText != null)
						m_tempText.TempDrawElement(e);

					if (mTextBox.Visible == true)
					{
						Rectangle rect = mTextBox.Bounds;
						Rectangle rectDraw = new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);

						using (Pen pen = new Pen(Color.Gray))
						{
							pen.Width = 2.0F;
							pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
							pen.DashStyle = DashStyle.Dash;

							e.Graphics.DrawRectangle(pen, rectDraw);
						}
					}
					
				}
			}
			catch(Exception ex)
			{
				int i = 0;
				i++;
			}
			
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.mTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// mTextBox
			// 
			this.mTextBox.BackColor = System.Drawing.Color.White;
			this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.mTextBox.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.mTextBox.Location = new System.Drawing.Point(0, 0);
			this.mTextBox.Multiline = true;
			this.mTextBox.Name = "mTextBox";
			this.mTextBox.Size = new System.Drawing.Size(100, 21);
			this.mTextBox.TabIndex = 0;
			this.mTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mTextBox_PreviewKeyDown);
			this.ResumeLayout(false);

		}

		private void mTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if(e.KeyData == Keys.Enter)
			{
				string szText = mTextBox.Text;
				if( szText != null && mTextBox.Text != "")
				{					
					PointF m_fPtDown = ScreenToGlobal(m_ptDown);
					Point ptNew = new Point(m_ptDown.X, m_ptDown.Y + 10);
					PointF point2 = ScreenToGlobal(ptNew);
					m_tempText.Point2 = point2;
					m_tempText.Text = szText;
					m_arEntityList.Add(m_tempText);
					m_tempText = null;

					mTextBox.Visible = false;
					Invalidate();
				}
				
			}
		}		
	}
}
