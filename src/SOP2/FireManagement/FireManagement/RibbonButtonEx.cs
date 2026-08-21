using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using UnE.GUI;

namespace FireManagement
{
    public class RibbonButtonMenu : UnE.GUI.RibbonButton
    {
        public RibbonButtonMenu() : base()
        {
            
        }

        public RibbonButtonMenu(int nInitButtonWidth) : base(nInitButtonWidth)
        {
            
        }


        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                m_rect.Y = m_rect.Y - 30;
            }
        }
    }

    public class RibbonButtonClickButton : UnE.GUI.RibbonButton
    {
        public RibbonButtonClickButton()
            : base()
        {
            m_brush = new System.Drawing.SolidBrush(Color.Black);
            m_font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }
        public RibbonButtonClickButton(int nInitButtonWidth)
            : base(nInitButtonWidth)
        {
        }
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                m_rect.Y = m_rect.Y - (m_rect.Y/2);
            }
        }


        protected override void DrawImage(Image img, Graphics g)
        {
            int x = (this.Size.Width - img.Width) / 2;
            int y = 10;

            g.DrawImage(img, x, y);
        }
    }

    public class RibbonButtonFireManagement : UnE.GUI.RibbonButton
    {
        public RibbonButtonFireManagement() : base()
        {
        }

        public RibbonButtonFireManagement(int nInitButtonWidth)
            : base(nInitButtonWidth)
        {
        }

        protected override void DrawImage(Image img, Graphics g)
        {
            int x = (this.Size.Width - img.Width) / 2;
            int y = 17;

            g.DrawImage(img, x, y);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {

            base.OnPaintBackground(pevent);

            Font font = (m_Font == null ? m_font : m_Font);
            Brush brush = (m_Brush == null ? m_brush : m_Brush);

            if (Enabled)
            {
                if (m_isChecked)
                {
                    if (CheckedBkgndImage != null)
                        pevent.Graphics.DrawImage(CheckedBkgndImage, 0, 0, this.Size.Width, this.Size.Height);
                }
                else
                {
                    if (m_isMouseOver && !m_isLClicked)
                    {
                        if (MouseOverBkgndImage != null)
                        {
                            //pevent.Graphics.DrawImage(MouseOverBkgndImage, 0, 0, this.Size.Width, this.Size.Height);
                        }
                    }
                }

                //base.OnPaint(pevent);

                if (m_isChecked)
                {
                    if (CheckedImage != null)
                    {
                        DrawImage(CheckedImage, pevent.Graphics);
                    }
                }
                else
                {
                    if (!m_isMouseOver)
                    {
                        if (NormalImage != null)
                            pevent.Graphics.DrawImage(NormalImage, 0, 0, this.Size.Width, this.Size.Height);
                    }
                    else
                        pevent.Graphics.DrawImage(MouseOverBkgndImage, 0, 0, this.Size.Width, this.Size.Height);
                }
            }
            else
            {
                if (DisabledBkgndImage != null)
                    pevent.Graphics.DrawImage(DisabledBkgndImage, 0, 0, this.Size.Width, this.Size.Height);

                if (DisabledImage != null)
                    DrawImage(DisabledImage, pevent.Graphics);
                else if (NormalImage != null)
                    DrawImage(NormalImage, pevent.Graphics);
            }

            if (Text.Length > 0)
            {
                if (UseTextLocation)
                {
                    Rectangle f = new Rectangle(m_ptTextLocation, m_rect.Size);
                    pevent.Graphics.DrawString(Text, font, brush, f, m_textFormat);
                }
                else
                {
                    pevent.Graphics.DrawString(Text, font, brush, m_rect, m_textFormat);
                }
					
                
            }

        }
    }
}
