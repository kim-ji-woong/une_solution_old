using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace UnE
{
    namespace GUI
    {
        public class RibbonButton : Button
        {
			public enum TextPosition
			{
				BOTTOM = 1,
				RIGHT,
				NONE
			}

            private Image m_imgNormal = null;
            private Image m_imgChecked = null;
            private Image m_imgDisabled = null;
            private Image m_imgMouseOverBkgnd = null;
            private Image m_imgCheckedBkgnd = null;
            private Image m_imgDisabledBkgnd = null;
            protected bool m_isChecked = false;
            
            protected System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            protected System.Drawing.Brush m_brush = new System.Drawing.SolidBrush(Color.White);
            protected static StringFormat m_textFormat = TextData.GetStringFormat();

            protected bool m_isLClicked = false;
            protected bool m_isMouseOver = false;

            protected IRibbonButtonOwner m_owner = null;

            private static int m_nOriginInitButtonWidth = 60;

            public static int OriginInitButtonWidth
            {
                get { return m_nOriginInitButtonWidth; }
                set { m_nOriginInitButtonWidth = value; }
            }

            private int m_nInitButtonWidth = -1;

            public int InitButtonWidth
            {
                get { return m_nInitButtonWidth; }
                set { m_nInitButtonWidth = value; }
            }

			protected Font m_Font = null;
			public new virtual System.Drawing.Font Font
			{
				get { return m_Font; }
				set 
				{
					m_Font = value;
					UpdateTextRect();
				}
			}

            // 이미지가 그려질 위치와 크기를 User가 지정할 것인지 여부
            protected bool m_useCustomImageRect = false;
            public bool UseCustomImageRect
            {
                get { return m_useCustomImageRect; }
                set { m_useCustomImageRect = value; }
            }

            protected Rectangle m_rectCustomImage = new Rectangle(0, 0, 32, 32);
            public Rectangle CustomImageRect
            {
                get { return m_rectCustomImage; }
                set { m_rectCustomImage = value; }
            }

			protected TextPosition m_textPos = TextPosition.BOTTOM;
			public UnE.GUI.RibbonButton.TextPosition TextPos
			{
				get { return m_textPos; }
				set 
				{
					m_textPos = value;
					if( m_textPos == TextPosition.RIGHT)
						m_textFormat = TextData.GetStringFormat(TextData.TextPosition.RIGHT);
					if (m_textPos == TextPosition.BOTTOM)
						m_textFormat = TextData.GetStringFormat(TextData.TextPosition.BOTTOM);
					UpdateTextRect();
				}
			}

			private void UpdateTextRect()
			{
				Font font = (m_Font == null ? m_font : m_Font);
				Graphics g = this.CreateGraphics();
				SizeF size = g.MeasureString(base.Text, font);

				this.Size = new Size(m_nInitButtonWidth, this.Size.Height);

				if ((int)size.Width + 3 > this.Size.Width)
				{
					this.Size = new Size((int)size.Width + 3, this.Size.Height);
				}
				if (m_textPos == TextPosition.BOTTOM)
				{
					m_rect = new Rectangle(0, this.Size.Height - (int)size.Height - 8, this.Size.Width, (int)size.Height);					
				}
				else if (m_textPos == TextPosition.RIGHT)
				{
					int nAddSize = 10;
					if (m_imgNormal != null)
					{
						nAddSize += m_imgNormal.Width;
					}
					int x = this.Size.Width / 5 + nAddSize;
					int y = (this.Size.Height - (int)size.Height) / 2;
					//m_rect = new Rectangle(x, y , this.Size.Width - x, (int)size.Height);

                    int width = size.Width > (int)size.Width ? (int)size.Width + 1 : (int)size.Width;
                    m_rect = new Rectangle(x, y, width, (int)size.Height);
				}
			}
			
			protected Brush m_Brush = null;
			public override Color ForeColor
			{
				get { return base.ForeColor; }
				set
				{
					base.ForeColor = value;
					m_Brush = new SolidBrush(value);
				}
			}

            protected System.Drawing.Rectangle m_rect = new Rectangle();
            public Rectangle TextRect
            {
                get { return m_rect; }
            }

			protected Point m_ptTextLocation = new Point();
			public System.Drawing.Point TextLocation
			{
				get { return m_ptTextLocation; }
				set
				{
					m_ptTextLocation = value;
					//SetTextLocation(m_ptTextLocation.X, m_ptTextLocation.Y);
				}
			}

			private bool m_bUseTextLocation = false;
			public bool UseTextLocation
			{
				get { return m_bUseTextLocation; }
				set { m_bUseTextLocation = value; }
			}
			

            public Image NormalImage
            {
                get { return m_imgNormal; }
                set 
				{ 
					m_imgNormal = value;
					UpdateTextRect();
				}
            }

            public Image CheckedImage
            {
                get { return m_imgChecked; }
                set { m_imgChecked = value; }
            }

            public Image DisabledImage
            {
                get { return m_imgDisabled; }
                set { m_imgDisabled = value; }
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

            public Image DisabledBkgndImage
            {
                get { return m_imgDisabledBkgnd; }
                set { m_imgDisabledBkgnd = value; }
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

            public override string Text
            {
                get { return base.Text; }
                set
                {
                    base.Text = value;
					UpdateTextRect();                    
                }
            }

			protected int m_nID = -1;
			public int ID
			{
				get { return m_nID; }
				set { m_nID = value; }
			}

            public RibbonButton()
            {
                m_nInitButtonWidth = m_nOriginInitButtonWidth;
                
                this.MouseUp += new MouseEventHandler(RibbonButton_MouseUp);
                this.MouseDown += new MouseEventHandler(RibbonButton_MouseDown);
                this.MouseEnter += new EventHandler(RibbonButton_MouseEnter);
                this.MouseLeave += new EventHandler(RibbonButton_MouseLeave);
            }

            public RibbonButton(int nInitButtonWidth)
            {
                m_nInitButtonWidth = nInitButtonWidth;

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

				Font font = (m_Font == null ? m_font : m_Font);
				Brush brush = (m_Brush == null ? m_brush : m_Brush);

                if (Enabled)
                {
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
                            {
                                pevent.Graphics.DrawImage(m_imgMouseOverBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                            }
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
                }
                else
                {
                    if (m_imgDisabledBkgnd != null)
                        pevent.Graphics.DrawImage(m_imgDisabledBkgnd, 0, 0, this.Size.Width, this.Size.Height);

                    if (m_imgDisabled != null)
                        DrawImage(m_imgDisabled, pevent.Graphics);
                    else if (m_imgNormal != null)
                        DrawImage(m_imgNormal, pevent.Graphics);
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
						StringFormat format = TextData.GetStringFormat();
						pevent.Graphics.DrawString(Text, font, brush, m_rect, format);
					}
					
                }
            }

            protected virtual void DrawImage(Image img, Graphics g)
            {
                if (UseCustomImageRect)
                {
                    g.DrawImage(img, m_rectCustomImage);
                }
                else
                {
                    if (m_textPos == TextPosition.BOTTOM)
                    {
                        int x = (this.Size.Width - img.Width) / 2;

                        int y = 5;

                        if (this.Text == "")
                            y = (this.Size.Height - img.Height) / 2;
                        g.DrawImage(img, x, y);
                    }
                    else if (m_textPos == TextPosition.RIGHT)
                    {

                        int x = this.Size.Width / 5;
                        if (this.Text == "")
                            x = (this.Size.Width - img.Width) / 2;

                        int y = (this.Size.Height - img.Height) / 2;


                        g.DrawImage(img, x, y);
                    }
                }
            }

            public void SetTextLocation(int x, int y)
            {
                m_rect.X = x;
                m_rect.Y = y;
            }
        }

        public interface IRibbonButtonOwner
        {
            void OnRibbonButtonMouseDown(object sender, MouseEventArgs e);
            void OnRibbonButtonMouseUp(object sender, MouseEventArgs e);
        }
    }
}
