using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.GUI;

namespace CCTVSingleViewer.Command
{
    public class RibbonButtonGroup
    {
        private List<RibbonButton> m_buttons = new List<RibbonButton>();
        private System.EventHandler m_handler = null;
        private bool m_isEnabled = false;

        public bool Enabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;

                foreach (RibbonButton btn in m_buttons)
                {
                    btn.Enabled = m_isEnabled;
                }
            }
        }

        public RibbonButtonGroup(System.EventHandler handler)
        {
            m_handler = handler;
        }

        public void AddButton(RibbonButton btn)
        {
            m_buttons.Add(btn);

            if (btn != null)
            {
                btn.Enabled = m_isEnabled;
                btn.Click += m_handler;
            }
        }        
    }
}
