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
using DXFViewer;
using RoadMan;

namespace UnE.Overlay
{
    public enum DrawMode { LINE = 1, FREE_DRAW, RECT, ELLIPSE, TEXT, SELECT };

	public delegate void InvalidateControl();
	public class OverlayPanel
	{

		public event InvalidateControl InvalidateControl;

		public OverlayPanel(DXFControl control)			
		{
			m_ctrlDXF = control;
			InitializeComponent();
			mTextBox.Visible = false;
			PanelDXFViewer panel = (PanelDXFViewer)control.Parent;
			control.Controls.Add(mTextBox);
		}

		public void Focus()
		{
			if(m_ctrlDXF!= null)
			{
				FormMain.Instance.Focus();

				PanelDXFViewer panel = (PanelDXFViewer)m_ctrlDXF.Parent;
				panel.Focus();
			}
			
		}

		private DXFControl m_ctrlDXF;
		public DXFControl TargetControl
		{
			get { return m_ctrlDXF; }
			set { m_ctrlDXF = value; }
		}

		private Timer timer1;		

		private float m_LineThick = 1.0f;
		public float LineThick
		{
			get { return m_LineThick; }
			set { m_LineThick = value; }
		}

		private DrawMode m_DrawMode = DrawMode.LINE;
		public DrawMode DrawMode
		{
			get { return m_DrawMode; }
			set { m_DrawMode = value; }
		}

		private string m_szFontName = "";
		public string FontName
		{
			get { return m_szFontName; }
			set { m_szFontName = value; }
		}

		private int m_nFontSize = 10;
		public int FontSize
		{
			get { return m_nFontSize; }
			set { m_nFontSize = value; }
		}

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

		private bool m_bVisibleOverlay = true;
		public bool VisibleOverlay
		{
			get { return m_bVisibleOverlay; }
			set
			{
				m_bVisibleOverlay = value;
				ClearTempMemo();
				Invalidate();
			}
		}

		private OverlayFreeHands m_tempDraw = null;
		private OverlayLine m_tempLine = null;
		private OverlayRect m_tempRect = null;
		private OverlayOval m_tempOval = null;
		private OverlayText m_tempText = null;

		private ArrayList m_arEntityList = new ArrayList();
		public ArrayList EntityList
		{
			get { return m_arEntityList; }
			set { m_arEntityList = value; }
		}

		private Point m_ptPrev;
		private Point m_ptDown;
		private Point m_ptUp;
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

        private int m_nSnapSize = 3;
        public int SnapSize
        {
            get { return m_nSnapSize; }
            set { m_nSnapSize = value; }
        }

		public void Clear()
		{

			UndoRedoManager.Instance.SaveSnapshot("Clar Overlay");

			CancelTextInput();
			m_arEntityList.Clear();

			m_tempDraw = null;
			m_tempLine = null;
			m_tempRect = null;
			m_tempOval = null;
			m_tempText = null;

			Focus();
		}

		public void Remove(OverlayElement element)
		{
 			if( element != null)
			{
				UndoRedoManager.Instance.SaveSnapshot("Remove Overlay");
				m_arEntityList.Remove(element);
			}
		}	

		private PointF ScreenToGlobal(Point pt)
		{
			Geometry.Vertex2D vert = m_ctrlDXF.ScreenToGlobal(pt.X, pt.Y);
			return new PointF((float)vert.x, (float)vert.y);
		}
		
		

		public void Invalidate()
		{
			if (InvalidateControl != null)
			{
				InvalidateControl();
			}
		}

		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			CancelTextInput();

			if (e.Button == MouseButtons.Left)
			{
				// PickMode
				if (m_DrawMode == Overlay.DrawMode.SELECT)
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


				if (m_DrawMode == DrawMode.LINE)
				{
					m_tempLine = new OverlayLine();
					m_tempLine.LineThick = m_LineThick;
					m_tempLine.LineColor = m_LineColor;
					m_tempLine.Point1 = m_fPtDown;
					m_tempLine.BasePoint = m_fPtDown;
				}

				else if (m_DrawMode == DrawMode.FREE_DRAW)
				{
					m_tempDraw = new OverlayFreeHands();
					m_tempDraw.LineThick = m_LineThick;
					m_tempDraw.LineColor = m_LineColor;
					m_tempDraw.AddPoint(m_fPtDown);
					m_tempDraw.BasePoint = m_fPtDown;
				}
				else if (m_DrawMode == DrawMode.RECT)
				{
					m_tempRect = new OverlayRect();
					m_tempRect.LineThick = m_LineThick;
					m_tempRect.LineColor = m_LineColor;
					m_tempRect.Point1 = m_fPtDown;
					m_tempRect.BasePoint = m_fPtDown;
				}
				else if (m_DrawMode == DrawMode.ELLIPSE)
				{
					m_tempOval = new OverlayOval();
					m_tempOval.LineThick = m_LineThick;
					m_tempOval.LineColor = m_LineColor;
					m_tempOval.Point1 = m_fPtDown;
					m_tempOval.BasePoint = m_fPtDown;					
				}
				else if (m_DrawMode == DrawMode.TEXT)
				{
					

					m_tempText = new OverlayText();
					m_tempText.LineThick = m_LineThick;
					m_tempText.LineColor = m_TextColor;
					m_tempText.Point1 = m_fPtDown;

					m_tempText.FontName = m_szFontName;
					m_tempText.FontStyleBold = m_bFontBold;
					m_tempText.FontStyleItalic = m_bFontItalic;
					m_tempText.FontStyleUnderLine = m_bFontUnderLine;

					PointF ptHeight = ScreenToGlobal(new Point(e.Location.X, e.Location.Y + m_nFontSize));
					float dy = Math.Abs(m_fPtDown.Y - ptHeight.Y);
					m_tempText.FontHeight = dy;

					m_tempText.BasePoint = m_fPtDown;
					Invalidate();
				}
			}		
		}

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			CancelTextInput();	
		}

		public OverlayElement Pick(Point pt)
		{
			OverlayElement target = null;

			/*PointF fPt = ScreenToGlobal(new Point(pt.X - m_nSnapSize, pt.Y - m_nSnapSize));
			PointF fpt2 = ScreenToGlobal(new Point(pt.X + m_nSnapSize, pt.Y + m_nSnapSize));

			float x = Math.Min(fPt.X , fpt2.X);
			float y = Math.Min(fPt.Y , fpt2.Y);

			float width = Math.Abs(fPt.X - fpt2.X);
			float height = Math.Abs(fPt.Y - fpt2.Y);

			RectangleF rect = new RectangleF(x, y, width, height);*/

            foreach (OverlayElement element in m_arEntityList)
            {
                element.Reset();
            }

			foreach (OverlayElement element in m_arEntityList)
			{
                // element의 선두께를 고려하기 위하여 element별로 rect를 따로 구한다.
                RectangleF rect = MakeElementRectangle(element, ref pt);

				if (element.IsPicked(rect))
				{
					target = element;
                    break;
				}
			}

			return target;
		}

        private RectangleF MakeElementRectangle(OverlayElement element, ref Point pt)
        {
            int nSnapSize = m_nSnapSize + (int)element.LineThick;
            PointF fPt = ScreenToGlobal(new Point(pt.X - nSnapSize, pt.Y - nSnapSize));
            PointF fpt2 = ScreenToGlobal(new Point(pt.X + nSnapSize, pt.Y + nSnapSize));

            float x = Math.Min(fPt.X, fpt2.X);
            float y = Math.Min(fPt.Y, fpt2.Y);

            float width = Math.Abs(fPt.X - fpt2.X);
            float height = Math.Abs(fPt.Y - fpt2.Y);

            RectangleF rect = new RectangleF(x, y, width, height);
            return rect;
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

			if (e.Button == MouseButtons.Left)
			{
				m_ptCurrent = e.Location;
				PointF fpt = ScreenToGlobal(m_ptCurrent);

				if (m_DrawMode == DrawMode.LINE)
				{
					m_tempLine.Point2 = fpt;
					Invalidate();
				}
				else if (m_DrawMode == DrawMode.FREE_DRAW)
				{
					m_tempDraw.AddPoint(fpt);
					Invalidate();
				}
				else if (m_DrawMode == DrawMode.RECT)
				{
					m_tempRect.Point2 = fpt;
					Invalidate();
				}
				else if (m_DrawMode == DrawMode.ELLIPSE)
				{
					m_tempOval.Point2 = fpt;
					Invalidate();
				}
				else if( m_DrawMode == DrawMode.TEXT)
				{
					
					m_tempText.Point2 = fpt;
					Invalidate();
				}
			}
		}

		public void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (m_bVisibleOverlay == false)
				return;

			if (m_DrawMode == DrawMode.LINE)
			{
				if (m_tempLine != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("Add Overlay Line");
					m_arEntityList.Add(m_tempLine);
					m_tempLine = null;
				}
			}
			else if (m_DrawMode == DrawMode.FREE_DRAW)
			{
				if (m_tempDraw != null)
				{
					m_tempDraw.Close();
					UndoRedoManager.Instance.SaveSnapshot("Add Overlay Free");
					m_arEntityList.Add(m_tempDraw);
					m_tempDraw = null;
				}
			}
			else if (m_DrawMode == DrawMode.RECT)
			{
				if (m_tempRect != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("Add Overlay Rect");
					m_arEntityList.Add(m_tempRect);
					m_tempRect = null;
				}
			}

			else if( m_DrawMode == DrawMode.ELLIPSE)
			{
				if( m_tempOval != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("Add Overlay Oval");
					m_arEntityList.Add(m_tempOval);
					m_tempOval = null;
				}
			}
			else if (m_DrawMode == DrawMode.TEXT)
			{
				if (m_tempText != null)	
				{
					m_ptUp = e.Location;

					int x = Math.Min(m_ptDown.X, e.Location.X);
					int y = Math.Min(m_ptDown.Y, e.Location.Y);
				

					int width = Math.Abs(e.Location.X - m_ptDown.X);
					int height = Math.Abs(e.Location.Y - m_ptDown.Y);

					if (width < 40)
					{
						width += 40;						
					}

					if (height < 20)
					{
						height += 20;						
					}


					FontStyle fontStyle = FontStyle.Regular;
					if(m_bFontBold)
					{
						fontStyle = FontStyle.Bold;
					}
					if( m_bFontItalic)
					{
						fontStyle |= FontStyle.Italic;
					}
					if (m_bFontUnderLine)
					{
						fontStyle |= FontStyle.Underline;
					}

					if( m_tempText != null)
					{
						PointF pt1 = ScreenToGlobal(new Point(x, y));
						PointF pt2 = ScreenToGlobal(new Point(x + width, y + height));
						m_tempText.Point1 = pt1;
						m_tempText.Point2 = pt2;
					}

					mTextBox.Font = new Font(m_szFontName, m_nFontSize, fontStyle);
					mTextBox.SetBounds(x, y, width, height);
					mTextBox.Location = new Point(x + 1, y+1);
					mTextBox.Text = "";
					mTextBox.Visible = true;
					mTextBox.BackColor = Color.White;
					mTextBox.ForeColor = Color.Black;
                    mTextBox.Multiline = true;
					mTextBox.Focus();


				}
			}

			m_bDragMode = false;
			m_ptPrev = e.Location;
			//m_bTranslateMode = false;
		}

		public void ClearTempMemo()
		{
			m_tempDraw = null;
			m_tempLine = null;
			m_tempRect = null;
			m_tempOval = null;
			m_tempText = null;

			mTextBox.Text = "";
			mTextBox.Visible = false;
		}

		public void CancelTextInput()
		{
			string szText = mTextBox.Text;
			szText = szText.Trim();
			if (szText != "" && m_tempText != null)
			{
				int x = Math.Min(m_ptDown.X, m_ptUp.X);
				int y = Math.Min(m_ptDown.Y, m_ptUp.Y);
				Point ptOrigin = new Point(x, y);
				PointF m_fPtDown = ScreenToGlobal(ptOrigin);
				Point ptNew = new Point(ptOrigin.X, ptOrigin.Y + this.FontSize);
				
				PointF point2 = ScreenToGlobal(ptNew);
				m_tempText.Point1 = m_fPtDown;
				m_tempText.BasePoint = m_fPtDown;
				m_tempText.Point2 = point2;
				m_tempText.Text = szText;


				UndoRedoManager.Instance.SaveSnapshot("Add Overlay Text");
				m_arEntityList.Add(m_tempText);				
			}

			mTextBox.Text = "";
			mTextBox.Visible = false;
			m_tempText = null;

			Focus();
		}

		public void DrawOverlay(System.Windows.Forms.PaintEventArgs e)
		{
			if (m_bVisibleOverlay == false)
				return;

			Graphics g = e.Graphics;
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			try
			{
				//g.Transform = mTransform;

				foreach (OverlayElement element in m_arEntityList)
				{
					element.DrawElement(e);
				}


				if (m_DrawMode == DrawMode.LINE)
				{
					if (m_tempLine != null)
						m_tempLine.TempDrawElement(e);
				}
				else if (m_DrawMode == DrawMode.FREE_DRAW)
				{
					if (m_tempDraw != null)
						m_tempDraw.TempDrawElement(e);
				}
				else if (m_DrawMode == DrawMode.RECT)
				{
					if (m_tempRect != null)
						m_tempRect.TempDrawElement(e);
				}
				else if (m_DrawMode == DrawMode.ELLIPSE)
				{
					if (m_tempOval != null)
					{
						m_tempOval.TempDrawElement(e);
					}
				}
				else if (m_DrawMode == DrawMode.TEXT)
				{
					if (m_tempText != null)
						m_tempText.TempDrawElement(e);

					if (mTextBox.Visible == true)
					{
						Rectangle rect = mTextBox.Bounds;
						Rectangle rectDraw = new Rectangle(rect.X - 3, rect.Y - 3, rect.Width + 6, rect.Height + 6);

						using (Pen pen = new Pen(Color.Gray))
						{
							pen.Width = 2.0F;
							pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
							pen.DashStyle = DashStyle.Dash;
							//g.Transform.Scale(1.0f, -1.0f);
							e.Graphics.DrawRectangle(pen, rectDraw);
							//g.Transform.Scale(1.0f, -1.0f);
						}
					}
					
				}
			}
			catch(Exception)
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
		}

		private void mTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
            // MultiLine 지원을 위하여 막는다. 
			/*if(e.KeyData == Keys.Enter)
			{
				string szText = mTextBox.Text;
				szText = szText.Trim();
				if( szText != null && mTextBox.Text != "")
				{

					int x = Math.Min(m_ptDown.X, m_ptUp.X);
					int y = Math.Min(m_ptDown.Y, m_ptUp.Y);
					Point ptOrigin = new Point(x, y);
					PointF m_fPtDown = ScreenToGlobal(ptOrigin);
					Point ptNew = new Point(ptOrigin.X, ptOrigin.Y + this.FontSize);
					PointF point2 = ScreenToGlobal(ptNew);
					m_tempText.Point1 = m_fPtDown;
					m_tempText.BasePoint = m_fPtDown;
					m_tempText.Point2 = point2;
					m_tempText.Text = szText;
					m_arEntityList.Add(m_tempText);
					m_tempText = null;

					mTextBox.Visible = false;
					Invalidate();
					Focus();
				}
				
			}*/
			if (e.KeyData == Keys.Escape)
			{
				m_tempText = null;
				CancelTextInput();
				Invalidate();
			}
		}		
	}
}
