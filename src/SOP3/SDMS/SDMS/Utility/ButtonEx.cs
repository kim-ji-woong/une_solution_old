using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SDMS
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

        public SDMS.TextData TextData
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

    public class TextData
    {
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
}
