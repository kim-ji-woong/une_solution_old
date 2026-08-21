using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class ShapeLine : ShapeBase
    {
        public ShapeLine()
        {
            this.X = 10;
            this.Y = 10;
            this.X2 = 20;
            this.Y2 = 10;
            this.Pen = new Pen(Color.Black, 2);
            //this.m_Thick = 2;
        }

        public ShapeLine(int x, int y, int x2, int y2, Color color, int nThick)
        {
            this.X = x;
            this.Y = y;
            this.X2 = x2;
            this.Y2 = y2;
            //this.Pen = pen;
            //this.m_Thick = nThick;
            this.Pen = new Pen(color, nThick);
            Pen.StartCap = Pen.EndCap = LineCap.Round;
        }

        public ShapeLine(Point pt, Point pt2, Color color, int nThick)
        {
            this.X = pt.X;
            this.Y = pt.Y;
            this.X2 = pt2.X;
            this.Y2 = pt2.Y;
            this.Pen = new Pen(color, nThick);
            Pen.StartCap = Pen.EndCap = LineCap.Round;
            //this.m_Thick = nThick;
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

        private int m_Thick = 2;

        public int Thick
        {
            get { return m_Thick; }
            set { m_Thick = value; }
        }

        private float m_Length = 0;

       // 길이
        public float Length
        {
            get
            {
                float a = Math.Abs(this.X2 - this.X);
                float b = Math.Abs(this.Y2 - this.Y);
                double c = Math.Sqrt((double)(a + b));
                m_Length = (float)c;

                return m_Length;
            }
        }


    }
}
