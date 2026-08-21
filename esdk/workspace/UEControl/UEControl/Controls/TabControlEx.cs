using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UnE.Controls
{

    public delegate void TabPageDeleted(object sender, TabControlExEventArgs e);
    public delegate void TabPageDeleting(object sender, TabControlExEventArgs e);
	public delegate void TabDoubleClicked(object sender, TabControlExEventArgs e);
    public delegate void TabMouseDown(object sender, TabControlMouseEventArgs e);
    public delegate void TabMouseUp(object sender, TabControlMouseEventArgs e);
    public class TabControlEx : System.Windows.Forms.TabControl
    {
        [Browsable(true), Description("탭이 삭제된 후에 호출됩니다.")]
        public event TabPageDeleted OnTabPageDeleted;
        [Browsable(true), Description("탭이 삭제되기 전에 호출됩니다.")]
        public event TabPageDeleting OnTabPageDeleting;
		[Browsable(true), Description("탭을 더블클릭 하는경우 호출됩니다.")]
		public event TabDoubleClicked OnTabDoubleClicked;
        [Browsable(true), Description("탭을 마우스로 눌렀을 경우 호출됩니다.")]
        public event TabMouseDown OnTabMouseDown;
        [Browsable(true), Description("탭을 마우스로 눌렀다 뗐을 경우 호출됩니다.")]
        public event TabMouseUp OnTabMouseUp;

        private int _hotTabIndex = -1;

        public TabControlEx()
            : base()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            TabBackColor = m_BackColor;
        }

        #region Properties

        protected Color m_ColorSelectedTab = Color.DarkGray;
        public Color SelectedTabColor
        {
            get { return m_ColorSelectedTab; }
            set { m_ColorSelectedTab = value; }
        }
        protected Color m_BackColor = Color.FromArgb(60, 56, 71);		
        public Color TabBackColor
        {
            get { return m_BackColor; }
            set
            {
                m_BackColor = value;
                base.BackColor = m_BackColor;
            }
        }
        protected Color m_foreColor = Color.White;
        public Color TabForeColor
        {
            get
            {
                return m_foreColor;
            }
            set
            {
                m_foreColor = value;
            }
        }

        protected Color m_disableColor = Color.DarkGray;
        public Color TabDisabledForeColor
        {
            get
            {
                return m_disableColor;
            }
            set
            {
                m_disableColor = value;
            }
        }
        public override Color BackColor
        {
            get
            {
                return m_BackColor;
            }
            set
            {
                m_BackColor = value;
            }
        }


        private int CloseButtonHeight
        {
            get { return FontHeight; }
        }

        private int HotTabIndex
        {
            get { return _hotTabIndex; }
            set
            {
                if (_hotTabIndex != value)
                {
                    _hotTabIndex = value;
                    this.Invalidate();
                }
            }
        }

        protected bool m_bUseCloseButton = true;
        public bool UseCloseButton
        {
            get { return m_bUseCloseButton; }
            set { m_bUseCloseButton = value; }
        }


        #endregion

        #region Overridden Methods

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnFontChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            IntPtr hFont = this.Font.ToHfont();
            SendMessage(this.Handle, WM_SETFONT, hFont, new IntPtr(-1));
            SendMessage(this.Handle, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
            this.UpdateStyles();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TCHITTESTINFO HTI = new TCHITTESTINFO(e.X, e.Y);
            HotTabIndex = SendMessage(this.Handle, TCM_HITTEST, IntPtr.Zero, ref HTI);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            HotTabIndex = -1;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            for (int id = 0; id < this.TabCount; id++)
                DrawTabBackground(pevent.Graphics, id);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int id = 0; id < this.TabCount; id++)
                DrawTabContent(e.Graphics, id);
        }

        private bool m_bDownCloseBtn = false;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_SETPADDING)
                m.LParam = MAKELPARAM(this.Padding.X + CloseButtonHeight / 2, this.Padding.Y);
			if (m.Msg == WM_LBUTTONDBLCLK && !this.DesignMode)
			{
				if(OnTabDoubleClicked!= null)
				{
					TabPage page = TabPages[HotTabIndex];
					TabControlExEventArgs arg = new TabControlExEventArgs();
					arg.DeletePage = page;
					OnTabDoubleClicked(this, arg);
				}
			}
            if (m.Msg == WM_LBUTTONDOWN && !this.DesignMode)
            {
                Point pt = this.PointToClient(Cursor.Position);

                if(m_bUseCloseButton == true)
                {
                    //Point pt = this.PointToClient(Cursor.Position);
                    if (HotTabIndex == -1)
                    {
                        m_bDownCloseBtn = false;
                        m.Msg = WM_NULL;
                        return;
                    }
                    Rectangle closeRect = GetCloseButtonRect(HotTabIndex);
                    if (closeRect.Contains(pt))
                    {
                        m_bDownCloseBtn = true;
                        m.Msg = WM_NULL;
                    }
                }
                
                if (OnTabMouseDown != null && HotTabIndex != -1)
                {
                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Left;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseDown(this, arg);
                }
            }
            else if (m.Msg == WM_RBUTTONDOWN && !this.DesignMode)
            {
                if (OnTabMouseDown != null && HotTabIndex != -1)
                {
                    Point pt = this.PointToClient(Cursor.Position);

                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Right;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseDown(this, arg);
                }
            }
            else if (m.Msg == WM_MBUTTONDOWN && !this.DesignMode)
            {
                if (OnTabMouseDown != null && HotTabIndex != -1)
                {
                    Point pt = this.PointToClient(Cursor.Position);

                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Middle;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseDown(this, arg);
                }
            }
            else if (m.Msg == WM_LBUTTONUP && !this.DesignMode)
            {
                Point pt = this.PointToClient(Cursor.Position);

                if (m_bUseCloseButton == true)
                {
                    //Point pt = this.PointToClient(Cursor.Position);
                    if (HotTabIndex == -1)
                    {
                        m_bDownCloseBtn = false;
                        m.Msg = WM_NULL;
                        return;
                    }

                    Rectangle closeRect = GetCloseButtonRect(HotTabIndex);
                    if (closeRect.Contains(pt) && m_bDownCloseBtn == true)
                    {
                        TabPage page = TabPages[HotTabIndex];
                        if (OnTabPageDeleting != null)
                        {
                            TabControlExEventArgs arg = new TabControlExEventArgs();
                            arg.DeletePage = page;
                            OnTabPageDeleting(this, arg);
                        }

                        TabPages.Remove(page);

                        if (OnTabPageDeleted != null)
                        {
                            TabControlExEventArgs arg = new TabControlExEventArgs();
                            arg.DeletePage = page;
                            OnTabPageDeleted(this, arg);
                        }
                        m.Msg = WM_NULL;
                    }
                }
                m_bDownCloseBtn = false;

                if (OnTabMouseUp != null && HotTabIndex != -1)
                {
                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Left;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseUp(this, arg);
                }
            }
            else if (m.Msg == WM_RBUTTONUP && !this.DesignMode)
            {
                if (OnTabMouseUp != null && HotTabIndex != -1)
                {
                    Point pt = this.PointToClient(Cursor.Position);

                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Right;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseUp(this, arg);
                }
            }
            else if (m.Msg == WM_MBUTTONUP && !this.DesignMode)
            {
                if (OnTabMouseUp != null && HotTabIndex != -1)
                {
                    Point pt = this.PointToClient(Cursor.Position);

                    TabPage page = TabPages[HotTabIndex];
                    TabControlMouseEventArgs arg = new TabControlMouseEventArgs();
                    arg.Button = System.Windows.Forms.MouseButtons.Middle;
                    arg.Page = page;
                    arg.Point = pt;
                    OnTabMouseUp(this, arg);
                }
            }
            base.WndProc(ref m);
        }

        #endregion

        #region Private Methods

        private IntPtr MAKELPARAM(int lo, int hi)
        {
            return new IntPtr((hi << 16) | (lo & 0xFFFF));
        }

        private void DrawTabBackground(Graphics graphics, int id)
        {
            if (id == SelectedIndex)
            {
                using (SolidBrush brush = new SolidBrush(m_ColorSelectedTab))
                    graphics.FillRectangle(brush, GetTabRect(id));
            }
            else if (id == HotTabIndex)
            {
                Rectangle rc = GetTabRect(id);
                rc.Width--;
                rc.Height--;

                using (Pen pen = new Pen(m_ColorSelectedTab))
                    graphics.DrawRectangle(pen, rc);
            }
        }

        private void DrawTabContent(Graphics graphics, int id)
        {
            bool selectedOrHot = id == this.SelectedIndex || id == this.HotTabIndex;
            bool vertical = this.Alignment >= TabAlignment.Left;

            Image tabImage = null;

            if (this.ImageList != null)
            {
                TabPage page = this.TabPages[id];
                if (page.ImageIndex > -1 && page.ImageIndex < this.ImageList.Images.Count)
                    tabImage = this.ImageList.Images[page.ImageIndex];

                if (page.ImageKey.Length > 0 && this.ImageList.Images.ContainsKey(page.ImageKey))
                    tabImage = this.ImageList.Images[page.ImageKey];
            }

            Rectangle tabRect = GetTabRect(id);

            if (tabRect.Size.Width == 0 || tabRect.Size.Height == 0)
                return;

            Rectangle contentRect = new Rectangle(Point.Empty, tabRect.Size);
            Rectangle textrect = contentRect;
            textrect.Width -= FontHeight;

            if (tabImage != null)
            {
                textrect.Width -= tabImage.Width;
                textrect.X += tabImage.Width;
            }

            bool bEnabled = ((System.Windows.Forms.Control)(this.TabPages[id])).Enabled;

            Color text = (bEnabled == true ? Color.White : m_disableColor);
            Color frColor = id == SelectedIndex ? text : m_foreColor;
            if( bEnabled == false)
            {
                frColor = text;
            }
            Color bkColor = id == SelectedIndex ? m_ColorSelectedTab : this.BackColor;

            using (Bitmap bm = new Bitmap(contentRect.Width, contentRect.Height))
            {
                using (Graphics bmGraphics = Graphics.FromImage(bm))
                {
                    //TextRenderer.DrawText(bmGraphics, this.TabPages[id].Text, this.Font, textrect, frColor, bkColor);

                    if (selectedOrHot)
                    {
                        if(m_bUseCloseButton == true)
                        {
                            Rectangle closeRect = new Rectangle(contentRect.Right - CloseButtonHeight, 0, CloseButtonHeight, CloseButtonHeight);
                            closeRect.Offset(-2, (contentRect.Height - closeRect.Height) / 2);
                            DrawCloseButton(bmGraphics, closeRect);
                        }
                        
                    }

                    if (tabImage != null)
                    {
                        Rectangle imageRect = new Rectangle(Padding.X, 0, tabImage.Width, tabImage.Height);
                        imageRect.Offset(0, (contentRect.Height - imageRect.Height) / 2);
                        bmGraphics.DrawImage(tabImage, imageRect);
                    }
                }

                //if (vertical)
                //{
                //  if (this.Alignment == TabAlignment.Left)
                //      bm.RotateFlip(RotateFlipType.Rotate270FlipNone);
                //  else
                //      bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
                //}

                graphics.DrawImage(bm, tabRect);

                StringFormat _stringFlags = new StringFormat();
                _stringFlags.Alignment = StringAlignment.Center;
                _stringFlags.LineAlignment = StringAlignment.Center;
                SolidBrush _textBrush = new SolidBrush(frColor);
                graphics.DrawString(this.TabPages[id].Text, Font, _textBrush, tabRect, new StringFormat(_stringFlags));

            }
        }


        private Image m_CloseImage = null;
        public Image CloseBtnImage
        {
            get { return m_CloseImage; }
            set { m_CloseImage = value; }
        }

        private void DrawCloseButton(Graphics graphics, Rectangle bounds)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (m_CloseImage == null)
            {
                m_CloseImage = UnE.Properties.Resources.CloseWindow_Normal;                
                graphics.DrawImage(m_CloseImage, bounds);
            }
            else
            {                
                graphics.DrawImage(m_CloseImage, bounds);
            }
        }

        private Rectangle GetCloseButtonRect(int id)
        {

            Rectangle tabRect = GetTabRect(id);
            Rectangle closeRect = new Rectangle(tabRect.Left, tabRect.Top, CloseButtonHeight, CloseButtonHeight);

            switch (Alignment)
            {
                // case TabAlignment.Left:
                //     closeRect.Offset((tabRect.Width - closeRect.Width) / 2, 0);
                //     break;
                //case TabAlignment.Right:
                //     closeRect.Offset((tabRect.Width - closeRect.Width) / 2, tabRect.Height - closeRect.Height);
                //    break;
                default:
                    closeRect.Offset(tabRect.Width - closeRect.Width, (tabRect.Height - closeRect.Height) / 2);
                    break;
            }

            return closeRect;
        }

        #endregion

        #region Interop

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref TCHITTESTINFO lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct TCHITTESTINFO
        {
            public Point pt;
            public TCHITTESTFLAGS flags;
            public TCHITTESTINFO(int x, int y)
            {
                pt = new Point(x, y);
                flags = TCHITTESTFLAGS.TCHT_NOWHERE;
            }
        }

        [Flags()]
        private enum TCHITTESTFLAGS
        {
            TCHT_NOWHERE = 1,
            TCHT_ONITEMICON = 2,
            TCHT_ONITEMLABEL = 4,
            TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
        }

        private const int WM_NULL = 0x0;
        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1D;
        //private const int WM_MOUSEDOWN = 0x201;
        //private const int WM_MOUSEUP = 0x202;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;

		private const int WM_LBUTTONDBLCLK = 0x0203;//client area 
		private const int WM_NCLBUTTONDBLCLK = 0x00A3;//non-client area


        private const int TCM_FIRST = 0x1300;
        private const int TCM_HITTEST = TCM_FIRST + 13;
        private const int TCM_SETPADDING = TCM_FIRST + 43;

        #endregion


        #region Component Designer generated code
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.SuspendLayout();

            this.ResumeLayout(false);

        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion


    }
	
    public class TabControlExEventArgs
    {
        public TabPage DeletePage = null;		
    }

    public class TabControlMouseEventArgs
    {
        private TabPage m_page = null;
        private MouseButtons m_btn = MouseButtons.None;
        private Point m_point = new Point(0, 0);

        public TabPage Page
        {
            get { return m_page; }
            set { m_page = value; }
        }

        public MouseButtons Button
        {
            get { return m_btn; }
            set { m_btn = value; }
        }

        public Point Point
        {
            get { return m_point; }
            set { m_point = value; }
        }
    }
}
