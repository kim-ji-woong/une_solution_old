using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class ShapeText : ShapeBase
    {
        public ShapeText()
        {
            this.X = 10;
            this.Y = 10;
            this.X2 = 20;
            this.Y2 = 10;
        }

        public ShapeText(int x, int y, string text, Pen pen, string strTextFont, int nTextSize, FontStyle style)
        {
            this.X = x;
            this.Y = y;
            this.m_text = text;
            this.Pen = pen;
            this.m_DrawFont = new Font(strTextFont, nTextSize, style);
            this.DrawFontStlye = style;
        }

        public ShapeText(Point pt, string text, Pen pen, string strTextFont, int nTextSize, FontStyle style)
        {
            this.X = pt.X;
            this.Y = pt.Y;
            this.m_text = text;
            this.Pen = pen;
            this.m_DrawFont = new Font(strTextFont, nTextSize,style);
            this.DrawFontStlye = style;
        }


        private int m_X2 = 0;
        /// <summary>
        /// // gets or sets
        /// // X2 좌표
        /// </summary>
        public int X2
        {
            get
            {
                return m_X2;
            }
            set
            {
                m_X2 = value;
            }
        }


        private int m_Y2 = 0;
        /// <summary>
        /// // gets or sets
        /// // Y2 좌표
        /// </summary>
        public int Y2
        {
            get
            {
                return m_Y2;
            }
            set
            {
                m_Y2 = value;
            }
        }

        private Font m_DrawFont = new Font("맑은고딕", 10);
        public Font DrawFont
        {
            get { return m_DrawFont; }
            set { m_DrawFont = value; }
        }
        private FontStyle m_DrawFontStlye = FontStyle.Regular;
        public FontStyle DrawFontStlye
        {
            get { return m_DrawFontStlye; }
            set { m_DrawFontStlye = value; }
        }

        /// <summary>
        /// // gets
        /// // Location2
        /// </summary>
        public Point Location2
        {
            get
            {
                return new Point(m_X2, m_Y2);
            }
        }

        private string m_text = "";
        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }
    }
}
