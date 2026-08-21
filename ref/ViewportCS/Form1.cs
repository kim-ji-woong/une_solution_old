using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public int VertexSize = 20;
        public ArrayList tempVert = new ArrayList();
        public Pen BOUNDARY_PEN = new Pen(Color.FromArgb(0, 0, 255), 1);

        // Transformation
        private Point m_ptScrCenter;
        private Point m_ptCurrent;
        private Point m_ptPrev;
        private PointF m_ptOrigin;

        private float m_fTranX;
        private float m_fTranY;

        private float m_fPrevScale = 1.0f;
        private float m_fCurScale = 1.0f;
 

        private bool m_bTranslation = false;

        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            m_ptScrCenter.X = this.ClientRectangle.Width / 2;
            m_ptScrCenter.Y = this.ClientRectangle.Height / 2;
        }

        void ScreenToGlobal(int x, int y, ref float gx, ref float gy)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            gx = (x / m_fCurScale - dx);
            gy = (y / m_fCurScale - dy);
        }

        PointF ScreenToGlobal(Point pt)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            float gx = (pt.X / m_fCurScale - dx);
            float gy = (pt.Y / m_fCurScale - dy);
            return new PointF(gx, gy);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_bTranslation == true)
            {
                m_ptCurrent.X = e.X;
                m_ptCurrent.Y = e.Y;
               
                Translate(m_ptPrev.X, m_ptPrev.Y, e.X, e.Y);
                
                m_ptPrev = m_ptCurrent;
                Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (m_bTranslation == true)
                {
                    m_bTranslation = false;
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            m_ptScrCenter.X = this.ClientRectangle.Width / 2;
            m_ptScrCenter.Y = this.ClientRectangle.Height / 2;
            
            // PAN
            if (e.Button == MouseButtons.Right)
            {
                m_bTranslation = true;
                m_ptPrev.X = e.X;
                m_ptPrev.Y = e.Y;
            }

            // ADD POINT
            if (e.Button == MouseButtons.Left)
            {
                float x = 0;
                float y = 0;
                ScreenToGlobal(e.X, e.Y, ref x, ref y);
                
                tempVert.Add(new PointF(x, y));                
                if (tempVert.Count > VertexSize)
                {
                    tempVert.RemoveAt(0);
                }
            }           
            Invalidate();
        }

        private void Translate(int prevX, int prevY, int x, int y)
        {
            m_ptOrigin.X += (x - prevX) ;
            m_ptOrigin.Y += (y - prevY) ;

            m_fTranX = m_ptOrigin.X;
            m_fTranY = m_ptOrigin.Y;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomIn(e.X, e.Y);
            }
            else
            {
                ZoomOut(e.X, e.Y);
            }
            Invalidate();
        }

        private void ZoomIn(int x, int y)
        {            
            if (m_fCurScale <= 10.0f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);
                 
                m_fCurScale = m_fCurScale * 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;

                m_fTranX += dx;
                m_fTranY += dy;

                m_fPrevScale = m_fCurScale;
            }            
        }

        private void ZoomOut(int x, int y)
        {           
            if (m_fCurScale > 0.01f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);

                m_fCurScale = m_fCurScale / 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;

                m_fTranX += dx;
                m_fTranY += dy;

                m_fPrevScale = m_fCurScale;
            }  
        }

    
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ResetTransform();
            e.Graphics.TranslateTransform(m_fTranX, m_fTranY);
            e.Graphics.ScaleTransform(m_fCurScale, m_fCurScale);   

            int nCount = tempVert.Count > VertexSize ? VertexSize : tempVert.Count;
            for (int i = 0; i < nCount; i++)
            {
                PointF pt = (PointF)tempVert[i];
                e.Graphics.DrawEllipse(BOUNDARY_PEN, pt.X - 5, pt.Y - 5, 10.0f, 10.0f);
            }
        }      
    }
}
