using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace SOPMonitoringSystem
{
    public class PanelSOP : Panel, UnE.GUI.IRibbonButtonOwner
    {
        private Pen m_pen = new Pen(Color.FromArgb(127, 134, 142), 2.0f);
        private Rectangle m_rectCircle = new Rectangle();
        private int m_nDiameter = 494;
        private ContextMenuStrip rButtonMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsMenuLoadSOP;
        private ToolStripMenuItem tsMenuShowCCTV;
        private ArrayList m_arrButtons = new ArrayList();

        public int backImgWidth { get; set; }
        public int backImgHeight { get; set; }

        public int CircleDiameter
        {
            get { return m_nDiameter; }
        }

        public PanelSOP()
        {
            InitializeComponent();
             
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true); 

            m_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.MouseDown += new MouseEventHandler(PanelSOP_MouseDown);
        }

        void PanelSOP_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (FormSOP.Instance.HasControl)
                    rButtonMenu.Show(this, e.X, e.Y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.DrawArc(m_pen, m_rectCircle, 0.0f, 360.0f);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            m_rectCircle.Location = new Point((this.Size.Width - m_nDiameter) / 2, (this.Size.Height - m_nDiameter + 80) / 2);
            m_rectCircle.Size = new Size(m_nDiameter, m_nDiameter - 80);

            int nButtonCount = m_arrButtons.Count;
            if (nButtonCount == 0)
                return;
             
            Button btn = (Button)m_arrButtons[0];
 
            int nButtonHalfWidth = btn.Size.Width;
            int nButtonHalfHeight = btn.Size.Height;
            m_rectCircle.Size = new Size(this.Width - nButtonHalfWidth, this.Height - Height);
            LocateButtons(); 
        }

        public void AddQuickButton(Button btn)
        {
            this.Controls.Add(btn);
            m_arrButtons.Add(btn);

            LocateButtons();
        } 

        private void LocateButtons()
        {
            int nButtonCount = m_arrButtons.Count;
            if (nButtonCount == 0) return;

            //if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
            {
                //버튼 간격 가로 길이
                int betweenWidth = 235; 

                int centerBtnPtX = (this.Size.Width / 2) - 100; // -100:BakcImg가 그림자때문에 오른쪽으로 치우쳐져 있는걸 감안
                int centerBtnptY = ((this.Size.Height - backImgHeight) / 2) - 70; // -70:BakcImg가 그림자때문에 아래으로 치우쳐져 있는걸 감안
                 
                for (int i = 0; i < nButtonCount; i++)
                {
                    RibbonButtonQuick button = (RibbonButtonQuick)m_arrButtons[i];
                    int x = 0;
                    int y = 0;

                    Point ptOrg = button.Location;

                    /*if (i == 0)
                    {
                        y = centerBtnptY + backImgHeight - 50; // -50:가운데 버튼보다 왼/오른쪽 버튼이미지가 위에 있음
                        x = centerBtnPtX - betweenWidth;
                    }
                    else if (i == 1)
                    {
                        y = centerBtnptY + backImgHeight;
                        x = centerBtnPtX;
                    }
                    else if (i == 2)*/
                    {
                        y = centerBtnptY + backImgHeight - 50; // -50:가운데 버튼보다 왼/오른쪽 버튼이미지가 위에 있음
                        x = centerBtnPtX + betweenWidth;
                    }
                    button.Size = new System.Drawing.Size(210, 220);

                    //전체 사이즈가 로고크기보다 작으면 고정값을 넣어준다
                    if (this.Size.Width < backImgWidth)
                    {
                        if (i == 0) x = -9;
                        else if (i == 1) x = 226;
                        else if (i == 2) x = 461;
                    }

                    if (this.Size.Height < backImgHeight)
                    {
                        if (i == 1) y = 480;
                        else y = 430;
                    }

                    button.Location = new Point((int)x, (int)y);
                }    
            }
            /*else
            {
                RibbonButtonQuick btn = (RibbonButtonQuick)m_arrButtons[0];

                int nBtnWidth = this.Size.Width / 3;
                int nBtnHeight = this.Size.Height / 3;

                btn.BackColor = Color.Aquamarine;

                double dCenterX = Size.Width / 2;
                double dCenterY = Size.Height / 2;

                for (int i = 0; i < nButtonCount; i++)
                {
                    RibbonButtonQuick button = (RibbonButtonQuick)m_arrButtons[i];
                    int x = 0;
                    int y = 0;

                    if (i == 0 || i == 1 || i == 7)
                    {
                        y = 0;
                    }
                    else if (i == 5 || i == 4 || i == 3)
                    {
                        y = Size.Height - nBtnHeight;
                    }
                    else if (i == 2 || i == 6)
                    {
                        y = Size.Height - nBtnHeight - nBtnHeight;
                    }

                    if (i == 1 || i == 2 || i == 3)
                    {
                        x = Size.Width - nBtnWidth;
                    }
                    else if (i == 5 || i == 6 || i == 7)
                    {
                        x = 0;
                    }
                    else if (i == 0 || i == 4)
                    {
                        x = Size.Width - nBtnWidth - nBtnWidth;
                    }
                    button.Size = new System.Drawing.Size(nBtnWidth, nBtnHeight);
                    button.Location = new Point((int)x, (int)y);
                }
            }*/
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e) { }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;

                if (btn.Tag == null)
                    return;

                FormSOP.Instance.GetPageHome().LoadQuickSOP(btn);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;

                e = new MouseEventArgs(e.Button, e.Clicks, btn.Location.X + e.X, btn.Location.Y + e.Y, e.Delta);
                this.PanelSOP_MouseDown(this, e);
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rButtonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuLoadSOP = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuShowCCTV = new ToolStripMenuItem();
            this.rButtonMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // rButtonMenu
            // 
            this.rButtonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuLoadSOP, this.tsMenuShowCCTV});
            this.rButtonMenu.Name = "rButtonMenu";
            this.rButtonMenu.Size = new System.Drawing.Size(150, 26);
            // 
            // tsMenuLoadSOP
            // 
            this.tsMenuLoadSOP.Name = "tsMenuLoadSOP";
            this.tsMenuLoadSOP.Size = new System.Drawing.Size(149, 22);
            this.tsMenuLoadSOP.Text = "시나리오 불러오기";
            this.tsMenuLoadSOP.Click += new EventHandler(LoadSOP_Click);
            //
            // tsMenuShowCCTV
            // 
            this.tsMenuShowCCTV.Name = "tsMenuShowCCTV";
            this.tsMenuShowCCTV.Size = new System.Drawing.Size(149, 22);
            this.tsMenuShowCCTV.Text = "CCTV 보기";
            this.tsMenuShowCCTV.Click += new EventHandler(ShowCCTV_Click);
            this.rButtonMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        void LoadSOP_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.GetPageHome().OpenSOP();
        }

        void ShowCCTV_Click(object sender, EventArgs e)
        {
            //FormSOP.Instance.SelectCCTVTab();
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        public void ShowCCTVToolStripMenuItem(bool isVisible)
        {
            tsMenuShowCCTV.Visible = isVisible;
        }

    }
}
