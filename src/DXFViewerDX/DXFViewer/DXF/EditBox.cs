using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class EditBox : IDrawableShape
    {
        protected DXFViewer.DXFControl m_Ctrl = null;

        protected RectangleF mRect = new SharpDX.RectangleF();

        protected SolidColorBrush mFillBrush = null;
        protected SolidColorBrush mRectBrush = null;

        private SharpDX.Color mFillColor = new SharpDX.Color();
        private SharpDX.Color mLineColor = new SharpDX.Color();
        
        public EditBox(DXFViewer.DXFControl ctrl)
        {
            m_Ctrl = ctrl;
        }

        public DXFDotNet.Shape GetShapeObject()
        {
            return null;
        }

        public bool CreateDXResource()
        {
            if (m_Ctrl == null)
                return false;

            RenderTarget g = m_Ctrl.RenderTarget;

            System.Drawing.Color fillColor = m_Ctrl.EditBoxBrush.Color;
            mFillColor.A = fillColor.A;
            mFillColor.R = fillColor.R;
            mFillColor.G = fillColor.G;
            mFillColor.B = fillColor.B;

            System.Drawing.Color lineColor = m_Ctrl.EditBoxPen.Color;
            mLineColor.A = lineColor.A;
            mLineColor.R = lineColor.R;
            mLineColor.G = lineColor.G;
            mLineColor.B = lineColor.B;

            mFillBrush = new SolidColorBrush(g, mFillColor);
            mRectBrush = new SolidColorBrush(g, mLineColor);

            return true;
        }

        public bool DiscardDXResource()
        {
            if (mFillBrush != null)
            {
                mFillBrush.Dispose();
                mFillBrush = null;
            }

            if (mRectBrush != null)
            {
                mRectBrush.Dispose();
                mRectBrush = null;
            }
            return true;
        }

        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            return true;
        }

        public bool Draw(SharpDX.Direct2D1.RenderTarget g, float x, float y)
        {
	        if (m_Ctrl == null)
		        return false;

            float fHalfSize = m_Ctrl.EditBoxLength / 2.0f;
            float fSize = m_Ctrl.EditBoxLength;

            mRect.Left = x - fHalfSize;
            mRect.Top = y + fHalfSize;
            mRect.Width = fSize;
            mRect.Height = fSize;

            float fLineWidth = SetScalePenWidth(m_Ctrl.EditBoxPen, g);

            g.FillRectangle(mRect, mFillBrush);
            g.DrawRectangle(mRect, mRectBrush, m_Ctrl.EditBoxPen.Width);

            m_Ctrl.EditBoxPen.Width = fLineWidth;	 

            return true;
        }

        private float SetScalePenWidth(System.Drawing.Pen brush, SharpDX.Direct2D1.RenderTarget g)
        {
            float fOldWidth = brush.Width;

            float fScaleX = g.Transform.M11;
            float fScaleY = g.Transform.M22;

            float fLineWidth = 1.0f / fScaleX * fOldWidth;
            brush.Width = fLineWidth;

            return fOldWidth;
        }        

       
    }
}
