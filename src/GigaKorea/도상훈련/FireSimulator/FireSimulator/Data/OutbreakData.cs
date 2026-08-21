using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSimulator.Data
{
    public class OutbreakData
    {
        private int m_nActionStepHistoryID = -1;
        private int m_nProcessID = -1;
        private string m_strText = "";

        public int ActionStepHistoryID 
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ProcessID
        {
            get { return m_nProcessID; }
            set { m_nProcessID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }


    }

    public class ProcessData
    {
        private int m_nID = -1;
        private string m_strText = "";
        private bool m_bFirst = false;
        private bool m_bChild = false;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool First
        {
            get { return m_bFirst; }
            set { m_bFirst = value; }
        }

        public bool Child
        {
            get { return m_bChild; }
            set { m_bChild = value; }
        }
    }
}
