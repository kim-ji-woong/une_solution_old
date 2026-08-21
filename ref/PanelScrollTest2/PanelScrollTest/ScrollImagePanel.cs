using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;


namespace Sections
{
    public class ScrollImagePanel : Panel
    {
        public enum PanelMode
        {
            NONE,
            PICK,
            ADD
        }

        // ScrollBar AutoPoistion을 위한 Hidden Control
        protected PictureBox m_PositionBox = null;

        private ArrayList m_arRects = new ArrayList();

        
        /// <summary>
        /// Panel이 늘어날 수 있는 최대 Width
        /// </summary>
        protected int m_MaxWidth = 1920;
        public int MaxWidth
        {
            get { return m_MaxWidth; }
            set { m_MaxWidth = value; }
        }

        /// <summary>
        /// Panel이 늘어날 수 있는 최대 Height
        /// </summary>
        protected int m_MaxHeight = 8000;
        public int MaxHeight
        {
            get { return m_MaxHeight; }
            set { m_MaxHeight = value; }
        }

        /// <summary>
        /// Panel이 줄어들 수 있는 최소 Width
        /// </summary>
        protected int m_MinWidth = 300;
        public int MinWidth
        {
            get { return m_MinWidth; }
            set { m_MinWidth = value; }
        }

        /// <summary>
        /// Panel이 줄어들 수 있는 최소 Height
        /// </summary>
        protected int m_MinHeight = 200;
        public int MinHeight
        {
            get { return m_MinHeight; }
            set { m_MinHeight = value; }
        }

        private PanelMode m_Mode = PanelMode.NONE;
        protected PanelMode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }        

        /// <summary>
        /// Panel에 Draw되는 Object의 전체 Bound , 최초는 Panel의 크기,
        /// Object가 Add될때 마다 자동 증가
        /// </summary>
        protected DrawableBound m_PanelBound = null;

        private ContextMenuStrip m_PopupMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItem삭제;

        public ScrollImagePanel()
        {            
            this.DoubleBuffered = true;
            
            InitializeComponent();

            m_PanelBound = new DrawableBound();
            m_PanelBound.Rect = this.Bounds;
            m_PanelBound.Location = new Point(0, 0);
        }       

        private void InitializeComponent()
        {  
            this.components = new System.ComponentModel.Container();
            this.m_PositionBox = new System.Windows.Forms.PictureBox();
            this.m_PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem삭제 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.m_PositionBox)).BeginInit();
            this.m_PopupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_PositionBox
            // 
            this.m_PositionBox.Location = new System.Drawing.Point(0, 0);
            this.m_PositionBox.Name = "m_PositionBox";
            this.m_PositionBox.Size = new System.Drawing.Size(80, 80);
            this.m_PositionBox.TabIndex = 0;
            this.m_PositionBox.TabStop = false;
            this.m_PositionBox.Visible = false;
            // 
            // m_PopupMenu
            // 
            this.m_PopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem삭제});
            this.m_PopupMenu.Name = "m_PopupMenu";
            this.m_PopupMenu.Size = new System.Drawing.Size(99, 26);
            this.m_PopupMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.m_PopupMenu_Closing);
            this.m_PopupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.m_PopupMenu_Opening);
            // 
            // toolStripMenuItem삭제
            // 
            this.toolStripMenuItem삭제.Name = "toolStripMenuItem삭제";
            this.toolStripMenuItem삭제.Size = new System.Drawing.Size(98, 22);
            this.toolStripMenuItem삭제.Text = "삭제";
            this.toolStripMenuItem삭제.Click += toolStripMenuItem삭제_Click;
            // 
            // ImagePanel
            // 
            this.Controls.Add(this.m_PositionBox);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImagePanel_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ImagePanel_MouseClick);
            this.Scroll += OnScroll;
            this.Resize += new System.EventHandler(this.ImagePanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.m_PositionBox)).EndInit();
            this.m_PopupMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        void OnScroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        float m_fTranY = 0.0f;
        float m_fTranX = 0.0f;

        public virtual PointF ScreenToGlobal(Point pt)
        {
            float dx = 0;
            float dy = 0;

            float gx = (pt.X / m_fScale - dx);
            float gy = (pt.Y / m_fScale - dy);

            return new PointF(gx, gy);
        }

        public virtual Point GlobalToScreen(PointF pt)
        {
            int x = (int)(pt.X * m_fScale + m_fTranX);
            int y = (int)(pt.Y * m_fScale + m_fTranY);
            return new Point(x, y);
        }


        public void InitPoistion()
        {
            m_PanelBound.Size = this.ClientSize;
            int x = ClientSize.Width / 2;
            int y = ClientSize.Height / 2;
            AdjustScrollPosition(x, y);
        }
                
        private void ImagePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.ResetTransform();


            g.ScaleTransform(m_fScale, m_fScale);
            ArrayList arDraws = (ArrayList)m_arRects.Clone();
            foreach (DrawableRect rect in arDraws)
            {
                rect.OnDraw(g, e.ClipRectangle);                
            }
        }

        private float m_fScale = 1.0f;
        
        private Image m_BackBuffer1 = null;
        private Image m_BackBuffer2 = null;

        private void ImagePanel_Resize(object sender, EventArgs e)
        {
            if( this.Size.Width == 0 || this.Size.Height == 0)
            {
                return;
            }

            if( this.Size.Width < m_PanelBound.Rect.Width)
            {
                this.Width = m_PanelBound.Rect.Width;
            }
            if (this.Size.Height < m_PanelBound.Rect.Height)
            {
                this.Height = m_PanelBound.Rect.Height;
            }



            int width = DisplayRectangle.Width;
            int height = DisplayRectangle.Height;
            if (m_BackBuffer1 != null)
            {
                Image temp = m_BackBuffer1;
                m_BackBuffer1 = null;
                temp.Dispose();
            }
            m_BackBuffer1 = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            if (m_BackBuffer2 != null)
            {
                Image temp = m_BackBuffer2;
                m_BackBuffer2 = null;
                temp.Dispose();
            }
            m_BackBuffer2 = new Bitmap(width, height);
        }

        private void AdjustScrollPosition(int x, int y)
        {
            m_PositionBox.Location = new Point(x - 40, y - 40);
            Panel pane = (Panel)this.Parent;
            if (pane != null)
            {
                pane.ScrollControlIntoView(m_PositionBox);
                pane.PerformLayout();
            }
        }

        public void AddRect(int x, int y)
        {
            Rectangle rectBtn = new Rectangle();
            rectBtn.Location = new Point(x - 40, y - 40);
            rectBtn.Size = new Size(80, 80);

            AddRect(rectBtn);           
        }

        public void AddRect(Rectangle rectBtn)
        {
            Rectangle rect = ClientRectangle;
            rect = Rectangle.Union(rect, rectBtn);

            int dx = 0;
            if (rect.X < 0)
                dx = Math.Abs(rect.X);

            int dy = 0;
            if (rect.Y < 0)
                dy = Math.Abs(rect.Y);
            
            int width = rect.Width;
            int height = rect.Height;

            if( width > m_MaxWidth)
            {
                width = m_MaxWidth;
                dx = 0;
            }
            if( height > m_MaxHeight)
            {
                height = m_MaxHeight;
                dy = 0;
            }

            m_PanelBound.Size = new Size(width, height);
        
            ClientSize = new Size(width, height);
            if ((dx != 0 || dy != 0))
            {
                for(int i = 0 ; i < m_arRects.Count ; i++)
                {
                    DrawableRect t = (DrawableRect)m_arRects[i];
                    t.Location = new Point(t.Location.X + dx, t.Location.Y + dy);
                }
            }

            DrawableRect added = new DrawableRect();
            added.Rect = rectBtn;
            m_arRects.Add(added);
        }
        
        //private void AddSizeControl(int x, int y)
        //{
        //    Panel btn = new Panel();
        //    btn.AutoSize = false;
        //    btn.BackColor = Color.White;
        //    btn.Visible = false;

        //    btn.Location = new Point(x - 40, y - 40);
        //    btn.Size = new Size(80, 80);

        //    Controls.Add(btn);

        //    Rectangle rectBtn = btn.Bounds;
        //    Rectangle rect = ClientRectangle;
        //    rect = Rectangle.Union(rect, rectBtn);

        //    int dx = 0;
        //    if (rect.X < 0)
        //        dx = Math.Abs(rect.X);

        //    int dy = 0;
        //    if (rect.Y < 0)
        //        dy = Math.Abs(rect.Y);

        //    ClientSize = new Size(rect.Width, rect.Height);

        //    if (dx != 0 || dy != 0)
        //    {
        //        foreach (Control c in Controls)
        //        {
        //            c.Location = new Point(c.Location.X + dx, c.Location.Y + dy);
        //        }
        //    }

        //    btn.Select();

        //    Panel pane = (Panel)this.Parent;
        //    if (pane != null)
        //    {
        //        pane.ScrollControlIntoView(btn);
        //        pane.PerformLayout();
        //    }           
        //}        

        private void ImagePanel_MouseClick(object sender, MouseEventArgs e)
        {
            //if (m_Mode == PanelMode.ADD)
            {
                if( e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    DrawableRect rect = PickRect(e.X, e.Y);
                    if (rect != null)
                    {
                        DrawableRect prevRect = (DrawableRect)m_PopupMenu.Tag;
                        if( prevRect != null)
                        {
                            if(prevRect.Selected == true)
                            {
                                prevRect.Selected = false;
                                Invalidate(prevRect.Rect);
                            }
                        }
                        rect.Selected = true;
                        Invalidate(rect.Rect);

                        Point pt = PointToScreen(e.Location);
                        m_PopupMenu.Tag = rect;  
                        m_PopupMenu.Show(pt);
                    }
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Left && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    PointF ptGlobal = ScreenToGlobal(e.Location);

                    int x = (int)ptGlobal.X;
                    int y = (int)ptGlobal.Y;
                    AddRect(x, y);
                    AdjustScrollPosition(x, y);
                    Invalidate();                    
                }
                else if( e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    DrawableRect rect = PickRect(e.X, e.Y);
                    if (rect != null)
                    {
                        rect.Selected = !rect.Selected;                        
                        Invalidate();
                    }
                }
            }
        }

        public void ZoomIn(int zoomValue = 1)
        {
            m_fScale *= 1.1f;
            this.ScaleControl(new SizeF(1.1f, 1.1f), BoundsSpecified.Size);


            //int x = Bounds.X + Bounds.Width / 2;
            //int y = Bounds.Y + Bounds.Height / 2;

            //AdjustScrollPosition(x, y);

            Invalidate();
        }

        public void ZoomOut(int zoomValue = 1)
        {
            m_fScale *= 0.9f;
            this.ScaleControl(new SizeF(0.9f, 0.9f), BoundsSpecified.Size);

            //int x = Bounds.X + Bounds.Width / 2;
            //int y = Bounds.Y + Bounds.Height / 2;

            //AdjustScrollPosition(x, y);

            Invalidate();
        }

        protected void DeleteRect(DrawableRect rect)
        {
            m_arRects.Remove(rect);
        }

        public DrawableRect DeleteRect(int x, int y)
        {
            return null;
        }

        void toolStripMenuItem삭제_Click(object sender, EventArgs e)
        {
            DrawableRect rect = (DrawableRect)m_PopupMenu.Tag;
            if (rect != null)
            {
                DeleteRect(rect);
            }

            Invalidate();
        }


        protected DrawableRect PickRect(int x, int y)
        {
            ArrayList arRect = (ArrayList)m_arRects.Clone();
            arRect.Reverse();

            foreach (DrawableRect rect in arRect)
            {
                if (rect.IsPick(x, y))
                    return rect;
            }
            return null;
        }

        private void m_PopupMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void m_PopupMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            DrawableRect rect = (DrawableRect)m_PopupMenu.Tag;
            if( rect != null)
            {
                rect.Selected = false;
                Invalidate(rect.Rect);
            }
        }
    }

    public class DrawableBound
    {
        private Rectangle ret = new Rectangle();
        public Rectangle Rect
        {
            get { return ret; }
            set { ret = value; }
        }

        public Point Location
        {
            get { return ret.Location; }
            set
            {
                ret.Location = value;
            }
        }

        public Size Size
        {
            get { return ret.Size; }
            set
            {
                ret.Size = value;
            }
        }
    }

    public class DrawableRect
    {

        private Color m_SelectedColor = Color.Yellow;
        private Color m_NormalColor = Color.Red;

        protected SolidBrush brush = new SolidBrush(Color.Red);


        private Rectangle rect = new Rectangle();
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public Point Location
        {
            get { return rect.Location; }
            set
            {
                rect.Location = value;
            }
        }

        public Size Size
        {
            get { return rect.Size; }
            set
            {
                rect.Size = value;
            }
        }

        private bool m_bSelected = false;
        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        public bool IsPick(Point pt)
        {
            if(rect.Contains(pt))
            {
                return true;
            }
            return false;
        }

        public bool IsPick(int x, int y)
        {
            return IsPick(new Point(x, y));
        }

        public virtual void OnDraw(Graphics g, Rectangle clipRect)
        {
            if (clipRect.IntersectsWith(rect))
            {
                if (m_bSelected == true)
                {
                    brush.Color = m_SelectedColor;
                    g.FillRectangle(brush, rect);
                }
                else
                {
                    brush.Color = m_NormalColor;
                    g.FillRectangle(brush, rect);
                }
            }
        }        
    }
}
