using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Sections
{
    public enum NotifierDrawMode
    {
        RECT = 1,
        ARC = 2,
        POLYGON = 3,
        PATH = 4,
    }
    public delegate void OnRefreshEvent();

    public class GradientNotifier
    {
            
        private List<Color> mColorList = null;
        private Timer timer = null;
        private bool mSelected = false;
        public event OnRefreshEvent OnRefresh;
        public bool Selected
        {
            get { return mSelected; }
            set { 
                mSelected = value;
                if( m_Painter != null)
                    m_Painter.Selected = value;
                //if( mSelected == true)
                //{
                if (OnRefresh != null && Parent != null)
                    {
                        Parent.Invoke(OnRefresh);
                    }
                //}                   
            }
        }
        private bool drawGrad = true;
        public bool DrawGradient
        {
            get { return drawGrad; }
            set { drawGrad = value; }
        }
        private PointF[] m_Verts = null;
        public System.Drawing.PointF[] VertexList
        {
            get 
            {
                return m_Verts;
            }
            set
            {
                if (nMode != NotifierDrawMode.POLYGON)
                    m_Verts = null;
                else
                {
                    m_Verts = value;                       
                }
            }
        }

        private NotifierDrawMode nMode = NotifierDrawMode.RECT;
        public NotifierDrawMode Mode
        {
            get { return nMode; }
            set { nMode = value; }
        }
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
            set
            {
                mColorBegin = value;
                mColorList = GetGradientColors(mColorBegin, mColorEnd, nGradStep);
            }
        }

        Color mColorEnd = Color.White;
        public System.Drawing.Color EndColor
        {
            get { return mColorEnd; }
            set
            {
                mColorEnd = value;
                mColorList = GetGradientColors(mColorBegin, mColorEnd, nGradStep);
            }
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
            set {
                parent = value;
                OnRefresh += new OnRefreshEvent(parent.Refresh);
            }
        }

        private bool bEnable = true;
        public bool Enabled
        {
            get { return bEnable; }
            set {
                bEnable = value;
                if (m_Painter != null)
                    m_Painter.EnableBlink(bEnable);
                if (bEnable == false)
                {

                    mSelected = false;
                    //t/imer.Enabled = false;

                    if (OnRefresh != null && Parent!= null)
                    {
                        Parent.Invoke(OnRefresh);
                    }
                }                    
            }
        }

        private ImagePainter m_Painter = null;
        public ImagePainter Painter
        {
            get { return m_Painter; }
            set { m_Painter = value; }
        }
            
        public GradientNotifier(Timer t)
        {
            timer = t;
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = nInterval;
                timer.Start();
            }
            timer.Tick += new EventHandler(OnTimer);

            mColorList = GetGradientColors(mColorBegin, mColorEnd, nGradStep);
        }
            
        public void Paint(Graphics dc)
        {
            if (mColorList == null)
                return;
               
            if (mSelected == true && drawGrad == true && bEnable == true)
            {                    
                float tx = mX;
                float ty = mY;
                    
                if( nMode == NotifierDrawMode.PATH)
                {
                    // draw Path
                }
                else
                {
                    for (int i = 1; i < nGradStep; i++)
                    {
                        int n = nGradStep - i;
                        Pen polyPen = new Pen(mColorList[n], n);

                        if (nMode == NotifierDrawMode.RECT)
                        {
                            dc.DrawRectangle(polyPen, mX, mY, mSize.Width, mSize.Height);
                        }
                        else if (nMode == NotifierDrawMode.ARC)
                        {
                            dc.DrawEllipse(polyPen, mX, mY, mSize.Width, mSize.Height);
                        }

                        else if (nMode == NotifierDrawMode.POLYGON)
                        {
                            if (m_Verts != null)
                            {
                                dc.DrawPolygon(polyPen, m_Verts);
                            }

                        }
                        polyPen.Dispose();
                    }
                }
                
            }
        }

        void OnTimer(object sender, System.EventArgs e)
        {
            drawGrad = !drawGrad;
            if (m_Painter != null)
                m_Painter.DrawOutImage(drawGrad);
            if (parent != null && OnRefresh != null)
            {
                
                if (mSelected == true)
                {
                     Parent.Invoke(OnRefresh);                    
                }
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

        public void Notify()
        {
            mSelected = !mSelected;
            //if (mSelected == true)
            //    timer.Enabled = true;
            //else
            //    timer.Enabled = false;

            if (parent != null)
            {
                parent.Refresh();
            }
        }

        public void SetPosition(int x, int y)
        {
            mX = x;
            mY = y;
        }
    }
}
