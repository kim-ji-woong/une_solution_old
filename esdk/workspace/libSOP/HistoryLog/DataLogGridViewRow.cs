using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sections;

using UnE.SOP;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;

namespace UnE.SOP.Log
{
    public class DataLogGridViewRow : DataGridViewRow
    {
        private int m_nActionStepID = -1;
        private int m_nComponentID = -1;
        private bool m_isRealMode = true;
        private Section.ComponentType m_componentType = Section.ComponentType.NONE;
        private int m_nActionStepHistoryID = -1;
        private int m_nComponentHistoryID = -1;
        private Section m_section = null;
        private Workstate.State m_sectionState = Workstate.State.NORMAL;
        private bool m_noDBWrite = false;

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        public bool IsRealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public Section.ComponentType ComponentType
        {
            get { return m_componentType; }
            set { m_componentType = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public Workstate.State SectionState
        {
            get { return m_sectionState; }
            set { m_sectionState = value; }
        }

        public bool NoDBWrite
        {
            get { return m_noDBWrite; }
            set { m_noDBWrite = value; }
        }
    }
}
