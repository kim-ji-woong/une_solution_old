using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using System.Drawing;

namespace SoilMan
{
	public delegate void OnOverlayObjectPaint(System.Windows.Forms.PaintEventArgs e);
	public delegate void OnSelectRectOverlayPaint(System.Windows.Forms.PaintEventArgs e);
	public delegate void OnUnderlayPrePaint(System.Windows.Forms.PaintEventArgs e);

    public class DXFExternPainter : ExternalPainter
    {
        public event OnOverlayObjectPaint OverlayObjectPainter;
        public event OnSelectRectOverlayPaint SelectRectOverlayPaint;
		public event OnUnderlayPrePaint UnderlayPrePainter;       

        private Shape m_selectedShape = null;

        private DXFControl mTargetControl = null;
        public DXFExternPainter(DXFControl ctrl)
        {
            mTargetControl = ctrl;
        }

        public override void OnPrevPaint(System.Drawing.Graphics g, bool m_bDrawText)
        //public override void OnPrevPaint(System.Windows.Forms.PaintEventArgs e)
        {
			if(UnderlayPrePainter!= null)
			{
                System.Drawing.Size size = mTargetControl.Size;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
                System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);
				UnderlayPrePainter(e);
			}
        }

        public override void OnPostPaint(System.Drawing.Graphics g, bool m_bDrawText)
		//public override void OnPostPaint(System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Size size = mTargetControl.Size;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);

        
            if (OverlayObjectPainter != null)
            {
                OverlayObjectPainter(e);
            }
        }

        public override void OnOverlayPaint(System.Drawing.Graphics g, bool bDrawText)
        {
            System.Drawing.Size size = mTargetControl.Size;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);

            if (SelectRectOverlayPaint != null)
            {
                SelectRectOverlayPaint(e);
            }

            if (OverlayObjectPainter != null)
            {
                OverlayObjectPainter(e);
            }
        }

		// OnPrintPage() 호출되기 직전에 호출된다.
        public override void OnPrevPrint(System.Drawing.Graphics g)		
		{			
		}

		// OnPrintPage() 호출된 직후에 호출된다.
        public override void OnPostPrint(System.Drawing.Graphics g)
		//public override void OnPostPrint(System.Windows.Forms.PaintEventArgs e)
		{
            System.Drawing.Size size = mTargetControl.Size;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);

            if (OverlayObjectPainter != null)
			{
                OverlayObjectPainter(e);
			}
		}
       
        public void Clear()
        {
        }
        
        public void ClearSelection()
        {
            if (m_selectedShape != null)
            {
                m_selectedShape.Visible = false;
                m_selectedShape = null;
            }
        }
    }
}
