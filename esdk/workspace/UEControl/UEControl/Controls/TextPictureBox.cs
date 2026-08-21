using System.Drawing;
using System.Windows.Forms;

namespace UnE
{
	namespace GUI
	{
		public class TextPictureBox : PictureBox
		{
			protected string m_strText = "";
			protected ITextPictureBoxOwner m_owner = null;
			protected Font TEXT_FONT = new Font("맑은고딕", 9, FontStyle.Bold);
			protected static StringFormat m_textFormat = GetStringFormat();
			protected Brush m_brushText = new SolidBrush(Color.FromArgb(255, 255, 255));

			protected Font m_Font = null;
			public new virtual System.Drawing.Font Font
			{
				get { return m_Font; }
				set { m_Font = value; }
			}


			protected Brush m_Brush = null;
			public override Color ForeColor
			{
				get { return base.BackColor; }
				set
				{
					base.BackColor = value;
					m_Brush = new SolidBrush(value);
				}
			}

			public Color TextColor
			{
				get
				{
					if (m_Brush == null)
						return ((SolidBrush)m_brushText).Color;
					return ((SolidBrush)m_Brush).Color;
				}
				set
				{
					if (m_Brush == null)
						m_Brush = new SolidBrush(value);
					else
						((SolidBrush)m_Brush).Color = value;
				}
			}

			public ITextPictureBoxOwner Owner
			{
				get { return m_owner; }
				set { m_owner = value; }
			}


			public override string Text
			{
				get { return m_strText; }
				set { m_strText = value; }
			}

			public string PictureBoxText
			{
				get { return m_strText; }
				set { m_strText = value; }
			}

			public TextPictureBox()
			{
				BackColor = Color.FromArgb(255, 255, 255);
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

			public void SetPictureBoxOwner(ITextPictureBoxOwner owner)
			{
				m_owner = owner;
			}

			protected override void OnPaint(PaintEventArgs pe)
			{
				Font font = (m_Font == null ? TEXT_FONT : m_Font);
				Brush brush = (m_Brush == null ? m_brushText : m_Brush);
				pe.Graphics.DrawString(m_strText, font, brush, this.ClientRectangle, m_textFormat);
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				if (m_owner != null)
					m_owner.TextPictureBox_MouseDown(this, e);
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				if (m_owner != null)
					m_owner.TextPictureBox_MouseUp(this, e);
			}
		}

		public interface ITextPictureBoxOwner
		{
			void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e);
			void TextPictureBox_MouseUp(TextPictureBox pictureBox, MouseEventArgs e);
		}
	}
}
