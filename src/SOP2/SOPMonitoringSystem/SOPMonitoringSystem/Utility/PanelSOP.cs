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
        private ToolStripMenuItem LoadSOP;
        private ArrayList m_arrButtons = new ArrayList();

        public int CircleDiameter
        {
            get { return m_nDiameter; }
        }

        public PanelSOP()
        {
            InitializeComponent();

            m_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.MouseDown += new MouseEventHandler(PanelSOP_MouseDown);
        }

        void PanelSOP_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (FormMain.Instance.HasControl)
                    rButtonMenu.Show(this, e.X, e.Y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawArc(m_pen, m_rectCircle, 0.0f, 360.0f);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            m_rectCircle.Location = new Point((this.Size.Width - m_nDiameter) / 2, (this.Size.Height - m_nDiameter) / 2);
            m_rectCircle.Size = new Size(m_nDiameter, m_nDiameter);

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
            if (nButtonCount == 0)
                return;

            Button btn = (Button)m_arrButtons[0];

            double dAngle = System.Math.PI * 2 / nButtonCount;
            double dRadius = m_nDiameter / 2;
            double dCenterX = m_rectCircle.Location.X + m_rectCircle.Size.Width / 2;
            double dCenterY = m_rectCircle.Location.Y + m_rectCircle.Size.Height / 2;

            int nButtonHalfWidth = btn.Size.Width / 2;
            int nButtonHalfHeight = btn.Size.Height / 2;

            for (int i = 0; i < nButtonCount; i++)
            {
                Button button = (Button)m_arrButtons[i];

                double x = dCenterX + System.Math.Sin(dAngle * i) * dRadius;
                double y = dCenterY - System.Math.Cos(dAngle * i) * dRadius;

                button.Location = new Point((int)x - nButtonHalfWidth, (int)y - nButtonHalfHeight);
            }
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;

            if (btn.Tag == null)
                return;

            FormMain.Instance.GetPageHome().LoadQuickSOP(btn);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rButtonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LoadSOP = new System.Windows.Forms.ToolStripMenuItem();
            this.rButtonMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // rButtonMenu
            // 
            this.rButtonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadSOP});
            this.rButtonMenu.Name = "rButtonMenu";
            this.rButtonMenu.Size = new System.Drawing.Size(150, 26);
            // 
            // LoadSOP
            // 
            this.LoadSOP.Name = "LoadSOP";
            this.LoadSOP.Size = new System.Drawing.Size(149, 22);
            this.LoadSOP.Text = "SOP 불러오기";
            this.LoadSOP.Click += new EventHandler(LoadSOP_Click);
            this.rButtonMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void LoadSOP_Click(object sender, EventArgs e)
        {
            FormMain.Instance.GetPageHome().OpenSOP();
        }
    }
}
