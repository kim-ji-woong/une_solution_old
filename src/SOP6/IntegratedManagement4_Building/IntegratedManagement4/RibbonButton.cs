using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace IntegratedManagement4
{
    public class RibbonButton : Button
    {
        private Image m_imgNormal = null;
        private Image m_imgChecked = null;
        private Image m_imgMouseOverBkgnd = null;
        private Image m_imgCheckedBkgnd = null;
        private bool m_isChecked = false;
        //private string m_strTitle = "";

        private static System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private static System.Drawing.Brush m_brush = new System.Drawing.SolidBrush(Color.FromArgb(154, 159, 164));
        protected static StringFormat m_textFormat = GetStringFormat();

        private bool m_isLClicked = false;
        private bool m_isMouseOver = false;

        private IRibbonButtonOwner m_owner = null;

        private System.Drawing.Rectangle m_rect = new Rectangle();
        
        public Image NormalImage
        {
            get { return m_imgNormal; }
            set { m_imgNormal = value; }
        }

        public Image CheckedImage
        {
            get { return m_imgChecked; }
            set { m_imgChecked = value; }
        }

        public Image MouseOverBkgndImage
        {
            get { return m_imgMouseOverBkgnd; }
            set { m_imgMouseOverBkgnd = value; }
        }

        public Image CheckedBkgndImage
        {
            get { return m_imgCheckedBkgnd; }
            set { m_imgCheckedBkgnd = value; }
        }

        public bool IsChecked
        {
            get { return m_isChecked; }
            set { m_isChecked = value; }
        }

        public IRibbonButtonOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        /*public string Title
        {
            get { return m_strTitle; }
            set
            {
                m_strTitle = value;
                
                Graphics g = this.CreateGraphics();
                SizeF size = g.MeasureString(m_strTitle, m_font);

                this.Size = new Size(60, this.Size.Height);

                if ((int)size.Width + 3 > this.Size.Width)
                {
                    this.Size = new Size((int)size.Width + 3, this.Size.Height);
                }

                m_rect = new Rectangle(0, this.Size.Height - (int)size.Height - 15, this.Size.Width, (int)size.Height);
            }
        }*/

        public RibbonButton()
        {
            this.MouseUp += new MouseEventHandler(RibbonButton_MouseUp);
            this.MouseDown += new MouseEventHandler(RibbonButton_MouseDown);
            this.MouseEnter += new EventHandler(RibbonButton_MouseEnter);
            this.MouseLeave += new EventHandler(RibbonButton_MouseLeave);
        }

        void RibbonButton_MouseLeave(object sender, EventArgs e)
        {
            m_isMouseOver = false;
        }

        void RibbonButton_MouseEnter(object sender, EventArgs e)
        {
            m_isMouseOver = true;
        }
        
        void RibbonButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_isLClicked = true;

            if (m_owner != null)
                m_owner.OnRibbonButtonMouseDown(sender, e);
        }

        void RibbonButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_isLClicked = false;

            if (m_owner != null)
                m_owner.OnRibbonButtonMouseUp(sender, e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            if (m_isChecked)
            {
                if (m_imgCheckedBkgnd != null)
                    pevent.Graphics.DrawImage(m_imgCheckedBkgnd, 0, 0, this.Size.Width, this.Size.Height);
            }
            else
            {
                if (m_isMouseOver && !m_isLClicked)
                {
                    if (m_imgMouseOverBkgnd != null)
                        pevent.Graphics.DrawImage(m_imgMouseOverBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                }
            }

            //base.OnPaint(pevent);

            if (m_isChecked)
            {
                if (m_imgChecked != null)
                    DrawImage(m_imgChecked, pevent.Graphics);
                else if (m_imgNormal != null)
                    DrawImage(m_imgNormal, pevent.Graphics);
            }
            else
            {
                if (m_imgNormal != null)
                    DrawImage(m_imgNormal, pevent.Graphics);
            }

            if (this.Text.Length > 0)
            {
                //pevent.Graphics.DrawString(m_strTitle, m_font, m_brush, m_rect, m_textFormat);
                pevent.Graphics.DrawString(this.Text, m_font, m_brush, this.ClientRectangle, m_textFormat);
            }
        }

        private void DrawImage(Image img, Graphics g)
        {
            //int x = (this.Size.Width - img.Width) / 2;
            //int y = 5;

            //g.DrawImage(img, x, y);
            g.DrawImage(img, this.ClientRectangle);
        }

        public static StringFormat GetStringFormat()
        {
            StringFormat format = new StringFormat();

            // Set the LineAlignment and Alignment properties for 
            // both StringFormat objects to different values.
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            return format;
        }
    }

    public interface IRibbonButtonOwner
    {
        void OnRibbonButtonMouseDown(object sender, MouseEventArgs e);
        void OnRibbonButtonMouseUp(object sender, MouseEventArgs e);
    }
}
