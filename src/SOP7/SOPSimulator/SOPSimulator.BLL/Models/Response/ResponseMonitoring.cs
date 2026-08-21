using SOPSimulator.BLL.Models.Data;
using System.Collections.Generic;

namespace SOPSimulator.BLL.Models.Response
{
    public class ResponseMonitoring
    {
        private bool m_bChanged = true;
        public bool Changed
        {
            get { return m_bChanged; }
            set { m_bChanged = value; }
        }

        private int m_nChanged = 1;
        public int nChanged
        {
            get { return m_nChanged; }
            set { m_nChanged = value; }
        }

        private int m_nLastAccessActionStepHistoryID = -1;
        public int LastAccessActionStepHistoryID
        {
            get { return m_nLastAccessActionStepHistoryID; }
            set { m_nLastAccessActionStepHistoryID = value; }
        }

        private List<SOPRunData> m_sopRunDatas = null;
        public List<SOPRunData> SOPRunDatas
        {
            get { return m_sopRunDatas; }
            set { m_sopRunDatas = value; }
        }

        private List<int> m_confirmTimeoutCloseSOPs = new List<int>();
        public List<int> ConfirmTimeoutCloseSOPs
        {
            get { return m_confirmTimeoutCloseSOPs; }
            set { m_confirmTimeoutCloseSOPs = value; }
        }
    }
}
