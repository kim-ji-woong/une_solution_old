using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Command
{
    public class CommandAddOverlay : UnE.Command.Command
    {
        private Overlay.OverlayShape m_shape = null;
        private Overlay.OverlayPainter m_painter = null;
        private DXFViewer.DXFControl m_ctrl = null;

        public Overlay.OverlayShape OverlayShape
        {
            get { return m_shape; }
            set { m_shape = value; }
        }

        public CommandAddOverlay(DXFViewer.DXFControl ctrl, Overlay.OverlayPainter painter, Overlay.OverlayShape shape)
        {
            m_shape = shape;
            m_painter = painter;
            m_ctrl = ctrl;
        }

        public override void RollBack()
        {
            if (m_shape == null)
                return;

            m_painter.RemoveOverlayShape(m_shape);
            m_ctrl.Refresh();
        }

        public override void Do()
        {
            if (m_shape == null)
                return;

            m_painter.AddOverlayShape(m_shape);
            m_ctrl.Refresh();
        }
    }
}
