using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor.uCustomize
{
    public class uPanel : Panel
    {
        private Page m_page = new Page();
        public Page Page
        {
            get { return m_page; }
            set { m_page = value; }
        }
        
        private bool m_bLeftMouseDown = false;
        private bool m_bBtnSizableDown = false;
        private Point m_ptMove = new Point();
        private int m_nBtnSizableSize = 20;
        private PictureBox m_btnSizable = null;
        private Size m_orgSize = new Size();
        public Size OrgSize
        {
            get { return m_orgSize; }
            set { m_orgSize = value; }
        }

        public uPanel()
        {
            this.DoubleBuffered = true;
            
            this.MouseDown += UPanel_MouseDown;
            this.MouseMove += UPanel_MouseMove;
            this.MouseUp += UPanel_MouseUp;
        }

        private void UPanel_MouseDown(object sender, MouseEventArgs e)
        {
            FormMain.Instance.SetChangeSelectionPanel(this);

            if (e.Button == MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
            }                  
        }

        private void UPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void UPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = false;
                if (m_page.PageType == PageType.None)
                    m_page.Medias[0].MediaLocation = this.Location;
                else
                    m_page.PageLocation = this.Location;
            }
        }

        /// <summary>
        /// 최상단 패널은 페이지 자체라서 마우스 이벤트 막음
        /// </summary>
        public void SetNoEvent()
        {
            this.MouseDown -= UPanel_MouseDown;
            this.MouseMove -= UPanel_MouseMove;
            this.MouseUp -= UPanel_MouseUp;
        }
        
        public void AddBtnSizable()
        {
            m_btnSizable = new PictureBox();
            m_btnSizable.Name = "picSizable";
            m_btnSizable.BackColor = Color.Transparent;
            m_btnSizable.Image = global::DidUIEditor.Properties.Resources.sizable;
            m_btnSizable.SizeMode = PictureBoxSizeMode.StretchImage;
            m_btnSizable.Parent = this;
            m_btnSizable.Size = new Size(m_nBtnSizableSize, m_nBtnSizableSize);
            m_btnSizable.Location = new Point(this.Width - m_nBtnSizableSize, this.Height - m_nBtnSizableSize);
            m_btnSizable.Visible = false;

            m_btnSizable.MouseDown += btnSizeble_MouseDown;
            m_btnSizable.MouseMove += btnSizeble_MouseMove;
            m_btnSizable.MouseUp += btnSizeble_MouseUp;            
        }
        
        private void btnSizeble_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_bBtnSizableDown = true;
                m_ptMove = Control.MousePosition;
            }
        }

        private void btnSizeble_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_bBtnSizableDown)
                return;

            PictureBox btn = sender as PictureBox;
            if (btn == null)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            if (this.Width + dx <= m_nBtnSizableSize && this.Height + dy <= m_nBtnSizableSize)
                return;

            //if (this.Width + dx > m_nBtnSizableSize)
            //{
            //    this.Width = this.Width + dx;
            //    m_ptMove.X += dx;
            //}
            //if (this.Height + dy > m_nBtnSizableSize)
            //{
            //    this.Height = this.Height + dy;
            //    m_ptMove.Y += dy;
            //}

            // 원본 비율대로 사이즈 조절
            float xPer = (this.Width + dx) / (float)this.m_orgSize.Width * 100;
            float yPer = (float)(this.m_orgSize.Height * xPer / 100);

            m_ptMove.X += dx;
            m_ptMove.Y += (int)yPer;

            this.Width = this.Width + dx;
            this.Height = (int)yPer;


            btn.Location = new Point(this.Width - m_nBtnSizableSize - 1, this.Height - m_nBtnSizableSize - 1);            
        }

        private void btnSizeble_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bBtnSizableDown = false;
                if (m_page.PageType == PageType.None)
                    m_page.Medias[0].MediaSize = this.Size;
                else
                    m_page.PageSize = this.Size;
            }
        }      

        public void SetVisible(bool visible)
        {
            m_btnSizable.Visible = visible;
            if (visible)
            {
                this.BringToFront();
                m_btnSizable.BringToFront();                
            }
        }
    }
}
