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
   

    public class PathNotifier
    {

        private List<Color> mColorList = null;
        private Timer timer = null;
        private bool mSelected = false;
        public event OnRefreshEvent OnRefresh;
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                if (m_Painter != null)
                    m_Painter.Selected = value;

                if (OnRefresh != null && Parent != null && Parent.Parent != null)
                {
                    Parent.Invoke(OnRefresh);
                }                 
            }
        }
        private bool drawGrad = true;
        public bool DrawGradient
        {
            get { return drawGrad; }
            set { drawGrad = value; }
        }
 
        private System.Drawing.Drawing2D.GraphicsPath m_path = null;
        public System.Drawing.Drawing2D.GraphicsPath Path
        {
            get { return m_path; }
            set 
            {
                nMode = NotifierDrawMode.PATH;
                m_path = value; 
            }
        }

        private NotifierDrawMode nMode = NotifierDrawMode.PATH;
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

     
        private static Color m_pathColor = Color.Red;
        public static Color PathColor
        {
            get { return PathNotifier.m_pathColor; }
            set { PathNotifier.m_pathColor = value; }
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
            set
            {
                parent = value;
                OnRefresh += new OnRefreshEvent(parent.Refresh);
            }
        }

        private bool bEnable = true;
        public bool Enabled
        {
            get { return bEnable; }
            set
            {
                bEnable = value;
                if (m_Painter != null)
                    m_Painter.EnableBlink(bEnable);
                if (bEnable == false)
                {
                    mSelected = false;

                    if (OnRefresh != null && Parent != null)
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

        public PathNotifier(Timer t)
        {
            timer = t;
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = nInterval;
                timer.Start();
            }
            timer.Tick += new EventHandler(OnTimer);            
        }

        public void Paint(Graphics dc)
        {
            if (mSelected == true && drawGrad == true && bEnable == true)
            {
                float tx = mX;
                float ty = mY;

                if (nMode == NotifierDrawMode.PATH)
                {
                    if(m_path != null)
                    {
                        // draw Path
                        using(Pen pen = new Pen(m_pathColor))
                        {
                            pen.Width = Shape.OutLineThick;
                            dc.DrawPath(pen, m_path);
                        } 
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
        
        public void Notify()
        {
            mSelected = !mSelected;           

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
