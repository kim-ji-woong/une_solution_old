using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE
{
    namespace GUI
    {
        public partial class FormNoFrameSizable : Form
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
            private ImageLayout m_systemButtonImageLayout = ImageLayout.Stretch;
            private Size m_systemButtonSize = new Size(17, 15);

            public Image MinButtonImage
            {
                get { return m_imgMin; }
                set
                {
                    m_imgMin = value;
                    btnMin.BackgroundImage = value;
                    btnMin.BackgroundImageLayout = m_systemButtonImageLayout;

                    if (value != null)
                        btnMin.Text = "";
                }
            }

            public Image NormalButtonImage
            {
                get { return m_imgNormal; }
                set
                {
                    m_imgNormal = value;
                    btnMax.BackgroundImageLayout = m_systemButtonImageLayout;

                    if (value != null)
                        btnMax.Text = "";
                }
            }

            public Image MaxButtonImage
            {
                get { return m_imgMax; }
                set
                {
                    m_imgMax = value;
                    btnMax.BackgroundImageLayout = m_systemButtonImageLayout;

                    if (value != null)
                        btnMax.Text = "";
                }
            }

            public Image CloseButtonImage
            {
                get { return m_imgClose; }
                set
                {
                    m_imgClose = value;
                    btnClose.BackgroundImage = value;
                    btnClose.BackgroundImageLayout = m_systemButtonImageLayout;

                    if (value != null)
                        btnClose.Text = "";
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
                    }
                }
            }

            public bool ShowMinButton
            {
                get { return btnMin.Visible; }
                set { btnMin.Visible = value; }
            }

            public bool ShowMaxButton
            {
                get { return btnMax.Visible; }
                set { btnMax.Visible = value; }
            }

            public bool ShowCloseButton
            {
                get { return btnClose.Visible; }
                set { btnClose.Visible = value; }
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
                get { return panelBottom.BackColor; }
                set { panelBottom.BackColor = value; }
            }

            public Image LBEdgeImage
            {
                get { return panelLB.BackgroundImage; }
                set { panelLB.BackgroundImage = value; }
            }

            public ImageLayout LBEdgeImageLayout
            {
                get { return panelLB.BackgroundImageLayout; }
                set { panelLB.BackgroundImageLayout = value; }
            }

            public Color LBEdgeBackColor
            {
                get { return panelLB.BackColor; }
                set { panelLB.BackColor = value; }
            }

            public Image RBEdgeImage
            {
                get { return panelRB.BackgroundImage; }
                set { panelRB.BackgroundImage = value; }
            }

            public ImageLayout RBEdgeImageLayout
            {
                get { return panelRB.BackgroundImageLayout; }
                set { panelRB.BackgroundImageLayout = value; }
            }

            public Color RBEdgeBackColor
            {
                get { return panelRB.BackColor; }
                set { panelRB.BackColor = value; }
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
                    labelTitle.Font = value;
                }
            }

            public Font TitleTextFont
            {
                get { return labelTitle.Font; }
                set { labelTitle.Font = value; }
            }

            public Color TitleTextColor
            {
                get
                {
                    return labelTitle.ForeColor; 
                }
                set
                {
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

            private int m_nTitlePos = 10;
            public int TitlePosition
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

            // 작아질 수 있는 최소 크기
            private Size m_sizeMinimum = new Size(84, 60);
            public Size MinFrameSize
            {
                get { return m_sizeMinimum; }
                set { m_sizeMinimum = value; }
            }

            public FormNoFrameSizable(Form frmMain)
            {
                InitializeComponent();

                m_frmMain = frmMain;

                if (m_frmMain != null)
                {
                    m_frmMain.StartPosition = FormStartPosition.Manual;
                    m_frmMain.TopLevel = false;
                    m_frmMain.ShowInTaskbar = false;
                    m_frmMain.TabIndex = 1;
                    this.Controls.Add(m_frmMain);
                    m_frmMain.Show();
                }
            }

            public void ResizeFrame()
            {
                ResizeFramePanels();
                ResizeTitle();
                ResizeSystemButtons();
                
                if (m_frmMain != null)
                {
                    m_frmMain.Location = new Point(m_nEdgeThick, m_nTitleHeight);
                    m_frmMain.Size = new Size(this.Size.Width - m_nEdgeThick * 2, this.Size.Height - m_nTitleHeight - m_nEdgeThick);
                }
            }

            private void ResizeSystemButtons()
            {
                int nSpace = 1;

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
                        btnMax.BackgroundImage = m_imgMax;
                    else if (this.WindowState == FormWindowState.Maximized)
                        btnMax.BackgroundImage = m_imgNormal;
                }

                if (ShowMinButton)
                {
                    btnMin.Location = new Point(x, y);
                }
            }

            private void ResizeTitle()
            {
                int nPosX = TitlePosition;
                int nPosY = (m_nTitleHeight - labelTitle.Size.Height) / 2;

                if (nPosY > 3)
                    nPosY = 3;

                labelTitle.Location = new Point(nPosX, nPosY);
                TitleTextWidth = labelTitle.Size.Width;
            }

            private void ResizeFramePanels()
            {
                panelTop.Location = new Point(0, 0);
                panelTop.Size = new Size(this.Size.Width, m_nTitleHeight);

                panelLeft.Location = new Point(0, m_nTitleHeight);
                panelLeft.Size = new Size(m_nEdgeThick, this.Size.Height - m_nTitleHeight - m_nEdgeThick);

                panelRight.Location = new Point(this.Size.Width - m_nEdgeThick, m_nTitleHeight);
                panelRight.Size = new Size(m_nEdgeThick, this.Size.Height - m_nTitleHeight - m_nEdgeThick);

                panelLB.Location = new Point(0, this.Size.Height - m_nEdgeThick);
                panelLB.Size = new Size(m_nEdgeThick, m_nEdgeThick);

                panelRB.Location = new Point(panelRight.Location.X, panelLB.Location.Y);
                panelRB.Size = panelLB.Size;

                panelBottom.Location = new Point(m_nEdgeThick, this.Size.Height - m_nEdgeThick);
                panelBottom.Size = new Size(this.Size.Width - m_nEdgeThick * 2, m_nEdgeThick);
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

            protected virtual void OnFormResize(object sender, EventArgs e)
            {
                ResizeFrame();
            }

            protected virtual void EdgePanelMouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    m_bLeftMouseDown = true;
                    m_ptMove = Control.MousePosition;
                    m_sizeOrigin = this.Size;
                    m_ptOrigin = this.Location;
                }

                m_isClicked = true;
            }

            protected virtual void EdgePanelMouseLeave(object sender, EventArgs e)
            {
                this.Cursor = Cursors.Arrow;
            }

            protected virtual void EdgePanelMouseMove(object sender, MouseEventArgs e)
            {
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

                Point ptScreen = Control.MousePosition;

                int dx = ptScreen.X - m_ptMove.X;
                int dy = ptScreen.Y - m_ptMove.Y;

                if (dx == 0 && dy == 0)
                    return;

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
                this.Close();
            }

            protected virtual void btnMax_Click(object sender, EventArgs e)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }

            private void btnMin_Click(object sender, EventArgs e)
            {
                this.WindowState = FormWindowState.Minimized;
            }

            private void panelTop_DoubleClick(object sender, EventArgs e)
            {
                btnMax_Click(null, null);
            }
        }
    }
}
