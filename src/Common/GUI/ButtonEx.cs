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
        public class ButtonEx : Button
        {
            private System.Drawing.Image m_imgExtra = null;
            private int x = 0;
            private int y = 0;
            private TextData m_text = null;

            public System.Drawing.Image ExtraImage
            {
                get { return m_imgExtra; }
                set { m_imgExtra = value; }
            }

            public int X
            {
                get { return x; }
                set { x = value; }
            }

            public int Y
            {
                get { return y; }
                set { y = value; }
            }

            public GUI.TextData TextData
            {
                get { return m_text; }
                set { m_text = value; }
            }

            public ButtonEx()
            {
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (m_imgExtra != null)
                    e.Graphics.DrawImage(m_imgExtra, x, y);

                if (m_text != null)
                    e.Graphics.DrawString(m_text.Text, m_text.Font, m_text.Brush, m_text.Rectangle, m_text.TextFormat);
            }
        }

        public class ImageButton : PictureBox
        {
            private Image m_imgNormal = null;
            private Image m_imgClicked = null;
            private Image m_imgMouseOver = null;

            protected bool m_isLClicked = false;
            protected bool m_isMouseOver = false;
            
            protected IImageButtonOwner m_owner = null;

            protected static SolidBrush m_defBrush = new SolidBrush(Color.Gray);
            protected SolidBrush m_ownBrush = new SolidBrush(Color.Black);
            protected System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            public Image ImageNormal
            {
                get { return m_imgNormal; }
                set { m_imgNormal = value; }
            }

            public Image ImageClicked
            {
                get { return m_imgClicked; }
                set { m_imgClicked = value; }
            }

            public Image ImageMouseOver
            {
                get { return m_imgMouseOver; }
                set { m_imgMouseOver = value; }
            }

            public IImageButtonOwner Owner
            {
                get { return m_owner; }
                set { m_owner = value; }
            }

            public Color TextColor
            {
                get { return m_ownBrush.Color; }
                set { m_ownBrush.Color = value; }
            }

            public System.Drawing.Font TextFont
            {
                get { return base.Font; }
                set { base.Font = value; }
            }

            public string ButtonText
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    base.Text = value;
                }
            }

            public ImageButton()
            {
                this.MouseDown += new MouseEventHandler(ImageButton_MouseDown);
                this.MouseUp += new MouseEventHandler(ImageButton_MouseUp);
                this.MouseEnter += new EventHandler(ImageButton_MouseEnter);
                this.MouseLeave += new EventHandler(ImageButton_MouseLeave);
            }

            private void ImageButton_MouseUp(object sender, MouseEventArgs e)
            {
                m_isLClicked = false;

                if (m_owner != null)
                    m_owner.OnImageButtonMouseUp(sender, e);

                Refresh();
            }

            private void ImageButton_MouseDown(object sender, MouseEventArgs e)
            {
                m_isLClicked = true;

                if (m_owner != null)
                    m_owner.OnImageButtonMouseDown(sender, e);

                Refresh();
            }

            private void ImageButton_MouseLeave(object sender, EventArgs e)
            {
                if (m_isMouseOver)
                {
                    m_isMouseOver = false;
                    Refresh();
                }
                else
                    m_isMouseOver = false;
            }

            private void ImageButton_MouseEnter(object sender, EventArgs e)
            {
                if (!m_isMouseOver)
                {
                    m_isMouseOver = true;
                    Refresh();
                }
                else
                    m_isMouseOver = true;
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                Image img = null;

                if (m_isLClicked)
                {
                    if (m_imgClicked != null)
                        img = m_imgClicked;
                    else
                        img = m_imgNormal;
                }
                else
                {
                    if (m_isMouseOver)
                        img = m_imgMouseOver;
                    else
                        img = m_imgNormal;
                }

                if (img != null)
                    pevent.Graphics.DrawImage(img, ClientRectangle);
                else
                    pevent.Graphics.FillRectangle(m_defBrush, ClientRectangle);
                
                if (Text.Length > 0)
                    pevent.Graphics.DrawString(Text, m_font, m_ownBrush, ClientRectangle, TextData.GetStringFormat());
            }
        }

        public class TextData
		{
			public enum TextPosition
			{
				BOTTOM = 1,
				RIGHT,
				NONE
			}
            private string m_strText = "";
            private System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            private System.Drawing.Brush m_brush = new System.Drawing.SolidBrush(Color.Black);
            private System.Drawing.Rectangle m_rect = new Rectangle();
            protected static StringFormat m_defTextFormat = GetStringFormat();
            private StringFormat m_textFormat;

            public TextData()
            {
                m_textFormat = m_defTextFormat;
            }

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public System.Drawing.Font Font
            {
                get { return m_font; }
                set { m_font = value; }
            }

            public System.Drawing.Brush Brush
            {
                get { return m_brush; }
                set { m_brush = value; }
            }

            public System.Drawing.Rectangle Rectangle
            {
                get { return m_rect; }
                set { m_rect = value; }
            }

            public StringFormat TextFormat
            {
                get { return m_textFormat; }
                set { m_textFormat = value; }
            }

            public static StringFormat GetStringFormat(TextPosition pos = TextPosition.BOTTOM)
            {
                StringFormat format = new StringFormat();

				if (pos == TextPosition.BOTTOM)
				{
					// Set the LineAlignment and Alignment properties for 
					// both StringFormat objects to different values.
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;
				}
				else if (pos == TextPosition.RIGHT)
				{
					format.LineAlignment = StringAlignment.Far;
					format.Alignment = StringAlignment.Near;
				}


                return format;
            }
        }

        public interface IImageButtonOwner
        {
            void OnImageButtonMouseDown(object sender, MouseEventArgs e);
            void OnImageButtonMouseUp(object sender, MouseEventArgs e);
        }
    }
}
