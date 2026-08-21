using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace libSplash
{
    public class TextData
    {
        private Color m_textColor = Color.White;
        private Font m_font = new Font("맑은 고딕", 22.0f, FontStyle.Bold);
        private string m_strText = "";
        private Rectangle m_rect = new Rectangle(198, 84, 200, 40);
        private StringFormat m_fmt = new StringFormat(StringFormat.GenericTypographic);
        private bool m_visible = true;

        public Color TextColor
        {
            get { return m_textColor; }
            set { m_textColor = value; }
        }

        public Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public Rectangle Rectangle
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        public StringFormat StringFormat
        {
            get { return m_fmt; }
            set { m_fmt = value; }
        }

        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public TextData()
        {
        }

        public TextData(string strText)
        {
            m_strText = strText;
        }

        public TextData(string strText, int x, int y, int width, int height)
        {
            m_strText = strText;
            m_rect = new Rectangle(x, y, width, height);
        }

        public void OnPaint(Graphics g, Color color)
        {
            if (m_visible == false)
                return;

            using (var br = new SolidBrush(color))
            {
                g.DrawString(m_strText, m_font, br, m_rect, m_fmt);
            }
        }

        /*public void OnPaint(Graphics g, int nDrawingCount, int nFrameCount)
        {
            if (m_visible == false)
                return;

            using (var br = new SolidBrush(Color.FromArgb(nDrawingCount * 255 / nFrameCount, m_textColor)))
            {
                g.DrawString(m_strText, m_font, br, m_rect, m_fmt);
            }
        }*/
    }
}
