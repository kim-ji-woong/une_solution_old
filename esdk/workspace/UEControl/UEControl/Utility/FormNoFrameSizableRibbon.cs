using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UnE
{
    namespace GUI
    {
        public partial class FormNoFrameSizableRibbon : Form
        {
            #region Form 이동
            private bool m_bLeftMouseDown = false;
            private Point m_ptMove = new Point();
            #endregion

            #region Resize
            private Size m_sizeOrigin = new Size();
            private Point m_ptOrigin = new Point();
            private Panel m_resizePanel = null;
            private bool m_isClicked = false;

            private int m_nTitleHeight = 20;
            private int m_nEdgeThick = 5;
            #endregion

            #region System Buttons

            private Image m_imgMin = null;
            private Image m_imgNormal = null;
            private Image m_imgMax = null;
            private Image m_imgClose = null;

            private Image m_imgMinOver = null;
            private Image m_imgNormalOver = null;
            private Image m_imgMaxOver = null;
            private Image m_imgCloseOver = null;


            private ImageLayout m_systemButtonImageLayout = ImageLayout.Stretch;
            private Size m_systemButtonSize = new Size(17, 15);
            private Size m_pictureBoxSize = new Size(22, 20);

            public Image MinButtonImage
            {
                get { return m_imgMin; }
                set
                {
                    m_imgMin = value;

                    btnMin.NormalImage = value;

                    if (value != null)
                    {
                        btnMin.Text = "";
                    }
                    btnMin.Update();
                }
            }

            public Image MinButtonOverImage
            {
                get { return m_imgMinOver; }
                set
                {
                    m_imgMinOver = value;

                    btnMin.MouseOverImage = value;
                    btnMin.ClickedImage = value;

                    if (value != null)
                    {
                        btnMin.Text = "";
                    }
                    btnMin.Update();
                }
            }

            public Image NormalButtonImage
            {
                get { return m_imgNormal; }
                set
                {
                    m_imgNormal = value;

                    btnMax.NormalImage = value;


                    if (value != null)
                    {
                        btnMax.Text = "";
                    }

                    btnMax.Update();
                }
            }

            public Image NormalButtonOverImage
            {
                get { return m_imgNormalOver; }
                set
                {
                    m_imgNormalOver = value;

                    btnMax.MouseOverImage = value;
                    btnMax.ClickedImage = value;

                    if (value != null)
                    {
                        btnMax.Text = "";
                    }

                    btnMax.Update();
                }
            }

            public Image MaxButtonImage
            {
                get { return m_imgMax; }
                set
                {
                    m_imgMax = value;

                    btnMax.NormalImage = value;

                    if (value != null)
                    {
                        btnMax.Text = "";
                    }
                    btnMax.Update();
                }
            }

            public Image MaxButtonOverImage
            {
                get { return m_imgMaxOver; }
                set
                {
                    m_imgMaxOver = value;

                    btnMax.MouseOverImage = value;
                    btnMax.ClickedImage = value;

                    if (value != null)
                    {
                        btnMax.Text = "";
                    }
                    btnMax.Update();
                }
            }

            public Image CloseButtonImage
            {
                get { return m_imgClose; }
                set
                {
                    m_imgClose = value;
                    
                    btnClose.NormalImage = value;

                    if (value != null)
                    {
                        btnClose.Text = "";
                    }
                    btnClose.Update();
                }
            }

            public Image CloseButtonOverImage
            {
                get { return m_imgCloseOver; }
                set
                {
                    m_imgCloseOver = value;

                    btnClose.MouseOverImage = value;
                    btnClose.ClickedImage = value;

                    if (value != null)
                    {
                        btnClose.Text = "";
                    }
                    btnClose.Update();
                }
            }

            public ImageLayout SystemButtonImageLayout
            {
                get { return m_systemButtonImageLayout; }
                set
                {
                    m_systemButtonImageLayout = value;
                    btnMin.BackgroundImageLayout = btnMax.BackgroundImageLayout = btnClose.BackgroundImageLayout = value;
                }
            }

            public Size SystemButtonSize
            {
                get { return m_systemButtonSize; }
                set
                {
                    if (m_systemButtonSize != value)
                    {
                        m_systemButtonSize = value;

                        btnMin.Size = btnMax.Size = btnClose.Size = value;

                        btnMin.InitButtonWidth = value.Width;
                        btnMax.InitButtonWidth = value.Width;
                        btnClose.InitButtonWidth = value.Width;

                        btnMin.CustomImageRect = new Rectangle(0, 0, value.Width, value.Height);
                        btnMax.CustomImageRect = new Rectangle(0, 0, value.Width, value.Height);
                        btnClose.CustomImageRect = new Rectangle(0, 0, value.Width, value.Height);
                    }
                }
            }

            public bool ShowMinButton
            {
                get 
                {                        
                    return btnMin.Visible; 
                }
                set 
                {                        
                    btnMin.Visible = value;                    
                }
            }

            public bool ShowMaxButton
            {
                get 
                {                        
                    return btnMax.Visible; 
                }
                set 
                {                        
                    btnMax.Visible = value;     
                }
            }

            public bool ShowCloseButton
            {
                get 
                {                        
                    return btnClose.Visible; 
                }
                set 
                {                        
                    btnClose.Visible = value;    
                }
            }
            #endregion

            #region TitleBar 및 Edge
            public Panel TitleBar
            {
                get { return panelTop; }
            }

            public int TitleBarHeight
            {
                get { return m_nTitleHeight; }
                set
                {
                    if (m_nTitleHeight != value)
                    {
                        m_nTitleHeight = value;
                        ResizeFrame();
                    }
                }
            }

            public int EdgeThick
            {
                get { return m_nEdgeThick; }
                set
                {
                    if (m_nEdgeThick != value)
                    {
                        m_nEdgeThick = value;
                        ResizeFrame();
                    }
                }
            }

            public PictureBox PictureBoxTitle
            {
                get { return pictureBoxTitle; }
            }

            public Image PictureBoxTitleImage
            {
                get { return pictureBoxTitle.BackgroundImage; }
                set { pictureBoxTitle.BackgroundImage = value; }
            }

            public bool ShowPictureBoxTitle
            {
                get
                {
                    return pictureBoxTitle.Visible;
                }
                set
                { 
                    pictureBoxTitle.Visible = value;
                    if( value == true)
                    {
                        Point p = pictureBoxTitle.Location;
                        Size size = pictureBoxTitle.Size;

                        Point p2 = labelTitle.Location;
                        labelTitle.Location = new Point(p.X + size.Width + 1, p2.Y);
                    }
                    else
                    {
                        Point p = pictureBoxTitle.Location;
                        Size size = pictureBoxTitle.Size;

                        Point p2 = labelTitle.Location;
                        labelTitle.Location = new Point(p.X , p2.Y);
                    }
                    
                }
            }


            public Size PictureBoxSize
            {
                get { return m_pictureBoxSize; }
                set
                {
                    if (m_pictureBoxSize != value)
                    {
                        m_pictureBoxSize = value;
                        pictureBoxTitle.Size = value;

                        if (pictureBoxTitle.Visible == true)
                        {
                            Point p = pictureBoxTitle.Location;
                            Size size = pictureBoxTitle.Size;

                            Point p2 = labelTitle.Location;
                            labelTitle.Location = new Point(p.X + size.Width + 1, p2.Y);
                        }
                        else
                        {
                            Point p = pictureBoxTitle.Location;
                            Size size = pictureBoxTitle.Size;

                            Point p2 = labelTitle.Location;
                            labelTitle.Location = new Point(p.X, p2.Y);
                        }
                    
                    }
                }
            }

            public Image TitleBarImage
            {
                get { return panelTop.BackgroundImage; }
                set { panelTop.BackgroundImage = value; }
            }

            public ImageLayout TitleBarImageLayout
            {
                get { return panelTop.BackgroundImageLayout; }
                set { panelTop.BackgroundImageLayout = value; }
            }

            public Color TitleBarBackColor
            {
                get { return panelTop.BackColor; }
                set { panelTop.BackColor = value; }
            }

            public Image LeftEdgeImage
            {
                get { return panelLeft.BackgroundImage; }
                set { panelLeft.BackgroundImage = value; }
            }

            public ImageLayout LeftEdgeImageLayout
            {
                get { return panelLeft.BackgroundImageLayout; }
                set { panelLeft.BackgroundImageLayout = value; }
            }

            public Color LeftEdgeBackColor
            {
                get { return panelLeft.BackColor; }
                set { panelLeft.BackColor = value; }
            }

            public Image RightEdgeImage
            {
                get { return panelRight.BackgroundImage; }
                set { panelRight.BackgroundImage = value; }
            }

            public ImageLayout RightEdgeImageLayout
            {
                get { return panelRight.BackgroundImageLayout; }
                set { panelRight.BackgroundImageLayout = value; }
            }

            public Color RightEdgeBackColor
            {
                get { return panelRight.BackColor; }
                set { panelRight.BackColor = value; }
            }

            public Image BottomEdgeImage
            {
                get { return panelBottom.BackgroundImage; }
                set { panelBottom.BackgroundImage = value; }
            }

            public ImageLayout BottomEdgeImageLayout
            {
                get { return panelBottom.BackgroundImageLayout; }
                set { panelBottom.BackgroundImageLayout = value; }
            }

            public Color BottomEdgeBackColor
            {
                get
                {
                    if (panelBottom == null)
                        return Color.Black;

                    return panelBottom.BackColor;
                }
                set
                {
                    if (panelBottom != null)
                        panelBottom.BackColor = value;
                }
            }

            public Image LBEdgeImage
            {
                get
                {
                    if (panelLB == null)
                        return null;

                    return panelLB.BackgroundImage;
                }
                set
                {
                    if (panelLB != null)
                        panelLB.BackgroundImage = value;
                }
            }

            public ImageLayout LBEdgeImageLayout
            {
                get
                {
                    if (panelLB == null)
                        return ImageLayout.None;

                    return panelLB.BackgroundImageLayout;
                }
                set
                {
                    if (panelLB != null)
                        panelLB.BackgroundImageLayout = value;
                }
            }

            public Color LBEdgeBackColor
            {
                get
                {
                    if (panelLB == null)
                        return Color.Black;

                    return panelLB.BackColor;
                }
                set
                {
                    if (panelLB != null)
                        panelLB.BackColor = value;
                }
            }

            public Image RBEdgeImage
            {
                get
                {
                    if (panelRB == null)
                        return null;

                    return panelRB.BackgroundImage;
                }
                set
                {
                    if (panelRB != null)
                        panelRB.BackgroundImage = value;
                }
            }

            public ImageLayout RBEdgeImageLayout
            {
                get
                {
                    if (panelRB == null)
                        return ImageLayout.None;

                    return panelRB.BackgroundImageLayout;
                }
                set
                {
                    if (panelRB != null)
                        panelRB.BackgroundImageLayout = value;
                }
            }

            public Color RBEdgeBackColor
            {
                get
                {
                    if (panelRB == null)
                        return Color.Black;

                    return panelRB.BackColor;
                }
                set
                {
                    if (panelRB != null)
                        panelRB.BackColor = value;
                }
            }
            #endregion

            #region Title Text, Font, 색상
            public override string Text
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    base.Text = value;
                    if (labelTitle != null)
                        labelTitle.Text = value;
                }
            }

            public override Font Font
            {
                get
                {
                    return base.Font;
                }
                set
                {
                    base.Font = value;
                    if (labelTitle != null)
                    labelTitle.Font = value;
                }
            }

            public Font TitleTextFont
            {
                get
                {
                    if (labelTitle == null)
                        return null;

                    return labelTitle.Font;
                }
                set
                {
                    if (labelTitle != null)
                        labelTitle.Font = value;
                }
            }

            public Color TitleTextColor
            {
                get
                {                    
                        return labelTitle.ForeColor;                    
                }
                set
                {
                    if (labelTitle != null)
                    labelTitle.ForeColor = value;
                }
            }

            public override Color ForeColor
            {
                get
                {
                    return base.ForeColor;
                }
                set
                {
                    base.ForeColor = value;
                    labelTitle.ForeColor = value;
                }
            }

            private Point m_nTitlePos = new Point(10, 2);
            public Point TitlePosition
            {
                get { return m_nTitlePos; }
                set { m_nTitlePos = value; }
            }

            private int m_nTItleTextWidth = 0;
            public int TitleTextWidth
            {
                get { return m_nTItleTextWidth; }
                set { m_nTItleTextWidth = value; }
            }
            #endregion

            protected Form m_frmMain = null;

            private bool m_sizable = true;
            public bool Sizable
            {
                get { return m_sizable; }
                set
                {
                    m_sizable = value;
                    btnMax.Enabled = m_sizable;
                }
            }

            // 작아질 수 있는 최소 크기
            private Size m_sizeMinimum = new Size(84, 60);
            public Size MinFrameSize
            {
                get { return m_sizeMinimum; }
                set { m_sizeMinimum = value; }
            }

            // 최대화시 작업표시줄을 보이도록 할 것인가?
            private bool m_maximizeWithTaskbar = true;
            public bool MaximizeWithTaskbar
            {
                get { return m_maximizeWithTaskbar; }
                set { m_maximizeWithTaskbar = value; }
            }

            public FormNoFrameSizableRibbon()
            {
                InitializeComponent();
                RegisterEventHandler();
            }

            public FormNoFrameSizableRibbon(Form frmMain)
            {
                InitializeComponent();

                m_frmMain = frmMain;

                if (m_frmMain != null)
                {
                    this.Location = m_frmMain.Location;
					this.StartPosition = m_frmMain.StartPosition;
					this.ShowInTaskbar = m_frmMain.ShowInTaskbar;
                    m_frmMain.StartPosition = FormStartPosition.Manual;
                    m_frmMain.TopLevel = false;
                    m_frmMain.ShowInTaskbar = false;
                    m_frmMain.TabIndex = 1;
                    m_frmMain.ShowIcon = false;
                    this.Controls.Add(m_frmMain);
                }
                pictureBoxTitle.Visible = false;
                RegisterEventHandler();

                this.Resize += new System.EventHandler(this.OnFormResize);          
            }

            public Size GetWindowSize()
            {
                Rectangle mBounce = Screen.FromControl(this).Bounds;
                Size mMaxSize = new Size(mBounce.Width, mBounce.Height);

                Size mSize = this.Size;

                if (mSize.Width > mMaxSize.Width)
                    mSize.Width = mMaxSize.Width;

                if (mSize.Height > mMaxSize.Height)
                    mSize.Height = mMaxSize.Height;

                return mSize;
            }

			protected virtual void ResizeFrame()
            {                
                Size mSize = GetWindowSize();                

                ResizeFramePanels(mSize);
                ResizeTitle();
                ResizeSystemButtons();
                
                if (m_frmMain != null)
                {
                    m_frmMain.Location = new Point(m_nEdgeThick, m_nTitleHeight);

                    m_frmMain.Size = new Size(mSize.Width - m_nEdgeThick * 2, mSize.Height - m_nTitleHeight - m_nEdgeThick);
                }
            }

			protected void ResizeSystemButtons()
            {
                int nSpace = 7;

                int x = panelTop.Size.Width - SystemButtonSize.Width - m_nEdgeThick;
                int y = (m_nTitleHeight - SystemButtonSize.Height) / 2;

                if (y > 3)
                    y = 3;

                if (ShowCloseButton)
                {                    
                    btnClose.Location = new Point(x, y);
                    x = x - SystemButtonSize.Width - nSpace;
                }

                if (ShowMaxButton)
                {                    
                    btnMax.Location = new Point(x, y);
                    x = x - SystemButtonSize.Width - nSpace;

                    if (this.WindowState == FormWindowState.Normal)
                    {                        
                        btnMax.BackgroundImage = m_imgMax;
                    }
                    else if (this.WindowState == FormWindowState.Maximized)
                    {                        
                        btnMax.BackgroundImage = m_imgNormal;
                    }
                }

                if (ShowMinButton)
                {                    
                    btnMin.Location = new Point(x, y);
                }
            }

			protected void ResizeTitle()
            {
                labelTitle.Location = TitlePosition;
                TitleTextWidth = labelTitle.Size.Width;
            }

            protected void ResizeFramePanels(Size pSize)
            {
                panelTop.Location = new Point(0, 0);
                panelTop.Size = new Size(pSize.Width, m_nTitleHeight);

                panelLeft.Location = new Point(0, m_nTitleHeight);
                panelLeft.Size = new Size(m_nEdgeThick, pSize.Height - m_nTitleHeight - m_nEdgeThick);

                panelRight.Location = new Point(pSize.Width - m_nEdgeThick, m_nTitleHeight);
                panelRight.Size = new Size(m_nEdgeThick, pSize.Height - m_nTitleHeight - m_nEdgeThick);

                panelLB.Location = new Point(0, pSize.Height - m_nEdgeThick);
                panelLB.Size = new Size(m_nEdgeThick, m_nEdgeThick);

                panelRB.Location = new Point(panelRight.Location.X, panelLB.Location.Y);
                panelRB.Size = panelLB.Size;

                panelBottom.Location = new Point(m_nEdgeThick, pSize.Height - m_nEdgeThick);
                panelBottom.Size = new Size(pSize.Width - m_nEdgeThick * 2, m_nEdgeThick);
            }

            private void SetAreaCursor(Panel mode)
            {
                if (mode == panelTop)
                    this.Cursor = Cursors.Arrow;
                else if (mode == panelRight || mode == panelLeft)
                    this.Cursor = Cursors.SizeWE;
                else if (mode == panelBottom)
                    this.Cursor = Cursors.SizeNS;
                else if (mode == panelLB)
                    this.Cursor = Cursors.SizeNESW;
                else if (mode == panelRB)
                    this.Cursor = Cursors.SizeNWSE;
            }


            private void RemoveClickEventHander(Button btn)
            {           
                FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
                object obj = f1.GetValue(btn);
                PropertyInfo pi = btn.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                EventHandlerList list = (EventHandlerList)pi.GetValue(btn, null);
                list.RemoveHandler(obj, list[obj]);
            }

            protected virtual void RegisterEventHandler()            
            {
                this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
                this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
                this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
                this.pictureBoxTitle.DoubleClick += new System.EventHandler(this.pictureBoxTitle_DoubleClick);
            }
			

            protected virtual void OnFormResize(object sender, EventArgs e)
            {
                ResizeFrame();
            }

            protected virtual void EdgePanelMouseDown(object sender, MouseEventArgs e)
            {
                ProcessPanelMouseDown(e, Control.MousePosition);
            }

            protected void ProcessPanelMouseDown(MouseEventArgs e, Point ptMouse)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    m_bLeftMouseDown = true;
                    m_ptMove = ptMouse;                    
                    m_sizeOrigin = this.Size;
                    m_ptOrigin = this.Location;
                }

                m_isClicked = true;
            }

            protected virtual void EdgePanelMouseLeave(object sender, EventArgs e)
            {
                this.Cursor = Cursors.Arrow;

                m_isClicked = false;
            }

            protected virtual void EdgePanelMouseMove(object sender, MouseEventArgs e)
            {
                ProcessPanelMouseMove(sender, e, Control.MousePosition);               
            }

            protected void ProcessPanelMouseMove(object sender, MouseEventArgs e, Point ptMouse)
            {
                if (sender == null)
                    sender = panelTop;

                if (!m_sizable && sender != panelTop && sender != labelTitle)
                    return;

                if (!m_isClicked)
                {
                    if (sender == labelTitle)
                        m_resizePanel = panelTop;
                    else
                        m_resizePanel = (Panel)sender;

                    SetAreaCursor(m_resizePanel);
                    return;
                }

                if (!m_bLeftMouseDown)
                    return;

                Point ptScreen = ptMouse;

                int dx = ptScreen.X - m_ptMove.X;
                int dy = ptScreen.Y - m_ptMove.Y;

                if (dx == 0 && dy == 0)
                    return;
                 
                if (this.WindowState == FormWindowState.Maximized)
                {
                    if (m_imgMax != null)
                    {
                        this.btnMax.BackgroundImage = m_imgMax;
                    }
                     
                    //PanelTop Click한 지점 %
                    double maxPerX = Math.Round((double)ptScreen.X * 100 / this.Size.Width);
                    double maxPerY = Math.Round((double)ptScreen.Y * 100 / this.Size.Height);
                    if (maxPerX > 100) maxPerX = maxPerX - 100; 
                    this.WindowState = FormWindowState.Normal;
                    //Size 변경된 후 maxPerX(%)에 알맞은 Mouse Cursor지점
                    int normalPerX = Convert.ToInt32(this.Size.Width * maxPerX / 100);
                    int normalPerY = Convert.ToInt32(this.Size.Height * maxPerY / 100);

                    this.Location = new Point(ptScreen.X - normalPerX, ptScreen.Y - normalPerY);
                    
                    ProcessPanelMouseDown(e, ptMouse); 
                    return;
                }

                if (m_resizePanel == panelTop)
                { 
                    Point ptCur = this.Location;
                    this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                    m_ptMove.X += dx;
                    m_ptMove.Y += dy;                                     
                }
                else if (m_resizePanel == panelRight)
                {
                    ChangeSize(this.m_sizeOrigin.Width + dx, this.m_sizeOrigin.Height);
                }
                else if (m_resizePanel == panelBottom)
                {
                    ChangeSize(this.m_sizeOrigin.Width, this.m_sizeOrigin.Height + dy);
                }
                else if (m_resizePanel == panelLeft)
                {
                    this.Location = new Point(this.m_ptOrigin.X + dx, this.m_ptOrigin.Y);
                    ChangeSize(this.m_sizeOrigin.Width - dx, this.m_sizeOrigin.Height);
                }
                else if (m_resizePanel == panelLB)
                {
                    this.Location = new Point(this.m_ptOrigin.X + dx, this.m_ptOrigin.Y);
                    ChangeSize(this.m_sizeOrigin.Width - dx, this.m_sizeOrigin.Height + dy);
                }
                else if (m_resizePanel == panelRB)
                {
                    ChangeSize(this.m_sizeOrigin.Width + dx, this.m_sizeOrigin.Height + dy);
                }
            } 

            private void ChangeSize(int width, int height)
            {
                if (width < m_sizeMinimum.Width)
                    width = m_sizeMinimum.Width;

                if (height < m_sizeMinimum.Height)
                    height = m_sizeMinimum.Height;

                this.Size = new Size(width, height);
            }

            protected virtual void EdgePanelMouseUp(object sender, MouseEventArgs e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    m_bLeftMouseDown = false;

                m_isClicked = false;
            }

            private void btnClose_Click(object sender, EventArgs e)
            {
                CloseButtonClicked();
            }   
         
            protected virtual void CloseButtonClicked()
            {
                this.Close();
            }

            [DllImport("user32")]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

            [DllImport("user32")]
            internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

            protected static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
            {

                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                int MONITOR_DEFAULTTONEAREST = 0x00000002;
                System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != System.IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    GetMonitorInfo(monitor, monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                    mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                    mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                    mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            private const int WM_GETMINMAXINFO = 0x0024;

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_GETMINMAXINFO:
                        if (m_maximizeWithTaskbar)
                            WmGetMinMaxInfo(m.HWnd, m.LParam);
                        break;
                }

                base.WndProc(ref m);
            }

            protected virtual void MaxButtonClicked()
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;

                    if (m_imgNormal != null)
                    {
                        this.btnMax.BackgroundImage = m_imgNormal;
                    }
                }
                else if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    if (m_imgMax != null)
                    {
                        this.btnMax.BackgroundImage = m_imgMax;
                    }
                }
            }

            protected virtual void btnMax_Click(object sender, EventArgs e)
            {
                MaxButtonClicked();
            }

            protected virtual void MinButtonClicked()
            {
                this.WindowState = FormWindowState.Minimized;
            }

            private void btnMin_Click(object sender, EventArgs e)
            {
                MinButtonClicked();
            }

            private void panelTop_DoubleClick(object sender, EventArgs e)
            {
                if (m_sizable)
                    btnMax_Click(null, null);
            }

            private void pictureBoxTitle_DoubleClick(object sender, EventArgs e)
            {
				CloseButtonClicked();
            }
        }
    }
}
