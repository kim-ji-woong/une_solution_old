using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class ShapeBase
    {

        private Boolean m_Visible = true;
        virtual public Boolean Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                Visible = value;
            }
        }

        private int m_X = 0;
        virtual public int X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }


        private int m_Y = 0;
        virtual public int Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }


        private int m_Width = 0;
        virtual public int Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = Math.Max(0, value);
            }
        }


        private int m_Height = 0;
        virtual public int Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = Math.Max(0, value);
            }
        }

        virtual public Point Location
        {
            get
            {
                return new Point(m_X, m_Y);
            }
        }

        virtual public Size Size
        {
            get
            {
                return new Size(m_Width, m_Height);
            }
        }

        virtual public int Left
        {
            get
            {
                return m_X;
            }
        }

        virtual public int Top
        {
            get
            {
                return m_Y;
            }
        }
        virtual public int Right
        {
            get
            {
                return (m_X + m_Width);
            }
        }
        virtual public int Bottom
        {
            get
            {
                return (m_Y + m_Height);
            }
        }

        virtual public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(m_X, m_Y, m_Width, m_Height);
            }
        }

        virtual public Boolean Contains(int x, int y)
        {
            return true;
        }

        virtual public Boolean Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        virtual public void Paint(Graphics g)
        {

        }

        private Pen m_Pen = new Pen(Color.Black, 1);
        virtual public Pen Pen
        {
            get { return m_Pen; }
            set { m_Pen = value; }
        }

    }
}
