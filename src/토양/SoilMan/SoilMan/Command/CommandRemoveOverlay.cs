using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Command
{
    public class CommandRemoveOverlay : UnE.Command.Command
    {
        private Overlay.OverlayShape m_shape = null;
        private Overlay.OverlayPainter m_painter = null;
        private DXFViewer.DXFControl m_ctrl = null;
        private List<Overlay.OverlayShape> mShapeList = null;
        private Overlay.OverlayShape m_Addedshape = null;

        public Overlay.OverlayShape OverlayShape
        {
            get { return m_shape; }
            set { m_shape = value; }
        }

        public CommandRemoveOverlay(DXFViewer.DXFControl ctrl, Overlay.OverlayPainter painter, Overlay.OverlayShape shape)
        {
            m_shape = shape;
            m_painter = painter;
            m_ctrl = ctrl;
        }

        public CommandRemoveOverlay(DXFViewer.DXFControl ctrl, Overlay.OverlayPainter painter, Overlay.OverlayShape addshape, List<Overlay.OverlayShape> shapeList)
        {
            m_shape = null;
            mShapeList = new List<Overlay.OverlayShape>(shapeList);
            m_painter = painter;
            m_ctrl = ctrl;
            m_Addedshape = addshape;
        }

        public override void RollBack()
        {
            if (m_shape == null)
            {
                if( mShapeList != null)
                {
                    foreach (Overlay.OverlayShape shape in mShapeList)
                    {
                        m_painter.AddOverlayShape(shape);
                    }
                    m_painter.RemoveOverlayShape(m_Addedshape);
                    m_ctrl.Refresh();
                }
            }
            else
            {
                m_painter.AddOverlayShape(m_shape);
                m_ctrl.Refresh();
            }

           
        }

        public override void Do()
        {
            if (m_shape == null)
            {
                if (mShapeList != null)
                {
                    foreach (Overlay.OverlayShape shape in mShapeList)
                    {
                        m_painter.RemoveOverlayShape(shape);
                    }
                    m_painter.AddOverlayShape(m_Addedshape);
                    m_ctrl.Refresh();
                }
            }
            else
            {
                m_painter.RemoveOverlayShape(m_shape);
                m_ctrl.Refresh();
            }          
        }
    }
}
