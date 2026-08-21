using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVSingleViewer.Command
{
    public class CommandOne : Command
    {
        private CCTVPanel m_panel = null;
        private CCTV m_cctvPrev = null;
        private CCTV m_cctvNext = null;

        public CommandOne(CCTVPanel panel, CCTV prev, CCTV next)
        {
            m_panel = panel;
            m_cctvPrev = prev;
            m_cctvNext = next;
        }

        public override void RollBack()
        {
            if (m_panel != null)
                m_panel.Connect(m_cctvPrev);
        }

        public override void Do()
        {
            if (m_panel != null)
                m_panel.Connect(m_cctvNext);
        }
    }
}
