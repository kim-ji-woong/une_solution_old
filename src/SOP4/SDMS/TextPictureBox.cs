using System.Drawing;
using System.Windows.Forms;

namespace SDMS
{
	public class TextPictureBox : PictureBox
	{
		private string m_strText = "";
		private ITextPictureBoxOwner m_owner = null;

		protected static Font TEXT_FONT = new Font("맑은고딕", 9, FontStyle.Bold);
		protected static StringFormat m_textFormat = GetStringFormat();
		protected static Brush m_brushText = new SolidBrush(Color.FromArgb(255, 255, 255));

		public override string Text
		{
			get { return m_strText; }
			set { m_strText = value; }
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
			pe.Graphics.DrawString(m_strText, TEXT_FONT, m_brushText, this.ClientRectangle, m_textFormat);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (m_owner != null)
				m_owner.TextPictureBox_MouseDown(this, e);
		}
	}

	public interface ITextPictureBoxOwner
	{
		void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e);
	}
}