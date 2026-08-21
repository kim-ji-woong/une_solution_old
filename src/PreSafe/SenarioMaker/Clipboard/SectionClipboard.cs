using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace UnE.SenarioMaker
{
    public class SectionClipboardEx : Sections.SectionClipboard
    {
        protected static SectionClipboardEx m_Instance = null;
        public static SectionClipboardEx Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SectionClipboardEx();
                return m_Instance;
            }
        }

        protected SectionClipboardEx()
            : base()
        {
        }

        public override void Copy(PanelSection panel)
        {
            base.Copy(panel);
        }

        public override void Cut(PanelSection panel)
        {
            base.Cut(panel);
        }

        public override void Paste(PanelSection panel)
        {
            if(m_editSection.Count > 0)
            {
                UndoRedoManager.Instance.SaveSnapshot("붙여넣기");
                base.Paste(panel);
            }
            
        }

        public override void Canel()
        {
            if (m_CurrentAction == Action.Copy)
            {

            }
            else if (m_CurrentAction == Action.Cut)
            {
                //UndoRedoManager.Instance.Undo();
				FormMain.Instance.Undo();
            }
            m_OrgPanel = null;
            m_editSection.Clear();
            m_CurrentAction = Action.None;
        }
    }

}
