using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPMonitoringSystem
{
    public class ProcessSectionEventArgsEx : UnE.SOP.Process.ProcessSectionEventArgs
    {
        public ProcessSectionEventArgsEx() : base()
        { 
        }

        private int m_nActionStepHistory = -1;
        public int ActionStepHistory
        {
            get { return m_nActionStepHistory; }
            set { m_nActionStepHistory = value; }
        }

        private int m_nActionStepID = -1;
        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        private bool m_bRealMode = false;
        public bool RealMode
        {
            get { return m_bRealMode; }
            set { m_bRealMode = value; }
        }
    }
}
