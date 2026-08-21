using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class GradientNotifier
    {
        private List<Color> mColorList = null;
        private Timer timer = new Timer();
        private bool mSelected = false;
        private bool drawGrad = true;
        
        private int nInterval = 500;
        public int Interval
        {
            get { return nInterval; }
            set { nInterval = value; }
        }

        private float nThick = 0.3f;
        public float Thick
        {
            get { return nThick; }
            set { nThick = value; }
        }

        private int nGradStep = 20;
        public int GradientStep
        {
            get { return nGradStep; }
            set 
            { 
                nGradStep = value;
                mColorList = GetGradientColors(mColorBegin, mColorEnd, nGradStep); 
            }
        }

        Color mColorBegin = Color.FromArgb(233, 0, 0);
        public System.Drawing.Color BeginColor
        {
            get { return mColorBegin; }
            set { mColorBegin = value; }
        }

        Color mColorEnd = Color.White;
        public System.Drawing.Color EndColor
        {
            get { return mColorEnd; }
            set { mColorEnd = value; }
        }

        private Size mSize = new Size();
        public System.Drawing.Size Size
        {
            get { return mSize; }
            set { mSize = value; }
        }

        private int mX = 0;
        public int X
        {
            get { return mX; }
            set { mX = value; }
        }
        private int mY = 0;
        public int Y
        {
            get { return mY; }
            set { mY = value; }
        }

        private Control parent = null;
        public System.Windows.Forms.Control Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        
        public GradientNotifier()
        {
            timer.Interval = nInterval; 
            timer.Tick += new EventHandler(OnTimer);
            mColorList = GetGradientColors(mColorBegin, mColorEnd, nGradStep);            
        }
        
        public void OnPaint(PaintEventArgs e)
        {
            if (mColorList == null)
                return;
            Graphics dc = e.Graphics;

            if (mSelected == true && drawGrad == true)
            {
                for (int i = 1; i < nGradStep; i++)
                {
                    Pen blurPen = new Pen(mColorList[i], nThick);
                    dc.DrawRectangle(blurPen, mX - nThick * i, mY - nThick * i, mSize.Width + 2 * (nThick * i), mSize.Width + 2 * (nThick * i));
                } 
            }
        }

        void OnTimer(object sender, System.EventArgs e)
        {
            drawGrad = !drawGrad;
            if (parent != null)
            {
                parent.Refresh();
            }

        }     
   

        public static List<Color> GetGradientColors(Color start, Color end, int steps)
        {
            return GetGradientColors(start, end, steps, 0, steps);
        }

        public static List<Color> GetGradientColors(Color start, Color end, int steps, int firstStep, int lastStep)
        {
            var colorList = new List<Color>();
            if (steps <= 0 || firstStep < 0 || lastStep > steps)
                return colorList;

            double aStep = (double)(end.A - start.A) / (double)steps;
            double rStep = (double)(end.R - start.R) / (double)steps;
            double gStep = (double)(end.G - start.G) / (double)steps;
            double bStep = (double)(end.B - start.B) / (double)steps;

            for (int i = firstStep; i < lastStep; i++)
            {
                var a = start.A + (int)(aStep * i);
                var r = start.R + (int)(rStep * i);
                var g = start.G + (int)(gStep * i);
                var b = start.B + (int)(bStep * i);
                colorList.Add(Color.FromArgb(a, r, g, b));
            }
            return colorList;
        }

        public void Select()
        {
            mSelected = !mSelected;
            if (mSelected == true)
                timer.Enabled = true;
            else
                timer.Enabled = false;

            if (parent != null)
            {
                parent.Refresh();
            }
        }

        public void SetPosition( int x, int y )
        {
            mX = x;
            mY = y;
        }
    }
}
