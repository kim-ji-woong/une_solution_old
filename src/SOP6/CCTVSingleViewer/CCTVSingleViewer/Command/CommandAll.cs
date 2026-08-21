using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVSingleViewer.Command
{
    public class CommandAll : Command
    {
        private CCTVPanel[] m_panels = null;
        private CCTV[] m_prevCCTVs = null;
        private CCTV[] m_nextCCTVs = null;

        public CommandAll(CCTVPanel[] panels, CCTV[] prevCCTVs, CCTV[] nextCCTVs)
        {
            m_panels = panels;
            m_prevCCTVs = prevCCTVs;
            m_nextCCTVs = nextCCTVs;
        }

        public override void RollBack()
        {
            if (m_panels == null)
                return;

            if (m_prevCCTVs == null)
            {
                foreach (CCTVPanel panel in m_panels)
                {
                    panel.Connect(null);
                }
            }
            else
            {
                int nPanelCount = m_panels.Count();

                for (int i = 0; i < nPanelCount; i++)
                {
                    m_panels[i].Connect(m_prevCCTVs[i]);
                }
            }
        }

        public override void Do()
        {
            if (m_panels == null)
                return;

            if (m_nextCCTVs == null)
            {
                foreach (CCTVPanel panel in m_panels)
                {
                    panel.Connect(null);
                }
            }
            else
            {
                int nPanelCount = m_panels.Count();

                for (int i = 0; i < nPanelCount; i++)
                {
                    m_panels[i].Connect(m_nextCCTVs[i]);
                }
            }
        }
    }
}
